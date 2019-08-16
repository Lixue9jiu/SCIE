using System;
using System.Collections.Generic;
using Point = Engine.Point2;

namespace LibPixz
{
	public class FileOps
	{
		protected internal static Dictionary<int, Point[]> tablasZigzag = new Dictionary<int, Point[]>()
		{
			{ 8, GetZigzagTable(8, 8) },
			{ 16, GetZigzagTable(16, 16) },
			{ 32, GetZigzagTable(32, 32) },
			{ 64, GetZigzagTable(64, 64) }
		};

		protected internal static Point[] GetZigzagTable(int width, int height)
		{
			if (width <= 0 || height <= 0)
				throw new Exception("Block dimensions can't be less than zero");

			var tabla = new Point[height * width];
			int x = 0, y = 0;
			int pos = 0;

			tabla[pos++] = new Point(x, y);

			while (pos < height * width)
			{
				if (x == width - 1)
					tabla[pos++] = new Point(x, ++y);
				else
					tabla[pos++] = new Point(++x, y);

				if (pos == height * width) break;

				while (x > 0 && y < height - 1)
					tabla[pos++] = new Point(--x, ++y);

				if (y == height - 1)
					tabla[pos++] = new Point(++x, y);
				else
					tabla[pos++] = new Point(x, ++y);

				if (pos == height * width) break;

				while (y > 0 && x < width - 1)
					tabla[pos++] = new Point(++x, --y);
			}

			return tabla;
		}

		/*protected internal static short[] ArrayToZigZag(short[,] coefDct, Point[] order, int size)
		{
			int numElem = size * size;
			short[] coefZig = new short[numElem];

			for (int i = 0; i < numElem; i++)
			{
				coefZig[i] = coefDct[order[i].Y, order[i].X];
			}

			return coefZig;
		}*/

		protected internal static void ZigZagToArray(short[] coefZig, short[,] coefDct, Point[] order, int size)
		{
			int numElem = size * size;

			for (int i = 0; i < numElem; i++)
			{
				coefDct[order[i].Y, order[i].X] = coefZig[i];
			}
		}
	}
}