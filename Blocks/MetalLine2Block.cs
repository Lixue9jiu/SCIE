using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000625 RID: 1573
	public class MetalLine2Block : FlatBlock
	{
		// Token: 0x0600234C RID: 9036
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawFlatBlock(primitivesRenderer, value, size, ref matrix, null, new Color(192, 192, 192), false, environmentData);
		}

		// Token: 0x04001AE1 RID: 6881
		public const int Index = 555;
	}
}
