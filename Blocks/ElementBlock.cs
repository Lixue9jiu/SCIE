using Engine;
using System.Collections.Generic;

namespace Game
{
	public class ElementBlock : PaintableItemBlock
	{
		public static Device[] Devices;
		/*public static readonly ElectricConnectionPath[] PathTable =
		{
			new ElectricConnectionPath(0, 0, 1, 5, 5, 5),
			new ElectricConnectionPath(1, 0, 0, 5, 5, 5),
			new ElectricConnectionPath(0, 0, -1, 5, 5, 5),
			new ElectricConnectionPath(-1, 0, 0, 5, 5, 5),
			new ElectricConnectionPath(0, 1, 0, 5, 5, 5),
			new ElectricConnectionPath(0, -1, 0, 5, 5, 5)
		};*/
		public override Item GetItem(ref int value)
		{
			if (Terrain.ExtractContents(value) != BlockIndex)
				return DefaultItem;
			int data = Terrain.ExtractData(value);
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
		/*public virtual Device GetDevice(Terrain terrain, int x, int y, int z)
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
			if (mountingFace != 5 || elem == null) return;
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
		}*/
		public new const int Index = 500;
		public override void Initialize()
		{
			Devices = new Device[]
			{
				new Fridge(),
				new Fridge(),
				new Fridge(),
				new Fridge(),
			};
			base.Initialize();
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
