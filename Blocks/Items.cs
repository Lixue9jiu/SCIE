using Chemistry;
using Engine;
using Engine.Graphics;
using Engine.Media;
using LibPixz;
using System;
using System.Collections.Generic;
using System.IO;

namespace Game
{
	partial class ItemBlock
	{
		internal static void InitItems()
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
            new CoalPowder("Coal", new Color(28, 28, 28), 1700, 60, "煤粉是通过破碎煤块而得到的黑色粉末。它可以用作燃料。"),
			new CoalPowder("CokeCoal", Color.DarkGray, 2000, 100, "焦炭粉看起来像银粉，通过压碎焦炭获得。 它可以用作工业领域中的燃料或还原剂。"),
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
			new Brick("耐火砖", "RefractoryBrick", new Color(255, 153, 18), Matrix.CreateTranslation(-32 % 16 / 16f, -32 / 16 / 16f, 0f), "耐火砖是一种耐火陶瓷材料，用于炉衬炉，窑炉，高级燃烧室和壁炉。 它具有耐高温性能，但导热系数低，能效高。"),
			new CokeCoal(),
			new Fan(Materials.Steel),
			new Fan(Materials.Aluminum),
			new Carriage(),
			new Airship(),
			new Plate(Materials.Titanium, true),
			new Icebreaker(),
			new CoalPowder("Sulphur", Color.Yellow, 1500f, 30f, "硫粉是通过破碎硫块而得到的黄色粉末。它可以用作燃料。"),
			new Powder("煤渣", "Ashes", new Color(125, 70, 70)),
			new Powder("海带灰", "KelpAshes", Color.DarkGreen),
			new Bucket("海带灰溶液", Color.DarkGreen),
			new Powder("矿渣", "Slag", new Color(40, 40, 40)),
			new Powder("木屑", "Sawdust", new Color(255, 255, 160)),
			new Powder("碎砖", "Brickbat", Color.DarkRed),
			new Powder("碎玻璃", "BrokenGlass", Color.White),
			new Cylinder(Matrix.CreateScale(70f)),
			new FuelCylinder(Matrix.CreateScale(70f), "液化石油气", 1100, 300),
			new FuelCylinder(Matrix.CreateScale(70f), "液化天然气", 700, 240),
			new Cylinder(Matrix.CreateScale(40f, 80f, 40f), "He"),
			new Cylinder(Matrix.CreateScale(40f, 80f, 40f), "Ar"),
			new Powder("粗盐", "CrudeSalt", Color.White),
			new Brick("石墨", "Graphite", new Color(44, 44, 44), Matrix.CreateTranslation(-32 % 16 / 16f, -32 / 16 / 16f, 0f), "石墨"),
			new PurePowder("Si", Color.DarkGray)
			{
				DefaultDisplayName = "粗硅"
			},
			new Powder("明矾", "Alum", Color.White),
			new Powder("石英砂", "QuartzPowder", Color.White),
			new Powder("苔粉", "Lichenin", Color.DarkGreen),
			new FlatItem()
			{
				DefaultDisplayName = Utils.Get("滑石"),
				DefaultTextureSlot = 231,
				Color = Color.White
			},
			new Powder("滑石粉", "TalcumPowder", Color.White),
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
			new Powder("石膏粉", "CaSO4", Color.White),
			new Powder("冰晶石", "Cryolite", Color.White),
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
			new Powder("酵母", "Yeast", Color.White),
			new Brick("混凝土砖", "ConcreteBrick", Color.Gray, Matrix.CreateTranslation(-32 % 16 / 16f, -32 / 16 / 16f, 0f), "混凝土砖"),
			new Brick("多孔混凝土砖", "PorousConcreteBrick", Color.Gray, Matrix.CreateTranslation(-32 % 16 / 16f, -32 / 16 / 16f, 0f), "多孔混凝土砖"),
			new Plate("多晶硅", Color.DarkGray, true),
			new Plate("单晶硅", Color.DarkGray, true),
			new Mould("Models/Snowball", "Snowball", Matrix.CreateTranslation(0.5f, 0.4f, 0.5f), Matrix.CreateTranslation(12f / 16f, -1f / 16f, 0f) * Matrix.CreateScale(20f), "A Ball of Rubber, an important component in the advanced industry", "Rubber", 2.6f),
			new FoodCan("EmptyCan can be used to make food can","Empty",Color.Gray),
			new FoodCan("MeatCan can store meat for a long time","Meat",Color.Gray),
			new FoodCan("ChickenCan can store Chicken for a long time","Chicken",Color.Gray),
			new FoodCan("PumpkinCan can store Pumpkin for a long time","Pumpkin",Color.Gray),
			new FoodCan("BreadCan can store Bread for a long time","Bread",Color.Gray),
			new FoodCan("FishCan can store Fish for a long time","Fish",Color.Gray),
			new Brick("玻璃砖", "玻璃砖", new Color(255, 255, 255, 32), Matrix.CreateTranslation(-32 % 16 / 16f, -32 / 16 / 16f, 0f), "玻璃砖"),
			new Car(),
			new Tractor(),
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
			new MeshItem("电容")
			{
				DefaultDisplayName = "电容"
			}
			.AppendMesh("Models/Rods", "SteelRod", Matrix.CreateTranslation(0f, -0.8f, 0f), Matrix.Identity, Color.White)
			.AppendMesh("Models/Battery", "Battery", Matrix.CreateScale(.7f) * Matrix.CreateTranslation(0f, -0.8f, 0f), Matrix.CreateTranslation(9f / 16f, -7f / 16f, 0f), Color.DarkGray),
			new MeshItem("火花塞")
			{
				DefaultDisplayName = "火花塞"
			}
			.AppendMesh("Models/Rods", "SteelRod", Matrix.CreateScale(.6f) * Matrix.CreateTranslation(0f, -0.5f, 0f), Matrix.Identity, Color.White)
			.AppendMesh("Models/Battery", "Battery", Matrix.CreateScale(.2f, .6f, .2f) * Matrix.CreateTranslation(0f, -0.3f, 0f), Matrix.CreateTranslation(9f / 16f, -7f / 16f, 0f), Color.LightGray),
			new Powder("聚乙烯", "PE", Color.White),
			new Powder("聚丙烯", "PP", Color.White),
			new Plate("聚乙烯板", Color.White),
			new Plate("聚乙烯片", Color.White, true),
			new Rod(Materials.Plastic),
			new Rod(Materials.Uranium),
			new Mould("Models/Ingots", "IronPlate",  Matrix.CreateScale(1.5f, 8.15f, 1.5f), Matrix.CreateTranslation(9f / 16f, -7f / 16f, 0f) * Matrix.CreateScale(20f), "不锈钢板", "不锈钢板"),
			new Mould("Models/Ingots", "IronPlate",  Matrix.CreateScale(1.5f, 8.15f, 1.5f), Matrix.CreateTranslation(9f / 16f, -7f / 16f, 0f) * Matrix.CreateScale(20f), "工业钢板", "工业钢板"),
			new Mould("Models/Rods", "SteelRod", Matrix.CreateTranslation(0f,-0.5f,0f) * Matrix.CreateScale(4f, 2f, 4f), Matrix.CreateTranslation(9f / 16f, -7f / 16f, 0f) * Matrix.CreateScale(20f), "炮钢棍", "炮钢棍"),
			new Mould("Models/Rods", "SteelRod", Matrix.CreateTranslation(0f,-0.5f,0f) * Matrix.CreateScale(4f, 2f, 4f), Matrix.CreateTranslation(9f / 16f, -7f / 16f, 0f) * Matrix.CreateScale(20f), "枪管", "枪管"),
			new Bottle("玻璃瓶", "Bottle"),
			new Bottle("酒精", "C2H5OH"),
			new MeshItem("RubberWheel")
			{
				DefaultDisplayName = "RubberWheel"
			}
			.AppendMesh("Models/Wheel", "Wheel",  Matrix.CreateTranslation(0.0f, 0f, 0.1f), Matrix.CreateTranslation(9f / 16f, -15f / 16f, 0f)* Matrix.CreateScale(40f), Color.White)
			.AppendMesh("Models/Wheel", "Wheel",  Matrix.CreateTranslation(0f, 0f, 0f)*Matrix.CreateScale(2f), Matrix.CreateTranslation(9f / 16f, -7f / 16f, 0f)* Matrix.CreateScale(20f), Color.Black),
			new MouldItem("Telescope", "Models/Battery", "Battery", Matrix.CreateRotationX(MathUtils.PI / 2) * Matrix.CreateScale(.5f, .5f, 1.2f) * Matrix.CreateTranslation(0.5f, 0.5f, -0.3f), Matrix.CreateTranslation(9f / 16f, -7f / 16f, 0f), "望远镜", "望远镜", 1f)
			.AppendMesh("Models/Battery", "Battery", Matrix.CreateRotationX(MathUtils.PI / 2) * Matrix.CreateScale(.7f) * Matrix.CreateTranslation(0.5f, 0.5f, 0.5f), Matrix.CreateTranslation(9f / 16f, -7f / 16f, 0f), Color.DarkGray),
			new Mould("Models/Battery", "Battery", Matrix.CreateTranslation(new Vector3(0.5f)) * Matrix.CreateScale(2f, 0.15f, 2f), Matrix.CreateTranslation(9f / 16f, -7f / 16f, 0f) * Matrix.CreateScale(20f), "晶圆", "晶圆"),
			new MouldItem("Syringe", "Models/Screwdriver", "obj1", Matrix.CreateRotationZ(0.5f) * Matrix.CreateTranslation(0f, -0.33f, 0f), Matrix.CreateTranslation(15f / 16f, 0f, 0f), "注射器", "注射器", 3f),
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
				new ElectricFurnace(),
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
				new Battery(Matrix.CreateTranslation(-2f / 16f, 4f / 16f, 0f), "伏打电池", "伏打电池", "VBattery")
				{
					Voltage = 36
				},
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
				new ElectricIFurnace(),
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
				new ElectricDriller(),
				new OilFractionalTower(),
				new Reductor(),
				new TEDC(),
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
			for (i = 1; i < 15; i++)
				IdTable.Add(ChemicalBlock.Items.Array[i].GetCraftingId(), ChemicalBlock.Index | i << 14);
			for (i = 1; i < Chemistry.GunpowderBlock.Items.Length; i++)
				IdTable.Add(Chemistry.GunpowderBlock.Items[i].GetCraftingId(), GunpowderBlock.Index | i << 14);
		}

		internal static void Load()
		{
			var list = new DynamicArray<Item>(Chemistry.GunpowderBlock.Items)
			{
				Capacity = Chemistry.GunpowderBlock.Items.Length
			};
			for (int i = 0; i < 1024; i++)
				list.Add(new Mine((MineType)(i & 63), (i >> 6) / 4.0));
			list.Capacity = list.Count;
			Chemistry.GunpowderBlock.Items = list.Array;
			Equation.Reactions = new HashSet<Equation>();
			var reader = new StreamReader(Utils.GetTargetFile("IndustrialEquations.txt")); 
			try
			{
				while (true)
				{
					var line = reader.ReadLine();
					if (line == null) break;
					if (line.Length > 0 && !Equation.Reactions.Add(Equation.Parse(line)))
						throw new InvalidOperationException(line);
				}
			}
			finally { reader.Close(); }
			Item.Images = new[]
			{
				GetTexture("Transport-belt_sprite.jpg"),
				GetTexture("Fast-transport-belt_sprite.jpg"),
				GetTexture("Express-transport-belt_sprite.jpg"),
			};
			FactorioTransportBeltBlock.m_textures = new Texture2D[3];
			var stream = Utils.GetTargetFile("IndustrialMod_en-us.lng", false);
			if (stream == null) return;
			reader = new StreamReader(stream);
			try
			{
				Utils.ReadKeyValueFile(Utils.TR, reader);
			}
			finally { reader.Close(); }
		}

		public static Image GetTexture(string name)
		{
			return Pixz.Decode(Utils.GetTargetFile(name)).Array[0];
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