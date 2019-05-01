using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	public class ComponentFireBox : ComponentMachine, IUpdateable
	{
		protected string m_smeltingRecipe;

		//protected int m_music;

		protected float m_fireTime;

		protected float m_speed;

		public override int RemainsSlotIndex => SlotsCount - 1;

		public override int ResultSlotIndex => SlotsCount - 1;

		public override int FuelSlotIndex => SlotsCount - 1;

		public int UpdateOrder => 0;

		public void Update(float dt)
		{
			Point3 coordinates = m_componentBlockEntity.Coordinates;
			if (coordinates.Y < 0 || coordinates.Y > 127)
				return;
			if (HeatLevel > 0f)
			{
				m_fireTimeRemaining = MathUtils.Max(0f, m_fireTimeRemaining - dt * m_speed);
				if (m_fireTimeRemaining == 0f)
					HeatLevel = 0f;
			}
			Slot slot;
			if (m_updateSmeltingRecipe)
			{
				m_updateSmeltingRecipe = false;
				if (HeatLevel <= 0f)
				{
					slot = m_slots[FuelSlotIndex];
					if (slot.Count > 0)
					{
						Block block = BlocksManager.Blocks[Terrain.ExtractContents(slot.Value)];
						HeatLevel = block is IFuel fuel ? fuel.GetHeatLevel(slot.Value) : block.FuelHeatLevel;
					}
				}
				if (!"text".Equals(m_smeltingRecipe))
				{
					m_smeltingRecipe = "text";
					SmeltingProgress = 0f;
					//m_music = 0;
				}
			}
			if (m_smeltingRecipe == null)
			{
				HeatLevel = 0f;
				m_fireTimeRemaining = 0f;
				//m_music = -1;
			}
			else if (m_fireTimeRemaining <= 0f)
			{
				slot = m_slots[FuelSlotIndex];
				if (slot.Count > 0)
				{
					var block = BlocksManager.Blocks[Terrain.ExtractContents(slot.Value)];
					if (block.GetExplosionPressure(slot.Value) > 0f)
					{
						slot.Count = 0;
						Utils.SubsystemExplosions.TryExplodeBlock(coordinates.X, coordinates.Y, coordinates.Z, slot.Value);
					}
					else
					{
						slot.Count--;
						if (block is IFuel fuel)
						{
							HeatLevel = fuel.GetHeatLevel(slot.Value);
							m_fireTime = m_fireTimeRemaining = fuel.GetFuelFireDuration(slot.Value);
						}
						else
						{
							HeatLevel = block.FuelHeatLevel;
							m_fireTime = m_fireTimeRemaining = block.FuelFireDuration;
						}
					}
				}
			}
			if (m_fireTimeRemaining <= 0f)
			{
				m_smeltingRecipe = null;
				SmeltingProgress = 0f;
				//m_music = -1;
			}
			if (m_smeltingRecipe != null)
			{
				if (m_fireTime == 0f)
					m_fireTime = m_fireTimeRemaining;
				SmeltingProgress = MathUtils.Min(m_fireTimeRemaining / m_fireTime, 1f);
				if (SmeltingProgress >= 2f)
				{
					m_smeltingRecipe = null;
					SmeltingProgress = 0f;
					m_updateSmeltingRecipe = true;
				}
			}
			TerrainChunk chunk = Utils.Terrain.GetChunkAtCell(coordinates.X, coordinates.Z);
			if (chunk != null && chunk.State == TerrainChunkState.Valid)
			{
				int cellValue = chunk.GetCellValueFast(coordinates.X & 15, coordinates.Y, coordinates.Z & 15);
				Utils.SubsystemTerrain.ChangeCell(coordinates.X, coordinates.Y, coordinates.Z, Terrain.ReplaceData(cellValue, FurnaceNBlock.SetHeatLevel(Terrain.ExtractData(cellValue), HeatLevel > 0f ? 1 : 0)));
			}
		}

		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			base.Load(valuesDictionary, idToEntityMap);
			m_fireTimeRemaining = valuesDictionary.GetValue("FireTimeRemaining", 0f);
			HeatLevel = valuesDictionary.GetValue("HeatLevel", 0f);
			m_speed = valuesDictionary.GetValue("Speed", 1f);
		}
	}
}