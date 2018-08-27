using Engine;
using System.Collections.Generic;
using TemplatesDatabase;

namespace Game
{
	public class SubsystemEngineBlockBehavior : SubsystemFurnaceNBlockBehavior
	{
		public override int[] HandledBlocks
		{
			get
			{
				return new []
				{
					EngineBlock.Index
				};
			}
		}

		public override void OnBlockAdded(int value, int oldValue, int x, int y, int z)
		{
			if (Terrain.ExtractContents(oldValue) != EngineBlock.Index)
			{
				var valuesDictionary = new ValuesDictionary();
				valuesDictionary.PopulateFromDatabaseObject(Project.GameDatabase.Database.FindDatabaseObject("SteamEngine", Project.GameDatabase.EntityTemplateType, true));
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
			ComponentEngine componentFurnace = blockEntity.Entity.FindComponent<ComponentEngine>(true);
			componentMiner.ComponentPlayer.ComponentGui.ModalPanelWidget = new EngineWidget(componentMiner.Inventory, componentFurnace);
			AudioManager.PlaySound("Audio/UI/ButtonClick", 1f, 0f, 0f);
			return true;
		}
	}
}
