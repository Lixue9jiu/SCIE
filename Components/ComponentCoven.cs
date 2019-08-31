using Engine;
using GameEntitySystem;
using System;
using TemplatesDatabase;

namespace Game
{
	public class ComponentCoven : ComponentMachine, IUpdateable
	{
		protected int m_time;

		protected readonly int[] m_matchedIngredients = new int[10], m_matchedIngredients2 = new int[9];

		protected bool m_smeltingRecipe, m_smeltingRecipe2;
		protected readonly int[] result = new int[3];

		public override int RemainsSlotIndex => SlotsCount - 1;

		public override int ResultSlotIndex => SlotsCount - 2;

		public void Update(float dt)
		{
			Point3 coordinates = m_componentBlockEntity.Coordinates;
			if (m_updateSmeltingRecipe)
			{
				m_updateSmeltingRecipe = false;
				result[0] = m_matchedIngredients[7];
				result[1] = m_matchedIngredients[8];
				result[2] = m_matchedIngredients[9];
				if (result[0] != m_matchedIngredients[7] || result[1] != m_matchedIngredients[8] || result[2] != m_matchedIngredients[9])
				{
					SmeltingProgress = 0f;
					m_time = 0;
				}
				m_smeltingRecipe2 = FindSmeltingRecipe();
				if (m_smeltingRecipe2 != m_smeltingRecipe)
				{
					m_smeltingRecipe = m_smeltingRecipe2;
					SmeltingProgress = 0f;
					m_time = 0;
				}
			}
			if (m_smeltingRecipe2 && Utils.SubsystemTime.PeriodicGameTimeEvent(0.2, 0.0))
			{
				int num = 1, num2 = 0;
				var point = CellFace.FaceToPoint3(FourDirectionalBlock.GetDirection(Utils.Terrain.GetCellValue(coordinates.X, coordinates.Y, coordinates.Z)));
				int num3 = coordinates.X - point.X;
				int num4 = coordinates.Y - point.Y;
				int num5 = coordinates.Z - point.Z, v;
				for (int i = -1; i < 2; i++)
				{
					for (int j = -1; j < 2; j++)
					{
						for (int k = -1; k < 2; k++)
						{
							int cellValue = Terrain.ReplaceLight(Utils.Terrain.GetCellValue(num3 + i, num4 + j, num5 + k), 0);
							int cellContents = Terrain.ExtractContents(cellValue);
							if (j == 1 && cellValue != 1049086)
							{
								num = 0;
								break;
							}
							if (i * i + k * k == 1 && j == 0 && cellContents == 0 && ((v = Utils.Terrain.GetCellValue(num3 + 2 * i, num4 + j, num5 + 2 * k)) != (BlastBlowerBlock.Index | FurnaceNBlock.SetHeatLevel(Terrain.ExtractData(v), 1) << 14)) && (num3 + i != coordinates.X || num5 + k != coordinates.Z))
							{
								num = 0;
								break;
							}
							if (i * i + k * k == 1 && j == 0 && cellContents == 0)
								num2 = 1;
							if (i * i + k * k == 1 && j == 0 && cellContents != 0 && cellValue != 1049086 && ((num3 + i) != coordinates.X || (num5 + k) != coordinates.Z))
							{
								num = 0;
								break;
							}
							if (i * i + k * k == 2 && j == 0 && cellValue != 1049086)
							{
								num = 0;
								break;
							}
							if (j < 0 && cellValue != 1049086)
							{
								num = 0;
								break;
							}
						}
					}
				}
				if (num == 0 || num2 == 0)
					m_smeltingRecipe = false;
				if (num == 1 && num2 == 1 && !m_smeltingRecipe)
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
					int[] array = new int[]
					{
						CoalChunkBlock.Index,
						ItemBlock.IdTable["CoalPowder"],
						PlanksBlock.Index,
						OakWoodBlock.Index,
						BirchWoodBlock.Index,
						SpruceWoodBlock.Index,
						CactusBlock.Index
					};

					var point1 = CellFace.FaceToPoint3(FourDirectionalBlock.GetDirection(Utils.Terrain.GetCellValue(coordinates.X, coordinates.Y, coordinates.Z)));
					int num31 = coordinates.X - point1.X;
					int num41 = coordinates.Y - point1.Y;
					int num51 = coordinates.Z - point1.Z;
					int cellValue1 = Terrain.ReplaceLight(Utils.Terrain.GetCellValue(num31, num41 + 2, num51), 0);
					if (ElementBlock.Block.GetDevice(num31, num41 + 2, num51, cellValue1) is AirPresser em && em.Powered)
					{
						var point3 = new Point3(num31, num41 + 2, num51);
						var entity = Utils.GetBlockEntity(point3);
						Component component3 = entity.Entity.FindComponent<ComponentAirPresser>();
						if (entity != null && component3!=null)
						{

							IInventory inventory = entity.Entity.FindComponent<ComponentAirPresser>(true);
							for (int i = 0; i < 6; i++)
							{
								//int value23 = 0;
								int va1 = inventory.GetSlotValue(i);
								if (Utils.Random.Bool(0.2f))
								{
									if (va1 == ItemBlock.IdTable["멀틸"] && ComponentInventoryBase.AcquireItems(inventory, ItemBlock.IdTable["NH3"], 1) == 0)
									{
										inventory.RemoveSlotItems(i, 1);
									}
								}else
								{
									if (va1 == ItemBlock.IdTable["멀틸"] && ComponentInventoryBase.AcquireItems(inventory, ItemBlock.IdTable["CH6"], 1) == 0)
									{
										inventory.RemoveSlotItems(i, 1);
									}
								}
							}
							
						}
					}

					for (int l = 0; l < 7; l++)
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
										break;
								}
							}
						}
					}
					if (m_matchedIngredients[7] >= 1)
					{
						m_slots[ResultSlotIndex].Value = ItemBlock.IdTable["CokeCoalChunk"];
						m_slots[ResultSlotIndex].Count += m_matchedIngredients[7];
					}
					if (m_matchedIngredients[8] >= 1)
					{
						m_slots[ResultSlotIndex].Value = ItemBlock.IdTable["CokeCoalPowder"];
						m_slots[ResultSlotIndex].Count += m_matchedIngredients[8];
					}
					if (m_matchedIngredients[9] >= 1)
					{
						m_slots[ResultSlotIndex].Value = CoalChunkBlock.Index;
						m_slots[ResultSlotIndex].Count += m_matchedIngredients[9];
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
			m_furnaceSize = SlotsCount - 2;
		}

		protected bool FindSmeltingRecipe()
		{
			bool flag = false;
			Array.Clear(m_matchedIngredients2, 0, m_matchedIngredients2.Length);
			Array.Clear(m_matchedIngredients, 0, m_matchedIngredients.Length);
			for (int i = 0; i < m_furnaceSize; i++)
			{
				int slotCount = GetSlotCount(i);
				if (slotCount <= 0) continue;
				int value = GetSlotValue(i);
				if (value == CoalChunkBlock.Index)
					m_matchedIngredients2[0] += slotCount;
				else if (value == ItemBlock.IdTable["CoalPowder"])
					m_matchedIngredients2[1] += slotCount;
				else if (value == PlanksBlock.Index)
					m_matchedIngredients2[2] += slotCount;
				else if (value == OakWoodBlock.Index)
					m_matchedIngredients2[3] += slotCount;
				else if (value == BirchWoodBlock.Index)
					m_matchedIngredients2[4] += slotCount;
				else if (value == SpruceWoodBlock.Index)
					m_matchedIngredients2[5] += slotCount;
				else if (value == CactusBlock.Index)
					m_matchedIngredients2[6] += slotCount;
				else
					m_matchedIngredients2[7] += slotCount;
			}
			if (m_matchedIngredients2[7] == 0)
			{
				if (m_matchedIngredients2[0] >= 6 && m_matchedIngredients2[1] + m_matchedIngredients2[2] + m_matchedIngredients2[3] + m_matchedIngredients2[4] + m_matchedIngredients2[5] + m_matchedIngredients2[6] <= 0)
				{
					m_matchedIngredients[7] = 1;
					m_matchedIngredients[0] = 6;
					flag = true;
				}
				if (m_matchedIngredients2[1] >= 6 && m_matchedIngredients2[0] + m_matchedIngredients2[2] + m_matchedIngredients2[3] + m_matchedIngredients2[4] + m_matchedIngredients2[5] + m_matchedIngredients2[6] <= 0)
				{
					m_matchedIngredients[8] = 2;
					m_matchedIngredients[1] = 6;
					flag = true;
				}
				if (m_matchedIngredients2[2] >= 6 && m_matchedIngredients2[0] + m_matchedIngredients2[1] + m_matchedIngredients2[3] + m_matchedIngredients2[4] + m_matchedIngredients2[5] + m_matchedIngredients2[6] <= 0)
				{
					m_matchedIngredients[9] = 2;
					m_matchedIngredients[2] = 6;
					flag = true;
				}
				if (m_matchedIngredients2[3] + m_matchedIngredients2[4] + m_matchedIngredients2[5] >= 6 && m_matchedIngredients2[0] + m_matchedIngredients2[1] + m_matchedIngredients2[2] + m_matchedIngredients2[6] <= 0)
				{
					m_matchedIngredients[9] = 8;
					int num = 6;
					for (int i = 3; i < 7; i++)
					{
						if (m_matchedIngredients2[i] >= num)
						{
							m_matchedIngredients[i] = num;
							num = 0;
						}
						else
						{
							m_matchedIngredients[i] = m_matchedIngredients2[i];
							num -= m_matchedIngredients2[i];
						}
						if (num == 0)
							break;
					}
					flag = true;
				}
				if (m_matchedIngredients2[6] >= 6 && m_matchedIngredients2[0] + m_matchedIngredients2[1] + m_matchedIngredients2[2] + m_matchedIngredients2[3] + m_matchedIngredients2[4] + m_matchedIngredients2[5] <= 0)
				{
					m_matchedIngredients[9] = 1;
					m_matchedIngredients[6] = 6;
					flag = true;
				}
			}
			if ((m_matchedIngredients[9] >= 1 && (m_slots[ResultSlotIndex].Value != CoalChunkBlock.Index || m_slots[ResultSlotIndex].Count + m_matchedIngredients[9] > 40) && m_slots[ResultSlotIndex].Count != 0) ||
				(m_matchedIngredients[8] >= 1 && (m_slots[ResultSlotIndex].Value != ItemBlock.IdTable["CokeCoalPowder"] || m_slots[ResultSlotIndex].Count + m_matchedIngredients[8] > 40) && m_slots[ResultSlotIndex].Count != 0) ||
				(m_matchedIngredients[7] >= 1 && (m_slots[ResultSlotIndex].Value != ItemBlock.IdTable["CokeCoalChunk"] || m_slots[ResultSlotIndex].Count + m_matchedIngredients[7] > 40) && m_slots[ResultSlotIndex].Count != 0))
				return false;
			return flag;
		}
	}
}