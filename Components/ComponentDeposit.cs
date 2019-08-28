using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	public class ComponentDeposit : ComponentSeparator
	{
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			base.Load(valuesDictionary, idToEntityMap);
			Powered = true;
		}

		protected override int FindSmeltingRecipe()
		{
			result.Clear();
			int text = 0;
			int i;
			if (GetSlotValue(0) == ItemBlock.IdTable["CrudeSalt"] && GetSlotValue(4) == CanvasBlock.Index && GetSlotValue(5) == ItemBlock.IdTable["Bottle"])
			{
				text = 1;
				result[ItemBlock.IdTable["S-NaCl"]] = 1;
				result[ItemBlock.IdTable["Bottle"]] = -1;
			}
			return FindSmeltingRecipe(text);
		}
	}
}