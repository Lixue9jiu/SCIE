namespace Game
{
	public class SubsystemFireBoxBlockBehavior : SubsystemFurnaceBlockBehavior<ComponentFireBox>
	{
		public override int[] HandledBlocks => new[] { FireBoxBlock.Index };

		public SubsystemFireBoxBlockBehavior() : base("FireBox")
		{
		}

		public override Widget GetWidget(IInventory inventory, ComponentFireBox component)
		{
			return new FireBoxWidget<ComponentFireBox>(inventory, component, "Widgets/FireBoxWidget");
		}
	}
}