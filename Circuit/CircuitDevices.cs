﻿using Engine;
using System;

namespace Game
{
	public class Fridge : InventoryEntityDevice<ComponentNewChest>
	{
		public Fridge() : base("Freezer", "冰箱", "冰箱是保护食物的好地方，可以延缓食物的腐烂。 它最多可容纳16件物品。", 110) { }

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
		public Magnetizer() : base("Magnetizer", "磁化机", "磁化机是通过在由线提供的强磁场中熔化钢锭来制造工业磁体的装置。", 12) { }

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

	public class Separator : InventoryEntityDevice<ComponentSeperator>
	{
		public Separator() : base("Seperator", "分离器", "分离器是通过高频旋转分离处材料的装置，它是离心机的缩小版本。", 120) { }

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

		public override Widget GetWidget(IInventory inventory, ComponentSeperator component)
		{
			return new SeperatorWidget(inventory, component);
		}
	}

	public class UThickener : Separator
	{
		public UThickener()
		{
			DefaultDisplayName = DefaultDescription = "铀浓缩机";
		}

		public override int GetFaceTextureSlot(int face, int value)
		{
			return face != 4 && face != 5 && face == (Terrain.ExtractData(value) >> 15) ? 131 : 221;
		}
	}

	public class Canpack : InteractiveEntityDevice<ComponentCanpack>
	{
		public Canpack() : base("Canpack", "灌装机", "灌装机是一种可以封装液体，变成液体罐头的机器", 80) { }

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
			return face != 4 && face != 5 && face == (Terrain.ExtractData(value) >> 15) ? 235 : 107;
		}

		public override Widget GetWidget(IInventory inventory, ComponentCanpack component)
		{
			return new CanpackWidget(inventory, component);
		}
	}

	public class Electrobath : InteractiveEntityDevice<ComponentElectrobath>
	{
		public Electrobath() : base("Electrobath", "电解机", "电解机是一种可以将电解质进行氧化还原反应的机器，在冶炼一些活泼金属上有重要的作用", 200) { }

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
			return new SeperatorWidget(inventory, component, "Widgets/ElectrobathWidget");
		}
	}

	public class AirBlower : FixedDevice, IBlockBehavior
	{
		public int Level = -1;

		public AirBlower() : base("鼓风机", "鼓风机是一种将空气输送到需要大量热空气的大型机器中的装置。")
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

			//SubsystemTerrain.ChangeCell(p.X, p.Y, p.Z, , true);
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

	public class EFurnace : InventoryEntityDevice<ComponentElectricFurnace>
	{
		public EFurnace() : base("ElectricFurnace", "电阻炉", "电阻炉是一种通过电阻器加热物品的装置，它可以达到很高的温度，但需要大量的热量。", 300) { }

		public override void OnBlockAdded(SubsystemTerrain subsystemTerrain, int value, int oldValue)
		{
			base.OnBlockAdded(subsystemTerrain, value, oldValue);
			Component.Powered = false;
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

	public class EIFurnace : InventoryEntityDevice<ComponentElectricIFurnace>
	{
		public EIFurnace() : base("ElectricIFurnace", "电磁感应炉", "电磁感应炉是一种通过电磁感应加热物品的装置，它可以达到很高的温度，但需要大量的电量。", 200) { }

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

    public class ElectricMotor : FixedDevice, IBlockBehavior
    {
        public ElectricMotor() : base("电动机", "电动机是一种可以把电能转化为动能的机器", 310) { }
        public void OnBlockAdded(SubsystemTerrain subsystemTerrain, int value, int oldValue)
        {
        }
		
		public override int GetFaceTextureSlot(int face, int value)
        {
            return face == 4 || face == 5 ? 107 : 147;
        }
        public void OnBlockRemoved(SubsystemTerrain terrain, int value, int newValue)
        {
            value = 0;
            Simulate(ref value);
        }
	}

	public class Transformer : FixedDevice, IBlockBehavior
	{
		public Transformer() : base("变压器", "变压器是一种把交流电转化成直流电的机器") { }
		public void OnBlockAdded(SubsystemTerrain subsystemTerrain, int value, int oldValue)
		{
		}
		public override void Simulate(ref int voltage)
		{
			if (voltage < 0)
				voltage = (int)(-voltage * Math.Sqrt(2));
		}
		public override int GetFaceTextureSlot(int face, int value)
		{
			return face == 4 || face == 5 ? 107 : 121;
		}
		public void OnBlockRemoved(SubsystemTerrain terrain, int value, int newValue)
		{
			value = 0;
			Simulate(ref value);
		}
	}

	public class WaterExtractor : Separator
	{
		public WaterExtractor()
		{
			DefaultDisplayName = DefaultDescription = "重水提取机";
		}

		public override int GetFaceTextureSlot(int face, int value)
		{
			return face != 4 && face != 5 && face == (Terrain.ExtractData(value) >> 15) ? 236 : 224;
		}
	}

	public class Condenser : InteractiveEntityDevice<ComponentCondenser>
	{
        public bool Charged;
        public int Energy;
		public Condenser() : base("Condenser", "超大电容", "超大电容允许你存储一些电量，并在需要的时候释放出去") { Type = ElementType.Supply | ElementType.Connector; }
		public static MachineMode GetMode(int data)
		{
			return (MachineMode)(data >> 14 & 1);
		}
		public override void Simulate(ref int voltage)
        {
            if (Component.Charged)
            {
				if (voltage > 0)
				{
					Component.m_fireTimeRemaining = MathUtils.Min(Component.m_fireTimeRemaining + voltage, 1000000f);
					voltage = 0;
				}
			}
            else if (Component.m_fireTimeRemaining >= 310f)
            {
				Component.m_fireTimeRemaining -= 310f;
				voltage += 310;
            }
        }
        public override int GetFaceTextureSlot(int face, int value)
        {
            return face == 4 || face == 5 ? 114 : 117;
        }
        public override void OnBlockAdded(SubsystemTerrain subsystemTerrain, int value, int oldValue)
        {
			base.OnBlockAdded(subsystemTerrain, value, oldValue);
			Component.Powered = false;
		}
		public override Widget GetWidget(IInventory inventory, ComponentCondenser component)
		{
			return new CondenserWidget(inventory, component);
		}
	}
	public class Charger : InventoryEntityDevice<ComponentCharger>
	{
		public Charger() : base("Charger", "充电器", "充电放电装置是一种可以为电池充电或者放电的装置") { Type = ElementType.Supply | ElementType.Connector; }
		
		public static MachineMode GetMode(int data)
		{
			return (MachineMode)(data >> 14 & 1);
		}

		public static int SetMode(int data) => data ^ 1 << 14;
		
		public override void Simulate(ref int voltage)
		{
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
		public TGenerator() : base("TGenerator", "热能发电机", "热能发电机是一种利用金属温差发电的装置，它可以把岩浆转换为能量") { Type = ElementType.Supply | ElementType.Connector; }

		public override void Simulate(ref int voltage)
		{
			if (Component.Powered)
				voltage += 310;
		}

		public override int GetFaceTextureSlot(int face, int value)
		{
			return face != 4 && face != 5 && face == (Terrain.ExtractData(value) >> 15) ? 125 : 107;
		}

		public override Widget GetWidget(IInventory inventory, ComponentTGenerator component)
		{
			return new TGeneratorWidget(inventory, component);
		}
	}

	public class Unpacker : Separator
	{
		public Unpacker()
		{
			Voltage = 100;
			DefaultDisplayName = DefaultDescription = "去包装机";
			Type |= ElementType.Pipe;
		}

		public override void Simulate(ref int voltage)
		{
			if (voltage > 1023 || voltage < Voltage)
				return;
			for (int i = 0; i < Component.SlotsCount; i++)
			{
				if (Component.GetSlotCount(i) > 0)
				{
					int value = Terrain.ExtractContents(Component.GetSlotValue(i));
					switch (value)
					{
						case -1:
							Component.RemoveSlotItems(i, 1);
							ComponentInventoryBase.AcquireItems(Component, EmptyBucketBlock.Index, 1);
							voltage = value << 10;
							return;
						case WaterBucketBlock.Index:
						case MagmaBucketBlock.Index:
						case PaintStripperBucketBlock.Index:
							goto case -1;
						case RottenMeatBlock.Index:
							if(Terrain.ExtractData(Component.GetSlotValue(i)) >> 4 == 2)
							goto case -1;
							continue;
					}
				}
			}
		}

		public override int GetFaceTextureSlot(int face, int value)
		{
			return face != 4 && face != 5 && face == (Terrain.ExtractData(value) >> 15) ? 235 : 124;
		}
	}
	public class ElectricPump : FixedDevice
	{
		public ElectricPump() : base("电子泵", "电子泵是一种可以直接吸收液体于管道传输的机器", 150) { Type = ElementType.Pipe; }

		/*public override void Simulate(ref int voltage)
		{
		}*/

		public override int GetFaceTextureSlot(int face, int value)
		{
			return face == 4 || face == 5 ? 107 : 236;
		}
	}
	public class OilPlant : InventoryEntityDevice<ComponentOilPlant>
	{
		public OilPlant() : base("OilPlant", "石油化工厂", "石油化工厂可以用来处理石油，产生各种") { }

		public override void Simulate(ref int voltage)
		{
			if (Component.Powered = voltage >= 310)
				voltage -= 310;
		}

		public override int GetFaceTextureSlot(int face, int value)
		{
			return face != 4 && face != 5 ? face == (Terrain.ExtractData(value) >> 15) ? 123 : 124 : 107;
		}

		public override Widget GetWidget(IInventory inventory, ComponentOilPlant component)
		{
			return new OilPlantWidget(inventory, component);
		}
	}
}