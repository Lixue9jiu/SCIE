namespace Game
{
	public class BlastFurnaceBlock : FourDirectionalBlock
	{
		public override int GetFaceTextureSlot(int face, int value)
		{
			if (face == GetDirection(Terrain.ExtractData(value)))
			{
				return 219;
			}
			return 70;
		}
		
		public const int Index = 531;
	}
}
