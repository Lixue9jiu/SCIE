using Engine;
using System.Collections.Generic;

namespace Game
{
	public class SubsystemUnstableBlockBehavior : SubsystemCollapsingBlockBehavior
	{
		public override int[] HandledBlocks => new int[]
		{
			67
		};
		public override void OnNeighborBlockChanged(int x, int y, int z, int neighborX, int neighborY, int neighborZ)
		{
			if (m_subsystemGameInfo.WorldSettings.EnvironmentBehaviorMode != EnvironmentBehaviorMode.Living || y <= 0)
				return;
			int value = SubsystemTerrain.Terrain.GetCellValue(x, y - 1, z);
			if (!IsCollapseSupportBlock(value) || Terrain.ExtractContents(value) == 67)
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
					foreach (MovingBlock item in list)
					{
						Point3 point = item.Offset;
						SubsystemTerrain.ChangeCell(point.X + x, point.Y + y, point.Z + z, 0, true);
					}
				}
			}
		}
	}
}
