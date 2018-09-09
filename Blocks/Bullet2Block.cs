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

		protected static readonly string[] m_displayNames = new string[]
		{
			"IronFixedBullet"
		};
		protected static readonly float[] m_sizes = new float[]
		{
			1f
		};
		protected static readonly int[] m_textureSlots = new int[]
		{
			226
		};
		protected static readonly float[] m_weaponPowers = new float[]
		{
			10f
		};
		//protected static readonly float[] m_explosionPressures = new float[3];

		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			int bulletType = (int)GetBulletType(Terrain.ExtractData(value));
			float size2 = (bulletType >= 0 && bulletType < m_sizes.Length) ? (size * m_sizes[bulletType]) : size;
			BlocksManager.DrawFlatBlock(primitivesRenderer, value, size2, ref matrix, null, color, false, environmentData);
		}

		public override int GetFaceTextureSlot(int face, int value)
		{
			int bulletType = (int)GetBulletType(Terrain.ExtractData(value));
			return bulletType < 0 || bulletType >= m_textureSlots.Length ? 226 : m_textureSlots[bulletType];
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
