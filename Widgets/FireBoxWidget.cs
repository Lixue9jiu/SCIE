using Engine;

namespace Game
{
	public class FireBoxWidget<T> : EntityWidget<T> where T : ComponentMachine
	{
		protected readonly FireWidget m_fire;

		protected readonly InventorySlotWidget m_fuelSlot;

		protected readonly ValueBarWidget m_progress;

		protected readonly InventorySlotWidget m_resultSlot;

		public FireBoxWidget(IInventory inventory, T component, string path) : base(component, path)
		{
			m_fire = Children.Find<FireWidget>("Fire");
			m_furnaceGrid = Children.Find<GridPanelWidget>("FurnaceGrid", false);
			m_progress = Children.Find<ValueBarWidget>("Progress");
			m_fuelSlot = Children.Find<InventorySlotWidget>("FuelSlot", false);
			m_resultSlot = Children.Find<InventorySlotWidget>("ResultSlot", false);
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
			m_fuelSlot?.AssignInventorySlot(component, component.FuelSlotIndex);
		}

		public override void Update()
		{
			m_fire.ParticlesPerSecond = m_component.HeatLevel > 0f ? 24f : 0f;
			m_progress.Value = m_component.SmeltingProgress;
			if (!m_component.IsAddedToProject)
				ParentWidget.Children.Remove(this);
		}
	}
}