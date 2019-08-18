using Engine;

namespace Game
{
	public class MachineToolWidget : EntityWidget<ComponentLargeCraftingTable>
	{
		protected readonly InventorySlotWidget m_craftingRemainsSlot,
												m_craftingResultSlot;

		public MachineToolWidget(IInventory inventory, ComponentLargeCraftingTable componentCraftingTable) : base(componentCraftingTable, "Widgets/MachineToolWidget")
		{
			m_furnaceGrid = Children.Find<GridPanelWidget>("CraftingGrid");
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
			for (y = 0; y < m_furnaceGrid.RowsCount; y++)
			{
				for (x = 0; x < m_furnaceGrid.ColumnsCount; x++)
				{
					inventorySlotWidget = new InventorySlotWidget();/*
					{
						Size = new Vector2(48f)
					};*/
					inventorySlotWidget.AssignInventorySlot(m_component, num++);
					m_furnaceGrid.Children.Add(inventorySlotWidget);
					m_furnaceGrid.SetWidgetCell(inventorySlotWidget, new Point2(x, y));
				}
			}
			m_craftingResultSlot.AssignInventorySlot(m_component, m_component.ResultSlotIndex);
			m_craftingRemainsSlot.AssignInventorySlot(m_component, m_component.RemainsSlotIndex);
		}
	}
}