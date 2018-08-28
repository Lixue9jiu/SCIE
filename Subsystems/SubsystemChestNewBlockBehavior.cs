using Engine;
using TemplatesDatabase;

namespace Game
{
	public class SubsystemChestNewBlockBehavior : SubsystemBlockBehavior
	{
		protected SubsystemAudio m_subsystemAudio;

		protected SubsystemBlockEntities m_subsystemBlockEntities;

		public override int[] HandledBlocks
		{
			get
			{
				return new []
				{
					ElementBlock.Index
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
			string name;
			switch (Terrain.ExtractData(value) & 32767)
			{
                case 3: name = "Seperator"; break;
                case 6: name = "ElectricFurnace"; break;
                default: return;
			}
			var valuesDictionary = new ValuesDictionary();
			valuesDictionary.PopulateFromDatabaseObject(Project.GameDatabase.Database.FindDatabaseObject(name, Project.GameDatabase.EntityTemplateType, true));
			valuesDictionary.GetValue<ValuesDictionary>("BlockEntity").SetValue("Coordinates", new Point3(x, y, z));
			Project.AddEntity(Project.CreateEntity(valuesDictionary));
		}
		/*public override void OnBlockGenerated(int value, int x, int y, int z, bool isLoaded)
		{
			base.OnBlockGenerated(value, x, y, z, isLoaded);
		}*/

		public override void OnBlockRemoved(int value, int newValue, int x, int y, int z)
		{
			switch (Terrain.ExtractData(value) & 32767)
			{
				case 0:
				case 1: return;
				case 3:
                case 6: break;
				default: return;
			}
			ComponentBlockEntity blockEntity = m_subsystemBlockEntities.GetBlockEntity(x, y, z);
			if (blockEntity != null)
			{
				Vector3 position = new Vector3(x, y, z) + new Vector3(0.5f);
				foreach (IInventory item in blockEntity.Entity.FindComponents<IInventory>())
				{
					item.DropAllItems(position);
				}
				Project.RemoveEntity(blockEntity.Entity, true);
			}
		}

		public override bool OnInteract(TerrainRaycastResult raycastResult, ComponentMiner componentMiner)
		{
			switch (Terrain.ExtractData(raycastResult.Value) & 32767)
			{
				case 3:
					{
						ComponentBlockEntity blockEntity = m_subsystemBlockEntities.GetBlockEntity(raycastResult.CellFace.X, raycastResult.CellFace.Y, raycastResult.CellFace.Z);
						if (blockEntity == null || componentMiner.ComponentPlayer == null)
						{
							return false;
						}
						ComponentSeperator componentChestNew = blockEntity.Entity.FindComponent<ComponentSeperator>(true);
						componentMiner.ComponentPlayer.ComponentGui.ModalPanelWidget = new SeperatorWidget(componentMiner.Inventory, componentChestNew);
						AudioManager.PlaySound("Audio/UI/ButtonClick", 1f, 0f, 0f);
						return true;
					}
                case 6:
                    {
                        ComponentBlockEntity blockEntity = m_subsystemBlockEntities.GetBlockEntity(raycastResult.CellFace.X, raycastResult.CellFace.Y, raycastResult.CellFace.Z);
                       
                        if (blockEntity == null || componentMiner.ComponentPlayer == null)
                        {
                            return false;
                        }
                       
                        ComponentElectricFurnace componentChestNew = blockEntity.Entity.FindComponent<ComponentElectricFurnace>(true);
                        componentMiner.ComponentPlayer.ComponentGui.ModalPanelWidget = new ElectricFurnaceWidget(componentMiner.Inventory, componentChestNew);
                        AudioManager.PlaySound("Audio/UI/ButtonClick", 1f, 0f, 0f);
                        return true;
                    }
            }
			return false;
        }

		public override void OnHitByProjectile(CellFace cellFace, WorldItem worldItem)
		{
			if ((Terrain.ExtractData(SubsystemTerrain.Terrain.GetCellValue(cellFace.X, cellFace.Y, cellFace.Z)) & 32767) != 1)
			{
				return;
			}
            if (!worldItem.ToRemove)
			{
				ComponentBlockEntity blockEntity = m_subsystemBlockEntities.GetBlockEntity(cellFace.X, cellFace.Y, cellFace.Z);
				if (blockEntity != null)
				{
					ComponentChestNew inventory = blockEntity.Entity.FindComponent<ComponentChestNew>(true);
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
