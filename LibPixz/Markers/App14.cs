using System;
using System.IO;

namespace LibPixz.Markers
{
	public enum App14ColorMode
	{
		Unknown,
		YCbCr,
		YCCK
	}

	internal class App14 : Marker
	{
		//static string name = "APP14";

		public static void Read(BinaryReader reader, ImgInfo imgInfo)
		{
			//LogMarker(reader, name);
			ushort markerLength = reader.ReadBEUInt16();

			reader.ReadBytes(11);

			imgInfo.app14MarkerFound = true;
			imgInfo.colorMode = (App14ColorMode)reader.ReadByte();

			if ((int)imgInfo.colorMode > 2)
				throw new Exception("Invalid Adobe colorspace");

			//Logger.WriteLine("Transform Flag: " + imgInfo.colorMode);
			//Logger.WriteLine();
		}
	}
}