using GameEntitySystem;
using System.Globalization;
using TemplatesDatabase;
using System;
namespace Game
{
	public class ComponentElectricIFurnace : ComponentElectricFurnace, IUpdateable, ICraftingMachine
	{
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			base.Load(valuesDictionary, idToEntityMap);
			m_speed = 0.3f;
		}

		public override CraftingRecipe FindSmeltingRecipe2(float heatLevel)
		{
			heatLevel *= 1.5f;
			if (heatLevel >= 0f)
			{
				for (int i = 0; i < m_furnaceSize; i++)
				{
					int slotValue = GetSlotValue(i);
					int num = Terrain.ExtractContents(slotValue);
					int num2 = Terrain.ExtractData(slotValue);
					if (GetSlotCount(i) > 0)
					{
						Block block = BlocksManager.Blocks[num];
						m_matchedIngredients[i] = block.CraftingId + ":" + num2.ToString(CultureInfo.InvariantCulture);
					}
					else
					{
						m_matchedIngredients[i] = null;
					}
				}
				CraftingRecipe craftingRecipe = FindMatchingRecipe3(m_subsystemTerrain, m_matchedIngredients, 2800f);
				if (craftingRecipe != null)
				{
					if (craftingRecipe.RequiredHeatLevel != 2800f)
					{
						craftingRecipe = null;
					}
					if (craftingRecipe != null)
					{
						Slot slot = m_slots[ResultSlotIndex];
						int num3 = Terrain.ExtractContents(craftingRecipe.ResultValue);
						if (slot.Count != 0 && (craftingRecipe.ResultValue != slot.Value || craftingRecipe.ResultCount + slot.Count > BlocksManager.Blocks[num3].MaxStacking))
						{
							craftingRecipe = null;
						}
					}
					if (craftingRecipe != null && craftingRecipe.RemainsValue != 0 && craftingRecipe.RemainsCount > 0)
					{
						if (m_slots[RemainsSlotIndex].Count == 0 || m_slots[RemainsSlotIndex].Value == craftingRecipe.RemainsValue)
						{
							if (BlocksManager.Blocks[Terrain.ExtractContents(craftingRecipe.RemainsValue)].MaxStacking - m_slots[RemainsSlotIndex].Count < craftingRecipe.RemainsCount)
							{
								craftingRecipe = null;
							}
						}
						else
						{
							craftingRecipe = null;
						}
					}
				}
				return craftingRecipe;
			}
			return null;
		}


		public static CraftingRecipe FindMatchingRecipe3(SubsystemTerrain terrain, string[] ingredients, float heatLevel)
		{
			Func<SubsystemTerrain, string[], float, CraftingRecipe> findMatchingRecipe = CraftingRecipesManager.FindMatchingRecipe1;
			if (findMatchingRecipe != null)
			{
				return findMatchingRecipe(terrain, ingredients, heatLevel);
			}
			Block[] blocks = BlocksManager.Blocks;
			for (int i = 0; i < blocks.Length; i++)
			{
				CraftingRecipe adHocCraftingRecipe = blocks[i].GetAdHocCraftingRecipe(terrain, ingredients, heatLevel);
				if (adHocCraftingRecipe == null)
					continue;
				if (adHocCraftingRecipe != null && adHocCraftingRecipe.RequiredHeatLevel > 0 && heatLevel == adHocCraftingRecipe.RequiredHeatLevel && CraftingRecipesManager.MatchRecipe(adHocCraftingRecipe.Ingredients, ingredients))
				{
					return adHocCraftingRecipe;
				}
			}
			int count = CraftingRecipesManager.Recipes.Count;
			for (int i = 0; i < count; i++)
			{
				CraftingRecipe adHocCraftingRecipe = CraftingRecipesManager.Recipes[i];
				if (adHocCraftingRecipe == null)
					continue;
				if (heatLevel == adHocCraftingRecipe.RequiredHeatLevel && adHocCraftingRecipe.RequiredHeatLevel > 0 && CraftingRecipesManager.MatchRecipe(adHocCraftingRecipe.Ingredients, ingredients))
				{
					return adHocCraftingRecipe;
				}
			}
			return null;
		}



	}
}