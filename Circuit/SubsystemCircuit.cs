using Engine;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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
		public LockFreeQueue<Request> Requests = new LockFreeQueue<Request>();
		protected float m_remainingSimulationTime;
		protected ElementBlock elementblock;
		public int UpdateStep;
		public bool UpdatePath;
		public HashSet<Device> Path;
		public Element[][] CircuitPath;
		public static Dictionary<Point3, Device> Table;
		public override int[] HandledBlocks => new int[] { ElementBlock.Index };
		public int UpdateOrder => 0;
		public override void Load(ValuesDictionary valuesDictionary)
		{
			CircuitPath = new Element[0][];
			base.Load(valuesDictionary);
			Utils.Load(Project);
			Path = new HashSet<Device>();
			Table = new Dictionary<Point3, Device>(valuesDictionary.GetValue("Count", 0));
			elementblock = BlocksManager.Blocks[ElementBlock.Index] as ElementBlock;
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
				Path.Add(device);
		}
		public override void OnBlockRemoved(int value, int newValue, int x, int y, int z)
		{
			var device = elementblock.GetDevice(x, y, z, value);
			if (device == null)
				return;
			UpdatePath = true;
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
					Path.Remove(device);
			}
			Table.Remove(device.Point);
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
					list.Add(i.Current);
			}
			for (startX = 0; startX < list.Count; startX++)
				Path.Remove(list[startX]);
		}
		public override void OnNeighborBlockChanged(int x, int y, int z, int neighborX, int neighborY, int neighborZ)
		{
			var device = elementblock.GetDevice(Utils.Terrain, x, y, z);
			if (device is IUnstableBlock behavior)
				behavior.OnNeighborBlockChanged(SubsystemTerrain, neighborX, neighborY, neighborZ);
		}
		public override bool OnInteract(TerrainRaycastResult raycastResult, ComponentMiner componentMiner)
		{
			var face = raycastResult.CellFace;
			return elementblock.GetDevice(face.X, face.Y, face.Z, raycastResult.Value) is IInteractiveBlock block && block.OnInteract(raycastResult, componentMiner);
		}
		public override void OnHitByProjectile(CellFace cellFace, WorldItem worldItem)
		{
			var item = elementblock.GetDevice(Utils.Terrain, cellFace.X, cellFace.Y, cellFace.Z);
			if (item is IItemAcceptableBlock block)
				block.OnHitByProjectile(cellFace, worldItem);
		}
		public override void OnItemHarvested(int x, int y, int z, int blockValue, ref BlockDropValue dropValue, ref int newBlockValue)
		{
			var item = elementblock.GetDevice(Utils.Terrain, x, y, z);
			if (item is IHarvestingItem block)
				block.OnItemHarvested(x, y, z, blockValue, ref dropValue, ref newBlockValue);
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
						int v = 0;
						CircuitPath[i][j].Simulate(ref v);
					}
				}
				CircuitPath = new Element[Path.Count][];
				i = 0;
				for (var enumerator = Path.GetEnumerator(); enumerator.MoveNext(); i++)
				{
					var v = enumerator.Current;
					var visited = new HashSet<Device>
					{
						v
					};
					var neighbors = new DynamicArray<Device>(6);
					var Q = new Queue<Device>();
					int count;
					Q.Enqueue(v);
					while (Q.Count > 0)
					{
						v = Q.Dequeue();
						neighbors.Clear();
						elementblock.GetAllConnectedNeighbors(Utils.Terrain, v, 4, neighbors);
						v.Next = new Element[neighbors.Count];
						count = 0;
						for (j = 0; j < neighbors.Count; j++)
						{
							var w = neighbors.Array[j];
							if (visited.Add(w))
							{
								v.Next[count++] = w;
								Q.Enqueue(w);
							}
						}
					}
					Element element = enumerator.Current;
					var stack = new DynamicArray<Element>(1)
					{
						element
					};
					var arr = new Element[visited.Count];
					count = 0;
					while (stack.Count > 0)
					{
						if ((element = stack.Array[--stack.Count]) != null)
						{
							arr[count++] = element;
							var next = element.Next;
							for (j = 0; j < next.Length && next[j] != null; j++)
								stack.Add(next[j]);
						}
					}
					CircuitPath[i] = new Element[count];
					Array.Copy(arr, CircuitPath[i], count);
				}
				UpdatePath = false;
			}
			m_remainingSimulationTime = MathUtils.Min(m_remainingSimulationTime + dt, 0.1f);
			while (m_remainingSimulationTime >= 0.02f)
			{
				UpdateStep++;
				m_remainingSimulationTime -= 0.02f;
				for (i = 0; i < CircuitPath.Length; i++)
					QueueSimulate(CircuitPath[i]);
			}
		}
		public override void Dispose()
		{
			Requests.Clear();
			Requests.Enqueue(null);
			Utils.LoadedProject = false;
		}
		public void ThreadFunction()
		{
			while (true)
			{
				Request request = null;
				while (Requests.Count == 0)
					Task.Delay(10).Wait();
				request = Requests.Dequeue();
				if (request == null)
					return;
				if (UpdatePath)
					Requests.Clear();
				else
				{
					//request.IsInProgress = false;
					//request.IsCompleted = true;
					var elements = request.Elements;
					int voltage = 0;
					for (int i = 0; i < elements.Length; i++)
						elements[i].Simulate(ref voltage);
				}
				Task.Delay(10).Wait();
			}
		}
		public void QueueSimulate(Element[] elements)
		{
			Requests.Enqueue(new Request
			{
				//IsCompleted = false,
				//IsInProgress = true,
				Elements = elements
			});
		}
		/*public static void QuickSort(INode[] R, int Low, int High, int voltage = 0)
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
						high--;
					if (high > low)
					{
						R[low] = R[high];
						R[high] = tmp;
					}
					while (low < high && r >= R[low].GetWeight(voltage))
						low++;
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