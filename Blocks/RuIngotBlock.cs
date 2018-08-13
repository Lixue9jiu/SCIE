using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x0200061C RID: 1564
	public class RuIngotBlock : CubeBlock
	{
		// Token: 0x060021A9 RID: 8617
		public RuIngotBlock()
		{
			this.m_standaloneBlockMesh = new BlockMesh();
			this.m_standaloneMesh = new BlockMesh();
		}

		// Token: 0x060021AA RID: 8618
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/Campfire");
			Matrix boneAbsoluteTransform2 = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Ashes", true).ParentBone);
			this.m_standaloneMesh.AppendModelMeshPart(model.FindMesh("Ashes", true).MeshParts[0], boneAbsoluteTransform2 * Matrix.CreateScale(3f) * Matrix.CreateTranslation(0f, 0f, 0f), false, false, true, false, Color.White);
			base.Initialize();
		}

		// Token: 0x060021AB RID: 8619
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneMesh, color, size * 2f, ref matrix, environmentData);
		}

		// Token: 0x040019AD RID: 6573
		public const int Index = 546;

		// Token: 0x040019AE RID: 6574
		private readonly BlockMesh m_standaloneBlockMesh;

		// Token: 0x040019B2 RID: 6578
		private readonly BlockMesh m_standaloneMesh;
	}
}
