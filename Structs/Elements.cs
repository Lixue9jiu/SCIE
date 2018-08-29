using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Engine;

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
	public abstract class Node : Item, IEquatable<Node>//, ICloneable, IComparable, IComparable<Node>, IFormattable, IConvertible
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
		public virtual void Simulate(ref int value)
		{
		}
		public bool Equals(Node other)
		{
			return other.Type == Type;
		}
		public override bool Equals(object obj)
		{
			return obj is Node node && Equals(node);
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
		/*public int CompareTo(object obj)
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
		public TypeCode GetTypeCode()
		{
			return Type.GetTypeCode();
		}
		public bool ToBoolean(IFormatProvider provider)
		{
			return ((IConvertible)Type).ToBoolean(provider);
		}
		public char ToChar(IFormatProvider provider)
		{
			return ((IConvertible)Type).ToChar(provider);
		}
		public sbyte ToSByte(IFormatProvider provider)
		{
			return ((IConvertible)Type).ToSByte(provider);
		}
		public byte ToByte(IFormatProvider provider)
		{
			return ((IConvertible)Type).ToByte(provider);
		}
		public short ToInt16(IFormatProvider provider)
		{
			return ((IConvertible)Type).ToInt16(provider);
		}
		public ushort ToUInt16(IFormatProvider provider)
		{
			return ((IConvertible)Type).ToUInt16(provider);
		}
		public int ToInt32(IFormatProvider provider)
		{
			return ((IConvertible)Type).ToInt32(provider);
		}
		public uint ToUInt32(IFormatProvider provider)
		{
			return ((IConvertible)Type).ToUInt32(provider);
		}
		public long ToInt64(IFormatProvider provider)
		{
			return ((IConvertible)Type).ToInt64(provider);
		}
		public ulong ToUInt64(IFormatProvider provider)
		{
			return ((IConvertible)Type).ToUInt64(provider);
		}
		public float ToSingle(IFormatProvider provider)
		{
			return ((IConvertible)Type).ToSingle(provider);
		}
		public double ToDouble(IFormatProvider provider)
		{
			return ((IConvertible)Type).ToDouble(provider);
		}
		public decimal ToDecimal(IFormatProvider provider)
		{
			return ((IConvertible)Type).ToDecimal(provider);
		}
		public DateTime ToDateTime(IFormatProvider provider)
		{
			return ((IConvertible)Type).ToDateTime(provider);
		}
		public string ToString(IFormatProvider provider)
		{
			return Type.ToString(provider);
		}
		public object ToType(Type conversionType, IFormatProvider provider)
		{
			return ((IConvertible)Type).ToType(conversionType, provider);
		}*/
	}
	[Serializable]
	public abstract class Element : Node, IEquatable<Element>//, ICollection, ICollection<Element>, IList, IList<Element>, IReadOnlyList<Element>, IStructuralComparable, IStructuralEquatable
	{
		public DynamicArray<Element> Next;
		/*public int Count => Next.Count;
		public object SyncRoot => this;
		public bool IsReadOnly => Next.IsReadOnly;
		public bool IsFixedSize => false;
		public bool IsSynchronized => false;
		object IList.this[int index] { get => Next.Array[index]; set => Next.Array[index] = value as Element; }
		public Element this[int index] { get => Next.Array[index]; set => Next.Array[index] = value; }*/
		protected Element(ElementType type = ElementType.Device | ElementType.Connector) : base(type)
		{
		}
		protected Element(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			Next = (DynamicArray<Element>)info.GetValue("Next", typeof(DynamicArray<Element>));
		}
		public override bool Equals(object obj)
		{
			return obj is Element node ? Equals(node) : base.Equals(obj);
		}
		public override int GetHashCode()
		{
			return Next == null ? (int)Type : (int)Type + Next.GetHashCode();
		}
		/*public DynamicArray<Element>.Enumerator GetEnumerator()
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
		}*/
		public bool Equals(Element other)
		{
			if (!base.Equals(other))
				return false;
			if (Next == null)
				return other.Next == null;
			return GetCraftingId().Equals(other.GetCraftingId());
		}
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("Type", (int)Type);
			info.AddValue("Next", Next, typeof(DynamicArray<Element>));
		}
		/*public void CopyTo(Element[] array, int index)
		{
			Next.CopyTo(array, index);
		}
		public void CopyTo(Array array, int index)
		{
			Next.Array.CopyTo(array, index);
		}
		public int Add(object value)
		{
			throw new NotImplementedException();
		}
		public void Add(Element item)
		{
			Next.Add(item);
		}
		public void Clear()
		{
			Next.Clear();
		}
		public bool Contains(object item)
		{
			return Next.Contains(item as Element);
		}
		public bool Contains(Element item)
		{
			return Next.Contains(item);
		}
		public void Remove(object item)
		{
			Next.IndexOf(item as Element);
		}
		public bool Remove(Element item)
		{
			return Next.Remove(item);
		}
		public int IndexOf(object item)
		{
			return Next.IndexOf(item as Element);
		}
		public int IndexOf(Element item)
		{
			return Next.IndexOf(item);
		}
		public void Insert(int index, object item)
		{
			Insert(index, item as Element);
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
			return ((IStructuralEquatable)Next.Array).GetHashCode(comparer);
		}
		public int CompareTo(object other, IComparer comparer)
		{
			return ((IStructuralComparable)Next.Array).CompareTo(other, comparer);
		}*/
	}
	[Serializable]
	public abstract class Device : Element, IEquatable<Device>
	{
		public Point3 Point;
		protected Device(ElementType type = ElementType.Device | ElementType.Connector) : base(type)
		{
		}
		public virtual Device Create(Point3 p)
		{
			if (SubsystemCircuit.Table.TryGetValue(p, out Device device))
			{
				return device;
			}
			device = (Device)MemberwiseClone();
			device.Point = p;
			device.Next = new DynamicArray<Element>();
			return device;
		}
		public virtual void UpdateState()
		{
		}
		protected Device(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			Point = (Point3)info.GetValue("Point", typeof(Point3));
		}
		public override bool Equals(object obj)
		{
			return obj is Device node ? Equals(node) : base.Equals(obj);
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
	}
	[Serializable]
	public abstract class FixedDevice : Device, IEquatable<FixedDevice>
	{
		public readonly int Resistance;
		protected FixedDevice(int resistance)
		{
			if (resistance < 1)
				throw new ArgumentOutOfRangeException("resistance", resistance, "Device has Resistance < 1");
			Resistance = resistance;
		}
		public FixedDevice(SerializationInfo info, StreamingContext context) : base(info, context)
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
				return other.Resistance == Resistance && other.GetCraftingId() == GetCraftingId();
			}
			return false;
		}
		public override bool Equals(object obj)
		{
			return obj is FixedDevice node ? Equals(node) : base.Equals(obj);
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
