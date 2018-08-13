using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	public class RuIngotBlock : CubeBlock
	{
		public RuIngotBlock()
		{
			this.m_standaloneBlockMesh = new BlockMesh();
			this.m_standaloneMesh = new BlockMesh();
		}

		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/Campfire");
			Matrix boneAbsoluteTransform2 = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Ashes", true).ParentBone);
			this.m_standaloneMesh.AppendModelMeshPart(model.FindMesh("Ashes", true).MeshParts[0], boneAbsoluteTransform2 * Matrix.CreateScale(3f) * Matrix.CreateTranslation(0f, 0f, 0f), false, false, true, false, Color.White);
			base.Initialize();
		}

		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneMesh, color, size * 2f, ref matrix, environmentData);
		}

		public const int Index = 546;

		private readonly BlockMesh m_standaloneBlockMesh;

		private readonly BlockMesh m_standaloneMesh;
	}
}
