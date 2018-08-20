using Engine;
using System.Collections.Generic;

namespace Game
{
	public class SubsystemCollapsingWaterBlockBehavior : SubsystemCollapsingBlockBehavior
	{
		public override int[] HandledBlocks => new int[] { 18 };

		public override void OnNeighborBlockChanged(int x, int y, int z, int neighborX, int neighborY, int neighborZ)
		{
			WorldSettings worldSettings = m_subsystemGameInfo.WorldSettings;
			if (worldSettings.EnvironmentBehaviorMode == EnvironmentBehaviorMode.Living && y > 0 && y > worldSettings.TerrainLevel + worldSettings.SeaLevelOffset)
			{
				if (IsCollapseSupportBlock(SubsystemTerrain.Terrain.GetCellValue(x, y - 1, z)))
					return;
				var list = new List<MovingBlock>();
				for (int i = y; i < 128; i++)
				{
					int cellValue = SubsystemTerrain.Terrain.GetCellValue(x, i, z);
					if (Terrain.ExtractContents(cellValue) != 18)
					{
						break;
					}
					list.Add(new MovingBlock
					{
						Value = cellValue,
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
	}
}
