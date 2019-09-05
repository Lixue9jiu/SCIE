using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	public class ComponentUnloader : ComponentInventoryBase
	{
		protected ComponentBlockEntity m_componentBlockEntity;
		public SubsystemPlayers SubsystemPlayers;
		public bool DispenseItem = true;

		public bool Place()
		{
			Point3 coordinates = m_componentBlockEntity.Coordinates;
			int cellValue = Utils.Terrain.GetCellValue(coordinates.X, coordinates.Y, coordinates.Z);
			if (Terrain.ExtractContents(cellValue) != Bullet2Block.Index || (Terrain.ExtractData(cellValue) >> 10) == 0)
				return false;
			int num = 0;
			int value;
			while (true)
			{
				if (num >= SlotsCount - 1)
					return false;
				value = GetSlotValue(num);
				int slotCount = GetSlotCount(num);
				if (value != 0 && slotCount > 0)
					break;
				num++;
			}
			int face = FourDirectionalBlock.GetDirection(cellValue);
			for (num = RemoveSlotItems(num, 1); num-- > 0;)
			{
				var position = new Vector3(coordinates) + new Vector3(0.5f);
				Vector3 vector = CellFace.FaceToVector3(face);
				if (!Place(position + vector, face, value) && DispenseItem)
				{
					Vector3 vector2 = position + 0.6f * vector;
					Utils.SubsystemPickables.AddPickable(value, 1, vector2, 1.8f * (vector + m_random.Vector3(0.2f, false)), null);
					Utils.SubsystemAudio.PlaySound("Audio/DispenserDispense", 1f, 0f, new Vector3(vector2.X, vector2.Y, vector2.Z), 3f, true);
				}
			}
			return true;
		}

		public bool Place(Vector3 position, int face, int value)
		{
			var result = Utils.SubsystemTerrain.Raycast(position, position + CellFace.FaceToVector3(face) * 8f, true, true, null);
			var componentMiner = SubsystemPlayers.FindNearestPlayer(position).ComponentMiner;
			if (result.HasValue && componentMiner.Place(result.Value, value))
			{
				if (componentMiner.ComponentCreature.PlayerStats != null)
					componentMiner.ComponentCreature.PlayerStats.BlocksPlaced--;
				return true;
			}
			return false;
		}

		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.LoadItems(valuesDictionary);
			m_componentBlockEntity = Entity.FindComponent<ComponentBlockEntity>(true);
			SubsystemPlayers = Project.FindSubsystem<SubsystemPlayers>(true);
		}

		public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
		{
			this.SaveItems(valuesDictionary);
		}
	}

	public class ComponentInserter : ComponentInventoryBase
	{
		protected ComponentBlockEntity m_componentBlockEntity;
		public SubsystemPlayers SubsystemPlayers;
		public bool DispenseItem = true;

		public bool Place()
		{
			Point3 coordinates = m_componentBlockEntity.Coordinates;
			int cellValue = Utils.Terrain.GetCellValue(coordinates.X, coordinates.Y, coordinates.Z);
			if (Terrain.ExtractContents(cellValue) != Bullet2Block.Index || (Terrain.ExtractData(cellValue) >> 10) == 0)
				return false;
			//int num = 0;
			int value = 0;
			int slotCount = GetSlotCount(0);
			if (slotCount > 0)
				value = GetSlotValue(0);
			int face = FourDirectionalBlock.GetDirection(cellValue);
			Vector3 vector = CellFace.FaceToVector3(face);
			var position = new Vector3(coordinates) + new Vector3(0.5f);
			Point3 coor2 = coordinates - new Point3((int)vector.X, (int)vector.Y, (int)vector.Z);
			var entity = Utils.GetBlockEntity(coor2);
			if (entity == null)
				return false;
			ComponentInventoryBase component = entity.Entity.FindComponent<ComponentInventoryBase>();
			if (component != null)
			{
				IInventory inventory = entity.Entity.FindComponent<ComponentInventoryBase>();
				for (int i = 0; i < inventory.SlotsCount; i++)
				{
					if (inventory.GetSlotCount(i) > 0)
					{
						if (value != 0)
						{
							if (inventory.GetSlotValue(i) == value)
							{
								Vector3 vector2 = position + 0.6f * vector;
								Utils.SubsystemPickables.AddPickable(value, 1, vector2, 1.8f * (vector + m_random.Vector3(0.2f, false)), null);
								inventory.RemoveSlotItems(i, 1);
								return true;
							}
						}
						else
						{//v == 262384 || v == 786672 || v == 1048816 || v == 1310960 || v == WaterBlock.Index || v==MagmaBlock.Index
							int v = inventory.GetSlotValue(i);
							if (v == 262384 || v == 786672 || v == 1048816 || v == 1310960 || v == WaterBlock.Index || v == MagmaBlock.Index)
							{
								Point3 coor3 = coordinates + new Point3((int)vector.X, (int)vector.Y, (int)vector.Z);
								var entity2 = Utils.GetBlockEntity(coor3);
								if (entity2!=null)
								{
									ComponentMachine component2 = entity2.Entity.FindComponent<ComponentMachine>();
									if (component2!=null)
									{
										IInventory inventory2 = entity2.Entity.FindComponent<ComponentInventoryBase>();
										if (ComponentInventoryBase.AcquireItems(inventory2, v, 1) < 1)
										{
											//ComponentInventoryBase.
											ComponentMachine component3 = entity.Entity.FindComponent<ComponentMachine>();
											component3.RemoveSlotItems2(i,1);
											//inventory.re;
										}
									}
								}
								return false;
							}
							else
							{
								Vector3 vector2 = position + 0.6f * vector;
								Utils.SubsystemPickables.AddPickable(inventory.GetSlotValue(i), 1, vector2, 1.8f * (vector + m_random.Vector3(0.2f, false)), null);
								inventory.RemoveSlotItems(i, 1);
								return true;
							}
							
						}
					}
				}
			}
			return false;
		}

		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.LoadItems(valuesDictionary);
			m_componentBlockEntity = Entity.FindComponent<ComponentBlockEntity>(true);
			//SubsystemPlayers = Project.FindSubsystem<SubsystemPlayers>(true);
		}

		public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
		{
			this.SaveItems(valuesDictionary);
		}
	}
}