using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	public class ComponentCanpack : ComponentMachine, IUpdateable
	{
		public bool Powered;

		protected readonly int[] result = new int[1];

		protected string m_smeltingRecipe;

		//protected int m_music;

		protected string m_smeltingRecipe2;

		public override int RemainsSlotIndex => -1;

		public override int ResultSlotIndex => SlotsCount - 1;

		public override int FuelSlotIndex => -1;

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

			
			if (m_smeltingRecipe != null)
			{
				SmeltingProgress = MathUtils.Min(SmeltingProgress + 0.15f * dt, 1f);
				if (SmeltingProgress >= 1f)
				{
					if (m_slots[0].Count > 0)
					{
						m_slots[0].Count--;
						m_slots[1].Count -= 5;
					}

					if (result[0] != 0)
					{
						int value = result[0];
						m_slots[2 + 0].Value = value;
						m_slots[2 + 0].Count++;
						m_smeltingRecipe = null;
						SmeltingProgress = 0f;
						m_updateSmeltingRecipe = true;
					}
				}
			}
		}

		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			base.Load(valuesDictionary, idToEntityMap);
			m_furnaceSize = SlotsCount - 1;
		}

		protected string FindSmeltingRecipe()
		{
			bool text = false;
			result[0] = 0;
			//int i;
			// Log.Information(GetSlotValue(0) == ItemBlock.IdTable["Empty"]);
			// Log.Information(GetSlotValue(1) == CookedMeatBlock.Index);
			// Log.Information(GetSlotCount(0) >= 1 && GetSlotCount(1) >= 5);
			int empty = ItemBlock.IdTable["Empty"];
			if (base.GetSlotValue(0) == empty && base.GetSlotValue(1) == CookedMeatBlock.Index && base.GetSlotCount(0) >= 1 && base.GetSlotCount(1) >= 2)
			{
				text = true;
				result[0] = ItemBlock.IdTable["Meat"];
			}
			if (base.GetSlotValue(0) == empty && base.GetSlotValue(1) == CookedBirdBlock.Index && base.GetSlotCount(0) >= 1 && base.GetSlotCount(1) >= 2)
			{
				text = true;
				result[0] = ItemBlock.IdTable["Chicken"];
			}
			if (base.GetSlotValue(0) == empty && base.GetSlotValue(1) == CookedFishBlock.Index && base.GetSlotCount(0) >= 1 && base.GetSlotCount(1) >= 2)
			{
				text = true;
				result[0] = ItemBlock.IdTable["Fish"];
			}
			if (base.GetSlotValue(0) == empty && base.GetSlotValue(1) == BreadBlock.Index && base.GetSlotCount(0) >= 1 && base.GetSlotCount(1) >= 2)
			{
				text = true;
				result[0] = ItemBlock.IdTable["Bread"];
			}
			if (base.GetSlotValue(0) == empty && base.GetSlotValue(1) == PumpkinBlock.Index && base.GetSlotCount(0) >= 1 && base.GetSlotCount(1) >= 2)
			{
				text = true;
				result[0] = ItemBlock.IdTable["Pumpkin"];
			}
			if (!text)
			{ return null; }

			Slot slot = m_slots[2 + 0];
			if (slot.Count != 0 && result[0] != 0 && (slot.Value != result[0] || slot.Count >= 40))
			{ return null; }

			return "AluminumOrePowder";
		}
	}
}