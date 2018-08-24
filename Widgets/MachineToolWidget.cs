using System.Xml.Linq;
using Engine;

namespace Game
{
	public class MachineToolWidget : CanvasWidget
	{
		public MachineToolWidget(IInventory inventory, ComponentLargeCraftingTable componentCraftingTable)
		{
			m_componentCraftingTable = componentCraftingTable;
			WidgetsManager.LoadWidgetContents(this, this, ContentManager.Get<XElement>("Widgets/MachineToolWidget"));
			m_inventoryGrid = Children.Find<GridPanelWidget>("InventoryGrid", true);
			m_craftingGrid = Children.Find<GridPanelWidget>("CraftingGrid", true);
			m_craftingResultSlot = Children.Find<InventorySlotWidget>("CraftingResultSlot", true);
			m_craftingRemainsSlot = Children.Find<InventorySlotWidget>("CraftingRemainsSlot", true);
			int num = 6, y, x;
			for (y = 0; y < m_inventoryGrid.RowsCount; y++)
			{
				for (x = 0; x < m_inventoryGrid.ColumnsCount; x++)
				{
					var inventorySlotWidget = new InventorySlotWidget();
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
					var inventorySlotWidget2 = new InventorySlotWidget();/*
					{
						Size = new Vector2(48f)
					};*/
					inventorySlotWidget2.AssignInventorySlot(m_componentCraftingTable, num++);
					m_craftingGrid.Children.Add(inventorySlotWidget2);
					m_craftingGrid.SetWidgetCell(inventorySlotWidget2, new Point2(x, y));
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
		
		protected readonly ComponentLargeCraftingTable m_componentCraftingTable;
		
		protected readonly GridPanelWidget m_craftingGrid;
		
		protected readonly InventorySlotWidget m_craftingRemainsSlot;
		
		protected readonly InventorySlotWidget m_craftingResultSlot;
		
		protected readonly GridPanelWidget m_inventoryGrid;
	}
}
