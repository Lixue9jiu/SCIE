using Engine;
using Engine.Graphics;

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
			int num = Terrain.ToCell(position.X);
			int num2 = Terrain.ToCell(position.Y);
			int num3 = Terrain.ToCell(position.Z);
			int x = 0;
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
			if (!flag && m_time >= 0.5f)
			{
				return true;
			}
			return false;
		}
	}
}