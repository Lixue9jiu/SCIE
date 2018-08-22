using Engine;
using Engine.Graphics;

namespace Game
{
	public class ScrapIron : MeshItem
	{
		public ScrapIron() : base("An chunk of scrap Iron, What can it do?")
		{
			DefaultTextureSlot = 196;
			DefaultDisplayName = "ScrapIron";
			Model model = ContentManager.Get<Model>("Models/Campfire");
			Matrix boneAbsoluteTransform2 = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Ashes", true).ParentBone);
			m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Ashes", true).MeshParts[0], boneAbsoluteTransform2 * Matrix.CreateScale(3f) * Matrix.CreateTranslation(0f, 0f, 0f), false, false, true, false, Color.White);
		}
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, color, size * 2f, ref matrix, environmentData);
		}
		public override float GetIconViewScale(int value, DrawBlockEnvironmentData environmentData)
		{
			return 0.85f;
		}
	}
}
