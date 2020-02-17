using Engine;
using GameEntitySystem;
using TemplatesDatabase;
using System.Globalization;
using System;
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
				return;
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
				for (int i = 0; i < 4; i++)
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
				CraftingRecipe craftingRecipe = FindMatchingRecipe1(m_subsystemTerrain, m_matchedIngredients, 2000f);
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
						if (slot.Count != 0 && (craftingRecipe.ResultValue != slot.Value))
						{
							craftingRecipe = null;
						}
					}
					if (craftingRecipe != null && craftingRecipe.RemainsValue != 0 && craftingRecipe.RemainsCount > 0)
					{
						if (m_slots[RemainsSlotIndex].Count == 0 || m_slots[RemainsSlotIndex].Value == craftingRecipe.RemainsValue)
						{
							
						}
						else
						{
							craftingRecipe = null;
						}
					}
				}
				return craftingRecipe;
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
				if (adHocCraftingRecipe != null && adHocCraftingRecipe.RequiredHeatLevel > 0 && heatLevel >= adHocCraftingRecipe.RequiredHeatLevel && CraftingRecipesManager.MatchRecipe(adHocCraftingRecipe.Ingredients, ingredients))
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
				if (heatLevel >= adHocCraftingRecipe.RequiredHeatLevel && adHocCraftingRecipe.RequiredHeatLevel > 0 && CraftingRecipesManager.MatchRecipe(adHocCraftingRecipe.Ingredients, ingredients))
				{
					return adHocCraftingRecipe;
				}
			}
			return null;
		}


		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			m_matchedIngredients = new string[36];
			this.LoadItems(valuesDictionary);
			m_componentBlockEntity = Entity.FindComponent<ComponentBlockEntity>(true);
			m_fireTimeRemaining = valuesDictionary.GetValue("FireTimeRemaining", 0f);
			m_furnaceSize = SlotsCount - 4;
			m_updateSmeltingRecipe = true;
			m_speed = 0.2f;
		}

		public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
		{
			this.SaveItems(valuesDictionary);
			valuesDictionary.SetValue("FireTimeRemaining", m_fireTimeRemaining);
		}
	}

	public class ComponentElectricHFurnace : ComponentElectricFurnace, IUpdateable, IElectricMachine
	{
		public override CraftingRecipe FindSmeltingRecipe2(float heatlevel)
		{
			for (int i = 0; i < 6; i++)
			{
				int n = 0;
				if (i >= 3)
					n = 3;
				int slotValue = GetSlotValue(i);
				int num = Terrain.ExtractContents(slotValue);
				int num2 = Terrain.ExtractData(slotValue);
				if (GetSlotCount(i) > 0)
				{
					Block block = BlocksManager.Blocks[num];
					m_matchedIngredients[i+n] = block.CraftingId + ":" + num2.ToString(CultureInfo.InvariantCulture);
				}
				else
				{
					m_matchedIngredients[i+n] = null;
				}
			}
			m_matchedIngredients[3] = null;
			m_matchedIngredients[4] = null;
			m_matchedIngredients[5] = null;
			CraftingRecipe craftingRecipe = FindMatchingRecipe1(m_subsystemTerrain, m_matchedIngredients, 3000f);
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
					if (slot.Count != 0 && (craftingRecipe.ResultValue != slot.Value))
					{
						craftingRecipe = null;
					}
				}
				if (craftingRecipe != null && craftingRecipe.RemainsValue != 0 && craftingRecipe.RemainsCount > 0)
				{
					if (m_slots[RemainsSlotIndex].Count == 0 || m_slots[RemainsSlotIndex].Value == craftingRecipe.RemainsValue)
					{

					}
					else
					{
						craftingRecipe = null;
					}
				}
			}
			return craftingRecipe;
		}

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
				CraftingRecipe craftingRecipe = FindSmeltingRecipe2(3000f);

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
				return;
			}



			if (m_smeltingRecipe != null && Utils.SubsystemTime.PeriodicGameTimeEvent(0.2, 0.0))
			{
				Point3 coordinates = m_componentBlockEntity.Coordinates;
				int num = 1, num2 = 0;
				var point = CellFace.FaceToPoint3((Terrain.ExtractData(Utils.Terrain.GetCellValue(coordinates.X, coordinates.Y, coordinates.Z)) >> 15));
				int num3 = coordinates.X - point.X;
				int num4 = coordinates.Y - point.Y;
				int num5 = coordinates.Z - point.Z, v;
				for (int i = -1; i < 2; i++)
				{
					for (int j = -1; j < 2; j++)
					{
						for (int k = -1; k < 2; k++)
						{
							int cellValue = Terrain.ReplaceLight(Utils.Terrain.GetCellValue(num3 + i, num4 + j, num5 + k), 0);
							int cellContents = Terrain.ExtractContents(cellValue);
							if (j == 0 && !(ElementBlock.Block.GetDevice(num3 + i, num4 + j, num5 + k,cellValue) is WireBlock2) && !(i==0 && k==0) && !(num3 + i == coordinates.X && num5 + k == coordinates.Z))
							{
								num = 0;
								break;
							}
							if (j==-1 && cellValue != (MetalBlock.Index | 96 << 14))
							{
								num = 0;
								break;
							}
							if (j == 1 && cellValue != (MetalBlock.Index | 96 << 14))
							{
								if (ElementBlock.Block.GetDevice(num3 + i, num4 + j, num5 + k, cellValue) is AirBlower)
								{
									num2 ++;
									break;
								}
								else
								{
									num = 0;
									break;
								}
								
							}

						}
					}
				}
				if (num == 0 || num2 <2)
				{
					m_smeltingRecipe = null;
					m_smeltingProgress = 0f;
					m_heatLevel = 0f;
					m_smeltingRecipe = null;
					return;
				}

			}
		    if (m_smeltingRecipe == null)
			{
				m_heatLevel = 0f;
				m_fireTimeRemaining = 0f;
			}
			if (m_smeltingRecipe != null && m_fireTimeRemaining <= 0f)
			{
				m_heatLevel = 3000f;
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
	}
}