using Engine;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Chemistry
{
	public class ReactionSystem : Dictionary<Compound, int>, ICloneable, IEquatable<ReactionSystem>
	{
		public static readonly ReactionSystem Air = new ReactionSystem {
				{ new Compound("N2"), 7800 },
				{ new Compound("O2"), 2100 },
				{ new Compound("O3"), 1 },
				{ new Compound("H20"), 10 },
				{ new Compound("CO2"), 3 }
		};
		//正数=溶解部分，负数=不溶部分

		public ReactionSystem()
		{
		}

		public ReactionSystem(int capacity) : base(capacity)
		{
		}

		public ReactionSystem(string s) : this(1)
		{
			AddCompounds(this, s);
		}

		public ReactionSystem(IDictionary<Compound, int> dictionary) : base(dictionary)
		{
		}

		public void Normalize()
		{
			var list = new DynamicArray<Compound>();
			for (var i = GetEnumerator(); i.MoveNext();)
			{
				var x = i.Current;
				if (x.Value == 0)
					list.Add(x.Key);
			}
			var arr = list.Array;
			for (int i = 0; i < list.Count; i++)
			{
				Remove(arr[i]);
			}
		}

		public static void AddCompounds(Dictionary<Compound, int> dict, string s, char separator = ',')
		{
			var arr = s.Split(separator);
			for (int i = 0; i < arr.Length; i++)
			{
				int n = 0, j = 0;
				string r = arr[i];
				for (; j < r.Length && r[j] <= '9' && '0' <= r[j]; j++)
					n = n * 10 + r[j] - '0';
				if (n == 0) n = 1;
				char c = r[r.Length - 1];
				if (c == '↑' || c == '↓')
				{
					n = -n;
					r = r.Substring(0, r.Length - 1);
				}
				dict.Add(new Compound(r.Substring(j)), n);
			}
		}

		public object Clone()
		{
			return new ReactionSystem(this);
		}

		public override bool Equals(object obj)
		{
			return obj is ReactionSystem && Equals((ReactionSystem)obj);
		}

		public bool Equals(ReactionSystem other)
		{
			if (Count != other.Count)
				return false;
			var j = other.GetEnumerator();
			for (var i = GetEnumerator(); i.MoveNext() && j.MoveNext();)
			{
				var x2 = i.Current;
				var y2 = j.Current;
				if (!x2.Key.Equals(y2.Key) || x2.Value != y2.Value)
					return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			int code = 0;
			for (var i = GetEnumerator(); i.MoveNext();)
			{
				var pair = i.Current;
				code ^= pair.Key.GetHashCode() * 2111 | pair.Value << 24;
			}
			return code ^ Count << 28;
		}

		public override string ToString()
		{
			var sb = new StringBuilder();
			for (var i = GetEnumerator(); i.MoveNext();)
			{
				var pair = i.Current;
				if (pair.Value > 0)
					sb.Append(pair.Key.ToString());
			}
			return sb.ToString();
		}

		public void Add(Compound c)
		{
			Dissolve(c);
		}

		public Equation React()
		{
			Equation equation = null;
			int min = int.MaxValue, value,
				cr, ratio = 0;
			Enumerator j;
			for (var i = Equation.Reactions.GetEnumerator(); i.MoveNext();)
			{
				value = int.MaxValue;
				cr = int.MaxValue;
				for (j = i.Current.Reactants.GetEnumerator(); j.MoveNext();)
				{
					Compound key = j.Current.Key;
					if (!TryGetValue(key, out int val))
					{
						value = int.MaxValue;
						break;
					}
					value += MathUtils.Abs(val * 10000 / MathUtils.Abs(j.Current.Value) - 10000);
					val /= j.Current.Value;
					if (val < cr)
						cr = val;
				}
				if (value != int.MaxValue && value < min)
				{
					min = value;
					equation = i.Current;
					ratio = cr;
				}
			}
			if (equation == null)
				return equation;
			for (j = equation.Reactants.GetEnumerator(); j.MoveNext();)
			{
				Compound key = j.Current.Key;
				cr = ratio * j.Current.Value;
					this[key] -= cr;
			}
			for (j = equation.Products.GetEnumerator(); j.MoveNext();)
			{
				Compound key = j.Current.Key;
				this[key] +=  MathUtils.Abs(j.Current.Value);
			}
			return equation;
		}

		public void Dissolve(Compound c)
		{
			Dissolve(c, Group.K);
			Dissolve(c, Group.Na);
			Dissolve(c, Group.NO3);
		}

		public void Dissolve(Compound c, Group group)
		{
			/*if (c.TryGetValue(group, out int value))
			{
				DispersedPhase.TryGetValue(new Compound(group), value);
			}*/
		}
		/*[MethodImpl((MethodImplOptions)0x100)]
		public static implicit operator DispersionSystem(string s)
		{
			return new DispersionSystem(s);
		}*/
		[MethodImpl((MethodImplOptions)0x100)]
		public static implicit operator ReactionSystem(Compound c)
		{
			return new ReactionSystem(1)
			{
				{ c, 1 }
			};
		}
	}
}