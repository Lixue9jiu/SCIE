using Engine;
using Engine.Graphics;
using System.Collections.Generic;

namespace Game
{
	public class CReactorBlock : PaintedCubeBlock
	{
		public const int Index = 524;

		public CReactorBlock() : base(0)
		{
		}

		public override IEnumerable<int> GetCreativeValues()
		{
			var arr = new int[34];
			arr[0] = BlockIndex;
			int i;
			for (i = 1; i < 17; i++)
				arr[i] = BlockIndex | SetColor(0, i - 1) << 14;
			arr[17] = BlockIndex | 1 << 24;
			for (i = 18; i < 34; i++)
				arr[i] = BlockIndex | SetColor(1 << 10, i - 1) << 14;
			return arr;
		}

		public override int GetFaceTextureSlot(int face, int value)
		{
			return face == 4 ? 192 : 107;
		}

		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			ItemBlock.DrawCubeBlock(primitivesRenderer, value, new Vector3(size), ref matrix, color, color, environmentData);
		}

		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometrySubsets geometry, int value, int x, int y, int z)
		{
			Utils.BlockGeometryGenerator.GenerateCubeVertices(this, value, x, y, z, SubsystemPalette.GetColor(generator, GetPaintColor(value)), Utils.GTV(x, z, geometry).OpaqueSubsetsByFace);
		}

		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			return (Terrain.ExtractData(value) & 1024) != 0 ? Utils.Get("真空炉") : DefaultDisplayName;
		}

		public override string GetDescription(int value)
		{
			return (Terrain.ExtractData(value) & 1024) != 0 ? Utils.Get("真空炉") : DefaultDescription;
		}
	}

	public class MachineToolBlock : PaintedCubeBlock, IElectricElementBlock
	{
		public const int Index = 508;

		public MachineToolBlock() : base(0)
		{
		}

		public override int GetFaceTextureSlot(int face, int value)
		{
			return face == 4 ? 146 : 107;
		}

		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			ItemBlock.DrawCubeBlock(primitivesRenderer, value, new Vector3(size), ref matrix, color, color, environmentData);
		}

		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometrySubsets geometry, int value, int x, int y, int z)
		{
			Utils.BlockGeometryGenerator.GenerateCubeVertices(this, value, x, y, z, SubsystemPalette.GetColor(generator, GetPaintColor(value)), Utils.GTV(x, z, geometry).OpaqueSubsetsByFace);
		}

		public ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			return new CraftingMachineElectricElement(subsystemElectricity, new Point3(x, y, z));
		}

		public ElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z)
		{
			return face == 4 || face == 5 ? (ElectricConnectorType?)ElectricConnectorType.Input : null;
		}

		public int GetConnectionMask(int value)
		{
			return 2147483647;
		}
	}

	public class BlastBlowerBlock : FourDirectionalBlock
	{
		public const int Index = 529;

		public override int GetFaceTextureSlot(int face, int value)
		{
			return face == 4 || face == 5 ? 107 : 241;
		}

		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			return new BlockPlacementData { Value = value, CellFace = raycastResult.CellFace };
		}
	}

	public class SourBlock : PaintedCubeBlock
	{
		public const int Index = 507;

		public SourBlock() : base(186)
		{
		}

		public static readonly string[] Names = new[]
		{
			"发酵池",
			"沉淀池"
		};

		public static readonly string[] Descriptions = new[]
		{
			"发酵池，可以用腐烂的食物来发酵，生产硝石",
			"沉淀池，可以用来沉淀某些特殊物质"
		};

		public override IEnumerable<int> GetCreativeValues()
		{
			var arr = new int[17 * 2];
			for (int i = 0; i < 2; i++)
			{
				arr[i * 17] = BlockIndex | i << 24;
				for (int j = 1; j < 17; j++)
					arr[i * 17 + j] = BlockIndex | SetColor(i << 10, j - 1) << 14;
			}
			return arr;
		}

		public override int GetFaceTextureSlot(int face, int value)
		{
			return (Terrain.ExtractData(value) >> 10 == 0) ? face == 4 ? 116 : 115 : face == 4 ? 224 : 107;
		}

		public override string GetDescription(int value)
		{
			value = Terrain.ExtractData(value) >> 10;
			return Utils.Get(Descriptions[Terrain.ExtractData(value) >> 10]);
		}

		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			return SubsystemPalette.GetName(subsystemTerrain, GetPaintColor(value), Names[Terrain.ExtractData(value) >> 10]);
		}

		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			ItemBlock.DrawCubeBlock(primitivesRenderer, value, new Vector3(size), ref matrix, color, color, environmentData);
		}

		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometrySubsets geometry, int value, int x, int y, int z)
		{
			Utils.BlockGeometryGenerator.GenerateCubeVertices(this, value, x, y, z, SubsystemPalette.GetColor(generator, GetPaintColor(value)), Utils.GTV(x, z, geometry).OpaqueSubsetsByFace);
		}
	}
}