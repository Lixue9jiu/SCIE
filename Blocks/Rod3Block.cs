using Engine;
using Engine.Graphics;

namespace Game
{
	public class Rod3Block : Block
	{
		public const int Index = 522;

		private readonly BlockMesh m_standaloneBlockMesh = new BlockMesh();

		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/Rod");
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("SteelRod", true).ParentBone);
			m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("SteelRod", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.5f, 0f), false, false, false, false, Color.White);
			base.Initialize();
		}

		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometrySubsets geometry, int value, int x, int y, int z)
		{
		}

		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, Color.Gray, 2f * size, ref matrix, environmentData);
		}
	}
}
