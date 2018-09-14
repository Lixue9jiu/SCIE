using Engine;
using TemplatesDatabase;

namespace Game
{
	public class SubsystemItemBlockBehavior : SubsystemThrowableBlockBehavior
	{
		protected SubsystemBodies m_subsystemBodies;
		protected SubsystemPickables m_subsystemPickables;
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
					Matrix matrix = componentMiner.ComponentCreature.ComponentBody.Matrix;
					Vector3 position = matrix.Translation + 1f * matrix.Forward + 1f * Vector3.UnitY;
					for (var i = body.Value.ComponentBody.Entity.FindComponents<IInventory>().GetEnumerator(); i.MoveNext();)
					{
						i.Current.DropAllItems(position);
					}
					Project.RemoveEntity(body.Value.ComponentBody.Entity, true);
					m_subsystemPickables.AddPickable(componentMiner.ActiveBlockValue, 1, position, null, null);
				}
				else if (result.HasValue && Terrain.ExtractContents(result.Value.Value) == RailBlock.Index)
				{
					var cell = result.Value.CellFace;
					var position = new Vector3(cell.X + 0.5f, cell.Y + 0.01f, cell.Z + 0.5f);

					var entity = DatabaseManager.CreateEntity(Project, "Train", true);
					entity.FindComponent<ComponentBody>(true).Position = position;
					entity.FindComponent<ComponentSpawn>(true).SpawnDuration = 0f;

					int dir;
					var rotation = componentMiner.ComponentCreature.ComponentCreatureModel.EyeRotation.ToForwardVector();
					if (RailBlock.IsDirectionX(RailBlock.GetRailType(Terrain.ExtractData(result.Value.Value))))
						dir = rotation.Z < 0 ? 0 : 2;
					else
						dir = rotation.X < 0 ? 1 : 3;
					entity.FindComponent<ComponentTrain>(true).SetDirectionImmediately(dir);
					Project.AddEntity(entity);
					m_subsystemAudio.PlaySound("Audio/BlockPlaced", 1f, 0f, position, 3f, true);
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
			m_subsystemBodies = Project.FindSubsystem<SubsystemBodies>(true);
			m_subsystemPickables = Project.FindSubsystem<SubsystemPickables>(true);
		}
	}
}
