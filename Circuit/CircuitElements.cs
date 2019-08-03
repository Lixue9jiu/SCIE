using System;

namespace Game
{
	public class FixedResistor : FixedDevice
	{
		public FixedResistor(string name, int resistance) : base(name, name, resistance)
		{
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
