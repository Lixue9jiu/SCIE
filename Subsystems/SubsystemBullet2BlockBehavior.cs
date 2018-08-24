using Engine;
using TemplatesDatabase;

namespace Game
{
	public class SubsystemBullet2BlockBehavior : SubsystemBlockBehavior
	{
		protected readonly Random m_random = new Random();

		protected SubsystemAudio m_subsystemAudio;

		protected SubsystemExplosions m_subsystemExplosions;

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
			if (cellFace.HasValue)
			{
				int cellValue = SubsystemTerrain.Terrain.GetCellValue(cellFace.Value.X, cellFace.Value.Y, cellFace.Value.Z);
				Block obj = BlocksManager.Blocks[Terrain.ExtractContents(cellValue)];
				if (worldItem.Velocity.Length() > 30f)
				{
					m_subsystemExplosions.TryExplodeBlock(cellFace.Value.X, cellFace.Value.Y, cellFace.Value.Z, cellValue);
				}
				if (obj.Density >= 1.5f && worldItem.Velocity.Length() > 30f)
				{
					const float num = 1f;
					const float minDistance = 8f;
					if (m_random.UniformFloat(0f, 1f) < num)
					{
						m_subsystemAudio.PlayRandomSound("Audio/Ricochets", 1f, m_random.UniformFloat(-0.2f, 0.2f), new Vector3((float)cellFace.Value.X, (float)cellFace.Value.Y, (float)cellFace.Value.Z), minDistance, true);
						return false;
					}
				}
			}
			return true;
		}

		public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			m_subsystemAudio = Project.FindSubsystem<SubsystemAudio>(true);
			m_subsystemExplosions = Project.FindSubsystem<SubsystemExplosions>(true);
		}
	}
}
