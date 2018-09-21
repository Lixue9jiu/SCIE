using System;
using System.Runtime.Serialization;
using Engine;
using Engine.Graphics;

namespace Game
{
	[Flags]
	//[Serializable]
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
		//Capacitor = 262144,
		//Inductor = 524288,
		Drive = 1048576,
		Tube = 2097152,
	}
	[Serializable]
	public abstract class Element : Item, IEquatable<Element>, INode//, ICloneable, IComparable, IComparable<Node>, IFormattable, IConvertible, ICollection, ICollection<Element>, IList, IList<Element>, IReadOnlyList<Element>, IStructuralComparable, IStructuralEquatable
	{
		public ElementType Type;
		//public ElectricConnectorDirection Direction;
		public DynamicArray<Element> Next;
		/*public int Count => Next.Count;
		public object SyncRoot => this;
		public bool IsReadOnly => Next.IsReadOnly;
		public bool IsFixedSize => false;
		public bool IsSynchronized => false;
		object IList.this[int index] { get => Next.Array[index]; set => Next.Array[index] = value as Element; }
		public Element this[int index] { get => Next.Array[index]; set => Next.Array[index] = value; }*/
		protected Element(ElementType type = ElementType.Device | ElementType.Connector)
		{
			Type = type;
		}
		protected Element(SerializationInfo info, StreamingContext context)
		{
			Type = (ElementType)info.GetInt32("Type");
			Next = (DynamicArray<Element>)info.GetValue("Next", typeof(DynamicArray<Element>));
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
			return obj is Element node ? Equals(node) : base.Equals(obj);
		}
		public override int GetHashCode()
		{
			return Next == null ? (int)Type : (int)Type + Next.GetHashCode();
		}
		public bool Equals(Element other)
		{
			if (other.Type != Type)
				return false;
			if (Next == null)
				return other.Next == null;
			return GetCraftingId().Equals(other.GetCraftingId());
		}
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("Type", (int)Type);
			info.AddValue("Next", Next, typeof(DynamicArray<Element>));
		}
		public override string ToString()
		{
			return Type.ToString();
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
		}
		public void CopyTo(Element[] array, int index)
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
		#endregion
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
			if ((device.Type & ElementType.Connector) != 0)
			{
				var color = PaintableItemBlock.GetColor(Terrain.ExtractData(Utils.Terrain.GetCellValue(p.X, p.Y, p.Z)));
				device.Type = device.Type & ~ElementType.Connector | (color.HasValue ? (ElementType)(1 << (color.Value + 2)) : ElementType.Connector);
			}
			return device;
		}
		public override void GenerateTerrainVertices(Block block, BlockGeometryGenerator generator, TerrainGeometrySubsets geometry, int value, int x, int y, int z)
		{
			generator.GenerateCubeVertices(ItemBlock, value, x, y, z, Color.LightGray * SubsystemPalette.GetColor(generator, PaintableItemBlock.GetColor(Terrain.ExtractData(value))), geometry.OpaqueSubsetsByFace);
		}
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			color = Color.LightGray * SubsystemPalette.GetColor(environmentData, PaintableItemBlock.GetColor(Terrain.ExtractData(value)));
			BlocksManager.DrawCubeBlock(primitivesRenderer, value, new Vector3(size), ref matrix, color, color, environmentData);
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
			//if (resistance < 1)
				//throw new ArgumentOutOfRangeException("resistance", resistance, "Device has Resistance < 1");
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

		protected DeviceBlock(int voltage, ElementType type = ElementType.Device | ElementType.Connector) : base(type)
		{
			Voltage = voltage;
		}
		public override string GetCraftingId()
		{
			return DefaultDisplayName;
		}
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			return DefaultDisplayName;
		}
		public override string GetDescription(int value)
		{
			return DefaultDescription;
		}
		/*public int CompareTo(DeviceBlock other)
		{
			return Voltage.CompareTo(other.Voltage);
		}*/
		public bool Equals(DeviceBlock other)
		{
			return base.Equals(other) && Voltage == other.Voltage;
		}
	}
}
