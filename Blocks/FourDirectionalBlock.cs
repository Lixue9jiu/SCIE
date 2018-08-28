using Engine;

namespace Game
{
	public abstract class FourDirectionalBlock : CubeBlock
	{
		//protected readonly BlockMesh[] m_blockMeshesByData = new BlockMesh[4];

		//protected readonly BlockMesh m_standaloneBlockMesh = new BlockMesh();

		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			return new BlockPlacementData
			{
				Value = Terrain.ReplaceData(BlockIndex, Utils.GetDirectionXZ(componentMiner)),
				CellFace = raycastResult.CellFace
			};
		}

		public override bool IsFaceTransparent(SubsystemTerrain subsystemTerrain, int face, int value)
		{
			return false;
		}

		public static int GetDirection(int value)
		{
			return Terrain.ExtractData(value) & 7;
		}

		public static int SetDirection(int data, int direction)
		{
			return (data & -8) | (direction & 7);
		}
	}
	public abstract class SixDirectionalBlock : FourDirectionalBlock
	{
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
			return new BlockPlacementData
			{
				Value = Terrain.ReplaceData(BlockIndex, SetDirection(0, direction)),
				CellFace = raycastResult.CellFace
			};
		}
	}
}
