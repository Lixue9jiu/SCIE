using Engine.Media;
using System;
using System.IO;

namespace LibPixz.Markers
{
	internal static class Sos
	{
		//static string name = "SOS";

		public static Image Read(BinaryReader reader, ImgInfo imgInfo)
		{
			//LogMarker(reader, name);

			if (imgInfo.numOfComponents != 1 && imgInfo.numOfComponents != 3)
				throw new Exception("Unsupported number of components (" +
					imgInfo.numOfComponents + ")");

			ushort length = reader.ReadBEUInt16();
			byte componentsInScan = reader.ReadByte();

			for (int i = 0; i < componentsInScan; i++)
			{
				byte componentId = (byte)(reader.ReadByte() - 1);
				byte huffmanTables = reader.ReadByte();

				byte acTable = (byte)(huffmanTables & 0xf);
				byte dcTable = (byte)(huffmanTables >> 4);

				imgInfo.components[componentId].dcHuffmanTable = dcTable;
				imgInfo.components[componentId].acHuffmanTable = acTable;
			}

			reader.ReadBytes(3); // "Unused" bytes

			return ImageDecoder.DecodeImage(reader, imgInfo);
		}
	}
}