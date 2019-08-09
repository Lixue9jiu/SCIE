using Engine;

namespace Game
{
	public class ComponentLiquidPump : ComponentDriller
	{
		public new void Dispense()
		{
			Point3 coordinates = m_componentBlockEntity.Coordinates;
			//DispenserNew2Block.GetMode(data);
			Driller(coordinates, FourDirectionalBlock.GetDirection(Utils.Terrain.GetCellValue(coordinates.X, coordinates.Y, coordinates.Z)));
		}

		protected new void Driller(Point3 point, int face)
		{
			Vector3 vector = Vector3.UnitY;
			int x = point.X;
			int y = point.Y;
			int z = point.Z;
			if (!ComponentEngine.IsPowered(Utils.Terrain, x, y, z))
				return;
			int num2 = 0;
			int l;
			for (l = 4; 3 < l && l < 8; l++)
				num2 += GetSlotCount(l);
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
						x = point.X + array[num5];
						y = point.Y - (int)vector.Y * n;
						z = point.Z + array2[num5];
						//new Vector3((float)array[num5] / (float)n, 0f, (float)array2[num5] / (float)n);
						int cellValue = Utils.Terrain.GetCellValue(x, y, z);
						var block = Terrain.ExtractContents(cellValue);
						if (block == 92 || block == 18 || block == RottenMeatBlock.Index)
						{
							int num6;
							for (num6 = 4; 3 < num6 && num6 < 8 && GetSlotCount(num6) == 0; num6++)
							{
							}
							int num7 = 0;
							while (true)
							{
								if (num7 >= 4)
									return;
								if (GetSlotCount(num7) < BlocksManager.Blocks[Terrain.ExtractContents(GetSlotValue(num7))].MaxStacking || GetSlotCount(num7) == 0)
								{
									if (block == 92 && (Terrain.ExtractContents(GetSlotValue(num7)) == 93 || GetSlotCount(num7) == 0))
									{
										if (FluidBlock.GetLevel(Terrain.ExtractData(cellValue)) == 0)
										{
											RemoveSlotItems(num6, 1);
											AddSlotItems(num7, 93, 1);
											RemoveSlotItems(8, 1);
											AddSlotItems(8, BlocksManager.DamageItem(slotValue, 1), 1);
										}
										Utils.SubsystemTerrain.ChangeCell(x, y, z, 0);
										return;
									}
									if (block == 18 && (Terrain.ExtractContents(GetSlotValue(num7)) == 91 || GetSlotCount(num7) == 0))
									{
										if (FluidBlock.GetLevel(Terrain.ExtractData(cellValue)) == 0)
										{
											RemoveSlotItems(num6, 1);
											AddSlotItems(num7, 91, 1);
										}
										Utils.SubsystemTerrain.ChangeCell(x, y, z, 0);
										return;
									}
									if (Terrain.ReplaceLight(cellValue, 0) == (RottenMeatBlock.Index | 1 << 4 << 14) && (Terrain.ReplaceLight(GetSlotValue(num7), 0) == (RottenMeatBlock.Index | 2 << 4 << 14) || GetSlotCount(num7) == 0))
									{
										RemoveSlotItems(num6, 1);
										AddSlotItems(num7, RottenMeatBlock.Index | 2 << 4 << 14, 1);
										Utils.SubsystemTerrain.ChangeCell(x, y, z, 0);
										return;
									}
								}
								num7++;
							}
						}
					}
				}
			}
		}

		public override int GetSlotCapacity(int slotIndex, int value)
		{
			if (slotIndex != 8)
				return slotIndex > 3 && Terrain.ExtractContents(value) != 90
					? 0
					: base.GetSlotCapacity(slotIndex, value);
			var type = DrillBlock.GetType(value);
			return (type == DrillType.IronTubularis || type == DrillType.SteelTubularis) ? base.GetSlotCapacity(slotIndex, DrillBlock.SetType(value, DrillType.SteelDrill)) : 0;
		}
	}
}