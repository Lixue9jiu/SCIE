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

	public class RadioCWidget : MultiStateMachWidget<ComponentRadioC>
	{

		public RadioCWidget(IInventory inventory, ComponentRadioC component) : base(inventory, component, "Widgets/RadioC")
		{
			
		}

		public override void Update()
		{
			if (!m_component.IsAddedToProject)
			{
				ParentWidget.Children.Remove(this);
				return;
			}
			Children.Find<LabelWidget>("DispenserLabel2").Text = m_component.m_fireTimeRemaining.ToString() + "/KHz";
			if (m_dispenseButton.IsClicked && m_component.m_fireTimeRemaining<50000f)
			{
				m_component.m_fireTimeRemaining += 1000;
			}
			if (m_shootButton.IsClicked && m_component.m_fireTimeRemaining > 1000f)
			{
				m_component.m_fireTimeRemaining -= 1000;
			}
		}
	}

	public class NuclearWidget : MultiStateMachWidget<ComponentMachine>
	{
		public readonly FireWidget m_fire;
		public NuclearWidget(IInventory inventory, ComponentMachine component) : base(inventory, component, "Widgets/NuclearWidget")
		{
			InitGrid();
			m_fire = Children.Find<FireWidget>("Fire");
		}
		public override void Update()
		{
			if (!m_component.IsAddedToProject)
			{
				ParentWidget.Children.Remove(this);
				return;
			}
			m_fire.m_colorTransform = Color.Green;
			m_fire.ParticlesPerSecond = m_component.HeatLevel > 0f ? 24f : 0f;
			if (m_dispenseButton.IsClicked && m_component.m_fireTimeRemaining == 0f)
			{
				m_component.m_fireTimeRemaining = 1000f;
			}
			if (m_shootButton.IsClicked && m_component.m_fireTimeRemaining == 1000f)
			{
				m_component.m_fireTimeRemaining = 0f;
			}
			m_dispenseButton.IsChecked = m_component.HeatLevel > 0f;
		}
	}

	public class SIWidget : MultiStateMachWidget<ComponentSInserter>
	{
		protected readonly ButtonWidget m_dispenseButton2,
										m_shootButton2;
		protected string tex1, tex2;
		public SIWidget(IInventory inventory, ComponentSInserter component, string text1="In: ",string text2="Out: ") : base(inventory, component, "Widgets/SIWidget")
		{
			InitGrid("ChestGrid");
			m_dispenseButton2 = Children.Find<ButtonWidget>("DispenseButton1");
			m_shootButton2 = Children.Find<ButtonWidget>("ShootButton1");
			tex1 = text1;
			tex2 = text2;
		}
		public override void Update()
		{
			if (!m_component.IsAddedToProject)
			{
				ParentWidget.Children.Remove(this);
				return;
			}
			Children.Find<LabelWidget>("DispenserLabel2").Text = tex1 + m_component.innum.ToString();
			Children.Find<LabelWidget>("DispenserLabel3").Text = tex2 + m_component.outnum.ToString();
			if (m_dispenseButton.IsClicked)
			{
				m_component.innum ++;
			}
			if (m_shootButton.IsClicked && m_component.innum >0)
			{
				m_component.innum --;
			}
			if (m_dispenseButton2.IsClicked)
			{
				m_component.outnum++;
			}
			if (m_shootButton2.IsClicked && m_component.outnum > 0)
			{
				m_component.outnum--;
			}
			//m_dispenseButton.IsChecked = m_component.innum > 0;
		}
	}

	public class CannonWidget : MultiStateMachWidget<ComponentCabinerC>
	{
		protected readonly ButtonWidget m_dispenseButton2,
										m_shootButton2;
		protected string tex1, tex2;
		public CannonWidget(IInventory inventory, ComponentCabinerC component, string text1 = "Angle: ", string text2 = "Pressure: ") : base(inventory, component, "Widgets/SIWidget")
		{
			InitGrid("ChestGrid");
			m_dispenseButton2 = Children.Find<ButtonWidget>("DispenseButton1");
			m_shootButton2 = Children.Find<ButtonWidget>("ShootButton1");
			tex1 = text1;
			tex2 = text2;
		}
		public override void Update()
		{
			if (!m_component.IsAddedToProject)
			{
				ParentWidget.Children.Remove(this);
				return;
			}
			Children.Find<LabelWidget>("DispenserLabel2").Text = tex1 + (m_component.angle+15).ToString();
			Children.Find<LabelWidget>("DispenserLabel3").Text = tex2 + (m_component.presure+40).ToString();
			if (m_dispenseButton.IsClicked && m_component.angle < 70)
			{
				m_component.angle++;
			}
			if (m_shootButton.IsClicked && m_component.angle > 0)
			{
				m_component.angle--;
			}
			if (m_dispenseButton2.IsClicked && m_component.presure < 120)
			{
				m_component.presure++;
			}
			if (m_shootButton2.IsClicked && m_component.presure > 0)
			{
				m_component.presure--;
			}
			//m_dispenseButton.IsChecked = m_component.innum > 0;
		}
	}

	public class RadioRWidget : MultiStateMachWidget<ComponentRadioR>
	{

		public RadioRWidget(IInventory inventory, ComponentRadioR component) : base(inventory, component, "Widgets/RadioC")
		{
			Children.Find<LabelWidget>("DispenserLabel").Text = "RadioReceiver";
		}

		public override void Update()
		{
			if (!m_component.IsAddedToProject)
			{
				ParentWidget.Children.Remove(this);
				return;
			}
			Children.Find<LabelWidget>("DispenserLabel2").Text = m_component.m_fireTimeRemaining.ToString() + "/KHz";
			if (m_dispenseButton.IsClicked && m_component.m_fireTimeRemaining < 50000f)
			{
				m_component.m_fireTimeRemaining += 1000;
			}
			if (m_shootButton.IsClicked && m_component.m_fireTimeRemaining > 1000f)
			{
				m_component.m_fireTimeRemaining -= 1000;
			}
		}
	}

	public class TControlWidget : MultiStateMachWidget<ComponentTControl>
	{

		public TControlWidget(IInventory inventory, ComponentTControl component) : base(inventory, component, "Widgets/RadioC")
		{
			Children.Find<LabelWidget>("DispenserLabel").Text = "Temperature Control";
		}

		public override void Update()
		{
			if (!m_component.IsAddedToProject)
			{
				ParentWidget.Children.Remove(this);
				return;
			}
			Children.Find<LabelWidget>("DispenserLabel2").Text = "Set Limit to "+m_component.m_fireTimeRemaining.ToString() + "/T";
			if (m_dispenseButton.IsClicked && m_component.m_fireTimeRemaining < 3000f)
			{
				m_component.m_fireTimeRemaining += 100;
			}
			if (m_shootButton.IsClicked && m_component.m_fireTimeRemaining > 0f)
			{
				m_component.m_fireTimeRemaining -= 100;
			}
		}
	}

	public class ETrainWidget : MultiStateMachWidget<ComponentEngineE>
	{
		//protected readonly ValueBarWidget m_progress;

		public ETrainWidget(IInventory inventory, ComponentEngineE component) : base(inventory, component, "Widgets/ETrainWidget")
		{
			//InitGrid("DispenserGrid");
			var name = "DispenserGrid";
			m_furnaceGrid = Children.Find<GridPanelWidget>(name);
			int num = 0, y, x;
			//var component = m_component;
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
			//	m_progress = Children.Find<ValueBarWidget>("Progress");
		}

		public override void Update()
		{
			if (!m_component.IsAddedToProject)
			{
				ParentWidget.Children.Remove(this);
				return;
			}
			
			if (m_dispenseButton.IsClicked && m_component.Charged == false)
			{
				//data = SetMode(data);
				//Utils.Terrain.SetCellValueFast(coordinates.X, coordinates.Y, coordinates.Z, Terrain.ReplaceData(value, data));
				m_component.Charged = true;
			}
			if (m_shootButton.IsClicked && m_component.Charged == true)
			{
				//data = SetMode(data);
				//Utils.Terrain.SetCellValueFast(coordinates.X, coordinates.Y, coordinates.Z, Terrain.ReplaceData(value, data));
				m_component.Charged = false;
			}

			//m_progress.Value = 1f - m_component.m_fireTimeRemaining / 1000000f;
			m_dispenseButton.IsChecked = m_component.Charged ;
			m_shootButton.IsChecked = !m_component.Charged;
		}
	}

	public class AGunWidget : MultiStateMachWidget<ComponentMachine>
	{
		//protected readonly ValueBarWidget m_progress;

		public AGunWidget(IInventory inventory, ComponentMachine component) : base(inventory, component, "Widgets/AGunWidget")
		{
			//InitGrid("DispenserGrid");
			var name = "DispenserGrid";
			m_furnaceGrid = Children.Find<GridPanelWidget>(name);
			int num = 0, y, x;
			//var component = m_component;
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
			//	m_progress = Children.Find<ValueBarWidget>("Progress");
		}

		public override void Update()
		{
			if (!m_component.IsAddedToProject)
			{
				ParentWidget.Children.Remove(this);
				return;
			}

			if (m_dispenseButton.IsClicked && m_component.HeatLevel == 0f)
			{
				//data = SetMode(data);
				//Utils.Terrain.SetCellValueFast(coordinates.X, coordinates.Y, coordinates.Z, Terrain.ReplaceData(value, data));
				m_component.HeatLevel = 1f;
			}
			if (m_shootButton.IsClicked && m_component.HeatLevel == 1f)
			{
				//data = SetMode(data);
				//Utils.Terrain.SetCellValueFast(coordinates.X, coordinates.Y, coordinates.Z, Terrain.ReplaceData(value, data));
				m_component.HeatLevel = 0f;
			}

			//m_progress.Value = 1f - m_component.m_fireTimeRemaining / 1000000f;
			m_dispenseButton.IsChecked = m_component.HeatLevel == 1f;
			//m_shootButton.IsChecked = !m_component.Charged;
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

	public class LaserWidget : EntityWidget<ComponentLaserG>
	{
		protected readonly ValueBarWidget m_progress;

		public LaserWidget(IInventory inventory, ComponentLaserG component) : base(inventory, component, "Widgets/TurbineWidget")
		{
			m_progress = Children.Find<ValueBarWidget>("Progress");
			var label = Children.Find<LabelWidget>("DispenserLabel", false);
			if (label != null)
				label.Text = "LaserGenerator";
		}

		public override void Update()
		{
			if (!m_component.IsAddedToProject)
			{
				ParentWidget.Children.Remove(this);
				return;
			}
			Children.Find<LabelWidget>("DispenserLabel2").Text = "Energy " + m_component.m_fireTimeRemaining.ToString() + "/100KE";
			m_progress.Value = 1f - m_component.m_fireTimeRemaining / 100000f;
		}
	}

	public class ALaserWidget : EntityWidget<ComponentALaser>
	{
		protected readonly ValueBarWidget m_progress;

		public ALaserWidget(IInventory inventory, ComponentALaser component) : base(inventory, component, "Widgets/TurbineWidget")
		{
			m_progress = Children.Find<ValueBarWidget>("Progress");
			var label = Children.Find<LabelWidget>("DispenserLabel", false);
			if (label != null)
				label.Text = "AutoLaser";
		}

		public override void Update()
		{
			if (!m_component.IsAddedToProject)
			{
				ParentWidget.Children.Remove(this);
				return;
			}
			Children.Find<LabelWidget>("DispenserLabel2").Text = "Energy " + (m_component.m_fireTimeRemaining).ToString() + "/100KE";
			m_progress.Value = 1f - m_component.m_fireTimeRemaining / 100000f;
		}
	}

	public class RocketWidget : MultiStateMachWidget<ComponentRocketEngine>
	{
		protected readonly InventorySlotWidget m_resultSlot,m_itemSlot;

		public RocketWidget(IInventory inventory, ComponentRocketEngine component) : base(inventory, component, "Widgets/REngineWidget")
		{
			int num = InitGrid();
			m_resultSlot = Children.Find<InventorySlotWidget>("ResultSlot");
			m_resultSlot.AssignInventorySlot(component, num++);
			m_itemSlot = Children.Find<InventorySlotWidget>("ItemSlot");
			m_itemSlot.AssignInventorySlot(component, num++);
		}

		public override void Update()
		{
			if (!m_component.IsAddedToProject)
			{
				ParentWidget.Children.Remove(this);
				return;
			}
			if (m_component.m_slots[0].Value == ItemBlock.IdTable["LH2"] && m_component.m_slots[0].Count>0  && (int)m_component.m_fireTimeRemaining / 1000 < 1000 && (m_component.m_slots[1].Count==0 || m_component.m_slots[1].Value == ItemBlock.IdTable["멀틸"]))
			{
				m_component.m_slots[1].Value = ItemBlock.IdTable["멀틸"];
				m_component.m_slots[1].Count ++;
				m_component.m_slots[0].Count --;
				m_component.m_fireTimeRemaining += 1f * 1000f;
			}
			if (m_component.m_slots[0].Value == ItemBlock.IdTable["LO2"] && m_component.m_slots[0].Count > 0 && (int)m_component.m_fireTimeRemaining % 1000 < 1000 && (m_component.m_slots[1].Count == 0 || m_component.m_slots[1].Value == ItemBlock.IdTable["멀틸"]))
			{
				m_component.m_slots[1].Value = ItemBlock.IdTable["멀틸"];
				m_component.m_slots[1].Count++;
				m_component.m_slots[0].Count--;
				m_component.m_fireTimeRemaining += 1f;
			}
			if (m_dispenseButton.IsClicked && m_component.HeatLevel <= 0f)
			{
				m_component.HeatLevel = 1200f;
			}
			if (m_shootButton.IsClicked && m_component.HeatLevel > 0f)
			{
				m_component.HeatLevel = 0f;
			}
			m_dispenseButton.IsChecked = m_component.HeatLevel != 0f;
			//m_shootButton.IsChecked = m_component.HeatLevel == 0f;
			Children.Find<LabelWidget>("DispenserLabel2").Text = "H2 " + ((int)m_component.m_fireTimeRemaining / 1000).ToString();
			Children.Find<LabelWidget>("DispenserLabel3").Text = "O2 " + ((int)m_component.m_fireTimeRemaining%1000).ToString();

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
				ParentWidget.Children.Remove(this);
			}
		}
	}

}