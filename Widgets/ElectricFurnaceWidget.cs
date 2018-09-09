using System.Xml.Linq;
using Engine;

namespace Game
{
	public class ElectricFurnaceWidget : CanvasWidget
	{
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
					var inventorySlotWidget = new InventorySlotWidget();
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
					var inventorySlotWidget2 = new InventorySlotWidget();
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
		
		private readonly ComponentElectricFurnace m_componentFurnace;
		
		private readonly FireWidget m_fire;
		
		private readonly GridPanelWidget m_furnaceGrid;
		
		private readonly GridPanelWidget m_inventoryGrid;
		
		private readonly ValueBarWidget m_progress;
		
		private readonly InventorySlotWidget m_remainsSlot;
		
		private readonly InventorySlotWidget m_resultSlot;
		
		private readonly InventorySlotWidget m_circuitSlot;
		
		private readonly InventorySlotWidget m_circuit2Slot;
		
		private readonly CheckboxWidget m_acceptsDropsBox;
	}
}
