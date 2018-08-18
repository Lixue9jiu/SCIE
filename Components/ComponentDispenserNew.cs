using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	public class ComponentDispenserNew : ComponentInventoryBase
	{
		private ComponentBlockEntity m_componentBlockEntity;

		private readonly Random m_random = new Random();

		private SubsystemAudio m_subsystemAudio;

		private SubsystemPickables m_subsystemPickables;

		private SubsystemProjectiles m_subsystemProjectiles;

		private SubsystemTerrain m_subsystemTerrain;

		//private SubsystemParticles m_subsystemParticles;

		public void Dispense()
		{
			Point3 coordinates = m_componentBlockEntity.Coordinates;
			int data = Terrain.ExtractData(m_subsystemTerrain.Terrain.GetCellValue(coordinates.X, coordinates.Y, coordinates.Z));
			int direction = DispenserNewBlock.GetDirection(data);
			DispenserNewBlock.Mode mode = DispenserNewBlock.GetMode(data);
			Driller(coordinates, direction);
			int num = 0;
			int slotValue;
			while (true)
			{
				if (num >= SlotsCount - 1)
				{
					return;
				}
				slotValue = GetSlotValue(num);
				int slotCount = GetSlotCount(num);
				if (slotValue != 0 && slotCount > 0)
				{
					break;
				}
				num++;
			}
			if (mode == DispenserNewBlock.Mode.Shoot)
			{
				int num2 = RemoveSlotItems(num, 1);
				for (int i = 0; i < num2; i++)
				{
					DispenseItem(coordinates, direction, slotValue, mode);
				}
			}
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

		private void DispenseItem(Point3 point, int face, int value, DispenserNewBlock.Mode mode)
		{
			Vector3 vector = CellFace.FaceToVector3(face);
			Vector3 vector2 = new Vector3((float)point.X + 0.5f, (float)point.Y + 0.5f, (float)point.Z + 0.5f) + 0.6f * vector;
			if (mode != 0)
			{
				float num = m_random.UniformFloat(39f, 41f);
				if (m_subsystemProjectiles.FireProjectile(value, vector2, 1f * num * (vector + m_random.Vector3(0.025f, false) + new Vector3(0f, 0.05f, 0f)), Vector3.Zero, null) != null)
				{
					m_subsystemAudio.PlaySound("Audio/DispenserShoot", 1f, 0f, new Vector3(vector2.X, vector2.Y, vector2.Z), 4f, true);
				}
				else
				{
					DispenseItem(point, face, value, DispenserNewBlock.Mode.Dispense);
				}
			}
		}

		private void Driller(Point3 point, int face)
		{
			Vector3 vector = CellFace.FaceToVector3(face);
			Vector3 v = new Vector3(0f, 0f, 0f);
			int x = point.X;
			int y = point.Y;
			int z = point.Z;
			int num = 0;
			int slotValue = GetSlotValue(8);
			for (int i = -1; i < 2; i++)
			{
				for (int j = -1; j < 2; j++)
				{
					for (int k = -1; k < 2; k++)
					{
						int cellContents = m_subsystemTerrain.Terrain.GetCellContents(point.X + i, point.Y + j, point.Z + k);
						if (i * i + j * j + k * k <= 1 && (cellContents == 509 || cellContents == 534))
						{
							num = 1;
							break;
						}
					}
				}
			}
			if (num != 0 && slotValue != 0 && BlocksManager.Blocks[Terrain.ExtractContents(slotValue)].Durability > 0)
			{
				int[] array = new int[9]
				{
					0,
					0,
					1,
					1,
					1,
					0,
					-1,
					-1,
					-1
				};
				int[] array2 = new int[9]
				{
					0,
					-1,
					-1,
					0,
					1,
					1,
					1,
					0,
					-1
				};
				int num2 = 0;
				for (int l = 1; l < 19; l++)
				{
					for (int m = 0; m < 9; m++)
					{
						x = point.X - (int)vector.X * l;
						y = point.Y - (int)vector.Y * l;
						z = point.Z - (int)vector.Z * l;
						if (vector.X != 0f)
						{
							y = point.Y + array[m];
							z = point.Z + array2[m];
							v = new Vector3(0f, (float)array[m] / (float)l, (float)array2[m] / (float)l);
						}
						if (vector.Y != 0f)
						{
							x = point.X + array[m];
							z = point.Z + array2[m];
							v = new Vector3((float)array[m] / (float)l, 0f, (float)array2[m] / (float)l);
						}
						if (vector.Z != 0f)
						{
							x = point.X + array[m];
							y = point.Y + array2[m];
							v = new Vector3((float)array[m] / (float)l, (float)array2[m] / (float)l, 0f);
						}
						int cellValue = m_subsystemTerrain.Terrain.GetCellValue(x, y, z);
						Block block = BlocksManager.Blocks[Terrain.ExtractContents(cellValue)];
						if (num2 == 0 && block.BlockIndex == 92)
						{
							num2 = 9;
						}
						if (block.IsPlaceable && block.DefaultCreativeData != -1 && !block.DefaultIsInteractive)
						{
							m_subsystemTerrain.ChangeCell(x, y, z, 0, true);
							m_subsystemProjectiles.FireProjectile(cellValue, new Vector3((float)x + 0.5f, (float)y + 0.5f, (float)z + 0.5f) - 0.25f * vector, 60f * (vector - v), Vector3.Zero, null);
							RemoveSlotItems(8, 1);
							AddSlotItems(8, BlocksManager.DamageItem(slotValue, 1 + num2), 1);
							return;
						}
					}
				}
			}
		}

		public override int GetSlotCapacity(int slotIndex, int value)
		{
			if (slotIndex != 8)
			{
				return base.GetSlotCapacity(slotIndex, value);
			}
			if (BlocksManager.Blocks[Terrain.ExtractContents(value)].BlockIndex == 535 || BlocksManager.Blocks[Terrain.ExtractContents(value)].BlockIndex == 536)
			{
				return base.GetSlotCapacity(slotIndex, value);
			}
			return 0;
		}
	}
}
