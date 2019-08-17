using System;

namespace LibPixz
{
	internal partial class ImgOps
	{
		private const int blkSize = ImgInfo.blockSize;

		private static float[,] coefYU = new float[blkSize, blkSize];
		private static float[,] coefUY = new float[blkSize, blkSize];
		private static float[,] coefUV = new float[blkSize, blkSize];

		private static ArraySlice<float> coefYUS = new ArraySlice<float>(coefYU);
		private static ArraySlice<float> coefUYS = new ArraySlice<float>(coefUY);
		private static ArraySlice<float> coefUVS = new ArraySlice<float>(coefUV);

		private static float[,] tCosXU = GetTablaICos(ImgInfo.blockSize);
		private static float[,] tCosYV = GetTablaICos(ImgInfo.blockSize);

		/*static Dictionary<int, int> bitsPorNum = new Dictionary<int, int>()
        {
            {1, 0}, {2, 1}, {4, 2}, {8, 3}, {16, 4}, {32, 5}, {64, 6}
        };*/

		private static int[] bitsPorNum = { 0, 0, 1, 0, 2, 0, 0, 0,
									3, 0, 0, 0, 0, 0, 0, 0,
									4, 0, 0, 0, 0, 0, 0, 0,
									0, 0, 0, 0, 0, 0, 0, 0,
									5, 0, 0, 0, 0, 0, 0, 0,
									0, 0, 0, 0, 0, 0, 0, 0,
									0, 0, 0, 0, 0, 0, 0, 0,
									0, 0, 0, 0, 0, 0, 0, 0,
									6
								  };

		protected static float[,] GetTablaICos(int tam)
		{
			float[,] tablaCosUX = new float[tam, tam];

			for (int u = 0; u < tam; u++)
			{
				for (int x = 0; x < tam; x++)
				{
					tablaCosUX[u, x] = (float)Math.Cos((2 * x + 1) * u * Math.PI / (2 * tam));
					tablaCosUX[u, x] *= (float)Math.Sqrt(2.0 / tam);
					if (u == 0) tablaCosUX[u, x] /= (float)Math.Sqrt(2f);
				}
			}

			return tablaCosUX;
		}

		private static void Ifct8(ArraySlice<float> bloque, ArraySlice<float> res, float[,] tIcos)
		{
			float dc = bloque[0] * tIcos[0, 0];

			float suma02 = 0;
			suma02 += bloque[4] * tIcos[4, 0];

			float suma01 = 0;
			suma01 += bloque[2] * tIcos[2, 0];
			suma01 += bloque[6] * tIcos[6, 0];

			float suma11 = 0;
			suma11 += bloque[2] * tIcos[2, 1];
			suma11 += bloque[6] * tIcos[6, 1];

			float suma00 = 0;
			suma00 += bloque[1] * tIcos[1, 0];
			suma00 += bloque[3] * tIcos[3, 0];
			suma00 += bloque[5] * tIcos[5, 0];
			suma00 += bloque[7] * tIcos[7, 0];

			float suma10 = 0;
			suma10 += bloque[1] * tIcos[1, 1];
			suma10 += bloque[3] * tIcos[3, 1];
			suma10 += bloque[5] * tIcos[5, 1];
			suma10 += bloque[7] * tIcos[7, 1];

			float suma20 = 0;
			suma20 += bloque[1] * tIcos[1, 2];
			suma20 += bloque[3] * tIcos[3, 2];
			suma20 += bloque[5] * tIcos[5, 2];
			suma20 += bloque[7] * tIcos[7, 2];

			float suma30 = 0;
			suma30 += bloque[1] * tIcos[1, 3];
			suma30 += bloque[3] * tIcos[3, 3];
			suma30 += bloque[5] * tIcos[5, 3];
			suma30 += bloque[7] * tIcos[7, 3];

			float p00 = dc + suma02;
			float p01 = dc - suma02;

			float p10 = p00 + suma01;
			float p11 = p00 - suma01;
			float p12 = p01 + suma11;
			float p13 = p01 - suma11;

			res[0] = p10 + suma00;
			res[7] = p10 - suma00;
			res[3] = p11 + suma30;
			res[4] = p11 - suma30;
			res[1] = p12 + suma10;
			res[6] = p12 - suma10;
			res[2] = p13 + suma20;
			res[5] = p13 - suma20;
		}

		protected internal static void Fidct(float[,] bloque, float[,] bloqueDct, int tamX, int tamY)
		{
			var bloqueS = new ArraySlice<float>(bloque);

			// Sacamos el IDCT de cada fila del bloque
			for (int y = 0; y < tamY; y++)
			{
				Ifct8(bloqueS.GetSlice(y), coefYUS.GetSlice(y), tCosXU);
			}

			Common.Transpose(coefYU, coefUY, tamX, tamY);

			// Ahora sacamos el DCT por columna de los resultados anteriores
			for (int u = 0; u < tamX; u++)
			{
				Ifct8(coefUYS.GetSlice(u), coefUVS.GetSlice(u), tCosYV);
			}

			for (int v = 0; v < tamY; v++)
			{
				for (int u = 0; u < tamX; u++)
				{
					bloqueDct[v, u] = (float)Math.Round(coefUV[u, v]);
				}
			}
		}

		protected internal static void Idct(float[,] bloque, float[,] bloqueDct, int tamX, int tamY)
		{
			int u, v, x, y;
			float suma, suma2;

			for (v = 0; v < tamY; v++)
			{
				for (u = 0; u < tamX; u++)
				{
					for (y = 0, suma2 = 0; y < tamY; y++)
					{
						for (x = 0, suma = 0; x < tamX; x++)
						{
							suma += bloque[y, x] * tCosXU[x, u];
						}

						suma2 += suma * tCosYV[y, v];
					}
					bloqueDct[v, u] = (float)Math.Round(suma2);
				}
			}
		}

		protected internal static void MostrarBordes(float[,] coefDct, int tam)
		{
			for (int y = 0; y < tam; y++)
				coefDct[y, tam - 1] = 96f;

			for (int x = 0; x < tam; x++)
				coefDct[tam - 1, x] = 96f;
		}

		internal static void ResizeAndInsertBlock(ImgInfo imgInfo, float[,] block, float[,] imagen, int tamX, int tamY, int ofsX, int ofsY, int scaleX, int scaleY)
		{
			// Nearest neighbor interpolation
			// For staircase FTW
			if (ofsX < imgInfo.width && ofsY < imgInfo.height)
			{
				for (int j = 0; j < tamY; j++)
				{
					for (int i = 0; i < tamX; i++)
					{
						for (int jj = 0; jj < scaleY; jj++)
						{
							for (int ii = 0; ii < scaleX; ii++)
							{
								int posYimg = j * scaleY + ofsY + jj;
								int posXimg = i * scaleX + ofsX + ii;

								if (posYimg < imgInfo.height && posXimg < imgInfo.width)
								{
									imagen[posYimg, posXimg] = block[j, i];
								}
							}
						}
					}
				}
			}
		}
	}
}