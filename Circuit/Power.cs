using Engine;
using Engine.Graphics;
using System;
using System.Collections.Generic;

namespace Game
{
	public class Generator : FixedDevice, IBlockBehavior, IInteractiveBlock, IUnstableBlock
	{
		public Generator(int voltage = 310) : base("直流发电机", "发电机是交流发电机产生之前的电力基础，它允许你把机械能转换成电能，这样你就可以使用很多电器了。", voltage)
		{
			Type = ElementType.Supply | ElementType.Connector;
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
			return GetPlacementValue(1, componentMiner, value, raycastResult);
		}

		public override bool IsFaceTransparent(SubsystemTerrain subsystemTerrain, int face, int value) => false;
		public void OnBlockAdded(SubsystemTerrain subsystemTerrain, int value, int oldValue)
		{
			Powered = ComponentEngine.IsPowered(subsystemTerrain.Terrain, Point.X, Point.Y, Point.Z ,false);
		}
		public void OnBlockRemoved(SubsystemTerrain subsystemTerrain, int value, int newValue) { }
		public void OnNeighborBlockChanged(SubsystemTerrain subsystemTerrain, int neighborX, int neighborY, int neighborZ)
		{
			Powered = ComponentEngine.IsPowered(subsystemTerrain.Terrain, Point.X, Point.Y, Point.Z ,false);
		}
		public bool OnInteract(TerrainRaycastResult raycastResult, ComponentMiner componentMiner)
		{
			return componentMiner.Project.FindSubsystem<SubsystemSignBlockBehavior>(true).OnInteract(raycastResult, componentMiner);
		}
	}
	public class ACGenerator : Generator
	{
		public ACGenerator() : base(-220)
		{
			DefaultDisplayName = "交流发电机";
		}
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			return GetPlacementValue(28, componentMiner, value, raycastResult);
		}
	}
	public class Battery : FixedDevice, IBlockBehavior, IHarvestingItem, IInteractiveBlock, IEquatable<Battery>
	{
		public int Factor = 1;
		public readonly BoundingBox[] m_collisionBoxes;
		protected readonly BlockMesh m_standaloneBlockMesh = new BlockMesh();
		public int RemainCount = 600;
		readonly string Id;
		public Battery(Matrix tcTransform, string name = "", string description = "", string id = "", int voltage = 12, string modelName = "Models/Battery", string meshName = "Battery") : base(name, description, voltage)
		{
			Type = ElementType.Connector | ElementType.Container;
			m_standaloneBlockMesh.AppendMesh(modelName, meshName, Matrix.CreateTranslation(0f, -0.5f, 0f) * Matrix.CreateTranslation(new Vector3(0.5f)), tcTransform, Color.LightGray);
			m_collisionBoxes = new BoundingBox[] { m_standaloneBlockMesh.CalculateBoundingBox() };
			Id = id;
		}
		public override void GenerateTerrainVertices(Block block, BlockGeometryGenerator generator, TerrainGeometrySubsets geometry, int value, int x, int y, int z)
		{
			if ((Terrain.ExtractData(value) & 16384) != 0)
				RemainCount = 0;
			Utils.BlockGeometryGenerator.GenerateMeshVertices(block, x, y, z, m_standaloneBlockMesh, SubsystemPalette.GetColor(generator, PaintableItemBlock.GetColor(Terrain.ExtractData(value))), null, Utils.GTV(x, z, geometry).SubsetOpaque);
			WireDevice.GenerateWireVertices(generator, value, x, y, z, 4, 0f, Vector2.Zero, geometry.SubsetOpaque);
		}
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, ItemBlock.Texture, color * SubsystemPalette.GetColor(environmentData, PaintableItemBlock.GetColor(Terrain.ExtractData(value))), size, ref matrix, environmentData);
		}
		public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
		{
			showDebris = true;
			dropValues.Add(new BlockDropValue { Value = Terrain.ReplaceLight(oldValue, 0), Count = 1 });
		}
		public bool OnInteract(TerrainRaycastResult raycastResult, ComponentMiner componentMiner)
		{
			DialogsManager.ShowDialog(componentMiner.ComponentPlayer.View.GameWidget, new EditElectricDialog(RemainCount));
			return true;
		}
		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value) => m_collisionBoxes;
		public void OnBlockRemoved(SubsystemTerrain terrain, int value, int newValue)
		{
			Simulate(ref value);
		}
		public void OnBlockAdded(SubsystemTerrain terrain, int value, int newValue)
		{
			Simulate(ref value);
		}
		public override void Simulate(ref int voltage)
		{
			if (voltage == 0 && RemainCount > 0)
			{
				voltage = Voltage;
				RemainCount--;
				if (RemainCount <= 0)
				{
					int x = Point.X,
						y = Point.Y,
						z = Point.Z;
					Utils.Terrain.SetCellValueFast(x, y, z, Utils.Terrain.GetCellValueFast(x, y, z) | 16384 << 14);
				}
				return;
			}
			if (voltage >= Voltage && RemainCount<=600)
			{
				voltage -= Voltage;
				RemainCount += Factor;
			}
		}
		public bool Equals(Battery other) => base.Equals(other) && Factor == other.Factor;
		public override string GetCraftingId() => Id;
		public override bool IsFaceTransparent(SubsystemTerrain subsystemTerrain, int face, int value) => true;
		public void OnItemHarvested(int x, int y, int z, int blockValue, ref BlockDropValue dropValue, ref int newBlockValue)
		{
			if (RemainCount <= 0)
				dropValue.Value = Terrain.ReplaceLight(blockValue, 0) | 16384 << 14;
		}
	}
	public class MachRod : FixedDevice
	{
		public BlockMesh[] m_blockMeshesByData = new BlockMesh[6];
		public MachRod() : base("连接杆", "连接杆", 0)
		{
			Type = ElementType.Rod;
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
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_blockMeshesByData[0], color, size, ref matrix, environmentData);
		}
	}
	public class LED : FixedDevice
	{
		public BlockMesh[] m_blockMeshesByFace = new BlockMesh[6];
		public BoundingBox[][] m_collisionBoxesByFace = new BoundingBox[6][];
		public LED() : base("LED", "LED", 12)
		{
			ModelMesh modelMesh = ContentManager.Get<Model>("Models/Leds").FindMesh("OneLed");
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(modelMesh.ParentBone);
			for (int i = 0; i < 6; i++)
			{
				Matrix m = i >= 4 ? ((i != 4) ? (Matrix.CreateRotationX((float)Math.PI) * Matrix.CreateTranslation(0.5f, 1f, 0.5f)) : Matrix.CreateTranslation(0.5f, 0f, 0.5f)) : (Matrix.CreateRotationX((float)Math.PI / 2f) * Matrix.CreateTranslation(0f, 0f, -0.5f) * Matrix.CreateRotationY(i * (float)Math.PI / 2f) * Matrix.CreateTranslation(0.5f, 0.5f, 0.5f));
				m_blockMeshesByFace[i] = new BlockMesh();
				m_blockMeshesByFace[i].AppendModelMeshPart(modelMesh.MeshParts[0], boneAbsoluteTransform * m, false, false, false, false, Color.White);
				m_collisionBoxesByFace[i] = new[] { m_blockMeshesByFace[i].CalculateBoundingBox() };
			}
		}
		public override int GetEmittedLightAmount()
		{
			return Powered ? 15 : 0;
		}
		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value) => m_collisionBoxesByFace[Terrain.ExtractData(value) >> 14];
	}
	public class SolarPanel : FixedDevice
	{
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();
		public BoundingBox[] m_collisionBoxes;
		public SolarPanel(string name, int voltage = 100) : base(name, name, voltage)
		{
			Type = ElementType.Supply | ElementType.Connector;
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
			PrecipitationShaftInfo info = Utils.SubsystemWeather.GetPrecipitationShaftInfo(x, z);
			voltage += info.Intensity > 0f && Point.Y >= info.YLimit - 1 ? 20 : Voltage;
		}
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, ItemBlock.Texture, color, size, ref matrix, environmentData);
		}
		public override void GenerateTerrainVertices(Block block, BlockGeometryGenerator generator, TerrainGeometrySubsets geometry, int value, int x, int y, int z)
		{
			Utils.BlockGeometryGenerator.GenerateMeshVertices(block, x, y, z, m_standaloneBlockMesh, Color.White, null, Utils.GTV(x, z, geometry).SubsetOpaque);
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
	public abstract class Diode : Device
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
	}*/
}