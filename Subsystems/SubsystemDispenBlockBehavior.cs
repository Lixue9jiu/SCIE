using Engine;

namespace Game
{
	public class SubsystemDispenBlockBehavior : SubsystemBlockBehavior
	{
		public override int[] HandledBlocks
		{
			get
			{
				return new []
				{
					DispenBlock.Index
				};
			}
		}

		public override void OnHitByProjectile(CellFace cellFace, WorldItem worldItem)
		{
			if (!worldItem.ToRemove)
			{
				CellFace.FaceToVector3(cellFace.Face);
				CellFace.FaceToVector3(FourDirectionalBlock.GetDirection(SubsystemTerrain.Terrain.GetCellValue(cellFace.X, cellFace.Y, cellFace.Z)));
				Vector3 v = CellFace.FaceToVector3((SubsystemTerrain.Terrain.GetCellValue(cellFace.X, cellFace.Y, cellFace.Z) - DispenBlock.Index) >> 14);
				Vector3 position = new Vector3((float)cellFace.X + 0.5f, (float)cellFace.Y + 0.5f, (float)cellFace.Z + 0.5f) + 0.75f * v;
				Utils.SubsystemProjectiles.FireProjectile(worldItem.Value, position, 30f * v, Vector3.Zero, null);
				worldItem.ToRemove = true;
			}
		}
	}
}
