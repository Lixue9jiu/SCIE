using Engine;
using Engine.Graphics;
using System.Collections.Generic;

namespace Game
{
	public class BulletBlock : FlatBlock
	{
		public enum BulletType
		{
			MusketBall,
			Buckshot,
			BuckshotBall,
			IronBullet
		}

		public const int Index = 214;

		protected static readonly string[] m_displayNames = new string[4]
		{
			"Musket Ball", "Buckshot", "Buckshot Ball", "IronBullet"
		};

		protected static readonly float[] m_sizes = new float[4] { 1f, 1f, 0.33f, 1f };

		protected static readonly int[] m_textureSlots = new int[4] { 229, 231, 229, 229 };

		protected static readonly float[] m_weaponPowers = new float[4] { 60f, 0f, 3.6f, 90f };

		protected static readonly float[] m_explosionPressures = new float[4] { 0f,0f,0f,1f};

		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometrySubsets geometry, int value, int x, int y, int z)
		{
		}

		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			int bulletType = (int)GetBulletType(Terrain.ExtractData(value));
			float size2 = (bulletType < 0 || bulletType >= m_sizes.Length) ? size : (size * m_sizes[bulletType]);
			BlocksManager.DrawFlatBlock(primitivesRenderer, value, size2, ref matrix, null, color, false, environmentData);
		}

		public override float GetProjectilePower(int value)
		{
			int bulletType = (int)GetBulletType(Terrain.ExtractData(value));
			return bulletType < 0 || bulletType >= m_weaponPowers.Length ? 0f : m_weaponPowers[bulletType];
		}

		public override float GetExplosionPressure(int value)
		{
			int bulletType = (int)GetBulletType(Terrain.ExtractData(value));
			return bulletType < 0 || bulletType >= m_explosionPressures.Length ? 0f : m_explosionPressures[bulletType];
		}

		public override IEnumerable<int> GetCreativeValues()
		{
			var list = EnumUtils.GetEnumValues(typeof(BulletType));
			for (int i = 0; i < list.Count; i++)
			{
				yield return Terrain.MakeBlockValue(214, 0, SetBulletType(0, (BulletType)list[i]));
			}
		}

		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			int bulletType = (int)GetBulletType(Terrain.ExtractData(value));
			if (bulletType < 0 || bulletType >= m_displayNames.Length)
			{
				return string.Empty;
			}
			return m_displayNames[bulletType];
		}

		public override int GetFaceTextureSlot(int face, int value)
		{
			int bulletType = (int)GetBulletType(Terrain.ExtractData(value));
			if (bulletType < 0 || bulletType >= m_textureSlots.Length)
			{
				return 229;
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