using Engine;

namespace Game
{
	public class CovenWidget : FireBoxWidget<ComponentMachine>
	{
		protected readonly InventorySlotWidget m_remainsSlot,
												m_remainsSlot2,
												m_remainsSlot3;

		public CovenWidget(IInventory inventory, ComponentMachine component, string path) : base(inventory, component, path)
		{
			m_remainsSlot = Children.Find<InventorySlotWidget>("RemainsSlot");
			m_remainsSlot2 = Children.Find<InventorySlotWidget>("RemainsSlot2");
			m_remainsSlot3 = Children.Find<InventorySlotWidget>("RemainsSlot3", false);
			int num = 0, y, x;
			for (y = 0; y < m_furnaceGrid.RowsCount; y++)
			{
				for (x = 0; x < m_furnaceGrid.ColumnsCount; x++)
				{
					var inventorySlotWidget = new InventorySlotWidget();
					inventorySlotWidget.AssignInventorySlot(component, num++);
					m_furnaceGrid.Children.Add(inventorySlotWidget);
					m_furnaceGrid.SetWidgetCell(inventorySlotWidget, new Point2(x, y));
				}
			}
			num = m_remainsSlot3 != null ? 8 : 6;
			m_resultSlot.AssignInventorySlot(component, num);
			m_remainsSlot.AssignInventorySlot(component, num + 1);
			m_remainsSlot2.AssignInventorySlot(component, num + 2);
			m_remainsSlot3?.AssignInventorySlot(component, 11);
		}
	}
}