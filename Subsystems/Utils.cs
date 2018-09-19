using System;
using Engine;
using GameEntitySystem;
using static Game.TerrainBrush;

namespace Game
{
	public static class Utils
	{
		public static Random Random = new Random();
		public static SubsystemGameInfo SubsystemGameInfo;
		public static SubsystemAudio SubsystemAudio;
		public static SubsystemTerrain SubsystemTerrain;
		public static SubsystemTime SubsystemTime;
		public static SubsystemItemsScanner SubsystemItemsScanner;
		public static SubsystemMovingBlocks SubsystemMovingBlocks;
		public static SubsystemBlockEntities SubsystemBlockEntities;
		public static SubsystemExplosions SubsystemExplosions;
		public static SubsystemCollapsingBlockBehavior SubsystemCollapsingBlockBehavior;
		public static SubsystemProjectiles SubsystemProjectiles;
		public static Terrain Terrain;
		public static bool LoadedProject;

		public static void Load(Project Project)
		{
			if (LoadedProject)
			{
				return;
			}
			SubsystemGameInfo = Project.FindSubsystem<SubsystemGameInfo>(true);
			SubsystemAudio = Project.FindSubsystem<SubsystemAudio>(true);
			SubsystemTime = Project.FindSubsystem<SubsystemTime>(true);
			SubsystemItemsScanner = Project.FindSubsystem<SubsystemItemsScanner>(true);
			SubsystemMovingBlocks = Project.FindSubsystem<SubsystemMovingBlocks>(true);
			SubsystemBlockEntities = Project.FindSubsystem<SubsystemBlockEntities>(true);
			SubsystemExplosions = Project.FindSubsystem<SubsystemExplosions>(true);
			SubsystemCollapsingBlockBehavior = Project.FindSubsystem<SubsystemCollapsingBlockBehavior>(true);
			SubsystemProjectiles = Project.FindSubsystem<SubsystemProjectiles>(true);
			Terrain = (SubsystemTerrain = Project.FindSubsystem<SubsystemTerrain>(true)).Terrain;
			LoadedProject = true;
		}
		public static void PaintSelective(this TerrainChunk chunk, Cell[] cells, int x, int y, int z, int src = BasaltBlock.Index)
		{
			x -= chunk.Origin.X;
			z -= chunk.Origin.Y;
			for (int i = 0; i < cells.Length; i++)
			{
				Cell cell = cells[i];
				int y2 = cell.Y + y;
				if (y2 >= 0 && y2 < 128)
				{
					int index = TerrainChunk.CalculateCellIndex(cell.X + x & 15, y2, cell.Z + z & 15);
					if (src == chunk.GetCellValueFast(index))
					{
						chunk.SetCellValueFast(index, cell.Value);
					}
				}
			}
		}
		public static void PaintFastSelective(this TerrainChunk chunk, Cell[] cells, int x, int y, int z, int onlyInBlock = BasaltBlock.Index)
		{
			x -= chunk.Origin.X;
			z -= chunk.Origin.Y;
			for (int i = 0; i < cells.Length; i++)
			{
				Cell cell = cells[i];
				int y2 = cell.Y + y;
				if (y2 >= 0 && y2 < 128)
				{
					int index = TerrainChunk.CalculateCellIndex(cell.X + x & 15, y2, cell.Z + z & 15);
					if (Terrain.ExtractContents(onlyInBlock) == Terrain.ExtractContents(chunk.GetCellValueFast(index)))
					{
						//SubsystemMineral.StoreItemData(cell.Value);
						chunk.SetCellValueFast(index, onlyInBlock);
					}
				}
			}
		}
		public static void PaintMaskSelective(this TerrainChunk chunk, Cell[] cells, int x, int y, int z, int mask = BasaltBlock.Index)
		{
			x -= chunk.Origin.X;
			z -= chunk.Origin.Y;
			for (int i = 0; i < cells.Length; i++)
			{
				Cell cell = cells[i];
				int y2 = cell.Y + y;
				if (y2 >= 0 && y2 < 128)
				{
					int index = TerrainChunk.CalculateCellIndex(cell.X + x & 15, y2, cell.Z + z & 15);
					y2 = chunk.GetCellValueFast(index);
					if (Terrain.ExtractContents(mask) == Terrain.ExtractContents(y2))
					{
						//SubsystemMineral.StoreItemData(cell.Value);
						chunk.SetCellValueFast(index, y2 | mask);
					}
				}
			}
		}
		public static int GetDirectionXZ(ComponentMiner componentMiner)
		{
			Vector3 forward = Matrix.CreateFromQuaternion(componentMiner.ComponentCreature.ComponentCreatureModel.EyeRotation).Forward;
			float num = Vector3.Dot(forward, Vector3.UnitZ);
			float num2 = Vector3.Dot(forward, Vector3.UnitX);
			float num3 = Vector3.Dot(forward, -Vector3.UnitZ);
			float num4 = Vector3.Dot(forward, -Vector3.UnitX);
			float max = MathUtils.Max(num, num2, num3, num4);
			if (num == max)
			{
				return 2;
			}
			else if (num2 == max)
			{
				return 3;
			}
			else if (num3 == max)
			{
				return 0;
			}
			else if (num4 == max)
			{
				return 1;
			}
			return 0;
		}
		public static void Push<T>(this DynamicArray<T> array, T item)
		{
			if (array.m_count >= array.Capacity)
			{
				int value = MathUtils.Max(array.Capacity << 1, 4);
				if (value != array.Capacity)
				{
					T[] arr = new T[value];
					if (array.Array != null)
					{
						Array.Copy(array.Array, 0, arr, 0, array.m_count);
					}
					array.Array = arr;
				}
			}
			array.Array[array.m_count++] = item;
		}

		public static int GetColor(int data)
		{
			return data & 0xF;
		}

		public static int SetColor(int data, int color)
		{
			return (data & -16) | (color & 0xF);
		}
		public static int[] GetCreativeValues(int BlockIndex)
		{
			var array = new int[16];
			for (int i = 0; i < 16; i++)
			{
				array[i] = BlockIndex | SetColor(0, i) << 14;
			}
			return array;
		}
		public static CraftingRecipe GetAdHocCraftingRecipe(int index, SubsystemTerrain subsystemTerrain, string[] ingredients, float heatLevel)
		{
			if (heatLevel < 1f)
			{
				return null;
			}
			int i = 0, num = 0;
			var array = new string[2];
			for (; i < ingredients.Length; i++)
			{
				if (!string.IsNullOrEmpty(ingredients[i]))
				{
					if (num > 1)
					{
						return null;
					}
					array[num] = ingredients[i];
					num++;
				}
			}
			if (num != 2)
			{
				return null;
			}
			num = 0;
			int num2 = 0;
			int num3 = 0;
			for (i = 0; i < array.Length; i++)
			{
				string item = array[i];
				CraftingRecipesManager.DecodeIngredient(item, out string craftingId, out int? data);
				int d = data ?? 0;
				if (craftingId == BlocksManager.Blocks[index].CraftingId)
				{
					num3 = Terrain.MakeBlockValue(index, 0, d);
				}
				else if (craftingId == BlocksManager.Blocks[129].CraftingId)
				{
					num = Terrain.MakeBlockValue(129, 0, d);
				}
				else if (craftingId == BlocksManager.Blocks[128].CraftingId)
				{
					num2 = Terrain.MakeBlockValue(128, 0, d);
				}
			}
			if (num != 0 && num3 != 0)
			{
				int num4 = GetColor(Terrain.ExtractData(num3));
				int color = PaintBucketBlock.GetColor(Terrain.ExtractData(num));
				int num5 = PaintBucketBlock.CombineColors(num4, color);
				if (num5 != num4)
				{
					return new CraftingRecipe
					{
						ResultCount = 1,
						ResultValue = Terrain.MakeBlockValue(index, 0, SetColor(Terrain.ExtractData(num3), num5)),
						RemainsCount = 1,
						RemainsValue = BlocksManager.DamageItem(Terrain.MakeBlockValue(129, 0, color), BlocksManager.Blocks[129].GetDamage(num) + 1),
						RequiredHeatLevel = 1f,
						Description = "Dye tool " + SubsystemPalette.GetName(subsystemTerrain, color, null),
						Ingredients = (string[])ingredients.Clone()
					};
				}
			}
			if (num2 != 0 && num3 != 0)
			{
				int num6 = Terrain.ExtractData(num3);
				if (GetColor(num6) != 0)
				{
					return new CraftingRecipe
					{
						ResultCount = 1,
						ResultValue = Terrain.MakeBlockValue(index, 0, SetColor(num6, 0)),
						RemainsCount = 1,
						RemainsValue = BlocksManager.DamageItem(Terrain.MakeBlockValue(128), BlocksManager.Blocks[128].GetDamage(num2) + 1),
						RequiredHeatLevel = 1f,
						Description = "Undye tool",
						Ingredients = (string[])ingredients.Clone()
					};
				}
			}
			return null;
		}
	}
}
