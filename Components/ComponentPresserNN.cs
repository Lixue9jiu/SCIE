using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	public class ComponentPresserNN : ComponentPMach
	{
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			base.Load(valuesDictionary, idToEntityMap);
			m_speed = 0.01f;
		}

		protected override string FindSmeltingRecipe()
		{
			string text = null;
			for (int i = 0; i < m_furnaceSize; i++)
				if (GetSlotCount(i) > 0 && GetSlotValue(i) == ItemBlock.IdTable["SteelRod"])
					text = "RifleBarrel";
			if (text != null)
			{
				Slot slot = m_slots[ResultSlotIndex];
				if (slot.Count != 0 && (slot.Value != ItemBlock.IdTable[text] || slot.Count >= 40))
					return null;
			}
			return text;
		}
	}
}