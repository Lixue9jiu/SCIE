using Engine;
using TemplatesDatabase;
using GameEntitySystem;
using System;
namespace Game
{
	class SubsystemCreatureS : SubsystemCreatureSpawn
	{
		public override void Load(ValuesDictionary valuesDictionary)
		{
			m_subsystemGameInfo = base.Project.FindSubsystem<SubsystemGameInfo>(throwOnError: true);
			m_subsystemSpawn = base.Project.FindSubsystem<SubsystemSpawn>(throwOnError: true);
			m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(throwOnError: true);
			m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(throwOnError: true);
			m_subsystemSky = base.Project.FindSubsystem<SubsystemSky>(throwOnError: true);
			m_subsystemBodies = base.Project.FindSubsystem<SubsystemBodies>(throwOnError: true);
			m_subsystemViews = base.Project.FindSubsystem<SubsystemViews>(throwOnError: true);
			InitializeCreatureTypes();
			m_subsystemSpawn.SpawningChunk += delegate (SpawnChunk chunk)
			{
				m_spawnChunks.Add(chunk);
				if (!chunk.IsSpawned)
				{
					m_newSpawnChunks.Add(chunk);
				}
			};
		}
		private new void InitializeCreatureTypes()
		{
			m_creatureTypes.Add(new CreatureType("T-100", SpawnLocationType.Surface, randomSpawn: false, constantSpawn: false)
			{
				SpawnSuitabilityFunction = delegate (CreatureType creatureType, Point3 point)
				{
					float num = m_subsystemTerrain.TerrainContentsGenerator.CalculateOceanShoreDistance(point.X, point.Z);
					int temperature = m_subsystemTerrain.Terrain.GetTemperature(point.X, point.Z);
					int humidity = m_subsystemTerrain.Terrain.GetHumidity(point.X, point.Z);
					int num2 = Terrain.ExtractContents(m_subsystemTerrain.Terrain.GetCellValueFast(point.X, point.Y - 1, point.Z));
					return (num > 40f && Utils.SubsystemGameInfo.WorldSettings.AreSupernaturalCreaturesEnabled && (num2 == 8 || num2 == 2 || num2 == 3 || num2 == 7 || num2 == 12) && Utils.SubsystemTimeOfDay.Day > 60) ? 0.15f : 0f;
				},
				SpawnFunction = ((CreatureType creatureType, Point3 point) => SpawnCreatures(creatureType, "T-100", point, 1).Count)
			});
			base.InitializeCreatureTypes();

		}


	}

	public class ComponentChaseBehavior2 : ComponentChaseBehavior, IUpdateable
	{

		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			m_subsystemGameInfo = base.Project.FindSubsystem<SubsystemGameInfo>(throwOnError: true);
			m_subsystemPlayers = base.Project.FindSubsystem<SubsystemPlayers>(throwOnError: true);
			m_subsystemSky = base.Project.FindSubsystem<SubsystemSky>(throwOnError: true);
			m_subsystemBodies = base.Project.FindSubsystem<SubsystemBodies>(throwOnError: true);
			m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(throwOnError: true);
			m_subsystemNoise = base.Project.FindSubsystem<SubsystemNoise>(throwOnError: true);
			m_componentCreature = base.Entity.FindComponent<ComponentCreature>(throwOnError: true);
			m_componentPathfinding = base.Entity.FindComponent<ComponentPathfinding>(throwOnError: true);
			m_componentMiner = base.Entity.FindComponent<ComponentMiner>(throwOnError: true);
			m_componentFeedBehavior = base.Entity.FindComponent<ComponentRandomFeedBehavior>();
			m_componentCreatureModel = base.Entity.FindComponent<ComponentCreatureModel>(throwOnError: true);
			m_dayChaseRange = valuesDictionary.GetValue<float>("DayChaseRange");
			m_nightChaseRange = valuesDictionary.GetValue<float>("NightChaseRange");
			m_dayChaseTime = valuesDictionary.GetValue<float>("DayChaseTime");
			m_nightChaseTime = valuesDictionary.GetValue<float>("NightChaseTime");
			m_autoChaseMask = valuesDictionary.GetValue<CreatureCategory>("AutoChaseMask");
			m_chaseNonPlayerProbability = valuesDictionary.GetValue<float>("ChaseNonPlayerProbability");
			m_chaseWhenAttackedProbability = valuesDictionary.GetValue<float>("ChaseWhenAttackedProbability");
			m_chaseOnTouchProbability = valuesDictionary.GetValue<float>("ChaseOnTouchProbability");
			m_componentCreature.ComponentHealth.Attacked += Load_b__40_0;
			m_componentCreature.ComponentBody.CollidedWithBody += Load_b__40_1;
			m_stateMachine.AddState("LookingForTarget", Load_b__40_2, Load_b__40_3, null);
			if (m_componentCreature.DisplayName != "T-100")
			m_stateMachine.AddState("RandomMoving", Load_b__40_4, Load_b__40_5, Load_b__40_6);
		//	if (m_componentCreature.DisplayName != "T-100")
	//		{
				m_stateMachine.AddState("Chasing", Load_b__40_7, Load_b__40_8, null);
	//		}else
	//		{
	//			m_stateMachine.AddState("Chasing", Load_b__40_7, Load_b__40_81, null);
	//		}
			m_stateMachine.TransitionTo("LookingForTarget");
		}

		public new void Update(float dt)
		{
			if (m_componentCreature.DisplayName == "T-100")
			{
				m_autoChaseSuppressionTime -= dt;
				if (IsActive && m_target != null)
				{
					m_chaseTime -= dt;
					//m_componentCreature.ComponentCreatureModel.LookAtOrder = m_target.ComponentCreatureModel.EyePosition;
					if (IsTargetInAttackRange2(m_target.ComponentBody) && Utils.SubsystemTime.PeriodicGameTimeEvent(0.20, 0))
					{
						m_componentCreatureModel.AttackOrder = false;
						
							BoundingBox boundingBox = m_componentCreature.ComponentBody.BoundingBox;
							BoundingBox boundingBox2 = m_target.ComponentBody.BoundingBox;
							Vector3 v = 0.5f * (boundingBox.Min + boundingBox.Max);
							Vector3 vector = 0.5f * (boundingBox2.Min + boundingBox2.Max) - v;
							Utils.SubsystemProjectiles.FireProjectile(Terrain.MakeBlockValue(214, 0, BulletBlock.SetBulletType(0, BulletBlock.BulletType.MiniBullet)), v + Vector3.Normalize(vector) * 1.5f, Vector3.Normalize(vector) * 200f, Vector3.Zero, m_componentCreature);
							Utils.SubsystemAudio.PlaySound("Audio/MusketFire", 1f, 0f, v, 20f, true);
							Utils.SubsystemParticles.AddParticleSystem(new GunSmokeParticleSystem2(Utils.SubsystemTerrain, v + Vector3.Normalize(vector) * 1.0f, Vector3.Normalize(vector)));
					
						
					}

				}
				if (m_subsystemTime.GameTime >= m_nextUpdateTime)
				{
					m_dt = m_random.UniformFloat(0.25f, 0.35f) + MathUtils.Min((float)(m_subsystemTime.GameTime - m_nextUpdateTime), 0.1f);
					m_nextUpdateTime = m_subsystemTime.GameTime + (double)m_dt;
					m_stateMachine.Update();
				}
			}
			else
			{
				base.Update(dt);
			}
			
		}

		public new ComponentCreature FindTarget()
		{
			Vector3 position = m_componentCreature.ComponentBody.Position;
			ComponentCreature result = null;
			float num = 0f;
			m_componentBodies.Clear();
			m_subsystemBodies.FindBodiesAroundPoint(new Vector2(position.X, position.Z), m_range, m_componentBodies);
			for (int i = 0; i < m_componentBodies.Count; i++)
			{
				ComponentCreature componentCreature = m_componentBodies.Array[i].Entity.FindComponent<ComponentCreature>();
				if (componentCreature != null)
				{
					float num2 = ScoreTarget(componentCreature);
					if (num2 > num)
					{
						num = num2;
						result = componentCreature;
					}
				}
			}
			return result;
		}

		public new float ScoreTarget(ComponentCreature componentCreature)
		{
			
			bool flag = componentCreature.Entity.FindComponent<ComponentPlayer>() != null;
			bool flag2 = m_componentCreature.Category != CreatureCategory.WaterPredator && m_componentCreature.Category != CreatureCategory.WaterOther;
			bool flag3 = componentCreature == Target || m_subsystemGameInfo.WorldSettings.GameMode > GameMode.Harmless;
			bool flag4 = (componentCreature.Category & m_autoChaseMask) != 0;
			if (m_componentCreature.DisplayName == "T-100" && componentCreature.DisplayName != "T-100")
			{
				flag4 = true;
			}
			bool flag5 = componentCreature == Target || (flag4 && MathUtils.Remainder(0.004999999888241291 * m_subsystemTime.GameTime + (double)((float)(GetHashCode() % 1000) / 1000f) + (double)((float)(componentCreature.GetHashCode() % 1000) / 1000f), 1.0) < (double)m_chaseNonPlayerProbability);
			if (componentCreature != m_componentCreature && ((!flag && flag5) || (flag && flag3)) && componentCreature.Entity.IsAddedToProject && componentCreature.ComponentHealth.Health > 0f && (flag2 || IsTargetInWater(componentCreature.ComponentBody)))
			{
				float num = Vector3.Distance(m_componentCreature.ComponentBody.Position, componentCreature.ComponentBody.Position);
				if (num < m_range)
				{
					return m_range - num;
				}
			}
			return 0f;
		}


		public bool IsTargetInAttackRange2(ComponentBody target)
		{
			if (IsBodyInAttackRange(target))
			{
				return true;
			}
			BoundingBox boundingBox = m_componentCreature.ComponentBody.BoundingBox;
			BoundingBox boundingBox2 = target.BoundingBox;
			Vector3 v = 0.5f * (boundingBox.Min + boundingBox.Max);
			Vector3 vector = 0.5f * (boundingBox2.Min + boundingBox2.Max) - v;
			float num = vector.Length();
			if (num < 30f)
			{

				TerrainRaycastResult? terrainRaycastResult = Utils.SubsystemTerrain.Raycast(v, 0.5f * (boundingBox2.Min + boundingBox2.Max), false, true, (value, distance) => !(Terrain.ExtractContents(value) == 0 || Terrain.ExtractContents(value) == WaterBlock.Index || Terrain.ExtractContents(value) == TallGrassBlock.Index || Terrain.ExtractContents(value) == GlassBlock.Index || Terrain.ExtractContents(value) == WoodenDoorBlock.Index));
				if (terrainRaycastResult == null )
				{
					return true;
				}

				
			}
			return false;
		}

	}
}
