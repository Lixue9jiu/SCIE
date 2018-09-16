using Engine;

namespace Game
{
	public class SubsystemBullet2BlockBehavior : SubsystemBlockBehavior
	{
		public override int[] HandledBlocks
		{
			get
			{
				return new int[0];
			}
		}

		public override bool OnHitAsProjectile(CellFace? cellFace, ComponentBody componentBody, WorldItem worldItem)
		{
			//Bullet2Block.GetBulletType(Terrain.ExtractData(worldItem.Value));
			if (cellFace.HasValue)
			{
				int cellValue = SubsystemTerrain.Terrain.GetCellValue(cellFace.Value.X, cellFace.Value.Y, cellFace.Value.Z);
				if (worldItem.Velocity.Length() > 30f)
				{
					Utils.SubsystemExplosions.TryExplodeBlock(cellFace.Value.X, cellFace.Value.Y, cellFace.Value.Z, cellValue);
				}
				if (BlocksManager.Blocks[Terrain.ExtractContents(cellValue)].Density >= 1.5f && worldItem.Velocity.Length() > 30f)
				{
					const float num = 1f;
					const float minDistance = 8f;
					if (Utils.Random.UniformFloat(0f, 1f) < num)
					{
						Utils.SubsystemAudio.PlayRandomSound("Audio/Ricochets", 1f, Utils.Random.UniformFloat(-0.2f, 0.2f), new Vector3((float)cellFace.Value.X, (float)cellFace.Value.Y, (float)cellFace.Value.Z), minDistance, true);
						return false;
					}
				}
			}
			return true;
		}
	}
}
