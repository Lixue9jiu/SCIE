using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	public abstract class ComponentMachine : ComponentInventoryBase
	{
		protected SubsystemExplosions SubsystemExplosions;
		protected SubsystemTerrain SubsystemTerrain;
		protected ComponentBlockEntity m_componentBlockEntity;
		protected bool m_updateSmeltingRecipe;
		public virtual int FuelSlotIndex
		{
			get
			{
				return SlotsCount - 2;
			}
		}

		public virtual int ResultSlotIndex => 0;

		public float HeatLevel;

		public float SmeltingProgress;

		public override int GetSlotCapacity(int slotIndex, int value)
		{
			Block block = BlocksManager.Blocks[Terrain.ExtractContents(value)];
			return slotIndex != FuelSlotIndex || (block is IFuel fuel ? fuel.GetHeatLevel(value): block.FuelHeatLevel) > 1f
				? base.GetSlotCapacity(slotIndex, value)
				: 0;
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
			base.Load(valuesDictionary, idToEntityMap);
			SubsystemTerrain = Project.FindSubsystem<SubsystemTerrain>(true);
			SubsystemExplosions = Project.FindSubsystem<SubsystemExplosions>(true);
			m_componentBlockEntity = Entity.FindComponent<ComponentBlockEntity>(false);
			m_updateSmeltingRecipe = true;
		}
	}
}
