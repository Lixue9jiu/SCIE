using Engine;
using GameEntitySystem;
using System.Globalization;
using TemplatesDatabase;

namespace Game
{
	public class ComponentSeperator : ComponentMachine, IUpdateable
	{

        public bool Powered;
        protected float m_fireTimeRemaining;

		protected int m_furnaceSize;

		protected readonly int[] m_matchedIngredients = new int[9];

        protected readonly int[] result = new int[3];

        protected string m_smeltingRecipe;

		protected SubsystemAudio m_subsystemAudio;

		//protected int m_music;

		protected string m_smeltingRecipe2;

		public int RemainsSlotIndex
		{
			get
			{
				return SlotsCount;
			}
		}

		public override int ResultSlotIndex
		{
			get
			{
				return SlotsCount - 1;
			}
		}

		public override int FuelSlotIndex
		{
			get
			{
				return SlotsCount;
			}
		}

		public int UpdateOrder
		{
			get
			{
				return 0;
			}
		}

		public void Update(float dt)
		{
			Point3 coordinates = m_componentBlockEntity.Coordinates;
			if (HeatLevel > 0f)
			{
				m_fireTimeRemaining = MathUtils.Max(0f, m_fireTimeRemaining - dt);
				if (m_fireTimeRemaining == 0f)
				{
					HeatLevel = 0f;
				}
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
                if (!Powered)
                {
                    SmeltingProgress = 0f;
                    HeatLevel = 0f;
                    m_smeltingRecipe = null;
                }
                else if (m_smeltingRecipe == null)
                {
                    m_smeltingRecipe = m_smeltingRecipe2;
                }
            }
            if (!Powered)
            {
                SmeltingProgress = 0f;
                HeatLevel = 0f;
                m_smeltingRecipe = null;
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
                   
                        if (m_slots[0].Count > 0)
                        {
                            m_slots[0].Count--;
                        }
                    for (int jk = 0; jk < 3; jk++)
                    {
                        if (result[jk] != 0)
                        {
                            int value = result[jk];
                            m_slots[1 + jk].Value = value;
                            m_slots[1 + jk].Count++;
                            m_smeltingRecipe = null;
                            SmeltingProgress = 0f;
                            m_updateSmeltingRecipe = true;
                        }
                    }
				}
			}
		}

		public override int GetSlotCapacity(int slotIndex, int value)
		{
			Block block = BlocksManager.Blocks[Terrain.ExtractContents(value)];
			return slotIndex != FuelSlotIndex || (block is IFuel fuel ? fuel.GetHeatLevel(value) : block.FuelHeatLevel) > 1f
				? base.GetSlotCapacity(slotIndex, value)
				: 0;
		}

		public override void AddSlotItems(int slotIndex, int value, int count)
		{
			base.AddSlotItems(slotIndex, value, count);
			m_updateSmeltingRecipe = true;
		}

		public override int RemoveSlotItems(int slotIndex, int count)
		{
			m_updateSmeltingRecipe = true;
			return base.RemoveSlotItems(slotIndex, count);
		}

		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			base.Load(valuesDictionary, idToEntityMap);
			m_subsystemAudio = Project.FindSubsystem<SubsystemAudio>(true);
			m_furnaceSize = SlotsCount - 1;
			m_fireTimeRemaining = valuesDictionary.GetValue<float>("FireTimeRemaining");
			HeatLevel = valuesDictionary.GetValue<float>("HeatLevel");
			m_updateSmeltingRecipe = true;
		}

		public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
		{
			base.Save(valuesDictionary, entityToIdMap);
			valuesDictionary.SetValue("FireTimeRemaining", m_fireTimeRemaining);
			valuesDictionary.SetValue("HeatLevel", HeatLevel);
		}

		protected string FindSmeltingRecipe(float heatLevel)
		{
            string text = null;
            result[0] = 0;
            result[1] = 0;
            result[2] = 0;
            for (int i = 0; i < m_furnaceSize; i++)
			{
				int slotValue = GetSlotValue(i);
				int num = Terrain.ExtractContents(slotValue);
				int num2 = Terrain.ExtractData(slotValue);
				if (GetSlotCount(i) > 0)
				{
                    if (slotValue == DirtBlock.Index)
                    {
                        text = "success";
                        result[0] = SandBlock.Index;
                        result[1] = StoneChunkBlock.Index;
                        int num3 = m_random.UniformInt(0,4);
                        if (num3==0)
                        {
                            result[2] = SaltpeterChunkBlock.Index;
                        }
                        if (num3==1)
                        {
                            result[2] = ItemBlock.IdTable["AluminumOrePowder"];
                        }

                    }
                }
				else
				{

				}
			}
            if (text != null)
            {
                for (int ik = 0; ik < 3; ik++)
                { 
                Slot slot = m_slots[1 + ik];
                if (slot.Count != 0 && result[ik] != 0 && (slot.Value != result[ik] || 1 + slot.Count > 40))
                {
                    text = null;
                }
               }
			}
			return text;
		}
	}
}
