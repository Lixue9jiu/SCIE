using System.Collections.Generic;
using Engine;
using TemplatesDatabase;

namespace Game
{
	public class SubsystemEnergy : SubsystemConnectionModel
	{
		public static readonly ElectricConnectionPath[] PathTable =
		{
			new ElectricConnectionPath(0, 0, 1, 5, 5, 5),
			new ElectricConnectionPath(1, 0, 0, 5, 5, 5),
			new ElectricConnectionPath(0, 0, -1, 5, 5, 5),
			new ElectricConnectionPath(-1, 0, 0, 5, 5, 5),
			new ElectricConnectionPath(0, 1, 0, 5, 5, 5),
			new ElectricConnectionPath(0, -1, 0, 5, 5, 5)
		};
		public override int[] HandledBlocks => new int[] { 601 };
		public static Element GetCircuitElement(int value)
		{
			if (Terrain.ExtractContents(value) == 300)
			{
				switch (Terrain.ExtractData(value))
				{
					case 1: return new SmallGenerator();
					case 2: return new WireElement();
					case 3: return new ElectricFurnace();
					case 10: return new DiodeDevice();
					case 12: return new Battery12V();
				}
			}
			return null;
		}
		public static Device GetDevice(Terrain terrain, int x, int y, int z)
		{
			if (GetCircuitElement(terrain.GetCellValueFast(x, y, z)) is Device device)
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
		public static void GetAllConnectedNeighbors(Terrain terrain, Device elem, int mountingFace, ICollection<Device> list)
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
	}
}