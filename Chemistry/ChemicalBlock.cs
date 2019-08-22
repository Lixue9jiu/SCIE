using Chemistry;
using Engine;
using System.Collections.Generic;

namespace Game
{
	public class ChemicalBlock : ItemBlock
	{
		public new const int Index = 517;
		public static readonly Group[] Cations = new[]{
			new Group("Na⁺"),
			new Group("Mg²⁺"),
			new Group("Al³⁺"),
			new Group("K⁺"),
			new Group("Ca²⁺"),
			new Group("Mn²⁺"),
			new Group("Fe²⁺"),
			new Group("Fe³⁺"),
			new Group("Ni²⁺"),
			new Group("Cu²⁺"),
			new Group("Zn²⁺"),
			new Group("Ba²⁺"),
			new Group("Ag⁺"),
			new Group("Hg²⁺"),
			new Group("Pb²⁺"),
			new Group("NH₄⁺"),
		};
		public static readonly Group[] Anions = new[]{
			new Group("OH⁻"),
			new Group("Cl⁻"),
			new Group("NO₂⁻"),
			new Group("NO₃⁻"),
			new Group("S²⁻"),
			new Group("SO₃²⁻"),
			new Group("SO₄²⁻"),
			new Group("CO₃²⁻"),
			new Group("SiO₃²⁻"),
			new Group("PO₄³⁻"),
			new Group("F⁻"),
			new Group("AsO₄³⁻"),
		};
		public new static IChemicalItem[] Items;
		static ChemicalBlock()
		{
			var list = new DynamicArray<IChemicalItem>(new IChemicalItem[]{
				new PurePowder(Materials.Steel),
				new PurePowder(Materials.Gold),
				new PurePowder(Materials.Silver),
				new PurePowder(Materials.Platinum),
				new PurePowder(Materials.Lead),
				new PurePowder(Materials.Stannary),
				new PurePowder(Materials.Zinc),
				new PurePowder(Materials.Chromium),
				new PurePowder(Materials.Nickel),
				new PurePowder(Materials.Aluminum),
				new PurePowder(Materials.Titanium),
				new PurePowder(Materials.Uranium),
				new PurePowder(Materials.Iron),
				new PurePowder(Materials.Copper),
				new PurePowder(Materials.Germanium),
				new FuelPowder("C", Color.Black),
				new FuelPowder("S", Color.Yellow, 1500f, 30f),
				new PurePowder("I₂", Color.DarkMagenta),
				new FuelCylinder("H₂", 900, 180),
				new Cylinder("O₂"),
				new Cylinder("CO₂"),
				new FuelCylinder("CO", 1100, 50),
				new Cylinder("Cl₂"),
				new Cylinder("N₂"),
				new Cylinder("NH₃"),
				new Cylinder("NO₂"),
				new Cylinder("NO"),
				new Cylinder("N₂O"),
				new Cylinder("HCl"),
				new Cylinder("SO₂"),
				new FuelCylinder("H₂S", 900, 30),
				new Cylinder("HF"),
				new FuelCylinder("PH₃", 900, 30),
				new FuelCylinder("C₂H₂", 3000, 30),
				new Cylinder("(CN)₂"),
				new Cylinder("Cl₂O"),
				new Cylinder("ClO₂"),
				new Bottle(new DispersionSystem("Br₂"), new Color(111, 21, 12)),
				new PurePowder("Na₂O"),
				new PurePowder("Na₂O₂", Color.LightYellow),
				new PurePowder("MgO"),
				new PurePowder("Al₂O₃"),
				new PurePowder("K₂O"),
				new PurePowder("CaO"),
				new PurePowder("Cr₂O₃", Color.DarkGreen),
				new PurePowder("MnO", Color.Black),
				new PurePowder("MnO₂", Color.Black),
				new PurePowder("Fe₂O₃", Color.DarkRed),
				new PurePowder("Fe₃O₄", Color.Black),
				new PurePowder("CuO", Color.Black),
				new PurePowder("Cu₂O", Color.Red),
				new PurePowder("CuCl"),
				new PurePowder("ZnO"),
				new PurePowder("Ag₂O"),
				new PurePowder("HgO", new Color(227, 23, 13)),
				new PurePowder("PbO", Color.Yellow),
				new PurePowder("PbO₂", Color.Black),
				new PurePowder("CaC₂", new Color(25, 25, 25)),
				new PurePowder("Mg₃N₂", Color.LightYellow),
				new PurePowder("SiO₂"),
				new PurePowder("SiC", new Color(25, 25, 25)),
				new PurePowder("P₂O₅"),
				new PurePowder("P₄O₆"),
				new PurePowder("PCl₃"),
				new PurePowder("PCl₅"),
				
				});
			for (int i = 0; i < Cations.Length; i++)
			{
				AtomKind atom = Cations[i].Array[0].Atom;
				Color color = atom == AtomKind.Fe
					? Cations[i].Charge == 2 ? Color.LightGreen : Color.DarkRed
					: atom == AtomKind.Cu ? Color.Blue : Color.White;
				for (int j = atom == AtomKind.Ag || Cations[i].Count == 2 ? 1 : 0; j < Anions.Length; j++)
					list.Add(new PurePowder(Cations[i] + Anions[j], color));
			}
			list.Add(new PurePowder("H₂(SiO₃)"));
			list.Add(new PurePowder("Na₂(SiO₃)"));
			list.Add(new PurePowder("Mg(SiO₃)"));
			list.Add(new PurePowder("Ca(SiO₃)"));
			list.Add(new PurePowder("Na(HCO₃)"));
			list.Add(new PurePowder("Na₂S₂O₃"));
			list.Add(new PurePowder("Ca(ClO)₂"));
			list.Add(new PurePowder("Na(ClO₂)"));
			list.Add(new PurePowder("Mg(ClO₂)₂"));
			list.Add(new PurePowder("K(ClO₂)"));
			list.Add(new PurePowder("Ca(ClO₂)₂"));
			list.Add(new PurePowder("Ba(ClO₂)₂"));
			list.Add(new PurePowder("Na(ClO₃)"));
			list.Add(new PurePowder("Mg(ClO₃)₂"));
			list.Add(new PurePowder("K(ClO₃)"));
			list.Add(new PurePowder("Ca(ClO₃)₂"));
			list.Add(new PurePowder("Ba(ClO₃)₂"));
			list.Add(new PurePowder("Na(ClO₄)"));
			list.Add(new PurePowder("Mg(ClO₄)₂"));
			list.Add(new PurePowder("K(ClO₄)"));
			list.Add(new PurePowder("Ca(ClO₄)₂"));
			list.Add(new PurePowder("Ba(ClO₄)₂"));
			list.Add(new PurePowder("Na(CN)"));
			list.Add(new PurePowder("K(CN)"));
			list.Add(new PurePowder("Ca(CN)₂"));
			list.Add(new PurePowder("K₂(HPO₄)"));
			list.Add(new PurePowder("K(H₂PO₄)"));
			list.Add(new PurePowder("Na₂(HPO₄)"));
			list.Add(new PurePowder("Na(H₂PO₄)"));
			list.Add(new PurePowder("Na₂Cr₂O₇"));
			list.Add(new PurePowder("K₂Cr₂O₇"));
			list.Add(new PurePowder("Cu₃P"));
			list.Add(new PurePowder("NH₄H"));
			list.Add(new PurePowder("LiH"));
			list.Add(new PurePowder("NaH"));
			list.Add(new PurePowder("MgH₂"));
			//list.Add(new PurePowder("AlH₃"));
			list.Add(new PurePowder("KH"));
			list.Add(new PurePowder("CaH₂"));
			list.Add(new PurePowder("Na₃P", Color.Red));
			//list.Add(new Cylinder("B₂H₆"));
			list.Add(new Cylinder("C₂H₄"));
			list.Add(new PurePowder("NaBr"));
			list.Add(new PurePowder("MgBr₂"));
			list.Add(new PurePowder("KBr"));
			list.Add(new PurePowder("CaBr₂"));
			list.Add(new PurePowder("NaI"));
			list.Add(new PurePowder("MgI₂"));
			list.Add(new PurePowder("KI"));
			list.Add(new PurePowder("CaI₂"));
			list.Add(new PurePowder("BaI₂"));
			list.Add(new PurePowder("AgBr", Color.LightYellow));
			list.Add(new PurePowder("AgI", Color.Yellow));
			list.Capacity = list.m_count;
			Items = list.Array;
		}
		/*public override CraftingRecipe GetAdHocCraftingRecipe(SubsystemTerrain subsystemTerrain, string[] ingredients, float heatLevel)
		{
			return base.GetAdHocCraftingRecipe(subsystemTerrain, ingredients, heatLevel);
		}*/
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
		public override string GetCategory(int value) => Utils.Get("化学");
	}
	public class Cylinder : Mould, IChemicalItem
	{
		public readonly DispersionSystem System;

		public Cylinder(string name) : base("Models/Cylinder", "obj1", Matrix.CreateScale(40f, 80f, 40f) * Matrix.CreateTranslation(0.5f, 0f, 0.5f), Matrix.CreateTranslation(9f / 16f, -7f / 16f, 0f), null, name, 1.5f)
		{
			DefaultDescription = DefaultDisplayName = (System = new DispersionSystem(name)).ToString();
		}
		public Cylinder(Matrix matrix, string name = "钢瓶") : base("Models/Cylinder", "obj1", matrix * Matrix.CreateTranslation(0.5f, 0f, 0.5f), Matrix.CreateTranslation(9f / 16f, -7f / 16f, 0f), null, name, 1.5f)
		{
			DefaultDescription = DefaultDisplayName = Utils.Get(name);
		}
		public override string GetCategory() => Utils.Get("化学");
		public DispersionSystem GetDispersionSystem() => System;

		public override int GetDamageDestructionValue()
		{
			return ItemBlock.IdTable["Cylinder"];
		}
	}
	public class FuelCylinder : Cylinder, IFuel
	{
		public readonly float HeatLevel;
		public readonly float FuelFireDuration;
		public FuelCylinder(string name, float heatLevel, float fuelFireDuration) : base(name)
		{
			HeatLevel = heatLevel;
			FuelFireDuration = fuelFireDuration;
		}
		public FuelCylinder(Matrix matrix, string name, float heatLevel, float fuelFireDuration) : base(matrix, name)
		{
			HeatLevel = heatLevel;
			FuelFireDuration = fuelFireDuration;
		}

		public float GetHeatLevel(int value) => HeatLevel;
		public float GetFuelFireDuration(int value) => FuelFireDuration;
	}
	public class Bottle : MeshItem, IChemicalItem
	{
		public string Id;
		public readonly DispersionSystem System;

		public Bottle(DispersionSystem system, Color color = default(Color)) : this(system.ToString(), null, color)
		{
			System = system;
		}

		public Bottle(string name, string id = null, Color color = default(Color)) : base(name)
		{
			System = DispersionSystem.Air;
			Id = id ?? name;
			DefaultDisplayName = DefaultDescription;
			if (color.PackedValue != 0u)
				m_standaloneBlockMesh.AppendMesh("Models/Glass", "content", Matrix.CreateRotationX(MathUtils.PI) * Matrix.CreateScale(0.5f, 0.5f, 0.3f) * Matrix.CreateTranslation(0f, -0.4f, 0f), Matrix.CreateTranslation(-1 / 16f, 0f, 0f), color);
			m_standaloneBlockMesh.AppendMesh("Models/Glass", "glassbottle", Matrix.CreateRotationX(MathUtils.PI) * Matrix.CreateScale(0.5f, 0.5f, 0.3f) * Matrix.CreateTranslation(0f, -0.4f, 0f), Matrix.CreateTranslation(-1 / 16f, 0f, 0f), new Color(255, 255, 255, 64));
		}

		public override string GetCraftingId() => Id;
		public DispersionSystem GetDispersionSystem() => System;
	}
	public class PurePowder : Powder, IChemicalItem
	{
		public readonly DispersionSystem System;

		public PurePowder(string name) : this(new DispersionSystem(name), Color.White)
		{
		}

		public PurePowder(Materials type) : base(type.ToStr() + Utils.Get("粉"), type.ToId(), Colors[(int)type])
		{
		}

		public PurePowder(string name, Color color) : this(new DispersionSystem(name), color)
		{
		}
		public PurePowder(DispersionSystem system, Color color) : base("", "", color) => DefaultDescription = DefaultDisplayName = (System = system).ToString();
		public DispersionSystem GetDispersionSystem() => System;
	}
	public class FuelPowder : PurePowder, IFuel
	{
		public readonly float HeatLevel;
		public readonly float FuelFireDuration;

		public FuelPowder(string name, Color color, float heatLevel = 1700f, float fuelFireDuration = 60f, string description = "") : base(name, color)
		{
			DefaultDescription = description;
			HeatLevel = heatLevel;
			FuelFireDuration = fuelFireDuration;
		}

		public float GetHeatLevel(int value) => HeatLevel;
		public float GetFuelFireDuration(int value) => FuelFireDuration;
	}
}