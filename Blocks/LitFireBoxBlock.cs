using System;

namespace Game
{
	// Token: 0x0200061A RID: 1562
	public class LitFireBoxBlock : FireBoxBlock
	{
		// Token: 0x0600219C RID: 8604 RVA: 0x000E24A4 File Offset: 0x000E06A4
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
					return 223;
				}
				return 221;
			case 1:
				if (face == 1)
				{
					return 223;
				}
				return 221;
			case 2:
				if (face == 2)
				{
					return 223;
				}
				return 221;
			default:
				if (face == 3)
				{
					return 223;
				}
				return 221;
			}
		}

		// Token: 0x040019AA RID: 6570
		public new const int Index = 544;
	}
}
