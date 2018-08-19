using Engine;
using Engine.Graphics;

namespace Game
{
	public class SteelRod : FlatItem
	{
		protected readonly BlockMesh m_standaloneBlockMesh = new BlockMesh();

		public SteelRod()
		{
			DefaultTextureSlot = 227;
			DefaultDescription = "Rods are made by forging steel into shape. They are useful for making many things.";
			Model model = ContentManager.Get<Model>("Models/Rod");
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("SteelRod", true).ParentBone);
			m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("SteelRod", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.5f, 0f), false, false, false, false, Color.White);
		}

		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, Color.LightGray, 2f * size, ref matrix, environmentData);
		}
	}
}
