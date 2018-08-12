using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	public class ComponentBoatI : Component, IUpdateable
	{
		private ComponentBody m_componentBody;

		private ComponentDamage m_componentDamage;

		private ComponentMount m_componentMount;

		private SubsystemAudio m_subsystemAudio;

		private SubsystemTime m_subsystemTime;

		private float m_turnSpeed;

		private readonly ComponentEngine2 m_heatlevel;

		public float MoveOrder
		{
			get;
			set;
		}

		public float TurnOrder
		{
			get;
			set;
		}

		public float Health
		{
			get;
			private set;
		}

		public int UpdateOrder
		{
			get
			{
				return 0;
			}
		}

		public float HeatLevel
		{
			get;
			private set;
		}

		public void Update(float dt)
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
			bool num = m_componentBody.ImmersionFactor > 0.95f;
			bool num2 = !num && m_componentBody.ImmersionFactor > 0.01f && !m_componentBody.StandingOnValue.HasValue && m_componentBody.StandingOnBody == null;
			m_turnSpeed += 2.5f * m_subsystemTime.GameTimeDelta * (1f * TurnOrder - m_turnSpeed);
			Quaternion rotation = m_componentBody.Rotation;
			float num3 = MathUtils.Atan2(2f * rotation.Y * rotation.W - 2f * rotation.X * rotation.Z, 1f - 2f * rotation.Y * rotation.Y - 2f * rotation.Z * rotation.Z);
			ComponentEngine2 componentEngine = base.Entity.FindComponent<ComponentEngine2>();
			float num4 = 1f;
			if (componentEngine != null)
			{
				num4 = componentEngine.HeatLevel;
			}
			if (num2)
			{
				num3 -= m_turnSpeed * dt * num4;
			}
			m_componentBody.Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, num3);
			if (num2 && MoveOrder != 0f)
			{
				m_componentBody.Velocity += dt * num4 * 10f * MoveOrder * m_componentBody.Matrix.Forward;
			}
			if (num)
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

		public void Injure(float amount, ComponentCreature attacker, bool ignoreInvulnerability)
		{
			if (!((double)amount <= 0.0))
			{
				Health = MathUtils.Max(Health - amount, 0f);
			}
		}

		protected override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			m_subsystemAudio = base.Project.FindSubsystem<SubsystemAudio>(true);
			m_componentMount = base.Entity.FindComponent<ComponentMount>(true);
			m_componentBody = base.Entity.FindComponent<ComponentBody>(true);
			m_componentDamage = base.Entity.FindComponent<ComponentDamage>(true);
		}
	}
}
