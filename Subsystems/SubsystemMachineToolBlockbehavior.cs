using System;
using Engine;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020003D3 RID: 979
	public class SubsystemMachineToolBlockBehavior : SubsystemBlockBehavior
	{
		// Token: 0x170003B0 RID: 944
		// (get) Token: 0x060016C9 RID: 5833 RVA: 0x0000FAB1 File Offset: 0x0000DCB1
		public override int[] HandledBlocks
		{
			get
			{
				return new int[]
				{
					534
				};
			}
		}

		// Token: 0x060016CA RID: 5834 RVA: 0x00098604 File Offset: 0x00096804
		public override void OnBlockAdded(int value, int oldValue, int x, int y, int z)
		{
			DatabaseObject databaseObject = base.SubsystemTerrain.Project.GameDatabase.Database.FindDatabaseObject("MachineTool", base.SubsystemTerrain.Project.GameDatabase.EntityTemplateType, true);
			ValuesDictionary valuesDictionary = new ValuesDictionary();
			valuesDictionary.PopulateFromDatabaseObject(databaseObject);
			valuesDictionary.GetValue<ValuesDictionary>("BlockEntity").SetValue<Point3>("Coordinates", new Point3(x, y, z));
			base.SubsystemTerrain.Project.AddEntity(base.SubsystemTerrain.Project.CreateEntity(valuesDictionary));
		}

		// Token: 0x060016CB RID: 5835 RVA: 0x00098694 File Offset: 0x00096894
		public override void OnBlockRemoved(int value, int newValue, int x, int y, int z)
		{
			ComponentBlockEntity blockEntity = base.SubsystemTerrain.Project.FindSubsystem<SubsystemBlockEntities>(true).GetBlockEntity(x, y, z);
			if (blockEntity == null)
			{
				return;
			}
			Vector3 position = new Vector3((float)x, (float)y, (float)z) + new Vector3(0.5f);
			foreach (IInventory inventory in blockEntity.Entity.FindComponents<IInventory>())
			{
				inventory.DropAllItems(position);
			}
			base.SubsystemTerrain.Project.RemoveEntity(blockEntity.Entity, true);
		}

		// Token: 0x060016CC RID: 5836 RVA: 0x00098744 File Offset: 0x00096944
		public override bool OnInteract(TerrainRaycastResult raycastResult, ComponentMiner componentMiner)
		{
			ComponentBlockEntity blockEntity = base.SubsystemTerrain.Project.FindSubsystem<SubsystemBlockEntities>(true).GetBlockEntity(raycastResult.CellFace.X, raycastResult.CellFace.Y, raycastResult.CellFace.Z);
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
