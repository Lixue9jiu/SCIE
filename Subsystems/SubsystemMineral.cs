using System;
using System.Collections.Generic;
using System.Text;
using Engine;
using TemplatesDatabase;
namespace Game
{
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
	public abstract class MineralBlock : StoneChunkBlock
	{
		/*public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
		{
			showDebris = true;
		}*/
		public virtual Mineral OnItemHarvested(SubsystemTerrain subsystemTerrain, int x, int y, int z, int value, ref BlockDropValue dropValue, ref int newValue)
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
		}
	}
	public class SubsystemMineral : SubsystemRotBlockBehavior
	{
		public SubsystemTime SubsystemTime;

		public static DynamicArray<long> MinesData;

		public static byte[] Used;

		public static HashSet<int> Handled;

		public override int[] HandledBlocks
		{
			get {
				return new []
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
					CoalBlock.Index,
				};
			}
		}

		public static int StoreItemData(long value)
		{
			int i;
			var array = MinesData.Array;
			for (i = 1; i < array.Length; i++)
				if (array[i] == 0 || array[i] == value)
					break;
			if (i == 262144)
				for (i = 1; i < 262144; i++)
					if (Used[i] == 0)
						break;
			if (i == 262144)
				return 0;
			array[i] = value;
			MinesData.Count++;
			return i;
		}

		public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			m_subsystemItemsScanner.ItemsScanned += GarbageCollectItems;
			SubsystemTime = Project.FindSubsystem<SubsystemTime>(true);
			var arr = valuesDictionary.GetValue<string>("MinesData", "0").Split(',');
			int i = arr.Length + 1;
			MinesData = new DynamicArray<long>(i)
			{
				Count = i
			};
			var array = MinesData.Array;
			for (i = 0; i < arr.Length;)
				long.TryParse(arr[i], out array[++i]);
			Used = new byte[262144];
			Handled = new HashSet<int>();
			var handledBlocks = HandledBlocks;
			for (i = 0; i < handledBlocks.Length; i++)
				Handled.Add(handledBlocks[i]);
		}

		public override void Save(ValuesDictionary valuesDictionary)
		{
			base.Save(valuesDictionary);
			var array = MinesData.Array;
			var sb = new StringBuilder(array.Length);
			sb.Append(array[0].ToString());
			for (int i = 1; i < array.Length; i++)
			{
				sb.Append(',');
				sb.Append(array[i].ToString());
			}
			valuesDictionary.SetValue("MinesData", sb.ToString());
		}

		public override void OnBlockGenerated(int value, int x, int y, int z, bool isLoaded)
		{
			Used[Terrain.ExtractData(value)] = 2;
		}

		public override void OnItemPlaced(int x, int y, int z, ref BlockPlacementData placementData, int itemValue)
		{
			placementData.Value = itemValue;
		}

		public override void OnItemHarvested(int x, int y, int z, int blockValue, ref BlockDropValue dropValue, ref int newBlockValue)
		{
			if (Terrain.ExtractData(blockValue) == 0)
				dropValue.Value = Terrain.ReplaceData(dropValue.Value, StoreItemData((long)((MineralBlock)BlocksManager.Blocks[79]).OnItemHarvested(SubsystemTerrain, x, y, z, blockValue, ref dropValue, ref newBlockValue)));
		}

		public void GarbageCollectItems(ReadOnlyList<ScannedItemData> allExistingItems)
		{
			if (!SubsystemTime.PeriodicGameTimeEvent(120.0, 0.0))
				return;
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
		}
	}
}
