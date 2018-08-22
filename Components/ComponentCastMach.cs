using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	public class ComponentCastMach : ComponentMachine, IUpdateable
	{
		private float m_fireTimeRemaining;

		private int m_furnaceSize;

		private readonly string[] m_matchedIngredients = new string[9];

		private string m_smeltingRecipe;

		private SubsystemAudio m_subsystemAudio;

		private int m_music;
		private float _heatLevel;
		private float _smeltingProgress;

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

		public float HeatLevel
		{
			get => _heatLevel;
			private set => _heatLevel = value;
		}

		public float SmeltingProgress
		{
			get => _smeltingProgress;
			private set => _smeltingProgress = value;
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
				if (SmeltingProgress >= 1f)
				{
					for (int i = 0; i < m_furnaceSize; i++)
					{
						if (m_slots[i].Count > 0)
						{
							m_slots[i].Count--;
						}
					}
					if (m_smeltingRecipe == "SteelWheel" || m_smeltingRecipe == "SteelGear")
					{
						m_slots[ResultSlotIndex].Value = ItemBlock.IdTable[m_smeltingRecipe];
						m_slots[ResultSlotIndex].Count++;
					}
					m_smeltingRecipe = null;
					SmeltingProgress = 0f;
					m_updateSmeltingRecipe = true;
				}
			}

		}

		public override int GetSlotCapacity(int slotIndex, int value)
		{
			if (slotIndex != FuelSlotIndex || BlocksManager.Blocks[Terrain.ExtractContents(value)].FuelHeatLevel > 1f)
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
			for (int i = 0; i < m_furnaceSize; i++)
			{
				int slotValue = GetSlotValue(i);
				int num = Terrain.ExtractContents(slotValue);
				int num2 = Terrain.ExtractData(slotValue);
				if (GetSlotCount(i) > 0 && GetSlotValue(1) > 0)
				{
					int slotvalue = GetSlotValue(1);
					if (slotValue == ItemBlock.IdTable["SteelIngot"] && slotvalue == ItemBlock.IdTable["SteelGearMould"])
					{
						text = "SteelGear";
					}
					if (slotValue == ItemBlock.IdTable["SteelIngot"] && slotvalue == ItemBlock.IdTable["SteelWheelMould"])
					{
						text = "SteelWheel";
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
				Block block3 = BlocksManager.Blocks[Terrain.ExtractContents(slot.Value)];
				if (slot.Count != 0 && (block3.CraftingId != text || 1 + slot.Count > 40))
				{
					text = null;
				}
			}
			return text;
		}
	}
}
