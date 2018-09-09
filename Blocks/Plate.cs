using Engine;
using Engine.Graphics;

namespace Game
{
	public class Sheet : Plate
	{
		public Sheet(MetalType type) : base(type)
		{
			DefaultDisplayName = type.ToString() + "Sheet";
			DefaultDescription = "A sheet of pure " + type.ToString() + ". Can be crafted into very durable and strong " + type.ToString() + " items. Very important in the industrial Era.";
		}
		public override float GetIconViewScale(int value, DrawBlockEnvironmentData environmentData)
		{
			return 0.5f;
		}
	}
	public class Plate : MeshItem
	{
		protected BoundingBox[] m_collisionBoxes;
		public readonly MetalType Type;
		public Plate(MetalType type) : base("A plate of pure " + type.ToString() + ". Can be crafted into very durable and strong " + type.ToString() + " items. Very important in the industrial Era.")
		{
			Type = type;
			DefaultDisplayName = type.ToString() + "Plate";
			Model model = ContentManager.Get<Model>("Models/Ingots");
			m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("IronPlate", true).MeshParts[0], BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("IronPlate", true).ParentBone) * Matrix.CreateTranslation(0.5f, 0f, 0.5f), false, false, false, false, Color.White);
			m_collisionBoxes = new BoundingBox[]
			{
				m_standaloneBlockMesh.CalculateBoundingBox()
			};
		}
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, MetalBlock.GetColor(Type), size * 1.5f, ref matrix, environmentData);
		}

		public override void GenerateTerrainVertices(Block block,BlockGeometryGenerator generator, TerrainGeometrySubsets geometry, int value, int x, int y, int z)
		{
			generator.GenerateMeshVertices(block, x, y, z, m_standaloneBlockMesh, MetalBlock.GetColor(Type), null, geometry.SubsetOpaque);
		}
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			return new BlockPlacementData
			{
				Value = value,
				CellFace = raycastResult.CellFace
			};
		}
		public override Vector3 GetIconBlockOffset(int value, DrawBlockEnvironmentData environmentData)
		{
			return new Vector3
			{
				Y = 0.45f
			};
		}
		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
		{
			return m_collisionBoxes;
		}
	}
}
