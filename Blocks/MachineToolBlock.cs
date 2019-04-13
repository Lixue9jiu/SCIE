using Engine;
using System.Collections.Generic;

namespace Game
{
	public class MachineToolBlock : PaintedCubeBlock, IElectricElementBlock
	{
		public const int Index = 508;

		public MachineToolBlock() : base(0)
		{
		}

		public override int GetFaceTextureSlot(int face, int value)
		{
			return face == 4 ? 146 : 107;
		}

		public ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			return new CraftingMachineElectricElement(subsystemElectricity, new Point3(x, y, z));
		}

		public ElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z)
		{
			return face == 4 || face == 5 ? (ElectricConnectorType?)ElectricConnectorType.Input : null;
		}

		public int GetConnectionMask(int value)
		{
			return 2147483647;
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
						Utils.SubsystemPickables.AddPickable(inventory.GetSlotValue(inventory.ResultSlotIndex), inventory.RemoveSlotItems(inventory.ResultSlotIndex, matchedRecipe.ResultCount * MathUtils.Min(inventory.GetSlotCount(inventory.ResultSlotIndex) / matchedRecipe.ResultCount, n)), position, null, null);
						if (matchedRecipe.RemainsCount > 0)
							Utils.SubsystemPickables.AddPickable(inventory.GetSlotValue(inventory.RemainsSlotIndex), inventory.RemoveSlotItems(inventory.RemainsSlotIndex, matchedRecipe.RemainsCount * MathUtils.Min(inventory.GetSlotCount(inventory.RemainsSlotIndex) / matchedRecipe.RemainsCount, n)), position, null, null);
					}
				}
			}
			return false;
		}
	}
}