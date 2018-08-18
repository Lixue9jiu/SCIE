using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	public class ComponentFireBox : ComponentInventoryBase, IUpdateable
	{
		public int FuelSlotIndex
		{
			get
			{
				return SlotsCount - 1;
			}
		}

		public float HeatLevel { get; private set; }

		public float SmeltingProgress { get; private set; }

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
				if ((double)HeatLevel > 0.0)
				{
					float heatLevel = HeatLevel;
				}
				else
				{
					ComponentInventoryBase.Slot slot = m_slots[FuelSlotIndex];
					if (slot.Count > 0)
					{
						float fuelHeatLevel = BlocksManager.Blocks[Terrain.ExtractContents(slot.Value)].FuelHeatLevel;
					}
				}
				string text = "text";
				if (text != m_smeltingRecipe)
				{
					m_smeltingRecipe = text;
					SmeltingProgress = 0f;
					m_music = 0;
				}
			}
			if (m_smeltingRecipe == null)
			{
				HeatLevel = 0f;
				m_fireTimeRemaining = 0f;
				m_music = -1;
			}
			if (m_smeltingRecipe != null && (double)m_fireTimeRemaining <= 0.0)
			{
				ComponentInventoryBase.Slot slot2 = m_slots[FuelSlotIndex];
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
						m_fireTime = block.FuelFireDuration;
						HeatLevel = block.FuelHeatLevel;
					}
				}
			}
			if ((double)m_fireTimeRemaining <= 0.0)
			{
				m_smeltingRecipe = null;
				SmeltingProgress = 0f;
				m_music = -1;
			}
			if (m_smeltingRecipe != null)
			{
				if (m_fireTime == 0f)
				{
					m_fireTime = m_fireTimeRemaining;
				}
				SmeltingProgress = MathUtils.Min(m_fireTimeRemaining / m_fireTime, 1f);
				if (SmeltingProgress >= 2f)
				{
					m_smeltingRecipe = null;
					SmeltingProgress = 0f;
					m_updateSmeltingRecipe = true;
				}
			}
			if (m_subsystemTerrain.Terrain.GetCellContents(coordinates.X, coordinates.Y, coordinates.Z) != 0)
			{
				int cellValue = m_subsystemTerrain.Terrain.GetCellValue(coordinates.X, coordinates.Y, coordinates.Z);
				m_subsystemTerrain.ChangeCell(coordinates.X, coordinates.Y, coordinates.Z, Terrain.ReplaceContents(cellValue, ((double)HeatLevel > 0.0) ? 544 : 543), true);
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
			m_subsystemTerrain = Project.FindSubsystem<SubsystemTerrain>(true);
			m_subsystemExplosions = Project.FindSubsystem<SubsystemExplosions>(true);
			m_componentBlockEntity = Entity.FindComponent<ComponentBlockEntity>(true);
			m_subsystemAudio = Project.FindSubsystem<SubsystemAudio>(true);
			m_furnaceSize = SlotsCount - 2;
			m_fireTimeRemaining = valuesDictionary.GetValue<float>("FireTimeRemaining");
			HeatLevel = valuesDictionary.GetValue<float>("HeatLevel");
			m_updateSmeltingRecipe = true;
		}

		public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
		{
			base.Save(valuesDictionary, entityToIdMap);
			valuesDictionary.SetValue<float>("FireTimeRemaining", m_fireTimeRemaining);
			valuesDictionary.SetValue<float>("HeatLevel", HeatLevel);
		}

		private ComponentBlockEntity m_componentBlockEntity;

		private float m_fireTimeRemaining;

		private int m_furnaceSize;

		private readonly string[] m_matchedIngredients = new string[9];

		private SubsystemExplosions m_subsystemExplosions;

		private SubsystemTerrain m_subsystemTerrain;

		private bool m_updateSmeltingRecipe;

		private string m_smeltingRecipe;

		private SubsystemAudio m_subsystemAudio;

		private int m_music;

		private float m_fireTime;
	}
}
