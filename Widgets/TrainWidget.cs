using Engine;
using System.Xml.Linq;

namespace Game
{
	public class TrainWidget : CanvasWidget
	{
		protected readonly ComponentEngine3 m_componentFurnace;

		protected readonly FireWidget m_fire;

		protected readonly InventorySlotWidget m_fuelSlot;

		protected readonly GridPanelWidget m_furnaceGrid;

		protected readonly GridPanelWidget m_inventoryGrid;

		protected readonly ValueBarWidget m_progress;

		//protected readonly InventorySlotWidget m_remainsSlot;

		protected readonly InventorySlotWidget m_resultSlot;

		public TrainWidget(IInventory inventory, ComponentEngine3 componentFurnace)
		{
			m_componentFurnace = componentFurnace;
			WidgetsManager.LoadWidgetContents(this, this, ContentManager.Get<XElement>("Widgets/TrainWidget"));
			m_inventoryGrid = Children.Find<GridPanelWidget>("InventoryGrid", true);
			m_furnaceGrid = Children.Find<GridPanelWidget>("FurnaceGrid", true);
			m_fire = Children.Find<FireWidget>("Fire", true);
			m_progress = Children.Find<ValueBarWidget>("Progress", true);
			m_resultSlot = Children.Find<InventorySlotWidget>("ResultSlot", true);
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
			int num3 = 0;
			for (int k = 0; k < m_furnaceGrid.RowsCount; k++)
			{
				for (int l = 0; l < m_furnaceGrid.ColumnsCount; l++)
				{
					var inventorySlotWidget2 = new InventorySlotWidget();
					inventorySlotWidget2.AssignInventorySlot(componentFurnace, num3++);
					Log.Information(num3.ToString());
					m_furnaceGrid.Children.Add(inventorySlotWidget2);
					m_furnaceGrid.SetWidgetCell(inventorySlotWidget2, new Point2(l, k));
				}
			}
			m_fuelSlot.AssignInventorySlot(componentFurnace, componentFurnace.FuelSlotIndex);
			m_resultSlot.AssignInventorySlot(componentFurnace, componentFurnace.ResultSlotIndex);
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
