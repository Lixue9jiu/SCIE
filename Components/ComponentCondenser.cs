using Engine;
using GameEntitySystem;
//using System.Globalization;
using TemplatesDatabase;
//using System;
namespace Game
{
	public class ComponentCondenser : ComponentInventoryBase
	{
		//public int SlotIndex { get; set; }
		public bool Powered;
		public bool Charged;
		public float m_fireTimeRemaining;
		
		//public static Func<int, int, int> DamageItem1;
		

		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			
			
			this.LoadItems(valuesDictionary);
		
			m_fireTimeRemaining = valuesDictionary.GetValue("FireTimeRemaining", 0f);
			
		}

		public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
		{
			this.SaveItems(valuesDictionary);
			valuesDictionary.SetValue("FireTimeRemaining", m_fireTimeRemaining);
		}

		
	}
}

