using Chemistry;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	public class ComponentCReactor : ComponentMachine, IUpdateable
	{
		protected int m_smeltingRecipe,
						m_smeltingRecipe2;

		//protected int m_music;

		protected readonly int[] result = new int[3];
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
				m_smeltingRecipe2 = FindSmeltingRecipe(0f);
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
					for (i = 0; i < 3; i++)
						if (m_slots[i].Count > 0)
							m_slots[i].Count--;
					for (int j = 0; j < 3; j++)
					{
						if (result[j] != 0)
						{
							int value = result[j];
							m_slots[3 + j].Value = value;
							m_slots[3 + j].Count++;
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

		protected int FindSmeltingRecipe(float heatLevel)
		{
			string text = null;
			int n = 0;
			for (int i = 0; i < m_furnaceSize; i++)
			{
				int value = GetSlotValue(i);
				if (GetSlotCount(i) > 0)
				{
					if (value == SulphurChunkBlock.Index)
						n++;
					else if (value == WaterBucketBlock.Index)
						n += 10;
					else if (value == ItemBlock.IdTable["Bottle"])
						n += 100;
				}
			}
			if (n == 111)
			{
				text = "H2SO4";
				result[0] = ItemBlock.IdTable[text];
				result[1] = EmptyBucketBlock.Index;
			}
			for (int i = 0; i < 3; i++)
			{
				Slot slot = m_slots[3 + i];
				if (slot.Count != 0 && result[i] != 0 && (slot.Value != result[i] || slot.Count >= 40))
					return 0;
			}
			if (text == null)
				return 0;
			return ItemBlock.IdTable[text];
		}

		public static ReactionSystem Get(int value)
		{
			int c = Terrain.ExtractContents(value);
			switch (c)
			{
				default:
					return null;
			}
		}
	}
}