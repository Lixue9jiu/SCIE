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
			this.m_componentFurnace = componentFurnace;
			WidgetsManager.LoadWidgetContents(this, this, ContentManager.Get<XElement>("Widgets/ElectricFurnaceWidget"));
			this.m_inventoryGrid = this.Children.Find<GridPanelWidget>("InventoryGrid", true);
			this.m_furnaceGrid = this.Children.Find<GridPanelWidget>("FurnaceGrid", true);
			this.m_fire = this.Children.Find<FireWidget>("Fire", true);
			this.m_progress = this.Children.Find<ValueBarWidget>("Progress", true);
			this.m_resultSlot = this.Children.Find<InventorySlotWidget>("ResultSlot", true);
			this.m_remainsSlot = this.Children.Find<InventorySlotWidget>("RemainsSlot", true);
			this.m_circuitSlot = this.Children.Find<InventorySlotWidget>("CircuitSlot", true);
			this.m_acceptsDropsBox = this.Children.Find<CheckboxWidget>("AcceptsElectBox", true);
			this.m_circuit2Slot = this.Children.Find<InventorySlotWidget>("Circuit2Slot", true);
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
					Log.Information(num2.ToString());
					this.m_furnaceGrid.Children.Add(inventorySlotWidget2);
					this.m_furnaceGrid.SetWidgetCell(inventorySlotWidget2, new Point2(l, k));
				}
			}
			this.m_resultSlot.AssignInventorySlot(componentFurnace, componentFurnace.ResultSlotIndex);
			this.m_circuitSlot.AssignInventorySlot(componentFurnace, componentFurnace.ResultSlotIndex);
			this.m_circuit2Slot.AssignInventorySlot(componentFurnace, componentFurnace.ResultSlotIndex);
			this.m_remainsSlot.AssignInventorySlot(componentFurnace, componentFurnace.RemainsSlotIndex);
		}

		// Token: 0x06000C7F RID: 3199 RVA: 0x000620B8 File Offset: 0x000602B8
		public override void Update()
		{
			this.m_fire.ParticlesPerSecond = (((double)this.m_componentFurnace.HeatLevel > 0.0) ? 24f : 0f);
			this.m_progress.Value = this.m_componentFurnace.SmeltingProgress;
			if (this.m_componentFurnace.IsAddedToProject)
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
		private readonly InventorySlotWidget m_fuelSlot;

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
