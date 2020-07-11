using Engine;
using Engine.Graphics;
namespace Game
{
	public class Fridge : InventoryEntityDevice<ComponentNewChest>
	{
		public Fridge() : base("冰箱", "冰箱是保护食物的好地方，可以延缓食物的腐烂。 它最多可容纳16件物品。", 110)
		{
			Name = "Freezer";
		}

		public override void Simulate(ref int voltage)
		{
			base.Simulate(ref voltage);
			Component.Powered = Powered;
		}

		public override void OnBlockAdded(SubsystemTerrain subsystemTerrain, int value, int oldValue)
		{
			base.OnBlockAdded(subsystemTerrain, value, oldValue);
			Component.Powered = false;
		}

		public override int GetFaceTextureSlot(int face, int value)
		{
			return face == 4 || face == 5 ? 107 : face == (Terrain.ExtractData(value) >> 15) ? 106 : 107;
		}

		public override Widget GetWidget(IInventory inventory, ComponentNewChest component)
		{
			return new NewChestWidget(inventory, component);
		}
	}

	public class Magnetizer : InventoryEntityDevice<ComponentMagnetizer>
	{
		public Magnetizer() : base("磁化机", "磁化机是通过在由线提供的强磁场中熔化钢锭来制造工业磁体的装置。", 12) { }

		public override void Simulate(ref int voltage)
		{
			base.Simulate(ref voltage);
			Component.Powered = Powered;
		}

		public override void OnBlockAdded(SubsystemTerrain subsystemTerrain, int value, int oldValue)
		{
			base.OnBlockAdded(subsystemTerrain, value, oldValue);
			Component.Powered = false;
		}

		public override int GetFaceTextureSlot(int face, int value)
		{
			return face != 4 && face != 5 && face == (Terrain.ExtractData(value) >> 15) ? 239 : 107;
		}

		public override Widget GetWidget(IInventory inventory, ComponentMagnetizer component)
		{
			return new StoveWidget(inventory, component, "Widgets/MagnetizerWidget");
		}
	}

	public class Separator : InventoryEntityDevice<ComponentSeparator>
	{
		public Separator() : base("分离器", "分离器是通过高频旋转分离处材料的装置，它是离心机的缩小版本。", 120) { }

		public override void Simulate(ref int voltage)
		{
			base.Simulate(ref voltage);
			Component.Powered = Powered;
		}

		public override void OnBlockAdded(SubsystemTerrain subsystemTerrain, int value, int oldValue)
		{
			base.OnBlockAdded(subsystemTerrain, value, oldValue);
			Component.Powered = false;
		}

		public override int GetFaceTextureSlot(int face, int value)
		{
			return face != 4 && face != 5 && face == (Terrain.ExtractData(value) >> 15) ? 240 : 107;
		}

		public override Widget GetWidget(IInventory inventory, ComponentSeparator component)
		{
			return new SeparatorWidget(inventory, component);
		}
	}

	public class Canpack : InteractiveEntityDevice<ComponentCanpack>
	{
		public Canpack() : base("灌装机", "灌装机是一种可以封装液体，变成液体罐头的机器", 80) { Type |= ElementType.Pipe; }

		public override void Simulate(ref int voltage)
		{
			if (voltage >= 10000)
			{
				var inventory = Component.Entity.FindComponent<ComponentCanpack>(true);
				if (ComponentInventoryBase.AcquireItems(inventory, voltage >> 10, 1) == 0)
				{
					voltage = 0;
				}
			}
			base.Simulate(ref voltage);
			Component.Powered = Powered;
		}

		public override void OnBlockAdded(SubsystemTerrain subsystemTerrain, int value, int oldValue)
		{
			base.OnBlockAdded(subsystemTerrain, value, oldValue);
			Component.Powered = false;
		}

		public override int GetFaceTextureSlot(int face, int value)
		{
			return face != 4 && face != 5 && face == (Terrain.ExtractData(value) >> 15) ? 235 : 107;
		}

		public override Widget GetWidget(IInventory inventory, ComponentCanpack component)
		{
			return new CanpackWidget(inventory, component);
		}
	}

	public class Electrobath : InteractiveEntityDevice<ComponentElectrobath>
	{
		public Electrobath() : base("电解机", "电解机是一种可以将电解质进行氧化还原反应的机器，在冶炼一些活泼金属上有重要的作用", 200) { }

		public override void Simulate(ref int voltage)
		{
			base.Simulate(ref voltage);
			Component.Powered = Powered;
		}

		public override void OnBlockAdded(SubsystemTerrain subsystemTerrain, int value, int oldValue)
		{
			base.OnBlockAdded(subsystemTerrain, value, oldValue);
			Component.Powered = false;
		}

		public override int GetFaceTextureSlot(int face, int value)
		{
			return face == 4 || face == 5 ? 224 : 107;
		}

		public override Widget GetWidget(IInventory inventory, ComponentElectrobath component)
		{
			return new SeparatorWidget(inventory, component, "ElectroBath", "Widgets/ElectrobathWidget");
		}
	}

	public class AirBlower : CubeDevice, IBlockBehavior
	{
		public int Level = -1;

		public AirBlower() : base("鼓风机", "鼓风机是一种将空气输送到需要大量热空气的大型机器中的装置。", 250)
		{
		}

		public override void Simulate(ref int voltage)
		{
			int level;
			if (voltage < 20)
				level = 0;
			else
			{
				level = voltage < 20 ? 0 : (voltage - 20) / 40;
				voltage -= level * 40 + 20;
			}
			if (level == Level)
				return;
			var p = Point;
			var chunk = Utils.Terrain.GetChunkAtCell(p.X, p.Z);
			if (chunk != null)
			{
				if (chunk.State < TerrainChunkState.InvalidLight)
					Level = -1;
				else
				{
					p.X &= 15; p.Z &= 15;
					int i;
					for (i = 1; i <= level && chunk.GetCellContentsFast(p.X, p.Y + i, p.Z) == 0; i++)
						chunk.SetCellValueFast(p.X, p.Y + i, p.Z, RottenMeatBlock.Index | 2 << 4 << 14);
					for (; i < 8 && p.Y < 127; i++)
						if (Terrain.ReplaceLight(chunk.GetCellValueFast(p.X, ++p.Y, p.Z), 0) == (RottenMeatBlock.Index | 2 << 4 << 14))
							chunk.SetCellValueFast(p.X, p.Y, p.Z, 0);
					Level = level;
				}
			}
		}

		public override int GetFaceTextureSlot(int face, int value)
		{
			return face == 4 || face == 5 ? 107 : 220;
		}

		public void OnBlockAdded(SubsystemTerrain terrain, int value, int oldValue)
		{
		}

		public void OnBlockRemoved(SubsystemTerrain terrain, int value, int newValue)
		{
			Level = -1;
			value = 0;
			Simulate(ref value);
		}
	}

	public class ElectricFurnace : InventoryEntityDevice<ComponentElectricFurnace>
	{
		public ElectricFurnace() : base("电阻炉", "电阻炉是一种通过电阻器加热物品的装置，它可以达到很高的温度，但需要大量的热量。", 300) { }

		public override void OnBlockAdded(SubsystemTerrain subsystemTerrain, int value, int oldValue)
		{
			base.OnBlockAdded(subsystemTerrain, value, oldValue);
			Component.Powered = false;
		}
		public override void Simulate(ref int voltage)
		{
			base.Simulate(ref voltage);
			Component.Powered = Powered;
		}
		public override int GetFaceTextureSlot(int face, int value)
		{
			return face != 4 && face != 5 && face == (Terrain.ExtractData(value) >> 15) ? 164 : 107;
		}

		public override Widget GetWidget(IInventory inventory, ComponentElectricFurnace component)
		{
			return new ElectricFurnaceWidget(inventory, component);
		}
	}

	public class ElectricIFurnace : InventoryEntityDevice<ComponentElectricIFurnace>
	{
		public ElectricIFurnace() : base("电磁感应炉", "电磁感应炉是一种通过电磁感应加热物品的装置，它可以达到很高的温度，但需要大量的电量。", 200) { }

		public override void Simulate(ref int voltage)
		{
			base.Simulate(ref voltage);
			Component.Powered = Powered;
		}
		public override void OnBlockAdded(SubsystemTerrain subsystemTerrain, int value, int oldValue)
		{
			base.OnBlockAdded(subsystemTerrain, value, oldValue);
			Component.Powered = false;
		}
		public override int GetFaceTextureSlot(int face, int value)
		{
			return face != 4 && face != 5 && face == (Terrain.ExtractData(value) >> 15) ? 165 : 107;
		}
		public override Widget GetWidget(IInventory inventory, ComponentElectricIFurnace component)
		{
			return new ElectricFurnaceWidget(inventory, component, "Widgets/ElectricIFurnaceWidget");
		}
	}

	public class FireIBlock : CubeDevice
	{
		public FireIBlock() : base("电子点火器", "电子点火器是一种给火箭点火的机器", 170) { }

		public override int GetFaceTextureSlot(int face, int value)
		{
			return face == 4 || face == 5 ? 166: 107;
		}
	}


	public class ElectricHFurnace : InventoryEntityDevice<ComponentElectricHFurnace>
	{
		public ElectricHFurnace() : base("工业电子高炉", "工业电子高炉是一种通过电来加热物体的机器，主要用来冶炼合金", 1300) { }
		public override void Simulate(ref int voltage)
		{
			base.Simulate(ref voltage);
			Component.Powered = Powered;
		}
		public override void OnBlockAdded(SubsystemTerrain subsystemTerrain, int value, int oldValue)
		{
			base.OnBlockAdded(subsystemTerrain, value, oldValue);
			Component.Powered = false;
		}
		public override int GetFaceTextureSlot(int face, int value)
		{
			return face != 4 && face != 5 && face == (Terrain.ExtractData(value) >> 15) ? 157 : 107;
		}
		public override Widget GetWidget(IInventory inventory, ComponentElectricHFurnace component)
		{
			return new ElectricFurnaceWidget(inventory, component, "Widgets/ElectricHFurnaceWidget");
		}
	}

	public class AutoFactory : InventoryEntityDevice<ComponentAutoFactory>
	{
		public AutoFactory() : base("自动合成机", "自动合成机是一种通过电来自动合成物体的机器", 310) { }
		public override void Simulate(ref int voltage)
		{
			base.Simulate(ref voltage);
			Component.Powered = Powered;
		}
		public override void OnBlockAdded(SubsystemTerrain subsystemTerrain, int value, int oldValue)
		{
			base.OnBlockAdded(subsystemTerrain, value, oldValue);
			Component.Powered = false;
		}
		public override int GetFaceTextureSlot(int face, int value)
		{
			return face != 4 && face != 5  ? 168 : 107;
		}
		public override Widget GetWidget(IInventory inventory, ComponentAutoFactory component)
		{
			return new CovenWidget(inventory, component, "Widgets/AutoFactoryWidget");
		}
	}

	public class ElectricMotor : CubeDevice
	{
		public ElectricMotor() : base("电动机", "电动机是一种可以把电能转化为动能的机器", 310) { }
		
		public override int GetFaceTextureSlot(int face, int value)
		{
			return face == 4 || face == 5 ? 107 : 147;
		}
	}

	public class WireBlock2 : CubeDevice
	{
		public WireBlock2() : base("电子方块", "电子方块是一种导电的，可以传输电能的方块",0) { }

		public override int GetFaceTextureSlot(int face, int value)
		{
			return face == 4 || face == 5 ? 107 : 156;
		}
	}


	public class ColdBlock : CubeDevice
	{
		public ColdBlock() : base("冷却机", "冷却机是一种可以冷却蒸汽的机器", 230) { }

		public override int GetFaceTextureSlot(int face, int value)
		{
			return face == 4 || face == 5 ? 241 : 162;
		}
	}

	public class TElectricWire : CubeDevice
	{
		public TElectricWire() : base("电车电缆", "铺在电车车上方给电车供电的", 0) { Type = ElementType.None; }
    
		public override int GetFaceTextureSlot(int face, int value)
		{
			return face == 4 || face == 5 ? 170 : 131;
		}
	}



	public class Condenser : InteractiveEntityDevice<ComponentCondenser>, IElectricElementBlock, IItemAcceptableBlock
	{
		public Condenser() : base("超大电容", "超大电容允许你存储一些电量，并在需要的时候释放出去") { Type = ElementType.Supply | ElementType.Connector; }
		public static MachineMode GetMode(int data)
		{
			return (MachineMode)(data >> 14 & 1);
		}
		public override void Simulate(ref int voltage)
		{
			if (voltage > 8000)
				return;
			if (Component.Charged)
			{
				if (voltage > 0 && voltage<8024 && Component.m_fireTimeRemaining < 1000000f)
				{
					Component.m_fireTimeRemaining = MathUtils.Min(Component.m_fireTimeRemaining + voltage/10f, 1000000f);
					voltage = 0;
				}
			}
			else if (Component.m_fireTimeRemaining >= 310f)
			{
				Component.m_fireTimeRemaining -= 310f/10f;
				voltage += 310;
			}
		}
		public override int GetFaceTextureSlot(int face, int value)
		{
			return face == 4 || face == 5 ? 114 : 117;
		}
		public override Widget GetWidget(IInventory inventory, ComponentCondenser component)
		{
			return new CondenserWidget(inventory, component);
		}
		public ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			return new ChargerElectricElement(subsystemElectricity, new Point3(x, y, z));
		}
		public ElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z)
		{
			return face == 4 || face == 5 ? (ElectricConnectorType?)ElectricConnectorType.Input : null;
		}
		public int GetConnectionMask(int value)
		{
			int? color = PaintableItemBlock.GetColor(Terrain.ExtractData(value));
			return color.HasValue ? 1 << color.Value : 2147483647;
		}
		public new void OnHitByProjectile(CellFace cellFace, WorldItem worldItem)
		{
		}
	}


	public class Charger : InventoryEntityDevice<ComponentCharger>
	{
		public Charger() : base("充电器", "充电放电装置是一种可以为电池充电或者放电的装置") { Type = ElementType.Supply | ElementType.Connector; }
		
		public static MachineMode GetMode(int data)
		{
			return (MachineMode)(data >> 14 & 1);
		}

		public static int SetMode(int data) => data ^ 1 << 14;
		
		public override void Simulate(ref int voltage)
		{
			if (voltage > 8023)
			{
				return;
			}
			if (Component.Charged)
			{
				if (Component.Powered = voltage >= 310)
					voltage -= 310;
			}
			else if (Component.Powered2)	
			{
				voltage += 310;
			}
		}
		public override void OnBlockAdded(SubsystemTerrain subsystemTerrain, int value, int oldValue)
		{
			base.OnBlockAdded(subsystemTerrain, value, oldValue);
			Component.Powered = false;
		}
		public override int GetFaceTextureSlot(int face, int value)
		{
			return face == 4 || face == 5 ? 114 : 122;
		}
		public override Widget GetWidget(IInventory inventory, ComponentCharger component)
		{
			return new ChargerWidget(inventory, component);
		}
	}

	public class TGenerator : InventoryEntityDevice<ComponentTGenerator>
	{
		public TGenerator() : base("热能发电机", "热能发电机是一种利用金属温差发电的装置，它可以把岩浆转换为能量") { Type = ElementType.Supply | ElementType.Connector; }

		public override void Simulate(ref int voltage)
		{
			if (voltage > 8023)
			{
				return;
			}
			if (Component.Powered)
				voltage += 310;
		}

		public override int GetFaceTextureSlot(int face, int value)
		{
			return face != 4 && face != 5 && face == (Terrain.ExtractData(value) >> 15) ? 125 : 107;
		}
		//return face != 4 && face != 5 ? face == (Terrain.ExtractData(value) >> 15) ? 240 : 241 : 147;
		public override Widget GetWidget(IInventory inventory, ComponentTGenerator component)
		{
			return new SeparatorWidget(inventory, component,"ThermalGenerator");
		}
	}

	public class HGenerator : InventoryEntityDevice<ComponentHGenerator>
	{
		public HGenerator() : base("氢动力发电机", "氢动力发电机是一种利用氢氧化产生电能 120V") { Type = ElementType.Supply | ElementType.Connector; }

		public override void Simulate(ref int voltage)
		{
			if (voltage > 8023)
			{
				return;
			}
			if (Component.Powered)
				voltage += 120;
		}

		public override int GetFaceTextureSlot(int face, int value)
		{
			return face != 4 && face != 5 && face == (Terrain.ExtractData(value) >> 15) ? 149 : 107;
		}
		//return face != 4 && face != 5 ? face == (Terrain.ExtractData(value) >> 15) ? 240 : 241 : 147;
		public override Widget GetWidget(IInventory inventory, ComponentHGenerator component)
		{
			return new SeparatorWidget(inventory, component, "HydrogenGenerator");
		}
	}

	public class Turbine : InventoryEntityDevice<ComponentTurbine>
	{
		public Turbine() : base("蒸汽涡轮机", "蒸汽涡轮机是一种利用高压蒸汽发电的装置，它可以把蒸汽转换为能量") { Type = ElementType.Supply | ElementType.Connector; }

		public override void Simulate(ref int voltage)
		{
			if (voltage > 8023)
			{
				return;
			}
			if (Component.Powered)
				voltage += Component.Output*60;
			//voltage = MathUtils.Max(8023,voltage);
		}
		public override int GetFaceTextureSlot(int face, int value)
		{
			return face != 4 && face != 5 ? face == (Terrain.ExtractData(value) >> 15) ? 147 : 125 : 142;
		}
		//return face != 4 && face != 5 ? face == (Terrain.ExtractData(value) >> 15) ? 240 : 241 : 147;
		public override Widget GetWidget(IInventory inventory, ComponentTurbine component)
		{
			return new TurbineWidget(inventory, component);
		}
	}

	public class LaserG : InventoryEntityDevice<ComponentLaserG>
	{
		public LaserG() : base("激光发生器", "激光发生器是一种利用谐振腔及放大介质产生超强激光") {  }

		public override void Simulate(ref int voltage)
		{
			if (voltage > 8000)
				return;
		    if (voltage > 0 && voltage < 8024 && Component.m_fireTimeRemaining< 100000f)
				{
					Component.m_fireTimeRemaining = MathUtils.Min(Component.m_fireTimeRemaining + voltage / 10f, 100000f);
					voltage = 0;
				}
		}
		public override int GetFaceTextureSlot(int face, int value)
		{
			return face != 4 && face != 5 ? face == (Terrain.ExtractData(value) >> 15) ? 148 : 107 : 107;
		}
		//return face != 4 && face != 5 ? face == (Terrain.ExtractData(value) >> 15) ? 240 : 241 : 147;
		public override Widget GetWidget(IInventory inventory, ComponentLaserG component)
		{
			return new LaserWidget(inventory, component);
		}
	}

	public class AutoGun : InventoryEntityDevice<ComponentAGun>
	{
		public AutoGun() : base("自动机枪塔", "自动机枪塔是一种自动朝着非主角单位发射子弹的机器",120) { }

		public override void Simulate(ref int voltage)
		{
			base.Simulate(ref voltage);
			Component.Powered = Powered;
		}
		public override int GetFaceTextureSlot(int face, int value)
		{
			return face != 4 && face != 5 ? 161 : 107;
		}
		//return face != 4 && face != 5 ? face == (Terrain.ExtractData(value) >> 15) ? 240 : 241 : 147;
		public override Widget GetWidget(IInventory inventory, ComponentAGun component)
		{
			return new AGunWidget(inventory, component);
		}
	}

	public class AutoLaser : InventoryEntityDevice<ComponentALaser>
	{
		public AutoLaser() : base("自动激光塔", "自动激光塔是一种自动朝着非主角单位发射激光的机器") { }

		public override void Simulate(ref int voltage)
		{
			if (voltage > 8000)
				return;
			if (voltage > 0 && voltage < 8024 && Component.m_fireTimeRemaining < 100000f)
			{
				Component.m_fireTimeRemaining = MathUtils.Min(Component.m_fireTimeRemaining + voltage / 10f, 100000f);
				voltage = 0;
			}
		}
		public override int GetFaceTextureSlot(int face, int value)
		{
			return face != 4 && face != 5 ? 148 : 107;
		}
		//return face != 4 && face != 5 ? face == (Terrain.ExtractData(value) >> 15) ? 240 : 241 : 147;
		public override Widget GetWidget(IInventory inventory, ComponentALaser component)
		{
			return new ALaserWidget(inventory, component);
		}
	}

	public class Liquid : InventoryEntityDevice<ComponentLiquid>
	{
		public Liquid() : base("液化机", "液化机是一种把特定的气体液化的机器", 310) { }

		public override void Simulate(ref int voltage)
		{
			base.Simulate(ref voltage);
			Component.Powered = Powered;
		}
		public override int GetFaceTextureSlot(int face, int value)
		{
			return face != 4 && face != 5 ? face == (Terrain.ExtractData(value) >> 15) ? 160 : 107 : 107;
		}
		//return face != 4 && face != 5 ? face == (Terrain.ExtractData(value) >> 15) ? 240 : 241 : 147;
		public override Widget GetWidget(IInventory inventory, ComponentLiquid component)
		{
			return new SeparatorWidget(inventory, component, "Liquefier");
		}
	}

	public class RadioC : InventoryEntityDevice<ComponentRadioC>
	{
		public RadioC() : base("无线电发射装置", "无线电发射装置是一种利用线圈通电发射无线电的装置，可以发出给定频率的脉冲信号",100) { }

		public override void Simulate(ref int voltage)
		{
			if (voltage > 8000)
				return;
			base.Simulate(ref voltage);
			Component.Powered = Powered;
		}
		public override int GetFaceTextureSlot(int face, int value)
		{
			return face != 4 && face != 5 ? face == (Terrain.ExtractData(value) >> 15) ? 150 : 107 : 107;
		}
		//return face != 4 && face != 5 ? face == (Terrain.ExtractData(value) >> 15) ? 240 : 241 : 147;
		public override Widget GetWidget(IInventory inventory, ComponentRadioC component)
		{
			return new RadioCWidget(inventory, component);
		}
	}

	public class RadioR : InventoryEntityDevice2<ComponentRadioR>
	{
		public RadioR() : base("无线电接受装置", "无线电接受装置是一种利用简单线圈回路接受无线电的装置，可以接受给定频率的脉冲信号，有时候会出现接受不到的情况") { }

		public override void Simulate(ref int voltage)
		{
		}
		public override int GetFaceTextureSlot(int face, int value)
		{
			return face != 4 && face != 5 ? face == (Terrain.ExtractData(value) >> 15) ? 151 : 107 : 107;
		}
		//return face != 4 && face != 5 ? face == (Terrain.ExtractData(value) >> 15) ? 240 : 241 : 147;
		public override Widget GetWidget(IInventory inventory, ComponentRadioR component)
		{
			return new RadioRWidget(inventory, component);
		}
	}

	public class TControl : InventoryEntityDevice<ComponentTControl>
	{
		public TControl() : base("温度控制器", "温度控制器通过改变核电站燃料棒的位置进行温度的精细控制") { }
		public override void Simulate(ref int voltage)
		{
		}
		public override int GetFaceTextureSlot(int face, int value)
		{
			return face != 4 && face != 5 ? face == (Terrain.ExtractData(value) >> 15) ? 163 : 107 : 107;
		}
		//return face != 4 && face != 5 ? face == (Terrain.ExtractData(value) >> 15) ? 240 : 241 : 147;
		public override Widget GetWidget(IInventory inventory, ComponentTControl component)
		{
			return new TControlWidget(inventory, component);
		}
	}


	public class Recycler : InventoryEntityDevice<ComponentRecycler>
	{
		public Recycler() : base("回收机", "回收机", 110)
		{
		}

		public override void Simulate(ref int voltage)
		{
			base.Simulate(ref voltage);
			Component.Powered = Powered;
		}

		public override int GetFaceTextureSlot(int face, int value)
		{
			return face != 4 && face != 5 && face == (Terrain.ExtractData(value) >> 15) ? 147 : 107;
		}

		public override Widget GetWidget(IInventory inventory, ComponentRecycler component)
		{
			var widget = new RecyclerWidget(inventory, component, "Recycler","Widgets/RecyclerWidget");
			//widget.m_fire.IsVisible = false;
			//widget.Children.Find<StackPanelWidget>().IsVisible = false;
			return widget;
		}
	}
	public class AirPresser : InventoryEntityDevice<ComponentAirPresser>
	{
		public AirPresser() : base("气体压缩机", "一种可以压缩气体装入钢瓶的机器", 210)
		{
		}

		public override void Simulate(ref int voltage)
		{
			base.Simulate(ref voltage);
			Component.Powered = Powered;
		}

		public override int GetFaceTextureSlot(int face, int value)
		{
			return face != 4 && face != 5 && face == (Terrain.ExtractData(value) >> 15) ? 220 : 241;
		}

		public override Widget GetWidget(IInventory inventory, ComponentAirPresser component)
		{
			var widget = new SeparatorWidget(inventory, component, "GasCompressor");
			//widget.m_fire.IsVisible = false;
			//widget.Children.Find<StackPanelWidget>().IsVisible = false;
			return widget;
		}
	}
}