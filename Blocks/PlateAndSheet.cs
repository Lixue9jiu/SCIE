using Engine;
using Engine.Graphics;

namespace Game
{
	public class Sheet : Plate
	{
		public Sheet(Materials type) : base(type)
		{
			string name = type.ToString();
			DefaultDisplayName = name + "Sheet";
			DefaultDescription = "A sheet of pure " + name + ". Can be crafted into very durable and strong " + name + " items.";
		}

		public Sheet(Color color) : base(color)
		{
		}

		public override float GetIconViewScale(int value, DrawBlockEnvironmentData environmentData)
		{
			return 0.5f;
		}
	}
	public class Plate : MeshItem
	{
		protected BoundingBox[] m_collisionBoxes;
		public readonly Color Color;
		public Plate(Color color)
		{
			Color = color;
			var model = ContentManager.Get<Model>("Models/Ingots");
			m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("IronPlate", true).MeshParts[0], BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("IronPlate", true).ParentBone) * Matrix.CreateTranslation(0.5f, 0f, 0.5f), false, false, false, false, Color.White);
			m_collisionBoxes = new BoundingBox[] { m_standaloneBlockMesh.CalculateBoundingBox() };
		}

		public Plate(Materials type) : this(MetalBlock.GetColor(type))
		{
			string name = type.ToString();
			DefaultDisplayName = name + "Plate";
			DefaultDescription = "A plate of pure " + name + ". Can be crafted into very durable and strong " + name + " items. Very important in the industrial era.";
		}

		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, Color, size * 1.5f, ref matrix, environmentData);
		}

		public override void GenerateTerrainVertices(Block block,BlockGeometryGenerator generator, TerrainGeometrySubsets geometry, int value, int x, int y, int z)
		{
			generator.GenerateMeshVertices(block, x, y, z, m_standaloneBlockMesh, Color, null, geometry.SubsetOpaque);
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