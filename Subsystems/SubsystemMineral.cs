using System;
using System.Collections.Generic;
using System.Text;
using Engine;
using TemplatesDatabase;
using static Game.TerrainBrush;

namespace Game
{
	public static class Utils
	{
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
	}
	[Flags]
	[Serializable]
	public enum Mineral : long
	{
		None = 0,
		Li = 1,
		Al = 1<<1, P = 1<<2, S = 1<<3, Sc = 1<<4, Ti = 1<<5, V = 1<<6, Cr = 1<<7, Mn = 1<<8,
		Fe = 1<<9, Co = 1<<10, Ni = 1<<11, Cu = 1<<12, Zn = 1<<13, Ga = 1<<14, Ge = 1<<15, As = 1<<16,
		Ag = 1<<17, Cd = 1<<18, Sn = 1<<19, Sb = 1<<20,
		Ba = 1<<21, W = 1<<22, Pt = 1<<23, Au = 1<<24, Hg = 1<<25, Pb = 1<<26,
		U = 1<<31
	}
	public class BasaltBlock : PaintedCubeBlock
	{
		public const int Index = 67;

		public BasaltBlock()
			: base(40)
		{
		}
		public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
		{
			int data = Terrain.ExtractData(oldValue);
			if (IsColored(data))
				base.GetDropValues(subsystemTerrain, oldValue, newValue, toolLevel, dropValues, out showDebris);
			data &= 30;
			if (data > 0 && data < 8)
			{
				dropValues.Add(new BlockDropValue
				{
					Value = Terrain.ReplaceData(ItemBlock.Index, data + 2),
					Count = 1
				});
				int num2 = (data & 1) + Random.UniformInt(0, 1);
				for (int j = -2; j < num2; j++)
				{
					dropValues.Add(new BlockDropValue
					{
						Value = ExperienceBlock.Index,
						Count = 1
					});
				}
			}
			showDebris = true;
		}
		public override int GetFaceTextureSlot(int face, int value)
		{
			int data = Terrain.ExtractData(value);
			if (!IsColored(data))
			{
				data &= 30;
				return data > 0 && data < 8 ? 9 : DefaultTextureSlot;
			}
			return m_coloredTextureSlot;
		}
		/*public virtual Mineral OnItemHarvested(SubsystemTerrain subsystemTerrain, int x, int y, int z, int value, ref BlockDropValue dropValue, ref int newValue)
		{
			var result = Mineral.None;
			var terrain = subsystemTerrain.Terrain;
			int v = terrain.GetCellValueFast(x ,y + 1, z);
			switch (Terrain.ExtractContents(value))
			{
				case DirtBlock.Index:
				if (Terrain.ExtractContents(v) != GrassBlock.Index)
				{
					if (y > 56 && (x + z - y) % 5 == 0)
						result |= Mineral.P;
				}
				break;
				case GraniteBlock.Index:
				if (y > 2 && y < 30 && (((x ^ y * 10007) + z) % 5 == 4 || (x * 89 - z) % y == 1))
					result |= Mineral.Ag;
				break;
				case SandstoneBlock.Index:
				break;
				case SandBlock.Index:
				if (Terrain.ExtractContents(v) == WaterBlock.Index && FluidBlock.GetIsTop(Terrain.ExtractData(v)) && (x + z * y) % 11 == 3)
					result |= Mineral.Au;
				break;
				case LimestoneBlock.Index:
				case BasaltBlock.Index:
				break;
				case SaltpeterOreBlock.Index:
				if ((int)SimplexNoise.Hash(y) % 3 == 1 && (x + z ^ 0xbcbabdc) > 262144)
				{
					result |= Mineral.As;
				}
				break;
			}
			return result;
		}*/
	}
	public class SubsystemMineral : SubsystemRotBlockBehavior
	{
		public enum BrushType
		{
			Au,
			Ag,
			Pt,
			Ti,
			Sn,
			Hg,
			Pb,
			Zn,
			Co,
			Ni,
			P,
			Oil,
			NaruralGas
		}
		public static TerrainBrush[] AuBrushes;
		public static TerrainBrush[] AgBrushes;
		public static TerrainBrush[] PtBrushes;
		public static TerrainBrush[] TiBrushes;
		public static TerrainBrush[] SnBrushes;
		public static TerrainBrush[] HgBrushes;
		public static TerrainBrush[] PbBrushes;
		public static TerrainBrush[] ZnBrushes;
		public static TerrainBrush[] CoBrushes;
		public static TerrainBrush[] NiBrushes;
		public static TerrainBrush[] PBrushes;
		public static TerrainBrush[] OilPocketBrushes;
		public static TerrainBrush[] NaruralGasBrushes;

		//public SubsystemTime SubsystemTime;

		//public static Dictionary<long, int> MinesData;
		//public static byte[] Used;
		//public static HashSet<int> Handled;

		public override int[] HandledBlocks
		{
			get {
				return new []
				{
					DirtBlock.Index,
					GraniteBlock.Index,
					/*SandstoneBlock.Index,
					GravelBlock.Index,
					SandBlock.Index,
					LimestoneBlock.Index,*/
					BasaltBlock.Index,
					/*ClayBlock.Index,
					MagmaBlock.Index,
					CoalOreBlock.Index,
					CopperOreBlock.Index,
					IronOreBlock.Index,
					SulphurOreBlock.Index,
					DiamondOreBlock.Index,
					GermaniumOreBlock.Index,
					SaltpeterOreBlock.Index,
					CoalBlock.Index,*/
				};
			}
		}

		/*public static void StoreItemData(long key)
		{
			MinesData.TryGetValue(key, out int count);
			MinesData[key] = count + 1;
		}*/

		public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			int i;
			//m_subsystemItemsScanner.ItemsScanned += GarbageCollectItems;
			//SubsystemTime = Project.FindSubsystem<SubsystemTime>(true);
			/*var arr = valuesDictionary.GetValue<string>("MinesData", "0").Split(',');
			MinesData = new Dictionary<long, int>(arr.Length + 1);
			for (i = 0; i < arr.Length; i++)
			{
				if (arr[i].Length > 3)
				{
					int p = arr[i].IndexOf('=');
					if (p > 0 && long.TryParse(arr[i].Substring(0, p), out long key) && int.TryParse(arr[i].Substring(p + 1), out int value))
							MinesData[key] = value;
				}
			}
			Used = new byte[262144];
			Handled = new HashSet<int>();
			var handledBlocks = HandledBlocks;
			for (i = 0; i < handledBlocks.Length; i++)
				Handled.Add(handledBlocks[i]);*/
			AuBrushes = new TerrainBrush[16];
			AgBrushes = new TerrainBrush[16];
			PtBrushes = new TerrainBrush[16];
			TiBrushes = new TerrainBrush[16];
			SnBrushes = new TerrainBrush[16];
			HgBrushes = new TerrainBrush[16];
			PbBrushes = new TerrainBrush[16];
			ZnBrushes = new TerrainBrush[16];
			CoBrushes = new TerrainBrush[16];
			NiBrushes = new TerrainBrush[16];
			PBrushes = new TerrainBrush[16];
			NaruralGasBrushes = new TerrainBrush[16];
			OilPocketBrushes = new TerrainBrush[16];
			var random = new Random(17034);
			TerrainBrush brush;
			int count, j, k, n;
			for (i = 0; i < 16; i++)
			{
				brush = new TerrainBrush();
				count = random.UniformInt(1, 3);
				for (j = 0; j < count; j++)
				{
					Vector3 v2 = 0.5f * Vector3.Normalize(new Vector3(random.UniformFloat(-1f, 1f), random.UniformFloat(-1f, 1f), random.UniformFloat(-1f, 1f)));
					n = random.UniformInt(3, 6);
					Vector3 vector2 = Vector3.Zero;
					for (k = 0; k < n; k++)
					{
						brush.AddBox((int)MathUtils.Floor(vector2.X), (int)MathUtils.Floor(vector2.Y), (int)MathUtils.Floor(vector2.Z), 1, 1, 1, (int)Mineral.Ag);
						vector2 += v2;
					}
				}
				brush.Compile();
				AuBrushes[i] = AgBrushes[i] = brush;
				brush = new TerrainBrush();
				count = random.UniformInt(4, 7);
				for (j = 0; j < count; j++)
				{
					Vector3 v3 = 0.5f * Vector3.Normalize(new Vector3(random.UniformFloat(-1f, 1f), random.UniformFloat(-2f, 2f), random.UniformFloat(-1f, 1f)));
					n = random.UniformInt(3, 6);
					Vector3 vector3 = Vector3.Zero;
					for (k = 0; k < n; k++)
					{
						brush.AddBox((int)MathUtils.Floor(vector3.X), (int)MathUtils.Floor(vector3.Y), (int)MathUtils.Floor(vector3.Z), 1, 1, 1, (int)Mineral.Pt);
						vector3 += v3;
					}
				}
				brush.Compile();
				PtBrushes[i] = brush;
				brush = new TerrainBrush();
				count = random.UniformInt(2, 6);
				for (j = 0; j < count; j++)
				{
					Vector3 v4 = 0.5f * Vector3.Normalize(new Vector3(random.UniformFloat(-1f, 1f), random.UniformFloat(-0.25f, 0.25f), random.UniformFloat(-1f, 1f)));
					n = random.UniformInt(3, 6);
					Vector3 vector4 = Vector3.Zero;
					for (k = 0; k < n; k++)
					{
						brush.AddBox((int)MathUtils.Floor(vector4.X), (int)MathUtils.Floor(vector4.Y), (int)MathUtils.Floor(vector4.Z), 1, 1, 1, (int)Mineral.Ti);
						vector4 += v4;
					}
				}
				brush.Compile();
				PbBrushes[i] = TiBrushes[i] = brush;
				brush = new TerrainBrush();
				count = random.UniformInt(4, 7);
				for (j = 0; j < count; j++)
				{
					Vector3 v5 = 0.5f * Vector3.Normalize(new Vector3(random.UniformFloat(-1f, 1f), random.UniformFloat(-1f, 1f), random.UniformFloat(-1f, 1f)));
					n = random.UniformInt(2, 5);
					Vector3 vector5 = Vector3.Zero;
					for (k = 0; k < n; k++)
					{
						brush.AddBox((int)MathUtils.Floor(vector5.X), (int)MathUtils.Floor(vector5.Y), (int)MathUtils.Floor(vector5.Z), 1, 1, 1, (int)Mineral.Sn);
						vector5 += v5;
					}
				}
				brush.Compile();
				HgBrushes[i] = SnBrushes[i] = brush;
			}
		}

		/*public override void Save(ValuesDictionary valuesDictionary)
		{
			base.Save(valuesDictionary);
			var sb = new StringBuilder(MinesData.Count * 3);
			foreach (var data in MinesData)
			{
				sb.Append(',');
				sb.Append(data.Key.ToString());
				sb.Append('=');
				sb.Append(data.Value.ToString());
			}
			valuesDictionary.SetValue("MinesData", sb.ToString(1, sb.Length));
		}*/

		public override void OnChunkInitialized(TerrainChunk chunk)
		{
			if (!(SubsystemTerrain.TerrainContentsGenerator is TerrainContentsGenerator generator))
			{
				return;
			}
			int x = chunk.Coords.X - 1;
			int y = chunk.Coords.Y - 1;
			const int
				f1 = 0x63721054,
				f2 = 0x04317562,
				f3 = 0x52473601,
				f4 = 0x61234057,
				f5 = 0x07142563,
				f6 = 0x53721604,
				f7 = 0x64317052,
				f8 = 0x02473561,
				f9 = 0x51234607,
				fa = 0x67142053,
				fb = 0x03721564,
				fc = 0x54317602,
				fd = 0x62473051,
				fe = 0x01234567,
				ff = 0x57142603;
			for (int i = x; i <= x + 2; i++)
			{
				for (int j = y; j <= y + 2; j++)
				{
					var random = new Random(generator.m_seed + i + (f1 ^ f4 ^ f5 ^ f7 ^ fa ^ fc ^ fd) * j);
					int k;
					for (k = 0; k < (random.Int() & 15) - 5; k++)
					{
						random.Int();
					}
					int ix16 = i << 4;
					int jx16 = j << 4;
					float num2 = generator.CalculateMountainRangeFactor((float)ix16, (float)jx16);
					int count = (int)(5f + 2f * num2 * SimplexNoise.OctavedNoise((float)(i ^ fe), (float)(j ^ ff), 0.33f, 1, 1f, 1f));
					const int index = BasaltBlock.Index;
					for (k = 0; k < count; k++)
					{
						chunk.PaintFastSelective(AuBrushes[random.Int() & 15].Cells, ix16 | (random.Int() & 15), random.UniformInt(5, 40), jx16 | (random.Int() & 15), index | (int)BrushType.Au << 15);
					}
					count = 6 + (int)(2f * num2 * SimplexNoise.OctavedNoise((float)(i + f2), (float)(j + 396), 0.33f, 1, 1f, 1f));
					for (k = 0; k < count; k++)
					{
						chunk.PaintFastSelective(PtBrushes[random.Int() & 15].Cells, ix16 | (random.Int() & 15), random.UniformInt(2, 40), jx16 | (random.Int() & 15), index | (int)BrushType.Pt << 15);
					}
					count = 5 + (int)(2f * num2 * SimplexNoise.OctavedNoise((float)(i + 713), (float)(j + f3), 0.33f, 1, 1f, 1f));
					for (k = 0; k < count; k++)
					{
						chunk.PaintFastSelective(AgBrushes[random.Int() & 15].Cells, ix16 | (random.Int() & 15), random.UniformInt(2, 40), jx16 | (random.Int() & 15), index | (int)BrushType.Ag << 15);
					}
					count = 3 + (int)(2f * num2 * SimplexNoise.OctavedNoise((float)(i + f8), (float)(j + 272), 0.33f, 1, 1f, 1f));
					for (k = 0; k < count; k++)
					{
						chunk.PaintFastSelective(TiBrushes[random.Int() & 15].Cells, ix16 | (random.Int() & 15), random.UniformInt(2, 40), jx16 | (random.Int() & 15), index | (int)BrushType.Ti << 15);
					}
					count = 3 + (int)(2f * num2 * SimplexNoise.OctavedNoise((float)(i + 711), (float)(j + fb), 0.33f, 1, 1f, 1f));
					for (k = 0; k < count; k++)
					{
						chunk.PaintFastSelective(SnBrushes[random.Int() & 15].Cells, ix16 | (random.Int() & 15), random.UniformInt(2, 40), jx16 | (random.Int() & 15), index | (int)BrushType.Sn << 15);
					}
					count = (int)(0.5f + 2f * num2 * SimplexNoise.OctavedNoise((float)(i + 432), (float)(j + f9), 0.33f, 1, 1f, 1f));
					for (k = 0; k < count; k++)
					{
						chunk.PaintFastSelective(HgBrushes[random.Int() & 15].Cells, ix16 | (random.Int() & 15), random.UniformInt(2, 15), jx16 | (random.Int() & 15), index | (int)BrushType.Hg << 15);
					}
					count = 3 + (int)(2f * num2 * SimplexNoise.OctavedNoise((float)(i + f6), (float)(j + 131), 0.33f, 1, 1f, 1f));
					for (k = 0; k < count; k++)
					{
						chunk.PaintFastSelective(PbBrushes[random.Int() & 15].Cells, ix16 | (random.Int() & 15), random.UniformInt(2, 50), jx16 | (random.Int() & 15), index | (int)BrushType.Pb << 15);
					}
				}
			}
		}

		public override void OnItemPlaced(int x, int y, int z, ref BlockPlacementData placementData, int itemValue)
		{
			placementData.Value = itemValue;
		}

		/*public override void OnItemHarvested(int x, int y, int z, int blockValue, ref BlockDropValue dropValue, ref int newBlockValue)
		{
			if (Terrain.ExtractData(blockValue) == 0)
				dropValue.Value = Terrain.ReplaceData(dropValue.Value, StoreItemData((long)((MineralBlock)BlocksManager.Blocks[79]).OnItemHarvested(SubsystemTerrain, x, y, z, blockValue, ref dropValue, ref newBlockValue)));
		}

		public void GarbageCollectItems(ReadOnlyList<ScannedItemData> allExistingItems)
		{
			int i;
			for (i = 1; i < MinesData.Count; i++)
				if (Used[i] == 1)
					Used[i] = 0;
			for (i = 0; i < allExistingItems.Count; i++)
			{
				int value = allExistingItems[i].Value;
				if (Handled.Contains(Terrain.ExtractContents(value)))
					Used[Terrain.ExtractData(value)] = 1;
			}
		}*/
	}
}
