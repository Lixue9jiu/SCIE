using System;
using Engine;

namespace Game
{
	// Token: 0x02000617 RID: 1559
	public class FireBoxBlock : CubeBlock
	{
		// Token: 0x06002187 RID: 8583 RVA: 0x000E1E28 File Offset: 0x000E0028
		public override int GetFaceTextureSlot(int face, int value)
		{
			if (face == 4 || face == 5)
			{
				return 221;
			}
			switch (Terrain.ExtractData(value))
			{
			case 0:
				if (face == 0)
				{
					return 222;
				}
				return 221;
			case 1:
				if (face == 1)
				{
					return 222;
				}
				return 221;
			case 2:
				if (face == 2)
				{
					return 222;
				}
				return 221;
			default:
				if (face == 3)
				{
					return 222;
				}
				return 221;
			}
		}

		// Token: 0x06002188 RID: 8584 RVA: 0x000E1EAC File Offset: 0x000E00AC
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
				Value = Terrain.ReplaceData(Terrain.ReplaceContents(0, 543), data),
				CellFace = raycastResult.CellFace
			};
		}

		// Token: 0x06002189 RID: 8585 RVA: 0x000091A1 File Offset: 0x000073A1
		public static int GetDirection(int data)
		{
			return data & 7;
		}

		// Token: 0x0600218A RID: 8586 RVA: 0x000091A6 File Offset: 0x000073A6
		public static int SetDirection(int data, int direction)
		{
			return (data & -8) | (direction & 7);
		}

		// Token: 0x0600218B RID: 8587 RVA: 0x000034CC File Offset: 0x000016CC
		public override bool IsFaceTransparent(SubsystemTerrain subsystemTerrain, int face, int value)
		{
			return false;
		}

		// Token: 0x04001992 RID: 6546
		public const int Index = 543;

		// Token: 0x04001993 RID: 6547
		private readonly BlockMesh[] m_blockMeshesByData = new BlockMesh[4];

		// Token: 0x04001994 RID: 6548
		private readonly BlockMesh m_standaloneBlockMesh = new BlockMesh();
	}
}
