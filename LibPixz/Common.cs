namespace LibPixz
{
	/*public enum ColorType
	{
		Lab,
		YCbCr,
		Otro
	}

	public enum EncodeType
	{
		Normal,
		Multithreaded,
		Fast
	}*/

	internal static class Common
	{
		internal static float Clamp(float num, float min, float max)
		{
			if (num < min)
				return min;
			else if (num > max)
				return max;

			return num;
		}

		/*internal static void Butterfly(float a, float b, ref float c, ref float d)
		{
			c = a + b;
			d = a - b;
		}

		internal static float[,] Transpose(float[][] bloque, int tamX, int tamY)
		{
			float[,] blTrns = new float[tamX, tamY];

			for (int y = 0; y < tamY; y++)
				for (int x = 0; x < tamX; x++)
					blTrns[x, y] = bloque[y][x];

			return blTrns;
		}*/

		internal static void Transpose(float[,] bloque, float[,] blTrns, int tamX, int tamY)
		{
			for (int y = 0; y < tamY; y++)
				for (int x = 0; x < tamX; x++)
					blTrns[x, y] = bloque[y, x];
		}

		/*internal static string FormatString(string label, object value, int margin)
		{
			return label + ": " + string.Format("{0," + margin + "}", value.ToString()) + " ";
		}

		internal static string FormatString(object value, int margin)
		{
			return string.Format("{0," + margin + "}", value.ToString()) + " ";
		}

		private static string ToBinary(Huffman.CodeInfo number)
		{
			string numStr = string.Empty;

			while (number.length > 0)
			{
				numStr = (number.code & 1) + numStr;
				number.code >>= 1;
				number.length--;
			}

			return numStr;
		}*/
	}
}