using Engine;
using TemplatesDatabase;

namespace Game
{
	public class SubsystemBullet2BlockBehavior : SubsystemBlockBehavior
	{
		private readonly Random m_random = new Random();

		private SubsystemAudio m_subsystemAudio;

		private SubsystemExplosions m_subsystemExplosions;

		private SubsystemTerrain m_subsystemTerrain;

		public override int[] HandledBlocks
		{
			get
			{
				return new int[0];
			}
		}

		public override bool OnHitAsProjectile(CellFace? cellFace, ComponentBody componentBody, WorldItem worldItem)
		{
			Bullet2Block.GetBulletType(Terrain.ExtractData(worldItem.Value));
			bool result = true;
			if (cellFace.HasValue)
			{
				int cellValue = m_subsystemTerrain.Terrain.GetCellValue(cellFace.Value.X, cellFace.Value.Y, cellFace.Value.Z);
				Block obj = BlocksManager.Blocks[Terrain.ExtractContents(cellValue)];
				if ((double)worldItem.Velocity.Length() > 30.0)
				{
					m_subsystemExplosions.TryExplodeBlock(cellFace.Value.X, cellFace.Value.Y, cellFace.Value.Z, cellValue);
				}
				if ((double)obj.Density >= 1.5 && (double)worldItem.Velocity.Length() > 30.0)
				{
					const float num = 1f;
					const float minDistance = 8f;
					if ((double)m_random.UniformFloat(0f, 1f) < (double)num)
					{
						m_subsystemAudio.PlayRandomSound("Audio/Ricochets", 1f, m_random.UniformFloat(-0.2f, 0.2f), new Vector3((float)cellFace.Value.X, (float)cellFace.Value.Y, (float)cellFace.Value.Z), minDistance, true);
						result = false;
					}
				}
			}
			return result;
		}

		public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			m_subsystemTerrain = Project.FindSubsystem<SubsystemTerrain>(true);
			m_subsystemAudio = Project.FindSubsystem<SubsystemAudio>(true);
			m_subsystemExplosions = Project.FindSubsystem<SubsystemExplosions>(true);
		}
	}
}
