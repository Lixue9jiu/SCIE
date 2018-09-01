namespace Game
{
	public class DispenBlock : SixDirectionalBlock
	{
		public const int Index = 505;

		public override int GetFaceTextureSlot(int face, int value)
		{
			if (face == GetDirection(Terrain.ExtractData(value)))
			{
				return 59;
			}
			return 186;
		}
	}
}
