using Engine;
using Engine.Graphics;

namespace Game
{
	public class SteelGearBlock : CubeBlock
    {
        public SteelGearBlock()
        {
            this.m_standaloneBlockMesh = new BlockMesh();
            this.m_standaloneMesh = new BlockMesh();
        }

        public override void Initialize()
        {
            Model model = ContentManager.Get<Model>("Models/Gear");
            Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Gear", true).ParentBone);
            BlockMesh blockMesh = new BlockMesh();
            blockMesh.AppendModelMeshPart(model.FindMesh("Gear", true).MeshParts[0], boneAbsoluteTransform * 0.7f * Matrix.CreateTranslation(0f, 0f, 0f), false, false, false, false, Color.LightGray);
            blockMesh.TransformTextureCoordinates(Matrix.CreateTranslation(4f, 3.8f, 0f) * Matrix.CreateScale(0.05f), -1);
			m_standaloneBlockMesh.AppendBlockMesh(blockMesh);
            base.Initialize();
        }

        public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
        {
            BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, size * 1f, ref matrix, environmentData);
        }

        public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometrySubsets geometry, int value, int x, int y, int z)
        {
        }

        public const int Index = 548;

        private readonly BlockMesh m_standaloneBlockMesh;

        private readonly BlockMesh m_standaloneMesh;
    }
}
