using Engine;
using System.Xml.Linq;

namespace Game
{
	public class CastMachWidget : CanvasWidget
	{
		private readonly ComponentCastMach m_componentFurnace;

		private readonly FireWidget m_fire;

		private readonly InventorySlotWidget m_fuelSlot;

		private readonly GridPanelWidget m_furnaceGrid;

		private readonly GridPanelWidget m_inventoryGrid;

		private readonly ValueBarWidget m_progress;

		private readonly InventorySlotWidget m_remainsSlot;

		private readonly InventorySlotWidget m_resultSlot;

		public CastMachWidget(IInventory inventory, ComponentCastMach componentFurnace)
		{
			m_componentFurnace = componentFurnace;
			WidgetsManager.LoadWidgetContents(this, this, ContentManager.Get<XElement>("Widgets/CastMachWidget"));
			m_inventoryGrid = Children.Find<GridPanelWidget>("InventoryGrid", true);
			m_furnaceGrid = Children.Find<GridPanelWidget>("FurnaceGrid", true);
			m_fire = Children.Find<FireWidget>("Fire", true);
			m_progress = Children.Find<ValueBarWidget>("Progress", true);
			m_resultSlot = Children.Find<InventorySlotWidget>("ResultSlot", true);
            m_remainsSlot = Children.Find<InventorySlotWidget>("RemainsSlot", true);
            m_fuelSlot = Children.Find<InventorySlotWidget>("FuelSlot", true);
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
			int num3 = 0;
			for (int k = 0; k < m_furnaceGrid.RowsCount; k++)
			{
				for (int l = 0; l < m_furnaceGrid.ColumnsCount; l++)
				{
					var inventorySlotWidget2 = new InventorySlotWidget();
					inventorySlotWidget2.AssignInventorySlot(componentFurnace, num3++);
					m_furnaceGrid.Children.Add(inventorySlotWidget2);
					m_furnaceGrid.SetWidgetCell(inventorySlotWidget2, new Point2(l, k));
				}
			}
			m_fuelSlot.AssignInventorySlot(componentFurnace, componentFurnace.FuelSlotIndex);
			m_resultSlot.AssignInventorySlot(componentFurnace, componentFurnace.ResultSlotIndex);
            m_remainsSlot.AssignInventorySlot(componentFurnace, componentFurnace.RemainsSlotIndex);
        }

		public override void Update()
		{
			m_fire.ParticlesPerSecond = ((double)m_componentFurnace.HeatLevel > 0.0) ? 24f : 0f;
			m_progress.Value = m_componentFurnace.SmeltingProgress;
			if (!m_componentFurnace.IsAddedToProject)
			{
				base.ParentWidget.Children.Remove(this);
			}
		}
	}
}
