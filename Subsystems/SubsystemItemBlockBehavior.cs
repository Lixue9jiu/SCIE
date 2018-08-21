using Engine;
namespace Game
{
	public class SubsystemItemBlockBehavior : SubsystemThrowableBlockBehavior
	{
		public override int[] HandledBlocks => new int[] { ItemBlock.Index };

		public override bool OnAim(Vector3 start, Vector3 direction, ComponentMiner componentMiner, AimState state)
		{
			int value = componentMiner.ActiveBlockValue;
			return BlocksManager.Blocks[Terrain.ExtractContents(value)] is ItemBlock item && item.GetItem(ref value) is OreChunkBlock && base.OnAim(start, direction, componentMiner, state);
		}
		public override bool OnUse(Vector3 start, Vector3 direction, ComponentMiner componentMiner)
		{
			if (componentMiner.ActiveBlockValue == ItemBlock.IdTable["Train"])
			{
				var result = componentMiner.PickTerrainForDigging(start, direction);
				if (result.HasValue && Terrain.ExtractContents(result.Value.Value) == RailBlock.Index)
				{
					var cell = result.Value.CellFace;
					var entity = DatabaseManager.CreateEntity(Project, "Train", true);
					entity.FindComponent<ComponentBody>(true).Position = new Vector3(cell.X + 0.5f, cell.Y + 1f, cell.Z + 0.5f);
					entity.FindComponent<ComponentSpawn>(true).SpawnDuration = 0f;
					Project.AddEntity(entity);
				}
				else
				{
					var body = componentMiner.PickBody(start, direction);
					if (body.HasValue && body.Value.ComponentBody.Entity.FindComponent<ComponentTrain>() != null)
						Project.RemoveEntity(body.Value.ComponentBody.Entity, true);
				}
			}
			return false;
		}
	}
}
