using Engine;
using TemplatesDatabase;

namespace Game
{
	public class SubsystemEngineHBlockBehavior : SubsystemFurnaceNBlockBehavior
	{
		public override int[] HandledBlocks
		{
			get
			{
				return new []
				{
					EngineHBlock.Index
				};
			}
		}

		public override void OnBlockAdded(int value, int oldValue, int x, int y, int z)
		{
			if (Terrain.ExtractContents(oldValue) != EngineHBlock.Index)
			{
				var valuesDictionary = new ValuesDictionary();
				valuesDictionary.PopulateFromDatabaseObject(Project.GameDatabase.Database.FindDatabaseObject("HeatEngine", Project.GameDatabase.EntityTemplateType, true));
				valuesDictionary.GetValue<ValuesDictionary>("BlockEntity").SetValue("Coordinates", new Point3(x, y, z));
				Project.AddEntity(Project.CreateEntity(valuesDictionary));
			}
			if (FurnaceNBlock.GetHeatLevel(value) != 0)
			{
				AddFire(value, x, y, z);
			}
		}

		public override bool OnInteract(TerrainRaycastResult raycastResult, ComponentMiner componentMiner)
		{
			ComponentBlockEntity blockEntity = Project.FindSubsystem<SubsystemBlockEntities>(true).GetBlockEntity(raycastResult.CellFace.X, raycastResult.CellFace.Y, raycastResult.CellFace.Z);
			if (blockEntity == null || componentMiner.ComponentPlayer == null)
			{
				return false;
			}
			ComponentEngineH componentFurnace = blockEntity.Entity.FindComponent<ComponentEngineH>(true);
			componentMiner.ComponentPlayer.ComponentGui.ModalPanelWidget = new EngineHWidget(componentMiner.Inventory, componentFurnace);
			AudioManager.PlaySound("Audio/UI/ButtonClick", 1f, 0f, 0f);
			return true;
		}
	}
}
