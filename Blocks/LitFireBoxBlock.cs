namespace Game
{
	public class LitFireBoxBlock : FireBoxBlock
	{
		public override int GetFaceTextureSlot(int face, int value)
		{
			if (face == 4 || face == 5)
			{
				return 221;
			}
			switch (Terrain.ExtractData(value))
			{
			case 0:
				if (face == 0)
				{
					return 223;
				}
				return 221;
			case 1:
				if (face == 1)
				{
					return 223;
				}
				return 221;
			case 2:
				if (face == 2)
				{
					return 223;
				}
				return 221;
			default:
				if (face == 3)
				{
					return 223;
				}
				return 221;
			}
		}
		public new const int Index = 533;
	}
}
