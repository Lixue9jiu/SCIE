using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	public class ComponentICEngine : ComponentMachine, IUpdateable
	{
		protected string m_smeltingRecipe;

		protected int m_music;

		public Point3 Coordinates;

		public override int RemainsSlotIndex => SlotsCount - 3;
		public override int ResultSlotIndex => SlotsCount - 1;
		public override int FuelSlotIndex => SlotsCount - 2;

		public int UpdateOrder => 0;

		public void Update(float dt)
		{
			
		}

		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			base.Load(valuesDictionary, idToEntityMap);
			if (m_componentBlockEntity != null)
				Coordinates = m_componentBlockEntity.Coordinates;
			m_furnaceSize = SlotsCount - 1;
			m_fireTimeRemaining = valuesDictionary.GetValue("FireTimeRemaining", 0f);
			HeatLevel = valuesDictionary.GetValue("HeatLevel", 0f);
		}

		

		
	}
}