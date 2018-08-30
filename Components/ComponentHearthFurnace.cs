using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	public class ComponentHearthFurnace : ComponentMachine, IUpdateable
	{
		protected SubsystemTime m_subsystemTime;
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
			if (m_updateSmeltingRecipe)
			{
				m_updateSmeltingRecipe = false;
				result[0] = m_matchedIngredients[7];
				result[1] = m_matchedIngredients[8];
				result[2] = m_matchedIngredients[9];
				bool flag = FindSmeltingRecipe(5f);
				if (result[0] != m_matchedIngredients[7] || result[1] != m_matchedIngredients[8] || result[2] != m_matchedIngredients[9])
				{
					SmeltingProgress = 0f;
					m_time = 0;
				}
				m_smeltingRecipe2 = flag;
				if (flag != m_smeltingRecipe)
				{
					m_smeltingRecipe = flag;
					SmeltingProgress = 0f;
					m_time = 0;
				}
			}
			if (m_smeltingRecipe2 && m_subsystemTime.PeriodicGameTimeEvent(0.2, 0.0))
			{
				int num = 1;
				int num2 = 0;
				var vector = new Vector3(CellFace.FaceToPoint3(FourDirectionalBlock.GetDirection(m_subsystemTerrain.Terrain.GetCellValue(coordinates.X, coordinates.Y, coordinates.Z))));
				int num3 = coordinates.X - (int)vector.X;
				int num4 = coordinates.Y - (int)vector.Y;
				int num5 = coordinates.Z - (int)vector.Z;
				for (int i = -1; i < 2; i++)
				{
					for (int j = -1; j < 3; j++)
					{
						for (int k = -1; k < 2; k++)
						{
                            int cellContents = m_subsystemTerrain.Terrain.GetCellValue(num3 + i, num4 + j, num5 + k);
                            int cellContents2 = m_subsystemTerrain.Terrain.GetCellContents(num3 + i, num4 + j, num5 + k);
                            if (j == 1 && cellContents != 1573374)
                            {
                                num = 0;
                                break;
                            }
                            if (i * i + k * k == 1 && j == 0 && cellContents2 == 0 && (m_subsystemTerrain.Terrain.GetCellContents(num3 + 2 * i, num4 + j, num5 + 2 * k) != BlastBlowerBlock.Index || m_subsystemTerrain.Terrain.GetCellValue(num3 + 2 * i, num4 + j, num5 + 2 * k) != (BlastBlowerBlock.Index | 1 << 14)) && (num3 + i != coordinates.X || num5 + k != coordinates.Z))
                            {

                                num = 0;
                                break;
                            }
                            if (i * i + k * k == 1 && j == 0 && cellContents2 == 0)
                            {
                                num2 = 1;
                            }
                            if (i * i + k * k == 1 && j == 0 && cellContents2 != 0 && cellContents != 1573374 && ((num3 + i) != coordinates.X || (num5 + k) != coordinates.Z))
                            {

                                num = 0;
                                break;
                            }
                            if (i * i + k * k == 2 && j == 0 && cellContents != 1573374)
                            {
                                num = 0;

                                break;
                            }
                            if (j < 0 && cellContents != 1049086)
                            {

                                num = 0;
                                break;
                            }
                        }
					}
				}
				if (num == 0 || num2 == 0)
				{
					m_smeltingRecipe = false;
				}
				if (num == 1 && num2 >= 2 && !m_smeltingRecipe)
				{
					m_smeltingRecipe = m_smeltingRecipe2;
				}
			}
			m_time++;
			if (!m_smeltingRecipe)
			{
				HeatLevel = 0f;
				m_fireTimeRemaining = 0f;
				SmeltingProgress = 0f;
			}
			if (m_smeltingRecipe && m_fireTimeRemaining <= 0f)
			{
				HeatLevel = 5f;
			}
			if (m_smeltingRecipe)
			{
				SmeltingProgress = MathUtils.Min(SmeltingProgress + 0.1f * dt, 1f);
				if (SmeltingProgress >= 1f)
				{
					int[] array = new int[]
					{
						IronIngotBlock.Index,
                        ItemBlock.IdTable["ScrapIron"],
                        ItemBlock.IdTable["AluminumOrePowder"],
                        ItemBlock.IdTable["ChromiumOrePowder"],
                        ItemBlock.IdTable["CokeCoalPowder"]
                    };
					for (int l = 0; l < 5; l++)
					{
						if (m_matchedIngredients[l] > 0)
						{
							int b = array[l];
							for (int m = 0; m < m_furnaceSize; m++)
							{
								if (m_slots[m].Count > 0 && GetSlotValue(m) == b)
								{
									if (m_slots[m].Count >= m_matchedIngredients[l])
									{
										m_slots[m].Count -= m_matchedIngredients[l];
										m_matchedIngredients[l] = 0;
									}
									else
									{
										m_matchedIngredients[l] -= m_slots[m].Count;
										m_slots[m].Count = 0;
									}
									if (m_matchedIngredients[l] == 0)
									{
										break;
									}
								}
							}
						}
					}
					if (m_matchedIngredients[5] >= 1)
					{
						m_slots[ResultSlotIndex].Value = ItemBlock.IdTable["SteelIngot"];
						m_slots[ResultSlotIndex].Count += m_matchedIngredients[5];
					}
					if (m_matchedIngredients[6] >= 1)
					{
						m_slots[ResultSlotIndex].Value = IronIngotBlock.Index;
						m_slots[ResultSlotIndex].Count += m_matchedIngredients[6];
					}
					if (m_matchedIngredients[7] >= 1)
					{
						m_slots[ResultSlotIndex].Value = ItemBlock.IdTable["FeAlCrAlloyIngot"];
						m_slots[ResultSlotIndex].Count += m_matchedIngredients[7];
					}
					m_smeltingRecipe = false;
					SmeltingProgress = 0f;
					m_updateSmeltingRecipe = true;
				}
			}
		}

		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			base.Load(valuesDictionary, idToEntityMap);
			m_subsystemTime = Project.FindSubsystem<SubsystemTime>(true);
			m_furnaceSize = SlotsCount - 3;
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

		protected bool FindSmeltingRecipe(float heatLevel)
		{
			if (heatLevel <= 0f)
			{
				return false;
			}
			bool flag = false;
			m_matchedIngredients2[0] = 0;
			m_matchedIngredients2[1] = 0;
			m_matchedIngredients2[2] = 0;
			m_matchedIngredients2[3] = 0;
			m_matchedIngredients2[4] = 0;
			m_matchedIngredients2[5] = 0;
			m_matchedIngredients2[6] = 0;
			m_matchedIngredients2[7] = 0;
			m_matchedIngredients2[8] = 0;
			m_matchedIngredients[0] = 0;
			m_matchedIngredients[1] = 0;
			m_matchedIngredients[2] = 0;
			m_matchedIngredients[3] = 0;
			m_matchedIngredients[4] = 0;
			m_matchedIngredients[5] = 0;
			m_matchedIngredients[6] = 0;
			m_matchedIngredients[7] = 0;
			m_matchedIngredients[8] = 0;
			m_matchedIngredients[9] = 0;
			for (int i = 0; i < m_furnaceSize+1; i++)
			{
				int slotValue = GetSlotValue(i);
				int num = Terrain.ExtractContents(slotValue);
				int slotCount = GetSlotCount(i);
				if (GetSlotCount(i) > 0)
				{
					if (slotValue == IronIngotBlock.Index)
					{
						m_matchedIngredients2[0] += slotCount;
					}
					else if (slotValue == ItemBlock.IdTable["ScrapIron"])
					{
						m_matchedIngredients2[1] += slotCount;
					}
					else if (slotValue == ItemBlock.IdTable["AluminumOrePowder"])
					{
						m_matchedIngredients2[2] += slotCount;
					}
					else if (slotValue == ItemBlock.IdTable["ChromiumOrePowder"])
					{
						m_matchedIngredients2[3] += slotCount;
					}
					else if (slotValue == ItemBlock.IdTable["CokeCoalPowder"])
					{
						m_matchedIngredients2[4] += slotCount;
					}
					else
					{
						m_matchedIngredients2[5] += slotCount;
					}
				}
			}
			if (m_matchedIngredients2[5] == 0)
			{
				if (m_matchedIngredients2[0] >= 4 && m_matchedIngredients2[4] >= 2 && m_matchedIngredients2[1] + m_matchedIngredients2[2] + m_matchedIngredients2[3] <= 0)
				{
					m_matchedIngredients[5] = 6;
					m_matchedIngredients[0] = 4;
					m_matchedIngredients[4] = 2;
					flag = true;
				}
                if (m_matchedIngredients2[1] >= 4 && m_matchedIngredients2[4] >= 2 && m_matchedIngredients2[0] + m_matchedIngredients2[2] + m_matchedIngredients2[3] <= 0)
                {
                    m_matchedIngredients[6] = 4;
                    m_matchedIngredients[1] = 4;
                    m_matchedIngredients[4] = 2;
                    flag = true;
                }
                if (m_matchedIngredients2[0] >= 2 && m_matchedIngredients2[2] >= 1 && m_matchedIngredients2[3] >= 1 && m_matchedIngredients2[4] >= 2 && m_matchedIngredients2[1]  <= 0)
                {
                    m_matchedIngredients[7] = 1;
                    m_matchedIngredients[0] = 2;
                    m_matchedIngredients[2] = 1;
                    m_matchedIngredients[3] = 1;
                    m_matchedIngredients[4] = 2;
                    flag = true;
                }
            }

			if (m_matchedIngredients[5] >= 1 && (m_slots[ResultSlotIndex].Value != ItemBlock.IdTable["SteelIngot"] || m_slots[ResultSlotIndex].Count + m_matchedIngredients[5] > 40) && m_slots[ResultSlotIndex].Count != 0)
			{
				flag = false;
			}
			if (m_matchedIngredients[6] >= 1 && (m_slots[ResultSlotIndex].Value != IronIngotBlock.Index || m_slots[ResultSlotIndex].Count + m_matchedIngredients[6] > 40) && m_slots[ResultSlotIndex].Count != 0)
			{
				flag = false;
			}
			if (m_matchedIngredients[7] >= 1 && (m_slots[ResultSlotIndex].Value != ItemBlock.IdTable["FeAlCrAlloyIngot"] || m_slots[ResultSlotIndex].Count + m_matchedIngredients[7] > 40) && m_slots[ResultSlotIndex].Count != 0)
			{
				flag = false;
			}
			return flag;
		}

		protected float m_fireTimeRemaining;

		protected int m_furnaceSize;

		protected int m_time;

		protected readonly int[] m_matchedIngredients2 = new int[9];

		protected readonly int[] m_matchedIngredients = new int[10];

		protected bool m_smeltingRecipe;

		protected bool m_smeltingRecipe2;
		protected readonly int[] result = new int[3];
	}
}
