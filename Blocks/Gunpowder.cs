using Engine;
using Engine.Graphics;
using System.Collections.Generic;
using System.Text;

namespace Game
{
	public class Gunpowder : OreChunk, IFuel
	{
		public Gunpowder() : base(Matrix.CreateScale(0.75f) * Matrix.CreateRotationX(4f) * Matrix.CreateRotationZ(3f), Matrix.CreateScale(1f) * Matrix.CreateTranslation(0.0625f, 0.875f, 0f), Color.White, false, Materials.Steel)
		{
			DefaultDisplayName = "Gunpowder";
			DefaultDescription = BlocksManager.Blocks[GunpowderBlock.Index].DefaultDescription;
		}
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, color, 2f * size, ref matrix, environmentData);
		}
		public override string GetCategory(int value)
		{
			return "Items";
		}
		public float GetHeatLevel(int value)
		{
			return 1650f;
		}
		public float GetFuelFireDuration(int value)
		{
			return 0.1f;
		}
	}
	public class PureGunpowder : Powder, IFuel
	{
		public readonly float ExplosionPressure;
		public PureGunpowder(string name, Color color, float ep = 100f) : base(name, color)
		{
			ExplosionPressure = ep;
		}

		public override float GetExplosionPressure(int value)
		{
			return ExplosionPressure;
		}
		public override bool GetExplosionIncendiary(int value)
		{
			return true;
		}
		public float GetHeatLevel(int value)
		{
			return 18.5f * ExplosionPressure;
		}
		public float GetFuelFireDuration(int value) => .1f;
	}
	public class GunpowderBlock : ItemBlock
	{
		public new const int Index = 109;
		public new static Item[] Items = new Item[]
		{
			null,
			new PureGunpowder("精制炸药", new Color(24, 24, 24)),
			new PureGunpowder("TNT", Color.Yellow, 150f),
			new PureGunpowder("阿马托强力炸药", Color.Black, 180f),
			new PureGunpowder("阿莫丁炸药", new Color(48, 48, 48), 110f),
			new PureGunpowder("阿莫格尔炸药", Color.Gray, 90f),
			new PureGunpowder("葛里炸药", Color.DarkYellow, 100f),
			new PureGunpowder("立德炸药", Color.Gray),
			new PureGunpowder("罗必赖特炸药", new Color(72, 72, 72), 90f),
			new PureGunpowder("木炸药", Color.DarkYellow, 90f),
		};

		static GunpowderBlock()
		{
			var list = new List<Item>(Items);
			list.AddRange(Mine.Mines);
			Items = list.ToArray();
			for (int i = 1; i < Items.Length; i++)
				IdTable.Add(Items[i].GetCraftingId(), Index | i << 14);
		}

		public override IItem GetItem(ref int value)
		{
			return Terrain.ExtractContents(value) != Index ? base.GetItem(ref value) : Items[Terrain.ExtractData(value)];
		}
		public override IEnumerable<int> GetCreativeValues()
		{
			var arr = new int[Items.Length];
			int value = Index;
			for (int i = 0; i < Items.Length; i++)
			{
				arr[i] = value;
				value += 1 << 14;
			}
			return arr;
		}

		public override void Initialize()
		{
			base.Initialize();
			Items[0] = new Gunpowder();
		}
	}
	public class Mine : Mould
	{
		public static Mine[] Mines = new Mine[2048];
		public float ExplosionPressure;
		public double Delay;
		public MineType MineType;
		static Mine()
		{
			for (int i = 0; i < 2048; i++)
				Mines[i] = new Mine((MineType)(i & 63), (i >> 6) / 4.0);
		}
		public Mine(MineType type = MineType.Medium, double delay = 0, string description = "Mine") : base("Models/Snowball", "Snowball", Matrix.CreateTranslation(Vector3.Zero), Matrix.CreateTranslation(Vector3.Zero) * Matrix.CreateScale(20f), (type & MineType.Incendiary) != 0 ? Color.DarkRed : Color.LightGray, description, "", 2.5f)
		{
			switch (MineType & MineType.Large)
			{
				case MineType.Tiny: ExplosionPressure = 50f; break;
				case MineType.Small: ExplosionPressure = 80f; break;
				case MineType.Medium: ExplosionPressure = 120f; break;
				case MineType.Large: ExplosionPressure = 300f; break;
			}
			if ((type & MineType.Incendiary) != 0)
				ExplosionPressure *= 0.9f;
			Delay = delay;
			MineType = type;
		}
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value) => GetCraftingId();
		public override string GetCategory(int value) => Utils.Get("地雷");
		public override string GetCraftingId()
		{
			var sb = new StringBuilder();
			string s;
			switch (MineType & MineType.Large)
			{
				case MineType.Tiny: s = "微型"; break;
				case MineType.Small: s = "小型"; break;
				case MineType.Medium: s = "中型"; break;
				default: s = "大型"; break;
			}
			sb.Append(Utils.Get(s));
			if ((MineType & MineType.Incendiary) != 0)
				sb.Append(Utils.Get("易燃"));
			if ((MineType & MineType.Sensitive) != 0)
				sb.Append(Utils.Get("敏感"));
			if ((MineType & MineType.FL) != 0)
				sb.Append(Utils.Get("松发"));
			if (Delay > 0.0)
			{
				sb.Append(Delay.ToString());
				sb.Append(Utils.Get("秒延迟"));
			}
			sb.Append(Utils.Get((MineType & MineType.Torpedo) == 0 ? "地雷" : "水雷"));
			return sb.ToString();
		}
	}
	public class ABomb : Mould, IFuel
	{
		public ABomb() : base("Models/Nuclearbomb", "Nuclearbomb", Matrix.CreateTranslation(0.2f, -0.2f, 0.2f) * Matrix.CreateScale(2f), Matrix.Identity, "原子弹", "原子弹")
		{
		}
		public override float GetExplosionPressure(int value) => 8e5f;

		public float GetFuelFireDuration(int value) => .1f;

		public float GetHeatLevel(int value) => 1e5f;
	}
	public class HBomb : ABomb
	{
		public HBomb()
		{
			DefaultDescription = DefaultDisplayName = "氢弹";
		}

		public override float GetExplosionPressure(int value) => 1.6e6f;
	}
}