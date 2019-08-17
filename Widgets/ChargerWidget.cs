using Engine;
using System.Xml.Linq;
using static Game.Charger;

namespace Game
{
	public class ChargerWidget : CanvasWidget
	{
		protected readonly ComponentBlockEntity m_componentBlockEntity;

		protected readonly ComponentCharger m_componentDispenser;

		protected readonly GridPanelWidget m_dispenserGrid;

		protected readonly GridPanelWidget m_inventoryGrid;

		protected readonly ButtonWidget m_dispenseButton,
										m_shootButton;

		protected readonly InventorySlotWidget m_drillSlot;

		//protected readonly ValueBarWidget m_progress;

		public ChargerWidget(IInventory inventory, ComponentCharger component)
		{
			//m_componentDispenser = componentDispenser;
			m_componentDispenser = component;
			m_componentBlockEntity = component.Entity.FindComponent<ComponentBlockEntity>(true);
			WidgetsManager.LoadWidgetContents(this, this, ContentManager.Get<XElement>("Widgets/ChargerWidget"));
			m_inventoryGrid = Children.Find<GridPanelWidget>("InventoryGrid");
			m_dispenserGrid = Children.Find<GridPanelWidget>("DispenserGrid");
			m_dispenseButton = Children.Find<ButtonWidget>("DispenseButton");
			m_shootButton = Children.Find<ButtonWidget>("ShootButton");
			//m_progress = Children.Find<ValueBarWidget>("Progress");
			//m_acceptsDropsBox = Children.Find<CheckboxWidget>("AcceptsDropsBox");
			//m_drillSlot = Children.Find<InventorySlotWidget>("DrillSlot");
			int num = 6, y, x;
			InventorySlotWidget inventorySlotWidget;
			for (y = 0; y < m_inventoryGrid.RowsCount; y++)
			{
				for (x = 0; x < m_inventoryGrid.ColumnsCount; x++)
				{
					inventorySlotWidget = new InventorySlotWidget();
					inventorySlotWidget.AssignInventorySlot(inventory, num++);
					m_inventoryGrid.Children.Add(inventorySlotWidget);
					m_inventoryGrid.SetWidgetCell(inventorySlotWidget, new Point2(x, y));
				}
			}
			num = 0;
			for (y = 0; y < m_dispenserGrid.RowsCount; y++)
			{
				for (x = 0; x < m_dispenserGrid.ColumnsCount; x++)
				{
					inventorySlotWidget = new InventorySlotWidget();
					inventorySlotWidget.AssignInventorySlot(component, num++);
					m_dispenserGrid.Children.Add(inventorySlotWidget);
					m_dispenserGrid.SetWidgetCell(inventorySlotWidget, new Point2(x, y));
				}
			}
		}

		public override void Update()
		{
			if (!m_componentDispenser.IsAddedToProject)
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
				m_componentDispenser.Charged = true;
			}
			if (m_shootButton.IsClicked && mode == MachineMode.Charge)
			{
				data = SetMode(data);
				Utils.Terrain.SetCellValueFast(coordinates.X, coordinates.Y, coordinates.Z, Terrain.ReplaceData(value, data));
				m_componentDispenser.Charged = false;
			}

			//m_progress.Value = m_componentDispenser2.m_fireTimeRemaining;
			m_dispenseButton.IsChecked = mode == MachineMode.Charge;
			m_componentDispenser.Charged = mode == MachineMode.Charge;
			m_shootButton.IsChecked = mode == MachineMode.Discharger;
		}
	}
}