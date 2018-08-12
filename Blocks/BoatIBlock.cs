using Engine;
using Engine.Graphics;

namespace Game
{
	public class BoatIBlock : Block
	{
		public const int Index = 513;

		private readonly BlockMesh m_standaloneBlockMesh = new BlockMesh();

		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/BoatItem");
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("BoatI", true).ParentBone);
			BlockMesh standaloneBlockMesh = m_standaloneBlockMesh;
			ReadOnlyList<ModelMeshPart> meshParts = model.FindMesh("BoatI", true).MeshParts;
			standaloneBlockMesh.AppendModelMeshPart(meshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.4f, 0f), false, false, false, false, new Color(96, 96, 96));
			BlockMesh standaloneBlockMesh2 = m_standaloneBlockMesh;
			meshParts = model.FindMesh("BoatI", true).MeshParts;
			standaloneBlockMesh2.AppendModelMeshPart(meshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.4f, 0f), false, true, false, false, new Color(255, 255, 255));
			base.Initialize();
		}

		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometrySubsets geometry, int value, int x, int y, int z)
		{
		}

		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, Color.DarkGray, 1f * size, ref matrix, environmentData);
		}
	}
}
