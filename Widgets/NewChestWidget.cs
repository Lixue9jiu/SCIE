using Engine;

namespace Game
{
	public class NewChestWidget : EntityWidget<ComponentInventoryBase>
	{
		public NewChestWidget(IInventory inventory, ComponentInventoryBase component, string text = "Freezer") : base(inventory, component, "Widgets/NewChestWidget")
		{
			Children.Find<LabelWidget>("ChestLabel").Text = text;
			if (Utils.TR.Count != 0)
				Children.Find<LabelWidget>("InventoryLabel").Text = "교관";
			InitGrid("ChestGrid");
		}
	}

	public class SteelChestWidget : EntityWidget<ComponentInventoryBase>
	{
		public SteelChestWidget(IInventory inventory, ComponentInventoryBase component, string text = "SteelChest") : base(inventory, component, "Widgets/SChestWidget")
		{
			Children.Find<LabelWidget>("ChestLabel").Text = text;
			if (Utils.TR.Count != 0)
				Children.Find<LabelWidget>("InventoryLabel").Text = "교관";
			//InitGrid("ChestGrid");
			m_furnaceGrid = Children.Find<GridPanelWidget>("ChestGrid");
			int num = 0, y, x;
			//var component = (IInventory)m_component;
			for (y = 0; y < m_furnaceGrid.RowsCount; y++)
			{
				for (x = 0; x < m_furnaceGrid.ColumnsCount; x++)
				{
					//var inventorySlotWidget = new InventorySlotWidget();
					var inventorySlotWidget = new InventorySlotWidget
					{
						Size_ = new Vector2(58)
					};
					inventorySlotWidget.AssignInventorySlot(component, num++);
					m_furnaceGrid.Children.Add(inventorySlotWidget);
					m_furnaceGrid.SetWidgetCell(inventorySlotWidget, new Point2(x, y));
				}
			}
		}
	}
}