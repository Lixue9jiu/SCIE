using System;
using System.IO;

namespace LibPixz
{
	/// <summary>
	/// Restart marker-aware bit reader for JPEG decoding
	/// </summary>
	internal class BitReader
	{
		private const int dataSize = sizeof(ushort) * 8;
		private const int readerSize = sizeof(byte) * 8;

		private BinaryReader reader;
		private Pixz.Markers lastReadMarker;
		private uint readData;
		private int availableBits;
		private bool dataPad;
		private bool lockReading;

		/// <summary>
		/// Returns true if end of file has been reached and reader is padding with zeros
		/// </summary>
		internal bool PastEndOfFile
		{
			get { return dataPad && availableBits <= 0; }
		}

		/// <summary>
		/// Returns the BinaryReader the BitReader is using
		/// </summary>
		internal BinaryReader BaseBinaryReader
		{
			get { return reader; }
		}

		internal BitReader(BinaryReader reader)
		{
			Flush();
			dataPad = false;

			this.reader = reader;
		}

		/// <summary>
		/// Peeks a certain number of bits from a certain stream
		/// </summary>
		/// <param name="length">The number of bits we want to read from the stream</param>
		/// <returns>An unsigned 2 byte integer containing the requested bits, with enough leading
		/// zeros to pad the result, and trailing zeros to fill the requested length if past the
		/// end of stream
		/// </returns>
		internal ushort Peek(uint length)
		{
			if (length > dataSize) throw new Exception("Reading too many bits");

			// If we don't have as many bits as needed, read another chunk from stream
			if (length > availableBits)
			{
				byte nextChunk = 0;

				try
				{
					while (availableBits <= length)
					{
						nextChunk = ReadByteOrMarker();

						if (lockReading) break;

						availableBits += readerSize;
						readData = (readData << (int)readerSize) | nextChunk;
					}
				}
				// If already at the end of stream, use only the remaining bits we have read
				// before. Because the number of requested bits is less than the available bits,
				// the result of the clean data has the number of missing bits as zeros appended to
				// the right, as the huffman decoding phase needs a fixed number of bits to work
				catch (EndOfStreamException)
				{
					dataPad = true;
				}
			}

			// We move data left and right in order to get only the bits we require
			uint cleanData = readData << (int)(dataSize * 2 - availableBits);
			cleanData >>= (int)(dataSize * 2 - length);

			return (ushort)cleanData;
		}

		/// <summary>
		/// Reads a certain number of bits from a certain stream
		/// </summary>
		/// <param name="length">The number of bits we want to read from the stream</param>
		/// <returns>An unsigned 2 byte integer containing the requested bits, with enough leading
		/// zeros to pad the result, and trailing zeros to fill the requested length if past the
		/// end of stream
		/// </returns>
		internal ushort Read(uint length)
		{
			if (length > dataSize) throw new Exception("Reading too many bits");

			ushort data = Peek(length);

			availableBits -= (int)length;

			int shift = (int)(dataSize * 2 - availableBits);
			// We move data to the left and then right in order to get only the bits we require
			readData <<= shift;
			readData >>= shift;

			return data;
		}

		/// <summary>
		/// Flush all data in the buffer and rewinds all readahead bytes
		/// </summary>
		internal void StopReading()
		{
			if (!dataPad)
			{
				// Rewind all (whole) bytes we didn't use (including 2 from eof marker)
				// ToDo: check if this is correct
				int rewind = ((availableBits + readerSize - 1) / readerSize) + 2;
				reader.BaseStream.Seek(-rewind, SeekOrigin.Current);
			}

			Flush();
		}

		/// <summary>
		/// Deletes all data in the buffer, without rewinding all readahead bytes
		/// in stream
		/// </summary>
		protected void Flush()
		{
			availableBits = 0;
			readData = 0;
			lastReadMarker = 0;
			lockReading = false;
		}

		/// <summary>
		/// Read a byte from the stream, taking into account when markers are found
		/// If we find a marker, lock the stream at that current position
		/// and return zeros on the next reads so we can at least decode part of
		/// the image (Happens when the file is corrupted)
		/// </summary>
		/// <returns>A byte read from the current stream</returns>
		protected byte ReadByteOrMarker()
		{
			if (!lockReading)
			{
				byte number = reader.ReadByte();

				if (number == 0xff) // Marker found
				{
					byte markerValue = reader.ReadByte();

					if (markerValue == 0x00) // 0xff00 is interpreted as a 0xff value
					{
						return number;
					}
					else
					{
						lastReadMarker = (Pixz.Markers)markerValue;
						lockReading = true;

						return 0;
					}
				}
				else // Not a marker, just return read value
				{
					return number;
				}
			}
			else
			{
				return 0;
			}
		}

		/// <summary>
		/// Finds the next restart marker, or the EOI marker in the stream
		/// </summary>
		/// <returns>The next restart marker</returns>
		internal Pixz.Markers SyncStreamToNextRestartMarker()
		{
			// When decoding actual image pixel data, ignore all markers except
			// restart markers, or EOI marker
			while (!((lastReadMarker >= Pixz.Markers.Rs0 &&
					  lastReadMarker <= Pixz.Markers.Rs7) ||
					  lastReadMarker == Pixz.Markers.Eoi))
			{
				ReadByteOrMarker();
			}

			Pixz.Markers tempMarker = lastReadMarker;
			Flush();

			return tempMarker;
		}

		internal bool WasEoiFound()
		{
			if (lastReadMarker == Pixz.Markers.Eoi && availableBits <= 0)
			{
				Flush();
				return true;
			}

			return false;
		}
	}
}