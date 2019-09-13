using Engine;
using Engine.Media;
using LibPixz.Colorspaces;
using System.IO;

namespace LibPixz
{
	internal class ImageDecoder
	{
		private const int blkSize = ImgInfo.blockSize; // JPEG decoding block size is 8x8

		// Arrays used in different parts of the decoding process
		protected static float[,] blockP = new float[blkSize, blkSize];

		protected static short[,] coefDctQnt = new short[blkSize, blkSize];
		protected static float[,] coefDct = new float[blkSize, blkSize];

		internal static Image DecodeImage(BinaryReader reader, ImgInfo imgInfo)
		{
			// Used for reading those nasty variable length codes
			var bReader = new BitReader(reader);
			var img = new float[imgInfo.numOfComponents][,];

			imgInfo.deltaDc = new short[imgInfo.numOfComponents];

			for (int ch = 0; ch < imgInfo.numOfComponents; ch++)
				img[ch] = new float[imgInfo.height, imgInfo.width];

			// Determine the width of height of the MCU, based on the read subsampling
			// values from all channels (don't know if this is the correct way for all cases)
			// but it works for the most common ones
			var arr = imgInfo.components;
			var componentMax = arr[0];
			for (int i = 1; i < arr.Length; i++)
			{
				componentMax = new Markers.ComponentInfo()
				{
					samplingFactorX = (byte)MathUtils.Max(componentMax.samplingFactorX, arr[i].samplingFactorX),
					samplingFactorY = (byte)MathUtils.Max(componentMax.samplingFactorY, arr[i].samplingFactorY)
				};
			}

			int sizeMcuX = blkSize * componentMax.samplingFactorX;
			int sizeMcuY = blkSize * componentMax.samplingFactorY;

			int numMcusX = (imgInfo.width + sizeMcuX - 1) / sizeMcuX;
			int numMcusY = (imgInfo.height + sizeMcuY - 1) / sizeMcuY;

			for (int mcu = 0; mcu < numMcusX * numMcusY; mcu = NextMcuPos(imgInfo, bReader, mcu, numMcusX, numMcusY))
			{
				// X and Y coordinates of current MCU
				int mcuX = mcu % numMcusX;
				int mcuY = mcu / numMcusX;
				// Starting X and Y pixels of current MCU
				int ofsX = mcuX * sizeMcuX;
				int ofsY = mcuY * sizeMcuY;

				for (int ch = 0; ch < imgInfo.numOfComponents; ch++)
				{
					int scaleX = componentMax.samplingFactorX / imgInfo.components[ch].samplingFactorX;
					int scaleY = componentMax.samplingFactorY / imgInfo.components[ch].samplingFactorY;

					for (int sy = 0; sy < imgInfo.components[ch].samplingFactorY; sy++)
					{
						for (int sx = 0; sx < imgInfo.components[ch].samplingFactorX; sx++)
						{
							DecodeBlock(bReader, imgInfo, img[ch], ch, ofsX + blkSize * sx, ofsY + blkSize * sy,
								scaleX, scaleY);
						}
					}
				}

				if (bReader.PastEndOfFile) break;
			}

			Color2[,] imagen = MergeChannels(imgInfo, img);
			var bmp = new Image(imgInfo.width, imgInfo.height);

			bReader.StopReading();
			for (int y = 0; y < imgInfo.height; y++)
			{
				for (int x = 0; x < imgInfo.width; x++)
				{
					var c = imagen[y, x];
					var color = new Color(c.r, c.g, c.b, c.a);
					if (MathUtils.Min(c.r, c.g, c.b) < 176)
						bmp.SetPixel(x, y, color);
				}
			}

			return bmp;
		}

		internal static int NextMcuPos(ImgInfo imgInfo, BitReader bReader, int mcu, int numMcusX, int numMcusY)
		{
			// If we are expecting a restart marker, find it in the stream,
			// reset the DC prediction variables and calculate the new MCU position
			// otherwise, just increment the position by one
			if (imgInfo.hasRestartMarkers &&
			   (mcu % imgInfo.restartInterval) == imgInfo.restartInterval - 1 &&
			   (mcu < numMcusX * numMcusY - 1))
			{
				Pixz.Markers currRestMarker = bReader.SyncStreamToNextRestartMarker();

				if (currRestMarker == Pixz.Markers.Eoi) // If EOI marker was found
					return numMcusX * numMcusY; // Early terminate the decoding process

				int difference = currRestMarker - imgInfo.prevRestMarker;

				if (difference <= 0) difference += Markers.Dri.RestartMarkerPeriod;

				// For non corrupted images, difference should be always 1
				ResetDeltas(imgInfo);
				imgInfo.mcuStrip += difference;
				imgInfo.prevRestMarker = currRestMarker;

				return imgInfo.mcuStrip * imgInfo.restartInterval;
			}
			if (bReader.WasEoiFound())
			{
				return numMcusX * numMcusY;
			}
			return ++mcu;
		}

		/// <summary>
		/// Converts from their colorspace to RGB and combines all previously decoded channels
		/// </summary>
		/// <param name="imgInfo"></param>
		/// <param name="imgS"></param>
		/// <returns>A 2D array representing the color data from the image</returns>
		internal static Color2[,] MergeChannels(ImgInfo imgInfo, float[][,] imgS)
		{
			var img = new Color2[imgInfo.height, imgInfo.width];
			IColorspaceConverter converter;

			if (imgInfo.app14MarkerFound)
			{
				switch (imgInfo.colorMode)
				{
					case Markers.App14ColorMode.Unknown:
						converter = imgInfo.numOfComponents == 3 ? new Rgb() : (IColorspaceConverter)new YCbCr();
						break;
					case Markers.App14ColorMode.YCbCr:
						converter = new YCbCr();
						break;
					case Markers.App14ColorMode.YCCK:
						converter = new YCbCr();
						break;
					default:
						converter = new Rgb();
						break;
				}
			}
			else
			{
				converter = new YCbCr();
			}

			for (int y = 0; y < imgInfo.height; y++)
			{
				for (int x = 0; x < imgInfo.width; x++)
				{
					Info info;

					if (imgInfo.numOfComponents == 1) // Y
					{
						info.a = imgS[0][y, x];
						info.b = 0;
						info.c = 0;
					}
					else // YCbCr
					{
						info.a = imgS[0][y, x];
						info.b = imgS[1][y, x];
						info.c = imgS[2][y, x];
					}

					img[y, x] = converter.ConvertToRgb(info);
				}
			}

			return img;
		}

		internal static void DecodeBlock(BitReader bReader, ImgInfo imgInfo, float[,] img,
			int compIndex, int ofsX, int ofsY, int scaleX, int scaleY)
		{
			int quantIndex = imgInfo.components[compIndex].quantTableId;

			short[] coefZig = GetCoefficients(bReader, imgInfo, compIndex, blkSize * blkSize);
			FileOps.ZigZagToArray(coefZig, coefDctQnt, FileOps.tablasZigzag[blkSize], blkSize);
			ImgOps.Dequant(coefDctQnt, coefDct, imgInfo.quantTables[quantIndex].table, blkSize);
			ImgOps.Fidct(coefDct, blockP, blkSize, blkSize);

			ImgOps.ResizeAndInsertBlock(imgInfo, blockP, img, blkSize, blkSize, ofsX, ofsY, scaleX, scaleY);
		}

		internal static short[] GetCoefficients(BitReader bReader, ImgInfo imgInfo, int compIndex, int numCoefs)
		{
			var coefZig = new short[numCoefs];
			int acIndex = imgInfo.components[compIndex].acHuffmanTable;
			int dcIndex = imgInfo.components[compIndex].dcHuffmanTable;

			// DC coefficient
			uint runAmplitude = Huffman.ReadRunAmplitude(bReader, imgInfo.huffmanTables[0, dcIndex]);

			uint run;// = runAmplitude >> 4; // Upper nybble
			uint amplitude = runAmplitude & 0xf; // Lower nybble

			coefZig[0] = (short)(Huffman.ReadCoefValue(bReader, amplitude) + imgInfo.deltaDc[compIndex]);

			imgInfo.deltaDc[compIndex] = coefZig[0];

			// AC coefficients
			uint pos = 0;

			while (pos < blkSize * blkSize - 1)
			{
				runAmplitude = Huffman.ReadRunAmplitude(bReader, imgInfo.huffmanTables[1, acIndex]);

				// 0x00 is End of Block
				if (runAmplitude == 0x00) break;

				run = runAmplitude >> 4;
				amplitude = runAmplitude & 0xf;
				pos += run + 1;

				if (pos >= blkSize * blkSize) break;

				coefZig[pos] = Huffman.ReadCoefValue(bReader, amplitude);
			}

			return coefZig;
		}

		// If we found a restart marker, reset all DC prediction deltas, so we can
		// start anew and not depend on previous (possibly corrupted) data
		internal static void ResetDeltas(ImgInfo imgInfo)
		{
			for (int i = 0; i < imgInfo.numOfComponents; i++)
			{
				imgInfo.deltaDc[i] = 0;
			}
		}
	}
}