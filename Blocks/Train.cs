using Engine;
using Engine.Graphics;

namespace Game
{
	public class Train : MeshItem
	{
		protected readonly Texture2D m_texture = ContentManager.Get<Texture2D>("Textures/Train");
		public Train() : base("A steam locomotive is a type of railway locomotive that produces its pulling power through a steam engine. These locomotives are fueled by burning combustible material usually coal to produce steam in a boiler. The steam moves reciprocating pistons which are mechanically connected to the locomotive's main wheels. Both fuel and water supplies are carried with the locomotive.")
		{
			DefaultDisplayName = "Steam Locomotive";
			var model = ContentManager.Get<Model>("Models/Train");
			const string Name = "Cylinder_003";
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
	}
}