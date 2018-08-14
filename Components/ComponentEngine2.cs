using Engine;
using GameEntitySystem;
using System.Globalization;
using TemplatesDatabase;

namespace Game
{
	public class ComponentEngine2 : ComponentInventoryBase, IUpdateable
	{
		private float m_fireTimeRemaining;

		private int m_furnaceSize;

		private readonly string[] m_matchedIngredients = new string[9];

		private SubsystemExplosions m_subsystemExplosions;

		private SubsystemTerrain m_subsystemTerrain;

		private bool m_updateSmeltingRecipe;

		private string m_smeltingRecipe;

		private SubsystemAudio m_subsystemAudio;

		private int m_music;

		public int RemainsSlotIndex
		{
			get
			{
				return SlotsCount - 3;
			}
		}

		public int ResultSlotIndex
		{
			get
			{
				return SlotsCount - 1;
			}
		}

		public int FuelSlotIndex
		{
			get
			{
				return SlotsCount - 2;
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
					Slot slot = m_slots[FuelSlotIndex];
					if (slot.Count > 0)
					{
						float fuelHeatLevel = BlocksManager.Blocks[Terrain.ExtractContents(slot.Value)].FuelHeatLevel;
					}
				}
				string text = null;
				int remainsSlotIndex = RemainsSlotIndex;
				int num = Terrain.ExtractContents(GetSlotValue(remainsSlotIndex));
				if (GetSlotCount(remainsSlotIndex) > 0 && BlocksManager.Blocks[num].CraftingId.ToString() == "waterbucket")
				{
					text = "bucket";
				}
				if (text != null)
				{
					Slot slot2 = m_slots[ResultSlotIndex];
					if (slot2.Count != 0 && (90 != slot2.Value || 1 + slot2.Count > 40))
					{
						text = null;
					}
				}
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
				Slot slot3 = m_slots[FuelSlotIndex];
				if (slot3.Count > 0)
				{
					Block block = BlocksManager.Blocks[Terrain.ExtractContents(slot3.Value)];
					if ((double)block.GetExplosionPressure(slot3.Value) > 0.0)
					{
						slot3.Count = 0;
					}
					else if ((double)block.FuelHeatLevel > 0.0)
					{
						slot3.Count--;
						m_fireTimeRemaining = block.FuelFireDuration;
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
				SmeltingProgress = MathUtils.Min(SmeltingProgress + 0.02f * dt, 1f);
				int num2 = m_music % 360;
				m_music += 2;
				if ((double)SmeltingProgress >= 1.0)
				{
					int remainsSlotIndex2 = RemainsSlotIndex;
					if (m_slots[remainsSlotIndex2].Count > 0)
					{
						m_slots[remainsSlotIndex2].Count--;
					}
					m_slots[ResultSlotIndex].Value = 90;
					m_slots[ResultSlotIndex].Count++;
					m_smeltingRecipe = null;
					SmeltingProgress = 0f;
					m_updateSmeltingRecipe = true;
				}
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
			m_subsystemAudio = Project.FindSubsystem<SubsystemAudio>(true);
			m_furnaceSize = SlotsCount - 3;
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

		private string FindSmeltingRecipe(float heatLevel)
		{
			string text = null;
			int remainsSlotIndex = RemainsSlotIndex;
			int slotValue = GetSlotValue(remainsSlotIndex);
			int num = Terrain.ExtractContents(slotValue);
			int num2 = Terrain.ExtractData(slotValue);
			if (GetSlotCount(remainsSlotIndex) > 0)
			{
				Block block = BlocksManager.Blocks[num];
				m_matchedIngredients[remainsSlotIndex] = block.CraftingId + ":" + num2.ToString(CultureInfo.InvariantCulture);
				if (block.CraftingId.ToString() == "waterbucket")
				{
					text = "bucket";
				}
			}
			else
			{
				m_matchedIngredients[remainsSlotIndex] = null;
			}
			if (text != null)
			{
				Slot slot = m_slots[ResultSlotIndex];
				Terrain.ExtractContents(90);
				if (slot.Count != 0 && (90 != slot.Value || 1 + slot.Count > 40))
				{
					text = null;
				}
			}
			return text;
		}
	}
}
