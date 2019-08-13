using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	public class ComponentOilPlant : ComponentMachine, IUpdateable
	{
		protected string m_smeltingRecipe;

		//protected int m_music;
		public bool Powered;

		protected string m_smeltingRecipe2;

		protected readonly int[] result = new int[3];

		public override int RemainsSlotIndex => -1;

		public override int ResultSlotIndex => SlotsCount - 1;

		public override int FuelSlotIndex => -1;

		public int UpdateOrder => 0;

		public void Update(float dt)
		{
			if (HeatLevel > 0f)
			{
				m_fireTimeRemaining = MathUtils.Max(0f, m_fireTimeRemaining - dt);
				if (m_fireTimeRemaining == 0f)
					HeatLevel = 0f;
			}
			if (m_updateSmeltingRecipe)
			{
				m_updateSmeltingRecipe = false;
				m_smeltingRecipe2 = FindSmeltingRecipe(0f);
				if (m_smeltingRecipe2 != m_smeltingRecipe)
				{
					m_smeltingRecipe = m_smeltingRecipe2;
					SmeltingProgress = 0f;
					//m_music = 0;
				}
			}
			int i;
			if (m_smeltingRecipe2 != null)
			{
				int num = 0;
				for (i = -1; i < 2; i++)
				{
					for (int j = -1; j < 2; j++)
					{
						for (int k = -1; k < 2; k++)
						{
							Point3 coordinates = m_componentBlockEntity.Coordinates;
							int cellValue = Utils.Terrain.GetCellValue(coordinates.X + i, coordinates.Y + j, coordinates.Z + k);
							if (i * i + j * j + k * k <= 1 && (Terrain.ExtractContents(cellValue) == FireBoxBlock.Index) && FurnaceNBlock.GetHeatLevel(cellValue) != 0)
							{
								num = 1;
								break;
							}
						}
					}
				}
				if (num == 0)
					m_smeltingRecipe = null;
				if (num == 1 && m_smeltingRecipe == null)
					m_smeltingRecipe = m_smeltingRecipe2;
			}
			if (m_smeltingRecipe == null)
			{
				HeatLevel = 0f;
				m_fireTimeRemaining = 0f;
				//m_music = -1;
			}
			else
				m_fireTimeRemaining = 100f;
			if (m_fireTimeRemaining <= 0f)
			{
				m_smeltingRecipe = null;
				SmeltingProgress = 0f;
				//m_music = -1;
			}
			if (m_smeltingRecipe != null)
			{
				SmeltingProgress = MathUtils.Min(SmeltingProgress + 0.1f * dt, 1f);
				if (SmeltingProgress >= 1f)
				{
					for (i = 0; i < 3; i++)
						if (m_slots[i].Count > 0)
							m_slots[i].Count--;
					for (int j = 0; j < 3; j++)
					{
						if (result[j] != 0)
						{
							m_slots[3 + j].Value = result[j];
							m_slots[3 + j].Count++;
							m_smeltingRecipe = null;
							SmeltingProgress = 0f;
							m_updateSmeltingRecipe = true;
						}
					}
				}
			}
		}

		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			base.Load(valuesDictionary, idToEntityMap);
			m_furnaceSize = SlotsCount - 1;
			m_fireTimeRemaining = valuesDictionary.GetValue("FireTimeRemaining", 0f);
			HeatLevel = valuesDictionary.GetValue("HeatLevel", 0f);
		}

		protected string FindSmeltingRecipe(float heatLevel)
		{
			string text = null;
			
			
				
			if (GetSlotValue(0) == (1310960) && GetSlotCount(0) > 0)
			{
				if (GetSlotValue(1) == (ItemBlock.IdTable["CokeCoalPowder"]) && GetSlotCount(1) > 0)
				{
					text = "RefinedOil";
					result[0] = RottenMeatBlock.Index | 2 << 12 << 14;
					result[1] = 0;
					result[2] = 0;
				}
			}
			if (GetSlotValue(0) == (240+16384*60) && GetSlotCount(0) > 0)
			{
				if (GetSlotValue(1) == (ItemBlock.IdTable["CokeCoalPowder"]) && GetSlotCount(1) > 0)
				{
					if (GetSlotValue(2) == (WaterBlock.Index) && GetSlotCount(2) > 0)
					{
						text = "RefinedOil2";
						result[0] = RottenMeatBlock.Index | 2 << 7 << 14;
						result[1] = 0;
						result[2] = 0;
					}
				}
			}
			if (GetSlotValue(0) == (240 + 16384 * 70) && GetSlotCount(0) > 0)
			{
				if (GetSlotValue(1) == (ItemBlock.IdTable["CokeCoalPowder"]) && GetSlotCount(1) > 0)
				{
					if (GetSlotValue(2) == (WaterBlock.Index) && GetSlotCount(2) > 0)
					{
						result[0] = RottenMeatBlock.Index | 2 << 5 << 14;
						result[1] = 0;
						result[2] = 0;
						text = "RefinedOil3";
					}
				}
			}
			if (text!=null)
			{
				for (int i = 0; i < 3; i++)
				{
					Slot slot = m_slots[3 + i];
					if (slot.Count != 0 && result[i] != 0 && (slot.Value != result[i] || slot.Count >= 40))
						return null;
				}
			}
			return text;
		}
	}
}