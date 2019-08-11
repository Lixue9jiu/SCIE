using Engine;
using Engine.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace Game
{
	public class BulletBlock : FlatBlock
	{
		public enum BulletType
		{
			MusketBall,
			Buckshot,
			BuckshotBall,
			IronBullet,
            CBullet
		}

		public const int Index = 214;

		protected static readonly string[] m_displayNames = new[]
		{
			"Musket Ball", "Buckshot", "Buckshot Ball", "IronBullet", "C-Bullet"
		};

		protected static readonly float[] m_sizes = new[] { 1f, 1f, 0.33f, 1f ,0.5f};

		protected static readonly int[] m_textureSlots = new[] { 229, 231, 229, 229 ,229};

		protected static readonly float[] m_weaponPowers = new[] { 60f, 0f, 3.6f, 90f ,50f};

		protected static readonly float[] m_explosionPressures = new[] { 0f, 0f, 0f, 0.1f ,0.1f};

		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			int bulletType = GetBulletType(Terrain.ExtractData(value));
			float size2 = (bulletType < 0 || bulletType >= m_sizes.Length) ? size : (size * m_sizes[bulletType]);
			BlocksManager.DrawFlatBlock(primitivesRenderer, value, size2, ref matrix, null, color, false, environmentData);
		}

		public override float GetProjectilePower(int value)
		{
			int bulletType = GetBulletType(Terrain.ExtractData(value));
			return bulletType < 0 || bulletType >= m_weaponPowers.Length ? 0f : m_weaponPowers[bulletType];
		}

		public override float GetExplosionPressure(int value)
		{
			int bulletType = GetBulletType(Terrain.ExtractData(value));
			return bulletType < 0 || bulletType >= m_explosionPressures.Length ? 0f : m_explosionPressures[bulletType];
		}

		public override IEnumerable<int> GetCreativeValues()
		{
			var arr = EnumUtils.GetEnumValues(typeof(BulletType)).ToArray();
			for (int i = 0; i < arr.Length; i++)
				arr[i] = Terrain.MakeBlockValue(214, 0, SetBulletType(0, (BulletType)i));
			return arr;
		}

		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			int bulletType = GetBulletType(Terrain.ExtractData(value));
			if (bulletType < 0 || bulletType >= m_displayNames.Length)
				return string.Empty;
			return m_displayNames[bulletType];
		}

		public override int GetFaceTextureSlot(int face, int value)
		{
			int bulletType = GetBulletType(Terrain.ExtractData(value));
			if (bulletType < 0 || bulletType >= m_textureSlots.Length)
				return 229;
			return m_textureSlots[bulletType];
		}

		public static int GetBulletType(int data)
		{
			return data & 0xF;
		}

		public static int SetBulletType(int data, BulletType bulletType)
		{
			return (data & -16) | (int)(bulletType & (BulletType)15);
		}
	}
}