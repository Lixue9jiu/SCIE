using System;
using Engine;
using Engine.Graphics;
using System.Collections.Generic;

namespace Game
{
	public class ElementBlock : ItemBlock, IPaintableBlock
	{
		public static Device[] Devices;
		public static readonly ElectricConnectionPath[] PathTable =
		{
			new ElectricConnectionPath(0, 0, 1, 5, 5, 5),
			new ElectricConnectionPath(1, 0, 0, 5, 5, 5),
			new ElectricConnectionPath(0, 0, -1, 5, 5, 5),
			new ElectricConnectionPath(-1, 0, 0, 5, 5, 5),
			new ElectricConnectionPath(0, 1, 0, 5, 5, 5),
			new ElectricConnectionPath(0, -1, 0, 5, 5, 5)
		};
		public override Item GetItem(ref int value)
		{
			if (Terrain.ExtractContents(value) != BlockIndex)
				return DefaultItem;
			value = Terrain.ExtractData(value);
			if (value < Devices.Length)
			{
				return Devices[value];
			}
			return DefaultItem;
		}
		public Element GetElement(int value)
		{
			return GetItem(ref value) as Element;
		}
		public Device GetDevice(int x, int y, int z, int value)
		{
			if (GetElement(value) is Device device)
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
		}
		public new const int Index = 500;
		public override void Initialize()
		{
			Devices = new Device[]
			{
				new Fridge(),
			};
			base.Initialize();
		}
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometrySubsets geometry, int value, int x, int y, int z)
		{
			var device = GetDevice(x, y, z, value);
			if (device != null)
			{
				device.GenerateTerrainVertices(this, generator, geometry, value, x, y, z);
			}
		}
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			var element = GetElement(value);
			if (element != null)
			{
				element.DrawBlock(primitivesRenderer, value, color, size, ref matrix, environmentData);
			}
		}
		public override IEnumerable<int> GetCreativeValues()
		{
			var list = new List<int>(8);
			for (int i = 0, value = Index; GetElement(value) != null; value = Terrain.MakeBlockValue(Index, 0, ++i))
			{
				list.Add(value);
			}
			return list;
		}
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			var element = GetElement(value);
			if (element != null)
			{
				int? paintColor = GetPaintColor(value);
				return SubsystemPalette.GetName(subsystemTerrain, paintColor, element.GetDisplayName(subsystemTerrain, value));
			}
			return DefaultDisplayName;
		}
		public override int GetFaceTextureSlot(int face, int value)
		{
			var element = GetElement(value);
			return element != null ? element.GetFaceTextureSlot(face, value) : DefaultTextureSlot;
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
		public override string GetDescription(int value)
		{
			var element = GetElement(value);
			return element != null ? element.GetDescription(value) : "";
		}
		public int? GetPaintColor(int value)
		{
			return GetColor(Terrain.ExtractData(value));
		}
		public int Paint(SubsystemTerrain subsystemTerrain, int value, int? color)
		{
			int data = Terrain.ExtractData(value);
			return Terrain.ReplaceData(value, SetColor(data, color));
		}
		public static int? GetColor(int data)
		{
			return (data & 64) != 0 ? data >> 7 & 15 : default(int?);
		}
		public static int SetColor(int data, int? color)
		{
			if (color.HasValue)
			{
				return (data & -1985) | 64 | (color.Value & 15) << 7;
			}
			return data & -1985;
		}
	}
}
