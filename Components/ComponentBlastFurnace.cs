using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000613 RID: 1555
	public class ComponentBlastFurnace : ComponentInventoryBase, IUpdateable
	{
		// Token: 0x17000551 RID: 1361
		// (get) Token: 0x06002166 RID: 8550 RVA: 0x00006A99 File Offset: 0x00004C99
		public int RemainsSlotIndex
		{
			get
			{
				return SlotsCount - 3;
			}
		}

		// Token: 0x17000552 RID: 1362
		// (get) Token: 0x06002167 RID: 8551 RVA: 0x00015665 File Offset: 0x00013865
		public int ResultSlotIndex
		{
			get
			{
				return SlotsCount - 4;
			}
		}

		// Token: 0x17000553 RID: 1363
		// (get) Token: 0x06002168 RID: 8552 RVA: 0x0001566F File Offset: 0x0001386F
		// (set) Token: 0x06002169 RID: 8553 RVA: 0x00015677 File Offset: 0x00013877
		public float HeatLevel { get; private set; }

		// Token: 0x17000554 RID: 1364
		// (get) Token: 0x0600216A RID: 8554 RVA: 0x00015680 File Offset: 0x00013880
		// (set) Token: 0x0600216B RID: 8555 RVA: 0x00015688 File Offset: 0x00013888
		public float SmeltingProgress { get; private set; }

		// Token: 0x17000555 RID: 1365
		// (get) Token: 0x0600216C RID: 8556 RVA: 0x000034CC File Offset: 0x000016CC
		public int UpdateOrder
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x0600216D RID: 8557 RVA: 0x000E0ABC File Offset: 0x000DECBC
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
				Vector3 vector = new Vector3(CellFace.FaceToPoint3(BlastFurnaceBlock.GetDirection(Terrain.ExtractData(m_subsystemTerrain.Terrain.GetCellValue(coordinates.X, coordinates.Y, coordinates.Z)))));
				int num3 = coordinates.X - (int)vector.X;
				int num4 = coordinates.Y - (int)vector.Y;
				int num5 = coordinates.Z - (int)vector.Z;
				for (int i = -1; i < 2; i++)
				{
					for (int j = -1; j < 3; j++)
					{
						for (int k = -1; k < 2; k++)
						{
							int cellContents = m_subsystemTerrain.Terrain.GetCellContents(num3 + i, num4 + j, num5 + k);
							if (i * i + k * k > 0 && j >= 1 && cellContents != 73)
							{
								num = 0;
								break;
							}
							if (i * i + k * k == 1 && j == 0 && cellContents == 0 && (m_subsystemTerrain.Terrain.GetCellContents(num3 + 2 * i, num4 + j, num5 + 2 * k) != 540 || m_subsystemTerrain.Terrain.GetCellValue(num3 + 2 * i, num4 + j, num5 + 2 * k) != 16924) && (num3 + i != coordinates.X || num5 + k != coordinates.Z))
							{
								num = 0;
								break;
							}
							if (i * i + k * k == 1 && j == 0 && cellContents == 0)
							{
								num2 = 1;
							}
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
				{
					m_smeltingRecipe = false;
				}
				if (num == 1 && num2 >= 1 && !m_smeltingRecipe)
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
			if (m_smeltingRecipe && (double)m_fireTimeRemaining <= 0.0)
			{
				HeatLevel = 5f;
			}
			if (m_smeltingRecipe)
			{
				SmeltingProgress = MathUtils.Min(SmeltingProgress + 0.1f * dt, 1f);
				if ((double)SmeltingProgress >= 1.0)
				{
					string[] array = new string[]
					{
						"ironorechunk",
						"ironorepowder",
						"coalchunk",
						"coalpowder",
						"sand",
						"pigment",
						"ironingot"
					};
					for (int l = 0; l < 7; l++)
					{
						if (m_matchedIngredients[l] > 0)
						{
							string b = array[l];
							for (int m = 0; m < m_furnaceSize; m++)
							{
								if (m_slots[m].Count > 0 && BlocksManager.Blocks[GetSlotValue(m)].CraftingId == b)
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
					if (m_matchedIngredients[8] >= 1)
					{
						m_slots[ResultSlotIndex].Value = 508;
						m_slots[ResultSlotIndex].Count += m_matchedIngredients[8];
					}
					if (m_matchedIngredients[7] >= 1)
					{
						m_slots[RemainsSlotIndex].Value = 546;
						m_slots[RemainsSlotIndex].Count += m_matchedIngredients[7];
					}
					if (m_matchedIngredients[9] >= 1)
					{
						m_slots[ResultSlotIndex].Value = 40;
						m_slots[ResultSlotIndex].Count += m_matchedIngredients[9];
					}
					m_smeltingRecipe = false;
					SmeltingProgress = 0f;
					m_updateSmeltingRecipe = true;
				}
			}
		}

		// Token: 0x0600216E RID: 8558 RVA: 0x00015691 File Offset: 0x00013891
		public override void AddSlotItems(int slotIndex, int value, int count)
		{
			base.AddSlotItems(slotIndex, value, count);
			m_updateSmeltingRecipe = true;
		}

		// Token: 0x0600216F RID: 8559 RVA: 0x000156A3 File Offset: 0x000138A3
		public override int RemoveSlotItems(int slotIndex, int count)
		{
			m_updateSmeltingRecipe = true;
			return base.RemoveSlotItems(slotIndex, count);
		}

		// Token: 0x06002170 RID: 8560 RVA: 0x000E10C4 File Offset: 0x000DF2C4
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			base.Load(valuesDictionary, idToEntityMap);
			m_subsystemTerrain = Project.FindSubsystem<SubsystemTerrain>(true);
			m_subsystemTime = Project.FindSubsystem<SubsystemTime>(true);
			m_subsystemExplosions = Project.FindSubsystem<SubsystemExplosions>(true);
			m_componentBlockEntity = Entity.FindComponent<ComponentBlockEntity>(true);
			m_furnaceSize = SlotsCount - 4;
			m_fireTimeRemaining = valuesDictionary.GetValue<float>("FireTimeRemaining");
			HeatLevel = valuesDictionary.GetValue<float>("HeatLevel");
			m_updateSmeltingRecipe = true;
		}

		// Token: 0x06002171 RID: 8561 RVA: 0x000156B4 File Offset: 0x000138B4
		public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
		{
			base.Save(valuesDictionary, entityToIdMap);
			valuesDictionary.SetValue<float>("FireTimeRemaining", m_fireTimeRemaining);
			valuesDictionary.SetValue<float>("HeatLevel", HeatLevel);
		}

		// Token: 0x06002173 RID: 8563 RVA: 0x000E1158 File Offset: 0x000DF358
		private bool FindSmeltingRecipe(float heatLevel)
		{
			if ((double)heatLevel <= 0.0)
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
			for (int i = 0; i < m_furnaceSize; i++)
			{
				int slotValue = GetSlotValue(i);
				int num = Terrain.ExtractContents(slotValue);
				Terrain.ExtractData(slotValue);
				int slotCount = GetSlotCount(i);
				if (GetSlotCount(i) > 0)
				{
					Block block = BlocksManager.Blocks[num];
					if (block.CraftingId == "ironorechunk")
					{
						m_matchedIngredients2[0] += slotCount;
					}
					else if (block.CraftingId == "ironorepowder")
					{
						m_matchedIngredients2[1] += slotCount;
					}
					else if (block.CraftingId == "coalchunk")
					{
						m_matchedIngredients2[2] += slotCount;
					}
					else if (block.CraftingId == "coalpowder")
					{
						m_matchedIngredients2[3] += slotCount;
					}
					else if (block.CraftingId == "sand")
					{
						m_matchedIngredients2[4] += slotCount;
					}
					else if (block.CraftingId == "pigment")
					{
						m_matchedIngredients2[5] += slotCount;
					}
					else if (block.CraftingId == "ironingot")
					{
						m_matchedIngredients2[6] += slotCount;
					}
					else
					{
						m_matchedIngredients2[7] += slotCount;
					}
				}
			}
			if (m_matchedIngredients2[7] == 0)
			{
				if (m_matchedIngredients2[0] >= 7 && (m_matchedIngredients2[2] >= 1 || m_matchedIngredients2[3] >= 1) && m_matchedIngredients2[1] + m_matchedIngredients2[4] + m_matchedIngredients2[5] + m_matchedIngredients2[6] <= 0)
				{
					int num2 = m_random.UniformInt(8, 11);
					m_matchedIngredients[9] = num2;
					m_matchedIngredients[0] = 7;
					m_matchedIngredients[2] = 1;
					m_matchedIngredients[3] = 1;
					m_matchedIngredients[7] = 14 - num2;
					flag = true;
				}
				if (m_matchedIngredients2[1] >= 7 && (m_matchedIngredients2[2] >= 1 || m_matchedIngredients2[3] >= 1) && m_matchedIngredients2[0] + m_matchedIngredients2[4] + m_matchedIngredients2[5] + m_matchedIngredients2[6] <= 0)
				{
					int num3 = m_random.UniformInt(10, 12);
					m_matchedIngredients[9] = num3;
					m_matchedIngredients[1] = 7;
					m_matchedIngredients[2] = 1;
					m_matchedIngredients[3] = 1;
					m_matchedIngredients[7] = 15 - num3;
					flag = true;
				}
				if (m_matchedIngredients2[1] >= 6 && (m_matchedIngredients2[2] >= 1 || m_matchedIngredients2[3] >= 1) && m_matchedIngredients2[4] >= 1 && m_matchedIngredients2[0] + m_matchedIngredients2[5] + m_matchedIngredients2[6] <= 0)
				{
					int num4 = 11;
					m_matchedIngredients[9] = num4;
					m_matchedIngredients[1] = 6;
					m_matchedIngredients[2] = 1;
					m_matchedIngredients[3] = 1;
					m_matchedIngredients[4] = 1;
					m_matchedIngredients[7] = 12 - num4;
					flag = true;
				}
				if (m_matchedIngredients2[1] >= 6 && (m_matchedIngredients2[2] >= 1 || m_matchedIngredients2[3] >= 1) && m_matchedIngredients2[5] >= 1 && m_matchedIngredients2[0] + m_matchedIngredients2[4] + m_matchedIngredients2[6] <= 0)
				{
					int num5 = 11;
					m_matchedIngredients[9] = num5;
					m_matchedIngredients[1] = 6;
					m_matchedIngredients[2] = 1;
					m_matchedIngredients[3] = 1;
					m_matchedIngredients[5] = 1;
					m_matchedIngredients[7] = 12 - num5;
					flag = true;
				}
				if (m_matchedIngredients2[1] >= 5 && (m_matchedIngredients2[2] >= 1 || m_matchedIngredients2[3] >= 1) && m_matchedIngredients2[4] >= 1 && m_matchedIngredients2[5] >= 1 && m_matchedIngredients2[0] + m_matchedIngredients2[6] <= 0)
				{
					int num6 = 10;
					m_matchedIngredients[1] = 5;
					m_matchedIngredients[2] = 1;
					m_matchedIngredients[3] = 1;
					m_matchedIngredients[4] = 1;
					m_matchedIngredients[5] = 1;
					m_matchedIngredients[9] = num6;
					m_matchedIngredients[7] = 0;
					flag = true;
				}
				if (m_matchedIngredients2[6] >= 6 && m_matchedIngredients2[3] >= 2 && m_matchedIngredients2[0] + m_matchedIngredients2[1] + m_matchedIngredients2[2] + m_matchedIngredients2[4] + m_matchedIngredients2[5] <= 0)
				{
					int num7 = m_random.UniformInt(2, 4);
					m_matchedIngredients[6] = 6;
					m_matchedIngredients[3] = 2;
					m_matchedIngredients[8] = num7;
					m_matchedIngredients[7] = 6 - num7;
					flag = true;
				}
			}
			if (m_matchedIngredients[8] >= 1 && (BlocksManager.Blocks[m_slots[ResultSlotIndex].Value].CraftingId != "steelingot" || m_slots[ResultSlotIndex].Count + m_matchedIngredients[8] > 40) && m_slots[ResultSlotIndex].Count != 0)
			{
				flag = false;
			}
			if (m_matchedIngredients[7] >= 1 && (BlocksManager.Blocks[m_slots[RemainsSlotIndex].Value].CraftingId != "scrapIron" || m_slots[RemainsSlotIndex].Count + m_matchedIngredients[7] > 40) && m_slots[RemainsSlotIndex].Count != 0)
			{
				flag = false;
			}
			if (m_matchedIngredients[9] >= 1 && (BlocksManager.Blocks[m_slots[ResultSlotIndex].Value].CraftingId != "ironingot" || m_slots[ResultSlotIndex].Count + m_matchedIngredients[9] > 40) && m_slots[ResultSlotIndex].Count != 0)
			{
				flag = false;
			}
			return flag;
		}

		// Token: 0x04001971 RID: 6513
		private ComponentBlockEntity m_componentBlockEntity;

		// Token: 0x04001972 RID: 6514
		private float m_fireTimeRemaining;

		// Token: 0x04001973 RID: 6515
		private int m_furnaceSize;

		// Token: 0x04001974 RID: 6516
		private SubsystemExplosions m_subsystemExplosions;

		// Token: 0x04001975 RID: 6517
		private SubsystemTerrain m_subsystemTerrain;

		// Token: 0x04001976 RID: 6518
		private bool m_updateSmeltingRecipe;

		// Token: 0x04001977 RID: 6519
		private int m_time;

		// Token: 0x04001978 RID: 6520
		private readonly int[] m_matchedIngredients2 = new int[9];

		// Token: 0x04001979 RID: 6521
		private readonly Game.Random m_random = new Game.Random();

		// Token: 0x0400197A RID: 6522
		private readonly int[] m_matchedIngredients = new int[10];

		// Token: 0x0400197B RID: 6523
		private bool m_smeltingRecipe;

		// Token: 0x0400197C RID: 6524
		private bool m_smeltingRecipe2;

		// Token: 0x0400197D RID: 6525
		private SubsystemTime m_subsystemTime;

		// Token: 0x0400197E RID: 6526
		private readonly int[] result = new int[3];
	}
}
