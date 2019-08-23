using Engine;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using TemplatesDatabase;

namespace Game
{
	public enum BrushType
	{
		Au,
		Ag,
		Pt,
		Pb,
		Zn,
		Sn,
		Hg,
		Cr,
		Ti,
		Ni,
		U,
		P
	}
	public partial class SubsystemMineral : SubsystemBlockBehavior
	{
		public static TerrainBrush[] SmallBrushes,
									PtBrushes,
									ABrushes,
									BBrushes;
		public static TerrainBrush.Cell[][] OilPocketCells;
		//public static TerrainBrush[] NaturalGasBrushes;
		//public static Dictionary<long, int> MinesData;

		public override int[] HandledBlocks => new[]
		{
			BasaltBlock.Index,
			/*DirtBlock.Index,
			GraniteBlock.Index,
			SandstoneBlock.Index,
			GravelBlock.Index,
			SandBlock.Index,
			LimestoneBlock.Index,
			BasaltBlock.Index,
			ClayBlock.Index,
			MagmaBlock.Index,
			CoalOreBlock.Index,
			CopperOreBlock.Index,
			IronOreBlock.Index,
			SulphurOreBlock.Index,
			DiamondOreBlock.Index,
			GermaniumOreBlock.Index,
			SaltpeterOreBlock.Index,
			CoalBlock.Index*/
		};

		public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			Utils.Load(Project);
			//Utils.SubsystemItemsScanner.ItemsScanned += GarbageCollectItems;
			var arr = valuesDictionary.GetValue("ChemData", "").Split(',');
			//ChemicalBlock.Items = new DynamicArray<Metal>(arr.Length);
			int i;
			/*for (i = 0; i < arr.Length; i++)
				if (short.TryParse(arr[i], NumberStyles.Integer, NumberFormatInfo.CurrentInfo, out short value))
					ChemData.Add(value);*/
			SmallBrushes = new TerrainBrush[16];
			PtBrushes = new TerrainBrush[16];
			BBrushes = new TerrainBrush[16];
			ABrushes = new TerrainBrush[16];
			//NaruralGasBrushes = new TerrainBrush[16];
			OilPocketCells = new TerrainBrush.Cell[16][];
			//MinCounts = new int[12, 16];
			var random = new Random(17034);
			TerrainBrush brush;
			int j, k;
			for (i = 0; i < 16; i++)
			{
				brush = new TerrainBrush();
				Vector3 v, vec;
				for (j = random.Int() & 1; j-- != 0;)
				{
					v = 0.5f * Vector3.Normalize(new Vector3(random.UniformFloat(-1f, 1f), random.UniformFloat(-1f, 1f), random.UniformFloat(-1f, 1f)));
					vec = Vector3.Zero;
					for (k = random.UniformInt(2, 5); k-- != 0;)
					{
						brush.AddBox((int)MathUtils.Floor(vec.X), (int)MathUtils.Floor(vec.Y), (int)MathUtils.Floor(vec.Z), 1, 1, 1, 1); //Ag
						vec += v;
					}
				}
				brush.Compile();
				SmallBrushes[i] = brush;
				brush = new TerrainBrush();
				for (j = random.UniformInt(1, 3); j-- != 0;)
				{
					v = 0.5f * Vector3.Normalize(new Vector3(random.UniformFloat(-1f, 1f), random.UniformFloat(-2f, 2f), random.UniformFloat(-1f, 1f)));
					vec = Vector3.Zero;
					for (k = random.UniformInt(2, 3); k-- != 0;)
					{
						brush.AddBox((int)MathUtils.Floor(vec.X), (int)MathUtils.Floor(vec.Y), (int)MathUtils.Floor(vec.Z), 1, 1, 1, 2); //Pt
						vec += v;
					}
				}
				brush.Compile();
				PtBrushes[i] = brush;
				brush = new TerrainBrush();
				for (j = random.UniformInt(2, 4); j-- != 0;)
				{
					v = 0.5f * Vector3.Normalize(new Vector3(random.UniformFloat(-1f, 1f), random.UniformFloat(-0.25f, 0.25f), random.UniformFloat(-1f, 1f)));
					vec = Vector3.Zero;
					for (k = random.UniformInt(3, 5); k-- != 0;)
					{
						brush.AddBox((int)MathUtils.Floor(vec.X), (int)MathUtils.Floor(vec.Y), (int)MathUtils.Floor(vec.Z), 1, 1, 1, 8); //Ti
						vec += v;
					}
				}
				brush.Compile();
				ABrushes[i] = brush;
				brush = new TerrainBrush();
				for (j = random.UniformInt(3, 5); j-- != 0;)
				{
					v = 0.5f * Vector3.Normalize(new Vector3(random.UniformFloat(-1f, 1f), random.UniformFloat(-1f, 1f), random.UniformFloat(-1f, 1f)));
					vec = Vector3.Zero;
					for (k = random.UniformInt(2, 5); k-- != 0;)
					{
						brush.AddBox((int)MathUtils.Floor(vec.X), (int)MathUtils.Floor(vec.Y), (int)MathUtils.Floor(vec.Z), 1, 1, 1, 5); //Sn
						vec += v;
					}
				}
				brush.Compile();
				BBrushes[i] = brush;
				var cells = TerrainContentsGenerator.m_basaltPocketBrushes[i].Cells;
				OilPocketCells[i] = new TerrainBrush.Cell[j = cells.Length];
				while (j-- != 0)
				{
					if ((cells[j].Value & random.Int()) != 0)
					{
						OilPocketCells[i][j] = cells[j];
						OilPocketCells[i][j].Value = RottenMeatBlock.Index | 1 << 4 << 14;
					}
				}
			}
		}

		public override void Save(ValuesDictionary valuesDictionary)
		{
			base.Save(valuesDictionary);
			/*var sb = new StringBuilder(MinesData.Count * 3);
			for (var i = MinesData.GetEnumerator(); i.MoveNext();)
			{
				var data = i.Current;
				sb.Append(',');
				sb.Append(data.Key.ToString());
				sb.Append('=');
				sb.Append(data.Value.ToString());
			}
			var sb = new StringBuilder(ChemData.Count);
			var values = ChemData.Array;
			if (values.Length == 0)
				return;
			sb.Append(values[0].ToString());
			for (int i = 1; i < ChemData.Count; i++)
			{
				sb.Append(',');
				sb.Append(values[i].ToString());
			}
			valuesDictionary.SetValue(nameof(ChemData), sb.ToString());*/
		}

		public override void OnChunkInitialized(TerrainChunk chunk)
		{
			if (!(Utils.SubsystemTerrain.TerrainContentsGenerator is TerrainContentsGenerator generator) || chunk.IsLoaded)
				return;
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
			Random random;
			for (int i = x; i < x + 2; i++)
			{
				int k, ix16 = i << 4;
				for (int j = y; j < y + 2; j++)
				{
					random = new Random(generator.m_seed + i + (f1 ^ f4 ^ f5 ^ f7 ^ fa ^ fc ^ fd) * j);
					int jx16 = j << 4;
					float num2 = generator.CalculateMountainRangeFactor(ix16, jx16);
					const int index = BasaltBlock.Index, index2 = GraniteBlock.Index;
					for (k = 1 + (int)(2f * num2 * SimplexNoise.OctavedNoise(i ^ fe, j ^ ff, 0.33f, 1, 1f, 1f)); k-- != 0;)
						chunk.PaintFastSelective(SmallBrushes[random.Int() & 15].Cells, ix16 | (random.Int() & 15), random.UniformInt(2, 30), jx16 | (random.Int() & 15), index | (int)BrushType.Au << 15);
					for (k = 1 + (int)(2f * num2 * SimplexNoise.OctavedNoise(i + 713, j + f3, 0.33f, 1, 1f, 1f)); k-- != 0;)
						chunk.PaintFastSelective(SmallBrushes[random.Int() & 15].Cells, ix16 | (random.Int() & 15), random.UniformInt(2, 30), jx16 | (random.Int() & 15), index | (int)BrushType.Ag << 15);
					for (k = 1 + (int)(2f * num2 * SimplexNoise.OctavedNoise(i + f2, j + 396, 0.33f, 1, 1f, 1f)); k-- != 0;)
						chunk.PaintFastSelective(PtBrushes[random.Int() & 15].Cells, ix16 | (random.Int() & 15), random.UniformInt(2, 15), jx16 | (random.Int() & 15), index | (int)BrushType.Pt << 15);
					for (k = 3 + (int)(2f * num2 * SimplexNoise.OctavedNoise(i + f6, j + 131, 0.33f, 1, 1f, 1f)); k-- != 0;)
						chunk.PaintFastSelective(ABrushes[random.Int() & 15].Cells, ix16 | (random.Int() & 15), random.UniformInt(20, 50), jx16 | (random.Int() & 15), index2 | (int)BrushType.Pb << 15);
					for (k = 3 + (int)(2f * num2 * SimplexNoise.OctavedNoise(i + f2, j + fb, 0.33f, 1, 1f, 1f)); k-- != 0;)
						chunk.PaintFastSelective(ABrushes[random.Int() & 15].Cells, ix16 | (random.Int() & 15), random.UniformInt(30, 60), jx16 | (random.Int() & 15), index2 | (int)BrushType.Zn << 15);
					for (k = 3 + (int)(2f * num2 * SimplexNoise.OctavedNoise(i + 711, j + fb, 0.33f, 1, 1f, 1f)); k-- != 0;)
						chunk.PaintFastSelective(BBrushes[random.Int() & 15].Cells, ix16 | (random.Int() & 15), random.UniformInt(25, 55), jx16 | (random.Int() & 15), index2 | (int)BrushType.Sn << 15);
					for (k = (int)(0.5f + 2f * num2 * SimplexNoise.OctavedNoise(i + 432, j + f9, 0.33f, 1, 1f, 1f)); k-- != 0;)
						chunk.PaintFastSelective(BBrushes[random.Int() & 15].Cells, ix16 | (random.Int() & 15), random.UniformInt(2, 15), jx16 | (random.Int() & 15), index | (int)BrushType.Hg << 15);
					for (k = 2 + (int)(2f * num2 * SimplexNoise.OctavedNoise(i + f8, j + 272, 0.33f, 1, 1f, 1f)); k-- != 0;)
						chunk.PaintFastSelective(ABrushes[random.Int() & 15].Cells, ix16 | (random.Int() & 15), random.UniformInt(2, 50), jx16 | (random.Int() & 15), index | (int)BrushType.Ti << 15);
					for (k = 2 + (int)(2f * num2 * SimplexNoise.OctavedNoise(i + fa, j + fc, 0.33f, 1, 1f, 1f)); k-- != 0;)
						chunk.PaintFastSelective(BBrushes[random.Int() & 15].Cells, ix16 | (random.Int() & 15), random.UniformInt(2, 50), jx16 | (random.Int() & 15), index | (int)BrushType.Cr << 15);
					for (k = 2 + (int)(2f * num2 * SimplexNoise.OctavedNoise(i + f3, j + f6, 0.33f, 1, 1f, 1f)); k-- != 0;)
						chunk.PaintFastSelective(ABrushes[random.Int() & 15].Cells, ix16 | (random.Int() & 15), random.UniformInt(2, 50), jx16 | (random.Int() & 15), index | (int)BrushType.Ni << 15);
					for (k = 20 + (int)(8f * num2 * SimplexNoise.OctavedNoise(i + fa ^ f5 + f1, j + fc - f9, 0.33f, 1, 1f, 1f)); k-- != 0;)
						chunk.PaintMaskSelective(ABrushes[random.Int() & 15].Cells, ix16 | (random.Int() & 15), random.UniformInt(2, 50), jx16 | (random.Int() & 15), index | 65536 << 14);
					for (k = 9 + (int)(8f * num2 * SimplexNoise.OctavedNoise(i + f5 ^ f8 + f1, j + f9 - fc, 0.33f, 1, 1f, 1f)); k-- != 0;)
						chunk.PaintMaskSelective(ABrushes[random.Int() & 15].Cells, ix16 | (random.Int() & 15), random.UniformInt(2, 50), jx16 | (random.Int() & 15), index | 32768 << 14);
					for (k = 1 + (int)(2f * num2 * SimplexNoise.OctavedNoise(i + fc, j + f9, 0.33f, 1, 1f, 1f)); k-- != 0;)
						chunk.PaintFastSelective(SmallBrushes[random.Int() & 15].Cells, ix16 | (random.Int() & 15), random.UniformInt(2, 20), jx16 | (random.Int() & 15), index | (int)BrushType.U << 15);
					for (k = 3 + (int)(2f * num2 * SimplexNoise.OctavedNoise(i + f3, j + f1, 0.33f, 1, 1f, 1f)); k-- != 0;)
						chunk.PaintFastSelective(ABrushes[random.Int() & 15].Cells, ix16 | (random.Int() & 15), random.UniformInt(45, 70), jx16 | (random.Int() & 15), index2 | (int)BrushType.P << 15);
					if (generator.CalculateOceanShoreDistance(ix16, y << 4) < -90f)
					{
						int n = TerrainChunk.CalculateCellIndex(random.Int() & 15, 35, random.Int() & 15);
						for (k = 0; k < 45; k++)
							if (Terrain.ExtractContents(chunk.GetCellValueFast(n + k)) == WaterBlock.Index && BlocksManager.Blocks[Terrain.ExtractContents(chunk.GetCellValueFast(n + k - 1))].IsCollidable)
							{
								chunk.SetCellValueFast(n + k, IceBlock.Index | 32 << 14);
								break;
							}
					}
				}
				random = new Random(generator.m_seed ^ (x << 16 | y));
				if ((random.Int() & 1) != 0)
					chunk.PaintSelective(OilPocketCells[random.Int() & 15], x << 16 | (random.Int() & 15), random.UniformInt(40, 70), y << 16 | (random.Int() & 15), 3);
			}
		}
		public override void OnNeighborBlockChanged(int x, int y, int z, int neighborX, int neighborY, int neighborZ)
		{
			if (Utils.SubsystemGameInfo.WorldSettings.EnvironmentBehaviorMode != EnvironmentBehaviorMode.Living || y <= 0)
				return;
			int value = Utils.Terrain.GetCellValue(x, y - 1, z);
			if (!Utils.SubsystemCollapsingBlockBehavior.IsCollapseSupportBlock(value))
			{
				var list = new List<MovingBlock>();
				int i;
				for (i = y; i < 128; i++)
				{
					value = Utils.Terrain.GetCellValue(x, i, z);
					if (Terrain.ExtractContents(value) != 67 || (Terrain.ExtractData(value) & 65536) == 0)
						break;
					list.Add(new MovingBlock
					{
						Value = value,
						Offset = new Point3(0, i - y, 0)
					});
				}
				if (list.Count != 0 && Utils.SubsystemMovingBlocks.AddMovingBlockSet(new Vector3(x, y, z), new Vector3(x, -list.Count - 1, z), 0f, 10f, 0.7f, new Vector2(0f), list, "CollapsingBlock", null, true) != null)
					for (i = 0; i < list.Count; i++)
					{
						Point3 point = list[i].Offset;
						SubsystemTerrain.ChangeCell(point.X + x, point.Y + y, point.Z + z, 0);
					}
			}
		}

		/*public override void OnItemPlaced(int x, int y, int z, ref BlockPlacementData placementData, int itemValue)
		{
			if (Terrain.ExtractContents(itemValue) == AlloyBlock.Index)
				AlloysData.Array[Terrain.ExtractData(itemValue)] |= Metal.Used;
			placementData.Value = itemValue;
		}

		public void GarbageCollectItems(ReadOnlyList<ScannedItemData> allExistingItems)
		{
			for (int i = 0; i < allExistingItems.Count; i++)
			{
				int value = allExistingItems[i].Value;
				if (Terrain.ExtractContents(value) == BasaltBlock.Index)
					AlloysData.Array[Terrain.ExtractData(value)] |= Metal.Used;
			}
		}*/
	}
}