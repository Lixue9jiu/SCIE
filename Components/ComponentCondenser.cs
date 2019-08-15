using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	public class ComponentCondenser : Component
	{
		public bool Powered, Charged;
		public float m_fireTimeRemaining;

		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			m_fireTimeRemaining = valuesDictionary.GetValue("FireTimeRemaining", 0f);
			Charged = valuesDictionary.GetValue("HeatLevel", 0f) == 0f ? true : false;
		}

		public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
		{
			valuesDictionary.SetValue("FireTimeRemaining", m_fireTimeRemaining);
			valuesDictionary.SetValue("HeatLevel", Charged ? 0f : 1f);
		}
	}
}