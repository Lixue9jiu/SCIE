﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Chemistry
{
	[Flags]
	public enum Condition : ushort
	{
		None = 0,
		l = 1,
		L = 2,
		UV = 4,
		H = 8,
		E = 16,
		I = 32,
	}
	/// <summary>
	///     Represents a chemical equation.
	/// </summary>
	public class Equation : ICloneable, IEquatable<Equation>
	{
		public static HashSet<Equation> Reactions;

		/// <summary>
		///     Initializes an instance of the <see cref="Equation" /> class.
		/// </summary>
		/// <param name="reactants">The reactants.</param>
		/// <param name="products">The products.</param>
		public Equation(ReactionSystem reactants, ReactionSystem products)
		{
			Reactants = reactants;
			Products = products;
		}

		public Equation()
		{
			Reactants = new ReactionSystem();
			Products = new ReactionSystem();
		}

		/// <summary>
		///     Gets the left-hand assignment (reactants) of the <see cref="Equation" />.
		/// </summary>
		public ReactionSystem Reactants;

		/// <summary>
		///     Gets the right-hand assignment (products) of the <see cref="Equation" />.
		/// </summary>
		public ReactionSystem Products;

		public ushort Temperature, Rate, Heat, Conversion;
		public Condition Conditions;

		public static Equation Parse(string s)
		{
			var e = new Equation();
			var x = s.Split(';');
			CultureInfo ic = CultureInfo.InvariantCulture;
			if (x.Length > 1)
			{
				s = x[0];
				e.Rate = ushort.Parse(x[1], ic);
				if (x.Length >= 2) {
					e.Heat = ushort.Parse(x[3], ic);
					if (x.Length >= 3)
						e.Conversion = ushort.Parse(x[4], ic);
				}
			}
			var l = s.Split('=');
			if (l.Length < 3)
				throw new ArgumentException("Invaild format", nameof(s));
			ReactionSystem.AddCompounds(e.Reactants, l[0], '+');
			ReactionSystem.AddCompounds(e.Products, l[2], '+');
			var c = l[1].Split(new[]{','}, StringSplitOptions.RemoveEmptyEntries);
			Condition cond = 0;
			for (int i = 0; i < c.Length; i++)
				switch (c[i][0])
				{
					case 'l': cond |= Condition.l; break;
					case 'L': cond |= Condition.L; break;
					case 'U': cond |= Condition.UV; break;
					case 'H': cond |= Condition.H; break;
					case 'E': cond |= Condition.E; break;
					case 'I': cond |= Condition.I; break;
					default:
						e.Temperature = ushort.Parse(c[i], ic);
						break;
				};
			e.Conditions = cond;
			return e;
		}

		public object Clone()
		{
			return new Equation(new ReactionSystem(Reactants), new ReactionSystem(Products));
		}

		/*/// <summary>
		///     Balances the <see cref="Equation{T}" />.
		/// </summary>
		/// <returns></returns>
		public abstract bool Balance(Equation<T> equation);*/

		/// <summary>Returns a string that represents the current object.</summary>
		/// <returns>A string that represents the current object.</returns>
		public override string ToString()
		{
			var sb = new StringBuilder();

			var left = Reactants.ToArray();
			var right = Products.ToArray();
			KeyValuePair<Compound, int> current;
			int i;
			for (i = 0; i < left.Length; i++)
			{
				current = left[i];
				if (i < left.Length - 1)
				{
					sb.Append(current);
					sb.Append(" + ");
					continue;
				}

				sb.Append(current);
				sb.Append(" -> ");
			}

			for (i = 0; i < right.Length; i++)
			{
				current = right[i];
				if (i < right.Length - 1)
				{
					sb.Append(current);
					sb.Append(" + ");
					continue;
				}

				sb.Append(current);
			}

			return sb.ToString();
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as Equation);
		}

		public bool Equals(Equation other)
		{
			return other != null &&
				   Reactants.Equals(other.Reactants) &&
				   Temperature == other.Temperature &&
				   Conditions == other.Conditions;
		}

		public override int GetHashCode()
		{
			return (Reactants.GetHashCode() * -1521134295 ^ Temperature) * -1521134295 ^ (int)Conditions;
		}
	}
}