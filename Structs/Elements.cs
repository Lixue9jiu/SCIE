using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Engine;
using Engine.Graphics;

namespace Game
{
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
	public abstract class Node : IComparable, IComparable<Node>, IEquatable<Node>, IFormattable
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
		public override string ToString()
		{
			return Type.ToString();
		}
		public int CompareTo(object obj)
		{
			return Type.CompareTo(obj);
		}
		public int CompareTo(Node other)
		{
			return Type.CompareTo(other);
		}
		public string ToString(string format, IFormatProvider formatProvider)
		{
			return Type.ToString(format);
		}
	}
	[Serializable]
	public abstract class Element : Node, ICloneable, ICollection, IEnumerable<Element>, IEnumerable, IEquatable<Element>, IList<Element>, IStructuralComparable, IStructuralEquatable
	{
		public DynamicArray<Element> Next;

		public int Count => Next.Count;
		public object SyncRoot => this;
		public bool IsReadOnly => Next.IsReadOnly;
		public bool IsFixedSize => true;
		public bool IsSynchronized => false;
		public Element this[int index] { get => Next.Array[index]; set => Next.Array[index] = value; }
		protected Element(ElementType type = ElementType.Device) : base(type)
		{
		}
		protected Element(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			Next = (DynamicArray<Element>)info.GetValue("Next", typeof(DynamicArray<Element>));
		}
		public object Clone()
		{
			return MemberwiseClone();
		}
		public virtual void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawCubeBlock(primitivesRenderer, value, new Vector3(size), ref matrix, color, color, environmentData);
		}
		public virtual string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			throw new NotImplementedException();
		}
		public virtual string GetDescription(int value)
		{
			return "";
		}
		public virtual int GetFaceTextureSlot(int face, int value)
		{
			return 245;
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

		public void CopyTo(Element[] array, int index)
		{
			Next.CopyTo(array, index);
		}
		public void CopyTo(Array array, int index)
		{
			Next.Array.CopyTo(array, index);
		}
		public void Add(Element item)
		{
			Next.Add(item);
		}
		public void Clear()
		{
			Next.Clear();
		}
		public bool Contains(Element item)
		{
			return Next.Contains(item);
		}
		public bool Remove(Element item)
		{
			return Next.Remove(item);
		}
		public int IndexOf(Element item)
		{
			return Next.IndexOf(item);
		}
		public void Insert(int index, Element item)
		{
			throw new NotImplementedException();
		}
		public void RemoveAt(int index)
		{
			Next.RemoveAt(index);
		}
		public bool Equals(object other, IEqualityComparer comparer)
		{
			return (Next.Array as IStructuralEquatable).Equals(other, comparer);
		}
		public int GetHashCode(IEqualityComparer comparer)
		{
			return (Next.Array as IStructuralEquatable).GetHashCode(comparer);
		}
		public int CompareTo(object other, IComparer comparer)
		{
			return (Next.Array as IStructuralComparable).CompareTo(other, comparer);
		}
	}
	[Serializable]
	public abstract class Device : Element, IEquatable<Device>, IEquatable<Point3>
	{
		public Point3 Point;
		protected Device(ElementType type = ElementType.Device) : base(type)
		{
		}
		protected Device(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			Point = (Point3)info.GetValue("Point", typeof(Point3));
		}
		public virtual void GenerateTerrainVertices(Block block, BlockGeometryGenerator generator, TerrainGeometrySubsets geometry, int value, int x, int y, int z)
		{
			generator.GenerateCubeVertices(block, value, x, y, z, Color.White, geometry.OpaqueSubsetsByFace);
		}
		public virtual BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			return new BlockPlacementData
			{
				Value = value,
				CellFace = raycastResult.CellFace
			};
		}
		public virtual void OnBlockAdded(int value, int oldValue, int x, int y, int z)
		{
		}
		public override bool Equals(object obj)
		{
			var node = obj as Device;
			return node != null ? Equals(node) : base.Equals(node);
		}
		public bool Equals(Device other)
		{
			return base.Equals(other) && Point.Equals(other.Point);
		}
		public override int GetHashCode()
		{
			return Point.X | Point.Z << 10 | (base.GetHashCode() ^ Point.Y) << 20;
		}
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("Point", Point, typeof(Point3));
		}
		public bool Equals(Point3 other)
		{
			return Point.Equals(other);
		}
	}
	[Serializable]
	public abstract class FixedDevice : Device, IEquatable<FixedDevice>
	{
		public readonly int Resistance;
		protected FixedDevice(int resistance)
		{
			if (resistance < 1)
				throw new ArgumentOutOfRangeException("resistance", resistance, "EnergyElement has Resistance < 1");
			Resistance = resistance;
		}
		protected FixedDevice(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			Resistance = info.GetInt32("Resistance");
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
			return base.GetHashCode() ^ Resistance;
		}
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("Resistance", Resistance);
		}
	}
}
