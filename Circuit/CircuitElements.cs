using System;

namespace Game
{
	public abstract class Resistor : Element
	{
		protected Resistor() : base(ElementType.Device)
		{
		}
	}
	public abstract class FixedResistor : Resistor, IEquatable<FixedResistor>
	{
		public readonly int Resistance;
		protected FixedResistor(int resistance)
		{
			if (resistance < 1)
				throw new ArgumentOutOfRangeException("resistance", resistance, "EnergyElement has Resistance < 1");
			Resistance = resistance;
		}
		public override int GetWeight(int voltage = 0)
		{
			return Resistance;
		}
		public bool Equals(FixedResistor other)
		{
			if (other.Type == Type)
				return other.Resistance == Resistance;
			return false;
		}
		public override bool Equals(object obj)
		{
			var node = obj as FixedResistor;
			return node != null ? Equals(node) : base.Equals(node);
		}
		public override int GetHashCode()
		{
			return (int)Type ^ Resistance;
		}
	}
	/*public abstract class IC : Element, IEquatable<IC>
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
