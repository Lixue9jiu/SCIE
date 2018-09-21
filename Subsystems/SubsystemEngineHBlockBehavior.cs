namespace Game
{
	public class SubsystemEngineHBlockBehavior : SubsystemFurnaceBlockBehavior<ComponentEngineH>
	{
		public override int[] HandledBlocks
		{
			get
			{
				return new[] { EngineHBlock.Index };
			}
		}

		public SubsystemEngineHBlockBehavior() : base("HeatEngine")
		{
		}

		public override Widget GetWidget(IInventory inventory, ComponentEngineH component)
		{
			return new EngineHWidget(inventory, component);
		}
	}
}
