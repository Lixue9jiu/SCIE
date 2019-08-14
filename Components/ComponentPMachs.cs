using Engine;
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
						case SandBlock.Index: text = "Ê¯Ó¢É°"; break;
						case PlanksBlock.Index: text = "Ä¾Ð¼"; break;
						case BrickBlock.Index: text = "Ëé×©"; break;
						case GlassBlock.Index:
						case FramedGlassBlock.Index:
						case WindowBlock.Index: text = "Ëé²£Á§"; break;
						case MalachiteChunkBlock.Index: text = "CopperOrePowder"; break;
						default:
							if (BlocksManager.Blocks[Terrain.ExtractContents(value)] is ChunkBlock block)
							{
								text = block.GetType().Name.Replace("ChunkBlock", "Powder");
							}
							else if (value == ItemBlock.IdTable["»¬Ê¯"])
								text = "»¬Ê¯·Û";
							else if (value == ItemBlock.IdTable["Plaster"])
								text = "Ê¯¸à·Û";
							else
							{
								var item = Item.Block.GetItem(ref value);
								if (item is OreChunk)
									text = item.GetCraftingId().Replace("Chunk", "Powder");
								else if (item is Brick)
									text = "Ëé×©";
							}
							if (!ItemBlock.IdTable.ContainsKey(text))
								return null;
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
					var block = BlocksManager.Blocks[Terrain.ExtractContents(value)];
					if (block.GetExplosionPressure(value) > 0f)
					{
						Point3 coordinates = m_componentBlockEntity.Coordinates;
						Utils.SubsystemExplosions.TryExplodeBlock(coordinates.X, coordinates.Y, coordinates.Z, value);
						return null;
					}
					if (value == IronIngotBlock.Index)
						text = "IronPlate";
					else if (value == CopperIngotBlock.Index)
						text = "CopperPlate";
					else
					{
						var item = Item.Block.GetItem(ref value);
						if (item is MetalIngot)
						{
							text = item.GetCraftingId().Replace("Ingot", "Plate");
							if (!ItemBlock.IdTable.ContainsKey(text))
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
					var item = Item.Block.GetItem(ref value);
					if (item is MetalIngot)
					{
						text = item.GetCraftingId().Replace("Ingot", "Line");
						if (!ItemBlock.IdTable.ContainsKey(text))
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
	public class ComponentHPress : ComponentPresser
	{
		protected override string FindSmeltingRecipe()
		{
			m_speed = 1f;
			m_count = 9;
			string text = null;
			for (int i = 0; i < m_furnaceSize; i++)
			{
				if (GetSlotCount(i) <= 0) continue;
				int value = GetSlotValue(i);
				if (GetSlotCount(i) > 0 && value == ItemBlock.IdTable["Ê¯Ä«"])
				{
					text = "Diamond";
					m_speed = 0.1f;
					m_count = 1;
				}
				else if (value == IronBlock.Index)
				{
					text = "IronPlate";
				}
				else if (value == CopperBlock.Index)
				{
					text = "CopperPlate";
				}
				else if (Terrain.ExtractContents(value) == MetalBlock.Index)
				{
					var type = MetalBlock.GetType(value);
					if (type > 2)
					{
						text = ((Materials)type - 3).ToString() + "Plate";
						if (!ItemBlock.IdTable.ContainsKey(text))
							return null;
					}
				}
			}
			if (text != null)
			{
				Slot slot = m_slots[ResultSlotIndex];
				if (slot.Count != 0 && (slot.Value != ItemBlock.IdTable[text] || slot.Count + m_count >= 40))
				{
					goto a;
				}
				return text;
			}
			a:
			m_count = 1;
			return base.FindSmeltingRecipe();
		}
	}
}