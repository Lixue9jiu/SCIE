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
		/*public void Add(Compound c)
		{
			for (var i = DispersedPhase.Keys.GetEnumerator(); i.MoveNext();)
			{
				if (i.Current)
				{
					Equation.Reactions.Contains();
					for (var j = Equation.IonicReactions.GetEnumerator(); j.MoveNext();)
					{
						j.Current;
					}
				}
			}
		}
		public void Dissolve(Compound c)
		{

		}
		[MethodImpl((MethodImplOptions)0x100)]
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