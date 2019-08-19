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
			if (!m_component.IsAddedToProject)
				ParentWidget.Children.Remove(this);
		}
	}
}