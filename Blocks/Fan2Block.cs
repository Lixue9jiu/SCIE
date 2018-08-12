using Engine;
using Engine.Graphics;

namespace Game
{
	public class Fan2Block : FlatBlock
	{
		public const int Index = 539;

		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawFlatBlock(primitivesRenderer, value, size, ref matrix, null, Color.Gray, false, environmentData);
		}
	}
}
