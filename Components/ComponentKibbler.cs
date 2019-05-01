using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	public class ComponentKibbler : ComponentPMach
	{
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			base.Load(valuesDictionary, idToEntityMap);
			m_count = 2;
		}

		protected override string FindSmeltingRecipe()
		{
			string text = null;
			for (int i = 0; i < m_furnaceSize; i++)
			{
				if (GetSlotCount(i) > 0)
				{
					int slotValue = GetSlotValue(i);
					switch (slotValue)
					{
						case IronOreChunkBlock.Index: text = "IronOrePowder"; break;
						case MalachiteChunkBlock.Index: text = "CopperOrePowder"; break;
						case GermaniumOreChunkBlock.Index: text = "GermaniumOrePowder"; break;
						case CoalChunkBlock.Index: text = "CoalPowder"; break;
						case SulphurChunkBlock.Index: text = "SulphurPowder"; break;
						default:
							var item = Item.ItemBlock.GetItem(ref slotValue);
							if (item is OreChunk)
							{
								text = item.GetCraftingId().Replace("Chunk", "Powder");
								if (!ItemBlock.IdTable.TryGetValue(text, out slotValue))
									return null;
							}
							break;
					}
				}
			}
			if (text != null)
			{
				Slot slot = m_slots[ResultSlotIndex];
				if (slot.Count != 0 && (slot.Value != ItemBlock.IdTable[text] || 2 + slot.Count > 40))
					return null;
			}
			return text;
		}
	}
}