using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000618 RID: 1560
	public class ComponentFireBox : ComponentInventoryBase, IUpdateable
	{
		// Token: 0x17000559 RID: 1369
		// (get) Token: 0x0600218D RID: 8589 RVA: 0x00005DEC File Offset: 0x00003FEC
		public int FuelSlotIndex
		{
			get
			{
				return this.SlotsCount - 1;
			}
		}

		// Token: 0x1700055A RID: 1370
		// (get) Token: 0x0600218E RID: 8590 RVA: 0x000157D8 File Offset: 0x000139D8
		// (set) Token: 0x0600218F RID: 8591 RVA: 0x000157E0 File Offset: 0x000139E0
		public float HeatLevel { get; private set; }

		// Token: 0x1700055B RID: 1371
		// (get) Token: 0x06002190 RID: 8592 RVA: 0x000157E9 File Offset: 0x000139E9
		// (set) Token: 0x06002191 RID: 8593 RVA: 0x000157F1 File Offset: 0x000139F1
		public float SmeltingProgress { get; private set; }

		// Token: 0x1700055C RID: 1372
		// (get) Token: 0x06002192 RID: 8594 RVA: 0x000034CC File Offset: 0x000016CC
		public int UpdateOrder
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x06002193 RID: 8595 RVA: 0x000E1F90 File Offset: 0x000E0190
		public void Update(float dt)
		{
			Point3 coordinates = this.m_componentBlockEntity.Coordinates;
			if ((double)this.HeatLevel > 0.0)
			{
				this.m_fireTimeRemaining = MathUtils.Max(0f, this.m_fireTimeRemaining - dt);
				if ((double)this.m_fireTimeRemaining == 0.0)
				{
					this.HeatLevel = 0f;
				}
			}
			if (this.m_updateSmeltingRecipe)
			{
				this.m_updateSmeltingRecipe = false;
				if ((double)this.HeatLevel > 0.0)
				{
					float heatLevel = this.HeatLevel;
				}
				else
				{
					ComponentInventoryBase.Slot slot = this.m_slots[this.FuelSlotIndex];
					if (slot.Count > 0)
					{
						float fuelHeatLevel = BlocksManager.Blocks[Terrain.ExtractContents(slot.Value)].FuelHeatLevel;
					}
				}
				string text = "text";
				if (text != this.m_smeltingRecipe)
				{
					this.m_smeltingRecipe = text;
					this.SmeltingProgress = 0f;
					this.m_music = 0;
				}
			}
			if (this.m_smeltingRecipe == null)
			{
				this.HeatLevel = 0f;
				this.m_fireTimeRemaining = 0f;
				this.m_music = -1;
			}
			if (this.m_smeltingRecipe != null && (double)this.m_fireTimeRemaining <= 0.0)
			{
				ComponentInventoryBase.Slot slot2 = this.m_slots[this.FuelSlotIndex];
				if (slot2.Count > 0)
				{
					Block block = BlocksManager.Blocks[Terrain.ExtractContents(slot2.Value)];
					if ((double)block.GetExplosionPressure(slot2.Value) > 0.0)
					{
						slot2.Count = 0;
						this.m_subsystemExplosions.TryExplodeBlock(coordinates.X, coordinates.Y, coordinates.Z, slot2.Value);
					}
					else if ((double)block.FuelHeatLevel > 0.0)
					{
						slot2.Count--;
						this.m_fireTimeRemaining = block.FuelFireDuration;
						this.m_fireTime = block.FuelFireDuration;
						this.HeatLevel = block.FuelHeatLevel;
					}
				}
			}
			if ((double)this.m_fireTimeRemaining <= 0.0)
			{
				this.m_smeltingRecipe = null;
				this.SmeltingProgress = 0f;
				this.m_music = -1;
			}
			if (this.m_smeltingRecipe != null)
			{
				if (this.m_fireTime == 0f)
				{
					this.m_fireTime = this.m_fireTimeRemaining;
				}
				this.SmeltingProgress = MathUtils.Min(this.m_fireTimeRemaining / this.m_fireTime, 1f);
				if ((double)this.SmeltingProgress >= 2.0)
				{
					this.m_smeltingRecipe = null;
					this.SmeltingProgress = 0f;
					this.m_updateSmeltingRecipe = true;
				}
			}
			if (this.m_subsystemTerrain.Terrain.GetCellContents(coordinates.X, coordinates.Y, coordinates.Z) != 0)
			{
				int cellValue = this.m_subsystemTerrain.Terrain.GetCellValue(coordinates.X, coordinates.Y, coordinates.Z);
				this.m_subsystemTerrain.ChangeCell(coordinates.X, coordinates.Y, coordinates.Z, Terrain.ReplaceContents(cellValue, ((double)this.HeatLevel > 0.0) ? 544 : 543), true);
			}
		}

		// Token: 0x06002194 RID: 8596 RVA: 0x000157FA File Offset: 0x000139FA
		public override int GetSlotCapacity(int slotIndex, int value)
		{
			if (slotIndex != this.FuelSlotIndex)
			{
				return base.GetSlotCapacity(slotIndex, value);
			}
			if ((double)BlocksManager.Blocks[Terrain.ExtractContents(value)].FuelHeatLevel > 0.0)
			{
				return base.GetSlotCapacity(slotIndex, value);
			}
			return 0;
		}

		// Token: 0x06002195 RID: 8597 RVA: 0x00015835 File Offset: 0x00013A35
		public override void AddSlotItems(int slotIndex, int value, int count)
		{
			base.AddSlotItems(slotIndex, value, count);
			this.m_updateSmeltingRecipe = true;
		}

		// Token: 0x06002196 RID: 8598 RVA: 0x00015847 File Offset: 0x00013A47
		public override int RemoveSlotItems(int slotIndex, int count)
		{
			this.m_updateSmeltingRecipe = true;
			return base.RemoveSlotItems(slotIndex, count);
		}

		// Token: 0x06002197 RID: 8599 RVA: 0x000E22A0 File Offset: 0x000E04A0
		protected override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			base.Load(valuesDictionary, idToEntityMap);
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_subsystemExplosions = base.Project.FindSubsystem<SubsystemExplosions>(true);
			this.m_componentBlockEntity = base.Entity.FindComponent<ComponentBlockEntity>(true);
			this.m_subsystemAudio = base.Project.FindSubsystem<SubsystemAudio>(true);
			this.m_furnaceSize = this.SlotsCount - 2;
			this.m_fireTimeRemaining = valuesDictionary.GetValue<float>("FireTimeRemaining");
			this.HeatLevel = valuesDictionary.GetValue<float>("HeatLevel");
			this.m_updateSmeltingRecipe = true;
		}

		// Token: 0x06002198 RID: 8600 RVA: 0x00015858 File Offset: 0x00013A58
		protected override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
		{
			base.Save(valuesDictionary, entityToIdMap);
			valuesDictionary.SetValue<float>("FireTimeRemaining", this.m_fireTimeRemaining);
			valuesDictionary.SetValue<float>("HeatLevel", this.HeatLevel);
		}

		// Token: 0x04001997 RID: 6551
		private ComponentBlockEntity m_componentBlockEntity;

		// Token: 0x04001998 RID: 6552
		private float m_fireTimeRemaining;

		// Token: 0x04001999 RID: 6553
		private int m_furnaceSize;

		// Token: 0x0400199A RID: 6554
		private readonly string[] m_matchedIngredients = new string[9];

		// Token: 0x0400199B RID: 6555
		private SubsystemExplosions m_subsystemExplosions;

		// Token: 0x0400199C RID: 6556
		private SubsystemTerrain m_subsystemTerrain;

		// Token: 0x0400199D RID: 6557
		private bool m_updateSmeltingRecipe;

		// Token: 0x0400199E RID: 6558
		private string m_smeltingRecipe;

		// Token: 0x0400199F RID: 6559
		private SubsystemAudio m_subsystemAudio;

		// Token: 0x040019A0 RID: 6560
		private int m_music;

		// Token: 0x040019A1 RID: 6561
		private float m_fireTime;
	}
}
