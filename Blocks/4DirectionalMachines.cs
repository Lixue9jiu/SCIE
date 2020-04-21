using Engine;
using Engine.Graphics;
using System.Collections.Generic;

namespace Game
{
	public class PresserBlock : FourDirectionalBlock
	{
		public const int Index = 509;

		public PresserBlock() : base(207) { }
	}
    public class KibblerBlock : FourDirectionalBlock
	{
		public const int Index = 518;

		public KibblerBlock() : base(208) { }
	}
	public class PresserNNBlock : FourDirectionalBlock
	{
		public const int Index = 519;
		
		public PresserNNBlock() : base(209) { }
	}
	public class SqueezerBlock : FourDirectionalBlock
	{
		public const int Index = 527;
		
		public SqueezerBlock() : base(236) { }
	}
	public class CastMachBlock : FurnaceNBlock
	{
		public new const int Index = 530;

		public CastMachBlock() : base(234) { }
	}
	public class BlastFurnaceBlock : FourDirectionalBlock
	{
		public const int Index = 531;

		public BlastFurnaceBlock() : base(219, 70) { }
	}
	public class CovenBlock : FourDirectionalBlock
	{
		public const int Index = 533;

		public override int GetFaceTextureSlot(int face, int value)
		{
			return face == GetDirection(value) ? 242 : 69;
		}
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			color *= SubsystemPalette.GetColor(environmentData, GetPaintColor(value)) * new Color(255, 153, 18);
			ItemBlock.DrawCubeBlock(primitivesRenderer, value, new Vector3(size), ref matrix, color, color, environmentData);
		}
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometrySubsets geometry, int value, int x, int y, int z)
		{
			Utils.BlockGeometryGenerator.GenerateCubeVertices(this, value, x, y, z, new Color(255, 153, 18) * SubsystemPalette.GetColor(generator, GetPaintColor(value)), Utils.GTV(x, z, geometry).OpaqueSubsetsByFace);
		}
	}
	public class HearthFurnaceBlock : FourDirectionalBlock
	{
		public const int Index = 534;

		public override int GetFaceTextureSlot(int face, int value)
		{
			return face == GetDirection(value) ? 243 : 107;
		}
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometrySubsets geometry, int value, int x, int y, int z)
		{
			Utils.BlockGeometryGenerator.GenerateCubeVertices(this, value, x, y, z, Color.LightGray * SubsystemPalette.GetColor(generator, GetPaintColor(value)), Utils.GTV(x, z, geometry).OpaqueSubsetsByFace);
		}
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			color *= SubsystemPalette.GetColor(environmentData, GetPaintColor(value)) * Color.LightGray;
			ItemBlock.DrawCubeBlock(primitivesRenderer, value, new Vector3(size), ref matrix, color, color, environmentData);
		}
	}
	public class FurnaceNBlock : FourDirectionalBlock
	{
		public const int Index = 506;

		public FurnaceNBlock() : base(191, 107) { }

		public FurnaceNBlock(int front, int back = 107) : base(front, back) { }

		public override int GetEmittedLightAmount(int value)
		{
			return GetHeatLevel(value) * 13;
		}

		public override float GetHeat(int value)
		{
			return GetHeatLevel(value) != 0 ? 0.66f : 0f;
		}

		public static int GetHeatLevel(int value)
		{
			return (Terrain.ExtractData(value) & 8) >> 3;
		}

		public static int SetHeatLevel(int data, int level)
		{
			return (data & -9) | (level & 1) << 3;
		}

		public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
		{
			showDebris = DestructionDebrisScale > 0f;
			dropValues.Add(new BlockDropValue
			{
				Value = Terrain.ReplaceLight(Terrain.ReplaceData(oldValue, SetDirection(SetHeatLevel(Terrain.ExtractData(oldValue), 0), 0)), 0),
				Count = 1
			});
		}
	}

	public class FireBoxBlock : FurnaceNBlock
	{
		public new const int Index = 532;
		public static readonly string[] Names =
		{
			"空气预热炉",
			"退火炉",
			"罩式炉",
			"箱式炉",
			"煤气炉",
			"淬火炉",
			"燃气加热炉",
			"玻璃退火窑",
			"水泥回转窑",
			//"单晶炉",
		};

		public override int GetFaceTextureSlot(int face, int value)
		{
			return face != 4 && face != 5 && face == GetDirection(value) ? 222 + GetHeatLevel(value) : 221;
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
			Utils.BlockGeometryGenerator.GenerateCubeVertices(this, value, x, y, z, Terrain.ExtractData(value) >> 10 != 0 ? color * Color.LightGray : color, Utils.GTV(x, z, geometry).OpaqueSubsetsByFace);
		}
		public override string GetDescription(int value)
		{
			return Utils.Get(Names[Terrain.ExtractData(value) >> 10]);
		}
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			return SubsystemPalette.GetName(subsystemTerrain, GetPaintColor(value), Names[Terrain.ExtractData(value) >> 10]);
		}
	}
	public class EngineBlock : FurnaceNBlock
	{
		public new const int Index = 504;

		public static readonly string[] Names =
		{
			"蒸汽机",
			"斯特林发动机",
			"蒸汽轮机",
			"燃气轮机",
			"内燃机",
			"柴油发动机",
			"核冲程发动机",
		};

		public static readonly string[] Descriptions =
		{
			"早期工业时代的象征，是一项可以解放人力劳动力的伟大发明。",
			"斯特林发动机",
			"蒸汽轮机",
			"燃气轮机",
			"内燃机是一种高效发动机，它通过燃料在气缸内的燃烧膨胀进行做功，进而输出能量。",
			"柴油发动机",
			"一种利用核能加热气体做功的发动机，做功时会输出核污染。"
		};

		public static readonly int[] TSlots =
		{
			// lit unlit
			175<<8|143,
			187<<8|171,
			175<<8|143,
			187<<8|171,
			124<<8|124,
			124<<8|124,
			153<<8|152,
		};

		public override IEnumerable<int> GetCreativeValues()
		{
			var arr = new int[17 * 7];
			for (int i = 0; i < 7; i++)
			{
				arr[i * 17] = BlockIndex | i << 24;
				for (int j = 1; j < 17; j++)
					arr[i * 17 + j] = BlockIndex | SetColor(i << 10, j - 1) << 14;
			}
			return arr;
		}
		public override float GetHeat(int value)
		{
			return base.GetHeat(value) * (Terrain.ExtractData(value) >> 10 != 1 ? 0.7f : 1.2f);
		}
		public override int GetFaceTextureSlot(int face, int value)
		{
			if (face == 4 || face == 5)
				return Terrain.ExtractData(value) >> 10 == 6 ? 137 : 107;
			int direction = GetDirection(value), slot = TSlots[Terrain.ExtractData(value) >> 10];
			if (Terrain.ExtractData(value) >> 10 == 6)
				return face == direction ? GetHeatLevel(value) > 0 ? slot >> 8 : slot & 255 : 141;
			return face == direction ? GetHeatLevel(value) > 0 ? slot >> 8 : slot & 255 : face == CellFace.OppositeFace(direction) ? 107 : 159;
		}
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			return Utils.Get(Names[Terrain.ExtractData(value) >> 10]);
		}
		public override string GetDescription(int value)
		{
			return Utils.Get(Descriptions[Terrain.ExtractData(value) >> 10]);
		}

		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			return new BlockPlacementData
			{
				Value = Index | SetDirection(Terrain.ExtractData(value), Utils.GetDirectionXZ(componentMiner)) << 14,
				CellFace = raycastResult.CellFace
			};
		}

		public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
		{
			showDebris = DestructionDebrisScale > 0f;
			dropValues.Add(new BlockDropValue
			{
				Value = Terrain.ReplaceLight(Terrain.ReplaceData(oldValue, SetDirection(SetHeatLevel(Terrain.ExtractData(oldValue), 0), 0) | Terrain.ExtractData(oldValue) & ~1023), 0),
				Count = 1
			});
		}
	}
}