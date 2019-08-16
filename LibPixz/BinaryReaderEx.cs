using System.IO;

namespace LibPixz
{
	/// <summary>
	/// Extensions for BinaryReader that enable reading big endian numbers,
	/// </summary>
	internal static class BinaryReaderEx
	{
		internal static ushort ReadBEUInt16(this BinaryReader reader)
		{
			byte upperByte = reader.ReadByte();
			byte lowerByte = reader.ReadByte();

			return (ushort)(upperByte << 8 | lowerByte);
		}
	}
}