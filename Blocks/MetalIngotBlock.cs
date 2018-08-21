using Engine;
using Engine.Graphics;
using System;

namespace Game
{
    public class SteelIngot : MetalIngotBlock
    {
        public SteelIngot() : base(MetalType.Steel)
        {
        }
    }
    public class GoldIngot : MetalIngotBlock
    {
        public GoldIngot() : base(MetalType.Gold)
        {
        }
    }
    public class SliverIngot : MetalIngotBlock
    {
        public SliverIngot() : base(MetalType.Sliver)
        {
        }
    }
    public class PlatinumIngot : MetalIngotBlock
    {
        public PlatinumIngot() : base(MetalType.Platinum)
        {
        }
    }
    public class LeadIngot : MetalIngotBlock
    {
        public LeadIngot() : base(MetalType.Lead)
        {
        }
    }
    public class ZincIngot : MetalIngotBlock
    {
        public ZincIngot() : base(MetalType.Zinc)
        {
        }
    }
    public class NickelIngot : MetalIngotBlock
    {
        public NickelIngot() : base(MetalType.Nickel)
        {
        }
    }
    public class ChromiumIngot : MetalIngotBlock
    {
        public ChromiumIngot() : base(MetalType.Chronmium)
        {
        }
    }
    public class AluminumIngot : MetalIngotBlock
    {
        public AluminumIngot() : base(MetalType.Aluminum)
        {
        }
    }
    public class StannaryIngot : MetalIngotBlock
    {
        public StannaryIngot() : base(MetalType.Stannary)
        {
        }
    }
    public abstract class MetalIngotBlock : Item
	{

		private BlockMesh m_standaloneBlockMesh = new BlockMesh();

		protected MetalIngotBlock(MetalType type)
		{
            Type = type;
            Model model = ContentManager.Get<Model>("Models/Ingots");
            Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("IronIngot", true).ParentBone);
            m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("IronIngot", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.1f, 0f), false, false, false, false, Color.White);
        }
        [Serializable]
        public enum MetalType
        {
            Steel,
            Gold,
            Sliver,
            Lead,
            Platinum,
            Zinc,
            Stannary,
            Chronmium,
            Titanium,
            Nickel,
            Aluminum
        }
        public override string GetDescription(int value)
        {
           
            return "An ingot of pure " + Type.ToString() + ". Can be crafted into very durable and strong " + Type.ToString() + " items. Very important in the industrial Era.";
            
            
        }
        public readonly MetalType Type;
        public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
        {
            return Type.ToString()+"Ingot";
        }

        public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
            switch (Type)
            {
                case MetalType.Steel:
                    color = Color.LightGray;
                    break;
                case MetalType.Gold:
                    color = new Color(255,215,0);
                    break;
                case MetalType.Lead:
                    color = new Color(88, 87, 86);
                    break;
                case MetalType.Chronmium:
                    color = new Color(58, 57, 56);
                    break;
                case MetalType.Platinum:
                    color = new Color(253, 253, 253);
                    break;
                default:
                    color = new Color(232,232,232);
                    break;
            }
            BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, color, 2f * size, ref matrix, environmentData);
		}
		public override float GetIconViewScale(int value, DrawBlockEnvironmentData environmentData)
		{
			return 0.85f;
		}
	}
}
