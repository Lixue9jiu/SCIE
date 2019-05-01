using Engine;
using Engine.Graphics;
using System.Globalization;

namespace Game
{
	public class Screwdriver : MeshItem
	{
		public Screwdriver(Color color, string description = "") : base(description)
		{
			DefaultDisplayName = "ÂÝË¿µ¶";
			var model = ContentManager.Get<Model>("Models/Screwdriver");
			m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("obj1", true).MeshParts[0], BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("obj1", true).ParentBone) * Matrix.CreateTranslation(0f, -0.33f, 0f), false, false, false, false, color);
            m_standaloneBlockMesh.TransformTextureCoordinates(Matrix.CreateTranslation(15f / 16f, 0f, 0f) * Matrix.CreateScale(0.05f));
        }
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, color, 3.3f * size, ref matrix, environmentData);
		}
		public override string GetCraftingId() => "Screwdriver";
		public override string GetCategory(int value) => "Tools";
	}
	public class Wrench : MeshItem
    {
        public Wrench(Color color, string description = "") : base(description)
		{
			DefaultDisplayName = "°âÊÖ";
			var model = ContentManager.Get<Model>("Models/Wrench");
            m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("obj1", true).MeshParts[0], BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("obj1", true).ParentBone) * Matrix.CreateScale(1.2f), false, false, false, false, color);
            m_standaloneBlockMesh.TransformTextureCoordinates(Matrix.CreateTranslation(15f / 16f, 0f, 0f) * Matrix.CreateScale(0.05f));
        }
        public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
        {
            BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, color, 6f * size, ref matrix, environmentData);
		}
		public override string GetCraftingId() => "Wrench";
		public override string GetCategory(int value) => "Tools";
	}
	public class MetalIngot : MeshItem
	{
		public MetalIngot(Materials type)
		{
			var color = MetalBlock.GetColor(type);
			string name = type.ToString();
			DefaultDisplayName = name + "Ingot";
			DefaultDescription = "An ingot of pure " + name + ". Can be crafted into very durable and strong " + name + " items. Very important in the industrial era.";
			var model = ContentManager.Get<Model>("Models/Ingots");
			m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("IronIngot", true).MeshParts[0], BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("IronIngot", true).ParentBone) * Matrix.CreateTranslation(0f, -0.1f, 0f), false, false, false, false, color);
		}

		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, color, 2f * size, ref matrix, environmentData);
		}
	}
	public class MetalLine : ColoredFlatItem
    {
		public MetalLine(Materials type)
		{
			DefaultTextureSlot = 212;
			DefaultDescription = (DefaultDisplayName = type.ToString() + "Line") + "is made of " + type.ToString() + " Ingot, it can be used in many place in the industrial era.";
			Color = MetalBlock.GetColor(type);
		}

		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
            CustomTextureItem.DrawFlatBlock(primitivesRenderer, value, size, ref matrix, CustomTextureItem.Texture, Color, false, environmentData);
		}
	}
	public class Rod : FlatItem
	{
		protected readonly BlockMesh m_standaloneBlockMesh = new BlockMesh();

		public Rod(Materials type, Color color) : this(color)
		{
			string name = type.ToString();
			DefaultDisplayName = name + "Rod";
			DefaultDescription = "Rods are made by forging " + char.ToLower(name[0], CultureInfo.CurrentCulture) + name.Substring(1) + " into shape. They are useful for making many things.";
		}
		public Rod(Color color)
		{
			var model = ContentManager.Get<Model>("Models/Rods");
			m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("SteelRod", true).MeshParts[0], BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("SteelRod", true).ParentBone) * Matrix.CreateTranslation(0f, -0.5f, 0f), false, false, false, false, color);
		}

		public override Vector3 GetIconViewOffset(int value, DrawBlockEnvironmentData environmentData) => new Vector3(-1, 0.5f, 0);

		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, color, 1.6f * size, ref matrix, environmentData);
		}

		public override float GetMeleePower(int value)
		{
			return 2f;
		}
	}
	public class GranulatedItem : ColoredFlatItem
	{
		public GranulatedItem(string name, Color color)
		{
			DefaultDisplayName = Utils.Get(name);
			DefaultTextureSlot = 231;
			Color = color;
		}
	}
	/*public class Slab : MeshItem
	{
		public BlockMesh[] BlockMeshes = new BlockMesh[2];
		public BoundingBox[][] m_collisionBoxes = new BoundingBox[2][];
		public Slab(int DefaultTextureSlot)
		{
			Model model = ContentManager.Get<Model>("Models/Slab");
			ModelMeshPart meshPart = model.FindMesh("Slab", true).MeshParts[0];
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Slab", true).ParentBone);
			for (int i = 0; i < 2; i++)
			{
				Matrix matrix = boneAbsoluteTransform * Matrix.CreateTranslation(0.5f, (i == 0) ? 0f : 0.5f, 0.5f);
				BlockMeshes[i] = new BlockMesh();
				BlockMeshes[i].AppendModelMeshPart(meshPart, matrix, false, false, false, false, Color.White);
				BlockMeshes[i].TransformTextureCoordinates(Matrix.CreateTranslation((float)(DefaultTextureSlot % 16) / 16f, (float)(DefaultTextureSlot / 16) / 16f, 0f), -1);
				BlockMeshes[i].GenerateSidesData();
			}
			m_standaloneBlockMesh.AppendModelMeshPart(meshPart, boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.5f, 0f), false, false, false, false, Color.White);
			m_standaloneBlockMesh.TransformTextureCoordinates(Matrix.CreateTranslation((float)(DefaultTextureSlot % 16) / 16f, (float)(DefaultTextureSlot / 16) / 16f, 0f), -1);
			m_collisionBoxes[0] = new BoundingBox[1]
			{
				new BoundingBox(new Vector3(0f, 0f, 0f), new Vector3(1f, 0.5f, 1f))
			};
			m_collisionBoxes[1] = new BoundingBox[1]
			{
				new BoundingBox(new Vector3(0f, 0.5f, 0f), new Vector3(1f, 1f, 1f))
			};
		}
		public override bool IsFaceTransparent(SubsystemTerrain subsystemTerrain, int face, int value)
		{
			if (GetIsTop(Terrain.ExtractData(value)))
				return face != 4;
			return face != 5;
		}
		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
		{
			return m_collisionBoxes[GetIsTop(Terrain.ExtractData(value)) ? 1 : 0];
		}
		public static bool GetIsTop(int data)
		{
			return (data & 1) != 0;
		}
	}*/
}