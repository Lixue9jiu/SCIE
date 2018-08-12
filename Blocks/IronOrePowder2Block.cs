using Engine;
using Engine.Graphics;

namespace Game
{
	public class IronOrePowder2Block : FlatBlock
	{
		public const int Index = 515;

		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawFlatBlock(primitivesRenderer, value, size, ref matrix, null, new Color(34, 139, 34), false, environmentData);
		}
	}
}
