using Engine;
using GameEntitySystem;
using System;
using TemplatesDatabase;

namespace Game
{
	public abstract class ComponentMachine : ComponentInventoryBase, ICraftingMachine
	{
		public float m_fireTimeRemaining;
		protected int m_furnaceSize;
		protected ComponentBlockEntity m_componentBlockEntity;
		protected bool m_updateSmeltingRecipe;

		public virtual int FuelSlotIndex => SlotsCount - 2;

		public virtual int ResultSlotIndex => 0;
		public virtual int RemainsSlotIndex => SlotsCount - 1;

		public int SlotIndex { get => -1; set => throw new NotImplementedException(); }

		public float HeatLevel;

		public float SmeltingProgress;

		public override int GetSlotCapacity(int slotIndex, int value)
		{
			if (slotIndex == FuelSlotIndex)
			{
				var block = BlocksManager.Blocks[Terrain.ExtractContents(value)];
				if ((block is IFuel fuel ? fuel.GetHeatLevel(value) : block.FuelHeatLevel) < 1f)
					return 0;
			}
			return base.GetSlotCapacity(slotIndex, value);
		}

		public override void AddSlotItems(int slotIndex, int value, int count)
		{
			base.AddSlotItems(slotIndex, value, count);
			m_updateSmeltingRecipe = true;
		}

		public override int RemoveSlotItems(int slotIndex, int count)
		{
			m_updateSmeltingRecipe = true;
			return base.RemoveSlotItems(slotIndex, count);
		}

		public override void ProcessSlotItems(int slotIndex, int value, int count, int processCount, out int processedValue, out int processedCount)
		{
			IItem item = Item.Block.GetItem(ref value);
			if (slotIndex == FuelSlotIndex && item is FuelCylinder fuel)
			{
				HeatLevel = fuel.GetHeatLevel(value);
				m_fireTimeRemaining = fuel.GetFuelFireDuration(value);
				processedValue = fuel.GetDamageDestructionValue();
				processedCount = 1;
				return;
			}
			base.ProcessSlotItems(slotIndex, value, count, processCount, out processedValue, out processedCount);
		}

		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.LoadItems(valuesDictionary);
			m_componentBlockEntity = Entity.FindComponent<ComponentBlockEntity>();
			m_updateSmeltingRecipe = true;
			m_fireTimeRemaining = valuesDictionary.GetValue("FireTimeRemaining", 0f);
			HeatLevel = valuesDictionary.GetValue("HeatLevel", 0f);
		}

		public CraftingRecipe GetRecipe() => null;

		public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
		{
			this.SaveItems(valuesDictionary);
			if (m_fireTimeRemaining > 0f)
				valuesDictionary.SetValue("FireTimeRemaining", m_fireTimeRemaining);
			if (HeatLevel > 0f)
				valuesDictionary.SetValue("HeatLevel", HeatLevel);
		}

		/*public static float GetFuelHeatLevel(int value)
		{
			Block block = BlocksManager.Blocks[Terrain.ExtractContents(value)];
			return block is IFuel fuel ? fuel.GetHeatLevel(value) : block.FuelHeatLevel;
		}*/
	}
	public abstract class ComponentPMach : ComponentMachine, IUpdateable
	{
		protected int m_count;
		protected float m_speed;
		protected string m_smeltingRecipe;

		//protected int m_music;

		protected string m_smeltingRecipe2;

		public override int RemainsSlotIndex => -1;

		public override int ResultSlotIndex => SlotsCount - 1;

		public override int FuelSlotIndex => -1;

		public int UpdateOrder => 0;

		public void Update(float dt)
		{
			if (HeatLevel > 0f)
			{
				m_fireTimeRemaining = MathUtils.Max(0f, m_fireTimeRemaining - dt);
				if (m_fireTimeRemaining == 0f)
					HeatLevel = 0f;
			}
			if (m_updateSmeltingRecipe)
			{
				m_updateSmeltingRecipe = false;
				m_smeltingRecipe2 = FindSmeltingRecipe();
				if (m_smeltingRecipe2 != m_smeltingRecipe)
				{
					m_smeltingRecipe = m_smeltingRecipe2;
					SmeltingProgress = 0f;
					//m_music = 0;
				}
			}
			if (m_smeltingRecipe2 != null)
			{
				Point3 coordinates = m_componentBlockEntity.Coordinates;
				m_smeltingRecipe = ComponentEngine.IsPowered(Utils.Terrain, coordinates.X, coordinates.Y, coordinates.Z) ? m_smeltingRecipe2 : null;
			}
			if (m_smeltingRecipe == null)
			{
				HeatLevel = 0f;
				m_fireTimeRemaining = 0f;
				//m_music = -1;
			}
			else
				m_fireTimeRemaining = 100f;
			if (m_fireTimeRemaining <= 0f)
			{
				m_smeltingRecipe = null;
				SmeltingProgress = 0f;
				//m_music = -1;
			}
			if (m_smeltingRecipe != null)
			{
				SmeltingProgress = MathUtils.Min(SmeltingProgress + m_speed * dt, 1f);
				if (SmeltingProgress >= 1f)
				{
					for (int l = 0; l < m_furnaceSize; l++)
						if (m_slots[l].Count > 0)
							m_slots[l].Count--;
					m_slots[ResultSlotIndex].Value = ItemBlock.IdTable[m_smeltingRecipe];
					m_slots[ResultSlotIndex].Count += m_count;
					m_smeltingRecipe = null;
					SmeltingProgress = 0f;
					m_updateSmeltingRecipe = true;
				}
			}
		}

		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			base.Load(valuesDictionary, idToEntityMap);
			m_furnaceSize = SlotsCount - 1;
			//m_speed = valuesDictionary.GetValue("Speed", 0.1f);
			//m_count = valuesDictionary.GetValue("Count", 1);
			m_speed = 0.1f;
			m_count = 1;
		}

		protected virtual string FindSmeltingRecipe()
		{
			return null;
		}
	}
}