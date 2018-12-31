﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Chemistry
{
	[Serializable]
	public class Compound : Dictionary<Group, int>
	{
		public Compound()
		{
		}

		public Compound(int capacity) : base(capacity)
		{
		}

		public Compound(IDictionary<Group, int> dictionary) : base(dictionary)
		{
		}

		public Compound(string s)
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

		public bool Equals(Compound obj)
		{
			return Count == obj.Count;
		}

		public override int GetHashCode()
		{
			int code = -1;
			for (var i = GetEnumerator(); i.MoveNext();)
			{
				var pair = i.Current;
				code ^= pair.Key.GetHashCode() * 2111 | pair.Value << 24;
			}
			return code ^ Count << 28;
		}

		/*public static Compound operator +(Compound c1, Compound c2)
		{
			var result = new Compound(c1.Count + c2.Count);
			Enumerator i;
			for (i = c1.GetEnumerator(); i.MoveNext();)
			{
				result.Add(i.Current.Key, i.Current.Value);
			}
			for (i = c2.GetEnumerator(); i.MoveNext();)
			{
				var key = i.Current.Key;
				result.TryGetValue(key, out int value);
				result[key] = value + i.Current.Value;
			}
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
					{
						result[key] = value;
					}
					else
					{
						result.Remove(key);
					}
				}
			}
			return result;
		}

		public static implicit operator Compound(string s)
		{
			return new Compound(s);
		}*/

		#endregion

		public override string ToString()
		{
			var sb = new StringBuilder();
			for (var i = GetEnumerator(); i.MoveNext();)
			{
				var pair = i.Current;
				bool bracket = pair.Value > 1 && pair.Key.Count > 1;
				if (bracket)
				{
					sb.Append('(');
				}
				sb.Append(pair.Key.ToString());
				if (bracket)
				{
					sb.Append(')');
				}
				if (pair.Value > 1)
				{
					sb.Append(pair.Value);
				}
			}
			return sb.ToString();
		}
	}
}