using Engine;
using Engine.Graphics;

namespace Game
{
	public class IronOrePowderBlock : FlatBlock
	{
		public const int Index = 514;

		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawFlatBlock(primitivesRenderer, value, size, ref matrix, null, new Color(139, 69, 19), false, environmentData);
		}
	}
}
