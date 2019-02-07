namespace Game
{
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
			Utils.SubsystemTerrain.ChangeCell(x, y, z, value, true);
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