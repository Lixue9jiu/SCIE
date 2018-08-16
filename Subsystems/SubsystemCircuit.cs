using System;
using System.Collections.Generic;
using Engine;
using TemplatesDatabase;

namespace Game
{
	public static class StackUtils
	{
		public static void Push<T>(this DynamicArray<T> array, T item)
		{
			if (array.m_count >= array.Capacity)
			{
				int value = MathUtils.Max(array.Capacity << 1, 4);
				if (value != array.Capacity)
				{
					T[] arr = new T[value];
					if (array.Array != null)
					{
						Array.Copy(array.Array, 0, arr, 0, array.m_count);
					}
					array.Array = arr;
				}
			}
			array.Array[array.m_count++] = item;
		}
	}
	public class SubsystemCircuit : SubsystemBlockBehavior, IUpdateable
	{
		public SubsystemTime SubsystemTime;
		//public SubsystemAudio SubsystemAudio;
		float m_remainingSimulationTime;
		public int UpdateStep;
		public Terrain Terrain;
		public DynamicArray<Element> Path;
		public Dictionary<Point3, Element> Table;
		public static readonly ElectricConnectionPath[] PathTable =
		{
			new ElectricConnectionPath(0, 0, 1, 5, 5, 5),
			new ElectricConnectionPath(1, 0, 0, 5, 5, 5),
			new ElectricConnectionPath(0, 0, -1, 5, 5, 5),
			new ElectricConnectionPath(-1, 0, 0, 5, 5, 5),
			new ElectricConnectionPath(0, 1, 0, 5, 5, 5),
			new ElectricConnectionPath(0, -1, 0, 5, 5, 5)
		};
		public override int[] HandledBlocks => new int[] { ElementBlock.Index };
		public int UpdateOrder
		{
			get { return 0; }
		}
		public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			Terrain = SubsystemTerrain.Terrain;
			int count = valuesDictionary.GetValue<int>("Count", 0);
			Path = new DynamicArray<Element>(count);
			Table = new Dictionary<Point3, Element>(count);
			SubsystemTime = Project.FindSubsystem<SubsystemTime>(true);
			//SubsystemAudio = Project.FindSubsystem<SubsystemAudio>(true);
		}
		public override void OnBlockGenerated(int value, int x, int y, int z, bool isLoaded)
		{
			if (isLoaded)
				OnBlockAdded(value, 0, x, y, z);
		}
		public override void OnBlockAdded(int value, int oldValue, int x, int y, int z)
		{
			var element = GetDevice(x, y, z);
			if (element == null || (element.Type & ElementType.Supply) == 0 || Table.ContainsKey(element.Point))
				return;
			var neighbors = new DynamicArray<Device>();//当前顶点的邻接表
			GetAllConnectedNeighbors(element, 5, neighbors);
			if (neighbors.Count < 2)
				return;
			Table.Add(element.Point, element);
			var stack = new DynamicArray<Device>(1);
			stack.Push(neighbors.Array[0]); // start
			var end = neighbors.Array[1];
			while (stack.Count > 0)
			{
				var current = stack.Array[stack.Count - 1];//当前顶点
				if (current == end)
				{
					Path.Push(element);
					//current.Next = null;
				}
				else
				{
					neighbors.Clear();
					GetAllConnectedNeighbors(current, 5, neighbors);
					if (neighbors.Count > 0)
					{
						current.Next = new DynamicArray<Element>(neighbors.Count);
						QuickSort(neighbors.Array, 0, neighbors.Count - 1);
						bool flag = false;
						for (int i = 0; i < neighbors.Count; i++)
						{
							var cur = neighbors.Array[i];
							if (Table.TryGetValue(cur.Point, out Element visited))//如果访问过
							{
								current.Next.Add(visited);
								continue;
							}
							Table.Add(cur.Point, cur);//将该点添加到表中
							if (cur.GetWeight() < 2)
								flag = true;
							else if (flag)
								break;
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
			var element = GetDevice(x, y, z);
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
			var element = GetDevice(x, y, z);
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
				if (key.X >= startX && key.X < chunk.Origin.X + 16 && key.Z >= startY && key.Z < startY + 16)
				{
					int index = Path.IndexOf(i.Current.Value);
					if (index >= 0)
					{
						Path.Array[index] = null;
					}
				}
			}
		}
		public static Element GetElement(int value)
		{
			if (Terrain.ExtractContents(value) == ElementBlock.Index)
			{
				switch (Terrain.ExtractData(value))
				{
					//case 1: return new SmallGenerator();
					//case 2: return new WireElement();
					//case 3: return new ElectricFurnace();
					case 0: return new Fridge();
					case 10: return new DiodeDevice();
					case 12: return new Battery12V();
				}
			}
			return null;
		}
		public virtual Device GetDevice(int x, int y, int z)
		{
			if (GetElement(Terrain.GetCellValueFast(x, y, z)) is Device device)
			{
				device.Point = new Point3(x, y, z);
				return device;
			}
			return null;
		}
		public static Device GetDevice(Terrain terrain, int x, int y, int z)
		{
			if (GetElement(terrain.GetCellValueFast(x, y, z)) is Device device)
			{
				device.Point = new Point3(x, y, z);
				return device;
			}
			return null;
		}
		public void Update(float dt)
		{
			m_remainingSimulationTime = MathUtils.Min(m_remainingSimulationTime + dt, 0.1f);
			while (m_remainingSimulationTime >= 0.02f)
			{
				UpdateStep++;
				m_remainingSimulationTime -= 0.02f;
				var arr = Path.Array;
				for (int i = 0; i < Path.Count; i++)
				{
					var element = arr[i];
					if (element == null)
						continue;
					int voltage = 0;
					var stack = new DynamicArray<Element>(1)
					{
						element
					};
					while (stack.Count > 0)
					{
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
				}
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
		public void GetAllConnectedNeighbors(Device elem, int mountingFace, ICollection<ElectricConnectionPath> list)
		{
			if (mountingFace != 5 || elem == null) return;
			int x, y, z;
			var type = elem.Type;
			var point = elem.Point;
			x = point.X;
			y = point.Y;
			z = point.Z;
			if ((elem = GetDevice(x, y, z + 1)) != null && (elem.Type & type) != 0)
			{
				list.Add(PathTable[0]);
			}
			if ((elem = GetDevice(x + 1, y, z)) != null && (elem.Type & type) != 0)
			{
				list.Add(PathTable[1]);
			}
			if ((elem = GetDevice(x, y, z - 1)) != null && (elem.Type & type) != 0)
			{
				list.Add(PathTable[2]);
			}
			if ((elem = GetDevice(x - 1, y, z)) != null && (elem.Type & type) != 0)
			{
				list.Add(PathTable[3]);
			}
			if ((elem = GetDevice(x, y + 1, z)) != null && (elem.Type & type) != 0)
			{
				list.Add(PathTable[4]);
			}
			if ((elem = GetDevice(x, y - 1, z)) != null && (elem.Type & type) != 0)
			{
				list.Add(PathTable[5]);
			}
		}
		public void GetAllConnectedNeighbors(Device elem, int mountingFace, ICollection<Device> list)
		{
			if (mountingFace != 5 || elem == null) return;
			int x, y, z;
			var type = elem.Type;
			var point = elem.Point;
			x = point.X;
			y = point.Y;
			z = point.Z;
			if ((elem = GetDevice(x, y, z + 1)) != null && (elem.Type & type) != 0)
			{
				list.Add(elem);
			}
			if ((elem = GetDevice(x + 1, y, z)) != null && (elem.Type & type) != 0)
			{
				list.Add(elem);
			}
			if ((elem = GetDevice(x, y, z - 1)) != null && (elem.Type & type) != 0)
			{
				list.Add(elem);
			}
			if ((elem = GetDevice(x - 1, y, z)) != null && (elem.Type & type) != 0)
			{
				list.Add(elem);
			}
			if ((elem = GetDevice(x, y + 1, z)) != null && (elem.Type & type) != 0)
			{
				list.Add(elem);
			}
			if ((elem = GetDevice(x, y - 1, z)) != null && (elem.Type & type) != 0)
			{
				list.Add(elem);
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