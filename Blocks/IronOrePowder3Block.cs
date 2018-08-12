using Engine;
using Engine.Graphics;

namespace Game
{
	public class IronOrePowder3Block : FlatBlock
	{
		public const int Index = 516;

		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawFlatBlock(primitivesRenderer, value, size, ref matrix, null, new Color(205, 190, 112), false, environmentData);
		}
	}
}
