using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	public class SteelGearMouldBlock : CubeBlock
	{
		public SteelGearMouldBlock()
		{
			this.m_standaloneBlockMesh = new BlockMesh();
			this.m_standaloneMesh = new BlockMesh();
		}

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

		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, size * 1f, ref matrix, environmentData);
		}

		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometrySubsets geometry, int value, int x, int y, int z)
		{
		}

		public const int Index = 550;

		private readonly BlockMesh m_standaloneBlockMesh;

		private readonly BlockMesh m_standaloneMesh;
	}
}
