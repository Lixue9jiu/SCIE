using Engine;
using Engine.Graphics;

namespace Game
{
	public class RottenEgg : MeshItem
	{
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();
		public RottenEgg()
		{
			DefaultDisplayName = "Rotten Egg";
			DefaultCategory = "Food";
			DefaultDescription = "Rotten egg. Do not eat.";
			DefaultTextureSlot = 15;
			Model model = ContentManager.Get<Model>("Models/RottenEgg");
			ReadOnlyList<ModelMesh> meshes = model.Meshes;
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(meshes[0].ParentBone);
			meshes = model.Meshes;
			m_standaloneBlockMesh.AppendModelMeshPart(meshes[0].MeshParts[0], boneAbsoluteTransform, false, false, false, false, Color.White);
			m_standaloneBlockMesh.TransformTextureCoordinates(Matrix.Identity, -1);
		}
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, color, 2f * size, ref matrix, environmentData);
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
