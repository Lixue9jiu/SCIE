﻿using Engine;
using Engine.Graphics;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Game
{
	public partial class ItemBlock
	{
		static ItemBlock()
		{
			if (Task == null)
				Task = Task.Run((Action)Load);
			TexturedMeshItem.WhiteTexture = new Texture2D(1, 1, false, ColorFormat.Rgba8888);
			TexturedMeshItem.WhiteTexture.SetData(0, new byte[] { 255, 255, 255, 255 });
			var stream = Utils.GetTargetFile("IndustrialMod.png");
			try
			{
				Texture = Texture2D.Load(stream);
			}
			finally { stream.Close(); }
		}

		internal static void Load()
		{
			var m = Matrix.CreateTranslation(0.5f, 0.4f, 0.5f);
			var m2 = Matrix.CreateTranslation(13f / 16f, -3f / 16f, 0f) * Matrix.CreateScale(20f);
			Items = new Item[] {
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
			new Rod("RifleBarrel", "Rifle Barrel are made by Rifling Machine. They are useful for making guns.", Color.Gray),
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
			new MetalIngot(Materials.Brass),
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
			new Powder(Materials.Titanium),
			new Powder(Materials.Uranium),
            //new Powder(Materials.Brass),
            new CoalPowder("Coal", new Color(28, 28, 28)),
			new CoalPowder("CokeCoal", Color.DarkGray, 2000f, 100f, "焦炭粉看起来像银粉，通过压碎焦炭获得。 它可以用作工业领域中的燃料或还原剂。"),
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
			new Plate(Materials.Brass),
			new SteamBoat(),
			new Train(),
			new Rod(Materials.Steel),
			new Rod(Materials.Copper),
			new Rod(Materials.Gold),
			new Rod(Materials.Silver),
			new Rod(Materials.Lead),
			new Rod(Materials.Platinum),
			new Rod(Materials.Zinc),
			new Rod(Materials.Stannary),
			new Rod(Materials.Chromium),
			new Rod(Materials.Titanium),
			new Rod(Materials.Nickel),
			new Rod(Materials.Aluminum),
			new Rod(Materials.Brass),
			new Mould("Gear", Matrix.CreateTranslation(new Vector3(0.5f)) * 2f, Matrix.CreateTranslation(4f, 3.8f, 0f), "A gear made of steel, the neccessary part of all the machine during the initial industrial era."),
			new Mould("Wheel", Matrix.CreateTranslation(new Vector3(0.5f)) * 1.2f, Matrix.CreateTranslation(4f, 3.8f, 0f), "A wheel made of steel, the neccessary part of the steam engine train.", 2f),
			new Mould("WheelMould", Matrix.CreateTranslation(0f, -0.02f, 0f) * Matrix.CreateTranslation(new Vector3(0.5f)), Matrix.CreateTranslation(2.6f, 1.4f, 0f), "A wheel mould made of dirt and sand, the neccessary part in making steel wheel.", 1.6f),
			new Mould("GearMould", Matrix.CreateTranslation(0f, -0.02f, 0f) * 1.6f * Matrix.CreateTranslation(new Vector3(0.5f)), Matrix.CreateTranslation(2.6f, 1.4f, 0f), "A gear mould made of dirt and sand, the neccessary part in making steel gear."),
			new Mould("Models/Piston", "Piston", Matrix.CreateTranslation(0.5f, 0.3f, 0.5f) * 1.2f, Matrix.CreateTranslation(4f, 3.8f, 0f), "A piston made of iron, copper and steel, the neccessary part of many machine.", "IndustrialPiston", 1.6f),
			new Wire("CopperWire"),
			new Plate(Materials.Steel, true),
			new Plate(Materials.Iron, true),
			new Plate(Materials.Copper, true),
			new Plate(Materials.Gold, true),
			new Plate(Materials.Silver, true),
			new Plate(Materials.Lead, true),
			new Plate(Materials.Zinc, true),
			new Plate(Materials.Stannary, true),
			new Plate(Materials.Platinum, true),
			new Plate(Materials.Aluminum, true),
			new Plate(Materials.Brass, true),
			new Mould("Models/Battery", "Battery", Matrix.CreateRotationX(MathUtils.PI / 2) * Matrix.CreateTranslation(0.5f, 0.5f, 0f), Matrix.CreateTranslation(9f / 16f, -7f / 16f, 0f) * Matrix.CreateScale(20f), "工业磁铁", "IndustrialMagnet"),
			new Brick("RefractoryBrick", "RefractoryBrick", new Color(255, 153, 18), Matrix.CreateTranslation(-32 % 16 / 16f, -32 / 16 / 16f, 0f), "耐火砖是一种耐火陶瓷材料，用于炉衬炉，窑炉，高级燃烧室和壁炉。 它具有耐高温性能，但导热系数低，能效高。"),
			new CokeCoal(),
			new Fan(Materials.Steel),
			new Fan(Materials.Aluminum),
			new Carriage(),
			new Airship(),
			new Plate(Materials.Titanium, true),
			new Icebreaker(),
			new CoalPowder("Sulphur", Color.Yellow, 1500f, 30f, "硫粉是通过破碎硫块而得到的黄色粉末。它可以用作燃料。"),
			new Powder("煤灰", new Color(25, 25, 25)),
			new Powder("海带灰", Color.DarkGreen),
			new Bucket("海带灰溶液", Color.LightGreen),
			new Powder("矿渣", new Color(40, 40, 40)),
			new Powder("木屑", new Color(255, 255, 160)),
			new Powder("碎砖", Color.DarkRed),
			new Powder("碎玻璃", Color.White),
			new Cylinder(Matrix.CreateScale(70f)),
			new FuelCylinder(Matrix.CreateScale(70f), "液化石油气", 1100, 300),
			new FuelCylinder(Matrix.CreateScale(70f), "液化天然气", 700, 240),
			new Cylinder(Matrix.CreateScale(40f, 80f, 40f), "He"),
			new Cylinder(Matrix.CreateScale(40f, 80f, 40f), "Ar"),
			new Powder("粗盐", Color.White),
			new Powder("精盐", Color.White),
			new Brick("石墨", "石墨", new Color(44, 44, 44), Matrix.CreateTranslation(-32 % 16 / 16f, -32 / 16 / 16f, 0f), "石墨"),
			new Powder("粗硅", Color.DarkGray),
			new Powder("明矾", Color.White),
			new Powder("石英砂", Color.White),
			new Powder("苔粉", Color.DarkGreen),
			new FlatItem()
			{
				DefaultDisplayName = Utils.Get("滑石"),
				DefaultTextureSlot = 231,
				Color = Color.White
			},
			new Powder("滑石粉", Color.White),
			new FlatItem
			{
				DefaultTextureSlot = 122,
				DefaultDisplayName = "基因查看器"
			},
			new MouldItem("Screwdriver", "Models/Screwdriver", "obj1", Matrix.CreateTranslation(0f, -0.33f, 0f), Matrix.CreateTranslation(15f / 16f, 0f, 0f), "螺丝刀可以快速拆除机器", "螺丝刀", 3.3f),
			new MouldItem("Wrench", "Models/Wrench", "obj1", Matrix.CreateScale(1.2f), Matrix.CreateTranslation(15f / 16f, 0f, 0f), "扳手可以拆除交通工具", "扳手", 6f),
			new OreChunk(Matrix.CreateRotationX(1f) * Matrix.CreateRotationZ(1f), Matrix.CreateTranslation(0.0625f, 0.4375f, 0f), Color.White, false, Materials.Steel)
			{
				Id = "Plaster",
				DefaultDisplayName = "石膏",
				DefaultDescription = "石膏"
			},
			new Powder("石膏粉", Color.White),
			new Plate("多晶硅", Color.DarkGray, true),
			new Plate("单晶硅", Color.DarkGray, true),
			new Powder("聚乙烯", Color.White),
			new Powder("聚丙烯", Color.White),
			new Plate("聚乙烯板", Color.White),
			new Plate("聚乙烯片", Color.White, true),
			new Mould("Models/Piston2", "Cylinder", Matrix.CreateTranslation(0.5f, 0.7f, 0.5f) * Matrix.CreateScale(0.6f), Matrix.CreateTranslation(4f, 3.8f, 0f), "A Cylinder made of alloy, Aluminum and steel, the neccessary part of many machine.", "Cylinder", 1.6f),
			new LightMould("Models/Snowball", "Snowball", m, m2, "A Ball of Melting Iron.", "MeltingIron", 2.9f),
			new LightMould("Models/Snowball", "Snowball", m, m2, "A Ball of Melting Chromium.", "MeltingChromium", 2.9f),
			new LightMould("Models/Snowball", "Snowball", m, m2, "A Ball of Melting Nickel.", "MeltingNickel", 2.9f),
			new LightMould("Models/Snowball", "Snowball", m, m2, "A Ball of Melting Titanium.", "MeltingTitanium", 2.9f),
			new LightMould("Models/Snowball", "Snowball", m, m2, "A Ball of Melting Aluminium.", "MeltingAluminium", 2.9f),
			new LightMould("Models/Snowball", "Snowball", m, m2, "A Ball of Melting Copper.", "MeltingCopper", 2.9f),
			new LightMould("Models/Snowball", "Snowball", m, m2, "A Ball of Melting Zinc.", "MeltingZinc", 2.9f),
			new Alloy(Materials.Steel,"Stainless Steel"),
			new Alloy(Materials.Aluminum, "Super Aluminium"),
			new Alloy(Materials.Steel, "Si-Steel"),
			new Alloy(Materials.Steel,"Industrial Steel"),
			new Alloy(Materials.Copper,"Industrial Copper"),
			new Alloy(Materials.Steel,"Titanium Steel"),
			new Alloy(Materials.Chromium,"Gun-Steel"),
			new Powder("酵母", Color.White),
			new Brick("混凝土砖", "ConcreteBrick", Color.Gray, Matrix.CreateTranslation(-32 % 16 / 16f, -32 / 16 / 16f, 0f), "混凝土砖"),
			new Mould("Models/Snowball", "Snowball", Matrix.CreateTranslation(0.5f, 0.4f, 0.5f), Matrix.CreateTranslation(12f / 16f, -1f / 16f, 0f) * Matrix.CreateScale(20f), "A Ball of Rubber, an important component in the advanced industry", "Rubber", 2.6f),
			new Rod(Materials.Uranium),
			new FoodCan("EmptyCan can be used to make food can","Empty",Color.Gray),
			new FoodCan("MeatCan can store meat for a long time","Meat",Color.Gray),
			new FoodCan("ChickenCan can store Chicken for a long time","Chicken",Color.Gray),
			new FoodCan("PumpkinCan can store Pumpkin for a long time","Pumpkin",Color.Gray),
			new FoodCan("BreadCan can store Bread for a long time","Bread",Color.Gray),
			new FoodCan("FishCan can store Fish for a long time","Fish",Color.Gray),
			new Brick("玻璃砖", "玻璃砖", new Color(255, 255, 255, 32), Matrix.CreateTranslation(-32 % 16 / 16f, -32 / 16 / 16f, 0f), "玻璃砖"),
			new Car(),
			new MouldItem("Rectifier", "Models/MotionDetector", "MotionDetector", Matrix.CreateTranslation(new Vector3(0.5f)), Matrix.CreateScale(20f), "整流器", "整流器", 2f),
			new MouldItem("ScR", "Models/Photodiode", "Photodiode", Matrix.CreateTranslation(new Vector3(0.5f)), Matrix.CreateScale(20f), "可控硅", "可控硅", 2f),
			new MouldItem("LEDSheet", "Models/MotionDetector", "MotionDetector", Matrix.CreateTranslation(new Vector3(0.5f)), Matrix.CreateScale(20f), "LED贴片", "LED贴片", 2f),
			//new Mould("Models/OBox", "Cube", Matrix.CreateTranslation(0.5f, 0.7f, 0.5f) * Matrix.CreateScale(0.6f), Matrix.CreateTranslation(4f, 3.8f, 0f), "A Cylinder made of alloy, Aluminum and steel, the neccessary part of many machine.", "Cube", 1.6f),
			new Spring("弹簧", "弹簧"),
			new Springboard("弹跳板", "弹跳板"),
			new Plate("散热片", Color.White, true)
			.AppendMesh("Models/Ingots", "IronPlate", Matrix.CreateTranslation(0.5f, -0.6f, 0.5f) * Matrix.CreateScale(.5f), Matrix.Identity, Color.White)
			.AppendMesh("Models/Ingots", "IronPlate", Matrix.CreateTranslation(0.5f, -0.3f, 0.5f) * Matrix.CreateScale(.5f), Matrix.Identity, Color.White)
			.AppendMesh("Models/Ingots", "IronPlate", Matrix.CreateTranslation(0.5f, 0.3f, 0.5f) * Matrix.CreateScale(.5f), Matrix.Identity, Color.White)
			.AppendMesh("Models/Ingots", "IronPlate", Matrix.CreateTranslation(0.5f, 0.6f, 0.5f) * Matrix.CreateScale(.5f), Matrix.Identity, Color.White),
			new Rod(Materials.Plastic),
			new Mould("Models/Battery", "Battery", Matrix.CreateTranslation(new Vector3(0.5f)) * Matrix.CreateScale(2f, 0.05f, 2f), Matrix.CreateTranslation(9f / 16f, -7f / 16f, 0f) * Matrix.CreateScale(20f), "晶圆", "晶圆"),
			/*nnew Plate("128K RAM", Color.DarkGreen, true),
			new Plate("256K RAM", Color.DarkGreen, true),
			new Plate("512K RAM", Color.DarkGreen, true),
			new Plate("RISC CPU", Color.DarkGray, true),
			new Plate("CISC CPU", Color.DarkGray, true),
			new CPanel(),*/
			};
			ElementBlock.Devices = new Device[]
			{
				new Fridge(),
				new Generator(),
				new Magnetizer(),
				new Separator(),
				new AirBlower(),
				new WireDevice(),
				new EFurnace(),
				new Canpack(),
				new Electrobath(), //8
				new Battery(Matrix.CreateTranslation(7f / 16f, 6f / 16f, 0f), "Cu-Zn电池", "Cu-Zn电池", "CuZnBattery"),
				new Battery(Matrix.CreateTranslation(7f / 16f, 6f / 16f, 0f), "Ag-Zn电池", "Ag-Zn电池", "AgZnBattery")
				{
					Voltage = 13
				},
				new Battery(Matrix.CreateTranslation(7f / 16f, 6f / 16f, 0f), "Au-Zn电池", "Au-Zn电池", "AuZnBattery")
				{
					Voltage = 15
				},
				new Battery(Matrix.CreateTranslation(-2f / 16f, 4f / 16f, 0f), "伏打电池", "伏打电池", "VBattery"),
				new Pipe(0),
				new Pipe(1),
				new Pipe(2),
				new Pipe(3),
				new Pipe(4),
				new Pipe(5),
				new Pipe(6),
				new Pipe(7),
				new AirCompressor(),
				new Switch(),
				new Relay(),
				new EIFurnace(),
				new SolarPanel("多晶硅太阳能电池板", 100),
				new SolarPanel("单晶硅太阳能电池板", 120),
				new ElectricMotor(),
				new TGenerator(),
				new ACGenerator(),
				new Condenser(), //30
				new AirPump(),
				new LED(),
				new ElectricFences(),
				new Transformer(),
				new Charger(),
				new Recycler(),
				new Gearbox(),
				new MachRod(),
				new Inverter(),
				new Stabilizer(),
				new Fuseblock(),
				new UThickener(),
				new WaterExtractor(),
				new Unpacker(),
				new ElectricPump(),
				new OilPlant(),
				new WaterCuttingMachine(),
				new TEDC(),
				new ElectricDriller(),
			};
			IdTable = new Dictionary<string, int>(Items.Length)
			{
				{ "Diamond", DiamondChunkBlock.Index }
			};
			int i;
			for (i = 0; i < Items.Length; i++)
				IdTable.Add(Items[i].GetCraftingId(), Index | i << 14);
			for (i = 0; i < ElementBlock.Devices.Length; i++)
			{
				if (ElementBlock.Devices[i] is CubeDevice device)
					device.Index = i;
				IdTable.Add(ElementBlock.Devices[i].GetCraftingId(), ElementBlock.Index | i << 14);
			}
			var list = new DynamicArray<Item>(Chemistry.GunpowderBlock.Items)
			{
				Capacity = Chemistry.GunpowderBlock.Items.Length
			};
			for (i = 0; i < 1024; i++)
				list.Add(new Mine((MineType)(i & 63), (i >> 6) / 4.0));
			list.Capacity = list.Count;
			Chemistry.GunpowderBlock.Items = list.Array;
			for (i = 1; i < Chemistry.GunpowderBlock.Items.Length; i++)
				IdTable.Add(Chemistry.GunpowderBlock.Items[i].GetCraftingId(), GunpowderBlock.Index | i << 14);
			/*var stream = Utils.GetTargetFile("IndustrialEquations.txt");
			Equation.Reactions = new HashSet<Equation>();
			try
			{
				var reader = new StreamReader(stream);
				while (true)
				{
					var line = reader.ReadLine();
					if (line == null) break;
					Equation.Reactions.Add(Equation.Parse(line));
				}
			}
			finally { stream.Close(); }*/
			var stream = Utils.GetTargetFile("IndustrialMod_en-us.lng", false);
			if (stream == null) return;
			try
			{
				Utils.ReadKeyValueFile(Utils.TR, stream);
			}
			finally { stream.Close(); }
		}

		public static Dictionary<string, int> IdTable;
		public static Item[] Items;
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
			finally { reader.Dispose(); }
			base.Initialize();
		}*/
	}
}