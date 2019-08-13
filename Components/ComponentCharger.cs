using GameEntitySystem;

//using System.Globalization;
using TemplatesDatabase;

//using System;
namespace Game
{
	public class ComponentCharger : ComponentInventoryBase, IUpdateable
	{
		//public int SlotIndex { get; set; }
		public bool Powered;

		public bool Powered2;
		public bool Charged;

		//	protected ComponentBlockEntity m_componentBlockEntity;
		private int time = 0;

		//public float m_fireTimeRemaining;
		public int UpdateOrder => 0;

		//public static Func<int, int, int> DamageItem1;
		public void Update(float dt)
		{
			//	Point3 coordinates = m_componentBlockEntity.Coordinates;
			//	int data = Terrain.ExtractData(Utils.Terrain.GetCellValue(coordinates.X, coordinates.Y, coordinates.Z));
			//	MachineMode1 mode = Charger.GetMode(data);
			//	if (mode == MachineMode1.Charge)
			//	{
			//		Charged = true;
			//	}
			//	else
			//	{
			//		Charged = false;
			//	}
			time += 1;
			if (time % 1 == 0)
			{
				if (Charged && Powered == true && m_slots[0].Count + m_slots[1].Count + m_slots[2].Count + m_slots[3].Count != 0)
				{
					int num = 0;
					int value;
					BatteryType type;
					while (true)
					{
						if (num >= SlotsCount - 0)
							return;
						value = GetSlotValue(num);
						int slotCount = GetSlotCount(num);
						type = IEBatteryBlock.GetType(value);

						if (value != 0 && slotCount > 0 && GetDamage(value) != 0)
							break;
						num++;
					}
					if (time % 20 == 0)
					{
						RemoveSlotItems(num, 1);
						AddSlotItems(num, BlocksManager.DamageItem(value, -1), 1);
					}
					return;
				}
				if (!Charged && m_slots[0].Count + m_slots[1].Count + m_slots[2].Count + m_slots[3].Count != 0)
				{
					int num = 0;
					int value;
					BatteryType type;
					while (true)
					{
						if (num >= SlotsCount - 0)
						{
							Powered2 = false;
							return;
						}

						value = GetSlotValue(num);
						int slotCount = GetSlotCount(num);
						type = IEBatteryBlock.GetType(value);
						if (value != 0 && slotCount > 0 && BlocksManager.DamageItem(value, 1) != 0 && GetDamage(value) != BlocksManager.Blocks[Terrain.ExtractContents(value)].Durability)
						{ break; }

						num++;
					}
					if (time % 20 == 0)
					{
						RemoveSlotItems(num, 1);
						AddSlotItems(num, BlocksManager.DamageItem(value, 1), 1);
					}
					Powered2 = true;
				}
				else { Powered2 = false; }
			}
		}

		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			//base.Load(valuesDictionary, idToEntityMap);
			this.LoadItems(valuesDictionary);
			//m_componentBlockEntity = Entity.FindComponent<ComponentBlockEntity>(true);
			//m_subsystemExplosions = Project.FindSubsystem<SubsystemExplosions>(true);
			//m_componentBlockEntity = Entity.FindComponent<ComponentBlockEntity>(true);
			//m_furnaceSize = SlotsCount - 1;  face == 4 || face == 5 ? 114 : 122
			Charged = valuesDictionary.GetValue("Charged", false);
			//m_updateSmeltingRecipe = true;
		}

		public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
		{
			this.SaveItems(valuesDictionary);
			valuesDictionary.SetValue("Charged", Charged);
		}

		public override int GetSlotCapacity(int slotIndex, int value)
		{
			if (Terrain.ExtractContents(value) != IEBatteryBlock.Index)
				return 0;

			return base.GetSlotCapacity(slotIndex, value);
		}

		public virtual int GetDamage(int value)
		{
			return (Terrain.ExtractData(value) >> 4) & 0xFFF;
		}
	}
}