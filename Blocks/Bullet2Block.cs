using Engine;
using Engine.Graphics;
using Game;
using System.Collections.Generic;

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

		/*public static int SetBulletType(int data, BulletType bulletType)
		{
			return (data & -16) | (int)(bulletType & (BulletType)15);
		}*/
	}
}
public class RottenMeatBlock : FluidBlock
{
	public BlockMesh m_standaloneBlockMesh;
	public const int Index = 240;

	public RottenMeatBlock() : base(1)
	{
	}
	public override void Initialize()
	{
		var rottenMeatBlock = new Game.RottenMeatBlock()
		{
			DefaultShadowStrength = -1
		};
		rottenMeatBlock.Initialize();
		m_standaloneBlockMesh = rottenMeatBlock.m_standaloneBlockMesh;
		base.Initialize();
	}
	public override IEnumerable<int> GetCreativeValues()
	{
		return new int[] { Index, Index | 1 << 4 << 14, Index | 1 << 8 << 14 };
	}
	public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
	{
		if (Terrain.ExtractData(value) >> 8 != 0)
			return;
		if (Terrain.ExtractData(value) != 0)
		{
			color = new Color(30, 30, 30);
			BlocksManager.DrawCubeBlock(primitivesRenderer, Terrain.ReplaceContents(value, 18), new Vector3(size), ref matrix, color, color, environmentData);
			return;
		}
		BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, color, 2f * size, ref matrix, environmentData);
	}
	public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometrySubsets geometry, int value, int x, int y, int z)
	{
		if (Terrain.ExtractData(value) >> 8 != 0)
			return;
		if (Terrain.ExtractData(value) != 0)
		{
			BlocksManager.FluidBlocks[WaterBlock.Index].GenerateFluidTerrainVertices(generator, value, x, y, z, new Color(30, 30, 30), new Color(30, 30, 30), geometry.OpaqueSubsetsByFace);
		}
	}
	public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
	{
		return new BlockPlacementData
		{
			Value = Terrain.ExtractData(value) == 0 ? 0 : Terrain.ReplaceLight(value, 0),
			CellFace = raycastResult.CellFace
		};
	}
	public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
	{
		return Terrain.ExtractData(value) >> 8 != 0 ? "Updraft" : Terrain.ExtractData(value) != 0 ? "Oil" : DefaultDisplayName;
	}
	public override string GetDescription(int value)
	{
		return Terrain.ExtractData(value) != 0 ? "" : DefaultDescription;
	}
	public override string GetCategory(int value)
	{
		return Terrain.ExtractData(value) != 0 ? "Terrain" : DefaultCategory;
	}
	public override Vector3 GetIconViewOffset(int value, DrawBlockEnvironmentData environmentData)
	{
		return Terrain.ExtractData(value) != 0 ? Vector3.One : DefaultIconViewOffset;
	}
}
