using Engine;
using GameEntitySystem;

namespace Game
{
	public class ComponentBoatI : ComponentBoat, IUpdateable
	{
		public new void Update(float dt)
		{
			if (m_componentDamage.Hitpoints < 0.33f)
			{
				m_componentBody.Density = 1.15f;
				if (m_componentDamage.Hitpoints - m_componentDamage.HitpointsChange >= 0.33f && m_componentBody.ImmersionFactor > 0f)
				{
					m_subsystemAudio.PlaySound("Audio/Sinking", 1f, 0f, m_componentBody.Position, 4f, true);
				}
			}
			else if (m_componentDamage.Hitpoints < 0.66f)
			{
				m_componentBody.Density = 0.7f;
				if (m_componentDamage.Hitpoints - m_componentDamage.HitpointsChange >= 0.66f && m_componentBody.ImmersionFactor > 0f)
				{
					m_subsystemAudio.PlaySound("Audio/Sinking", 1f, 0f, m_componentBody.Position, 4f, true);
				}
			}
			bool flag = m_componentBody.ImmersionFactor > 0.95f;
			bool obj = !flag && m_componentBody.ImmersionFactor > 0.01f && m_componentBody.StandingOnValue == null && m_componentBody.StandingOnBody == null;
			m_turnSpeed += 2.5f * m_subsystemTime.GameTimeDelta * (1f * TurnOrder - m_turnSpeed);
			Quaternion rotation = m_componentBody.Rotation;
			float num = MathUtils.Atan2(2f * rotation.Y * rotation.W - 2f * rotation.X * rotation.Z, 1f - 2f * rotation.Y * rotation.Y - 2f * rotation.Z * rotation.Z);
			ComponentEngine2 componentEngine = Entity.FindComponent<ComponentEngine2>();
			float num2 = 3f;
			if (componentEngine != null)
			{
				num2 = componentEngine.HeatLevel;
			}
			bool obj2 = obj;
			if (obj2 && num2 != 0f)
			{
				num -= m_turnSpeed * dt;
			}
			m_componentBody.Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, num);
			if (obj2 && MoveOrder != 0f)
			{
				m_componentBody.Velocity += dt * num2 / 90f * MoveOrder * m_componentBody.Matrix.Forward;
			}
			if (flag)
			{
				m_componentDamage.Damage(0.005f * dt);
				if (m_componentMount.Rider != null)
				{
					m_componentMount.Rider.StartDismounting();
				}
			}
			MoveOrder = 0f;
			TurnOrder = 0f;
		}
	}
}
