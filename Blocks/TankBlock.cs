using System.Collections.Generic;

namespace Game
{
	public class TankBlock : CubeBlock
	{
		public enum Type
		{
			Tank,
			FractionatingTower,
			Reductor
		}
		static readonly string[] Names = { "������", "������", "��ѹ��" };
		static readonly string[] Description = { "ĳЩ��ѧ���ϵ��������ǳ��������á��ڻ�ѧ��������Ҫ���á�", "����������ͨ������ѧ��������ȵ�һ�����������ﲿ�ֽ��������¶������뻯ѧ������û���ͨ������ʯ�ͻ�ѧ��", "��ѹ����һ�ֿ��Խ���Һ�������ѹ���Ļ���������ʯ�ͻ�ѧ�бز����ٵ���ɲ��֡�" };
		public const int Index = 522;

		public override IEnumerable<int> GetCreativeValues()
		{
			return new[] { Index, Index | 1 << 14, Index | 2 << 14 };
		}

		public override int GetFaceTextureSlot(int face, int value)
		{
			if (GetType(value) == Type.FractionatingTower)
				return face == 4 ? 107 : 112;
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
			showDebris = true;
			dropValues.Add(new BlockDropValue { Value = oldValue, Count = 1 });
		}

		public static Type GetType(int value)
		{
			return (Type)Terrain.ExtractData(value);
		}
	}
}