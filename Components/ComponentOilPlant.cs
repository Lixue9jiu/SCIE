using Engine;

namespace Game
{
	public class ComponentOilPlant : ComponentSeparator, IUpdateable
	{
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
				m_smeltingRecipe2 = FindSmeltingRecipe(FindSmeltingRecipe());
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
				SmeltingProgress = MathUtils.Min(SmeltingProgress + 0.1f * dt, 1f);
				if (SmeltingProgress >= 1f)
				{
					var e = result.GetEnumerator();
					while (e.MoveNext())
					{
						Slot slot = m_slots[FindAcquireSlotForItem(this, e.Current.Key)];
						slot.Value = e.Current.Key;
						slot.Count += e.Current.Value;
						m_smeltingRecipe = 0;
						SmeltingProgress = 0f;
						m_updateSmeltingRecipe = true;
					}
					for (i = 0; i < 3; i++)
						if (m_slots[i].Count > 0)
							m_slots[i].Count--;
				}
			}
		}

		protected override int FindSmeltingRecipe()
		{
			int text = 0;
			result.Clear();
			if (GetSlotValue(0) == 786672 && GetSlotCount(0) > 0 && GetSlotValue(1) == ItemBlock.IdTable["CokeCoalPowder"] && GetSlotCount(1) > 0 && Terrain.ExtractContents(GetSlotValue(2)) == OakWoodBlock.Index)
			{
				text = 1;
				result[ItemBlock.IdTable["Rubber"]] = 1;
			}
			return text;
		}
	}
}