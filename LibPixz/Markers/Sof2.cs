﻿using System.IO;

namespace LibPixz.Markers
{
	internal class Sof2 : Marker
	{
		//static string name = "SOF2";

		public static void Read(BinaryReader reader, ImgInfo imgInfo)
		{
			/*LogMarker(reader, name);
            Logger.WriteLine("Progressive images are not supported yet");
            Logger.WriteLine();*/
		}
	}
}