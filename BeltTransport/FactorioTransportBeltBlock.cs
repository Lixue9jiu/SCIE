using Engine;
using Engine.Graphics;
using Engine.Media;
using System.Collections.Generic;
using System.IO;

namespace Game
{
	public class FactorioTransportBeltBlock : FlatBlock
	{
		public const int Index = 400;
		public static Texture2D[] m_textures;

		public static int[,] m_cornerType2Rotations = new int[8, 3] {
            //{OriginalRotation, TailRotation, TailBackRotation}
            {0,1,3 },
			{0,3,1 },
			{3,2,0 },
			{1,2,0 },
			{2,3,1 },
			{2,1,3 },
			{3,0,2 },
			{1,0,2 }
		};

		public static int?[,] m_rotations2CornerType = new int?[4, 2] {
            //{OriginalRotation, IsTailOnRight(true 1; false 0)}
            {1,0},
			{7,3},
			{5,4},
			{2,6}
		};

		public static string[] m_displayNames = new[]
		{
			"Factorio ",
			"Factorio Fast ",
			"Factorio Express "
		};

		public override void Initialize()
		{
			for (int y = 0; y < m_texRowCount; y++)
				for (int x = 0; x < m_texColCount; x++)
					m_texCoords[y, x] = XYToTextureCoords(x, y);
			//base.Initialize();
		}

		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			ItemBlock.DrawFlatBlock(primitivesRenderer, value, size, ref matrix, m_textures[GetColor(Terrain.ExtractData(value))], color, false, environmentData);
		}

		public override BlockDebrisParticleSystem CreateDebrisParticleSystem(SubsystemTerrain subsystemTerrain, Vector3 position, int value, float strength)
		{
			return new BlockDebrisParticleSystem(subsystemTerrain, position, 0f, DestructionDebrisScale, Color.White, GetFaceTextureSlot(4, value));
		}

		public override IEnumerable<int> GetCreativeValues()
		{
			return new[] { Terrain.ReplaceData(BlockIndex, SetColor(0, 0)), Terrain.ReplaceData(BlockIndex, SetColor(0, 1)), Terrain.ReplaceData(BlockIndex, SetColor(0, 2)) };
		}

		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			return m_displayNames[GetColor(Terrain.ExtractData(value))] + base.GetDisplayName(subsystemTerrain, value);
		}

		public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
		{
			showDebris = false;
			int color = GetColor(Terrain.ExtractData(oldValue));
			dropValues.Add(new BlockDropValue
			{
				Value = Terrain.MakeBlockValue(BlockIndex, 0, SetColor(0, color)),
				Count = 1
			});
		}

		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			Vector3 forward = componentMiner.ComponentCreature.ComponentCreatureModel.EyeRotation.ToForwardVector();
			float num = Vector3.Dot(forward, Vector3.UnitZ),
				  num2 = Vector3.Dot(forward, Vector3.UnitX),
				  num3 = Vector3.Dot(forward, -Vector3.UnitZ),
				  num4 = Vector3.Dot(forward, -Vector3.UnitX);
			int rotation = 0;
			if (num == MathUtils.Max(num, num2, num3, num4))
			{
				rotation = 2;
			}
			else if (num2 == MathUtils.Max(num, num2, num3, num4))
			{
				rotation = 3;
			}
			else if (num3 == MathUtils.Max(num, num2, num3, num4))
			{
				rotation = 0;
			}
			else if (num4 == MathUtils.Max(num, num2, num3, num4))
			{
				rotation = 1;
			}
			int data = Terrain.ExtractData(value);
			return new BlockPlacementData
			{
				Value = Terrain.MakeBlockValue(BlockIndex, 0, SetRotation(data, rotation)),
				CellFace = raycastResult.CellFace
			};
		}
		public override int GetFaceTextureSlot(int face, int value)
		{
			return 0;
		}

		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
		{
			//value = Terrain.ExtractData(value);
			return RailBlock.boundingBoxes[/*GetSlopeType(value).HasValue ? 9 - GetRotation(value) : */0];
		}

		//0 z-; 1 x-; 2 z+; 3 x+
		public static int GetRotation(int data)
		{
			return data & 3;
		}

		public static int SetRotation(int data, int rotation)
		{
			return (data & -4) | (rotation & 3);
		}

		public static Point3 RotationToDirection(int rotation)
		{
			return CellFace.FaceToPoint3((rotation + 2) % 4);
		}

		public static int GetColor(int data)
		{
			return data >> 3 & 3;
		}

		public static int SetColor(int data, int color)
		{
			return (data & -25) | ((color & 3) << 3);
		}

		public static bool? GetSlopeType(int data)
		{
			return (data & 4) != 0 ? new bool?((data & 512) != 0) : null;
		}

		public static int SetSlopeType(int data, bool? slopeType)
		{
			return slopeType != null ? (data & -513) | 4 | (slopeType.Value ? 1 : 0) << 9 : data & -5;
		}

		public static int? GetCornerType(int data)
		{
			return (data & 32) != 0 ? data >> 6 & 7 : (int?)null;
		}

		public static int SetCornerType(int data, int? cornertype)
		{
			return cornertype != null ? (data & -481) | 32 | (cornertype.Value & 7) << 6 : data & -481;
		}

		public static int m_texRowCount = 12;
		public static int m_texColCount = 16;
		public static Vector4[,] m_texCoords = new Vector4[m_texRowCount, m_texColCount];

		public static Vector4 XYToTextureCoords(int x, int y)
		{
			return new Vector4(
				x / (float)m_texColCount,
				y / (float)m_texRowCount,
				(x + 1f) / m_texColCount,
				(y + 1f) / m_texRowCount);
		}
	}
}