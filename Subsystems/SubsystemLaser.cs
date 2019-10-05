// Game.SubsystemSky
using Engine;
using Engine.Graphics;
using GameEntitySystem;

namespace Game
{
	public class SubsystemLaser : Subsystem, IDrawable
	{
		public PrimitivesRenderer3D m_primitivesRenderer3d = new PrimitivesRenderer3D();

		public Vector3?[] m_lightningStrikePosition = new Vector3?[666];

		public Vector3?[] m_lightningStartPosition = new Vector3?[666];

		public float?[] m_lightningStrikeBrightness = new float?[666];

		public FlatBatch3D flatBatch3D;

		public int[] m_drawOrders = new[] { 500 };

		public int[] DrawOrders => m_drawOrders;

		public void MakeLightningStrike(Vector3 targetPosition, Vector3 startPosition)
		{
			for (int nu1 = 0; nu1 < m_lightningStrikePosition.Length; nu1++)
			{
				if (m_lightningStrikePosition[nu1] == null)
				{
					m_lightningStrikePosition[nu1] = targetPosition;
					m_lightningStartPosition[nu1] = startPosition;
					m_lightningStrikeBrightness[nu1] = 1f;
					break;
				}
			}
		}

		public void Draw(Camera camera, int drawOrder)
		{
			flatBatch3D = m_primitivesRenderer3d.FlatBatch(0, DepthStencilState.DepthRead, null, BlendState.Additive);
			//if (Utils.SubsystemTime.PeriodicGameTimeEvent(0.05, 0.0))
			for (int nu1 = 0; nu1 < m_lightningStrikePosition.Length; nu1++)
			{
				if (m_lightningStrikeBrightness[nu1].HasValue)
				{
					Vector3 p = m_lightningStrikePosition[nu1].Value;
					Vector3 p2 = m_lightningStartPosition[nu1].Value;
					flatBatch3D.QueueQuad(p + new Vector3(0, 0.01f, 0), p2 + new Vector3(0, 0.01f, 0), p - new Vector3(0, 0.01f, 0), p2 - new Vector3(0, 0.01f, 0), Color.Red);
					flatBatch3D.QueueQuad(p2 + new Vector3(0, 0.01f, 0), p + new Vector3(0, 0.01f, 0), p2 - new Vector3(0, 0.01f, 0), p - new Vector3(0, 0.01f, 0), Color.Red);
					flatBatch3D.QueueQuad(p + new Vector3(0, 0, 0.01f), p2 + new Vector3(0, 0, 0.01f), p - new Vector3(0, 0, 0.01f), p2 - new Vector3(0, 0, 0.01f), Color.Red);
					flatBatch3D.QueueQuad(p2 + new Vector3(0, 0, 0.01f), p + new Vector3(0, 0, 0.01f), p2 - new Vector3(0, 0, 0.01f), p - new Vector3(0, 0, 0.01f), Color.Red);
					flatBatch3D.QueueQuad(p + new Vector3(0.01f, 0, 0), p2 + new Vector3(0.01f, 0, 0), p - new Vector3(0.01f, 0, 0), p2 - new Vector3(0.01f, 0, 0), Color.Red);
					flatBatch3D.QueueQuad(p2 + new Vector3(0.01f, 0, 0), p + new Vector3(0.01f, 0, 0), p2 - new Vector3(0.01f, 0, 0), p - new Vector3(0.01f, 0, 0), Color.Red);
					m_lightningStrikeBrightness[nu1] -= Utils.SubsystemTime.GameTimeDelta / 0.8f * 3;
					if (m_lightningStrikeBrightness[nu1] <= 0f)
					{
						m_lightningStrikePosition[nu1] = null;
						m_lightningStartPosition[nu1] = null;
						m_lightningStrikeBrightness[nu1] = null;
					}
				}
			}
			m_primitivesRenderer3d.Flush(camera.ViewProjectionMatrix);
		}
	}
}