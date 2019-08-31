using Engine;
namespace Game
{
	public class MachineToolWidget : EntityWidget<ComponentLargeCraftingTable>
	{
		protected readonly InventorySlotWidget m_craftingRemainsSlot,
												m_craftingResultSlot;

		public MachineToolWidget(IInventory inventory, ComponentLargeCraftingTable componentCraftingTable, string path = "Widgets/MachineToolWidget") : base(inventory, componentCraftingTable, path)
		{
			m_craftingResultSlot = Children.Find<InventorySlotWidget>("CraftingResultSlot");
			m_craftingRemainsSlot = Children.Find<InventorySlotWidget>("CraftingRemainsSlot");
			//InitGrid("CraftingGrid");
			m_furnaceGrid = Children.Find<GridPanelWidget>("CraftingGrid");
			int num = 0, y, x;
			float ss = 72f;
			for (y = 0; y < m_furnaceGrid.RowsCount; y++)
			{
				for (x = 0; x < m_furnaceGrid.ColumnsCount; x++)
				{
					if(m_furnaceGrid.RowsCount==5)
					{
						ss = 60f;
					}
						var inventorySlotWidget = new InventorySlotWidget
						{
							Size_ = new Vector2(ss)
						};
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