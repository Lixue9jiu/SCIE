using Engine;
using TemplatesDatabase;

namespace Game
{
	public class SubsystemDispenserNewBlockBehavior : SubsystemBlockBehavior
	{
		protected SubsystemAudio m_subsystemAudio;

		protected SubsystemBlockEntities SubsystemBlockEntities;

		protected SubsystemGameInfo m_subsystemGameInfo;

		public override int[] HandledBlocks
		{
			get
			{
				return new []
				{
					DispenserNewBlock.Index
				};
			}
		}

		public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			SubsystemBlockEntities = Project.FindSubsystem<SubsystemBlockEntities>(true);
			m_subsystemGameInfo = Project.FindSubsystem<SubsystemGameInfo>(true);
			m_subsystemAudio = Project.FindSubsystem<SubsystemAudio>(true);
		}

		public override void OnBlockAdded(int value, int oldValue, int x, int y, int z)
		{
			var valuesDictionary = new ValuesDictionary();
			valuesDictionary.PopulateFromDatabaseObject(Project.GameDatabase.Database.FindDatabaseObject("DispenserNew", Project.GameDatabase.EntityTemplateType, true));
			valuesDictionary.GetValue<ValuesDictionary>("BlockEntity").SetValue("Coordinates", new Point3(x, y, z));
			Project.AddEntity(Project.CreateEntity(valuesDictionary));
		}

		public override void OnBlockRemoved(int value, int newValue, int x, int y, int z)
		{
			ComponentBlockEntity blockEntity = SubsystemBlockEntities.GetBlockEntity(x, y, z);
			if (blockEntity != null)
			{
				var position = new Vector3((float)x, (float)y, (float)z) + new Vector3(0.5f);
				for (var i = blockEntity.Entity.FindComponents<IInventory>().GetEnumerator(); i.MoveNext();)
				{
					i.Current.DropAllItems(position);
				}
				Project.RemoveEntity(blockEntity.Entity, true);
			}
		}

		public override bool OnInteract(TerrainRaycastResult raycastResult, ComponentMiner componentMiner)
		{
			if (m_subsystemGameInfo.WorldSettings.GameMode != GameMode.Adventure)
			{
				ComponentBlockEntity blockEntity = SubsystemBlockEntities.GetBlockEntity(raycastResult.CellFace.X, raycastResult.CellFace.Y, raycastResult.CellFace.Z);
				if (blockEntity != null && componentMiner.ComponentPlayer != null)
				{
					ComponentDispenserNew componentDispenser = blockEntity.Entity.FindComponent<ComponentDispenserNew>(true);
					componentMiner.ComponentPlayer.ComponentGui.ModalPanelWidget = new DispenserNewWidget(componentMiner.Inventory, componentDispenser);
					AudioManager.PlaySound("Audio/UI/ButtonClick", 1f, 0f, 0f);
					return true;
				}
			}
			return false;
		}

		public override void OnHitByProjectile(CellFace cellFace, WorldItem worldItem)
		{
			if (!worldItem.ToRemove)
			{
				ComponentBlockEntity blockEntity = SubsystemBlockEntities.GetBlockEntity(cellFace.X, cellFace.Y, cellFace.Z);
				if (blockEntity != null && DispenserNewBlock.GetAcceptsDrops(Terrain.ExtractData(SubsystemTerrain.Terrain.GetCellValue(cellFace.X, cellFace.Y, cellFace.Z))))
				{
					ComponentDispenserNew inventory = blockEntity.Entity.FindComponent<ComponentDispenserNew>(true);
					var pickable = worldItem as Pickable;
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
