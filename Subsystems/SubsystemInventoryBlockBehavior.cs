using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	public abstract class SubsystemInventoryBlockBehavior<T> : SubsystemCraftingTableBlockBehavior where T : Component
	{
		public readonly string Name;

		protected SubsystemInventoryBlockBehavior(string name)
		{
			Name = name;
		}
		public override void OnBlockAdded(int value, int oldValue, int x, int y, int z)
		{
			var valuesDictionary = new ValuesDictionary();
			valuesDictionary.PopulateFromDatabaseObject(Project.GameDatabase.Database.FindDatabaseObject(Name, Project.GameDatabase.EntityTemplateType, true));
			valuesDictionary.GetValue<ValuesDictionary>("BlockEntity").SetValue<Point3>("Coordinates", new Point3(x, y, z));
			Project.AddEntity(Project.CreateEntity(valuesDictionary));
		}
		public override bool OnInteract(TerrainRaycastResult raycastResult, ComponentMiner componentMiner)
		{
			ComponentBlockEntity blockEntity = Utils.SubsystemBlockEntities.GetBlockEntity(raycastResult.CellFace.X, raycastResult.CellFace.Y, raycastResult.CellFace.Z);
			if (blockEntity == null || componentMiner.ComponentPlayer == null)
			{
				return false;
			}
			componentMiner.ComponentPlayer.ComponentGui.ModalPanelWidget = GetWidget(componentMiner.Inventory, blockEntity.Entity.FindComponent<T>(true));
			AudioManager.PlaySound("Audio/UI/ButtonClick", 1f, 0f, 0f);
			return true;
		}
		public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			Utils.Load(Project);
		}
		public abstract Widget GetWidget(IInventory inventory, T component);
	}
}
