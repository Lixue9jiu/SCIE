using Engine;
using Engine.Graphics;

namespace Game
{
	public class ScrapIron : MeshItem
	{
		public ScrapIron() : base("一大块废铁，它能做什么？")
		{
			var model = ContentManager.Get<Model>("Models/Campfire");
			m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Ashes", true).MeshParts[0], BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Ashes", true).ParentBone) * Matrix.CreateScale(3f) * Matrix.CreateTranslation(Vector3.Zero), false, false, true, false, Color.White);
		}
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, color, size * 2f, ref matrix, environmentData);
		}
	}

	public class RottenEgg : MeshItem
	{
		public RottenEgg() : base(Utils.Get("腐蛋，不能吃。"))
		{
			DefaultDisplayName = "Rotten Egg";
			var meshes = ContentManager.Get<Model>("Models/RottenEgg").Meshes;
			m_standaloneBlockMesh.AppendModelMeshPart(meshes[0].MeshParts[0], BlockMesh.GetBoneAbsoluteTransform(meshes[0].ParentBone), false, false, false, false, Color.White);
		}
		public override int GetFaceTextureSlot(int face, int value)
		{
			return 15;
		}
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, color, 2f * size, ref matrix, environmentData);
		}
		public override float GetIconViewScale(int value, DrawBlockEnvironmentData environmentData)
		{
			return 0.85f;
		}
		public override string GetCategory(int value)
		{
			return "Food";
		}
		public override float GetNutritionalValue(int value)
		{
			return 0.1f;
		}
		public override float GetSicknessProbability(int value)
		{
			return 0.75f;
		}
		public override int GetDamageDestructionValue(int value)
		{
			return 246;
		}
	}
}