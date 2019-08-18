namespace Game
{
	public class Engine2Widget : EngineAWidget
	{
		public Engine2Widget(IInventory inventory, ComponentMachine component) : base(inventory, component, "Widgets/Engine2Widget")
		{
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