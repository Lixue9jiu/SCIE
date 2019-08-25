using Engine;
using Engine.Graphics;
using System.Collections.Generic;

namespace Game
{
	public abstract class CubeMachBlock : PaintedCubeBlock
	{
		protected CubeMachBlock() : base(0)
		{
		}
		public override IEnumerable<int> GetCreativeValues()
		{
			var arr = new int[17];
			arr[0] = BlockIndex;
			for (int i = 1; i < 17; i++)
				arr[i] = BlockIndex | SetColor(0, i - 1) << 14;
			return arr;
		}

		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			color *= SubsystemPalette.GetColor(environmentData, GetPaintColor(value));
			ItemBlock.DrawCubeBlock(primitivesRenderer, value, new Vector3(size), ref matrix, color, color, environmentData);
		}

		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometrySubsets geometry, int value, int x, int y, int z)
		{
			Utils.BlockGeometryGenerator.GenerateCubeVertices(this, value, x, y, z, SubsystemPalette.GetColor(generator, GetPaintColor(value)), Utils.GTV(x, z, geometry).OpaqueSubsetsByFace);
		}

		public override string GetCategory(int value)
		{
			return GetPaintColor(value).HasValue ? "Painted" : Utils.Get("机器");
		}
	}

	public class MachineToolBlock : FourDirectionalBlock, IElectricElementBlock
	{
		public const int Index = 508;

		public override int GetFaceTextureSlot(int face, int value)
		{
			return face == 4 ? 146 : 107;
		}

		public new ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			return new CraftingMachineElectricElement(subsystemElectricity, new Point3(x, y, z));
		}

		public new ElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z)
		{
			return face == 4 || face == 5 ? (ElectricConnectorType?)ElectricConnectorType.Input : null;
		}
	}

	public class CReactorBlock : CubeMachBlock
	{
		public const int Index = 524;

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

		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			return (Terrain.ExtractData(value) & 1024) != 0 ? Utils.Get("真空炉") : DefaultDisplayName;
		}

		public override string GetDescription(int value)
		{
			return (Terrain.ExtractData(value) & 1024) != 0 ? Utils.Get("真空炉") : DefaultDescription;
		}
	}

	public class BlastBlowerBlock : CubeMachBlock
	{
		public const int Index = 529;

		public override int GetFaceTextureSlot(int face, int value)
		{
			return face == 4 || face == 5 ? 107 : 241;
		}
	}

	public class SpinnerBlock : CubeMachBlock
	{
		public const int Index = 535;

		public override int GetFaceTextureSlot(int face, int value)
		{
			return face == 4 ? 118 : 115;
		}
	}

	public class SourBlock : CubeMachBlock
	{
		public const int Index = 507;

		public static readonly string[] Names = new[]
		{
			"发酵池",
			"沉淀池",
			"分拣机"
		};

		public static readonly string[] Descriptions = new[]
		{
			"发酵池，可以用腐烂的食物来发酵，生产硝石",
			"沉淀池，可以用来沉淀某些特殊物质",
			"分拣机，可以用来分拣不同物品"
		};

		public override IEnumerable<int> GetCreativeValues()
		{
			var arr = new int[17 * 3];
			for (int i = 0; i < 3; i++)
			{
				arr[i * 17] = BlockIndex | i << 24;
				for (int j = 1; j < 17; j++)
					arr[i * 17 + j] = BlockIndex | SetColor(i << 10, j - 1) << 14;
			}
			return arr;
		}

		public override int GetFaceTextureSlot(int face, int value)
		{
			value = Terrain.ExtractData(value) >> 10;
			if (value>=2)
				return 131;
			return value == 0 ? face == 4 ? 116 : 115 : face == 4 ? 224 : 107;
		}

		public override string GetDescription(int value)
		{
			return Utils.Get(Descriptions[Terrain.ExtractData(value) >> 10]);
		}

		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			return SubsystemPalette.GetName(subsystemTerrain, GetPaintColor(value), Names[Terrain.ExtractData(value) >> 10]);
		}

		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			return new BlockPlacementData { Value = value, CellFace = raycastResult.CellFace };
		}

		public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
		{
			showDebris = false;
			dropValues.Add(new BlockDropValue { Value = Terrain.ReplaceLight(oldValue, 0), Count = 1 });
		}
	}
}