using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
    // Token: 0x0200055A RID: 1370
    public class ComponentBoatI : Component, IUpdateable
    {
        // Token: 0x17000529 RID: 1321
        // (get) Token: 0x0600205F RID: 8287 RVA: 0x00014DC7 File Offset: 0x00012FC7
        // (set) Token: 0x06002060 RID: 8288 RVA: 0x00014DCF File Offset: 0x00012FCF
        public float MoveOrder { get; set; }

        // Token: 0x1700052A RID: 1322
        // (get) Token: 0x06002061 RID: 8289 RVA: 0x00014DD8 File Offset: 0x00012FD8
        // (set) Token: 0x06002062 RID: 8290 RVA: 0x00014DE0 File Offset: 0x00012FE0
        public float TurnOrder { get; set; }

        // Token: 0x1700052B RID: 1323
        // (get) Token: 0x06002063 RID: 8291 RVA: 0x00014DE9 File Offset: 0x00012FE9
        // (set) Token: 0x06002064 RID: 8292 RVA: 0x00014DF1 File Offset: 0x00012FF1
        public float Health { get; private set; }

        // Token: 0x1700052C RID: 1324
        // (get) Token: 0x06002065 RID: 8293 RVA: 0x000034CC File Offset: 0x000016CC
        public int UpdateOrder
        {
            get
            {
                return 0;
            }
        }

        // Token: 0x06002067 RID: 8295 RVA: 0x000DBF14 File Offset: 0x000DA114
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
            object obj = !flag && this.m_componentBody.ImmersionFactor > 0.01f && this.m_componentBody.StandingOnValue == null && this.m_componentBody.StandingOnBody == null;
            this.m_turnSpeed += 2.5f * this.m_subsystemTime.GameTimeDelta * (1f * this.TurnOrder - this.m_turnSpeed);
            Quaternion rotation = this.m_componentBody.Rotation;
            float num = MathUtils.Atan2(2f * rotation.Y * rotation.W - 2f * rotation.X * rotation.Z, 1f - 2f * rotation.Y * rotation.Y - 2f * rotation.Z * rotation.Z);
            ComponentEngine2 componentEngine = base.Entity.FindComponent<ComponentEngine2>();
            float num2 = 3f;
            if (componentEngine != null)
            {
                num2 =  componentEngine.HeatLevel;
            }
            object obj2 = obj;
            if (obj2 != null && num2 != 0f)
            {
                num -= this.m_turnSpeed * dt;
            }
            this.m_componentBody.Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, num);
            if (obj2 != null && this.MoveOrder != 0f)
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

        // Token: 0x06002068 RID: 8296 RVA: 0x00014DFA File Offset: 0x00012FFA
        public void Injure(float amount, ComponentCreature attacker, bool ignoreInvulnerability)
        {
            if ((double)amount <= 0.0)
            {
                return;
            }
            this.Health = MathUtils.Max(this.Health - amount, 0f);
        }

        // Token: 0x06002069 RID: 8297 RVA: 0x000DC1E0 File Offset: 0x000DA3E0
        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
        {
            this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
            this.m_subsystemAudio = base.Project.FindSubsystem<SubsystemAudio>(true);
            this.m_componentMount = base.Entity.FindComponent<ComponentMount>(true);
            this.m_componentBody = base.Entity.FindComponent<ComponentBody>(true);
            this.m_componentDamage = base.Entity.FindComponent<ComponentDamage>(true);
        }

        // Token: 0x1700052D RID: 1325
        // (get) Token: 0x0600206A RID: 8298 RVA: 0x00014E22 File Offset: 0x00013022
        // (set) Token: 0x0600206B RID: 8299 RVA: 0x00014E2A File Offset: 0x0001302A
        public float HeatLevel { get; private set; }

        // Token: 0x0600206C RID: 8300 RVA: 0x00014E33 File Offset: 0x00013033
        public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
        {
            base.Save(valuesDictionary, entityToIdMap);
        }

        // Token: 0x04001821 RID: 6177
        private ComponentBody m_componentBody;

        // Token: 0x04001822 RID: 6178
        private ComponentDamage m_componentDamage;

        // Token: 0x04001823 RID: 6179
        private ComponentMount m_componentMount;

        // Token: 0x04001824 RID: 6180
        private SubsystemAudio m_subsystemAudio;

        // Token: 0x04001825 RID: 6181
        private SubsystemTime m_subsystemTime;

        // Token: 0x04001826 RID: 6182
        private float m_turnSpeed;

        // Token: 0x04001827 RID: 6183
        private readonly ComponentEngine2 m_heatlevel;
    }
}
