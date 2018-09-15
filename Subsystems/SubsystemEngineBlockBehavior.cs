namespace Game
{
	public class SubsystemEngineBlockBehavior : SubsystemFurnaceBlockBehavior<ComponentEngine>
	{
		public override int[] HandledBlocks
		{
			get
			{
				return new []
				{
					EngineBlock.Index
				};
			}
		}

		public SubsystemEngineBlockBehavior() : base("SteamEngine")
		{
		}

		public override Widget GetWidget(IInventory inventory, ComponentEngine component)
		{
			return new EngineWidget(inventory, component);
		}
	}
}
