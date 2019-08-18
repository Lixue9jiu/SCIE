using Engine;
namespace Game
{
	public class NewChestWidget : EntityWidget<ComponentInventoryBase>
	{
		public NewChestWidget(IInventory inventory, ComponentInventoryBase component, string text = "Freezer") : base(component, "Widgets/NewChestWidget")
		{
			Children.Find<LabelWidget>("ChestLabel").Text = text;
			if (Utils.TR.Count != 0)
				Children.Find<LabelWidget>("InventoryLabel").Text = "±³°ü";
			m_furnaceGrid = Children.Find<GridPanelWidget>("ChestGrid");
			int num = 0, y, x;
			InventorySlotWidget inventorySlotWidget;
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