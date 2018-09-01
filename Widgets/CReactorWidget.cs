using Engine;
using System.Xml.Linq;

namespace Game
{
	public class CReactorWidget : CanvasWidget
	{
		protected readonly ComponentCReactor m_componentFurnace;

		protected readonly FireWidget m_fire;

		protected readonly InventorySlotWidget m_fuelSlot;

		protected readonly GridPanelWidget m_furnaceGrid;

        protected readonly InventorySlotWidget m_result1;

        protected readonly InventorySlotWidget m_result2;

        protected readonly InventorySlotWidget m_result3;

        protected readonly GridPanelWidget m_inventoryGrid;

		protected readonly ValueBarWidget m_progress;


		public CReactorWidget(IInventory inventory, ComponentCReactor componentFurnace)
		{
			m_componentFurnace = componentFurnace;
			WidgetsManager.LoadWidgetContents(this, this, ContentManager.Get<XElement>("Widgets/CReactorWidget"));
			m_inventoryGrid = Children.Find<GridPanelWidget>("InventoryGrid", true);
			m_furnaceGrid = Children.Find<GridPanelWidget>("FurnaceGrid", true);
            m_result1 = Children.Find<InventorySlotWidget>("ResultSlot1", true);
            m_result2 = Children.Find<InventorySlotWidget>("ResultSlot2", true);
            m_result3 = Children.Find<InventorySlotWidget>("ResultSlot3", true);
            m_progress = Children.Find<ValueBarWidget>("Progress", true);
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
            m_result2.AssignInventorySlot(componentFurnace, num++);
            m_result3.AssignInventorySlot(componentFurnace, num++);
        }

		public override void Update()
		{
			m_progress.Value = m_componentFurnace.SmeltingProgress;
			if (!m_componentFurnace.IsAddedToProject)
			{
				ParentWidget.Children.Remove(this);
			}
		}
	}
}
