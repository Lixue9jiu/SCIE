using Engine;

namespace Game
{
	public class CastMachBlock : CubeBlock
	{
		public const int Index = 547;

		private readonly BlockMesh[] m_blockMeshesByData = new BlockMesh[4];

		private readonly BlockMesh m_standaloneBlockMesh = new BlockMesh();

		public override int GetFaceTextureSlot(int face, int value)
		{
			if (face != 4 && face != 5)
			{
				switch (Terrain.ExtractData(value))
				{
				case 0:
					switch (face)
					{
					case 0:
						return 234;
					default:
						return 107;
					case 2:
						return 107;
					}
				case 1:
					switch (face)
					{
					case 1:
						return 234;
					default:
						return 107;
					case 3:
						return 107;
					}
				case 2:
					switch (face)
					{
					case 2:
						return 234;
					default:
						return 107;
					case 0:
						return 107;
					}
				default:
					switch (face)
					{
					case 3:
						return 234;
					default:
						return 107;
					case 1:
						return 107;
					}
				}
			}
			return 107;
		}

		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			Vector3 forward = Matrix.CreateFromQuaternion(componentMiner.ComponentCreature.ComponentCreatureModel.EyeRotation).Forward;
			float num = Vector3.Dot(forward, Vector3.UnitZ);
			float num2 = Vector3.Dot(forward, Vector3.UnitX);
			float num3 = Vector3.Dot(forward, -Vector3.UnitZ);
			float num4 = Vector3.Dot(forward, -Vector3.UnitX);
			int data = 0;
			if ((double)num == (double)MathUtils.Max(num, num2, num3, num4))
			{
				data = 2;
			}
			else if ((double)num2 == (double)MathUtils.Max(num, num2, num3, num4))
			{
				data = 3;
			}
			else if ((double)num3 == (double)MathUtils.Max(num, num2, num3, num4))
			{
				data = 0;
			}
			else if ((double)num4 == (double)MathUtils.Max(num, num2, num3, num4))
			{
				data = 1;
			}
			BlockPlacementData result = default(BlockPlacementData);
			result.Value = Terrain.ReplaceData(Terrain.ReplaceContents(0, 547), data);
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

		public override bool IsFaceTransparent(SubsystemTerrain subsystemTerrain, int face, int value)
		{
			return false;
		}
	}
}