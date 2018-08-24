using Engine;
using System.Collections.Generic;
using TemplatesDatabase;

namespace Game
{
	public class SubsystemEngineHBlockBehavior : SubsystemBlockBehavior
	{
		private readonly Dictionary<Point3, FireParticleSystem> m_particleSystemsByCell = new Dictionary<Point3, FireParticleSystem>();

		private SubsystemParticles m_subsystemParticles;

		public override int[] HandledBlocks
		{
			get
			{
				return new int[]
				{
					LitEngineHBlock.Index,
					EngineHBlock.Index
				};
			}
		}

		public override void OnBlockAdded(int value, int oldValue, int x, int y, int z)
		{
			if (Terrain.ExtractContents(oldValue) != EngineHBlock.Index && Terrain.ExtractContents(oldValue) != LitEngineHBlock.Index)
			{
				var valuesDictionary = new ValuesDictionary();
				valuesDictionary.PopulateFromDatabaseObject(Project.GameDatabase.Database.FindDatabaseObject("HeatEngine", Project.GameDatabase.EntityTemplateType, true));
				valuesDictionary.GetValue<ValuesDictionary>("BlockEntity").SetValue("Coordinates", new Point3(x, y, z));
				Project.AddEntity(Project.CreateEntity(valuesDictionary));
			}
			if (Terrain.ExtractContents(value) == LitEngineHBlock.Index)
			{
				AddFire(value, x, y, z);
			}
		}

		public override void OnBlockRemoved(int value, int newValue, int x, int y, int z)
		{
			if (Terrain.ExtractContents(newValue) != EngineHBlock.Index && Terrain.ExtractContents(newValue) != LitEngineHBlock.Index)
			{
				ComponentBlockEntity blockEntity = Project.FindSubsystem<SubsystemBlockEntities>(true).GetBlockEntity(x, y, z);
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
			if (Terrain.ExtractContents(value) == LitEngineHBlock.Index)
			{
				RemoveFire(x, y, z);
			}
		}

		public override void OnBlockGenerated(int value, int x, int y, int z, bool isLoaded)
		{
			if (Terrain.ExtractContents(value) == LitEngineHBlock.Index)
			{
				AddFire(value, x, y, z);
			}
		}

		public override void OnChunkDiscarding(TerrainChunk chunk)
		{
			var list = new List<Point3>();
			foreach (Point3 key in m_particleSystemsByCell.Keys)
			{
				if (key.X >= chunk.Origin.X && key.X < chunk.Origin.X + 16 && key.Z >= chunk.Origin.Y && key.Z < chunk.Origin.Y + 16)
				{
					list.Add(key);
				}
			}
			foreach (var item in list)
			{
				RemoveFire(item.X, item.Y, item.Z);
			}
		}

		public override bool OnInteract(TerrainRaycastResult raycastResult, ComponentMiner componentMiner)
		{
			ComponentBlockEntity blockEntity = Project.FindSubsystem<SubsystemBlockEntities>(true).GetBlockEntity(raycastResult.CellFace.X, raycastResult.CellFace.Y, raycastResult.CellFace.Z);
			if (blockEntity == null || componentMiner.ComponentPlayer == null)
			{
				return false;
			}
			ComponentEngineH componentFurnace = blockEntity.Entity.FindComponent<ComponentEngineH>(true);
			componentMiner.ComponentPlayer.ComponentGui.ModalPanelWidget = new EngineHWidget(componentMiner.Inventory, componentFurnace);
			AudioManager.PlaySound("Audio/UI/ButtonClick", 1f, 0f, 0f);
			return true;
		}

		public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			m_subsystemParticles = Project.FindSubsystem<SubsystemParticles>(true);
		}

		private void AddFire(int value, int x, int y, int z)
		{
			var v = new Vector3(0.5f, 0.2f, 0.5f);
			const float size = 0.15f;
			var fireParticleSystem = new FireParticleSystem(new Vector3((float)x, (float)y, (float)z) + v, size, 16f);
			m_subsystemParticles.AddParticleSystem(fireParticleSystem);
			m_particleSystemsByCell[new Point3(x, y, z)] = fireParticleSystem;
		}

		private void RemoveFire(int x, int y, int z)
		{
			var key = new Point3(x, y, z);
			m_subsystemParticles.RemoveParticleSystem(m_particleSystemsByCell[key]);
			m_particleSystemsByCell.Remove(key);
		}
	}
}
