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
			m_componentFurnace = componentFurnace;
			WidgetsManager.LoadWidgetContents(this, this, ContentManager.Get<XElement>("Widgets/FireBoxWidget"));
			m_inventoryGrid = Children.Find<GridPanelWidget>("InventoryGrid", true);
			m_fire = Children.Find<FireWidget>("Fire", true);
			m_progress = Children.Find<ValueBarWidget>("Progress", true);
			m_fuelSlot = Children.Find<InventorySlotWidget>("FuelSlot", true);
			int num = 6;
			for (int i = 0; i < m_inventoryGrid.RowsCount; i++)
			{
				for (int j = 0; j < m_inventoryGrid.ColumnsCount; j++)
				{
					InventorySlotWidget inventorySlotWidget = new InventorySlotWidget();
					inventorySlotWidget.AssignInventorySlot(inventory, num++);
					m_inventoryGrid.Children.Add(inventorySlotWidget);
					m_inventoryGrid.SetWidgetCell(inventorySlotWidget, new Point2(j, i));
				}
			}
			m_fuelSlot.AssignInventorySlot(componentFurnace, componentFurnace.FuelSlotIndex);
		}

		// Token: 0x0600219B RID: 8603 RVA: 0x000E2434 File Offset: 0x000E0634
		public override void Update()
		{
			m_fire.ParticlesPerSecond = (((double)m_componentFurnace.HeatLevel > 0.0) ? 24f : 0f);
			m_progress.Value = m_componentFurnace.SmeltingProgress;
			if (m_componentFurnace.IsAddedToProject)
			{
				return;
			}
			ParentWidget.Children.Remove(this);
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
