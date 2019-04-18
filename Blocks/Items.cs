using Engine;
using Engine.Serialization;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Game
{
	public partial class ItemBlock : CubeBlock, IItemBlock
	{
		public static Dictionary<string, int> IdTable;

		public static Item[] Items = new Item[]
		{
			new RottenEgg(),
			new MetalLine(Materials.Iron),
			new MetalLine(Materials.Copper),
			new MetalLine(Materials.Steel),
			new MetalLine(Materials.Gold),
			new MetalLine(Materials.Silver),
			new MetalLine(Materials.Platinum),
			new MetalLine(Materials.Lead),
			new MetalLine(Materials.Stannary),
			new MetalLine(Materials.Chromium),
			new MetalLine(Materials.Aluminum),
			new MetalLine(Materials.FeAlCrAlloy),
			new Resistor(Materials.FeAlCrAlloy),
			new Rod(Color.Gray)
			{
				DefaultDisplayName = "RifleBarrel",
				DefaultDescription = "Rifle Barrel are made by Rifling Machine. They are useful for making guns."
			},
			new ScrapIron(),
			new OreChunk(Matrix.CreateRotationX(4f) * Matrix.CreateRotationZ(2f), Matrix.CreateTranslation(32 % 16 / 16f, 32 / 16 / 16f, 0f), Color.White, false, Materials.Gold),
			new OreChunk(Matrix.CreateRotationX(5f) * Matrix.CreateRotationZ(2f), Matrix.CreateTranslation(33 % 16 / 16f, 33 / 16 / 16f, 0f), Color.White, false, Materials.Silver),
			new OreChunk(Matrix.CreateRotationX(6f) * Matrix.CreateRotationZ(2f), Matrix.CreateTranslation(34 % 16 / 16f, 34 / 16 / 16f, 0f), Color.White, false, Materials.Platinum),
			new OreChunk(Matrix.CreateRotationX(1f) * Matrix.CreateRotationZ(3f), Matrix.CreateTranslation(35 % 16 / 16f, 35 / 16 / 16f, 0f), Color.LightGray, false, Materials.Lead),
			new OreChunk(Matrix.CreateRotationX(1f) * Matrix.CreateRotationZ(4f), Matrix.CreateTranslation(36 % 16 / 16f, 36 / 16 / 16f, 0f), new Color(65, 224, 205), false, Materials.Zinc),
			new OreChunk(Matrix.CreateRotationX(1f) * Matrix.CreateRotationZ(5f), Matrix.CreateTranslation(37 % 16 / 16f, 37 / 16 / 16f, 0f), new Color(225, 225, 225), false, Materials.Stannary),
			new OreChunk(Matrix.CreateRotationX(1f) * Matrix.CreateRotationZ(6f), Matrix.CreateTranslation(38 % 16 / 16f, 38 / 16 / 16f, 0f), Color.White, false, Materials.Mercury),
			new OreChunk(Matrix.CreateRotationX(1f) * Matrix.CreateRotationZ(7f), Matrix.CreateTranslation(39 % 16 / 16f, 39 / 16 / 16f, 0f), Color.White, false, Materials.Chromium),
			new OreChunk(Matrix.CreateRotationX(2f) * Matrix.CreateRotationZ(-1f), Matrix.CreateTranslation(40 % 16 / 16f, 40 / 16 / 16f, 0f), new Color(190, 190, 190), false, Materials.Titanium),
			new OreChunk(Matrix.CreateRotationX(3f) * Matrix.CreateRotationZ(-1f), Matrix.CreateTranslation(41 % 16 / 16f, 41 / 16 / 16f, 0f), Color.White, false, Materials.Nickel),
			new MetalIngot(Materials.Steel),
			new MetalIngot(Materials.Gold),
			new MetalIngot(Materials.Silver),
			new MetalIngot(Materials.Platinum),
			new MetalIngot(Materials.Lead),
			new MetalIngot(Materials.Zinc),
			new MetalIngot(Materials.Nickel),
			new MetalIngot(Materials.Chromium),
			new MetalIngot(Materials.Aluminum),
			new MetalIngot(Materials.Stannary),
			new MetalIngot(Materials.FeAlCrAlloy),
			new Powder(Materials.Iron),
			new Powder(Materials.Copper),
			new Powder(Materials.Germanium),
			new Powder(Materials.Gold),
			new Powder(Materials.Silver),
			new Powder(Materials.Platinum),
			new Powder(Materials.Lead),
			new Powder(Materials.Stannary),
			new Powder(Materials.Zinc),
			new Powder(Materials.Chromium),
			new Powder(Materials.Nickel),
			new Powder(Materials.Aluminum),
			new CoalPowder("Coal", new Color(28, 28, 28)),
			new CoalPowder("CokeCoal", Color.DarkGray, 2000f, 100f, "Coke Coal Powder looks like silver powder obtained by crushing coke coal. It can be used to be fuel or the reductant agent in the industrial field."),
			new Plate(Materials.Steel),
			new Plate(Materials.Iron),
			new Plate(Materials.Copper),
			new Plate(Materials.Gold),
			new Plate(Materials.Silver),
			new Plate(Materials.Lead),
			new Plate(Materials.Zinc),
			new Plate(Materials.Stannary),
			new Plate(Materials.Platinum),
			new Plate(Materials.Aluminum),
			new SteamBoat(),
			new Train(),
			new Rod(Materials.Steel, Color.LightGray),
			new Rod(Materials.Copper, new Color(255, 127, 80)),
			new Rod(Materials.Gold, new Color(255, 215, 0)),
			new Rod(Materials.Silver, new Color(253, 253, 253)),
			new Rod(Materials.Lead, new Color(88, 87, 86)),
			new Rod(Materials.Platinum, new Color(253, 253, 253)),
			new Rod(Materials.Zinc, new Color(232, 232, 232)),
			new Rod(Materials.Stannary, new Color(232, 232, 232)),
			new Rod(Materials.Chromium, new Color(90, 90, 90)),
			new Rod(Materials.Titanium, new Color(253, 253, 253)),
			new Rod(Materials.Nickel, new Color(253, 253, 253)),
			new Rod(Materials.Aluminum, new Color(232, 232, 232)),
			new Mould("Gear", Matrix.CreateTranslation(new Vector3(0.5f)) * 2f, Matrix.CreateTranslation(4f, 3.8f, 0f), "A gear made of steel, the neccessary part of all the machine during the initial industrial era."),
			new Mould("Wheel", Matrix.CreateTranslation(new Vector3(0.5f)) * 1.2f, Matrix.CreateTranslation(4f, 3.8f, 0f), "A wheel made of steel, the neccessary part of the steam engine train.", 2f),
			new Mould("WheelMould", Matrix.CreateTranslation(0f, -0.02f, 0f) * Matrix.CreateTranslation(new Vector3(0.5f)), Matrix.CreateTranslation(2.6f, 1.4f, 0f), "A wheel mould made of dirt and sand, the neccessary part in making steel wheel.", 1.6f),
			new Mould("GearMould", Matrix.CreateTranslation(0f, -0.02f, 0f) * 1.6f * Matrix.CreateTranslation(new Vector3(0.5f)), Matrix.CreateTranslation(2.6f, 1.4f, 0f), "A gear mould made of dirt and sand, the neccessary part in making steel gear."),
			new Mould("Models/Piston", "Piston", Matrix.CreateTranslation(0.5f, 0.3f, 0.5f) * 1.2f, Matrix.CreateTranslation(4f, 3.8f, 0f), "A piston made of iron, copper and steel, the neccessary part of many machine.", "IndustrialPiston", 1.6f),
			new Wire("CopperWire"),
			new Sheet(Materials.Steel),
			new Sheet(Materials.Iron),
			new Sheet(Materials.Copper),
			new Sheet(Materials.Gold),
			new Sheet(Materials.Silver),
			new Sheet(Materials.Lead),
			new Sheet(Materials.Zinc),
			new Sheet(Materials.Stannary),
			new Sheet(Materials.Platinum),
			new Sheet(Materials.Aluminum),
			new Mould("Models/Battery", "Battery", Matrix.CreateRotationX(MathUtils.PI / 2) * Matrix.CreateTranslation(0.5f, 0.5f, 0f), Matrix.CreateTranslation(9f / 16f, -7f / 16f, 0f) * Matrix.CreateScale(20f), "工业磁铁", "IndustrialMagnet"),
			new RefractoryBrick(),
			new CokeCoal(),
			new Fan(Materials.Steel),
			new Fan(Materials.Aluminum),
			new Carriage(),
			new Airship(),
			new Sheet(Materials.Titanium),
			new CoalPowder("Sulphur", Color.Yellow, 1500f, 30f, "Sulphur is powder obtained by crushing sulphur chunk."),
			new Powder("Ashes", new Color(25, 25, 25)),
			new Powder("Slag", new Color(25, 25, 25)),
			new Powder("Sawdust", Color.LightYellow),
			new Powder("Brickbat", Color.DarkRed),
			new Powder("Broken Glass", Color.White),
			new Cylinder(Matrix.CreateScale(70f)),
			new Cylinder(Matrix.CreateScale(70f), "LPG"),
			new Cylinder(Matrix.CreateScale(70f), "LNG"),
			new Cylinder(Matrix.CreateScale(40f, 80f, 40f), "He"),
			new Cylinder(Matrix.CreateScale(40f, 80f, 40f), "Ar"),
			new Powder("Crude Salt", Color.White),
			new Powder("Refined Salt", Color.White),
			new Powder("Yeast", Color.White),
			new Powder("Coarse Silicon", Color.DarkGray),
			new Powder("Alum", Color.White),
			new Powder("Quartz Powder", Color.White),
			new Powder("Lichenin", Color.DarkGreen),
			new GranulatedItem("Talcum", Color.White),
			new Powder("Talcum Powder", Color.White),
			new Powder("Si", Color.DarkGray),
			new Sheet(Color.DarkGray)
			{
				DefaultDisplayName = "Polycrystalline Silicon",
				DefaultDescription = "Polycrystalline Silicon"
			},
			new Sheet(Color.DarkGray)
			{
				DefaultDisplayName = "Monocrystalline Silicon",
				DefaultDescription = "Monocrystalline Silicon"
			},
			new FlatItem
			{
				DefaultTextureSlot = 122,
				DefaultDisplayName = "基因查看器"
			},
			new Screwdriver(Color.White),
			new Wrench(Color.White),
			new ABomb(),
			new HBomb(),
		};
		static ItemBlock()
		{
			IdTable = new Dictionary<string, int>(Items.Length);
			for (int i = 0; i < Items.Length; i++)
				IdTable.Add(Items[i].GetCraftingId(), Index | i << 14);
		}
		/*public override void Initialize()
		{
			var reader = new StreamReader(Utils.GetTargetFile("IndustrialMod.icsv"));
			try
			{
				LoadBlocksData(reader.ReadToEnd());
			}
			catch (Exception e)
			{
				Log.Warning("\"IndustrialMod.icsv\": " + e);
			}
			finally
			{
				reader.Dispose();
			}
			base.Initialize();
		}*/
	}
}