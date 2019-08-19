namespace Game
{
	public class CastMachWidget : FireBoxWidget<ComponentMachine>
	{
		protected readonly InventorySlotWidget m_remainsSlot;

		public CastMachWidget(IInventory inventory, ComponentMachine component) : base(inventory, component, "Widgets/CastMachWidget")
		{
			m_remainsSlot = Children.Find<InventorySlotWidget>("RemainsSlot");
			InitGrid();
			m_fuelSlot.AssignInventorySlot(component, component.FuelSlotIndex);
			m_resultSlot.AssignInventorySlot(component, component.ResultSlotIndex);
			m_remainsSlot.AssignInventorySlot(component, component.RemainsSlotIndex);
		}
	}
}