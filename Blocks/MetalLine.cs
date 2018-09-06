using Engine;
using Engine.Graphics;

namespace Game
{
	public class MetalLine : FlatItem
	{
		public Color Color;
		public MetalLine(MetalType type)
		{
			DefaultTextureSlot = 235;
			DefaultDisplayName = type.ToString() + "Line";
			DefaultDescription = type.ToString() + "Line is made of " + type.ToString() + " Ingot, it can be used in many place in the industrial era.";
			Color = MetalBlock.GetColor(type);
		}
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawFlatBlock(primitivesRenderer, value, size, ref matrix, null, Color, false, environmentData);
		}
	}
}
