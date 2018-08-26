namespace Game
{
	public abstract class FourDirectionalBlock : CubeBlock
	{
		public static int GetDirection(int data)
		{
			return data & 7;
		}

		public static int SetDirection(int data, int direction)
		{
			return (data & -8) | (direction & 7);
		}
	}
}
