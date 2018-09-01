namespace Game
{
	public class EngineWidget : PresserWidget<ComponentEngine>
	{
		public EngineWidget(IInventory inventory, ComponentEngine component) : base(inventory, component, "Widgets/EngineWidget")
		{
			m_fuelSlot.AssignInventorySlot(component, component.FuelSlotIndex);
		}
	}
}
