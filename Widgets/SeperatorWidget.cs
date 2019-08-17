using Engine;
using System.Xml.Linq;

namespace Game
{
	public class SeperatorWidget : EntityWidget<ComponentMachine>
	{
		protected readonly GridPanelWidget m_furnaceGrid;

		protected readonly InventorySlotWidget m_result1,
												m_result2,
												m_result3;

		protected readonly InventorySlotWidget m_cir1, m_cir2;

		protected readonly ValueBarWidget m_progress;
		protected readonly CheckboxWidget m_acceptsDropsBox;

		public SeperatorWidget(IInventory inventory, ComponentMachine component, string name = "Seperator", string path = "Widgets/SeperatorWidget") : base(component, path)
		{
			m_furnaceGrid = Children.Find<GridPanelWidget>("FurnaceGrid");
			var label = Children.Find<LabelWidget>("Label1", false);
			if (label != null)
				label.Text = name;
			m_result1 = Children.Find<InventorySlotWidget>("ResultSlot1");
			m_result2 = Children.Find<InventorySlotWidget>("ResultSlot2");
			m_result3 = Children.Find<InventorySlotWidget>("ResultSlot3");
			m_cir1 = Children.Find<InventorySlotWidget>("CircuitSlot1", false);
			m_cir2 = Children.Find<InventorySlotWidget>("CircuitSlot2", false);
			m_acceptsDropsBox = Children.Find<CheckboxWidget>("AcceptsElectBox", false);
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
			m_result1.AssignInventorySlot(component, num++);
			m_result2.AssignInventorySlot(component, num++);
			m_result3.AssignInventorySlot(component, num++);
			m_cir1?.AssignInventorySlot(component, num++);
			m_cir2?.AssignInventorySlot(component, num++);
		}

		public override void Update()
		{
			m_progress.Value = m_component.SmeltingProgress;
			if (!m_component.IsAddedToProject)
				ParentWidget.Children.Remove(this);
		}
	}
}