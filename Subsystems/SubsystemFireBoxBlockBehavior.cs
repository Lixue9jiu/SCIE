using System.Collections.Generic;
using Engine;
using TemplatesDatabase;

namespace Game
{
	public class SubsystemFireBoxBlockBehavior : SubsystemBlockBehavior
	{
		public override int[] HandledBlocks
		{
			get
			{
				return new int[]
				{
					FireBoxBlock.Index,
					LitFireBoxBlock.Index
				};
			}
		}
		
		public override void OnBlockAdded(int value, int oldValue, int x, int y, int z)
		{
			if (Terrain.ExtractContents(oldValue) != FireBoxBlock.Index && Terrain.ExtractContents(oldValue) != LitFireBoxBlock.Index)
			{
				DatabaseObject databaseObject = Project.GameDatabase.Database.FindDatabaseObject("FireBox", Project.GameDatabase.EntityTemplateType, true);
				ValuesDictionary valuesDictionary = new ValuesDictionary();
				valuesDictionary.PopulateFromDatabaseObject(databaseObject);
				valuesDictionary.GetValue<ValuesDictionary>("BlockEntity").SetValue<Point3>("Coordinates", new Point3(x, y, z));
				Project.AddEntity(Project.CreateEntity(valuesDictionary));
			}
			if (Terrain.ExtractContents(value) != LitFireBoxBlock.Index)
			{
				return;
			}
			AddFire(value, x, y, z);
		}
		
		public override void OnBlockRemoved(int value, int newValue, int x, int y, int z)
		{
			if (Terrain.ExtractContents(newValue) != FireBoxBlock.Index && Terrain.ExtractContents(newValue) != LitFireBoxBlock.Index)
			{
				ComponentBlockEntity blockEntity = Project.FindSubsystem<SubsystemBlockEntities>(true).GetBlockEntity(x, y, z);
				if (blockEntity != null)
				{
					Vector3 position = new Vector3((float)x, (float)y, (float)z) + new Vector3(0.5f);
					foreach (IInventory inventory in blockEntity.Entity.FindComponents<IInventory>())
					{
						inventory.DropAllItems(position);
					}
					Project.RemoveEntity(blockEntity.Entity, true);
				}
			}
			if (Terrain.ExtractContents(value) != LitFireBoxBlock.Index)
			{
				return;
			}
			RemoveFire(x, y, z);
		}
		
		public override void OnBlockGenerated(int value, int x, int y, int z, bool isLoaded)
		{
			if (Terrain.ExtractContents(value) != LitFireBoxBlock.Index)
			{
				return;
			}
			AddFire(value, x, y, z);
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
			foreach (Point3 point2 in list)
			{
				RemoveFire(point2.X, point2.Y, point2.Z);
			}
		}
		
		public override bool OnInteract(TerrainRaycastResult raycastResult, ComponentMiner componentMiner)
		{
			ComponentBlockEntity blockEntity = Project.FindSubsystem<SubsystemBlockEntities>(true).GetBlockEntity(raycastResult.CellFace.X, raycastResult.CellFace.Y, raycastResult.CellFace.Z);
			if (blockEntity == null || componentMiner.ComponentPlayer == null)
			{
				return false;
			}
			ComponentFireBox componentFurnace = blockEntity.Entity.FindComponent<ComponentFireBox>(true);
			componentMiner.ComponentPlayer.ComponentGui.ModalPanelWidget = new FireBoxWidget(componentMiner.Inventory, componentFurnace);
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

		private void AddFire(int value, int x, int y, int z)
		{
			Vector3 v = new Vector3(0.5f, 0.2f, 0.5f);
			float size = 0.15f;
			FireParticleSystem fireParticleSystem = new FireParticleSystem(new Vector3((float)x, (float)y, (float)z) + v, size, 16f);
			m_subsystemParticles.AddParticleSystem(fireParticleSystem);
			m_particleSystemsByCell[new Point3(x, y, z)] = fireParticleSystem;
		}

		private void RemoveFire(int x, int y, int z)
		{
			Point3 key = new Point3(x, y, z);
			m_subsystemParticles.RemoveParticleSystem(m_particleSystemsByCell[key]);
			m_particleSystemsByCell.Remove(key);
		}

		private readonly Dictionary<Point3, FireParticleSystem> m_particleSystemsByCell = new Dictionary<Point3, FireParticleSystem>();

		private SubsystemParticles m_subsystemParticles;
	}
}
