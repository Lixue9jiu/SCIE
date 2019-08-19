namespace Game
{
	public class DepositWidget : EntityWidget<ComponentMachine>
	{
		protected readonly ValueBarWidget m_progress;

		protected readonly InventorySlotWidget m_remainsSlot,
												m_resultSlot;

		protected readonly CheckboxWidget m_acceptsDropsBox;

		public DepositWidget(IInventory inventory, ComponentMachine component) : base(inventory, component, "Widgets/ElectricFurnaceWidget")
		{
			m_progress = Children.Find<ValueBarWidget>("Progress");
			m_resultSlot = Children.Find<InventorySlotWidget>("ResultSlot");
			m_remainsSlot = Children.Find<InventorySlotWidget>("RemainsSlot");
			m_acceptsDropsBox = Children.Find<CheckboxWidget>("AcceptsElectBox");
			InitGrid("FurnaceGrid");
			m_resultSlot.AssignInventorySlot(component, component.ResultSlotIndex);
			m_remainsSlot.AssignInventorySlot(component, component.RemainsSlotIndex);
		}

		public override void Update()
		{
			//m_fire.ParticlesPerSecond = m_component.HeatLevel > 0f ? 24f : 0f;
			m_progress.Value = m_component.SmeltingProgress;
			if (!m_component.IsAddedToProject)
				ParentWidget.Children.Remove(this);
		}
	}
}