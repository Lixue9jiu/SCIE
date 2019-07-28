using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	public class ComponentEngineA : ComponentMachine, IUpdateable
	{
		protected string m_smeltingRecipe;

		//protected int m_music;

		public override int RemainsSlotIndex => SlotsCount - 2;

		public override int ResultSlotIndex => SlotsCount - 1;

		public override int FuelSlotIndex => -1;

		public int UpdateOrder => 0;

		public void Update(float dt)
		{
			if (m_updateSmeltingRecipe)
			{
				m_updateSmeltingRecipe = false;
				string text = null;
				if (base.GetSlotCount(RemainsSlotIndex) > 0 && Terrain.ExtractContents(base.GetSlotValue(RemainsSlotIndex)) == (RottenMeatBlock.Index | 2 << 14))
					text = "bucket";
				//while (text != null && SmeltingProgress <= 900f)
				//{
				if (text != null)
				{
					int resultSlotIndex = ResultSlotIndex;
					if (m_slots[resultSlotIndex].Count != 0 && (90 != m_slots[resultSlotIndex].Value || m_slots[resultSlotIndex].Count >= 40))
						text = null;
					else
					{
						if (m_slots[RemainsSlotIndex].Count > 0)
						{
							m_slots[resultSlotIndex].Value = 90;
							m_slots[resultSlotIndex].Count += 1;
							m_slots[RemainsSlotIndex].Count = 0;
							SmeltingProgress += 50f;
						}
						m_fireTimeRemaining = SmeltingProgress;
					}
				}
				//}
			}
			SmeltingProgress = m_fireTimeRemaining;
			if (SmeltingProgress > 0f)
			{
				SmeltingProgress = MathUtils.Min(SmeltingProgress - 1f * dt, 1000f);
				m_fireTimeRemaining = SmeltingProgress;
				HeatLevel = 1000f;
			}
			else
				HeatLevel = 0f;
		}

		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			base.Load(valuesDictionary, idToEntityMap);
			m_furnaceSize = SlotsCount - 3;
			m_fireTimeRemaining = valuesDictionary.GetValue("FireTimeRemaining", 0f);
			HeatLevel = valuesDictionary.GetValue("HeatLevel", 0f);
		}

		public override int GetSlotCapacity(int slotIndex, int value)
		{
			return (slotIndex == RemainsSlotIndex && Terrain.ExtractContents(value) == WaterBucketBlock.Index) ||
				(slotIndex == ResultSlotIndex && Terrain.ExtractContents(value) == EmptyBucketBlock.Index) ||
				(slotIndex != RemainsSlotIndex && slotIndex != ResultSlotIndex)
				? base.GetSlotCapacity(slotIndex, value)
				: 0;
		}
	}
}