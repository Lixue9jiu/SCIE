using Engine;
using System.Xml.Linq;

namespace Game
{
	public class LiquidPumpWidget : CanvasWidget
	{
		//protected readonly CheckboxWidget m_acceptsDropsBox;

		protected readonly ComponentBlockEntity m_componentBlockEntity;

		protected readonly ComponentInventoryBase m_componentDispenser;

		protected readonly GridPanelWidget m_dispenserGrid;

		protected readonly GridPanelWidget m_inventoryGrid;

		protected readonly SubsystemTerrain m_subsystemTerrain;

		protected readonly InventorySlotWidget m_drillSlot;

		protected readonly GridPanelWidget m_dispenserGrid2;

		public LiquidPumpWidget(IInventory inventory, ComponentInventoryBase componentDispenser)
		{
			m_componentDispenser = componentDispenser;
			m_componentBlockEntity = componentDispenser.Entity.FindComponent<ComponentBlockEntity>(true);
			m_subsystemTerrain = componentDispenser.Project.FindSubsystem<SubsystemTerrain>(true);
			WidgetsManager.LoadWidgetContents(this, this, ContentManager.Get<XElement>("Widgets/LiquidPumpWidget"));
			m_inventoryGrid = Children.Find<GridPanelWidget>("InventoryGrid");
			m_dispenserGrid = Children.Find<GridPanelWidget>("DispenserGrid");
			m_dispenserGrid2 = Children.Find<GridPanelWidget>("DispenserGrid2");
			m_drillSlot = Children.Find<InventorySlotWidget>("DrillSlot");
			int num = 0, y, x;
			for (y = 0; y < m_dispenserGrid.RowsCount; y++)
			{
				for (x = 0; x < m_dispenserGrid.ColumnsCount; x++)
				{
					var inventorySlotWidget = new InventorySlotWidget();
					inventorySlotWidget.AssignInventorySlot(componentDispenser, num++);
					m_dispenserGrid.Children.Add(inventorySlotWidget);
					m_dispenserGrid.SetWidgetCell(inventorySlotWidget, new Point2(x, y));
				}
			}
			for (y = 0; y < m_dispenserGrid2.RowsCount; y++)
			{
				for (x = 0; x < m_dispenserGrid2.ColumnsCount; x++)
				{
					var inventorySlotWidget2 = new InventorySlotWidget();
					inventorySlotWidget2.AssignInventorySlot(componentDispenser, num++);
					m_dispenserGrid2.Children.Add(inventorySlotWidget2);
					m_dispenserGrid2.SetWidgetCell(inventorySlotWidget2, new Point2(x, y));
				}
			}
			num = 6;
			for (y = 0; y < m_inventoryGrid.RowsCount; y++)
			{
				for (x = 0; x < m_inventoryGrid.ColumnsCount; x++)
				{
					var inventorySlotWidget3 = new InventorySlotWidget();
					inventorySlotWidget3.AssignInventorySlot(inventory, num++);
					m_inventoryGrid.Children.Add(inventorySlotWidget3);
					m_inventoryGrid.SetWidgetCell(inventorySlotWidget3, new Point2(x, y));
				}
			}
			m_drillSlot.AssignInventorySlot(componentDispenser, 8);
		}

		public override void Update()
		{
			if (!m_componentDispenser.IsAddedToProject)
				ParentWidget.Children.Remove(this);
		}
	}
}