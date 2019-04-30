using Engine;
using System.Collections.Generic;

namespace Game
{
	public class ElementBlock : PaintableItemBlock, IElectricElementBlock
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
			Item.ItemBlock = new ElementBlock { BlockIndex = -1 };
			Block = (ElementBlock)BlocksManager.Blocks[Index];
			WireBlock = (WireBlock)BlocksManager.Blocks[WireBlock.Index];
			base.Initialize();
		}
		public override IItem GetItem(ref int value)
		{
			return Terrain.ExtractContents(value) != Index ? base.GetItem(ref value) : Devices[Terrain.ExtractData(value) & 1023];
		}
		/*public Element GetElement(int value)
		{
			return GetItem(ref value) as Element;
		}*/
		public Device GetDevice(int x, int y, int z, int value)
		{
			var device = GetItem(ref value);
			return (device is Device d) ? d.Create(new Point3(x, y, z)) : null;
		}
		public virtual Device GetDevice(Terrain terrain, int x, int y, int z)
		{
			int value = terrain.GetCellValueFast(x, y, z);
			var device = GetItem(ref value);
			return (device is Device d) ? d.Create(new Point3(x, y, z)) : null;
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
				list.Add(PathTable[0]);
			if ((elem = GetDevice(terrain, x + 1, y, z)) != null && (elem.Type & type) != 0)
				list.Add(PathTable[1]);
			if ((elem = GetDevice(terrain, x, y, z - 1)) != null && (elem.Type & type) != 0)
				list.Add(PathTable[2]);
			if ((elem = GetDevice(terrain, x - 1, y, z)) != null && (elem.Type & type) != 0)
				list.Add(PathTable[3]);
			if (y < 127 && (elem = GetDevice(terrain, x, y + 1, z)) != null && (elem.Type & type) != 0)
				list.Add(PathTable[4]);
			if (y > 0 && (elem = GetDevice(terrain, x, y - 1, z)) != null && (elem.Type & type) != 0)
				list.Add(PathTable[5]);
		}
		public void GetAllConnectedNeighbors(Terrain terrain, Device elem, int mountingFace, ICollection<Device> list)
		{
			if (mountingFace != 4 || elem == null) return;
			int x, y, z;
			var type = elem.Type;
			var point = elem.Point;
			x = point.X;
			y = point.Y;
			z = point.Z;
			if ((elem = GetDevice(terrain, x, y, z + 1)) != null && (elem.Type & type) != 0)
				list.Add(elem);
			if ((elem = GetDevice(terrain, x + 1, y, z)) != null && (elem.Type & type) != 0)
				list.Add(elem);
			if ((elem = GetDevice(terrain, x, y, z - 1)) != null && (elem.Type & type) != 0)
				list.Add(elem);
			if ((elem = GetDevice(terrain, x - 1, y, z)) != null && (elem.Type & type) != 0)
				list.Add(elem);
			if (y < 127 && (elem = GetDevice(terrain, x, y + 1, z)) != null && (elem.Type & type) != 0)
				list.Add(elem);
			if (y > 0 && (elem = GetDevice(terrain, x, y - 1, z)) != null && (elem.Type & type) != 0)
				list.Add(elem);
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
				new EFurnace(),
                new Canpack(),
                new Electrobath(),
                new Battery(12, "Models/Battery", "Battery", Matrix.CreateTranslation(0f, -0.5f, 0f) * Matrix.CreateTranslation(new Vector3(0.5f)), Matrix.CreateTranslation(11f / 16f, 4f / 256f, 0f), "CuZnBattery", "CuZnBattery"),
				new Battery(12, "Models/Battery", "Battery", Matrix.CreateTranslation(0f, -0.5f, 0f) * Matrix.CreateTranslation(new Vector3(0.5f)), Matrix.CreateTranslation(11f / 16f, 4f / 256f, 0f), "Ag-Zn电池", "Ag-Zn电池"),
				new Battery(12, "Models/Battery", "Battery", Matrix.CreateTranslation(0f, -0.5f, 0f) * Matrix.CreateTranslation(new Vector3(0.5f)), Matrix.CreateTranslation(11f / 16f, 4f / 256f, 0f), "Au-Zn电池", "Au-Zn电池"),
				new Battery(12, "Models/Battery", "Battery", Matrix.CreateTranslation(0f, -0.5f, 0f) * Matrix.CreateTranslation(new Vector3(0.5f)), Matrix.CreateTranslation(-2f / 16f, 4f / 16f, 0f), "伏打电池", "伏打电池"),
				new Pipe(),
				new Pipe(1),
				new Pipe(2),
				new Pipe(3),
				new Pipe(4),
				new Pipe(5),
				new Pipe(6),
				new Pipe(7),
				new TEDC(),
               
            };
			for (int i = 0; i < Devices.Length; i++)
				IdTable.Add(Devices[i].GetCraftingId(), Index | i << 14);
		}
		public override IEnumerable<int> GetCreativeValues()
		{
			var list = new List<int>(Devices.Length << 4);
			int value = Index, i = 0, j;
			for (; i < Devices.Length; i++)
			{
				list.Add(value);
				for (j = 1; j < 16; j++)
					list.Add(Paint(null, value, j));
				value += 1 << 14;
			}
			for (i = 11; i < 18; i++)
			{
				for (j = 0; j < (16 << 3); j++)
				{
					value = Index | i << 14 | (j >> 4) << (14 + 12);
					list.Add(Paint(null, value, j & 15));
				}
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
			var p = CellFace.FaceToPoint3(cellFace.Face);
			var device = GetDevice(cellFace.X + p.X, cellFace.Y + p.X, cellFace.Z + p.Z, value);
			return device != null ? device.GetPlacementValue(subsystemTerrain, componentMiner, value, raycastResult) : new BlockPlacementData
			{
				Value = value,
				CellFace = cellFace
			};
		}
		public ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			return GetDevice(x, y, z, value) is IElectricElementBlock electricElementBlock
				? electricElementBlock.CreateElectricElement(subsystemElectricity, value, x, y, z)
				: null;
		}

		public ElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z)
		{
			return GetDevice(x, y, z, value) is IElectricElementBlock electricElementBlock
				? electricElementBlock.GetConnectorType(terrain, value, face, connectorFace, x, y, z)
				: null;
		}

		public int GetConnectionMask(int value)
		{
			return GetItem(ref value) is IElectricElementBlock electricElementBlock ? electricElementBlock.GetConnectionMask(value) : 0;
		}
	}
}