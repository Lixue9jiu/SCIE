using Engine;
using Engine.Graphics;

namespace Game
{
	public class Drill2Block : FlatBlock
	{
		public const int Index = 536;

		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawFlatBlock(primitivesRenderer, value, size, ref matrix, null, Color.Cyan, false, environmentData);
		}
	}
}
