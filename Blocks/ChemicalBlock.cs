﻿using Chemistry;
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
		};
		public new static IChemicalItem[] Items = new IChemicalItem[]{
			new Cylinder("H₂"),
			new Cylinder("O₂"),
			new Cylinder("CO₂"),
			new Cylinder("CO"),
			new Cylinder("Cl₂"),
			new Cylinder("N₂"),
			new Cylinder("NH₃"),
			new Cylinder("NO₂"),
			new Cylinder("NO"),
			new Cylinder("N₂O"),
			new Cylinder("HCl"),
			new Cylinder("SO₂"),
			new Cylinder("H₂S"),
			new Cylinder("HF"),
			new Cylinder("PH₃"),
			new Cylinder("C₂H₂"),
			new Cylinder("(CN)₂"),
			new PurePowder("Na₂O", Color.White),
			new PurePowder("Na₂O₂", Color.LightYellow),
			new PurePowder("MgO", Color.White),
			new PurePowder("Al₂O₃", Color.White),
			new PurePowder("K₂O", Color.White),
			new PurePowder("CaO", Color.White),
			new PurePowder("Cr₂O₃", Color.DarkGreen),
			new PurePowder("MnO", Color.Black),
			new PurePowder("MnO₂", Color.Black),
			new PurePowder("Fe₂O₃", Color.DarkRed),
			new PurePowder("Fe₃O₄", Color.Black),
			new PurePowder("CuO", Color.Black),
			new PurePowder("Cu₂O", Color.Red),
			new PurePowder("ZnO", Color.White),
			new PurePowder("Ag₂O", Color.White),
			new PurePowder("HgO", new Color(227, 23, 13)),
			new PurePowder("PbO", Color.Yellow),
			new PurePowder("PbO₂", Color.Black),
			new PurePowder("CaC₂", new Color(25, 25, 25)),
			new PurePowder("Mg₃N₂", Color.LightYellow),
			new PurePowder("SiO₂", Color.White),
			new PurePowder("SiC", new Color(25, 25, 25)),
			new PurePowder("P₂O₅", Color.White),
		};
		static ChemicalBlock()
		{
			var list = new List<IChemicalItem>(Items);
			for (int i = 0; i < Cations.Length; i++)
			{
				AtomKind atom = Cations[i][0].Atom;
				Color color = atom == AtomKind.Fe
					? Cations[i].Charge == 2 ? Color.LightGreen : Color.DarkRed
					: atom == AtomKind.Cu ? Color.Blue : Color.White;
				for (int j = atom == AtomKind.Ag || Cations[i].Count == 2 ? 1 : 0; j < Anions.Length; j++)
				{
					list.Add(new PurePowder(Cations[i] + Anions[j], color));
				}
			}
			list.Add(new PurePowder("H₂(SiO₃)", Color.White));
			list.Add(new PurePowder("Na₂(SiO₃)", Color.White));
			list.Add(new PurePowder("Mg(SiO₃)", Color.White));
			list.Add(new PurePowder("Ca(SiO₃)", Color.White));
			list.Add(new PurePowder("Na(HCO₃)", Color.White));
			list.Add(new PurePowder("Na₂S₂O₃", Color.White));
			list.Add(new PurePowder("Na(ClO)", Color.White));
			list.Add(new PurePowder("Ca(ClO)₂", Color.White));
			list.Add(new PurePowder("K(CN)", Color.White));
			list.Add(new PurePowder("Na(CN)", Color.White));
			Items = list.ToArray();
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
	}
	public class Cylinder : Mould, IChemicalItem
	{
		public readonly DispersionSystem System;

		public Cylinder(string name) : base("Models/Cylinder", "obj1", Matrix.CreateScale(40f, 80f, 40f) * Matrix.CreateTranslation(0.5f, 0f, 0.5f), Matrix.CreateTranslation(9f / 16f, -7f / 16f, 0f), null, name, 1.5f)
		{
			DefaultDescription = DefaultDisplayName = (System = new DispersionSystem(name)).ToString();
		}
		public override string GetCategory(int value)
		{
			return "Chemical";
		}
		public DispersionSystem GetDispersionSystem()
		{
			return System;
		}
	}
	public class PurePowder : Powder, IChemicalItem
	{
		public readonly DispersionSystem System;

		public PurePowder(string name, Color color) : this(new DispersionSystem(name), color)
		{
		}
		public PurePowder(DispersionSystem system, Color color) : base("", color)
		{
			DefaultDescription = DefaultDisplayName = (System = system).ToString();
		}
		public override string GetCategory(int value)
		{
			return "Chemical";
		}
		public DispersionSystem GetDispersionSystem()
		{
			return System;
		}
	}
}