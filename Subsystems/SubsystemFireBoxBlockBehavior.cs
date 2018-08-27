using System.Collections.Generic;
using Engine;
using TemplatesDatabase;

namespace Game
{
	public class SubsystemFireBoxBlockBehavior : SubsystemFurnaceNBlockBehavior
	{
		public override int[] HandledBlocks
		{
			get
			{
				return new []
				{
					FireBoxBlock.Index
				};
			}
		}
		
		public override void OnBlockAdded(int value, int oldValue, int x, int y, int z)
		{
			if (Terrain.ExtractContents(oldValue) != FireBoxBlock.Index)
			{
				DatabaseObject databaseObject = Project.GameDatabase.Database.FindDatabaseObject("FireBox", Project.GameDatabase.EntityTemplateType, true);
				ValuesDictionary valuesDictionary = new ValuesDictionary();
				valuesDictionary.PopulateFromDatabaseObject(databaseObject);
				valuesDictionary.GetValue<ValuesDictionary>("BlockEntity").SetValue<Point3>("Coordinates", new Point3(x, y, z));
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
			ComponentFireBox componentFurnace = blockEntity.Entity.FindComponent<ComponentFireBox>(true);
			componentMiner.ComponentPlayer.ComponentGui.ModalPanelWidget = new FireBoxWidget(componentMiner.Inventory, componentFurnace);
			AudioManager.PlaySound("Audio/UI/ButtonClick", 1f, 0f, 0f);
			return true;
		}
		
		/*public override void OnNeighborBlockChanged(int x, int y, int z, int neighborX, int neighborY, int neighborZ)
		{
			base.OnNeighborBlockChanged(x, y, z, neighborX, neighborY, neighborZ);
		}*/
	}
}
