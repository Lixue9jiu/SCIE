using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	public class ComponentICEngine : ComponentMachine, IUpdateable
	{
		public override int RemainsSlotIndex => SlotsCount - 0;
		public override int ResultSlotIndex => SlotsCount - 1;
		//public int resultSlotIndex => SlotsCount - 1;
		public override int FuelSlotIndex => -1;
		public int Fuel2SlotIndex => 0;
		public int UpdateOrder => 0;

		public void Update(float dt)
		{
			if (m_updateSmeltingRecipe)
			{
				m_updateSmeltingRecipe = false;
				string text = null;
				if (base.GetSlotCount(Fuel2SlotIndex) > 0 && Terrain.ExtractContents(base.GetSlotValue(Fuel2SlotIndex)) == (WaterBucketBlock.Index))
					text = "bucket";
				//while (text != null && SmeltingProgress <= 900f)
				//{
				if (text != null)
				{
					int resultSlotIndex = ResultSlotIndex;
					if (m_slots[ResultSlotIndex].Count != 0 && (90 != m_slots[ResultSlotIndex].Value || m_slots[ResultSlotIndex].Count >= 40))
						text = null;
					else
					{
						if (m_slots[Fuel2SlotIndex].Count > 0)
						{
							m_slots[ResultSlotIndex].Value = 90;
							m_slots[ResultSlotIndex].Count += 1;
							m_slots[Fuel2SlotIndex].Count = 0;
							SmeltingProgress += 50f;
						}
						m_fireTimeRemaining = SmeltingProgress;
					}
				}
				//}
			}
			SmeltingProgress = m_fireTimeRemaining;
			if (SmeltingProgress > 0f && HeatLevel > 0f)
			{
				SmeltingProgress = MathUtils.Min(SmeltingProgress - 1f * dt, 1000f);
				m_fireTimeRemaining = SmeltingProgress;
				//HeatLevel = 1000f;
			}
		}

		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			base.Load(valuesDictionary, idToEntityMap);
			m_furnaceSize = SlotsCount - 1;
			m_fireTimeRemaining = valuesDictionary.GetValue("FireTimeRemaining", 0f);
			HeatLevel = valuesDictionary.GetValue("HeatLevel", 0f);
		}
		public override int GetSlotCapacity(int slotIndex, int value)
		{
			return (slotIndex == Fuel2SlotIndex && Terrain.ExtractContents(value) == WaterBucketBlock.Index) ||
				(slotIndex == ResultSlotIndex && Terrain.ExtractContents(value) == EmptyBucketBlock.Index) ||
				(slotIndex != Fuel2SlotIndex && slotIndex != ResultSlotIndex)
				? base.GetSlotCapacity(slotIndex, value)
				: 0;
		}
	}
}