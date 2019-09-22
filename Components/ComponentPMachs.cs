using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	public class ComponentKibbler : ComponentPMach
	{
		protected override string FindSmeltingRecipe()
		{
			string text = null;
			m_count = 2;
			for (int i = 0; i < m_furnaceSize; i++)
			{
				if (GetSlotCount(i) > 0)
				{
					int value = GetSlotValue(i);
					switch (value)
					{
						case SandBlock.Index: text = "QuartzPowder"; break;
						case WoodenStairsBlock.Index:
						case PlanksBlock.Index: text = "Sawdust"; break;
						case BrickBlock.Index: text = "Brickbat"; break;
						case GlassBlock.Index:
						case FramedGlassBlock.Index:
						case WindowBlock.Index: text = "BrokenGlass"; break;
						case MalachiteChunkBlock.Index: text = "CopperOrePowder"; break;
						case IronIngotBlock.Index: text = "Fe"; m_count = 1; break;
						case CopperIngotBlock.Index: text = "Cu"; m_count = 1; break;
						case SulphurChunkBlock.Index: text = "S"; break;
						default:
							if (BlocksManager.Blocks[Terrain.ExtractContents(value)] is ChunkBlock block)
							{
								text = block.GetType().Name.Replace("ChunkBlock", "Powder");
							}
							else if (value == ItemBlock.IdTable["»¬Ê¯"])
								text = "TalcumPowder";
							else if (value == ItemBlock.IdTable["Plaster"])
								text = "Gesso";
							else if (value == 360515 || value == 1074102339 || value == 537231427 || value == 1610973251)
								text = "UraniumOrePowder";
							else
							{
								var item = Item.Block.GetItem(ref value);
								if (item is OreChunk)
									text = item.GetCraftingId().Replace("Chunk", "Powder");
								else if (item is MetalIngot ingot)
								{
									m_count = 1;
									text = ingot.Type.ToId();
								}
								else if (item is Brick)
									goto case BrickBlock.Index;
							}
							if (text == null || !ItemBlock.IdTable.ContainsKey(text))
								return null;
							break;
					}
				}
			}
			if (text != null)
			{
				Slot slot = m_slots[ResultSlotIndex];
				if (slot.Count != 0 && (slot.Value != ItemBlock.IdTable[text] || m_count + slot.Count > 40))
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
				if (GetSlotCount(i) > 0)
				{
					if (GetSlotValue(i) == ItemBlock.IdTable["SteelRod"])
						text = "RifleBarrel";
					else if (GetSlotValue(i) == ItemBlock.IdTable["ÅÚ¸Ö¹÷"])
						text = "Ç¹¹Ü";
					else if (GetSlotValue(i) == ItemBlock.IdTable["Cannon"])
						text = "CannonB";
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
					else if (value == ItemBlock.IdTable["Stainless Steel"])
						text = "²»Ðâ¸Ö°å";
					else if (value == ItemBlock.IdTable["Industrial Steel"])
						text = "¹¤Òµ¸Ö°å";
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
			m_count = 1;
			m_speed = 1f;
			string text = null;
			for (int i = 0; i < m_furnaceSize; i++)
			{
				if (GetSlotCount(i) <= 0) continue;
				int value = GetSlotValue(i);
				if (GetSlotCount(i) > 0 && value == ItemBlock.IdTable["Graphite"])
				{
					text = "Diamond";
					m_speed = 0.1f;
				}
				else if (value == ItemBlock.IdTable["»¬Ê¯"])
					text = "TalcumPowder";
				else if (value == ItemBlock.IdTable["Plaster"])
					text = "Gesso";
				else switch (Terrain.ExtractContents(value))
					{
						case IronBlock.Index:
							m_count = 9;
							goto case IronFenceGateBlock.Index;
						case IronFenceGateBlock.Index:
						case IronHammerBlock.Index:
						case IronLadderBlock.Index:
							text = "IronPlate"; break;
						case CopperBlock.Index:
							m_count = 9;
							goto case EmptyBucketBlock.Index;
						case EmptyBucketBlock.Index:
						case SwitchBlock.Index:
						case ButtonBlock.Index:
							text = "CopperPlate"; break;
						case GlassBlock.Index:
						case FramedGlassBlock.Index:
						case WindowBlock.Index:
							text = "BrokenGlass"; break;
						case BrickBlock.Index:
							text = "Brickbat"; break;
						case BricksBlock.Index:
							m_count = 4;
							goto case BrickBlock.Index;
						case MetalBlock.Index:
							var type = MetalBlock.GetType(value);
							if (type > 2)
							{
								text = ((Materials)type - 3).ToString() + "Plate";
								if (!ItemBlock.IdTable.ContainsKey(text))
									return null;
								m_count = 9;
							}
							break;
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