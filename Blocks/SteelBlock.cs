namespace Game
{
	public class SteelBlock : CubeBlock
	{
		public const int Index = 510;

		public override int GetFaceTextureSlot(int face, int value)
		{
			return 107;
		}
	}
}
