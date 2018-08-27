using Engine;
using TemplatesDatabase;

namespace Game
{
	public class SubsystemItemBlockBehavior : SubsystemThrowableBlockBehavior
	{
		protected SubsystemBodies m_subsystemBodies;
		public override int[] HandledBlocks => new int[] { ItemBlock.Index };

		public override bool OnAim(Vector3 start, Vector3 direction, ComponentMiner componentMiner, AimState state)
		{
			int value = componentMiner.ActiveBlockValue;
			if (BlocksManager.Blocks[Terrain.ExtractContents(value)] is ItemBlock itemblock)
			{
				Item item = itemblock.GetItem(ref value);
				return (item is OreChunk || item is Mould || value == ItemBlock.IdTable["RefractoryBrick"] || value == ItemBlock.IdTable["ScrapIron"]) && base.OnAim(start, direction, componentMiner, state);
			}
			return false;
		}
		public override bool OnUse(Vector3 start, Vector3 direction, ComponentMiner componentMiner)
		{
			if (componentMiner.ActiveBlockValue == ItemBlock.IdTable["Steam Locomotive"])
			{
				var body = componentMiner.PickBody(start, direction);
				var result = componentMiner.PickTerrainForDigging(start, direction);
				if (body.HasValue && (!result.HasValue || body.Value.Distance < result.Value.Distance) && body.Value.ComponentBody.Entity.FindComponent<ComponentTrain>() != null)
				{
					Project.RemoveEntity(body.Value.ComponentBody.Entity, true);
				}
				else if (result.HasValue && Terrain.ExtractContents(result.Value.Value) == RailBlock.Index)
				{
					var cell = result.Value.CellFace;
					var entity = DatabaseManager.CreateEntity(Project, "Train", true);
					entity.FindComponent<ComponentBody>(true).Position = new Vector3(cell.X + 0.5f, cell.Y + 1f, cell.Z + 0.5f);
					entity.FindComponent<ComponentFrame>(true).Rotation = componentMiner.ComponentCreature.ComponentBody.Rotation;
					entity.FindComponent<ComponentSpawn>(true).SpawnDuration = 0f;
					Project.AddEntity(entity);
				}
			}
			else if (componentMiner.ActiveBlockValue == ItemBlock.IdTable["SteamBoat"])
			{
				var nullable = componentMiner.PickTerrainForDigging(start, direction);
				if (nullable.HasValue)
				{
					Vector3 vector = nullable.Value.RaycastStart + Vector3.Normalize(nullable.Value.RaycastEnd - nullable.Value.RaycastStart) * nullable.Value.Distance;
					//var result = new DynamicArray<ComponentBody>();
					//m_subsystemBodies.FindBodiesInArea(new Vector2(vector.X, vector.Z) - new Vector2(8f), new Vector2(vector.X, vector.Z) + new Vector2(8f), result);
					var entity = DatabaseManager.CreateEntity(Project, "BoatI", true);
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
