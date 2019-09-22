using Engine;
using static Game.Charger;
using System.Xml.Linq;
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



	public class TurbineWidget : EntityWidget<ComponentTurbine>
	{
		protected readonly ValueBarWidget m_progress;

		public TurbineWidget(IInventory inventory, ComponentTurbine component) : base(inventory, component, "Widgets/TurbineWidget")
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
			Children.Find<LabelWidget>("DispenserLabel2").Text = "Voltage " + (m_component.Output*60).ToString() + "V";
			m_progress.Value = 1f - m_component.Output / 30f;
		}
	}



	public class RControlWidget : CanvasWidget
	{
		public SubsystemTerrain m_subsystemTerrain;

		public ComponentRControl m_componentDispenser;

		public ButtonWidget m_FR1;

		public ButtonWidget m_FR0;

		public ButtonWidget m_CR1;

		public ButtonWidget m_CR0;

		public ButtonWidget m_GR1;

		public ButtonWidget m_GR0;
		public ButtonWidget m_AZ5;

		public RControlWidget(IInventory inventory, ComponentRControl componentDispenser)
		{
			m_componentDispenser = componentDispenser;
			//m_componentBlockEntity = componentDispenser.Entity.FindComponent<ComponentBlockEntity>(throwOnError: true);
			//m_subsystemTerrain = componentDispenser.Project.FindSubsystem<SubsystemTerrain>(throwOnError: true);
			XElement node = ContentManager.Get<XElement>("Widgets/RControlWidget");
			WidgetsManager.LoadWidgetContents(this, this, node);
			//m_inventoryGrid = Children.Find<GridPanelWidget>("InventoryGrid");
			//m_dispenserGrid = Children.Find<GridPanelWidget>("DispenserGrid");
			m_FR1 = Children.Find<ButtonWidget>("FR1");
			m_FR0 = Children.Find<ButtonWidget>("FR0");
			m_CR1 = Children.Find<ButtonWidget>("CR1");
			m_CR0 = Children.Find<ButtonWidget>("CR0");
			m_GR1 = Children.Find<ButtonWidget>("GR1");
			m_GR0 = Children.Find<ButtonWidget>("GR0");
			m_AZ5 = Children.Find<ButtonWidget>("AZ5");
			
		}
		public override void Update()
		{
			Children.Find<LabelWidget>("Temperature").Text = "Temperature " + m_componentDispenser.temp.ToString() +"T";
			Children.Find<LabelWidget>("OutputRatio").Text = "OutputRatio " + m_componentDispenser.output.ToString() + "U";
			Children.Find<LabelWidget>("FuelRod").Text = "FuelRod " + m_componentDispenser.fuel.ToString() ;
			Children.Find<LabelWidget>("ControlRod").Text = "ControlRod " + m_componentDispenser.control.ToString();
			Children.Find<LabelWidget>("CarbonRod").Text = "CarbonRod " + m_componentDispenser.carbon.ToString();
			Children.Find<LabelWidget>("RemainFuelRod").Text = "RemainFuelRod " + m_componentDispenser.rfuel.ToString();
			Children.Find<LabelWidget>("RemainControlRod").Text = "RemainControlRod " + m_componentDispenser.rcontrol.ToString();
			Children.Find<LabelWidget>("RemainCarbonRod").Text = "RemainCarbonRod " + m_componentDispenser.rcarbon.ToString();

			if (m_FR1.IsClicked && !m_componentDispenser.fr1 && m_componentDispenser.rfuel>0)
			{
				m_componentDispenser.fr1 = true;
			}
			if (m_FR0.IsClicked && !m_componentDispenser.fr0 && m_componentDispenser.fuel>0)
			{
				m_componentDispenser.fr0 = true;
			}
			if (m_AZ5.IsClicked && !m_componentDispenser.cr1 && m_componentDispenser.rcontrol > 0)
			{
				m_componentDispenser.az5 = true;
			}
			if (m_CR1.IsClicked && !m_componentDispenser.cr1 && m_componentDispenser.rcontrol > 0)
			{
				m_componentDispenser.cr1 = true;
			}
			if (m_CR0.IsClicked && !m_componentDispenser.cr0 && m_componentDispenser.control > 0)
			{
				m_componentDispenser.cr0 = true;
			}
			if (m_GR1.IsClicked && !m_componentDispenser.gr1 && m_componentDispenser.rcarbon > 0)
			{
				m_componentDispenser.gr1 = true;
			}
			if (m_GR0.IsClicked && !m_componentDispenser.gr0 && m_componentDispenser.carbon > 0)
			{
				m_componentDispenser.gr0 = true;
			}
			m_FR1.IsChecked = m_componentDispenser.rfuel == 0;
			m_FR0.IsChecked = m_componentDispenser.fuel == 0;
			m_CR1.IsChecked = m_componentDispenser.rcontrol == 0;
			m_CR0.IsChecked = m_componentDispenser.control == 0;
			m_GR1.IsChecked = m_componentDispenser.rcarbon == 0;
			m_GR0.IsChecked = m_componentDispenser.carbon == 0;
			if (!m_componentDispenser.IsAddedToProject)
			{
				base.ParentWidget.Children.Remove(this);
			}
		}
	}

}