using Engine;
using GameEntitySystem;
using System.Xml.Linq;

namespace Game
{
	//´øFuelSlot
	public class StoveWidget : PresserWidget
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

	public class PresserWidget : EntityWidget<ComponentMachine>
	{
		protected readonly FireWidget m_fire;

		protected readonly ValueBarWidget m_progress;

		//protected readonly InventorySlotWidget m_remainsSlot;

		protected readonly InventorySlotWidget m_resultSlot;

		public PresserWidget(IInventory inventory, ComponentMachine component, string path = "Widgets/PresserWidget") : base(inventory, component, path)
		{
			if (path == null)
				Children.Find<LabelWidget>("ChestLabel").Text = component.ValuesDictionary.DatabaseObject.Name;
			if (Utils.TR.Count < 3)
				Children.Find<LabelWidget>("InventoryLabel").Text = "±³°ü";
			m_fire = Children.Find<FireWidget>("Fire");
			m_progress = Children.Find<ValueBarWidget>("Progress");
			m_resultSlot = Children.Find<InventorySlotWidget>("ResultSlot");
			InitGrid("FurnaceGrid");
			m_resultSlot.AssignInventorySlot(component, component.ResultSlotIndex);
		}

		public override void Update()
		{
			base.Update();
			m_fire.ParticlesPerSecond = m_component.HeatLevel > 0f ? 24f : 0f;
			m_progress.Value = m_component.SmeltingProgress;
		}
	}

	public class EntityWidget<T> : CanvasWidget where T : Component
	{
		protected readonly T m_component;
		protected readonly GridPanelWidget m_inventoryGrid;
		protected GridPanelWidget m_furnaceGrid;

		public EntityWidget(IInventory inventory, T component, string path)
		{
			m_component = component;
			WidgetsManager.LoadWidgetContents(this, this, ContentManager.Get<XElement>(path));
			m_inventoryGrid = Children.Find<GridPanelWidget>("InventoryGrid");
			int num = 6, y, x;
			for (y = 0; y < m_inventoryGrid.RowsCount; y++)
			{
				for (x = 0; x < m_inventoryGrid.ColumnsCount; x++)
				{
					var inventorySlotWidget = new InventorySlotWidget();
					inventorySlotWidget.AssignInventorySlot(inventory, num++);
					m_inventoryGrid.Children.Add(inventorySlotWidget);
					m_inventoryGrid.SetWidgetCell(inventorySlotWidget, new Point2(x, y));
				}
			}
		}

		public int InitGrid(string name = "FurnaceGrid")
		{
			m_furnaceGrid = Children.Find<GridPanelWidget>(name);
			int num = 0, y, x;
			var component = (IInventory)m_component;
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
			return num;
		}

		public override void Update()
		{
			if (!m_component.IsAddedToProject)
				ParentWidget.Children.Remove(this);
		}
	}

	public class ProcessWidget<T> : EntityWidget<T> where T : ComponentInventoryBase
	{
		protected readonly ValueBarWidget m_progress;
		protected readonly InventorySlotWidget m_result1,
												m_result2,
												m_result3,
												m_cir1, m_cir2;

		public ProcessWidget(IInventory inventory, T component, string path) : base(inventory, component, path)
		{
			m_progress = Children.Find<ValueBarWidget>("Progress");
			m_result1 = Children.Find<InventorySlotWidget>("ResultSlot1");
			m_result2 = Children.Find<InventorySlotWidget>("ResultSlot2");
			m_result3 = Children.Find<InventorySlotWidget>("ResultSlot3");
			m_cir1 = Children.Find<InventorySlotWidget>("CircuitSlot1", false);
			m_cir2 = Children.Find<InventorySlotWidget>("CircuitSlot2", false);
		}
	}

	public class MultiStateMachWidget<T> : EntityWidget<T> where T : Component
	{
		protected readonly ComponentBlockEntity m_componentBlockEntity;

		protected readonly ButtonWidget m_dispenseButton,
										m_shootButton;

		public MultiStateMachWidget(IInventory inventory, T component, string path) : base(inventory, component, path)
		{
			m_componentBlockEntity = component.Entity.FindComponent<ComponentBlockEntity>(false);
			m_dispenseButton = Children.Find<ButtonWidget>("DispenseButton");
			m_shootButton = Children.Find<ButtonWidget>("ShootButton");
		}
	}
}