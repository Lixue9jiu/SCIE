using GameEntitySystem;
using System;
using TemplatesDatabase;

namespace Game
{
	public abstract class ComponentMachine : ComponentInventoryBase, ICraftingMachine
	{
		protected float m_fireTimeRemaining;
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
				Block block = BlocksManager.Blocks[Terrain.ExtractContents(value)];
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

		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.LoadItems(valuesDictionary);
			m_componentBlockEntity = Entity.FindComponent<ComponentBlockEntity>();
			m_updateSmeltingRecipe = true;
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
}