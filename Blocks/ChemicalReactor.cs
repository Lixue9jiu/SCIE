namespace Game
{
	public class CReactorBlock : PaintedCubeBlock
	{
		public const int Index = 524;

		public CReactorBlock() : base(0)
		{
		}

		public override int GetFaceTextureSlot(int face, int value)
		{
			if (face == 4)
			{
				return 192;
			}
			/*if (face == 5)
			{
				return 107;
			}*/
			return 107;
		}
	}
}
