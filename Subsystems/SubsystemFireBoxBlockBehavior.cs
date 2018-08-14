using System;
using System.Collections.Generic;
using Engine;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x0200061B RID: 1563
	public class SubsystemFireBoxBlockBehavior : SubsystemBlockBehavior
	{
		// Token: 0x1700055D RID: 1373
		// (get) Token: 0x0600219E RID: 8606 RVA: 0x000158A1 File Offset: 0x00013AA1
		public override int[] HandledBlocks
		{
			get
			{
				return new int[]
				{
					543,
					544
				};
			}
		}

		// Token: 0x0600219F RID: 8607 RVA: 0x000E2518 File Offset: 0x000E0718
		public override void OnBlockAdded(int value, int oldValue, int x, int y, int z)
		{
			if (Terrain.ExtractContents(oldValue) != 543 && Terrain.ExtractContents(oldValue) != 544)
			{
				DatabaseObject databaseObject = base.SubsystemTerrain.Project.GameDatabase.Database.FindDatabaseObject("FireBox", base.SubsystemTerrain.Project.GameDatabase.EntityTemplateType, true);
				ValuesDictionary valuesDictionary = new ValuesDictionary();
				valuesDictionary.PopulateFromDatabaseObject(databaseObject);
				valuesDictionary.GetValue<ValuesDictionary>("BlockEntity").SetValue<Point3>("Coordinates", new Point3(x, y, z));
				base.SubsystemTerrain.Project.AddEntity(base.SubsystemTerrain.Project.CreateEntity(valuesDictionary));
			}
			if (Terrain.ExtractContents(value) != 544)
			{
				return;
			}
			this.AddFire(value, x, y, z);
		}

		// Token: 0x060021A0 RID: 8608 RVA: 0x000E25E4 File Offset: 0x000E07E4
		public override void OnBlockRemoved(int value, int newValue, int x, int y, int z)
		{
			if (Terrain.ExtractContents(newValue) != 543 && Terrain.ExtractContents(newValue) != 544)
			{
				ComponentBlockEntity blockEntity = base.SubsystemTerrain.Project.FindSubsystem<SubsystemBlockEntities>(true).GetBlockEntity(x, y, z);
				if (blockEntity != null)
				{
					Vector3 position = new Vector3((float)x, (float)y, (float)z) + new Vector3(0.5f);
					foreach (IInventory inventory in blockEntity.Entity.FindComponents<IInventory>())
					{
						inventory.DropAllItems(position);
					}
					base.SubsystemTerrain.Project.RemoveEntity(blockEntity.Entity, true);
				}
			}
			if (Terrain.ExtractContents(value) != 544)
			{
				return;
			}
			this.RemoveFire(x, y, z);
		}

		// Token: 0x060021A1 RID: 8609 RVA: 0x000158B9 File Offset: 0x00013AB9
		public override void OnBlockGenerated(int value, int x, int y, int z, bool isLoaded)
		{
			if (Terrain.ExtractContents(value) != 544)
			{
				return;
			}
			this.AddFire(value, x, y, z);
		}

		// Token: 0x060021A2 RID: 8610 RVA: 0x000E26CC File Offset: 0x000E08CC
		public override void OnChunkDiscarding(TerrainChunk chunk)
		{
			List<Point3> list = new List<Point3>();
			foreach (Point3 point in this.m_particleSystemsByCell.Keys)
			{
				if (point.X >= chunk.Origin.X && point.X < chunk.Origin.X + 16 && point.Z >= chunk.Origin.Y && point.Z < chunk.Origin.Y + 16)
				{
					list.Add(point);
				}
			}
			foreach (Point3 point2 in list)
			{
				this.RemoveFire(point2.X, point2.Y, point2.Z);
			}
		}

		// Token: 0x060021A3 RID: 8611 RVA: 0x000E27D0 File Offset: 0x000E09D0
		public override bool OnInteract(TerrainRaycastResult raycastResult, ComponentMiner componentMiner)
		{
			ComponentBlockEntity blockEntity = base.SubsystemTerrain.Project.FindSubsystem<SubsystemBlockEntities>(true).GetBlockEntity(raycastResult.CellFace.X, raycastResult.CellFace.Y, raycastResult.CellFace.Z);
			if (blockEntity == null || componentMiner.ComponentPlayer == null)
			{
				return false;
			}
			ComponentFireBox componentFurnace = blockEntity.Entity.FindComponent<ComponentFireBox>(true);
			componentMiner.ComponentPlayer.ComponentGui.ModalPanelWidget = new FireBoxWidget(componentMiner.Inventory, componentFurnace);
			AudioManager.PlaySound("Audio/UI/ButtonClick", 1f, 0f, 0f);
			return true;
		}

		// Token: 0x060021A4 RID: 8612 RVA: 0x0001064E File Offset: 0x0000E84E
		public override void OnNeighborBlockChanged(int x, int y, int z, int neighborX, int neighborY, int neighborZ)
		{
			base.OnNeighborBlockChanged(x, y, z, neighborX, neighborY, neighborZ);
		}

		// Token: 0x060021A5 RID: 8613 RVA: 0x000158D4 File Offset: 0x00013AD4
		public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			this.m_subsystemParticles = base.Project.FindSubsystem<SubsystemParticles>(true);
		}

		// Token: 0x060021A6 RID: 8614 RVA: 0x000E2868 File Offset: 0x000E0A68
		private void AddFire(int value, int x, int y, int z)
		{
			Vector3 v = new Vector3(0.5f, 0.2f, 0.5f);
			float size = 0.15f;
			FireParticleSystem fireParticleSystem = new FireParticleSystem(new Vector3((float)x, (float)y, (float)z) + v, size, 16f);
			this.m_subsystemParticles.AddParticleSystem(fireParticleSystem);
			this.m_particleSystemsByCell[new Point3(x, y, z)] = fireParticleSystem;
		}

		// Token: 0x060021A7 RID: 8615 RVA: 0x000E28D0 File Offset: 0x000E0AD0
		private void RemoveFire(int x, int y, int z)
		{
			Point3 key = new Point3(x, y, z);
			this.m_subsystemParticles.RemoveParticleSystem(this.m_particleSystemsByCell[key]);
			this.m_particleSystemsByCell.Remove(key);
		}

		// Token: 0x040019AB RID: 6571
		private readonly Dictionary<Point3, FireParticleSystem> m_particleSystemsByCell = new Dictionary<Point3, FireParticleSystem>();

		// Token: 0x040019AC RID: 6572
		private SubsystemParticles m_subsystemParticles;
	}
}
