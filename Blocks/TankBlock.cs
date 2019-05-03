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
		static readonly string[] Names = { "空油箱", "分馏塔", "减压器" };
		static readonly string[] Description = { "某些化学材料的容器，非常重且耐用。在化学领域有重要作用。", "分馏塔用于通过将化学化合物加热到一个或多个化合物部分将蒸发的温度来分离化学化合物，该机器通常用于石油化学。", "减压器是一种可以降低液体或气体压力的机器，这是石油化学中必不可少的组成部分。" };
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

		public static Type GetType(int value)
		{
			return (Type)Terrain.ExtractData(value);
		}
	}
}