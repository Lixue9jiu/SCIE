using Engine;
using Engine.Graphics;
using System;

namespace Game
{
    public class Resistor : FlatItem
	{
		public readonly Color[] Colors = new Color[]
		{
			new Color(55, 55, 55)
		};
		public readonly MetalType Type;
		public Resistor(MetalType type)
		{
			DefaultTextureSlot = 163;
			DefaultDisplayName = type.ToString() + "Resistor";
			Type = type;
		}
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawFlatBlock(primitivesRenderer, value, size, ref matrix, null, new Color(55, 55, 55), false, environmentData);
		}
		public override string GetDescription(int value)
		{
			return Type.ToString() + " Resistor is a kind of Resistor obtained by " + Type.ToString() + ".";
		}
	}
}
