namespace Game
{
	public class SubsystemMachineToolBlockBehavior : SubsystemInventoryBlockBehavior<ComponentLargeCraftingTable>
	{
		public SubsystemMachineToolBlockBehavior() : base("MachineTool")
		{
		}

		public override int[] HandledBlocks
		{
			get
			{
				return new []
				{
					MachineToolBlock.Index
				};
			}
		}

		public override Widget GetWidget(IInventory inventory, ComponentLargeCraftingTable component)
		{
			return new MachineToolWidget(inventory, component);
		}
	}
}
