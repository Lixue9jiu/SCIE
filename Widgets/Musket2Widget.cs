using Engine;
using System.Xml.Linq;

namespace Game
{
	public class Musket2Widget : CanvasWidget
	{
		private readonly LabelWidget m_instructionsLabel;

		private readonly IInventory m_inventory;

		private readonly GridPanelWidget m_inventoryGrid;

		private readonly InventorySlotWidget m_inventorySlotWidget;

		private readonly int m_slotIndex;

		public Musket2Widget(IInventory inventory, int slotIndex)
		{
			m_inventory = inventory;
			m_slotIndex = slotIndex;
			WidgetsManager.LoadWidgetContents(this, this, ContentManager.Get<XElement>("Widgets/Musket2Widget"));
			m_inventoryGrid = Children.Find<GridPanelWidget>("InventoryGrid", true);
			m_inventorySlotWidget = Children.Find<InventorySlotWidget>("InventorySlot", true);
			m_instructionsLabel = Children.Find<LabelWidget>("InstructionsLabel", true);
			for (int i = 0; i < m_inventoryGrid.RowsCount; i++)
			{
				for (int j = 0; j < m_inventoryGrid.ColumnsCount; j++)
				{
					InventorySlotWidget widget = new InventorySlotWidget();
					m_inventoryGrid.Children.Add(widget);
					m_inventoryGrid.SetWidgetCell(widget, new Point2(j, i));
				}
			}
			int num = 6;
			foreach (Widget child in m_inventoryGrid.Children)
			{
				InventorySlotWidget inventorySlotWidget = child as InventorySlotWidget;
				if (inventorySlotWidget != null)
				{
					inventorySlotWidget.AssignInventorySlot(inventory, num++);
				}
			}
			m_inventorySlotWidget.AssignInventorySlot(inventory, slotIndex);
			m_inventorySlotWidget.CustomViewMatrix = Matrix.CreateLookAt(new Vector3(1f, 0f, 0f), new Vector3(0f, 0f, 0f), -Vector3.UnitZ);
		}

		public override void Update()
		{
			int slotValue = m_inventory.GetSlotValue(m_slotIndex);
			int slotCount = m_inventory.GetSlotCount(m_slotIndex);
			if (Terrain.ExtractContents(slotValue) == 524 && slotCount > 0)
			{
				switch (Musket2Block.GetLoadState(Terrain.ExtractData(slotValue)))
				{
				case Musket2Block.LoadState.Empty:
					m_instructionsLabel.Text = "Load bullet";
					break;
				case Musket2Block.LoadState.Gunpowder:
					m_instructionsLabel.Text = "Fire or Load bullet";
					break;
				case Musket2Block.LoadState.Wad:
					m_instructionsLabel.Text = "Ready for Fire";
					break;
				default:
					m_instructionsLabel.Text = string.Empty;
					break;
				}
			}
			else
			{
				ParentWidget.Children.Remove(this);
			}
		}
	}
}
