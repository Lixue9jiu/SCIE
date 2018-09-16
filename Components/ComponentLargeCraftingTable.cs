using Engine;
using GameEntitySystem;
using System;
using System.Globalization;
using TemplatesDatabase;

namespace Game
{
	public class ComponentLargeCraftingTable : ComponentCraftingTable
	{
		public int SlotIndex;
		internal new string[] m_matchedIngredients = new string[36];
		public override void AddSlotItems(int slotIndex, int value, int count)
		{
			if (count > 0 && slotIndex >= 0 && slotIndex < m_slots.Count)
			{
				Slot slot = m_slots[slotIndex];
				if ((GetSlotCount(slotIndex) != 0 && GetSlotValue(slotIndex) != value) || GetSlotCount(slotIndex) + count > GetSlotCapacity(slotIndex, value))
				{
					throw new InvalidOperationException("Cannot add slot items.");
				}
				slot.Value = value;
				slot.Count += count;
			}
			UpdateCraftingResult();
		}

		public override int RemoveSlotItems(int slotIndex, int count)
		{
			int num = 0;
			if (slotIndex == ResultSlotIndex)
			{
				if (m_matchedRecipe != null)
				{
					if (m_matchedRecipe.RemainsValue != 0 && m_matchedRecipe.RemainsCount > 0)
					{
						if (m_slots[RemainsSlotIndex].Count == 0 || m_slots[RemainsSlotIndex].Value == m_matchedRecipe.RemainsValue)
						{
							int num2 = BlocksManager.Blocks[Terrain.ExtractContents(m_matchedRecipe.RemainsValue)].MaxStacking - m_slots[RemainsSlotIndex].Count;
							count = MathUtils.Min(count, num2 / m_matchedRecipe.RemainsCount);
						}
						else
						{
							count = 0;
						}
					}
					count = count / m_matchedRecipe.ResultCount * m_matchedRecipe.ResultCount;
					if (slotIndex >= 0 && slotIndex < m_slots.Count)
					{
						Slot slot = m_slots[slotIndex];
						count = MathUtils.Min(count, GetSlotCount(slotIndex));
						slot.Count -= count;
						num = count;
					}
					else
						num = 0;
					if (num > 0)
					{
						for (int index = 0; index < 36; index++)
						{
							if (!string.IsNullOrEmpty(m_matchedIngredients[index]))
							{
								int index2 = index % 6 + m_craftingGridSize * (index / 6);
								m_slots[index2].Count = MathUtils.Max(m_slots[index2].Count - num / m_matchedRecipe.ResultCount, 0);
							}
						}
						if (m_matchedRecipe.RemainsValue != 0 && m_matchedRecipe.RemainsCount > 0)
						{
							m_slots[RemainsSlotIndex].Value = m_matchedRecipe.RemainsValue;
							m_slots[RemainsSlotIndex].Count += num / m_matchedRecipe.ResultCount * m_matchedRecipe.RemainsCount;
						}
						ComponentPlayer componentPlayer = Entity.FindComponent<ComponentPlayer>();
						if (componentPlayer == null)
						{
							ComponentBlockEntity componentBlockEntity = Entity.FindComponent<ComponentBlockEntity>();
							if (componentBlockEntity != null)
							{
								Vector3 position = new Vector3(componentBlockEntity.Coordinates);
								componentPlayer = Project.FindSubsystem<SubsystemPlayers>(true).FindNearestPlayer(position);
							}
						}
						if (componentPlayer != null && componentPlayer.PlayerStats != null)
						{
							componentPlayer.PlayerStats.ItemsCrafted += num;
						}
					}
				}
			}
			else
			{
				if (slotIndex >= 0 && slotIndex < m_slots.Count)
				{
					Slot slot = m_slots[slotIndex];
					count = MathUtils.Min(count, GetSlotCount(slotIndex));
					slot.Count -= count;
					num = count;
				}
				else
					num = 0;
			}
			UpdateCraftingResult();
			return num;
		}
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			int value = valuesDictionary.GetValue<int>("SlotsCount");
			m_slots.Capacity = value;
			int i;
			for (i = 0; i < value; i++)
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
			m_craftingGridSize = (int)MathUtils.Sqrt((double)(SlotsCount - 2));
			if (m_craftingGridSize < 1 || m_craftingGridSize > 6)
			{
				throw new InvalidOperationException("Invalid crafting grid size.");
			}
			base.m_matchedIngredients = null;
		}
		public new void UpdateCraftingResult()
		{
			int num = 2147483647;
			for (int i = 0; i < m_craftingGridSize; i++)
			{
				for (int j = 0; j < m_craftingGridSize; j++)
				{
					int num2 = i + j * 6;
					int slotIndex = i + j * m_craftingGridSize;
					int slotValue = GetSlotValue(slotIndex);
					int num3 = Terrain.ExtractContents(slotValue);
					int num4 = Terrain.ExtractData(slotValue);
					int slotCount = GetSlotCount(slotIndex);
					if (slotCount > 0)
					{
						m_matchedIngredients[num2] = BlocksManager.Blocks[num3].CraftingId + ":" + num4.ToString(CultureInfo.InvariantCulture);
						num = MathUtils.Min(num, slotCount);
					}
					else
					{
						m_matchedIngredients[num2] = null;
					}
				}
			}
			CraftingRecipe craftingRecipe = CraftingRecipesManager.FindMatchingRecipe(Project.FindSubsystem<SubsystemTerrain>(true), m_matchedIngredients, 0f);
			if (craftingRecipe != null)
			{
				m_matchedRecipe = craftingRecipe;
				m_slots[ResultSlotIndex].Value = craftingRecipe.ResultValue;
				m_slots[ResultSlotIndex].Count = craftingRecipe.ResultCount * num;
			}
			else
			{
				m_matchedRecipe = null;
				m_slots[ResultSlotIndex].Value = 0;
				m_slots[ResultSlotIndex].Count = 0;
			}
		}
	}
}
