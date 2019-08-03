using Engine;
using Engine.Graphics;
using System.Collections.Generic;

namespace Game
{
	public enum Materials
	{
		Steel,
		Gold,
		Silver,
		Lead,
		Platinum,
		Zinc,
		Stannary,
		Chromium,
		Titanium,
		Nickel,
		Aluminum,
		Uranium,
		Phosphorus,
		Iron,
		Copper,
		Mercury,
		Germanium,
		FeAlCrAlloy,
		Plastic,
        Brass,
	}
	public class MetalBlock : PaintedCubeBlock
	{
		public const int Index = 510;
		public static readonly string[] Names = new[]
		{
			"基础机器外壳", "高级机器外壳",
			"防火砖墙"
		};
		public static readonly Color[] Colors = new[]
		{
			Color.White, Color.LightGray, new Color(255, 153, 18)
		};
		public MetalBlock() : base(0) { }
		public override IEnumerable<int> GetCreativeValues()
		{
			var arr = new int[15 * (16 + 1)];
			int i;
			for (i = 0; i < 15; i++)
				arr[i] = Terrain.ReplaceData(Index, i << 5);
			for (i = 0; i < 15 * 16; i++)
				arr[i + 15] = Terrain.ReplaceData(Index, i << 1 | 1);
			return arr;
		}
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			int type = GetType(value);
			color = (type < 3 ? Colors[type] : GetColor((Materials)(type - 3))) * SubsystemPalette.GetColor(environmentData, GetPaintColor(value));
			ItemBlock.DrawCubeBlock(primitivesRenderer, value, new Vector3(size), ref matrix, color, color, environmentData);
		}

		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			return new BlockPlacementData { Value = value, CellFace = raycastResult.CellFace };
		}
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometrySubsets geometry, int value, int x, int y, int z)
		{
			int type = GetType(value);
			Utils.BlockGeometryGenerator.GenerateCubeVertices(this, value, x, y, z, (type < 3 ? Colors[type] : GetColor((Materials)(type - 3))) * SubsystemPalette.GetColor(generator, GetPaintColor(value)), Utils.GTV(x, z, geometry).OpaqueSubsetsByFace);
		}
		public static int GetType(int value)
		{
			return Terrain.ExtractData(value) >> 5 & 0xF;
		}
		/*public static int SetType(int value, Type type)
		{
			return Terrain.ReplaceData(value, (Terrain.ExtractData(value) & -481) | ((int)type & 0xF) << 5);
		}*/
		public static Color GetColor(Materials type)
		{
			switch (type)
			{
				case Materials.Steel: return Color.LightGray;
				case Materials.Gold: return new Color(255, 215, 0);
				case Materials.Lead: return new Color(88, 87, 86);
				case Materials.Platinum: return new Color(253, 253, 253);
				case Materials.Chromium: return new Color(58, 57, 56);
				case Materials.Iron: return Color.White;
				case Materials.Copper: return new Color(255, 127, 80);
				case Materials.FeAlCrAlloy: return new Color(200, 200, 200);
                case Materials.Brass: return new Color(255, 228, 196);
            }
			return new Color(232, 232, 232);
		}
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			int type = GetType(value);
			return type < 3 ? Utils.Get(Names[type]): Utils.Get("固态") + ((Materials)(type - 3)).ToStr() + Utils.Get("块");
		}
		public override string GetDescription(int value)
		{
			switch (GetType(value))
			{
				case 0: return Utils.Get("一些机器的基础外壳。 非常重且耐用。对 挖掘和爆炸都非常有抗性。 可以用多个铁板或钢锭制成。");
				case 1: return Utils.Get("某些机器或设备的高级外壳。 非常重且耐用。 对挖掘和爆炸都有非常有抗性。 可以用多个钢板制作。");
				case 2: return Utils.Get("防火砖墙可以通过将几块防火砖组合在一起并用砂浆粘合而制成。 它是一种多功能，坚固且美观的工业材料。");
				default:
					var type = (Materials)(GetType(value) - 3);
					return Utils.Get(type == Materials.Steel
						? "钢块。 非常重且耐用。 对挖掘和爆炸都有非常好的抗性。 可以用多个钢锭制作。"
						: Utils.Get("一块纯") + type.ToStr() + Utils.Get("能由多块") + type.ToStr() + Utils.Get("锭制得。"));
			}
		}
		public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
		{
			showDebris = true;
			dropValues.Add(new BlockDropValue { Value = oldValue, Count = 1 });
		}
		public override int GetFaceTextureSlot(int face, int value)
		{
			switch (GetType(value))
			{
				case 0:
				case 1: return 107;
				case 2: return 69;
			}
			return 180;
		}
	}
}