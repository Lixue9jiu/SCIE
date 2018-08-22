using Engine;
using System.Xml.Linq;

namespace Game
{
	public class DispenserNew2Widget : CanvasWidget
	{
		//private readonly CheckboxWidget m_acceptsDropsBox;

		private readonly ComponentBlockEntity m_componentBlockEntity;

		private readonly ComponentDispenserNew2 m_componentDispenser;

		private readonly GridPanelWidget m_dispenserGrid;

		private readonly GridPanelWidget m_inventoryGrid;

		private readonly SubsystemTerrain m_subsystemTerrain;

		private readonly InventorySlotWidget m_drillSlot;

		private readonly GridPanelWidget m_dispenserGrid2;

		public DispenserNew2Widget(IInventory inventory, ComponentDispenserNew2 componentDispenser)
		{
			m_componentDispenser = componentDispenser;
			m_componentBlockEntity = componentDispenser.Entity.FindComponent<ComponentBlockEntity>(true);
			m_subsystemTerrain = componentDispenser.Project.FindSubsystem<SubsystemTerrain>(true);
			WidgetsManager.LoadWidgetContents(this, this, ContentManager.Get<XElement>("Widgets/DispenserNew2Widget"));
			m_inventoryGrid = Children.Find<GridPanelWidget>("InventoryGrid", true);
			m_dispenserGrid = Children.Find<GridPanelWidget>("DispenserGrid", true);
			m_dispenserGrid2 = Children.Find<GridPanelWidget>("DispenserGrid2", true);
			m_drillSlot = Children.Find<InventorySlotWidget>("DrillSlot", true);
			int num = 0;
			for (int i = 0; i < m_dispenserGrid.RowsCount; i++)
			{
				for (int j = 0; j < m_dispenserGrid.ColumnsCount; j++)
				{
					var inventorySlotWidget = new InventorySlotWidget();
					inventorySlotWidget.AssignInventorySlot(componentDispenser, num++);
					m_dispenserGrid.Children.Add(inventorySlotWidget);
					m_dispenserGrid.SetWidgetCell(inventorySlotWidget, new Point2(j, i));
				}
			}
			for (int k = 0; k < m_dispenserGrid2.RowsCount; k++)
			{
				for (int l = 0; l < m_dispenserGrid2.ColumnsCount; l++)
				{
					var inventorySlotWidget2 = new InventorySlotWidget();
					inventorySlotWidget2.AssignInventorySlot(componentDispenser, num++);
					m_dispenserGrid2.Children.Add(inventorySlotWidget2);
					m_dispenserGrid2.SetWidgetCell(inventorySlotWidget2, new Point2(l, k));
				}
			}
			int num4 = 6;
			for (int m = 0; m < m_inventoryGrid.RowsCount; m++)
			{
				for (int n = 0; n < m_inventoryGrid.ColumnsCount; n++)
				{
					InventorySlotWidget inventorySlotWidget3 = new InventorySlotWidget();
					inventorySlotWidget3.AssignInventorySlot(inventory, num4++);
					m_inventoryGrid.Children.Add(inventorySlotWidget3);
					m_inventoryGrid.SetWidgetCell(inventorySlotWidget3, new Point2(n, m));
				}
			}
			m_drillSlot.AssignInventorySlot(componentDispenser, 8);
		}

		public override void Update()
		{
			if (!m_componentDispenser.IsAddedToProject)
			{
				ParentWidget.Children.Remove(this);
			}
		}
	}
}
