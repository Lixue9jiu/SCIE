using Engine;
using Engine.Graphics;
using System.Globalization;
using System.Text;

namespace Game
{
	public class OreChunk : FlatItem
	{
		protected string Id;
		protected readonly BlockMesh m_standaloneBlockMesh = new BlockMesh();

		public OreChunk(Matrix transform, Matrix tcTransform, Color color, bool smooth, Materials type)
		{
			string name = type.ToStr();
			Id = type.ToString() + "OreChunk";
			DefaultDisplayName = name + Utils.Get("矿石块");
			var sb = new StringBuilder(Utils.Get("一块"));
			if (type == Materials.Mercury)
				sb.Append(Utils.Get("氧化"));
			name = char.ToLower(name[0], CultureInfo.CurrentCulture) + name.Substring(1);
			sb.Append(name).Append(" ore. When smelted in the furnace will turn into pure ");
			if (type == Materials.Mercury)
				sb.Append(Utils.Get("液态"));
			sb.Append(name).Append('.');
			DefaultDescription = sb.ToString();
			Color = color;
			var model = ContentManager.Get<Model>(smooth ? "Models/ChunkSmooth" : "Models/Chunk");
			m_standaloneBlockMesh.AppendModelMeshPart(model.Meshes[0].MeshParts[0], BlockMesh.GetBoneAbsoluteTransform(model.Meshes[0].ParentBone) * transform, false, false, false, false, color);
			m_standaloneBlockMesh.TransformTextureCoordinates(tcTransform);
		}

		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, ItemBlock.Texture, Color * color, 2f * size, ref matrix, environmentData);
		}

		public override string GetCraftingId() => Id;
		public override string GetCategory(int value) => "Terrain";
		public override float GetProjectilePower(int value) => 2f;
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
		protected string Id;

		public Powder(Materials type) : this(type.ToStr() + Utils.Get("矿粉"), Colors[(int)type])
		{
			Id = type.ToString() + "OrePowder";
		}

		public Powder(string name, Color color, string description = null)
		{
			Id = name;
			DefaultDisplayName = Utils.Get(name);
			Color = color;
			DefaultTextureSlot = 198;
			DefaultDescription = description ?? name + " is powder obtained by crushing " + name + ".";
		}
		public override string GetCraftingId() => Id;
	}
	public class CoalPowder : Powder, IFuel
	{
		public readonly float HeatLevel;
		public readonly float FuelFireDuration;

		public CoalPowder(string name, Color color, float heatLevel = 1700f, float fuelFireDuration = 60f, string description = "煤粉是通过破碎煤块而得到的黑色粉末。它可以用作燃料。") : base(Utils.Get(name) + Utils.Get("粉"), color)
		{
			Id = name + "Powder";
			DefaultDescription = description;
			HeatLevel = heatLevel;
			FuelFireDuration = fuelFireDuration;
		}

		public float GetHeatLevel(int value) => HeatLevel;
		public float GetFuelFireDuration(int value) => FuelFireDuration;
	}
	public class CokeCoal : OreChunk, IFuel
	{
		public CokeCoal() : base(Matrix.CreateRotationX(1f) * Matrix.CreateRotationZ(2f), Matrix.CreateTranslation(0.0625f, 0.4375f, 0f), new Color(175, 175, 175), false, Materials.Steel)
		{
			Id = "CokeCoal";
			DefaultDisplayName = Utils.Get("焦炭");
			DefaultDescription = "焦炭看起来像炼焦煤获得的银块。 它可以用作工业领域中的燃料或还原剂。";
		}

		public override string GetCategory(int value) => "Items";
		public float GetHeatLevel(int value) => 2000f;
		public float GetFuelFireDuration(int value) => 100f;
	}
}