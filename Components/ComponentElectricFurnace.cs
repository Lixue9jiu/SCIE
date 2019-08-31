using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	public class ComponentElectricFurnace : ComponentFurnace, IUpdateable, IElectricMachine
	{
		public int SlotIndex { get; set; }
		public bool Powered;
		protected float m_speed;
		protected CraftingRecipe m_smeltingRecipe2;

		public CraftingRecipe GetRecipe() => m_smeltingRecipe;

		public new int RemainsSlotIndex => SlotsCount - 3;

		public new int ResultSlotIndex => SlotsCount - 4;

		public new int FuelSlotIndex => SlotsCount;

		public int Cir1SlotIndex => SlotsCount - 2;

		public int Cir2SlotIndex => SlotsCount - 1;

		public new void Update(float dt)
		{
			if (HeatLevel > 0f)
			{
				m_fireTimeRemaining = MathUtils.Max(0f, m_fireTimeRemaining - dt);
				if (m_fireTimeRemaining == 0f)
					m_heatLevel = 0f;
			}
			if (m_updateSmeltingRecipe)
			{
				m_updateSmeltingRecipe = false;
				CraftingRecipe craftingRecipe = FindSmeltingRecipe2(2000f);
				if (craftingRecipe != m_smeltingRecipe)
				{
					m_smeltingRecipe = craftingRecipe;
					m_smeltingRecipe2 = craftingRecipe;
					m_smeltingProgress = 0f;
				}
			}
			if (m_smeltingRecipe2 != null)
			{
				if (!Powered)
				{
					m_smeltingProgress = 0f;
					m_heatLevel = 0f;
					m_smeltingRecipe = null;
				}
				else if (m_smeltingRecipe == null)
					m_smeltingRecipe = m_smeltingRecipe2;
			}
			if (!Powered)
			{
				m_smeltingProgress = 0f;
				m_heatLevel = 0f;
				m_smeltingRecipe = null;
			}
			if (m_smeltingRecipe == null)
			{
				m_heatLevel = 0f;
				m_fireTimeRemaining = 0f;
			}
			if (m_smeltingRecipe != null && m_fireTimeRemaining <= 0f)
			{
				m_heatLevel = 2000f;
				m_fireTimeRemaining = 100f;
			}
			if (m_fireTimeRemaining <= 0f)
			{
				m_smeltingRecipe = null;
				m_smeltingProgress = 0f;
			}
			if (m_smeltingRecipe != null)
			{
				m_smeltingProgress = MathUtils.Min(SmeltingProgress + m_speed * dt, 1f);
				if (SmeltingProgress >= 1f)
				{
					for (int i = 0; i < m_furnaceSize; i++)
						if (m_slots[i].Count > 0)
							m_slots[i].Count--;
					m_slots[ResultSlotIndex].Value = m_smeltingRecipe.ResultValue;
					m_slots[ResultSlotIndex].Count += m_smeltingRecipe.ResultCount;
					if (m_smeltingRecipe.RemainsValue != 0 && m_smeltingRecipe.RemainsCount > 0)
					{
						m_slots[RemainsSlotIndex].Value = m_smeltingRecipe.RemainsValue;
						m_slots[RemainsSlotIndex].Count += m_smeltingRecipe.RemainsCount;
					}
					m_smeltingRecipe = null;
					m_smeltingRecipe2 = null;
					m_smeltingProgress = 0f;
					m_updateSmeltingRecipe = true;
				}
			}
		}

		public virtual CraftingRecipe FindSmeltingRecipe2(float heatlevel)
		{
			return FindSmeltingRecipe(heatlevel);
		}

		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			m_matchedIngredients = new string[36];
			this.LoadItems(valuesDictionary);
			m_componentBlockEntity = Entity.FindComponent<ComponentBlockEntity>(true);
			m_fireTimeRemaining = valuesDictionary.GetValue("FireTimeRemaining", 0f);
			m_furnaceSize = SlotsCount - 5;
			m_updateSmeltingRecipe = true;
			m_speed = 0.2f;
		}

		public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
		{
			this.SaveItems(valuesDictionary);
			valuesDictionary.SetValue("FireTimeRemaining", m_fireTimeRemaining);
		}
	}
}
