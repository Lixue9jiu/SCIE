using Engine;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
			Item.Block = new ElementBlock { BlockIndex = -1 };
			Block = (ElementBlock)BlocksManager.Blocks[Index];
			WireBlock = (WireBlock)BlocksManager.Blocks[WireBlock.Index];
			base.Initialize();
			if (!Item.Task.IsCompleted)
				Item.Task.Wait();
			Item.Task.Dispose();
			Item.Task = Task.Run((Action)Load);
		}

		public override IItem GetItem(ref int value)
		{
			return Terrain.ExtractContents(value) != Index ? base.GetItem(ref value) : Devices[Terrain.ExtractData(value) & 1023];
		}

		public Device GetDevice(int x, int y, int z, int value)
		{
			var p = new Point3(x, y, z);
			if (SubsystemCircuit.Table.TryGetValue(p, out Device device))
				return device;
			device = GetItem(ref value) as Device;
			if (device == null) return null;
			device = (Device)device.Clone();
			device.Point = p;
			device.Next = new Element[0];
			if ((device.Type & ElementType.Connector) != 0)
			{
				var color = GetColor(Terrain.ExtractData(Utils.Terrain.GetCellValue(p.X, p.Y, p.Z)));
				device.Type = device.Type & ~ElementType.Connector | (color.HasValue ? (ElementType)(1 << (color.Value + 2)) : ElementType.Connector);
			}
			return device.Create(p, value);
		}

		public Device GetDevice(Terrain terrain, int x, int y, int z)
		{
			return GetDevice(x, y, z, terrain.GetCellValueFast(x, y, z));
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
			if ((elem = GetDevice(terrain, x, y, z + 1)) != null && (elem.Type & type) > ElementType.Supply)
				list.Add(PathTable[0]);
			if ((elem = GetDevice(terrain, x + 1, y, z)) != null && (elem.Type & type) > ElementType.Supply)
				list.Add(PathTable[1]);
			if ((elem = GetDevice(terrain, x, y, z - 1)) != null && (elem.Type & type) > ElementType.Supply)
				list.Add(PathTable[2]);
			if ((elem = GetDevice(terrain, x - 1, y, z)) != null && (elem.Type & type) > ElementType.Supply)
				list.Add(PathTable[3]);
			if (y < 127 && (elem = GetDevice(terrain, x, y + 1, z)) != null && (elem.Type & type) > ElementType.Supply)
				list.Add(PathTable[4]);
			if (y > 0 && (elem = GetDevice(terrain, x, y - 1, z)) != null && (elem.Type & type) > ElementType.Supply)
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
			if ((elem = GetDevice(terrain, x, y, z + 1)) != null && (elem.Type & type) > ElementType.Supply)
				list.Add(elem);
			if ((elem = GetDevice(terrain, x + 1, y, z)) != null && (elem.Type & type) > ElementType.Supply)
				list.Add(elem);
			if ((elem = GetDevice(terrain, x, y, z - 1)) != null && (elem.Type & type) > ElementType.Supply)
				list.Add(elem);
			if ((elem = GetDevice(terrain, x - 1, y, z)) != null && (elem.Type & type) > ElementType.Supply)
				list.Add(elem);
			if (y < 127 && (elem = GetDevice(terrain, x, y + 1, z)) != null && (elem.Type & type) > ElementType.Supply)
				list.Add(elem);
			if (y > 0 && (elem = GetDevice(terrain, x, y - 1, z)) != null && (elem.Type & type) > ElementType.Supply)
				list.Add(elem);
		}

		public new const int Index = 501;

		public override IEnumerable<int> GetCreativeValues()
		{
			var list = new DynamicArray<int>(Devices.Length << 4);
			int value = Index, i = 0, j;
			for (; i < Devices.Length; i++)
			{
				list.Add(value);
				for (j = 1; j < 16; j++)
					list.Add(Paint(null, value, j));
				value += 1 << 14;
			}
			for (i = 13; i < 20; i++)
			{
				for (j = 0; j < (16 << 3); j++)
				{
					value = Index | i << 14 | (j >> 4) << (14 + 12);
					list.Add(Paint(null, value, j & 15));
				}
			}
			return list;
		}

		public override string GetCategory(int value)
		{
			return GetPaintColor(value).HasValue ? "Painted" : Utils.Get("»úÆ÷");
		}

		public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
		{
			GetItem(ref oldValue).GetDropValues(subsystemTerrain, Terrain.ReplaceData(Index, Terrain.ExtractData(oldValue) & 32767), newValue, toolLevel, dropValues, out showDebris);
			showDebris = false;
		}

		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			var cellFace = raycastResult.CellFace;
			var p = CellFace.FaceToPoint3(cellFace.Face);
			var device = GetDevice(cellFace.X + p.X, cellFace.Y + p.Y, cellFace.Z + p.Z, value);
			return device != null ? device.GetPlacementValue(subsystemTerrain, componentMiner, value, raycastResult) : new BlockPlacementData
			{
				Value = value,
				CellFace = cellFace
			};
		}

		public ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			return GetDevice(x, y, z, value) is IElectricElementBlock block
				? block.CreateElectricElement(subsystemElectricity, value, x, y, z)
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