using Engine;
using Engine.Graphics;

namespace Game
{
	public class RottenEgg : MeshItem
	{
		public RottenEgg() : base("Rotten egg. Do not eat.")
		{
			DefaultDisplayName = "Rotten Egg";
			DefaultCategory = "Food";
			DefaultTextureSlot = 15;
			ReadOnlyList<ModelMesh> meshes = ContentManager.Get<Model>("Models/RottenEgg").Meshes;
			m_standaloneBlockMesh.AppendModelMeshPart(meshes[0].MeshParts[0], BlockMesh.GetBoneAbsoluteTransform(meshes[0].ParentBone), false, false, false, false, Color.White);
			m_standaloneBlockMesh.TransformTextureCoordinates(Matrix.Identity, -1);
		}
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, color, 2f * size, ref matrix, environmentData);
		}
		public override float GetIconViewScale(int value, DrawBlockEnvironmentData environmentData)
		{
			return 0.85f;
		}
		public override float GetNutritionalValue(int value)
		{
			return 0.1f;
		}
		public override int GetDamageDestructionValue(int value)
		{
			return 246;
		}
	}
}
