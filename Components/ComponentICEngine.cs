using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	public class ComponentRocketEngine : ComponentSMachine
	{
		public int Fuel2SlotIndex => 0;
		public float h2a;
		public float o2a;
		public bool start;
	}

	public class ComponentICEngine : ComponentSMachine, IUpdateable
	{
		public override int ResultSlotIndex => SlotsCount - 1;

		public int Fuel2SlotIndex => 0;

		public void Update(float dt)
		{
			if (m_updateSmeltingRecipe)
			{
				if (m_slots[Fuel2SlotIndex].Count == 0)
				{
					m_updateSmeltingRecipe = false;
				}
				string text = null;
				if (base.GetSlotCount(Fuel2SlotIndex) > 0 && base.GetSlotValue(Fuel2SlotIndex) == (240 | 12 << 18) && SmeltingProgress <= 950f)
					text = "bucket";
				//while (text != null && SmeltingProgress <= 900f)
				//{
				if (text != null)
				{
					int resultSlotIndex = ResultSlotIndex;
					if (m_slots[resultSlotIndex].Count != 0 && (90 != m_slots[ResultSlotIndex].Value || m_slots[ResultSlotIndex].Count >= 40))
						text = null;
					else
					{
						if (m_slots[Fuel2SlotIndex].Count > 0)
						{
							m_slots[ResultSlotIndex].Value = 90;
							m_slots[ResultSlotIndex].Count += 1;
							m_slots[Fuel2SlotIndex].Count -= 1;
							SmeltingProgress += 50f;
						}

						m_fireTimeRemaining = SmeltingProgress;
					}
				}
				//}
			}
			SmeltingProgress = m_fireTimeRemaining;
			if (SmeltingProgress > 0f && HeatLevel > 0f)
			{
				SmeltingProgress = MathUtils.Min(SmeltingProgress - 0.1f * dt, 1000f);
				m_fireTimeRemaining = SmeltingProgress;
				//HeatLevel = 1000f;
			}
			if (SmeltingProgress < 0f)
			{
				SmeltingProgress = 0f;
			}
			if (SmeltingProgress == 0f && HeatLevel > 0f)
			{
				HeatLevel = 0f;
			}
			if (m_componentBlockEntity != null)
			{
				var c = m_componentBlockEntity.Coordinates;
				TerrainChunk chunk = Utils.Terrain.GetChunkAtCell(c.X, c.Z);
				if (chunk != null && chunk.State == TerrainChunkState.Valid)
				{
					int cellValue = chunk.GetCellValueFast(c.X & 15, c.Y, c.Z & 15);
					Utils.SubsystemTerrain.ChangeCell(c.X, c.Y, c.Z, Terrain.ReplaceData(cellValue, FurnaceNBlock.SetHeatLevel(Terrain.ExtractData(cellValue), (int)MathUtils.Sign(HeatLevel))));
				}
			}
		}

		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			base.Load(valuesDictionary, idToEntityMap);
			m_furnaceSize = SlotsCount - 1;
			m_fireTimeRemaining = valuesDictionary.GetValue("FireTimeRemaining", 0f);
			HeatLevel = valuesDictionary.GetValue("HeatLevel", 0f);
		}

		public override int GetSlotCapacity(int slotIndex, int value)
		{
			return (slotIndex == Fuel2SlotIndex && value == (240 | 12 << 18)) ||
				(slotIndex == ResultSlotIndex && Terrain.ExtractContents(value) == EmptyBucketBlock.Index) ||
				(slotIndex != Fuel2SlotIndex && slotIndex != ResultSlotIndex)
				? base.GetSlotCapacity(slotIndex, value)
				: 0;
		}
	}



	public class ComponentNEngine : ComponentSMachine, IUpdateable
	{
		public override int ResultSlotIndex => SlotsCount - 1;

		public int Fuel2SlotIndex => 0;

		public void Update(float dt)
		{
			
				int value = GetSlotValue(0);
				if (m_fireTimeRemaining == 1000f && value != 0 && Terrain.ExtractContents(value) == FuelRodBlock.Index && FuelRodBlock.GetType(value) == RodType.UFuelRod)
				{
					HeatLevel = 1000f;
				if (Utils.Random.Bool(0.0005f))
				{
					
					if (BlocksManager.DamageItem(value, 800) != BlocksManager.DamageItem(value, 2000))
					{
						RemoveSlotItems(0, 1);
						AddSlotItems(0, BlocksManager.DamageItem(value, 1), 1);
					}
					else
					{
						m_fireTimeRemaining = 0;
						//AddSlotItems(0, ItemBlock.IdTable["UsedUpFuel"], 1);

					}
				}
				


			}
				else
				{
					m_fireTimeRemaining = 0f;
					HeatLevel = 0f;
				}
			
			if (m_componentBlockEntity != null)
			{
				var c = m_componentBlockEntity.Coordinates;
				if (Utils.SubsystemTime.PeriodicGameTimeEvent(1.5, 0.0) && HeatLevel>0f)
				{
					bool flag = false;
					if (Utils.SubsystemTime.PeriodicGameTimeEvent(1.2, 0.0))
						for (int iii = 0; iii < Utils.SubsystemSour.m_radations.Count; iii++)
						{
							if (Utils.SubsystemSour.m_radations.Array[iii] == new Vector4(c.X, c.Y, c.Z, 30f))
							{
								flag = true; break;
							}
						}
					if (!flag)
						Utils.SubsystemSour.m_radations.Add(new Vector4(c.X, c.Y, c.Z, 30f));
				}

				TerrainChunk chunk = Utils.Terrain.GetChunkAtCell(c.X, c.Z);
				if (chunk != null && chunk.State == TerrainChunkState.Valid)
				{
					int cellValue = chunk.GetCellValueFast(c.X & 15, c.Y, c.Z & 15);
					Utils.SubsystemTerrain.ChangeCell(c.X, c.Y, c.Z, Terrain.ReplaceData(cellValue, FurnaceNBlock.SetHeatLevel(Terrain.ExtractData(cellValue), (int)MathUtils.Sign(HeatLevel))));
				}
			}
		}

		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			base.Load(valuesDictionary, idToEntityMap);
			m_furnaceSize = SlotsCount - 1;
			m_fireTimeRemaining = valuesDictionary.GetValue("FireTimeRemaining", 0f);
			HeatLevel = valuesDictionary.GetValue("HeatLevel", 0f);
		}

		public override int GetSlotCapacity(int slotIndex, int value)
		{
			return (Terrain.ExtractContents(value) == FuelRodBlock.Index && FuelRodBlock.GetType(value) == RodType.UFuelRod) || value == ItemBlock.IdTable["UsedUpFuel"] ? base.GetSlotCapacity(slotIndex, value)
			: 0;
		}
	}
	public class ComponentCabiner : ComponentChest
	{
		public override int GetSlotCapacity(int slotIndex, int value)
		{
			return (Terrain.ExtractContents(value) == Bullet2Block.Index && value == Terrain.MakeBlockValue(521, 0, Bullet2Block.SetBulletType(0, Bullet2Block.BulletType.HandBullet))) ? base.GetSlotCapacity(slotIndex, value)
			: 0;
		}
	}
}