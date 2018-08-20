using Engine;
using Engine.Graphics;

namespace Game
{
	public class CarbonAerogels : MeshItem
	{
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();
		public CarbonAerogels()
		{
			Model model = ContentManager.Get<Model>("Models/Brick");
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Brick", true).ParentBone);
			m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Brick", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.075f, 0f), false, false, false, false, Color.White);
		}
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, color * new Color(28, 28, 28), 2.5f * size, ref matrix, environmentData);
		}
	}
}
