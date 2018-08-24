using Engine;
using Engine.Graphics;
using System;

namespace Game
{
	public class GoldOreChunk : OreChunk
	{
		public GoldOreChunk() : base(Matrix.CreateRotationX(4f) * Matrix.CreateRotationZ(2f), Matrix.CreateTranslation(0.9375f, 0.1875f, 0f), new Color(255,215,0), false,ChunkType.GoldOreChunk)
		{
		}
	}
	public class SliverOreChunk : OreChunk
	{
		public SliverOreChunk() : base(Matrix.CreateRotationX(5f) * Matrix.CreateRotationZ(2f), Matrix.CreateTranslation(0.9375f, 0.1875f, 0f), new Color(212,212,212), false,ChunkType.SliverOreChunk)
		{
		}
	}
		public class PlatinumOreChunk : OreChunk
	{
		public PlatinumOreChunk() : base(Matrix.CreateRotationX(6f) * Matrix.CreateRotationZ(2f), Matrix.CreateTranslation(0.9375f, 0.1875f, 0f), new Color(232,232,232), false,ChunkType.PlatinumOreChunk)
		{
		}
	}
	public class LeadOreChunk : OreChunk
	{
		public LeadOreChunk() : base(Matrix.CreateRotationX(1f) * Matrix.CreateRotationZ(3f), Matrix.CreateTranslation(0.9375f, 0.1875f, 0f), new Color(88, 87, 86), false, ChunkType.LeadOreChunk)
		{
		}
	}
	public class ZincOreChunk : OreChunk
	{
		public ZincOreChunk() : base(Matrix.CreateRotationX(1f) * Matrix.CreateRotationZ(4f), Matrix.CreateTranslation(0.9375f, 0.1875f, 0f), new Color(65, 224, 205), false, ChunkType.ZincOreChunk)
		{
		}
	}
	public class StannaryChunk : OreChunk
	{
		public StannaryChunk() : base(Matrix.CreateRotationX(1f) * Matrix.CreateRotationZ(5f), Matrix.CreateTranslation(0.9375f, 0.1875f, 0f), new Color(225, 225, 225), false, ChunkType.StannaryChunk)
		{
		}
	}
	public class MercuryOreChunk : OreChunk
	{
		public MercuryOreChunk() : base(Matrix.CreateRotationX(1f) * Matrix.CreateRotationZ(6f), Matrix.CreateTranslation(0.9375f, 0.1875f, 0f), new Color(227, 23, 13), false, ChunkType.MercuryOreChunk)
		{
		}
	}
	public class ChromiumOreChunk : OreChunk
	{
		public ChromiumOreChunk() : base(Matrix.CreateRotationX(1f) * Matrix.CreateRotationZ(7f), Matrix.CreateTranslation(0.9375f, 0.1875f, 0f), new Color(90, 90, 90), false, ChunkType.ChromiumOreChunk)
		{
		}
	}
	public class TitaniumOreChunk : OreChunk
	{
		public TitaniumOreChunk() : base(Matrix.CreateRotationX(2f) * Matrix.CreateRotationZ(-1f), Matrix.CreateTranslation(0.9375f, 0.1875f, 0f), new Color(190, 190, 190), false, ChunkType.TitaniumOreChunk)
		{
		}
	}
	public class NickelOreChunk : OreChunk
	{
		public NickelOreChunk() : base(Matrix.CreateRotationX(3f) * Matrix.CreateRotationZ(-1f), Matrix.CreateTranslation(0.9375f, 0.1875f, 0f), new Color(120, 120, 120), false, ChunkType.NickelOreChunk)
		{
		}
	}
	public abstract class OreChunk : Item
	{
		[Serializable]
		public enum ChunkType
		{
			GoldOreChunk,
			SliverOreChunk,
			PlatinumOreChunk,
			LeadOreChunk,
			ZincOreChunk,
			StannaryChunk,
			MercuryOreChunk,
			ChromiumOreChunk,
			TitaniumOreChunk,
			NickelOreChunk
		}
		public readonly ChunkType Type;
		public readonly Color Color;

		protected readonly bool m_smooth;

		protected readonly BlockMesh m_standaloneBlockMesh = new BlockMesh();

		protected readonly Matrix m_tcTransform;

		protected readonly Matrix m_transform;

		protected OreChunk(Matrix transform, Matrix tcTransform, Color color, bool smooth,ChunkType type)
		{
			Type = type;
			m_transform = transform;
			m_tcTransform = tcTransform;
			Color = color;
			m_smooth = smooth;
			Model model = m_smooth ? ContentManager.Get<Model>("Models/ChunkSmooth") : ContentManager.Get<Model>("Models/Chunk");
			Matrix matrix = BlockMesh.GetBoneAbsoluteTransform(model.Meshes[0].ParentBone) * m_transform;
			BlockMesh standaloneBlockMesh = m_standaloneBlockMesh;
			ModelMeshPart meshPart = model.Meshes[0].MeshParts[0];
			Matrix matrix2 = matrix;
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			Color color2 = Color;
			standaloneBlockMesh.AppendModelMeshPart(meshPart, matrix2, num != 0, num2 != 0, num3 != 0, num4 != 0, color2);
			m_standaloneBlockMesh.TransformTextureCoordinates(m_tcTransform, -1);
		}

		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, color, 2f * size, ref matrix, environmentData);
		}
		public override string GetCategory(int value)
		{
			return "Terrain";
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
					return "A chunk of gold ore. When smelted in the furnace will turn into pure gold.";
				case ChunkType.SliverOreChunk:
					return "A chunk of sliver ore. When smelted in the furnace will turn into pure sliver.";
				case ChunkType.PlatinumOreChunk:
					return "A chunk of Platinum ore. When smelted in the furnace will turn into pure platinum.";
				case ChunkType.LeadOreChunk:
					return "A chunk of lead ore. When smelted in the furnace will turn into pure lead.";
				case ChunkType.MercuryOreChunk:
					return "A chunk of Mercury oxide ore. When smelted in the furnace will turn into pure liquid mercury.";
				case ChunkType.StannaryChunk:
					return "A chunk of Stannary ore. When smelted in the furnace will turn into pure stannary.";
				case ChunkType.ZincOreChunk:
					return "A chunk of Zinc ore. When smelted in the furnace will turn into pure zinc.";
				case ChunkType.ChromiumOreChunk:
					return "A chunk of Chromium ore. When smelted in the furnace will turn into pure chromium.";
				case ChunkType.TitaniumOreChunk:
					return "A chunk of Titanium ore. When smelted in the furnace will turn into pure titanium.";
				case ChunkType.NickelOreChunk:
					return "A chunk of Nickel ore. When smelted in the furnace will turn into pure nickel.";
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
		public override float GetProjectilePower(int value)
		{
			return 2f;
		}
	}
}
