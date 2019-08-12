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
			TankBlock.Index
		};

		public override void OnBlockAdded(int value, int oldValue, int x, int y, int z)
		{
			string name;
			switch (Terrain.ExtractContents(value))
			{
				case BlastFurnaceBlock.Index: name = "BlastFurnace"; break;
				case CovenBlock.Index: name = "CokeOven"; break;
				case HearthFurnaceBlock.Index: name = "HearthFurnace"; break;
                case TankBlock.Index:
					if (TankBlock.GetType(value) == TankBlock.Type.FractionatingTower)
					{ name = "FractionalTower"; break; }
					return;
				default: return;
			}
			Project.CreateBlockEntity(name, new Point3(x, y, z));
		}

		public override void OnHitByProjectile(CellFace cellFace, WorldItem worldItem) => Utils.OnHitByProjectile(cellFace, worldItem);

		public override bool OnInteract(TerrainRaycastResult raycastResult, ComponentMiner componentMiner)
		{
			var blockEntity = Utils.GetBlockEntity(raycastResult.CellFace.Point);
			if (blockEntity == null || componentMiner.ComponentPlayer == null)
				return false;
			switch (Terrain.ExtractContents(raycastResult.Value))
			{
				case BlastFurnaceBlock.Index:
					componentMiner.ComponentPlayer.ComponentGui.ModalPanelWidget = new BlastFurnaceWidget(componentMiner.Inventory, blockEntity.Entity.FindComponent<ComponentBlastFurnace>(true));
					break;
				case CovenBlock.Index:
					componentMiner.ComponentPlayer.ComponentGui.ModalPanelWidget = new CovenWidget(componentMiner.Inventory, blockEntity.Entity.FindComponent<ComponentCoven>(true));
					break;
				case HearthFurnaceBlock.Index:
					componentMiner.ComponentPlayer.ComponentGui.ModalPanelWidget = new CovenWidget(componentMiner.Inventory, blockEntity.Entity.FindComponent<ComponentHearthFurnace>(true), "Widgets/HearthFurnaceWidget");
					break;
                case TankBlock.Index:
					if (TankBlock.GetType(raycastResult.Value) == TankBlock.Type.FractionatingTower)
					{ 
						componentMiner.ComponentPlayer.ComponentGui.ModalPanelWidget = new FractionalTowerWidget(componentMiner.Inventory, blockEntity.Entity.FindComponent<ComponentFractionalTower>(true));
						break;
					}
					return false;
				default: return false;
			}
			AudioManager.PlaySound("Audio/UI/ButtonClick", 1f, 0f, 0f);
			return true;
		}
	}
}
