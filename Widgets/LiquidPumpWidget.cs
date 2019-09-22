using Engine;

namespace Game
{
	public class LiquidPumpWidget : EntityWidget<ComponentInventoryBase>
	{
		//protected readonly CheckboxWidget m_acceptsDropsBox;

		protected readonly GridPanelWidget m_dispenserGrid;

		protected readonly InventorySlotWidget m_drillSlot;

		public LiquidPumpWidget(IInventory inventory, ComponentInventoryBase component) : base(inventory, component, "Widgets/LiquidPumpWidget")
		{
			m_drillSlot = Children.Find<InventorySlotWidget>("DrillSlot");
			int num = InitGrid("DispenserGrid"), y, x;
			m_dispenserGrid = Children.Find<GridPanelWidget>("DispenserGrid2");
			for (y = 0; y < m_dispenserGrid.RowsCount; y++)
			{
				for (x = 0; x < m_dispenserGrid.ColumnsCount; x++)
				{
					var inventorySlotWidget = new InventorySlotWidget();
					inventorySlotWidget.AssignInventorySlot(component, num++);
					m_dispenserGrid.Children.Add(inventorySlotWidget);
					m_dispenserGrid.SetWidgetCell(inventorySlotWidget, new Point2(x, y));
				}
			}
			m_drillSlot.AssignInventorySlot(component, 8);
		}
	}


	public class HchangerWidget : EntityWidget<ComponentHChanger>
	{
		//protected readonly CheckboxWidget m_acceptsDropsBox;

		protected readonly GridPanelWidget m_dispenserGrid;
		public HchangerWidget(IInventory inventory, ComponentHChanger component) : base(inventory, component, "Widgets/HChangerWidget")
		{
			
			int num = InitGrid("DispenserGrid"), y, x;
			m_dispenserGrid = Children.Find<GridPanelWidget>("DispenserGrid2");
			for (y = 0; y < m_dispenserGrid.RowsCount; y++)
			{
				for (x = 0; x < m_dispenserGrid.ColumnsCount; x++)
				{
					var inventorySlotWidget = new InventorySlotWidget();
					inventorySlotWidget.AssignInventorySlot(component, num++);
					m_dispenserGrid.Children.Add(inventorySlotWidget);
					m_dispenserGrid.SetWidgetCell(inventorySlotWidget, new Point2(x, y));
				}
			}
			
		}
		public override void Update()
		{
			Children.Find<LabelWidget>("TEXT2").Text = "HeatLevel " + m_component.Pressure.ToString() + "Level";
			
			if (!m_component.IsAddedToProject)
			{
				base.ParentWidget.Children.Remove(this);
			}
		}
	}

}