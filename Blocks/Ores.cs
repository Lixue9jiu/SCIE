using Engine;
using Engine.Graphics;
using System.Globalization;
using System.Text;

namespace Game
{
	public class OreChunk : FlatItem
	{
		public readonly Color Color;

		protected readonly BlockMesh m_standaloneBlockMesh = new BlockMesh();

		public OreChunk(Matrix transform, Matrix tcTransform, Color color, bool smooth, Materials type)
		{
			string name = type.ToString();
			DefaultDisplayName = name + "OreChunk";
			var sb = new StringBuilder("A chunk of ");
			if (type == Materials.Mercury)
				sb.Append("oxide ");
			name = char.ToLower(name[0], CultureInfo.CurrentCulture) + name.Substring(1);
			sb.Append(name);
			sb.Append(" ore. When smelted in the furnace will turn into pure ");
			if (type == Materials.Mercury)
				sb.Append("liquid ");
			sb.Append(name);
			sb.Append('.');
			DefaultDescription = sb.ToString();
			Color = color;
			var model = ContentManager.Get<Model>(smooth ? "Models/ChunkSmooth" : "Models/Chunk");
			m_standaloneBlockMesh.AppendModelMeshPart(model.Meshes[0].MeshParts[0], BlockMesh.GetBoneAbsoluteTransform(model.Meshes[0].ParentBone) * transform, false, false, false, false, color);
			m_standaloneBlockMesh.TransformTextureCoordinates(tcTransform, -1);
		}

		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, CustomTextureItem.Texture, color, 2f * size, ref matrix, environmentData);
		}

		public override string GetCategory(int value)
		{
			return "Terrain";
		}

		public override float GetProjectilePower(int value)
		{
			return 2f;
		}
	}
	public class Powder : FlatItem
	{
		public static readonly Color[] Colors = new[]
		{
			Color.White,
			new Color(255, 215, 0),
			new Color(212, 212, 212),
			new Color(87, 86, 85),
			new Color(232, 232, 232),
			new Color(64, 224, 205),
			new Color(225, 225, 225),
			new Color(60, 60, 60),
			Color.White,
			new Color(120, 120, 120),
			new Color(199, 97, 20),
			Color.White,
			Color.White,
			new Color(139, 69, 19),
			new Color(34, 139, 34),
			Color.White,
			new Color(205, 190, 112),
		};

		public readonly Color Color;

		public Powder(Materials type) : this(type.ToString() + "OrePowder", Colors[(int)type])
		{
		}

		public Powder(string name, Color color, string description = null)
		{
			DefaultDisplayName = name;
			Color = color;
			DefaultTextureSlot = 198;
			DefaultDescription = description ?? name + " is powder obtained by crushing " + name + ".";
		}

		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawFlatBlock(primitivesRenderer, value, size, ref matrix, null, Color, false, environmentData);
		}
	}
}