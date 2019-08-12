using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	public class ComponentDeposit : ComponentSour
	{
		public override int RemainsSlotIndex => SlotsCount - 1;

		public override int ResultSlotIndex => SlotsCount - 2;

		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			base.Load(valuesDictionary, idToEntityMap);
			m_furnaceSize = SlotsCount - 2;
		}
	}
}