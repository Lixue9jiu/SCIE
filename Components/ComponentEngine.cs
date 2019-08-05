using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	public class ComponentEngine : ComponentMachine, IUpdateable
	{
		protected string m_smeltingRecipe;

		protected int m_music;

		public Point3 Coordinates;

		public override int RemainsSlotIndex => SlotsCount - 3;
		public override int ResultSlotIndex => SlotsCount - 1;
		public override int FuelSlotIndex => SlotsCount - 2;

		public int UpdateOrder => 0;

		public void Update(float dt)
		{
			if (Coordinates.Y < 0 || Coordinates.Y > 127)
				return;
			if (HeatLevel > 0f)
			{
				m_fireTimeRemaining = MathUtils.Max(0f, m_fireTimeRemaining - dt);
				if (m_fireTimeRemaining == 0f)
					HeatLevel = 0f;
			}
			Slot slot;
			if (m_updateSmeltingRecipe)
			{
				m_updateSmeltingRecipe = false;
				float heatLevel = 0f;
				if (HeatLevel > 0f)
					heatLevel = HeatLevel;
				else
				{
					slot = m_slots[FuelSlotIndex];
					if (slot.Count > 0)
					{
						var block = BlocksManager.Blocks[Terrain.ExtractContents(slot.Value)];
						heatLevel = block is IFuel fuel ? fuel.GetHeatLevel(slot.Value) : block.FuelHeatLevel;
					}
				}
				string text = FindSmeltingRecipe(heatLevel);
				if (text != m_smeltingRecipe)
				{
					m_smeltingRecipe = text;
					SmeltingProgress = 0f;
					m_music = 0;
				}
			}
			if (m_smeltingRecipe == null)
			{
				HeatLevel = 0f;
				m_fireTimeRemaining = 0f;
				m_music = -1;
			}
			else if (m_smeltingRecipe != null && m_fireTimeRemaining <= 0f)
			{
				slot = m_slots[FuelSlotIndex];
				if (slot.Count > 0)
				{
					var block = BlocksManager.Blocks[Terrain.ExtractContents(slot.Value)];
					if (block.GetExplosionPressure(slot.Value) > 0f)
					{
						slot.Count = 0;
						Utils.SubsystemExplosions.TryExplodeBlock(Coordinates.X, Coordinates.Y, Coordinates.Z, slot.Value);
					}
					else
					{
						slot.Count--;
						if (block is IFuel fuel)
						{
							HeatLevel = fuel.GetHeatLevel(slot.Value);
							m_fireTimeRemaining = fuel.GetFuelFireDuration(slot.Value);
						}
						else
						{
							HeatLevel = block.FuelHeatLevel;
							m_fireTimeRemaining = block.FuelFireDuration;
						}
					}
				}
			}
			if (m_fireTimeRemaining <= 0f)
			{
				m_smeltingRecipe = null;
				SmeltingProgress = 0f;
				m_music = -1;
			}
			if (m_smeltingRecipe != null)
			{
				SmeltingProgress = MathUtils.Min(SmeltingProgress + 0.02f * dt, 1f);
				if (m_music % 90 == 0)
					Utils.SubsystemAudio.PlaySound("Audio/SteamEngine", 1f, 0f, new Vector3(Coordinates), 4f, true);
				m_music++;
				if (SmeltingProgress >= 1.0)
				{
					for (int i = 0; i < m_furnaceSize; i++)
						if (m_slots[i].Count > 0)
							m_slots[i].Count--;
					m_slots[ResultSlotIndex].Value = EmptyBucketBlock.Index;
					m_slots[ResultSlotIndex].Count++;
					m_smeltingRecipe = null;
					SmeltingProgress = 0f;
					m_updateSmeltingRecipe = true;
				}
			}
			if (m_componentBlockEntity != null)
			{
				TerrainChunk chunk = Utils.Terrain.GetChunkAtCell(Coordinates.X, Coordinates.Z);
				if (chunk != null && chunk.State == TerrainChunkState.Valid)
				{
					int cellValue = chunk.GetCellValueFast(Coordinates.X & 15, Coordinates.Y, Coordinates.Z & 15);
					Utils.SubsystemTerrain.ChangeCell(Coordinates.X, Coordinates.Y, Coordinates.Z, Terrain.ReplaceData(cellValue, FurnaceNBlock.SetHeatLevel(Terrain.ExtractData(cellValue), HeatLevel > 0f ? 1 : 0)));
				}
			}
		}

		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			base.Load(valuesDictionary, idToEntityMap);
			if (m_componentBlockEntity != null)
				Coordinates = m_componentBlockEntity.Coordinates;
			m_furnaceSize = SlotsCount - 2;
			m_fireTimeRemaining = valuesDictionary.GetValue("FireTimeRemaining", 0f);
			HeatLevel = valuesDictionary.GetValue("HeatLevel", 0f);
		}

		protected string FindSmeltingRecipe(float heatLevel)
		{
			if (heatLevel < 100f)
				return null;
			string text = null;
			for (int i = 0; i < m_furnaceSize; i++)
			{
				if (GetSlotCount(i) > 0)
					if (Terrain.ExtractContents(base.GetSlotValue(i)) == WaterBucketBlock.Index)
						text = "bucket";
			}
			if (text != null)
			{
				Slot slot = m_slots[ResultSlotIndex];
				if (slot.Count != 0 && (90 != slot.Value || slot.Count >= 40))
					text = null;
			}
			return text;
		}

        public static bool IsPowered(Terrain terrain, int x, int y, int z ,bool flag=true)
        {
            var chunk = terrain.GetChunkAtCell(x, z);
            if (y < 0 || y > 127 || chunk == null)
                return false;
            int cellValue = terrain.GetCellValueFast(x + 1, y, z);
            
            if (ElementBlock.Block.GetDevice(x + 1, y, z, cellValue) is ElectricMotor abb && abb.EPowered && flag)
                 return true;
            
            if (FurnaceNBlock.GetHeatLevel(cellValue) != 0)
            {
                cellValue = Terrain.ExtractContents(cellValue);
                if (cellValue == EngineBlock.Index || cellValue == EngineHBlock.Index)
                    return true;
            }

            cellValue = terrain.GetCellValueFast(x - 1, y, z);
            if (ElementBlock.Block.GetDevice(x - 1, y, z, cellValue) is ElectricMotor abc && abc.EPowered && flag)
                return true;

            if (FurnaceNBlock.GetHeatLevel(cellValue) != 0)
            {
                cellValue = Terrain.ExtractContents(cellValue);
                if (cellValue == EngineBlock.Index || cellValue == EngineHBlock.Index)
                    return true;
            }
            if (y < 127)
            {
                cellValue = chunk.GetCellValueFast(x & 15, y + 1, z & 15);
                if (ElementBlock.Block.GetDevice(x & 15, y + 1, z & 15, cellValue) is ElectricMotor abd && abd.EPowered && flag)
                    return true;

                if (FurnaceNBlock.GetHeatLevel(cellValue) != 0)
                {
                    cellValue = Terrain.ExtractContents(cellValue);
                    if (cellValue == EngineBlock.Index || cellValue == EngineHBlock.Index)
                        return true;
                }
            }
            if (y > 0)
            {
                cellValue = chunk.GetCellValueFast(x & 15, y - 1, z & 15);
                if (ElementBlock.Block.GetDevice(x & 15, y - 1, z & 15, cellValue) is ElectricMotor abe && abe.EPowered && flag)
                    return true;

                if (FurnaceNBlock.GetHeatLevel(cellValue) != 0)
                {
                    cellValue = Terrain.ExtractContents(cellValue);
                    if (cellValue == EngineBlock.Index || cellValue == EngineHBlock.Index)
                        return true;
                }
            }
            cellValue = terrain.GetCellValueFast(x, y, z + 1);
            if (ElementBlock.Block.GetDevice(x, y , z +1, cellValue) is ElectricMotor abf && abf.EPowered && flag)
                return true;

            if (FurnaceNBlock.GetHeatLevel(cellValue) != 0)
            {
                cellValue = Terrain.ExtractContents(cellValue);
                if (cellValue == EngineBlock.Index || cellValue == EngineHBlock.Index)
                    return true;
            }
            cellValue = terrain.GetCellValueFast(x, y, z - 1);
            if (ElementBlock.Block.GetDevice(x, y , z -1, cellValue) is ElectricMotor abq && abq.EPowered && flag)
                return true;

            if (FurnaceNBlock.GetHeatLevel(cellValue) != 0)
            {
                cellValue = Terrain.ExtractContents(cellValue);
                if (cellValue == EngineBlock.Index || cellValue == EngineHBlock.Index)
                    return true;
            }
            return false;
        }
    }
}