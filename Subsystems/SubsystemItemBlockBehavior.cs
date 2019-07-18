using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	public class SubsystemItemBlockBehavior : SubsystemThrowableBlockBehavior
	{
		public override int[] HandledBlocks => new int[] { 90, RottenMeatBlock.Index, ItemBlock.Index };

		public override bool OnAim(Vector3 start, Vector3 direction, ComponentMiner componentMiner, AimState state)
		{
			int value = componentMiner.ActiveBlockValue;
			if (!(BlocksManager.Blocks[Terrain.ExtractContents(value)] is ItemBlock block))
				return false;
			var item = block.GetItem(ref value);
			return (item is OreChunk || (item is Mould && !(item is Mine)) || value == ItemBlock.IdTable["RefractoryBrick"] || value == ItemBlock.IdTable["ScrapIron"]) && base.OnAim(start, direction, componentMiner, state);
		}

		public override bool OnUse(Vector3 start, Vector3 direction, ComponentMiner componentMiner)
		{
			int activeBlockValue = componentMiner.ActiveBlockValue;
			var result = componentMiner.PickTerrainForDigging(start, direction);
			Entity entity;
			Vector3 position;
			BodyRaycastResult? body;
			if (activeBlockValue == ItemBlock.IdTable["Wrench"])
			{
				body = componentMiner.PickBody(start, direction);
				Matrix matrix = componentMiner.ComponentCreature.ComponentBody.Matrix;
				position = matrix.Translation + 1f * matrix.Forward + Vector3.UnitY;
				if (body.HasValue && (!result.HasValue || body.Value.Distance < result.Value.Distance))
				{
					entity = body.Value.ComponentBody.Entity;
					if (entity.FindComponent<ComponentTrain>() != null || entity.FindComponent<ComponentBoatI>() != null)
					{
						for (var i = entity.FindComponents<IInventory>().GetEnumerator(); i.MoveNext();)
							i.Current.DropAllItems(position);
						Utils.SubsystemPickables.AddPickable(ItemBlock.IdTable[entity.ValuesDictionary.DatabaseObject.Name.Length == 8 ? "Minecart" : entity.ValuesDictionary.DatabaseObject.Name], 1, position, null, null);
						Project.RemoveEntity(entity, true);
						return true;
					}
				}
			}
			else if ((activeBlockValue == ItemBlock.IdTable["Train"] || activeBlockValue == ItemBlock.IdTable["Minecart"]) && result.HasValue && Terrain.ExtractContents(result.Value.Value) == RailBlock.Index)
			{
				position = new Vector3(result.Value.CellFace.Point) + new Vector3(0.5f);
				entity = DatabaseManager.CreateEntity(Project, activeBlockValue == ItemBlock.IdTable["Minecart"] ? "Carriage" : "Train", true);

				var rotation = componentMiner.ComponentCreature.ComponentCreatureModel.EyeRotation.ToForwardVector();
				entity.FindComponent<ComponentTrain>(true).SetDirection(RailBlock.IsDirectionX(RailBlock.GetRailType(Terrain.ExtractData(result.Value.Value)))
					? rotation.Z < 0 ? 0 : 2
					: rotation.X < 0 ? 1 : 3);
				entity.FindComponent<ComponentBody>(true).Position = position;
				entity.FindComponent<ComponentSpawn>(true).SpawnDuration = 0f;
				Project.AddEntity(entity);
				var componentTrain = entity.FindComponent<ComponentTrain>(true);
				if (activeBlockValue == ItemBlock.IdTable["Minecart"])
				{
					componentTrain.Update(0);
					var train = componentTrain.FindNearestTrain();
					if (train != null)
						componentTrain.ParentBody = train;
				}
				componentMiner.RemoveActiveTool(1);
				Utils.SubsystemAudio.PlaySound("Audio/BlockPlaced", 1f, 0f, position, 3f, true);
				return true;
			}
			/*else if (activeBlockValue == ItemBlock.IdTable["Minecart"])
			{
				entity = DatabaseManager.CreateEntity(Project, "Carriage", true);
				body = componentMiner.PickBody(start, direction);
				if (body.HasValue && (!result.HasValue || body.Value.Distance < result.Value.Distance))
				{
					body = componentMiner.PickBody(start, direction);
					var componentTrain = entity.FindComponent<ComponentTrain>(true);
					var train = body.Value.ComponentBody.Entity.FindComponent<ComponentTrain>();
					if (train != null)
						componentTrain.ParentBody = train;
				}
				else if (result.HasValue)
					position = result.Value.RaycastStart + Vector3.Normalize(result.Value.RaycastEnd - result.Value.RaycastStart) * result.Value.Distance; ;
				var rotation = componentMiner.ComponentCreature.ComponentCreatureModel.EyeRotation.ToForwardVector();
				entity.FindComponent<ComponentTrain>(true).SetDirection(RailBlock.IsDirectionX(RailBlock.GetRailType(Terrain.ExtractData(result.Value.Value)))
					? rotation.Z < 0 ? 0 : 2
					: rotation.X < 0 ? 1 : 3);
				goto put;
			}*/
			else if (activeBlockValue == ItemBlock.IdTable["基因查看器"])
			{
				body = componentMiner.PickBody(start, direction);
				if (body.HasValue && (!result.HasValue || body.Value.Distance < result.Value.Distance))
				{
					var cv = body.Value.ComponentBody.Entity.FindComponent<ComponentVariant>();
					if (cv != null)
						DialogsManager.ShowDialog(componentMiner.ComponentPlayer?.View.GameWidget, new MessageDialog("Result", cv.Genome.ToString(), "OK", null, null));
					return true;
				}
			}
			else if (result.HasValue)
			{
				position = result.Value.RaycastStart + Vector3.Normalize(result.Value.RaycastEnd - result.Value.RaycastStart) * result.Value.Distance;
				if (activeBlockValue == ItemBlock.IdTable["SteamBoat"])
				{
					entity = DatabaseManager.CreateEntity(Project, "SteamBoat", true);
					entity.FindComponent<ComponentFrame>(true).Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, m_random.UniformFloat(0f, 6.283185f));
					goto put;
				}
				/*else if (activeBlockValue == ItemBlock.IdTable["Minecart"])
				{
					entity = DatabaseManager.CreateEntity(Project, "Carriage", true);
					entity.FindComponent<ComponentFrame>(true).Position = position;
					entity.FindComponent<ComponentSpawn>(true).SpawnDuration = 0f;
					var componentTrain = entity.FindComponent<ComponentTrain>(true);
					var componentMount = componentTrain.FindNearestTrain();
					if (componentMount != null)
						componentTrain.ParentBody = componentMount.m_componentBody;
					goto put;
				}*/
				else if (activeBlockValue == ItemBlock.IdTable["Airship"])
				{
					entity = DatabaseManager.CreateEntity(Project, "Airship", true);
					entity.FindComponent<ComponentFrame>(true).Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, m_random.UniformFloat(0f, 6.283185f));
					goto put;
				}
				else if (BlocksManager.Blocks[Terrain.ExtractContents(activeBlockValue)] is ItemBlock item && item.GetItem(ref activeBlockValue) is Mine mine)
				{
					entity = DatabaseManager.CreateEntity(Project, "Mine", new ValuesDictionary
					{
						{ "Mine", new ValuesDictionary { { "Type", mine.MineType } } }
					}, true);
					entity.FindComponent<ComponentFrame>(true).Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, m_random.UniformFloat(0f, 6.283185f));
					var componentMine = entity.FindComponent<ComponentMine>(true);
					componentMine.ExplosionPressure = mine.ExplosionPressure;
					componentMine.Delay = mine.Delay;
					goto put;
				}
			}
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
						return false;
					inventory.RemoveSlotItems(inventory.ActiveSlotIndex, inventory.GetSlotCount(inventory.ActiveSlotIndex));
					if (inventory.GetSlotCount(inventory.ActiveSlotIndex) == 0)
						inventory.AddSlotItems(inventory.ActiveSlotIndex, RottenMeatBlock.Index | 2 << 4 << 14, 1);
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
						inventory.AddSlotItems(inventory.ActiveSlotIndex, Terrain.ReplaceContents(activeBlockValue, 90), 1);
					return true;
				}
			}
			return false;
			put:
			entity.FindComponent<ComponentBody>(true).Position = position;
			entity.FindComponent<ComponentSpawn>(true).SpawnDuration = 0f;
			Project.AddEntity(entity);
			componentMiner.RemoveActiveTool(1);
			Utils.SubsystemAudio.PlaySound("Audio/BlockPlaced", 1f, 0f, position, 3f, true);
			return true;
		}
	}
}