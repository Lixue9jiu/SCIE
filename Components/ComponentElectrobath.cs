using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	public class ComponentElectrobath : ComponentSMachine, IUpdateable
	{
		public bool Powered;

		protected readonly int[] result = new int[3];

		protected int m_smeltingRecipe, m_smeltingRecipe2;

		//protected int m_music;

		public override int ResultSlotIndex => SlotsCount - 1;

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
				m_smeltingRecipe2 = FindSmeltingRecipe();
				if (m_smeltingRecipe2 != m_smeltingRecipe)
				{
					m_smeltingRecipe = m_smeltingRecipe2;
					SmeltingProgress = 0f;
					//m_music = 0;
				}
			}
			if (m_smeltingRecipe2 != 0)
			{
				if (!Powered)
				{
					SmeltingProgress = 0f;
					HeatLevel = 0f;
					m_smeltingRecipe = 0;
				}
				else if (m_smeltingRecipe == 0)
					m_smeltingRecipe = m_smeltingRecipe2;
			}
			if (!Powered)
				m_smeltingRecipe = 0;
			if (m_smeltingRecipe == 0)
			{
				HeatLevel = 0f;
				m_fireTimeRemaining = 0f;
				//m_music = -1;
			}
			else
				m_fireTimeRemaining = 100f;
			if (m_fireTimeRemaining <= 0f)
			{
				m_smeltingRecipe = 0;
				SmeltingProgress = 0f;
				//m_music = -1;
			}
			if (m_smeltingRecipe != 0)
			{
				SmeltingProgress = MathUtils.Min(SmeltingProgress + 0.1f * dt, 1f);
				if (SmeltingProgress >= 1f)
				{
					if (m_slots[0].Value == ItemBlock.IdTable["H2O"])
					{
						Point3 coordinates = m_componentBlockEntity.Coordinates;
						int num31 = coordinates.X;
						int num41 = coordinates.Y;
						int num51 = coordinates.Z;
						int cellValue1 = Terrain.ReplaceLight(Utils.Terrain.GetCellValue(num31, num41 + 1, num51), 0);
						if (ElementBlock.Block.GetDevice(num31, num41 + 1, num51, cellValue1) is AirPresser em && em.Powered)
						{
							var point3 = new Point3(num31, num41 + 1, num51);
							var entity = Utils.GetBlockEntity(point3);
							Component component3 = entity.Entity.FindComponent<ComponentAirPresser>();
							if (entity != null && component3 != null)
							{
								IInventory inventory = entity.Entity.FindComponent<ComponentAirPresser>(true);
								for (int i = 0; i < 6; i++)
								{
									//int value23 = 0;
									int va1 = inventory.GetSlotValue(i);
									if (Utils.Random.Bool(0.660f))
									{
										if (va1 == ItemBlock.IdTable["멀틸"] && AcquireItems(inventory, ItemBlock.IdTable["H2"], 1) == 0)
										{
											inventory.RemoveSlotItems(i, 1);
											i += 6;
										}
									}
									else
									{
										if (va1 == ItemBlock.IdTable["멀틸"] && AcquireItems(inventory, ItemBlock.IdTable["O2"], 1) == 0)
										{
											inventory.RemoveSlotItems(i, 1);
											i += 6;
										}
									}
								}
							}
						}
					}
					if (m_slots[0].Value == ItemBlock.IdTable["S-NaCl"])
					{
						Point3 coordinates = m_componentBlockEntity.Coordinates;
						int num31 = coordinates.X;
						int num41 = coordinates.Y;
						int num51 = coordinates.Z;
						int cellValue1 = Terrain.ReplaceLight(Utils.Terrain.GetCellValue(num31, num41 + 1, num51), 0);
						if (ElementBlock.Block.GetDevice(num31, num41 + 1, num51, cellValue1) is AirPresser em && em.Powered)
						{
							var point3 = new Point3(num31, num41 + 1, num51);
							var entity = Utils.GetBlockEntity(point3);
							Component component3 = entity.Entity.FindComponent<ComponentAirPresser>();
							if (entity != null && component3 != null)
							{
								IInventory inventory = entity.Entity.FindComponent<ComponentAirPresser>(true);
								for (int i = 0; i < 6; i++)
								{
									//int value23 = 0;
									int va1 = inventory.GetSlotValue(i);
									if (Utils.Random.Bool(0.5f))
									{
										if (va1 == ItemBlock.IdTable["멀틸"] && AcquireItems(inventory, ItemBlock.IdTable["H2"], 1) == 0)
										{
											inventory.RemoveSlotItems(i, 1);
											i += 6;
										}
									}
									else
									{
										if (va1 == ItemBlock.IdTable["멀틸"] && AcquireItems(inventory, ItemBlock.IdTable["Cl2"], 1) == 0)
										{
											inventory.RemoveSlotItems(i, 1);
											i += 6;
										}
									}
								}
							}
						}
					}
					if (m_slots[0].Value == SaltpeterChunkBlock.Index)
						m_slots[4].Count--;
					if (m_slots[0].Count > 0)
						m_slots[0].Count--;

					for (int j = 0; j < 3; j++)
					{
						if (result[j] != 0)
						{
							int value = result[j];
							m_slots[1 + j].Value = value;
							m_slots[1 + j].Count++;
							m_smeltingRecipe = 0;
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
		}

		protected int FindSmeltingRecipe()
		{
			result[0] = 0;
			result[1] = 0;
			result[2] = 0;
			int text = 0;
			int i;
			for (i = 0; i < m_furnaceSize; i++)
			{
				if (GetSlotCount(i) <= 0) continue;
				int value = GetSlotValue(i);
				bool v = i == 0;
				if (value == ItemBlock.IdTable["AluminumOrePowder"] && v)
				{
					text |= 1;
				}
				if (value == ItemBlock.IdTable["Cryolite"])
				{
					text |= 2;
				}
				if (value == ItemBlock.IdTable["H2O"] && v)
				{
					text = 8;
					result[0] = ItemBlock.IdTable["Bottle"];
					break;
				}
				if (value == ItemBlock.IdTable["S-NaCl"] && v)
				{
					text = 9;
					result[0] = ItemBlock.IdTable["S-NaOH"];
					break;
				}
				if (GetSlotValue(0) == SaltpeterChunkBlock.Index && GetSlotValue(4) == ItemBlock.IdTable["Bottle"])
				{
					text = 10;
					result[0] = ItemBlock.IdTable["HNO3"];
					break;
				}
			}
			if (text == 3)
			{
				result[0] =
				result[1] = ItemBlock.IdTable["AluminumIngot"];
			}
			else if (text == 1)
				result[0] = ItemBlock.IdTable["AluminumIngot"];
			else
				text &= ~3;
			if (text != 0)
			{
				for (i = 0; i < 3; i++)
				{
					Slot slot = m_slots[1 + i];
					if (slot.Count != 0 && result[i] != 0 && (slot.Value != result[i] || slot.Count >= 40))
						return 0;
				}
			}
			return text;
		}
	}

	public class ComponentAirPresser : ComponentSMachine
	{
		public bool Powered;

		public override int ResultSlotIndex => SlotsCount - 1;
	}
}