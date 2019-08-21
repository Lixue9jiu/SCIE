using Engine;
namespace Game
{
	public class SeparatorWidget : ProcessWidget<ComponentMachine>
	{
		protected readonly CheckboxWidget m_acceptsDropsBox;

		public SeparatorWidget(IInventory inventory, ComponentMachine component, string name = "Separator", string path = "Widgets/SeparatorWidget") : base(inventory, component, path)
		{
			var label = Children.Find<LabelWidget>("Label1", false);
			if (label != null)
				label.Text = name;
			m_acceptsDropsBox = Children.Find<CheckboxWidget>("AcceptsElectBox", false);
			int num = InitGrid();
			m_result1?.AssignInventorySlot(component, num++);
			m_result2?.AssignInventorySlot(component, num++);
			m_result3?.AssignInventorySlot(component, num++);
			m_cir1?.AssignInventorySlot(component, num++);
			m_cir2?.AssignInventorySlot(component, num++);
		}

		public override void Update()
		{
			m_progress.Value = m_component.SmeltingProgress;
			base.Update();
		}
	}

	public class RecyclerWidget : EntityWidget<ComponentMachine>
	{
		protected readonly CheckboxWidget m_acceptsDropsBox;
		protected readonly ValueBarWidget m_progress;
		protected readonly InventorySlotWidget m_result1,
												m_cir1, m_cir2;

		public RecyclerWidget(IInventory inventory, ComponentMachine component, string name = "Recycler", string path = "Widgets/RecyclerWidget") : base(inventory, component, path)
		{
			var label = Children.Find<LabelWidget>("Label1", false);
			if (label != null)
				label.Text = name;
			m_acceptsDropsBox = Children.Find<CheckboxWidget>("AcceptsElectBox", false);
			m_result1 = Children.Find<InventorySlotWidget>("ResultSlot1");
			m_cir1 = Children.Find<InventorySlotWidget>("CircuitSlot1", false);
			m_cir2 = Children.Find<InventorySlotWidget>("CircuitSlot2", false);
			m_progress = Children.Find<ValueBarWidget>("Progress");
			m_furnaceGrid = Children.Find<GridPanelWidget>("FurnaceGrid");
			int num = 0, y, x;
			
			for (y = 0; y < m_furnaceGrid.RowsCount; y++)
			{
				for (x = 0; x < m_furnaceGrid.ColumnsCount; x++)
				{
					var inventorySlotWidget = new InventorySlotWidget();
					inventorySlotWidget.Size_ = new Vector2(40, 40);
					inventorySlotWidget.AssignInventorySlot(component, num++);
					m_furnaceGrid.Children.Add(inventorySlotWidget);
					m_furnaceGrid.SetWidgetCell(inventorySlotWidget, new Point2(x, y));
				}
			}
			m_result1?.AssignInventorySlot(component, num++);
			m_cir1?.AssignInventorySlot(component, num++);
			m_cir2?.AssignInventorySlot(component, num++);
		}
		
		public override void Update()
		{
			m_progress.Value = m_component.SmeltingProgress;
			base.Update();
		}
	}
}