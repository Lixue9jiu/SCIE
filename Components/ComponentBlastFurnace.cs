using Engine;
using GameEntitySystem;
using System;
using TemplatesDatabase;

namespace Game
{
	public class ComponentBlastFurnace : ComponentMachine, IUpdateable
	{
		protected int m_time;

		protected readonly int[] m_matchedIngredients = new int[10], m_matchedIngredients2 = new int[9];

		protected bool m_smeltingRecipe, m_smeltingRecipe2;

		protected readonly int[] result = new int[3];

		public override int RemainsSlotIndex => SlotsCount - 3;
		public override int ResultSlotIndex => SlotsCount - 4;

		public int UpdateOrder => 0;

		public void Update(float dt)
		{
			Point3 coordinates = m_componentBlockEntity.Coordinates;
			if (m_updateSmeltingRecipe)
			{
				m_updateSmeltingRecipe = false;
				result[0] = m_matchedIngredients[7];
				result[1] = m_matchedIngredients[8];
				result[2] = m_matchedIngredients[9];
				bool flag = FindSmeltingRecipe();
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
			if (m_smeltingRecipe2 && Utils.SubsystemTime.PeriodicGameTimeEvent(0.2, 0.0))
			{
				int num = 1;
				int num2 = 0;
				var point = CellFace.FaceToPoint3(FourDirectionalBlock.GetDirection(Utils.Terrain.GetCellValue(coordinates.X, coordinates.Y, coordinates.Z)));
				int num3 = coordinates.X - point.X;
				int num4 = coordinates.Y - point.Y;
				int num5 = coordinates.Z - point.Z, v;
				for (int i = -1; i < 2; i++)
				{
					for (int j = -1; j < 3; j++)
					{
						for (int k = -1; k < 2; k++)
						{
							int cellContents = Utils.Terrain.GetCellContents(num3 + i, num4 + j, num5 + k);
							if (i * i + k * k > 0 && j >= 1 && cellContents != 73)
							{
								num = 0;
								break;
							}
							if (i * i + k * k == 1 && j == 0 && cellContents == 0 && (v = Terrain.ReplaceLight(Utils.Terrain.GetCellValue(num3 + 2 * i, num4 + j, num5 + 2 * k), 0)) != (BlastBlowerBlock.Index | FurnaceNBlock.SetHeatLevel(Terrain.ExtractData(v), 1) << 14) && (num3 + i != coordinates.X || num5 + k != coordinates.Z))
							{
								num = 0;
								break;
							}
							if (i * i + k * k == 1 && j == 0 && cellContents == 0)
								num2 = 1;
							if (i * i + k * k == 1 && j == 0 && cellContents != 0 && cellContents != 73 && (num3 + i != coordinates.X || num5 + k != coordinates.Z))
							{
								num = 0;
								break;
							}
							if (i * i + k * k == 2 && j == 0 && cellContents != 73)
							{
								num = 0;
								break;
							}
							if (j < 0 && cellContents != 73)
							{
								num = 0;
								break;
							}
						}
					}
				}
				if (num == 0 || num2 == 0)
					m_smeltingRecipe = false;
				if (num == 1 && num2 >= 1 && !m_smeltingRecipe)
					m_smeltingRecipe = m_smeltingRecipe2;
			}
			m_time++;
			if (!m_smeltingRecipe)
			{
				HeatLevel = 0f;
				m_fireTimeRemaining = 0f;
				SmeltingProgress = 0f;
			}
			if (m_smeltingRecipe && m_fireTimeRemaining <= 0f)
				HeatLevel = 5f;
			if (m_smeltingRecipe)
			{
				SmeltingProgress = MathUtils.Min(SmeltingProgress + 0.1f * dt, 1f);
				if (SmeltingProgress >= 1f)
				{
					int[] arr =
					{
						IronOreChunkBlock.Index,
						ItemBlock.IdTable["IronOrePowder"],
						CoalChunkBlock.Index,
						ItemBlock.IdTable["CoalPowder"],
						SandBlock.Index,
						PigmentBlock.Index,
						IronIngotBlock.Index
					};
					for (int l = 0; l < 7; l++)
					{
						if (m_matchedIngredients[l] > 0)
						{
							int b = arr[l];
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
										break;
								}
							}
						}
					}
					if (m_matchedIngredients[8] >= 1)
					{
						m_slots[ResultSlotIndex].Value = ItemBlock.IdTable["SteelIngot"];
						m_slots[ResultSlotIndex].Count += m_matchedIngredients[8];
					}
					if (m_matchedIngredients[7] >= 1)
					{
						m_slots[RemainsSlotIndex].Value = ItemBlock.IdTable["ScrapIron"];
						m_slots[RemainsSlotIndex].Count += m_matchedIngredients[7];
					}
					if (m_matchedIngredients[9] >= 1)
					{
						m_slots[ResultSlotIndex].Value = IronIngotBlock.Index;
						m_slots[ResultSlotIndex].Count += m_matchedIngredients[9];
					}
					m_smeltingRecipe = false;
					SmeltingProgress = 0f;
					m_updateSmeltingRecipe = true;
				}
			}
		}

		public override int RemoveSlotItems(int slotIndex, int count)
		{
			count = base.RemoveSlotItems(slotIndex, count);
			m_updateSmeltingRecipe = slotIndex != ResultSlotIndex && slotIndex != RemainsSlotIndex;
			return count;
		}

		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			base.Load(valuesDictionary, idToEntityMap);
			m_furnaceSize = SlotsCount - 4;
			m_fireTimeRemaining = valuesDictionary.GetValue("FireTimeRemaining", 0f);
			HeatLevel = valuesDictionary.GetValue("HeatLevel", 0f);
		}

		protected bool FindSmeltingRecipe()
		{
			Array.Clear(m_matchedIngredients2, 0, m_matchedIngredients2.Length);
			Array.Clear(m_matchedIngredients, 0, m_matchedIngredients.Length);
			for (int i = 0; i < m_furnaceSize; i++)
			{
				int slotCount = GetSlotCount(i);
				if (slotCount <= 0) continue;
				int value = GetSlotValue(i);
				int contents = Terrain.ExtractContents(value);
				if (value == IronOreChunkBlock.Index)
					m_matchedIngredients2[0] += slotCount;
				else if (value == ItemBlock.IdTable["IronOrePowder"])
					m_matchedIngredients2[1] += slotCount;
				else if (contents == CoalChunkBlock.Index)
					m_matchedIngredients2[2] += slotCount;
				else if (value == ItemBlock.IdTable["CoalPowder"] || value == ItemBlock.IdTable["CokeCoalPowder"])
					m_matchedIngredients2[3] += slotCount;
				else if (contents == SandBlock.Index)
					m_matchedIngredients2[4] += slotCount;
				else if (contents == PigmentBlock.Index)
					m_matchedIngredients2[5] += slotCount;
				else if (contents == IronIngotBlock.Index)
					m_matchedIngredients2[6] += slotCount;
				else
					m_matchedIngredients2[7] += slotCount;
			}
			bool flag = false;
			int N;
			if (m_matchedIngredients2[7] == 0)
			{
				if (m_matchedIngredients2[0] >= 7 && (m_matchedIngredients2[2] >= 1 || m_matchedIngredients2[3] >= 1) && m_matchedIngredients2[1] + m_matchedIngredients2[4] + m_matchedIngredients2[5] + m_matchedIngredients2[6] <= 0)
				{
					N = m_random.UniformInt(8, 11);
					m_matchedIngredients[9] = N;
					m_matchedIngredients[0] = 7;
					m_matchedIngredients[2] = 1;
					m_matchedIngredients[3] = 1;
					m_matchedIngredients[7] = 14 - N;
					flag = true;
				}
				if (m_matchedIngredients2[1] >= 7 && (m_matchedIngredients2[2] >= 1 || m_matchedIngredients2[3] >= 1) && m_matchedIngredients2[0] + m_matchedIngredients2[4] + m_matchedIngredients2[5] + m_matchedIngredients2[6] <= 0)
				{
					N = m_random.UniformInt(10, 12);
					m_matchedIngredients[9] = N;
					m_matchedIngredients[1] = 7;
					m_matchedIngredients[2] = 1;
					m_matchedIngredients[3] = 1;
					m_matchedIngredients[7] = 15 - N;
					flag = true;
				}
				if (m_matchedIngredients2[1] >= 6 && (m_matchedIngredients2[2] >= 1 || m_matchedIngredients2[3] >= 1) && m_matchedIngredients2[4] >= 1 && m_matchedIngredients2[0] + m_matchedIngredients2[5] + m_matchedIngredients2[6] <= 0)
				{
					const int C = 11;
					m_matchedIngredients[9] = C;
					m_matchedIngredients[1] = 6;
					m_matchedIngredients[2] = 1;
					m_matchedIngredients[3] = 1;
					m_matchedIngredients[4] = 1;
					m_matchedIngredients[7] = 12 - C;
					flag = true;
				}
				if (m_matchedIngredients2[1] >= 6 && (m_matchedIngredients2[2] >= 1 || m_matchedIngredients2[3] >= 1) && m_matchedIngredients2[5] >= 1 && m_matchedIngredients2[0] + m_matchedIngredients2[4] + m_matchedIngredients2[6] <= 0)
				{
					const int C = 11;
					m_matchedIngredients[9] = C;
					m_matchedIngredients[1] = 6;
					m_matchedIngredients[2] = 1;
					m_matchedIngredients[3] = 1;
					m_matchedIngredients[5] = 1;
					m_matchedIngredients[7] = 12 - C;
					flag = true;
				}
				if (m_matchedIngredients2[1] >= 5 && (m_matchedIngredients2[2] >= 1 || m_matchedIngredients2[3] >= 1) && m_matchedIngredients2[4] >= 1 && m_matchedIngredients2[5] >= 1 && m_matchedIngredients2[0] + m_matchedIngredients2[6] <= 0)
				{
					const int C = 10;
					m_matchedIngredients[1] = 5;
					m_matchedIngredients[2] = 1;
					m_matchedIngredients[3] = 1;
					m_matchedIngredients[4] = 1;
					m_matchedIngredients[5] = 1;
					m_matchedIngredients[9] = C;
					m_matchedIngredients[7] = 0;
					flag = true;
				}
				if (m_matchedIngredients2[6] >= 6 && m_matchedIngredients2[3] >= 2 && m_matchedIngredients2[0] + m_matchedIngredients2[1] + m_matchedIngredients2[2] + m_matchedIngredients2[4] + m_matchedIngredients2[5] <= 0)
				{
					N = m_random.UniformInt(2, 4);
					m_matchedIngredients[6] = 6;
					m_matchedIngredients[3] = 2;
					m_matchedIngredients[8] = N;
					m_matchedIngredients[7] = 6 - N;
					flag = true;
				}
				if (m_matchedIngredients2[6] >= 5 && m_matchedIngredients2[3] >= 2 && m_matchedIngredients2[5] >= 1 && m_matchedIngredients2[0] + m_matchedIngredients2[1] + m_matchedIngredients2[2] + m_matchedIngredients2[4] <= 0)
				{
					N = m_random.UniformInt(3, 5);
					m_matchedIngredients[6] = 6;
					m_matchedIngredients[3] = 2;
					m_matchedIngredients[8] = N;
					m_matchedIngredients[7] = 6 - N;
					flag = true;
				}
			}

			if ((m_matchedIngredients[8] >= 1 && (m_slots[ResultSlotIndex].Value != ItemBlock.IdTable["SteelIngot"] || m_slots[ResultSlotIndex].Count + m_matchedIngredients[8] > 40) && m_slots[ResultSlotIndex].Count != 0) ||
				(m_matchedIngredients[7] >= 1 && (m_slots[RemainsSlotIndex].Value != ItemBlock.IdTable["ScrapIron"] || m_slots[RemainsSlotIndex].Count + m_matchedIngredients[7] > 40) && m_slots[RemainsSlotIndex].Count != 0) ||
				(m_matchedIngredients[9] >= 1 && (m_slots[ResultSlotIndex].Value != IronIngotBlock.Index || m_slots[ResultSlotIndex].Count + m_matchedIngredients[9] > 40) && m_slots[ResultSlotIndex].Count != 0))
				return false;
			return flag;
		}
	}
}