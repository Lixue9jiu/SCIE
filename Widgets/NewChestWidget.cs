using Engine;
using System.Xml.Linq;

namespace Game
{
	public class NewChestWidget : CanvasWidget
	{
		protected readonly GridPanelWidget m_inventoryGrid;

		public readonly ComponentInventoryBase Component;

		protected readonly GridPanelWidget m_chestNewGrid;

		public override void Update()
		{
			if (!Component.IsAddedToProject)
				ParentWidget.Children.Remove(this);
		}

		public NewChestWidget(IInventory inventory, ComponentInventoryBase component, string text = "Freezer")
		{
			Component = component;
			WidgetsManager.LoadWidgetContents(this, this, ContentManager.Get<XElement>("Widgets/NewChestWidget"));
			Children.Find<LabelWidget>("ChestLabel").Text = text;
			if (Utils.TR.Count != 0)
				Children.Find<LabelWidget>("InventoryLabel").Text = "±³°ü";
			m_inventoryGrid = Children.Find<GridPanelWidget>("InventoryGrid");
			m_chestNewGrid = Children.Find<GridPanelWidget>("ChestGrid");
			int num = 0, y, x;
			InventorySlotWidget inventorySlotWidget;
			for (y = 0; y < m_chestNewGrid.RowsCount; y++)
			{
				for (x = 0; x < m_chestNewGrid.ColumnsCount; x++)
				{
					inventorySlotWidget = new InventorySlotWidget();
					inventorySlotWidget.AssignInventorySlot(component, num++);
					m_chestNewGrid.Children.Add(inventorySlotWidget);
					m_chestNewGrid.SetWidgetCell(inventorySlotWidget, new Point2(x, y));
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