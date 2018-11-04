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