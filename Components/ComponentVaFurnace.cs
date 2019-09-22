using Chemistry;
using Engine;

namespace Game
{
	public class ComponentVaFurnace : ComponentSeparator, IUpdateable
	{
		protected float speed;

		public override int RemainsSlotIndex => SlotsCount - 3;

		public override int ResultSlotIndex => SlotsCount - 4;

		public bool all;

		protected ReactionSystem system;

		public new void Update(float dt)
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
			if (Utils.SubsystemTime.PeriodicGameTimeEvent(0.2, 0.0) && m_smeltingRecipe2 != 0)
			{
				int num = 0;
				for (int i = -1; i < 2; i++)
				{
					for (int j = -1; j < 2; j++)
					{
						for (int k = -1; k < 2; k++)
						{
							Point3 coordinates = m_componentBlockEntity.Coordinates;
							int cellValue = Utils.Terrain.GetCellValue(coordinates.X + i, coordinates.Y + j, coordinates.Z + k);
							if (i * i + j * j + k * k <= 1 && ElementBlock.Block.GetDevice(coordinates.X + i, coordinates.Y + j, coordinates.Z + k, cellValue) is AirPump em && em.Powered)
							{
								num = 1;
								break;
							}
						}
					}
				}
				if (num == 0 || !Powered)
				{
					SmeltingProgress = 0f;
					HeatLevel = 0f;
					m_smeltingRecipe = 0;
				}
				else if (m_smeltingRecipe == 0)
					m_smeltingRecipe = m_smeltingRecipe2;
			}
			if (m_smeltingRecipe == 0)
			{
				HeatLevel = 0f;
				m_fireTimeRemaining = 0f;
				//m_music = -1;
			}
			else
				m_fireTimeRemaining = 1f;
			if (m_fireTimeRemaining <= 0f)
			{
				m_smeltingRecipe = 0;
				SmeltingProgress = 0f;
				//m_music = -1;
			}
			if (m_smeltingRecipe != 0)
			{
				SmeltingProgress = MathUtils.Min(SmeltingProgress + speed * dt, 1f);
				if (SmeltingProgress >= 1f)
				{
					var e = result.GetEnumerator();
					if (all)
					for (int i = 0; i < 4; i++)
						if (m_slots[i].Count > 0)
							m_slots[i].Count--;
					while (e.MoveNext())
					{
						Slot slot = m_slots[ComponentCReactor.FindAcquireSlotForItem(this, e.Current.Key,e.Current.Value)];
						slot.Value = e.Current.Key;
						slot.Count += e.Current.Value;
					}
					m_smeltingRecipe = 0;
					SmeltingProgress = 0f;
					m_updateSmeltingRecipe = true;
				}
			}
		}

		protected override int FindSmeltingRecipe()
		{
			speed = 0.33f;
			result.Clear();
			int n = 0;
			all = false;
			for (int i = 0; i < m_furnaceSize; i++)
			{
				if (GetSlotCount(i) > 0)
				{
					int value = GetSlotValue(i);
					if (value == ItemBlock.IdTable["Si"])
						n += 1;
					else if (value == ItemBlock.IdTable["H2"])
						n += 5;
					else if (value == ItemBlock.IdTable["Cl2"])
						n += 6;
				}
			}
			if (n == 12)
			{
				speed = 0.2f;
				n = m_random.Int() & 7;
				if (n == 1)
				{
					result[ItemBlock.IdTable["µ¥¾§¹è"]] = 1;
				}
				
					result[ItemBlock.IdTable["¶à¾§¹è"]] = 1;
				
					
				result[ItemBlock.IdTable["HCl"]] = 2;
				result[ItemBlock.IdTable["H2"]] = -1;
				result[ItemBlock.IdTable["Si"]] = -1;
				result[ItemBlock.IdTable["Cl2"]] = -1;
				return FindSmeltingRecipe(result, 2);
			}
			else if (n == 4)
			{
				result[ItemBlock.IdTable["¶à¾§¹è"]] = 1;
				all = true;
				//result[ItemBlock.IdTable["Si"]] = -1;
				//result[ItemBlock.IdTable["Si"]] = -1;
				//result[ItemBlock.IdTable["Si"]] = -1;
				//result[ItemBlock.IdTable["Si"]] = -1;
				return FindSmeltingRecipe(result, 3);
			}
			system = new ReactionSystem();
			return FindSmeltingRecipe(result, FindSmeltingRecipe(result, system));
		}
	}
}