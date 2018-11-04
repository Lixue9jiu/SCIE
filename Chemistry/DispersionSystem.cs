using System;
using System.Collections.Generic;
using System.Text;

namespace Chemistry
{
	public struct DispersionSystem : ICloneable, IEquatable<DispersionSystem>
	{
		public Dictionary<Compound, int> Dispersant;
		public Dictionary<Compound, int> DispersedPhase;

		public DispersionSystem(int capacity)
		{
			Dispersant = new Dictionary<Compound, int>();
			DispersedPhase = new Dictionary<Compound, int>(capacity);
		}

		public DispersionSystem(string s) : this(1)
		{
			var arr = s.Split(',');
			for (int i = 0; i < arr.Length; i++)
			{
				int x = s.IndexOf('*');
				if (x >= 0)
				{
					if (!int.TryParse(s, out x))
					{
						x = 1;
					}
				}
				else
				{
					x = 1;
				}
				DispersedPhase.Add(new Compound(arr[i]), x);
			}
		}

		public object Clone()
		{
			var ds = new DispersionSystem
			{
				Dispersant = new Dictionary<Compound, int>(Dispersant),
				DispersedPhase = new Dictionary<Compound, int>(DispersedPhase)
			};
			return ds;
		}

		public override bool Equals(object obj)
		{
			return obj is DispersionSystem && Equals((DispersionSystem)obj);
		}

		public bool Equals(DispersionSystem other)
		{
			return EqualityComparer<Dictionary<Compound, int>>.Default.Equals(Dispersant, other.Dispersant) &&
				   EqualityComparer<Dictionary<Compound, int>>.Default.Equals(DispersedPhase, other.DispersedPhase);
		}

		public override int GetHashCode()
		{
			var hashCode = -512996931;
			hashCode = hashCode * -1521134295 + EqualityComparer<Dictionary<Compound, int>>.Default.GetHashCode(Dispersant);
			hashCode = hashCode * -1521134295 + EqualityComparer<Dictionary<Compound, int>>.Default.GetHashCode(DispersedPhase);
			return hashCode;
		}

		public override string ToString()
		{
			var sb = new StringBuilder();
			for (var i = DispersedPhase.GetEnumerator(); i.MoveNext();)
			{
				var pair = i.Current;
				if (pair.Value > 0)
				{
					sb.Append(pair.Key.ToString());
				}
			}
			return sb.ToString();
		}
		/*public void Add(Compound c)
		{
			for (var i = DispersedPhase.Keys.GetEnumerator(); i.MoveNext();)
			{
				if (i.Current is Ion ion)
				{
					IonReactions.First(ion, this);
					for (var j = IonReactions.Find(ion).GetEnumerator(); j.MoveNext();)
					{
						j.Current
					}
				}
			}
		}
		public static implicit operator DispersionSystem(string s)
		{
			return new DispersionSystem(s);
		}*/
		public static implicit operator DispersionSystem(Compound c)
		{
			var result = new DispersionSystem(1);
			result.DispersedPhase.Add(c, 1);
			return result;
		}
	}
}