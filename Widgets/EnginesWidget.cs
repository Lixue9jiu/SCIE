namespace Game
{
	public class EngineAWidget : FireBoxWidget<ComponentMachine>
	{
		protected readonly ButtonWidget m_dispenseButton,
										m_shootButton;

		protected readonly InventorySlotWidget m_remainsSlot;

		public EngineAWidget(IInventory inventory, ComponentMachine component, string path = "Widgets/EngineAWidget") : base(inventory, component, path)
		{
			m_dispenseButton = Children.Find<ButtonWidget>("DispenseButton", false);
			m_shootButton = Children.Find<ButtonWidget>("ShootButton", false);
			m_remainsSlot = Children.Find<InventorySlotWidget>("RemainsSlot");
			int num = InitGrid();
			m_remainsSlot.AssignInventorySlot(component, num++);
			m_resultSlot.AssignInventorySlot(component, num++);
		}

		public override void Update()
		{
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
			//m_componentDispenser2.Charged = mode == MachineMode1.Charge;
			m_shootButton.IsChecked = m_component.HeatLevel == 0f;
			if (!m_component.IsAddedToProject)
				ParentWidget.Children.Remove(this);
		}
	}

	public class Engine2Widget : EngineAWidget
	{
		protected new readonly InventorySlotWidget m_remainsSlot;
		public Engine2Widget(IInventory inventory, ComponentMachine component) : base(inventory, component, "Widgets/Engine2Widget")
		{
			m_remainsSlot = Children.Find<InventorySlotWidget>("RemainsSlot");
			m_remainsSlot.AssignInventorySlot(component, component.RemainsSlotIndex);
			m_resultSlot.AssignInventorySlot(component, component.ResultSlotIndex);
		}

		public override void Update()
		{
			m_fire.ParticlesPerSecond = m_component.HeatLevel > 0f ? 24f : 0f;
			m_progress.Value = m_component.SmeltingProgress;
			if (!m_component.IsAddedToProject)
				ParentWidget.Children.Remove(this);
		}
	}

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


	public class EngineTWidget : FireBoxWidget<ComponentMachine>
	{
		protected readonly ButtonWidget m_dispenseButton,
										m_shootButton,
			                            m_dispenseButton2,
										m_shootButton2;
		protected readonly InventorySlotWidget m_remainsSlot;
		public EngineTWidget(IInventory inventory, ComponentMachine component, string path = "Widgets/TractorWidget") : base(inventory, component, path)
		{
			m_dispenseButton = Children.Find<ButtonWidget>("DispenseButton");
			m_shootButton = Children.Find<ButtonWidget>("ShootButton");
			m_dispenseButton2 = Children.Find<ButtonWidget>("DispenseButton2");
			m_shootButton2 = Children.Find<ButtonWidget>("ShootButton2");
			m_remainsSlot = Children.Find<InventorySlotWidget>("RemainsSlot");
			//m_acceptsDropsBox = Children.Find<CheckboxWidget>("AcceptsDropsBox");
			//m_drillSlot = Children.Find<InventorySlotWidget>("DrillSlot");
			int num = InitGrid();
			m_remainsSlot.AssignInventorySlot(component, num++);
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
			if (m_dispenseButton2.IsClicked && m_component.HeatLevel !=500f && m_component.SmeltingProgress > 0f)
			{
				m_component.HeatLevel = 500f;
			}
			if (m_shootButton2.IsClicked && m_component.HeatLevel !=499f && m_component.SmeltingProgress > 0f)
			{
				m_component.HeatLevel = 499f;
			}
			m_dispenseButton.IsChecked = m_component.HeatLevel != 0f;
			m_shootButton.IsChecked = m_component.HeatLevel == 0f;
			m_dispenseButton2.IsChecked = m_component.HeatLevel == 500f;
			m_shootButton2.IsChecked = m_component.HeatLevel == 499f;
		}
	}
}