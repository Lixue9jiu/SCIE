using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Engine;
using Game;
using TemplatesDatabase;

namespace Game
{
	public class SubsystemEnergy : SubsystemBlockBehavior, IUpdateable
	{
		public SubsystemTime SubsystemTime;
		//public SubsystemAudio SubsystemAudio;
		float m_remainingSimulationTime;
		public int CircuitStep;
		public DynamicArray<CircuitElement> Path;
		public Dictionary<Point3, CircuitElement> Table;
		public static readonly ElectricConnectionPath[] PathTable =
		{
			new ElectricConnectionPath(0, 0, 1, 5, 5, 5),
			new ElectricConnectionPath(1, 0, 0, 5, 5, 5),
			new ElectricConnectionPath(0, 0, -1, 5, 5, 5),
			new ElectricConnectionPath(-1, 0, 0, 5, 5, 5),
			new ElectricConnectionPath(0, 1, 0, 5, 5, 5),
			new ElectricConnectionPath(0, -1, 0, 5, 5, 5)
		};
		public override int[] HandledBlocks
		{
			get { return new int[0]; }
		}
		public int UpdateOrder
		{
			get { return 0; }
		}
		public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			int count = valuesDictionary.GetValue<int>("Count", 0);
			Path = new DynamicArray<CircuitElement>(count);
			Table = new Dictionary<Point3, CircuitElement>(count);
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
			var element = GetCircuitDevice(SubsystemTerrain.Terrain, x, y, z);
			if (element != null)
			{
				var stack = new DynamicArray<Device>(1);
				CircuitElement visited;
				var powers = new Dictionary<CircuitElement, int>();
				stack.Add(element);
				while (stack.Count > 0)
				{
					var current = stack.Array[stack.Count - 1];//当前顶点
					if (Table.TryGetValue(current.Point, out visited))//如果访问过
					{
						continue;
					}
					Table.Add(current.Point, current);//将该点添加到表中
					var neighbors = new DynamicArray<Device>();//当前顶点的邻接表
					GetAllConnectedNeighbors(SubsystemTerrain.Terrain, current, 5, (ICollection<ElectricConnectionPath>)neighbors);
					var narr = neighbors.Array;
					QuickSort(narr, 0, narr.Length - 1);
					bool flag = false;
					for (int i = 0; i < narr.Length; i++)
					{
						int index;
						if (narr[i].GetResistance() == 1)
							flag = true;
						else if (flag)
						{
							if ((narr[i].Type & ElementType.Power) != 0)
							{
								if (powers.TryGetValue(narr[i], out index))
								{
									if (index < stack.Count)
									{
										goto pop;
									}
								}
								else
									powers.Add(narr[i], stack.Count);
							}
						}
						stack.Add(narr[i]);//将该点添加到访问栈中
					}
					pop: stack.RemoveAtEnd();
				}
				if ((element.Type & ElementType.Power) != 0)
					Path.Add(element);
			}
		}
		public override void OnBlockRemoved(int value, int newValue, int x, int y, int z)
		{
			var element = GetCircuitDevice(SubsystemTerrain.Terrain, x, y, z);
			if (element != null)
			{
				var next = element.Next.Array;
				for (int i = 0; i < next.Length; i++)
				{
					next[i].Next.Remove(element);
				}
				if ((element.Type & ElementType.Power) != 0)
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
			var element = GetCircuitDevice(SubsystemTerrain.Terrain, x, y, z);
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
			var arr = Path.Array;
			for (var i = Table.GetEnumerator(); i.MoveNext();)
			{
				var key = i.Current.Key;
				if (key.X >= startX && key.X < chunk.Origin.X + 16 && key.Z >= startY && key.Z < startY + 16)
				{
					int index = Path.IndexOf(i.Current.Value);
					if (index >= 0)
					{
						arr[index] = null;
					}
				}
			}
		}
		public void Update(float dt)
		{
			m_remainingSimulationTime = MathUtils.Min(m_remainingSimulationTime + dt, 0.1f);
			while (m_remainingSimulationTime >= 0.02f)
			{
				CircuitStep++;
				m_remainingSimulationTime -= 0.02f;
				var arr = Path.Array;
				for (int i = 0; i < Path.Count; i++)
				{
					var element = arr[i];
					if (element == null)
						continue;
					int voltage = 0;
					var stack = new DynamicArray<CircuitElement>(1);
					stack.Add(element);
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
		public static CircuitElement GetCircuitElement(int value)
		{
			if (Terrain.ExtractContents(value) == 300)
			{
				switch (Terrain.ExtractData(value))
				{
					case 1: return new SmallGenerator();
					case 2: return new Wire();
					case 3: return new ElectricFurnace();
					case 10: return new DiodeDevice();
					case 12: return new Battery12V();
				}
			}
			return null;
		}
		public static Device GetCircuitDevice(Terrain terrain, int x, int y, int z)
		{
			var device = GetCircuitElement(terrain.GetCellValueFast(x, y, z)) as Device;
			if (device != null)
			{
				device.Point = new Point3(x, y, z);
				return device;
			}
			return null;
		}
		public static void GetAllConnectedNeighbors(Terrain terrain, Device elem, int mountingFace, ICollection<ElectricConnectionPath> list)
		{
			if (mountingFace != 5 || elem == null) return;
			int x, y, z;
			var type = elem.Type;
			var point = elem.Point;
			x = point.X;
			y = point.Y;
			z = point.Z;
			if ((elem = GetCircuitDevice(terrain, x, y, z + 1)) != null && (elem.Type & type) != 0)
			{
				list.Add(PathTable[0]);
			}
			if ((elem = GetCircuitDevice(terrain, x + 1, y, z)) != null && (elem.Type & type) != 0)
			{
				list.Add(PathTable[1]);
			}
			if ((elem = GetCircuitDevice(terrain, x, y, z - 1)) != null && (elem.Type & type) != 0)
			{
				list.Add(PathTable[2]);
			}
			if ((elem = GetCircuitDevice(terrain, x - 1, y, z)) != null && (elem.Type & type) != 0)
			{
				list.Add(PathTable[3]);
			}
			if ((elem = GetCircuitDevice(terrain, x, y + 1, z)) != null && (elem.Type & type) != 0)
			{
				list.Add(PathTable[4]);
			}
			if ((elem = GetCircuitDevice(terrain, x, y - 1, z)) != null && (elem.Type & type) != 0)
			{
				list.Add(PathTable[5]);
			}
		}
		public void GarbageCollectItems()
		{
			var path = new DynamicArray<CircuitElement>(Path.Count >> 1);
			var arr = Path.Array;
			for (int i = 0; i < arr.Length; i++)
				if (arr[i] != null)
					path.Add(arr[i]);
			Path = path;
		}
	}
}