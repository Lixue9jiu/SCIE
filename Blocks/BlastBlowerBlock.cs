using System;
using Engine;

namespace Game
{
	// Token: 0x02000611 RID: 1553
	public class BlastBlowerBlock : CubeBlock
	{
		// Token: 0x0600215B RID: 8539 RVA: 0x0001562E File Offset: 0x0001382E
		public override int GetFaceTextureSlot(int face, int value)
		{
			return 220;
		}

		// Token: 0x0600215C RID: 8540 RVA: 0x000E08D4 File Offset: 0x000DEAD4
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			Vector3 forward = Matrix.CreateFromQuaternion(componentMiner.ComponentCreature.ComponentCreatureModel.EyeRotation).Forward;
			float num = Vector3.Dot(forward, Vector3.UnitZ);
			float num2 = Vector3.Dot(forward, Vector3.UnitX);
			float num3 = Vector3.Dot(forward, -Vector3.UnitZ);
			float x = Vector3.Dot(forward, -Vector3.UnitX);
			float x2 = Vector3.Dot(forward, Vector3.UnitY);
			float x3 = Vector3.Dot(forward, -Vector3.UnitY);
			float num4 = MathUtils.Min(MathUtils.Min(num, num2, num3), MathUtils.Min(x, x2, x3));
			if ((double)num != (double)num4 && (double)num2 != (double)num4)
			{
				double num5 = (double)num3;
				double num6 = (double)num4;
			}
			return new BlockPlacementData
			{
				Value = Terrain.MakeBlockValue(540, 0, 0),
				CellFace = raycastResult.CellFace
			};
		}

		// Token: 0x0600215E RID: 8542 RVA: 0x00015635 File Offset: 0x00013835
		public static int GetPower(int data)
		{
			return data & 99;
		}

		// Token: 0x0600215F RID: 8543 RVA: 0x0001563B File Offset: 0x0001383B
		public static int SetPower(int data, int direction)
		{
			return (data & -100) | (direction & 99);
		}

		// Token: 0x0400196B RID: 6507
		public const int Index = 540;
	}
}
