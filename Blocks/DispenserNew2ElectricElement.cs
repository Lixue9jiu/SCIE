using Engine;
using System.Collections.Generic;

namespace Game
{
	public class DispenserNew2ElectricElement : ElectricElement
	{
		private bool m_isDispenseAllowed = true;

		private double? m_lastDispenseTime;

		private readonly SubsystemBlockEntities m_subsystemBlockEntities;

		public DispenserNew2ElectricElement(SubsystemElectricity subsystemElectricity, Point3 point)
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
			m_subsystemBlockEntities = SubsystemElectricity.Project.FindSubsystem<SubsystemBlockEntities>(true);
		}

		public override bool Simulate()
		{
			if (CalculateHighInputsCount() > 0)
			{
				if (m_isDispenseAllowed && (!m_lastDispenseTime.HasValue || SubsystemElectricity.SubsystemTime.GameTime - m_lastDispenseTime > 0.1))
				{
					m_isDispenseAllowed = false;
					m_lastDispenseTime = SubsystemElectricity.SubsystemTime.GameTime;
					SubsystemBlockEntities subsystemBlockEntities = m_subsystemBlockEntities;
					ReadOnlyList<CellFace> cellFaces = CellFaces;
					CellFace cellFace = cellFaces[0];
					int x = cellFace.Point.X;
					cellFaces = CellFaces;
					cellFace = cellFaces[0];
					int y = cellFace.Point.Y;
					cellFaces = CellFaces;
					cellFace = cellFaces[0];
					int z = cellFace.Point.Z;
					ComponentBlockEntity blockEntity = subsystemBlockEntities.GetBlockEntity(x, y, z);
					if (blockEntity != null)
					{
						ComponentDispenserNew2 componentDispenserNew = blockEntity.Entity.FindComponent<ComponentDispenserNew2>();
						if (componentDispenserNew != null)
						{
							componentDispenserNew.Dispense();
						}
					}
				}
			}
			else
			{
				m_isDispenseAllowed = true;
			}
			return false;
		}
	}
}
