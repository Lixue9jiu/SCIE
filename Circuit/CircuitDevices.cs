using Engine;

namespace Game
{
	public class Fridge : InteractiveEntityDevice<ComponentNewChest>
	{
		public Fridge() : base("Freezer", "冰箱", "冰箱是保护食物的好地方，可以延缓食物的腐烂。 它最多可容纳16件物品。") { }

		public override void Simulate(ref int voltage)
		{
			Component.Powered = voltage >= 110;
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

		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			return GetPlacementValue(0, componentMiner, value, raycastResult);
		}

		public override Widget GetWidget(IInventory inventory, ComponentNewChest component)
		{
			return new NewChestWidget(inventory, component);
		}
	}

	public class Magnetizer : InteractiveEntityDevice<ComponentMagnetizer>
	{
		public Magnetizer() : base("Magnetizer", "磁化机", "磁化机是通过在由线提供的强磁场中熔化钢锭来制造工业磁体的装置。") { }

		public override void Simulate(ref int voltage)
		{
			if (Component.Powered = voltage >= 12)
				voltage -= 12;
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

		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			return GetPlacementValue(2, componentMiner, value, raycastResult);
		}

		public override Widget GetWidget(IInventory inventory, ComponentMagnetizer component)
		{
			return new StoveWidget(inventory, component, "Widgets/MagnetizerWidget");
		}
	}

	public class Separator : InteractiveEntityDevice<ComponentSeperator>
	{
		public Separator() : base("Seperator", "分离器", "分离器是通过高频旋转分离处材料的装置，它是离心机的缩小版本。") { }

		/*public override Device Create(Point3 p)
		{
			var other = (Separator)base.Create(p);
			return other;
		}*/

		public override void Simulate(ref int voltage)
		{
			if (Component.Powered = voltage >= 120)
				voltage -= 120;
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

		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			return GetPlacementValue(3, componentMiner, value, raycastResult);
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

		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			return GetPlacementValue(20, componentMiner, value, raycastResult);
		}
	}

	public class Canpack : InteractiveEntityDevice<ComponentCanpack>
	{
		public Canpack() : base("Canpack", "灌装机", "灌装机是一种可以封装液体，变成液体罐头的机器") { }

		public override void Simulate(ref int voltage)
		{
			if (Component.Powered = voltage >= 80)
				voltage -= 80;
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

		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			return GetPlacementValue(7, componentMiner, value, raycastResult);
		}

		public override Widget GetWidget(IInventory inventory, ComponentCanpack component)
		{
			return new CanpackWidget(inventory, component);
		}
	}

	public class Electrobath : InteractiveEntityDevice<ComponentElectrobath>
	{
		public Electrobath() : base("Electrobath", "电解机", "电解机是一种可以将电解质进行氧化还原反应的机器，在冶炼一些活泼金属上有重要的作用") { }

		public override void Simulate(ref int voltage)
		{
			if (Component.Powered = voltage >= 200)
				voltage -= 200;
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

		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			return GetPlacementValue(8, componentMiner, value, raycastResult);
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

		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			return GetPlacementValue(4, componentMiner, value, raycastResult);
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

	public class EFurnace : InteractiveEntityDevice<ComponentElectricFurnace>, IElectricElementBlock
	{
		public EFurnace() : base("ElectricFurnace", "电阻炉", "电阻炉是一种通过电阻器加热物品的装置，它可以达到很高的温度，但需要大量的热量。") { }

		/*public override Device Create(Point3 p)
		{
			var other = (EFurnace)base.Create(p);
			return other;
		}*/

		public override void Simulate(ref int voltage)
		{
			if (Component.Powered = voltage >= 300)
				voltage -= 300;
		}

		public override void OnBlockAdded(SubsystemTerrain subsystemTerrain, int value, int oldValue)
		{
			base.OnBlockAdded(subsystemTerrain, value, oldValue);
			Component.Powered = false;
		}

		public override int GetFaceTextureSlot(int face, int value)
		{
			return face != 4 && face != 5 && face == (Terrain.ExtractData(value) >> 15) ? 164 : 107;
		}

		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			return GetPlacementValue(6, componentMiner, value, raycastResult);
		}

		public override Widget GetWidget(IInventory inventory, ComponentElectricFurnace component)
		{
			return new ElectricFurnaceWidget(inventory, component);
		}

		public ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			return new CraftingMachineElectricElement(subsystemElectricity, new Point3(x, y, z));
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
	}

	public class EIFurnace : InteractiveEntityDevice<ComponentElectricIFurnace>, IElectricElementBlock
	{
		public EIFurnace() : base("ElectricIFurnace", "电磁感应炉", "电磁感应炉是一种通过电磁感应加热物品的装置，它可以达到很高的温度，但需要大量的电量。") { }

		/*public override Device Create(Point3 p)
		{
			var other = (EFurnace)base.Create(p);
			return other;
		}*/

		public override void Simulate(ref int voltage)
		{
			if (Component.Powered = voltage >= 400)
				voltage -= 400;
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

		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			return GetPlacementValue(24, componentMiner, value, raycastResult);
		}

		public override Widget GetWidget(IInventory inventory, ComponentElectricIFurnace component)
		{
			return new ElectricIFurnaceWidget(inventory, component);
		}

		public ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			return new CraftingMachineElectricElement(subsystemElectricity, new Point3(x, y, z));
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
	}
}