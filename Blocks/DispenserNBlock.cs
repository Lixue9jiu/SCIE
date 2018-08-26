using Engine;

namespace Game
{
	public class DispenserNBlock : SixDirectionalBlock
	{
		public const int Index = 503;

		public override int GetFaceTextureSlot(int face, int value)
		{
			if (face == GetDirection(Terrain.ExtractData(value)))
			{
				return 127;
			}
			return 111;
		}
	}
}
