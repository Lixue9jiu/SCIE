namespace Game
{
	public class Musket3Widget : Musket2Widget
	{
		public Musket3Widget(IInventory inventory, int slotIndex) : base(inventory, slotIndex)
		{
		}

		public override void Update()
		{
			int slotValue = m_inventory.GetSlotValue(m_slotIndex);
			if (Terrain.ExtractContents(slotValue) == Musket3Block.Index && m_inventory.GetSlotCount(m_slotIndex) > 0)
			{
				switch (Musket2Block.GetLoadState(Terrain.ExtractData(slotValue)))
				{
				case Musket2Block.LoadState.Empty:
					m_instructionsLabel.Text = "Load bullet";
					break;
				case Musket2Block.LoadState.Bullet:
					m_instructionsLabel.Text = "Fire or Load bullet";
					break;
				case Musket2Block.LoadState.Bullet2:
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