using Engine;
using Engine.Graphics;
using System;

namespace Game
{
	public class Switch : CubeDevice, IInteractiveBlock
	{
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();
		public BlockMesh m_standaloneBlockMesh2 = new BlockMesh();
		public BoundingBox[] m_collisionBoxes;

		public Switch() : base("电闸", "电闸控制工业电路的通断")
		{
			Model model = ContentManager.Get<Model>("Models/Switch");
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Body").ParentBone) * Matrix.CreateTranslation(.5f, 0, .5f);
			Matrix boneAbsoluteTransform2 = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Lever").ParentBone);
			ModelMeshPart meshPart = model.FindMesh("Body").MeshParts[0];
			m_standaloneBlockMesh.AppendModelMeshPart(meshPart, boneAbsoluteTransform, false, false, false, false, Color.White);
			m_standaloneBlockMesh2.AppendModelMeshPart(meshPart, boneAbsoluteTransform, false, false, false, false, Color.White);
			meshPart = model.FindMesh("Lever").MeshParts[0];
			m_standaloneBlockMesh.AppendModelMeshPart(meshPart, boneAbsoluteTransform2 * Matrix.CreateRotationX(MathUtils.DegToRad(30f)) * Matrix.CreateTranslation(.5f, 0f, .5f), false, false, false, false, Color.White);
			m_standaloneBlockMesh2.AppendModelMeshPart(meshPart, boneAbsoluteTransform2 * Matrix.CreateRotationX(MathUtils.DegToRad(-30f)) * Matrix.CreateTranslation(.5f, 0f, .5f), false, false, false, false, Color.White);
			m_collisionBoxes = new[] { m_standaloneBlockMesh.CalculateBoundingBox() };
		}

		public override Device Create(Point3 p, int value)
		{
			Powered = (Terrain.ExtractData(value) & 16384) != 0;
			return this;
		}

		public override void Simulate(ref int voltage)
		{
			if (!Powered) voltage = 0;
		}

		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, color * Color.LightGray, size * 2f, ref matrix, environmentData);
		}

		public override void GenerateTerrainVertices(Block block, BlockGeometryGenerator generator, TerrainGeometrySubsets geometry, int value, int x, int y, int z)
		{
			generator.GenerateMeshVertices(block, x, y, z, (Terrain.ExtractData(value) & 16384) != 0 ? m_standaloneBlockMesh : m_standaloneBlockMesh2, Color.LightGray, null, geometry.SubsetOpaque);
			WireDevice.GenerateWireVertices(generator, value, x, y, z, 4, 0f, Vector2.Zero, geometry.SubsetOpaque);
		}

		public override bool IsFaceTransparent(SubsystemTerrain subsystemTerrain, int face, int value) => true;

		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value) => m_collisionBoxes;

		public bool OnInteract(TerrainRaycastResult raycastResult, ComponentMiner componentMiner)
		{
			int x = Point.X,
				y = Point.Y,
				z = Point.Z;
			Utils.SubsystemTerrain.ChangeCell(x, y, z, Utils.Terrain.GetCellValueFast(x, y, z) ^ 16384 << 14);
			return true;
		}

		public override Vector3 GetIconBlockOffset(int value, DrawBlockEnvironmentData environmentData) => new Vector3 { Y = .5f };
	}
	public class Relay : CubeDevice, IElectricElementBlock
	{
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();
		public BoundingBox[] m_collisionBoxes;

		public Relay() : base("继电器", "继电器允许你用逻辑电路控制工业电路")
		{
			m_standaloneBlockMesh.AppendMesh("Models/Switch", "Body", Matrix.CreateTranslation(.5f, 0, .5f), Matrix.Identity, Color.White);
			m_collisionBoxes = new[] { m_standaloneBlockMesh.CalculateBoundingBox() };
		}

		public override void Simulate(ref int voltage)
		{
			if (!Powered) voltage = 0;
		}

		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, color, size * 2f, ref matrix, environmentData);
		}
		public override void GenerateTerrainVertices(Block block, BlockGeometryGenerator generator, TerrainGeometrySubsets geometry, int value, int x, int y, int z)
		{
			WireDevice.GenerateWireVertices(generator, value, x, y, z, 4, 0f, Vector2.Zero, geometry.SubsetOpaque);
			generator.GenerateMeshVertices(Block, x, y, z, m_standaloneBlockMesh, SubsystemPalette.GetColor(generator, PaintableItemBlock.GetColor(Terrain.ExtractData(value))), null, geometry.SubsetOpaque);
			generator.GenerateWireVertices(value, x, y, z, 4, 0.25f, Vector2.Zero, geometry.SubsetOpaque);
		}

		public override bool IsFaceTransparent(SubsystemTerrain subsystemTerrain, int face, int value) => true;
		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value) => m_collisionBoxes;
		public override Vector3 GetIconBlockOffset(int value, DrawBlockEnvironmentData environmentData) => new Vector3 { Y = 0.45f };
		public ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			return new RelayElectricElement(subsystemElectricity, new Point3(x, y, z));
		}
		public ElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z)
		{
			return face == 4 || face == 5 ? (ElectricConnectorType?)ElectricConnectorType.Input : null;
		}
		public int GetConnectionMask(int value)
		{
			int? color = PaintableItemBlock.GetColor(Terrain.ExtractData(value));
			return color.HasValue ? 1 << color.Value : 2147483647;
		}
	}

	public class Transformer : CubeDevice
	{
		public Transformer() : base("变压器", "变压器是一种把交流电转化成直流电的机器") { }
		public override void Simulate(ref int voltage)
		{
			if (voltage < 0)
				voltage = (int)(-voltage * Math.Sqrt(2));
		}
		public override int GetFaceTextureSlot(int face, int value)
		{
			return face == 4 || face == 5 ? 107 : 121;
		}
	}
	public class Inverter : CubeDevice
	{
		public Inverter() : base("逆变器", "逆变器是一种把直流电转化成交流电的机器") { }
		public override void Simulate(ref int voltage)
		{
			if (voltage > 0)
				voltage = (int)(-voltage / Math.Sqrt(2));
		}
		public override int GetFaceTextureSlot(int face, int value)
		{
			return face == 4 || face == 5 ? 239 : 121;
		}
	}
	public class Fuseblock : CubeDevice
	{
		public Fuseblock() : base("熔丝盒", "熔丝盒", 0) { }

		public override void Simulate(ref int voltage)
		{
			if (voltage > 700)
				voltage = 0;
		}

		public override int GetFaceTextureSlot(int face, int value)
		{
			return 236;
		}
	}
	public class Stabilizer : CubeDevice
	{
		public int RemainsCount;
		public Stabilizer() : base("稳压器", "稳压器", 220) { }

		public override void Simulate(ref int voltage)
		{
			if (voltage > Voltage)
			{
				RemainsCount += voltage - Voltage;
				voltage = Voltage;
			}
			else if (RemainsCount > 0)
			{
				RemainsCount -= Voltage - voltage;
				voltage = Voltage;
			}
		}

		public override int GetFaceTextureSlot(int face, int value)
		{
			return face != 4 && face != 5 ? 129 : 107;
		}
	}
	/*public class Resistor : CubeDevice
	{
		public FixedResistor(string name) : base(name, name)
		{
		}
	}
	public abstract class IC : Element, IEquatable<IC>
	{
		protected IC(ElementType type) : base(type)
		{
		}
		public override bool Equals(object obj)
		{
			var node = obj as IC;
			return node != null ? Equals(node) : base.Equals(node);
		}
		public bool Equals(IC other)
		{
			return base.Equals(other);
		}
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}*/
}
