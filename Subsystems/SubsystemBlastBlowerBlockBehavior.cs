using TemplatesDatabase;

namespace Game
{
	public class SubsystemBlastBlowerBlockBehavior : SubsystemBlockBehavior//, IUpdateable
	{
		public override int[] HandledBlocks
		{
			get
			{
				return new int[]
				{
					540
				};
			}
		}
		
		/*public int UpdateOrder
		{
			get
			{
				return 0;
			}
		}*/
		
		public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			m_subsystemTime = Project.FindSubsystem<SubsystemTime>(true);
			m_subsystemProjectiles = Project.FindSubsystem<SubsystemProjectiles>(true);
			m_subsystemTerrain = Project.FindSubsystem<SubsystemTerrain>(true);
		}
		/*public void Update(float dt)
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
						int cellContents = SubsystemTerrain.Terrain.GetCellContents(x + i, y + j, z + k);
						if (i * i + j * j + k * k <= 1 && (cellContents == 509 || cellContents == 534))
						{
							num = 1;
						}
						if (i * i + j * j + k * k <= 1 && cellContents == 544)
						{
							num2 = 1;
						}
					}
				}
			}
			if (num == 0 || num2 == 0)
			{
				SubsystemTerrain.ChangeCell(x, y, z, 540, true);
				return;
			}
			SubsystemTerrain.ChangeCell(x, y, z, 16924, true);
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
						int cellContents = SubsystemTerrain.Terrain.GetCellContents(x + i, y + j, z + k);
						if (i * i + j * j + k * k <= 1 && (cellContents == 509 || cellContents == 534))
						{
							num = 1;
						}
						if (i * i + j * j + k * k <= 1 && cellContents == 544)
						{
							num2 = 1;
						}
					}
				}
			}
			if (num == 0 || num2 == 0)
			{
				SubsystemTerrain.ChangeCell(x, y, z, 540, true);
				return;
			}
			SubsystemTerrain.ChangeCell(x, y, z, 16924, true);
		}
		
		private readonly Random m_random = new Random();
		
		private SubsystemTime m_subsystemTime;
		
		private SubsystemProjectiles m_subsystemProjectiles;
		
		private SubsystemTerrain m_subsystemTerrain;
		
		//private Vector3 coordinate;
		
		//private bool flag;
	}
}
