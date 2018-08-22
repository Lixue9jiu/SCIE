using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
    public class ComponentBoatI : Component, IUpdateable
    {
        public float MoveOrder { get; set; }

        public float TurnOrder { get; set; }

        public float Health { get; private set; }

        public int UpdateOrder
        {
            get
            {
                return 0;
            }
        }

        public void Update(float dt)
        {
            if (this.m_componentDamage.Hitpoints < 0.33f)
            {
                this.m_componentBody.Density = 1.15f;
                if (this.m_componentDamage.Hitpoints - this.m_componentDamage.HitpointsChange >= 0.33f && this.m_componentBody.ImmersionFactor > 0f)
                {
                    this.m_subsystemAudio.PlaySound("Audio/Sinking", 1f, 0f, this.m_componentBody.Position, 4f, true);
                }
            }
            else if (this.m_componentDamage.Hitpoints < 0.66f)
            {
                this.m_componentBody.Density = 0.7f;
                if (this.m_componentDamage.Hitpoints - this.m_componentDamage.HitpointsChange >= 0.66f && this.m_componentBody.ImmersionFactor > 0f)
                {
                    this.m_subsystemAudio.PlaySound("Audio/Sinking", 1f, 0f, this.m_componentBody.Position, 4f, true);
                }
            }
            bool flag = this.m_componentBody.ImmersionFactor > 0.95f;
            bool obj = !flag && this.m_componentBody.ImmersionFactor > 0.01f && this.m_componentBody.StandingOnValue == null && this.m_componentBody.StandingOnBody == null;
            this.m_turnSpeed += 2.5f * this.m_subsystemTime.GameTimeDelta * (1f * this.TurnOrder - this.m_turnSpeed);
            Quaternion rotation = this.m_componentBody.Rotation;
            float num = MathUtils.Atan2(2f * rotation.Y * rotation.W - 2f * rotation.X * rotation.Z, 1f - 2f * rotation.Y * rotation.Y - 2f * rotation.Z * rotation.Z);
            ComponentEngine2 componentEngine = Entity.FindComponent<ComponentEngine2>();
            float num2 = 3f;
            if (componentEngine != null)
            {
                num2 =  componentEngine.HeatLevel;
            }
            bool obj2 = obj;
            if (obj2  && num2 != 0f)
            {
                num -= this.m_turnSpeed * dt;
            }
            this.m_componentBody.Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, num);
            if (obj2  && this.MoveOrder != 0f)
            {
                this.m_componentBody.Velocity += dt * num2/90f * this.MoveOrder * this.m_componentBody.Matrix.Forward;
            }
            if (flag)
            {
                this.m_componentDamage.Damage(0.005f * dt);
                if (this.m_componentMount.Rider != null)
                {
                    this.m_componentMount.Rider.StartDismounting();
                }
            }
            this.MoveOrder = 0f;
            this.TurnOrder = 0f;
        }

        public void Injure(float amount, ComponentCreature attacker, bool ignoreInvulnerability)
        {
            if ((double)amount <= 0.0)
            {
                return;
            }
            this.Health = MathUtils.Max(this.Health - amount, 0f);
        }

        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
        {
            this.m_subsystemTime = Project.FindSubsystem<SubsystemTime>(true);
            this.m_subsystemAudio = Project.FindSubsystem<SubsystemAudio>(true);
            this.m_componentMount = Entity.FindComponent<ComponentMount>(true);
            this.m_componentBody = Entity.FindComponent<ComponentBody>(true);
            this.m_componentDamage = Entity.FindComponent<ComponentDamage>(true);
        }

        public float HeatLevel { get; private set; }

        public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
        {
            base.Save(valuesDictionary, entityToIdMap);
        }

        private ComponentBody m_componentBody;

        private ComponentDamage m_componentDamage;

        private ComponentMount m_componentMount;

        private SubsystemAudio m_subsystemAudio;

        private SubsystemTime m_subsystemTime;

        private float m_turnSpeed;

        private readonly ComponentEngine2 m_heatlevel;
    }
}
