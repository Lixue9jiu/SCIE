using Engine;
using Engine.Graphics;

namespace Game
{
	public class RuIngotBlock : FlatItem
	{
		public RuIngotBlock()
		{
			DefaultTextureSlot = 196;
			DefaultDisplayName = "ScrapIron";
			DefaultDescription = "An chunk of scrap Iron, What can it do?";
			Model model = ContentManager.Get<Model>("Models/Campfire");
			Matrix boneAbsoluteTransform2 = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Ashes", true).ParentBone);
			m_standaloneMesh.AppendModelMeshPart(model.FindMesh("Ashes", true).MeshParts[0], boneAbsoluteTransform2 * Matrix.CreateScale(3f) * Matrix.CreateTranslation(0f, 0f, 0f), false, false, true, false, Color.White);
		}

		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneMesh, color, size * 2f, ref matrix, environmentData);
		}

		protected readonly BlockMesh m_standaloneBlockMesh = new BlockMesh();

		protected readonly BlockMesh m_standaloneMesh = new BlockMesh();
	}
}
