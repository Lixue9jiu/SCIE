using Engine;
using Engine.Graphics;
using System.Collections.Generic;
namespace Game
{
	public class GunSmokeParticleSystem2 : ParticleSystem<GunSmokeParticleSystem.Particle>
	{
		public Random m_random = new Random();

		public float m_time;

		public float m_toGenerate;

		public Vector3 m_position;

		public Vector3 m_direction;

		public Color m_color;

		public GunSmokeParticleSystem2(SubsystemTerrain terrain, Vector3 position, Vector3 direction)
			: base(50)
		{
			Texture = ContentManager.Get<Texture2D>("Textures/FireParticle");
			TextureSlotsCount = 3;
			m_position = position;
			m_direction = Vector3.Normalize(direction);
			int num = Terrain.ToCell(position.X),
				num2 = Terrain.ToCell(position.Y),
				num3 = Terrain.ToCell(position.Z),
				x = 0;
			x = MathUtils.Max(x, terrain.Terrain.GetCellLight(num + 1, num2, num3));
			x = MathUtils.Max(x, terrain.Terrain.GetCellLight(num - 1, num2, num3));
			x = MathUtils.Max(x, terrain.Terrain.GetCellLight(num, num2 + 1, num3));
			x = MathUtils.Max(x, terrain.Terrain.GetCellLight(num, num2 - 1, num3));
			x = MathUtils.Max(x, terrain.Terrain.GetCellLight(num, num2, num3 + 1));
			x = MathUtils.Max(x, terrain.Terrain.GetCellLight(num, num2, num3 - 1));
			float num4 = LightingManager.LightIntensityByLightValue[x];
			m_color = new Color(num4, num4, num4);
		}

		public override bool Simulate(float dt)
		{
			m_time += dt;
			float num = MathUtils.Lerp(150f, 20f, MathUtils.Saturate(2f * m_time / 0.5f));
			float num2 = MathUtils.Pow(0.01f, dt);
			float s = MathUtils.Lerp(20f, 0f, MathUtils.Saturate(2f * m_time / 0.5f));
			var v = new Vector3(2f, 2f, 1f);
			if (m_time < 0.1f)
			{
				m_toGenerate += num * dt;
			}
			else
			{
				m_toGenerate = 0f;
			}
			bool flag = false;
			for (int i = 0; i < Particles.Length; i++)
			{
				GunSmokeParticleSystem.Particle particle = Particles[i];
				if (particle.IsActive)
				{
					flag = true;
					particle.Time += dt;
					if (particle.Time <= particle.Duration)
					{
						particle.Position += particle.Velocity * dt;
						particle.Velocity *= num2;
						particle.Velocity += v * dt;
						particle.TextureSlot = (int)MathUtils.Min(30f * particle.Time / particle.Duration, 8f);
						particle.Size = new Vector2(0.2f);
					}
					else
					{
						particle.IsActive = false;
					}
				}
				else if (m_toGenerate >= 1f)
				{
					particle.IsActive = true;
					Vector3 v2 = m_random.UniformVector3(0f, 1f);
					particle.Position = m_position + 0.3f * v2;
					particle.Color = m_color;
					particle.Velocity = s * (m_direction + m_random.UniformVector3(0f, 0.1f)) + 2.5f * v2;
					particle.Size = Vector2.Zero;
					particle.Time = 0f;
					particle.Duration = m_random.UniformFloat(0.1f, 1f);
					particle.FlipX = m_random.Bool();
					particle.FlipY = m_random.Bool();
					m_toGenerate -= 1f;
				}
			}
			m_toGenerate = MathUtils.Remainder(m_toGenerate, 1f);
			return !flag && m_time >= 0.5f;
		}
	}

	public class RParticleSystem : ParticleSystem<FireParticleSystem.Particle>
	{
		public class Particle : Game.Particle
		{
			public float Time;

			public float TimeToLive;

			public float Speed;
		}

		public Random m_random = new Random();

		public Vector3 m_position;

		public float m_size;

		public float m_toGenerate;

		public bool m_visible;

		public float m_maxVisibilityDistance;

		public float m_age;

		public bool IsStopped_;

		public bool IsStopped
		{
			get
			{
				return IsStopped_;
			}
			set
			{
				IsStopped_ = value;
			}
		}

		public RParticleSystem(Vector3 position, float size, float maxVisibilityDistance)
			: base(10)
		{
			m_position = position;
			m_size = size;
			m_maxVisibilityDistance = maxVisibilityDistance;
			Texture = ContentManager.Get<Texture2D>("Textures/FireParticle2");
			TextureSlotsCount = 3;
			//m_color = new Color(num4, num4, num4);
		}

		public override bool Simulate(float dt)
		{
			m_age += dt;
			bool flag = false;
			if (m_visible || true)
			{
				m_toGenerate += (IsStopped ? 0f : (5f * dt));
				for (int i = 0; i < Particles.Length; i++)
				{
					FireParticleSystem.Particle particle = Particles[i];
					if (particle.IsActive)
					{
						flag = true;
						particle.Time += dt;
						particle.TimeToLive -= dt;
						if (particle.TimeToLive > 0f)
						{
							particle.Position.Y += particle.Speed * dt;
							particle.TextureSlot = (int)MathUtils.Min(9f * particle.Time / 1.25f, 8f);
						}
						else
						{
							particle.IsActive = false;
						}
					}
					else if (m_toGenerate >= 1f)
					{
						particle.IsActive = true;
						particle.Position = m_position + 0.25f * m_size * new Vector3(m_random.UniformFloat(-1f, 1f), 0f, m_random.UniformFloat(-1f, 1f));
						particle.Color = Color.White;
						particle.Size = new Vector2(m_size);
						particle.Speed = m_random.UniformFloat(0.45f, 0.55f) * m_size / 0.15f;
						particle.Time = 0f;
						particle.TimeToLive = m_random.UniformFloat(0.5f, 2f);
						particle.FlipX = (m_random.UniformInt(0, 1) == 0);
						particle.FlipY = (m_random.UniformInt(0, 1) == 0);
						m_toGenerate -= 1f;
					}
				}
				m_toGenerate = MathUtils.Remainder(m_toGenerate, 1f);
			}
			m_visible = false;
			if (IsStopped)
			{
				return !flag;
			}
			return false;
		}

	}
	public class NParticleSystem : ParticleSystem<FireParticleSystem.Particle>
	{
		public class Particle : Game.Particle
		{
			public float Time;

			public float TimeToLive;

			public float Speed;
		}

		public Random m_random = new Random();

		public Vector3 m_position;

		public float m_size;

		public float m_toGenerate;

		public bool m_visible;

		public float m_maxVisibilityDistance;

		public float m_age;

		public bool IsStopped_;

		public bool IsStopped
		{
			get
			{
				return IsStopped_;
			}
			set
			{
				IsStopped_ = value;
			}
		}

		public NParticleSystem(Vector3 position, float size, float maxVisibilityDistance)
			: base(20)
		{
			m_position = position;
			m_size = size;
			m_age = 0f;
			m_visible = true;
			m_maxVisibilityDistance = maxVisibilityDistance;
			Texture = ContentManager.Get<Texture2D>("Textures/FireParticle");
			TextureSlotsCount = 3;
			//m_color = new Color(num4, num4, num4);
		}

		public override bool Simulate(float dt)
		{
			m_age += dt;
			bool flag = false;
			if (m_visible)
			{
				m_toGenerate += (IsStopped ? 0f : (5f * dt));
				for (int i = 0; i < Particles.Length; i++)
				{
					FireParticleSystem.Particle particle = Particles[i];
					if (particle.IsActive)
					{
						flag = true;
						particle.Time += dt;
						particle.TimeToLive -= dt/4f;
						if (particle.TimeToLive > 0f)
						{
							if (particle.Position.Y - m_position.Y <= 70f)
							particle.Position.Y += particle.Speed * dt;
							if (particle.Size.LengthSquared()/m_size < 60f && particle.Position.Y - m_position.Y >= 50f)
							particle.Size *= 1f+dt;
							particle.TextureSlot = (int)MathUtils.Min(9f * particle.Time / 1.25f, 4f);
							if (9f * particle.Time / 1.25f >= 4f)
								particle.Time = 0f;
						}
						else
						{
							particle.IsActive = false;
						}
					}
					else if (m_toGenerate >= 1f)
					{
						particle.IsActive = true;
						particle.Position = m_position + 0.25f * m_size * new Vector3(m_random.UniformFloat(-1f, 1f), 0f, m_random.UniformFloat(-1f, 1f));
						particle.Color = Color.White;
						particle.Size = new Vector2(m_size);
						particle.Speed = m_random.UniformFloat(0.45f, 0.55f) * m_size / 0.15f;
						particle.Time = 0f;
						particle.TimeToLive = m_random.UniformFloat(0.5f, 2f);
						particle.FlipX = (m_random.UniformInt(0, 1) == 0);
						particle.FlipY = (m_random.UniformInt(0, 1) == 0);
						m_toGenerate -= 1f;
					}
				}
				m_toGenerate = MathUtils.Remainder(m_toGenerate, 1f);
			}
			if (m_age >= 12f)
			{
				m_visible = false;
				IsStopped = true;
			}
			
			if (IsStopped)
			{
				return !flag;
			}
			return false;
		}

	}
	public class NParticleSystem2 : ParticleSystem<FireParticleSystem.Particle>
	{
		public class Particle : Game.Particle
		{
			public float Time;

			public float TimeToLive;

			public float Speed;
		}

		public Random m_random = new Random();

		public Vector3 m_position;

		public float m_size;

		public float m_toGenerate;

		public bool m_visible;

		public float m_maxVisibilityDistance;

		public float m_age;

		public bool IsStopped_;

		public bool IsStopped
		{
			get
			{
				return IsStopped_;
			}
			set
			{
				IsStopped_ = value;
			}
		}

		public NParticleSystem2(Vector3 position, float size, float maxVisibilityDistance)
			: base(20)
		{
			m_position = position;
			m_size = size;
			m_age = 0f;
			m_visible = true;
			m_maxVisibilityDistance = maxVisibilityDistance;
			Texture = ContentManager.Get<Texture2D>("Textures/FireParticle");
			TextureSlotsCount = 3;
			//m_color = new Color(num4, num4, num4);
		}

		public override bool Simulate(float dt)
		{
			m_age += dt;
			bool flag = false;
			if (m_visible)
			{
				m_toGenerate += (IsStopped ? 0f : (5f * dt));
				for (int i = 0; i < Particles.Length; i++)
				{
					FireParticleSystem.Particle particle = Particles[i];
					if (particle.IsActive)
					{
						flag = true;
						particle.Time += dt;
						particle.TimeToLive -= dt / 4f;
						if (particle.TimeToLive > 0f)
						{
							
							if (particle.Size.LengthSquared() / m_size < 60f)
								particle.Size *= 1f + dt;
							particle.Position.Y += particle.Speed * dt;
							particle.TextureSlot = (int)MathUtils.Min(9f * particle.Time / 1.25f, 4f);
							if (9f * particle.Time / 1.25f >= 4f)
								particle.Time = 0f;
						}
						else
						{
							particle.IsActive = false;
						}
					}
					else if (m_toGenerate >= 1f)
					{
						particle.IsActive = true;
						particle.Position = m_position + 0.25f * m_size * new Vector3(m_random.UniformFloat(-1f, 1f), 0f, m_random.UniformFloat(-1f, 1f));
						particle.Color = Color.White;
						particle.Size = new Vector2(m_size);
						particle.Speed = m_random.UniformFloat(0.45f, 0.55f)  *10f/ 0.15f /6f;
						particle.Time = 0f;
						particle.TimeToLive = m_random.UniformFloat(0.5f, 2f);
						particle.FlipX = (m_random.UniformInt(0, 1) == 0);
						particle.FlipY = (m_random.UniformInt(0, 1) == 0);
						m_toGenerate -= 1f;
					}
				}
				m_position += 0.5f * m_size / 0.15f * dt * new Vector3(0f, 1f, 0f) / 6f;
				m_toGenerate = MathUtils.Remainder(m_toGenerate, 1f);
			}
			if (m_age >= 15f)
			{
				m_visible = false;
				IsStopped = true;
			}

			if (IsStopped)
			{
				return !flag;
			}
			return false;
		}

	}
}