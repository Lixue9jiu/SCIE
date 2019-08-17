using Engine;
using System.Xml.Linq;

namespace Game
{
	public class CReactorWidget : EntityWidget<ComponentMachine>
	{
		//protected readonly FireWidget m_fire;

		//protected readonly InventorySlotWidget m_fuelSlot;

		protected readonly GridPanelWidget m_furnaceGrid;

		protected readonly InventorySlotWidget m_result1,
												m_result2,
												m_result3;

		protected readonly ValueBarWidget m_progress;

		public CReactorWidget(IInventory inventory, ComponentMachine component) : base(component, "Widgets/CReactorWidget")
		{
			m_furnaceGrid = Children.Find<GridPanelWidget>("FurnaceGrid");
			m_result1 = Children.Find<InventorySlotWidget>("ResultSlot1");
			m_result2 = Children.Find<InventorySlotWidget>("ResultSlot2");
			m_result3 = Children.Find<InventorySlotWidget>("ResultSlot3");
			m_progress = Children.Find<ValueBarWidget>("Progress");
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
			m_result1.AssignInventorySlot(component, num);
			m_result2.AssignInventorySlot(component, num + 1);
			m_result3.AssignInventorySlot(component, num + 2);
		}

		public override void Update()
		{
			m_progress.Value = m_component.SmeltingProgress;
			if (!m_component.IsAddedToProject)
				ParentWidget.Children.Remove(this);
		}
	}
}