using Engine;
using static Game.Charger;

namespace Game
{
	public class ChargerWidget : EntityWidget<ComponentCharger>
	{
		protected readonly ComponentBlockEntity m_componentBlockEntity;

		protected readonly GridPanelWidget m_dispenserGrid;

		protected readonly ButtonWidget m_dispenseButton,
										m_shootButton;

		protected readonly InventorySlotWidget m_drillSlot;

		//protected readonly ValueBarWidget m_progress;

		public ChargerWidget(IInventory inventory, ComponentCharger component) : base(inventory, component, "Widgets/ChargerWidget")
		{
			m_componentBlockEntity = component.Entity.FindComponent<ComponentBlockEntity>(true);
			m_dispenserGrid = Children.Find<GridPanelWidget>("DispenserGrid");
			m_dispenseButton = Children.Find<ButtonWidget>("DispenseButton");
			m_shootButton = Children.Find<ButtonWidget>("ShootButton");
			//m_progress = Children.Find<ValueBarWidget>("Progress");
			//m_acceptsDropsBox = Children.Find<CheckboxWidget>("AcceptsDropsBox");
			//m_drillSlot = Children.Find<InventorySlotWidget>("DrillSlot");
			int num = 0, y, x;
			for (y = 0; y < m_dispenserGrid.RowsCount; y++)
			{
				for (x = 0; x < m_dispenserGrid.ColumnsCount; x++)
				{
					var inventorySlotWidget = new InventorySlotWidget();
					inventorySlotWidget.AssignInventorySlot(component, num++);
					m_dispenserGrid.Children.Add(inventorySlotWidget);
					m_dispenserGrid.SetWidgetCell(inventorySlotWidget, new Point2(x, y));
				}
			}
		}

		public override void Update()
		{
			if (!m_component.IsAddedToProject)
			{
				ParentWidget.Children.Remove(this);
				return;
			}
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

			//m_progress.Value = m_componentDispenser2.m_fireTimeRemaining;
			m_dispenseButton.IsChecked = mode == MachineMode.Charge;
			m_component.Charged = mode == MachineMode.Charge;
			m_shootButton.IsChecked = mode == MachineMode.Discharger;
		}
	}
}