using System;
using Engine;
using Engine.Graphics;

namespace Game
{
    // Token: 0x02000622 RID: 1570
    public class IPistonBlock : CubeBlock
    {
        // Token: 0x0600221F RID: 8735
        public IPistonBlock()
        {
            this.m_standaloneBlockMesh = new BlockMesh();
            this.m_standaloneMesh = new BlockMesh();
        }

        // Token: 0x06002311 RID: 8977
        public override void Initialize()
        {
            Model model = ContentManager.Get<Model>("Models/Piston");
            Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Piston", true).ParentBone);
            BlockMesh blockMesh = new BlockMesh();
            blockMesh.AppendModelMeshPart(model.FindMesh("Piston", true).MeshParts[0], boneAbsoluteTransform * 1.2f * Matrix.CreateTranslation(0f, -0.02f, 0f), false, false, false, false, Color.White);
            blockMesh.TransformTextureCoordinates(Matrix.CreateTranslation(4f, 3.8f, 0f) * Matrix.CreateScale(0.05f), -1);
            this.m_standaloneBlockMesh.AppendBlockMesh(blockMesh);
            base.Initialize();
        }

        // Token: 0x06002312 RID: 8978
        public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
        {
            BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, size * 1.6f, ref matrix, environmentData);
        }

        // Token: 0x06002313 RID: 8979
        public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometrySubsets geometry, int value, int x, int y, int z)
        {
        }

        // Token: 0x04001A05 RID: 6661
        public const int Index = 551;

        // Token: 0x04001AB9 RID: 6841
        private readonly BlockMesh m_standaloneBlockMesh;

        // Token: 0x04001ABA RID: 6842
        private readonly BlockMesh m_standaloneMesh;
    }
}
