namespace Game
{
	public class MagnetizerWidget : PresserWidget<ComponentMagnetizer>
	{
		public MagnetizerWidget(IInventory inventory, ComponentMagnetizer component) : base(inventory, component, "Widgets/MagnetizerWidget")
		{
			m_fuelSlot.AssignInventorySlot(component, component.FuelSlotIndex);
		}
	}
}
