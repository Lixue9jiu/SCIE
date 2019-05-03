using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	public abstract class SubsystemInventoryBlockBehavior<T> : SubsystemCraftingTableBlockBehavior where T : Component
	{
		public string Name;

		protected SubsystemInventoryBlockBehavior(string name) { Name = name; }

		public override void OnBlockAdded(int value, int oldValue, int x, int y, int z)
		{
			var vd = new ValuesDictionary();
			vd.PopulateFromDatabaseObject(Project.GameDatabase.Database.FindDatabaseObject(Name, Project.GameDatabase.EntityTemplateType, true));
			vd.GetValue<ValuesDictionary>("BlockEntity").SetValue("Coordinates", new Point3(x, y, z));
			Project.AddEntity(Project.CreateEntity(vd));
		}

		public override bool OnInteract(TerrainRaycastResult raycastResult, ComponentMiner componentMiner)
		{
			var entity = Utils.GetBlockEntity(raycastResult.CellFace.Point);
			if (entity == null || componentMiner.ComponentPlayer == null)
				return false;
			componentMiner.ComponentPlayer.ComponentGui.ModalPanelWidget = GetWidget(componentMiner.Inventory, entity.Entity.FindComponent<T>(true));
			AudioManager.PlaySound("Audio/UI/ButtonClick", 1f, 0f, 0f);
			return true;
		}

		public override void OnHitByProjectile(CellFace cellFace, WorldItem worldItem) => Utils.OnHitByProjectile(cellFace, worldItem);

		public abstract Widget GetWidget(IInventory inventory, T component);
	}

	public partial class Utils
	{
		public static void OnHitByProjectile(CellFace cellFace, WorldItem worldItem)
		{
			if (worldItem.ToRemove)
				return;
			var blockEntity = GetBlockEntity(cellFace.Point);
			if (blockEntity == null)
				return;
			var inventory = blockEntity.Entity.FindComponent<ComponentInventoryBase>(true);
			var pickable = worldItem as Pickable;
			int count = (pickable == null) ? 1 : pickable.Count;
			int value = worldItem.Value;
			int max = ComponentInventoryBase.AcquireItems(inventory, value, count);
			if (max < count)
				SubsystemAudio.PlaySound("Audio/PickableCollected", 1f, 0f, worldItem.Position, 3f, true);
			if (max <= 0)
				worldItem.ToRemove = true;
			else if (pickable != null)
				pickable.Count = max;
		}
	}
}