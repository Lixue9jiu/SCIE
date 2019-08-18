using Engine;

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
			int num = 0, y, x;
			for (y = 0; y < m_furnaceGrid.RowsCount; y++)
			{
				for (x = 0; x < m_furnaceGrid.ColumnsCount; x++)
				{
					var inventorySlotWidget = new InventorySlotWidget();
					inventorySlotWidget.AssignInventorySlot(component, num++);
					m_furnaceGrid.Children.Add(inventorySlotWidget);
					m_furnaceGrid.SetWidgetCell(inventorySlotWidget, new Point2(x, y));
				}
			}
			m_remainsSlot.AssignInventorySlot(component, component.RemainsSlotIndex);
			m_resultSlot.AssignInventorySlot(component, component.ResultSlotIndex);
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
}