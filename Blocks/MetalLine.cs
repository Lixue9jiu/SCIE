using Engine;
using Engine.Graphics;
using System;

namespace Game
{
	public class IronLine : MetalLine
	{
		public IronLine() : base(MetalType.Iron)
		{
		}
	}
	public class CopperLine : MetalLine
	{
		public CopperLine() : base(MetalType.Copper)
		{
		}
	}
	public class SteelLine : MetalLine
	{
		public SteelLine() : base(MetalType.Steel)
		{
		}
	}
    public class GoldLine : MetalLine
    {
        public GoldLine() : base(MetalType.Gold)
        {
        }
    }
    public class SliverLine : MetalLine
    {
        public SliverLine() : base(MetalType.Sliver)
        {
        }
    }
    public class PlatinumLine : MetalLine
    {
        public PlatinumLine() : base(MetalType.Platinum)
        {
        }
    }
    public class LeadLine : MetalLine
    {
        public LeadLine() : base(MetalType.Lead)
        {
        }
    }
    public class StannaryLine : MetalLine
    {
        public StannaryLine() : base(MetalType.Stannary)
        {
        }
    }
    public abstract class MetalLine : FlatItem
	{
		[Serializable]
		public enum MetalType
		{
			Iron,
			Copper,
			Steel,
            Gold,
            Sliver,
            Platinum,
            Lead,
            Stannary
		}
		public readonly MetalType Type;
		protected MetalLine(MetalType type)
		{
			DefaultTextureSlot = 235;
			DefaultDisplayName = type.ToString() + "Line";
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
		public override string GetDescription(int value)
		{
			return Type.ToString() + "Line is made of " + Type.ToString() + " Ingot, it can be used in many place in the industrial era.";
		}
	}
}
