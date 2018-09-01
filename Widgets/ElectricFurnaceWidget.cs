using System;
using System.Xml.Linq;
using Engine;

namespace Game
{
	// Token: 0x020001D7 RID: 471
	public class ElectricFurnaceWidget : CanvasWidget
	{
		// Token: 0x06000C7E RID: 3198 RVA: 0x00061E90 File Offset: 0x00060090
		public ElectricFurnaceWidget(IInventory inventory, ComponentElectricFurnace componentFurnace)
		{
			m_componentFurnace = componentFurnace;
			WidgetsManager.LoadWidgetContents(this, this, ContentManager.Get<XElement>("Widgets/ElectricFurnaceWidget"));
			m_inventoryGrid = Children.Find<GridPanelWidget>("InventoryGrid", true);
			m_furnaceGrid = Children.Find<GridPanelWidget>("FurnaceGrid", true);
			m_fire = Children.Find<FireWidget>("Fire", true);
			m_progress = Children.Find<ValueBarWidget>("Progress", true);
			m_resultSlot = Children.Find<InventorySlotWidget>("ResultSlot", true);
			m_remainsSlot = Children.Find<InventorySlotWidget>("RemainsSlot", true);
			m_circuitSlot = Children.Find<InventorySlotWidget>("CircuitSlot", true);
			m_acceptsDropsBox = Children.Find<CheckboxWidget>("AcceptsElectBox", true);
			m_circuit2Slot = Children.Find<InventorySlotWidget>("Circuit2Slot", true);
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
			num = 0;
			for (int k = 0; k < m_furnaceGrid.RowsCount; k++)
			{
				for (int l = 0; l < m_furnaceGrid.ColumnsCount; l++)
				{
					InventorySlotWidget inventorySlotWidget2 = new InventorySlotWidget();
					inventorySlotWidget2.AssignInventorySlot(componentFurnace, num++);
					m_furnaceGrid.Children.Add(inventorySlotWidget2);
					m_furnaceGrid.SetWidgetCell(inventorySlotWidget2, new Point2(l, k));
				}
			}
			m_resultSlot.AssignInventorySlot(componentFurnace, componentFurnace.ResultSlotIndex);
			m_remainsSlot.AssignInventorySlot(componentFurnace, componentFurnace.RemainsSlotIndex);
            m_circuitSlot.AssignInventorySlot(componentFurnace, componentFurnace.Cir1SlotIndex);
            m_circuit2Slot.AssignInventorySlot(componentFurnace, componentFurnace.Cir2SlotIndex);
        }

		// Token: 0x06000C7F RID: 3199 RVA: 0x000620B8 File Offset: 0x000602B8
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

		// Token: 0x040009D5 RID: 2517
		private readonly ComponentElectricFurnace m_componentFurnace;

		// Token: 0x040009D6 RID: 2518
		private readonly FireWidget m_fire;

		// Token: 0x040009D7 RID: 2519

		// Token: 0x040009D8 RID: 2520
		private readonly GridPanelWidget m_furnaceGrid;

		// Token: 0x040009D9 RID: 2521
		private readonly GridPanelWidget m_inventoryGrid;

		// Token: 0x040009DA RID: 2522
		private readonly ValueBarWidget m_progress;

		// Token: 0x040009DB RID: 2523
		private readonly InventorySlotWidget m_remainsSlot;

		// Token: 0x040009DC RID: 2524
		private readonly InventorySlotWidget m_resultSlot;

		// Token: 0x040009DD RID: 2525
		private readonly InventorySlotWidget m_circuitSlot;

		// Token: 0x040009DE RID: 2526
		private readonly InventorySlotWidget m_circuit2Slot;

		// Token: 0x040009DF RID: 2527
		private readonly CheckboxWidget m_acceptsDropsBox;
	}
}
