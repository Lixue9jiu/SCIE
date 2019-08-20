using Engine;
using GameEntitySystem;
using System.Linq;
using TemplatesDatabase;

namespace Game
{
	public class ComponentBoatI : ComponentBoat, IUpdateable
	{
		private ComponentEngine componentEngine;

		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			base.Load(valuesDictionary, idToEntityMap);
			componentEngine = Entity.FindComponent<ComponentEngine>();
		}

		public new void Update(float dt)
		{
			if (componentEngine != null)
				componentEngine.Coordinates = new Point3((int)m_componentBody.Position.X, (int)m_componentBody.Position.Y, (int)m_componentBody.Position.Z);
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

	public class ComponentAirship : ComponentBoatI, IUpdateable
	{
		protected ComponentEngineA componentEngine;

		public new void Update(float dt)
		{
			var componentBody = m_componentBody;
			componentBody.IsGravityEnabled = false;
			componentBody.IsGroundDragEnabled = false;
			Quaternion rotation = componentBody.Rotation;
			float num = MathUtils.Atan2(2f * rotation.Y * rotation.W - 2f * rotation.X * rotation.Z, 1f - 2f * rotation.Y * rotation.Y - 2f * rotation.Z * rotation.Z);
			if ((m_turnSpeed += 2.5f * Utils.SubsystemTime.GameTimeDelta * (TurnOrder - m_turnSpeed)) != 0 && componentEngine.HeatLevel > 0f)
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
		}
	}

	public class ComponentCar : ComponentBoatI, IUpdateable
	{
		protected ComponentEngineA componentEngine;
		protected ComponentEngineT componentEngine2;


		public new void Update(float dt)
		{
			if (componentEngine2 != null)
			{
				var componentBody = m_componentBody;
				componentBody.IsGravityEnabled = true;
				componentBody.IsGroundDragEnabled = true;
				Quaternion rotation = componentBody.Rotation;
				float num = MathUtils.Atan2(2f * rotation.Y * rotation.W - 2f * rotation.X * rotation.Z, 1f - 2f * rotation.Y * rotation.Y - 2f * rotation.Z * rotation.Z);
				if ((m_turnSpeed += 2.5f * Utils.SubsystemTime.GameTimeDelta * (TurnOrder - m_turnSpeed)) != 0 && componentEngine2.HeatLevel > 0f && MoveOrder != 0f)
					num -= m_turnSpeed * dt;
				componentBody.Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, num);
				ComponentRider rider = m_componentMount.Rider;
				if (MoveOrder != 0f)
				{
					if (rider != null && componentBody.StandingOnValue.HasValue && componentEngine2 != null && componentEngine2.HeatLevel > 0f)
						m_componentBody.Velocity += dt * 10f * MoveOrder * m_componentBody.Matrix.Forward;
					MoveOrder = 0f;
				}
				if (componentBody.ImmersionFactor > 0.95f)
				{
					m_componentDamage.Damage(0.005f * dt);
					if (rider != null)
						rider.StartDismounting();
				}
				TurnOrder = 0f;
				if (componentEngine2.HeatLevel <= 0f || componentBody.Mass > 1200f || !componentBody.StandingOnValue.HasValue)
					return;
				int value = Terrain.ExtractContents(componentBody.StandingOnValue.Value);
				Vector3 p2 = Vector3.Normalize(componentBody.Rotation.ToForwardVector());
				if (value == SoilBlock.Index)
					componentBody.IsSneaking = true;
				if (value == DirtBlock.Index || value == GrassBlock.Index)
				{
					var p = Terrain.ToCell(componentBody.Position-p2*2f);
					Utils.SubsystemTerrain.ChangeCell(p.X, p.Y - 1, p.Z, Terrain.ReplaceContents(value, 168));
				}
			}
			if (componentEngine != null)
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
					if (rider != null && componentBody.StandingOnValue.HasValue && componentEngine != null && componentEngine.HeatLevel > 0f)
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
				if (componentEngine.HeatLevel <= 0f || componentBody.Mass > 1200f || !componentBody.StandingOnValue.HasValue)
					return;
				int value = Terrain.ExtractContents(componentBody.StandingOnValue.Value);
				//if (value == SoilBlock.Index)
				//	componentBody.IsSneaking = true;
				//if (value == DirtBlock.Index || value == GrassBlock.Index)
				//{
				//	var p = Terrain.ToCell(componentBody.Position);
				//	Utils.SubsystemTerrain.ChangeCell(p.X, p.Y - 1, p.Z, Terrain.ReplaceContents(value, 168));
				//}
			}

			

		}

		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			m_componentMount = Entity.FindComponent<ComponentMount>(true);
			m_componentBody = Entity.FindComponent<ComponentBody>(true);
			m_componentDamage = Entity.FindComponent<ComponentDamage>(true);
			componentEngine = Entity.FindComponent<ComponentEngineA>();
			componentEngine2 = Entity.FindComponent<ComponentEngineT>();
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

	public class ComponentTrain : Component, IUpdateable
	{
		static readonly Vector3 center = new Vector3(0.5f, 0, 0.5f);
		static readonly Quaternion[] directions = new[]
		{
			Quaternion.CreateFromAxisAngle(Vector3.UnitY, 0),
			Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathUtils.PI * 0.5f),
			Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathUtils.PI),
			Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathUtils.PI * 1.5f)
		};

		static readonly Quaternion upwardDirection = Quaternion.CreateFromAxisAngle(new Vector3(1, 0, 0), MathUtils.PI * 0.25f);
		static readonly Quaternion downwardDirection = Quaternion.CreateFromAxisAngle(new Vector3(1, 0, 0), MathUtils.PI * -0.25f);
		static readonly Vector3[] forwardVectors = new[]
		{
			new Vector3(0, 0, -1),
			new Vector3(-1, 0, 0),
			new Vector3(0, 0, 1),
			new Vector3(1, 0, 0)
		};
		
		internal ComponentBody m_componentBody;
		public ComponentTrain ParentBody;
		protected ComponentDamage componentDamage;
		protected float m_outOfMountTime;
		ComponentMount m_componentMount;
		public ComponentEngine ComponentEngine;
		int m_forwardDirection;
		Quaternion rotation;
		Vector3 forwardVector;

		public int Direction
		{
			get { return m_forwardDirection; }
			set
			{
				forwardVector = forwardVectors[value];
				m_forwardDirection = value;
				rotation = directions[value];
			}
		}

		public int UpdateOrder => 0;

		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			base.Load(valuesDictionary, idToEntityMap);
			m_componentBody = Entity.FindComponent<ComponentBody>(true);
			componentDamage = Entity.FindComponent<ComponentDamage>();
			m_componentMount = Entity.FindComponent<ComponentMount>(true);
			if ((ComponentEngine = Entity.FindComponent<ComponentEngine>()) != null)
				m_componentBody.CollidedWithBody += CollidedWithBody;
			else
				ParentBody = valuesDictionary.GetValue("ParentBody", default(EntityReference)).GetComponent<ComponentTrain>(Entity, idToEntityMap, false);
			
			Direction = valuesDictionary.GetValue("Direction", 0);
		}

		public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
		{
			if (m_forwardDirection != 0)
				valuesDictionary.SetValue("Direction", m_forwardDirection);
			var value = EntityReference.FromId(ParentBody, entityToIdMap);
			if (!value.IsNullOrEmpty())
				valuesDictionary.SetValue("ParentBody", value);
		}

		public void CollidedWithBody(ComponentBody body)
		{
			if (body.Density - 4.76f <= float.Epsilon)
				return;
			Vector2 v = m_componentBody.Velocity.XZ - body.Velocity.XZ;
			float amount = v.LengthSquared() * .3f;
			if (amount < .02f || m_componentBody.Velocity.XZ.LengthSquared()==0f) return;
			var health = body.Entity.FindComponent<ComponentHealth>();
			if (health != null)
				health.Injure(amount / health.AttackResilience, null, false, "Struck by a train");
			else
				body.Entity.FindComponent<ComponentDamage>()?.Damage(amount);
			body.ApplyImpulse(MathUtils.Clamp(1.25f * 6f * MathUtils.Pow(m_componentBody.Mass / body.Mass, 0.33f), 0f, 6f) * Vector3.Normalize(body.Position - m_componentBody.Position));
		}

		public ComponentTrain FindNearestTrain()
		{
			var bodies = new DynamicArray<ComponentBody>();
			Utils.SubsystemBodies.FindBodiesAroundPoint(m_componentBody.Position.XZ, 2f, bodies);
			float num = 0f;
			ComponentTrain result = null;
			foreach (ComponentTrain train in bodies.Select(GetRailEntity))
			{
				if (train == null || train.Entity == Entity) continue;
				float score = 0f;
				const float maxDistance = 4f;
				if (train.m_componentBody.Velocity.LengthSquared() < 1f && train.Direction == Direction)
				{
					var v = train.m_componentBody.Position + Vector3.Transform(train.m_componentMount.MountOffset, train.m_componentBody.Rotation) - m_componentBody.Position;
					if (v.LengthSquared() <= maxDistance)
						score = maxDistance - v.LengthSquared();
				}
				if (score > num)
				{
					num = score;
					result = train;
				}
			}
			return result;
		}

		public void SetDirection(int value)
		{
			Direction = value;
			m_componentBody.Rotation = rotation;
		}

		public void Update(float dt)
		{
			if (ComponentEngine != null)
				ComponentEngine.Coordinates = new Point3((int)m_componentBody.Position.X, (int)m_componentBody.Position.Y, (int)m_componentBody.Position.Z);
			if (m_componentMount.Rider != null)
			{
				var player = m_componentMount.Rider.Entity.FindComponent<ComponentPlayer>(true);
				player.ComponentLocomotion.LookOrder = player.ComponentInput.PlayerInput.Look;
			}

			ComponentTrain t = this;
			int level = 0;
			for (; t.ParentBody != null; level++) t = t.ParentBody;
			if (level > 0)
			{
				var body = t.m_componentBody;
				var pos = body.Position;
				var r = body.Rotation;
				Utils.SubsystemTime.QueueGameTimeDelayedExecution(Utils.SubsystemTime.GameTime + 0.23 * level, delegate
				{
					if (body.Velocity.XZ.LengthSquared() > 30f)
					{
						m_componentBody.Position = pos;
						m_componentBody.Rotation = r;
					}
				});
				m_outOfMountTime = Vector3.DistanceSquared(ParentBody.m_componentBody.Position, m_componentBody.Position) > 8f
					? m_outOfMountTime + dt
					: 0f;
				ComponentDamage ComponentDamage = ParentBody.Entity.FindComponent<ComponentDamage>();
				if (m_outOfMountTime > 1f || (componentDamage != null && componentDamage.Hitpoints <= .05f) || ComponentDamage != null && ComponentDamage.Hitpoints <= .05f)
					ParentBody = null;
				return;
			}

			switch (Direction)
			{
				case 0:
				case 2:
					m_componentBody.Position = new Vector3(MathUtils.Floor(m_componentBody.Position.X) + 0.5f, m_componentBody.Position.Y, m_componentBody.Position.Z);
					break;
				case 1:
				case 3:
					m_componentBody.Position = new Vector3(m_componentBody.Position.X, m_componentBody.Position.Y, MathUtils.Floor(m_componentBody.Position.Z) + 0.5f);
					break;
			}
			
			if (ComponentEngine != null && ComponentEngine.HeatLevel >= 100f && m_componentBody.StandingOnValue.HasValue)
			{
				var result = Utils.SubsystemTerrain.Raycast(m_componentBody.Position, m_componentBody.Position + new Vector3(0, -3f, 0), false, true, null);

				if (result.HasValue && Terrain.ExtractContents(result.Value.Value) == RailBlock.Index && (dt *= SimulateRail(RailBlock.GetRailType(Terrain.ExtractData(result.Value.Value)))) > 0f)
					m_componentBody.m_velocity += dt * rotation.ToForwardVector();
			}
			m_componentBody.Rotation = Quaternion.Slerp(m_componentBody.Rotation, rotation, 0.15f);
		}

		float SimulateRail(int railType)
		{
			if (RailBlock.IsCorner(railType))
			{
				if (GetOffsetOnDirection(m_componentBody.Position, m_forwardDirection) > 0.5f)
					Turn(railType);
				return 50f;
			}
			if (RailBlock.IsDirectionX(railType) ^ !RailBlock.IsDirectionX(m_forwardDirection))
			{
				rotation = railType > 5
					? railType - 6 != Direction ? directions[Direction] * upwardDirection : directions[Direction] * downwardDirection
					: directions[Direction];
				return railType > 5 && railType - 6 != Direction ? 30f : 50f;
			}
			return 0f;
		}

		bool Turn(int turnType)
		{
			if (Direction == turnType)
			{
				Direction = (Direction - 1) & 3;
				m_componentBody.Velocity = MathUtils.Abs(m_componentBody.Velocity.X + m_componentBody.Velocity.Z) * forwardVector;
				m_componentBody.Position = Vector3.Floor(m_componentBody.Position) + center;
				return true;
			}
			if (((Direction - 1) & 3) == turnType)
			{
				Direction = (Direction + 1) & 3;
				m_componentBody.Velocity = MathUtils.Abs(m_componentBody.Velocity.X + m_componentBody.Velocity.Z) * forwardVector;
				m_componentBody.Position = Vector3.Floor(m_componentBody.Position) + center;
				return true;
			}
			return false;
		}

		static float GetOffsetOnDirection(Vector3 vec, int direction)
		{
			float offset = (direction & 1) == 0 ? vec.Z - MathUtils.Floor(vec.Z) : vec.X - MathUtils.Floor(vec.X);
			return (direction & 2) == 0 ? 1 - offset : offset;
		}

		public static ComponentTrain GetRailEntity(Component c) => c.Entity.FindComponent<ComponentTrain>();
	}
}