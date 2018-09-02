using System.Xml.Linq;
using Engine;

namespace Game
{
	public class FireBoxWidget<T> : CanvasWidget where T : ComponentMachine
	{
		protected readonly T m_componentFurnace;

		protected readonly FireWidget m_fire;

		protected readonly InventorySlotWidget m_fuelSlot;

		//protected readonly GridPanelWidget m_furnaceGrid;

		protected readonly GridPanelWidget m_inventoryGrid;

		protected readonly ValueBarWidget m_progress;

		//protected readonly InventorySlotWidget m_remainsSlot;

		//protected readonly InventorySlotWidget m_resultSlot;

		public FireBoxWidget(IInventory inventory, T component, string path)
		{
			m_componentFurnace = component;
			WidgetsManager.LoadWidgetContents(this, this, ContentManager.Get<XElement>(path));
			m_inventoryGrid = Children.Find<GridPanelWidget>("InventoryGrid", true);
			m_fire = Children.Find<FireWidget>("Fire", true);
			m_progress = Children.Find<ValueBarWidget>("Progress", true);
			m_fuelSlot = Children.Find<InventorySlotWidget>("FuelSlot", true);
			int num = 6;
			for (int i = 0; i < m_inventoryGrid.RowsCount; i++)
			{
				for (int j = 0; j < m_inventoryGrid.ColumnsCount; j++)
				{
					var inventorySlotWidget = new InventorySlotWidget();
					inventorySlotWidget.AssignInventorySlot(inventory, num++);
					m_inventoryGrid.Children.Add(inventorySlotWidget);
					m_inventoryGrid.SetWidgetCell(inventorySlotWidget, new Point2(j, i));
				}
			}
			m_fuelSlot.AssignInventorySlot(component, component.FuelSlotIndex);
		}

		public override void Update()
		{
			m_fire.ParticlesPerSecond = (m_componentFurnace.HeatLevel > 0f) ? 24f : 0f;
			m_progress.Value = m_componentFurnace.SmeltingProgress;
			if (!m_componentFurnace.IsAddedToProject)
			{
				ParentWidget.Children.Remove(this);
			}
		}
	}
}
