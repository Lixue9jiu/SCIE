using Engine;
using Engine.Graphics;

namespace Game
{
	public class TexturedMeshItem : MeshItem
	{
		//public static Texture2D WhiteTexture;
		public readonly Texture2D Texture;
		protected readonly string ModelName;

		public TexturedMeshItem(string name, string modelName, string meshName, Texture2D texture, string description = null, float scale = 1 ,float transform2 = 0f) : base(description)
		{
			DefaultDisplayName = name;
			ModelName = modelName;
			var model = ContentManager.Get<Model>("Models/" + modelName);
			Matrix transform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh(meshName).ParentBone) * Matrix.CreateTranslation(0f, -0.4f, 0f) * Matrix.CreateScale(scale) * Matrix.CreateRotationZ(transform2);
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

	public class Rocket : TexturedMeshItem
	{

		public static Texture2D RocketTexture;
		public Rocket() : base("初级火箭", "Ra1", "Cylinder", RocketTexture, "一种比较初级的小型火箭，可以用来发射卫星，或者进行大气层核试验")
		{
		}

		public override Vector3 GetIconViewOffset(int value, DrawBlockEnvironmentData environmentData) => new Vector3(-0.7f, 0.8f, -1);
		public override Vector3 GetIconBlockOffset(int value, DrawBlockEnvironmentData environmentData) => new Vector3(0f, -0.9f, 0f);
		public override float GetIconViewScale(int value, DrawBlockEnvironmentData environmentData) => 0.25f;
		public override string GetCraftingId() => "Rocket";
	}

	public class MGun : TexturedMeshItem
	{
		//public static Texture2D BoatTexture;

		public MGun() : base("重机枪", "MGun3", "Musket", Carriage.TrainTexture, "重型机枪，一种利冷水冷却枪管，连射机，弹舱来获得强大持续火力的机器。")
		{
		}

		public override Vector3 GetIconViewOffset(int value, DrawBlockEnvironmentData environmentData) => new Vector3(-0.7f, 0.8f, -1);

		public override float GetIconViewScale(int value, DrawBlockEnvironmentData environmentData) => 1.4f;
		public override Vector3 GetIconBlockOffset(int value, DrawBlockEnvironmentData environmentData) => new Vector3(0f, 0.8f, 0f);
		public override string GetCraftingId() => "MGun";
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

	public class ETrain : TexturedMeshItem
	{
		public static Texture2D ETrainTexture;
		public ETrain() : base("电力机车", "ETrain2", "Electrongtrain", ETrainTexture, "电力机车是一种通过铁轨下面的电缆中的电来获得前进的动力。 再也不需携带化石燃料了。",0.6f,3.14f/2)
		{
			
		}
		public override string GetCraftingId() => "ETrain";
		public override Vector3 GetIconViewOffset(int value, DrawBlockEnvironmentData environmentData) => new Vector3(-0.6f, 0.6f, -0.8f);

		public override Vector3 GetIconBlockOffset(int value, DrawBlockEnvironmentData environmentData) => new Vector3(-0.3f, -0.5f, 0f);
	}

	public class Carriage : TexturedMeshItem
	{
		public static Texture2D TrainTexture;

		public Carriage() : base("车厢", "Minecart", "obj1", TrainTexture, "可以过载在火车头的后面，装载大量的货物",1f,1.6f)
		{
		}
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			//m_standaloneBlockMesh.AppendMesh
			//
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh , Texture, color, size, ref matrix, environmentData);
		}
		public override Vector3 GetIconViewOffset(int value, DrawBlockEnvironmentData environmentData) => new Vector3(-0.6f, 0.6f, -0.8f);

		public override Vector3 GetIconBlockOffset(int value, DrawBlockEnvironmentData environmentData) => new Vector3(-0.2f, -0.2f, 0f);
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

	public class Airplane : TexturedMeshItem
	{
		public static Texture2D PTexture;
		public Airplane() : base("飞机", "Plane", "Plane", PTexture, "使用螺旋桨，铝制机身内燃机的快速交通工具，不过要准备一座飞机场。", 0.4f)
		{
			m_standaloneBlockMesh.AppendMesh("Models/Plane", "Body", Matrix.CreateScale(0.4f) * Matrix.CreateTranslation(0f, -0.2f, 0f), Matrix.Identity, Color.White);
		}

		public override Vector3 GetIconViewOffset(int value, DrawBlockEnvironmentData environmentData) => new Vector3(-0.6f, 0.6f, -0.8f);

		public override Vector3 GetIconBlockOffset(int value, DrawBlockEnvironmentData environmentData) => new Vector3(-2.6f, 2.0f, 0f);
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
		public Tank() : base("坦克", "Tank", "Body", SteamBoat.BoatTexture, "一种使用内燃机，燃烧汽油获得动力的战争机器。", -0.5f)
		{
			m_standaloneBlockMesh.AppendMesh("Models/Tank", "Head", Matrix.CreateScale(-0.5f)*Matrix.CreateTranslation(0f,0f,0f), Matrix.Identity, Color.White);
		}
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			//m_standaloneBlockMesh.AppendMesh
			//
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, Texture, color, size, ref matrix, environmentData);
		}
		public override Vector3 GetIconViewOffset(int value, DrawBlockEnvironmentData environmentData) => new Vector3(-0.6f, 1.2f, 0.5f);
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

	public class Pavior : TexturedMeshItem
	{
		public Pavior() : base("铺路机", "Car", "ChamferBox01", SteamBoat.BoatTexture, "一种使用内燃机，燃烧汽油获得动力，可以把方块铺在后面。", 0.5f)
		{
		}

		public override Vector3 GetIconViewOffset(int value, DrawBlockEnvironmentData environmentData) => new Vector3(-0.6f, 0.6f, -0.8f);
		public override string GetCraftingId() => "Pavior";
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, Texture, Color.Gray, size, ref matrix, environmentData);
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