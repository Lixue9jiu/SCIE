using Engine;
using GameEntitySystem;
using System.Collections.Generic;
using TemplatesDatabase;

namespace Game
{
	public class ComponentDeposit : ComponentMachine, IUpdateable
	{
		public bool Powered;

		protected readonly Dictionary<int, int> result = new Dictionary<int, int>();

		protected int m_smeltingRecipe, m_smeltingRecipe2;

		//protected int m_music;

		public override int RemainsSlotIndex => 0;

		public override int ResultSlotIndex => SlotsCount - 1;

		public override int FuelSlotIndex => -1;

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
			if (m_smeltingRecipe2 != 0)
			{
				
					m_smeltingRecipe = m_smeltingRecipe2;
			}
			
			if (m_smeltingRecipe == 0)
			{
				HeatLevel = 0f;
				m_fireTimeRemaining = 0f;
				//m_music = -1;
			}
			else
				m_fireTimeRemaining = 100f;
			if (m_fireTimeRemaining <= 0f)
			{
				m_smeltingRecipe = 0;
				SmeltingProgress = 0f;
				//m_music = -1;
			}
			if (m_smeltingRecipe != 0)
			{
				SmeltingProgress = MathUtils.Min(SmeltingProgress + 0.1f * dt, 1f);
				if (SmeltingProgress >= 1f)
				{
					var e = result.GetEnumerator();
					while (e.MoveNext())
					{
						Slot slot = m_slots[FindAcquireSlotForItem(this, e.Current.Key)];
						slot.Value = e.Current.Key;
						slot.Count += e.Current.Value;
					}
					if (m_slots[RemainsSlotIndex].Count > 0)
						m_slots[RemainsSlotIndex].Count--;
					m_smeltingRecipe = 0;
					SmeltingProgress = 0f;
					m_updateSmeltingRecipe = true;
				}
			}
		}

		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			base.Load(valuesDictionary, idToEntityMap);
			m_furnaceSize = SlotsCount - 1;
		}

		protected int FindSmeltingRecipe(int value)
		{
			if (value == 0)
				return 0;
			var e = result.GetEnumerator();
			while (e.MoveNext())
			{
				int index = FindAcquireSlotForItem(this, e.Current.Key);
				if (index < 0)
					return 0;
			}
			return value;
		}

		protected virtual int FindSmeltingRecipe()
		{
			result.Clear();
			int text = 0;
			int i;
			if (GetSlotValue(0) == ItemBlock.IdTable["CrudeSalt"] && GetSlotValue(4)==CanvasBlock.Index && GetSlotValue(5) == ItemBlock.IdTable["Bottle"])
			{
				text = 1;
				result[ItemBlock.IdTable["S-NaCl"]] = 1;
				result[ItemBlock.IdTable["Bottle"]] = -1;
			}
			return FindSmeltingRecipe(text);
		}
	}
}