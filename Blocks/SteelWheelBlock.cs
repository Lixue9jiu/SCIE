using System;
using Engine;
using Engine.Graphics;

namespace Game
{
    // Token: 0x0200061D RID: 1565
    public class SteelWheelBlock : CubeBlock
    {
        // Token: 0x060021AC RID: 8620
        public SteelWheelBlock()
        {
            this.m_standaloneBlockMesh = new BlockMesh();
            this.m_standaloneMesh = new BlockMesh();
        }

        // Token: 0x060021AD RID: 8621
        public override void Initialize()
        {
            Model model = ContentManager.Get<Model>("Models/Wheel");
            Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Wheel", true).ParentBone);
            BlockMesh blockMesh = new BlockMesh();
            blockMesh.AppendModelMeshPart(model.FindMesh("Wheel", true).MeshParts[0], boneAbsoluteTransform * 1.2f * Matrix.CreateTranslation(0f, 0f, 0f), false, false, false, false, Color.LightGray);
            blockMesh.TransformTextureCoordinates(Matrix.CreateTranslation(4f, 3.8f, 0f) * Matrix.CreateScale(0.05f), -1);
            this.m_standaloneBlockMesh.AppendBlockMesh(blockMesh);
            base.Initialize();
        }

        // Token: 0x060021AE RID: 8622
        public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
        {
            BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, size * 2f, ref matrix, environmentData);
        }

        // Token: 0x060021AF RID: 8623
        public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometrySubsets geometry, int value, int x, int y, int z)
        {
        }

        // Token: 0x040019B0 RID: 6576
        public const int Index = 552;

        // Token: 0x040019B1 RID: 6577
        private readonly BlockMesh m_standaloneBlockMesh;

        // Token: 0x040019B2 RID: 6578
        private readonly BlockMesh m_standaloneMesh;
    }
}
