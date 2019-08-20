namespace Game
{
	public class FireBoxWidget<T> : EntityWidget<T> where T : ComponentMachine
	{
		public readonly FireWidget m_fire;

		protected readonly InventorySlotWidget m_fuelSlot,
												m_resultSlot;

		protected readonly ValueBarWidget m_progress;

		public FireBoxWidget(IInventory inventory, T component, string path) : base(inventory, component, path)
		{
			m_fire = Children.Find<FireWidget>("Fire");
			m_progress = Children.Find<ValueBarWidget>("Progress");
			m_fuelSlot = Children.Find<InventorySlotWidget>("FuelSlot", false);
			m_resultSlot = Children.Find<InventorySlotWidget>("ResultSlot", false);
			m_fuelSlot?.AssignInventorySlot(component, component.FuelSlotIndex);
		}

		public override void Update()
		{
			m_fire.ParticlesPerSecond = m_component.HeatLevel > 0f ? 24f : 0f;
			m_progress.Value = m_component.SmeltingProgress;
			base.Update();
		}
	}
}