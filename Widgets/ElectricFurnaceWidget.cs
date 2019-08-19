namespace Game
{
	public class ElectricFurnaceWidget : ProcessWidget<ComponentElectricFurnace>
	{
		protected readonly FireWidget m_fire;

		protected readonly InventorySlotWidget m_remainsSlot,
												m_resultSlot;

		protected readonly CheckboxWidget m_acceptsDropsBox;

		public ElectricFurnaceWidget(IInventory inventory, ComponentElectricFurnace component, string path = "Widgets/ElectricFurnaceWidget") : base(inventory, component, path)
		{
			m_fire = Children.Find<FireWidget>("Fire");
			m_resultSlot = Children.Find<InventorySlotWidget>("ResultSlot");
			m_remainsSlot = Children.Find<InventorySlotWidget>("RemainsSlot");
			m_acceptsDropsBox = Children.Find<CheckboxWidget>("AcceptsElectBox");
			m_resultSlot.AssignInventorySlot(component, component.ResultSlotIndex);
			m_remainsSlot.AssignInventorySlot(component, component.RemainsSlotIndex);
			m_cir1.AssignInventorySlot(component, component.Cir1SlotIndex);
			m_cir2.AssignInventorySlot(component, component.Cir2SlotIndex);
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