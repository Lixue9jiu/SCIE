using Engine;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Chemistry
{
	/// <summary>
	///     Represents a dispersion system.
	/// </summary>
	public struct DispersionSystem : ICloneable, IEquatable<DispersionSystem>
	{
		public static readonly CompoundsComparer Comparer = new CompoundsComparer();
		public static readonly DispersionSystem Air = new DispersionSystem
		{
			DispersedPhase = new Dictionary<Compound, int>
			{
				{ new Compound("N2"), 7800 },
				{ new Compound("O2"), 2100 },
				{ new Compound("O3"), 1 },
				{ new Compound("H20"), 10 },
				{ new Compound("CO2"), 3 }
			}
		};
		public Dictionary<Compound, int> Dispersant;
		public Dictionary<Compound, int> DispersedPhase;

		public DispersionSystem(int capacity)
		{
			Dispersant = new Dictionary<Compound, int>();
			DispersedPhase = new Dictionary<Compound, int>(capacity);
		}

		public DispersionSystem(string s) : this(1)
		{
			AddCompounds(DispersedPhase, s);
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
			return new DispersionSystem
			{
				Dispersant = new Dictionary<Compound, int>(Dispersant),
				DispersedPhase = new Dictionary<Compound, int>(DispersedPhase)
			};
		}

		public override bool Equals(object obj)
		{
			return obj is DispersionSystem && Equals((DispersionSystem)obj);
		}

		[MethodImpl((MethodImplOptions)0x100)]
		public bool Equals(DispersionSystem other)
		{
			return Comparer.Equals(Dispersant, other.Dispersant) && Comparer.Equals(DispersedPhase, other.DispersedPhase);
		}

		public override int GetHashCode()
		{
			return Comparer.GetHashCode(Dispersant) * -1521134295 ^ Comparer.GetHashCode(DispersedPhase);
		}

		public override string ToString()
		{
			var sb = new StringBuilder();
			for (var i = DispersedPhase.GetEnumerator(); i.MoveNext();)
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
			Dictionary<Compound, int>.Enumerator j;
			for (var i = Equation.Reactions.GetEnumerator(); i.MoveNext();)
			{
				value = int.MaxValue;
				cr = int.MaxValue;
				for (j = i.Current.Reactants.GetEnumerator(); j.MoveNext();)
				{
					Compound key = j.Current.Key;
					if (!DispersedPhase.TryGetValue(key, out int val) && !Dispersant.TryGetValue(key, out val))
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
				if (!DispersedPhase.TryGetValue(key, out value))
				{
					Dispersant[key] -= cr;
				}
				else
					DispersedPhase[key] = value - cr;
			}
			for (j = equation.Products.GetEnumerator(); j.MoveNext();)
			{
				Compound key = j.Current.Key;
				if (!DispersedPhase.TryGetValue(key, out value))
				{
					if (Dispersant.TryGetValue(key, out value))
						Dispersant[key] = value + MathUtils.Abs(j.Current.Value);
					else
						goto a;
					continue;
				}
				a:
					DispersedPhase[key] = value + MathUtils.Abs(j.Current.Value);
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
		public static implicit operator DispersionSystem(Compound c)
		{
			var result = new DispersionSystem(1);
			result.DispersedPhase.Add(c, 1);
			return result;
		}
	}
}