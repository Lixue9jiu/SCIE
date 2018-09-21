using Engine;

namespace Game
{
	public class SubsystemDispenserNBlockBehavior : SubsystemBlockBehavior
	{
		public override int[] HandledBlocks
		{
			get
			{
				return new[] { DispenserNBlock.Index };
			}
		}

		public override void OnHitByProjectile(CellFace cellFace, WorldItem worldItem)
		{
			Vector3 v = CellFace.FaceToVector3(cellFace.Face);
			Vector3 position = new Vector3((float)cellFace.X + 0.5f, (float)cellFace.Y + 0.5f, (float)cellFace.Z + 0.5f) - 0.75f * v;
			if (ComponentEngine.IsPowered(SubsystemTerrain.Terrain, cellFace.X, cellFace.Y, cellFace.Z))
			{
				int l;
				if (worldItem.Velocity.Length() >= 20f && BlocksManager.Blocks[Terrain.ExtractContents(worldItem.Value)].DefaultDropCount >= 2f)
				{
					for (l = 0; (float)l <= BlocksManager.Blocks[Terrain.ExtractContents(worldItem.Value)].DefaultDropCount + 1f; l++)
					{
						Utils.SubsystemProjectiles.FireProjectile(BlocksManager.Blocks[Terrain.ExtractContents(worldItem.Value)].DefaultDropContent, position, -20f * v, Vector3.Zero, null);
					}
					worldItem.ToRemove = true;
				}
				else
				{
					if (worldItem.Velocity.Length() >= 20f && BlocksManager.Blocks[Terrain.ExtractContents(worldItem.Value)].DefaultDropContent == 5)
					{
						Utils.SubsystemProjectiles.FireProjectile(BlocksManager.Blocks[Terrain.ExtractContents(worldItem.Value)].DefaultDropContent, position, -20f * v, Vector3.Zero, null);
						worldItem.ToRemove = true;
					}
					else if (worldItem.Velocity.Length() >= 20f && BlocksManager.Blocks[Terrain.ExtractContents(worldItem.Value)].BlockIndex == 6)
					{
						for (l = 0; l <= 4; l++)
						{
							Utils.SubsystemProjectiles.FireProjectile(79, position, -20f * v, Vector3.Zero, null);
						}
						worldItem.ToRemove = true;
					}
				}
			}
		}
	}
}
