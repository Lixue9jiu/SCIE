using Engine;
using GameEntitySystem;
using System;
using System.Globalization;
using TemplatesDatabase;

namespace Game
{
	public class ComponentFurnaceN : ComponentInventoryBase, IUpdateable
	{
		private ComponentBlockEntity m_componentBlockEntity;

		private float m_fireTimeRemaining;

		private int m_furnaceSize;

		private readonly string[] m_matchedIngredients = new string[9];

		private CraftingRecipe m_smeltingRecipe;

		private SubsystemExplosions m_subsystemExplosions;

		private SubsystemTerrain m_subsystemTerrain;

		private bool m_updateSmeltingRecipe;

		public int RemainsSlotIndex
		{
			get
			{
				return SlotsCount - 1;
			}
		}

		public int ResultSlotIndex
		{
			get
			{
				return SlotsCount - 2;
			}
		}

		public int FuelSlotIndex
		{
			get
			{
				return SlotsCount - 3;
			}
		}

		public float HeatLevel
		{
			get;
			private set;
		}

		public float SmeltingProgress
		{
			get;
			private set;
		}

		public int UpdateOrder
		{
			get
			{
				return 0;
			}
		}

		public void Update(float dt)
		{
			Point3 coordinates = m_componentBlockEntity.Coordinates;
			if ((double)HeatLevel > 0.0)
			{
				m_fireTimeRemaining = MathUtils.Max(0f, m_fireTimeRemaining - dt);
				if ((double)m_fireTimeRemaining == 0.0)
				{
					HeatLevel = 0f;
				}
			}
			if (m_updateSmeltingRecipe)
			{
				m_updateSmeltingRecipe = false;
				float heatLevel = 0f;
				if ((double)HeatLevel > 0.0)
				{
					heatLevel = HeatLevel;
				}
				else
				{
					Slot slot = m_slots[FuelSlotIndex];
					if (slot.Count > 0)
					{
						heatLevel = BlocksManager.Blocks[Terrain.ExtractContents(slot.Value)].FuelHeatLevel;
					}
				}
				CraftingRecipe craftingRecipe = FindSmeltingRecipe(heatLevel);
				if (craftingRecipe != m_smeltingRecipe)
				{
					m_smeltingRecipe = craftingRecipe;
					SmeltingProgress = 0f;
				}
			}
			if (m_smeltingRecipe == null)
			{
				HeatLevel = 0f;
				m_fireTimeRemaining = 0f;
			}
			if (m_smeltingRecipe != null && (double)m_fireTimeRemaining <= 0.0)
			{
				Slot slot2 = m_slots[FuelSlotIndex];
				if (slot2.Count > 0)
				{
					Block block = BlocksManager.Blocks[Terrain.ExtractContents(slot2.Value)];
					if ((double)block.GetExplosionPressure(slot2.Value) > 0.0)
					{
						slot2.Count = 0;
						m_subsystemExplosions.TryExplodeBlock(coordinates.X, coordinates.Y, coordinates.Z, slot2.Value);
					}
					else if ((double)block.FuelHeatLevel > 0.0)
					{
						slot2.Count--;
						m_fireTimeRemaining = block.FuelFireDuration;
						HeatLevel = block.FuelHeatLevel;
					}
				}
			}
			if ((double)m_fireTimeRemaining <= 0.0)
			{
				m_smeltingRecipe = null;
				SmeltingProgress = 0f;
			}
			if (m_smeltingRecipe != null)
			{
				SmeltingProgress = MathUtils.Min(SmeltingProgress + 0.15f * dt, 1f);
				if ((double)SmeltingProgress >= 1.0)
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
					SmeltingProgress = 0f;
					m_updateSmeltingRecipe = true;
				}
			}
			TerrainChunk chunkAtCell = m_subsystemTerrain.Terrain.GetChunkAtCell(coordinates.X, coordinates.Z);
			if (chunkAtCell != null && chunkAtCell.State == TerrainChunkState.Valid)
			{
				int cellValue = m_subsystemTerrain.Terrain.GetCellValue(coordinates.X, coordinates.Y, coordinates.Z);
				m_subsystemTerrain.ChangeCell(coordinates.X, coordinates.Y, coordinates.Z, Terrain.ReplaceContents(cellValue, ((double)HeatLevel > 0.0) ? 507 : 506), true);
			}
		}

		public override int GetSlotCapacity(int slotIndex, int value)
		{
			if (slotIndex != FuelSlotIndex)
			{
				return base.GetSlotCapacity(slotIndex, value);
			}
			if ((double)BlocksManager.Blocks[Terrain.ExtractContents(value)].FuelHeatLevel > 0.0)
			{
				return base.GetSlotCapacity(slotIndex, value);
			}
			return 0;
		}

		public override void AddSlotItems(int slotIndex, int value, int count)
		{
			base.AddSlotItems(slotIndex, value, count);
			m_updateSmeltingRecipe = true;
		}

		public override int RemoveSlotItems(int slotIndex, int count)
		{
			m_updateSmeltingRecipe = true;
			return base.RemoveSlotItems(slotIndex, count);
		}

		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			base.Load(valuesDictionary, idToEntityMap);
			m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			m_subsystemExplosions = base.Project.FindSubsystem<SubsystemExplosions>(true);
			m_componentBlockEntity = base.Entity.FindComponent<ComponentBlockEntity>(true);
			m_furnaceSize = SlotsCount - 3;
			if (m_furnaceSize < 1 || m_furnaceSize > 3)
			{
				throw new InvalidOperationException("Invalid furnace size.");
			}
			m_fireTimeRemaining = valuesDictionary.GetValue<float>("FireTimeRemaining");
			HeatLevel = valuesDictionary.GetValue<float>("HeatLevel");
			m_updateSmeltingRecipe = true;
		}

		public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
		{
			base.Save(valuesDictionary, entityToIdMap);
			valuesDictionary.SetValue("FireTimeRemaining", m_fireTimeRemaining);
			valuesDictionary.SetValue("HeatLevel", HeatLevel);
		}

		private CraftingRecipe FindSmeltingRecipe(float heatLevel)
		{
			if ((double)heatLevel <= 0.0)
			{
				return null;
			}
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
			CraftingRecipe craftingRecipe = CraftingRecipesManager.FindMatchingRecipe(m_subsystemTerrain, m_matchedIngredients, heatLevel);
			if (craftingRecipe != null)
			{
				if ((double)craftingRecipe.RequiredHeatLevel <= 0.0)
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
	}
}