using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	public class ComponentCondenser : Component
	{
		public bool Powered, Charged;
		public float m_fireTimeRemaining;

		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			m_fireTimeRemaining = valuesDictionary.GetValue("FireTimeRemaining", 0f);
			Charged = valuesDictionary.GetValue("HeatLevel", 0f) == 0f ? true : false;
		}

		public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
		{
			valuesDictionary.SetValue("FireTimeRemaining", m_fireTimeRemaining);
			valuesDictionary.SetValue("HeatLevel", Charged ? 0f : 1f);
		}
	}

	public class ComponentRControl : ComponentMachine, IUpdateable
	{
		public bool Powered, Charged;

		//public float m_fireTimeRemaining;
		//public int UpdateOrder => 0;
		//protected ComponentBlockEntity m_componentBlockEntity;
		public float output;

		public int fuel;
		public int carbon;
		public int control;
		public int rfuel;
		public int rcarbon;
		public int rcontrol;
		public float temp;
		public bool fr1;
		public bool cr1;
		public bool gr1;
		public bool fr0;
		public bool cr0;
		public bool gr0;
		public bool az5;

		public void Update(float dt)
		{
			if (Utils.SubsystemTime.PeriodicGameTimeEvent(0.2, 0.0))
			{
				Point3 coordinates = m_componentBlockEntity.Coordinates;
				var point = CellFace.FaceToPoint3(Terrain.ExtractData(Utils.Terrain.GetCellValue(coordinates.X, coordinates.Y, coordinates.Z)) >> 15);
				int num3 = coordinates.X - point.X;
				int num4 = coordinates.Y - point.Y;
				int num5 = coordinates.Z - point.Z;
				int cellValue = Terrain.ReplaceLight(Utils.Terrain.GetCellValue(num3, num4, num5), 0);
				//int cellContents = Terrain.ExtractContents(cellValue);Terrain.ExtractData(value) >> 15
				fuel = 0;
				control = 0;
				carbon = 0;
				rfuel = 0;
				rcontrol = 0;
				rcarbon = 0;
				if (4 == Terrain.ExtractData(cellValue) >> 10)
				{
					var point3 = new Point3(num3, num4, num5);
					var entity = Utils.GetBlockEntity(point3);
					ComponentRCore component3 = entity.Entity.FindComponent<ComponentRCore>();
					if (entity != null && component3 != null)
					{
						temp = component3.HeatLevel;
						output = component3.m_fireTimeRemaining;
						IInventory inventory = entity.Entity.FindComponent<ComponentRCore>(true);
						for (int i = 0; i < inventory.SlotsCount; i++)
						{
							//if ()
							int va1 = inventory.GetSlotValue(i);
							if (Terrain.ExtractContents(va1) == FuelRodBlock.Index)
							{
								if (FuelRodBlock.GetType(va1) == RodType.UFuelRod)
								{
									fuel++;
								}
								if (FuelRodBlock.GetType(va1) == RodType.ControlRod)
								{
									control++;
								}
								if (FuelRodBlock.GetType(va1) == RodType.CarbonRod)
								{
									carbon++;
								}
							}
						}
					}
				}
				int cellValue2 = Terrain.ReplaceLight(Utils.Terrain.GetCellValue(num3, num4 + 1, num5), 0);
				//int cellContents = Terrain.ExtractContents(cellValue);
				if (5 == Terrain.ExtractData(cellValue2) >> 10)
				{
					var point3 = new Point3(num3, num4 + 1, num5);
					var entity = Utils.GetBlockEntity(point3);
					IInventory inventory = entity?.Entity.FindComponent<ComponentSorter>(true);
					if (inventory != null)
					{
						for (int i = 0; i < inventory.SlotsCount; i++)
						{
							int va1 = inventory.GetSlotValue(i);
							if (Terrain.ExtractContents(va1) == FuelRodBlock.Index)
							{
								if (FuelRodBlock.GetType(va1) == RodType.UFuelRod)
								{
									rfuel++;
								}
								if (FuelRodBlock.GetType(va1) == RodType.ControlRod)
								{
									rcontrol++;
								}
								if (FuelRodBlock.GetType(va1) == RodType.CarbonRod)
								{
									rcarbon++;
								}
							}
						}
					}
				}
			}
			if (fr1 || cr1 || gr1)
			{
				Point3 coordinates = m_componentBlockEntity.Coordinates;
				var point = CellFace.FaceToPoint3(Terrain.ExtractData(Utils.Terrain.GetCellValue(coordinates.X, coordinates.Y, coordinates.Z)) >> 15);
				int num3 = coordinates.X - point.X;
				int num4 = coordinates.Y - point.Y;
				int num5 = coordinates.Z - point.Z;
				int cellValue = Terrain.ReplaceLight(Utils.Terrain.GetCellValue(num3, num4, num5), 0);
				bool flag12 = false;
				//int cellContents = Terrain.ExtractContents(cellValue);Terrain.ExtractData(value) >> 15
				if (4 == Terrain.ExtractData(cellValue) >> 10)
				{
					var point3 = new Point3(num3, num4, num5);
					var entity = Utils.GetBlockEntity(point3);
					ComponentRCore component3 = entity.Entity.FindComponent<ComponentRCore>();
					if (entity != null && component3 != null)
					{
						temp = component3.HeatLevel;
						output = component3.m_fireTimeRemaining;
						IInventory inventory = entity.Entity.FindComponent<ComponentRCore>(true);
						//inventory.
						for (int i = 0; i < inventory.SlotsCount; i++)
						{
							//if ()
							int va1 = inventory.GetSlotValue(i);
							if (va1 == 0)
							{
								//inventory.AddSlotItems(i, va1, 1);
								flag12 = true;
								i += inventory.SlotsCount;
							}
						}
					}
				}
				if (!flag12)
				{
					fr1 = false;
					cr1 = false;
					gr1 = false;
					return;
				}

				int cellValue2 = Terrain.ReplaceLight(Utils.Terrain.GetCellValue(num3, num4 + 1, num5), 0);
				//int cellContents = Terrain.ExtractContents(cellValue);
				int vv = 0;
				if (5 == Terrain.ExtractData(cellValue2) >> 10)
				{
					var point3 = new Point3(num3, num4 + 1, num5);
					var entity = Utils.GetBlockEntity(point3);
					IInventory inventory = entity?.Entity.FindComponent<ComponentSorter>(true);
					if (inventory != null)
					{
						for (int i = 0; i < inventory.SlotsCount; i++)
						{
							int va1 = inventory.GetSlotValue(i);
							if (Terrain.ExtractContents(va1) == FuelRodBlock.Index)
							{
								if (FuelRodBlock.GetType(va1) == RodType.UFuelRod && fr1)
								{
									//inventory.AddSlotItems(i, va1, -1);
									vv = va1;
									inventory.RemoveSlotItems(i, 1);
									break;
								}
								if (FuelRodBlock.GetType(va1) == RodType.ControlRod && cr1)
								{
									//inventory.AddSlotItems(i, va1, -1);
									vv = va1;
									inventory.RemoveSlotItems(i, 1);
									break;
								}
								if (FuelRodBlock.GetType(va1) == RodType.CarbonRod && gr1)
								{
									//inventory.AddSlotItems(i, va1, -1);
									vv = va1;
									inventory.RemoveSlotItems(i, 1);
									break;
								}
							}
						}
					}
				}
				if (vv == 0)
				{
					return;
				}
				if (4 == Terrain.ExtractData(cellValue) >> 10)
				{
					var point3 = new Point3(num3, num4, num5);
					var entity = Utils.GetBlockEntity(point3);
					ComponentRCore component3 = entity.Entity.FindComponent<ComponentRCore>();
					if (entity != null && component3 != null)
					{
						//temp = component3.HeatLevel;
						//output = component3.m_fireTimeRemaining;
						IInventory inventory = entity.Entity.FindComponent<ComponentRCore>(true);
						//inventory.
						for (int i = 0; i < inventory.SlotsCount; i++)
						{
							//if ()
							int va1 = inventory.GetSlotValue(i);
							if (va1 == 0)
							{
								inventory.AddSlotItems(i, vv, 1);
								//flag12 = true;
								i += inventory.SlotsCount;
							}
						}
					}
				}
				fr1 = false;
				cr1 = false;
				gr1 = false;
			}
			if (fr0 || cr0 || gr0)
			{
				Point3 coordinates = m_componentBlockEntity.Coordinates;
				var point = CellFace.FaceToPoint3(Terrain.ExtractData(Utils.Terrain.GetCellValue(coordinates.X, coordinates.Y, coordinates.Z)) >> 15);
				int num3 = coordinates.X - point.X;
				int num4 = coordinates.Y - point.Y;
				int num5 = coordinates.Z - point.Z;
				int cellValue = Terrain.ReplaceLight(Utils.Terrain.GetCellValue(num3, num4 + 1, num5), 0);
				bool flag12 = false;
				//int cellContents = Terrain.ExtractContents(cellValue);Terrain.ExtractData(value) >> 15
				if (5 == Terrain.ExtractData(cellValue) >> 10)
				{
					var point3 = new Point3(num3, num4 + 1, num5);
					var entity = Utils.GetBlockEntity(point3);
					IInventory inventory = entity?.Entity.FindComponent<ComponentSorter>(true);
					if (inventory != null)
					{
						for (int i = 0; i < inventory.SlotsCount; i++)
						{
							int va1 = inventory.GetSlotValue(i);
							if (va1 == 0)
							{
								//inventory.AddSlotItems(i, va1, 1);
								flag12 = true;
								i += inventory.SlotsCount;
							}
						}
					}
				}
				if (!flag12)
				{
					fr0 = false;
					cr0 = false;
					gr0 = false;
					return;
				}

				int cellValue2 = Terrain.ReplaceLight(Utils.Terrain.GetCellValue(num3, num4, num5), 0);
				//int cellContents = Terrain.ExtractContents(cellValue);
				int vv = 0;
				if (4 == Terrain.ExtractData(cellValue2) >> 10)
				{
					var point3 = new Point3(num3, num4, num5);
					var entity = Utils.GetBlockEntity(point3);
					ComponentRCore component3 = entity.Entity.FindComponent<ComponentRCore>();
					if (entity != null && component3 != null)
					{
						IInventory inventory = entity.Entity.FindComponent<ComponentRCore>(true);
						for (int i = 0; i < inventory.SlotsCount; i++)
						{
							int va1 = inventory.GetSlotValue(i);
							if (Terrain.ExtractContents(va1) == FuelRodBlock.Index)
							{
								if (FuelRodBlock.GetType(va1) == RodType.UFuelRod && fr0)
								{
									//inventory.AddSlotItems(i, va1, -1);
									vv = va1;
									inventory.RemoveSlotItems(i, 1);
									break;
								}
								if (FuelRodBlock.GetType(va1) == RodType.ControlRod && cr0)
								{
									//inventory.AddSlotItems(i, va1, -1);
									vv = va1;
									inventory.RemoveSlotItems(i, 1);
									break;
								}
								if (FuelRodBlock.GetType(va1) == RodType.CarbonRod && gr0)
								{
									//inventory.AddSlotItems(i, va1, -1);
									vv = va1;
									inventory.RemoveSlotItems(i, 1);
									break;
								}
							}
						}
					}
				}
				if (vv == 0)
				{
					return;
				}

				if (5 == Terrain.ExtractData(cellValue) >> 10)
				{
					var point3 = new Point3(num3, num4 + 1, num5);
					var entity = Utils.GetBlockEntity(point3);
					ComponentSorter componentSorter = entity?.Entity.FindComponent<ComponentSorter>();
					if (componentSorter != null)
					{
						IInventory inventory = componentSorter;
						//inventory.
						for (int i = 0; i < inventory.SlotsCount; i++)
						{
							int va1 = inventory.GetSlotValue(i);
							if (va1 == 0)
							{
								inventory.AddSlotItems(i, vv, 1);
								//flag12 = true;
								i += inventory.SlotsCount;
							}
						}
					}
				}
				fr0 = false;
				cr0 = false;
				gr0 = false;
			}
			if (az5 && rcontrol > 0)
			{
				cr1 = true;
			}
			else
			{
				az5 = false;
			}
		}

		//public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		//{
		//	m_fireTimeRemaining = valuesDictionary.GetValue("FireTimeRemaining", 0f);
		//	Charged = valuesDictionary.GetValue("HeatLevel", 0f) == 0f ? true : false;
		//}

		//public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
		//{
		//	valuesDictionary.SetValue("FireTimeRemaining", m_fireTimeRemaining);
		//	valuesDictionary.SetValue("HeatLevel", Charged ? 0f : 1f);
		//}
	}
}