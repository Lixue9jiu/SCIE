// Game.SubsystemSky
using Engine;
using Engine.Graphics;
using Game;
using GameEntitySystem;
using System;
using TemplatesDatabase;

namespace Game
{
	public class SubsystemLaser : Subsystem, IDrawable, IUpdateable
	{
		public SubsystemTimeOfDay m_subsystemTimeOfDay;

		public SubsystemTime m_subsystemTime;

		public SubsystemGameInfo m_subsystemGameInfo;

		public SubsystemTerrain m_subsystemTerrain;

		public SubsystemWeather m_subsystemWeather;

		public SubsystemAudio m_subsystemAudio;

		public SubsystemBodies m_subsystemBodies;

		public SubsystemFluidBlockBehavior m_subsystemFluidBlockBehavior;

		public PrimitivesRenderer2D m_primitivesRenderer2d = new PrimitivesRenderer2D();

		public PrimitivesRenderer3D m_primitivesRenderer3d = new PrimitivesRenderer3D();

		public Game.Random m_random = new Game.Random();

		public Color m_viewFogColor;

		public Vector2 m_viewFogRange;

		public bool m_viewIsSkyVisible;

		public Texture2D m_sunTexture;

		public Texture2D m_glowTexture;

		public Texture2D m_cloudsTexture;

		public Texture2D[] m_moonTextures = new Texture2D[8];

		public static UnlitShader m_shaderFlat = new UnlitShader(useVertexColor: true, useTexture: false, useAlphaThreshold: false);

		public static UnlitShader m_shaderTextured = new UnlitShader(useVertexColor: true, useTexture: true, useAlphaThreshold: false);

		public VertexDeclaration m_skyVertexDeclaration = new VertexDeclaration(new VertexElement(0, VertexElementFormat.Vector3, VertexElementSemantic.Position), new VertexElement(12, VertexElementFormat.NormalizedByte4, VertexElementSemantic.Color));

		//public Dictionary<View, SkyDome> m_skyDomes = new Dictionary<View, SkyDome>();

		public VertexBuffer m_starsVertexBuffer;

		public IndexBuffer m_starsIndexBuffer;

		public VertexDeclaration m_starsVertexDeclaration = new VertexDeclaration(new VertexElement(0, VertexElementFormat.Vector3, VertexElementSemantic.Position), new VertexElement(12, VertexElementFormat.Vector2, VertexElementSemantic.TextureCoordinate), new VertexElement(20, VertexElementFormat.NormalizedByte4, VertexElementSemantic.Color));

		public const int m_starsCount = 150;

		public Vector3?[] m_lightningStrikePosition = new Vector3?[9999];

		public Vector3?[] m_lightningStartPosition = new Vector3?[9999];

		public float?[] m_lightningStrikeBrightness = new float?[9999];

		public double m_lastLightningStrikeTime;

		public const float DawnStart = 0.2f;

		public const float DayStart = 0.3f;

		public const float DuskStart = 0.7f;

		public const float NightStart = 0.8f;

		public bool DrawSkyEnabled = true;

		public bool DrawCloudsWireframe;

		public int[] m_drawOrders = new int[3]
		{
		-100,
		5,
		105
		};

		public static int[] m_lightValuesMoonless = new int[6]
		{
		0,
		3,
		6,
		9,
		12,
		15
		};

		public static int[] m_lightValuesNormal = new int[6]
		{
		3,
		5,
		8,
		10,
		13,
		15
		};

		public float SkyLightIntensity_;

		public int SkyLightValue_;

		public float VisibilityRange_;

		public float ViewUnderWaterDepth_;

		public float ViewUnderMagmaDepth_;

		public float SkyLightIntensity
		{
			get
			{
				return SkyLightIntensity_;
			}
			set
			{
				SkyLightIntensity_ = value;
			}
		}



		public float VisibilityRange
		{
			get
			{
				return VisibilityRange_;
			}
			set
			{
				VisibilityRange_ = value;
			}
		}

		public int UpdateOrder => 0;

		public int[] DrawOrders => m_drawOrders;

		public void MakeLightningStrike(Vector3 targetPosition, Vector3 startPosition)
		{
			//m_lightningStrikePosition.
			//m_lastLightningStrikeTime = m_subsystemTime.GameTime;
			for (int nu1=0; nu1<m_lightningStrikePosition.Length; nu1++)
			{
				if (m_lightningStrikePosition[nu1]==null)
				{
					m_lightningStrikePosition[nu1]= targetPosition;
					m_lightningStartPosition[nu1] = startPosition;
					m_lightningStrikeBrightness[nu1] = 1f;
					break;
				}
			}
		}

		public void Update(float dt)
		{
			//MoonPhase = ((int)MathUtils.Floor(m_subsystemTimeOfDay.Day - 0.5 + 5.0) % 8 + 8) % 8;
			//UpdateLightAndViewParameters();
		}

		public void Draw(Camera camera, int drawOrder)
		{
			DrawLightning(camera);
			m_primitivesRenderer3d.Flush(camera.ViewProjectionMatrix);
		}

		public override void Load(ValuesDictionary valuesDictionary)
		{
			//base.Load(valuesDictionary);
			//Utils.Load(Project);
			m_subsystemTimeOfDay = base.Project.FindSubsystem<SubsystemTimeOfDay>(throwOnError: true);
			m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(throwOnError: true);
			m_subsystemGameInfo = base.Project.FindSubsystem<SubsystemGameInfo>(throwOnError: true);
			m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(throwOnError: true);
			m_subsystemWeather = base.Project.FindSubsystem<SubsystemWeather>(throwOnError: true);
			m_subsystemAudio = base.Project.FindSubsystem<SubsystemAudio>(throwOnError: true);
			m_subsystemBodies = base.Project.FindSubsystem<SubsystemBodies>(throwOnError: true);
			m_subsystemFluidBlockBehavior = base.Project.FindSubsystem<SubsystemFluidBlockBehavior>(throwOnError: true);
			Display.DeviceReset += Display_DeviceReset;
		}

		public override void Dispose()
		{
			Display.DeviceReset -= Display_DeviceReset;
			Utilities.Dispose(ref m_starsVertexBuffer);
			Utilities.Dispose(ref m_starsIndexBuffer);
		}

		public void Display_DeviceReset()
		{
			Utilities.Dispose(ref m_starsVertexBuffer);
			Utilities.Dispose(ref m_starsIndexBuffer);
		}

		public void DrawLightning(Camera camera)
		{
			for (int nu1 = 0; nu1 < m_lightningStrikePosition.Length; nu1++)
			{
				if (!m_lightningStrikePosition[nu1].HasValue || !m_lightningStartPosition[nu1].HasValue || !m_lightningStrikeBrightness[nu1].HasValue)
				{
					break;
				}
				FlatBatch3D flatBatch3D = m_primitivesRenderer3d.FlatBatch(0, DepthStencilState.DepthRead, null, BlendState.Additive);
				Vector3 value = m_lightningStrikePosition[nu1].Value;
				Vector3 start = m_lightningStartPosition[nu1].Value;
				Vector3 dir = Vector3.Normalize(new Vector3(1, 0, 0) * (m_lightningStrikePosition[nu1].Value.X - m_lightningStartPosition[nu1].Value.X) + new Vector3(0, 1, 0) * (m_lightningStrikePosition[nu1].Value.Y - m_lightningStartPosition[nu1].Value.Y) + new Vector3(0, 0, 1) * (m_lightningStrikePosition[nu1].Value.Z - m_lightningStartPosition[nu1].Value.Z));
				Vector3 unitY = Vector3.UnitY;
				Vector3 v = Vector3.Normalize(Vector3.Cross(camera.ViewDirection, unitY));
				Viewport viewport = Display.Viewport;
				float num = Vector4.Transform(new Vector4(value, 1f), camera.ViewProjectionMatrix).W * 2f / ((float)viewport.Width * camera.ProjectionMatrix.M11);
				for (int i = 0; i < (int)(m_lightningStrikeBrightness[nu1] * 5f); i++)
				{
					float s = m_random.NormalFloat(0f, 1f * num);
					float s2 = m_random.NormalFloat(0f, 1f * num);
					Vector3 v2 = dir;
					float num4;
					for (float num2 = 130f; num2 > value.Y; num2 -= num4)
					{
						uint num3 = MathUtils.Hash((uint)(m_lightningStrikePosition[nu1].Value.X + 100f * m_lightningStrikePosition[nu1].Value.Z + 200f * num2));
						num4 = MathUtils.Lerp(4f, 10f, (float)(double)(num3 & 0xFF) / 255f);
						float s3 = ((num3 & 1) == 0) ? 1 : (-1);
						float s4 = MathUtils.Lerp(0.05f, 0.2f, (float)(double)((num3 >> 8) & 0xFF) / 255f);
						float num5 = num2;
						float num6 = num5 - num4 * MathUtils.Lerp(0.45f, 0.55f, (float)(double)((num3 >> 16) & 0xFF) / 255f);
						float num7 = num5 - num4 * MathUtils.Lerp(0.45f, 0.55f, (float)(double)((num3 >> 24) & 0xFF) / 255f);
						float num8 = num5 - num4;
						Vector3 p = value;
						Vector3 vector = new Vector3(value.X, num6, value.Z) + v2 - num4 * v * s3 * s4;
						Vector3 vector2 = new Vector3(value.X, num7, value.Z) + v2 + num4 * v * s3 * s4;
						Vector3 p2 = start;
						Color color = Color.Red * 0.2f * MathUtils.Saturate((130f - num5) * 0.2f);
						Color color2 = Color.Red * 0.2f * MathUtils.Saturate((130f - num6) * 0.2f);
						Color color3 = Color.Red * 0.2f * MathUtils.Saturate((130f - num7) * 0.2f);
						Color color4 = Color.Red * 0.2f * MathUtils.Saturate((130f - num8) * 0.2f);
						flatBatch3D.QueueLine(p, p2, color, color2);
						flatBatch3D.QueueLine(p, p2, color2, color3);
						flatBatch3D.QueueLine(p, p2, color3, color4);
					}
				}
				float num9 = MathUtils.Lerp(0.3f, 0.75f, 0.5f * (float)MathUtils.Sin(MathUtils.Remainder(1.0 * m_subsystemTime.GameTime, 6.2831854820251465)) + 0.5f);
				m_lightningStrikeBrightness[nu1] -= m_subsystemTime.GameTimeDelta / 0.8f;
				if (m_lightningStrikeBrightness[nu1] <= 0f)
				{
					m_lightningStrikePosition[nu1] = null;
					m_lightningStartPosition[nu1] = null;
					m_lightningStrikeBrightness[nu1] = null;
				}
			}
		}


		public void QueueCelestialBody(TexturedBatch3D batch, Vector3 viewPosition, Color color, float distance, float radius, float angle)
		{
			if (color.A > 0)
			{
				Vector3 vector = default(Vector3);
				vector.X = 0f - MathUtils.Sin(angle);
				vector.Y = 0f - MathUtils.Cos(angle);
				vector.Z = 0f;
				Vector3 vector2 = vector;
				Vector3 unitZ = Vector3.UnitZ;
				Vector3 v = Vector3.Cross(unitZ, vector2);
				Vector3 p = viewPosition + vector2 * distance - radius * unitZ - radius * v;
				Vector3 p2 = viewPosition + vector2 * distance + radius * unitZ - radius * v;
				Vector3 p3 = viewPosition + vector2 * distance + radius * unitZ + radius * v;
				Vector3 p4 = viewPosition + vector2 * distance - radius * unitZ + radius * v;
				batch.QueueQuad(p, p2, p3, p4, new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(1f, 1f), new Vector2(0f, 1f), color);
			}
		}

		public float CalculateLightIntensity(float timeOfDay)
		{
			if (timeOfDay <= 0.2f || timeOfDay > 0.8f)
			{
				return 0f;
			}
			if (timeOfDay > 0.2f && timeOfDay <= 0.3f)
			{
				return (timeOfDay - 0.2f) / (71f / (226f * (float)Math.PI));
			}
			if (timeOfDay > 0.3f && timeOfDay <= 0.7f)
			{
				return 1f;
			}
			return 1f - (timeOfDay - 0.7f) / 0.100000024f;
		}


	}
}
