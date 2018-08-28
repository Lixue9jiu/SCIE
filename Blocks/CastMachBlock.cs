using Engine;

namespace Game
{
	public class CastMachBlock : FourDirectionalBlock
	{
		public const int Index = 530;

		public override int GetFaceTextureSlot(int face, int value)
		{
			if (face != 4 && face != 5)
			{
				switch (GetDirection(value))
				{
				case 0:
					switch (face)
					{
					case 0:
						return 234;
					default:
						return 107;
					case 2:
						return 107;
					}
				case 1:
					switch (face)
					{
					case 1:
						return 234;
					default:
						return 107;
					case 3:
						return 107;
					}
				case 2:
					switch (face)
					{
					case 2:
						return 234;
					default:
						return 107;
					case 0:
						return 107;
					}
				default:
					switch (face)
					{
					case 3:
						return 234;
					default:
						return 107;
					case 1:
						return 107;
					}
				}
			}
			return 107;
		}
	}
}
