using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	public class ComponentEngineA : ComponentMachine, IUpdateable
	{
		protected readonly string[] m_matchedIngredients = new string[9];

		protected string m_smeltingRecipe;

		//protected int m_music;

		public override int RemainsSlotIndex => SlotsCount - 2;

		public override int ResultSlotIndex => SlotsCount - 1;

		public override int FuelSlotIndex => SlotsCount;

		public int UpdateOrder => 0;

		public void Update(float dt)
		{
			if (m_updateSmeltingRecipe)
			{
				m_updateSmeltingRecipe = false;

				string text = null;
				int remainsSlotIndex = RemainsSlotIndex;
				int num = Terrain.ExtractContents(GetSlotValue(remainsSlotIndex));

				if (GetSlotCount(remainsSlotIndex) > 0 && num == WaterBucketBlock.Index)
				{
					text = "bucket";
				}
				//while (text != null && SmeltingProgress <= 900f)
				//{
				if (text != null)
				{
					if (m_slots[ResultSlotIndex].Count != 0 && (90 != m_slots[ResultSlotIndex].Value || 1 + m_slots[ResultSlotIndex].Count > 40))
					{
						text = null;
					}
					else
					{
						if (m_slots[remainsSlotIndex].Count > 0)
						{
							m_slots[ResultSlotIndex].Value = 90;
							m_slots[ResultSlotIndex].Count += 1;
							m_slots[remainsSlotIndex].Count = 0;
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
			{
				HeatLevel = 0f;
			}
		}

		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			base.Load(valuesDictionary, idToEntityMap);
			m_furnaceSize = SlotsCount - 3;
			m_fireTimeRemaining = valuesDictionary.GetValue<float>("FireTimeRemaining");
			HeatLevel = valuesDictionary.GetValue<float>("HeatLevel");
			m_updateSmeltingRecipe = true;
		}

		public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
		{
			base.Save(valuesDictionary, entityToIdMap);
			valuesDictionary.SetValue("FireTimeRemaining", m_fireTimeRemaining);
			valuesDictionary.SetValue("HeatLevel", HeatLevel);
		}

		public override int GetSlotCapacity(int slotIndex, int value)
		{
			if ((slotIndex == RemainsSlotIndex && Terrain.ExtractContents(value) == WaterBucketBlock.Index) ||
				(slotIndex == ResultSlotIndex && Terrain.ExtractContents(value) == EmptyBucketBlock.Index) ||
				(slotIndex != RemainsSlotIndex && slotIndex != ResultSlotIndex))
			{
				return base.GetSlotCapacity(slotIndex, value);
			}
			return 0;
		}
	}
}