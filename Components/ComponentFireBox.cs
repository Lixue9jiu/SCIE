using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	public class ComponentFireBox : ComponentMachine, IUpdateable
	{
		public override int FuelSlotIndex
		{
			get
			{
				return SlotsCount - 1;
			}
		}

		public float HeatLevel;

		public float SmeltingProgress;

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
			if (HeatLevel > 0f)
			{
				m_fireTimeRemaining = MathUtils.Max(0f, m_fireTimeRemaining - dt);
				if (m_fireTimeRemaining == 0f)
				{
					HeatLevel = 0f;
				}
			}
			if (m_updateSmeltingRecipe)
			{
				m_updateSmeltingRecipe = false;
				if (HeatLevel > 0f)
				{
					float heatLevel = HeatLevel;
				}
				else
				{
					Slot slot = m_slots[FuelSlotIndex];
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
			if (m_smeltingRecipe != null && m_fireTimeRemaining <= 0f)
			{
				Slot slot2 = m_slots[FuelSlotIndex];
				if (slot2.Count > 0)
				{
					Block block = BlocksManager.Blocks[Terrain.ExtractContents(slot2.Value)];
					if (block.GetExplosionPressure(slot2.Value) > 0f)
					{
						slot2.Count = 0;
						m_subsystemExplosions.TryExplodeBlock(coordinates.X, coordinates.Y, coordinates.Z, slot2.Value);
					}
					else if (block.FuelHeatLevel > 0f)
					{
						slot2.Count--;
						m_fireTimeRemaining = block.FuelFireDuration;
						m_fireTime = block.FuelFireDuration;
						HeatLevel = block.FuelHeatLevel;
					}
				}
			}
			if (m_fireTimeRemaining <= 0f)
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
				m_subsystemTerrain.ChangeCell(coordinates.X, coordinates.Y, coordinates.Z, Terrain.ReplaceContents(cellValue, (HeatLevel > 0f) ? LitFireBoxBlock.Index : FireBoxBlock.Index), true);
			}
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
			valuesDictionary.SetValue("FireTimeRemaining", m_fireTimeRemaining);
			valuesDictionary.SetValue("HeatLevel", HeatLevel);
		}

		private float m_fireTimeRemaining;

		private int m_furnaceSize;

		private readonly string[] m_matchedIngredients = new string[9];

		private string m_smeltingRecipe;

		private SubsystemAudio m_subsystemAudio;

		private int m_music;

		private float m_fireTime;
	}
}
