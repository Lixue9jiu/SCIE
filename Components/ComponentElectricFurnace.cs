using Engine;
using GameEntitySystem;
using System.Globalization;
using TemplatesDatabase;

namespace Game
{
	public class ComponentElectricFurnace : ComponentFurnace, IUpdateable, ICraftingMachine
	{
		public int SlotIndex { get; set; }
		public bool Powered;
		protected CraftingRecipe m_smeltingRecipe2;

		public CraftingRecipe GetRecipe()
		{
			return m_smeltingRecipe;
		}

		public new int RemainsSlotIndex => SlotsCount - 3;

		public new int ResultSlotIndex => SlotsCount - 4;

		public new int FuelSlotIndex => SlotsCount - 0;

		public int Cir1SlotIndex => SlotsCount - 2;

		public int Cir2SlotIndex => SlotsCount - 1;

		public new void Update(float dt)
		{
			Point3 coordinates = m_componentBlockEntity.Coordinates;
			if (HeatLevel > 0f)
			{
				m_fireTimeRemaining = MathUtils.Max(0f, m_fireTimeRemaining - dt);
				if (m_fireTimeRemaining == 0f)
				{
					m_heatLevel = 0f;
				}
			}
			if (m_updateSmeltingRecipe)
			{
				m_updateSmeltingRecipe = false;
				float heatLevel = 0f;
				if (HeatLevel > 0f)
				{
					heatLevel = HeatLevel;
				}
				else
				{
					heatLevel = 2000f;
				}
				CraftingRecipe craftingRecipe = FindSmeltingRecipe(2000f);
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
				{
					m_smeltingRecipe = m_smeltingRecipe2;
				}
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
				m_smeltingProgress = MathUtils.Min(SmeltingProgress + 0.15f * dt, 1f);
				if (SmeltingProgress >= 1f)
				{
					for (int i = 0; i < m_furnaceSize; i++)
					{
						if (m_slots[i].Count > 0)
						{
							m_slots[i].Count--;
						}
					}
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

		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			//base.Load(valuesDictionary, idToEntityMap);
			m_matchedIngredients = new string[36];
			int count = valuesDictionary.GetValue<int>("SlotsCount");
			m_slots.Capacity = count;
			int i;
			for (i = 0; i < count; i++)
			{
				m_slots.Add(new Slot());
			}
			ValuesDictionary value2 = valuesDictionary.GetValue<ValuesDictionary>("Slots");
			for (i = 0; i < m_slots.Count; i++)
			{
				ValuesDictionary value3 = value2.GetValue<ValuesDictionary>("Slot" + i.ToString(CultureInfo.InvariantCulture), null);
				if (value3 != null)
				{
					Slot slot = m_slots[i];
					slot.Value = value3.GetValue<int>("Contents");
					slot.Count = value3.GetValue<int>("Count");
				}
			}
			//m_subsystemTerrain = Project.FindSubsystem<SubsystemTerrain>(true);
			//m_subsystemExplosions = Project.FindSubsystem<SubsystemExplosions>(true);
			m_componentBlockEntity = Entity.FindComponent<ComponentBlockEntity>(true);
			m_fireTimeRemaining = valuesDictionary.GetValue<float>("FireTimeRemaining");
			m_heatLevel = valuesDictionary.GetValue<float>("HeatLevel");
			m_updateSmeltingRecipe = true;
			m_furnaceSize = SlotsCount - 5;
			m_fireTimeRemaining = valuesDictionary.GetValue<float>("FireTimeRemaining");
			m_updateSmeltingRecipe = true;
		}

		public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
		{
			base.Save(valuesDictionary, entityToIdMap);
			valuesDictionary.SetValue("FireTimeRemaining", m_fireTimeRemaining);
			valuesDictionary.SetValue("HeatLevel", HeatLevel);
		}
	}
}