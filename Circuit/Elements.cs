using System;
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
		Container = 3, // Battery
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
		Pipe0 = 1 << 20,
		Pipe1 = 1 << 21,
		Pipe2 = 1 << 22,
		Pipe3 = 1 << 23,
		Pipe4 = 1 << 24,
		Pipe5 = 1 << 25,
		RodX = 1 << 26,
		RodY = 1 << 27,
		RodZ = 1 << 28,
		Rod = RodX | RodY | RodZ
	}
	//[Serializable]
	public abstract class Element : Item, IEquatable<Element>, INode
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
		public virtual int GetWeight(int value = 0)
		{
			if ((Type & ElementType.Connector) != 0)
				return 1;
			if ((Type & ElementType.Supply) != 0)
				return 100;
			return 2147483647;
		}
		#region implements & overloads
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
		public virtual Device Create(Point3 p)
		{
			if (SubsystemCircuit.Table.TryGetValue(p, out Device device))
				return device;
			device = (Device)MemberwiseClone();
			device.Point = p;
			device.Next = new Element[0];
			if ((device.Type & ElementType.Connector) != 0)
			{
				var color = PaintableItemBlock.GetColor(Terrain.ExtractData(Utils.Terrain.GetCellValue(p.X, p.Y, p.Z)));
				device.Type = device.Type & ~ElementType.Connector | (color.HasValue ? (ElementType)(1 << (color.Value + 2)) : ElementType.Connector);
			}
			return device;
		}
		public override void GenerateTerrainVertices(Block block, BlockGeometryGenerator generator, TerrainGeometrySubsets geometry, int value, int x, int y, int z)
		{
			generator.GenerateCubeVertices(Block, value, x, y, z, Color.LightGray * SubsystemPalette.GetColor(generator, PaintableItemBlock.GetColor(Terrain.ExtractData(value))), geometry.OpaqueSubsetsByFace);
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
	}
	[Serializable]
	public abstract class FixedDevice : Device, IEquatable<FixedDevice>
	{
		public readonly int Resistance;
		public string DefaultDisplayName;
		public string DefaultDescription;

		protected FixedDevice(string name, string description = "", int resistance = 1000)
		{
			//if (resistance < 1) throw new ArgumentOutOfRangeException("resistance", resistance, "Device has Resistance < 1");
			DefaultDisplayName = Utils.Get(name);
			DefaultDescription = Utils.Get(description);
			Resistance = resistance;
		}
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value) => DefaultDisplayName;
		public override string GetDescription(int value) => DefaultDescription;
		public override int GetWeight(int value) => Resistance;
		public bool Equals(FixedDevice other) => other != null && other.Type == Type && other.Resistance == Resistance && other.GetCraftingId() == GetCraftingId();
		public override bool Equals(object obj) => Equals(obj as FixedDevice);
		public override int GetHashCode() => base.GetHashCode() ^ Resistance;
		public static BlockPlacementData GetPlacementValue(int index, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			return new BlockPlacementData
			{
				Value = Terrain.ReplaceData(value, Terrain.ExtractData(value) & -229377 | Utils.GetDirectionXZ(componentMiner) << 15 | index),
				CellFace = raycastResult.CellFace
			};
		}
	}
	public abstract class DeviceBlock : Device, IEquatable<DeviceBlock>//, IComparable<DeviceBlock>
	{
		public string DefaultDisplayName;
		public string DefaultDescription;
		public readonly int Voltage;

		protected DeviceBlock(int voltage, string name = "", string description = "", ElementType type = ElementType.Device | ElementType.Connector) : base(type)
		{
			DefaultDisplayName = Utils.Get(name);
			DefaultDescription = Utils.Get(description);
			Voltage = voltage;
		}
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value) => DefaultDisplayName;
		public override string GetDescription(int value) => DefaultDescription;
		// public int CompareTo(DeviceBlock other) => Voltage.CompareTo(other.Voltage);
		public bool Equals(DeviceBlock other) => base.Equals(other) && Voltage == other.Voltage;
	}
	public class EntityDevice<T> : FixedDevice, IBlockBehavior, IItemAcceptableBlock where T : Component
	{
		public T Component;
		public string Name;
		public EntityDevice(string ename, string name, string description = "") : base(name, description)
		{
			Name = ename;
		}
		public override Device Create(Point3 p)
		{
			var device = (EntityDevice<T>)base.Create(p);
			device.Component = Utils.GetBlockEntity(p)?.Entity.FindComponent<T>(true);
			device.Name = Name;
			return device;
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
		protected InteractiveEntityDevice(string ename, string name, string description) : base(ename, name, description) { }

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
}