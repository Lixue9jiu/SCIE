using Engine;
using Engine.Media;
using LibPixz.Markers;
using System.IO;

namespace LibPixz
{
	public partial class Pixz
	{
		public enum Markers
		{
			LiteralFF = 0x00,
			Soi = 0xd8,
			App0 = 0xe0,
			App14 = 0xee,
			Dqt = 0xdb,
			Sof0 = 0xc0, Sof2 = 0xc2,
			Dht = 0xc4,
			Rs0 = 0xd0, Rs1 = 0xd1, Rs2 = 0xd2, Rs3 = 0xd3,
			Rs4 = 0xd4, Rs5 = 0xd5, Rs6 = 0xd6, Rs7 = 0xd7,
			Sos = 0xda,
			Eoi = 0xd9,
			Dri = 0xdd
		}

		public static DynamicArray<Image> Decode(Stream stream)
		{
			var reader = new BinaryReader(stream);
			var images = new DynamicArray<Image>();

			stream.Seek(0, SeekOrigin.Begin);

			var imgInfo = new ImgInfo();

			for (long length = stream.Length; ; )
			{
				int markerId;
				do
				{
					if (stream.Position == length)
						goto end;
				} while (reader.ReadByte() != 0xff);

				markerId = reader.ReadByte();

				switch ((Markers)markerId)
				{
					case Markers.App0:
						App0.Read(reader, imgInfo);
						break;
					case Markers.App14:
						App14.Read(reader, imgInfo);
						break;
					case Markers.Dqt:
						Dqt.Read(reader, imgInfo);
						break;
					case Markers.Sof0:
						Sof0.Read(reader, imgInfo);
						break;
					case Markers.Sof2:
						Sof2.Read(reader, imgInfo);
						break;
					case Markers.Dht:
						Dht.Read(reader, imgInfo);
						break;
					case Markers.Sos:
						images.Add(Sos.Read(reader, imgInfo));
						break;
					case Markers.Soi:
						//Logger.Write("Start of Image " + image);
						//Logger.WriteLine(" at: " + (reader.BaseStream.Position - 2).ToString("X"));
						imgInfo = new ImgInfo
						{
							startOfImageFound = true
						};
						break;
					case Markers.Dri:
						Dri.Read(reader, imgInfo);
						break;
					case Markers.Eoi:
						//Logger.Write("End of Image " + image);
						//Logger.WriteLine(" at: " + (reader.BaseStream.Position - 2).ToString("X"));
						//eof = true;
						break;
					// Unknown markers, or markers used outside of their specified area
					default:
						Default.Read(reader, imgInfo, (Markers)markerId);
						break;
				}
			}
			end:
			reader.Close();
			return images;
		}
	}
}