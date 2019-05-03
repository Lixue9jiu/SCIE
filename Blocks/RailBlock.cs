using System.Collections.Generic;
using Engine;
using Engine.Graphics;

namespace Game
{
	public class RailBlock : Block
	{
		public const int Index = 528;

		BoundingBox[][] boundingBoxes = new BoundingBox[10][];

		BlockMesh[] m_blockMeshes = new BlockMesh[10];

		public override void Initialize()
		{
			base.Initialize();
			var mesh = new BlockMesh();
			var vertices = new[]
			{
				new BlockMeshVertex
				{
					Color = Color.White,
					TextureCoordinates = new Vector2(0, 0.0625f),
					Position = new Vector3(0, 0.01f, 1)
				},
				new BlockMeshVertex
				{
					Color = Color.White,
					TextureCoordinates = new Vector2(0.0625f, 0.0625f),
					Position = new Vector3(1, 0.01f, 1)
				},
				new BlockMeshVertex
				{
					Color = Color.White,
					TextureCoordinates = new Vector2(0.0625f, 0),
					Position = new Vector3(1, 0.01f, 0)
				},
				new BlockMeshVertex
				{
					Color = Color.White,
					TextureCoordinates = new Vector2(0, 0),
					Position = new Vector3(0, 0.01f, 0)
				}
			};
			var indices = new ushort[] { 2, 1, 0, 0, 3, 2 };
			mesh.Vertices.AddRange(vertices);
			mesh.Indices.AddRange(indices);

			var flatMesh = new BlockMesh();
			flatMesh.AppendBlockMesh(mesh);
			flatMesh.TransformTextureCoordinates(Matrix.CreateTranslation(new Vector3(238 % 16 / 16f, 238 / 16 / 16f, 0f)));

			var center = Matrix.CreateTranslation(new Vector3(0.5f, 0, 0.5f));
			var reverseCenter = Matrix.CreateTranslation(new Vector3(-0.5f, 0, -0.5f));

			for (int i = 0; i < 4; i++)
			{
				m_blockMeshes[i] = new BlockMesh();
				m_blockMeshes[i].AppendBlockMesh(flatMesh);
				m_blockMeshes[i].TransformPositions(reverseCenter * Matrix.CreateRotationY(MathUtils.PI * 0.5f * i) * center);
				m_blockMeshes[i].GenerateSidesData();
			}

			flatMesh = new BlockMesh();
			flatMesh.AppendBlockMesh(mesh);
			flatMesh.TransformTextureCoordinates(Matrix.CreateTranslation(new Vector3(237 % 16 / 16f, 237 / 16 / 16f, 0f)));

			m_blockMeshes[4] = new BlockMesh();
			m_blockMeshes[4].AppendBlockMesh(flatMesh);
			m_blockMeshes[4].GenerateSidesData();

			m_blockMeshes[5] = new BlockMesh();
			m_blockMeshes[5].AppendBlockMesh(flatMesh);
			m_blockMeshes[5].TransformPositions(reverseCenter * Matrix.CreateRotationY(MathUtils.PI * 0.5f) * center);
			m_blockMeshes[5].GenerateSidesData();

			mesh = new BlockMesh();
			vertices = new BlockMeshVertex[]
			{
				new BlockMeshVertex
				{
					Color = Color.White,
					TextureCoordinates = new Vector2(0, 0.0625f),
					Position = new Vector3(0, 0.01f, 1)
				},
				new BlockMeshVertex
				{
					Color = Color.White,
					TextureCoordinates = new Vector2(0.0625f, 0.0625f),
					Position = new Vector3(1, 0.01f, 1)
				},
				new BlockMeshVertex
				{
					Color = Color.White,
					TextureCoordinates = new Vector2(0.0625f, 0),
					Position = new Vector3(1, 1.01f, 0)
				},
				new BlockMeshVertex
				{
					Color = Color.White,
					TextureCoordinates = new Vector2(0, 0),
					Position = new Vector3(0, 1.01f, 0)
				}
			};
			indices = new ushort[] { 2, 1, 0, 0, 1, 2, 0, 3, 2, 2, 3, 0 };
			mesh.Vertices.AddRange(vertices);
			mesh.Indices.AddRange(indices);
			mesh.TransformTextureCoordinates(Matrix.CreateTranslation(new Vector3(237 % 16 / 16f, 237 / 16 / 16f, 0f)));

			for (int i = 6; i < 10; i++)
			{
				m_blockMeshes[i] = new BlockMesh();
				m_blockMeshes[i].AppendBlockMesh(mesh);
				m_blockMeshes[i].TransformPositions(reverseCenter * Matrix.CreateRotationY(MathUtils.PI * 0.5f * i) * center);
			}

			var boxes = new []
			{
				new BoundingBox(0, 0, 0, 1, 0.01f, 1)
			};
			for (int i = 0; i < 6; i++)
				boundingBoxes[i] = boxes;
			boundingBoxes[6] = new BoundingBox[]
			{
				new BoundingBox(0, 0, 0, 1, 0.01f, 1),
				new BoundingBox(0, 0, 0.25f, 1, 0.25f, 1),
				new BoundingBox(0, 0, 0.5f, 1, 0.5f, 1),
				new BoundingBox(0, 0, 0.75f, 1, 0.75f, 1)
			};
			boundingBoxes[7] = new BoundingBox[]
			{
				new BoundingBox(0, 0, 0, 1, 0.01f, 1),
				new BoundingBox(0.25f, 0, 0, 1, 0.25f, 1),
				new BoundingBox(0.5f, 0, 0, 1, 0.5f, 1),
				new BoundingBox(0.75f, 0, 0, 1, 0.75f, 1)
			};
			boundingBoxes[8] = new BoundingBox[]
			{
				new BoundingBox(0, 0, 0, 1, 0.01f, 1),
				new BoundingBox(0, 0, 0, 1, 0.25f, 0.75f),
				new BoundingBox(0, 0, 0, 1, 0.5f, 0.5f),
				new BoundingBox(0, 0, 0, 1, 0.75f, 0.25f)
			};
			boundingBoxes[9] = new BoundingBox[]
			{
				new BoundingBox(0, 0, 0, 1, 0.01f, 1),
				new BoundingBox(0, 0, 0, 0.75f, 0.25f, 1),
				new BoundingBox(0, 0, 0, 0.5f, 0.5f, 1),
				new BoundingBox(0, 0, 0, 0.25f, 0.75f, 1)
			};
		}

		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
		{
			return boundingBoxes[GetRailType(Terrain.ExtractData(value))];
		}

		/*static int GetTextureSlot(int value)
		{
			return IsCorner(GetRailType(Terrain.ExtractData(value))) ? 238 : 237;
		}*/

		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawFlatBlock(primitivesRenderer, value, size, ref matrix, null, color, false, environmentData);
		}

		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometrySubsets geometry, int value, int x, int y, int z)
		{
			generator.GenerateMeshVertices(this, x, y, z, m_blockMeshes[GetRailType(Terrain.ExtractData(value))], Color.White, null, geometry.SubsetAlphaTest);
		}

		public static int GetRailType(int data)
		{
			return data & 15;
		}

		public static int SetRailType(int data, int type)
		{
			return (data & -16) | (type & 15);
		}

		public static bool IsCorner(int type)
		{
			return (type >> 2) == 0;
		}

		public static bool IsDirectionX(int type)
		{
			return (type & 1) == 0;
		}

		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			Vector3 forward = componentMiner.ComponentCreature.ComponentCreatureModel.EyeRotation.ToForwardVector();
			return new BlockPlacementData
			{
				CellFace = raycastResult.CellFace,
				Value = Terrain.MakeBlockValue(Index, 0, SetRailType(0, MathUtils.Abs(forward.X) < MathUtils.Abs(forward.Z) ? 4 : 5))
			};
		}

		public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
		{
			showDebris = true;
			dropValues.Add(new BlockDropValue
			{
				Value = Index,
				Count = 1
			});
		}

		public static bool CanConnectTo(int railType, int direction)
		{
			if (IsCorner(railType))
				return direction == railType || direction == ((railType + 1) & 3);
			return IsDirectionX(railType) ^ !IsDirectionX(direction);
		}
	}
}