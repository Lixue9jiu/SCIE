using Engine;
using System;
using System.Globalization;
namespace Game
{
	public class ComponentFurnaceN : ComponentFurnace, IUpdateable, ICraftingMachine
	{
		public ComponentFurnaceN()
		{
			m_matchedIngredients = new string[36];
		}

		public new int RemainsSlotIndex => SlotsCount - 1;

		public new int ResultSlotIndex => SlotsCount - 2;

		public new int FuelSlotIndex => SlotsCount - 3;

		public int SlotIndex { get => throw new NotSupportedException(); set => throw new NotImplementedException(); }

		public CraftingRecipe GetRecipe() => throw new NotImplementedException();

		public new void Update(float dt)
		{
			Point3 coordinates = m_componentBlockEntity.Coordinates;
			if (coordinates.Y < 0 || coordinates.Y > 127)
				return;
			if (HeatLevel > 0f)
			{
				m_fireTimeRemaining = MathUtils.Max(0f, m_fireTimeRemaining - dt);
				if (m_fireTimeRemaining == 0f)
					m_heatLevel = 0f;
			}
			Slot slot;
			if (m_updateSmeltingRecipe)
			{
				m_updateSmeltingRecipe = false;
				float heatLevel = 0f;
				if (HeatLevel > 0f)
					heatLevel = HeatLevel;
				else
				{
					slot = m_slots[FuelSlotIndex];
					if (slot.Count > 0)
					{
						var block = BlocksManager.Blocks[Terrain.ExtractContents(slot.Value)];
						heatLevel = block is IFuel fuel ? fuel.GetHeatLevel(slot.Value) : block.FuelHeatLevel;
					}
				}
				CraftingRecipe craftingRecipe = FindSmeltingRecipe2(heatLevel);
				if (craftingRecipe != m_smeltingRecipe)
				{
					m_smeltingRecipe = craftingRecipe;
					m_smeltingProgress = 0f;
				}
			}
			if (m_smeltingRecipe == null)
			{
				m_heatLevel = 0f;
				m_fireTimeRemaining = 0f;
			}
			if (m_smeltingRecipe != null && m_fireTimeRemaining <= 0f)
			{
				slot = m_slots[FuelSlotIndex];
				if (slot.Count > 0)
				{
					var block = BlocksManager.Blocks[Terrain.ExtractContents(slot.Value)];
					if (block.GetExplosionPressure(slot.Value) > 0f)
					{
						slot.Count = 0;
						m_subsystemExplosions.TryExplodeBlock(coordinates.X, coordinates.Y, coordinates.Z, slot.Value);
					}
					else
					{
						slot.Count--;
						if (block is IFuel fuel)
						{
							m_heatLevel = fuel.GetHeatLevel(slot.Value);
							m_fireTimeRemaining = fuel.GetFuelFireDuration(slot.Value);
						}
						else
						{
							m_heatLevel = block.FuelHeatLevel;
							m_fireTimeRemaining = block.FuelFireDuration;
						}
					}
				}
			}
			if (m_fireTimeRemaining <= 0f)
			{
				m_smeltingRecipe = null;
				m_smeltingProgress = 0f;
			}
			if (m_smeltingRecipe != null)
			{
				m_smeltingProgress = MathUtils.Min(m_smeltingProgress + 0.15f * dt, 1f);
				if (m_smeltingProgress >= 1f)
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
					//else if (m_smeltingRecipe.Ingredients[0] == m_smeltingRecipe.Ingredients[2] && m_smeltingRecipe.Ingredients[2].Contains("item") && m_smeltingRecipe.Ingredients[0] != null && m_smeltingRecipe.Ingredients[0].!= m_smeltingRecipe.ResultValue)
					//{
					//	m_slots[RemainsSlotIndex].Value = ItemBlock.IdTable["Slag"];
					//	m_slots[RemainsSlotIndex].Count += m_smeltingRecipe.ResultCount >> 1;
					//}
					//Utils.Random.UniformFloat(1f, f);
					m_smeltingRecipe = null;
					m_smeltingProgress = 0f;
					m_updateSmeltingRecipe = true;
				}
			}
			TerrainChunk chunk = m_subsystemTerrain.Terrain.GetChunkAtCell(coordinates.X, coordinates.Z);
			if (chunk != null && chunk.State == TerrainChunkState.Valid)
			{
				int cellValue = chunk.GetCellValueFast(coordinates.X & 15, coordinates.Y, coordinates.Z & 15);
				m_subsystemTerrain.ChangeCell(coordinates.X, coordinates.Y, coordinates.Z, (Terrain.ExtractContents(cellValue) >> 1) == 32 ? Terrain.ReplaceContents(cellValue, (m_heatLevel > 0f) ? 65 : 64) : Terrain.ReplaceData(cellValue, FurnaceNBlock.SetHeatLevel(Terrain.ExtractData(cellValue), HeatLevel > 0f ? 1 : 0)));
			}
		}


		public CraftingRecipe FindSmeltingRecipe2(float heatLevel)
		{
			if (heatLevel > 0f)
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
				CraftingRecipe craftingRecipe = FindMatchingRecipe1(m_subsystemTerrain, m_matchedIngredients, heatLevel);
				if (craftingRecipe != null)
				{
					if (craftingRecipe.RequiredHeatLevel <= 0f)
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


		public static CraftingRecipe FindMatchingRecipe1(SubsystemTerrain terrain, string[] ingredients, float heatLevel)
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
				if (adHocCraftingRecipe != null && adHocCraftingRecipe.RequiredHeatLevel > 0  && heatLevel >= adHocCraftingRecipe.RequiredHeatLevel && CraftingRecipesManager.MatchRecipe(adHocCraftingRecipe.Ingredients, ingredients))
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
				if (heatLevel >= adHocCraftingRecipe.RequiredHeatLevel && adHocCraftingRecipe.RequiredHeatLevel > 0  && CraftingRecipesManager.MatchRecipe(adHocCraftingRecipe.Ingredients, ingredients))
				{
					return adHocCraftingRecipe;
				}
			}
			return null;
		}

	}

}