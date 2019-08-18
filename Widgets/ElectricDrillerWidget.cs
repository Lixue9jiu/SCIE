using Engine;

namespace Game
{
	public class ElectricDrillerWidget : EntityWidget<ComponentElectricDriller>
	{
		protected readonly CheckboxWidget m_acceptsDropsBox;

		protected readonly ButtonWidget m_dispenseButton,
										m_shootButton;

		protected readonly SubsystemTerrain m_subsystemTerrain;

		protected readonly InventorySlotWidget m_drillSlot,
												m_batterySlot;

		public ElectricDrillerWidget(IInventory inventory, ComponentElectricDriller component) : base(component, "Widgets/ElectricDrillerWidget")
		{
			m_subsystemTerrain = component.Project.FindSubsystem<SubsystemTerrain>(true);
			m_furnaceGrid = Children.Find<GridPanelWidget>("DispenserGrid");
			m_dispenseButton = Children.Find<ButtonWidget>("DispenseButton");
			m_shootButton = Children.Find<ButtonWidget>("ShootButton");
			m_drillSlot = Children.Find<InventorySlotWidget>("DrillSlot");
			m_batterySlot = Children.Find<InventorySlotWidget>("BatterySlot");
			int num = 6, y, x;
			InventorySlotWidget inventorySlotWidget;
			for (y = 0; y < m_inventoryGrid.RowsCount; y++)
			{
				for (x = 0; x < m_inventoryGrid.ColumnsCount; x++)
				{
					inventorySlotWidget = new InventorySlotWidget();
					inventorySlotWidget.AssignInventorySlot(inventory, num++);
					m_inventoryGrid.Children.Add(inventorySlotWidget);
					m_inventoryGrid.SetWidgetCell(inventorySlotWidget, new Point2(x, y));
				}
			}
			num = 0;
			for (y = 0; y < m_furnaceGrid.RowsCount; y++)
			{
				for (x = 0; x < m_furnaceGrid.ColumnsCount; x++)
				{
					inventorySlotWidget = new InventorySlotWidget();
					inventorySlotWidget.AssignInventorySlot(component, num++);
					m_furnaceGrid.Children.Add(inventorySlotWidget);
					m_furnaceGrid.SetWidgetCell(inventorySlotWidget, new Point2(x, y));
				}
			}
			m_drillSlot.AssignInventorySlot(component, 8);
			m_batterySlot.AssignInventorySlot(component, 9);
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