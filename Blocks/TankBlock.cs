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
		public const int Index = 522;
		public static readonly string[] Names = { "������", "��ѹ��" };
		public static readonly string[] Description = { "ĳЩ��ѧ���ϵ��������ǳ��������á��ڻ�ѧ��������Ҫ���á�", "��ѹ����һ�ֿ��Խ���Һ�������ѹ���Ļ���������ʯ�ͻ�ѧ�бز����ٵ���ɲ��֡�" };

		public override IEnumerable<int> GetCreativeValues()
		{
			return new[] { Index, Index | 1 << 14 };
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