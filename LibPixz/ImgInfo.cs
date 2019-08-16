using LibPixz.Markers;

namespace LibPixz
{
	internal class ImgInfo
	{
		public ushort length;
		public byte dataPrecision;
		public ushort height;
		public ushort width;
		public bool hasRestartMarkers;
		public ushort restartInterval;
		public byte numOfComponents;
		public ComponentInfo[] components;

		public HuffmanTable[,] huffmanTables = new HuffmanTable[2, 4];
		public QuantTable[] quantTables = new QuantTable[4];

		public bool startOfImageFound;

		// For App14 Marker (Adobe)
		public bool app14MarkerFound;

		public App14ColorMode colorMode;

		// Helper image decoding variables
		public short[] deltaDc;

		//public short restartMarker;
		public int mcuStrip = 0;
		public Pixz.Markers prevRestMarker = Pixz.Markers.Rs7;

		// Constants
		public const int blockSize = 8;
	}
}