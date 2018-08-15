using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000620 RID: 1568
	public class SteelGearMouldBlock : CubeBlock
	{
		// Token: 0x06002214 RID: 8724
		public SteelGearMouldBlock()
		{
			this.m_standaloneBlockMesh = new BlockMesh();
			this.m_standaloneMesh = new BlockMesh();
		}

		// Token: 0x06002215 RID: 8725
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/GearMould");
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("GearMould", true).ParentBone);
			BlockMesh blockMesh = new BlockMesh();
			blockMesh.AppendModelMeshPart(model.FindMesh("GearMould", true).MeshParts[0], boneAbsoluteTransform * 1.6f * Matrix.CreateTranslation(0f, -0.02f, 0f), false, false, false, false, Color.LightGray);
			blockMesh.TransformTextureCoordinates(Matrix.CreateTranslation(2.6f, 1.4f, 0f) * Matrix.CreateScale(0.05f), -1);
			this.m_standaloneBlockMesh.AppendBlockMesh(blockMesh);
			base.Initialize();
		}

		// Token: 0x06002216 RID: 8726
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, size * 1f, ref matrix, environmentData);
		}

		// Token: 0x06002217 RID: 8727
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometrySubsets geometry, int value, int x, int y, int z)
		{
		}

		// Token: 0x040019FE RID: 6654
		public const int Index = 550;

		// Token: 0x040019FF RID: 6655
		private readonly BlockMesh m_standaloneBlockMesh;

		// Token: 0x04001A00 RID: 6656
		private readonly BlockMesh m_standaloneMesh;
	}
}
