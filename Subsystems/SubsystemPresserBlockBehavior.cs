namespace Game
{
	public class SubsystemPresserBlockBehavior : SubsystemInventoryBlockBehavior<ComponentPresser>
	{
		public SubsystemPresserBlockBehavior() : base("Presser")
		{
		}

		public override int[] HandledBlocks
		{
			get
			{
				return new[] { PresserBlock.Index };
			}
		}

		public override Widget GetWidget(IInventory inventory, ComponentPresser component)
		{
			return new PresserWidget<ComponentPresser>(inventory, component, "Widgets/PresserWidget");
		}
	}
}
