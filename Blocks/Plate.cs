using Engine;
using Engine.Graphics;
using System;

namespace Game
{
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
		Chromium,
		Titanium,
		Nickel,
		Aluminum,
		Iron,
		Copper,
		Mercury,
		Germanium
	}
	public class Sheet : Plate
	{
		public Sheet(MetalType type) : base(type)
		{
			DefaultDisplayName = Type.ToString() + "Sheet";
			DefaultDescription = "A sheet of pure " + Type.ToString() + ". Can be crafted into very durable and strong " + Type.ToString() + " items. Very important in the industrial Era.";
		}
		public override float GetIconViewScale(int value, DrawBlockEnvironmentData environmentData)
		{
			return 0.5f;
		}
	}
	public class Plate : BlockItem
    {
		protected readonly BlockMesh m_standaloneBlockMesh = new BlockMesh();
        public readonly MetalType Type;
        public Plate(MetalType type)
		{
            Type = type;
			DefaultDisplayName = Type.ToString() + "Plate";
			DefaultDescription = "A plate of pure " + Type.ToString() + ". Can be crafted into very durable and strong " + Type.ToString() + " items. Very important in the industrial Era.";
            Model model = ContentManager.Get<Model>("Models/Ingots");
            Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("IronPlate", true).ParentBone);
            m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("IronPlate", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.1f, 0f), false, false, false, false, Color.White);
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
                case MetalType.Chromium:
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
