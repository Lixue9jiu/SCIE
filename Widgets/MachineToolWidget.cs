using System;
using System.Xml.Linq;
using Engine;

namespace Game
{
	// Token: 0x0200013F RID: 319
	public class MachineToolWidget : CanvasWidget
	{
		// Token: 0x060009E6 RID: 2534 RVA: 0x00053C10 File Offset: 0x00051E10
		public MachineToolWidget(IInventory inventory, ComponentMachineTool componentCraftingTable)
		{
			this.m_componentCraftingTable = componentCraftingTable;
			WidgetsManager.LoadWidgetContents(this, this, ContentManager.Get<XElement>("Widgets/MachineToolWidget"));
			this.m_inventoryGrid = this.Children.Find<GridPanelWidget>("InventoryGrid", true);
			this.m_craftingGrid = this.Children.Find<GridPanelWidget>("CraftingGrid", true);
			this.m_craftingResultSlot = this.Children.Find<InventorySlotWidget>("CraftingResultSlot", true);
			this.m_craftingRemainsSlot = this.Children.Find<InventorySlotWidget>("CraftingRemainsSlot", true);
			int num = 6;
			for (int y = 0; y < this.m_inventoryGrid.RowsCount; y++)
			{
				for (int x = 0; x < this.m_inventoryGrid.ColumnsCount; x++)
				{
					InventorySlotWidget inventorySlotWidget = new InventorySlotWidget();
					inventorySlotWidget.AssignInventorySlot(inventory, num++);
					this.m_inventoryGrid.Children.Add(inventorySlotWidget);
					this.m_inventoryGrid.SetWidgetCell(inventorySlotWidget, new Point2(x, y));
				}
			}
			int num2 = 0;
			for (int y2 = 0; y2 < this.m_craftingGrid.RowsCount; y2++)
			{
				for (int x2 = 0; x2 < this.m_craftingGrid.ColumnsCount; x2++)
				{
					InventorySlotWidget inventorySlotWidget2 = new InventorySlotWidget();
					inventorySlotWidget2.AssignInventorySlot(this.m_componentCraftingTable, num2++);
					this.m_craftingGrid.Children.Add(inventorySlotWidget2);
					this.m_craftingGrid.SetWidgetCell(inventorySlotWidget2, new Point2(x2, y2));
				}
			}
			this.m_craftingResultSlot.AssignInventorySlot(this.m_componentCraftingTable, this.m_componentCraftingTable.ResultSlotIndex);
			this.m_craftingRemainsSlot.AssignInventorySlot(this.m_componentCraftingTable, this.m_componentCraftingTable.RemainsSlotIndex);
		}

		// Token: 0x060009E7 RID: 2535 RVA: 0x00008D0A File Offset: 0x00006F0A
		public override void Update()
		{
			if (this.m_componentCraftingTable.IsAddedToProject)
			{
				return;
			}
			base.ParentWidget.Children.Remove(this);
		}

		// Token: 0x040007AF RID: 1967
		private readonly ComponentMachineTool m_componentCraftingTable;

		// Token: 0x040007B0 RID: 1968
		private readonly GridPanelWidget m_craftingGrid;

		// Token: 0x040007B1 RID: 1969
		private readonly InventorySlotWidget m_craftingRemainsSlot;

		// Token: 0x040007B2 RID: 1970
		private readonly InventorySlotWidget m_craftingResultSlot;

		// Token: 0x040007B3 RID: 1971
		private readonly GridPanelWidget m_inventoryGrid;
	}
}
