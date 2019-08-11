using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	public class ComponentCar : ComponentBoatI, IUpdateable
	{
		protected ComponentEngineA componentEngine;

		public new void Update(float dt)
		{
			var componentBody = m_componentBody;
			componentBody.IsGravityEnabled = true;
			componentBody.IsGroundDragEnabled = false;
			Quaternion rotation = componentBody.Rotation;
			float num = MathUtils.Atan2(2f * rotation.Y * rotation.W - 2f * rotation.X * rotation.Z, 1f - 2f * rotation.Y * rotation.Y - 2f * rotation.Z * rotation.Z);
			if ((m_turnSpeed += 2.5f * Utils.SubsystemTime.GameTimeDelta * (TurnOrder - m_turnSpeed)) != 0 && componentEngine.HeatLevel > 0f && MoveOrder != 0f)
				num -= m_turnSpeed * dt;
			componentBody.Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, num);
			ComponentRider rider = m_componentMount.Rider;
			if (MoveOrder != 0f)
			{
				if (rider != null && componentEngine != null && componentEngine.HeatLevel>0f)
					//componentBody.Velocity += dt * (componentEngine != null ? componentEngine.HeatLevel * 0.01f : 3f) * MoveOrder * rider.ComponentCreature.ComponentCreatureModel.EyeRotation.ToForwardVector();
				    m_componentBody.Velocity += dt * 40f * MoveOrder * m_componentBody.Matrix.Forward;
				MoveOrder = 0f;
			}
			if (componentBody.ImmersionFactor > 0.95f)
			{
				m_componentDamage.Damage(0.005f * dt);
				if (rider != null)
					rider.StartDismounting();
			}
			TurnOrder = 0f;
		}

		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			m_componentMount = Entity.FindComponent<ComponentMount>(true);
			m_componentBody = Entity.FindComponent<ComponentBody>(true);
			m_componentDamage = Entity.FindComponent<ComponentDamage>(true);
			componentEngine = Entity.FindComponent<ComponentEngineA>();
			Entity.FindComponent<ComponentModel>(true).TextureOverride = TexturedMeshItem.WhiteTexture;
			m_componentBody.CollidedWithBody += CollidedWithBody;
		}

		public void CollidedWithBody(ComponentBody body)
		{
			Vector2 v = m_componentBody.Velocity.XZ - body.Velocity.XZ;
			float amount = v.LengthSquared() * .3f;
			if (amount < .02f || m_componentBody.Velocity.XZ.LengthSquared() == 0f) return;
			var health = body.Entity.FindComponent<ComponentHealth>();
			if (health != null)
				health.Injure(amount / health.AttackResilience, null, false, "Struck by a car");
			else
				body.Entity.FindComponent<ComponentDamage>()?.Damage(amount);
			body.ApplyImpulse(MathUtils.Clamp(2.5f * MathUtils.Pow(m_componentBody.Mass / body.Mass, 0.33f), 0f, 6f) * Vector3.Normalize(body.Position - m_componentBody.Position));
		}
	}
}