namespace Game
{
	public class CanpackWidget : EntityWidget<ComponentMachine>
	{
		protected readonly InventorySlotWidget m_result;

		protected readonly ValueBarWidget m_progress;

		protected readonly CheckboxWidget m_acceptsDropsBox;

		public CanpackWidget(IInventory inventory, ComponentMachine component) : base(inventory, component, "Widgets/CanpackWidget")
		{
			m_result = Children.Find<InventorySlotWidget>("ResultSlot");
			m_acceptsDropsBox = Children.Find<CheckboxWidget>("AcceptsElectBox");
			m_progress = Children.Find<ValueBarWidget>("Progress");
			int num = InitGrid();
			m_result.AssignInventorySlot(component, num++);
		}

		public override void Update()
		{
			m_progress.Value = m_component.SmeltingProgress;
			base.Update();
		}
	}

	public class SorterWidget : EntityWidget<ComponentInventoryBase>
	{
		protected readonly InventorySlotWidget m_slot1;
		protected readonly InventorySlotWidget m_slot2;
		protected readonly InventorySlotWidget m_slot3;
		protected readonly InventorySlotWidget m_slot4;

		public SorterWidget(IInventory inventory, ComponentInventoryBase component) : base(inventory, component, "Widgets/SortWidget")
		{
			m_slot1 = Children.Find<InventorySlotWidget>("Slot1");
			m_slot2 = Children.Find<InventorySlotWidget>("Slot2");
			m_slot3 = Children.Find<InventorySlotWidget>("Slot3");
			m_slot4 = Children.Find<InventorySlotWidget>("Slot4");
			//int num = InitGrid();
			int num = 0;
			m_slot1.AssignInventorySlot(component, num++);
			m_slot2.AssignInventorySlot(component, num++);
			m_slot3.AssignInventorySlot(component, num++);
			m_slot4.AssignInventorySlot(component, num++);
		}

		public override void Update()
		{
			
			base.Update();
		}
	}
}