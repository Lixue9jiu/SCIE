using System.Collections.Generic;
using Engine;
using Engine.Graphics;

namespace Game
{
	public class TankBlock : CubeBlock
	{
		public enum Type
		{
			Tank,
			Reductor
		}
		static readonly string[] Names = { "空油箱", "减压器" };
		static readonly string[] Description = { "某些化学材料的容器，非常重且耐用。在化学领域有重要作用。", "减压器是一种可以降低液体或气体压力的机器，这是石油化学中必不可少的组成部分。" };
		public const int Index = 522;

		public override IEnumerable<int> GetCreativeValues()
		{
			return new[] { Index, Index | 1 << 14, Index | 2 << 14 };
		}

		public override int GetFaceTextureSlot(int face, int value)
		{
			if (GetType(value) == Type.Reductor)
				return 144;
			return face == 4 || face == 5 ? 181 : 210;
		}

		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			return Names[Terrain.ExtractData(value)];
		}

		public override string GetDescription(int value)
		{
			return Description[Terrain.ExtractData(value)];
		}

		public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
		{
			showDebris = false;
			dropValues.Add(new BlockDropValue { Value = oldValue, Count = 1 });
		}

		public static Type GetType(int value)
		{
			return (Type)Terrain.ExtractData(value);
		}

		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			ItemBlock.DrawCubeBlock(primitivesRenderer, value, new Vector3(size), ref matrix, color, color, environmentData);
		}

		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometrySubsets geometry, int value, int x, int y, int z)
		{
			Utils.BlockGeometryGenerator.GenerateCubeVertices(this, value, x, y, z, Color.White, Utils.GTV(x, z, geometry).OpaqueSubsetsByFace);
		}
	}
}