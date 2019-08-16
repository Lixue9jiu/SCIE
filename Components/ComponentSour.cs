using Engine;
using System;

namespace Game
{
	public class ComponentSour : ComponentSeperator, IUpdateable
	{
		public new void Update(float dt)
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
					//m_music = 0;
				}
			}
			if (m_smeltingRecipe2 != null)
			{
				if (m_smeltingRecipe == null)
					m_smeltingRecipe = m_smeltingRecipe2;
			}

			if (m_smeltingRecipe == null)
			{
				HeatLevel = 0f;

				//m_music = -1;
			}
			else

			if (m_smeltingRecipe != null)
			{
				m_fireTimeRemaining = MathUtils.Min(m_fireTimeRemaining - 0.001f * dt, 1f);
				SmeltingProgress = 1f - m_fireTimeRemaining;
				//SmeltingProgress = MathUtils.Min(SmeltingProgress + 0.001f * dt, 1f);
				//m_fireTimeRemaining = SmeltingProgress;
				if (SmeltingProgress >= 1f)
				{
					if (m_slots[0].Count > 0)
						m_slots[0].Count--;
					for (int j = 0; j < 2; j++)
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
			}
		}

		public override int RemoveSlotItems(int slotIndex, int count)
		{
			m_updateSmeltingRecipe = true;
			return m_smeltingRecipe != null && slotIndex != 1 ? 0 : base.RemoveSlotItems(slotIndex, count);
		}

		protected new string FindSmeltingRecipe()
		{
			Array.Clear(result, 0, 3);
			bool text = false;
			int i;
			for (i = 0; i < m_furnaceSize; i++)
			{
				if (GetSlotCount(i) <= 0) continue;
				int content = Terrain.ExtractContents(GetSlotValue(i));
				if (content == RottenMilkBucketBlock.Index || content == RottenPumpkinSoupBucketBlock.Index)
				{
					text = true;
					result[0] = SaltpeterChunkBlock.Index;
					result[1] = EmptyBucketBlock.Index;
					result[2] = 0;
				}
				else if (content < 255 && BlocksManager.Blocks[content] is FoodBlock || content == RottenMeatBlock.Index || content == RottenEggBlock.Index)
				{
					text = true;
					result[0] = SaltpeterChunkBlock.Index;
					result[1] = 0;
					result[2] = 0;
				}
			}
			if (!text)
				return null;
			for (i = 0; i < 3; i++)
			{
				Slot slot = m_slots[1 + i];
				if (slot.Count != 0 && result[i] != 0 && (slot.Value != result[i] || slot.Count >= 40))
					return null;
			}
			return "AluminumOrePowder";
		}
	}
}