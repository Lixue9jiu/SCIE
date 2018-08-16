using Engine;
using Engine.Graphics;
using System.Collections.Generic;

namespace Game
{
	public class ElementBlock : Block, IPaintableBlock
	{
		public const int Index = 600;
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometrySubsets geometry, int value, int x, int y, int z)
		{
			var device = SubsystemCircuit.GetDevice(generator.Terrain, x, y, z);
			if (device != null)
			{
				device.GenerateTerrainVertices(this, generator, geometry, value, x, y, z);
			}
		}
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			var element = SubsystemCircuit.GetElement(value);
			if (element != null)
			{
				element.DrawBlock(primitivesRenderer, value, color, size, ref matrix, environmentData);
			}
		}
		public override IEnumerable<int> GetCreativeValues()
		{
			var list = new List<int>(8);
			for (int i = 0, value = Terrain.MakeBlockValue(Index, 0, i); SubsystemCircuit.GetElement(value) != null; value = Terrain.MakeBlockValue(Index, 0, ++i))
			{
				list.Add(value);
			}
			return list;
		}
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			var element = SubsystemCircuit.GetElement(value);
			if (element != null)
			{
				int? paintColor = GetPaintColor(value);
				return SubsystemPalette.GetName(subsystemTerrain, paintColor, element.GetDisplayName(subsystemTerrain, value));
			}
			return DefaultDisplayName;
		}
		public override int GetFaceTextureSlot(int face, int value)
		{
			var element = SubsystemCircuit.GetElement(value);
			return element != null ? element.GetFaceTextureSlot(face, value) : DefaultTextureSlot;
		}

		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			var cellFace = raycastResult.CellFace;
			var device = SubsystemCircuit.GetDevice(subsystemTerrain.Terrain, cellFace.X, cellFace.Y, cellFace.Z);
			return device != null ? device.GetPlacementValue(subsystemTerrain, componentMiner, value, raycastResult) : new BlockPlacementData
			{
				Value = value,
				CellFace = raycastResult.CellFace
			};
		}
		public override string GetDescription(int value)
		{
			var element = SubsystemCircuit.GetElement(value);
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
