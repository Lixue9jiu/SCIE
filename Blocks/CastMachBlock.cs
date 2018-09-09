namespace Game
{
	public class CastMachBlock : FurnaceNBlock
	{
		public new const int Index = 530;

		public override int GetFaceTextureSlot(int face, int value)
		{
			if (face != 4 && face != 5)
			{
				switch (GetDirection(value))
				{
					case 0:
						if (face == 0)
						{
							return 234;
						}
						return 107;
					case 1:
						if (face == 1)
						{
							return 234;
						}
						return 107;
					case 2:
						if (face == 2)
						{
							return 234;
						}
						return 107;
					default:
						if (face == 3)
						{
							return 234;
						}
						return 107;
				}
			}
			return 107;
		}
	}
}
