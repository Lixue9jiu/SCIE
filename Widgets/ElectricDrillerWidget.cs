namespace Game
{
	public class ElectricDrillerWidget : MultiStateMachWidget<ComponentElectricDriller>
	{
		protected readonly CheckboxWidget m_acceptsDropsBox;

		protected readonly SubsystemTerrain m_subsystemTerrain;

		protected readonly InventorySlotWidget m_drillSlot,
												m_batterySlot;

		public ElectricDrillerWidget(IInventory inventory, ComponentElectricDriller component) : base(inventory, component, "Widgets/ElectricDrillerWidget")
		{
			m_drillSlot = Children.Find<InventorySlotWidget>("DrillSlot");
			m_batterySlot = Children.Find<InventorySlotWidget>("BatterySlot");
			InitGrid("DispenserGrid");
			m_drillSlot.AssignInventorySlot(component, 8);
			m_batterySlot.AssignInventorySlot(component, 9);
			m_subsystemTerrain = component.Project.FindSubsystem<SubsystemTerrain>(true);
		}

		public override void Update()
		{
			if (!m_component.IsAddedToProject)
			{
				ParentWidget.Children.Remove(this);
				return;
			}
			if (m_dispenseButton.IsClicked && !m_component.Charged)
			{
				//m_componentDispenser.HeatLevel = 1000f;
				m_component.Charged = true;
			}
			if (m_shootButton.IsClicked && m_component.Charged)
			{
				//m_componentDispenser.HeatLevel = 0f;
				m_component.Charged = false;
			}
			m_dispenseButton.IsChecked = m_component.Charged;
			m_shootButton.IsChecked = !m_component.Charged;
		}
	}
}