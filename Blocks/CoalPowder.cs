using Engine;
using Engine.Graphics;

namespace Game
{
	public class CoalPowderBlock : FlatBlock
	{
		public const int Index = 517;

		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawFlatBlock(primitivesRenderer, value, size, ref matrix, null, new Color(28, 28, 28), false, environmentData);
		}
	}
}
