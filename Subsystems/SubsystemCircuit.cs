using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Engine;
using TemplatesDatabase;

namespace Game
{
	public class SubsystemCircuit : SubsystemBlockBehavior, IUpdateable
	{
		public class Request
		{
			//public volatile bool IsCompleted;
			//public bool IsInProgress;
			public Element Element;
		}
		public Queue<Request> Requests = new Queue<Request>();
		public SubsystemTime SubsystemTime;
		//public SubsystemAudio SubsystemAudio;
		protected float m_remainingSimulationTime;
		protected ElementBlock elementblock;
		public int UpdateStep;
		public Terrain Terrain;
		public ICollection<Device> Path;
		public static Dictionary<Point3, Device> Table;
		public override int[] HandledBlocks => new int[] { ElementBlock.Index };
		public int UpdateOrder => 0;
		public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			Terrain = SubsystemTerrain.Terrain;
			int count = valuesDictionary.GetValue<int>("Count", 0);
			Path = new HashSet<Device>();
			Table = new Dictionary<Point3, Device>(count);
			SubsystemTime = Project.FindSubsystem<SubsystemTime>(true);
			elementblock = BlocksManager.Blocks[ElementBlock.Index] as ElementBlock;
			//SubsystemAudio = Project.FindSubsystem<SubsystemAudio>(true);
			Task.Run((Action)ThreadFunction);
		}
		public override void OnBlockGenerated(int value, int x, int y, int z, bool isLoaded)
		{
			if (isLoaded)
				OnBlockAdded(value, -1, x, y, z);
		}
		public override void OnBlockAdded(int value, int oldValue, int x, int y, int z)
		{
			var device = elementblock.GetDevice(Terrain, x, y, z);
			if (device == null)
				return;
			if (device is IBlockBehavior behavior)
				behavior.OnBlockAdded(SubsystemTerrain, value, oldValue);
			//if (oldValue == -1 && (device.Type & ElementType.Supply) == 0)
				//return;
			var v = device;
			var visited = new HashSet<Element>();
			var neighbors = new DynamicArray<Device>();
			visited.Add(v);
			var Q = new Queue<Device>();
			Q.Enqueue(v);
			while (Q.Count > 0)
			{
				v = Q.Dequeue();
				neighbors.Clear();
				elementblock.GetAllConnectedNeighbors(Terrain, v, 4, neighbors);
				v.Next = new DynamicArray<Element>(neighbors.Count);
				for (int i = 0; i < neighbors.Count; i++)
				{
					var w = neighbors.Array[i];
					if (visited.Add(w))
					{
						v.Next.Add(w);
						Q.Enqueue(w);
					}
				}
			}
			Table[device.Point] = device;
			if ((device.Type & ElementType.Supply) != 0)
				Path.Add(device);
		}
		public override void OnBlockRemoved(int value, int newValue, int x, int y, int z)
		{
			if (elementblock.GetItem(ref value) is Device device)
			{
				device.Point = new Point3(x, y, z);
				if (device is IBlockBehavior behavior)
					behavior.OnBlockRemoved(SubsystemTerrain, value, newValue);
				if (device.Next != null)
				{
					var next = device.Next;
					for (int i = 0; i < next.Count; i++)
					{
						var element = next.Array[i];
						QueueSimulate(element);
						element.Next.Remove(device);
					}
					if ((device.Type & ElementType.Supply) != 0)
					{
						Path.Remove(device);
					}
				}
				Table.Remove(device.Point);
			}
		}
		public override void OnBlockModified(int value, int oldValue, int x, int y, int z)
		{
			var device = elementblock.GetDevice(Terrain, x, y, z);
			if (device != null)
			{
				OnBlockRemoved(oldValue, value, x, y, z);
				OnBlockAdded(value, oldValue, x, y, z);
			}
		}
		public override void OnChunkDiscarding(TerrainChunk chunk)
		{
			int startX = chunk.Origin.X;
			int startY = chunk.Origin.Y;
			var list = new List<Device>();
			for (var i = Path.GetEnumerator(); i.MoveNext();)
			{
				var key = i.Current.Point;
				if (key.X >= startX && key.X < startX + 16 && key.Z >= startY && key.Z < startY + 16)
				{
					list.Add(i.Current);
				}
			}
			for (startX = 0; startX < list.Count; startX++)
			{
				Path.Remove(list[startX]);
			}
		}
		public override void OnNeighborBlockChanged(int x, int y, int z, int neighborX, int neighborY, int neighborZ)
		{
			var device = elementblock.GetDevice(Terrain, x, y, z);
			if (device != null && device is IUnstableBlock behavior)
			{
				behavior.OnNeighborBlockChanged(SubsystemTerrain, neighborX, neighborY, neighborZ);
			}
		}
		public override bool OnInteract(TerrainRaycastResult raycastResult, ComponentMiner componentMiner)
		{
			var item = elementblock.GetItem(ref raycastResult.Value);
			return item != null && item is IInteractiveBlock block && block.OnInteract(raycastResult, componentMiner);
		}
		public void Update(float dt)
		{
			m_remainingSimulationTime = MathUtils.Min(m_remainingSimulationTime + dt, 0.1f);
			while (m_remainingSimulationTime >= 0.02f)
			{
				UpdateStep++;
				m_remainingSimulationTime -= 0.02f;
				for (var enumerator = Path.GetEnumerator(); enumerator.MoveNext();)
				{
					QueueSimulate(enumerator.Current);
				}
			}
		}
		public override void Dispose()
		{
			lock (Requests)
			{
				Requests.Clear();
				Requests.Enqueue(null);
				Monitor.Pulse(Requests);
			}
		}
		public void ThreadFunction()
		{
			while (true)
			{
				Request request = null;
				lock (Requests)
				{
					while (Requests.Count == 0)
					{
						Monitor.Wait(Requests);
					}
					request = Requests.Dequeue();
				}
				if (request == null)
				{
					return;
				}
				//request.IsInProgress = false;
				//request.IsCompleted = true;
				var element = request.Element;
				int voltage = 0;
				var stack = new DynamicArray<Element>(1)
				{
					element
				};
				while (stack.Count > 0)
				{
					if (stack.Count > 20000)
					{
						throw new InvalidOperationException("Stack overflow");
					}
					if ((element = stack.Array[--stack.Count]) != null)
					{
						element.Simulate(ref voltage);
						if (voltage != 0)
						{
							var array = element.Next.Array;
							for (int j = 0; j < array.Length; j++)
								stack.Add(array[j]);
						}
					}
				}
				Task.Delay(10).Wait();
			}
		}
		public void QueueSimulate(Element element)
		{
			lock (Requests)
			{
				Requests.Enqueue(new Request
				{
					//IsCompleted = false,
					//IsInProgress = true,
					Element = element
				});
				Monitor.Pulse(Requests);
			}
		}
		/*public static void QuickSort(Node[] R, int Low, int High, int voltage = 0)
		{
			int low = 2, high = High - Low;
			while ((high >>= 1) > 0)
				low += 2;
			var stack = new int[low];
			stack[0] = Low;
			stack[1] = High;
			int count = 2;
			var random = new Random(R.Length - Low + High);
			while (count > 0)
			{
				low = Low = stack[count - 1];
				high = High = stack[count -= 2];
				var tmp = R[low];
				int r = R[random.UniformInt(low, high)].GetWeight(voltage);
				while (high > low)
				{
					while (low < high && r <= R[high].GetWeight(voltage))
					{
						high--;
					}
					if (high > low)
					{
						R[low] = R[high];
						R[high] = tmp;
					}
					while (low < high && r >= R[low].GetWeight(voltage))
					{
						low++;
					}
					if (high > low)
					{
						R[high] = R[low];
						R[low] = tmp;
					}
					if (low == high)
					{
						if (Low < low - 1)
						{
							stack[count + 1] = Low;
							stack[count += 2] = low - 1;
						}
						if (High > low + 1)
						{
							stack[count + 1] = low + 1;
							stack[count += 2] = High;
						}
					}
				}
			}
		}*/
	}
}