using Engine;
using Engine.Graphics;

namespace Game
{
	public class Mould : MeshItem
	{
		public readonly float Size;
		public readonly BoundingBox[] m_collisionBoxes;

		public Mould(string modelName, string meshName, Matrix boneTransform, Matrix tcTransform, Color color, float size = 1f) : base("")
		{
			Size = size;
			var model = ContentManager.Get<Model>(modelName);
			var blockMesh = new BlockMesh();
			blockMesh.AppendModelMeshPart(model.FindMesh(meshName, true).MeshParts[0], BlockMesh.GetBoneAbsoluteTransform(model.FindMesh(meshName, true).ParentBone) * boneTransform, false, false, false, false, color);
			blockMesh.TransformTextureCoordinates(tcTransform * Matrix.CreateScale(0.05f));
			m_standaloneBlockMesh.AppendBlockMesh(blockMesh);
			m_collisionBoxes = new[]
			{
				blockMesh.CalculateBoundingBox()
			};
		}
		public Mould(string meshName, Matrix boneTransform, Matrix tcTransform, string description = "", float size = 1f) : this("Models/" + meshName, meshName, boneTransform, tcTransform, Color.LightGray, size)
		{
			DefaultDisplayName = "Steel" + meshName;
			DefaultDescription = description;
		}
		public Mould(string modelName, string meshName, Matrix boneTransform, Matrix tcTransform, string description = "", string name = "", float size = 1f) : this(modelName, meshName, boneTransform, tcTransform, Color.LightGray, size)
		{
			DefaultDisplayName = name;
			DefaultDescription = description;
		}
		public Mould(string modelName, string meshName, Matrix boneTransform, Matrix tcTransform, Color color, string description = "", string name = "", float size = 1f) : this(modelName, meshName, boneTransform, tcTransform, color, size)
		{
		}

		public override void GenerateTerrainVertices(Block block, BlockGeometryGenerator generator, TerrainGeometrySubsets geometry, int value, int x, int y, int z)
		{
			generator.GenerateMeshVertices(block, x, y, z, m_standaloneBlockMesh, Color.White, null, geometry.SubsetOpaque);
		}

		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, color, size * Size, ref matrix, environmentData);
		}

		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			return new BlockPlacementData
			{
				Value = value,
				CellFace = raycastResult.CellFace
			};
		}

		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
		{
			return m_collisionBoxes;
		}
	}
}