using Engine;
using System.Collections.Generic;
using TemplatesDatabase;

namespace Game
{
	public class SubsystemCollapsingWaterBlockBehavior : SubsystemWaterBlockBehavior
	{
		public SubsystemGameInfo m_subsystemGameInfo;

		public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			m_subsystemGameInfo = Project.FindSubsystem<SubsystemGameInfo>(true);
		}
		public override void OnBlockAdded(int value, int oldValue, int x, int y, int z)
		{
			UpdateIsTop(value, x, y, z);
		}
		public override void OnNeighborBlockChanged(int x, int y, int z, int neighborX, int neighborY, int neighborZ)
		{
			base.OnNeighborBlockChanged(x, y, z, neighborX, neighborY, neighborZ);
			WorldSettings worldSettings = m_subsystemGameInfo.WorldSettings;
			if (worldSettings.EnvironmentBehaviorMode == EnvironmentBehaviorMode.Living && y > 0 && y > worldSettings.TerrainLevel + worldSettings.SeaLevelOffset &&(neighborX != x || neighborY != y || neighborZ != z))
			{
				Terrain terrain = SubsystemTerrain.Terrain;
				if (BlocksManager.Blocks[terrain.GetCellContents(x, y - 1, z)].IsFluidBlocker || BlocksManager.Blocks[terrain.GetCellContents(x, y - 1, z)].IsFluidBlocker)
					return;
				int i = y;
				while (i < 128 && Terrain.ExtractContents(terrain.GetCellValue(x, i, z)) == 18)
					i++;
				SubsystemTerrain.DestroyCell(0, x, i - 1, z, 0, false, false);
			}
		}
	}
}
