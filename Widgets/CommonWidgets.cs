using Engine;
using GameEntitySystem;
using System.Xml.Linq;

namespace Game
{
	//´øFuelSlot
	public class StoveWidget : PresserWidget<ComponentMachine>
	{
		protected readonly InventorySlotWidget m_fuelSlot;

		public StoveWidget(IInventory inventory, ComponentMachine component, string path, string name = null) : base(inventory, component, path)
		{
			if (name != null)
				Children.Find<LabelWidget>("Label").Text = name;
			m_fuelSlot = Children.Find<InventorySlotWidget>("FuelSlot");
			m_fuelSlot.AssignInventorySlot(component, component.FuelSlotIndex);
		}
	}

	public class PresserWidget<T> : CanvasWidget where T : ComponentMachine
	{
		protected readonly T m_componentFurnace;

		protected readonly FireWidget m_fire;

		protected readonly GridPanelWidget m_furnaceGrid;

		protected readonly GridPanelWidget m_inventoryGrid;

		protected readonly ValueBarWidget m_progress;

		//protected readonly InventorySlotWidget m_remainsSlot;

		protected readonly InventorySlotWidget m_resultSlot;

		public PresserWidget(IInventory inventory, T component, string path = null)
		{
			m_componentFurnace = component;
			WidgetsManager.LoadWidgetContents(this, this, ContentManager.Get<XElement>(path ?? "Widgets/PresserWidget"));
			if (path == null)
				Children.Find<LabelWidget>("ChestLabel").Text = component.ValuesDictionary.DatabaseObject.Name;
			if (Utils.TR.Count != 0)
				Children.Find<LabelWidget>("InventoryLabel").Text = "±³°ü";
			m_inventoryGrid = Children.Find<GridPanelWidget>("InventoryGrid");
			m_furnaceGrid = Children.Find<GridPanelWidget>("FurnaceGrid");
			m_fire = Children.Find<FireWidget>("Fire");
			m_progress = Children.Find<ValueBarWidget>("Progress");
			m_resultSlot = Children.Find<InventorySlotWidget>("ResultSlot");
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
			if (!m_componentFurnace.IsAddedToProject)
				ParentWidget.Children.Remove(this);
			m_fire.ParticlesPerSecond = m_componentFurnace.HeatLevel > 0f ? 24f : 0f;
			m_progress.Value = m_componentFurnace.SmeltingProgress;
		}
	}

	public class EntityWidget<T> : CanvasWidget where T : Component
	{
		protected readonly T m_component;
		protected readonly GridPanelWidget m_inventoryGrid;

		public EntityWidget(T component, string path)
		{
			m_component = component;
			WidgetsManager.LoadWidgetContents(this, this, ContentManager.Get<XElement>(path));
			m_inventoryGrid = Children.Find<GridPanelWidget>("InventoryGrid");
		}

		public override void Update()
		{
			if (!m_component.IsAddedToProject)
				ParentWidget.Children.Remove(this);
		}
	}

	/*public class FurnaceWidget<T> : EntityWidget<T> where T : Component
	{
		public FurnaceWidget(T component) : base(component)
		{
		}

		public override void Update()
		{
			base.Update();
		}
	}*/
}