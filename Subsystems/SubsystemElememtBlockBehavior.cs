using Engine;
using TemplatesDatabase;

namespace Game
{
	public class SubsystemElememtBlockBehavior : SubsystemBlockBehavior
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
              //  case 3: name = "Seperator"; break;
              //  case 2: name = "Magnetizer"; break;
            //    case 6: name = "ElectricFurnace"; break;
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
				case 1:
              //  case 2:return;
            //    case 3:
           //     case 6: break;
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
              
               
            }
			return false;
        }
	}
}
