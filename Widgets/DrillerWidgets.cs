using Engine;

namespace Game
{
	public class DrillerWidget : MultiStateMachWidget<ComponentInventoryBase>
	{
		protected readonly CheckboxWidget m_acceptsDropsBox;

		protected readonly SubsystemTerrain m_subsystemTerrain;

		protected readonly InventorySlotWidget m_drillSlot;

		public DrillerWidget(IInventory inventory, ComponentInventoryBase component) : base(inventory, component, "Widgets/DrillerWidget")
		{
			m_subsystemTerrain = component.Project.FindSubsystem<SubsystemTerrain>(true);
			m_acceptsDropsBox = Children.Find<CheckboxWidget>("AcceptsDropsBox");
			m_drillSlot = Children.Find<InventorySlotWidget>("DrillSlot");
			InitGrid("DispenserGrid");
			m_drillSlot.AssignInventorySlot(component, 8);
		}

		public override void Update()
		{
			if (!m_component.IsAddedToProject)
			{
				ParentWidget.Children.Remove(this);
				return;
			}
			Point3 coordinates = m_componentBlockEntity.Coordinates;
			int value = m_subsystemTerrain.Terrain.GetCellValue(coordinates.X, coordinates.Y, coordinates.Z);
			int data = Terrain.ExtractData(value);
			if (m_dispenseButton.IsClicked)
			{
				data = DrillerBlock.SetMode(data, MachineMode.Dispense);
				m_subsystemTerrain.ChangeCell(coordinates.X, coordinates.Y, coordinates.Z, Terrain.ReplaceData(value, data));
			}
			if (m_shootButton.IsClicked)
			{
				data = DrillerBlock.SetMode(data, MachineMode.Shoot);
				m_subsystemTerrain.ChangeCell(coordinates.X, coordinates.Y, coordinates.Z, Terrain.ReplaceData(value, data));
			}
			if (m_acceptsDropsBox.IsClicked)
			{
				data = SixDirectionalBlock.SetAcceptsDrops(data, !SixDirectionalBlock.GetAcceptsDrops(data));
				m_subsystemTerrain.ChangeCell(coordinates.X, coordinates.Y, coordinates.Z, Terrain.ReplaceData(value, data));
			}
			var mode = DrillerBlock.GetMode(data);
			m_dispenseButton.IsChecked = mode == MachineMode.Dispense;
			m_shootButton.IsChecked = mode == MachineMode.Shoot;
			m_acceptsDropsBox.IsChecked = SixDirectionalBlock.GetAcceptsDrops(data);
		}
	}

	public class ElectricDrillerWidget : MultiStateMachWidget<ComponentElectricDriller>
	{
		protected readonly CheckboxWidget m_acceptsDropsBox;

		protected readonly SubsystemTerrain m_subsystemTerrain;

		protected readonly InventorySlotWidget m_drillSlot,
												m_batterySlot;

		public ElectricDrillerWidget(IInventory inventory, ComponentElectricDriller component) : base(inventory, component, "Widgets/ElectricDrillerWidget")
		{
			m_drillSlot = Children.Find<InventorySlotWidget>("DrillSlot");
			m_batterySlot = Children.Find<InventorySlotWidget>("BatterySlot");
			InitGrid("DispenserGrid");
			m_drillSlot.AssignInventorySlot(component, 8);
			m_batterySlot.AssignInventorySlot(component, 9);
			m_subsystemTerrain = component.Project.FindSubsystem<SubsystemTerrain>(true);
		}

		public override void Update()
		{
			if (!m_component.IsAddedToProject)
			{
				ParentWidget.Children.Remove(this);
				return;
			}
			if (m_dispenseButton.IsClicked && !m_component.Charged)
			{
				//m_componentDispenser.HeatLevel = 1000f;
				m_component.Charged = true;
			}
			if (m_shootButton.IsClicked && m_component.Charged)
			{
				//m_componentDispenser.HeatLevel = 0f;
				m_component.Charged = false;
			}
			m_dispenseButton.IsChecked = m_component.Charged;
			m_shootButton.IsChecked = !m_component.Charged;
		}
	}
}