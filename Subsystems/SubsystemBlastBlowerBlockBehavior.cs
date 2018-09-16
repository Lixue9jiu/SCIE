namespace Game
{
	public class SubsystemBlastBlowerBlockBehavior : SubsystemBlockBehavior//, IUpdateable
	{
		public override int[] HandledBlocks
		{
			get
			{
				return new []
				{
					BlastBlowerBlock.Index
				};
			}
		}
		
		/*public int UpdateOrder
		{
			get
			{
				return 0;
			}
		}
		
		public void Update(float dt)
		{
		}*/
		public override void OnBlockGenerated(int value, int x, int y, int z, bool isLoaded)
		{
			int num = 0;
			int num2 = 0;
			for (int i = -1; i < 2; i++)
			{
				for (int j = -1; j < 2; j++)
				{
					for (int k = -1; k < 2; k++)
					{
						if (i * i + j * j + k * k <= 1)
						{
							int cellvalue = SubsystemTerrain.Terrain.GetCellValue(x + i, y + j, z + k);
							if (FurnaceNBlock.GetHeatLevel(cellvalue) != 0)
							{
								cellvalue = Terrain.ExtractContents(cellvalue);
								if (cellvalue == EngineBlock.Index || cellvalue == EngineHBlock.Index)
								{
									num = 1;
								}
								else if (cellvalue == FireBoxBlock.Index)
								{
									num2 = 1;
								}
							}
						}
					}
				}
			}
			SubsystemTerrain.ChangeCell(x, y, z, (num == 0 || num2 == 0) ? BlastBlowerBlock.Index : BlastBlowerBlock.Index | FurnaceNBlock.SetHeatLevel(Terrain.ExtractData(SubsystemTerrain.Terrain.GetCellValue(x, y, z)), 1) << 14, true);
		}
		/*public void Scanner(int x, int y, int z)
		{
		}*/
		public override void OnNeighborBlockChanged(int x, int y, int z, int neighborX, int neighborY, int neighborZ)
		{
			int num = 0;
			int num2 = 0;
			for (int i = -1; i < 2; i++)
			{
				for (int j = -1; j < 2; j++)
				{
					for (int k = -1; k < 2; k++)
					{
						if (i * i + j * j + k * k <= 1)
						{
							int cellvalue = SubsystemTerrain.Terrain.GetCellValue(x + i, y + j, z + k);
							if (FurnaceNBlock.GetHeatLevel(cellvalue) != 0)
							{
								cellvalue = Terrain.ExtractContents(cellvalue);
								if (cellvalue == EngineBlock.Index || cellvalue == EngineHBlock.Index)
								{
									num = 1;
								}
								else if (cellvalue == FireBoxBlock.Index)
								{
									num2 = 1;
								}
							}
						}
					}
				}
			}
			SubsystemTerrain.ChangeCell(x, y, z, (num == 0 || num2 == 0) ? BlastBlowerBlock.Index : BlastBlowerBlock.Index | FurnaceNBlock.SetHeatLevel(Terrain.ExtractData(SubsystemTerrain.Terrain.GetCellValue(x, y, z)), 1) << 14, true);
		}
		
		//protected Vector3 coordinate;
		
		//protected bool flag;
	}
}
