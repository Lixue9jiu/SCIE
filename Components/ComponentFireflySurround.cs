using Engine;
using Engine.Graphics;
using GameEntitySystem;
using System;
using TemplatesDatabase;

namespace Game
{
	public class ComponentFireflySurround : Component, IDrawable
	{
		public class Firefly
		{
			public Firefly(Vector3 vector3, double time, float hue0, float saturation0)
			{
				position = vector3;
				nextPosition = vector3;
				timeToStopMoving = 0;
				spawnTime = time;
				hue = hue0;
				saturation = saturation0;
			}

			public Vector3 position,
							nextPosition;

			public double timeToStopMoving,
						  spawnTime;

			public float hue,
						saturation;
		}

		protected ComponentCreature m_componentCreature;
		public static SubsystemTime m_subsystemTime;
		public static SubsystemSky m_subsystemSky;
		public static SubsystemWeather m_subsystemWeather;
		public static SubsystemTerrain m_subsystemTerrain;
		public Random m_random = new Random();
		public static PrimitivesRenderer3D m_primitivesRenderer;
		public static Texture2D m_texture;
		public DynamicArray<Firefly> m_fireFlies = new DynamicArray<Firefly>();
		public double m_lastSpawnTime;
		public bool isLivingMode;

		public int[] DrawOrders
		{
			get { return new[] { 10 }; }
		}

		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			m_componentCreature = Entity.FindComponent<ComponentCreature>(true);
			m_subsystemTime = Project.FindSubsystem<SubsystemTime>(true);
			m_subsystemSky = Project.FindSubsystem<SubsystemSky>(true);
			m_subsystemWeather = Project.FindSubsystem<SubsystemWeather>(true);
			m_subsystemTerrain = Project.FindSubsystem<SubsystemTerrain>(true);
			m_texture = ContentManager.Get<Texture2D>("Textures/Sun");
			m_primitivesRenderer = Project.FindSubsystem<SubsystemModelsRenderer>(true).PrimitivesRenderer;
			isLivingMode = Project.FindSubsystem<SubsystemGameInfo>(true).WorldSettings.EnvironmentBehaviorMode == EnvironmentBehaviorMode.Living;
		}

		public void Draw(Camera camera, int drawOrder)
		{
			if (!isLivingMode || drawOrder != 10)
				return;
			double nowTime = m_subsystemTime.GameTime;
			float dt = m_subsystemTime.GameTimeDelta;
			var playerPosition = m_componentCreature.ComponentBody.Position;
			var playerCell = new Point2((int)playerPosition.X, (int)playerPosition.Z);
			//环境亮度<0.4，不下雨，玩家所在位置温度不至于结冰
			if (m_subsystemSky.SkyLightIntensity < 0.4 && m_subsystemWeather.m_globalPrecipitationIntensity == 0 && !SubsystemWeather.IsPlaceFrozen(m_subsystemTerrain.Terrain.GetTemperature(playerCell.X, playerCell.Y), (int)playerPosition.Y))
			{
				if (nowTime > m_lastSpawnTime + 1)
				{
					m_lastSpawnTime = nowTime;
					var randomPosition = new Vector3(playerPosition.X + m_random.UniformFloat(-20f, 20f), 0, playerPosition.Z + m_random.UniformFloat(-20f, 20f));
					var cell = new Point2((int)randomPosition.X, (int)randomPosition.Z);
					int humidity = m_subsystemTerrain.Terrain.GetHumidity(cell.X, cell.Y);
					randomPosition.Y = m_subsystemTerrain.Terrain.GetTopHeight(cell.X, cell.Y) + m_random.UniformFloat(0.4f, 4f);
					float randomFloat = m_random.UniformFloat(0f, 1f);
					//生成速率和限制由湿度决定，亮度大于8处不生成
					if (m_subsystemTerrain.Terrain.GetCellLight(cell.X, (int)randomPosition.Y, cell.Y) < 9 && ((humidity > 12 && randomFloat < 0.8f) || (humidity > 10 && randomFloat < 0.6f) || (humidity > 8 && randomFloat < 0.45f) || (humidity > 6 && randomFloat < 0.3f)))
					{
						m_fireFlies.Add(new Firefly(randomPosition, nowTime, m_random.UniformFloat(60f, 150f), m_random.UniformFloat(0.5f, 1f)));
					}
					humidity = m_subsystemTerrain.Terrain.GetHumidity(playerCell.X, playerCell.Y);
					if (m_fireFlies.Count > 120 || (humidity < 6 && m_fireFlies.Count > 40))
					{
						m_fireFlies.RemoveAt(0);
					}
				}
			}
			else if (nowTime > m_lastSpawnTime + 0.1 && m_fireFlies.Count > 0)
			{
				m_fireFlies.RemoveAt(0);
			}
			foreach (Firefly firefly in m_fireFlies)
			{
				float distance = Vector3.Distance(firefly.position, camera.ViewPosition);
				if (distance < 32)
				{
					if (nowTime >= firefly.timeToStopMoving)
					{
						firefly.nextPosition += m_random.Vector3(0.3f);
						firefly.timeToStopMoving += Vector3.Distance(firefly.position, firefly.nextPosition) * m_random.UniformFloat(2f, 10f);
					}
					else
					{
						Vector3 speed = (firefly.nextPosition - firefly.position) / (float)(firefly.timeToStopMoving - nowTime) * dt;
						firefly.position += speed;
					}
					double timePassed = (nowTime - firefly.spawnTime) * 0.5;
					float size = (float)(Math.Sin(7 * timePassed) / (7 * Math.Sin(timePassed))) * 0.01f + 0.012f;
					var v1 = Vector3.Normalize(Vector3.Cross(camera.ViewDirection, Vector3.UnitY));
					Vector3 v2 = -Vector3.Normalize(Vector3.Cross(camera.ViewDirection, v1));
					var p1 = Vector3.Transform(firefly.position + size * (-v1 - v2), camera.ViewMatrix);
					var p2 = Vector3.Transform(firefly.position + size * (v1 - v2), camera.ViewMatrix);
					var p3 = Vector3.Transform(firefly.position + size * (-v1 + v2), camera.ViewMatrix);
					var p4 = Vector3.Transform(firefly.position + size * (v1 + v2), camera.ViewMatrix);
					TexturedBatch3D texturedBatch3D = m_primitivesRenderer.TexturedBatch(m_texture, true, 0, null, RasterizerState.CullCounterClockwiseScissor, null, SamplerState.AnisotropicWrap);
					var hsv = new Vector3(firefly.hue, firefly.saturation, MathUtils.Clamp(0.85f + size * 10 - distance * 0.01f, 0f, 1f));
					texturedBatch3D.QueueQuad(p1, p3, p4, p2, new Vector2(0, 1), new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Color(Color.HsvToRgb(hsv)));
				}
			}
		}
	}
}