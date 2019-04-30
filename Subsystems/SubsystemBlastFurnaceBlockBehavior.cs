using Engine;
using TemplatesDatabase;

namespace Game
{
	public class SubsystemBlastFurnaceBlockBehavior : SubsystemCraftingTableBlockBehavior
	{
		public override int[] HandledBlocks => new[]
		{
			BlastFurnaceBlock.Index,
			CovenBlock.Index,
			HearthFurnaceBlock.Index,
            FractionatingTowerBlock.Index
		};

		public override void OnBlockAdded(int value, int oldValue, int x, int y, int z)
		{
			string name;
			switch (Terrain.ExtractContents(value))
			{
				case BlastFurnaceBlock.Index: name = "BlastFurnace"; break;
				case CovenBlock.Index: name = "CokeOven"; break;
				case HearthFurnaceBlock.Index: name = "HearthFurnace"; break;
                case FractionatingTowerBlock.Index: name = "FractionalTower"; break;
                default: return;
			}
			var vd = new ValuesDictionary();
			vd.PopulateFromDatabaseObject(Project.GameDatabase.Database.FindDatabaseObject(name, Project.GameDatabase.EntityTemplateType, true));
			vd.GetValue<ValuesDictionary>("BlockEntity").SetValue("Coordinates", new Point3(x, y, z));
			Project.AddEntity(Project.CreateEntity(vd));
		}

		public override void OnHitByProjectile(CellFace cellFace, WorldItem worldItem)
		{
			Utils.OnHitByProjectile(cellFace, worldItem);
		}

		public override bool OnInteract(TerrainRaycastResult raycastResult, ComponentMiner componentMiner)
		{
			var blockEntity = Utils.GetBlockEntity(raycastResult.CellFace.Point);
			if (blockEntity == null || componentMiner.ComponentPlayer == null)
				return false;
			switch (Terrain.ExtractContents(raycastResult.Value))
			{
				case BlastFurnaceBlock.Index:
					componentMiner.ComponentPlayer.ComponentGui.ModalPanelWidget = new BlastFurnaceWidget(componentMiner.Inventory, blockEntity.Entity.FindComponent<ComponentBlastFurnace>(true));
					AudioManager.PlaySound("Audio/UI/ButtonClick", 1f, 0f, 0f);
					break;
				case CovenBlock.Index:
					componentMiner.ComponentPlayer.ComponentGui.ModalPanelWidget = new CovenWidget(componentMiner.Inventory, blockEntity.Entity.FindComponent<ComponentCoven>(true));
					break;
				case HearthFurnaceBlock.Index:
					componentMiner.ComponentPlayer.ComponentGui.ModalPanelWidget = new HearthFurnaceWidget(componentMiner.Inventory, blockEntity.Entity.FindComponent<ComponentHearthFurnace>(true));
					break;
                case FractionatingTowerBlock.Index:
                    componentMiner.ComponentPlayer.ComponentGui.ModalPanelWidget = new FractionalTowerWidget(componentMiner.Inventory, blockEntity.Entity.FindComponent<ComponentFractionalTower>(true));
                    break;
                default: return false;
			}
			AudioManager.PlaySound("Audio/UI/ButtonClick", 1f, 0f, 0f);
			return true;
		}
	}
}
