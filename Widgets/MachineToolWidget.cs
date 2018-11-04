using Engine;
using System.Xml.Linq;

namespace Game
{
	public class MachineToolWidget : CanvasWidget
	{
		protected readonly ComponentLargeCraftingTable m_componentCraftingTable;

		protected readonly GridPanelWidget m_craftingGrid;

		protected readonly InventorySlotWidget m_craftingRemainsSlot;

		protected readonly InventorySlotWidget m_craftingResultSlot;

		protected readonly GridPanelWidget m_inventoryGrid;

		public MachineToolWidget(IInventory inventory, ComponentLargeCraftingTable componentCraftingTable)
		{
			m_componentCraftingTable = componentCraftingTable;
			WidgetsManager.LoadWidgetContents(this, this, ContentManager.Get<XElement>("Widgets/MachineToolWidget"));
			m_inventoryGrid = Children.Find<GridPanelWidget>("InventoryGrid");
			m_craftingGrid = Children.Find<GridPanelWidget>("CraftingGrid");
			m_craftingResultSlot = Children.Find<InventorySlotWidget>("CraftingResultSlot");
			m_craftingRemainsSlot = Children.Find<InventorySlotWidget>("CraftingRemainsSlot");
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
			for (y = 0; y < m_craftingGrid.RowsCount; y++)
			{
				for (x = 0; x < m_craftingGrid.ColumnsCount; x++)
				{
					inventorySlotWidget = new InventorySlotWidget();/*
					{
						Size = new Vector2(48f)
					};*/
					inventorySlotWidget.AssignInventorySlot(m_componentCraftingTable, num++);
					m_craftingGrid.Children.Add(inventorySlotWidget);
					m_craftingGrid.SetWidgetCell(inventorySlotWidget, new Point2(x, y));
				}
			}
			m_craftingResultSlot.AssignInventorySlot(m_componentCraftingTable, m_componentCraftingTable.ResultSlotIndex);
			m_craftingRemainsSlot.AssignInventorySlot(m_componentCraftingTable, m_componentCraftingTable.RemainsSlotIndex);
		}

		public override void Update()
		{
			if (!m_componentCraftingTable.IsAddedToProject)
			{
				ParentWidget.Children.Remove(this);
			}
		}
	}
}