using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	public abstract class ComponentMachine : ComponentInventoryBase
	{
		protected SubsystemExplosions m_subsystemExplosions;
		protected SubsystemTerrain m_subsystemTerrain;
		protected ComponentBlockEntity m_componentBlockEntity;
		protected bool m_updateSmeltingRecipe;
		public virtual int FuelSlotIndex
		{
			get
			{
				return SlotsCount - 2;
			}
		}
		public override int GetSlotCapacity(int slotIndex, int value)
		{
			return slotIndex != FuelSlotIndex || BlocksManager.Blocks[Terrain.ExtractContents(value)].FuelHeatLevel > 1f
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
			m_subsystemTerrain = Project.FindSubsystem<SubsystemTerrain>(true);
			m_subsystemExplosions = Project.FindSubsystem<SubsystemExplosions>(true);
			m_componentBlockEntity = Entity.FindComponent<ComponentBlockEntity>(false);
			m_updateSmeltingRecipe = true;
		}
	}
}
