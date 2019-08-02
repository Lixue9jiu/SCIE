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
	public class GunpowderBlock : ItemBlock, IElectricElementBlock
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
			new ABomb(),
			new HBomb(),
			new HABomb(),
			new NBomb(),
		};

		public override void Initialize()
		{
			IsTransparent = true;
			IsPlaceable = true;
			Behaviors += ",ElectricBlockBehavior";
			base.Initialize();
		}

		public override IItem GetItem(ref int value)
		{
			return Terrain.ExtractContents(value) != Index ? base.GetItem(ref value) : Items[Terrain.ExtractData(value)];
		}
		public override IEnumerable<int> GetCreativeValues()
		{
			Items[0] = new Gunpowder();
			var arr = new int[Items.Length];
			int value = Index;
			for (int i = 0; i < Items.Length; i++)
			{
				arr[i] = value;
				value += 1 << 14;
			}
			return arr;
		}

		public ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			return GetItem(ref value) is ABomb aBomb ? new ABombElectricElement(aBomb, subsystemElectricity, new CellFace(x, y, z, 4)) : null;
		}

		public ElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z)
		{
			return face == 4 ? (ElectricConnectorType?)ElectricConnectorType.Input : null;
		}

		public int GetConnectionMask(int value)
		{
			return 2147483647;
		}
	}
	public class Mine : Mould
	{
		public float ExplosionPressure;
		public double Delay;
		public MineType MineType;
		public Mine(MineType type = MineType.Medium, double delay = 0, string description = "Mine") : base("Models/Snowball", "Snowball", Matrix.CreateTranslation(Vector3.Zero), Matrix.CreateTranslation(Vector3.Zero) * Matrix.CreateScale(20f), (type & MineType.Incendiary) != 0 ? Color.DarkRed : Color.LightGray, 2.5f)
		{
			DefaultDescription = description;
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

		public ABomb(string name) : this()
		{
			DefaultDescription = DefaultDisplayName = name;
		}

		public float GetFuelFireDuration(int value) => 28f;
		public float GetHeatLevel(int value) => 7.5e3f;
		public override float GetExplosionPressure(int value) => 150f;
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			return new BlockPlacementData { Value = value, CellFace = raycastResult.CellFace };
		}
	}

	public class HBomb : ABomb, IFuel
	{
		public HBomb() : base("氢弹") { }
		public new float GetFuelFireDuration(int value) => 32f;
		public new float GetHeatLevel(int value) => 1.5e4f;
	}

	public class HABomb : ABomb, IFuel
	{
		public HABomb() : base("三相弹") { }
		public new float GetFuelFireDuration(int value) => 48f;
		public new float GetHeatLevel(int value) => 2.5e4f;
	}

	public class NBomb : ABomb
	{
		public NBomb() : base("中子弹") { }
	}

	public class ABombElectricElement : ElectricElement
	{
		IFuel Bomb;
		public ABombElectricElement(IFuel fuel, SubsystemElectricity subsystemElectricity, CellFace cellFace) : base(subsystemElectricity, cellFace) { Bomb = fuel; }

		public override bool Simulate()
		{
			if (CalculateHighInputsCount() <= 0)
				return false;
			var cellFace = CellFaces[0];
			int x = cellFace.X, y = cellFace.Y, z = cellFace.Z;
			float pressure = Bomb.GetHeatLevel(0);
			if (Bomb is NBomb)
			{
				Explode(x, y, z, 3e3f, 8);
				var e = Utils.SubsystemBodies.Bodies.GetEnumerator();
				while (e.MoveNext())
				{
					var entity = e.Current.Entity;
					entity.FindComponent<ComponentHealth>()?.Injure(1f, null, false, "Killed by radiation");
				}
				if(!(Bomb is HABomb)) return false;
			}
			int r = (int)Bomb.GetFuelFireDuration(0);
			Explode(x, y + (r >> 1), z, pressure, r);
			pressure *= 1.5f;
			Explode(x, y - (r >> 1), z, pressure * 1.5f, r);
			Explode(x + r, y, z, pressure, r);
			Explode(x - r, y, z, pressure, r);
			Explode(x, y, z + r, pressure, r);
			Explode(x, y, z - r, pressure, r);
			return false;
		}

		public static void Explode(int x, int y, int z, float pressure, int r)
		{
			var se = Utils.SubsystemExplosions;
			r >>= 1;
			//se.AddExplosion(x, y + r, z, pressure, false, false);
			//se.AddExplosion(x, y - r, z, pressure, false, false);
			se.AddExplosion(x + r, y, z, pressure, false, false);
			se.AddExplosion(x - r, y, z, pressure, false, false);
			se.AddExplosion(x, y, z + r, pressure, false, false);
			se.AddExplosion(x, y, z - r, pressure, false, false);
		}
	}
}