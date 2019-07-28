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
					int value = GetSlotValue(i);
					switch (value)
					{
						case IronOreChunkBlock.Index: text = "IronOrePowder"; break;
						case MalachiteChunkBlock.Index: text = "CopperOrePowder"; break;
						case GermaniumOreChunkBlock.Index: text = "GermaniumOrePowder"; break;
						case CoalChunkBlock.Index: text = "CoalPowder"; break;
						case SulphurChunkBlock.Index: text = "SulphurPowder"; break;
						default:
							var item = Item.ItemBlock.GetItem(ref value);
							if (item is OreChunk)
							{
								text = item.GetCraftingId().Replace("Chunk", "Powder");
								if (!ItemBlock.IdTable.TryGetValue(text, out value))
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
	public class ComponentPresserNN : ComponentPMach
	{
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			base.Load(valuesDictionary, idToEntityMap);
			m_speed = .01f;
		}

		protected override string FindSmeltingRecipe()
		{
			string text = null;
			for (int i = 0; i < m_furnaceSize; i++)
				if (GetSlotCount(i) > 0 && GetSlotValue(i) == ItemBlock.IdTable["SteelRod"])
					text = "RifleBarrel";
			if (text != null)
			{
				Slot slot = m_slots[ResultSlotIndex];
				if (slot.Count != 0 && (slot.Value != ItemBlock.IdTable[text] || slot.Count >= 40))
					return null;
			}
			return text;
		}
	}
	public class ComponentPresser : ComponentPMach
	{
		protected override string FindSmeltingRecipe()
		{
			string text = null;
			for (int i = 0; i < m_furnaceSize; i++)
			{
				if (GetSlotCount(i) > 0)
				{
					int value = GetSlotValue(i);
					if (value == IronIngotBlock.Index)
						text = "IronPlate";
					else if (value == CopperIngotBlock.Index)
						text = "CopperPlate";
					else
					{
						var item = Item.ItemBlock.GetItem(ref value);
						if (item is MetalIngot)
						{
							text = item.GetCraftingId().Replace("Ingot", "Plate");
							if (!ItemBlock.IdTable.TryGetValue(text, out value))
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
	public class ComponentSqueezer : ComponentPMach
	{
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			base.Load(valuesDictionary, idToEntityMap);
			m_speed = .06f;
		}

		protected override string FindSmeltingRecipe()
		{
			string text = null;
			for (int i = 0; i < m_furnaceSize; i++)
			{
				if (GetSlotCount(i) <= 0) continue;
				int value = GetSlotValue(i);
				if (value == IronIngotBlock.Index)
					text = "IronLine";
				else if (value == CopperIngotBlock.Index)
					text = "CopperLine";
				else
				{
					var item = Item.ItemBlock.GetItem(ref value);
					if (item is MetalIngot)
					{
						text = item.GetDisplayName(Utils.SubsystemTerrain, value).Replace("Ingot", "Line");
						if (!ItemBlock.IdTable.TryGetValue(text, out value))
							return null;
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