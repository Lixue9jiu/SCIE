using Engine;
using Engine.Graphics;

namespace Game
{
	public class IndustrialPiston : MeshItem
    {
        public IndustrialPiston() : base("An piston made of iron,copper and steel, the neccessary part of many machine.")
        {
			Model model = ContentManager.Get<Model>("Models/Piston");
            Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Piston", true).ParentBone);
            BlockMesh blockMesh = new BlockMesh();
            blockMesh.AppendModelMeshPart(model.FindMesh("Piston", true).MeshParts[0], boneAbsoluteTransform * 1.2f * Matrix.CreateTranslation(0f, -0.02f, 0f), false, false, false, false, Color.White);
            blockMesh.TransformTextureCoordinates(Matrix.CreateTranslation(4f, 3.8f, 0f) * Matrix.CreateScale(0.05f), -1);
            m_standaloneBlockMesh.AppendBlockMesh(blockMesh);
        }
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
        {
            BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, color, size * 1.6f, ref matrix, environmentData);
        }
    }
}
