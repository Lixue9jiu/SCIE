using Engine;
using System.Xml.Linq;

namespace Game
{
	public class ChestNewWidget : CanvasWidget
	{
		private readonly GridPanelWidget m_inventoryGrid;

		private readonly ComponentChestNew m_componentChestNew;

		private readonly GridPanelWidget m_chestNewGrid;

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
			int num = 0;
			for (int i = 0; i < m_chestNewGrid.RowsCount; i++)
			{
				for (int j = 0; j < m_chestNewGrid.ColumnsCount; j++)
				{
					var inventorySlotWidget = new InventorySlotWidget();
					inventorySlotWidget.AssignInventorySlot(componentChestNew, num++);
					m_chestNewGrid.Children.Add(inventorySlotWidget);
					m_chestNewGrid.SetWidgetCell(inventorySlotWidget, new Point2(j, i));
				}
			}
			int num3 = 6;
			for (int k = 0; k < m_inventoryGrid.RowsCount; k++)
			{
				for (int l = 0; l < m_inventoryGrid.ColumnsCount; l++)
				{
					var inventorySlotWidget2 = new InventorySlotWidget();
					inventorySlotWidget2.AssignInventorySlot(inventory, num3++);
					m_inventoryGrid.Children.Add(inventorySlotWidget2);
					m_inventoryGrid.SetWidgetCell(inventorySlotWidget2, new Point2(l, k));
				}
			}
		}
	}
}
