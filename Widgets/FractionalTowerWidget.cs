namespace Game
{
	public class FractionalTowerWidget : ProcessWidget<ComponentMachine>
	{
		protected readonly ButtonWidget m_1Button,
										m_2Button,
										m_3Button;

		public FractionalTowerWidget(IInventory inventory, ComponentMachine component) : base(inventory, component, "Widgets/FractionalTowerWidget")
		{
			m_1Button = Children.Find<ButtonWidget>("1Button");
			m_2Button = Children.Find<ButtonWidget>("2Button");
			m_3Button = Children.Find<ButtonWidget>("3Button");
			int num = InitGrid();
			m_result1.AssignInventorySlot(component, num++);
			m_result2.AssignInventorySlot(component, num++);
			m_result3.AssignInventorySlot(component, num++);
			m_cir1.AssignInventorySlot(component, num++);
			m_cir2.AssignInventorySlot(component, num++);
		}

		public override void Update()
		{
			if (!m_component.IsAddedToProject)
			{
				ParentWidget.Children.Remove(this);
				return;
			}
			if (m_1Button.IsClicked && m_component.HeatLevel != 1)
			{
				m_component.HeatLevel = 1;
			}
			if (m_2Button.IsClicked && m_component.HeatLevel != 2)
			{
				m_component.HeatLevel = 2;
			}
			if (m_3Button.IsClicked && m_component.HeatLevel != 3)
			{
				m_component.HeatLevel = 3;
			}
			m_progress.Value = m_component.SmeltingProgress;
			m_1Button.IsChecked = m_component.HeatLevel == 1;
			m_2Button.IsChecked = m_component.HeatLevel == 2;
			m_3Button.IsChecked = m_component.HeatLevel == 3;
		}
	}
}