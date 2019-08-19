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

		public override void Update()
		{
			m_fire.ParticlesPerSecond = m_component.HeatLevel > 0f ? 24f : 0f;
			m_progress.Value = m_component.SmeltingProgress;
			if (!m_component.IsAddedToProject)
				ParentWidget.Children.Remove(this);
		}
	}
}