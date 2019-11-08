using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	public class ComponentTGenerator : ComponentSeparator, IUpdateable
	{
		protected float m_speed = 0.01f;

		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			m_speed = valuesDictionary.GetValue("Speed", 0.01f);
			base.Load(valuesDictionary, idToEntityMap);
		}

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
					if (m_fireTimeRemaining <= 0f)
						m_fireTimeRemaining = 1f;
				}
			}
			if (m_smeltingRecipe2 != 0 && m_smeltingRecipe == 0)
				m_smeltingRecipe = m_smeltingRecipe2;
			if (m_smeltingRecipe != 0)
			{
				Powered = true;
				m_fireTimeRemaining = MathUtils.Min(m_fireTimeRemaining - 0.05f * dt, 1f);
				SmeltingProgress = 1f - m_fireTimeRemaining;
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
			else
				Powered = false;
		}

		protected override int FindSmeltingRecipe()
		{
			result.Clear();
			int text = 0;
			int i;
			for (i = 0; i < 1; i++)
			{
				if (GetSlotCount(i) <= 0) continue;
				int value = GetSlotValue(i);
				if (m_speed < 0.05f)
				{
					if (Terrain.ExtractContents(value) == MagmaBucketBlock.Index)
					{
						text = 1;
						result[EmptyBucketBlock.Index] = 1;
						result[BasaltBlock.Index] = 1;
					}
				}
				else if (value == ItemBlock.IdTable["H2"])
				{
					text = 1;
					result[ItemBlock.IdTable["¸ÖÆ¿"]] = 1;
				}
			}
			return FindSmeltingRecipe(text);
		}
	}
}