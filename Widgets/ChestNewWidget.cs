using Engine;
using System.Xml.Linq;

namespace Game
{
	public class ChestNewWidget : CanvasWidget
	{
		protected readonly GridPanelWidget m_inventoryGrid;

		protected readonly ComponentChestNew m_componentChestNew;

		protected readonly GridPanelWidget m_chestNewGrid;

		public override void Update()
		{
			if (!m_componentChestNew.IsAddedToProject)
			{
				ParentWidget.Children.Remove(this);
			}
		}

		public ChestNewWidget(IInventory inventory, ComponentChestNew componentChestNew)
		{
			m_componentChestNew = componentChestNew;
			WidgetsManager.LoadWidgetContents(this, this, ContentManager.Get<XElement>("Widgets/ChestNewWidget"));
			m_inventoryGrid = Children.Find<GridPanelWidget>("InventoryGrid", true);
			m_chestNewGrid = Children.Find<GridPanelWidget>("ChestGrid", true);
			int num = 0, y, x;
			for (y = 0; y < m_chestNewGrid.RowsCount; y++)
			{
				for (x = 0; x < m_chestNewGrid.ColumnsCount; x++)
				{
					var inventorySlotWidget = new InventorySlotWidget();
					inventorySlotWidget.AssignInventorySlot(componentChestNew, num++);
					m_chestNewGrid.Children.Add(inventorySlotWidget);
					m_chestNewGrid.SetWidgetCell(inventorySlotWidget, new Point2(x, y));
				}
			}
			num = 6;
			for (y = 0; y < m_inventoryGrid.RowsCount; y++)
			{
				for (x = 0; x < m_inventoryGrid.ColumnsCount; x++)
				{
					var inventorySlotWidget2 = new InventorySlotWidget();
					inventorySlotWidget2.AssignInventorySlot(inventory, num++);
					m_inventoryGrid.Children.Add(inventorySlotWidget2);
					m_inventoryGrid.SetWidgetCell(inventorySlotWidget2, new Point2(x, y));
				}
			}
		}
	}
}
