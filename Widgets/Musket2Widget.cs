using Engine;
using System.Xml.Linq;

namespace Game
{
	public class Musket2Widget : CanvasWidget
	{
		protected readonly LabelWidget m_instructionsLabel;

		protected readonly IInventory m_inventory;

		protected readonly GridPanelWidget m_inventoryGrid;

		protected readonly InventorySlotWidget m_inventorySlotWidget;

		protected readonly int m_slotIndex;

		public Musket2Widget(IInventory inventory, int slotIndex)
		{
			m_inventory = inventory;
			m_slotIndex = slotIndex;
			WidgetsManager.LoadWidgetContents(this, this, ContentManager.Get<XElement>("Widgets/Musket2Widget"));
			m_inventoryGrid = Children.Find<GridPanelWidget>("InventoryGrid");
			m_inventorySlotWidget = Children.Find<InventorySlotWidget>("InventorySlot");
			m_instructionsLabel = Children.Find<LabelWidget>("InstructionsLabel");
			int i = 0, num;
			for (; i < m_inventoryGrid.RowsCount; i++)
			{
				for (num = 0; num < m_inventoryGrid.ColumnsCount; num++)
				{
					var widget = new InventorySlotWidget();
					m_inventoryGrid.Children.Add(widget);
					m_inventoryGrid.SetWidgetCell(widget, new Point2(num, i));
				}
			}
			num = 6;
			i = 0;
			for (int count = m_inventoryGrid.Children.Count; i < count; i++)
				if (m_inventoryGrid.Children[i] is InventorySlotWidget inventorySlotWidget)
					inventorySlotWidget.AssignInventorySlot(inventory, num++);
			m_inventorySlotWidget.AssignInventorySlot(inventory, slotIndex);
			m_inventorySlotWidget.CustomViewMatrix = Matrix.CreateLookAt(new Vector3(1f, 0f, 0f), new Vector3(0f, 0f, 0f), -Vector3.UnitZ);
		}

		public override void Update()
		{
			int slotValue = m_inventory.GetSlotValue(m_slotIndex);
			if (Terrain.ExtractContents(slotValue) == Musket2Block.Index && m_inventory.GetSlotCount(m_slotIndex) > 0)
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
				ParentWidget.Children.Remove(this);
		}
	}
}