using System.Collections.Generic;
using Engine;
using Engine.Graphics;

namespace Game
{
	public class FireBoxBlock : FurnaceNBlock
	{
		public new const int Index = 532;
		public static readonly string[] Names = new[]
		{
			"ÍË»ğÂ¯",
			"ÕÖÊ½Â¯",
			"ÏäÊ½Â¯",
			"ÃºÆøÂ¯",
			"´ã»ğÂ¯",
			"È¼Æø¼ÓÈÈÂ¯",
			"Õæ¿ÕÂ¯",
			"²£Á§ÍË»ğÒ¤",
		};

		public override int GetFaceTextureSlot(int face, int value)
		{
			if (face == 4 || face == 5)
				return 221;
			int offset = GetHeatLevel(value);
			switch (GetDirection(value))
			{
				case 0:
					return face == 0 ? 222 + offset : 221;
				case 1:
					return face == 1 ? 222 + offset : 221;
				case 2:
					return face == 2 ? 222 + offset : 221;
			}
					return face == 3 ? 222 + offset : 221;
		}
		public override IEnumerable<int> GetCreativeValues()
		{
			var arr = new int[17 * 9];
			for (int i = 0; i < 9; i++)
			{
				arr[i * 17] = BlockIndex | i << 24;
				for (int j = 1; j < 17; j++)
					arr[i * 17 + j] = BlockIndex | SetColor(i << 10, j - 1) << 14;
			}
			return arr;
		}
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			base.DrawBlock(primitivesRenderer, value, Terrain.ExtractData(value) >> 10 != 0 ? color * Color.LightGray : color, size, ref matrix, environmentData);
		}
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometrySubsets geometry, int value, int x, int y, int z)
		{
			Color color = SubsystemPalette.GetColor(generator, GetPaintColor(value));
			generator.GenerateCubeVertices(this, value, x, y, z, Terrain.ExtractData(value) >> 10 != 0 ? color * Color.LightGray : color, geometry.OpaqueSubsetsByFace);
		}
		public override string GetDescription(int value)
		{
			value = Terrain.ExtractData(value) >> 10;
			return value != 0 ? Utils.Get(Names[value - 1]) : DefaultDescription;
		}
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			return SubsystemPalette.GetName(subsystemTerrain, GetPaintColor(value), Terrain.ExtractData(value) >> 10 != 0 ? Names[(Terrain.ExtractData(value) >> 10) - 1] : DefaultDisplayName);
		}
	}
}