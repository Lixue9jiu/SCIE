namespace Game
{
	public class DispenserNBlock : SixDirectionalBlock
	{
		public const int Index = 503;

		public override int GetFaceTextureSlot(int face, int value)
		{
			return face == GetDirection(value) ? 127 : 111;
		}
	}
}
