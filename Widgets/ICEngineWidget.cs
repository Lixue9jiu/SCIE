using Engine;
using System.Xml.Linq;

namespace Game
{
	public class ICEngineWidget : CanvasWidget
	{
		protected readonly CheckboxWidget m_acceptsDropsBox;

		protected readonly ComponentBlockEntity m_componentBlockEntity;

		protected readonly ComponentMachine m_componentDispenser;

		protected readonly ButtonWidget m_dispenseButton;

		protected readonly GridPanelWidget m_inventoryGrid;

		protected readonly GridPanelWidget m_furnaceGrid;

		protected readonly ButtonWidget m_shootButton;

		protected readonly SubsystemTerrain m_subsystemTerrain;

		protected readonly ValueBarWidget m_progress;

		protected readonly InventorySlotWidget m_resultSlot;

		//protected readonly ValueBarWidget m_progress;
		protected readonly FireWidget m_fire;

		public ICEngineWidget(IInventory inventory, ComponentMachine component)
		{
			m_componentDispenser = component;
			m_componentBlockEntity = component.Entity.FindComponent<ComponentBlockEntity>(true);
			m_subsystemTerrain = component.Project.FindSubsystem<SubsystemTerrain>(true);
			WidgetsManager.LoadWidgetContents(this, this, ContentManager.Get<XElement>("Widgets/ICEngineWidget"));
			m_inventoryGrid = Children.Find<GridPanelWidget>("InventoryGrid");
			m_furnaceGrid = Children.Find<GridPanelWidget>("DispenserGrid");
			m_dispenseButton = Children.Find<ButtonWidget>("DispenseButton");
			m_fire = Children.Find<FireWidget>("Fire");
			m_shootButton = Children.Find<ButtonWidget>("ShootButton");
			m_resultSlot = Children.Find<InventorySlotWidget>("ResultSlot");
			m_progress = Children.Find<ValueBarWidget>("Progress");

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
			for (y = 0; y < m_furnaceGrid.RowsCount; y++)
			{
				for (x = 0; x < m_furnaceGrid.ColumnsCount; x++)
				{
					inventorySlotWidget = new InventorySlotWidget();
					inventorySlotWidget.AssignInventorySlot(component, num++);
					m_furnaceGrid.Children.Add(inventorySlotWidget);
					m_furnaceGrid.SetWidgetCell(inventorySlotWidget, new Point2(x, y));
				}
			}
			m_resultSlot.AssignInventorySlot(component, component.ResultSlotIndex);
		}

		public override void Update()
		{
			if (!m_componentDispenser.IsAddedToProject)
			{
				ParentWidget.Children.Remove(this);
				return;
			}
			m_fire.ParticlesPerSecond = m_componentDispenser.HeatLevel > 0f ? 24f : 0f;
			m_progress.Value = m_componentDispenser.SmeltingProgress / 1000f;

			if (m_dispenseButton.IsClicked && m_componentDispenser.HeatLevel <= 0f && m_componentDispenser.SmeltingProgress > 0f)
			{
				m_componentDispenser.HeatLevel = 1000f;
			}
			if (m_shootButton.IsClicked && m_componentDispenser.HeatLevel > 0f)
			{
				m_componentDispenser.HeatLevel = 0f;
			}
			m_dispenseButton.IsChecked = m_componentDispenser.HeatLevel != 0f;
		}
	}
}