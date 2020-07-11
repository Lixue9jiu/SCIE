using Engine;
using Engine.Graphics;
using System;
using System.Collections.Generic;

namespace Game
{
	public class Generator : CubeDevice, IBlockBehavior, IInteractiveBlock, IUnstableBlock
	{
		public Generator(int voltage = 310) : base("直流发电机", "发电机是交流发电机产生之前的电力基础，它允许你把机械能转换成电能，这样你就可以使用很多电器了。", voltage)
		{
			Type = ElementType.Supply | ElementType.Connector;
		}
		public override void Simulate(ref int voltage)
		{
			if (voltage > 8191)
				return;
			if (Powered)
				voltage += Voltage;
		}
		public override int GetFaceTextureSlot(int face, int value)
		{
			return face != 4 && face != 5 && face == (Terrain.ExtractData(value) >> 15) ? 145 : 107;
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
	}

	//public class TGenerator : InventoryEntityDevice<ComponentTGenerator>
	//{
	//	public TGenerator() : base("热能发电机", "热能发电机是一种利用金属温差发电的装置，它可以把岩浆转换为能量") { Type = ElementType.Supply | ElementType.Connector; Voltage = 310; }
	//
	//	public override void Simulate(ref int voltage)
	//	{
	//		if (voltage > 8023)
	//		{
	//			return;
	//		}
	//		if (Component.Powered)
	//			voltage += Voltage;
	//	}
	//
	//	public override int GetFaceTextureSlot(int face, int value)
	//	{
	//		return face != 4 && face != 5 && face == (Terrain.ExtractData(value) >> 15) ? 125 : 107;
	//	}
	//	public override Widget GetWidget(IInventory inventory, ComponentTGenerator component)
	//	{
	//		return new SeparatorWidget(inventory, component, "ThermalGenerator");
	//	}
	//}

	public class MHDGenerator : Generator
	{
		public MHDGenerator() : base(1000)
		{
			DefaultDisplayName = "磁流体发电机";
		}

		public override void Simulate(ref int voltage)
		{
			if ((Utils.Random.Int() & 31) != 0)
				base.Simulate(ref voltage);
		}
	}
	public class FuelCell : TGenerator
	{
		public FuelCell()
		{
			DefaultDisplayName = DefaultDescription = "燃料电池";
		}

		public override int GetFaceTextureSlot(int face, int value)
		{
			return face != 4 && face != 5 ? face == (Terrain.ExtractData(value) >> 15) ? 240 : 241 : 147;
		}
		public override Widget GetWidget(IInventory inventory, ComponentTGenerator component)
		{
			return new SeparatorWidget(inventory, component, Utils.Get("燃料电池"));
		}
	}

	public class Battery : CubeDevice, IHarvestingItem, IInteractiveBlock, IEquatable<Battery>
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
		public override Device Create(Point3 p, int value)
		{
			RemainCount = (Terrain.ExtractData(value) >> 14 & 1 ^ 1) * 600;
			return this;
		}
		public override void GenerateTerrainVertices(Block block, BlockGeometryGenerator generator, TerrainGeometrySubsets geometry, int value, int x, int y, int z)
		{
			Utils.BlockGeometryGenerator.GenerateMeshVertices(block, x, y, z, m_standaloneBlockMesh, SubsystemPalette.GetColor(generator, PaintableItemBlock.GetColor(Terrain.ExtractData(value))), null, Utils.GTV(x, z, geometry).SubsetOpaque);
			WireDevice.GenerateWireVertices(generator, value, x, y, z, 4, 0f, Vector2.Zero, geometry.SubsetOpaque);
		}
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, Voltage >30 ? BlocksTexturesManager.DefaultBlocksTexture : ItemBlock.Texture, color * SubsystemPalette.GetColor(environmentData, PaintableItemBlock.GetColor(Terrain.ExtractData(value))), size, ref matrix, environmentData);
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
		public override void Simulate(ref int voltage)
		{
			int x, y, z;
			if (voltage == 0 && RemainCount > 0)
			{
				voltage = Voltage;
				RemainCount--;
				if (RemainCount <= 0)
				{
					x = Point.X;
					y = Point.Y;
					z = Point.Z;
					Utils.Terrain.SetCellValueFast(x, y, z, Utils.Terrain.GetCellValueFast(x, y, z) | 16384 << 14);
				}
				return;
			}
			if (voltage >= Voltage && RemainCount <= 600)
			{
				voltage -= Voltage;
				RemainCount += Factor;
				x = Point.X;
				y = Point.Y;
				z = Point.Z;
				Utils.Terrain.SetCellValueFast(x, y, z, Utils.Terrain.GetCellValueFast(x, y, z) & ~(16384 << 14));
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

	public class SolarPanel : CubeDevice
	{
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();
		public BoundingBox[] m_collisionBoxes;
		public SolarPanel(string name, int voltage = 50) : base(name, name, voltage)
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
			if (Utils.SubsystemWeather.m_subsystemSky.SkyLightValue < 13 || Utils.Terrain.GetCellLight(x, Point.Y + 1, z) < 13)
				return;
			PrecipitationShaftInfo info = Utils.SubsystemWeather.GetPrecipitationShaftInfo(x, z);
			voltage += info.Intensity > 0f && Point.Y >= info.YLimit - 1 ? 5 : Voltage;
		}
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, ItemBlock.Texture, color, size, ref matrix, environmentData);
		}
		public override void GenerateTerrainVertices(Block block, BlockGeometryGenerator generator, TerrainGeometrySubsets geometry, int value, int x, int y, int z)
		{
			Utils.BlockGeometryGenerator.GenerateMeshVertices(block, x, y, z, m_standaloneBlockMesh, Color.White, null, Utils.GTV(x, z, geometry).SubsetOpaque);
		}
		public override bool IsFaceTransparent(SubsystemTerrain subsystemTerrain, int face, int value) => face != 4;
		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value) => m_collisionBoxes;
		public override string GetCraftingId() => "SolarPanel" + Voltage.ToString();
	}

	public class Gearbox : ElectricMotor, IBlockBehavior, IUnstableBlock
	{
		public Gearbox()
		{
			DefaultDisplayName = DefaultDescription = "变速箱";
			Type = ElementType.Supply | ElementType.Rod;
		}

		public override void Simulate(ref int voltage)
		{
			if (Powered)
			{
				voltage += 400;
				return;
			}
			voltage = voltage * 81 / 100;
			Powered = voltage > 320;
		}

		public override int GetFaceTextureSlot(int face, int value) => 126;
		public void OnBlockAdded(SubsystemTerrain subsystemTerrain, int value, int oldValue)
		{
			Powered = ComponentEngine.IsPowered(subsystemTerrain.Terrain, Point.X, Point.Y, Point.Z, false);
		}
		public void OnBlockRemoved(SubsystemTerrain subsystemTerrain, int value, int newValue) { }
		public void OnNeighborBlockChanged(SubsystemTerrain subsystemTerrain, int neighborX, int neighborY, int neighborZ)
		{
			Powered = ComponentEngine.IsPowered(subsystemTerrain.Terrain, Point.X, Point.Y, Point.Z, false);
		}
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