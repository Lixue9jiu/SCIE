using System.Collections.Generic;
using Engine;
using TemplatesDatabase;

namespace Game
{
	public class SubsystemBlastFurnaceBlockBehavior : SubsystemBlockBehavior
	{
		public override int[] HandledBlocks
		{
			get
			{
				return new []
				{
					BlastFurnaceBlock.Index,
                    CovenBlock.Index,
                    HearthFurnaceBlock.Index
				};
			}
		}
		
		public override void OnBlockAdded(int value, int oldValue, int x, int y, int z)
		{
            string name;
            switch (Terrain.ExtractContents(value))
            {
                case BlastFurnaceBlock.Index: name = "BlastFurnace"; break;
                case CovenBlock.Index: name = "CokeOven"; break;
                case HearthFurnaceBlock.Index: name = "HearthFurnace"; break;
                default: return;
            }
            var valuesDictionary = new ValuesDictionary();
			valuesDictionary.PopulateFromDatabaseObject(Project.GameDatabase.Database.FindDatabaseObject(name, Project.GameDatabase.EntityTemplateType, true));
			valuesDictionary.GetValue<ValuesDictionary>("BlockEntity").SetValue<Point3>("Coordinates", new Point3(x, y, z));
			Project.AddEntity(Project.CreateEntity(valuesDictionary));
		}
		
		public override void OnBlockRemoved(int value, int newValue, int x, int y, int z)
		{
			ComponentBlockEntity blockEntity = Project.FindSubsystem<SubsystemBlockEntities>(true).GetBlockEntity(x, y, z);
			if (blockEntity != null)
			{
				Vector3 position = new Vector3((float)x, (float)y, (float)z) + new Vector3(0.5f);
				for (var i = blockEntity.Entity.FindComponents<IInventory>().GetEnumerator(); i.MoveNext();)
				{
					i.Current.DropAllItems(position);
				}
				Project.RemoveEntity(blockEntity.Entity, true);
			}
		}
		
		/*public override void OnBlockGenerated(int value, int x, int y, int z, bool isLoaded)
		{
		}
		
		public override void OnChunkDiscarding(TerrainChunk chunk)
		{
			var list = new List<Point3>();
			foreach (Point3 point in m_particleSystemsByCell.Keys)
			{
				if (point.X >= chunk.Origin.X && point.X < chunk.Origin.X + 16 && point.Z >= chunk.Origin.Y && point.Z < chunk.Origin.Y + 16)
				{
					list.Add(point);
				}
			}
		}*/
		
		public override bool OnInteract(TerrainRaycastResult raycastResult, ComponentMiner componentMiner)
		{
			ComponentBlockEntity blockEntity = Project.FindSubsystem<SubsystemBlockEntities>(true).GetBlockEntity(raycastResult.CellFace.X, raycastResult.CellFace.Y, raycastResult.CellFace.Z);
			if (blockEntity == null || componentMiner.ComponentPlayer == null)
			{
				return false;
			}
            switch (Terrain.ExtractContents(raycastResult.Value))
            {
                case BlastFurnaceBlock.Index:
                    ComponentBlastFurnace componentFurnace = blockEntity.Entity.FindComponent<ComponentBlastFurnace>(true);
                    componentMiner.ComponentPlayer.ComponentGui.ModalPanelWidget = new BlastFurnaceWidget(componentMiner.Inventory, componentFurnace);
                    AudioManager.PlaySound("Audio/UI/ButtonClick", 1f, 0f, 0f);
                    return true;
                   
                case CovenBlock.Index:
                    ComponentCoven componentFurnace2 = blockEntity.Entity.FindComponent<ComponentCoven>(true);
                    componentMiner.ComponentPlayer.ComponentGui.ModalPanelWidget = new CovenWidget(componentMiner.Inventory, componentFurnace2);
                    AudioManager.PlaySound("Audio/UI/ButtonClick", 1f, 0f, 0f);
                    return true;

                case HearthFurnaceBlock.Index:
                    ComponentHearthFurnace componentFurnace3 = blockEntity.Entity.FindComponent<ComponentHearthFurnace>(true);
                    componentMiner.ComponentPlayer.ComponentGui.ModalPanelWidget = new HearthFurnaceWidget(componentMiner.Inventory, componentFurnace3);
                    AudioManager.PlaySound("Audio/UI/ButtonClick", 1f, 0f, 0f);
                    return true;
                default: return false;
            }
		}
		
		/*public override void OnNeighborBlockChanged(int x, int y, int z, int neighborX, int neighborY, int neighborZ)
		{
			base.OnNeighborBlockChanged(x, y, z, neighborX, neighborY, neighborZ);
		}*/
		
		public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			m_subsystemParticles = Project.FindSubsystem<SubsystemParticles>(true);
		}
		
		protected readonly Dictionary<Point3, FireParticleSystem> m_particleSystemsByCell = new Dictionary<Point3, FireParticleSystem>();
		
		protected SubsystemParticles m_subsystemParticles;
	}
}
