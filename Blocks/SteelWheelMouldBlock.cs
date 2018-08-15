using System;
using Engine;
using Engine.Graphics;

namespace Game
{
    // Token: 0x0200061F RID: 1567
    public class SteelWheelMouldBlock : CubeBlock
    {
        // Token: 0x060021B4 RID: 8628
        public SteelWheelMouldBlock()
        {
            this.m_standaloneBlockMesh = new BlockMesh();
            this.m_standaloneMesh = new BlockMesh();
        }

        // Token: 0x060021B5 RID: 8629
        public override void Initialize()
        {
            Model model = ContentManager.Get<Model>("Models/WheelMould");
            Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("WheelMould", true).ParentBone);
            BlockMesh blockMesh = new BlockMesh();
            blockMesh.AppendModelMeshPart(model.FindMesh("WheelMould", true).MeshParts[0], boneAbsoluteTransform * 1f * Matrix.CreateTranslation(0f, -0.02f, 0f), false, false, false, false, Color.LightGray);
            blockMesh.TransformTextureCoordinates(Matrix.CreateTranslation(2.6f, 1.4f, 0f) * Matrix.CreateScale(0.05f), -1);
            this.m_standaloneBlockMesh.AppendBlockMesh(blockMesh);
            base.Initialize();
        }

        // Token: 0x060021B6 RID: 8630
        public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
        {
            BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, size * 1.6f, ref matrix, environmentData);
        }

        // Token: 0x060021B7 RID: 8631
        public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometrySubsets geometry, int value, int x, int y, int z)
        {
        }

        // Token: 0x040019B6 RID: 6582
        public const int Index = 549;

        // Token: 0x040019B7 RID: 6583
        private readonly BlockMesh m_standaloneBlockMesh;

        // Token: 0x040019B8 RID: 6584
        private readonly BlockMesh m_standaloneMesh;
    }
}
