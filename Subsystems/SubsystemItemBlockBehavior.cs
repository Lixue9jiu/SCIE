using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	public class SubsystemItemBlockBehavior : SubsystemThrowableBlockBehavior
	{
		//public SubsystemBodies SubsystemBodies;
		public override int[] HandledBlocks => new int[] { 90, GunpowderBlock.Index, RottenMeatBlock.Index, ItemBlock.Index };

		public override bool OnAim(Vector3 start, Vector3 direction, ComponentMiner componentMiner, AimState state)
		{
			int value = componentMiner.ActiveBlockValue;
			if (!(BlocksManager.Blocks[Terrain.ExtractContents(value)] is ItemBlock itemblock))
				return false;
			var item = itemblock.GetItem(ref value);
			return (item is OreChunk || (item is Mould && !(item is Mine)) || value == ItemBlock.IdTable["RefractoryBrick"] || value == ItemBlock.IdTable["ScrapIron"]) && base.OnAim(start, direction, componentMiner, state);
		}
		public override bool OnUse(Vector3 start, Vector3 direction, ComponentMiner componentMiner)
		{
			int activeBlockValue = componentMiner.ActiveBlockValue;
			if (!(BlocksManager.Blocks[Terrain.ExtractContents(activeBlockValue)] is ItemBlock itemblock))
				return false;
			var result = componentMiner.PickTerrainForDigging(start, direction);
			Entity entity;
			if (activeBlockValue == ItemBlock.IdTable["Steam Locomotive"])
			{
				var body = componentMiner.PickBody(start, direction);
				if (body.HasValue && (!result.HasValue || body.Value.Distance < result.Value.Distance) && body.Value.ComponentBody.Entity.FindComponent<ComponentTrain>() != null)
				{
					Matrix matrix = componentMiner.ComponentCreature.ComponentBody.Matrix;
					Vector3 position = matrix.Translation + 1f * matrix.Forward + Vector3.UnitY;
					for (var i = body.Value.ComponentBody.Entity.FindComponents<IInventory>().GetEnumerator(); i.MoveNext();)
					{
						i.Current.DropAllItems(position);
					}
					Project.RemoveEntity(body.Value.ComponentBody.Entity, true);
					Utils.SubsystemPickables.AddPickable(activeBlockValue, 1, position, null, null);
				}
				else if (result.HasValue && Terrain.ExtractContents(result.Value.Value) == RailBlock.Index)
				{
					var position = new Vector3(result.Value.CellFace.Point) + new Vector3(0.5f);

					entity = DatabaseManager.CreateEntity(Project, "Train", true);
					entity.FindComponent<ComponentBody>(true).Position = position;
					entity.FindComponent<ComponentSpawn>(true).SpawnDuration = 0f;

					var rotation = componentMiner.ComponentCreature.ComponentCreatureModel.EyeRotation.ToForwardVector();
					entity.FindComponent<ComponentTrain>(true).SetDirectionImmediately(RailBlock.IsDirectionX(RailBlock.GetRailType(Terrain.ExtractData(result.Value.Value)))
						? rotation.Z < 0 ? 0 : 2
						: rotation.X < 0 ? 1 : 3);
					Project.AddEntity(entity);
					componentMiner.RemoveActiveTool(1);
					Utils.SubsystemAudio.PlaySound("Audio/BlockPlaced", 1f, 0f, position, 3f, true);
				}
			}
			else if (result.HasValue)
			{
				Vector3 position = result.Value.RaycastStart + Vector3.Normalize(result.Value.RaycastEnd - result.Value.RaycastStart) * result.Value.Distance;
				if (activeBlockValue == ItemBlock.IdTable["SteamBoat"])
				{
					//var result = new DynamicArray<ComponentBody>();
					//m_subsystemBodies.FindBodiesInArea(new Vector2(vector.X, vector.Z) - new Vector2(8f), new Vector2(vector.X, vector.Z) + new Vector2(8f), result);
					entity = DatabaseManager.CreateEntity(Project, "BoatI", true);
					entity.FindComponent<ComponentFrame>(true).Position = position;
					entity.FindComponent<ComponentFrame>(true).Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, m_random.UniformFloat(0f, 6.283185f));
					entity.FindComponent<ComponentSpawn>(true).SpawnDuration = 0f;
					Project.AddEntity(entity);
					componentMiner.RemoveActiveTool(1);
					Utils.SubsystemAudio.PlaySound("Audio/BlockPlaced", 1f, 0f, position, 3f, true);
					return true;
				}
				else if (activeBlockValue == ItemBlock.IdTable["Carriage"])
				{
					entity = DatabaseManager.CreateEntity(Project, "Carriage", true);
					entity.FindComponent<ComponentFrame>(true).Position = position;
					entity.FindComponent<ComponentFrame>(true).Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, m_random.UniformFloat(0f, 6.283185f));
					entity.FindComponent<ComponentSpawn>(true).SpawnDuration = 0f;
					var componentCarriage = entity.FindComponent<ComponentCarriage>(true);
					Project.AddEntity(entity);
					var componentMount = componentCarriage.FindNearestMount();
					if (componentMount != null)
					{
						componentCarriage.StartMounting(componentMount);
					}
					componentMiner.RemoveActiveTool(1);
					Utils.SubsystemAudio.PlaySound("Audio/BlockPlaced", 1f, 0f, position, 3f, true);
					return true;
				}
				else if (activeBlockValue == ItemBlock.IdTable["Airship"])
				{
					entity = DatabaseManager.CreateEntity(Project, "AirShip", true);
					entity.FindComponent<ComponentFrame>(true).Position = position;
					entity.FindComponent<ComponentFrame>(true).Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, m_random.UniformFloat(0f, 6.283185f));
					entity.FindComponent<ComponentSpawn>(true).SpawnDuration = 0f;
					Project.AddEntity(entity);
					componentMiner.RemoveActiveTool(1);
					Utils.SubsystemAudio.PlaySound("Audio/BlockPlaced", 1f, 0f, position, 3f, true);
					return true;
				}
				else if (itemblock.GetItem(ref activeBlockValue) is Mine mine)
				{
					entity = DatabaseManager.CreateEntity(Project, "Mine", new ValuesDictionary
					{
						{ "Mine", new ValuesDictionary { { "Type", mine.MineType } } }
					}, true);
					entity.FindComponent<ComponentFrame>(true).Position = position;
					entity.FindComponent<ComponentFrame>(true).Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, m_random.UniformFloat(0f, 6.283185f));
					entity.FindComponent<ComponentSpawn>(true).SpawnDuration = 0f;
					var componentMine = entity.FindComponent<ComponentMine>(true);
					componentMine.ExplosionPressure = mine.ExplosionPressure;
					componentMine.Delay = mine.Delay;
					Project.AddEntity(entity);
					componentMiner.RemoveActiveTool(1);
					Utils.SubsystemAudio.PlaySound("Audio/BlockPlaced", 1f, 0f, position, 3f, true);
					return true;
				}
			}
			else
			{
				IInventory inventory = componentMiner.Inventory;
				TerrainRaycastResult? result2;
				if (Terrain.ExtractContents(activeBlockValue) == 90)
				{
					result2 = componentMiner.PickTerrainForGathering(start, direction);
					if (result2.HasValue)
					{
						CellFace cellFace = result2.Value.CellFace;
						int cellValue = Terrain.ReplaceLight(Utils.Terrain.GetCellValue(cellFace.X, cellFace.Y, cellFace.Z), 0);
						if (cellValue != (RottenMeatBlock.Index | 1 << 4 << 14))
						{
							return false;
						}
						inventory.RemoveSlotItems(inventory.ActiveSlotIndex, inventory.GetSlotCount(inventory.ActiveSlotIndex));
						if (inventory.GetSlotCount(inventory.ActiveSlotIndex) == 0)
						{
							inventory.AddSlotItems(inventory.ActiveSlotIndex, RottenMeatBlock.Index | 2 << 4 << 14, 1);
						}
						Utils.SubsystemTerrain.DestroyCell(0, cellFace.X, cellFace.Y, cellFace.Z, 0, false, false);
						return true;
					}
				}
				if (activeBlockValue == (RottenMeatBlock.Index | 2 << 4 << 14))
				{
					result2 = componentMiner.PickTerrainForInteraction(start, direction);
					if (result2.HasValue && componentMiner.Place(result2.Value, RottenMeatBlock.Index | 1 << 4 << 14))
					{
						inventory.RemoveSlotItems(inventory.ActiveSlotIndex, 1);
						if (inventory.GetSlotCount(inventory.ActiveSlotIndex) == 0)
						{
							inventory.AddSlotItems(inventory.ActiveSlotIndex, Terrain.ReplaceContents(activeBlockValue, 90), 1);
						}
						return true;
					}
				}
			}
			return false;
		}
		public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			//SubsystemBodies = Project.FindSubsystem<SubsystemBodies>(true);
		}
	}
}
