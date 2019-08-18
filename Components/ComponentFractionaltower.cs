using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	public class ComponentFractionalTower : ComponentMachine, IUpdateable
	{
		protected int m_time;
		protected readonly int[] result = new int[3];

		protected string m_smeltingRecipe, m_smeltingRecipe2;

		//protected int m_music;
		public bool Powered;

		public int valuein;
		public int value;

		public override int RemainsSlotIndex => -1;

		public override int ResultSlotIndex => SlotsCount - 1;

		public override int FuelSlotIndex => -1;

		public float Heatlevel;

		public void Update(float dt)
		{
			Point3 coordinates = m_componentBlockEntity.Coordinates;

			m_fireTimeRemaining = MathUtils.Max(0f, m_fireTimeRemaining - dt);

			if (m_updateSmeltingRecipe)
			{
				m_updateSmeltingRecipe = false;
				m_smeltingRecipe2 = FindSmeltingRecipe();
				if (m_smeltingRecipe2 != m_smeltingRecipe)
				{
					m_smeltingRecipe = m_smeltingRecipe2;
					SmeltingProgress = 0f;
					m_time = 0;
					//m_music = 0;
				}
			}
			if (m_smeltingRecipe2 != null && Utils.SubsystemTime.PeriodicGameTimeEvent(0.2, 0.0))
			{
				int num = 1, num2 = 0;
				int num3 = coordinates.X;
				int num4 = coordinates.Y;
				int num5 = coordinates.Z;
				for (int i = -1; i < 2; i++)
				{
					for (int j = 0; j < 4; j++)
					{
						for (int k = -1; k < 2; k++)
						{
							int cellContents = Utils.Terrain.GetCellContents(num3 + i, num4 + j, num5 + k);
							if (j != 0 && cellContents != MetalBlock.Index)
							{
								num = 0;
								break;
							}
							if (j == 0 && i * i + k * k > 1 && cellContents != MetalBlock.Index)
							{
								num = 0;
								break;
							}
						}
					}
				}
				int cellValue = Utils.Terrain.GetCellValue(num3, num4 - 1, num5);
				if ((Terrain.ExtractContents(cellValue) != FireBoxBlock.Index) || FurnaceNBlock.GetHeatLevel(cellValue) == 0)
					num = 0;
				if (num == 0 || num2 == 0)
					m_smeltingRecipe = null;
				if (num == 1 && m_smeltingRecipe == null)
					m_smeltingRecipe = m_smeltingRecipe2;
			}
			int flag2222 = 0;
			if (Utils.SubsystemTime.PeriodicGameTimeEvent(2.0, 0.0))
			{
				int cellValue2 = Utils.Terrain.GetCellValueFast(coordinates.X + 1, coordinates.Y, coordinates.Z);
				if (ElementBlock.Block.GetDevice(coordinates.X + 1, coordinates.Y, coordinates.Z, cellValue2) is ElectricPump)
				{
					flag2222 = 1;
				}
				int cellValue3 = Utils.Terrain.GetCellValueFast(coordinates.X - 1, coordinates.Y, coordinates.Z);
				if (ElementBlock.Block.GetDevice(coordinates.X - 1, coordinates.Y, coordinates.Z, cellValue3) is ElectricPump)
				{
					flag2222 = 2;
				}
				int cellValue4 = Utils.Terrain.GetCellValueFast(coordinates.X, coordinates.Y, coordinates.Z + 1);
				if (ElementBlock.Block.GetDevice(coordinates.X, coordinates.Y, coordinates.Z + 1, cellValue4) is ElectricPump)
				{
					flag2222 = 3;
				}
				//	int cellValue5 = Utils.Terrain.GetCellValueFast(coordinates.X, coordinates.Y, coordinates.Z-1);
				//	if (ElementBlock.Block.GetDevice(coordinates.X, coordinates.Y, coordinates.Z - 1, cellValue5) is ElectricPump)
				//	{
				//		flag2222 = 4;
				//	}
			}
			if (flag2222 > 0 && valuein < 10000 && Powered && HeatLevel > 0)
			{
				int j = (int)HeatLevel - 1;
				if (m_slots[1 + j].Value != 0 && m_slots[1 + j].Count > 0)
				{
					value = m_slots[1 + j].Value << 10;
					m_slots[1 + j].Count--;
					flag2222 = 0;
				}
				else
				{
					HeatLevel = 0;
				}
			}
			if (m_smeltingRecipe == null)
			{
				//HeatLevel = 0f;
				m_fireTimeRemaining = 0f;
				//m_music = -1;
			}
			else
				m_fireTimeRemaining = 100f;
			if (m_fireTimeRemaining <= 0f)
			{
				m_smeltingRecipe = null;
				SmeltingProgress = 0f;
				m_time = 0;
				//m_music = -1;
			}
			if (m_smeltingRecipe != null)
			{
				SmeltingProgress = MathUtils.Min(SmeltingProgress + 0.1f * dt, 1f);
				if (SmeltingProgress >= 1f)
				{
					if (m_slots[0].Count > 0)
						m_slots[0].Count--;
					for (int j = 0; j < 3; j++)
					{
						if (result[j] != 0)
						{
							int value = result[j];
							m_slots[1 + j].Value = value;
							m_slots[1 + j].Count++;
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
			HeatLevel = valuesDictionary.GetValue("HeatLevel", 0f);
		}

		public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
		{
			this.SaveItems(valuesDictionary);
			if (HeatLevel > 0f)
				valuesDictionary.SetValue("HeatLevel", HeatLevel);
		}

		protected string FindSmeltingRecipe()
		{
			bool text = false;
			result[0] = 0;
			result[1] = 0;
			result[2] = 0;
			int i;
			for (i = 0; i < m_furnaceSize; i++)
			{
				if (GetSlotCount(i) <= 0) continue;
				if (GetSlotValue(i) == 262384)
				{
					text = true;
					result[0] = 786672;
					result[1] = 1048816;
					result[2] = 1310960;
				}
			}
			if (!text)
				return null;
			for (i = 0; i < 3; i++)
			{
				Slot slot = m_slots[1 + i];
				if (slot.Count != 0 && result[i] != 0 && (slot.Value != result[i] || slot.Count >= 40))
					return null;
			}
			return "AluminumOrePowder";
		}
	}
}