using System;
using Engine;

namespace Game
{
	// Token: 0x02000612 RID: 1554
	public class BlastFurnaceBlock : CubeBlock
	{
		// Token: 0x06002160 RID: 8544 RVA: 0x000034CC File Offset: 0x000016CC
		public override bool IsFaceTransparent(SubsystemTerrain subsystemTerrain, int face, int value)
		{
			return false;
		}

		// Token: 0x06002161 RID: 8545 RVA: 0x000E09B0 File Offset: 0x000DEBB0
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
			return new BlockPlacementData
			{
				Value = Terrain.ReplaceData(Terrain.ReplaceContents(0, 542), data),
				CellFace = raycastResult.CellFace
			};
		}

		// Token: 0x06002162 RID: 8546 RVA: 0x000E0A94 File Offset: 0x000DEC94
		public override int GetFaceTextureSlot(int face, int value)
		{
			int direction = FurnaceNBlock.GetDirection(Terrain.ExtractData(value));
			if (face == direction)
			{
				return 219;
			}
			return 70;
		}

		// Token: 0x06002163 RID: 8547 RVA: 0x000091A1 File Offset: 0x000073A1
		public static int GetDirection(int data)
		{
			return data & 7;
		}

		// Token: 0x06002164 RID: 8548 RVA: 0x000091A6 File Offset: 0x000073A6
		public static int SetDirection(int data, int direction)
		{
			return (data & -8) | (direction & 7);
		}

		// Token: 0x0400196C RID: 6508
		public const int Index = 542;

		// Token: 0x0400196D RID: 6509
		private readonly BlockMesh[] m_blockMeshesByData = new BlockMesh[4];

		// Token: 0x0400196E RID: 6510
		private readonly BlockMesh m_standaloneBlockMesh = new BlockMesh();
	}
}
