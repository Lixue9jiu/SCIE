using System;
using Engine;
using Engine.Graphics;

namespace Game
{
    // Token: 0x0200061E RID: 1566
    public class SteelGearBlock : CubeBlock
    {
        // Token: 0x060021B0 RID: 8624
        public SteelGearBlock()
        {
            this.m_standaloneBlockMesh = new BlockMesh();
            this.m_standaloneMesh = new BlockMesh();
        }

        // Token: 0x060021B1 RID: 8625
        public override void Initialize()
        {
            Model model = ContentManager.Get<Model>("Models/Gear");
            Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Gear", true).ParentBone);
            BlockMesh blockMesh = new BlockMesh();
            blockMesh.AppendModelMeshPart(model.FindMesh("Gear", true).MeshParts[0], boneAbsoluteTransform * 0.7f * Matrix.CreateTranslation(0f, 0f, 0f), false, false, false, false, Color.LightGray);
            blockMesh.TransformTextureCoordinates(Matrix.CreateTranslation(4f, 3.8f, 0f) * Matrix.CreateScale(0.05f), -1);
            this.m_standaloneBlockMesh.AppendBlockMesh(blockMesh);
            base.Initialize();
        }

        // Token: 0x060021B2 RID: 8626
        public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
        {
            BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, size * 1f, ref matrix, environmentData);
        }

        // Token: 0x060021B3 RID: 8627
        public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometrySubsets geometry, int value, int x, int y, int z)
        {
        }

        // Token: 0x040019B3 RID: 6579
        public const int Index = 548;

        // Token: 0x040019B4 RID: 6580
        private readonly BlockMesh m_standaloneBlockMesh;

        // Token: 0x040019B5 RID: 6581
        private readonly BlockMesh m_standaloneMesh;
    }
}
