using Engine;
using Engine.Graphics;
using System;

namespace Game
{//ElementType.Container | ElementType.Connector |
	public class Generator : DeviceBlock, IBlockBehavior, IInteractiveBlock, IUnstableBlock
	{
		public bool Powered;
		public Generator(int voltage = 310) : base(voltage, "直流发电机", "发电机是交流发电机产生之前的电力基础，它允许你把机械能转换成电能，这样你就可以使用很多电器了。", ElementType.Supply | ElementType.Connector)
		{
		}
		public override void Simulate(ref int voltage)
		{
			if (Powered)
				voltage += Voltage;
		}
		public override int GetFaceTextureSlot(int face, int value)
		{
			return face != 4 && face != 5 && face == (Terrain.ExtractData(value) >> 15) ? 145 : 107;
		}
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			return FixedDevice.GetPlacementValue(1, componentMiner, value, raycastResult);
		}

		//public static int GetDirection(int data) => data & 7;
		//public static int SetDirection(int data, int direction) => (data & -8) | (direction & 7);
		public override bool IsFaceTransparent(SubsystemTerrain subsystemTerrain, int face, int value) => false;
		public void OnBlockAdded(SubsystemTerrain subsystemTerrain, int value, int oldValue)
		{
			Powered = ComponentEngine.IsPowered(subsystemTerrain.Terrain, Point.X, Point.Y, Point.Z);
		}
		public void OnBlockRemoved(SubsystemTerrain subsystemTerrain, int value, int newValue) { }
		public void OnNeighborBlockChanged(SubsystemTerrain subsystemTerrain, int neighborX, int neighborY, int neighborZ)
		{
			Powered = ComponentEngine.IsPowered(subsystemTerrain.Terrain, Point.X, Point.Y, Point.Z);
		}
		public bool OnInteract(TerrainRaycastResult raycastResult, ComponentMiner componentMiner)
		{
			return componentMiner.Project.FindSubsystem<SubsystemSignBlockBehavior>(true).OnInteract(raycastResult, componentMiner);
		}
	}
	public class MachRod : DeviceBlock
	{
		public BlockMesh[] m_blockMeshesByData = new BlockMesh[48];
		public MachRod() : base(0, "", "", ElementType.Rod)
		{
			Model model = ContentManager.Get<Model>("Models/Pistons");
			const string name = "PistonShaft";
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh(name).ParentBone);
			for (int j = 0; j < 6; j++)
			{
				int num = j;
				Matrix m = j < 4 ? (Matrix.CreateTranslation(0f, -0.5f, 0f) * Matrix.CreateRotationY(j * (float)Math.PI / 2f + (float)Math.PI) * Matrix.CreateTranslation(0.5f, 0.5f, 0.5f)) : ((j != 4) ? (Matrix.CreateTranslation(0f, -0.5f, 0f) * Matrix.CreateRotationX(-(float)Math.PI / 2f) * Matrix.CreateTranslation(0.5f, 0.5f, 0.5f)) : (Matrix.CreateTranslation(0f, -0.5f, 0f) * Matrix.CreateRotationX((float)Math.PI / 2f) * Matrix.CreateTranslation(0.5f, 0.5f, 0.5f)));
				m_blockMeshesByData[num] = new BlockMesh();
				m_blockMeshesByData[num].AppendModelMeshPart(model.FindMesh(name).MeshParts[0], boneAbsoluteTransform * m, false, false, false, false, Color.White);
			}
		}
	}
	public class SolarPanel : DeviceBlock
	{
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();
		public BoundingBox[] m_collisionBoxes;
		public SolarPanel(string name, int voltage = 100) : base(voltage, name, name, ElementType.Supply | ElementType.Connector)
		{
			Model model = ContentManager.Get<Model>("Models/CellTrapdoor");
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Trapdoor").ParentBone);
			m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Trapdoor").MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0.5f, 0, 0.5f), false, false, false, false, Color.White);
			m_collisionBoxes = new[] { m_standaloneBlockMesh.CalculateBoundingBox() };
		}

		public override void Simulate(ref int voltage)
		{
			int x = Point.X, z = Point.Z;
			if (Utils.SubsystemWeather.m_subsystemSky.SkyLightValue < 15 || Utils.Terrain.GetCellLight(x, Point.Y + 1, z) < 15)
				return;
			PrecipitationShaftInfo precipitationShaftInfo = Utils.SubsystemWeather.GetPrecipitationShaftInfo(x, z);
			voltage += precipitationShaftInfo.Intensity > 0f && Point.Y >= precipitationShaftInfo.YLimit - 1 ? 20 : Voltage;
		}
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, ItemBlock.Texture, color, size, ref matrix, environmentData);
		}
		public override void GenerateTerrainVertices(Block block, BlockGeometryGenerator generator, TerrainGeometrySubsets geometry, int value, int x, int y, int z)
		{
			generator.GenerateMeshVertices(block, x, y, z, m_standaloneBlockMesh, Color.White, null, geometry.SubsetOpaque);
		}
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			return new BlockPlacementData { Value = value, CellFace = raycastResult.CellFace };
		}
		public override bool IsFaceTransparent(SubsystemTerrain subsystemTerrain, int face, int value) => true;
		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value) => m_collisionBoxes;
		public override string GetCraftingId() => DefaultDisplayName + Voltage.ToString();
	}
	/*public class Voltmeter : DeviceBlock
	{
		public Voltmeter() : base(0, "电压表", "电压表")
		{
			Model model = ContentManager.Get<Model>("Models/Hygrometer");
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Case").ParentBone);
			Matrix matrix = m_pointerMatrix = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Pointer").ParentBone);
			m_invPointerMatrix = Matrix.Invert(m_pointerMatrix);
			m_caseMesh.AppendModelMeshPart(model.FindMesh("Case").MeshParts[0], boneAbsoluteTransform, false, false, true, false, Color.White);
			m_pointerMesh.AppendModelMeshPart(model.FindMesh("Pointer").MeshParts[0], matrix, false, false, false, false, Color.White);
		}
	}
	public abstract class Diode : Element
	{
		public int MaxVoltage;
		protected Diode() : base(ElementType.Connector)
		{
		}
		public override void Simulate(ref int voltage)
		{
			if (voltage < 0)
			{
				if (voltage > -MaxVoltage)
					voltage = 0;
				else MaxVoltage = 0;
			}
		}
	}
	public class DiodeDevice : Diode
	{
	}*/
}