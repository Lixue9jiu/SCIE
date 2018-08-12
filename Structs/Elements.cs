using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Engine;
using Engine.Graphics;

namespace Game
{
	public class ElementBlock : Block, IPaintableBlock
	{
		public const int Index = 300;
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometrySubsets geometry, int value, int x, int y, int z)
		{
			var element = SubsystemEnergy.GetCircuitElement(value);
			if (element != null)
			{
				element.GenerateTerrainVertices(this, generator, geometry, value, x, y, z);
			}
		}
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			var element = SubsystemEnergy.GetCircuitElement(value);
			if (element != null)
			{
				element.DrawBlock(primitivesRenderer, value, color, size, ref matrix, environmentData);
			}
		}
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			return new BlockPlacementData
			{
				Value = value,
				CellFace = raycastResult.CellFace
			};
		}
		public override IEnumerable<int> GetCreativeValues()
		{
			var list = new List<int>(8);
			int i = 0;
			Element element;
			do
			{
				element = SubsystemEnergy.GetCircuitElement(Terrain.MakeBlockValue(Index, 0, i++));
			}
			while (element != null);
			return list;
		}
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			var element = SubsystemEnergy.GetCircuitElement(value);
			var name = element.GetDisplayName(subsystemTerrain, value);
			if (element != null)
			{
				int? paintColor = GetPaintColor(value);
				return SubsystemPalette.GetName(subsystemTerrain, paintColor, name);
			}
			return name;
		}
		public override string GetDescription(int value)
		{
			return "";
		}
		public override int GetFaceTextureSlot(int face, int value)
		{
			return value;
		}
		public int? GetPaintColor(int value)
		{
			return GetColor(Terrain.ExtractData(value));
		}
		public int Paint(SubsystemTerrain subsystemTerrain, int value, int? color)
		{
			int data = Terrain.ExtractData(value);
			return Terrain.ReplaceData(value, SetColor(data, color));
		}
		public static int? GetColor(int data)
		{
			return (data & 64) != 0 ? data >> 7 & 15 : default(int?);
		}
		public static int SetColor(int data, int? color)
		{
			if (color.HasValue) {
				return (data & -1985) | 64 | (color.Value & 15) << 7;
			}
			return data & -1985;
		}
	}
	[Flags]
	[Serializable]
	public enum ElementType
	{
		None = 0,
		Supply = 1, // Power
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
		Capacitor = 262144,
		Inductor = 524288,
	}
	[Serializable]
	public abstract class Node : IEquatable<Node>
	{
		public readonly ElementType Type;
		//public ElectricConnectorDirection Direction;
		protected Node(ElementType type)
		{
			Type = type;
		}
        protected Node(SerializationInfo info, StreamingContext context)
        {
        	Type = (ElementType)info.GetInt32("Type");
        }
		public bool Equals(Node other)
		{
			return other.Type == Type;
		}
		public override bool Equals(object obj)
		{
			var node = obj as Node;
			return node != null && Equals(node);
		}
		public virtual int GetWeight(int value = 0)
		{
			if ((Type & ElementType.Connector) != 0)
				return 1;
			if ((Type & ElementType.Supply) != 0)
				return 100;
			return 2147483647;
		}
		public override int GetHashCode()
		{
			return (int)Type;
		}
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
        	info.AddValue("Type", (int)Type);
        }
	}
	[Serializable]
	public abstract class Element : Node, IEnumerable<Element>, IEnumerable, IEquatable<Element>
	{
		public DynamicArray<Element> Next;
		protected Element(ElementType type) : base(type)
		{
		}
		protected Element(SerializationInfo info, StreamingContext context) : base(info, context)
        {
			Next = (DynamicArray<Element>)info.GetValue("Next", typeof(DynamicArray<Element>));
        }
		public virtual void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawCubeBlock(primitivesRenderer, value, new Vector3(size), ref matrix, color, color, environmentData);
		}
		public virtual string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			throw new NotImplementedException();
		}
		public virtual void Simulate(ref int value)
		{
		}
		public override bool Equals(object obj)
		{
			var node = obj as Element;
			return node != null ? Equals(node) : base.Equals(node);
		}
		public override int GetHashCode()
		{
			return Next == null ? (int)Type : (int)Type + Next.Count;
		}
		public DynamicArray<Element>.Enumerator GetEnumerator()
		{
			return new DynamicArray<Element>.Enumerator(Next);
		}
		IEnumerator<Element> IEnumerable<Element>.GetEnumerator()
		{
			return new DynamicArray<Element>.Enumerator(Next);
		}
		IEnumerator IEnumerable.GetEnumerator()
		{
			return new DynamicArray<Element>.Enumerator(Next);
		}
		public bool Equals(Element other)
		{
			if (!base.Equals(other))
				return false;
			if (Next == null)
				return other.Next == null;
			return Next.Count == other.Next.Count;
		}
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
        	info.AddValue("Type", (int)Type);
        	info.AddValue("Next", Next, typeof(DynamicArray<Element>));
        }
	}
	public abstract class Device : Element
	{
		public Point3 Point;
		protected Device() : base(ElementType.Device)
		{
		}
		public virtual void GenerateTerrainVertices(Block block, BlockGeometryGenerator generator, TerrainGeometrySubsets geometry, int value, int x, int y, int z)
		{
			generator.GenerateCubeVertices(block, value, x, y, z, Color.White, geometry.OpaqueSubsetsByFace);
		}
		public override bool Equals(object obj)
		{
			var node = obj as CircuitDevice;
			return node != null ? Equals(node) : base.Equals(node);
		}
		public bool Equals(CircuitDevice other)
		{
			return base.Equals(other) && Point.Equals(other.Point);
		}
		public override int GetHashCode()
		{
			return Point.X | Point.Z << 10 | (base.GetHashCode() ^ Point.Y) << 20;
		}
	}
	public abstract class FixedDevice : Device, IEquatable<FixedDevice>
	{
		public readonly int Resistance;
		protected FixedDevice(int resistance)
		{
			if (resistance < 1)
				throw new ArgumentOutOfRangeException("resistance", resistance, "EnergyElement has Resistance < 1");
			Resistance = resistance;
		}
		public override int GetWeight(int value)
		{
			return Resistance;
		}
		public bool Equals(FixedDevice other)
		{
			if (other.Type == Type)
			{
				return other.Resistance == Resistance;
			}
			return false;
		}
		public override bool Equals(object obj)
		{
			var node = obj as FixedDevice;
			return node != null ? Equals(node) : base.Equals(node);
		}
		public override int GetHashCode()
		{
			return (int)Type ^ Resistance;
		}
	}
}
