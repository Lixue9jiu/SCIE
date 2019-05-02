using Engine;
using Engine.Graphics;

namespace Game
{
	public class TexturedMeshItem : MeshItem
	{
		public Texture2D Texture;
		private readonly string ModelName;

		public TexturedMeshItem(string name, string modelName, string meshName, Texture2D texture, float scale = 1, string description = null) : base(description)
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
		public SteamBoat() : base("蒸汽船", "SteamBoat", "Cylinder", ContentManager.Get<Texture2D>("Textures/SteamBoat"), 1f, "蒸汽船可让您更安全，更快速地穿越大面积的水，就像您拥有足够的燃料一样，在最初的工业时代，这是一种强大的运输工具。")
		{
		}
		public override Vector3 GetIconViewOffset(int value, DrawBlockEnvironmentData environmentData) => new Vector3(-0.7f, 0.8f, -1);
		public override float GetIconViewScale(int value, DrawBlockEnvironmentData environmentData) => 0.45f;
	}
	public class Train : TexturedMeshItem
    {
		public Train() : base("蒸汽机车", "Train", "Cylinder", ContentManager.Get<Texture2D>("Textures/Train"), 1f, "蒸汽机车是一种通过蒸汽机产生牵引力的铁路机车。 这些机车通过燃烧可燃材料（通常是煤）在锅炉中产生蒸汽来加燃料。 蒸汽移动往复活塞，机械连接到机车的主轮。 燃料和水供应都由机车携带。")
		{
		}
		public override Vector3 GetIconViewOffset(int value, DrawBlockEnvironmentData environmentData) => new Vector3(-0.6f, 0.6f, -0.8f);
		public override Vector3 GetIconBlockOffset(int value, DrawBlockEnvironmentData environmentData) => new Vector3(0f, -0.2f, 0f);
	}
    public class Carriage : TexturedMeshItem
    {
        public Carriage() : base("车厢", "Minecart", "obj1", ContentManager.Get<Texture2D>("Textures/Creatures/Jaguar"), 1f, "")
        {
        }
		public override Vector3 GetIconViewOffset(int value, DrawBlockEnvironmentData environmentData) => new Vector3(-0.6f, 0.6f, -0.8f);
		public override Vector3 GetIconBlockOffset(int value, DrawBlockEnvironmentData environmentData) => new Vector3(0f, 0.2f, 0f);
	}
	public class Airship : TexturedMeshItem
    {
		public static readonly Texture2D WhiteTexture = new Texture2D(1, 1, false, ColorFormat.Rgba8888);
		static Airship() { WhiteTexture.SetData(0, new byte[] { 255, 255, 255, 255 }); }
		public Airship() : base("飞艇", "Airship", "Airship", WhiteTexture, 0.08f, "使用比空气更轻的气体来产生浮力并使其飞行的工艺，其驱动动力源来自通过燃烧航空汽油的往复式发动机。")
		{
		}
		public override Vector3 GetIconViewOffset(int value, DrawBlockEnvironmentData environmentData) => new Vector3(-0.6f, 0.6f, -0.8f);
		public override Vector3 GetIconBlockOffset(int value, DrawBlockEnvironmentData environmentData) => new Vector3(0f, -0.2f, 0f);
	}
}