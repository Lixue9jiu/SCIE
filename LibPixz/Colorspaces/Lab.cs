using System;

namespace LibPixz.Colorspaces
{
	internal class Lab : IColorspaceConverter
	{
		protected struct D65
		{
			internal const float x = 0.95047f;
			internal const float y = 1.00f;
			internal const float z = 1.08883f;
		};

		protected static float[,] msRgbXyz =
		{
			{ 0.4124564f, 0.3575761f, 0.1804375f },
			{ 0.2126729f, 0.7151522f, 0.0721750f },
			{ 0.0193339f, 0.1191920f, 0.9503041f }
		};

		protected static float[,] mXyzsRgb =
		{
			{  3.2404542f, -1.5371385f, -0.4985314f },
			{ -0.9692660f,  1.8760108f,  0.0415560f },
			{  0.0556434f, -0.2040259f,  1.0572252f }
		};

		private static float[] lutPow;
		private static byte[] lutIPow;
		private static float eps = 216f / 24389f;
		private static float ka = 24389f / 27f;

		public Color2 ConvertToRgb(Info info)
		{
			info = LabToXyz(info);
			return XyzToRgb(info);
		}

		public Info ConvertFromRgb(Color2 rgb)
		{
			Info info = RgbToXyz(rgb);
			return XyzToLab(info);
		}

		protected internal Lab()
		{
			lutPow = new float[256];
			lutIPow = new byte[256]; // Nota: Muy poca precisión para abarcar todos los colores

			for (int i = 0; i < 256; i++)
			{
				lutPow[i] = (float)Math.Pow(i / 255.0, 2.2);
				lutIPow[i] = (byte)(Math.Pow(i / 255.0, 1 / 2.2) * 255.0);
			}
		}

		internal Info RgbToXyz(Color2 color)
		{
			Info rgb;
			Info xyz;

			// Valores rgb lineales [0, 1]
			//rgb.a = (float)Math.Pow(color.R / 255.0, 2.2);
			//rgb.b = (float)Math.Pow(color.G / 255.0, 2.2);
			//rgb.c = (float)Math.Pow(color.B / 255.0, 2.2);

			rgb.a = lutPow[color.r];
			rgb.b = lutPow[color.g];
			rgb.c = lutPow[color.b];

			// Valores xyz
			xyz.a = rgb.a * msRgbXyz[0, 0] + rgb.b * msRgbXyz[0, 1] + rgb.c * msRgbXyz[0, 2];
			xyz.b = rgb.a * msRgbXyz[1, 0] + rgb.b * msRgbXyz[1, 1] + rgb.c * msRgbXyz[1, 2];
			xyz.c = rgb.a * msRgbXyz[2, 0] + rgb.b * msRgbXyz[2, 1] + rgb.c * msRgbXyz[2, 2];

			return xyz;
		}

		internal Color2 XyzToRgb(Info xyz)
		{
			float r, g, b;
			int ir, ig, ib;

			r = xyz.a * mXyzsRgb[0, 0] + xyz.b * mXyzsRgb[0, 1] + xyz.c * mXyzsRgb[0, 2];
			g = xyz.a * mXyzsRgb[1, 0] + xyz.b * mXyzsRgb[1, 1] + xyz.c * mXyzsRgb[1, 2];
			b = xyz.a * mXyzsRgb[2, 0] + xyz.b * mXyzsRgb[2, 1] + xyz.c * mXyzsRgb[2, 2];

			ir = GetRgbColor(r);
			ig = GetRgbColor(g);
			ib = GetRgbColor(b);

			return new Color2() { a = 255, r = (byte)ir, g = (byte)ig, b = (byte)ib };
		}

		internal Info XyzToLab(Info xyz, bool clamp = false)
		{
			Info lab;

			// Usando D65
			float fx = NoSeQueHace(xyz.a / D65.x);
			float fy = NoSeQueHace(xyz.b / D65.y);
			float fz = NoSeQueHace(xyz.c / D65.z);

			lab.a = (116f * fy - 16f) * 2.55f - 128f;
			lab.b = 500f * (fx - fy);
			lab.c = 200f * (fy - fz);

			if (clamp)
			{
				lab.a = Common.Clamp(lab.a, -128f, 127f);
				lab.b = Common.Clamp(lab.b, -128f, 127f);
				lab.c = Common.Clamp(lab.c, -128f, 127f);
			}

			return lab;
		}

		internal Info LabToXyz(Info lab)
		{
			Info xyz;

			float fy = (((lab.a + 128f) / 2.55f) + 16f) / 116f;
			float fx = lab.b / 500f + fy;
			float fz = fy - lab.c / 200f;

			xyz.a = CreoQueSeQueHace(fx) * D65.x;
			xyz.b = CreoQueSeQueHace(fy) * D65.y;
			xyz.c = CreoQueSeQueHace(fz) * D65.z;

			return xyz;
		}

		protected float NoSeQueHace(float val)
		{
			if (val > eps)
				return (float)Math.Pow(val, 1 / 3.0);
			else
				return (ka * val + 16) / 116;
		}

		protected float CreoQueSeQueHace(float val)
		{
			float valC = (float)Math.Pow(val, 3);

			if (valC > eps)
				return valC;
			else
				return (116 * val - 16) / ka;
		}

		protected byte GetRgbColor(float col)
		{
			//int iCol = (int)(Common.Clamp(col, 0, 1) * 255);

			return (byte)Common.Clamp((float)(Math.Pow(col, 1 / 2.2) * 255.0), 0, 255);
			//return (byte)lutIPow[iCol];
		}
	}
}