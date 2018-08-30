using Engine;
using Engine.Graphics;
using System;

namespace Game
{
    public class OrePowder : FlatItem
	{
		public readonly Color[] Colors = new Color[]
		{
			Color.White,
			new Color(255, 215, 0),
			new Color(212, 212, 212),
			new Color(87, 86, 85),
			new Color(232, 232, 232),
			new Color(64, 224, 205),
			new Color(225, 225, 225),
			new Color(60, 60, 60),
			Color.White,
			new Color(120, 120, 120),
			new Color(199, 97, 20),
			new Color(139, 69, 19),
			new Color(34, 139, 34),
			Color.White,
			new Color(205, 190, 112),
		};
		public readonly Color Color;
		public OrePowder(MetalType type) : this(type.ToString() + "Ore", default(Color))
		{
			Color = Colors[(int)type];
		}
		public OrePowder(string name, Color color)
		{
			Color = color;
			DefaultTextureSlot = 198;
			DefaultDescription = (DefaultDisplayName = name + "Powder") + " is Ore powder obtained by crushing " + name + ".";
		}
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawFlatBlock(primitivesRenderer, value, size, ref matrix, null, Color, false, environmentData);
		}
	}
}
