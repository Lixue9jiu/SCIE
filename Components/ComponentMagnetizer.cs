using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	public class ComponentMagnetizer : ComponentMachine, IUpdateable
	{
		public bool Powered;

		protected string m_smeltingRecipe, m_smeltingRecipe2;

		//protected int m_music;

		public override int RemainsSlotIndex => -1;

		public override int ResultSlotIndex => SlotsCount - 1;

		public override int FuelSlotIndex => SlotsCount - 2;

		public int UpdateOrder => 0;

		public void Update(float dt)
		{
			Point3 coordinates = m_componentBlockEntity.Coordinates;
			if (HeatLevel > 0f)
			{
				m_fireTimeRemaining = MathUtils.Max(0f, m_fireTimeRemaining - dt);
				if (m_fireTimeRemaining == 0f)
					HeatLevel = 0f;
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
				m_smeltingRecipe2 = FindSmeltingRecipe(heatLevel);
				if (m_smeltingRecipe2 != m_smeltingRecipe)
				{
					m_smeltingRecipe = m_smeltingRecipe2;
					SmeltingProgress = 0f;
					//m_music = 0;
				}
			}
			if (m_smeltingRecipe2 != null)
			{
				if (!Powered)
				{
					SmeltingProgress = 0f;
					HeatLevel = 0f;
					m_smeltingRecipe = null;
				}
				else if (m_smeltingRecipe == null)
					m_smeltingRecipe = m_smeltingRecipe2;
			}
			if (!Powered)
			{
				SmeltingProgress = 0f;
				HeatLevel = 0f;
				m_smeltingRecipe = null;
			}
			if (m_smeltingRecipe == null)
			{
				HeatLevel = 0f;
				m_fireTimeRemaining = 0f;
				//m_music = -1;
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
						Utils.SubsystemExplosions.TryExplodeBlock(coordinates.X, coordinates.Y, coordinates.Z, slot.Value);
					}
					else if (block.FuelHeatLevel > 0f)
					{
						slot.Count--;
						if (block is IFuel fuel)
						{
							HeatLevel = fuel.GetHeatLevel(slot.Value);
							m_fireTimeRemaining = fuel.GetFuelFireDuration(slot.Value);
						}
						else
						{
							HeatLevel = block.FuelHeatLevel;
							m_fireTimeRemaining = block.FuelFireDuration;
						}
					}
				}
			}
			if (m_smeltingRecipe != null)
			{
				SmeltingProgress = MathUtils.Min(SmeltingProgress + 0.1f * dt, 1f);
				if (SmeltingProgress >= 1f)
				{
					m_slots[0].Count--;
					m_slots[ResultSlotIndex].Value = ItemBlock.IdTable[m_smeltingRecipe];
					m_slots[ResultSlotIndex].Count++;
					m_smeltingRecipe = null;
					SmeltingProgress = 0f;
					m_updateSmeltingRecipe = true;
				}
			}
		}

		public override int GetSlotCapacity(int slotIndex, int value)
		{
			if (slotIndex == FuelSlotIndex)
			{
				var block = BlocksManager.Blocks[Terrain.ExtractContents(value)];
				if ((block is IFuel fuel ? fuel.GetHeatLevel(value) : block.FuelHeatLevel) < 1400f)
					return 0;
			}
			return base.GetSlotCapacity(slotIndex, value);
		}

		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			Powered = false;
			base.Load(valuesDictionary, idToEntityMap);
			m_furnaceSize = SlotsCount - 1;
			m_fireTimeRemaining = valuesDictionary.GetValue("FireTimeRemaining", 0f);
			HeatLevel = valuesDictionary.GetValue("HeatLevel", 0f);
		}

		protected string FindSmeltingRecipe(float heatLevel)
		{
			if (heatLevel < 1500f)
				return null;
			string text = null;
			for (int i = 0; i < 1; i++)
			{
				if (GetSlotCount(FuelSlotIndex) > 0 && GetSlotValue(i) == ItemBlock.IdTable["SteelIngot"])
					text = "IndustrialMagnet";
			}
			if (text != null)
			{
				Slot slot = m_slots[ResultSlotIndex];
				if (slot.Count != 0 && (slot.Value != ItemBlock.IdTable[text] || slot.Count >= 40))
					text = null;
			}
			return text;
		}
	}
}