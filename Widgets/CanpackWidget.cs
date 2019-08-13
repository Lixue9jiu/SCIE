using Engine;
using System.Xml.Linq;

namespace Game
{
	public class CanpackWidget : CanvasWidget
	{
		protected readonly ComponentMachine m_componentFurnace;

		protected readonly GridPanelWidget m_furnaceGrid;

		protected readonly InventorySlotWidget m_result1;

		protected readonly GridPanelWidget m_inventoryGrid;

		protected readonly ValueBarWidget m_progress;
		protected readonly CheckboxWidget m_acceptsDropsBox;

		public CanpackWidget(IInventory inventory, ComponentMachine componentFurnace)
		{
			m_componentFurnace = componentFurnace;
			WidgetsManager.LoadWidgetContents(this, this, ContentManager.Get<XElement>("Widgets/CanpackWidget"));
			m_inventoryGrid = Children.Find<GridPanelWidget>("InventoryGrid");
			m_furnaceGrid = Children.Find<GridPanelWidget>("FurnaceGrid");
			m_result1 = Children.Find<InventorySlotWidget>("ResultSlot");

			m_acceptsDropsBox = Children.Find<CheckboxWidget>("AcceptsElectBox");
			m_progress = Children.Find<ValueBarWidget>("Progress");

			int num = 6, y, x;
			for (y = 0; y < m_inventoryGrid.RowsCount; y++)
			{
				for (x = 0; x < m_inventoryGrid.ColumnsCount; x++)
				{
					var inventorySlotWidget = new InventorySlotWidget();
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
					var inventorySlotWidget2 = new InventorySlotWidget();
					inventorySlotWidget2.AssignInventorySlot(componentFurnace, num++);
					m_furnaceGrid.Children.Add(inventorySlotWidget2);
					m_furnaceGrid.SetWidgetCell(inventorySlotWidget2, new Point2(x, y));
				}
			}
			m_result1.AssignInventorySlot(componentFurnace, num++);
		}

		public override void Update()
		{
			m_progress.Value = m_componentFurnace.SmeltingProgress;
			if (!m_componentFurnace.IsAddedToProject)
				ParentWidget.Children.Remove(this);
		}
	}
}