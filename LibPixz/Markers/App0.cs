using System.IO;

namespace LibPixz.Markers
{
	internal static class App0
	{
		//static string name = "APP0";

		public static void Read(BinaryReader reader, ImgInfo imgInfo)
		{
			//LogMarker(reader, name);

			ushort length = reader.ReadBEUInt16();
			//Logger.WriteLine("Length: " + length.ToString());

			reader.BaseStream.Seek(length - 2, SeekOrigin.Current);
		}
	}
}