namespace Game
{
	public class FurnaceNWidget : EntityWidget<ComponentFurnace>
	{
		protected readonly FireWidget m_fire;

		protected readonly InventorySlotWidget m_fuelSlot;

		protected readonly ValueBarWidget m_progress;

		protected readonly InventorySlotWidget m_remainsSlot,
												m_resultSlot;

		public FurnaceNWidget(IInventory inventory, ComponentFurnace component) : base(inventory, component, "Widgets/FurnaceNWidget")
		{
			m_fire = Children.Find<FireWidget>("Fire");
			m_progress = Children.Find<ValueBarWidget>("Progress");
			m_resultSlot = Children.Find<InventorySlotWidget>("ResultSlot");
			m_remainsSlot = Children.Find<InventorySlotWidget>("RemainsSlot");
			m_fuelSlot = Children.Find<InventorySlotWidget>("FuelSlot");
			InitGrid();
			m_fuelSlot.AssignInventorySlot(component, component.FuelSlotIndex);
			m_resultSlot.AssignInventorySlot(component, component.ResultSlotIndex);
			m_remainsSlot.AssignInventorySlot(component, component.RemainsSlotIndex);
		}

		public override void Update()
		{
			m_fire.ParticlesPerSecond = m_component.HeatLevel > 0f ? 24f : 0f;
			m_progress.Value = m_component.SmeltingProgress;
			if (!m_component.IsAddedToProject)
				ParentWidget.Children.Remove(this);
		}
	}
}