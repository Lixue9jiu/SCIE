using Engine;
using System;

namespace Game
{
	public class ComponentTGenerator : ComponentSeperator, IUpdateable
	{
		public override int FuelSlotIndex => SlotsCount - 2;

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
			}
			else
			{
				Powered = false;
				//m_fireTimeRemaining = 0f;
			}
		}

		protected string FindSmeltingRecipe()
		{
			Array.Clear(result, 0, 3);
			string text = null;
			int i;
			for (i = 0; i < m_furnaceSize; i++)
			{
				if (GetSlotCount(i) <= 0) continue;
				if (Terrain.ExtractContents(GetSlotValue(i)) == MagmaBucketBlock.Index)
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