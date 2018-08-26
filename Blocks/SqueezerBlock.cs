namespace Game
{
	public class SqueezerBlock : FourDirectionalBlock
	{
		public const int Index = 527;

		public override int GetFaceTextureSlot(int face, int value)
		{
			if (face != 4 && face != 5)
			{
				switch (Terrain.ExtractData(value))
				{
				case 0:
					if (face == 0)
					{
						return 236;
					}
					return 107;
				case 1:
					if (face == 1)
					{
						return 236;
					}
					return 107;
				case 2:
					if (face == 2)
					{
						return 236;
					}
					return 107;
				default:
					if (face == 3)
					{
						return 236;
					}
					return 107;
				}
			}
			return 107;
		}
	}
}
