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
			Matrix matrix = m_pointerMatrix = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Pointer").ParentBone);
			m_invPointerMatrix = Matrix.Invert(m_pointerMatrix);
			m_caseMesh.AppendModelMeshPart(model.FindMesh("Case").MeshParts[0], boneAbsoluteTransform, makeEmissive: false, flipWindingOrder: false, doubleSided: true, flipNormals: false, Color.Gray);
			m_pointerMesh.AppendModelMeshPart(model.FindMesh("Pointer").MeshParts[0], matrix, makeEmissive: false, flipWindingOrder: false, doubleSided: false, flipNormals: false, Color.White);
			for (int i = 0; i < 4; i++)
			{
				m_matricesByData[i] = Matrix.CreateScale(5f) * Matrix.CreateTranslation(0.95f, 0.15f, 0.5f) * Matrix.CreateTranslation(-0.5f, 0f, -0.5f) * Matrix.CreateRotationY((float)(i + 1) * (float)Math.PI / 2f) * Matrix.CreateTranslation(0.5f, 0f, 0.5f);
				m_collisionBoxesByData[i] = new BoundingBox[1]
				{
				m_caseMesh.CalculateBoundingBox(m_matricesByData[i])
				};
			}
			base.Initialize();
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
				Matrix value2 = m_invPointerMatrix * Matrix.CreateRotationX(radians) * m_pointerMatrix * matrix;
				generator.GenerateMeshVertices(this, x, y, z, m_caseMesh, Color.White, matrix, geometry.SubsetOpaque);
				generator.GenerateMeshVertices(this, x, y, z, m_pointerMesh, Color.White, value2, geometry.SubsetOpaque);
				generator.GenerateWireVertices(value, x, y, z, num & 3, 0.25f, Vector2.Zero, geometry.SubsetOpaque);
			}
		}
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			int value2 = 0;
			if (raycastResult.CellFace.Face == 0)
			{
				value2 = Terrain.ReplaceData(Terrain.ReplaceContents(0, Index), 0);
			}
			if (raycastResult.CellFace.Face == 1)
			{
				value2 = Terrain.ReplaceData(Terrain.ReplaceContents(0, Index), 1);
			}
			if (raycastResult.CellFace.Face == 2)
			{
				value2 = Terrain.ReplaceData(Terrain.ReplaceContents(0, Index), 2);
			}
			if (raycastResult.CellFace.Face == 3)
			{
				value2 = Terrain.ReplaceData(Terrain.ReplaceContents(0, Index), 3);
			}
			BlockPlacementData result = default(BlockPlacementData);
			result.Value = value2;
			result.CellFace = raycastResult.CellFace;
			return result;
		}
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			float num = 8f;
			if (environmentData != null && environmentData.SubsystemTerrain != null)
			{
				Vector3 translation = environmentData.InWorldMatrix.Translation;
				int num2 = Terrain.ToCell(translation.X);
				int num3 = Terrain.ToCell(translation.Z);
				int num4 = Terrain.ToCell(translation.Y);
				float f = translation.X - (float)num2;
				float f2 = translation.Z - (float)num3;
				float level = Utils.SubsystemSour.FindNearestCompassTarget(new Vector3(num2,num4,num3));
				num = MathUtils.Clamp(level,0f,100f);
			}
			float radians = -MathUtils.Lerp(1.5f, -1.5f, num / 100f);
			Matrix matrix2 = Matrix.CreateScale(7f * size) * Matrix.CreateTranslation(0f, -0.1f, 0f) * matrix;
			Matrix matrix3 = m_invPointerMatrix * Matrix.CreateRotationX(radians) * m_pointerMatrix * matrix2;
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_caseMesh, color, 1f, ref matrix2, environmentData);
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_pointerMesh, color, 1f, ref matrix3, environmentData);
		}
	}

}