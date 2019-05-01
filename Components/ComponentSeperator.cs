using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	public class ComponentSeperator : ComponentMachine, IUpdateable
	{
		public bool Powered;

		protected readonly int[] result = new int[3];

		protected string m_smeltingRecipe;

		//protected int m_music;

		protected string m_smeltingRecipe2;

		public override int RemainsSlotIndex => -1;

		public override int ResultSlotIndex => SlotsCount - 1;

		public override int FuelSlotIndex => -1;

		public int UpdateOrder => 0;

		public void Update(float dt)
		{
			if (HeatLevel > 0f)
			{
				m_fireTimeRemaining = MathUtils.Max(0f, m_fireTimeRemaining - dt);
				if (m_fireTimeRemaining == 0f)
					HeatLevel = 0f;
			}
			if (m_updateSmeltingRecipe)
			{
				m_updateSmeltingRecipe = false;
				m_smeltingRecipe2 = FindSmeltingRecipe();
				if (m_smeltingRecipe2 != m_smeltingRecipe)
				{
					m_smeltingRecipe = m_smeltingRecipe2;
					SmeltingProgress = 0f;
					//m_music = 0;
				}
			}
			if (m_smeltingRecipe2 != null)
			{
				if (!Powered)
				{
					SmeltingProgress = 0f;
					HeatLevel = 0f;
					m_smeltingRecipe = null;
				}
				else if (m_smeltingRecipe == null)
					m_smeltingRecipe = m_smeltingRecipe2;
			}
			if (!Powered)
				m_smeltingRecipe = null;
			if (m_smeltingRecipe == null)
			{
				HeatLevel = 0f;
				m_fireTimeRemaining = 0f;
				//m_music = -1;
			}
			else
				m_fireTimeRemaining = 100f;
			if (m_fireTimeRemaining <= 0f)
			{
				m_smeltingRecipe = null;
				SmeltingProgress = 0f;
				//m_music = -1;
			}
			if (m_smeltingRecipe != null)
			{
				SmeltingProgress = MathUtils.Min(SmeltingProgress + 0.1f * dt, 1f);
				if (SmeltingProgress >= 1f)
				{
					if (m_slots[0].Count > 0)
						m_slots[0].Count--;
					for (int j = 0; j < 3; j++)
					{
						if (result[j] != 0)
						{
							int value = result[j];
							m_slots[1 + j].Value = value;
							m_slots[1 + j].Count++;
							m_smeltingRecipe = null;
							SmeltingProgress = 0f;
							m_updateSmeltingRecipe = true;
						}
					}
				}
			}
		}

		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			base.Load(valuesDictionary, idToEntityMap);
			m_furnaceSize = SlotsCount - 1;
			m_fireTimeRemaining = valuesDictionary.GetValue("FireTimeRemaining", 0f);
			HeatLevel = valuesDictionary.GetValue("HeatLevel", 0f);
		}

		protected string FindSmeltingRecipe()
		{
			bool text = false;
			result[0] = 0;
			result[1] = 0;
			result[2] = 0;
			int i;
			for (i = 0; i < m_furnaceSize; i++)
			{
				int slotValue = GetSlotValue(i);
				if (GetSlotCount(i) > 0)
				{
					if (slotValue == DirtBlock.Index)
					{
						text = true;
						result[0] = SandBlock.Index;
						result[1] = StoneChunkBlock.Index;
						int x = m_random.Int() & 3;
						if (x == 0)
							result[2] = SaltpeterChunkBlock.Index;
						if (x == 1)
							result[2] = ItemBlock.IdTable["AluminumOrePowder"];
					}
				}
			}
			if (text)
			{
				for (i = 0; i < 3; i++)
				{
					Slot slot = m_slots[1 + i];
					if (slot.Count != 0 && result[i] != 0 && (slot.Value != result[i] || slot.Count >= 40))
						text = false;
				}
			}
			return text ? "AluminumOrePowder" : null;
		}
	}
}