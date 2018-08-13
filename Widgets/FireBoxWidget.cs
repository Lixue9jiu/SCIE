using System;
using System.Xml.Linq;
using Engine;

namespace Game
{
	// Token: 0x02000619 RID: 1561
	public class FireBoxWidget : CanvasWidget
	{
		// Token: 0x0600219A RID: 8602 RVA: 0x000E2334 File Offset: 0x000E0534
		public FireBoxWidget(IInventory inventory, ComponentFireBox componentFurnace)
		{
			this.m_componentFurnace = componentFurnace;
			WidgetsManager.LoadWidgetContents(this, this, ContentManager.Get<XElement>("Widgets/FireBoxWidget"));
			this.m_inventoryGrid = this.Children.Find<GridPanelWidget>("InventoryGrid", true);
			this.m_fire = this.Children.Find<FireWidget>("Fire", true);
			this.m_progress = this.Children.Find<ValueBarWidget>("Progress", true);
			this.m_fuelSlot = this.Children.Find<InventorySlotWidget>("FuelSlot", true);
			int num = 6;
			for (int i = 0; i < this.m_inventoryGrid.RowsCount; i++)
			{
				for (int j = 0; j < this.m_inventoryGrid.ColumnsCount; j++)
				{
					InventorySlotWidget inventorySlotWidget = new InventorySlotWidget();
					inventorySlotWidget.AssignInventorySlot(inventory, num++);
					this.m_inventoryGrid.Children.Add(inventorySlotWidget);
					this.m_inventoryGrid.SetWidgetCell(inventorySlotWidget, new Point2(j, i));
				}
			}
			this.m_fuelSlot.AssignInventorySlot(componentFurnace, componentFurnace.FuelSlotIndex);
		}

		// Token: 0x0600219B RID: 8603 RVA: 0x000E2434 File Offset: 0x000E0634
		public override void Update()
		{
			this.m_fire.ParticlesPerSecond = (((double)this.m_componentFurnace.HeatLevel > 0.0) ? 24f : 0f);
			this.m_progress.Value = this.m_componentFurnace.SmeltingProgress;
			if (this.m_componentFurnace.IsAddedToProject)
			{
				return;
			}
			base.ParentWidget.Children.Remove(this);
		}

		// Token: 0x040019A2 RID: 6562
		private readonly ComponentFireBox m_componentFurnace;

		// Token: 0x040019A3 RID: 6563
		private readonly FireWidget m_fire;

		// Token: 0x040019A4 RID: 6564
		private readonly InventorySlotWidget m_fuelSlot;

		// Token: 0x040019A5 RID: 6565
		private readonly GridPanelWidget m_furnaceGrid;

		// Token: 0x040019A6 RID: 6566
		private readonly GridPanelWidget m_inventoryGrid;

		// Token: 0x040019A7 RID: 6567
		private readonly ValueBarWidget m_progress;

		// Token: 0x040019A8 RID: 6568
		private readonly InventorySlotWidget m_remainsSlot;

		// Token: 0x040019A9 RID: 6569
		private readonly InventorySlotWidget m_resultSlot;
	}
}