using Engine;

namespace Game
{
	public class LiquidPumpWidget : EntityWidget<ComponentInventoryBase>
	{
		//protected readonly CheckboxWidget m_acceptsDropsBox;

		protected readonly GridPanelWidget m_dispenserGrid,
											m_dispenserGrid2;

		protected readonly InventorySlotWidget m_drillSlot;


		public LiquidPumpWidget(IInventory inventory, ComponentInventoryBase componentDispenser) : base(componentDispenser, "Widgets/LiquidPumpWidget")
		{
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
	}
}