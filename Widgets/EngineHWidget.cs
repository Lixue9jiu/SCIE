using Engine;
using System.Xml.Linq;

namespace Game
{
	public class EngineHWidget : CanvasWidget
	{
		private readonly ComponentEngineH m_componentFurnace;

		private readonly FireWidget m_fire;

		private readonly InventorySlotWidget m_fuelSlot;

		//private readonly GridPanelWidget m_furnaceGrid;

		private readonly GridPanelWidget m_inventoryGrid;

		private readonly ValueBarWidget m_progress;

		//private readonly InventorySlotWidget m_remainsSlot;

		//private readonly InventorySlotWidget m_resultSlot;

		public EngineHWidget(IInventory inventory, ComponentEngineH componentFurnace)
		{
			m_componentFurnace = componentFurnace;
			WidgetsManager.LoadWidgetContents(this, this, ContentManager.Get<XElement>("Widgets/EngineHWidget"));
			m_inventoryGrid = Children.Find<GridPanelWidget>("InventoryGrid", true);
			m_fire = Children.Find<FireWidget>("Fire", true);
			m_progress = Children.Find<ValueBarWidget>("Progress", true);
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
			m_fuelSlot.AssignInventorySlot(componentFurnace, componentFurnace.FuelSlotIndex);
		}

		public override void Update()
		{
			m_fire.ParticlesPerSecond = m_componentFurnace.HeatLevel > 0f ? 24f : 0f;
			m_progress.Value = m_componentFurnace.SmeltingProgress;
			if (!m_componentFurnace.IsAddedToProject)
			{
				ParentWidget.Children.Remove(this);
			}
		}
	}
}
