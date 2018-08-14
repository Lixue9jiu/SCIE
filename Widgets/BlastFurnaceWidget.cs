using System;
using System.Xml.Linq;
using Engine;

namespace Game
{
	// Token: 0x02000614 RID: 1556
	public class BlastFurnaceWidget : CanvasWidget
	{
		// Token: 0x06002174 RID: 8564 RVA: 0x000E1850 File Offset: 0x000DFA50
		public BlastFurnaceWidget(IInventory inventory, ComponentBlastFurnace componentFurnace)
		{
			this.m_componentFurnace = componentFurnace;
			WidgetsManager.LoadWidgetContents(this, this, ContentManager.Get<XElement>("Widgets/BlastFurnaceWidget"));
			this.m_inventoryGrid = this.Children.Find<GridPanelWidget>("InventoryGrid", true);
			this.m_furnaceGrid = this.Children.Find<GridPanelWidget>("FurnaceGrid", true);
			this.m_fire = this.Children.Find<FireWidget>("Fire", true);
			this.m_progress = this.Children.Find<ValueBarWidget>("Progress", true);
			this.m_resultSlot = this.Children.Find<InventorySlotWidget>("ResultSlot", true);
			this.m_remainsSlot = this.Children.Find<InventorySlotWidget>("RemainsSlot", true);
			this.m_remainsSlot2 = this.Children.Find<InventorySlotWidget>("RemainsSlot2", true);
			this.m_remainsSlot3 = this.Children.Find<InventorySlotWidget>("RemainsSlot3", true);
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
			int num2 = 0;
			for (int k = 0; k < this.m_furnaceGrid.RowsCount; k++)
			{
				for (int l = 0; l < this.m_furnaceGrid.ColumnsCount; l++)
				{
					InventorySlotWidget inventorySlotWidget2 = new InventorySlotWidget();
					inventorySlotWidget2.AssignInventorySlot(componentFurnace, num2++);
					this.m_furnaceGrid.Children.Add(inventorySlotWidget2);
					this.m_furnaceGrid.SetWidgetCell(inventorySlotWidget2, new Point2(l, k));
				}
			}
			this.m_resultSlot.AssignInventorySlot(componentFurnace, 8);
			this.m_remainsSlot.AssignInventorySlot(componentFurnace, 9);
			this.m_remainsSlot2.AssignInventorySlot(componentFurnace, 10);
			this.m_remainsSlot3.AssignInventorySlot(componentFurnace, 11);
		}

		// Token: 0x06002175 RID: 8565 RVA: 0x000E1A44 File Offset: 0x000DFC44
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

		// Token: 0x0400197F RID: 6527
		private readonly ComponentBlastFurnace m_componentFurnace;

		// Token: 0x04001980 RID: 6528
		private readonly FireWidget m_fire;

		// Token: 0x04001981 RID: 6529
		private readonly GridPanelWidget m_furnaceGrid;

		// Token: 0x04001982 RID: 6530
		private readonly GridPanelWidget m_inventoryGrid;

		// Token: 0x04001983 RID: 6531
		private readonly ValueBarWidget m_progress;

		// Token: 0x04001984 RID: 6532
		private readonly InventorySlotWidget m_remainsSlot;

		// Token: 0x04001985 RID: 6533
		private readonly InventorySlotWidget m_resultSlot;

		// Token: 0x04001986 RID: 6534
		private readonly GridPanelWidget m_fuelGrid;

		// Token: 0x04001987 RID: 6535
		private readonly InventorySlotWidget m_remainsSlot2;

		// Token: 0x04001988 RID: 6536
		private readonly InventorySlotWidget m_remainsSlot3;
	}
}
