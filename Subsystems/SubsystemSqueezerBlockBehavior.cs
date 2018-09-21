namespace Game
{
	public class SubsystemSqueezerBlockBehavior : SubsystemInventoryBlockBehavior<ComponentSqueezer>
	{
		public SubsystemSqueezerBlockBehavior() : base("Squeezer")
		{
		}

		public override int[] HandledBlocks
		{
			get
			{
				return new[] { SqueezerBlock.Index };
			}
		}
		public override Widget GetWidget(IInventory inventory, ComponentSqueezer component)
		{
			return new SqueezerWidget(inventory, component);
		}
	}
}
