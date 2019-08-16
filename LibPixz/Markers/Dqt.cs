using System;
using System.IO;

namespace LibPixz.Markers
{
	public struct QuantTable
	{
		public bool valid;
		public byte id;
		public ushort length;
		public byte precision;
		public ushort[] table;
	}

	internal class Dqt : Marker
	{
		//static string name = "DQT";

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
			byte tableId = (byte)(tableInfo & 0xf); // Low 4 bits of tableInfo

			if (tableId > 3)
				throw new Exception("Invalid ID for quantization table");

			var quantTable = new QuantTable();

			quantTable.id = tableId;
			quantTable.precision = (byte)(tableInfo >> 4); // High 4 bits of tableInfo
			quantTable.valid = true;
			quantTable.table = new ushort[64];

			int sizeOfElement = quantTable.precision == 0 ? 1 : 2;

			var tabla = FileOps.tablasZigzag[8];

			// Quantizer tables are in zigzag too!
			if (quantTable.precision == 0)
			{
				for (int i = 0; i < 64; i++)
				{
					quantTable.table[tabla[i].Y * 8 + tabla[i].X] = reader.ReadByte();
				}
			}
			else
			{
				for (int i = 0; i < 64; i++)
				{
					quantTable.table[tabla[i].Y * 8 + tabla[i].X] = reader.ReadBEUInt16();
				}
			}

			imgInfo.quantTables[tableId] = quantTable;

			//Log(reader, quantTable);

			return 1 + 64 * sizeOfElement;
		}

		/*static void Log(BinaryReader reader, QuantTable quantTable)
        {
            Logger.WriteLine("Table ID: " + quantTable.id);
            Logger.WriteLine("Precision: " + quantTable.precision);
            Logger.WriteLine("The table itself");
            Common.PrintTable(quantTable.table, 8, 8, 4);
            Logger.WriteLine();
        }*/
	}
}