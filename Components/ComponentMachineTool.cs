using System;
using System.Globalization;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020000C4 RID: 196
	public class ComponentMachineTool : ComponentInventoryBase
	{
		// Token: 0x170000D7 RID: 215
		// (get) Token: 0x0600051A RID: 1306 RVA: 0x00005DEC File Offset: 0x00003FEC
		public int RemainsSlotIndex
		{
			get
			{
				return SlotsCount - 1;
			}
		}

		// Token: 0x170000D8 RID: 216
		// (get) Token: 0x0600051B RID: 1307 RVA: 0x00005DF6 File Offset: 0x00003FF6
		public int ResultSlotIndex
		{
			get
			{
				return SlotsCount - 2;
			}
		}

		// Token: 0x0600051C RID: 1308 RVA: 0x00005E00 File Offset: 0x00004000
		public override int GetSlotCapacity(int slotIndex, int value)
		{
			if (slotIndex < SlotsCount - 2)
			{
				return base.GetSlotCapacity(slotIndex, value);
			}
			return 0;
		}

		// Token: 0x0600051D RID: 1309 RVA: 0x00005E17 File Offset: 0x00004017
		public override void AddSlotItems(int slotIndex, int value, int count)
		{
			base.AddSlotItems(slotIndex, value, count);
			UpdateCraftingResult();
		}

		// Token: 0x0600051E RID: 1310 RVA: 0x00033314 File Offset: 0x00031514
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
					num = base.RemoveSlotItems(slotIndex, count);
					if (num > 0)
					{
						for (int index = 0; index < 16; index++)
						{
							if (!string.IsNullOrEmpty(m_matchedIngredients[index]))
							{
								int index2 = index % 4 + m_craftingGridSize * (index / 4);
								m_slots[index2].Count = MathUtils.Max(m_slots[index2].Count - num / m_matchedRecipe.ResultCount, 0);
							}
						}
						if (m_matchedRecipe.RemainsValue != 0 && m_matchedRecipe.RemainsCount > 0)
						{
							m_slots[RemainsSlotIndex].Value = m_matchedRecipe.RemainsValue;
							m_slots[RemainsSlotIndex].Count += num / m_matchedRecipe.ResultCount * m_matchedRecipe.RemainsCount;
						}
						ComponentPlayer componentPlayer = base.Entity.FindComponent<ComponentPlayer>();
						if (componentPlayer == null)
						{
							ComponentBlockEntity component = base.Entity.FindComponent<ComponentBlockEntity>();
							if (component != null)
							{
								componentPlayer = base.Project.FindSubsystem<SubsystemPlayers>(true).FindNearestPlayer(new Vector3(component.Coordinates));
							}
						}
						if (componentPlayer != null && componentPlayer.PlayerStats != null)
						{
							componentPlayer.PlayerStats.ItemsCrafted += (long)num;
						}
					}
				}
			}
			else
			{
				num = base.RemoveSlotItems(slotIndex, count);
			}
			UpdateCraftingResult();
			return num;
		}

		// Token: 0x0600051F RID: 1311 RVA: 0x00005E28 File Offset: 0x00004028
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			base.Load(valuesDictionary, idToEntityMap);
			m_craftingGridSize = (int)MathUtils.Sqrt((float)(SlotsCount - 2));
		}

		// Token: 0x06000520 RID: 1312 RVA: 0x00033554 File Offset: 0x00031754
		private void UpdateCraftingResult()
		{
			int x = int.MaxValue;
			for (int index = 0; index < m_craftingGridSize; index++)
			{
				for (int index2 = 0; index2 < m_craftingGridSize; index2++)
				{
					int index3 = index + index2 * 4;
					int slotIndex = index + index2 * m_craftingGridSize;
					int slotValue = GetSlotValue(slotIndex);
					int contents = Terrain.ExtractContents(slotValue);
					int data = Terrain.ExtractData(slotValue);
					int slotCount = GetSlotCount(slotIndex);
					if (slotCount > 0)
					{
						Block block = BlocksManager.Blocks[contents];
						m_matchedIngredients[index3] = block.CraftingId + ":" + data.ToString(CultureInfo.InvariantCulture);
						x = MathUtils.Min(x, slotCount);
					}
					else
					{
						m_matchedIngredients[index3] = null;
					}
				}
			}
			CraftingRecipe matchingRecipe = CraftingRecipesManager.FindMatchingRecipe(base.Project.FindSubsystem<SubsystemTerrain>(true), m_matchedIngredients, 0f);
			if (matchingRecipe != null)
			{
				m_matchedRecipe = matchingRecipe;
				m_slots[ResultSlotIndex].Value = matchingRecipe.ResultValue;
				m_slots[ResultSlotIndex].Count = matchingRecipe.ResultCount * x;
				return;
			}
			m_matchedRecipe = null;
			m_slots[ResultSlotIndex].Value = 0;
			m_slots[ResultSlotIndex].Count = 0;
		}
		
        private int m_craftingGridSize;
		
		private readonly string[] m_matchedIngredients = new string[16];
		
		private CraftingRecipe m_matchedRecipe;
	}
}
