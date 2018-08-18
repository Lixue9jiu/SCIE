using Engine;
using System.Collections.Generic;
using TemplatesDatabase;

namespace Game
{
	public class SubsystemDispenserNBlockBehavior : SubsystemBlockBehavior
	{
		private readonly Random m_random = new Random();

		private SubsystemTime m_subsystemTime;

		private readonly Dictionary<Point3, bool> m_toDegrade = new Dictionary<Point3, bool>();

		private readonly Dictionary<Point3, bool> m_toHydrate = new Dictionary<Point3, bool>();

		private SubsystemProjectiles m_subsystemProjectiles;

		public override int[] HandledBlocks
		{
			get
			{
				return new int[]
				{
					503
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
		}

		public override void OnHitByProjectile(CellFace cellFace, WorldItem worldItem)
		{
			Vector3 v = CellFace.FaceToVector3(cellFace.Face);
			Vector3 position = new Vector3((float)cellFace.X + 0.5f, (float)cellFace.Y + 0.5f, (float)cellFace.Z + 0.5f) - 0.75f * v;
			int num = 0;
			for (int i = -1; i < 2; i++)
			{
				for (int j = -1; j < 2; j++)
				{
					for (int k = -1; k < 2; k++)
					{
						int cellContents = SubsystemTerrain.Terrain.GetCellContents(cellFace.X + i, cellFace.Y + j, cellFace.Z + k);
						if (i * i + j * j + k * k <= 1 && (cellContents == 509 || cellContents == 534))
						{
							num = 1;
							break;
						}
					}
				}
			}
			if (num != 0)
			{
				if (worldItem.Velocity.Length() >= 20f && BlocksManager.Blocks[Terrain.ExtractContents(worldItem.Value)].DefaultDropCount >= 2f)
				{
					for (int l = 0; (float)l <= BlocksManager.Blocks[Terrain.ExtractContents(worldItem.Value)].DefaultDropCount + 1f; l++)
					{
						m_subsystemProjectiles.FireProjectile(BlocksManager.Blocks[Terrain.ExtractContents(worldItem.Value)].DefaultDropContent, position, -20f * v, Vector3.Zero, null);
					}
					worldItem.ToRemove = true;
				}
				else
				{
					Log.Information(BlocksManager.Blocks[Terrain.ExtractContents(worldItem.Value)].DefaultDropContent.ToString());
					if (worldItem.Velocity.Length() >= 20f && BlocksManager.Blocks[Terrain.ExtractContents(worldItem.Value)].DefaultDropContent == 5)
					{
						m_subsystemProjectiles.FireProjectile(BlocksManager.Blocks[Terrain.ExtractContents(worldItem.Value)].DefaultDropContent, position, -20f * v, Vector3.Zero, null);
						worldItem.ToRemove = true;
					}
					else if (worldItem.Velocity.Length() >= 20f && BlocksManager.Blocks[Terrain.ExtractContents(worldItem.Value)].BlockIndex == 6)
					{
						for (int m = 0; m <= 4; m++)
						{
							m_subsystemProjectiles.FireProjectile(79, position, -20f * v, Vector3.Zero, null);
						}
						worldItem.ToRemove = true;
					}
				}
			}
		}
	}
}
