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
			public Element[] Elements;
		}
		public Queue<Request> Requests = new Queue<Request>();
		public SubsystemTime SubsystemTime;
		//public SubsystemAudio SubsystemAudio;
		protected float m_remainingSimulationTime;
		protected ElementBlock elementblock;
		public int UpdateStep;
		public bool UpdatePath;
		public Terrain Terrain;
		public HashSet<Device> Path;
		public Device[][] CircuitPath;
		public static Dictionary<Point3, Device> Table;
		public override int[] HandledBlocks => new int[] { ElementBlock.Index };
		public int UpdateOrder => 0;
		public override void Load(ValuesDictionary valuesDictionary)
		{
			CircuitPath = new Device[0][];
			base.Load(valuesDictionary);
			((WireDevice)ElementBlock.Devices[5]).Terrain = Terrain = SubsystemTerrain.Terrain;
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
			var device = elementblock.GetDevice(x, y, z, value);
			if (device == null)
				return;
			if (device is IBlockBehavior behavior)
				behavior.OnBlockAdded(SubsystemTerrain, value, oldValue);
			Table[device.Point] = device;
			UpdatePath = true;
			if ((device.Type & ElementType.Supply) == 0)
			{
				if (oldValue == -1)
					return;
			}
			else
			{
				Path.Add(device);
			}
		}
		public override void OnBlockRemoved(int value, int newValue, int x, int y, int z)
		{
			var device = elementblock.GetDevice(x, y, z, value);
			if (device == null)
				return;
			if (device is IBlockBehavior behavior)
				behavior.OnBlockRemoved(SubsystemTerrain, value, newValue);
			if (device.Next != null)
			{
				/*var next = device.Next;
				for (int i = 0; i < next.Count; i++)
				{
					var element = next.Array[i];
					element.Next.Remove(device);
				}*/
				if ((device.Type & ElementType.Supply) != 0)
				{
					Path.Remove(device);
				}
			}
			Table.Remove(device.Point);
			UpdatePath = true;
		}
		public override void OnBlockModified(int value, int oldValue, int x, int y, int z)
		{
			OnBlockRemoved(oldValue, value, x, y, z);
			OnBlockAdded(value, oldValue, x, y, z);
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
			var face = raycastResult.CellFace;
			return elementblock.GetDevice(face.X, face.Y, face.Z, raycastResult.Value) is IInteractiveBlock block && block.OnInteract(raycastResult, componentMiner);
		}
		public override void OnHitByProjectile(CellFace cellFace, WorldItem worldItem)
		{
			var item = elementblock.GetDevice(Terrain, cellFace.X, cellFace.Y, cellFace.Z);
			if (item != null && item is IItemAcceptableBlock block)
				block.OnHitByProjectile(cellFace, worldItem);
		}
		public void Update(float dt)
		{
			int i, j;
			if (UpdatePath)
			{
				for (i = 0; i < CircuitPath.Length; i++)
				{
					int length = CircuitPath[i].Length;
					for (j = 1; j < length; j++)
					{
						CircuitPath[i][j].UpdateState();
					}
				}
				CircuitPath = new Device[Path.Count][];
				i = 0;
				for (var enumerator = Path.GetEnumerator(); enumerator.MoveNext(); i++)
				{
					var v = enumerator.Current;
					var visited = new HashSet<Element>();
					var neighbors = new DynamicArray<Device>(6);
					visited.Add(v);
					var Q = new Queue<Device>();
					Q.Enqueue(v);
					while (Q.Count > 0)
					{
						v = Q.Dequeue();
						neighbors.Clear();
						elementblock.GetAllConnectedNeighbors(Terrain, v, 4, neighbors);
						v.Next = new DynamicArray<Element>(neighbors.Count);
						for (j = 0; j < neighbors.Count; j++)
						{
							var w = neighbors.Array[j];
							if (visited.Add(w))
							{
								v.Next.Add(w);
								Q.Enqueue(w);
							}
						}
					}
					Element element = enumerator.Current;
					var stack = new DynamicArray<Element>(1)
					{
						element
					};
					var arr = new DynamicArray<Element>(1);
					while (stack.Count > 0)
					{
						if (stack.Count > 20000)
						{
							stack.Clear();
							throw new InvalidOperationException("Stack overflow");
						}
						if ((element = stack.Array[--stack.Count]) != null)
						{
							arr.Add(element);
							//if (voltage != 0)
							//{
							var next = element.Next;
							for (j = 0; j < next.Count; j++)
								stack.Add(next.Array[j]);
							//}
						}
					}
					CircuitPath[i] = new Device[arr.Count];
					arr.CopyTo(CircuitPath[i], 0);
				}
				UpdatePath = false;
			}
			m_remainingSimulationTime = MathUtils.Min(m_remainingSimulationTime + dt, 0.1f);
			while (m_remainingSimulationTime >= 0.02f)
			{
				UpdateStep++;
				m_remainingSimulationTime -= 0.02f;
				for (i = 0; i < CircuitPath.Length; i++)
				{
					QueueSimulate(CircuitPath[i]);
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
				if (UpdatePath)
				{
					Requests.Clear();
				}
				else
				{
					//request.IsInProgress = false;
					//request.IsCompleted = true;
					var elements = request.Elements;
					int voltage = 0;
					for (int i = 0; i < elements.Length; i++)
					{
						elements[i].Simulate(ref voltage);
					}
				}
				Task.Delay(10).Wait();
			}
		}
		public void QueueSimulate(Element[] elements)
		{
			lock (Requests)
			{
				Requests.Enqueue(new Request
				{
					//IsCompleted = false,
					//IsInProgress = true,
					Elements = elements
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