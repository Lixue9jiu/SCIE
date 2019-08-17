using Engine;
using System.Xml.Linq;

namespace Game
{
	public class ElectricFurnaceWidget : CanvasWidget
	{
		protected readonly ComponentElectricFurnace m_component;

		protected readonly FireWidget m_fire;

		protected readonly GridPanelWidget m_furnaceGrid;

		protected readonly GridPanelWidget m_inventoryGrid;

		protected readonly ValueBarWidget m_progress;

		protected readonly InventorySlotWidget m_remainsSlot,
												m_resultSlot;

		protected readonly InventorySlotWidget m_circuitSlot, m_circuit2Slot;

		protected readonly CheckboxWidget m_acceptsDropsBox;

		public ElectricFurnaceWidget(IInventory inventory, ComponentElectricFurnace component, string path = "Widgets/ElectricFurnaceWidget")
		{
			m_component = component;
			WidgetsManager.LoadWidgetContents(this, this, ContentManager.Get<XElement>(path));
			m_inventoryGrid = Children.Find<GridPanelWidget>("InventoryGrid");
			m_furnaceGrid = Children.Find<GridPanelWidget>("FurnaceGrid");
			m_fire = Children.Find<FireWidget>("Fire");
			m_progress = Children.Find<ValueBarWidget>("Progress");
			m_resultSlot = Children.Find<InventorySlotWidget>("ResultSlot");
			m_remainsSlot = Children.Find<InventorySlotWidget>("RemainsSlot");
			m_acceptsDropsBox = Children.Find<CheckboxWidget>("AcceptsElectBox");
			m_circuitSlot = Children.Find<InventorySlotWidget>("CircuitSlot");
			m_circuit2Slot = Children.Find<InventorySlotWidget>("Circuit2Slot");
			int num = 6, y, x;
			InventorySlotWidget inventorySlotWidget;
			for (y = 0; y < m_inventoryGrid.RowsCount; y++)
			{
				for (x = 0; x < m_inventoryGrid.ColumnsCount; x++)
				{
					inventorySlotWidget = new InventorySlotWidget();
					inventorySlotWidget.AssignInventorySlot(inventory, num++);
					m_inventoryGrid.Children.Add(inventorySlotWidget);
					m_inventoryGrid.SetWidgetCell(inventorySlotWidget, new Point2(x, y));
				}
			}
			num = 0;
			for (y = 0; y < m_furnaceGrid.RowsCount; y++)
			{
				for (x = 0; x < m_furnaceGrid.ColumnsCount; x++)
				{
					inventorySlotWidget = new InventorySlotWidget();
					inventorySlotWidget.AssignInventorySlot(component, num++);
					m_furnaceGrid.Children.Add(inventorySlotWidget);
					m_furnaceGrid.SetWidgetCell(inventorySlotWidget, new Point2(x, y));
				}
			}
			m_resultSlot.AssignInventorySlot(component, component.ResultSlotIndex);
			m_remainsSlot.AssignInventorySlot(component, component.RemainsSlotIndex);
			m_circuitSlot.AssignInventorySlot(component, component.Cir1SlotIndex);
			m_circuit2Slot.AssignInventorySlot(component, component.Cir2SlotIndex);
		}

		public override void Update()
		{
			m_fire.ParticlesPerSecond = m_component.HeatLevel > 0f ? 24f : 0f;
			m_progress.Value = m_component.SmeltingProgress;
			if (!m_component.IsAddedToProject)
				ParentWidget.Children.Remove(this);
		}
	}
}