using Engine;

namespace Game
{
	public class SubsystemRailBlockBehavior : SubsystemBlockBehavior
	{
		protected Point3[][] m_pointsToUpdate = new Point3[][]
		{
			new Point3[]
			{
				new Point3(0, 0, -1),
				new Point3(0, 1, -1),
				new Point3(0, -1, -1)
			},
			new Point3[]
			{
				new Point3(-1, 0, 0),
				new Point3(-1, 1, 0),
				new Point3(-1, -1, 0)
			},
			new Point3[]
			{
				new Point3(0, 0, 1),
				new Point3(0, 1, 1),
				new Point3(0, -1, 1)
			},
			new Point3[]
			{
				new Point3(1, 0, 0),
				new Point3(1, 1, 0),
				new Point3(1, -1, 0)
			}
		};

		public override int[] HandledBlocks => new int[]
		{
			RailBlock.Index
		};

		public override void OnBlockAdded(int value, int oldValue, int x, int y, int z)
		{
			base.OnBlockAdded(value, oldValue, x, y, z);
			UpdateCornerState(value, x, y, z, 1);
		}

		void UpdateCornerState(int value, int x, int y, int z, int step)
		{
			int? resultType = default(int?);
			var neighbors = new bool[4];
			var raisedNeighbors = new bool[4];
			
			for (int i = 0; i < 4; i++)
			{
				for (int k = 0; k < 3; k++)
				{
					var p = m_pointsToUpdate[i][k];
					int val = SubsystemTerrain.Terrain.GetCellValueFastChunkExists(x + p.X, y + p.Y, z + p.Z);
					if (Terrain.ExtractContents(val) == RailBlock.Index)
					{
						if (k == 1)
							raisedNeighbors[i] = RailBlock.CanConnectTo(RailBlock.GetRailType(Terrain.ExtractData(val)), i);
						else
							neighbors[i] = RailBlock.CanConnectTo(RailBlock.GetRailType(Terrain.ExtractData(val)), i);
						if (step > 0)
							UpdateCornerState(val, x + p.X, y + p.Y, z + p.Z, step - 1);
						break;
					}
				}
			}
			
			if (neighbors[0] && neighbors[3])
			{
				resultType = 1;
			}
			else if (neighbors[0] && neighbors[1])
			{
				resultType = 2;
			}
			else if (neighbors[1] && neighbors[2])
			{
				resultType = 3;
			}
			else if (neighbors[2] && neighbors[3])
			{
				resultType = 0;
			}
			else if (raisedNeighbors[2])
			{
				resultType = 6;
			}
			else if (raisedNeighbors[0])
			{
				resultType = 8;
			}
			else if (raisedNeighbors[3])
			{
				resultType = 7;
			}
			else if (raisedNeighbors[1])
			{
				resultType = 9;
			}
			else if (neighbors[0] || neighbors[2])
			{
				resultType = 4;
			}
			else if (neighbors[1] || neighbors[3])
			{
				resultType = 5;
			}

			if (resultType.HasValue)
				SubsystemTerrain.ChangeCell(x, y, z, Terrain.MakeBlockValue(RailBlock.Index, 0, RailBlock.SetRailType(0, resultType.Value)));
		}
	}
}