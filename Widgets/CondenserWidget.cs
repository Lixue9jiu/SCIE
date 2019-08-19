using Engine;
using static Game.Charger;

namespace Game
{
	public class CondenserWidget : MultiStateMachWidget<ComponentCondenser>
	{
		protected readonly ValueBarWidget m_progress;

		public CondenserWidget(IInventory inventory, ComponentCondenser component) : base(inventory, component, "Widgets/CondenserWidget")
		{
			m_progress = Children.Find<ValueBarWidget>("Progress");
		}

		public override void Update()
		{
			if (!m_component.IsAddedToProject)
			{
				ParentWidget.Children.Remove(this);
				return;
			}
			Children.Find<LabelWidget>("DispenserLabel2").Text = m_component.m_fireTimeRemaining.ToString() + "/1M E";
			Point3 coordinates = m_componentBlockEntity.Coordinates;
			int value = Utils.Terrain.GetCellValue(coordinates.X, coordinates.Y, coordinates.Z);
			int data = Terrain.ExtractData(value);
			MachineMode mode = GetMode(data);
			if (m_dispenseButton.IsClicked && mode == MachineMode.Discharger)
			{
				data = SetMode(data);
				Utils.Terrain.SetCellValueFast(coordinates.X, coordinates.Y, coordinates.Z, Terrain.ReplaceData(value, data));
				m_component.Charged = true;
			}
			if (m_shootButton.IsClicked && mode == MachineMode.Charge)
			{
				data = SetMode(data);
				Utils.Terrain.SetCellValueFast(coordinates.X, coordinates.Y, coordinates.Z, Terrain.ReplaceData(value, data));
				m_component.Charged = false;
			}

			m_progress.Value = 1f - m_component.m_fireTimeRemaining / 1000000f;
			m_dispenseButton.IsChecked =
			m_component.Charged = mode == MachineMode.Charge;
			m_shootButton.IsChecked = mode == MachineMode.Discharger;
		}
	}
}