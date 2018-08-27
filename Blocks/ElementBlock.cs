using Engine;
using System.Collections.Generic;

namespace Game
{
	public class ElementBlock : PaintableItemBlock
	{
		public static ElementBlock Block;
		public static WireBlock WireBlock;
		public static Device[] Devices;
		public static readonly ElectricConnectionPath[] PathTable =
		{
			new ElectricConnectionPath(0, 0, 1, 4, 0, 4),
			new ElectricConnectionPath(1, 0, 0, 4, 1, 4),
			new ElectricConnectionPath(0, 0, -1, 4, 2, 4),
			new ElectricConnectionPath(-1, 0, 0, 4, 3, 4),
			new ElectricConnectionPath(0, 1, 0, 4, 4, 4),
			new ElectricConnectionPath(0, -1, 0, 4, 5, 4)
		};
		public override void Initialize()
		{
			Block = (ElementBlock)BlocksManager.Blocks[Index];
			WireBlock = (WireBlock)BlocksManager.Blocks[WireBlock.Index];
			base.Initialize();
		}
		public override Item GetItem(ref int value)
		{
			if (Terrain.ExtractContents(value) != Index)
				return DefaultItem;
			int data = Terrain.ExtractData(value) & 32767;
			return data < Devices.Length ? Devices[data] : DefaultItem;
		}
		/*public Element GetElement(int value)
		{
			return GetItem(ref value) as Element;
		}*/
		public Device GetDevice(int x, int y, int z, int value)
		{
			if (GetItem(ref value) is Device device)
			{
				device.Point = new Point3(x, y, z);
				return device;
			}
			return null;
		}
		public virtual Device GetDevice(Terrain terrain, int x, int y, int z)
		{
			int value = terrain.GetCellValueFast(x, y, z);
			if (GetItem(ref value) is Device device)
			{
				device.Point = new Point3(x, y, z);
				return device;
			}
			return null;
		}
		public void GetAllConnectedNeighbors(Terrain terrain, Device elem, int mountingFace, ICollection<ElectricConnectionPath> list)
		{
			if (mountingFace != 4 || elem == null) return;
			int x, y, z;
			var type = elem.Type;
			var point = elem.Point;
			x = point.X;
			y = point.Y;
			z = point.Z;
			if ((elem = GetDevice(terrain, x, y, z + 1)) != null && (elem.Type & type) != 0)
			{
				list.Add(PathTable[0]);
			}
			if ((elem = GetDevice(terrain, x + 1, y, z)) != null && (elem.Type & type) != 0)
			{
				list.Add(PathTable[1]);
			}
			if ((elem = GetDevice(terrain, x, y, z - 1)) != null && (elem.Type & type) != 0)
			{
				list.Add(PathTable[2]);
			}
			if ((elem = GetDevice(terrain, x - 1, y, z)) != null && (elem.Type & type) != 0)
			{
				list.Add(PathTable[3]);
			}
			if ((elem = GetDevice(terrain, x, y + 1, z)) != null && (elem.Type & type) != 0)
			{
				list.Add(PathTable[4]);
			}
			if ((elem = GetDevice(terrain, x, y - 1, z)) != null && (elem.Type & type) != 0)
			{
				list.Add(PathTable[5]);
			}
		}
		public void GetAllConnectedNeighbors(Terrain terrain, Device elem, int mountingFace, ICollection<Device> list)
		{
			if (mountingFace != 5 || elem == null) return;
			int x, y, z;
			var type = elem.Type;
			var point = elem.Point;
			x = point.X;
			y = point.Y;
			z = point.Z;
			if ((elem = GetDevice(terrain, x, y, z + 1)) != null && (elem.Type & type) != 0)
			{
				list.Add(elem);
			}
			if ((elem = GetDevice(terrain, x + 1, y, z)) != null && (elem.Type & type) != 0)
			{
				list.Add(elem);
			}
			if ((elem = GetDevice(terrain, x, y, z - 1)) != null && (elem.Type & type) != 0)
			{
				list.Add(elem);
			}
			if ((elem = GetDevice(terrain, x - 1, y, z)) != null && (elem.Type & type) != 0)
			{
				list.Add(elem);
			}
			if ((elem = GetDevice(terrain, x, y + 1, z)) != null && (elem.Type & type) != 0)
			{
				list.Add(elem);
			}
			if ((elem = GetDevice(terrain, x, y - 1, z)) != null && (elem.Type & type) != 0)
			{
				list.Add(elem);
			}
		}
		public new const int Index = 501;
		static ElementBlock()
		{
			Devices = new Device[]
			{
				new Fridge(),
				new Generator(),
                new Magnetizer(),
                new Separator(),
                new AirBlower(),
				new WireDevice(),
				new Battery(12, "Models/Battery", "Battery", Matrix.CreateTranslation(0f, -0.5f, 0f) * Matrix.CreateTranslation(0.5f, 0.5f, 0.5f), Matrix.CreateTranslation(11f / 16f, 4f / 256f, 0f) * Matrix.CreateScale(20f), "CuZnBattery", "CuZnBattery"),
				new Battery(12, "Models/Battery", "Battery", Matrix.CreateTranslation(0f, -0.5f, 0f) * Matrix.CreateTranslation(0.5f, 0.5f, 0.5f), Matrix.CreateTranslation(11f / 16f, 4f / 256f, 0f) * Matrix.CreateScale(20f), "AgZnBattery", "AgZnBattery"),
				new Battery(12, "Models/Battery", "Battery", Matrix.CreateTranslation(0f, -0.5f, 0f) * Matrix.CreateTranslation(0.5f, 0.5f, 0.5f), Matrix.CreateTranslation(11f / 16f, 4f / 256f, 0f) * Matrix.CreateScale(20f), "AuZnBattery", "AuZnBattery"),
				new Battery(12, "Models/Battery", "Battery", Matrix.CreateTranslation(0f, -0.5f, 0f) * Matrix.CreateTranslation(0.5f, 0.5f, 0.5f), Matrix.CreateTranslation(-2f / 16f, 4f / 16f, 0f) * Matrix.CreateScale(20f), "VoltaicBattery", "VoltaicBattery"),
			};
			for (int i = 0; i < Devices.Length; i++)
			{
				IdTable.Add(Devices[i].GetCraftingId(), Index | i << 14);
			}
		}
		public override IEnumerable<int> GetCreativeValues()
		{
			var list = new List<int>(Items.Length + 1);
			var set = new HashSet<Item>()
			{
				DefaultItem
			};
			int value = Index;
			Item item = GetItem(ref value);
			var itemBlock =
			item.ItemBlock = new ElementBlock
			{
				BlockIndex = -1
			};
			while (set.Add(item))
			{
				list.Add(value);
				value += 1 << 14;
				(item = GetItem(ref value)).ItemBlock = itemBlock;
			}
			return list;
		}
		public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
		{
			oldValue = Terrain.ReplaceData(Index, Terrain.ExtractData(oldValue) & 32767);
			GetItem(ref oldValue).GetDropValues(subsystemTerrain, oldValue, newValue, toolLevel, dropValues, out showDebris);
		}

		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			var cellFace = raycastResult.CellFace;
			var device = GetDevice(cellFace.X, cellFace.Y, cellFace.Z, value);
			return device != null ? device.GetPlacementValue(subsystemTerrain, componentMiner, value, raycastResult) : new BlockPlacementData
			{
				Value = value,
				CellFace = cellFace
			};
		}
	}
}
