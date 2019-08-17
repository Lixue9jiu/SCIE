using System.IO;

namespace LibPixz.Markers
{
	internal static class Default
	{
		public static void Read(BinaryReader reader, ImgInfo imgInfo, Pixz.Markers markerId)
		{
			/*Logger.Write("Unknown marker (" + markerId.ToString("X") + ")");

            if (!imgInfo.startOfImageFound)
            {
                Logger.Write(" found outside of image");
            }

            Logger.WriteLine(" at: " + (reader.BaseStream.Position - 2).ToString("X"));*/

			// Check if marker is not followed by a length argument
			if (markerId >= Pixz.Markers.Rs0 && markerId <= Pixz.Markers.Rs7)
				return;
			if (markerId == Pixz.Markers.LiteralFF)
				return;

			if (!imgInfo.startOfImageFound) return;

			ushort length = reader.ReadBEUInt16();
			//Logger.WriteLine("Length: " + length.ToString());

			reader.BaseStream.Seek(length - 2, SeekOrigin.Current);
		}
	}
}