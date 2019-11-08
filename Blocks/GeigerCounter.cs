using Engine;
using Engine.Graphics;
using Game;
using System;
namespace Game
{
	public class GeigerCounterBlock : Block
	{
		public const int Index = 540;

		public BlockMesh m_caseMesh = new BlockMesh();

		public BlockMesh m_pointerMesh = new BlockMesh();

		public Matrix m_pointerMatrix;

		public Matrix m_invPointerMatrix;

		public Matrix[] m_matricesByData = new Matrix[4];

		public BoundingBox[][] m_collisionBoxesByData = new BoundingBox[4][];

		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/Hygrometer");
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Case").ParentBone);
			m_pointerMatrix = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Pointer").ParentBone);
			m_invPointerMatrix = Matrix.Invert(m_pointerMatrix);
			m_caseMesh.AppendModelMeshPart(model.FindMesh("Case").MeshParts[0], boneAbsoluteTransform, false, false, true,  false, Color.Gray);
			m_pointerMesh.AppendModelMeshPart(model.FindMesh("Pointer").MeshParts[0], m_pointerMatrix, false, false, false, false, Color.White);
			for (int i = 0; i < 4; i++)
			{
				m_matricesByData[i] = Matrix.CreateScale(5f) * Matrix.CreateTranslation(0.95f, 0.15f, 0.5f) * Matrix.CreateTranslation(-0.5f, 0f, -0.5f) * Matrix.CreateRotationY((i + 1) * (float)Math.PI / 2f) * Matrix.CreateTranslation(0.5f, 0f, 0.5f);
				m_collisionBoxesByData[i] = new[]
				{
					m_caseMesh.CalculateBoundingBox(m_matricesByData[i])
				};
			}
			//base.Initialize();
		}

		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometrySubsets geometry, int value, int x, int y, int z)
		{
			int num = Terrain.ExtractData(value);
			if (num < m_matricesByData.Length)
			{
				float level = Utils.SubsystemSour.FindNearestCompassTarget(new Vector3(x, y, z));
				float num2 = MathUtils.Clamp(level, 0f, 100f);
				float radians = -MathUtils.Lerp(1.5f, -1.5f, num2 / 100f);
				Matrix matrix = m_matricesByData[num];
				generator.GenerateMeshVertices(this, x, y, z, m_caseMesh, Color.White, matrix, geometry.SubsetOpaque);
				generator.GenerateMeshVertices(this, x, y, z, m_pointerMesh, Color.White, m_invPointerMatrix * Matrix.CreateRotationX(radians) * m_pointerMatrix * matrix, geometry.SubsetOpaque);
				generator.GenerateWireVertices(value, x, y, z, num & 3, 0.25f, Vector2.Zero, geometry.SubsetOpaque);
			}
		}
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			int face = raycastResult.CellFace.Face;
			return new BlockPlacementData { Value = face <= 3 ? Terrain.ReplaceData(Index, face) : 0, CellFace = raycastResult.CellFace };
		}
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			float num = 8f;
			if (environmentData != null && environmentData.SubsystemTerrain != null)
			{
				Vector3 translation = environmentData.InWorldMatrix.Translation;
				//float f = translation.X - num2;
				//float f2 = translation.Z - num3;
				num = MathUtils.Clamp(Utils.SubsystemSour.FindNearestCompassTarget(new Vector3(Terrain.ToCell(translation))), 0f, 100f);
			}
			Matrix mat2 = Matrix.CreateScale(2f * size) * Matrix.CreateTranslation(0f, -0.1f, 0f) * matrix;
			Matrix mat3 = m_invPointerMatrix * Matrix.CreateRotationX(-MathUtils.Lerp(1.5f, -1.5f, num / 100f)) * m_pointerMatrix * mat2;
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_caseMesh, color, size, ref mat2, environmentData);
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_pointerMesh, color, size, ref mat3, environmentData);
		}
	}
}