namespace Game
{
	public class TankBlock : CubeBlock
	{
		public const int Index = 522;

		public override int GetFaceTextureSlot(int face, int value)
		{
			if (face == 4 || face == 5)
			{
				return 181;
			}
			return 210;
		}
	}
}
