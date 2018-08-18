using Engine;

namespace Game
{
	public class BlastFurnaceBlock : CubeBlock
	{
		public override bool IsFaceTransparent(SubsystemTerrain subsystemTerrain, int face, int value)
		{
			return false;
		}
		
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			Vector3 forward = Matrix.CreateFromQuaternion(componentMiner.ComponentCreature.ComponentCreatureModel.EyeRotation).Forward;
			float num = Vector3.Dot(forward, Vector3.UnitZ);
			float num2 = Vector3.Dot(forward, Vector3.UnitX);
			float num3 = Vector3.Dot(forward, -Vector3.UnitZ);
			float num4 = Vector3.Dot(forward, -Vector3.UnitX);
			int data = 0;
			if (num == MathUtils.Max(num, num2, num3, num4))
			{
				data = 2;
			}
			else if (num2 == MathUtils.Max(num, num2, num3, num4))
			{
				data = 3;
			}
			else if (num3 == MathUtils.Max(num, num2, num3, num4))
			{
				data = 0;
			}
			else if (num4 == MathUtils.Max(num, num2, num3, num4))
			{
				data = 1;
			}
			return new BlockPlacementData
			{
				Value = Terrain.ReplaceData(Terrain.ReplaceContents(0, 542), data),
				CellFace = raycastResult.CellFace
			};
		}
		
		public override int GetFaceTextureSlot(int face, int value)
		{
			if (face == FurnaceNBlock.GetDirection(Terrain.ExtractData(value)))
			{
				return 219;
			}
			return 70;
		}
		
		public static int GetDirection(int data)
		{
			return data & 7;
		}
		
		public static int SetDirection(int data, int direction)
		{
			return (data & -8) | (direction & 7);
		}
		
		public const int Index = 542;

		private readonly BlockMesh[] m_blockMeshesByData = new BlockMesh[4];

		private readonly BlockMesh m_standaloneBlockMesh = new BlockMesh();
	}
}
