using Engine;
using Engine.Graphics;
using System;

namespace Game
{
    public class OrePowder : FlatItem
	{
		public readonly MetalType Type;
		public OrePowder(MetalType type)
		{
			DefaultTextureSlot = 198;
			DefaultDisplayName = type.ToString() + "OrePowder";
			Type = type;
		}
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			switch (Type)
			{
				case MetalType.Iron:
					color = new Color(139, 69, 19);
					break;
                case MetalType.Copper:
                    color = new Color(34, 139, 34);
                    break;
                case MetalType.Germanium:
                    color = new Color(205, 190, 112);
                    break;
                case MetalType.Gold:
                    color = new Color(255, 215, 0);
                    break;
                case MetalType.Sliver:
                    color = new Color(212, 212, 212);
                    break;
                case MetalType.Platinum:
                    color = new Color(232, 232, 232);
                    break;
                case MetalType.Lead:
                    color = new Color(87, 86, 85);
                    break;
                case MetalType.Zinc:
                    color = new Color(64, 224, 205);
                    break;
                case MetalType.Stannary:
                    color = new Color(225, 225, 225);
                    break;
                case MetalType.Chromium:
                    color = new Color(60, 60, 60);
                    break;
                case MetalType.Nickel:
                    color = new Color(120, 120, 120);
                    break;
                case MetalType.Aluminum:
                    color = new Color(199, 97, 20);
                    break;
                default:
					color = Color.White;
					break;
			}
			BlocksManager.DrawFlatBlock(primitivesRenderer, value, size, ref matrix, null, color, false, environmentData);
		}
		public override string GetDescription(int value)
		{
			return Type.ToString() + "OrePowder is Ore powder obtained by crushing " + Type.ToString() + ".";
		}
	}
}
