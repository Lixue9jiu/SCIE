using Engine;
using System.Collections.Generic;
using TemplatesDatabase;

namespace Game
{
	public class SubsystemSqueezerBlockBehavior : SubsystemBlockBehavior
	{
		private readonly Dictionary<Point3, FireParticleSystem> m_particleSystemsByCell = new Dictionary<Point3, FireParticleSystem>();

		private SubsystemParticles m_subsystemParticles;

		public override int[] HandledBlocks
		{
			get
			{
				return new int[]
				{
					SqueezerBlock.Index
				};
			}
		}

		public override void OnBlockAdded(int value, int oldValue, int x, int y, int z)
		{
			var valuesDictionary = new ValuesDictionary();
			valuesDictionary.PopulateFromDatabaseObject(Project.GameDatabase.Database.FindDatabaseObject("Squeezer", Project.GameDatabase.EntityTemplateType, true));
			valuesDictionary.GetValue<ValuesDictionary>("BlockEntity").SetValue("Coordinates", new Point3(x, y, z));
			Project.AddEntity(Project.CreateEntity(valuesDictionary));
		}

		public override void OnBlockRemoved(int value, int newValue, int x, int y, int z)
		{
			ComponentBlockEntity blockEntity = Project.FindSubsystem<SubsystemBlockEntities>(true).GetBlockEntity(x, y, z);
			if (blockEntity != null)
			{
				var position = new Vector3((float)x, (float)y, (float)z) + new Vector3(0.5f);
				foreach (IInventory item in blockEntity.Entity.FindComponents<IInventory>())
				{
					item.DropAllItems(position);
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
			foreach (Point3 key in m_particleSystemsByCell.Keys)
			{
				if (key.X >= chunk.Origin.X && key.X < chunk.Origin.X + 16 && key.Z >= chunk.Origin.Y && key.Z < chunk.Origin.Y + 16)
				{
					list.Add(key);
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
			ComponentSqueezer componentFurnace = blockEntity.Entity.FindComponent<ComponentSqueezer>(true);
			componentMiner.ComponentPlayer.ComponentGui.ModalPanelWidget = new SqueezerWidget(componentMiner.Inventory, componentFurnace);
			AudioManager.PlaySound("Audio/UI/ButtonClick", 1f, 0f, 0f);
			return true;
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
	}
}
