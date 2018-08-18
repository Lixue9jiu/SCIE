using Engine;

namespace Game
{
	public class DispenBlock : CubeBlock
	{
		public const int Index = 505;

		public override int GetFaceTextureSlot(int face, int value)
		{
			int direction = GetDirection(Terrain.ExtractData(value));
			if (face == direction)
			{
				return 59;
			}
			return 186;
		}

		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			Vector3 forward = Matrix.CreateFromQuaternion(componentMiner.ComponentCreature.ComponentCreatureModel.EyeRotation).Forward;
			float num = Vector3.Dot(forward, Vector3.UnitZ);
			float num2 = Vector3.Dot(forward, Vector3.UnitX);
			float num3 = Vector3.Dot(forward, -Vector3.UnitZ);
			float num4 = Vector3.Dot(forward, -Vector3.UnitX);
			float num5 = Vector3.Dot(forward, Vector3.UnitY);
			float num6 = Vector3.Dot(forward, -Vector3.UnitY);
			float num7 = MathUtils.Min(MathUtils.Min(num, num2, num3), MathUtils.Min(num4, num5, num6));
			int direction = 0;
			if (num == num7)
			{
				direction = 0;
			}
			else if (num2 == num7)
			{
				direction = 1;
			}
			else if (num3 == num7)
			{
				direction = 2;
			}
			else if (num4 == num7)
			{
				direction = 3;
			}
			else if (num5 == num7)
			{
				direction = 4;
			}
			else if (num6 == num7)
			{
				direction = 5;
			}
			BlockPlacementData result = default(BlockPlacementData);
			result.Value = Terrain.MakeBlockValue(505, 0, SetDirection(0, direction));
			result.CellFace = raycastResult.CellFace;
			return result;
		}

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
