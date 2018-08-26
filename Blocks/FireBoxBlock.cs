namespace Game
{
	public class FireBoxBlock : FourDirectionalBlock
	{
		public override int GetFaceTextureSlot(int face, int value)
		{
			if (face == 4 || face == 5)
			{
				return 221;
			}
			int offset = value;
			switch (Terrain.ExtractData(value))
			{
			case 0:
				if (face == 0)
				{
					return 222;
				}
				return 221;
			case 1:
				if (face == 1)
				{
					return 222;
				}
				return 221;
			case 2:
				if (face == 2)
				{
					return 222;
				}
				return 221;
			default:
				if (face == 3)
				{
					return 222;
				}
				return 221;
			}
		}
		
		public const int Index = 532;
	}
}
