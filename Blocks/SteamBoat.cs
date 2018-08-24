using Engine;
using Engine.Graphics;

namespace Game
{
	public class SteamBoat : BlockItem
	{
		protected readonly BlockMesh m_standaloneBlockMesh = new BlockMesh();
		protected readonly Texture2D m_texture = ContentManager.Get<Texture2D>("Textures/SteamBoat");

		public SteamBoat()
		{
			DefaultDescription = "SteamBoat allows you to cross large areas of water more safely and quickly as if you have enough fuel, a powerful vehicle during the initial industrial era.";
			var model = ContentManager.Get<Model>("Models/SteamBoat");
			const string Name = "Cylinder_001";
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh(Name, true).ParentBone);
			BlockMesh standaloneBlockMesh = m_standaloneBlockMesh;
			ReadOnlyList<ModelMeshPart> meshParts = model.FindMesh(Name, true).MeshParts;
			standaloneBlockMesh.AppendModelMeshPart(meshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.4f, 0f), false, false, false, false, new Color(255, 255, 255));
			meshParts = model.FindMesh(Name, true).MeshParts;
			standaloneBlockMesh.AppendModelMeshPart(meshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.4f, 0f), false, true, false, false, new Color(255, 255, 255));
		}

		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, m_texture, Color.White, size, ref matrix, environmentData);
		}
		public override Vector3 GetIconViewOffset(int value, DrawBlockEnvironmentData environmentData)
		{
			return new Vector3(-0.7f, 0.8f, -1);
		}
		public override float GetIconViewScale(int value, DrawBlockEnvironmentData environmentData)
		{
			return 0.45f;
		}
	}
}
