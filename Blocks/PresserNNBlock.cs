namespace Game
{
	public class PresserNNBlock : FourDirectionalBlock
	{
		public const int Index = 519;

		public override int GetFaceTextureSlot(int face, int value)
		{
			if (face != 4 && face != 5)
			{
				switch (Terrain.ExtractData(value))
				{
				case 0:
					if (face == 0)
					{
						return 209;
					}
					return 107;
				case 1:
					if (face == 1)
					{
						return 209;
					}
					return 107;
				case 2:
					if (face == 2)
					{
						return 209;
					}
					return 107;
				default:
					if (face == 3)
					{
						return 209;
					}
					return 107;
				}
			}
			return 107;
		}
	}
}
