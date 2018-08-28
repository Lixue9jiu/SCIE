using Engine;
using Engine.Graphics;

namespace Game
{
	public class MetalLine : FlatItem
	{
		public readonly MetalType Type;
		public MetalLine(MetalType type)
		{
			DefaultTextureSlot = 235;
			DefaultDisplayName = type.ToString() + "Line";
			DefaultDescription = Type.ToString() + "Line is made of " + Type.ToString() + " Ingot, it can be used in many place in the industrial era.";
			Type = type;
		}
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawFlatBlock(primitivesRenderer, value, size, ref matrix, null, MetalBlock.GetColor(Type), false, environmentData);
		}
	}
}
