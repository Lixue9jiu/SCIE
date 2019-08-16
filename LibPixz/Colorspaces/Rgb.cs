namespace LibPixz.Colorspaces
{
	internal class Rgb : IColorspaceConverter
	{
		public Color2 ConvertToRgb(Info info)
		{
			int r, g, b;

			r = (byte)Common.Clamp(info.a + 128, 0, 255);
			g = (byte)Common.Clamp(info.b + 128, 0, 255);
			b = (byte)Common.Clamp(info.c + 128, 0, 255);

			return new Color2() { a = 255, r = (byte)r, g = (byte)g, b = (byte)b };
		}

		public Info ConvertFromRgb(Color2 color)
		{
			return new Info() { a = color.r - 128, b = color.g - 128, c = color.b - 128 };
		}
	}
}