using Engine;
using GameEntitySystem;
using System;
using TemplatesDatabase;

namespace Game
{
	public class ComponentHearthFurnace : ComponentMachine, IUpdateable
	{
		protected int m_time;

		protected readonly int[] m_matchedIngredients2 = new int[9];

		protected readonly int[] m_matchedIngredients = new int[10];

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
			int num = 1, m = 0;
			if (m_smeltingRecipe2 && Utils.SubsystemTime.PeriodicGameTimeEvent(0.2, 0.0))
			{
				num = 1;
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
							int cellValue = Terrain.ReplaceLight(Utils.Terrain.GetCellValue(num3 + i, num4 + j, num5 + k), 0);
							int cellContents2 = Terrain.ExtractContents(cellValue);
							if (j == 1 && cellValue != (MetalBlock.Index | 96 << 14))
							{
								num = 0;
								break;
							}
							if (i * i + k * k == 1 && j == 0 && cellContents2 == 0 && ((v = Utils.Terrain.GetCellValue(num3 + 2 * i, num4 + j, num5 + 2 * k)) != (BlastBlowerBlock.Index | FurnaceNBlock.SetHeatLevel(Terrain.ExtractData(v), 1) << 14)) && (num3 + i != coordinates.X || num5 + k != coordinates.Z))
							{
								num = 0;
								break;
							}
							if (i * i + k * k == 1 && j == 0 && cellContents2 == 0)
								m++;
							if (i * i + k * k == 1 && j == 0 && cellContents2 != 0 && cellValue != (MetalBlock.Index | 96 << 14) && (num3 + i != coordinates.X || num5 + k != coordinates.Z))
							{
								num = 0;
								break;
							}
							if (i * i + k * k == 2 && j == 0 && cellValue != (MetalBlock.Index | 96 << 14))
							{
								num = 0;
								break;
							}
							if (j < 0 && cellValue != (MetalBlock.Index | 64 << 14))
							{
								num = 0;
								break;
							}
						}
					}
				}
				if (num == 0 || m == 0)
					m_smeltingRecipe = false;
				if (num == 1 && m >= 2 && !m_smeltingRecipe)
					m_smeltingRecipe = m_smeltingRecipe2;
			}

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
					int[] array =
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
							num = array[l];
							for (m = 0; m < m_furnaceSize; m++)
							{
								if (m_slots[m].Count > 0 && GetSlotValue(m) == num)
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
			m_furnaceSize = SlotsCount - 2;
		}

		protected bool FindSmeltingRecipe(float heatLevel)
		{
			if (heatLevel <= 0f)
				return false;
			Array.Clear(m_matchedIngredients2, 0, m_matchedIngredients2.Length);
			Array.Clear(m_matchedIngredients, 0, m_matchedIngredients.Length);
			for (int i = 0; i < m_furnaceSize; i++)
			{
				if (GetSlotCount(i) <= 0) continue;
				int slotValue = GetSlotValue(i);
				int num = Terrain.ExtractContents(slotValue);
				int slotCount = GetSlotCount(i);
				if (num == IronIngotBlock.Index)
					m_matchedIngredients2[0] += slotCount;
				else if (slotValue == ItemBlock.IdTable["ScrapIron"])
					m_matchedIngredients2[1] += slotCount;
				else if (slotValue == ItemBlock.IdTable["AluminumOrePowder"])
					m_matchedIngredients2[2] += slotCount;
				else if (slotValue == ItemBlock.IdTable["ChromiumOrePowder"])
					m_matchedIngredients2[3] += slotCount;
				else if (slotValue == ItemBlock.IdTable["CokeCoalPowder"])
					m_matchedIngredients2[4] += slotCount;
				else
					m_matchedIngredients2[5] += slotCount;
			}
			bool flag = false;
			if (m_matchedIngredients2[5] == 0)
			{
				if (m_matchedIngredients2[0] >= 4 && m_matchedIngredients2[4] >= 2 && m_matchedIngredients2[1] + m_matchedIngredients2[2] + m_matchedIngredients2[3] <= 0)
				{
					m_matchedIngredients[5] = 4;
					m_matchedIngredients[0] = 4;
					m_matchedIngredients[4] = 1;
					flag = true;
				}
				if (m_matchedIngredients2[1] >= 4 && m_matchedIngredients2[4] >= 2 && m_matchedIngredients2[0] + m_matchedIngredients2[2] + m_matchedIngredients2[3] <= 0)
				{
					m_matchedIngredients[6] = 4;
					m_matchedIngredients[1] = 4;
					m_matchedIngredients[4] = 1;
					flag = true;
				}
				if (m_matchedIngredients2[0] >= 2 && m_matchedIngredients2[2] >= 1 && m_matchedIngredients2[3] >= 1 && m_matchedIngredients2[4] >= 2 && m_matchedIngredients2[1] <= 0)
				{
					m_matchedIngredients[7] = 1;
					m_matchedIngredients[0] = 2;
					m_matchedIngredients[2] = 1;
					m_matchedIngredients[3] = 1;
					m_matchedIngredients[4] = 2;
					flag = true;
				}
			}
			if ((m_matchedIngredients[5] >= 1 && (m_slots[ResultSlotIndex].Value != ItemBlock.IdTable["SteelIngot"] || m_slots[ResultSlotIndex].Count + m_matchedIngredients[5] > 40) && m_slots[ResultSlotIndex].Count != 0) ||
				(m_matchedIngredients[6] >= 1 && (Terrain.ExtractContents(m_slots[ResultSlotIndex].Value) != IronIngotBlock.Index || m_slots[ResultSlotIndex].Count + m_matchedIngredients[6] > 40) && m_slots[ResultSlotIndex].Count != 0) ||
				(m_matchedIngredients[7] >= 1 && (m_slots[ResultSlotIndex].Value != ItemBlock.IdTable["FeAlCrAlloyIngot"] || m_slots[ResultSlotIndex].Count + m_matchedIngredients[7] > 40) && m_slots[ResultSlotIndex].Count != 0))
				return false;
			return flag;
		}
	}

	public class ComponentAutoFactory : ComponentMachine, IUpdateable
	{
		protected int m_time;
		public bool Powered;
		protected readonly int[] m_matchedIngredients2 = new int[12];

		protected readonly int[] m_matchedIngredients = new int[36];

		protected bool m_smeltingRecipe, m_smeltingRecipe2;
		protected readonly int[] result = new int[4];
		public override int RemainsSlotIndex => 9 - 2;

		public override int ResultSlotIndex => 9 - 3;

		
		public void Update(float dt)
		{
			if (m_updateSmeltingRecipe || Utils.SubsystemTime.PeriodicGameTimeEvent(0.5, 0.0))
			{
				m_updateSmeltingRecipe = false;
				//result[0] = GetSlotValue(ResultSlotIndex);
				//result[2] = GetSlotValue(RemainsSlotIndex);
				bool flag = FindSmeltingRecipe(5f);
				if ((result[0] != GetSlotValue(ResultSlotIndex) && GetSlotCount(ResultSlotIndex) >0 ) || (result[2] != GetSlotValue(RemainsSlotIndex) && GetSlotCount(RemainsSlotIndex) > 0))
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
			if (Powered)
			{
				if (m_smeltingRecipe)
				{
					SmeltingProgress = MathUtils.Min(SmeltingProgress + 0.1f * dt, 1f);
					if (SmeltingProgress >= 1f)
					{
						m_slots[ResultSlotIndex].Value = result[0];
						m_slots[ResultSlotIndex].Count += result[1];
						m_slots[RemainsSlotIndex].Value = result[2];
						m_slots[RemainsSlotIndex].Count += result[3];
						SmeltingProgress = 0f;
						for (int i1 = 0; i1 < 6; i1++)
						{
							if (m_slots[i1].Value == m_matchedIngredients2[i1 * 2])
								m_slots[i1].Count = m_matchedIngredients2[i1 * 2 + 1];
						}
						//m_heatLevel = 0f;
					}
				}
			}
			
		}

		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			base.Load(valuesDictionary, idToEntityMap);
			m_furnaceSize = SlotsCount - 2;
		}

		protected bool FindSmeltingRecipe(float heatLevel)
		{
			Point3 coordinates = m_componentBlockEntity.Coordinates;
			int num3 = coordinates.X;
			int num4 = coordinates.Y-1;
			int num5 = coordinates.Z;
			var component3 = Utils.GetBlockEntity(new Point3(num3, num4, num5))?.Entity.FindComponent<ComponentLargeCraftingTable>();
			result[0] = 0;
			result[1] = 0;
			result[2] = 0;
			result[3] = 0;
			if (component3 != null && component3.GetSlotValue(component3.ResultSlotIndex)>0 && component3.m_matchedRecipe != null && component3.m_matchedRecipe.ResultCount>0)
			{
				result[0] = component3.m_matchedRecipe.ResultValue;
				result[1] = component3.m_matchedRecipe.ResultCount;
				result[2] = component3.m_matchedRecipe.RemainsValue;
				result[3] = component3.m_matchedRecipe.RemainsCount;
				for (int i1 =0; i1 <6; i1++)
				{
					m_matchedIngredients[i1 * 2] = GetSlotValue(i1);
					m_matchedIngredients[i1 * 2+1] = GetSlotCount(i1);
					m_matchedIngredients2[i1 * 2] = GetSlotValue(i1);
					m_matchedIngredients2[i1 * 2 + 1] = GetSlotCount(i1);
				}
				for (int ii = 0; ii < component3.m_craftingGridSize * component3.m_craftingGridSize; ii++)
				{
					int yy = component3.GetSlotValue(ii);
					int num = MathUtils.Min(1,component3.GetSlotCount(ii));
					//if (num == 0) break;
					for (int i1 = 0; i1 < 6; i1++)
					{
						if (m_matchedIngredients2[i1 * 2] == yy && m_matchedIngredients2[i1 * 2 + 1] >= num)
						{
							m_matchedIngredients2[i1 * 2 + 1] -= num;
							i1 = 6;
						}
						if (i1==5)
						return false;
					}
				}



				return true;
			}
			if (component3 != null && component3.GetSlotValue(component3.ResultSlotIndex) > 0)
			{
				result[0] = component3.GetSlotValue(component3.ResultSlotIndex);
				result[1] = component3.GetSlotCount(component3.ResultSlotIndex);
				result[2] = component3.GetSlotValue(component3.RemainsSlotIndex);
				result[3] = component3.GetSlotCount(component3.RemainsSlotIndex);
				for (int i1 = 0; i1 < 6; i1++)
				{
					m_matchedIngredients[i1 * 2] = GetSlotValue(i1);
					m_matchedIngredients[i1 * 2 + 1] = GetSlotCount(i1);
					m_matchedIngredients2[i1 * 2] = GetSlotValue(i1);
					m_matchedIngredients2[i1 * 2 + 1] = GetSlotCount(i1);
				}
				int min = 9999999;
				for (int ii = 0; ii < component3.m_craftingGridSize * component3.m_craftingGridSize; ii++)
				{
					int num = component3.GetSlotCount(ii);
					if (num>0)
					{
						min = MathUtils.Min(min, num);
					}
				}
				for (int ii = 0; ii < component3.m_craftingGridSize * component3.m_craftingGridSize; ii++)
				{
					int yy = component3.GetSlotValue(ii);
					int num = MathUtils.Min(min,component3.GetSlotCount(ii));
					//if (num == 0) break;
					for (int i1 = 0; i1 < 6; i1++)
					{
						if (m_matchedIngredients2[i1 * 2] == yy && m_matchedIngredients2[i1 * 2 + 1] >= num)
						{
							m_matchedIngredients2[i1 * 2 + 1] -= num;
							i1 = 6;
						}
						if (i1 == 5)
							return false;
					}
				}
				return true;
			}

			return false;
		}
	}


}