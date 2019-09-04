using Engine;
using GameEntitySystem;
using System;
using TemplatesDatabase;

namespace Game
{
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
			if (MineType != 0)
				valuesDictionary.SetValue("Type", (int)MineType);
			if (Delay != 0)
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