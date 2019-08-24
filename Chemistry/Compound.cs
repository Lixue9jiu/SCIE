using System;
using System.Collections.Generic;
using System.Text;

namespace Chemistry
{
	public struct Compound : ICloneable, IEquatable<Compound>
	{
		public Group Group1, Group2, Group3;
		public int   Count1, Count2;

		//public int CrystalWater;

		public int this[Group group]
		{
			get
			{
				if (group != null)
				{
					if (group.Equals(Group1))
						return Count1;
					if (group.Equals(Group2))
						return Count2;
					if (group.Equals(Group3))
						return 1;
				}
				return 0;
			}
			set
			{
				Add(group, value);
			}
		}

		public int Count
		{
			get
			{
				int n = Group1 != null ? 1 : 0;
				if (Group2 != null)
				{
					n++;
				}
				return Group3 != null ? n + 1 : n;
			}
		}

		public Compound(string s) : this()
		{
			if (string.IsNullOrEmpty(s)) return;
			var stack = new Stack<int>();
			int i = s.IndexOf('(');
			if (i != 0)
				Add(new Group(i < 0 ? s : s.Substring(0, i)), 1);
			for (i = 0; i < s.Length; i++)
			{
				char c = s[i];
				if (c == '(') stack.Push(i + 1);
				else if (c == ')')
				{
					var key = new Group(s.Substring(stack.Peek(), i - stack.Pop()));
					i = Stack.ParseCount(s, out int count, i + 1);
					Add(key, count);
				}
			}
			i = s.LastIndexOf(')') + 1;
			if (i > 0 && i < s.Length)
				Add(new Group(s.Substring(i)), 1);
		}

		public object Clone()
		{
			return new Compound
			{
				Group1 = Group1,
				Group2 = Group2,
				Group3 = Group3,
				Count1 = Count1,
				Count2 = Count2,
			};
		}

		public void Add(Group group, int count = 1)
		{
#if DEBUG
			if (count <= 0)
				throw new ArgumentOutOfRangeException(nameof(count));
#endif
			if (Group1 == null)
			{
				Group1 = group;
				Count1 = count;
				return;
			}
			if (Group2 == null)
			{
				Group2 = group;
				Count2 = count;
				return;
			}
			if (Group3 == null)
			{
				Group3 = group;
				if (count == 1)
					return;
				if (Count1 == 1)
				{
					Group3 = Group1;
					Group1 = group;
					return;
				}
				if (Count2 == 1)
				{
					Group3 = Group2;
					Group2 = group;
					return;
				}
			}
			throw new InvalidOperationException("Stack full");
		}

		public bool Remove(Group group)
		{
			if (group == null)
				return false;
			if (group.Equals(Group1))
				goto a;
			if (group.Equals(Group2))
				goto b;
			if (group.Equals(Group3))
				goto c;
			return false;
			a: Group1 = Group2;
			b: Group2 = Group3;
			c: Group3 = null;
			return true;
		}

		/*public float AtomicWeight
		{
			get
			{
				float weight = 0;
				for (var i = GetEnumerator(); i.MoveNext();)
				{
					var pair = i.Current;
					weight += pair.Key.AtomicWeight * pair.Value;
				}
				return weight;
			}
		}

		public int GetAtomCount()
		{
			int count = 0;
			for (var i = GetEnumerator(); i.MoveNext();)
			{
				var pair = i.Current;
				count += pair.Key.GetAtomCount() * pair.Value;
			}
			return count;
		}

		public int GetAtomCount(AtomKind atom)
		{
			int count = 0;
			for (var i = GetEnumerator(); i.MoveNext();)
			{
				var pair = i.Current;
				count += pair.Key.GetAtomCount(atom) * pair.Value;
			}
			return count;
		}*/

		#region Operator overloads

		public override bool Equals(object obj)
		{
			return obj is Compound compound ? Equals(compound) : base.Equals(obj);
		}

		public bool Equals(Compound other)
		{
			return Equals(Group1, other.Group1) && Equals(Group2, other.Group2) && Equals(Group3, other.Group3)
				&& Count1 == other.Count1 && Count2 == other.Count2;
		}

		public override int GetHashCode()
		{
			int code = Count1 << 3 ^ Count2;
			if (Group1 != null)
			{
				code ^= Group1.GetHashCode();
			}
			if (Group2 != null)
			{
				code ^= Group2.GetHashCode();
			}
			if (Group3 != null)
			{
				code ^= Group3.GetHashCode() * 2111;
			}
			return code;
		}

		/*public static Compound operator +(Compound c1, Compound c2)
		{
			var result = new Compound(c1.Count + c2.Count);
			Enumerator i;
			for (i = c1.GetEnumerator(); i.MoveNext();)
				result.Add(i.Current.Key, i.Current.Value);
			for (i = c2.GetEnumerator(); i.MoveNext();)
			{
				var key = i.Current.Key;
				result.TryGetValue(key, out int value);
				result[key] = value + i.Current.Value;
			}
			return result;
		}
		
		[MethodImpl((MethodImplOptions)0x100)]
		public static Compound operator +(Compound c, Group g)
		{
			var result = (Compound)c.Clone();
			result.Add(g);
			return result;
		}

		public static Compound operator -(Compound c1, Compound c2)
		{
			var result = new Compound(c1);
			for (var i = c2.GetEnumerator(); i.MoveNext();)
			{
				var key = i.Current.Key;
				if (result.TryGetValue(key, out int value))
				{
					value -= i.Current.Value;
					if (value > 0)
						result[key] = value;
					else
						result.Remove(key);
				}
			}
			return result;
		}
		
		[MethodImpl((MethodImplOptions)0x100)]
		public static Compound operator -(Compound c, Group g)
		{
			var result = (Compound)c.Clone();
			result.Remove(g);
			return result;
		}
		
		public static Compound operator *(Compound c, int f)
		{
			var result = new Compound(c.Count);
			for (var i = c.Keys.GetEnumerator(); i.MoveNext();)
				result[i.Current] = c[i.Current] * f;
			return result;
		}
		
		public static Compound operator /(Compound c, int f)
		{
			var result = new Compound(c.Count);
			for (var i = c.Keys.GetEnumerator(); i.MoveNext();)
				result[i.Current] = c[i.Current] / f;
			return result;
		}

		[MethodImpl((MethodImplOptions)0x100)]
		public static implicit operator Compound(string s)
		{
			return new Compound(s);
		}*/

		#endregion

		public override string ToString()
		{
			var sb = new StringBuilder();
			if (Group3 != null)
			{
				sb.Append(Group3.ToString());
			}
			bool bracket;
			if (Group1 != null)
			{
				bracket = Count1 > 1 && Group1.Count > 1;
				if (bracket)
					sb.Append('(');
				sb.Append(Group1.ToString());
				if (bracket)
					sb.Append(')');
				if (Count1 > 1)
					sb.Append(Count1);
			}
			if (Group2 != null)
			{
				bracket = Count2 > 1 && Group2.Count > 1;
				if (bracket)
					sb.Append('(');
				sb.Append(Group2.ToString());
				if (bracket)
					sb.Append(')');
				if (Count2 > 1)
					sb.Append(Count2);
			}
			return sb.ToString();
		}
	}
}