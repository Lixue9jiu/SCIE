using System;
using System.IO;

namespace LibPixz.Markers
{
	internal static class Dri
	{
		public const int RestartMarkerPeriod = 8;
		//static string name = "DRI";

		public static void Read(BinaryReader reader, ImgInfo imgInfo)
		{
			//LogMarker(reader, name);

			reader.ReadBEUInt16();

			ushort restartInterval = reader.ReadBEUInt16();

			if (restartInterval == 0)
				throw new Exception("Invalid restart interval (0)");

			imgInfo.restartInterval = restartInterval;
			imgInfo.hasRestartMarkers = true;

			Log(reader, imgInfo.restartInterval);
		}

		private static void Log(BinaryReader reader, ushort restartInterval)
		{
			//Logger.WriteLine("Restart Marker Interval: " + restartInterval);
			//Logger.WriteLine();
		}
	}
}