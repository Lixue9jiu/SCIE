namespace Game
{
	public class TrainWidget : PresserWidget<ComponentTrain>
	{
		public TrainWidget(IInventory inventory, ComponentTrain component) : base(inventory, component, "Widgets/TrainWidget")
		{
			m_fuelSlot.AssignInventorySlot(component, component.FuelSlotIndex);
		}
	}
}
