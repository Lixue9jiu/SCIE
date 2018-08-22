using Engine;
using Engine.Graphics;

namespace Game
{
	public class Bullet2Block : FlatBlock
	{
		public enum BulletType
		{
			IronBullet
		}

		public const int Index = 521;

		private static readonly string[] m_displayNames = new string[]
		{
			"IronFixedBullet"
		};
		private static readonly float[] m_sizes = new float[]
		{
			1f
		};
		private static readonly int[] m_textureSlots = new int[]
		{
			226
		};
		private static readonly float[] m_weaponPowers = new float[]
		{
			10f
		};
		//private static readonly float[] m_explosionPressures = new float[3];

		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			int bulletType = (int)GetBulletType(Terrain.ExtractData(value));
			float size2 = (bulletType >= 0 && bulletType < m_sizes.Length) ? (size * m_sizes[bulletType]) : size;
			BlocksManager.DrawFlatBlock(primitivesRenderer, value, size2, ref matrix, null, color, false, environmentData);
		}

		public override int GetFaceTextureSlot(int face, int value)
		{
			int bulletType = (int)GetBulletType(Terrain.ExtractData(value));
			if (bulletType < 0 || bulletType >= m_textureSlots.Length)
			{
				return 226;
			}
			return m_textureSlots[bulletType];
		}

		public static BulletType GetBulletType(int data)
		{
			return (BulletType)(data & 0xF);
		}

		public static int SetBulletType(int data, BulletType bulletType)
		{
			return (data & -16) | (int)(bulletType & (BulletType)15);
		}
	}
}
