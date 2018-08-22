using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	public class SubsystemBoatIBlockBehavior : SubsystemBlockBehavior
	{
		private readonly Random m_random = new Random();

		private SubsystemAudio m_subsystemAudio;

		private SubsystemBodies m_subsystemBodies;

		public override int[] HandledBlocks
		{
			get
			{
				return new int[]
				{
					513
				};
			}
		}

		public override bool OnUse(Vector3 start, Vector3 direction, ComponentMiner componentMiner)
		{
			if (Terrain.ExtractContents(componentMiner.ActiveBlockValue) == 513)
			{
				TerrainRaycastResult? nullable = componentMiner.PickTerrainForDigging(start, direction);
				if (nullable.HasValue)
				{
					Vector3 vector = nullable.Value.RaycastStart + Vector3.Normalize(nullable.Value.RaycastEnd - nullable.Value.RaycastStart) * nullable.Value.Distance;
					var result = new DynamicArray<ComponentBody>();
					m_subsystemBodies.FindBodiesInArea(new Vector2(vector.X, vector.Z) - new Vector2(8f), new Vector2(vector.X, vector.Z) + new Vector2(8f), result);
					Entity entity = DatabaseManager.CreateEntity(Project, "BoatI", true);
					entity.FindComponent<ComponentFrame>(true).Position = vector;
					entity.FindComponent<ComponentFrame>(true).Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, m_random.UniformFloat(0f, 6.283185f));
					entity.FindComponent<ComponentSpawn>(true).SpawnDuration = 0f;
					Project.AddEntity(entity);
					componentMiner.RemoveActiveTool(1);
					m_subsystemAudio.PlaySound("Audio/BlockPlaced", 1f, 0f, vector, 3f, true);
					return true;
				}
			}
			return false;
		}

		public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			m_subsystemAudio = Project.FindSubsystem<SubsystemAudio>(true);
			m_subsystemBodies = Project.FindSubsystem<SubsystemBodies>(true);
		}
	}
}
