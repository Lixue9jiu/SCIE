namespace Game
{
	public class DispenBlock : SixDirectionalBlock
	{
		public const int Index = 505;

		public override int GetFaceTextureSlot(int face, int value)
		{
			return face == GetDirection(value) ? 59 : 186;
		}
	}
}
