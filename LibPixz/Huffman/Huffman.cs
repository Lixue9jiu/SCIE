using LibPixz.Markers;
using System;
using System.Collections.Generic;

namespace LibPixz
{
	internal class Huffman
	{
		internal struct CodeInfo
		{
			public ushort number;
			public uint code;
			public byte length;
		}

		internal static void CreateTable(ref HuffmanTable huffmanTable)
		{
			ConvertToCanonicalCode(ref huffmanTable);
			PreparePreindexedTables(ref huffmanTable);
		}

		internal static ushort ReadRunAmplitude(BitReader bReader, HuffmanTable table)
		{
			ushort code = bReader.Peek(table.maxCodeLength);

			CodeInfo currentCode = table.preIndexTable[code];
			bReader.Read(currentCode.length);

			return currentCode.number;
		}

		internal static short ReadCoefValue(BitReader bReader, uint size)
		{
			if (size == 0) return 0;

			ushort specialBits = bReader.Read(size);

			return SpecialBitsToValue(specialBits, (short)size);
		}

		internal static short SpecialBitsToValue(ushort number, short size)
		{
			int threshold = 1 << (size - 1);

			if (number < threshold)
			{
				return (short)(number - ((threshold << 1) - 1));
			}
			else
			{
				return (short)number;
			}
		}

		internal static void ConvertToCanonicalCode(ref HuffmanTable huffmanTable)
		{
			int newCode = -1; // Para compensar con el incremento inicial dentro del for debajo
			uint prevLength = 1;
			uint newCodeIndex = 0;
			huffmanTable.table = new List<CodeInfo>();

			for (uint i = 0; i < huffmanTable.numSymbols.Length; i++)
			{
				uint codeLength = i + 1;

				for (uint j = 0; j < huffmanTable.numSymbols[i]; j++)
				{
					var code = new CodeInfo();
					int difLengths = (int)(codeLength - prevLength);

					newCode = (newCode + 1) << difLengths;
					prevLength = codeLength;

					code.code = (uint)newCode;
					code.length = (byte)codeLength;
					code.number = (ushort)huffmanTable.codes[newCodeIndex++];

					huffmanTable.table.Add(code);
				}
			}

			huffmanTable.maxCodeLength = (byte)prevLength;
		}

		internal static void PreparePreindexedTables(ref HuffmanTable huffmanTable)
		{
			// Codigo de datos preindizados

			// En esta parte, se construye una tabla de a que simbolos corresponden a cada codigo
			// tomando en cuenta los bits posteriores del codigo siguiente, ya que se lee del stream
			// un numero fijo de bits, esta misma tabla nos va a decir despues cuantos bits se van a extraer
			// de la lectura hecha anteriormente
			huffmanTable.preIndexTable = new CodeInfo[1U << huffmanTable.maxCodeLength];

			foreach (var codeWord in huffmanTable.table)
			{
				int shift = huffmanTable.maxCodeLength - (int)codeWord.length;
				uint numCodes = 1U << shift;
				uint initialCode = codeWord.code << shift;

				for (uint nextCode = initialCode; nextCode < (numCodes + initialCode); nextCode++)
				{
					huffmanTable.preIndexTable[nextCode] = codeWord;
				}
			}
		}

		private int[] Test()
		{
			int size = 32;
			int count = 40;
			var freqTable = new int[size];

			var rnd = new Random((int)DateTime.UtcNow.Ticks);

			for (int i = 0; i < size; i++)
			{
				freqTable[i] += rnd.Next(count);
			}

			return freqTable;
		}
	}
}