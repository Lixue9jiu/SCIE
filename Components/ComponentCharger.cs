using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	public class ComponentCharger : ComponentInventoryBase, IUpdateable
	{
		public bool Powered,
					Powered2,
					Charged;

		protected int time = 0;

		public int UpdateOrder => 0;

		public void Update(float dt)
		{
			//Point3 coordinates = m_componentBlockEntity.Coordinates;
			//int data = Terrain.ExtractData(Utils.Terrain.GetCellValue(coordinates.X, coordinates.Y, coordinates.Z));
			//MachineMode1 mode = Charger.GetMode(data);
			//if (mode == MachineMode1.Charge)
			//{
			//	Charged = true;
			//}
			//else
			//{
			//	Charged = false;
			//}
			time++;
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

					if (value != 0 && slotCount > 0 && BlocksManager.DamageItem(value, 1) != 0 && GetDamage(value) != BlocksManager.Blocks[Terrain.ExtractContents(value)].Durability && value != BlocksManager.DamageItem(value, 1))
						break;

					num++;
				}
				if (time % 20 == 0)
				{
					RemoveSlotItems(num, 1);
					AddSlotItems(num, BlocksManager.DamageItem(value, 1), 1);
				}
				Powered2 = true;
			}
			else
				Powered2 = false;
		}

		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.LoadItems(valuesDictionary);
			Charged = valuesDictionary.GetValue("Charged", false);
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

		public static int GetDamage(int value)
		{
			return (Terrain.ExtractData(value) >> 4) & 0xFFF;
		}
	}
}