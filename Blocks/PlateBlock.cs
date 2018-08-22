using Engine;
using Engine.Graphics;
using System;

namespace Game
{
        public class SteelPlate : PlateBlock
        {
            public SteelPlate() : base(MetalType.Steel)
            {
            }
        }
        public class IronPlate : PlateBlock
        {
            public IronPlate() : base(MetalType.Iron)
            {
            }
        }
        public class CopperPlate : PlateBlock
        {
            public CopperPlate() : base(MetalType.Copper)
            {
            }
        }
        public class LeadPlate : PlateBlock
        {
            public LeadPlate() : base(MetalType.Lead)
            {
            }
        }
        public class ZincPlate : PlateBlock
        {
            public ZincPlate() : base(MetalType.Zinc)
            {
            }
        }
        public class PlatinumPlate : PlateBlock
        {
            public PlatinumPlate() : base(MetalType.Platinum)
            {
            }
        }
        public class AluminumPlate : PlateBlock
        {
            public AluminumPlate() : base(MetalType.Aluminum)
            {
            }
        }
    public abstract class PlateBlock : Item
    {
        private readonly BlockMesh m_standaloneBlockMesh = new BlockMesh();
        public readonly MetalType Type;
        protected PlateBlock(MetalType type)
		{
            Type = type;
            Model model = ContentManager.Get<Model>("Models/Ingots");
            Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("IronPlate", true).ParentBone);
            m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("IronPlate", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.1f, 0f), false, false, false, false, Color.White);
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
            Aluminum,
            Iron,
            Copper
        }

        public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
        {
            return Type.ToString() + "Plate";
        }
        public override string GetDescription(int value)
        {
            return "An plate of pure " + Type.ToString() + ". Can be crafted into very durable and strong " + Type.ToString() + " items. Very important in the industrial Era.";
        }

        public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
            switch (Type)
            {
                case MetalType.Steel:
                    color = Color.LightGray;
                    break;
                case MetalType.Iron:
                    color = Color.White;
                    break;
                case MetalType.Gold:
                    color = new Color(255, 215, 0);
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
                case MetalType.Copper:
                    color = new Color(255, 127, 80);
                    break;
                default:
                    color = new Color(232, 232, 232);
                    break;
            }
            BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, color, size * 1.5f, ref matrix, environmentData);
		}
		public override float GetIconViewScale(int value, DrawBlockEnvironmentData environmentData)
		{
			return 0.85f;
		}
	}
}
