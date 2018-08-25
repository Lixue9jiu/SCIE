namespace Game
{
	public class CReactorBlock : CubeBlock
	{
		public const int Index = 535;

		public override int GetFaceTextureSlot(int face, int value)
		{
			if (face == 4)
			{
				return 192;
			}
            if (face == 5)
			{
				return 107;
			}
			return 107;
		}
	}
}
