using Engine;
using Engine.Graphics;

namespace Game
{
	public class TexturedMeshItem : MeshItem
	{
		//public static Texture2D WhiteTexture;
		public readonly Texture2D Texture;
		protected readonly string ModelName;

		public TexturedMeshItem(string name, string modelName, string meshName, Texture2D texture, string description = null, float scale = 1) : base(description)
		{
			DefaultDisplayName = name;
			ModelName = modelName;
			var model = ContentManager.Get<Model>("Models/" + modelName);
			Matrix transform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh(meshName).ParentBone) * Matrix.CreateTranslation(0f, -0.4f, 0f) * Matrix.CreateScale(scale);
			ModelMeshPart meshPart = model.FindMesh(meshName).MeshParts[0];
			m_standaloneBlockMesh.AppendModelMeshPart(meshPart, transform, false, false, false, false, Color.White);
			m_standaloneBlockMesh.AppendModelMeshPart(meshPart, transform, false, true, false, false, Color.White);
			Texture = texture;
		}

		public override string GetCraftingId() => ModelName;

		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, Texture, Color.White, size, ref matrix, environmentData);
		}
	}

	public class SteamBoat : TexturedMeshItem
	{
		public static Texture2D BoatTexture;

		public SteamBoat() : base("蒸汽船", "SteamBoat", "Cylinder", BoatTexture, "蒸汽船可让您更安全，更快速地穿越大面积的水，就像您拥有足够的燃料一样，在最初的工业时代，这是一种强大的运输工具。")
		{
		}

		public override Vector3 GetIconViewOffset(int value, DrawBlockEnvironmentData environmentData) => new Vector3(-0.7f, 0.8f, -1);

		public override float GetIconViewScale(int value, DrawBlockEnvironmentData environmentData) => 0.45f;
	}

	public class Icebreaker : TexturedMeshItem
	{
		public Icebreaker() : base("破冰船", "SteamBoat", "Cylinder", SteamBoat.BoatTexture, "破冰船")
		{
		}

		public override Vector3 GetIconViewOffset(int value, DrawBlockEnvironmentData environmentData) => new Vector3(-0.7f, 0.4f, -1);

		public override float GetIconViewScale(int value, DrawBlockEnvironmentData environmentData) => 0.5f;

		public override string GetCraftingId() => "Icebreaker";
	}

	public class Train : TexturedMeshItem
	{
		public static Texture2D TrainTexture;

		public Train() : base("蒸汽机车", "Train", "Cylinder", TrainTexture, "蒸汽机车是一种通过蒸汽机产生牵引力的铁路机车。 这些机车通过燃烧可燃材料（通常是煤）在锅炉中产生蒸汽来加燃料。 蒸汽移动往复活塞，机械连接到机车的主轮。 燃料和水供应都由机车携带。")
		{
		}

		public override Vector3 GetIconViewOffset(int value, DrawBlockEnvironmentData environmentData) => new Vector3(-0.6f, 0.6f, -0.8f);

		public override Vector3 GetIconBlockOffset(int value, DrawBlockEnvironmentData environmentData) => new Vector3(0f, -0.2f, 0f);
	}

	public class Carriage : TexturedMeshItem
	{
		public static Texture2D TrainTexture;

		public Carriage() : base("车厢", "Minecart", "obj1", TrainTexture, "可以过载在火车头的后面，装载大量的货物")
		{
		}

		public override Vector3 GetIconViewOffset(int value, DrawBlockEnvironmentData environmentData) => new Vector3(-0.6f, 0.6f, -0.8f);

		public override Vector3 GetIconBlockOffset(int value, DrawBlockEnvironmentData environmentData) => new Vector3(0f, 0.2f, 0f);
	}

	public class Airship : TexturedMeshItem
	{
		public static Texture2D ATexture;
		public Airship() : base("飞艇", "Airship", "small_airship", ATexture, "使用比空气更轻的气体来产生浮力并使其飞行的工艺，其驱动动力源来自通过燃烧航空汽油的往复式发动机。", 0.2f)
		{
		}

		public override Vector3 GetIconViewOffset(int value, DrawBlockEnvironmentData environmentData) => new Vector3(-0.6f, 0.6f, -0.8f);

		public override Vector3 GetIconBlockOffset(int value, DrawBlockEnvironmentData environmentData) => new Vector3(0.3f, -0.4f, 0f);
	}

	public class Car : TexturedMeshItem
	{
		public static Texture2D CarTexture;
		public Car() : base("汽车", "Car", "ChamferBox01", CarTexture, "一种使用内燃机，燃烧汽油获得动力，在平坦的地面上行驶的交通工具。", 0.5f)
		{
		}

		public override Vector3 GetIconViewOffset(int value, DrawBlockEnvironmentData environmentData) => new Vector3(-0.6f, 0.6f, 0.5f);
	}
	public class Tank : TexturedMeshItem
	{
		//public static Texture2D CarTexture;
		public Tank() : base("坦克", "Tank", "Body", Car.CarTexture, "一种使用内燃机，燃烧汽油获得动力，在平坦的地面上行驶的交通工具。", 0.5f)
		{
		}

		public override Vector3 GetIconViewOffset(int value, DrawBlockEnvironmentData environmentData) => new Vector3(-0.6f, 0.6f, 0.5f);
	}
	public class Tractor : TexturedMeshItem
	{
		public Tractor() : base("拖拉机", "Car", "ChamferBox01", Car.CarTexture, "一种使用内燃机，燃烧汽油获得动力，可以耕地施肥种植收割的农业用具。", 0.5f)
		{
		}

		public override Vector3 GetIconViewOffset(int value, DrawBlockEnvironmentData environmentData) => new Vector3(-0.6f, 0.6f, -0.8f);
		public override string GetCraftingId() => "Tractor";
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, Texture, Color.LightMagenta, size, ref matrix, environmentData);
		}
	}

	public class Digger : Car
	{
		public Digger()
		{
			DefaultDisplayName = "盾构机";
			DefaultDescription = "一种使用内燃机，燃烧汽油获得动力，挖掘前面的方块的用具。";
		}

		public override string GetCraftingId() => "Digger";
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, Texture, Color.Gray, size, ref matrix, environmentData);
		}
	}
}