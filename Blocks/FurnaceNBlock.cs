namespace Game
{
	public class FurnaceNBlock : FourDirectionalBlock
	{
		public const int Index = 506;

		public override int GetFaceTextureSlot(int face, int value)
		{
			if (face == GetDirection(Terrain.ExtractData(value)))
			{
				return 191;
			}
			return 107;
		}
	}
}
