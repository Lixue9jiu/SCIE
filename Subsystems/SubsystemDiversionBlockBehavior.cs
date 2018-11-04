using Engine;

namespace Game
{
	public class SubsystemDiversionBlockBehavior : SubsystemBlockBehavior
	{
		public override int[] HandledBlocks => new[] { DiversionBlock.Index };

		public override void OnHitByProjectile(CellFace cellFace, WorldItem worldItem)
		{
			if (worldItem.ToRemove)
			{
				return;
			}
			//CellFace.FaceToVector3(cellFace.Face);
			//CellFace.FaceToVector3(FourDirectionalBlock.GetDirection(SubsystemTerrain.Terrain.GetCellValue(cellFace.X, cellFace.Y, cellFace.Z)));
			Vector3 v = CellFace.FaceToVector3((SubsystemTerrain.Terrain.GetCellValue(cellFace.X, cellFace.Y, cellFace.Z) - DiversionBlock.Index) >> 14);
			Utils.SubsystemProjectiles.FireProjectile(worldItem.Value, new Vector3((float)cellFace.X + 0.5f, (float)cellFace.Y + 0.5f, (float)cellFace.Z + 0.5f) + 0.75f * v, 30f * v, Vector3.Zero, null);
			worldItem.ToRemove = true;
		}
	}
}