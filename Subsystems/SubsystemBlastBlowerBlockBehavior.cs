using TemplatesDatabase;

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
						if (i * i + j * j + k * k <= 1 && (cellContents == LitEngineBlock.Index || cellContents == LitEngineHBlock.Index))
						{
							num = 1;
						}
						if (i * i + j * j + k * k <= 1 && cellContents == LitFireBoxBlock.Index)
						{
							num2 = 1;
						}
					}
				}
			}
			if (num == 0 || num2 == 0)
			{
				SubsystemTerrain.ChangeCell(x, y, z, BlastBlowerBlock.Index, true);
				return;
			}
			SubsystemTerrain.ChangeCell(x, y, z, BlastBlowerBlock.Index | 1 << 14, true);
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
						if (i * i + j * j + k * k <= 1 && (cellContents == LitEngineBlock.Index || cellContents == LitEngineHBlock.Index))
						{
							num = 1;
						}
						if (i * i + j * j + k * k <= 1 && cellContents == LitFireBoxBlock.Index)
						{
							num2 = 1;
						}
					}
				}
			}
			if (num == 0 || num2 == 0)
			{
				SubsystemTerrain.ChangeCell(x, y, z, BlastBlowerBlock.Index, true);
				return;
			}
			SubsystemTerrain.ChangeCell(x, y, z, BlastBlowerBlock.Index | 1 << 14, true);
		}
		
		protected readonly Random m_random = new Random();
		
		protected SubsystemTime m_subsystemTime;
		
		protected SubsystemProjectiles m_subsystemProjectiles;
		
		protected SubsystemTerrain m_subsystemTerrain;
		
		//protected Vector3 coordinate;
		
		//protected bool flag;
	}
}
