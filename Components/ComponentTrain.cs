using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
    public class ComponentTrain : ComponentInventoryBase, IUpdateable
    {
        static Vector3 center = new Vector3(0.5f, 0, 0.5f);

        Quaternion[] directions = new Quaternion[]
        {
            Quaternion.CreateFromAxisAngle(Vector3.UnitY, 0),
            Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathUtils.PI * 0.5f),
            Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathUtils.PI),
            Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathUtils.PI * 1.5f)
        };

        Quaternion upwardDirection = Quaternion.CreateFromAxisAngle(new Vector3(1, 0, 0), MathUtils.PI * 0.25f);
        Quaternion downwardDirection = Quaternion.CreateFromAxisAngle(new Vector3(1, 0, 0), MathUtils.PI * -0.25f);

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

        float m_remainedBurnTime = -1;

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
        }

        private void CollidedWithBody(ComponentBody obj)
        {
            var health = obj.Entity.FindComponent<ComponentHealth>();
            if (health != null)
                health.Injure(1 / health.AttackResilience, null, false, "Train");
        }

        public void SetDirectionImmediately(int value)
        {
            Direction = value;
            m_componentBody.Rotation = currentRotation;
        }

        public void Update(float dt)
        {
            if (m_remainedBurnTime < 0)
                return;

            if (m_componentMount.Rider != null)
            {
                var player = m_componentMount.Rider.Entity.FindComponent<ComponentPlayer>();
                player.ComponentLocomotion.LookOrder = new Vector2(player.ComponentInput.PlayerInput.Look.X, player.ComponentLocomotion.LookOrder.Y);
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

            if (m_componentBody.StandingOnValue.HasValue)
            {
                var result = subsystemTerrain.Raycast(m_componentBody.Position, m_componentBody.Position + new Vector3(0, -3.0f, 0), false, true, null);
                
                if (result.HasValue && Terrain.ExtractContents(result.Value.Value) == RailBlock.Index)
                {
                    if (SimulateRail(RailBlock.GetRailType(Terrain.ExtractData(result.Value.Value))))
                    {
                        m_componentBody.Velocity += Speed * dt * currentRotation.ToForwardVector();
                    }
                }
            }
            m_componentBody.Rotation = Quaternion.Slerp(m_componentBody.Rotation, currentRotation, 0.15f);
            m_remainedBurnTime -= dt;
        }

        bool SimulateRail(int railType)
        {
            if (RailBlock.IsCorner(railType))
            {
                if (GetOffsetOnDirection(m_componentBody.Position, m_forwardDirection) > 0.5f)
                {
                    Turn(railType);
                }
                return true;
            }
            if (RailBlock.IsDirectionX(railType) ^ !RailBlock.IsDirectionX(m_forwardDirection))
            {
                if (railType > 5)
                {
                    if (railType - 6 != Direction)
                    {
                        currentRotation = directions[Direction] * upwardDirection;
                    }
                    else
                    {
                        currentRotation = directions[Direction] * downwardDirection;
                    }
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
                return true;
            }
            else if (((Direction - 1) & 3) == turnType)
            {
                Direction = (Direction + 1) & 3;
                m_componentBody.Velocity = MathUtils.Abs(m_componentBody.Velocity.X + m_componentBody.Velocity.Z) * forwardVector;
                m_componentBody.Position = Vector3.Floor(m_componentBody.Position) + center;
                return true;
            }
            else
            {
                return false;
            }
        }

        static float GetOffsetOnDirection(Vector3 vec, int direction)
        {
            float offset = (direction & 1) == 0 ? vec.Z - MathUtils.Floor(vec.Z) : vec.X - MathUtils.Floor(vec.X);
            return (direction & 2) == 0 ? 1 - offset : offset;
        }
    }
}
