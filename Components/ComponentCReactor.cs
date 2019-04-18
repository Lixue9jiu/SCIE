using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	public class ComponentCReactor : ComponentMachine, IUpdateable
	{
		//protected readonly int[] m_matchedIngredients = new int[9];

		protected string m_smeltingRecipe;

		//protected int m_music;

		protected string m_smeltingRecipe2;

		public override int RemainsSlotIndex => SlotsCount;

		public override int ResultSlotIndex => SlotsCount - 1;

		public override int FuelSlotIndex => SlotsCount;

		public int UpdateOrder => 0;

		public void Update(float dt)
		{
			Point3 coordinates = m_componentBlockEntity.Coordinates;
			if (HeatLevel > 0f)
			{
				m_fireTimeRemaining = MathUtils.Max(0f, m_fireTimeRemaining - dt);
				if (m_fireTimeRemaining == 0f)
					HeatLevel = 0f;
			}
			if (m_updateSmeltingRecipe)
			{
				m_updateSmeltingRecipe = false;
				string text = m_smeltingRecipe2 = FindSmeltingRecipe(1f);
				if (text != m_smeltingRecipe)
				{
					m_smeltingRecipe = text;
					m_smeltingRecipe2 = text;
					SmeltingProgress = 0f;
					//m_music = 0;
				}
			}
			if (m_smeltingRecipe2 != null)
			{
				int num = 0;
				for (int i = -1; i < 2; i++)
				{
					for (int j = -1; j < 2; j++)
					{
						for (int k = -1; k < 2; k++)
						{
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
					m_smeltingRecipe = null;
				if (num == 1 && m_smeltingRecipe == null)
					m_smeltingRecipe = m_smeltingRecipe2;
			}
			if (m_smeltingRecipe == null)
			{
				HeatLevel = 0f;
				m_fireTimeRemaining = 0f;
				//m_music = -1;
			}
			if (m_smeltingRecipe != null)
			{
				m_fireTimeRemaining = 0f;
				float fireTimeRemaining = m_fireTimeRemaining;
				m_fireTimeRemaining = 100f;
			}
			if (m_fireTimeRemaining <= 0f)
			{
				m_smeltingRecipe = null;
				SmeltingProgress = 0f;
				//m_music = -1;
			}
			if (m_smeltingRecipe != null)
			{
				SmeltingProgress = MathUtils.Min(SmeltingProgress + 0.1f * dt, 1f);
				if (SmeltingProgress >= 1f)
				{
					for (int i = 0; i < 3; i++)
						if (m_slots[i].Count > 0)
							m_slots[i].Count--;
					int value = ItemBlock.IdTable[m_smeltingRecipe];
					m_slots[ResultSlotIndex].Value = value;
					m_slots[ResultSlotIndex].Count++;
					m_smeltingRecipe = null;
					SmeltingProgress = 0f;
					m_updateSmeltingRecipe = true;
				}
			}
		}

		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			base.Load(valuesDictionary, idToEntityMap);
			m_furnaceSize = SlotsCount - 1;
			m_fireTimeRemaining = valuesDictionary.GetValue<float>("FireTimeRemaining");
			HeatLevel = valuesDictionary.GetValue<float>("HeatLevel");
		}

		public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
		{
			this.SaveItems(valuesDictionary);
			valuesDictionary.SetValue("FireTimeRemaining", m_fireTimeRemaining);
			valuesDictionary.SetValue("HeatLevel", HeatLevel);
		}

		protected string FindSmeltingRecipe(float heatLevel)
		{
			string text = null;
			int n = 0;
			for (int i = 0; i < m_furnaceSize; i++)
			{
				int slotValue = GetSlotValue(i);
				int num = Terrain.ExtractContents(slotValue);
				//int num2 = Terrain.ExtractData(slotValue);
				if (GetSlotCount(i) > 0)
				{
					if (num == TankBlock.Index)
						n |= 1;
					else if (slotValue == ItemBlock.IdTable["ZincRod"])
						n |= 2;
					else if (num == SulphurChunkBlock.Index)
						n |= 4;
				}
				/*else
				{
				}*/
			}
			if (n == 7)
			{
				text = "CuZnBattery";
			//}
			//if (text != null)
			//{
				Slot slot = m_slots[ResultSlotIndex];
				//int num3 = Terrain.ExtractContents(GetSlotValue(1));
				if (slot.Count != 0 && (slot.Value != ItemBlock.IdTable[text] || slot.Count >= 40))
					text = null;
			}
			return text;
		}
	}
}