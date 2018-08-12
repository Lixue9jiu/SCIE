using Engine;
using Engine.Graphics;

namespace Game
{
	public abstract class PlateBlock : Block
	{
		private readonly string m_meshName;

		private readonly BlockMesh m_standaloneBlockMesh = new BlockMesh();

		protected PlateBlock(string meshName)
		{
			m_meshName = meshName;
		}

		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/Ingots");
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh(m_meshName, true).ParentBone);
			m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh(m_meshName, true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.1f, 0f), false, false, false, false, Color.White);
			base.Initialize();
		}

		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometrySubsets geometry, int value, int x, int y, int z)
		{
		}

		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			if (m_meshName == "SteelPlate")
			{
				color = Color.LightGray;
			}
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, color, size * 1.5f, ref matrix, environmentData);
		}
	}
}
