using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	public class ComponentDriller : ComponentInventoryBase
	{
		protected ComponentBlockEntity m_componentBlockEntity;

		public virtual void Dispense()
		{
			Point3 coordinates = m_componentBlockEntity.Coordinates;
			int data = Terrain.ExtractData(Utils.Terrain.GetCellValue(coordinates.X, coordinates.Y, coordinates.Z));
			int direction = FourDirectionalBlock.GetDirection(Utils.Terrain.GetCellValue(coordinates.X, coordinates.Y, coordinates.Z));
			Driller(coordinates, direction);
			int num = 0;
			int value;
			while (true)
			{
				if (num >= SlotsCount - 1)
					return;
				value = GetSlotValue(num);
				int slotCount = GetSlotCount(num);
				if (value != 0 && slotCount > 0)
					break;
				num++;
			}
			MachineMode mode = DrillerBlock.GetMode(data);
			if (mode == MachineMode.Shoot)
			{
				int num2 = RemoveSlotItems(num, 1);
				for (int i = 0; i < num2; i++)
					DispenseItem(coordinates, direction, value, mode);
			}
		}

		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.LoadItems(valuesDictionary);
			m_componentBlockEntity = Entity.FindComponent<ComponentBlockEntity>(true);
		}

		protected void DispenseItem(Point3 point, int face, int value, MachineMode mode)
		{
			Vector3 vec = CellFace.FaceToVector3(face);
			var vec2 = new Vector3(point) + new Vector3(0.5f) + 0.6f * vec;
			if (mode != 0)
			{
				if (Utils.SubsystemProjectiles.FireProjectile(value, vec2, 1f * m_random.UniformFloat(39f, 41f) * (vec + m_random.Vector3(0.025f, false) + new Vector3(0f, 0.05f, 0f)), Vector3.Zero, null) != null)
					Utils.SubsystemAudio.PlaySound("Audio/DispenserShoot", 1f, 0f, vec2, 4f, true);
				else
					DispenseItem(point, face, value, MachineMode.Dispense);
			}
		}

		protected void Driller(Point3 point, int face)
		{
			Vector3 vector = CellFace.FaceToVector3(face);
			int x = point.X;
			int y = point.Y;
			int z = point.Z;
			int value = GetSlotValue(8);
			if (!(ComponentEngine.IsPowered(Utils.Terrain, x, y, z) || Utils.SubsystemGameInfo.WorldSettings.GameMode == 0) || BlocksManager.Blocks[Terrain.ExtractContents(value)].Durability <= 0)
				return;
			int[] array = new[]
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
			}, array2 = new[]
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
			Vector3 v = Vector3.Zero;
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
						v = new Vector3(0f, array[m] / (float)l, array2[m] / (float)l);
					}
					if (vector.Y != 0f)
					{
						x = point.X + array[m];
						z = point.Z + array2[m];
						v = new Vector3(array[m] / (float)l, 0f, array2[m] / (float)l);
					}
					if (vector.Z != 0f)
					{
						x = point.X + array[m];
						y = point.Y + array2[m];
						v = new Vector3(array[m] / (float)l, array2[m] / (float)l, 0f);
					}
					int cellValue = Terrain.ReplaceLight(Utils.Terrain.GetCellValue(x, y, z), 0);
					int content = Terrain.ExtractContents(cellValue);
					var block = BlocksManager.Blocks[content];
					if (content == 92)
						num2 = 9;
					if (cellValue != (TorchBlock.Index | 5 << 14) && block.IsPlaceable && !block.IsDiggingTransparent && !block.DefaultIsInteractive)
					{
						Utils.SubsystemTerrain.ChangeCell(x, y, z, 0);
						Utils.SubsystemProjectiles.FireProjectile(cellValue, new Vector3(x + 0.5f, y + 0.5f, z + 0.5f) - 0.25f * vector, 60f * (vector - v), Vector3.Zero, null);
						DrillType type = DrillBlock.GetType(value);
						RemoveSlotItems(8, 1);
						if (DrillBlock.GetType(BlocksManager.DamageItem(value, 1 + num2)) == type && BlocksManager.DamageItem(value, 1 + num2) != value && GetDamage(BlocksManager.DamageItem(value, 1 + num2)) != GetDamage(value))
						{
							AddSlotItems(8, BlocksManager.DamageItem(value, 1 + num2), 1);
						}
						return;
					}
				}
			}
		}

		public virtual int GetDamage(int value)
		{
			return (Terrain.ExtractData(value) >> 4) & 0xFFF;
		}

		public override int GetSlotCapacity(int slotIndex, int value)
		{
			if (slotIndex != 8)
				return base.GetSlotCapacity(slotIndex, value);
			if (Terrain.ExtractContents(value) != DrillBlock.Index)
				return 0;
			var type = DrillBlock.GetType(value);
			return (type == DrillType.DiamondDrill || type == DrillType.SteelDrill) ? base.GetSlotCapacity(slotIndex, value) : 0;
		}

		public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
		{
			this.SaveItems(valuesDictionary);
		}
	}
}