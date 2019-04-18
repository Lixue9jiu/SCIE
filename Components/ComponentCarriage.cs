using Engine;
using GameEntitySystem;
using System.Linq;
using TemplatesDatabase;

namespace Game
{
	public class ComponentRailEntity : ComponentMount
	{
	}
	public class ComponentCarriage : ComponentChest, IUpdateable
	{
		public Vector3 RiderOffset;
		public float m_outOfMountTime;
		public static SubsystemBodies m_subsystemBodies;
		public ComponentBody ComponentBody;
		public ComponentDamage ComponentDamage;
		ComponentMount m_componentMount;
		public ComponentMount Mount => ComponentBody.ParentBody?.Entity.FindComponent<ComponentMount>();
		public int UpdateOrder => 0;
		public ComponentRailEntity FindNearestMount()
		{
			var bodies = new DynamicArray<ComponentBody>();
			m_subsystemBodies.FindBodiesAroundPoint(new Vector2(ComponentBody.Position.X, ComponentBody.Position.Z), 2.5f, bodies);
			float num = 0f;
			ComponentRailEntity result = null;
			foreach (ComponentRailEntity componentMount in bodies.Select(GetRailEntity).Where(IsTargetMount))
			{
				float score = 0f;
				const float maxDistance = 16f;
				if (componentMount.ComponentBody.Velocity.LengthSquared() < 1f)
				{
					var v = componentMount.ComponentBody.Position + Vector3.Transform(componentMount.MountOffset, componentMount.ComponentBody.Rotation) - ComponentBody.Position;
					if (v.LengthSquared() < maxDistance)
						score = maxDistance - v.Length();
				}
				if (score > num)
				{
					num = score;
					result = componentMount;
				}
			}
			return result;
		}
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.LoadItems(valuesDictionary);
			m_subsystemBodies = Project.FindSubsystem<SubsystemBodies>(true);
			ComponentBody = Entity.FindComponent<ComponentBody>(true);
			ComponentDamage = Entity.FindComponent<ComponentDamage>(true);
			m_componentMount = Entity.FindComponent<ComponentMount>(true);
			RiderOffset = valuesDictionary.GetValue<Vector3>("RiderOffset");
		}
		public void StartMounting(ComponentMount componentMount)
		{
			if (Mount == null)
			{
				ComponentBody.ParentBody = componentMount.ComponentBody;
				//ComponentBody.ParentBodyPositionOffset = Vector3.Transform(ComponentBody.Position - componentMount.ComponentBody.Position, Quaternion.Conjugate(componentMount.ComponentBody.Rotation));
				//ComponentBody.ParentBodyRotationOffset = Quaternion.Conjugate(componentMount.ComponentBody.Rotation) * ComponentBody.Rotation;
				ComponentBody.ParentBodyPositionOffset = componentMount.MountOffset + RiderOffset;
				ComponentBody.ParentBodyRotationOffset = Quaternion.Identity;
			}
		}
		public void StartDismounting()
		{
			ComponentMount mount = Mount;
			if (mount == null)
				return;
			float x = 0f;
			if (mount.DismountOffset.X > 0f)
			{
				Vector3 vector = 0.5f * (ComponentBody.BoundingBox.Min + ComponentBody.BoundingBox.Max);
				x = mount.DismountOffset.X + 0.5f;
				var right = ComponentBody.Matrix.Right;
				var result = Utils.SubsystemTerrain.Raycast(vector, vector - x * right, false, true, null);
				var result2 = Utils.SubsystemTerrain.Raycast(vector, vector + x * right, false, true, null);
				x = !result.HasValue ? (0f - mount.DismountOffset.X) : ((!result2.HasValue) ? mount.DismountOffset.X : ((!(result.Value.Distance > result2.Value.Distance)) ? MathUtils.Min(result2.Value.Distance, mount.DismountOffset.X) : (0f - MathUtils.Min(result.Value.Distance, mount.DismountOffset.X))));
			}
			if (ComponentBody.ParentBody != null)
			{
				ComponentBody.Velocity = ComponentBody.ParentBody.Velocity;
				ComponentBody.ParentBody = null;
			}
			ComponentBody.ParentBodyPositionOffset = mount.MountOffset + RiderOffset + new Vector3(x, mount.DismountOffset.Y, mount.DismountOffset.Z);
			ComponentBody.ParentBodyRotationOffset = Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathUtils.Sign(x) * MathUtils.DegToRad(60f));
		}
		public void Update(float dt)
		{
			ComponentMount mount = m_componentMount;
			if (mount.Rider != null)
			{
				var player = mount.Rider.Entity.FindComponent<ComponentPlayer>();
				player.ComponentLocomotion.LookOrder = player.ComponentInput.PlayerInput.Look;
			}
			mount = Mount;
			if (mount != null)
			{
				ComponentBody componentBody = ComponentBody;
				ComponentBody parentBody = ComponentBody.ParentBody;
				m_outOfMountTime = Vector3.DistanceSquared(parentBody.Position + Vector3.Transform(componentBody.ParentBodyPositionOffset, parentBody.Rotation), componentBody.Position) > 6
					? m_outOfMountTime + dt
					: 0f;
				ComponentDamage componentDamage = mount.Entity.FindComponent<ComponentDamage>();
				if (m_outOfMountTime > 0.1f || (componentDamage != null && componentDamage.Hitpoints <= .05f) || ComponentDamage.Hitpoints <= .05f)
					StartDismounting();
				ComponentBody.ParentBodyPositionOffset = mount.MountOffset + RiderOffset;
				ComponentBody.ParentBodyRotationOffset = Quaternion.Identity;
			}
		}
		public static ComponentRailEntity GetRailEntity(ComponentBody b)
		{
			return b.Entity.FindComponent<ComponentRailEntity>();
		}
		public bool IsTargetMount(ComponentMount m)
		{
			return m != null && m.Entity != Entity;
		}

		public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
		{
			this.SaveItems(valuesDictionary);
		}
	}
}