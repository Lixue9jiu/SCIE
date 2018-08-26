using System;
using Engine;
using TemplatesDatabase;

namespace Game
{
	public class SubsystemMachineToolBlockBehavior : SubsystemBlockBehavior
	{
		public override int[] HandledBlocks
		{
			get
			{
				return new []
				{
					MachineToolBlock.Index
				};
			}
		}
		
		public override void OnBlockAdded(int value, int oldValue, int x, int y, int z)
		{
			var valuesDictionary = new ValuesDictionary();
			valuesDictionary.PopulateFromDatabaseObject(Project.GameDatabase.Database.FindDatabaseObject("MachineTool", Project.GameDatabase.EntityTemplateType, true));
			valuesDictionary.GetValue<ValuesDictionary>("BlockEntity").SetValue<Point3>("Coordinates", new Point3(x, y, z));
			Project.AddEntity(Project.CreateEntity(valuesDictionary));
		}
		
		public override void OnBlockRemoved(int value, int newValue, int x, int y, int z)
		{
			ComponentBlockEntity blockEntity = Project.FindSubsystem<SubsystemBlockEntities>(true).GetBlockEntity(x, y, z);
			if (blockEntity == null)
			{
				return;
			}
			var position = new Vector3((float)x, (float)y, (float)z) + new Vector3(0.5f);
			foreach (IInventory inventory in blockEntity.Entity.FindComponents<IInventory>())
			{
				inventory.DropAllItems(position);
			}
			Project.RemoveEntity(blockEntity.Entity, true);
		}
		
		public override bool OnInteract(TerrainRaycastResult raycastResult, ComponentMiner componentMiner)
		{
			ComponentBlockEntity blockEntity = Project.FindSubsystem<SubsystemBlockEntities>(true).GetBlockEntity(raycastResult.CellFace.X, raycastResult.CellFace.Y, raycastResult.CellFace.Z);
			if (blockEntity == null || componentMiner.ComponentPlayer == null)
			{
				return false;
			}
			ComponentLargeCraftingTable component = blockEntity.Entity.FindComponent<ComponentLargeCraftingTable>(true);
			componentMiner.ComponentPlayer.ComponentGui.ModalPanelWidget = new MachineToolWidget(componentMiner.Inventory, component);
			AudioManager.PlaySound("Audio/UI/ButtonClick", 1f, 0f, 0f);
			return true;
		}
	}
}
