using Engine;
using TemplatesDatabase;

namespace Game
{
	public class SubsystemChestNewBlockBehavior : SubsystemBlockBehavior
	{
		private SubsystemAudio m_subsystemAudio;

		private SubsystemBlockEntities m_subsystemBlockEntities;

		public override int[] HandledBlocks
		{
			get
			{
				return new int[1]
				{
					601
				};
			}
		}

		public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			m_subsystemBlockEntities = Project.FindSubsystem<SubsystemBlockEntities>(true);
			m_subsystemAudio = Project.FindSubsystem<SubsystemAudio>(true);
		}

		public override void OnBlockAdded(int value, int oldValue, int x, int y, int z)
		{
			DatabaseObject databaseObject = Project.GameDatabase.Database.FindDatabaseObject("ChestNew", Project.GameDatabase.EntityTemplateType, true);
			ValuesDictionary valuesDictionary = new ValuesDictionary();
			valuesDictionary.PopulateFromDatabaseObject(databaseObject);
			valuesDictionary.GetValue<ValuesDictionary>("BlockEntity").SetValue("Coordinates", new Point3(x, y, z));
			Project.AddEntity(Project.CreateEntity(valuesDictionary));
		}

		public override void OnBlockRemoved(int value, int newValue, int x, int y, int z)
		{
			ComponentBlockEntity blockEntity = m_subsystemBlockEntities.GetBlockEntity(x, y, z);
			if (blockEntity != null)
			{
				Vector3 position = new Vector3((float)x, (float)y, (float)z) + new Vector3(0.5f);
				foreach (IInventory item in blockEntity.Entity.FindComponents<IInventory>())
				{
					item.DropAllItems(position);
				}
				Project.RemoveEntity(blockEntity.Entity, true);
			}
		}

		public override bool OnInteract(TerrainRaycastResult raycastResult, ComponentMiner componentMiner)
		{
			ComponentBlockEntity blockEntity = m_subsystemBlockEntities.GetBlockEntity(raycastResult.CellFace.X, raycastResult.CellFace.Y, raycastResult.CellFace.Z);
			if (blockEntity == null || componentMiner.ComponentPlayer == null)
			{
				return false;
			}
			ComponentChestNew componentChestNew = blockEntity.Entity.FindComponent<ComponentChestNew>(true);
			componentMiner.ComponentPlayer.ComponentGui.ModalPanelWidget = new ChestNewWidget(componentMiner.Inventory, componentChestNew);
			AudioManager.PlaySound("Audio/UI/ButtonClick", 1f, 0f, 0f);
			return true;
		}

		public override void OnHitByProjectile(CellFace cellFace, WorldItem worldItem)
		{
			if (!worldItem.ToRemove)
			{
				ComponentBlockEntity blockEntity = m_subsystemBlockEntities.GetBlockEntity(cellFace.X, cellFace.Y, cellFace.Z);
				if (blockEntity != null)
				{
					ComponentChestNew inventory = blockEntity.Entity.FindComponent<ComponentChestNew>(true);
					Pickable pickable = worldItem as Pickable;
					int num = (pickable == null) ? 1 : pickable.Count;
					int value = worldItem.Value;
					int count = num;
					int num2 = ComponentInventoryBase.AcquireItems(inventory, value, count);
					if (num2 < num)
					{
						m_subsystemAudio.PlaySound("Audio/PickableCollected", 1f, 0f, worldItem.Position, 3f, true);
					}
					if (num2 <= 0)
					{
						worldItem.ToRemove = true;
					}
					else if (pickable != null)
					{
						pickable.Count = num2;
					}
				}
			}
		}
	}
}
