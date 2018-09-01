namespace Game
{
	public class BlastBlowerBlock : CubeBlock
	{
		public override int GetFaceTextureSlot(int face, int value)
		{
			if (face == 4 || face ==5)
			{
				return 107;
			}
			return 241;
		}
		
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			/*Vector3 forward = Matrix.CreateFromQuaternion(componentMiner.ComponentCreature.ComponentCreatureModel.EyeRotation).Forward;
			float num = Vector3.Dot(forward, Vector3.UnitZ);
			float num2 = Vector3.Dot(forward, Vector3.UnitX);
			float num3 = Vector3.Dot(forward, -Vector3.UnitZ);
			float x = Vector3.Dot(forward, -Vector3.UnitX);
			float x2 = Vector3.Dot(forward, Vector3.UnitY);
			float x3 = Vector3.Dot(forward, -Vector3.UnitY);
			float num4 = MathUtils.Min(MathUtils.Min(num, num2, num3), MathUtils.Min(x, x2, x3));
			if (num != num4 && num2 != num4)
			{
				double num5 = num3;
				double num6 = num4;
			}*/
			return new BlockPlacementData
			{
				Value = Terrain.MakeBlockValue(Index),
				CellFace = raycastResult.CellFace
			};
		}
		
		public static int GetPower(int data)
		{
			return data & 99;
		}
		
		public static int SetPower(int data, int direction)
		{
			return (data & -100) | (direction & 99);
		}
		
		public const int Index = 529;
	}
}
