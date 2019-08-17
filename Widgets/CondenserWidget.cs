using Engine;
using System.Xml.Linq;
using static Game.Charger;

namespace Game
{
	public class CondenserWidget : CanvasWidget
	{
		protected readonly ComponentBlockEntity m_componentBlockEntity;

		protected readonly ComponentCondenser m_component;

		protected readonly ButtonWidget m_dispenseButton;

		protected readonly GridPanelWidget m_inventoryGrid;

		protected readonly ButtonWidget m_shootButton;

		protected readonly ValueBarWidget m_progress;

		public CondenserWidget(IInventory inventory, ComponentCondenser component)
		{
			m_component = component;
			m_componentBlockEntity = component.Entity.FindComponent<ComponentBlockEntity>(true);
			WidgetsManager.LoadWidgetContents(this, this, ContentManager.Get<XElement>("Widgets/CondenserWidget"));
			m_inventoryGrid = Children.Find<GridPanelWidget>("InventoryGrid");
			m_dispenseButton = Children.Find<ButtonWidget>("DispenseButton");
			m_shootButton = Children.Find<ButtonWidget>("ShootButton");
			m_progress = Children.Find<ValueBarWidget>("Progress");

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
			m_dispenseButton.IsChecked = mode == MachineMode.Charge;
			m_component.Charged = mode == MachineMode.Charge;
			m_shootButton.IsChecked = mode == MachineMode.Discharger;
		}
	}
}