namespace Game
{
	public class MachineToolBlock : CubeBlock
	{
		public const int Index = 534;

		public override int GetFaceTextureSlot(int face, int value)
		{
			if (face == 4)
			{
				return 146;
			}
			return 107;
		}
	}
}
