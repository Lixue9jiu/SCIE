using Engine;
using GameEntitySystem;
using System;
using TemplatesDatabase;

namespace Game
{
	public class ComponentBoatI : ComponentBoat, IUpdateable
	{
		public new void Update(float dt)
		{
			bool flag = m_componentBody.ImmersionFactor > 0f;
			if (m_componentDamage.Hitpoints < 0.33f)
			{
				m_componentBody.Density = 1.15f;
				if (m_componentDamage.Hitpoints - m_componentDamage.HitpointsChange >= 0.33f && flag)
					m_subsystemAudio.PlaySound("Audio/Sinking", 1f, 0f, m_componentBody.Position, 4f, true);
			}
			else if (m_componentDamage.Hitpoints < 0.66f)
			{
				m_componentBody.Density = 0.7f;
				if (m_componentDamage.Hitpoints - m_componentDamage.HitpointsChange >= 0.66f && flag)
					m_subsystemAudio.PlaySound("Audio/Sinking", 1f, 0f, m_componentBody.Position, 4f, true);
			}
			flag = m_componentBody.ImmersionFactor > 0.95f;
			bool obj = !flag && m_componentBody.ImmersionFactor > 0.01f && m_componentBody.StandingOnValue == null && m_componentBody.StandingOnBody == null;
			m_turnSpeed += 2.5f * m_subsystemTime.GameTimeDelta * (1f * TurnOrder - m_turnSpeed);
			Quaternion rotation = m_componentBody.Rotation;
			float num = MathUtils.Atan2(2f * rotation.Y * rotation.W - 2f * rotation.X * rotation.Z, 1f - 2f * rotation.Y * rotation.Y - 2f * rotation.Z * rotation.Z);
			var componentEngine = Entity.FindComponent<ComponentEngine2>();
			float num2 = 3f;
			if (componentEngine != null)
				num2 = componentEngine.HeatLevel;
			if (obj && num2 != 0f)
				num -= m_turnSpeed * dt;
			m_componentBody.Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, num);
			if (obj && MoveOrder != 0f)
				m_componentBody.Velocity += dt * num2 / 90f * MoveOrder * m_componentBody.Matrix.Forward;
			if (flag)
			{
				m_componentDamage.Damage(0.005f * dt);
				if (m_componentMount.Rider != null)
					m_componentMount.Rider.StartDismounting();
			}
			MoveOrder = 0f;
			TurnOrder = 0f;
		}
	}
	[Flags]
	public enum MineType
	{
		Tiny = 0,
		Small = 1,
		Medium = 2,
		Large = 3,
		Incendiary = 4,
		Sensitive = 8,
		FL = 16,
		Torpedo = 32
	}
	public class ComponentMine : Component
	{
		public ComponentBody ComponentBody;
		public SubsystemExplosivesBlockBehavior SubsystemExplosivesBlockBehavior;
		public float ExplosionPressure;
		public MineType MineType;
		public double Delay;
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			base.Load(valuesDictionary, idToEntityMap);
			ExplosionPressure = valuesDictionary.GetValue("ExplosionPressure", 60f);
			MineType = valuesDictionary.GetValue<MineType>("Type", 0);
			Delay = valuesDictionary.GetValue("Delay", .0);
			(ComponentBody = Entity.FindComponent<ComponentBody>(true)).CollidedWithBody += CollidedWithBody;
			if ((MineType & MineType.Torpedo) != 0)
				ComponentBody.Density = .8f;
		}
		public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
		{
			base.Save(valuesDictionary, entityToIdMap);
			valuesDictionary.SetValue("ExplosionPressure", ExplosionPressure);
			valuesDictionary.SetValue("Type", (int)MineType);
			valuesDictionary.SetValue("Delay", Delay);
		}
		public void CollidedWithBody(ComponentBody body)
		{
			if ((MineType & MineType.Sensitive) == 0 && body.Density - 2f < .1f)
				return;
			if ((MineType & MineType.FL) != 0)
			{
				body.PositionChanged += PositionChanged;
				return;
			}
			Fire();
		}
		public void Fire()
		{
			if (Delay > .0)
			{
				Time.QueueTimeDelayedExecution(Time.FrameStartTime + Delay, Fire);
				Delay = 0;
				return;
			}
			Project.RemoveEntity(Entity, true);
			var p = Terrain.ToCell(ComponentBody.Position);
			Utils.SubsystemExplosions.AddExplosion(p.X, p.Y, p.Z, ExplosionPressure, (MineType & MineType.Incendiary) != 0, false);
		}
		public void PositionChanged(ComponentFrame frame)
		{
			if ((frame.Position - ComponentBody.Position).LengthSquared() > .216f)
			{
				frame.PositionChanged -= PositionChanged;
				Fire();
			}
		}
	}
	/*public class ComponentSteelTrap : Component
	{
		public float Damage;
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			base.Load(valuesDictionary, idToEntityMap);
			Damage = valuesDictionary.GetValue("Damage", 11f);
			Entity.FindComponent<ComponentBody>(true).CollidedWithBody += CollidedWithBody;
		}
		public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
		{
			base.Save(valuesDictionary, entityToIdMap);
			valuesDictionary.SetValue("Damage", Damage);
		}
		public void CollidedWithBody(ComponentBody body)
		{
			Project.RemoveEntity(Entity, true);
			var health = body.Entity.FindComponent<ComponentHealth>();
			if (health != null)
				health.Injure(Damage / health.AttackResilience, null, false, "Trap");
			else
				body.Entity.FindComponent<ComponentDamage>()?.Damage(Damage);
			body.ApplyImpulse(new Vector3 { Y = .2f });
		}
	}*/
}