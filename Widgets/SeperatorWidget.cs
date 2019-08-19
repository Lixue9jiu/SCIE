using Engine;

namespace Game
{
	public class SeparatorWidget : ProcessWidget<ComponentMachine>
	{
		protected readonly CheckboxWidget m_acceptsDropsBox;

		public SeparatorWidget(IInventory inventory, ComponentMachine component, string name = "Separator", string path = "Widgets/SeperatorWidget") : base(inventory, component, path)
		{
			var label = Children.Find<LabelWidget>("Label1", false);
			if (label != null)
				label.Text = name;
			m_acceptsDropsBox = Children.Find<CheckboxWidget>("AcceptsElectBox", false);
			int num = 0, y, x;
			for (y = 0; y < m_furnaceGrid.RowsCount; y++)
			{
				for (x = 0; x < m_furnaceGrid.ColumnsCount; x++)
				{
					var inventorySlotWidget = new InventorySlotWidget();
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