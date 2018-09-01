namespace Game
{
	public class TrainWidget : PresserWidget<ComponentEngine3>
	{
		public TrainWidget(IInventory inventory, ComponentEngine3 component) : base(inventory, component, "Widgets/TrainWidget")
		{
			m_fuelSlot.AssignInventorySlot(component, component.FuelSlotIndex);
		}
	}
}
