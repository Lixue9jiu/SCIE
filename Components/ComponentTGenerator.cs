using Engine;
using GameEntitySystem;
using System;
using TemplatesDatabase;

namespace Game
{
	public class ComponentTGenerator : ComponentMachine, IUpdateable
	{
		public bool Powered;

		protected readonly int[] result = new int[3];

		protected string m_smeltingRecipe, m_smeltingRecipe2;

		//protected int m_music;
		
		public override int RemainsSlotIndex => -1;

		public override int ResultSlotIndex => SlotsCount - 1;

		//public float m_fireTimeRemaining;

		public int UpdateOrder => 0;

		public void Update(float dt)
		{
			if (m_updateSmeltingRecipe)
			{
				m_updateSmeltingRecipe = false;
				m_smeltingRecipe2 = FindSmeltingRecipe();
				if (m_smeltingRecipe2 != m_smeltingRecipe)
				{
					m_smeltingRecipe = m_smeltingRecipe2;
					SmeltingProgress = 0f;
					if (m_fireTimeRemaining == 0f)
						m_fireTimeRemaining = 1f;
				}

			}
			if (m_smeltingRecipe2 != null)
			{
				
				if (m_smeltingRecipe == null)
					m_smeltingRecipe = m_smeltingRecipe2;

			}
			if (m_smeltingRecipe != null)
			{
				Powered = true;
				m_fireTimeRemaining = MathUtils.Min(m_fireTimeRemaining - 0.01f * dt, 1f);
				SmeltingProgress = 1f - m_fireTimeRemaining;
				if (SmeltingProgress >= 1f)
				{
					if (m_slots[0].Count > 0)
						m_slots[0].Count--;
					for (int j = 0; j < 3; j++)
					{
						if (result[j] != 0)
						{
							m_slots[1 + j].Value = result[j];
							m_slots[1 + j].Count++;
							m_smeltingRecipe = null;
							SmeltingProgress = 0f;
							m_updateSmeltingRecipe = true;
							m_fireTimeRemaining = 0f;
						}
					}
				}
			}else
			{
				Powered = false;
				//m_fireTimeRemaining = 0f;
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
			Array.Clear(result, 0, 3);
			string text = null;
			int i;
			for (i = 0; i < m_furnaceSize; i++)
			{
				if (GetSlotCount(i) <= 0) continue;
				int value = GetSlotValue(i);
				if (value == MagmaBucketBlock.Index )
				{
					text = "AluminumOrePowder";
					result[0] = EmptyBucketBlock.Index;
					result[1] = BasaltBlock.Index;
					
				}
			}
			if (text == null)
				return null;
			for (i = 0; i < 3; i++)
			{
				Slot slot = m_slots[1 + i];
				if (slot.Count != 0 && result[i] != 0 && (slot.Value != result[i] || slot.Count >= 40))
					return null;
			}
			return "";
		}
	}
}