using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Engine;
using Engine.Graphics;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	[Flags]
	public enum ElementType
	{
		None = 0,
		Supply = 1, // Power / Engine
		Device = 2, // Resistor
		Container = Supply | Device, // Battery
		Connector0 = 4,
		Connector1 = 8,
		Connector2 = 16,
		Connector3 = 32,
		Connector4 = 64,
		Connector5 = 128,
		Connector6 = 256,
		Connector7 = 512,
		Connector8 = 1024,
		Connector9 = 2048,
		ConnectorA = 4096,
		ConnectorB = 8192,
		ConnectorC = 16384,
		ConnectorD = 32768,
		ConnectorE = 65536,
		ConnectorF = 131072,
		Connector = 262140,
		//Capacitor = 262144,
		//Inductor = 524288,
		PipeX = 1 << 20,
		Pipex = 1 << 21,
		PipeY = 1 << 22,
		Pipey = 1 << 23,
		PipeZ = 1 << 24,
		Pipez = 1 << 25,
		Pipe = PipeX | Pipex | PipeY | Pipey | PipeZ | Pipez,
		RodX = 1 << 26,
		RodY = 1 << 27,
		RodZ = 1 << 28,
		Rod = RodX | RodY | RodZ
	}
	//[Serializable]
	public abstract class Element : Item, IEquatable<Element>, INode, ICloneable
	{
		public ElementType Type;
		//public ElectricConnectorDirection Direction;
		public Element[] Next;
		protected Element(ElementType type = ElementType.Device | ElementType.Connector)
		{
			Type = type;
		}
		public virtual void Simulate(ref int value)
		{
		}
		#region implements & overloads
		public object Clone()
		{
			return MemberwiseClone();
		}
		public override bool Equals(object obj)
		{
			return Equals(obj as Element);
		}
		public override int GetHashCode()
		{
			return (int)Type;
		}
		[MethodImpl((MethodImplOptions)0x100)]
		public bool Equals(Element other)
		{
			return other != null && other.Type == Type && Next == null ? other.Next == null : GetCraftingId().Equals(other.GetCraftingId());
		}
		//public override string ToString() => Type.ToString();
		#endregion
	}
	//[Serializable]
	public abstract class Device : Element, IEquatable<Device>
	{
		public Point3 Point;
		protected Device(ElementType type = ElementType.Device | ElementType.Connector) : base(type) { }
		public virtual Device Create(Point3 p, int value)
		{
			return this;
		}
		public override void GenerateTerrainVertices(Block block, BlockGeometryGenerator generator, TerrainGeometrySubsets geometry, int value, int x, int y, int z)
		{
			Utils.BlockGeometryGenerator.GenerateCubeVertices(Block, value, x, y, z, Color.LightGray * SubsystemPalette.GetColor(generator, PaintableItemBlock.GetColor(Terrain.ExtractData(value))), Utils.GTV(x, z, geometry).OpaqueSubsetsByFace);
		}
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			color = Color.LightGray * SubsystemPalette.GetColor(environmentData, PaintableItemBlock.GetColor(Terrain.ExtractData(value)));
			ItemBlock.DrawCubeBlock(primitivesRenderer, value, new Vector3(size), ref matrix, color, color, environmentData);
		}
		public override bool Equals(object obj) => Equals(obj as Device);
		public bool Equals(Device other) => base.Equals(other) && Point.Equals(other.Point);
		public override int GetHashCode()
		{
			return Point.X | Point.Z << 10 | (base.GetHashCode() ^ Point.Y) << 20;
		}
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			return new BlockPlacementData { Value = value, CellFace = raycastResult.CellFace };
		}
	}
	[Serializable]
	public abstract class CubeDevice : Device, IEquatable<CubeDevice>
	{
		public int Voltage;
		public int Index;
		public bool Powered;
		public string DefaultDisplayName, DefaultDescription;

		protected CubeDevice(string name, string description = "", int voltage = 0)
		{
			DefaultDisplayName = Utils.Get(name);
			DefaultDescription = Utils.Get(description);
			Voltage = voltage;
		}
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value) => DefaultDisplayName;
		public override string GetDescription() => DefaultDescription + (Voltage != 0 ? "(电压：" + Voltage + "V)" : "。");
		public override void Simulate(ref int voltage)
		{
			voltage &= 8191;
			if (voltage > 8000)
				return;
			if (Powered = voltage >= Voltage)
				voltage -= Voltage;
			else voltage = 0;
		}
		public bool Equals(CubeDevice other) => other != null && other.GetCraftingId() == GetCraftingId();
		public override bool Equals(object obj) => Equals(obj as CubeDevice);
		public override int GetHashCode() => base.GetHashCode() ^ Index;

		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			return new BlockPlacementData
			{
				Value = Terrain.ReplaceData(value, Terrain.ExtractData(value) & -229377 | Utils.GetDirectionXZ(componentMiner) << 15 | Index),
				CellFace = raycastResult.CellFace
			};
		}
		public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
		{
			base.GetDropValues(subsystemTerrain, Terrain.ReplaceData(oldValue, Terrain.ExtractData(oldValue) & 16383), newValue, toolLevel, dropValues, out showDebris);
		}
	}
	/*public abstract class MeshDevice : FixedDevice
	{
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();
		public BoundingBox[] m_collisionBoxes;

		protected MeshDevice(string name, string description, int voltage = 0) : base(name, description, voltage)
		{
		}
	}*/
	public class EntityDevice<T> : CubeDevice, IBlockBehavior, IItemAcceptableBlock where T : Component
	{
		public T Component;
		public string Name;
		public EntityDevice(string name, string description = "", int voltage = 0) : base(name, description, voltage)
		{
			Name = GetType().Name;
		}
		public override Device Create(Point3 p, int value)
		{
			Component = Utils.GetBlockEntity(p)?.Entity.FindComponent<T>(true);
			return this;
		}
		public virtual void OnBlockAdded(SubsystemTerrain subsystemTerrain, int value, int oldValue)
		{
			if (oldValue == -1) return;
			var vd = new ValuesDictionary();
			vd.PopulateFromDatabaseObject(subsystemTerrain.Project.GameDatabase.Database.FindDatabaseObject(Name, subsystemTerrain.Project.GameDatabase.EntityTemplateType, true));
			vd.GetValue<ValuesDictionary>("BlockEntity").SetValue("Coordinates", Point);
			var entity = subsystemTerrain.Project.CreateEntity(vd);
			Component = entity.FindComponent<T>(true);
			subsystemTerrain.Project.AddEntity(entity);
		}
		public virtual void OnBlockRemoved(SubsystemTerrain subsystemTerrain, int value, int newValue)
		{
			var blockEntity = Utils.GetBlockEntity(Point);
			if (blockEntity == null) return;
			var position = new Vector3(Point) + new Vector3(.5f);
			for (var i = blockEntity.Entity.FindComponents<IInventory>().GetEnumerator(); i.MoveNext();)
				i.Current.DropAllItems(position);
			subsystemTerrain.Project.RemoveEntity(blockEntity.Entity, true);
		}
		public void OnHitByProjectile(CellFace cellFace, WorldItem worldItem) => Utils.OnHitByProjectile(cellFace, worldItem);
	}
	public abstract class InteractiveEntityDevice<T> : EntityDevice<T>, IInteractiveBlock where T : Component
	{
		protected InteractiveEntityDevice(string name, string description, int voltage = 0) : base(name, description, voltage) { }

		public virtual bool OnInteract(TerrainRaycastResult raycastResult, ComponentMiner componentMiner)
		{
			var blockEntity = Utils.GetBlockEntity(raycastResult.CellFace.Point);
			if (blockEntity == null || componentMiner.ComponentPlayer == null)
				return false;
			componentMiner.ComponentPlayer.ComponentGui.ModalPanelWidget = GetWidget(componentMiner.Inventory, blockEntity.Entity.FindComponent<T>(true));
			AudioManager.PlaySound("Audio/UI/ButtonClick", 1f, 0f, 0f);
			return true;
		}
		public abstract Widget GetWidget(IInventory inventory, T component);
	}
	public abstract class InventoryEntityDevice<T> : InteractiveEntityDevice<T>, IElectricElementBlock where T : Component
	{
		protected InventoryEntityDevice(string name, string description, int voltage = 0) : base(name, description, voltage) { }
		public ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			return new MachineElectricElement(subsystemElectricity, new Point3(x, y, z));
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

	public abstract class InventoryEntityDevice2<T> : InteractiveEntityDevice<T>, IElectricElementBlock where T : Component
	{
		protected InventoryEntityDevice2(string name, string description, int voltage = 0) : base(name, description, voltage) { }
		public ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			return new MachineElectricElement2(subsystemElectricity, new Point3(x, y, z));
		}
		public ElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z)
		{
			return (ElectricConnectorType?)ElectricConnectorType.Output;
		}
		public int GetConnectionMask(int value)
		{
			int? color = PaintableItemBlock.GetColor(Terrain.ExtractData(value));
			return color.HasValue ? 1 << color.Value : 2147483647;
		}
	}
}