using Engine;
namespace Game
{
	public class NewChestWidget : EntityWidget<ComponentInventoryBase>
	{
		public readonly ComponentInventoryBase Component;

		protected readonly GridPanelWidget m_chestGrid;

		public NewChestWidget(IInventory inventory, ComponentInventoryBase component, string text = "Freezer") : base(component, "Widgets/NewChestWidget")
		{
			Children.Find<LabelWidget>("ChestLabel").Text = text;
			if (Utils.TR.Count != 0)
				Children.Find<LabelWidget>("InventoryLabel").Text = "±³°ü";
			m_chestGrid = Children.Find<GridPanelWidget>("ChestGrid");
			int num = 0, y, x;
			InventorySlotWidget inventorySlotWidget;
			for (y = 0; y < m_chestGrid.RowsCount; y++)
			{
				for (x = 0; x < m_chestGrid.ColumnsCount; x++)
				{
					inventorySlotWidget = new InventorySlotWidget();
					inventorySlotWidget.AssignInventorySlot(component, num++);
					m_chestGrid.Children.Add(inventorySlotWidget);
					m_chestGrid.SetWidgetCell(inventorySlotWidget, new Point2(x, y));
				}
			}
			num = 6;
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
		}
	}
}