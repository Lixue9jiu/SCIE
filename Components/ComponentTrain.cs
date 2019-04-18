using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	public class ComponentTrain : ComponentMachine, IUpdateable
	{

		protected string m_smeltingRecipe;

		//protected int m_music;

		public override int RemainsSlotIndex => SlotsCount - 0;

		public override int ResultSlotIndex => SlotsCount - 1;

		public override int FuelSlotIndex => SlotsCount - 2;
		/* ============================ */
		static readonly Vector3 center = new Vector3(0.5f, 0, 0.5f);
		static readonly Quaternion[] directions = new Quaternion[]
		{
			Quaternion.CreateFromAxisAngle(Vector3.UnitY, 0),
			Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathUtils.PI * 0.5f),
			Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathUtils.PI),
			Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathUtils.PI * 1.5f)
		};

		static readonly Quaternion upwardDirection = Quaternion.CreateFromAxisAngle(new Vector3(1, 0, 0), MathUtils.PI * 0.25f);
		static readonly Quaternion downwardDirection = Quaternion.CreateFromAxisAngle(new Vector3(1, 0, 0), MathUtils.PI * -0.25f);
		static readonly Vector3[] forwardVectors = new Vector3[]
		{
			new Vector3(0, 0, -1),
			new Vector3(-1, 0, 0),
			new Vector3(0, 0, 1),
			new Vector3(1, 0, 0)
		};
		
		ComponentBody m_componentBody;
		ComponentMount m_componentMount;
		int m_forwardDirection;
		Quaternion rotation;
		Vector3 forwardVector;

		public const float Speed = 50f;

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
			m_componentMount = Entity.FindComponent<ComponentMount>(true);

			m_componentBody.CollidedWithBody += CollidedWithBody;

			m_furnaceSize = SlotsCount - 2;
			m_fireTimeRemaining = valuesDictionary.GetValue("FireTimeRemaining", 0f);
			HeatLevel = valuesDictionary.GetValue("HeatLevel", 0f);
			Direction = valuesDictionary.GetValue("Direction", 0);
			m_updateSmeltingRecipe = true;
		}

		public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
		{
			this.SaveItems(valuesDictionary);
			valuesDictionary.SetValue("FireTimeRemaining", m_fireTimeRemaining);
			valuesDictionary.SetValue("HeatLevel", HeatLevel);
			valuesDictionary.SetValue("Direction", m_forwardDirection);
		}

		public void CollidedWithBody(ComponentBody body)
		{
			Vector3 v = m_componentBody.Velocity;
			float amount = v.LengthSquared() * 0.5f;
			if (amount < .02f) return;
			var health = body.Entity.FindComponent<ComponentHealth>();
			if (health != null)
				health.Injure(amount / health.AttackResilience, null, false, "Train");
			else
				body.Entity.FindComponent<ComponentDamage>()?.Damage(amount);
			body.ApplyImpulse(MathUtils.Clamp(1.25f * 6f * MathUtils.Pow(m_componentBody.Mass / body.Mass, 0.33f), 0f, 6f) * Vector3.Normalize(body.Position - m_componentBody.Position));
		}

		public void SetDirection(int value)
		{
			Direction = value;
			m_componentBody.Rotation = rotation;
		}

		public void Update(float dt)
		{
			if (HeatLevel > 0f)
			{
				m_fireTimeRemaining = MathUtils.Max(0f, m_fireTimeRemaining - dt);
				if (m_fireTimeRemaining == 0f)
					HeatLevel = 0f;
			}
			Slot slot;
			if (m_updateSmeltingRecipe)
			{
				m_updateSmeltingRecipe = false;
				float heatLevel = 0f;
				if (HeatLevel > 0f)
					heatLevel = HeatLevel;
				else
				{
					slot = m_slots[FuelSlotIndex];
					if (slot.Count > 0)
					{
						var block = BlocksManager.Blocks[Terrain.ExtractContents(slot.Value)];
						heatLevel = block is IFuel fuel ? fuel.GetHeatLevel(slot.Value) : block.FuelHeatLevel;
					}
				}
				string text = FindSmeltingRecipe(heatLevel);
				if (text != m_smeltingRecipe)
				{
					m_smeltingRecipe = text;
					SmeltingProgress = 0f;
					//m_music = 0;
				}
			}
			if (m_smeltingRecipe == null)
			{
				HeatLevel = 0f;
				m_fireTimeRemaining = 0f;
				//m_music = -1;
			}
			if (m_smeltingRecipe != null && m_fireTimeRemaining <= 0f)
			{
				slot = m_slots[FuelSlotIndex];
				if (slot.Count > 0)
				{
					var block = BlocksManager.Blocks[Terrain.ExtractContents(slot.Value)];
					if (block.GetExplosionPressure(slot.Value) > 0f)
						slot.Count = 0;
					else
					{
						slot.Count--;
						if (block is IFuel fuel)
						{
							HeatLevel = fuel.GetHeatLevel(slot.Value);
							m_fireTimeRemaining = fuel.GetFuelFireDuration(slot.Value);
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
				//m_music = -1;
			}
			if (m_smeltingRecipe != null)
			{
				SmeltingProgress = MathUtils.Min(SmeltingProgress + 0.02f * dt, 1f);

				//m_music++;
				if (SmeltingProgress >= 1.0)
				{
					for (int i = 0; i < m_furnaceSize; i++)
						if (m_slots[i].Count > 0)
							m_slots[i].Count--;
					m_slots[ResultSlotIndex].Value = 90;
					m_slots[ResultSlotIndex].Count++;
					m_smeltingRecipe = null;
					SmeltingProgress = 0f;
					m_updateSmeltingRecipe = true;
				}
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

			if (HeatLevel >= 100f && m_componentBody.StandingOnValue.HasValue)
			{
				var result = Utils.SubsystemTerrain.Raycast(m_componentBody.Position, m_componentBody.Position + new Vector3(0, -3.0f, 0), false, true, null);

				if (result.HasValue && Terrain.ExtractContents(result.Value.Value) == RailBlock.Index)
					if (SimulateRail(RailBlock.GetRailType(Terrain.ExtractData(result.Value.Value))))
						m_componentBody.Velocity += Speed * dt * rotation.ToForwardVector();
			}
			m_componentBody.Rotation = Quaternion.Slerp(m_componentBody.Rotation, rotation, 0.15f);
		}

		bool SimulateRail(int railType)
		{
			if (RailBlock.IsCorner(railType))
			{
				if (GetOffsetOnDirection(m_componentBody.Position, m_forwardDirection) > 0.5f)
					Turn(railType);
				return true;
			}
			if (RailBlock.IsDirectionX(railType) ^ !RailBlock.IsDirectionX(m_forwardDirection))
			{
				rotation = railType > 5
					? railType - 6 != Direction ? directions[Direction] * upwardDirection : directions[Direction] * downwardDirection
					: directions[Direction];
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

		protected string FindSmeltingRecipe(float heatLevel)
		{
			if (heatLevel < 100f)
				return null;
			string text = null;
			for (int i = 0; i < m_furnaceSize; i++)
			{
				int slotValue = GetSlotValue(i);
				int num = Terrain.ExtractContents(slotValue);
				int num2 = Terrain.ExtractData(slotValue);
				if (GetSlotCount(i) > 0)
				{
					var craftingId = BlocksManager.Blocks[num].CraftingId;
					if (craftingId.Equals("waterbucket"))
						text = "bucket";
				}
			}
			if (text != null)
			{
				Slot slot = m_slots[ResultSlotIndex];
				if (slot.Count != 0 && (90 != slot.Value || slot.Count >= 40))
					text = null;
			}
			return text;
		}
	}
}