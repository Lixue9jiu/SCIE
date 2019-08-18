using System.Collections.Generic;

namespace Game
{
	public class SubsystemBlastFurnaceBlockBehavior : SubsystemCombinedBlockBehavior<ComponentMachine>
	{
		public override int[] HandledBlocks => new[]
		{
			BlastFurnaceBlock.Index,
			CovenBlock.Index,
			HearthFurnaceBlock.Index,
		};

		public SubsystemBlastFurnaceBlockBehavior() : base(new Dictionary<int, string>
		{
			{ BlastFurnaceBlock.Index, "BlastFurnace" },
			{ CovenBlock.Index, "CokeOven" },
			{ HearthFurnaceBlock.Index, "HearthFurnace" }
		})
		{
		}

		public override Widget GetWidget(IInventory inventory, ComponentMachine component, string path)
		{
			return new CovenWidget(inventory, component, path);
		}
	}

	public class SubsystemBlastBlowerBlockBehavior : SubsystemBlockBehavior
	{
		public override int[] HandledBlocks => new[] { BlastBlowerBlock.Index };

		public override void OnBlockGenerated(int value, int x, int y, int z, bool isLoaded)
		{
			value = BlastBlowerBlock.Index;
			if (ComponentEngine.IsPowered(Utils.Terrain, x, y, z) &&
				(Check(x + 1, y, z) ||
				Check(x - 1, y, z) ||
				Check(x, y + 1, z) ||
				Check(x, y - 1, z) ||
				Check(x, y, z + 1) ||
				Check(x, y, z - 1)))
				value |= FurnaceNBlock.SetHeatLevel(Terrain.ExtractData(Utils.Terrain.GetCellValue(x, y, z)), 1) << 14;
			Utils.SubsystemTerrain.ChangeCell(x, y, z, value);
		}

		public override void OnNeighborBlockChanged(int x, int y, int z, int neighborX, int neighborY, int neighborZ)
		{
			OnBlockGenerated(0, x, y, z, true);
		}

		public static bool Check(int x, int y, int z)
		{
			int value = Utils.Terrain.GetCellValue(x, y, z);
			return FurnaceNBlock.GetHeatLevel(value) != 0 && Terrain.ExtractContents(value) == FireBoxBlock.Index;
		}
	}
}