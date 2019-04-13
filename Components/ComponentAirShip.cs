using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	public class ComponentAirShip : ComponentBoatI, IUpdateable
	{
		protected ComponentEngineA componentEngine;

		public new void Update(float dt)
		{
			var componentBody = m_componentBody;
			componentBody.IsGravityEnabled = false;
			componentBody.IsGroundDragEnabled = false;
			Quaternion rotation = componentBody.Rotation;
			float num = MathUtils.Atan2(2f * rotation.Y * rotation.W - 2f * rotation.X * rotation.Z, 1f - 2f * rotation.Y * rotation.Y - 2f * rotation.Z * rotation.Z);
			if ((m_turnSpeed += 2.5f * Utils.SubsystemTime.GameTimeDelta * (TurnOrder - m_turnSpeed)) != 0)
				num -= m_turnSpeed * dt;
			componentBody.Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, num);
			ComponentRider rider = m_componentMount.Rider;
			if (MoveOrder != 0f)
			{
				if (rider != null)
					componentBody.Velocity += dt * (componentEngine != null ? componentEngine.HeatLevel * 0.01f : 3f) * MoveOrder * rider.ComponentCreature.ComponentCreatureModel.EyeRotation.ToForwardVector();
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
			Entity.FindComponent<ComponentModel>().TextureOverride = Airship.WhiteTexture;
		}
	}
}