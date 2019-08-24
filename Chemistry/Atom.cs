using Game;
using System;
using System.Collections.Generic;
using System.Text;
/*
namespace Game
{
	public static partial class Utils
	{
		public static readonly float[] AtomicWeights = new float[50];
		public static readonly int[] AtomicNumbers = new int[50];

		public static float AtomicWeight(this AtomKind kind) => AtomicWeights[(int)kind];
		public static float AtomicNumber(this AtomKind kind) => AtomicNumbers[(int)kind];
	}
}
*/
namespace Chemistry
{
	public enum AtomKind
	{
		Nu, H, D, T, Li, Be, B, C, N, O, F, Na, Mg, Al, Si, P, S, Cl, K, Ca, Ti, V, Cr, Mn, Fe, Co, Ni, Cu, Zn, Ga, Ge, As, Br, Zr, Nb, Ag, Cd, In, Sn, Sb, I, Ba, W, Ir, Pt, Au, Hg, Pb, U235, U238
	}

	public struct Stack : IEquatable<Stack>
	{
		public AtomKind Atom;
		public int Count;

		public Stack(AtomKind atom, int count)
		{
			Atom = atom;
			Count = count;
		}

		public override bool Equals(object obj)
		{
			return obj is Stack stack && Equals(stack);
		}

		public bool Equals(Stack other)
		{
			return Atom == other.Atom && Count == other.Count;
		}

		public override int GetHashCode()
		{
			return (int)Atom | Count << 6;
		}

		public override string ToString()
		{
			return Count == 1 ? Atom.ToString() : Atom.ToString() + Count;
		}

		public static int ParseCount(string s, out int n, int start = 0)
		{
			n = 0;
			if (string.IsNullOrEmpty(s)) return 2000000000;
			char c;
			if (start < s.Length)
			{
				c = s[start];
				if ('₀' <= c && c <= '₉')
					n = n * 10 + (c - '₀');
				else if (c <= '9' && '0' <= c)
					n = n * 10 + (c - '0');
				else goto a;
			}
			else goto a;
			for (start++; start < s.Length; start++)
			{
				c = s[start];
				if ('₀' <= c && c <= '₉')
					n = n * 10 + (c - '₀');
				else if (c <= '9' && '0' <= c)
					n = n * 10 + (c - '0');
				else return start;
			}
			return start;
			a:
			n = 1;
			return start;
		}
	}

	public class Group : IEquatable<Group>
	{
		public static readonly Dictionary<string, Group> Groups = new Dictionary<string, Group>(9);
		public Stack Stack1,
					 Stack2,
					 Stack3;

		/*public Stack this[int i]
		{
			get
			{
				return i == 0 ? Stack1 :
					i == 1 ? Stack2 : Stack3;
			}
		}*/

		public int Count
		{
			get
			{
				int n = Stack1.Count != 0 ? 1 : 0;
				if (Stack2.Count != 0)
				{
					n++;
				}
				return Stack3.Count != 0 ? n + 1 : n;
			}
		}

		public int Charge;

		public Group(string s)
		{
			if (string.IsNullOrEmpty(s)) return;
			if (Groups.TryGetValue(s, out Group group))
			{
				Charge = group.Charge;
				Stack1 = group.Stack1;
				Stack2 = group.Stack2;
				Stack3 = group.Stack3;
				return;
			}
			char c = s[s.Length - 1];
			if (c == '⁺')
			{
				Charge = 1;
				goto a;
			}
			if (c == '⁻')
			{
				Charge = -1;
				goto a;
			}
			goto b;
			a:
			c = s[s.Length - 2];
			switch (c)
			{
				case '²': Charge *= 2; break;
				case '³': Charge *= 3; break;
				default:
					if ('⁴' <= c && c <= '⁹')
						Charge *= c - '⁰';
					break;
			}
			b:
			var type = typeof(AtomKind);
			for (int i = 0; i < s.Length;)
			{
				if (char.IsUpper(s[i]))
				{
					int count;
					if (i + 1 < s.Length)
					{
						c = s[i + 1];
						count = char.IsLower(c) ? 2 : s[i] == 'U' && c == '2' && i < s.Length - 3 ? 4 : 1;
					}
					else count = 1;
					var atom = (AtomKind)Enum.Parse(type, s.Substring(i, count), false);
					i = Stack.ParseCount(s, out count, i + count);
					Add(new Stack(atom, count));
				}
				else i++;
			}
			Groups.Add(s, this);
		}

		public static Group Get(string s)
		{
			return Groups.TryGetValue(s, out Group group) ? group : new Group(s);
		}

		public void Add(Stack stack)
		{
			if (Stack1.Count == 0)
			{
				Stack1 = stack;
				return;
			}
			if (Stack2.Count == 0)
			{
				Stack2 = stack;
				return;
			}
			if (Stack3.Count == 0)
			{
				Stack3 = stack;
				return;
			}
			//throw new InvalidOperationException("Stack full");
		}

		/*public float AtomicWeight
		{
			get
			{
				float weight = 0;
				for (int i = 0; i < Count; i++)
				{
					var stack = this[i];
					weight += Atom.AtomicWeights[(int)stack.Atom] * stack.Count;
				}
				return weight;
			}
		}

		public int GetAtomCount()
		{
			int count = 0;
			for (int i = 0; i < Count; i++)
				count += this[i].Count;
			return count;
		}

		public int GetAtomCount(AtomKind atom)
		{
			int count = 0;
			for (int i = 0; i < Count; i++)
				if (this[i].Atom == atom)
					count += this[i].Count;
			return count;
		}*/

		#region Operator overloads

		public override bool Equals(object obj)
		{
			return obj is Group g && Equals(g);
		}

		public bool Equals(Group other)
		{
			return Charge == other.Charge && Stack1.Equals(other.Stack1) && Stack2.Equals(other.Stack2) && Stack3.Equals(other.Stack3);
		}

		public override string ToString()
		{
			var sb = new StringBuilder();
			if (Stack1.Count != 0)
			{
				sb.Append(Stack1.ToString());
			}
			if (Stack2.Count != 0)
			{
				sb.Append(Stack2.ToString());
			}
			if (Stack3.Count != 0)
			{
				sb.Append(Stack3.ToString());
			}
			/*if (Charge != 0)
			{
				i = Math.Abs(Charge);
				if (i == 2 || i == 3)
					sb.Append('²');
				else if (i == 3)
					sb.Append('³');
				else if (i > 3)
					sb.Append((char)('⁰' + i));
				sb.Append(Charge < 0 ? '⁻' : '⁺');
			}*/
			return sb.ToString();
		}

		public override int GetHashCode()
		{
			return Charge.GetHashCode() << 27 | Stack1.GetHashCode() << 18 | Stack2.GetHashCode() << 9 | Stack3.GetHashCode();
		}

		public static Compound operator +(Group g1, Group g2)
		{
			if ((g1.Charge | g2.Charge) == 0)
				throw new InvalidOperationException();
			if (g1.Charge < g2.Charge)
			{
				Group t = g1;
				g1 = g2;
				g2 = t;
			}
			int c1 = g1.Charge, c2 = -g2.Charge, gcd = Utils.GCD[c1, c2];
			var result = new Compound();
			result.Add(g1, c2 / gcd);
			result.Add(g2, c1 / gcd);
			return result;
		}

		/*public static implicit operator Group(string s)
		{
			return new Group(s);
		}*/

		#endregion Operator overloads
	}
}

namespace Game
{
	public partial class Utils
	{
		public static int[,] GCD = {
			{0,1,2,3,4},
			{1,1,1,1,1},
			{2,1,2,1,2},
			{3,1,1,3,1},
			{4,1,2,1,4},
			{5,1,1,1,1},
			{6,1,2,3,2},
			{7,1,1,1,1} };
	}
}