using Engine;
using System.Collections.Generic;

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
}