using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
    public class ComponentTrain : Component, IUpdateable
    {
        static Vector3 center = new Vector3(0.5f, 0, 0.5f);

        Quaternion[] directions = new Quaternion[]
        {
            Quaternion.CreateFromAxisAngle(Vector3.UnitY, 0),
            Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathUtils.PI * 0.5f),
            Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathUtils.PI),
            Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathUtils.PI * 1.5f)
        };

        Quaternion uprightDirection = Quaternion.CreateFromAxisAngle(new Vector3(1, 0, 0), MathUtils.PI * 0.25f);

        Vector3[] forwardVectors = new Vector3[]
        {
            new Vector3(0, 0, -1),
            new Vector3(-1, 0, 0),
            new Vector3(0, 0, 1),
            new Vector3(1, 0, 0)
        };

        SubsystemTerrain subsystemTerrain;
        ComponentBody m_componentBody;
        ComponentMount m_componentMount;

        int m_forwardDirection;
        Quaternion currentRotation;
        Vector3 forwardVector;

        Vector3 lastTurnedPosition;

        public float Speed = 30f;

        public int Direction
        {
            get
            {
                return m_forwardDirection;
            }
            set
            {
                forwardVector = forwardVectors[value];
                m_forwardDirection = value;
                currentRotation = directions[value];
            }
        }

        public int UpdateOrder => 0;

        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
        {
            base.Load(valuesDictionary, idToEntityMap);
            subsystemTerrain = Project.FindSubsystem<SubsystemTerrain>(true);
            m_componentBody = Entity.FindComponent<ComponentBody>(true);
            m_componentMount = Entity.FindComponent<ComponentMount>(true);

            m_componentBody.CollidedWithBody += CollidedWithBody;

            Direction = 0;
        }

        protected void CollidedWithBody(ComponentBody obj)
        {
            var health = obj.Entity.FindComponent<ComponentHealth>();
            if (health != null)
                health.Injure(1 / health.AttackResilience, null, false, "Train");

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
        }

        public void Update(float dt)
        {
            if (m_componentBody.StandingOnValue.HasValue)
            {
                var point = Terrain.ToCell(m_componentBody.Position);
                int value = subsystemTerrain.Terrain.GetCellValueFast(point.X, point.Y, point.Z);
                bool flag;
                if (Terrain.ExtractContents(value) != RailBlock.Index)
                {
                    value = m_componentBody.StandingOnValue.Value;
                    flag = Terrain.ExtractContents(value) == RailBlock.Index;
                }
                else
                {
                    flag = true;
                }

                if (flag)
                {
                    if (SimulateRail(RailBlock.GetRailType(Terrain.ExtractData(value))))
                    {
                        m_componentBody.Rotation = currentRotation;
                        m_componentBody.Velocity += Speed * dt * m_componentBody.Matrix.Forward;
                    }
                    else
                    {
                        m_componentBody.Velocity = Vector3.Zero;
                    }
                }
                else
                {
                    m_componentBody.Velocity = Vector3.Zero;
                }
            }
        }

        bool SimulateRail(int railType)
        {
            if (RailBlock.IsCorner(railType))
            {
                if (AtCenter(m_componentBody.Position))
                {
                    if ((m_componentBody.Position - lastTurnedPosition).LengthSquared() > 0.8f)
                        return Turn(railType);
                }
                return true;
            }
            if (RailBlock.IsDirectionX(railType) ^ !RailBlock.IsDirectionX(m_forwardDirection))
            {
                if (railType > 5)
                {
                    currentRotation = uprightDirection * directions[Direction];
                }
                else
                {
                    currentRotation = directions[Direction];
                }
                return true;
            }
            return false;
        }

        bool Turn(int turnType)
        {
            if (Direction == turnType)
            {
                Direction = (Direction - 1) & 3;
                m_componentBody.Velocity = MathUtils.Abs(m_componentBody.Velocity.X + m_componentBody.Velocity.Z) * forwardVector;
                m_componentBody.Position = Vector3.Floor(m_componentBody.Position) + center;
                lastTurnedPosition = m_componentBody.Position;
                return true;
            }
            else if (((Direction - 1) & 3) == turnType)
            {
                Direction = (Direction + 1) & 3;
                m_componentBody.Velocity = MathUtils.Abs(m_componentBody.Velocity.X + m_componentBody.Velocity.Z) * forwardVector;
                m_componentBody.Position = Vector3.Floor(m_componentBody.Position) + center;
                lastTurnedPosition = m_componentBody.Position;
                return true;
            }
            else
            {
                return false;
            }
        }

        static bool AtCenter(Vector3 vec)
        {
            var v = vec - Vector3.Floor(vec);
            return v.X > 0.45f && v.X < 0.55f && v.Z > 0.45f && v.Z < 0.55f;
        }
    }
}
