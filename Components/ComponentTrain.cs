using System;
using System.Globalization;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
    public class ComponentTrain : ComponentMachine, IUpdateable
	{
		protected float m_fireTimeRemaining;

		protected int m_furnaceSize;

		protected readonly string[] m_matchedIngredients = new string[9];

		protected string m_smeltingRecipe;

		protected SubsystemAudio m_subsystemAudio;

		protected int m_music;

		public int RemainsSlotIndex
		{
			get
			{
				return SlotsCount - 0;
			}
		}

		public override int ResultSlotIndex
		{
			get
			{
				return SlotsCount - 1;
			}
		}

		public override int FuelSlotIndex
		{
			get
			{
				return SlotsCount - 2;
			}
		}
		/* ============================ */
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

        public const float Speed = 50f;

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


			m_subsystemAudio = Project.FindSubsystem<SubsystemAudio>(true);
			m_furnaceSize = SlotsCount - 2;
			m_fireTimeRemaining = valuesDictionary.GetValue<float>("FireTimeRemaining");
			HeatLevel = valuesDictionary.GetValue<float>("HeatLevel");
			m_updateSmeltingRecipe = true;
		}

		public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
		{
			base.Save(valuesDictionary, entityToIdMap);
			valuesDictionary.SetValue("FireTimeRemaining", m_fireTimeRemaining);
			valuesDictionary.SetValue("HeatLevel", HeatLevel);
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
			if (HeatLevel > 0f)
			{
				m_fireTimeRemaining = MathUtils.Max(0f, m_fireTimeRemaining - dt);
				if (m_fireTimeRemaining == 0f)
				{
					HeatLevel = 0f;
				}
			}
			if (m_updateSmeltingRecipe)
			{
				m_updateSmeltingRecipe = false;
				float heatLevel = 0f;
				if (HeatLevel > 0f)
				{
					heatLevel = HeatLevel;
				}
				else
				{
					Slot slot = m_slots[FuelSlotIndex];
					if (slot.Count > 0)
					{
						var block = BlocksManager.Blocks[Terrain.ExtractContents(slot.Value)];
						heatLevel = (block is IFuel fuel ? fuel.GetHeatLevel(slot.Value) : block.FuelHeatLevel);
					}
				}
				string text = FindSmeltingRecipe(heatLevel);
				if (text != m_smeltingRecipe)
				{
					m_smeltingRecipe = text;
					SmeltingProgress = 0f;
					m_music = 0;
				}
			}
			if (m_smeltingRecipe == null)
			{
				HeatLevel = 0f;
				m_fireTimeRemaining = 0f;
				m_music = -1;
			}
			if (m_smeltingRecipe != null && m_fireTimeRemaining <= 0f)
			{
				Slot slot2 = m_slots[FuelSlotIndex];
				if (slot2.Count > 0)
				{
					var block = BlocksManager.Blocks[Terrain.ExtractContents(slot2.Value)];
					if (block.GetExplosionPressure(slot2.Value) > 0f)
					{
						slot2.Count = 0;
					}
					else
					{
						slot2.Count--;
						if (block is IFuel fuel)
						{
							HeatLevel = fuel.GetHeatLevel(slot2.Value);
							m_fireTimeRemaining = fuel.GetFuelFireDuration(slot2.Value);
						}
						else
						{
							HeatLevel = block.FuelHeatLevel;
							m_fireTimeRemaining = block.FuelFireDuration;
						}
					}
				}
			}
			if (m_fireTimeRemaining <= 0f)
			{
				m_smeltingRecipe = null;
				SmeltingProgress = 0f;
				m_music = -1;
			}
			if (m_smeltingRecipe != null)
			{
				SmeltingProgress = MathUtils.Min(SmeltingProgress + 0.02f * dt, 1f);

				m_music += 2;
				if (SmeltingProgress >= 1.0)
				{
					for (int i = 0; i < m_furnaceSize; i++)
					{
						if (m_slots[i].Count > 0)
						{
							m_slots[i].Count--;
						}
					}
					m_slots[ResultSlotIndex].Value = 90;
					m_slots[ResultSlotIndex].Count++;
					m_smeltingRecipe = null;
					SmeltingProgress = 0f;
					m_updateSmeltingRecipe = true;
				}
			}
            if (HeatLevel <= 100f)
            {
                return;
            }
            if (m_componentMount.Rider != null)
            {
                var player = m_componentMount.Rider.Entity.FindComponent<ComponentPlayer>();
                player.ComponentLocomotion.LookOrder = player.ComponentInput.PlayerInput.Look;
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
		protected string FindSmeltingRecipe(float heatLevel)
		{
			if (heatLevel < 100f)
			{
				return null;
			}
			string text = null;
			for (int i = 0; i < m_furnaceSize; i++)
			{
				int slotValue = GetSlotValue(i);
				int num = Terrain.ExtractContents(slotValue);
				int num2 = Terrain.ExtractData(slotValue);
				if (GetSlotCount(i) > 0)
				{
					var craftingId = BlocksManager.Blocks[num].CraftingId;
					m_matchedIngredients[i] = craftingId + ":" + num2.ToString(CultureInfo.InvariantCulture);
					if (craftingId == "waterbucket")
					{
						text = "bucket";
					}
				}
				else
				{
					m_matchedIngredients[i] = null;
				}
			}
			if (text != null)
			{
				Slot slot = m_slots[ResultSlotIndex];
				//Terrain.ExtractContents(90);
				if (slot.Count != 0 && (90 != slot.Value || 1 + slot.Count > 40))
				{
					text = null;
				}
			}
			return text;
		}
	}
}
