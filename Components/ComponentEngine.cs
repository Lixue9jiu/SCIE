using Engine;
using GameEntitySystem;
using System.Globalization;
using TemplatesDatabase;

namespace Game
{
	public class ComponentEngine : ComponentMachine, IUpdateable
	{
		protected float m_fireTimeRemaining;

		protected int m_furnaceSize;

		protected readonly string[] m_matchedIngredients = new string[9];

		protected string m_smeltingRecipe;

		protected SubsystemAudio m_subsystemAudio;

		protected int m_music;

		public int RemainsSlotIndex
		{
			get
			{
				return SlotsCount - 1;
			}
		}

		public override int ResultSlotIndex
		{
			get
			{
				return SlotsCount - 1;
			}
		}

		public override int FuelSlotIndex
		{
			get
			{
				return SlotsCount - 2;
			}
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
				float heatLevel = 0f;
				if (HeatLevel > 0f)
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
				string text = FindSmeltingRecipe(heatLevel);
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
					var block = BlocksManager.Blocks[Terrain.ExtractContents(slot2.Value)];
					if (block.GetExplosionPressure(slot2.Value) > 0f)
					{
						slot2.Count = 0;
						m_subsystemExplosions.TryExplodeBlock(coordinates.X, coordinates.Y, coordinates.Z, slot2.Value);
					}
					else if (block.FuelHeatLevel > 0f)
					{
						slot2.Count--;
						m_fireTimeRemaining = block is IFuel fuel ? fuel.GetFuelFireDuration(slot2.Value) : block.FuelFireDuration;
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
				SmeltingProgress = MathUtils.Min(SmeltingProgress + 0.02f * dt, 1f);
				if (m_music % 330 == 0)
				{
					m_subsystemAudio.PlaySound("Audio/SteamEngine", 1f, 0f, new Vector3(coordinates.X, coordinates.Y, coordinates.Z), 4f, true);
				}
				m_music += 2;
				if (SmeltingProgress >= 1.0)
				{
					for (int i = 0; i < m_furnaceSize; i++)
					{
						if (m_slots[i].Count > 0)
						{
							m_slots[i].Count--;
						}
					}
					m_slots[ResultSlotIndex].Value = 90;
					m_slots[ResultSlotIndex].Count++;
					m_smeltingRecipe = null;
					SmeltingProgress = 0f;
					m_updateSmeltingRecipe = true;
				}
			}
			TerrainChunk chunkAtCell = m_subsystemTerrain.Terrain.GetChunkAtCell(coordinates.X, coordinates.Z);
			if (chunkAtCell != null && chunkAtCell.State == TerrainChunkState.Valid)
			{
				int cellValue = m_subsystemTerrain.Terrain.GetCellValue(coordinates.X, coordinates.Y, coordinates.Z);
				m_subsystemTerrain.ChangeCell(coordinates.X, coordinates.Y, coordinates.Z, Terrain.ReplaceData(cellValue, FurnaceNBlock.SetHeatLevel(Terrain.ExtractData(cellValue), (HeatLevel > 0f) ? 1 : 0)), true);
			}
		}

		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			base.Load(valuesDictionary, idToEntityMap);
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

		protected string FindSmeltingRecipe(float heatLevel)
		{
			if (heatLevel < 100f)
			{
				return null;
			}
			string text = null;
			for (int i = 0; i < m_furnaceSize; i++)
			{
				int slotValue = GetSlotValue(i);
				int num = Terrain.ExtractContents(slotValue);
				int num2 = Terrain.ExtractData(slotValue);
				if (GetSlotCount(i) > 0)
				{
					var craftingId = BlocksManager.Blocks[num].CraftingId;
					m_matchedIngredients[i] = craftingId + ":" + num2.ToString(CultureInfo.InvariantCulture);
					if (craftingId == "waterbucket")
					{
						text = "bucket";
					}
				}
				else
				{
					m_matchedIngredients[i] = null;
				}
			}
			if (text != null)
			{
				Slot slot = m_slots[ResultSlotIndex];
				//Terrain.ExtractContents(90);
				if (slot.Count != 0 && (90 != slot.Value || 1 + slot.Count > 40))
				{
					text = null;
				}
			}
			return text;
		}
		public static bool IsPowered(Terrain terrain, int x, int y, int z)
		{
			int cellValue = terrain.GetCellValue(x + 1, y, z);
			if (FurnaceNBlock.GetHeatLevel(cellValue) != 0)
			{
				cellValue = Terrain.ExtractContents(cellValue);
				if (cellValue == EngineBlock.Index || cellValue == EngineHBlock.Index)
				{
					return true;
				}
			}
			cellValue = terrain.GetCellValue(x - 1, y, z);
			if (FurnaceNBlock.GetHeatLevel(cellValue) != 0)
			{
				cellValue = Terrain.ExtractContents(cellValue);
				if (cellValue == EngineBlock.Index || cellValue == EngineHBlock.Index)
				{
					return true;
				}
			}
			cellValue = terrain.GetCellValue(x, y + 1, z);
			if (FurnaceNBlock.GetHeatLevel(cellValue) != 0)
			{
				cellValue = Terrain.ExtractContents(cellValue);
				if (cellValue == EngineBlock.Index || cellValue == EngineHBlock.Index)
				{
					return true;
				}
			}
			cellValue = terrain.GetCellValue(x, y - 1, z);
			if (FurnaceNBlock.GetHeatLevel(cellValue) != 0)
			{
				cellValue = Terrain.ExtractContents(cellValue);
				if (cellValue == EngineBlock.Index || cellValue == EngineHBlock.Index)
				{
					return true;
				}
			}
			cellValue = terrain.GetCellValue(x, y, z + 1);
			if (FurnaceNBlock.GetHeatLevel(cellValue) != 0)
			{
				cellValue = Terrain.ExtractContents(cellValue);
				if (cellValue == EngineBlock.Index || cellValue == EngineHBlock.Index)
				{
					return true;
				}
			}
			cellValue = terrain.GetCellValue(x, y, z - 1);
			if (FurnaceNBlock.GetHeatLevel(cellValue) != 0)
			{
				cellValue = Terrain.ExtractContents(cellValue);
				if (cellValue == EngineBlock.Index || cellValue == EngineHBlock.Index)
				{
					return true;
				}
			}
			return false;
		}
	}
}
