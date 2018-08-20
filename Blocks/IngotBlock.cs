using Engine;
using Engine.Graphics;
using System;

namespace Game
{
    public class SteelIngot : MetalIngotBlock
    {
        public SteelIngot() : base(MetalType.SteelIngot)
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
            SteelIngot
        }
        public override string GetDescription(int value)
        {
            switch (Type)
            {
                case MetalType.SteelIngot:
                    return "An ingot of pure steel. Can be crafted into very durable and strong steel tools. Even diamond-edged tools cannot best them. Combine multiple ingots to form an steel block.";
            }
            return string.Empty;
        }
        public readonly MetalType Type;
        public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
        {
            return Type.ToString();
        }

        public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
            switch (Type)
            {
                case MetalType.SteelIngot:
                    color = Color.LightGray;
                    break;
                default:
                    color = Color.White;
                    break;
            }
            BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, color, 2f * size, ref matrix, environmentData);
		}
	}
}
