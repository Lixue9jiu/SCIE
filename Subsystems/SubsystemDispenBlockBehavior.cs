using Engine;
using System.Collections.Generic;
using TemplatesDatabase;

namespace Game
{
	public class SubsystemDispenBlockBehavior : SubsystemBlockBehavior
	{
		protected readonly Random m_random = new Random();

		protected SubsystemTime m_subsystemTime;

		//protected readonly Dictionary<Point3, bool> m_toDegrade = new Dictionary<Point3, bool>();

		//protected readonly Dictionary<Point3, bool> m_toHydrate = new Dictionary<Point3, bool>();

		protected SubsystemProjectiles m_subsystemProjectiles;

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
			if (!worldItem.ToRemove)
			{
				CellFace.FaceToVector3(cellFace.Face);
				CellFace.FaceToVector3(FourDirectionalBlock.GetDirection(SubsystemTerrain.Terrain.GetCellValue(cellFace.X, cellFace.Y, cellFace.Z)));
				Vector3 v = CellFace.FaceToVector3((SubsystemTerrain.Terrain.GetCellValue(cellFace.X, cellFace.Y, cellFace.Z) - DispenBlock.Index) >> 14);
				Vector3 position = new Vector3((float)cellFace.X + 0.5f, (float)cellFace.Y + 0.5f, (float)cellFace.Z + 0.5f) + 0.75f * v;
				m_subsystemProjectiles.FireProjectile(worldItem.Value, position, 30f * v, Vector3.Zero, null);
				worldItem.ToRemove = true;
			}
		}
	}
}
