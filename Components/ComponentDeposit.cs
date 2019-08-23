namespace Game
{
	public class ComponentDeposit : ComponentSeparator
	{
		protected override int FindSmeltingRecipe()
		{
			result.Clear();
			if (GetSlotCount(0) <= 0 || GetSlotValue(0) != ItemBlock.IdTable["QuartzPowder"])
			{
				return 0;
			}
			int text = 0;
			int i;
			for (i = 4; i < 6; i++)
			{
				if (GetSlotCount(i) <= 0) continue;
				if (GetSlotValue(i) == ItemBlock.IdTable["C2H5OH"])
					text = -text;
				if (GetSlotValue(i) == ItemBlock.IdTable["C2H5OH"])
					text = -text;
			}
			result[ItemBlock.IdTable["C2H5OH"]] = -1;
			return FindSmeltingRecipe(text);
		}
	}
}