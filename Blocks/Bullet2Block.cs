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

		public const int Index = 525;

		private static string[] m_displayNames;

		private static float[] m_sizes;

		private static int[] m_textureSlots;

		private static float[] m_weaponPowers;

		private static float[] m_explosionPressures;

		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometrySubsets geometry, int value, int x, int y, int z)
		{
		}

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

		static Bullet2Block()
		{
			m_displayNames = new string[1]
			{
				"IronFixedBullet"
			};
			m_sizes = new float[1]
			{
				1f
			};
			m_textureSlots = new int[1]
			{
				226
			};
			m_weaponPowers = new float[1]
			{
				10f
			};
			m_explosionPressures = new float[3];
		}
	}
}
