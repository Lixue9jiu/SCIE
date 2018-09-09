using Engine;
using Engine.Graphics;

namespace Game
{
	public class MetalIngot : BlockItem
	{
		protected BlockMesh m_standaloneBlockMesh = new BlockMesh();
		public readonly MetalType Type;

		public MetalIngot(MetalType type)
		{
			Type = type;
			DefaultDisplayName = type.ToString() + "Ingot";
			DefaultDescription = "An ingot of pure " + type.ToString() + ". Can be crafted into very durable and strong " + type.ToString() + " items. Very important in the industrial Era.";
			Model model = ContentManager.Get<Model>("Models/Ingots");
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("IronIngot", true).ParentBone);
			m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("IronIngot", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.1f, 0f), false, false, false, false, Color.White);
		}
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, MetalBlock.GetColor(Type), 2f * size, ref matrix, environmentData);
		}
		public override float GetIconViewScale(int value, DrawBlockEnvironmentData environmentData)
		{
			return 0.85f;
		}
	}
}
