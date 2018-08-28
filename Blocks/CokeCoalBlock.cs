using System;
using Engine;

namespace Game
{
	// Token: 0x02000097 RID: 151
	public class CokeCoalBlock : ChunkBlock
	{
		// Token: 0x060003D3 RID: 979 RVA: 0x000299E0 File Offset: 0x00027BE0
		public CokeCoalBlock() : base(Matrix.CreateRotationX(1f) * Matrix.CreateRotationZ(2f), Matrix.CreateTranslation(0.875f, 0.1875f, 0f), new Color(215, 215, 215), false)
		{
		}

		// Token: 0x04000254 RID: 596
		public const int Index = 535;
	}
}
