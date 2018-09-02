using Engine;
using System.Collections.Generic;

namespace Game
{
	public class MachineToolBlock : CubeBlock, IElectricElementBlock
	{
		public const int Index = 508;

		public override int GetFaceTextureSlot(int face, int value)
		{
			if (face == 4)
			{
				return 146;
			}
			return 107;
		}

		public ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			return new MachineToolElectricElement(subsystemElectricity, new Point3(x, y, z));
		}

		public ElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z)
		{
			if (face == 4 || face == 5)
			{
				return ElectricConnectorType.Input;
			}
			return null;
		}

		public int GetConnectionMask(int value)
		{
			return 2147483647;
		}
	}
	public class MachineToolElectricElement : ElectricElement
	{
		public SubsystemBlockEntities SubsystemBlockEntities;
		public SubsystemPickables SubsystemPickables;
		public Point3 Point;
		protected ComponentLargeCraftingTable Inventory;
		protected double m_lastDispenseTime = double.NegativeInfinity;
		public MachineToolElectricElement(SubsystemElectricity subsystemElectricity, Point3 point)
			: base(subsystemElectricity, new List<CellFace>
			{
				new CellFace(point.X, point.Y, point.Z, 4),
				new CellFace(point.X, point.Y, point.Z, 5)
			})
		{
			Point = point;
			SubsystemBlockEntities = SubsystemElectricity.Project.FindSubsystem<SubsystemBlockEntities>(true);
			SubsystemPickables = SubsystemElectricity.Project.FindSubsystem<SubsystemPickables>(true);
		}

		public override bool Simulate()
		{
			for (int i = 0; i < Connections.Count; i++)
			{
				var connection = Connections[i];
				if (connection.ConnectorType != ElectricConnectorType.Output && connection.NeighborConnectorType != 0)
				{
					ComponentLargeCraftingTable inventory = Inventory;
					if (inventory == null)
					{
						Inventory = inventory = SubsystemBlockEntities.GetBlockEntity(Point.X, Point.Y, Point.Z).Entity.FindComponent<ComponentLargeCraftingTable>(true);
					}
					int n = (int)MathUtils.Round(connection.NeighborElectricElement.GetOutputVoltage(connection.NeighborConnectorFace) * 15f);
					if (connection.CellFace.Face == 4)
						inventory.SlotIndex = n;
					else if (n > 0 && (SubsystemElectricity.SubsystemTime.GameTime - m_lastDispenseTime > 0.1) && connection.CellFace.Face == 5)
					{
						m_lastDispenseTime = SubsystemElectricity.SubsystemTime.GameTime;
						CraftingRecipe m_matchedRecipe = inventory.m_matchedRecipe;
						if (m_matchedRecipe == null)
						{
							return false;
						}
						SubsystemPickables.AddPickable(inventory.GetSlotValue(inventory.ResultSlotIndex), inventory.RemoveSlotItems(inventory.ResultSlotIndex, m_matchedRecipe.ResultCount * MathUtils.Min(inventory.GetSlotCount(inventory.ResultSlotIndex) / m_matchedRecipe.ResultCount, n)), new Vector3(Point) + new Vector3(0.5f, 1f, 0.5f), null, null);
						if (m_matchedRecipe.RemainsCount > 0)
						{
							SubsystemPickables.AddPickable(inventory.GetSlotValue(inventory.RemainsSlotIndex), inventory.RemoveSlotItems(inventory.RemainsSlotIndex, m_matchedRecipe.RemainsCount * MathUtils.Min(inventory.GetSlotCount(inventory.RemainsSlotIndex) / m_matchedRecipe.RemainsCount, n)), new Vector3(Point) + new Vector3(0.5f, 1f, 0.5f), null, null);
						}
					}
				}
			}
			return false;
		}
	}
}
