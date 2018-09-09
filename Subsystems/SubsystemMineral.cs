using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Engine;
using Engine.Graphics;
using TemplatesDatabase;

namespace Game
{
	[Flags]
	[Serializable]
	public enum Metal : short
	{
		None,
		Li = 1,
		Al = 1<<1, /*P = 1<<2, S = 1<<3, Sc = 1<<4, Ti = 1<<5, V = 1<<6,*/ Cr = 1<<7, Mn = 1<<8,
		Fe = 1<<9, Co = 1<<10, Ni = 1<<11, Cu = 1<<12, Zn = 1<<13, Ga = 1<<14, /*Ge = 1<<15, As = 1<<16,
		Ag = 1<<17, Cd = 1<<18, Sn = 1<<19, Sb = 1<<20,
		Ba = 1<<21, W = 1<<22, Pt = 1<<23, Au = 1<<24, Hg = 1<<25, Pb = 1<<26,
		U = 1<<31*/
		Used = -32768
	}
	public class BasaltBlock : PaintedCubeBlock
	{
		public static readonly Color[] Colors = new Color[]
		{
			new Color(255, 255, 255),
			new Color(255, 215, 0),//Gold
			new Color(212, 212, 212),//Silver
			new Color(232, 232, 232),//Platinum
			new Color(65, 224, 205),//Zinc
			new Color(88, 87, 86),//Lead
			new Color(225, 225, 225),//Stannary
			new Color(255, 123, 113),//Mercury
			new Color(190, 190, 190),//Titanium
			new Color(90, 90, 90), //Chromium
			new Color(120, 120, 120) //Nickel
		};
		public const int Index = 67;

		public BasaltBlock()
			: base(40)
		{
		}
		public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
		{
			int data = Terrain.ExtractData(oldValue);
			if (toolLevel > 2 && (data & 98304) == 32768 && (Random.Int() & 3) == 0)
			{
				dropValues.Add(new BlockDropValue
				{
					Value = oldValue,
					Count = 1
				});
				showDebris = true;
				return;
			}
			if (!IsColored(data) && toolLevel > 2 && (data = data >> 1 & 16383) > 0 && data < 11)
			{
				if ((Random.Int() & 7) == 0)
				{
					dropValues.Add(new BlockDropValue
					{
						Value = ItemBlock.IdTable["ScrapIron"],
						Count = 1
					});
				}
				dropValues.Add(new BlockDropValue
				{
					Value = Terrain.ReplaceData(ItemBlock.Index, data + 14),
					Count = 1
				});
				for (data = (Random.Int() & 1) + (2 | data & 1); data-- != 0;)
				{
					dropValues.Add(new BlockDropValue
					{
						Value = ExperienceBlock.Index,
						Count = 1
					});
				}
				showDebris = true;
			}
			else
			{
				base.GetDropValues(subsystemTerrain, oldValue, newValue, toolLevel, dropValues, out showDebris);
			}
		}
		public override BlockPlacementData GetDigValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, int toolValue, TerrainRaycastResult raycastResult)
		{
			var cellFace = raycastResult.CellFace;
			int x = cellFace.X, y = cellFace.Y, z = cellFace.Z;
			Terrain terrain = subsystemTerrain.Terrain;
			return new BlockPlacementData
			{
				Value = (Terrain.ExtractData(value) & 65536) != 0 ? Terrain.ReplaceData(MagmaBlock.Index
					, FluidBlock.SetIsTop(FluidBlock.SetLevel(0, 4), toolValue == MagmaBucketBlock.Index)) : 0,
				CellFace = raycastResult.CellFace
			};
		}
		public override int GetFaceTextureSlot(int face, int value)
		{
			int data = Terrain.ExtractData(value);
			if (IsColored(data))
				return m_coloredTextureSlot;
			data = data >> 1 & 16383;
			return data > 0 && data < 11 ? 9 : DefaultTextureSlot;
		}
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			int data = Terrain.ExtractData(value);
			string name = (data & 65536) != 0 ? "Unstable " : string.Empty;
			if ((data & 32768) != 0)
			{
				name += "Explosive ";
			}
			data &= 16383;
			if (IsColored(data) || data == 0 || data > 20)
				return name + base.GetDisplayName(subsystemTerrain, value);
			name += BlocksManager.Blocks[ItemBlock.Index].GetDisplayName(subsystemTerrain, Terrain.ReplaceData(ItemBlock.Index, (data >> 1) + 14));
			return name.Substring(0, name.Length - 5);
		}
		public override float GetExplosionPressure(int value)
		{
			return (Terrain.ExtractData(value) & 32768) != 0 ? 10f : base.GetExplosionPressure(value);
		}
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			int data = (Terrain.ExtractData(value) & 16383) >> 1;
			if (data < 11)
			{
				color *= Colors[data];
			}
			base.DrawBlock(primitivesRenderer, value, color, size, ref matrix, environmentData);
		}
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometrySubsets geometry, int value, int x, int y, int z)
		{
			int data = Terrain.ExtractData(value) & 16383;
			generator.GenerateCubeVertices(this, value, x, y, z, IsColored(data) ? SubsystemPalette.GetColor(generator, GetColor(Terrain.ExtractData(value))) : data < 22 ? Colors[data >> 1] : Color.White, geometry.OpaqueSubsetsByFace);
		}
		public override IEnumerable<int> GetCreativeValues()
		{
			var list = new List<int>(base.GetCreativeValues());
			int count = list.Count;
			int i = 0;
			for (; i < count; i++)
			{
				list.Add(list[i] | 65536 << 14);
			}
			for (i = 0; i < count; i++)
			{
				list.Add(list[i] | 32768 << 14);
			}
			for (i = 0; i < count; i++)
			{
				list.Add(list[i] | (65536 | 32768) << 14);
			}
			const int M = 11;
			for (i = 1; i < M; i++)
			{
				list.Add(BlockIndex | i << 15);
			}
			for (i = 1; i < M; i++)
			{
				list.Add(BlockIndex | i << 15 | 65536 << 14);
			}
			for (i = 1; i < M; i++)
			{
				list.Add(BlockIndex | i << 15 | 32768 << 14);
			}
			for (i = 1; i < M; i++)
			{
				list.Add(BlockIndex | i << 15 | (65536 | 32768) << 14);
			}
			return list;
		}
	}
	public class SubsystemMineral : SubsystemBlockBehavior
	{
		public enum BrushType
		{
			Au,
			Ag,
			Pt,
			Zn,
			Pb,
			Hg,
			Sn,
			Ti,
			Cr,
			Ni,
			//P,
			//Oil,
			//NaruralGas
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
		//public static TerrainBrush[] OilPocketBrushes;
		//public static TerrainBrush[] NaruralGasBrushes;
		public static TerrainBrush[,] Brushes;
		//public SubsystemTime SubsystemTime;
		protected SubsystemGameInfo m_subsystemGameInfo;
		protected SubsystemItemsScanner m_subsystemItemsScanner;
		protected SubsystemMovingBlocks m_subsystemMovingBlocks;
		SubsystemCollapsingBlockBehavior m_subsystemCollapsingBlockBehavior;
		//public static Dictionary<long, int> MinesData;
		public static DynamicArray<Metal> AlloysData;
		//public static HashSet<int> Handled;

		public override int[] HandledBlocks => new int[]
		{
			BasaltBlock.Index,
		};/*
				{
					DirtBlock.Index,
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
					CoalBlock.Index
				};*/

		public static int StoreItemData(Metal key)
		{
			int i;
			var array = AlloysData.Array;
			for (i = 1; i < AlloysData.Count; i++)
				if (array[i] == 0)
				{
					array[i] = key;
					return i;
				}
				else if (array[i] == key || (array[i] & Metal.Used) == 0)
					return i;
			if (i == 262144)
				return 0;
			AlloysData.Add(key);
			return i;
			//MinesData.TryGetValue(key, out int count);
			//MinesData[key] = count + 1;
		}

		public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			m_subsystemGameInfo = base.Project.FindSubsystem<SubsystemGameInfo>(true);
			m_subsystemMovingBlocks = base.Project.FindSubsystem<SubsystemMovingBlocks>(true);
			m_subsystemCollapsingBlockBehavior = base.Project.FindSubsystem<SubsystemCollapsingBlockBehavior>(true);
			int i;
			//(m_subsystemItemsScanner = Project.FindSubsystem<SubsystemItemsScanner>(true)).ItemsScanned += GarbageCollectItems;
			//SubsystemTime = Project.FindSubsystem<SubsystemTime>(true);
			var arr = valuesDictionary.GetValue<string>("AlloysData", "0").Split(',');
			AlloysData = new DynamicArray<Metal>(arr.Length);
			for (i = 0; i < arr.Length; i++)
			{
				if (short.TryParse(arr[i], NumberStyles.Integer, NumberFormatInfo.CurrentInfo, out short value))
					AlloysData.Add((Metal)value);
			}
			/*Used = new byte[262144];
			Handled = new HashSet<int>();
			for (i = 0; i < HandledBlocks.Length; i++)
				Handled.Add(HandledBlocks[i]);*/
			AuBrushes = new TerrainBrush[16];
			AgBrushes = new TerrainBrush[16];
			PtBrushes = new TerrainBrush[16];
			TiBrushes = new TerrainBrush[16];
			SnBrushes = new TerrainBrush[16];
			HgBrushes = new TerrainBrush[16];
			PbBrushes = new TerrainBrush[16];
			ZnBrushes = new TerrainBrush[16];
			//CoBrushes = new TerrainBrush[16];
			//NiBrushes = new TerrainBrush[16];
			PBrushes = new TerrainBrush[16];
			//NaruralGasBrushes = new TerrainBrush[16];
			//OilPocketBrushes = new TerrainBrush[16];
			//Brushes = new TerrainBrush[12, 16];
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
						brush.AddBox((int)MathUtils.Floor(vec.X), (int)MathUtils.Floor(vec.Y), (int)MathUtils.Floor(vec.Z), 1, 1, 1, (int)BrushType.Ag);
						vec += v;
					}
				}
				brush.Compile();
				AuBrushes[i] = AgBrushes[i] = brush;
				brush = new TerrainBrush();
				for (j = random.UniformInt(1, 3); j-- != 0;)
				{
					v = 0.5f * Vector3.Normalize(new Vector3(random.UniformFloat(-1f, 1f), random.UniformFloat(-2f, 2f), random.UniformFloat(-1f, 1f)));
					vec = Vector3.Zero;
					for (k = random.UniformInt(2, 3); k-- != 0;)
					{
						brush.AddBox((int)MathUtils.Floor(vec.X), (int)MathUtils.Floor(vec.Y), (int)MathUtils.Floor(vec.Z), 1, 1, 1, (int)BrushType.Pt);
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
					for (k = random.UniformInt(3, 6); k-- != 0;)
					{
						brush.AddBox((int)MathUtils.Floor(vec.X), (int)MathUtils.Floor(vec.Y), (int)MathUtils.Floor(vec.Z), 1, 1, 1, (int)BrushType.Ti);
						vec += v;
					}
				}
				brush.Compile();
				PbBrushes[i] = TiBrushes[i] = brush;
				brush = new TerrainBrush();
				for (j = random.UniformInt(3, 5); j-- != 0;)
				{
					v = 0.5f * Vector3.Normalize(new Vector3(random.UniformFloat(-1f, 1f), random.UniformFloat(-1f, 1f), random.UniformFloat(-1f, 1f)));
					vec = Vector3.Zero;
					for (k = random.UniformInt(2, 5); k-- != 0;)
					{
						brush.AddBox((int)MathUtils.Floor(vec.X), (int)MathUtils.Floor(vec.Y), (int)MathUtils.Floor(vec.Z), 1, 1, 1, (int)BrushType.Sn);
						vec += v;
					}
				}
				brush.Compile();
				HgBrushes[i] = SnBrushes[i] = brush;
				/*brush = new TerrainBrush();
				for (j = random.UniformInt(10, 20); j-- != 0;)
				{
					v = 0.5f * Vector3.Normalize(new Vector3(random.UniformFloat(-1f, 1f), random.UniformFloat(-1f, 1f), random.UniformFloat(-1f, 1f)));
					vec = Vector3.Zero;
					for (k = random.UniformInt(6, 10); k-- != 0;)
					{
						brush.AddBox((int)MathUtils.Floor(vec.X), (int)MathUtils.Floor(vec.Y), (int)MathUtils.Floor(vec.Z), 1, 1, 1, 67);
						vec += v;
					}
				}
				brush.Compile();
				PBrushes[i] = brush;*/
			}
		}

		public override void Save(ValuesDictionary valuesDictionary)
		{
			base.Save(valuesDictionary);
			/*var sb = new StringBuilder(MinesData.Count * 3);
			foreach (var data in MinesData)
			{
				sb.Append(',');
				sb.Append(data.Key.ToString());
				sb.Append('=');
				sb.Append(data.Value.ToString());
			}*/
			var sb = new StringBuilder(AlloysData.Count);
			var values = AlloysData.Array;
			if (values.Length == 0)
			{
				return;
			}
			sb.Append(values[0].ToString());
			for (int i = 1; i < AlloysData.Count; i++)
			{
				sb.Append(',');
				sb.Append(values[i].ToString());
			}
			valuesDictionary.SetValue("AlloysData", sb.ToString());
		}

		public override void OnChunkInitialized(TerrainChunk chunk)
		{
			if (!(SubsystemTerrain.TerrainContentsGenerator is TerrainContentsGenerator generator) || chunk.ModificationCounter != 0)
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
			for (int i = x; i < x + 2; i++)
			{
				for (int j = y; j < y + 2; j++)
				{
					var random = new Random(generator.m_seed + i + (f1 ^ f4 ^ f5 ^ f7 ^ fa ^ fc ^ fd) * j);
					int k, ix16 = i << 4, jx16 = j << 4;
					float num2 = generator.CalculateMountainRangeFactor((float)ix16, (float)jx16);
					const int index = BasaltBlock.Index, index2 = BasaltBlock.Index;
					for (k = 1 + (int)(2f * num2 * SimplexNoise.OctavedNoise((float)(i ^ fe), (float)(j ^ ff), 0.33f, 1, 1f, 1f)); k-- != 0;)
					{
						chunk.PaintFastSelective(AuBrushes[random.Int() & 15].Cells, ix16 | (random.Int() & 15), random.UniformInt(2, 40), jx16 | (random.Int() & 15), index2 | (int)BrushType.Au << 15);
					}
					for (k = 1 + (int)(2f * num2 * SimplexNoise.OctavedNoise((float)(i + 713), (float)(j + f3), 0.33f, 1, 1f, 1f)); k-- != 0;)
					{
						chunk.PaintFastSelective(AgBrushes[random.Int() & 15].Cells, ix16 | (random.Int() & 15), random.UniformInt(2, 40), jx16 | (random.Int() & 15), index2 | (int)BrushType.Ag << 15);
					}
					for (k = 1 + (int)(2f * num2 * SimplexNoise.OctavedNoise((float)(i + f2), (float)(j + 396), 0.33f, 1, 1f, 1f)); k-- != 0;)
					{
						chunk.PaintFastSelective(PtBrushes[random.Int() & 15].Cells, ix16 | (random.Int() & 15), random.UniformInt(2, 40), jx16 | (random.Int() & 15), index2 | (int)BrushType.Pt << 15);
					}
					for (k = 3 + (int)(2f * num2 * SimplexNoise.OctavedNoise((float)(i + f6), (float)(j + 131), 0.33f, 1, 1f, 1f)); k-- != 0;)
					{
						chunk.PaintFastSelective(PbBrushes[random.Int() & 15].Cells, ix16 | (random.Int() & 15), random.UniformInt(2, 50), jx16 | (random.Int() & 15), index | (int)BrushType.Pb << 15);
					}
					for (k = (int)(0.5f + 2f * num2 * SimplexNoise.OctavedNoise((float)(i + 432), (float)(j + f9), 0.33f, 1, 1f, 1f)); k-- != 0;)
					{
						chunk.PaintFastSelective(HgBrushes[random.Int() & 15].Cells, ix16 | (random.Int() & 15), random.UniformInt(2, 15), jx16 | (random.Int() & 15), index | (int)BrushType.Hg << 15);
					}
					for (k = 3 + (int)(2f * num2 * SimplexNoise.OctavedNoise((float)(i + 711), (float)(j + fb), 0.33f, 1, 1f, 1f)); k-- != 0;)
					{
						chunk.PaintFastSelective(SnBrushes[random.Int() & 15].Cells, ix16 | (random.Int() & 15), random.UniformInt(2, 40), jx16 | (random.Int() & 15), index | (int)BrushType.Sn << 15);
					}
					for (k = 2 + (int)(2f * num2 * SimplexNoise.OctavedNoise((float)(i + f8), (float)(j + 272), 0.33f, 1, 1f, 1f)); k-- != 0;)
					{
						chunk.PaintFastSelective(TiBrushes[random.Int() & 15].Cells, ix16 | (random.Int() & 15), random.UniformInt(2, 50), jx16 | (random.Int() & 15), index | (int)BrushType.Ti << 15);
					}
					for (k = 2 + (int)(2f * num2 * SimplexNoise.OctavedNoise((float)(i + fa), (float)(j + fc), 0.33f, 1, 1f, 1f)); k-- != 0;)
					{
						chunk.PaintFastSelective(HgBrushes[random.Int() & 15].Cells, ix16 | (random.Int() & 15), random.UniformInt(2, 50), jx16 | (random.Int() & 15), index | (int)BrushType.Cr << 15);
					}
					for (k = 2 + (int)(2f * num2 * SimplexNoise.OctavedNoise((float)(i + f3), (float)(j + f6), 0.33f, 1, 1f, 1f)); k-- != 0;)
					{
						chunk.PaintFastSelective(TiBrushes[random.Int() & 15].Cells, ix16 | (random.Int() & 15), random.UniformInt(2, 50), jx16 | (random.Int() & 15), index | (int)BrushType.Ni << 15);
					}
					for (k = 20 + (int)(8f * num2 * SimplexNoise.OctavedNoise((float)(i + fa ^ f5 + f1), (float)(j + fc - f9), 0.33f, 1, 1f, 1f)); k-- != 0;)
					{
						chunk.PaintMaskSelective(PbBrushes[random.Int() & 15].Cells, ix16 | (random.Int() & 15), random.UniformInt(2, 50), jx16 | (random.Int() & 15), index | 65536 << 14);
					}
					for (k = 9 + (int)(8f * num2 * SimplexNoise.OctavedNoise((float)(i + f5 ^ f8 + f1), (float)(j + f9 - fc), 0.33f, 1, 1f, 1f)); k-- != 0;)
					{
						chunk.PaintMaskSelective(PbBrushes[random.Int() & 15].Cells, ix16 | (random.Int() & 15), random.UniformInt(2, 50), jx16 | (random.Int() & 15), index | 32768 << 14);
					}
				}
			}
		}
		public override void OnNeighborBlockChanged(int x, int y, int z, int neighborX, int neighborY, int neighborZ)
		{
			if (m_subsystemGameInfo.WorldSettings.EnvironmentBehaviorMode != EnvironmentBehaviorMode.Living || y <= 0)
				return;
			int value = SubsystemTerrain.Terrain.GetCellValue(x, y - 1, z);
			if (!m_subsystemCollapsingBlockBehavior.IsCollapseSupportBlock(value))
			{
				List<MovingBlock> list = new List<MovingBlock>();
				for (int i = y; i < 128; i++)
				{
					value = SubsystemTerrain.Terrain.GetCellValue(x, i, z);
					if (Terrain.ExtractContents(value) != 67 || (Terrain.ExtractData(value) & 65536) == 0)
					{
						break;
					}
					list.Add(new MovingBlock
					{
						Value = value,
						Offset = new Point3(0, i - y, 0)
					});
				}
				if (list.Count != 0 && m_subsystemMovingBlocks.AddMovingBlockSet(new Vector3(x, y, z), new Vector3((float)x, (float)(-list.Count - 1), (float)z), 0f, 10f, 0.7f, new Vector2(0f), list, "CollapsingBlock", null, true) != null)
				{
					for (int i = 0; i < list.Count; i++)
					{
						Point3 point = list[i].Offset;
						SubsystemTerrain.ChangeCell(point.X + x, point.Y + y, point.Z + z, 0, true);
					}
				}
			}
		}

		public override void OnItemPlaced(int x, int y, int z, ref BlockPlacementData placementData, int itemValue)
		{
			/*if (Terrain.ExtractContents(itemValue) == AlloyBlock.Index)
			{
				AlloysData.Array[Terrain.ExtractData(itemValue)] |= Metal.Used;
			}*/
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
		}
	}
}
