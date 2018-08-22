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
				return new int[]
				{
					BlastFurnaceBlock.Index
				};
			}
		}
		
		public override void OnBlockAdded(int value, int oldValue, int x, int y, int z)
		{
			DatabaseObject databaseObject = SubsystemTerrain.Project.GameDatabase.Database.FindDatabaseObject("BlastFurnace", SubsystemTerrain.Project.GameDatabase.EntityTemplateType, true);
			ValuesDictionary valuesDictionary = new ValuesDictionary();
			valuesDictionary.PopulateFromDatabaseObject(databaseObject);
			valuesDictionary.GetValue<ValuesDictionary>("BlockEntity").SetValue<Point3>("Coordinates", new Point3(x, y, z));
			SubsystemTerrain.Project.AddEntity(SubsystemTerrain.Project.CreateEntity(valuesDictionary));
		}
		
		public override void OnBlockRemoved(int value, int newValue, int x, int y, int z)
		{
			ComponentBlockEntity blockEntity = SubsystemTerrain.Project.FindSubsystem<SubsystemBlockEntities>(true).GetBlockEntity(x, y, z);
			if (blockEntity != null)
			{
				Vector3 position = new Vector3((float)x, (float)y, (float)z) + new Vector3(0.5f);
				foreach (IInventory inventory in blockEntity.Entity.FindComponents<IInventory>())
				{
					inventory.DropAllItems(position);
				}
				SubsystemTerrain.Project.RemoveEntity(blockEntity.Entity, true);
			}
		}
		
		public override void OnBlockGenerated(int value, int x, int y, int z, bool isLoaded)
		{
		}
		
		public override void OnChunkDiscarding(TerrainChunk chunk)
		{
			List<Point3> list = new List<Point3>();
			foreach (Point3 point in m_particleSystemsByCell.Keys)
			{
				if (point.X >= chunk.Origin.X && point.X < chunk.Origin.X + 16 && point.Z >= chunk.Origin.Y && point.Z < chunk.Origin.Y + 16)
				{
					list.Add(point);
				}
			}
		}
		
		public override bool OnInteract(TerrainRaycastResult raycastResult, ComponentMiner componentMiner)
		{
			ComponentBlockEntity blockEntity = SubsystemTerrain.Project.FindSubsystem<SubsystemBlockEntities>(true).GetBlockEntity(raycastResult.CellFace.X, raycastResult.CellFace.Y, raycastResult.CellFace.Z);
			if (blockEntity == null || componentMiner.ComponentPlayer == null)
			{
				return false;
			}
			ComponentBlastFurnace componentFurnace = blockEntity.Entity.FindComponent<ComponentBlastFurnace>(true);
			componentMiner.ComponentPlayer.ComponentGui.ModalPanelWidget = new BlastFurnaceWidget(componentMiner.Inventory, componentFurnace);
			AudioManager.PlaySound("Audio/UI/ButtonClick", 1f, 0f, 0f);
			return true;
		}
		
		public override void OnNeighborBlockChanged(int x, int y, int z, int neighborX, int neighborY, int neighborZ)
		{
			base.OnNeighborBlockChanged(x, y, z, neighborX, neighborY, neighborZ);
		}
		
		public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			m_subsystemParticles = Project.FindSubsystem<SubsystemParticles>(true);
		}
		
		private readonly Dictionary<Point3, FireParticleSystem> m_particleSystemsByCell = new Dictionary<Point3, FireParticleSystem>();
		
		private SubsystemParticles m_subsystemParticles;
	}
}
