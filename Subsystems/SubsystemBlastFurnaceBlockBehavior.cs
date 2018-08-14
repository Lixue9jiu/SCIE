using System;
using System.Collections.Generic;
using Engine;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000615 RID: 1557
	public class SubsystemBlastFurnaceBlockBehavior : SubsystemBlockBehavior
	{
		// Token: 0x17000556 RID: 1366
		// (get) Token: 0x06002176 RID: 8566 RVA: 0x00015719 File Offset: 0x00013919
		public override int[] HandledBlocks
		{
			get
			{
				return new int[]
				{
					542
				};
			}
		}

		// Token: 0x06002177 RID: 8567 RVA: 0x000E1AB4 File Offset: 0x000DFCB4
		public override void OnBlockAdded(int value, int oldValue, int x, int y, int z)
		{
			DatabaseObject databaseObject = SubsystemTerrain.Project.GameDatabase.Database.FindDatabaseObject("BlastFurnace", SubsystemTerrain.Project.GameDatabase.EntityTemplateType, true);
			ValuesDictionary valuesDictionary = new ValuesDictionary();
			valuesDictionary.PopulateFromDatabaseObject(databaseObject);
			valuesDictionary.GetValue<ValuesDictionary>("BlockEntity").SetValue<Point3>("Coordinates", new Point3(x, y, z));
			SubsystemTerrain.Project.AddEntity(SubsystemTerrain.Project.CreateEntity(valuesDictionary));
		}

		// Token: 0x06002178 RID: 8568 RVA: 0x000DBAD4 File Offset: 0x000D9CD4
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

		// Token: 0x06002179 RID: 8569 RVA: 0x0000391B File Offset: 0x00001B1B
		public override void OnBlockGenerated(int value, int x, int y, int z, bool isLoaded)
		{
		}

		// Token: 0x0600217A RID: 8570 RVA: 0x000E1B44 File Offset: 0x000DFD44
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

		// Token: 0x0600217B RID: 8571 RVA: 0x000E1BF4 File Offset: 0x000DFDF4
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

		// Token: 0x0600217C RID: 8572 RVA: 0x0001064E File Offset: 0x0000E84E
		public override void OnNeighborBlockChanged(int x, int y, int z, int neighborX, int neighborY, int neighborZ)
		{
			base.OnNeighborBlockChanged(x, y, z, neighborX, neighborY, neighborZ);
		}

		// Token: 0x0600217D RID: 8573 RVA: 0x00015729 File Offset: 0x00013929
		public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			m_subsystemParticles = Project.FindSubsystem<SubsystemParticles>(true);
		}

		// Token: 0x04001989 RID: 6537
		private readonly Dictionary<Point3, FireParticleSystem> m_particleSystemsByCell = new Dictionary<Point3, FireParticleSystem>();

		// Token: 0x0400198A RID: 6538
		private SubsystemParticles m_subsystemParticles;
	}
}
