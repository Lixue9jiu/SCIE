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
			base.Update();
		}
	}

	public class ElectricFurnaceWidget : EntityWidget<ComponentElectricFurnace>
	{
		protected readonly FireWidget m_fire;

		protected readonly InventorySlotWidget m_remainsSlot,
												m_resultSlot;

		protected readonly CheckboxWidget m_acceptsDropsBox;
		protected readonly ValueBarWidget m_progress;
		protected readonly InventorySlotWidget m_cir1,
												m_cir2;

		public ElectricFurnaceWidget(IInventory inventory, ComponentElectricFurnace component, string path = "Widgets/ElectricFurnaceWidget") : base(inventory, component, path)
		{
			//m_component = component;
			m_fire = Children.Find<FireWidget>("Fire");
			m_resultSlot = Children.Find<InventorySlotWidget>("ResultSlot");
			m_remainsSlot = Children.Find<InventorySlotWidget>("RemainsSlot");
			m_acceptsDropsBox = Children.Find<CheckboxWidget>("AcceptsElectBox");
			m_cir1 = Children.Find<InventorySlotWidget>("CircuitSlot1", false);
			m_cir2 = Children.Find<InventorySlotWidget>("CircuitSlot2", false);
			m_progress = Children.Find<ValueBarWidget>("Progress");
			InitGrid();
			m_resultSlot.AssignInventorySlot(component, component.ResultSlotIndex);
			m_remainsSlot.AssignInventorySlot(component, component.RemainsSlotIndex);
			m_cir1.AssignInventorySlot(component, component.Cir1SlotIndex);
			m_cir2.AssignInventorySlot(component, component.Cir2SlotIndex);
		}

		public override void Update()
		{
			m_fire.ParticlesPerSecond = m_component.HeatLevel > 0f ? 24f : 0f;
			m_progress.Value = m_component.SmeltingProgress;
			base.Update();
		}
	}
}