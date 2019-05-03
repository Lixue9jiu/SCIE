using Engine;

namespace Game
{
	public partial class SubsystemMineral
	{
		public override bool OnHitAsProjectile(CellFace? cellFace, ComponentBody componentBody, WorldItem worldItem)
		{
			if (Terrain.ExtractContents(worldItem.Value) != Bullet2Block.Index)
				return false;
			if (cellFace.HasValue)
			{
				var value = cellFace.Value;
				int cellValue = SubsystemTerrain.Terrain.GetCellValue(value.X, value.Y, value.Z);
				if (worldItem.Velocity.Length() > 30f)
					Utils.SubsystemExplosions.TryExplodeBlock(value.X, value.Y, value.Z, cellValue);
				if (BlocksManager.Blocks[Terrain.ExtractContents(cellValue)].Density >= 1.5f && worldItem.Velocity.Length() > 30f)
				{
					const float num = 1f;
					const float minDistance = 8f;
					if (Utils.Random.UniformFloat(0f, 1f) < num)
					{
						Utils.SubsystemAudio.PlayRandomSound("Audio/Ricochets", 1f, Utils.Random.UniformFloat(-0.2f, 0.2f), new Vector3(value.X, value.Y, value.Z), minDistance, true);
						return false;
					}
				}
			}
			return true;
		}
	}
}