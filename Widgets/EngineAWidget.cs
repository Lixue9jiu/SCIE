using Engine;

namespace Game
{
	public class EngineAWidget : EntityWidget<ComponentMachine>
	{
		protected readonly FireWidget m_fire;

		//protected readonly InventorySlotWidget m_fuelSlot;
		protected readonly ButtonWidget m_dispenseButton;

		protected readonly ButtonWidget m_shootButton;

		protected readonly GridPanelWidget m_furnaceGrid;

		protected readonly ValueBarWidget m_progress;

		protected readonly InventorySlotWidget m_remainsSlot,
												m_resultSlot;

		public EngineAWidget(IInventory inventory, ComponentMachine component, string path = "Widgets/EngineAWidget") : base(component, path)
		{
			m_furnaceGrid = Children.Find<GridPanelWidget>("FurnaceGrid");
			m_dispenseButton = Children.Find<ButtonWidget>("DispenseButton", false);
			m_fire = Children.Find<FireWidget>("Fire");
			m_shootButton = Children.Find<ButtonWidget>("ShootButton", false);
			m_progress = Children.Find<ValueBarWidget>("Progress");
			m_remainsSlot = Children.Find<InventorySlotWidget>("RemainsSlot");
			m_resultSlot = Children.Find<InventorySlotWidget>("ResultSlot");
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
			m_remainsSlot.AssignInventorySlot(component, component.RemainsSlotIndex);
			m_resultSlot.AssignInventorySlot(component, component.ResultSlotIndex);
		}

		public override void Update()
		{
			m_fire.ParticlesPerSecond = m_component.HeatLevel > 0f ? 24f : 0f;
			m_progress.Value = m_component.SmeltingProgress / 1000f;
			if (m_dispenseButton.IsClicked && m_component.HeatLevel <= 0f && m_component.SmeltingProgress >= 0f)
			{
				m_component.HeatLevel = 1000f;
			}
			if (m_shootButton.IsClicked && m_component.HeatLevel > 0f)
			{
				m_component.HeatLevel = 0f;
			}
			m_dispenseButton.IsChecked = m_component.HeatLevel != 0f;
			//m_componentDispenser2.Charged = mode == MachineMode1.Charge;
			m_shootButton.IsChecked = m_component.HeatLevel == 0f;
			if (!m_component.IsAddedToProject)
				ParentWidget.Children.Remove(this);
		}
	}
}