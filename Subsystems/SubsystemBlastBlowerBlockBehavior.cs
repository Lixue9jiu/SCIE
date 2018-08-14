using System;
using Engine;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000616 RID: 1558
	public class SubsystemBlastBlowerBlockBehavior : SubsystemBlockBehavior, IUpdateable
	{
		// Token: 0x17000557 RID: 1367
		// (get) Token: 0x0600217F RID: 8575 RVA: 0x00015757 File Offset: 0x00013957
		public override int[] HandledBlocks
		{
			get
			{
				return new int[]
				{
					540
				};
			}
		}

		// Token: 0x17000558 RID: 1368
		// (get) Token: 0x06002180 RID: 8576 RVA: 0x000034CC File Offset: 0x000016CC
		public int UpdateOrder
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x06002181 RID: 8577 RVA: 0x00015767 File Offset: 0x00013967
		public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemProjectiles = base.Project.FindSubsystem<SubsystemProjectiles>(true);
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
		}

		// Token: 0x06002182 RID: 8578 RVA: 0x0000391B File Offset: 0x00001B1B
		public void Update(float dt)
		{
		}

		// Token: 0x06002183 RID: 8579 RVA: 0x000E1C9C File Offset: 0x000DFE9C
		public override void OnBlockGenerated(int value, int x, int y, int z, bool isLoaded)
		{
			int num = 0;
			int num2 = 0;
			for (int i = -1; i < 2; i++)
			{
				for (int j = -1; j < 2; j++)
				{
					for (int k = -1; k < 2; k++)
					{
						int cellContents = base.SubsystemTerrain.Terrain.GetCellContents(x + i, y + j, z + k);
						if (i * i + j * j + k * k <= 1 && (cellContents == 509 || cellContents == 534))
						{
							num = 1;
						}
						if (i * i + j * j + k * k <= 1 && cellContents == 544)
						{
							num2 = 1;
						}
					}
				}
			}
			if (num == 0 || num2 == 0)
			{
				base.SubsystemTerrain.ChangeCell(x, y, z, 540, true);
				return;
			}
			base.SubsystemTerrain.ChangeCell(x, y, z, 16924, true);
		}

		// Token: 0x06002185 RID: 8581 RVA: 0x0000391B File Offset: 0x00001B1B
		public void Scanner(int x, int y, int z)
		{
		}

		// Token: 0x06002186 RID: 8582 RVA: 0x000E1D64 File Offset: 0x000DFF64
		public override void OnNeighborBlockChanged(int x, int y, int z, int neighborX, int neighborY, int neighborZ)
		{
			int num = 0;
			int num2 = 0;
			for (int i = -1; i < 2; i++)
			{
				for (int j = -1; j < 2; j++)
				{
					for (int k = -1; k < 2; k++)
					{
						int cellContents = base.SubsystemTerrain.Terrain.GetCellContents(x + i, y + j, z + k);
						if (i * i + j * j + k * k <= 1 && (cellContents == 509 || cellContents == 534))
						{
							num = 1;
						}
						if (i * i + j * j + k * k <= 1 && cellContents == 544)
						{
							num2 = 1;
						}
					}
				}
			}
			if (num == 0 || num2 == 0)
			{
				base.SubsystemTerrain.ChangeCell(x, y, z, 540, true);
				return;
			}
			base.SubsystemTerrain.ChangeCell(x, y, z, 16924, true);
		}

		// Token: 0x0400198B RID: 6539
		private readonly Game.Random m_random = new Game.Random();

		// Token: 0x0400198C RID: 6540
		private SubsystemTime m_subsystemTime;

		// Token: 0x0400198D RID: 6541
		private SubsystemProjectiles m_subsystemProjectiles;

		// Token: 0x0400198E RID: 6542
		private SubsystemTerrain m_subsystemTerrain;

		// Token: 0x0400198F RID: 6543
		private Vector3 coordinate;

		// Token: 0x04001990 RID: 6544

		// Token: 0x04001991 RID: 6545
		private bool flag;
	}
}
