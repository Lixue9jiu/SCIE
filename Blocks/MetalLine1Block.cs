using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000624 RID: 1572
	public class MetalLine1Block : FlatBlock
	{
		// Token: 0x06002348 RID: 9032
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawFlatBlock(primitivesRenderer, value, size, ref matrix, null, new Color(255, 127, 80), false, environmentData);
		}

		// Token: 0x04001AE0 RID: 6880
		public const int Index = 554;
	}
}
