namespace LibPixz.Colorspaces
{
	public class YCbCr : IColorspaceConverter
	{
		protected static float[,] mRgbYcbcr =
		{
			{  0.299f,   0.587f,   0.114f  },
			{ -0.1687f, -0.3313f,  0.5f    },
			{  0.5f,    -0.4187f, -0.0813f }
		};

		protected static float[,] mYcbcrRgb =
		{
			{  1f,  0f,        1.402f   },
			{  1f, -0.34414f, -0.71414f },
			{  1f,  1.772f,    0f       }
		};

		public Color2 ConvertToRgb(Info info)
		{
			byte r, g, b;
			float y = info.a + 128;
			float cb = info.b;
			float cr = info.c;

			r = (byte)Common.Clamp(y + cr * mYcbcrRgb[0, 2], 0, 255);
			g = (byte)Common.Clamp(y + cb * mYcbcrRgb[1, 1] + cr * mYcbcrRgb[1, 2], 0, 255);
			b = (byte)Common.Clamp(y + cb * mYcbcrRgb[2, 1], 0, 255);

			return new Color2() { a = 255, r = r, g = g, b = b };
		}

		public Info ConvertFromRgb(Color2 rgb)
		{
			Info yCbCr;

			// Valores YCbCr
			yCbCr.a = rgb.r * mRgbYcbcr[0, 0] + rgb.g * mRgbYcbcr[0, 1] + rgb.b * mRgbYcbcr[0, 2] - 128;
			yCbCr.b = rgb.r * mRgbYcbcr[1, 0] + rgb.g * mRgbYcbcr[1, 1] + rgb.b * mRgbYcbcr[1, 2];
			yCbCr.c = rgb.r * mRgbYcbcr[2, 0] + rgb.g * mRgbYcbcr[2, 1] + rgb.b * mRgbYcbcr[2, 2];

			//if (ycbcr.a > 255f || ycbcr.b > 255f || ycbcr.c > 255f)
			//    Console.WriteLine("Valor ycbcr desbordado");

			return yCbCr;
		}
	}
}