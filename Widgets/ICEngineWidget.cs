namespace Game
{
	public class ICEngineWidget : FireBoxWidget<ComponentMachine>
	{
		protected readonly ButtonWidget m_dispenseButton,
										m_shootButton;

		public ICEngineWidget(IInventory inventory, ComponentMachine component) : base(inventory, component, "Widgets/ICEngineWidget")
		{
			m_dispenseButton = Children.Find<ButtonWidget>("DispenseButton");
			m_shootButton = Children.Find<ButtonWidget>("ShootButton");
			//m_acceptsDropsBox = Children.Find<CheckboxWidget>("AcceptsDropsBox");
			//m_drillSlot = Children.Find<InventorySlotWidget>("DrillSlot");
			int num = InitGrid();
			m_resultSlot.AssignInventorySlot(component, num++);
		}

		public override void Update()
		{
			if (!m_component.IsAddedToProject)
			{
				ParentWidget.Children.Remove(this);
				return;
			}
			m_fire.ParticlesPerSecond = m_component.HeatLevel > 0f ? 24f : 0f;
			m_progress.Value = m_component.SmeltingProgress / 1000f;

			if (m_dispenseButton.IsClicked && m_component.HeatLevel <= 0f && m_component.SmeltingProgress > 0f)
			{
				m_component.HeatLevel = 1000f;
			}
			if (m_shootButton.IsClicked && m_component.HeatLevel > 0f)
			{
				m_component.HeatLevel = 0f;
			}
			m_dispenseButton.IsChecked = m_component.HeatLevel != 0f;
			m_shootButton.IsChecked = m_component.HeatLevel == 0f;
		}
	}
}