namespace Game
{
	public class TankBlock : CubeBlock
	{
		public const int Index = 522;

		public override int GetFaceTextureSlot(int face, int value)
		{
			return face == 4 || face == 5 ? 181 : 210;
		}
	}
}