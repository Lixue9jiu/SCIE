using Engine;
using Engine.Graphics;
using System.Globalization;

namespace Game
{
	public class Screwdriver : MeshItem
	{
		public Screwdriver(Color color, string description = "") : base(description)
		{
			DefaultDisplayName = "螺丝刀";
			m_standaloneBlockMesh.AppendMesh("Models/Screwdriver", "obj1", Matrix.CreateTranslation(0f, -0.33f, 0f), Matrix.CreateTranslation(15f / 16f, 0f, 0f) * Matrix.CreateScale(0.05f), color);
		}

		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, color, 3.3f * size, ref matrix, environmentData);
		}

		public override string GetCraftingId() => "Screwdriver";

		public override string GetCategory() => "Tools";
	}

	public class Wrench : MeshItem
	{
		public Wrench(Color color, string description = "") : base(description)
		{
			DefaultDisplayName = "扳手";
			m_standaloneBlockMesh.AppendMesh("Models/Wrench", "obj1", Matrix.CreateScale(1.2f), Matrix.CreateTranslation(15f / 16f, 0f, 0f) * Matrix.CreateScale(0.05f), color);
		}

		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, color, 6f * size, ref matrix, environmentData);
		}

		public override string GetCraftingId() => "Wrench";

		public override string GetCategory() => "Tools";
	}

	public class MetalIngot : MeshItem
	{
		protected string Id;

		public MetalIngot(Materials type)
		{
			Id = type.ToString() + "Ingot";
			string name = type.ToStr();
			DefaultDisplayName = name + Utils.Get("锭");
			DefaultDescription = "An ingot of pure " + name + ". Can be crafted into very durable and strong " + name + " items. Very important in the industrial era.";
			m_standaloneBlockMesh.AppendMesh("Models/Ingots", "IronIngot", Matrix.CreateTranslation(0f, -0.1f, 0f), Matrix.Identity, MetalBlock.GetColor(type));
		}

		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, color, 2f * size, ref matrix, environmentData);
		}

		public override string GetCraftingId() => Id;
	}

	public class MetalLine : FlatItem
	{
		protected string Id;

		public MetalLine(Materials type)
		{
			Id = type.ToString() + "Line";
			DefaultTextureSlot = 212;
			DefaultDescription = (DefaultDisplayName = type.ToStr() + Utils.Get("丝")) + "is made of " + type.ToStr() + " Ingot, it can be used in many place in the industrial era.";
			Color = MetalBlock.GetColor(type);
		}

		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			ItemBlock.DrawFlatBlock(primitivesRenderer, value, size, ref matrix, ItemBlock.Texture, Color, false, environmentData);
		}

		public override string GetCraftingId() => Id;
	}

	public class Rod : MeshItem
	{
		protected string Id;

		public Rod(Materials type) : this(type.ToString() + "Rod", null, MetalBlock.GetColor(type))
		{
			string name = type.ToStr();
			DefaultDisplayName = name + Utils.Get("棒");
			DefaultDescription = "Rods are made by forging " + char.ToLower(name[0], CultureInfo.CurrentCulture) + name.Substring(1) + " into shape. They are useful for making many things.";
		}

		public Rod(string name, string description, Color color) : base(description)
		{
			Id = name;
			m_standaloneBlockMesh.AppendMesh("Models/Rods", "SteelRod", Matrix.CreateTranslation(0, -0.5f, 0), Matrix.Identity, color);
		}

		public override Vector3 GetIconViewOffset(int value, DrawBlockEnvironmentData environmentData) => new Vector3(-1, 0.5f, 0);

		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, color, 1.6f * size, ref matrix, environmentData);
		}

		public override float GetMeleePower() => 2f;

		public override string GetCraftingId() => Id;
	}

	public class Plate : MeshItem
	{
		protected BoundingBox[] m_collisionBoxes;
		protected string Id;

		public Plate(string name, Color color, bool small = false)
		{
			Id = DefaultDisplayName = DefaultDescription = name;
			Matrix matrix = small ? Matrix.CreateScale(.5f) : Matrix.Identity;
			m_standaloneBlockMesh.AppendMesh("Models/Ingots", "IronPlate", Matrix.CreateTranslation(0.5f, 0f, 0.5f) * matrix, Matrix.Identity, color);
			m_collisionBoxes = new BoundingBox[] { m_standaloneBlockMesh.CalculateBoundingBox() };
		}

		public Plate(Materials type, bool small = false) : this(type.ToString() + "Plate", MetalBlock.GetColor(type), small)
		{
			string name = type.ToStr();
			if (small)
			{
				Id = type.ToString() + "Sheet";
				DefaultDisplayName = name + Utils.Get("片");
				DefaultDescription = "A sheet of pure " + name + ". Can be crafted into very durable and strong " + name + " items.";
				return;
			}
			DefaultDisplayName = name + Utils.Get("板");
			DefaultDescription = "A plate of pure " + name + ". Can be crafted into very durable and strong " + name + " items. Very important in the industrial era.";
		}

		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, color, size * 1.5f, ref matrix, environmentData);
		}

		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			return new BlockPlacementData { Value = value, CellFace = raycastResult.CellFace };
		}

		public override string GetCraftingId() => Id;

		public override Vector3 GetIconBlockOffset(int value, DrawBlockEnvironmentData environmentData) => new Vector3 { Y = 0.45f };

		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value) => m_collisionBoxes;
	}

	public class Alloy : MeshItem
	{
		protected BoundingBox[] m_collisionBoxes;
		public readonly Color Color;
		protected string Id;

		private Alloy(string name, string name1, Color color)
		{
			DefaultDescription = name;
			Color = color;
			DefaultDisplayName = name1 + Utils.Get("合金");
			m_standaloneBlockMesh.AppendMesh("Models/Alloy", "Torch", Matrix.CreateTranslation(0.5f, 0f, 0.5f) * Matrix.CreateScale(1.2f), Matrix.CreateTranslation(12f / 16f, 1f / 16f, 0f), color);
			m_collisionBoxes = new BoundingBox[] { m_standaloneBlockMesh.CalculateBoundingBox() };
		}

		public Alloy(Materials type, string name) : this(type.ToString() + "Alloy", name, MetalBlock.GetColor(type))
		{
			Id = name;
			DefaultDescription = "Alloy of " + name + ". Can be crafted into very durable and strong " + name + " items. Very important in the industrial era.";
		}

		/*public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, Color, size * 1.5f, ref matrix, environmentData);
		}*/

		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			return new BlockPlacementData { Value = value, CellFace = raycastResult.CellFace };
		}

		public override string GetCraftingId() => Id;

		public override Vector3 GetIconBlockOffset(int value, DrawBlockEnvironmentData environmentData) => new Vector3 { Y = 0.45f };

		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value) => m_collisionBoxes;
	}

	public class FoodCan : MeshItem
	{
		protected BoundingBox[] m_collisionBoxes;
		public readonly Color Color;
		protected string Id;

		public FoodCan(string name, string name1, Color color)
		{
			DefaultDescription = name;
			Color = color;
			Id = name1;
			DefaultDisplayName = name1 + Utils.Get("Can");
			m_standaloneBlockMesh.AppendMesh("Models/Battery", "Battery", Matrix.CreateTranslation(0.5f, 0f, 0.5f), Matrix.CreateTranslation(12f / 16f, 1f / 16f, 0f), color);
			m_collisionBoxes = new BoundingBox[] { m_standaloneBlockMesh.CalculateBoundingBox() };
		}

		public override string GetCraftingId() => Id;

		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value) => m_collisionBoxes;
	}

	public class Brick : MeshItem
	{
		public Brick(Color color) : base(Utils.Get("耐火砖是一种耐火陶瓷材料，用于炉衬炉，窑炉，高级燃烧室和壁炉。 它具有耐高温性能，但导热系数低，能效高。"))
		{
			m_standaloneBlockMesh.AppendMesh("Models/Brick", "Brick", Matrix.CreateTranslation(0f, -.02f, 0f) * 1.4f, Matrix.CreateTranslation(-32 % 16 / 16f, -32 / 16 / 16f, 0f), color);
		}

		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, Color.White, 2f * size, ref matrix, environmentData);
		}

		public override float GetMeleePower() => 2f;

		public override float GetProjectilePower() => 2f;
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