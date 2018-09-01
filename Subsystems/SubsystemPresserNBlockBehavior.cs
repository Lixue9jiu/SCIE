namespace Game
{
	public class SubsystemPresserNBlockBehavior : SubsystemInventoryBlockBehavior<ComponentPresserN>
	{
		public SubsystemPresserNBlockBehavior() : base("PresserN")
		{
		}
		public override int[] HandledBlocks
		{
			get
			{
				return new[]
				{
					KibblerBlock.Index
				};
			}
		}

		public override Widget GetWidget(IInventory inventory, ComponentPresserN component)
		{
			return new PresserWidget<ComponentPresserN>(inventory, component, "Widgets/PresserNWidget");
		}
	}
}
