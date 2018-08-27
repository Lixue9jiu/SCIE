using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	public class ComponentDispenserNew2 : ComponentInventoryBase
	{
		protected ComponentBlockEntity m_componentBlockEntity;

		protected SubsystemAudio m_subsystemAudio;

		protected SubsystemPickables m_subsystemPickables;

		protected SubsystemProjectiles m_subsystemProjectiles;

		protected SubsystemTerrain m_subsystemTerrain;

		//protected SubsystemParticles m_subsystemParticles;

		public void Dispense()
		{
			Point3 coordinates = m_componentBlockEntity.Coordinates;
			int data = Terrain.ExtractData(m_subsystemTerrain.Terrain.GetCellValue(coordinates.X, coordinates.Y, coordinates.Z));
			//DispenserNew2Block.GetMode(data);
			Driller(coordinates, DispenserNew2Block.GetDirection(data));
		}

		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			base.Load(valuesDictionary, idToEntityMap);
			m_subsystemTerrain = Project.FindSubsystem<SubsystemTerrain>(true);
			m_subsystemAudio = Project.FindSubsystem<SubsystemAudio>(true);
			m_subsystemPickables = Project.FindSubsystem<SubsystemPickables>(true);
			m_subsystemProjectiles = Project.FindSubsystem<SubsystemProjectiles>(true);
			m_componentBlockEntity = Entity.FindComponent<ComponentBlockEntity>(true);
		}

		protected void Driller(Point3 point, int face)
		{
			Vector3 vector = Vector3.UnitY;
			//new Vector3(0f, 0f, 0f);
			int x = point.X;
			int y = point.Y;
			int z = point.Z;
			int num = ComponentEngine.IsPowered(m_subsystemTerrain.Terrain, x, y, z) ? 1 : 0;
			if (num != 0)
			{
				int num2 = 0;
				int l;
				for (l = 4; 3 < l && l < 8; l++)
				{
					num2 += GetSlotCount(l);
				}
				int num3 = 0;
				int num4 = 0;
				for (int m = 0; m < 4; m++)
				{
					if (GetSlotCount(l) == 0)
					{
						num3 = 0;
						num4 = 1;
						break;
					}
					num4 += BlocksManager.Blocks[Terrain.ExtractContents(GetSlotValue(m))].MaxStacking;
					num3 += GetSlotCount(m);
				}
				int slotValue = base.GetSlotValue(8);
				if (num2 != 0 && num4 > num3 && slotValue != 0 && BlocksManager.Blocks[Terrain.ExtractContents(slotValue)].Durability > 0)
				{
					int[] array = new int[25]
					{
						0,
						0,
						1,
						1,
						1,
						0,
						-1,
						-1,
						-1,
						-1,
						0,
						1,
						2,
						2,
						2,
						2,
						2,
						1,
						0,
						-1,
						-2,
						-2,
						-2,
						-2,
						-2
					};
					int[] array2 = new int[25]
					{
						0,
						-1,
						-1,
						0,
						1,
						1,
						1,
						0,
						-1,
						-2,
						-2,
						-2,
						-2,
						-1,
						0,
						1,
						2,
						2,
						2,
						2,
						2,
						1,
						0,
						-1,
						-2
					};
					for (int n = 1; n < 8; n++)
					{
						for (int num5 = 0; num5 < 25; num5++)
						{
							x = point.X - (int)vector.X * n;
							y = point.Y - (int)vector.Y * n;
							z = point.Z - (int)vector.Z * n;
							x = point.X + array[num5];
							z = point.Z + array2[num5];
							//new Vector3((float)array[num5] / (float)n, 0f, (float)array2[num5] / (float)n);
							int cellValue = m_subsystemTerrain.Terrain.GetCellValue(x, y, z);
							Block block = BlocksManager.Blocks[Terrain.ExtractContents(cellValue)];
							if (block.BlockIndex == 92 || block.BlockIndex == 18)
							{
								int num6;
								for (num6 = 4; 3 < num6 && num6 < 8 && GetSlotCount(num6) == 0; num6++)
								{
								}
								int num7 = 0;
								while (true)
								{
									if (num7 >= 4)
									{
										return;
									}
									if (GetSlotCount(num7) < BlocksManager.Blocks[Terrain.ExtractContents(GetSlotValue(num7))].MaxStacking || GetSlotCount(num7) == 0)
									{
										if (block.BlockIndex == 92 && (BlocksManager.Blocks[Terrain.ExtractContents(GetSlotValue(num7))].BlockIndex == 93 || GetSlotCount(num7) == 0))
										{
											if (FluidBlock.GetLevel(Terrain.ExtractData(cellValue)) == 0)
											{
												AddSlotItems(num7, 93, 1);
												RemoveSlotItems(num6, 1);
												RemoveSlotItems(8, 1);
												AddSlotItems(8, BlocksManager.DamageItem(slotValue, 1), 1);
											}
											m_subsystemTerrain.ChangeCell(x, y, z, 0, true);
											return;
										}
										if (block.BlockIndex == 18 && (BlocksManager.Blocks[Terrain.ExtractContents(GetSlotValue(num7))].BlockIndex == 91 || GetSlotCount(num7) == 0))
										{
											break;
										}
									}
									num7++;
								}
								if (FluidBlock.GetLevel(Terrain.ExtractData(cellValue)) == 0)
								{
									AddSlotItems(num7, 91, 1);
									RemoveSlotItems(num6, 1);
								}
								m_subsystemTerrain.ChangeCell(x, y, z, 0, true);
								return;
							}
						}
					}
				}
			}
		}

		public override int GetSlotCapacity(int slotIndex, int value)
		{
			if (slotIndex != 8)
			{
				return slotIndex > 3 && BlocksManager.Blocks[Terrain.ExtractContents(value)].BlockIndex != 90
					? 0
					: base.GetSlotCapacity(slotIndex, value);
			}
			if (Terrain.ExtractContents(value) != DrillBlock.Index)
				return 0;
			var type = DrillBlock.GetType(value);
			return (type == DrillBlock.Type.IronTubularis || type == DrillBlock.Type.SteelTubularis) ? base.GetSlotCapacity(slotIndex, value) : 0;
		}
	}
}
