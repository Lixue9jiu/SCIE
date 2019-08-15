using Engine;
using System.Xml.Linq;

namespace Game
{
	public class ElectricDrillerWidget : CanvasWidget
	{
		protected readonly CheckboxWidget m_acceptsDropsBox;

		protected readonly ComponentBlockEntity m_componentBlockEntity;

		protected readonly ComponentElectricDriller m_componentDispenser;

		protected readonly ButtonWidget m_dispenseButton;

		protected readonly GridPanelWidget m_inventoryGrid;

		protected readonly GridPanelWidget m_furnaceGrid;

		protected readonly ButtonWidget m_shootButton;

		protected readonly SubsystemTerrain m_subsystemTerrain;

		protected readonly InventorySlotWidget m_drillSlot;

		protected readonly InventorySlotWidget m_batterySlot;

		public ElectricDrillerWidget(IInventory inventory, ComponentElectricDriller componentDispenser)
		{
			m_componentDispenser = componentDispenser;
			m_componentBlockEntity = componentDispenser.Entity.FindComponent<ComponentBlockEntity>(true);
			m_subsystemTerrain = componentDispenser.Project.FindSubsystem<SubsystemTerrain>(true);
			WidgetsManager.LoadWidgetContents(this, this, ContentManager.Get<XElement>("Widgets/ElectricDrillerWidget"));
			m_inventoryGrid = Children.Find<GridPanelWidget>("InventoryGrid");
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
					inventorySlotWidget.AssignInventorySlot(componentDispenser, num++);
					m_furnaceGrid.Children.Add(inventorySlotWidget);
					m_furnaceGrid.SetWidgetCell(inventorySlotWidget, new Point2(x, y));
				}
			}
			m_drillSlot.AssignInventorySlot(componentDispenser, 8);
			m_batterySlot.AssignInventorySlot(componentDispenser, 9);
		}

		public override void Update()
		{
			if (m_dispenseButton.IsClicked && m_componentDispenser.HeatLevel <= 0f)
			{
				//m_componentDispenser.HeatLevel = 1000f;
				m_componentDispenser.Charged = true;
			}
			if (m_shootButton.IsClicked && m_componentDispenser.HeatLevel > 0f)
			{
				//m_componentDispenser.HeatLevel = 0f;
				m_componentDispenser.Charged = false;
			}
			m_dispenseButton.IsChecked = m_componentDispenser.Charged;
			m_shootButton.IsChecked = !m_componentDispenser.Charged;
			if (!m_componentDispenser.IsAddedToProject)
				ParentWidget.Children.Remove(this);
		}
	}
}