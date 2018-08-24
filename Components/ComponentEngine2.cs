using Engine;
using GameEntitySystem;
using System.Globalization;
using TemplatesDatabase;

namespace Game
{
	public class ComponentEngine2 : ComponentMachine, IUpdateable
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

		public override int FuelSlotIndex
		{
			get
			{
				return SlotsCount - 2;
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
					//float heatLevel = HeatLevel;
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
			if (m_smeltingRecipe != null && m_fireTimeRemaining <= 0f)
			{
				Slot slot3 = m_slots[FuelSlotIndex];
				if (slot3.Count > 0)
				{
					var block = BlocksManager.Blocks[Terrain.ExtractContents(slot3.Value)];
					if (block.GetExplosionPressure(slot3.Value) > 0f)
					{
						slot3.Count = 0;
					}
					else if (block.FuelHeatLevel > 0f)
					{
						slot3.Count--;
						m_fireTimeRemaining = block.FuelFireDuration;
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
				//int num2 = m_music % 360;
				m_music += 2;
				if (SmeltingProgress >= 1f)
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

		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			base.Load(valuesDictionary, idToEntityMap);
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

		protected string FindSmeltingRecipe(float heatLevel)
		{
			string text = null;
			int remainsSlotIndex = RemainsSlotIndex;
			if (GetSlotCount(remainsSlotIndex) > 0)
			{
				int slotValue = base.GetSlotValue(remainsSlotIndex);
				var block = BlocksManager.Blocks[Terrain.ExtractContents(slotValue)].CraftingId;
				m_matchedIngredients[remainsSlotIndex] = block + ":" + Terrain.ExtractData(slotValue).ToString(CultureInfo.InvariantCulture);
				if (block == "waterbucket")
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
				//Terrain.ExtractContents(90);
				if (slot.Count != 0 && (90 != slot.Value || 1 + slot.Count > 40))
				{
					text = null;
				}
			}
			return text;
		}
	}
}
