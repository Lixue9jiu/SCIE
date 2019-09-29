using Engine;
using Engine.Graphics;

namespace Game
{
	public class LaserBeam : ParticleSystem<GunSmokeParticleSystem.Particle>
	{
		public Random m_random = new Random();

		public float m_time;

		public float m_toGenerate;

		public Vector3 m_position;

		public Vector3 m_direction;

		public Vector3 m_position2;
		public Color m_color;

		public LaserBeam(SubsystemTerrain terrain, Vector3 position, Vector3 position2)
			: base(1)
		{
			Texture = ContentManager.Get<Texture2D>("Textures/FireParticle");
			TextureSlotsCount = 3;
			m_position = position;
			m_position2 = position2;
			//m_direction = Vector3.Normalize(direction);
			m_lightningStrikeBrightness = 1f;
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


		public PrimitivesRenderer3D m_primitivesRenderer3d = new PrimitivesRenderer3D();

		public float m_lightningStrikeBrightness;

		public override bool Simulate(float dt)
		{
			m_time += dt;
			
			FlatBatch3D flatBatch3D = m_primitivesRenderer3d.FlatBatch(0, DepthStencilState.DepthRead, null, BlendState.Additive);
			var color = new Color(51, 51, 51, 51);
			var color2 = new Color(51, 51, 51, 51);
			var color3 = new Color(51, 51, 51, 51);
			var color4 = new Color(51, 51, 51, 51);
			flatBatch3D.QueueLine(m_position, m_position2, color, color2);
			flatBatch3D.QueueLine(m_position, m_position2, color2, color3);
			flatBatch3D.QueueLine(m_position, m_position2, color3, color4);
				
			return (m_time >= 0.7f);
		}
	}
}