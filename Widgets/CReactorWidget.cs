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
            for (int m = 0; m < m_furnaceGrid.RowsCount; m++)
            {
                for (int n = 0; n < m_furnaceGrid.ColumnsCount; n++)
                {
                    var inventorySlotWidget3 = new InventorySlotWidget();
                    inventorySlotWidget3.AssignInventorySlot(componentFurnace, num3++);
                    m_furnaceGrid.Children.Add(inventorySlotWidget3);
                    m_furnaceGrid.SetWidgetCell(inventorySlotWidget3, new Point2(n, m));
                }
            }
            m_result1.AssignInventorySlot(componentFurnace, num3++);
            m_result2.AssignInventorySlot(componentFurnace, num3++);
            m_result3.AssignInventorySlot(componentFurnace, num3++);
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
