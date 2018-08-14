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
			m_componentFurnace = componentFurnace;
			WidgetsManager.LoadWidgetContents(this, this, ContentManager.Get<XElement>("Widgets/BlastFurnaceWidget"));
			m_inventoryGrid = Children.Find<GridPanelWidget>("InventoryGrid", true);
			m_furnaceGrid = Children.Find<GridPanelWidget>("FurnaceGrid", true);
			m_fire = Children.Find<FireWidget>("Fire", true);
			m_progress = Children.Find<ValueBarWidget>("Progress", true);
			m_resultSlot = Children.Find<InventorySlotWidget>("ResultSlot", true);
			m_remainsSlot = Children.Find<InventorySlotWidget>("RemainsSlot", true);
			m_remainsSlot2 = Children.Find<InventorySlotWidget>("RemainsSlot2", true);
			m_remainsSlot3 = Children.Find<InventorySlotWidget>("RemainsSlot3", true);
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
			int num2 = 0;
			for (int k = 0; k < m_furnaceGrid.RowsCount; k++)
			{
				for (int l = 0; l < m_furnaceGrid.ColumnsCount; l++)
				{
					InventorySlotWidget inventorySlotWidget2 = new InventorySlotWidget();
					inventorySlotWidget2.AssignInventorySlot(componentFurnace, num2++);
					m_furnaceGrid.Children.Add(inventorySlotWidget2);
					m_furnaceGrid.SetWidgetCell(inventorySlotWidget2, new Point2(l, k));
				}
			}
			m_resultSlot.AssignInventorySlot(componentFurnace, 8);
			m_remainsSlot.AssignInventorySlot(componentFurnace, 9);
			m_remainsSlot2.AssignInventorySlot(componentFurnace, 10);
			m_remainsSlot3.AssignInventorySlot(componentFurnace, 11);
		}

		// Token: 0x06002175 RID: 8565 RVA: 0x000E1A44 File Offset: 0x000DFC44
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
