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

	public class MachineElectricElement2 : ElectricElement
	{
		public float m_voltage;
		public Point3 Point;
		public MachineElectricElement2(SubsystemElectricity subsystemElectricity, Point3 point)
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
			m_voltage = CalculateVoltage();
			Point = point;
		}

		public override float GetOutputVoltage(int face)
		{
			return m_voltage;
		}

		public override bool Simulate()
		{
			float voltage = m_voltage;
			m_voltage = CalculateVoltage();
			base.SubsystemElectricity.QueueElectricElementForSimulation(this, base.SubsystemElectricity.CircuitStep + MathUtils.Max(50, 1));
			return m_voltage != voltage;
		}

		public float CalculateVoltage()
		{
			bool flag=false;
			ComponentRadioR laserg2 = Utils.GetBlockEntity(Point)?.Entity.FindComponent<ComponentRadioR>();
			if (laserg2 != null)
			{
				flag = laserg2.Active();
			}
			return flag ? 15f : 0f;
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
				ComponentRadioC laserg2 = Utils.GetBlockEntity(Point)?.Entity.FindComponent<ComponentRadioC>();
				if (laserg2 != null)
				{
					m_lastDispenseTime = SubsystemElectricity.SubsystemTime.GameTime;
					laserg2.Active();
					return true;
				}
			}
			if (n > 7 && SubsystemElectricity.SubsystemTime.GameTime - m_lastDispenseTime > 0.1)
			{
				ComponentLaserG laserg = Utils.GetBlockEntity(Point)?.Entity.FindComponent<ComponentLaserG>();
				if (laserg !=null && SubsystemElectricity.SubsystemTime.GameTime - m_lastDispenseTime > 1.0)
				{
					m_lastDispenseTime = SubsystemElectricity.SubsystemTime.GameTime;
					laserg.Beam();
					return true;
				}
				
				//m_lastDispenseTime = SubsystemElectricity.SubsystemTime.GameTime;
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
					ComponentCondenser laserg = Utils.GetBlockEntity(Point)?.Entity.FindComponent<ComponentCondenser>();
					if (laserg != null && SubsystemElectricity.SubsystemTime.GameTime - m_lastDispenseTime > 1.0)
					{
						m_lastDispenseTime = SubsystemElectricity.SubsystemTime.GameTime;
						laserg.Charged = !laserg.Charged;
						//return true;
					}
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
			
			//Utils.SubsystemPlayersm_componentPlayer.ComponentGui.TemperatureBarWidget.Flash(10);
			if (Bomb is NBomb)
			{
				Explode(x, y, z, 3e3f, 8);
				var e = Utils.SubsystemBodies.Bodies.GetEnumerator();
				Utils.SubsystemParticles.AddParticleSystem(new NParticleSystem2(new Vector3(x, y, z), 6f, 400f));
				Explode2(x, y, z, 6, 30);
				while (e.MoveNext())
				{

					var entity = e.Current.Entity;
				//	if (Vector3.Distance(entity.FindComponent<ComponentBody>().m_position, new Vector3(x, y, z))<30f)
					entity.FindComponent<ComponentHealth>()?.Injure(1f * Vector3.Distance(entity.FindComponent<ComponentBody>().m_position, new Vector3(x, y, z))*40f, null, false, "Killed by radiation");
					if (entity.FindComponent<ComponentPlayer>() != null)
						entity.FindComponent<ComponentPlayer>().ComponentGui.TemperatureBarWidget.Flash(10);
				}
				if (!(Bomb is HABomb)) return false;
			}
			Explode(x, y, z, 3e2f, 8);
			Explode2(x, y, z, (int)pressure / 100, (int)Bomb.GetFuelFireDuration(0) / 4);
			var e1 = Utils.SubsystemBodies.Bodies.GetEnumerator();
			while (e1.MoveNext())
			{
				var entity = e1.Current.Entity;
				entity.FindComponent<ComponentHealth>()?.Injure(Bomb.GetFuelFireDuration(0) / 4f / entity.FindComponent<ComponentHealth>().AttackResilience  *Vector3.Distance(entity.FindComponent<ComponentBody>().m_position, new Vector3(x, y, z)) * (pressure/100f), null, false, "Killed by radiation");
				if (entity.FindComponent<ComponentPlayer>() != null)
					entity.FindComponent<ComponentPlayer>().ComponentGui.TemperatureBarWidget.Flash(10);
			}
			if (Bomb is HBomb)
			{
				Utils.SubsystemParticles.AddParticleSystem(new NParticleSystem2(new Vector3(x, y, z), 40f, 400f));
				return false;
			}
			if (Bomb is HABomb)
			{
				Utils.SubsystemParticles.AddParticleSystem(new NParticleSystem2(new Vector3(x, y, z), 60f, 400f));
				return false;
			}
			if (Bomb is ABomb)
			{
				//Utils.SubsystemParticles.AddParticleSystem(new NParticleSystem(new Vector3(x, y, z), 10f, 400f));
				Utils.SubsystemParticles.AddParticleSystem(new NParticleSystem2(new Vector3(x, y, z), 10f, 400f));
			}
			Utils.SubsystemSour.m_radations.Add(new Vector4(x, y, z, 10000f));
			//int r = (int)Bomb.GetFuelFireDuration(0);
			//Explode(x, y + (r >> 1), z, pressure, r);
			//pressure *= 1.5f;
			//Explode(x, y - (r >> 1), z, pressure * 1.5f, r);
			//Explode(x + r, y, z, pressure, r);
			//Explode(x - r, y, z, pressure, r);
			//Explode(x, y, z + r, pressure, r);
			//Explode(x, y, z - r, pressure, r);

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

		public void Explode2(int x, int y, int z, int r, int level=1)
		{
			var se = Utils.SubsystemTerrain;
			for (int x1=-r;x1<r+1;x1++)
				for (int y1 = -r/6; y1 < r/6 + 1; y1++)
					for (int z1 = -r; z1 < r + 1; z1++)
						if (Utils.Random.Bool(60f*level*level/Vector3.DistanceSquared(new Vector3(0,0,0),new Vector3(x1,y1*6,z1))) && y+y1>1 && Vector3.DistanceSquared(new Vector3(0, 0, 0), new Vector3(x1, y1*6, z1))<=r*r)
								se.ChangeCell(x + x1, y + y1, z + z1, 0,true);
							//se.Terrain.SetCellValueFast(x + x1, y + y1, z + z1, 0);
			//	se.DestroyCell(3,x+x1,y+y1,z+z1,0,true,true);

		}
	}
}