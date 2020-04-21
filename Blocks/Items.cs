using Chemistry;
using Engine;
using Engine.Content;
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
			var m3 = Matrix.CreateTranslation(new Vector3(0.5f));
			var m4 = Matrix.CreateScale(.5f);
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
			new Rod("RifleBarrel", "枪管是由膛线机制造的。它们对制造枪支很有用。", Color.Gray),
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
			new Mould("Gear", m3, Matrix.CreateTranslation(4f, 3.8f, 0f), "此钢制齿轮，在早期工业时是制作所有机器的必要部件。") { Size = 2f },
			new Mould("Wheel", m3, Matrix.CreateTranslation(4f, 3.8f, 0f), "由钢制成的轮子，是蒸汽机车的重要部件。") { Size = 2.4f },
			new Mould("WheelMould", Matrix.CreateTranslation(0f, -.02f, 0f) * m3, Matrix.CreateTranslation(2.6f, 1.4f, 0f), "由沙子和泥土制成的轮子模具，是在制造钢轮子过程中的必要部件。") { Size = 1.6f },
			new Mould("GearMould", Matrix.CreateTranslation(0f, -.02f, 0f) * m3, Matrix.CreateTranslation(2.6f, 1.4f, 0f), "由沙子和泥土制成的齿轮模具，是在制造钢齿轮过程中的必要部件。") { Size = 1.6f },
			new Mould("Piston", Matrix.CreateTranslation(.5f, 0.3f, 0.5f), Matrix.CreateTranslation(4f, 3.8f, 0f), "这个由铁，铜和钢制成的活塞，是许多机器的必要部件。") { Size = 2f, Id = "IndustrialPiston" },
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
			new Powder("滑石粉", "TalcumPowder", Color.White)
			{
				DefaultDescription = "滑石粉是一种天然矿物的粉末，可以用来生产水泥"
			},
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
				DefaultDescription = "石膏是一种天然矿物，也是一种很好的建筑材料"
			},
			new Powder("石膏粉", "Gesso", Color.White),
			new Powder("冰晶石", "Cryolite", Color.White),
			new Mould("Models/Piston2", "Cylinder", Matrix.CreateTranslation(0.5f, 0.7f, 0.5f) * Matrix.CreateScale(.6f), Matrix.CreateTranslation(4f, 3.8f, 0f), "这个由合金，铝和钢制成的圆筒，是许多机器的必要零件。", "Cylinder") { Size = 1.6f },
			new LightMould("Models/Snowball", "Snowball", m, m2, "A Ball of Melting Iron.", "MeltingIron"),
			new LightMould("Models/Snowball", "Snowball", m, m2, "A Ball of Melting Chromium.", "MeltingChromium"),
			new LightMould("Models/Snowball", "Snowball", m, m2, "A Ball of Melting Nickel.", "MeltingNickel"),
			new LightMould("Models/Snowball", "Snowball", m, m2, "A Ball of Melting Titanium.", "MeltingTitanium"),
			new LightMould("Models/Snowball", "Snowball", m, m2, "A Ball of Melting Aluminium.", "MeltingAluminium"),
			new LightMould("Models/Snowball", "Snowball", m, m2, "A Ball of Melting Copper.", "MeltingCopper"),
			new LightMould("Models/Snowball", "Snowball", m, m2, "A Ball of Melting Zinc.", "MeltingZinc"),
			new Alloy(Materials.Steel,"Stainless Steel"),
			new Alloy(Materials.Aluminum, "Super Aluminium"),
			new Alloy(Materials.Steel, "Si-Steel"),
			new Alloy(Materials.Steel,"Industrial Steel"),
			new Alloy(Materials.Copper,"Industrial Copper"),
			new Alloy(Materials.Steel,"Titanium Steel"),
			new Alloy(Materials.Chromium,"Gun-Steel"),
			new Powder("酵母", "Yeast", Color.White)
			{
				DefaultDescription = "酵母可以用来制作发酵类食品"
			},
			new Brick("混凝土砖", "ConcreteBrick", Color.Gray, Matrix.CreateTranslation(-32 % 16 / 16f, -32 / 16 / 16f, 0f), "混凝土砖")
			{
				DefaultDescription = "一种新型的建筑材料"
			},
			new Brick("多孔混凝土砖", "PorousConcreteBrick", Color.Gray, Matrix.CreateTranslation(-32 % 16 / 16f, -32 / 16 / 16f, 0f), "多孔混凝土砖")
			{
				DefaultDescription = "具有很多孔洞的混凝土砖"
			},
			new Plate("多晶硅", Color.DarkGray, true),
			new Plate("单晶硅", Color.DarkGray, true),
			new Mould("Models/Snowball", "Snowball", Matrix.CreateTranslation(0.5f, 0.4f, 0.5f), Matrix.CreateTranslation(12f / 16f, -1f / 16f, 0f) * Matrix.CreateScale(20f), "橡胶球，是在高新科技工业中的重要物质。", "Rubber") { Size = 2.6f },
			new FoodCan("空罐可以用来做食物罐头", "Empty", Color.Gray),
			new FoodCan("肉罐头可以长时间地保存肉", "Meat", Color.Gray),
			new FoodCan("鸡肉罐头可以长时间地保存鸡肉", "Chicken", Color.Gray),
			new FoodCan("南瓜罐头可以长时间地保存南瓜", "Pumpkin", Color.Gray),
			new FoodCan("面包罐头可以长时间地保存面包", "Bread", Color.Gray),
			new FoodCan("鱼罐头可以长时间地保存鱼", "Fish", Color.Gray),
			new Brick("玻璃砖", "玻璃砖", new Color(255, 255, 255, 32), Matrix.CreateTranslation(-32 % 16 / 16f, -32 / 16 / 16f, 0f), "玻璃砖"),
			new Car(),
			new Tractor(),
			new MouldItem("Rectifier", "Models/MotionDetector", "MotionDetector", m3, Matrix.CreateScale(20f), "整流器可以将交流电转换为直流电，可用于供电装置", "整流器", 2f),
			new MouldItem("ScR", "Models/Photodiode", "Photodiode", m3, Matrix.CreateScale(20f), "可控硅可以用来控制大功率设备，在自动控制工业中有重要用途", "可控硅", 2f),
			new MouldItem("LEDSheet", "Models/MotionDetector", "MotionDetector", m3, Matrix.CreateScale(20f), "LED贴片是一种简单但是亮度高的灯具", "LED贴片", 2f),
			new Spring("弹簧", "弹簧"),
			new Springboard("弹跳板", "弹跳板"),
			//new Mould("Models/OBox", "Cube", Matrix.CreateTranslation(0.5f, 0.7f, 0.5f) * Matrix.CreateScale(0.6f), Matrix.CreateTranslation(4f, 3.8f, 0f), "A Cylinder made of alloy, Aluminum and steel, the neccessary part of many machine.", "Cube", 1.6f),
			new Plate("散热片", Color.White, true)
			.AppendMesh("Models/Ingots", "IronPlate", Matrix.CreateTranslation(0.5f, -.6f, 0.5f) * m4, Matrix.Identity, Color.White)
			.AppendMesh("Models/Ingots", "IronPlate", Matrix.CreateTranslation(0.5f, -.3f, 0.5f) * m4, Matrix.Identity, Color.White)
			.AppendMesh("Models/Ingots", "IronPlate", Matrix.CreateTranslation(0.5f, 0.3f, 0.5f) * m4, Matrix.Identity, Color.White)
			.AppendMesh("Models/Ingots", "IronPlate", Matrix.CreateTranslation(0.5f, 0.6f, 0.5f) * m4, Matrix.Identity, Color.White),
			new MeshItem("电容")
			{
				DefaultDisplayName = "电容",
				DefaultDescription = "可以存储少量的电量，是电子工业重要的零件之一。"
			}
			.AppendMesh("Models/Rods", "SteelRod", Matrix.CreateTranslation(0f, -0.8f, 0f), Matrix.Identity, Color.White)
			.AppendMesh("Models/Battery", "Battery", Matrix.CreateScale(.7f) * Matrix.CreateTranslation(0f, -0.8f, 0f), Matrix.CreateTranslation(9f / 16f, -7f / 16f, 0f), Color.DarkGray),
			new MeshItem("火花塞")
			{
				DefaultDisplayName = "火花塞",
				DefaultDescription = "安装在汽缸的上端，是汽油机发动的重要工具"
			}
			.AppendMesh("Models/Rods", "SteelRod", Matrix.CreateScale(.6f) * Matrix.CreateTranslation(0f, -.5f, 0f), Matrix.Identity, Color.White)
			.AppendMesh("Models/Battery", "Battery", Matrix.CreateScale(.2f, .6f, .2f) * Matrix.CreateTranslation(0f, -.3f, 0f), Matrix.CreateTranslation(9f / 16f, -7f / 16f, 0f), Color.LightGray),
			new Powder("聚乙烯", "PE", Color.White),
			new Powder("聚丙烯", "PP", Color.White),
			new Plate("聚乙烯板", Color.White),
			new Plate("聚乙烯片", Color.White, true),
			new Rod(Materials.Plastic),
			new Rod(Materials.Uranium),
			new Mould("Models/Brick", "Brick", Matrix.CreateScale(2f, 0.5f, 1f) * Matrix.CreateScale(3f), Matrix.CreateTranslation(9f / 16f, -7f / 16f, 0f) * Matrix.CreateScale(20f), "不锈钢板是一种非常坚硬，不会生锈的材料", "不锈钢板"),
			new Mould("Models/Brick", "Brick", Matrix.CreateScale(2f, 0.5f, 1f) * Matrix.CreateScale(3f), Matrix.CreateTranslation(9f / 16f, -7f / 16f, 0f) * Matrix.CreateScale(20f), "工业钢板与不锈钢板所含的金属不同。工业钢板也是一种重要的工业材料", "工业钢板"),
			new Mould("Models/Rods", "SteelRod", Matrix.CreateTranslation(0f, -.5f, 0f) * Matrix.CreateScale(4f, 2f, 4f), Matrix.CreateTranslation(9f / 16f, -7f / 16f, 0f) * Matrix.CreateScale(20f), "炮钢棍", "炮钢棍"),
			new Mould("Models/Rods", "SteelRod", Matrix.CreateTranslation(0f, -.5f, 0f) * Matrix.CreateScale(4f, 2f, 4f), Matrix.CreateTranslation(9f / 16f, -7f / 16f, 0f) * Matrix.CreateScale(20f), "枪管", "枪管"),
			new Bottle("玻璃瓶", "Bottle"),
			new Bottle("酒精", "C2H5OH"),
			new MeshItem("RubberWheel")
			{
				DefaultDisplayName = "RubberWheel"
			}
			.AppendMesh("Models/Wheel", "Wheel", Matrix.CreateTranslation(0f, 0f, 0.1f), Matrix.CreateTranslation(9f / 16f, -15f / 16f, 0f)* Matrix.CreateScale(40f), Color.White)
			.AppendMesh("Models/Wheel", "Wheel", Matrix.CreateScale(2f), Matrix.CreateTranslation(9f / 16f, -7f / 16f, 0f)* Matrix.CreateScale(20f), Color.Black),
			new MouldItem("Telescope", "Models/Battery", "Battery", Matrix.CreateRotationX(MathUtils.PI / 2) * Matrix.CreateScale(.5f, .5f, 1.2f) * Matrix.CreateTranslation(0.5f, 0.5f, -0.3f), Matrix.CreateTranslation(9f / 16f, -7f / 16f, 0f), "望远镜", "望远镜", 1f)
			.AppendMesh("Models/Battery", "Battery", Matrix.CreateRotationX(MathUtils.PI / 2) * Matrix.CreateScale(.7f) * Matrix.CreateTranslation(0.5f, 0.5f, 0.5f), Matrix.CreateTranslation(9f / 16f, -7f / 16f, 0f), Color.DarkGray),
			new Digger(),
			new MouldItem("Syringe", "Models/Screwdriver", "obj1", Matrix.CreateRotationZ(0.5f) * Matrix.CreateTranslation(0f, -0.33f, 0f), Matrix.CreateTranslation(15f / 16f, 0f, 0f), "可以将液体药物注入动物或玩家体内", "注射器", 3f),
			new Mould("Models/Battery", "Battery", m3 * Matrix.CreateScale(2f, 0.15f, 2f), Matrix.CreateTranslation(9f / 16f, -7f / 16f, 0f) * Matrix.CreateScale(20f), "晶圆是芯片工业中的重要原料", "晶圆"),
			new Powder(Materials.Vanadium),
			new Powder("U235", "U235P", Color.LightGreen),
			new Powder("U238", "U238P", Color.DarkGreen),
			new Mould("Models/Rods", "SteelRod", Matrix.CreateTranslation(0f, -.5f, 0f) * Matrix.CreateScale(16f,0.5f, 16f), Matrix.CreateTranslation(9f / 16f, -3f / 16f, 0f) * Matrix.CreateScale(20f),Color.LightGreen, "U235C", "U235C"),
			new Mould("Models/Rods", "SteelRod", Matrix.CreateTranslation(0f, -.5f, 0f) * Matrix.CreateScale(16f,0.5f, 16f), Matrix.CreateTranslation(9f / 16f, -3f / 16f, 0f) * Matrix.CreateScale(20f),Color.DarkGreen, "U238C", "U238C"),
			//new Wire("CopperWire"),
			new Circuit("Circuit1", "初级电路板", "初级电路板，由电线，电子管构成的电路板"),
			new MeshItem("电子二极管")
			{
				DefaultDisplayName = "电子二极管",
				DefaultDescription = "电子二极管是半导体工业中重要的零件"
			}
			.AppendMesh("Models/Rods", "SteelRod", Matrix.CreateTranslation(0.2f, -0.8f, 0f), Matrix.Identity, Color.White)
			.AppendMesh("Models/Rods", "SteelRod", Matrix.CreateTranslation(-0.2f, -0.8f, 0f), Matrix.Identity, Color.White)
			.AppendMesh("Models/Battery", "Battery", Matrix.CreateScale(.7f) * Matrix.CreateTranslation(0f, -0.8f, 0f), Matrix.CreateTranslation(-32 % 16 / 16f, -32 / 16 / 16f, 0f) * Matrix.CreateScale(20f), new Color(255, 255, 255, 32)),
			new Tank(),
			new MeshItem("液压元件")
			{
				DefaultDisplayName = "液压元件",
				DefaultDescription = "液压元件可以在液压系统中传动"
			}
			.AppendMesh("Models/Rods", "SteelRod", Matrix.CreateTranslation(0f, -0.8f, 0f), Matrix.Identity, Color.White)
			.AppendMesh("Models/Battery", "Battery", m4 * Matrix.CreateTranslation(0f, -0.8f, 0f), Matrix.CreateTranslation(9f / 16f, -7f / 16f, 0f), Color.Gray)
			.AppendMesh("Models/Battery", "Battery", m4 * Matrix.CreateTranslation(0f, 0.1f, 0f),  Matrix.CreateTranslation(9f / 16f, -7f / 16f, 0f), Color.Black),
			new MouldItem("Cannon", "Models/Battery", "Battery",  Matrix.CreateScale(.8f, 2.0f, .8f) * Matrix.CreateRotationX(MathUtils.PI / 2) * Matrix.CreateTranslation(0.5f, 0.5f, -0.3f), Matrix.CreateTranslation(9f / 16f, -7f / 16f, 0f), "炮管(未镗)", "炮管(未镗)", 1f),
			new MouldItem("CannonB", "Models/Battery", "Battery",  Matrix.CreateScale(.8f, 2.0f, .8f) * Matrix.CreateRotationX(MathUtils.PI / 2) * Matrix.CreateTranslation(0.5f, 0.5f, -0.3f), Matrix.CreateTranslation(9f / 16f, -7f / 16f, 0f), "炮管", "炮管", 0.9f),
			new Mould("Models/Rods", "SteelRod", Matrix.CreateTranslation(0f,-0.5f,0f) * Matrix.CreateScale(4f,1.5f, 4f), Matrix.CreateTranslation(9f / 16f, -3f / 16f, 0f) * Matrix.CreateScale(20f),Color.Black, "CarbonTunnel", "CarbonTunnel"),
			new Brick("PlasticBar", "PlasticBar", Color.White, Matrix.CreateTranslation(9f / 16f, -7f / 16f, 0f), "塑料条", 1.5f)
			{
				DefaultDescription = "通过一些有机反应得到的高分子化合物，有多种重要的性质"
			},
			new MeshItem("RubyCyrstal")
			{
				DefaultDisplayName = "RubyCrystal"
			}
			.AppendMesh("Models/Diamond", "Diamond", Matrix.CreateScale(.8f), Matrix.CreateTranslation(9f / 16f, -12f / 16f, 0f), Color.LightRed),
			new Brick("RubyMaterial", "RubyMaterial", Color.LightRed, Matrix.CreateTranslation(9f / 16f, -7f / 16f, 0f), "红宝石介质", 0.8f),
			new MeshItem("电阻")
			{
				DefaultDisplayName = "电阻"
			}
			.AppendMesh("Models/Rods", "SteelRod", Matrix.CreateScale(1.5f) * Matrix.CreateTranslation(0f, -.7f, 0f) * Matrix.CreateRotationX(1.5f), Matrix.Identity, Color.White)
			.AppendMesh("Models/Battery", "Battery", Matrix.CreateScale(.7f) * Matrix.CreateTranslation(0f, -.3f, 0f) * Matrix.CreateRotationX(1.5f), Matrix.CreateTranslation(9 / 16f,-7 / 16 / 16f, 0f) * Matrix.CreateScale(20f), Color.Black),
			new MeshItem("晶体三极管")
			{
				DefaultDisplayName = "晶体三极管"
			}
			.AppendMesh("Models/Rods", "SteelRod", Matrix.CreateTranslation(0.25f, -0.8f, 0f), Matrix.Identity, Color.White)
			.AppendMesh("Models/Rods", "SteelRod", Matrix.CreateTranslation(0f, -0.8f, 0f), Matrix.Identity, Color.White)
			.AppendMesh("Models/Rods", "SteelRod", Matrix.CreateTranslation(-0.25f, -0.8f, 0f), Matrix.Identity, Color.White)
			.AppendMesh("Models/Battery", "Battery", Matrix.CreateScale(1f,.7f,1f) * Matrix.CreateTranslation(0f, -0.3f, 0f), Matrix.CreateTranslation(9 / 16f,-7 / 16 / 16f, 0f) * Matrix.CreateScale(20f), Color.Black),
			new Circuit("Circuit2", "高级电路板", "高级电路板，由锡线，晶体管构成的电路板", 195),
			new Pavior(),
			new MouldItem("Electrongun", "Models/Electrongun", "Electrongun", Matrix.CreateRotationX(MathUtils.PI / 2) * Matrix.CreateTranslation(.5f, .5f, -.3f), Matrix.CreateTranslation(9f / 16f, -7f / 16f, 0f), "电子枪可以发射电子，是加速器的重要部件", "电子枪", .6f),
			new ETrain(),
			new Mould("Snowball", m * 2.9f, m2, "铜汞齐是铜与汞形成的合金", "铜汞齐", "CuAmalgam"),
			new Mould("Snowball", m * 2.9f, m2, "金汞齐是金与汞形成的合金", "金汞齐", "AuAmalgam"),
			new Mould("Snowball", m * 2.9f, m2, "银汞齐是银与汞形成的合金", "银汞齐", "AgAmalgam"),
			new Mould("Snowball", m * 2.9f, m2, "铂汞齐是铂与汞形成的合金", "铂汞齐", "PtAmalgam"),
			new Mould("Snowball", m * 2.9f, m2, "铜汞齐是铜与汞形成的合金", "铅汞齐", "PbAmalgam"),
			new Mould("Snowball", m * 2.9f, m2, "锌汞齐是锌与汞形成的合金", "锌汞齐", "ZnAmalgam"),
			new Mould("Snowball", m * 2.9f, m2, "锡汞齐是锡与汞形成的合金", "锡汞齐", "SnAmalgam"),
			new Mould("Snowball", m * 2.9f, m2, "钛汞齐是钛与汞形成的合金", "钛汞齐", "TiAmalgam"),
			new Mould("Battery", Matrix.CreateRotationX(MathUtils.PI / 2) * Matrix.CreateTranslation(.5f, .5f, 0f), Matrix.CreateTranslation(9f / 16f, -7f / 16f, 0f) * Matrix.CreateScale(10f), "差速器是汽车工业中的重要零件", "差速器", "DMechanism"),
			new Mould("Models/Rods", "SteelRod", Matrix.CreateRotationX(MathUtils.PI / 2) * Matrix.CreateTranslation(.5f, .5f, 0f), Matrix.Identity * Matrix.CreateScale(10f), Color.White, "X射线管可以发射X射线", "X射线管") { Id = "XRayTube" },
			new Bottle("洗手液", "HandSanitizer", Color.LightGreen)
			{
				DefaultDescription = "洗手液可以有效除去手上携带的病菌，让你保持健康"
			},
			/*new Plane(),
			new Plate("128K RAM", Color.DarkGreen, true),
			new Airplane(),
			new MouldItem("UsedUpFuel", "Models/Rods", "SteelRod", Matrix.CreateTranslation(0.0f, -0.5f, 0f)* Matrix.CreateScale(1.6f,1f,1.6f), Matrix.Identity, "用尽的核燃料","用尽的核燃料",2.3f),
			new Powder("PU239P", "PU239P", Color.DarkGray),
			new Mould("Models/Rods", "SteelRod", Matrix.CreateTranslation(0f,-0.5f,0f) * Matrix.CreateScale(16f,0.5f, 16f), Matrix.CreateTranslation(9f / 16f, -3f / 16f, 0f) * Matrix.CreateScale(20f),Color.DarkGray, "PU239C", "PU239C"),
			new Flat("中子反射板","可以反射一部分中子",154),
			new MGun(),
			new MeshItem("火箭发动机")
			{
				DefaultDisplayName = "火箭发动机",
				DefaultDescription = "火箭发动机，火箭的必备组件，提供强大的推力"
			}
			.AppendMesh("Models/EmptyBucket", "Bucket", Matrix.CreateScale(1f)*Matrix.CreateTranslation(0.0f, 0.5f, 0.0f)  * Matrix.CreateRotationX(1.5f), Matrix.CreateTranslation(9f / 16f,-12f / 16 / 16f, 0f), Color.Black)
			.AppendMesh("Models/Battery", "Battery", Matrix.CreateScale(1.2f) * Matrix.CreateTranslation(0f, -0.6f, 0f)* Matrix.CreateRotationX(1.5f), Matrix.CreateTranslation(9f / 16f, -7f / 16f, 0f), Color.Gray),
			new MouldItem("LH2","Models/Cylinder", "obj1", Matrix.CreateScale(40f, 80f, 40f) * Matrix.CreateTranslation(0.5f, 0f, 0.5f), Matrix.CreateTranslation(9f / 16f, -7f / 16f, 0f), "液态氢", "液态氢", 1.5f),
			new MouldItem("LO2","Models/Cylinder", "obj1", Matrix.CreateScale(40f, 80f, 40f) * Matrix.CreateTranslation(0.5f, 0f, 0.5f), Matrix.CreateTranslation(9f / 16f, -7f / 16f, 0f), "液态氧", "液态氧", 1.5f),
			new MouldItem("LD2","Models/Cylinder", "obj1", Matrix.CreateScale(40f, 80f, 40f) * Matrix.CreateTranslation(0.5f, 0f, 0.5f), Matrix.CreateTranslation(9f / 16f, -7f / 16f, 0f), "液态D2", "液态D2", 1.5f),
			new MouldItem("LN2","Models/Cylinder", "obj1", Matrix.CreateScale(40f, 80f, 40f) * Matrix.CreateTranslation(0.5f, 0f, 0.5f), Matrix.CreateTranslation(9f / 16f, -7f / 16f, 0f), "液态氮", "液态氮", 1.5f),
			new MouldItem("LCl2","Models/Cylinder", "obj1", Matrix.CreateScale(40f, 80f, 40f) * Matrix.CreateTranslation(0.5f, 0f, 0.5f), Matrix.CreateTranslation(9f / 16f, -7f / 16f, 0f), "液态氯", "液态氯", 1.5f),
			new Mould("Models/Brick", "Brick", Matrix.CreateScale(2f, 0.5f, 1f)*Matrix.CreateScale(3f), Matrix.CreateTranslation(9f / 16f, -7f / 16f, 0f) * Matrix.CreateScale(20f), "钛合金板", "钛合金板"),
			new Rocket(),
			new MeshItem("卫星")
			{
				DefaultDisplayName = "卫星",
				DefaultDescription = "给予提供侦察功能"
			}
			.AppendMesh("Models/Battery", "Battery", Matrix.CreateScale(0.7f) * Matrix.CreateTranslation(0f, -0.3f, 0f) * Matrix.CreateRotationZ(1.5f), Matrix.CreateTranslation(9f / 16f, -7f / 16f, 0f), Color.DarkGray)
			.AppendMesh("Models/EmptyBucket", "Bucket", Matrix.CreateScale(0.6f)*Matrix.CreateTranslation(0.05f, 0.3f, 0.0f)  * Matrix.CreateRotationZ(-1.5f), Matrix.CreateTranslation(9f / 16f,-12f / 16 / 16f, 0f), Color.Black)
			.AppendMesh("Models/Brick", "Brick",Matrix.CreateScale(1f, 0.015f, 3f)*Matrix.CreateScale(1.8f), Matrix.CreateTranslation(9f / 16f, -7f / 16f, 0f) * Matrix.CreateScale(20f), Color.Blue),
			new MeshItem("卫星连接器")
			{
				DefaultDisplayName = "卫星连接器",
				DefaultDescription = "卫星连接器，可以探查地形。"
			}
			.AppendMesh("Models/Brick", "Brick", Matrix.CreateScale(1f, 0.25f, 1f), Matrix.CreateTranslation(9f / 16f, -7f / 16f, 0f) * Matrix.CreateScale(20f), Color.Black)
			.AppendMesh("Models/Brick", "Brick", Matrix.CreateScale(0.5f, 0.25f, 0.5f)*Matrix.CreateTranslation(0f,0.05f,0f), Matrix.CreateTranslation(9f / 16f, -7f / 16f, 0f) * Matrix.CreateScale(20f), Color.Gray),
			//new MouldItem("LH2","Models/Cylinder", "obj1", Matrix.CreateScale(40f, 80f, 40f) * Matrix.CreateTranslation(0.5f, 0f, 0.5f), Matrix.CreateTranslation(9f / 16f, -7f / 16f, 0f), "液态氢", "液态氢", 1.5f),
			//new MouldItem("LO2","Models/Cylinder", "obj1", Matrix.CreateScale(40f, 80f, 40f) * Matrix.CreateTranslation(0.5f, 0f, 0.5f), Matrix.CreateTranslation(9f / 16f, -7f / 16f, 0f), "液态氧", "液态氧", 1.5f),
			//new Plane(),
			/*nnew Plate("128K RAM", Color.DarkGreen, true),
			new Plate("256K RAM", Color.DarkGreen, true),
			new Plate("512K RAM", Color.DarkGreen, true),
			new Mould("Box", Matrix.Identity, Matrix.Identity, "主板"),
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
				new AirPresser(),
				new VaFurnace(),
				new Centrifugal(),
				new Workshop(),
				new RControl(),
				new Hchanger(),
				new Turbine(),
				new LaserG(),
				new MHDGenerator(),
				new TElectricWire(),
				new HGenerator(),
				new RadioC(),
				new RadioR(),
				new WireBlock2(),
				new ElectricHFurnace(),
				new AutoGun(),
				new Liquid(),
				new Ultracentrifuge(),
				new FlowCytometer(),
				new LightningCatcher(),
				new Cyclotron(),
				new FuelCell(),
				new CloudChamber(),
			};
			ContentCache.m_contentByName.TryGetValue("CraftingIdTable", out object value);
			if (!(value is Dictionary<string, int> dict))
			{
				ContentCache.m_contentByName["CraftingIdTable"] = dict = new Dictionary<string, int>(Items.Length);
			}
			dict.Add("Diamond", DiamondChunkBlock.Index);
			dict.Add("Hg", RottenMeatBlock.Index | 96 << 14);
			int i;
			for (i = 0; i < Items.Length; i++)
				dict.Add(Items[i].GetCraftingId(), Index | i << 14);
			for (i = 0; i < ElementBlock.Devices.Length; i++)
			{
				if (ElementBlock.Devices[i] is CubeDevice device)
					device.Index = i;
				dict.Add(ElementBlock.Devices[i].GetCraftingId(), ElementBlock.Index | i << 14);
			}
			for (i = 1; i < ChemicalBlock.Items.Count; i++)
				dict.Add(ChemicalBlock.Items.Array[i].GetCraftingId(), ChemicalBlock.Index | i << 14);
			for (i = 1; i < Chemistry.GunpowderBlock.Items.Length; i++)
				dict.Add(Chemistry.GunpowderBlock.Items[i].GetCraftingId(), GunpowderBlock.Index | i << 14);
			IdTable = dict;
		}

		internal static void Load()
		{
			var list = new DynamicArray<Item>(Chemistry.GunpowderBlock.Items);
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