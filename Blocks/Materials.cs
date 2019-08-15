using Engine;
using Engine.Graphics;
using System.Globalization;

namespace Game
{
	public class MouldItem : Mould
	{
		public string Id;
		public string Category;
		public MouldItem(string id, string modelName, string meshName, Matrix boneTransform, Matrix tcTransform, string description = "", string name = "", float size = 1) : base(modelName, meshName, boneTransform, tcTransform, description, name, size)
		{
			Id = id;
			Category = meshName.Length == 4 ? "Tools" : "Items";
		}

		public override string GetCraftingId() => Id;

		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			return default(BlockPlacementData);
		}
		public override string GetCategory() => Category;
	}
	public class LightMould : Mould
	{
		public LightMould(string modelName, string meshName, Matrix boneTransform, Matrix tcTransform, string description = "", string name = "", float size = 1) : base(modelName, meshName, boneTransform, tcTransform, description, name, size)
		{
		}

		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			base.DrawBlock(primitivesRenderer, value, Color.White, size, ref matrix, environmentData);
		}
	}
	public class MetalIngot : MeshItem
	{
		protected string Id;

		public MetalIngot(Materials type)
		{
			Id = type.ToString() + "Ingot";
			string name = type.ToStr();
			DefaultDisplayName = name + Utils.Get("¶§");
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
			DefaultDescription = (DefaultDisplayName = type.ToStr() + Utils.Get("Ë¿")) + "is made of " + type.ToStr() + " Ingot, it can be used in many place in the industrial era.";
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
			DefaultDisplayName = name + Utils.Get("°ô");
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
			m_standaloneBlockMesh.AppendMesh("Models/Ingots", "IronPlate", Matrix.CreateTranslation(0.5f, 0f, 0.5f) * (small ? Matrix.CreateScale(.5f) : Matrix.Identity), Matrix.Identity, color);
			m_collisionBoxes = new BoundingBox[] { m_standaloneBlockMesh.CalculateBoundingBox() };
		}

		public Plate(Materials type, bool small = false) : this(type.ToString() + "Plate", MetalBlock.GetColor(type), small)
		{
			string name = type.ToStr();
			if (small)
			{
				Id = type.ToString() + "Sheet";
				DefaultDisplayName = name + Utils.Get("Æ¬");
				DefaultDescription = "A sheet of pure " + name + ". Can be crafted into very durable and strong " + name + " items.";
				return;
			}
			DefaultDisplayName = name + Utils.Get("°å");
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

	public class Alloy : Mould
	{
		public readonly Color Color;
		protected string Id;

		public Alloy(Materials type, string name) : base("Models/Alloy", "Torch", Matrix.CreateTranslation(0.5f, 0f, 0.5f) * Matrix.CreateScale(1.2f), Matrix.CreateTranslation(12f / 16f, 1f / 16f, 0f))
		{
			Id = name;
			DefaultDescription = "Alloy of " + name + ". Can be crafted into very durable and strong " + name + " items. Very important in the industrial era.";
			Color = MetalBlock.GetColor(type);
			DefaultDisplayName = name + Utils.Get("ºÏ½ð");
		}

		public override string GetCraftingId() => Id;

		public override Vector3 GetIconBlockOffset(int value, DrawBlockEnvironmentData environmentData) => new Vector3 { Y = 0.45f };
	}

	public class FoodCan : MeshItem
	{
		protected BoundingBox[] m_collisionBoxes;
		protected string Id;

		public FoodCan(string name, string id, Color color)
		{
			Id = id;
			DefaultDescription = name;
			DefaultDisplayName = id + Utils.Get("Can");
			m_standaloneBlockMesh.AppendMesh("Models/Battery", "Battery", Matrix.CreateTranslation(0.5f, 0f, 0.5f), Matrix.CreateTranslation(12f / 16f, 1f / 16f, 0f), color);
			m_collisionBoxes = new BoundingBox[] { m_standaloneBlockMesh.CalculateBoundingBox() };
		}

		public override string GetCraftingId() => Id;

		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value) => m_collisionBoxes;
	}

	public class Brick : MeshItem
	{
		public string Id;
		public Brick(string name, string id, Color color, Matrix tcTrarsform, string description = "") : base(Utils.Get(description))
		{
			DefaultDisplayName = Utils.Get(name);
			Id = id;
			m_standaloneBlockMesh.AppendMesh("Models/Brick", "Brick", Matrix.CreateTranslation(0f, -.02f, 0f) * 1.4f, tcTrarsform, color);
		}

		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, Color.White, 2f * size, ref matrix, environmentData);
		}

		public override float GetMeleePower() => 2f;

		public override float GetProjectilePower() => 2f;
		public override string GetCraftingId() => Id ?? DefaultDisplayName;
	}

	public class Spring : MeshItem
	{
		public Spring(string name, string description = "") : base(Utils.Get(description))
		{
			DefaultDisplayName = Utils.Get(name);
			m_standaloneBlockMesh = CreateRing();
		}
		public override string GetCraftingId() => GetType().Name;
	}

	public class Springboard : MeshItem
	{
		protected BoundingBox[] m_collisionBoxes;
		public Springboard(string name, string description = "") : base(Utils.Get(description))
		{
			DefaultDisplayName = Utils.Get(name);
			var mesh = CreateRing();
			mesh.TransformPositions(Matrix.CreateScale(0.9f) * Matrix.CreateTranslation(new Vector3(0.5f)));
			m_standaloneBlockMesh.AppendBlockMesh(mesh);
			m_standaloneBlockMesh.AppendMesh("Models/Ingots", "IronPlate", Matrix.CreateTranslation(0.5f, 0.8f, 0.5f), Matrix.Identity, Color.White);
			m_collisionBoxes = new BoundingBox[] { m_standaloneBlockMesh.CalculateBoundingBox() };
		}
		public override string GetCraftingId() => GetType().Name;
		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value) => m_collisionBoxes;
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			return new BlockPlacementData { Value = value, CellFace = raycastResult.CellFace };
		}
	}

	/*public class MetalDetector : MeshItem
	{
		public BlockMesh m_pointerMesh = new BlockMesh();
		public MetalDetector() : base("")
		{
			Model model = ContentManager.Get<Model>("Models/Compass");
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Case").ParentBone);
			Matrix boneAbsoluteTransform2 = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Pointer").ParentBone);
			m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Case").MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.01f, 0f), false, false, true, false, Color.Gray);
			m_pointerMesh.AppendModelMeshPart(model.FindMesh("Pointer").MeshParts[0], boneAbsoluteTransform2 * Matrix.CreateTranslation(0f, -0.01f, 0f), false, false, false, false, Color.Gray);
		}

		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			float radians = 0f;
			if (environmentData != null && environmentData.SubsystemTerrain != null)
			{
				Vector3 forward = environmentData.InWorldMatrix.Forward;
				Vector3 translation = environmentData.InWorldMatrix.Translation;
				float num = float.MaxValue;
				Vector3 v = Vector3.Zero;
				for (int i = 0; i < m_magnets.Count && i < 8; i++)
				{
					Vector3 vector = m_magnets.Array[i];
					float num2 = Vector3.DistanceSquared(compassPosition, vector);
					if (num2 < num)
					{
						num = num2;
						v = vector;
					}
				}
				Vector3 vector = translation - v + new Vector3(0.5f);
				radians = Vector2.Angle(v2: new Vector2(forward.X, forward.Z), v1: new Vector2(vector.X, vector.Z));
			}
			Matrix matrix2 = matrix;
			Matrix matrix3 = Matrix.CreateRotationY(radians) * matrix;
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, color, size * 6f, ref matrix2, environmentData);
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_pointerMesh, color, size * 6f, ref matrix3, environmentData);
		}
	}
	public class Slab : MeshItem
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