using Engine;

namespace Game
{
	public class ComponentSour : ComponentSeparator, IUpdateable
	{
		public new void Update(float dt)
		{
			if (m_updateSmeltingRecipe)
			{
				m_updateSmeltingRecipe = false;
				m_smeltingRecipe2 = FindSmeltingRecipe(FindSmeltingRecipe());
				if (m_smeltingRecipe2 != m_smeltingRecipe)
				{
					m_smeltingRecipe = m_smeltingRecipe2;
					SmeltingProgress = 0f;
					if (m_fireTimeRemaining == 0f)
						m_fireTimeRemaining = 1f;
					//m_music = 0;
				}
			}
			if (m_smeltingRecipe2 != 0)
			{
				if (m_smeltingRecipe == 0)
					m_smeltingRecipe = m_smeltingRecipe2;
			}
			if (m_smeltingRecipe == 0)
			{
				HeatLevel = 0f;
				//m_music = -1;
			}
			else if (m_smeltingRecipe != 0)
			{
				m_fireTimeRemaining = MathUtils.Min(m_fireTimeRemaining - 0.001f * dt, 1f);
				SmeltingProgress = 1f - m_fireTimeRemaining;
				//m_fireTimeRemaining = SmeltingProgress;
				if (SmeltingProgress >= 1f)
				{
					var e = result.GetEnumerator();
					while (e.MoveNext())
					{
						Slot slot = m_slots[FindAcquireSlotForItem(this, e.Current.Key)];
						slot.Value = e.Current.Key;
						slot.Count += e.Current.Value;
						m_smeltingRecipe = 0;
						SmeltingProgress = 0f;
						m_updateSmeltingRecipe = true;
					}
					if (m_slots[0].Count > 0)
						m_slots[0].Count--;
				}
			}
		}

		public override int RemoveSlotItems(int slotIndex, int count)
		{
			m_updateSmeltingRecipe = true;
			return m_smeltingRecipe != 0 && slotIndex != 1 ? 0 : base.RemoveSlotItems(slotIndex, count);
		}

		protected override int FindSmeltingRecipe()
		{
			result.Clear();
			int text = 0;
			int i;
			for (i = 0; i < m_furnaceSize; i++)
			{
				if (GetSlotCount(i) <= 0) continue;
				int content = Terrain.ExtractContents(GetSlotValue(i));
				if (content == RottenMilkBucketBlock.Index || content == RottenPumpkinSoupBucketBlock.Index)
				{
					text = 1;
					result[SaltpeterChunkBlock.Index] = 1;
					result[EmptyBucketBlock.Index] = 1;
				}
				else if (content < 255 && BlocksManager.Blocks[content] is FoodBlock || content == RottenMeatBlock.Index || content == RottenEggBlock.Index)
				{
					text = 1;
					result[SaltpeterChunkBlock.Index] = 1;
				}
			}
			return text;
		}
	}
}