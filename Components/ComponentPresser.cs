namespace Game
{
	public class ComponentPresser : ComponentPMach
	{
		protected override string FindSmeltingRecipe()
		{
			string text = null;
			for (int i = 0; i < m_furnaceSize; i++)
			{
				if (GetSlotCount(i) > 0)
				{
					int slotValue = GetSlotValue(i);
					if (slotValue == IronIngotBlock.Index)
						text = "IronPlate";
					else if (slotValue == CopperIngotBlock.Index)
						text = "CopperPlate";
					else
					{
						var item = Item.ItemBlock.GetItem(ref slotValue);
						if (item is MetalIngot)
						{
							text = item.GetCraftingId().Replace("Ingot", "Plate");
							if (!ItemBlock.IdTable.TryGetValue(text, out slotValue))
								return null;
						}
					}
				}
			}
			if (text != null)
			{
				Slot slot = m_slots[ResultSlotIndex];
				if (slot.Count != 0 && (slot.Value != ItemBlock.IdTable[text] || slot.Count >= 40))
					return null;
			}
			return text;
		}
	}
}