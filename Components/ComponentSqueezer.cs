using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	public class ComponentSqueezer : ComponentPMach
	{
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			base.Load(valuesDictionary, idToEntityMap);
			m_speed = 0.06f;
		}

		protected override string FindSmeltingRecipe()
		{
			string text = null;
			for (int i = 0; i < m_furnaceSize; i++)
			{
				if (GetSlotCount(i) > 0)
				{
					int slotValue = GetSlotValue(i);
					if (slotValue == IronIngotBlock.Index)
						text = "IronLine";
					else if (slotValue == CopperIngotBlock.Index)
						text = "CopperLine";
					else
					{
						var item = Item.ItemBlock.GetItem(ref slotValue);
						if (item is MetalIngot)
						{
							text = item.GetDisplayName(Utils.SubsystemTerrain, slotValue).Replace("Ingot", "Line");
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