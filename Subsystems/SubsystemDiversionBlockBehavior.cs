using Engine;

namespace Game
{
	public class SubsystemDiversionBlockBehavior : SubsystemBlockBehavior
	{
		public override int[] HandledBlocks => new[] { IceBlock.Index, DiversionBlock.Index };

		public override void OnHitByProjectile(CellFace cellFace, WorldItem worldItem)
		{
			if (worldItem.ToRemove)
				return;
			int value = SubsystemTerrain.Terrain.GetCellValue(cellFace.X, cellFace.Y, cellFace.Z);
			if (Terrain.ExtractContents(value) != DiversionBlock.Index) return;
			Vector3 v = CellFace.FaceToVector3(value - DiversionBlock.Index >> 14);
			Utils.SubsystemProjectiles.FireProjectile(worldItem.Value, new Vector3(cellFace.X + 0.5f, cellFace.Y + 0.5f, cellFace.Z + 0.5f) + 0.75f * v, 30f * v, Vector3.Zero, null);
			worldItem.ToRemove = true;
		}
		public override void OnCollide(CellFace cellFace, float velocity, ComponentBody componentBody)
		{
			int x = cellFace.X,
				y = cellFace.Y,
				z = cellFace.Z;
			if (Utils.Terrain.GetCellContentsFast(x, y, z) != IceBlock.Index || componentBody.Mass < 999f)
				return;
			SubsystemTerrain.DestroyCell(0, x, y, z, WaterBlock.Index, false, false);
		}
	}
}