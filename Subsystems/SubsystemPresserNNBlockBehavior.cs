namespace Game
{
	public class SubsystemPresserNNBlockBehavior : SubsystemInventoryBlockBehavior<ComponentPresserNN>
	{
		public SubsystemPresserNNBlockBehavior() : base("PresserNN")
		{
		}
		public override int[] HandledBlocks
		{
			get
			{
				return new[]
				{
					PresserNNBlock.Index
				};
			}
		}

		public override Widget GetWidget(IInventory inventory, ComponentPresserNN component)
		{
			return new PresserWidget<ComponentPresserNN>(inventory, component, "Widgets/PresserNNWidget");
		}
	}
}
