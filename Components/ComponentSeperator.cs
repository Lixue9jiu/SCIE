using Engine;
using GameEntitySystem;
using System.Collections.Generic;
using TemplatesDatabase;

namespace Game
{
	public class ComponentCentrifugal : ComponentSeparator, IUpdateable
	{
		protected override int FindSmeltingRecipe()
		{
			result.Clear();
			int text = 0;
			const int i = 0;
			if (GetSlotCount(i) <= 0) return 0;
			int value = GetSlotValue(i);
			if (value == ItemBlock.IdTable["UraniumOrePowder"])
			{
				text = 1;
				result[Utils.Random.Bool(.0072f) ? ItemBlock.IdTable["U235P"] : ItemBlock.IdTable["U238P"]] = 1;
			}
			return text != 0 ? FindSmeltingRecipe(text) : base.FindSmeltingRecipe();
		}
	}

	public class ComponentUThicker : ComponentSeparator, IUpdateable
	{
		protected override int FindSmeltingRecipe()
		{
			result.Clear();
			int text = 0;
			int i;
			for (i = 0; i < 1; i++)
			{
				if (GetSlotCount(i) <= 0) continue;
				if (GetSlotValue(i) == ItemBlock.IdTable["U235P"] && GetSlotValue(4) == ItemBlock.IdTable["H2SO4"])
				{
					text = 8;
					result[ItemBlock.IdTable["U235C"]] = 1;
					result[ItemBlock.IdTable["Bottle"]] = 1;
					result[ItemBlock.IdTable["H2SO4"]] = -1;
				}
				if (GetSlotValue(i) == ItemBlock.IdTable["U238P"] && GetSlotValue(4) == ItemBlock.IdTable["H2SO4"])
				{
					text = 9;
					result[ItemBlock.IdTable["U238C"]] = 1;
					result[ItemBlock.IdTable["Bottle"]] = 1;
					result[ItemBlock.IdTable["H2SO4"]] = -1;
				}
			}
			return FindSmeltingRecipe(text);
		}
	}

	public class ComponentSeparator : ComponentSMachine, IUpdateable, IElectricMachine
	{
		public int Cir1SlotIndex => SlotsCount - 2;

		public int Cir2SlotIndex => SlotsCount - 1;

		public bool Powered;

		protected readonly Dictionary<int, int> result = new Dictionary<int, int>();

		protected int m_smeltingRecipe, m_smeltingRecipe2;//, Type;

		//protected int m_music;

		public override int RemainsSlotIndex => 0;

		public override int ResultSlotIndex => SlotsCount - 1;

		public void Update(float dt)
		{
			Point3 coordinates = m_componentBlockEntity.Coordinates;
			int cellValue = Utils.Terrain.GetCellValueFast(coordinates.X, coordinates.Y, coordinates.Z);
			if (ElementBlock.Block.GetDevice(coordinates.X, coordinates.Y, coordinates.Z, cellValue) is Unpacker)
			{
				return;
			}
			if (HeatLevel > 0f)
			{
				m_fireTimeRemaining = MathUtils.Max(0f, m_fireTimeRemaining - dt);
				if (m_fireTimeRemaining == 0f)
					HeatLevel = 0f;
			}
			if (m_updateSmeltingRecipe)
			{
				m_updateSmeltingRecipe = false;
				m_smeltingRecipe2 = FindSmeltingRecipe();
				if (m_smeltingRecipe2 != m_smeltingRecipe)
				{
					m_smeltingRecipe = m_smeltingRecipe2;
					SmeltingProgress = 0f;
					//m_music = 0;
				}
			}
			if (m_smeltingRecipe2 != 0)
			{
				if (!Powered)
				{
					SmeltingProgress = 0f;
					HeatLevel = 0f;
					m_smeltingRecipe = 0;
				}
				else if (m_smeltingRecipe == 0)
					m_smeltingRecipe = m_smeltingRecipe2;
			}
			if (!Powered)
				m_smeltingRecipe = 0;
			if (m_smeltingRecipe == 0)
			{
				HeatLevel = 0f;
				m_fireTimeRemaining = 0f;
				//m_music = -1;
			}
			else
				m_fireTimeRemaining = 100f;
			if (m_fireTimeRemaining <= 0f)
			{
				m_smeltingRecipe = 0;
				SmeltingProgress = 0f;
				//m_music = -1;
			}
			if (m_smeltingRecipe != 0)
			{
				SmeltingProgress = MathUtils.Min(SmeltingProgress + 0.1f * dt, 1f);
				if (SmeltingProgress >= 1f)
				{
					var e = result.GetEnumerator();
					if (m_slots[RemainsSlotIndex].Count > 0)
						m_slots[RemainsSlotIndex].Count--;
					m_smeltingRecipe = 0;
					SmeltingProgress = 0f;
					m_updateSmeltingRecipe = true;
					while (e.MoveNext())
					{
						Slot slot = m_slots[ComponentCReactor.FindAcquireSlotForItem(this, e.Current.Key, e.Current.Value)];
						slot.Value = e.Current.Key;
						slot.Count += e.Current.Value;
					}
				}
			}
		}

		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			base.Load(valuesDictionary, idToEntityMap);
			m_furnaceSize = SlotsCount - 1;
			//m_updateSmeltingRecipe = true;
		}

		public int FindSmeltingRecipe(int value)
		{
			return FindSmeltingRecipe(result, value);
		}

		protected virtual int FindSmeltingRecipe()
		{
			result.Clear();
			int text = 0;
			/*if (ElementBlock.Block.GetDevice(coordinates.X, coordinates.Y, coordinates.Z, cellValue) is Centrifugal)
				if (GetSlotValue(0)== ItemBlock.IdTable["UraniumOrePowder"])
				{
					text = 8;
					result[ItemBlock.IdTable["U235P"]] = 1;
				}
				return FindSmeltingRecipe(text);
			if (ElementBlock.Block.GetDevice(coordinates.X, coordinates.Y, coordinates.Z, cellValue) is UThickener)
				return FindSmeltingRecipe(text);
			if (ElementBlock.Block.GetDevice(coordinates.X, coordinates.Y, coordinates.Z, cellValue) is Unpacker)
				return FindSmeltingRecipe(text);*/
			int i;
			for (i = 0; i < 1; i++)
			{
				if (GetSlotCount(i) <= 0) continue;
				int content = Terrain.ExtractContents(GetSlotValue(i)), x;
				switch (content)
				{
					case DirtBlock.Index:
						text = 1;
						result[SandBlock.Index] = 1;
						result[StoneChunkBlock.Index] = 1;
						x = m_random.Int() & 3;
						if (x == 0)
							result[SaltpeterChunkBlock.Index] = 1;
						else if (x == 1)
							result[ItemBlock.IdTable["AluminumOrePowder"]] = 1;
						break;

					case GraniteBlock.Index:
						text = 2;
						result[SandBlock.Index] = 1;
						result[StoneChunkBlock.Index] = 1;
						x = m_random.Int() & 7;
						if (x == 0)
							result[PigmentBlock.Index] = 1;
						else if (x == 1)
							result[ItemBlock.IdTable["Alum"]] = 1;
						else if (x == 2)
							result[ItemBlock.IdTable["Plaster"]] = 1;
						break;

					case GravelBlock.Index:
						text = 3;
						x = m_random.Int() & 3;
						if (x == 0)
							result[ItemBlock.IdTable["Cryolite"]] = 1;
						result[StoneChunkBlock.Index] = 1;
						break;

					case BasaltBlock.Index:
						text = 3;
						result[BasaltStairsBlock.Index] = 1;
						x = m_random.Int() & 7;
						if (x == 1)
							result[ItemBlock.IdTable["»¬Ê¯"]] = 1;
						break;

					case CoalBlock.Index:
						text = 4;
						result[ItemBlock.IdTable["Ashes"]] = 1;
						result[CoalChunkBlock.Index] = 7;
						x = m_random.Int() & 1;
						if (x == 0)
						{
							result[ItemBlock.IdTable["Graphite"]] = 1;
						}
						break;
				}
				if (GetSlotValue(i) == ItemBlock.IdTable["Slag"])
				{
					text = 5;
					if ((m_random.Int() & 1) != 0)
						result[ItemBlock.IdTable["VanadiumOrePowder"]] = 1;
				}
			}
			return FindSmeltingRecipe(text);
		}
	}

	public class ComponentRecycler : ComponentSeparator
	{
		public override int RemainsSlotIndex => 10;

		public static readonly int[] Prices = {
			20,
			50,
			35,
			30,
			35,
			25,
			25,
			28,
			48,
			38,
			28,
			38,
			35,
			18,
			15,
		};

		public Dictionary<string, int> Result = new Dictionary<string, int>();
		public Dictionary<string, Dictionary<string, int>> Results = new Dictionary<string, Dictionary<string, int>>();

		protected override int FindSmeltingRecipe()
		{
			Result.Clear();
			result.Clear();
			string id = null;
			int value;
			if (GetSlotCount(RemainsSlotIndex) > 0)
			{
				value = GetSlotValue(RemainsSlotIndex);
				id = BlocksManager.Blocks[Terrain.ExtractContents(value)].CraftingId + ":" + Terrain.ExtractData(value);
				if (id.IndexOf(':') < 0)
					id += ":0";
				GetValue(id);
				Result = Results[id] ?? new Dictionary<string, int>();
			}
			if (id == null || Result.Count == 0)
				return 0;
			var e = Result.GetEnumerator();
			while (e.MoveNext())
			{
				if (e.Current.Key.Length == 0) continue;
				value = GetResult(e.Current.Key);
				switch (Terrain.ExtractContents(value))
				{
					case GraniteBlock.Index:
					case BasaltBlock.Index: value = ItemBlock.IdTable["Slag"]; break;
					case PlanksBlock.Index: value = ItemBlock.IdTable["Sawdust"]; break;
					case BrickBlock.Index: value = ItemBlock.IdTable["Brickbat"]; break;
					case GlassBlock.Index: value = ItemBlock.IdTable["BrokenGlass"]; break;
				}
				result.Add(value, e.Current.Value);
			}
			return id.GetHashCode();
		}

		public static int GetResult(string id)
		{
			CraftingRecipesManager.DecodeIngredient(id, out string craftingId, out int? data);
			return Terrain.MakeBlockValue(BlocksManager.FindBlocksByCraftingId(craftingId)[0].BlockIndex, 0, data ?? 0);
		}

		public void GetValue(string id)
		{
			if (Results.TryGetValue(id, out Dictionary<string, int> dic))
				return;
			Results.Add(id, null);
			int min = 10000000, value = GetResult(id);
			var d = new Dictionary<string, int>();
			var list = CraftingRecipesManager.Recipes.m_list;
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].RequiredHeatLevel < 1 && list[i].ResultValue == value)
				{
					int v = 0;
					var arr = list[i].Ingredients;
					for (int j = 0; j < arr.Length; j++)
					{
						string s = arr[j];
						if (string.IsNullOrEmpty(s)) continue;
						if (s.IndexOf(':') < 0)
							s += ":0";
						GetValue(s);
						dic = Results[s];
						if (dic == null)
							continue;
						v += dic[""];
						var e = dic.GetEnumerator();
						while (e.MoveNext())
							if (e.Current.Key.Length != 0)
							{
								d.TryGetValue(e.Current.Key, out int count);
								count += e.Current.Value / list[i].ResultCount;
								if (count > 0)
									d[e.Current.Key] = count;
							}
					}
					v /= list[i].ResultCount;
					if (v < min)
					{
						d[""] = v;
						Results[id] = d;
						min = v;
						d = new Dictionary<string, int>();
					}
					else
						d.Clear();
				}
			}
			//if (Results[id] != null)
			//Results[id][""] = min;
		}

		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			Results.Clear();
			var vals = new Dictionary<string, int>
			{
				{ "cobblestone:0", 1 },
				{ "granite:0", 2 },
				{ "basalt:0", 1 },
				{ "sandstone:0", 1 },
				{ "glass:0", 2 },
				{ "marble:0", 5 },
				{ "brick:0", 3 },
				{ "coalchunk:0", 5 },
				{ "malachitechunk:0", 9 },
				{ "germaniumchunk:0", 14 },
				{ "diamond:0", 45 },
				{ "planks:0", 1 },
				{ "cottonwad:0", 2 },
				{ "waterbucket:0", 46 },
				{ "piston:0", 14*9+2 },
				{ "string:0", 1 },
			};
			string key;
			vals.Add("item:" + Terrain.ExtractData(ItemBlock.IdTable["SteelGear"]), Prices[0] + 1);
			vals.Add("item:" + Terrain.ExtractData(ItemBlock.IdTable["SteelWheel"]), Prices[0] + 1);
			vals.Add("item:" + Terrain.ExtractData(ItemBlock.IdTable["RefractoryBrick"]), 10);
			vals.Add("item:" + Terrain.ExtractData(ItemBlock.IdTable["Ç¹¹Ü"]), 21);
			vals.Add("item:" + Terrain.ExtractData(ItemBlock.IdTable["RifleBarrel"]), 23);
			vals.Add("item:" + Terrain.ExtractData(ItemBlock.IdTable["IndustrialMagnet"]), 24);
			var e = vals.GetEnumerator();
			while (e.MoveNext())
			{
				key = e.Current.Key;
				Results.Add(key, new Dictionary<string, int>
				{
					{ "", e.Current.Value },
					{ key, 1 }
				});
			}
			for (var i = Materials.Steel; i <= Materials.Copper; i++)
			{
				key = i.ToString();
				if (!ItemBlock.IdTable.TryGetValue(i.ToId(), out int value)) continue;
				vals = new Dictionary<string, int>
				{
					{ "", Prices[(int)i] },
					{ "chem:" + Terrain.ExtractData(value), 1 }
				};
				if (ItemBlock.IdTable.TryGetValue(key + "Ingot", out value))
					Results.Add("item:" + Terrain.ExtractData(value), vals);
				if (ItemBlock.IdTable.TryGetValue(key + "Line", out value))
					Results.Add("item:" + Terrain.ExtractData(value), vals);
				if (ItemBlock.IdTable.TryGetValue(key + "Plate", out value))
					Results.Add("item:" + Terrain.ExtractData(value), vals);
				if (ItemBlock.IdTable.TryGetValue(key + "Sheet", out value))
					Results.Add("item:" + Terrain.ExtractData(value), vals);
				if (ItemBlock.IdTable.TryGetValue(key + "Rod", out value))
					Results.Add("item:" + Terrain.ExtractData(value), vals);
			}
			Results.Add("ironingot:0", new Dictionary<string, int>
			{
				{ "", 10 },
				{ "chem:" + Terrain.ExtractData(ItemBlock.IdTable["Fe"]), 1 }
			});
			Results.Add("copperingot:0", new Dictionary<string, int>
			{
				{ "", 11 },
				{ "chem:" + Terrain.ExtractData(ItemBlock.IdTable["Cu"]), 1 }
			});
			base.Load(valuesDictionary, idToEntityMap);
		}

		public override int RemoveSlotItems(int slotIndex, int count)
		{
			m_updateSmeltingRecipe = true;
			return m_smeltingRecipe != 0 && slotIndex == RemainsSlotIndex ? 0 : base.RemoveSlotItems(slotIndex, count);
		}
	}
}