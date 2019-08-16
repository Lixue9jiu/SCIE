using System;
using System.Collections.Generic;
using System.IO;

namespace LibPixz.Markers
{
	internal struct HuffmanTable
	{
		public bool valid;
		public byte id;
		public byte type;
		public byte[] numSymbols;
		public byte[] codes;

		// Helper variables for image decoding
		public byte maxCodeLength;

		public List<Huffman.CodeInfo> table;
		public Huffman.CodeInfo[] preIndexTable;
	}

	internal class Dht : Marker
	{
		//static string name = "DHT";

		public static void Read(BinaryReader reader, ImgInfo imgInfo)
		{
			//LogMarker(reader, name);
			int markerLength = reader.ReadBEUInt16() - 2;

			while (markerLength > 0)
			{
				int length = ReadTable(reader, imgInfo);
				markerLength -= length;
			}
		}

		public static int ReadTable(BinaryReader reader, ImgInfo imgInfo)
		{
			byte tableInfo = reader.ReadByte();
			byte tableId = (byte)(tableInfo & 0x7); // Low 3 bits of tableInfo
			int numCodes = 0;

			if (tableId > 3)
				throw new Exception("Invalid ID for huffman table");

			if ((tableInfo & 0xe0) != 0)  // High 3 bits of tableInfo must be zero
				throw new Exception("Invalid huffman table");

			var huffmanTable = new HuffmanTable();

			huffmanTable.id = tableId;
			huffmanTable.type = (byte)((tableInfo >> 4) & 0x1); // Bit 4 of tableInfo
			huffmanTable.valid = true;
			huffmanTable.numSymbols = new byte[16];

			for (int i = 0; i < 16; i++)
			{
				huffmanTable.numSymbols[i] = reader.ReadByte();
				numCodes += huffmanTable.numSymbols[i];
			}

			huffmanTable.codes = new byte[numCodes];

			for (int i = 0; i < numCodes; i++)
			{
				huffmanTable.codes[i] = reader.ReadByte();
			}

			Huffman.CreateTable(ref huffmanTable);

			imgInfo.huffmanTables[huffmanTable.type, huffmanTable.id] = huffmanTable;
			return 1 + 16 + numCodes;
		}
	}
}