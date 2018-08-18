using Engine;
using Engine.Graphics;
using System;

namespace Game
{
	public class GoldOreChunk : OreChunkBlock
	{
		public GoldOreChunk() : base(Matrix.CreateRotationX(4f) * Matrix.CreateRotationZ(2f), Matrix.CreateTranslation(0.9375f, 0.1875f, 0f), new Color(255,215,0), false,ChunkType.GoldOreChunk)
		{
		}
	}
	public class SliverOreChunk : OreChunkBlock
	{
		public SliverOreChunk() : base(Matrix.CreateRotationX(5f) * Matrix.CreateRotationZ(2f), Matrix.CreateTranslation(0.9375f, 0.1875f, 0f), new Color(192,192,192), false,ChunkType.SliverOreChunk)
		{
		}
	}
        public class PlatinumOreChunk : OreChunkBlock
	{
		public PlatinumOreChunk() : base(Matrix.CreateRotationX(6f) * Matrix.CreateRotationZ(2f), Matrix.CreateTranslation(0.9375f, 0.1875f, 0f), new Color(172,172,172), false,ChunkType.PlatinumOreChunk)
		{
		}
	}
	public abstract class OreChunkBlock : Item
	{
		[Serializable]
		public enum ChunkType
		{
			// Token: 0x040019C9 RID: 6601
			GoldOreChunk,
			// Token: 0x040019CA RID: 6602
			SliverOreChunk,
			// Token: 0x040019CB RID: 6603
			PlatinumOreChunk
			// Token: 0x040019CC RID: 6604
		}
		public readonly ChunkType Type;
		private readonly Color m_color;

		// Token: 0x04000216 RID: 534
		private readonly bool m_smooth;

		// Token: 0x04000217 RID: 535
		private readonly BlockMesh m_standaloneBlockMesh = new BlockMesh();

		// Token: 0x04000218 RID: 536
		private readonly Matrix m_tcTransform;

		// Token: 0x04000219 RID: 537
		private readonly Matrix m_transform;

        protected OreChunkBlock(Matrix transform, Matrix tcTransform, Color color, bool smooth,ChunkType type)
		{
            Type = type;
			this.m_transform = transform;
			this.m_tcTransform = tcTransform;
			this.m_color = color;
			this.m_smooth = smooth;
            Model model = this.m_smooth ? ContentManager.Get<Model>("Models/ChunkSmooth") : ContentManager.Get<Model>("Models/Chunk");
            Matrix matrix = BlockMesh.GetBoneAbsoluteTransform(model.Meshes[0].ParentBone) * this.m_transform;
            BlockMesh standaloneBlockMesh = this.m_standaloneBlockMesh;
            ModelMeshPart meshPart = model.Meshes[0].MeshParts[0];
            Matrix matrix2 = matrix;
            int num = 0;
            int num2 = 0;
            int num3 = 0;
            int num4 = 0;
            Color color2 = this.m_color;
            standaloneBlockMesh.AppendModelMeshPart(meshPart, matrix2, num != 0, num2 != 0, num3 != 0, num4 != 0, color2);
            this.m_standaloneBlockMesh.TransformTextureCoordinates(this.m_tcTransform, -1);
        }

        // Token: 0x0600038D RID: 909 RVA: 0x00028814 File Offset: 0x00026A14
        public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
        {
            BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, 2f * size, ref matrix, environmentData);
        }



        public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			return Type.ToString();
		}
		public override string GetDescription(int value)
		{
			switch (Type)
			{
				case ChunkType.GoldOreChunk:
					return "IronLine is made of Iron Ingot, it can be used in many place in the industrial era like heating wire.";
				case ChunkType.SliverOreChunk:
					return "CopperLine is made of Copper Ingot, it can be used in many place in the industrial era like electric wire.";
				case ChunkType.PlatinumOreChunk:
					return "SteelLine is made of Steel Ingot, it can be used in many place in the industrial era.";
			}
			return string.Empty;
		}
		public override Vector3 GetIconViewOffset(int value, DrawBlockEnvironmentData environmentData)
		{
			return new Vector3
			{
				Z = 1
			};
		}
	}
}
