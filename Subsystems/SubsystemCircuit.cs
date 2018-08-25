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
			public volatile bool IsCompleted;
			public bool IsInProgress;
			public Element Element;
		}
		public Queue<Request> Requests = new Queue<Request>();
		public SubsystemTime SubsystemTime;
		//public SubsystemAudio SubsystemAudio;
		protected float m_remainingSimulationTime;
		protected ElementBlock elementblock;
		public int UpdateStep;
		public Terrain Terrain;
		public DynamicArray<Element> Path;
		public Dictionary<Point3, Element> Table;
		public override int[] HandledBlocks => new int[] { ElementBlock.Index };
		public int UpdateOrder => 0;
		public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			Terrain = SubsystemTerrain.Terrain;
			int count = valuesDictionary.GetValue<int>("Count", 0);
			Path = new DynamicArray<Element>(count);
			Table = new Dictionary<Point3, Element>(count);
			SubsystemTime = Project.FindSubsystem<SubsystemTime>(true);
			elementblock = BlocksManager.Blocks[ElementBlock.Index] as ElementBlock;
			//SubsystemAudio = Project.FindSubsystem<SubsystemAudio>(true);
			Task.Run((Action)ThreadFunction);
		}
		public override void OnBlockGenerated(int value, int x, int y, int z, bool isLoaded)
		{
			if (isLoaded)
				OnBlockAdded(value, 0, x, y, z);
		}
		public override void OnBlockAdded(int value, int oldValue, int x, int y, int z)
		{
			var device = elementblock.GetDevice(Terrain, x, y, z);
			if (device == null)
				return;
			device.OnBlockAdded(value, oldValue, x, y, z);
			if ((device.Type & ElementType.Supply) == 0 || Table.ContainsKey(device.Point))
				return;
			var neighbors = new DynamicArray<Device>();//当前顶点的邻接表
			elementblock.GetAllConnectedNeighbors(Terrain, device, 5, neighbors);
			if (neighbors.Count < 2)
				return;
			Table.Add(device.Point, device);
			var stack = new DynamicArray<Device>(1);
			stack.Push(neighbors.Array[0]); // start
			var end = neighbors.Array[1];
			//非递归BFS
			while (stack.Count > 0)
			{
				var current = stack.Array[stack.Count - 1];//当前顶点
				if (current == end)//如果电流能回到负极
					Path.Push(device);
				else
				{
					neighbors.Clear();
					elementblock.GetAllConnectedNeighbors(Terrain, current, 5, neighbors);
					if (neighbors.Count > 0)
					{
						current.Next = new DynamicArray<Element>(neighbors.Count);
						QuickSort(neighbors.Array, 0, neighbors.Count - 1);
						bool isWire = false;
						for (int i = 0; i < neighbors.Count; i++)
						{
							var cur = neighbors.Array[i];
							if (Table.TryGetValue(cur.Point, out Element visited))//如果访问过
							{
								//current.Next.Add(visited);
								continue;
							}
							Table.Add(cur.Point, cur);//将该点添加到表中
							if (cur.GetWeight() < 2)//判断是否为导线
								isWire = true;
							else if (isWire)
								break;//元件被短接
							current.Next.Add(cur);
							stack.Push(cur);//将该点添加到访问栈中
						}
					}
				}
				stack.RemoveAtEnd();
			}
		}
		public override void OnBlockRemoved(int value, int newValue, int x, int y, int z)
		{
			var element = elementblock.GetDevice(Terrain, x, y, z);
			if (element != null)
			{
				var next = element.Next.Array;
				for (int i = 0; i < next.Length; i++)
				{
					next[i].Next.Remove(element);
				}
				if ((element.Type & ElementType.Supply) != 0)
				{
					int index = Path.IndexOf(element);
					if (index >= 0)
					{
						Path.Array[index] = null;
					}
				}
				Table.Remove(element.Point);
			}
		}
		public override void OnBlockModified(int value, int oldValue, int x, int y, int z)
		{
			var element = elementblock.GetDevice(Terrain, x, y, z);
			if (element != null)
			{
				OnBlockAdded(value, oldValue, x, y, z);
				OnBlockRemoved(oldValue, value, x, y, z);
			}
		}
		public override void OnChunkDiscarding(TerrainChunk chunk)
		{
			int startX = chunk.Origin.X;
			int startY = chunk.Origin.Y;
			for (var i = Table.GetEnumerator(); i.MoveNext();)
			{
				var key = i.Current.Key;
				if (key.X >= startX && key.X < startX + 16 && key.Z >= startY && key.Z < startY + 16)
				{
					int index = Path.IndexOf(i.Current.Value);
					if (index >= 0)
					{
						Path.Array[index] = null;
					}
				}
			}
		}
		public void Update(float dt)
		{
			m_remainingSimulationTime = MathUtils.Min(m_remainingSimulationTime + dt, 0.1f);
			while (m_remainingSimulationTime >= 0.02f)
			{
				UpdateStep++;
				m_remainingSimulationTime -= 0.02f;
				for (int i = 0; i < Path.Count; i++)
				{
					var element = Path.Array[i];
					if (element != null)
						QueueSimulate(element);
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
					break;
				}
				request.IsInProgress = false;
				request.IsCompleted = true;
				var element = request.Element;
				int voltage = 0;
				var stack = new DynamicArray<Element>(1)
				{
					element
				};
				while (stack.Count > 0)
				{
					if (stack.Count > 10000)
					{
						throw new InvalidOperationException("Stack overflow");
					}
					if ((element = stack.Array[--stack.Count]) != null)
					{
						element.Simulate(ref voltage);
						if (voltage != 0 && element.Next != null)
						{
							var array = element.Next.Array;
							for (int j = 0; j < array.Length; j++)
								stack.Add(array[j]);
						}
					}
				}
				Task.Delay(20).Wait();
			}
		}
		public void QueueSimulate(Element element)
		{
			lock (Requests)
			{
				Requests.Enqueue(new Request
				{
					IsCompleted = false,
					IsInProgress = true,
					Element = element
				});
				Monitor.Pulse(Requests);
			}
		}
		public static void QuickSort(Node[] R, int Low, int High, int voltage = 0)
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
		}
		public void GarbageCollectItems()
		{
			var path = new DynamicArray<Element>(Path.Count >> 1);
			var arr = Path.Array;
			for (int i = 0; i < arr.Length; i++)
				if (arr[i] != null)
					path.Add(arr[i]);
			Path = path;
		}
	}
}