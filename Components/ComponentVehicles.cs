using Engine;
using Engine.Graphics;
using GameEntitySystem;
using System.Collections.Generic;
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

		public void CollidedWithBody(ComponentBody body)
		{
			Vector2 v = m_componentBody.Velocity.XZ - body.Velocity.XZ;
			float amount = v.LengthSquared() * .3f;
			if (amount < .02f || m_componentBody.Velocity.XZ.LengthSquared() < 1f || m_componentBody.Mass <300) return;
			var health = body.Entity.FindComponent<ComponentHealth>();
			if (health != null)
				health.Injure(amount / health.AttackResilience, null, false, "Struck by a plane");
			else
				body.Entity.FindComponent<ComponentDamage>()?.Damage(amount);
			m_componentBody.Entity.FindComponent<ComponentHealth>().Injure(amount / health.AttackResilience, null, false, "Accident");
			body.ApplyImpulse(MathUtils.Clamp(2.5f * MathUtils.Pow(m_componentBody.Mass / body.Mass, 0.33f), 0f, 6f) * Vector3.Normalize(body.Position - m_componentBody.Position));
		}

		public Model m_model;
		public ModelBone m_bodyBone,
						 m_headBone;
		public Matrix?[] m_boneTransforms;
		public Matrix m_headT = ContentManager.Get<Model>("Models/Plane").FindBone("Body").Transform;

		public Matrix? GetBoneTransform(int boneIndex)
		{
			return m_boneTransforms[boneIndex];
		}

		public void SetBoneTransform(int boneIndex, Matrix? transformation)
		{
			m_boneTransforms[boneIndex] = transformation;
		}

		public void SetModel(Model model)
		{
			m_model = model;
			if (m_model != null)
			{
				m_boneTransforms = new Matrix?[m_model.Bones.Count];
				m_bodyBone = m_model.FindBone("Body");
				m_headT = ContentManager.Get<Model>("Models/Plane").FindBone("Body").Transform;
				//m_boneTransforms[0] = m_bodyBone;
			}
			else
			{
				m_bodyBone = null;
				m_headBone = null;
				m_boneTransforms = null;
			}
		}

		public float time = 0f;
		public new void Update(float dt)
		{
			var componentBody = m_componentBody;
			componentBody.IsGravityEnabled = true;
			Quaternion rotation = componentBody.Rotation;
			if (componentBody.Mass == 250)
			{
				componentBody.IsGravityEnabled = false;
				componentBody.IsGroundDragEnabled = false;
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
			}else
			{
				float dt2 = 0.2f;
				//	if (m_componentBody.StandingOnValue == null)
				//	m_componentBody.Position -= 9f * dt * new Vector3(0f, 1f, 0f);
				//componentBody.IsGravityEnabled = true;
				//componentBody.ImmersionFactor = 0f;
				componentBody.IsGravityEnabled = true;
				componentBody.IsGroundDragEnabled = true;
				ComponentRider rider = m_componentMount.Rider;
				if (componentEngine.HeatLevel == 0f)
				{
					componentEngine.velo -= dt2 * 1f;
				//	m_bodyBone.Transform = m_headT;
					componentEngine.velo = MathUtils.Max(componentEngine.velo, 0f);
					componentBody.m_velocity.XZ = componentEngine.velo * m_componentBody.Matrix.Forward.XZ;
					//m_componentBody.Position += componentEngine.velo * m_componentBody.Matrix.Forward *dt;
				}
				if (componentEngine.HeatLevel > 0f)
				{
					componentEngine.velo += dt2 * 5f;
					//float num3 = 1f + dt *
					time = time + dt2 * 100f;
					//Matrix.
					if (rider !=null)
					m_bodyBone.Transform = m_headT * Matrix.CreateTranslation(0f, 0f, -120f) * Matrix.CreateRotationY(time) * Matrix.CreateTranslation(0f, 0f, +120f);
					//m_bodyBone.Transform = m_headT * Matrix.CreateRotationY(time);
					componentEngine.velo = MathUtils.Min(componentEngine.velo,100f);
					//m_componentBody.m_velocity = componentEngine.velo * m_componentBody.Matrix.Forward;
					componentBody.m_velocity.XZ = componentEngine.velo * m_componentBody.Matrix.Forward.XZ;
				}
				
				if (rider != null && MoveOrder!=0 && componentEngine.HeatLevel > 0f)
				{
					//float num2 = 0f;
					if ((MoveOrder>0 && componentEngine.velo > 40f) || MoveOrder<0f)
					componentBody.m_velocity += dt2 * 5f * new Vector3(0f,1f,0f)*MoveOrder;
				//	float num2 = MathUtils.Atan2(2f * rotation.X * rotation.Z - 2f * rotation.W * rotation.Y, 1f - 2f * rotation.X * rotation.X - 2f * rotation.Y * rotation.Y);
				//	if ((m_turnSpeed += 2.5f * Utils.SubsystemTime.GameTimeDelta * (MoveOrder - m_turnSpeed)) != 0)
				//		num2 -= m_turnSpeed * dt;
				//	componentBody.Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitX,num2);
					//componentBody.Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MoveOrder*dt);
				}
				if (componentEngine.velo < 40f && !m_componentBody.StandingOnValue.HasValue)
				{
					componentBody.m_velocity -= dt2 * 5f * new Vector3(0f, 1f, 0f);
				}
				if (componentEngine.velo > 40f && MoveOrder==0)
				{
					componentBody.m_velocity = new Vector3(componentBody.m_velocity.X, 0,componentBody.m_velocity.Z); 
				}
				if (componentBody.m_position.Y>=120f)
				{
					componentBody.m_velocity = new Vector3(componentBody.m_velocity.X, 0, componentBody.m_velocity.Z);
				}
				float num = MathUtils.Atan2(2f * rotation.Y * rotation.W - 2f * rotation.X * rotation.Z, 1f - 2f * rotation.Y * rotation.Y - 2f * rotation.Z * rotation.Z);
				if ((m_turnSpeed += 2.5f * Utils.SubsystemTime.GameTimeDelta * (TurnOrder - m_turnSpeed)) != 0)
					num -= m_turnSpeed * dt;
				componentBody.Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, num);
				if (componentBody.ImmersionFactor > 0.95f)
				{
					m_componentDamage.Damage(0.005f * dt);
					if (rider != null)
						rider.StartDismounting();
				}
				float num4 = MathUtils.Abs(m_componentBody.CollisionVelocityChange.Length());
				if (num4 > 50 && componentEngine.velo>40)
				{
					if (num4 > 75)
						Utils.SubsystemExplosions.AddExplosion((int)m_componentBody.Position.X, (int)m_componentBody.Position.Y, (int)m_componentBody.Position.Z,100f,true,false);
					float amount = MathUtils.Sqr(MathUtils.Max(num4 - 5, 0f)) / 15f;
					m_componentDamage.Damage(amount);
					
				}
				MoveOrder = 0f;	
				TurnOrder = 0f;
			}
		}

		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			m_componentMount = Entity.FindComponent<ComponentMount>(true);
			m_componentBody = Entity.FindComponent<ComponentBody>(true);
			m_componentBody.ImmersionFactor = 0f;
			m_componentDamage = Entity.FindComponent<ComponentDamage>(true);
			componentEngine = Entity.FindComponent<ComponentEngineA>();
			SetModel(ContentManager.Get<Model>("Models/Plane"));
		}
	}

	public class ComponentCar : ComponentBoatI, IUpdateable
	{
		protected ComponentEngineA componentEngine;
		protected ComponentEngineT componentEngine2;
		protected ComponentEngineT2 componentEngine3;
		protected ComponentEngineT3 componentEngine4;
		protected ComponentEngineT4 componentEngine5;
		protected int use = 0;
		protected bool jump = false;

		public new void Update(float dt)
		{
			var componentBody = m_componentBody;
			if (componentEngine4 != null)
			{
				componentBody.IsGravityEnabled = true;
				componentBody.IsGroundDragEnabled = true;
				Quaternion rotation = componentBody.Rotation;
				float num = MathUtils.Atan2(2f * rotation.Y * rotation.W - 2f * rotation.X * rotation.Z, 1f - 2f * rotation.Y * rotation.Y - 2f * rotation.Z * rotation.Z);
				if ((m_turnSpeed += 2.5f * Utils.SubsystemTime.GameTimeDelta * (TurnOrder - m_turnSpeed)) != 0 && componentEngine4.HeatLevel > 0f)
					num -= m_turnSpeed * dt;
				componentBody.Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, num);
				ComponentRider rider = m_componentMount.Rider;
				if (componentBody.StandingOnValue.HasValue)
					jump = false;
				if (MoveOrder != 0f)
				{
					if (rider != null && componentBody.StandingOnValue.HasValue && componentEngine4 != null && componentEngine4.HeatLevel > 0f)
					{
						//jump = false;
						m_componentBody.Velocity += dt * 15f * MoveOrder * m_componentBody.Matrix.Forward;
						//componentBody.IsSmoothRiseEnabled = true;
						//	Vector3 newp = m_componentBody.Position + new Vector3(m_componentBody.Velocity.X, 0f, m_componentBody.Velocity.Z);
						//	if ((newp.Y - m_componentBody.Position.Y) <2f)
						//	{
						var p2 = Vector3.Normalize(componentBody.Rotation.ToForwardVector() * MoveOrder);
						var p = Terrain.ToCell(componentBody.Position + p2 * 1.0f + new Vector3(0f, 0.5f, 0f));
						int value2 = Utils.Terrain.GetCellContentsFast(p.X, p.Y, p.Z);
						var block = BlocksManager.Blocks[Terrain.ExtractContents(value2)];
						if (value2 != 0 && !block.IsTransparent)
						{
							m_componentBody.Velocity += 0.012f * 15f * new Vector3(0f, 30f, 0f);
							jump = true;
						}
						MoveOrder = 0f;
					}
					if (jump)
					{
						m_componentBody.Velocity += dt * 15f * m_componentBody.Matrix.Forward;
						//jump = false;
					}
				}

				//if (jum)
				//componentBody.
				//rider = m_componentMount.Rider;
				if (rider != null)
				{
					var xian = new Vector3(rider.ComponentCreature.ComponentCreatureModel.EyeRotation.ToForwardVector().X, 0f, rider.ComponentCreature.ComponentCreatureModel.EyeRotation.ToForwardVector().Z);
					var xian1 = Vector3.Normalize(xian);
					var xian2 = Vector3.Normalize(m_componentBody.Matrix.Forward);
					float num4 = xian1.X * xian2.X + xian1.Z * xian2.Z;
					float num2 = MathUtils.Acos(num4) * MathUtils.Sign(xian1.X * xian2.Z - xian1.Z * xian2.X);
					//Vector3 xian3 = Vector3.Normalize(new Vector3(rider.ComponentCreature.ComponentCreatureModel.EyeRotation.ToForwardVector().X, rider.ComponentCreature.ComponentCreatureModel.EyeRotation.ToForwardVector().Y, 0f));
					//float num5 = xian1.X * xian2.X + xian1.Y * xian2.Y;
					//float num6 = MathUtils.Acos(num5) * MathUtils.Sign(xian3.X * xian2.Y - xian3.Y * xian2.X);
					//float num44 = 0.1f;
					//SetBoneTransform(m_headBone.Index, Matrix.CreateRotationY(num2));
					//m_headBone.Transform.Forward = xian1;
					if (num2 > 0 || num2 < 0)
						m_headBone.Transform = m_headT * Matrix.CreateRotationZ(num2);
				}
				if (componentBody.ImmersionFactor > 0.95f)
				{
					m_componentDamage.Damage(0.005f * dt);
					if (rider != null)
						rider.StartDismounting();
				}
				TurnOrder = 0f;
				if (componentEngine4.HeatLevel <= 0f || !componentBody.StandingOnValue.HasValue)
					return;
				if (rider == null)
					return;
				ComponentMiner componentMiner = rider.Entity.FindComponent<ComponentMiner>();

				int num22 = Terrain.ExtractContents(componentMiner.ActiveBlockValue);

				if (componentEngine4.HeatLevel == 489f && Utils.SubsystemTime.PeriodicGameTimeEvent(0.20, 0) && num22 == 0)
				{
					int abb = -1;
					int flag = 0;
					for (int i = 0; i < 9; i++)
					{
						int va1 = componentEngine4.GetSlotValue(i);
						if (va1 == Terrain.MakeBlockValue(521, 0, Bullet2Block.SetBulletType(0, Bullet2Block.BulletType.HandBullet)))
						{
							abb = i;
							flag = -1;
							i += 6;
						}
					}
					if (flag == -1)
					{
						var xian77 = new Vector3(rider.ComponentCreature.ComponentCreatureModel.EyeRotation.ToForwardVector().X, MathUtils.Clamp(rider.ComponentCreature.ComponentCreatureModel.EyeRotation.ToForwardVector().Y, -0.05f, 0.05f), rider.ComponentCreature.ComponentCreatureModel.EyeRotation.ToForwardVector().Z);
						var xian88 = new Vector3(componentEngine4.vet1.X, MathUtils.Clamp(componentEngine4.vet1.Y, -0.2f, 0.2f), componentEngine4.vet1.Z);
						//m_subsystemProjectiles.FireProjectile(value2, vector2 + 1.3f * dir, s * (vector31 + v4), Vector3.Zero, componentMiner.ComponentCreature);
						Utils.SubsystemProjectiles.FireProjectile(Terrain.MakeBlockValue(214, 0, BulletBlock.SetBulletType(0, BulletBlock.BulletType.MiniBullet)), componentBody.Position + Vector3.Normalize(xian88) * 1.8f + new Vector3(0f, 1.5f, 0f), Vector3.Normalize(xian88) * 200f, Vector3.Zero, null);
						Utils.SubsystemAudio.PlaySound("Audio/MusketFire", 1f, 0f, componentBody.Position, 10f, true);
						componentBody.m_subsystemParticles.AddParticleSystem(new GunSmokeParticleSystem2(Utils.SubsystemTerrain, componentBody.Position + Vector3.Normalize(xian88) * 1.8f + new Vector3(0f, 1.8f, 0f), xian77));
						//Utils.
						//componentMiner.Inventory.ActiveSlotIndex;
						componentEngine4.m_slots[abb].Count -= 1;
					}
					//componentMiner.Inventory_.RemoveSlotItems(componentMiner.Inventory.ActiveSlotIndex,1);
					componentEngine4.HeatLevel = 499f;
				}
				if (componentEngine4.HeatLevel == 490f && num22 == 0)
				{
					int abb = -1;
					int flag = 0;
					for (int i = 0; i < 9; i++)
					{
						int va1 = componentEngine4.GetSlotValue(i);
						if (va1 == Terrain.MakeBlockValue(521, 0, Bullet2Block.SetBulletType(0, Bullet2Block.BulletType.Shell)))
						{
							abb = i;
							flag = 1;
							i += 6;
						}
						if (va1 == Terrain.MakeBlockValue(521, 0, Bullet2Block.SetBulletType(0, Bullet2Block.BulletType.UShell)))
						{
							abb = i;
							flag = 2;
							i += 6;
						}
					}
					if (flag == 1)
					{
						var xian77 = new Vector3(rider.ComponentCreature.ComponentCreatureModel.EyeRotation.ToForwardVector().X, MathUtils.Clamp(rider.ComponentCreature.ComponentCreatureModel.EyeRotation.ToForwardVector().Y, -0.05f, 0.2f), rider.ComponentCreature.ComponentCreatureModel.EyeRotation.ToForwardVector().Z);
						//m_subsystemProjectiles.FireProjectile(value2, vector2 + 1.3f * dir, s * (vector31 + v4), Vector3.Zero, componentMiner.ComponentCreature);
						var xian88 = new Vector3(componentEngine4.vet1.X, MathUtils.Clamp(componentEngine4.vet1.Y, -0.2f, 0.2f), componentEngine4.vet1.Z);
						Utils.SubsystemProjectiles.FireProjectile(Terrain.MakeBlockValue(214, 0, BulletBlock.SetBulletType(0, BulletBlock.BulletType.Shell)), componentBody.Position + Vector3.Normalize(xian88) * 1.8f + new Vector3(0f, 1.5f, 0f), Vector3.Normalize(xian88) * 100f, Vector3.Zero, null);
						Utils.SubsystemAudio.PlaySound("Audio/MusketFire", 1f, 0f, componentBody.Position, 10f, true);
						componentBody.m_subsystemParticles.AddParticleSystem(new GunSmokeParticleSystem2(Utils.SubsystemTerrain, componentBody.Position + Vector3.Normalize(xian88) * 1.8f + new Vector3(0f, 1.8f, 0f), xian77));
						//Utils.
						componentEngine4.m_slots[abb].Count -= 1;
					}
					if (flag == 2)
					{
						var xian77 = new Vector3(rider.ComponentCreature.ComponentCreatureModel.EyeRotation.ToForwardVector().X, MathUtils.Clamp(rider.ComponentCreature.ComponentCreatureModel.EyeRotation.ToForwardVector().Y, -0.05f, 0.2f), rider.ComponentCreature.ComponentCreatureModel.EyeRotation.ToForwardVector().Z);
						//m_subsystemProjectiles.FireProjectile(value2, vector2 + 1.3f * dir, s * (vector31 + v4), Vector3.Zero, componentMiner.ComponentCreature);
						var xian88 = new Vector3(componentEngine4.vet1.X, MathUtils.Clamp(componentEngine4.vet1.Y, -0.2f, 0.2f), componentEngine4.vet1.Z);
						Utils.SubsystemProjectiles.FireProjectile(Terrain.MakeBlockValue(214, 0, BulletBlock.SetBulletType(0, BulletBlock.BulletType.UShell)), componentBody.Position + Vector3.Normalize(xian88) * 1.8f + new Vector3(0f, 1.5f, 0f), Vector3.Normalize(xian88) * 100f, Vector3.Zero, null);
						Utils.SubsystemAudio.PlaySound("Audio/MusketFire", 1f, 0f, componentBody.Position, 10f, true);
						componentBody.m_subsystemParticles.AddParticleSystem(new GunSmokeParticleSystem2(Utils.SubsystemTerrain, componentBody.Position + Vector3.Normalize(xian88) * 1.8f + new Vector3(0f, 1.8f, 0f), xian77));
						//Utils.
						componentEngine4.m_slots[abb].Count -= 1;
					}
					//componentMiner.Inventory.ActiveSlotIndex;
					//componentMiner.Inventory_.RemoveSlotItems(componentMiner.Inventory.ActiveSlotIndex, 1);
					componentEngine4.HeatLevel = 500f;
				}
				return;
			}
			if (componentEngine3 != null)
			{
				componentBody.IsGravityEnabled = true;
				componentBody.IsGroundDragEnabled = false;
				Quaternion rotation = componentBody.Rotation;
				float num = MathUtils.Atan2(2f * rotation.Y * rotation.W - 2f * rotation.X * rotation.Z, 1f - 2f * rotation.Y * rotation.Y - 2f * rotation.Z * rotation.Z);
				if ((m_turnSpeed += 2.5f * Utils.SubsystemTime.GameTimeDelta * (TurnOrder - m_turnSpeed)) != 0 && componentEngine3.HeatLevel > 0f)
					num -= m_turnSpeed * dt;
				componentBody.Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, num);
				ComponentRider rider = m_componentMount.Rider;
				if (MoveOrder != 0f)
				{
					if (rider != null && componentBody.StandingOnValue.HasValue && componentEngine3 != null && componentEngine3.HeatLevel > 0f)
						m_componentBody.Velocity += dt * 4f * MoveOrder * m_componentBody.Matrix.Forward;
					MoveOrder = 0f;
				}
				if (componentBody.ImmersionFactor > 0.95f)
				{
					m_componentDamage.Damage(0.005f * dt);
					if (rider != null)
						rider.StartDismounting();
				}
				TurnOrder = 0f;
				if (componentEngine3.HeatLevel <= 0f || componentBody.Mass > 1200f || !componentBody.StandingOnValue.HasValue)
					return;
				//int value = Terrain.ExtractContents(componentBody.StandingOnValue.Value);
				if (componentEngine2.HeatLevel == 500f)
				{
					var p2 = Vector3.Normalize(componentBody.Rotation.ToForwardVector());
					var p3 = Vector3.Transform(p2, Matrix.CreateRotationY(MathUtils.PI / 2));
					for (int a = -1; a < 1 + 1; a++)
					{
						for (int b = 0; b < 3; b++)
						{
							var p = Terrain.ToCell(componentBody.Position + p2 * 1.5f + a * p3 + new Vector3(0f, 0.1f, 0f));
							int value2 = Utils.Terrain.GetCellContentsFast(p.X, p.Y + b, p.Z);
							int value444 = Utils.Terrain.GetCellValueFast(p.X, p.Y + b, p.Z);
							IInventory inventory = componentBody.Entity.FindComponent<ComponentEngineT2>(true);
							var block = BlocksManager.Blocks[value2];
							if (value2 > 0 && block.IsPlaceable && !block.IsDiggingTransparent && !block.DefaultIsInteractive && block.DefaultCategory == "Terrain")
							{
								if (ComponentInventoryBase.AcquireItems(inventory, value444, 1) < 1)
								{
									Utils.SubsystemTerrain.ChangeCell(p.X, p.Y + b, p.Z, 0);
								}
								else
								{
									Utils.SubsystemTerrain.ChangeCell(p.X, p.Y + b, p.Z, 0);
									Utils.SubsystemPickables.AddPickable(value2, 1, new Vector3(p.X, p.Y + b, p.Z) + new Vector3(0.5f), null, null);
									//m_subsystemPickables.AddPickable(v, 1, new Vector3(x2, y2, z2) + new Vector3(0.5f), null, null);
									//m_subsystemTerrain.DestroyCell(block2.ToolLevel, x2, y2, z2, digValue.Value, false, false);
									//a += 5;
									//b += 3;
								}
							}
						}
					}
				}
				return;
			}

			if (componentEngine5 != null)
			{
				componentBody.IsGravityEnabled = true;
				componentBody.IsGroundDragEnabled = true;
				Quaternion rotation = componentBody.Rotation;
				float num = MathUtils.Atan2(2f * rotation.Y * rotation.W - 2f * rotation.X * rotation.Z, 1f - 2f * rotation.Y * rotation.Y - 2f * rotation.Z * rotation.Z);
				if ((m_turnSpeed += 2.5f * Utils.SubsystemTime.GameTimeDelta * (TurnOrder - m_turnSpeed)) != 0 && componentEngine5.HeatLevel > 0f)
					num -= m_turnSpeed * dt;
				componentBody.Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, num);
				ComponentRider rider = m_componentMount.Rider;
				if (MoveOrder != 0f)
				{
					if (rider != null && componentBody.StandingOnValue.HasValue && componentEngine5 != null && componentEngine5.HeatLevel > 0f)
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
				if (componentEngine5.HeatLevel <= 0f || componentBody.Mass > 1200f || !componentBody.StandingOnValue.HasValue || m_componentBody.Velocity.LengthSquared() == 0f)
					return;
				if (componentEngine5.HeatLevel == 500f)
				{
					int aaaa = -1;
					int bbbb = 0;
					for (int i = 0; i < 9; i++)
					{
						int va1 = componentEngine5.GetSlotValue(i);
						//componentEngine5.
						int con = Terrain.ExtractContents(va1);
						int data = Terrain.ExtractData(va1);
						if (con == PaintBucketBlock.Index && componentEngine5.GetSlotCount(i) > 0)
						{
							aaaa = i;
							bbbb = PaintBucketBlock.GetColor(data);
							break;
						}
					}
					var p2 = Vector3.Normalize(componentBody.Rotation.ToForwardVector());
					var p3 = Vector3.Transform(p2, Matrix.CreateRotationY(MathUtils.PI / 2));
					//if (value == SoilBlock.Index)
					componentBody.IsSneaking = true;

					for (int aaa = -3; aaa < 3 + 1; aaa++)
					{
						var p = Terrain.ToCell(componentBody.Position + p2 * 2f + aaa * p3 - new Vector3(0f, 0.1f, 0f));
						int value2 = Utils.Terrain.GetCellContentsFast(p.X, p.Y, p.Z);
						int value4 = Utils.Terrain.GetCellValueFast(p.X, p.Y, p.Z);
						IInventory inventory = componentBody.Entity.FindComponent<ComponentEngineT4>(true);
						if (value2 == 0 && rider != null)
							for (int i = 0; i < 9; i++)
							{
								if (inventory.GetSlotValue(i) != 0 && BlocksManager.Blocks[Terrain.ExtractContents(inventory.GetSlotValue(i))].IsPlaceable)
								{
									int color2 = 0;
									int color;
									//BlocksManager.Blocks[Terrain.ExtractContents(inventory.GetSlotValue(i))].
									if (aaaa != -1 && (BlocksManager.Blocks[Terrain.ExtractContents(inventory.GetSlotValue(i))] is PaintableItemBlock || BlocksManager.Blocks[Terrain.ExtractContents(inventory.GetSlotValue(i))] is PaintedCubeBlock))
									{
										color2 = PaintBucketBlock.SetColor(Terrain.ExtractData(inventory.GetSlotValue(i)), bbbb);
										//color = 16384 * color2 + Terrain.ExtractContents(inventory.GetSlotValue(i));
										color = ((IPaintableBlock)BlocksManager.Blocks[Terrain.ExtractContents(inventory.GetSlotValue(i))]).Paint(Utils.SubsystemTerrain, inventory.GetSlotValue(i), bbbb);
										int value = inventory.GetSlotValue(aaaa);
										componentEngine5.RemoveSlotItems(aaaa, 1);
										componentEngine5.AddSlotItems(aaaa, BlocksManager.DamageItem(value, 1), 1);
									}
									else
									{
										color = inventory.GetSlotValue(i);
									}
									Utils.SubsystemTerrain.ChangeCell(p.X, p.Y, p.Z, color);
									inventory.RemoveSlotItems(i, 1);
									break;
									//componentMiner.Place(result.Value, value);
								}
							}
					}
				}
				if (componentEngine5.HeatLevel == 499f)
				{
					//bool flagg = false;
					//int use = 0;

					//int value; = Terrain.ExtractContents(componentBody.StandingOnValue.Value);
					var p2 = Vector3.Normalize(componentBody.Rotation.ToForwardVector());
					var p3 = Vector3.Transform(p2, Matrix.CreateRotationY(MathUtils.PI / 2));
					//if (value == SoilBlock.Index)
					componentBody.IsSneaking = true;
					int aaa = 0;
					var p = Terrain.ToCell(componentBody.Position - p2 * 2f + aaa * p3 + new Vector3(0f, 0.1f, 0f));
					int value2 = Utils.Terrain.GetCellContentsFast(p.X, p.Y, p.Z);
					int value4 = Utils.Terrain.GetCellValueFast(p.X, p.Y, p.Z);
					IInventory inventory = componentBody.Entity.FindComponent<ComponentEngineT4>(true);
					if (value2 == 0 && rider != null)
						for (int i = 0; i < 9; i++)
						{
							if (inventory.GetSlotValue(i) != 0)
							{
								var componentMiner = rider.Entity.FindComponent<ComponentMiner>();
								var result = Utils.SubsystemTerrain.Raycast(componentBody.Position + new Vector3(0f, 1.7f, 0f), componentBody.Position - p2 * 2f - new Vector3(0f, 0.2f, 0f) + aaa * p3, true, true, null);
								if (componentMiner != null && result.HasValue && componentMiner.Place(result.Value, inventory.GetSlotValue(i)))
								{
									inventory.RemoveSlotItems(i, 1);
									break;
								}
								//componentMiner.Place(result.Value, value);
							}
						}
				}
				return;
			}

			if (componentEngine2 != null)
			{
				componentBody.IsGravityEnabled = true;
				componentBody.IsGroundDragEnabled = true;
				Quaternion rotation = componentBody.Rotation;
				float num = MathUtils.Atan2(2f * rotation.Y * rotation.W - 2f * rotation.X * rotation.Z, 1f - 2f * rotation.Y * rotation.Y - 2f * rotation.Z * rotation.Z);
				if ((m_turnSpeed += 2.5f * Utils.SubsystemTime.GameTimeDelta * (TurnOrder - m_turnSpeed)) != 0 && componentEngine2.HeatLevel > 0f)
					num -= m_turnSpeed * dt;
				componentBody.Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, num);
				ComponentRider rider = m_componentMount.Rider;
				if (MoveOrder != 0f)
				{
					if (rider != null && componentBody.StandingOnValue.HasValue && componentEngine2 != null && componentEngine2.HeatLevel > 0f)
						m_componentBody.Velocity += dt * 7f * MoveOrder * m_componentBody.Matrix.Forward;
					MoveOrder = 0f;
				}
				if (componentBody.ImmersionFactor > 0.95f)
				{
					m_componentDamage.Damage(0.005f * dt);
					if (rider != null)
						rider.StartDismounting();
				}
				TurnOrder = 0f;
				if (componentEngine2.HeatLevel <= 0f || componentBody.Mass > 1200f || !componentBody.StandingOnValue.HasValue || m_componentBody.Velocity.LengthSquared() == 0f)
					return;
				if (componentEngine2.HeatLevel == 500f)
				{
					int ab = -1;
					//bool flagg = false;
					//int use = 0;
					for (int i = 0; i < 6; i++)
					{
						int va1 = componentEngine2.GetSlotValue(i);
						if (va1 == SaltpeterChunkBlock.Index && componentEngine2.GetSlotCount(i) > 0)
						{
							ab = i;
						}
					}
					if (ab >= 0 && use == 0)
					{
						componentEngine2.m_slots[ab].Count -= 1;
						//flagg = true;
						use = 9;
					}
					int value = Terrain.ExtractContents(componentBody.StandingOnValue.Value);
					var p2 = Vector3.Normalize(componentBody.Rotation.ToForwardVector());
					var p3 = Vector3.Transform(p2, Matrix.CreateRotationY(MathUtils.PI / 2));
					componentBody.IsSneaking = true;

					for (int a = -3; a < 3 + 1; a++)
					{
						var p = Terrain.ToCell(componentBody.Position - p2 * 2f + a * p3 + new Vector3(0f, 0.1f, 0f));
						int value2 = Utils.Terrain.GetCellContentsFast(p.X, p.Y - 1, p.Z);
						if (value2 == DirtBlock.Index || value2 == GrassBlock.Index || value2 == SoilBlock.Index)
						{
							if (Utils.Terrain.GetCellContentsFast(p.X, p.Y, p.Z) == 0)
								for (int i = 0; i < 6; i++)
								{
									int value23 = 0;
									int va1 = componentEngine2.GetSlotValue(i);
									switch (va1)
									{
										case 16557:
											value23 = 20 | FlowerBlock.SetIsSmall(0, true) << 14;
											break;
										case 173:
											value23 = 19 | TallGrassBlock.SetIsSmall(0, true) << 14;
											break;
										case 49325:
											value23 = 25 | FlowerBlock.SetIsSmall(0, true) << 14;
											break;
										case 32941:
											value23 = 24 | FlowerBlock.SetIsSmall(0, true) << 14;
											break;
										case 82093:
											value23 = 174 | RyeBlock.SetSize(RyeBlock.SetIsWild(0, false), 0) << 14;
											break;
										case 65709:
											value23 = 174 | RyeBlock.SetSize(RyeBlock.SetIsWild(0, false), 0) << 14;
											break;
										case 114861:
											value23 = 131 | BasePumpkinBlock.SetSize(BasePumpkinBlock.SetIsDead(0, false), 0) << 14;
											break;
										case 98477:
											value23 = 204 | CottonBlock.SetSize(CottonBlock.SetIsWild(0, false), 0) << 14;
											break;
									}
									if (value23 > 0 && componentEngine2.GetSlotCount(i) > 0 && Utils.Terrain.GetCellContentsFast(p.X, p.Y, p.Z) == 0)
									{
										componentEngine2.m_slots[i].Count -= 1;
										Utils.SubsystemTerrain.ChangeCell(p.X, p.Y, p.Z, value23);
										i += 6;
									}
								}
							if (use > 0 && Utils.Terrain.GetCellValueFast(p.X, p.Y - 1, p.Z) < 98472 && value2 == SoilBlock.Index)
							{
								Utils.SubsystemTerrain.ChangeCell(p.X, p.Y - 1, p.Z, 98472);
								use -= 1;
							}
							else
							//cellFace.X + i, cellFace.Y, cellFace.Z + j, 168 | SoilBlock.SetNitrogen(Terrain.ExtractData(cellValueFast), 3) << 14
							if (value2 != SoilBlock.Index)
								Utils.SubsystemTerrain.ChangeCell(p.X, p.Y - 1, p.Z, Terrain.ReplaceContents(value, 168));
						}
					}
				}
				if (componentEngine2.HeatLevel == 499f)
				{
					var p2 = Vector3.Normalize(componentBody.Rotation.ToForwardVector());
					var p3 = Vector3.Transform(p2, Matrix.CreateRotationY(MathUtils.PI / 2));
					componentBody.IsSneaking = true;
					for (int aaa = -3; aaa < 3 + 1; aaa++)
					{
						var p = Terrain.ToCell(componentBody.Position + p2 * 2f + aaa * p3 + new Vector3(0f, 0.1f, 0f));
						int val = Utils.Terrain.GetCellContentsFast(p.X, p.Y, p.Z);
						int value4 = Utils.Terrain.GetCellValueFast(p.X, p.Y, p.Z);
						bool flag = false;
						switch (val)
						{
							case RyeBlock.Index:
								if (RyeBlock.GetSize(value4) >= 6)
								{
									flag = true;
								}
								break;
							case PurpleFlowerBlock.Index:
								if (!FlowerBlock.GetIsSmall(value4))
								{
									flag = true;
								}
								break;
							case RedFlowerBlock.Index:
								if (!FlowerBlock.GetIsSmall(value4))
								{
									flag = true;
								}
								break;
							case WhiteFlowerBlock.Index:
								if (!FlowerBlock.GetIsSmall(value4))
								{
									flag = true;
								}
								break;
							case TallGrassBlock.Index:
								if (!TallGrassBlock.GetIsSmall(value4))
								{
									flag = true;
								}
								break;
							case PumpkinBlock.Index:
								if (BasePumpkinBlock.GetSize(value4) >= 4 || BasePumpkinBlock.GetIsDead(value4))
								{
									flag = true;
								}
								break;
							case CottonBlock.Index:
								if (CottonBlock.GetSize(value4) >= 0)
								{
									flag = true;
								}
								break;
							case RottenPumpkinBlock.Index:
								flag = true;
								break;
						}
						if (flag)
						{
							IInventory inventory = componentBody.Entity.FindComponent<ComponentEngineT>(true);
							var list = new List<BlockDropValue>(8);
							BlocksManager.Blocks[val].GetDropValues(Utils.SubsystemTerrain, value4, 0, 3, list, out bool s);
							for (int l = 0; l < list.Count; l++)
							{
								var blockDropValue = list[l];
								if (ComponentInventoryBase.AcquireItems(inventory, blockDropValue.Value, blockDropValue.Count) < blockDropValue.Count)
								{
									Utils.SubsystemTerrain.ChangeCell(p.X, p.Y, p.Z, 0);
								}
								else
								{
									l += list.Count;
								}
							}
						}
					}
				}
				return;
			}
			if (componentEngine != null)
			{
				componentBody.IsGravityEnabled = true;
				componentBody.IsGroundDragEnabled = false;
				Quaternion rotation = componentBody.Rotation;
				float num = MathUtils.Atan2(2f * rotation.Y * rotation.W - 2f * rotation.X * rotation.Z, 1f - 2f * rotation.Y * rotation.Y - 2f * rotation.Z * rotation.Z);
				if ((m_turnSpeed += 2.5f * Utils.SubsystemTime.GameTimeDelta * (TurnOrder - m_turnSpeed)) != 0 && componentEngine.HeatLevel > 0f)
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
			}
		}

		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			m_componentMount = Entity.FindComponent<ComponentMount>(true);
			m_componentBody = Entity.FindComponent<ComponentBody>(true);
			m_componentDamage = Entity.FindComponent<ComponentDamage>(true);
			componentEngine = Entity.FindComponent<ComponentEngineA>();
			componentEngine2 = Entity.FindComponent<ComponentEngineT>();
			componentEngine3 = Entity.FindComponent<ComponentEngineT2>();
			componentEngine4 = Entity.FindComponent<ComponentEngineT3>();
			componentEngine5 = Entity.FindComponent<ComponentEngineT4>();
			m_componentBody.CollidedWithBody += CollidedWithBody;
			//string value = valuesDictionary.GetValue<string>("ModelName");
			if (componentEngine4 != null)
				SetModel(ContentManager.Get<Model>("Models/Tank"));
		}

		public Model m_model;
		public ModelBone m_bodyBone,
						 m_headBone;
		public Matrix?[] m_boneTransforms;
		public Matrix m_headT = ContentManager.Get<Model>("Models/Tank").FindBone("Head").Transform;

		public Matrix? GetBoneTransform(int boneIndex)
		{
			return m_boneTransforms[boneIndex];
		}

		public void SetBoneTransform(int boneIndex, Matrix? transformation)
		{
			m_boneTransforms[boneIndex] = transformation;
		}

		public void SetModel(Model model)
		{
			m_model = model;
			if (m_model != null)
			{
				m_boneTransforms = new Matrix?[m_model.Bones.Count];
				m_bodyBone = m_model.FindBone("Body");
				m_boneTransforms[0] = m_bodyBone.Transform;
				m_headBone = m_model.FindBone("Head");
				m_boneTransforms[1] = m_headBone.Transform;
				//m_headT = ContentManager.Get<Model>("Models/Tank").FindBone("Head").Transform;
				//m_boneTransforms[0] = m_bodyBone;
			}
			else
			{
				m_bodyBone = null;
				m_headBone = null;
				m_boneTransforms = null;
			}
		}

		public void CollidedWithBody(ComponentBody body)
		{
			Vector2 v = m_componentBody.Velocity.XZ - body.Velocity.XZ;
			float amount = v.LengthSquared() * .3f;
			if (amount < .02f || m_componentBody.Velocity.XZ.LengthSquared() < 1f) return;
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
		private static readonly Vector3 center = new Vector3(0.5f, 0, 0.5f);

		private static readonly Quaternion[] directions =
		{
			Quaternion.CreateFromAxisAngle(Vector3.UnitY, 0),
			Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathUtils.PI * 0.5f),
			Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathUtils.PI),
			Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathUtils.PI * 1.5f)
		};

		private static readonly Quaternion upwardDirection = Quaternion.CreateFromAxisAngle(new Vector3(1, 0, 0), MathUtils.PI * 0.25f);
		private static readonly Quaternion downwardDirection = Quaternion.CreateFromAxisAngle(new Vector3(1, 0, 0), MathUtils.PI * -0.25f);

		private static readonly Vector3[] forwardVectors =
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
		private ComponentMount m_componentMount;
		public ComponentEngine ComponentEngine;
		public ComponentEngineE ComponentEngineE;
		private int m_forwardDirection;
		private Quaternion rotation;
		private Vector3 forwardVector;

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
			if ((ComponentEngine = Entity.FindComponent<ComponentEngine>()) != null || (ComponentEngineE = Entity.FindComponent<ComponentEngineE>()) != null)
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
			if (MathUtils.Abs(body.Density - 9.76f) <= float.Epsilon)
				return;
			Vector2 v = m_componentBody.Velocity.XZ - body.Velocity.XZ;
			float amount = v.LengthSquared() * .3f;
			if (amount < .02f || m_componentBody.Velocity.XZ.LengthSquared() == 0f) return;
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
			//if (ComponentEngineE != null)
			//	ComponentEngineE.Coordinates = new Point3((int)m_componentBody.Position.X, (int)m_componentBody.Position.Y, (int)m_componentBody.Position.Z);
			if (m_componentMount.Rider != null)
			{
				var player = m_componentMount.Rider.Entity.FindComponent<ComponentPlayer>(true);
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
			ComponentTrain t = this;
			int level = 0;
			for (; t.ParentBody != null; level++) t = t.ParentBody;
			if (level > 0)
			{
				var body = t.m_componentBody;
				var pos = body.Position;
				var r = body.Rotation;
				//if (ParentBody!=null)
				//m_componentBody.Rotation = Quaternion.Slerp(m_componentBody.Rotation, rotation, 0.715f);
				var Com = body.Entity.FindComponent<ComponentEngine>();
				var Com2 = body.Entity.FindComponent<ComponentEngineE>();
				float HLD = 0f;
				if (Com != null)
					HLD = Com.HeatLevel >= 100f ? 100f : 1f;
				if (Com2 != null)
				{
					var result = Utils.SubsystemTerrain.Raycast(m_componentBody.Position, m_componentBody.Position + new Vector3(0, 3f, 0), false, true, null) ?? default;
					if ((ElementBlock.Block.GetDevice(result.CellFace.X, result.CellFace.Y, result.CellFace.Z, result.Value) is TElectricWire em ) )
					{
						HLD = Com2.Charged ? 200f : 1f;
					}
					//HLD = Com2.Charged ? 200f : 1f;
				}
					//HLD = Com2.Charged ? 200f : 1f;
				if (m_componentBody.StandingOnValue.HasValue && HLD >= 100f && body.m_velocity.Length() > 0f) //body.Velocity.LengthSquared() > 10f &&
																											  //Utils.SubsystemTime.QueueGameTimeDelayedExecution(Utils.SubsystemTime.GameTime + 0.23 * level, delegate
				{
					var result = Utils.SubsystemTerrain.Raycast(m_componentBody.Position, m_componentBody.Position + new Vector3(0, -3f, 0), false, true, null);
					//result.Value.CellFace.Point
					if (result.HasValue && Terrain.ExtractContents(result.Value.Value) == RailBlock.Index && (dt *= SimulateRail(RailBlock.GetRailType(Terrain.ExtractData(result.Value.Value)))) > 0f)
					{
						//if (rotation.ToForwardVector().Y != 0 || ParentBody.m_componentBody.Position.Y != m_componentBody.Position.Y)
						//{
						//	m_componentBody.m_velocity += dt2 * rotation.ToForwardVector();
						//	m_componentBody.Rotation = Quaternion.Slerp(m_componentBody.Rotation, rotation, 0.15f);
						//	return;
						//}
						float ABS = 6f;
						if (HLD>=200f)
						ABS = 30f; 
						//	if (rotation.ToForwardVector().Y != 0f) { ABS = 3f; };
						m_componentBody.m_velocity = ABS * rotation.ToForwardVector();
						//m_componentBody.m_velocity.Z = ABS * rotation.ToForwardVector().Z;
						// m_componentBody.m_velocity.Y += rotation.ToForwardVector().Y * dt2/2;
						if (ParentBody != null && MathUtils.Abs(Vector3.Distance(ParentBody.m_componentBody.Position, m_componentBody.Position)) > 2f)
						{
							m_componentBody.m_velocity = ABS * rotation.ToForwardVector() * 1.4f;
							//m_componentBody.m_velocity.Z = ABS * rotation.ToForwardVector().Z * 1.4f;
						}
						//m_componentBody.m_velocity = ParentBody.m_componentBody.m_velocity.Length() * rotation.ToForwardVector() * 1.2f;
						if (ParentBody != null && MathUtils.Abs(Vector3.Distance(ParentBody.m_componentBody.Position, m_componentBody.Position)) < 1.2f)
						{
							m_componentBody.m_velocity = ABS * rotation.ToForwardVector() * 0.3f;
							//m_componentBody.m_velocity.Z = ABS * rotation.ToForwardVector().Z * 0.6f;
						}
						if (ParentBody != null && MathUtils.Abs(Vector3.Distance(ParentBody.m_componentBody.Position, m_componentBody.Position)) < 0.8f)
						{
							m_componentBody.m_velocity = ABS * rotation.ToForwardVector() * 0f;
							//m_componentBody.m_velocity.Z = ABS * rotation.ToForwardVector().Z * 0.6f;
						}
						m_componentBody.Rotation = Quaternion.Slerp(m_componentBody.Rotation, rotation, 0.715f);

						//m_componentBody.m_velocity = ParentBody.m_componentBody.m_velocity.Length() * rotation.ToForwardVector() * 0.8f;
					}
				}
				else if (Com != null && HLD < 100f && m_componentBody.m_velocity.Length() - 0.06f > 0f && m_componentBody.StandingOnValue.HasValue)
				{
					var result = Utils.SubsystemTerrain.Raycast(m_componentBody.Position, m_componentBody.Position + new Vector3(0, -3f, 0), false, true, null) ?? default;
					SimulateRail(RailBlock.GetRailType(Terrain.ExtractData(result.Value)));
					m_componentBody.m_velocity = (m_componentBody.m_velocity.Length() - 0.06f) * rotation.ToForwardVector();
				}
				if (!m_componentBody.StandingOnValue.HasValue)
				{
					//rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, 0);
					var result = Utils.SubsystemTerrain.Raycast(m_componentBody.Position, m_componentBody.Position + new Vector3(0, -3f, 0), false, true, null) ?? default;
					SimulateRail(RailBlock.GetRailType(Terrain.ExtractData(result.Value)));
					m_componentBody.Rotation = rotation;
					m_componentBody.m_velocity -= new Vector3(0f, 40f, 0f) * dt;
					//m_componentBody.m_velocity -= new Vector3(0f,0.1f, 0f);
				}
				//m_componentBody.Rotation = Quaternion.Slerp(m_componentBody.Rotation, rotation, 0.715f);
				m_componentBody.Rotation = Quaternion.Slerp(m_componentBody.Rotation, rotation, 0.715f);
				m_outOfMountTime = MathUtils.Abs(Vector3.DistanceSquared(ParentBody.m_componentBody.Position, m_componentBody.Position)) > 24f
					? m_outOfMountTime + dt
					: 0f;
				ComponentDamage ComponentDamage = ParentBody.Entity.FindComponent<ComponentDamage>();
				if (m_outOfMountTime > 12f || (componentDamage != null && componentDamage.Hitpoints <= .05f) || ComponentDamage != null && ComponentDamage.Hitpoints <= .05f)
					ParentBody = null;
				return;
			}
			if (!m_componentBody.StandingOnValue.HasValue)
			{
				//rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, 0);
				var result = Utils.SubsystemTerrain.Raycast(m_componentBody.Position, m_componentBody.Position + new Vector3(0, -3f, 0), false, true, null) ?? default;
				SimulateRail(RailBlock.GetRailType(Terrain.ExtractData(result.Value)));
				m_componentBody.Rotation = rotation;
				m_componentBody.m_velocity -= new Vector3(0f, 40f, 0f) * dt;
				//m_componentBody.m_velocity -= new Vector3(0f, 0.1f, 0f);
			}
			var Com1 = m_componentBody.Entity.FindComponent<ComponentEngine>();
			var Com21 = m_componentBody.Entity.FindComponent<ComponentEngineE>();
			float HLD1 = 0f;
			if (Com1 != null)
				HLD1 = Com1.HeatLevel >= 100f ? 100f : 1f;
			if (Com21 != null)
			{
				var result = Utils.SubsystemTerrain.Raycast(m_componentBody.Position, m_componentBody.Position + new Vector3(0, 3f, 0), false, true, null) ?? default;
				if (ElementBlock.Block.GetDevice(result.CellFace.X, result.CellFace.Y, result.CellFace.Z, result.Value) is TElectricWire em )
				{
					HLD1 = Com21.Charged ? 200f : 1f;
				}
				//HLD1 = Com21.Charged ? 200f : 1f;
			}
				
			if (HLD1 >= 100f && m_componentBody.StandingOnValue.HasValue) //
			{
				//time = 0;
				float ABS = 6f;
				if (HLD1 >= 200f)
					ABS = 30f;
				// if (rotation.ToForwardVector().Y != 0f) { ABS = 3f; };
				var result = Utils.SubsystemTerrain.Raycast(m_componentBody.Position, m_componentBody.Position + new Vector3(0, -3f, 0), false, true, null) ?? default;
				if (Terrain.ExtractContents(result.Value) == RailBlock.Index && (dt *= SimulateRail(RailBlock.GetRailType(Terrain.ExtractData(result.Value)))) > 0f)
					m_componentBody.m_velocity = ABS * rotation.ToForwardVector();
			}
			else if (HLD1 < 100f && m_componentBody.m_velocity.Length() - 0.06f > 0f && m_componentBody.StandingOnValue.HasValue)
			{
				//float yyy = m_componentBody.m_velocity.Y;
				//time++;
				var result = Utils.SubsystemTerrain.Raycast(m_componentBody.Position, m_componentBody.Position + new Vector3(0, -3f, 0), false, true, null) ?? default;
				SimulateRail(RailBlock.GetRailType(Terrain.ExtractData(result.Value)));
				m_componentBody.m_velocity = (m_componentBody.m_velocity.Length() - 0.06f) * rotation.ToForwardVector();
				//m_componentBody.m_velocity.Y = yyy;
			}
			m_componentBody.Rotation = Quaternion.Slerp(m_componentBody.Rotation, rotation, 0.715f);
		}

		//public int time=0;
		private float SimulateRail(int railType)
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

		private bool Turn(int turnType)
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

		private static float GetOffsetOnDirection(Vector3 vec, int direction)
		{
			float offset = (direction & 1) == 0 ? vec.Z - MathUtils.Floor(vec.Z) : vec.X - MathUtils.Floor(vec.X);
			return (direction & 2) == 0 ? 1 - offset : offset;
		}

		public static ComponentTrain GetRailEntity(Component c) => c.Entity.FindComponent<ComponentTrain>();
	}
}