using Engine;
using Engine.Graphics;
using System.Collections.Generic;

namespace Game
{
	public abstract class FourDirectionalBlock : CubeBlock, IPaintableBlock
	{
		//protected readonly BlockMesh[] m_blockMeshesByData = new BlockMesh[4];

		//protected readonly BlockMesh m_standaloneBlockMesh = new BlockMesh();

		/*public override int GetFaceTextureSlot(int face, int value)
		{
			return DefaultTextureSlot;
		}*/
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometrySubsets geometry, int value, int x, int y, int z)
		{
			generator.GenerateCubeVertices(this, value, x, y, z, SubsystemPalette.GetColor(generator, GetPaintColor(value)), geometry.OpaqueSubsetsByFace);
		}

		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			color *= SubsystemPalette.GetColor(environmentData, GetPaintColor(value));
			BlocksManager.DrawCubeBlock(primitivesRenderer, value, new Vector3(size), ref matrix, color, color, environmentData);
		}
		public override IEnumerable<int> GetCreativeValues()
		{
			var array = new int[17];
			array[0] = BlockIndex;
			for (int i = 1; i < 17; i++)
			{
				array[i] = BlockIndex | SetColor(0, i - 1) << 14;
			}
			return array;
		}
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			return new BlockPlacementData
			{
				Value = Terrain.ReplaceData(BlockIndex, SetDirection(Terrain.ExtractData(value), Utils.GetDirectionXZ(componentMiner))),
				CellFace = raycastResult.CellFace
			};
		}

		public override bool IsFaceTransparent(SubsystemTerrain subsystemTerrain, int face, int value)
		{
			return false;
		}

		public override BlockDebrisParticleSystem CreateDebrisParticleSystem(SubsystemTerrain subsystemTerrain, Vector3 position, int value, float strength)
		{
			return new BlockDebrisParticleSystem(subsystemTerrain, position, strength, DestructionDebrisScale, SubsystemPalette.GetColor(subsystemTerrain, GetPaintColor(value)), GetFaceTextureSlot(0, value));
		}

		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			return SubsystemPalette.GetName(subsystemTerrain, GetPaintColor(value), DefaultDisplayName);
		}

		public override string GetCategory(int value)
		{
			return GetPaintColor(value).HasValue ? "Painted" : base.GetCategory(value);
		}

		public int? GetPaintColor(int value)
		{
			return GetColor(Terrain.ExtractData(value));
		}

		public int Paint(SubsystemTerrain terrain, int value, int? color)
		{
			return Terrain.ReplaceData(value, SetColor(Terrain.ExtractData(value), color));
		}

		public static int? GetColor(int data)
		{
			if ((data & 0x10) != 0)
			{
				return (data >> 5) & 0xF;
			}
			return null;
		}

		public static int SetColor(int data, int? color)
		{
			if (color.HasValue)
			{
				return (data & -497) | 0x10 | ((color.Value & 0xF) << 5);
			}
			return data & -497;
		}

		public static int GetDirection(int value)
		{
			return Terrain.ExtractData(value) & 7;
		}

		public static int SetDirection(int data, int direction)
		{
			return (data & -8) | (direction & 7);
		}
	}
	public abstract class SixDirectionalBlock : FourDirectionalBlock
	{
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			Vector3 forward = Matrix.CreateFromQuaternion(componentMiner.ComponentCreature.ComponentCreatureModel.EyeRotation).Forward;
			float num = Vector3.Dot(forward, Vector3.UnitZ);
			float num2 = Vector3.Dot(forward, Vector3.UnitX);
			float num3 = Vector3.Dot(forward, -Vector3.UnitZ);
			float num4 = Vector3.Dot(forward, -Vector3.UnitX);
			float num5 = Vector3.Dot(forward, Vector3.UnitY);
			float num6 = Vector3.Dot(forward, -Vector3.UnitY);
			float num7 = MathUtils.Min(MathUtils.Min(num, num2, num3), MathUtils.Min(num4, num5, num6));
			int direction = 0;
			if (num == num7)
			{
				direction = 0;
			}
			else if (num2 == num7)
			{
				direction = 1;
			}
			else if (num3 == num7)
			{
				direction = 2;
			}
			else if (num4 == num7)
			{
				direction = 3;
			}
			else if (num5 == num7)
			{
				direction = 4;
			}
			else if (num6 == num7)
			{
				direction = 5;
			}
			return new BlockPlacementData
			{
				Value = Terrain.ReplaceData(BlockIndex, SetDirection(0, direction)),
				CellFace = raycastResult.CellFace
			};
		}
	}
}
