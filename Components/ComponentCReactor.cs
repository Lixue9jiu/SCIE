using Chemistry;
using Engine;
using GameEntitySystem;
using System.Collections.Generic;
using TemplatesDatabase;

namespace Game
{
	public class ComponentCReactor : ComponentMachine, IUpdateable
	{
		protected float m_speed;

		protected int m_smeltingRecipe,
						m_smeltingRecipe2;

		//protected int m_music;

		protected readonly Dictionary<int, int> result = new Dictionary<int, int>();
		protected ReactionSystem system;
		public override int RemainsSlotIndex => -1;

		public override int ResultSlotIndex => SlotsCount - 1;

		public override int FuelSlotIndex => -1;

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
				m_smeltingRecipe2 = FindSmeltingRecipe(1600f);
				if (m_smeltingRecipe2 != m_smeltingRecipe)
				{
					m_smeltingRecipe = m_smeltingRecipe2;
					SmeltingProgress = 0f;
					//m_music = 0;
				}
			}
			int i;
			if (m_smeltingRecipe2 != 0)
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
					m_smeltingRecipe = 0;
				if (num == 1 && m_smeltingRecipe == 0)
					m_smeltingRecipe = m_smeltingRecipe2;
			}
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
				SmeltingProgress = MathUtils.Min(SmeltingProgress + m_speed * dt, 1f);
				if (SmeltingProgress >= 1f)
				{
					var e = result.GetEnumerator();
					m_smeltingRecipe = 0;
					SmeltingProgress = 0f;
					m_updateSmeltingRecipe = true;
					while (e.MoveNext())
					{
						Slot slot = m_slots[FindAcquireSlotForItem(this, e.Current.Key, e.Current.Value)];
						slot.Value = e.Current.Key;
						slot.Count += e.Current.Value;
					}
				}
			}
		}

		public static int FindAcquireSlotForItem(IInventory inventory, int value, int count)
		{
			if (count > 0)
			{
				for (int i = 0; i < inventory.SlotsCount; i++)
				{
					if (inventory.GetSlotCount(i) > 0 && inventory.GetSlotValue(i) == value && inventory.GetSlotCount(i) < inventory.GetSlotCapacity(i, value))
					{
						return i;
					}
				}
				for (int j = 0; j < inventory.SlotsCount; j++)
				{
					if (inventory.GetSlotCount(j) == 0 && inventory.GetSlotCapacity(j, value) > 0)
					{
						return j;
					}
				}
			}
			else
			{
				for (int i = 0; i < inventory.SlotsCount; i++)
				{
					if (inventory.GetSlotCount(i) > 0 && inventory.GetSlotValue(i) == value)
					{
						return i;
					}
				}
			}
			return -1;
		}

		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			base.Load(valuesDictionary, idToEntityMap);
			m_furnaceSize = SlotsCount - 1;
		}

		protected int FindSmeltingRecipe(float heatLevel)
		{
			result.Clear();
			int n = 0;
			for (int i = 0; i < m_furnaceSize; i++)
			{
				if (GetSlotCount(i) > 0)
				{
					int value = GetSlotValue(i);
					if (value == ItemBlock.IdTable["S"])
						n |= 1;
					else if (value == ItemBlock.IdTable["H2O"])
						n |= 2;
					else if (value == ItemBlock.IdTable["Cl2"])
						n |= 4;
					else if (value == ItemBlock.IdTable["C7H8"])
						n |= 8;
					else if (value == ItemBlock.IdTable["HNO3"])
						n |= 16;
				}
			}
			if (n == 3)
			{
				m_speed = 0.1f;
				result[ItemBlock.IdTable["H2SO4"]] = 1;
				//result[ItemBlock.IdTable["S-HCl"]] = 2;
				//result[ItemBlock.IdTable["钢瓶"]] = 1;
				result[ItemBlock.IdTable["S"]] = -1;
				result[ItemBlock.IdTable["H2O"]] = -1;
				//result[ItemBlock.IdTable["Cl2"]] = -1;
			}
			else if (n == 6)
			{
				m_speed = 0.1f;
				//result[ItemBlock.IdTable["H2SO4"]] = 1;
				result[ItemBlock.IdTable["S-HCl"]] = 1;
				result[ItemBlock.IdTable["钢瓶"]] = 1;
				//result[ItemBlock.IdTable["S"]] = -1;
				result[ItemBlock.IdTable["H2O"]] = -1;
				result[ItemBlock.IdTable["Cl2"]] = -1;
			}
			else if (n == (4 | 8 | 16))
			{
				m_speed = 0.05f;
				result[ItemBlock.IdTable["钢瓶"]] = 1;
				result[ItemBlock.IdTable["C7H8"]] = -1;
				result[ItemBlock.IdTable["Cl2"]] = -1;
				result[ItemBlock.IdTable["HNO3"]] = -1;
				result[ItemBlock.IdTable["Bottle"]] = 2;
				result[ItemBlock.IdTable["TNT"]] = 1;
			}
			else
			{
				m_speed = 0.2f;
				system = new ReactionSystem();
				n = FindSmeltingRecipe(result, system, Condition.l, (ushort)heatLevel);
				if (n == 0)
					return 0;
				var e = result.GetEnumerator();
				while (e.MoveNext())
				{
					int index = FindAcquireSlotForItem(this, e.Current.Key);
					if (index < 0 || GetSlotCount(index) + e.Current.Value < 0)
						return 0;
				}
				return n;
			}
			return FindSmeltingRecipe(result, n);
		}
	}
}