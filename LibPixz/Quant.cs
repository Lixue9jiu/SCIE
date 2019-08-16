namespace LibPixz
{
	internal partial class ImgOps
	{
		/*protected internal static short[,] Quant(float[,] pixDct, ushort[] matriz, int tam)
		{
			short[,] pixQnt = new short[tam, tam];

			for (int y = 0; y < tam; y++)
			{
				for (int x = 0; x < tam; x++)
				{
					float val = (float)Math.Round(pixDct[y, x] / matriz[y * tam + x]);

					pixQnt[y, x] = (short)Common.Clamp(val, short.MinValue, short.MaxValue);
				}
			}

			return pixQnt;
		}*/

		protected internal static void Dequant(short[,] pixQnt, float[,] coefDct, ushort[] matriz, int tam)
		{
			for (int y = 0; y < tam; y++)
			{
				for (int x = 0; x < tam; x++)
				{
					coefDct[y, x] = (float)(pixQnt[y, x] * matriz[y * tam + x]);
				}
			}
		}
	}
}