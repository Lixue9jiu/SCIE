using Engine;
using GameEntitySystem;
using System.Globalization;
using TemplatesDatabase;
using static Game.Charger;
namespace Game
{
	public class ComponentCharger : ComponentInventoryBase, IUpdateable
	{
		//public int SlotIndex { get; set; }
		public bool Powered;
		public bool Powered2;
		public bool Charged;
		protected ComponentBlockEntity m_componentBlockEntity;

		public int UpdateOrder => 0;

		public void Update(float dt)
		{
			Point3 coordinates = m_componentBlockEntity.Coordinates;
			int data = Terrain.ExtractData(Utils.Terrain.GetCellValue(coordinates.X, coordinates.Y, coordinates.Z));
			MachineMode1 mode = Charger.GetMode(data);
			if (mode==MachineMode1.Charge)
			{
				Charged = true;
			}
			else
			{
				Charged = false;
			}

			if (Charged && Powered==true)
			{
				int num = 0;
				int value;
				while (true)
				{
					if (num >= SlotsCount - 1)
						return;
					value = GetSlotValue(num);
					int slotCount = GetSlotCount(num);
					if (value != 0 && slotCount > 0)
						break;
					num++;
					
				}
				//RemoveSlotItems(num, 1);
				//AddSlotItems(num, BlocksManager.DamageItem(value, -1), 1);

			}
			if (!Charged && m_slots[0].Count + m_slots[1].Count + m_slots[2].Count + m_slots[3].Count!=0)
			{
				int num = 0;
				int value;
				while (true)
				{
					if (num >= SlotsCount - 1)
						return;
					value = GetSlotValue(num);
					int slotCount = GetSlotCount(num);
					if (value != 0 && slotCount > 0)
						break;
					num++;
					
				}
				RemoveSlotItems(num, 1);
				AddSlotItems(num, BlocksManager.DamageItem(value, 1), 1);
				Powered2 = true;

			}
			else { Powered2 = false; }

		}

		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			//base.Load(valuesDictionary, idToEntityMap);
			
			this.LoadItems(valuesDictionary);
			m_componentBlockEntity = Entity.FindComponent<ComponentBlockEntity>(true);
			//m_subsystemExplosions = Project.FindSubsystem<SubsystemExplosions>(true);
			//m_componentBlockEntity = Entity.FindComponent<ComponentBlockEntity>(true);

			//m_furnaceSize = SlotsCount - 1;
			//m_fireTimeRemaining = valuesDictionary.GetValue("FireTimeRemaining", 0f);
			//m_updateSmeltingRecipe = true;
		}

		public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
		{
			this.SaveItems(valuesDictionary);
			//valuesDictionary.SetValue("FireTimeRemaining", m_fireTimeRemaining);
		}

		public override int GetSlotCapacity(int slotIndex, int value)
		{
			
			if (Terrain.ExtractContents(value) != IEBatteryBlock .Index)
				return 0;
			
			return base.GetSlotCapacity(slotIndex, value);
		}

	}
}

