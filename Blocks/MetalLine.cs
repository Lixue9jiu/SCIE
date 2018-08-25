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
			switch (Type)
			{
				case MetalType.Copper:
					color = new Color(255, 127, 80);
					break;
				case MetalType.Steel:
					color = new Color(192, 192, 192);
					break;
                case MetalType.Gold:
                    color = new Color(255, 215, 0);
                    break;
                case MetalType.Lead:
                    color = new Color(88, 87, 86);
                    break;
                case MetalType.Platinum:
                    color = new Color(253, 253, 253);
                    break;
                default:
					color = Color.White;
					break;
			}
			BlocksManager.DrawFlatBlock(primitivesRenderer, value, size, ref matrix, null, color, false, environmentData);
		}
	}
}
