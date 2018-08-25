using Engine;
using Engine.Graphics;
using System;

namespace Game
{
	public class OreChunk : BlockItem
	{
		public readonly MetalType Type;
		public readonly Color Color;

		protected readonly bool m_smooth;

		protected readonly BlockMesh m_standaloneBlockMesh = new BlockMesh();

		protected readonly Matrix m_tcTransform;

		protected readonly Matrix m_transform;

		public OreChunk(Matrix transform, Matrix tcTransform, Color color, bool smooth, MetalType type)
		{
			Type = type;
			DefaultDisplayName = Type.ToString() + "OreChunk";
			DefaultDescription = GetDescription(type);
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
		public static string GetDescription(MetalType type)
		{
			switch (type)
			{
				case MetalType.Gold:
					return "A chunk of gold ore. When smelted in the furnace will turn into pure gold.";
				case MetalType.Sliver:
					return "A chunk of sliver ore. When smelted in the furnace will turn into pure sliver.";
				case MetalType.Platinum:
					return "A chunk of Platinum ore. When smelted in the furnace will turn into pure platinum.";
				case MetalType.Lead:
					return "A chunk of lead ore. When smelted in the furnace will turn into pure lead.";
				case MetalType.Mercury:
					return "A chunk of Mercury oxide ore. When smelted in the furnace will turn into pure liquid mercury.";
				case MetalType.Stannary:
					return "A chunk of Stannary ore. When smelted in the furnace will turn into pure stannary.";
				case MetalType.Zinc:
					return "A chunk of Zinc ore. When smelted in the furnace will turn into pure zinc.";
				case MetalType.Chromium:
					return "A chunk of Chromium ore. When smelted in the furnace will turn into pure chromium.";
				case MetalType.Titanium:
					return "A chunk of Titanium ore. When smelted in the furnace will turn into pure titanium.";
				case MetalType.Nickel:
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
