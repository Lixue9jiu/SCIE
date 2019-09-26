using Engine;
using System.Collections.Generic;
using static Game.Charger;

namespace Game
{
	public class DrillerElectricElement : ElectricElement
	{
		protected bool m_isDispenseAllowed = true;

		protected double? m_lastDispenseTime;

		public DrillerElectricElement(SubsystemElectricity subsystemElectricity, Point3 point)
			: base(subsystemElectricity, new List<CellFace>
			{
				new CellFace(point.X, point.Y, point.Z, 0),
				new CellFace(point.X, point.Y, point.Z, 1),
				new CellFace(point.X, point.Y, point.Z, 2),
				new CellFace(point.X, point.Y, point.Z, 3),
				new CellFace(point.X, point.Y, point.Z, 4),
				new CellFace(point.X, point.Y, point.Z, 5)
			})
		{
		}

		public override bool Simulate()
		{
			if (CalculateHighInputsCount() > 0)
			{
				if (m_isDispenseAllowed && (!m_lastDispenseTime.HasValue || SubsystemElectricity.SubsystemTime.GameTime - m_lastDispenseTime > 0.1))
				{
					m_isDispenseAllowed = false;
					m_lastDispenseTime = SubsystemElectricity.SubsystemTime.GameTime;
					CellFace cellFace = CellFaces[0];
					int x = cellFace.Point.X;
					int y = cellFace.Point.Y;
					int z = cellFace.Point.Z;
					ComponentBlockEntity blockEntity = Utils.SubsystemBlockEntities.GetBlockEntity(x, y, z);
					if (blockEntity != null)
					{
						blockEntity.Entity.FindComponent<ComponentDriller>()?.Dispense();
					}
				}
			}
			else
				m_isDispenseAllowed = true;
			return false;
		}
	}

	public class MachineElectricElement : ElectricElement
	{
		public Point3 Point;
		protected ICraftingMachine Inventory;
		protected double m_lastDispenseTime = double.NegativeInfinity;

		public MachineElectricElement(SubsystemElectricity subsystemElectricity, Point3 point)
			: base(subsystemElectricity, new List<CellFace>
		{
			new CellFace(point.X, point.Y, point.Z, 0),
			new CellFace(point.X, point.Y, point.Z, 1),
			new CellFace(point.X, point.Y, point.Z, 2),
			new CellFace(point.X, point.Y, point.Z, 3),
			new CellFace(point.X, point.Y, point.Z, 4),
			new CellFace(point.X, point.Y, point.Z, 5)
		})
		{
			Point = point;
		}
		public override bool Simulate()
		{
			int n = 0;
			for (int i = 0; i < Connections.Count; i++)
			{
				var connection = Connections[i];
				if (connection.ConnectorType != ElectricConnectorType.Output && connection.NeighborConnectorType != 0)
				{
					n = MathUtils.Max(n, (int)MathUtils.Round(connection.NeighborElectricElement.GetOutputVoltage(connection.NeighborConnectorFace) * 15f));
					if (n > 7) break;
				}
			}
			if (n > 7 && SubsystemElectricity.SubsystemTime.GameTime - m_lastDispenseTime > 0.1)
			{
				ComponentLaserG laserg = Utils.GetBlockEntity(Point)?.Entity.FindComponent<ComponentLaserG>();
				if (laserg !=null)
				{
					laserg.Beam();
					return true;
				}
				var inventory = Inventory;
				if (inventory == null)
					inventory = Utils.GetBlockEntity(Point)?.Entity.FindComponent<ICraftingMachine>();
				if (inventory == null)
					return false;
				var position = new Vector3(Point) + new Vector3(0.5f, 1f, 0.5f);
				if (inventory.GetSlotCount(inventory.ResultSlotIndex) > 0 && inventory.GetSlotValue(inventory.ResultSlotIndex) != 0)
				Utils.SubsystemPickables.AddPickable(inventory.GetSlotValue(inventory.ResultSlotIndex), inventory.RemoveSlotItems(inventory.ResultSlotIndex, 1), position, null, null);
				if (inventory.GetSlotCount(inventory.RemainsSlotIndex) > 0 && inventory.GetSlotValue(inventory.RemainsSlotIndex)!=0)
					Utils.SubsystemPickables.AddPickable(inventory.GetSlotValue(inventory.RemainsSlotIndex), inventory.RemoveSlotItems(inventory.RemainsSlotIndex, 1), position, null, null);
			}
			return false;
		}
	}

	public class CraftingMachineElectricElement : ElectricElement
	{
		public Point3 Point;
		protected ICraftingMachine Inventory;
		protected double m_lastDispenseTime = double.NegativeInfinity;

		public CraftingMachineElectricElement(SubsystemElectricity subsystemElectricity, Point3 point)
			: base(subsystemElectricity, new List<CellFace>
			{
				new CellFace(point.X, point.Y, point.Z, 4),
				new CellFace(point.X, point.Y, point.Z, 5)
			})
		{
			Point = point;
		}

		public override bool Simulate()
		{
			for (int i = 0; i < Connections.Count; i++)
			{
				var connection = Connections[i];
				if (connection.ConnectorType != ElectricConnectorType.Output && connection.NeighborConnectorType != 0)
				{
					ICraftingMachine inventory = Inventory;
					if (inventory == null)
					{
						Inventory = inventory = Utils.GetBlockEntity(Point)?.Entity.FindComponent<ICraftingMachine>();
						if (inventory == null)
							return false;
					}
					int n = (int)MathUtils.Round(connection.NeighborElectricElement.GetOutputVoltage(connection.NeighborConnectorFace) * 15f);
					if (connection.CellFace.Face == 4)
						inventory.SlotIndex = n;
					else if (n > 0 && (SubsystemElectricity.SubsystemTime.GameTime - m_lastDispenseTime > 0.1) && connection.CellFace.Face == 5)
					{
						m_lastDispenseTime = SubsystemElectricity.SubsystemTime.GameTime;
						var matchedRecipe = inventory.GetRecipe() ?? new CraftingRecipe
						{
							ResultCount = 1,
							RemainsCount = 1
						};
						var position = new Vector3(Point) + new Vector3(0.5f, 1f, 0.5f);
						if (inventory.GetSlotCount(inventory.ResultSlotIndex) > 0 && inventory.GetSlotValue(inventory.ResultSlotIndex) != 0)
						Utils.SubsystemPickables.AddPickable(inventory.GetSlotValue(inventory.ResultSlotIndex), inventory.RemoveSlotItems(inventory.ResultSlotIndex, 1), position, null, null);
						if (matchedRecipe.RemainsCount > 0 && inventory.GetSlotValue(inventory.RemainsSlotIndex)!=0)
							Utils.SubsystemPickables.AddPickable(inventory.GetSlotValue(inventory.RemainsSlotIndex), inventory.RemoveSlotItems(inventory.RemainsSlotIndex, 1), position, null, null);
					}
				}
			}
			return false;
		}
	}

	public class RelayElectricElement : ElectricElement
	{
		public Relay Relay;
		public float m_voltage;
		protected double m_lastDispenseTime = double.NegativeInfinity;

		public RelayElectricElement(SubsystemElectricity subsystemElectricity, Point3 point)
			: base(subsystemElectricity, new List<CellFace>
			{
				new CellFace(point.X, point.Y, point.Z, 4),
				new CellFace(point.X, point.Y, point.Z, 5)
			})
		{
			Relay = ElementBlock.Block.GetDevice(point.X, point.Y, point.Z, Utils.Terrain.GetCellValueFast(point.X, point.Y, point.Z)) as Relay;
		}
		public override bool Simulate()
		{
			if (Relay == null)
				return false;
			float voltage = m_voltage;
			m_voltage = CalculateHighInputsCount() > 0 ? 1 : 0;
			if (IsSignalHigh(m_voltage) != IsSignalHigh(voltage))
			{
				Relay.Powered = IsSignalHigh(m_voltage);
			}
			return false;
		}
	}

	public class ChargerElectricElement : ElectricElement
	{
		public bool m_isActionAllowed;
		public double m_lastActionTime = -1e3;
		public float m_voltage;
		protected double m_lastDispenseTime = double.NegativeInfinity;
		public Point3 Point;

		public ChargerElectricElement(SubsystemElectricity subsystemElectricity, Point3 point)
			: base(subsystemElectricity, new List<CellFace>
			{
				new CellFace(point.X, point.Y, point.Z, 4),
				new CellFace(point.X, point.Y, point.Z, 5)
			})
		{
			Point = point;
		}

		public override bool Simulate()
		{
			if (CalculateHighInputsCount() > 0)
			{
				if (m_isActionAllowed && SubsystemElectricity.SubsystemTime.GameTime - m_lastActionTime > 0.1)
				{
					m_isActionAllowed = false;
					m_lastActionTime = SubsystemElectricity.SubsystemTime.GameTime;
					int value = Utils.Terrain.GetCellValueFast(Point.X, Point.Y, Point.Z),
						data = SetMode(Terrain.ExtractData(value));
					Utils.Terrain.SetCellValueFast(Point.X, Point.Y, Point.Z, Terrain.ReplaceData(value, data));
				}
			}
			else
				m_isActionAllowed = true;
			return false;
		}
	}

	public class ABombElectricElement : ElectricElement
	{
		readonly IFuel Bomb;
		public ABombElectricElement(IFuel fuel, SubsystemElectricity subsystemElectricity, CellFace cellFace) : base(subsystemElectricity, cellFace) { Bomb = fuel; }

		public override bool Simulate()
		{
			if (CalculateHighInputsCount() <= 0)
				return false;
			var cellFace = CellFaces[0];
			int x = cellFace.X, y = cellFace.Y, z = cellFace.Z;
			float pressure = Bomb.GetHeatLevel(0);
			if (Bomb is NBomb)
			{
				Explode(x, y, z, 3e3f, 8);
				var e = Utils.SubsystemBodies.Bodies.GetEnumerator();
				while (e.MoveNext())
				{
					var entity = e.Current.Entity;
					entity.FindComponent<ComponentHealth>()?.Injure(1f, null, false, "Killed by radiation");
				}
				if (!(Bomb is HABomb)) return false;
			}
			int r = (int)Bomb.GetFuelFireDuration(0);
			Explode(x, y + (r >> 1), z, pressure, r);
			pressure *= 1.5f;
			Explode(x, y - (r >> 1), z, pressure * 1.5f, r);
			Explode(x + r, y, z, pressure, r);
			Explode(x - r, y, z, pressure, r);
			Explode(x, y, z + r, pressure, r);
			Explode(x, y, z - r, pressure, r);
			return false;
		}

		public static void Explode(int x, int y, int z, float pressure, int r)
		{
			var se = Utils.SubsystemExplosions;
			r >>= 1;
			//se.AddExplosion(x, y + r, z, pressure, false, false);
			//se.AddExplosion(x, y - r, z, pressure, false, false);
			se.AddExplosion(x + r, y, z, pressure, false, false);
			se.AddExplosion(x - r, y, z, pressure, false, false);
			se.AddExplosion(x, y, z + r, pressure, false, false);
			se.AddExplosion(x, y, z - r, pressure, false, false);
		}
	}
}