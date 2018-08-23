using System.Xml.Linq;
using Engine;

namespace Game
{
	public class BlastFurnaceWidget : CanvasWidget
	{
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
					var inventorySlotWidget = new InventorySlotWidget();
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
					var inventorySlotWidget2 = new InventorySlotWidget();
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

		public override void Update()
		{
			m_fire.ParticlesPerSecond = (m_componentFurnace.HeatLevel > 0f) ? 24f : 0f;
			m_progress.Value = m_componentFurnace.SmeltingProgress;
			if (m_componentFurnace.IsAddedToProject)
			{
				return;
			}
			ParentWidget.Children.Remove(this);
		}

		private readonly ComponentBlastFurnace m_componentFurnace;

		private readonly FireWidget m_fire;

		private readonly GridPanelWidget m_furnaceGrid;

		private readonly GridPanelWidget m_inventoryGrid;

		private readonly ValueBarWidget m_progress;

		private readonly InventorySlotWidget m_remainsSlot;

		private readonly InventorySlotWidget m_resultSlot;

		//private readonly GridPanelWidget m_fuelGrid;

		private readonly InventorySlotWidget m_remainsSlot2;

		private readonly InventorySlotWidget m_remainsSlot3;
	}
}
