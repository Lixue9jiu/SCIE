using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	public class ComponentElectricDriller : ComponentInventoryBase, IUpdateable
	{
		public bool Powered;
		protected ComponentBlockEntity m_componentBlockEntity;

		public int UpdateOrder => 0;
		public bool Charged;
		public float m_fireTimeRemaining;
		public float HeatLevel;

		public void Update(float dt)
		{
			if (Charged)
			{
				m_fireTimeRemaining = MathUtils.Max(0f, m_fireTimeRemaining - dt);
				if (m_fireTimeRemaining<1f)
				{
					Point3 coordinates = m_componentBlockEntity.Coordinates;
					int data = Terrain.ExtractData(Utils.Terrain.GetCellValue(coordinates.X, coordinates.Y, coordinates.Z));
					Driller(coordinates);
					m_fireTimeRemaining = 7f;
				}
			}
		}

		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.LoadItems(valuesDictionary);
			m_componentBlockEntity = Entity.FindComponent<ComponentBlockEntity>(true);
			m_fireTimeRemaining = valuesDictionary.GetValue("FireTimeRemaining", 0f);
			Charged = valuesDictionary.GetValue("HeatLevel", 0f) == 0f ? true : false;
		}

		public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
		{
			this.SaveItems(valuesDictionary);
			valuesDictionary.SetValue("FireTimeRemaining", m_fireTimeRemaining);
			valuesDictionary.SetValue("HeatLevel", Charged ? 0f : 1f);
		}


		protected void Driller(Point3 point)
		{
			int value = GetSlotValue(8);
			if (BlocksManager.Blocks[Terrain.ExtractContents(value)].Durability <= 0)
				return;
			int a = 40;
			for (int x1 = -a+point.X; x1 <  a + 1 + point.X; x1++)
			{
				for (int y1 = 0; y1 < point.Y; y1++)
				{
					for (int z1 = -a + point.Z; z1 <  a + 1 + point.Z; z1++)
					{
						int cellValue = Utils.Terrain.GetCellValueFast(x1, y1, z1);
						var block = BlocksManager.Blocks[Terrain.ExtractContents(cellValue)];
						if (block.BlockIndex == BasaltBlock.Index && cellValue!= BasaltBlock.Index && cellValue !=536870979 && cellValue !=1073741891 && cellValue != 1610612803)
						{
							
							ComponentElectricDriller inventory = m_componentBlockEntity.Entity.FindComponent<ComponentElectricDriller>(throwOnError: true);
							if (AcquireItems(inventory, cellValue, 1) != 0)
							{
								Charged = false;
								return;
							}
								
							Utils.SubsystemTerrain.ChangeCell(x1, y1, z1, 0, true);
							DrillType type = DrillBlock.GetType(value);
							RemoveSlotItems(8, 1);
							if (DrillBlock.GetType(BlocksManager.DamageItem(value, 1)) == type && BlocksManager.DamageItem(value, 1) != value && GetDamage(BlocksManager.DamageItem(value, 1)) != GetDamage(value))
							{
								AddSlotItems(8, BlocksManager.DamageItem(value, 1), 1);
							}
							return;
						}
						if (block.BlockIndex==IronOreBlock.Index|| block.BlockIndex==CopperOreBlock.Index|| block.BlockIndex == DiamondOreBlock.Index || block.BlockIndex==GermaniumOreBlock.Index || block.BlockIndex ==SaltpeterOreBlock.Index || block.BlockIndex==SulphurOreBlock.Index|| block.BlockIndex==CoalOreBlock.Index)
						{
							
							ComponentElectricDriller inventory = m_componentBlockEntity.Entity.FindComponent<ComponentElectricDriller>(throwOnError: true);
							if (AcquireItems(inventory, cellValue, 1) != 0)
							{
								Charged = false;
								return;
							}
							Utils.SubsystemTerrain.ChangeCell(x1, y1, z1, 0, true);
							DrillType type = DrillBlock.GetType(value);
							RemoveSlotItems(8, 1);
							if (DrillBlock.GetType(BlocksManager.DamageItem(value, 1 )) == type && BlocksManager.DamageItem(value, 1) != value && GetDamage(BlocksManager.DamageItem(value, 1)) != GetDamage(value))
							{
								AddSlotItems(8, BlocksManager.DamageItem(value, 1 ), 1);
							}
							return;
						}
					}
				}
			}
		}
		public virtual int GetDamage(int value)
		{
			return (Terrain.ExtractData(value) >> 4) & 0xFFF;
		}
		public override int GetSlotCapacity(int slotIndex, int value)
		{
			if (slotIndex != 8 && slotIndex != 9)
				return base.GetSlotCapacity(slotIndex, value);
			if (slotIndex == 8 && Terrain.ExtractContents(value) == DrillBlock.Index)
			{
				var type = DrillBlock.GetType(value);
				return (type == DrillType.DiamondDrill || type == DrillType.SteelDrill) ? base.GetSlotCapacity(slotIndex, value) : 0;
			}
			if (slotIndex == 9 && Terrain.ExtractContents(value) == IEBatteryBlock.Index)
			{
				var type = IEBatteryBlock.GetType(value);
				return (type == BatteryType.Cu_Zn_Battery || type == BatteryType.Lead_Battery || type == BatteryType.Fission_Battery || type == BatteryType.Fusion_Battery) ? base.GetSlotCapacity(slotIndex, value) : 0;
			}
			return 0;

		}

		
	}
}