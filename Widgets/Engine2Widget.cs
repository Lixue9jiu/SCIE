namespace Game
{
	public class Engine2Widget : EngineAWidget
	{
		protected readonly InventorySlotWidget m_fuelSlot;

		public Engine2Widget(IInventory inventory, ComponentMachine componentFurnace) : base(inventory, componentFurnace, "Widgets/Engine2Widget")
		{
			m_fuelSlot = Children.Find<InventorySlotWidget>("FuelSlot");
			m_fuelSlot.AssignInventorySlot(componentFurnace, componentFurnace.FuelSlotIndex);
		}

		public override void Update()
		{
			m_fire.ParticlesPerSecond = m_componentFurnace.HeatLevel > 0f ? 24f : 0f;
			m_progress.Value = m_componentFurnace.SmeltingProgress;
			if (!m_componentFurnace.IsAddedToProject)
				ParentWidget.Children.Remove(this);
		}
	}
}