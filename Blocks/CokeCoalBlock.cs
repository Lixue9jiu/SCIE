using System;
using Engine;

namespace Game
{
	// Token: 0x02000097 RID: 151
	public class CokeCoalBlock : ChunkBlock
	{
		// Token: 0x060003D3 RID: 979 RVA: 0x000299E0 File Offset: 0x00027BE0
		public CokeCoalBlock() : base(Matrix.CreateRotationX(1f) * Matrix.CreateRotationZ(2f), Matrix.CreateTranslation(0.0625f, 0.4375f, 0f), new Color(175, 175, 175), false)
		{
		}

		// Token: 0x04000254 RID: 596
		public const int Index = 535;
	}
}
