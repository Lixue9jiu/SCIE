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
        Brass,
		Plastic,
	}
	public class MetalBlock : PaintedCubeBlock
	{
		public const int Index = 510;
		public static readonly string[] Names = new[]
		{
			"�����������", "�߼��������",
			"����שǽ"//, "������ǽ"
		};
		public static readonly Color[] Colors = new[]
		{
			Color.White, Color.LightGray, new Color(255, 153, 18)//, Color.LightGray
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
			return type < 3 ? Utils.Get(Names[type]): Utils.Get("��̬") + ((Materials)(type - 3)).ToStr() + Utils.Get("��");
		}
		public override string GetDescription(int value)
		{
			switch (GetType(value))
			{
				case 0: return Utils.Get("һЩ�����Ļ�����ǡ� �ǳ��������á��� �ھ�ͱ�ը���ǳ��п��ԡ� �����ö�������ֶ��Ƴɡ�");
				case 1: return Utils.Get("ĳЩ�������豸�ĸ߼���ǡ� �ǳ��������á� ���ھ�ͱ�ը���зǳ��п��ԡ� �����ö���ְ�������");
				case 2: return Utils.Get("����שǽ����ͨ�����������ש�����һ����ɰ��ճ�϶��Ƴɡ� ����һ�ֶ๦�ܣ���������۵Ĺ�ҵ���ϡ�");
				//case 3: return Utils.Get("����һ�ֶ๦�ܣ���������۵Ĺ�ҵ���ϡ�");
				default:
					var type = (Materials)(GetType(value) - 3);
					return Utils.Get(type == Materials.Steel
						? "�ֿ顣 �ǳ��������á� ���ھ�ͱ�ը���зǳ��õĿ��ԡ� �����ö���ֶ�������"
						: Utils.Get("һ�鴿") + type.ToStr() + Utils.Get("���ɶ��") + type.ToStr() + Utils.Get("���Ƶá�"));
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