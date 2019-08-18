using Engine;

namespace Game
{
	public class FractionalTowerWidget : ProcessWidget<ComponentMachine>
	{
		protected readonly ButtonWidget m_1Button,
										m_2Button,
		                                m_3Button;

		public FractionalTowerWidget(IInventory inventory, ComponentMachine component) : base(component, "Widgets/FractionalTowerWidget")
		{
			m_furnaceGrid = Children.Find<GridPanelWidget>("FurnaceGrid");
			m_1Button = Children.Find<ButtonWidget>("1Button");
			m_2Button = Children.Find<ButtonWidget>("2Button");
			m_3Button = Children.Find<ButtonWidget>("3Button");
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
			m_result1.AssignInventorySlot(component, num++);
			m_result2.AssignInventorySlot(component, num++);
			m_result3.AssignInventorySlot(component, num++);
			m_cir1.AssignInventorySlot(component, num++);
			m_cir2.AssignInventorySlot(component, num++);
		}

		public override void Update()
		{
			if (!m_component.IsAddedToProject)
			{
				ParentWidget.Children.Remove(this);
				return;
			}
			if (m_1Button.IsClicked && m_component.HeatLevel!=1)
			{
				//m_componentDispenser.HeatLevel = 1000f;
				m_component.HeatLevel = 1;
			}
			if (m_2Button.IsClicked && m_component.HeatLevel != 2)
			{
				//m_componentDispenser.HeatLevel = 0f;
				m_component.HeatLevel = 2;
			}
			if (m_3Button.IsClicked && m_component.HeatLevel != 3)
			{
				//m_componentDispenser.HeatLevel = 0f;
				m_component.HeatLevel = 3;
			}
			m_progress.Value = m_component.SmeltingProgress;
			m_1Button.IsChecked = m_component.HeatLevel == 1;
			m_2Button.IsChecked = m_component.HeatLevel == 2;
			m_3Button.IsChecked = m_component.HeatLevel == 3;
		}
	}
}