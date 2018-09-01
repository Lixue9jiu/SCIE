namespace Game
{
	public class SubsystemCReactorBlockBehavior : SubsystemInventoryBlockBehavior<ComponentCReactor>
	{
		public SubsystemCReactorBlockBehavior() : base("CReactor")
		{
		}

		public override int[] HandledBlocks
		{
			get
			{
				return new []
				{
					CReactorBlock.Index
				};
			}
		}
		public override Widget GetWidget(IInventory inventory, ComponentCReactor component)
		{
			return new CReactorWidget(inventory, component);
		}
	}
}
