namespace Game
{
	public class FurnaceNBlock : FourDirectionalBlock
	{
		public const int Index = 506;

		public override int GetFaceTextureSlot(int face, int value)
		{
			if (face == GetDirection(Terrain.ExtractData(value)))
			{
				return 191;
			}
			return 107;
		}
		public override int GetEmittedLightAmount(int value)
		{
			return GetHeatLevel(value) != 0 ? 13 : 0;
		}
		public override float GetHeat(int value)
		{
			return GetHeatLevel(value) != 0 ? 1f : 0f;
		}
		public static int GetHeatLevel(int value)
		{
			return (Terrain.ExtractData(value) & 8) >> 3;
		}

		public static int SetHeatLevel(int data, int level)
		{
			return (data & -9) | (level & 1) << 3;
		}
	}
}
