using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
    public class ComponentSour : ComponentMachine, IUpdateable
    {

        protected readonly int[] result = new int[3];

        protected string m_smeltingRecipe;

        //protected int m_music;

        protected string m_smeltingRecipe2;

        public override int RemainsSlotIndex => -1;

        public override int ResultSlotIndex => SlotsCount - 1;

        public override int FuelSlotIndex => -1;

        public int UpdateOrder => 0;

        public void Update(float dt)
        {
           
            if (m_updateSmeltingRecipe)
            {
                m_updateSmeltingRecipe = false;
                m_smeltingRecipe2 = FindSmeltingRecipe();
                if (m_smeltingRecipe2 != m_smeltingRecipe)
                {
                    m_smeltingRecipe = m_smeltingRecipe2;
                    SmeltingProgress = 0f;
                    if (m_fireTimeRemaining == 0f)
                    m_fireTimeRemaining = 1f; 
                    //m_music = 0;
                }
            }
            if (m_smeltingRecipe2 != null)
            {
               if (m_smeltingRecipe == null)
                    m_smeltingRecipe = m_smeltingRecipe2;
            }
            
            if (m_smeltingRecipe == null)
            {
                HeatLevel = 0f;
                
                //m_music = -1;
            }
            else
               
            
            if (m_smeltingRecipe != null)
            {
                m_fireTimeRemaining = MathUtils.Min(m_fireTimeRemaining - 0.001f * dt, 1f);
                SmeltingProgress = 1f - m_fireTimeRemaining;
                //SmeltingProgress = MathUtils.Min(SmeltingProgress + 0.001f * dt, 1f);
                //m_fireTimeRemaining = SmeltingProgress;
                if (SmeltingProgress >= 1f)
                {
                    if (m_slots[0].Count > 0)
                        m_slots[0].Count--;
                    for (int j = 0; j < 3; j++)
                    {
                        if (result[j] != 0)
                        {
                            int value = result[j];
                            m_slots[1 + j].Value = value;
                            m_slots[1 + j].Count++;
                            m_smeltingRecipe = null;
                            SmeltingProgress = 0f;
                            m_updateSmeltingRecipe = true;
                            m_fireTimeRemaining = 0f;
                        }
                    }
                }
            }
        }

        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
        {
            base.Load(valuesDictionary, idToEntityMap);
            m_furnaceSize = SlotsCount - 1;
            m_fireTimeRemaining = valuesDictionary.GetValue("FireTimeRemaining", 0f);
            HeatLevel = valuesDictionary.GetValue("HeatLevel", 0f);
        }

		public override int RemoveSlotItems(int slotIndex, int count)
		{
			m_updateSmeltingRecipe = true;
			if (m_smeltingRecipe != null && slotIndex!=1)
			{
				return 0;
			}
			return base.RemoveSlotItems(slotIndex, count);
		}

		protected string FindSmeltingRecipe()
        {
            bool text = false;
            result[0] = 0;
            result[1] = 0;
            result[2] = 0;
            int i;
            for (i = 0; i < m_furnaceSize; i++)
            {
                if (GetSlotCount(i) <= 0) continue;
                int value = GetSlotValue(i);
                if (value == RottenBirdBlock.Index || value == RottenBreadBlock.Index || value == RottenDoughBlock.Index || value == RottenFishBlock.Index || value == RottenEggBlock.Index || value == RottenMeatBlock.Index || value==RottenPumpkinBlock.Index)
                {
                    text = true;
                    result[0] = SaltpeterChunkBlock.Index;
                    result[1] = 0;
                    result[2] = 0;
                }
                if (value == RottenMilkBucketBlock.Index || value == RottenPumpkinSoupBucketBlock.Index)
                {
                    text = true;
                    result[0] = SaltpeterChunkBlock.Index;
                    result[1] = EmptyBucketBlock.Index;
                    result[2] = 0;
                }
            }
            if (!text)
                return null;
            for (i = 0; i < 3; i++)
            {
                Slot slot = m_slots[1 + i];
                if (slot.Count != 0 && result[i] != 0 && (slot.Value != result[i] || slot.Count >= 40))
                    return null;
            }
            return "AluminumOrePowder";
        }
    }
}
