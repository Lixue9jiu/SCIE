using System;
using System.IO;

namespace LibPixz.Markers
{
	public struct ComponentInfo
	{
		public byte id;
		public byte samplingFactorX;
		public byte samplingFactorY;
		public byte quantTableId;
		public byte dcHuffmanTable;
		public byte acHuffmanTable;
	}

	internal class Sof0 : Marker
	{
		//static string name = "SOF0";

		public static void Read(BinaryReader reader, ImgInfo imgInfo)
		{
			//LogMarker(reader, name);

			imgInfo.length = reader.ReadBEUInt16();
			imgInfo.dataPrecision = reader.ReadByte();
			imgInfo.height = reader.ReadBEUInt16();
			imgInfo.width = reader.ReadBEUInt16();
			imgInfo.numOfComponents = reader.ReadByte();

			if (imgInfo.length < 8)
				throw new Exception("Invalid length of Sof0");

			if (imgInfo.height == 0 || imgInfo.width == 0)
				throw new Exception("Invalid image size");

			if (imgInfo.dataPrecision != 8)
				throw new Exception("Unsupported data precision");

			if (imgInfo.numOfComponents != 1 && imgInfo.numOfComponents != 3)
				throw new Exception("Invalid number of components");

			imgInfo.components = new ComponentInfo[imgInfo.numOfComponents];

			for (int i = 0; i < imgInfo.numOfComponents; i++)
			{
				byte id = reader.ReadByte();

				if (id > 3)
					throw new Exception("Invalid component type");

				byte samplingFactor = reader.ReadByte();

				imgInfo.components[i].id = id;
				imgInfo.components[i].samplingFactorX = (byte)(samplingFactor >> 4);
				imgInfo.components[i].samplingFactorY = (byte)(samplingFactor & 0x0f);

				imgInfo.components[i].quantTableId = reader.ReadByte();
			}

			//Log(reader, imgInfo);
		}

		/*static void Log(BinaryReader reader, ImgInfo imgInfo)
        {
            Logger.WriteLine("Length: " + imgInfo.length);
            Logger.WriteLine("Width: " + imgInfo.width + " Height: " + imgInfo.height);

            Logger.WriteLine("Number of components: " + imgInfo.numOfComponents);
            Logger.WriteLine("Data precision: " + imgInfo.dataPrecision);
            Logger.WriteLine();

            for (int i = 0; i < imgInfo.numOfComponents; i++)
            {
                Logger.WriteLine("Component " + i);
                Logger.WriteLine("ID: " + imgInfo.components[i].id);
                Logger.WriteLine("Sampling Factor X: " + imgInfo.components[i].samplingFactorX);
                Logger.WriteLine("Sampling Factor Y: " + imgInfo.components[i].samplingFactorY);
            }

            Logger.WriteLine();
        }*/
	}
}