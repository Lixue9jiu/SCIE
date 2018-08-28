using Engine;
using Engine.Graphics;

namespace Game
{
	public class ScrapIron : MeshItem
	{
		public ScrapIron() : base("A chunk of scrap Iron, What can it do?")
		{
			Model model = ContentManager.Get<Model>("Models/Campfire");
			m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Ashes", true).MeshParts[0], BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Ashes", true).ParentBone) * Matrix.CreateScale(3f) * Matrix.CreateTranslation(0f, 0f, 0f), false, false, true, false, Color.White);
		}
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, color, size * 2f, ref matrix, environmentData);
		}
	}
}
