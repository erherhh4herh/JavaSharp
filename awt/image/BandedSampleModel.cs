/*
 * Copyright (c) 1997, 2013, Oracle and/or its affiliates. All rights reserved.
 * ORACLE PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 *
 *
 *
 *
 *
 *
 *
 *
 *
 *
 *
 *
 *
 *
 *
 *
 *
 *
 *
 *
 */

/* ****************************************************************
 ******************************************************************
 ******************************************************************
 *** COPYRIGHT (c) Eastman Kodak Company, 1997
 *** As  an unpublished  work pursuant to Title 17 of the United
 *** States Code.  All rights reserved.
 ******************************************************************
 ******************************************************************
 ******************************************************************/

namespace java.awt.image
{

	/// <summary>
	///  This class represents image data which is stored in a band interleaved
	///  fashion and for
	///  which each sample of a pixel occupies one data element of the DataBuffer.
	///  It subclasses ComponentSampleModel but provides a more efficient
	///  implementation for accessing band interleaved image data than is provided
	///  by ComponentSampleModel.  This class should typically be used when working
	///  with images which store sample data for each band in a different bank of the
	///  DataBuffer. Accessor methods are provided so that image data can be
	///  manipulated directly. Pixel stride is the number of
	///  data array elements between two samples for the same band on the same
	///  scanline. The pixel stride for a BandedSampleModel is one.
	///  Scanline stride is the number of data array elements between
	///  a given sample and the corresponding sample in the same column of the next
	///  scanline.  Band offsets denote the number
	///  of data array elements from the first data array element of the bank
	///  of the DataBuffer holding each band to the first sample of the band.
	///  The bands are numbered from 0 to N-1.
	///  Bank indices denote the correspondence between a bank of the data buffer
	///  and a band of image data.  This class supports
	///  <seealso cref="DataBuffer#TYPE_BYTE TYPE_BYTE"/>,
	///  <seealso cref="DataBuffer#TYPE_USHORT TYPE_USHORT"/>,
	///  <seealso cref="DataBuffer#TYPE_SHORT TYPE_SHORT"/>,
	///  <seealso cref="DataBuffer#TYPE_INT TYPE_INT"/>,
	///  <seealso cref="DataBuffer#TYPE_FLOAT TYPE_FLOAT"/>, and
	///  <seealso cref="DataBuffer#TYPE_DOUBLE TYPE_DOUBLE"/> datatypes
	/// </summary>


	public sealed class BandedSampleModel : ComponentSampleModel
	{

		/// <summary>
		/// Constructs a BandedSampleModel with the specified parameters.
		/// The pixel stride will be one data element.  The scanline stride
		/// will be the same as the width.  Each band will be stored in
		/// a separate bank and all band offsets will be zero. </summary>
		/// <param name="dataType">  The data type for storing samples. </param>
		/// <param name="w">         The width (in pixels) of the region of
		///                  image data described. </param>
		/// <param name="h">         The height (in pixels) of the region of image
		///                  data described. </param>
		/// <param name="numBands">  The number of bands for the image data. </param>
		/// <exception cref="IllegalArgumentException"> if <code>dataType</code> is not
		///         one of the supported data types </exception>
		public BandedSampleModel(int dataType, int w, int h, int numBands) : base(dataType, w, h, 1, w, BandedSampleModel.CreateIndicesArray(numBands), BandedSampleModel.CreateOffsetArray(numBands))
		{
		}

		/// <summary>
		/// Constructs a BandedSampleModel with the specified parameters.
		/// The number of bands will be inferred from the lengths of the
		/// bandOffsets bankIndices arrays, which must be equal.  The pixel
		/// stride will be one data element. </summary>
		/// <param name="dataType">  The data type for storing samples. </param>
		/// <param name="w">         The width (in pixels) of the region of
		///                  image data described. </param>
		/// <param name="h">         The height (in pixels) of the region of
		///                  image data described. </param>
		/// <param name="scanlineStride"> The line stride of the of the image data. </param>
		/// <param name="bankIndices"> The bank index for each band. </param>
		/// <param name="bandOffsets"> The band offset for each band. </param>
		/// <exception cref="IllegalArgumentException"> if <code>dataType</code> is not
		///         one of the supported data types </exception>
		public BandedSampleModel(int dataType, int w, int h, int scanlineStride, int[] bankIndices, int[] bandOffsets) : base(dataType, w, h, 1,scanlineStride, bankIndices, bandOffsets)
		{

		}

		/// <summary>
		/// Creates a new BandedSampleModel with the specified
		/// width and height.  The new BandedSampleModel will have the same
		/// number of bands, storage data type, and bank indices
		/// as this BandedSampleModel.  The band offsets will be compressed
		/// such that the offset between bands will be w*pixelStride and
		/// the minimum of all of the band offsets is zero. </summary>
		/// <param name="w"> the width of the resulting <code>BandedSampleModel</code> </param>
		/// <param name="h"> the height of the resulting <code>BandedSampleModel</code> </param>
		/// <returns> a new <code>BandedSampleModel</code> with the specified
		///         width and height. </returns>
		/// <exception cref="IllegalArgumentException"> if <code>w</code> or
		///         <code>h</code> equals either
		///         <code>Integer.MAX_VALUE</code> or
		///         <code>Integer.MIN_VALUE</code> </exception>
		/// <exception cref="IllegalArgumentException"> if <code>dataType</code> is not
		///         one of the supported data types </exception>
		public override SampleModel CreateCompatibleSampleModel(int w, int h)
		{
			int[] bandOffs;

			if (NumBanks == 1)
			{
				bandOffs = OrderBands(BandOffsets_Renamed, w * h);
			}
			else
			{
				bandOffs = new int[BandOffsets_Renamed.Length];
			}

			SampleModel sampleModel = new BandedSampleModel(DataType_Renamed, w, h, w, BankIndices_Renamed, bandOffs);
			return sampleModel;
		}

		/// <summary>
		/// Creates a new BandedSampleModel with a subset of the bands of this
		/// BandedSampleModel.  The new BandedSampleModel can be
		/// used with any DataBuffer that the existing BandedSampleModel
		/// can be used with.  The new BandedSampleModel/DataBuffer
		/// combination will represent an image with a subset of the bands
		/// of the original BandedSampleModel/DataBuffer combination. </summary>
		/// <exception cref="RasterFormatException"> if the number of bands is greater than
		///                               the number of banks in this sample model. </exception>
		/// <exception cref="IllegalArgumentException"> if <code>dataType</code> is not
		///         one of the supported data types </exception>
		public override SampleModel CreateSubsetSampleModel(int[] bands)
		{
			if (bands.Length > BankIndices_Renamed.Length)
			{
				throw new RasterFormatException("There are only " + BankIndices_Renamed.Length + " bands");
			}
			int[] newBankIndices = new int[bands.Length];
			int[] newBandOffsets = new int[bands.Length];

			for (int i = 0; i < bands.Length; i++)
			{
				newBankIndices[i] = BankIndices_Renamed[bands[i]];
				newBandOffsets[i] = BandOffsets_Renamed[bands[i]];
			}

			return new BandedSampleModel(this.DataType_Renamed, Width_Renamed, Height_Renamed, this.ScanlineStride_Renamed, newBankIndices, newBandOffsets);
		}

		/// <summary>
		/// Creates a DataBuffer that corresponds to this BandedSampleModel,
		/// The DataBuffer's data type, number of banks, and size
		/// will be consistent with this BandedSampleModel. </summary>
		/// <exception cref="IllegalArgumentException"> if <code>dataType</code> is not
		///         one of the supported types. </exception>
		public override DataBuffer CreateDataBuffer()
		{
			DataBuffer dataBuffer = null;

			int size = ScanlineStride_Renamed * Height_Renamed;
			switch (DataType_Renamed)
			{
			case DataBuffer.TYPE_BYTE:
				dataBuffer = new DataBufferByte(size, NumBanks);
				break;
			case DataBuffer.TYPE_USHORT:
				dataBuffer = new DataBufferUShort(size, NumBanks);
				break;
			case DataBuffer.TYPE_SHORT:
				dataBuffer = new DataBufferShort(size, NumBanks);
				break;
			case DataBuffer.TYPE_INT:
				dataBuffer = new DataBufferInt(size, NumBanks);
				break;
			case DataBuffer.TYPE_FLOAT:
				dataBuffer = new DataBufferFloat(size, NumBanks);
				break;
			case DataBuffer.TYPE_DOUBLE:
				dataBuffer = new DataBufferDouble(size, NumBanks);
				break;
			default:
				throw new IllegalArgumentException("dataType is not one " + "of the supported types.");
			}

			return dataBuffer;
		}


		/// <summary>
		/// Returns data for a single pixel in a primitive array of type
		/// TransferType.  For a BandedSampleModel, this will be the same
		/// as the data type, and samples will be returned one per array
		/// element.  Generally, obj
		/// should be passed in as null, so that the Object will be created
		/// automatically and will be of the right primitive data type.
		/// <para>
		/// The following code illustrates transferring data for one pixel from
		/// DataBuffer <code>db1</code>, whose storage layout is described by
		/// BandedSampleModel <code>bsm1</code>, to DataBuffer <code>db2</code>,
		/// whose storage layout is described by
		/// BandedSampleModel <code>bsm2</code>.
		/// The transfer will generally be more efficient than using
		/// getPixel/setPixel.
		/// <pre>
		///       BandedSampleModel bsm1, bsm2;
		///       DataBufferInt db1, db2;
		///       bsm2.setDataElements(x, y, bsm1.getDataElements(x, y, null, db1),
		///                            db2);
		/// </pre>
		/// Using getDataElements/setDataElements to transfer between two
		/// DataBuffer/SampleModel pairs is legitimate if the SampleModels have
		/// the same number of bands, corresponding bands have the same number of
		/// bits per sample, and the TransferTypes are the same.
		/// </para>
		/// <para>
		/// If obj is non-null, it should be a primitive array of type TransferType.
		/// Otherwise, a ClassCastException is thrown.  An
		/// ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		/// not in bounds, or if obj is non-null and is not large enough to hold
		/// the pixel data.
		/// </para>
		/// </summary>
		/// <param name="x">         The X coordinate of the pixel location </param>
		/// <param name="y">         The Y coordinate of the pixel location </param>
		/// <param name="obj">       If non-null, a primitive array in which to return
		///                  the pixel data. </param>
		/// <param name="data">      The DataBuffer containing the image data. </param>
		/// <returns> the data for the specified pixel. </returns>
		/// <seealso cref= #setDataElements(int, int, Object, DataBuffer) </seealso>
		public override Object GetDataElements(int x, int y, Object obj, DataBuffer data)
		{
			if ((x < 0) || (y < 0) || (x >= Width_Renamed) || (y >= Height_Renamed))
			{
				throw new ArrayIndexOutOfBoundsException("Coordinate out of bounds!");
			}
			int type = TransferType;
			int numDataElems = NumDataElements;
			int pixelOffset = y * ScanlineStride_Renamed + x;

			switch (type)
			{

			case DataBuffer.TYPE_BYTE:

				sbyte[] bdata;

				if (obj == null)
				{
					bdata = new sbyte[numDataElems];
				}
				else
				{
					bdata = (sbyte[])obj;
				}

				for (int i = 0; i < numDataElems; i++)
				{
					bdata[i] = (sbyte)data.GetElem(BankIndices_Renamed[i], pixelOffset + BandOffsets_Renamed[i]);
				}

				obj = (Object)bdata;
				break;

			case DataBuffer.TYPE_USHORT:
			case DataBuffer.TYPE_SHORT:

				short[] sdata;

				if (obj == null)
				{
					sdata = new short[numDataElems];
				}
				else
				{
					sdata = (short[])obj;
				}

				for (int i = 0; i < numDataElems; i++)
				{
					sdata[i] = (short)data.GetElem(BankIndices_Renamed[i], pixelOffset + BandOffsets_Renamed[i]);
				}

				obj = (Object)sdata;
				break;

			case DataBuffer.TYPE_INT:

				int[] idata;

				if (obj == null)
				{
					idata = new int[numDataElems];
				}
				else
				{
					idata = (int[])obj;
				}

				for (int i = 0; i < numDataElems; i++)
				{
					idata[i] = data.GetElem(BankIndices_Renamed[i], pixelOffset + BandOffsets_Renamed[i]);
				}

				obj = (Object)idata;
				break;

			case DataBuffer.TYPE_FLOAT:

				float[] fdata;

				if (obj == null)
				{
					fdata = new float[numDataElems];
				}
				else
				{
					fdata = (float[])obj;
				}

				for (int i = 0; i < numDataElems; i++)
				{
					fdata[i] = data.GetElemFloat(BankIndices_Renamed[i], pixelOffset + BandOffsets_Renamed[i]);
				}

				obj = (Object)fdata;
				break;

			case DataBuffer.TYPE_DOUBLE:

				double[] ddata;

				if (obj == null)
				{
					ddata = new double[numDataElems];
				}
				else
				{
					ddata = (double[])obj;
				}

				for (int i = 0; i < numDataElems; i++)
				{
					ddata[i] = data.GetElemDouble(BankIndices_Renamed[i], pixelOffset + BandOffsets_Renamed[i]);
				}

				obj = (Object)ddata;
				break;
			}

			return obj;
		}

		/// <summary>
		/// Returns all samples for the specified pixel in an int array.
		/// ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		/// not in bounds. </summary>
		/// <param name="x">         The X coordinate of the pixel location </param>
		/// <param name="y">         The Y coordinate of the pixel location </param>
		/// <param name="iArray">    If non-null, returns the samples in this array </param>
		/// <param name="data">      The DataBuffer containing the image data </param>
		/// <returns> the samples for the specified pixel. </returns>
		/// <seealso cref= #setPixel(int, int, int[], DataBuffer) </seealso>
		public override int[] GetPixel(int x, int y, int[] iArray, DataBuffer data)
		{
			if ((x < 0) || (y < 0) || (x >= Width_Renamed) || (y >= Height_Renamed))
			{
				throw new ArrayIndexOutOfBoundsException("Coordinate out of bounds!");
			}

			int[] pixels;

			if (iArray != null)
			{
			   pixels = iArray;
			}
			else
			{
			   pixels = new int [NumBands];
			}

			int pixelOffset = y * ScanlineStride_Renamed + x;
			for (int i = 0; i < NumBands; i++)
			{
				pixels[i] = data.GetElem(BankIndices_Renamed[i], pixelOffset + BandOffsets_Renamed[i]);
			}
			return pixels;
		}

		/// <summary>
		/// Returns all samples for the specified rectangle of pixels in
		/// an int array, one sample per data array element.
		/// ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		/// not in bounds. </summary>
		/// <param name="x">         The X coordinate of the upper left pixel location </param>
		/// <param name="y">         The Y coordinate of the upper left pixel location </param>
		/// <param name="w">         The width of the pixel rectangle </param>
		/// <param name="h">         The height of the pixel rectangle </param>
		/// <param name="iArray">    If non-null, returns the samples in this array </param>
		/// <param name="data">      The DataBuffer containing the image data </param>
		/// <returns> the samples for the pixels within the specified region. </returns>
		/// <seealso cref= #setPixels(int, int, int, int, int[], DataBuffer) </seealso>
		public override int[] GetPixels(int x, int y, int w, int h, int[] iArray, DataBuffer data)
		{
			int x1 = x + w;
			int y1 = y + h;

			if (x < 0 || x >= Width_Renamed || w > Width_Renamed || x1 < 0 || x1 > Width_Renamed || y < 0 || y >= Height_Renamed || h > Height_Renamed || y1 < 0 || y1 > Height_Renamed)
			{
				throw new ArrayIndexOutOfBoundsException("Coordinate out of bounds!");
			}
			int[] pixels;

			if (iArray != null)
			{
			   pixels = iArray;
			}
			else
			{
			   pixels = new int[w * h * NumBands];
			}

			for (int k = 0; k < NumBands; k++)
			{
				int lineOffset = y * ScanlineStride_Renamed + x + BandOffsets_Renamed[k];
				int srcOffset = k;
				int bank = BankIndices_Renamed[k];

				for (int i = 0; i < h; i++)
				{
					int pixelOffset = lineOffset;
					for (int j = 0; j < w; j++)
					{
						pixels[srcOffset] = data.GetElem(bank, pixelOffset++);
						srcOffset += NumBands;
					}
					lineOffset += ScanlineStride_Renamed;
				}
			}
			return pixels;
		}

		/// <summary>
		/// Returns as int the sample in a specified band for the pixel
		/// located at (x,y).
		/// ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		/// not in bounds. </summary>
		/// <param name="x">         The X coordinate of the pixel location </param>
		/// <param name="y">         The Y coordinate of the pixel location </param>
		/// <param name="b">         The band to return </param>
		/// <param name="data">      The DataBuffer containing the image data </param>
		/// <returns> the sample in the specified band for the specified pixel. </returns>
		/// <seealso cref= #setSample(int, int, int, int, DataBuffer) </seealso>
		public override int GetSample(int x, int y, int b, DataBuffer data)
		{
			// Bounds check for 'b' will be performed automatically
			if ((x < 0) || (y < 0) || (x >= Width_Renamed) || (y >= Height_Renamed))
			{
				throw new ArrayIndexOutOfBoundsException("Coordinate out of bounds!");
			}
			int sample = data.GetElem(BankIndices_Renamed[b], y * ScanlineStride_Renamed + x + BandOffsets_Renamed[b]);
			return sample;
		}

		/// <summary>
		/// Returns the sample in a specified band
		/// for the pixel located at (x,y) as a float.
		/// ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		/// not in bounds. </summary>
		/// <param name="x">         The X coordinate of the pixel location </param>
		/// <param name="y">         The Y coordinate of the pixel location </param>
		/// <param name="b">         The band to return </param>
		/// <param name="data">      The DataBuffer containing the image data </param>
		/// <returns> a float value that represents the sample in the specified
		/// band for the specified pixel. </returns>
		public override float GetSampleFloat(int x, int y, int b, DataBuffer data)
		{
			// Bounds check for 'b' will be performed automatically
			if ((x < 0) || (y < 0) || (x >= Width_Renamed) || (y >= Height_Renamed))
			{
				throw new ArrayIndexOutOfBoundsException("Coordinate out of bounds!");
			}

			float sample = data.GetElemFloat(BankIndices_Renamed[b], y * ScanlineStride_Renamed + x + BandOffsets_Renamed[b]);
			return sample;
		}

		/// <summary>
		/// Returns the sample in a specified band
		/// for a pixel located at (x,y) as a double.
		/// ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		/// not in bounds. </summary>
		/// <param name="x">         The X coordinate of the pixel location </param>
		/// <param name="y">         The Y coordinate of the pixel location </param>
		/// <param name="b">         The band to return </param>
		/// <param name="data">      The DataBuffer containing the image data </param>
		/// <returns> a double value that represents the sample in the specified
		/// band for the specified pixel. </returns>
		public override double GetSampleDouble(int x, int y, int b, DataBuffer data)
		{
			// Bounds check for 'b' will be performed automatically
			if ((x < 0) || (y < 0) || (x >= Width_Renamed) || (y >= Height_Renamed))
			{
				throw new ArrayIndexOutOfBoundsException("Coordinate out of bounds!");
			}

			double sample = data.GetElemDouble(BankIndices_Renamed[b], y * ScanlineStride_Renamed + x + BandOffsets_Renamed[b]);
			return sample;
		}

		/// <summary>
		/// Returns the samples in a specified band for the specified rectangle
		/// of pixels in an int array, one sample per data array element.
		/// ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		/// not in bounds. </summary>
		/// <param name="x">         The X coordinate of the upper left pixel location </param>
		/// <param name="y">         The Y coordinate of the upper left pixel location </param>
		/// <param name="w">         The width of the pixel rectangle </param>
		/// <param name="h">         The height of the pixel rectangle </param>
		/// <param name="b">         The band to return </param>
		/// <param name="iArray">    If non-null, returns the samples in this array </param>
		/// <param name="data">      The DataBuffer containing the image data </param>
		/// <returns> the samples in the specified band for the pixels within
		/// the specified region. </returns>
		/// <seealso cref= #setSamples(int, int, int, int, int, int[], DataBuffer) </seealso>
		public override int[] GetSamples(int x, int y, int w, int h, int b, int[] iArray, DataBuffer data)
		{
			// Bounds check for 'b' will be performed automatically
			if ((x < 0) || (y < 0) || (x + w > Width_Renamed) || (y + h > Height_Renamed))
			{
				throw new ArrayIndexOutOfBoundsException("Coordinate out of bounds!");
			}
			int[] samples;
			if (iArray != null)
			{
			   samples = iArray;
			}
			else
			{
			   samples = new int [w * h];
			}

			int lineOffset = y * ScanlineStride_Renamed + x + BandOffsets_Renamed[b];
			int srcOffset = 0;
			int bank = BankIndices_Renamed[b];

			for (int i = 0; i < h; i++)
			{
			   int sampleOffset = lineOffset;
			   for (int j = 0; j < w; j++)
			   {
				   samples[srcOffset++] = data.GetElem(bank, sampleOffset++);
			   }
			   lineOffset += ScanlineStride_Renamed;
			}
			return samples;
		}

		/// <summary>
		/// Sets the data for a single pixel in the specified DataBuffer from a
		/// primitive array of type TransferType.  For a BandedSampleModel,
		/// this will be the same as the data type, and samples are transferred
		/// one per array element.
		/// <para>
		/// The following code illustrates transferring data for one pixel from
		/// DataBuffer <code>db1</code>, whose storage layout is described by
		/// BandedSampleModel <code>bsm1</code>, to DataBuffer <code>db2</code>,
		/// whose storage layout is described by
		/// BandedSampleModel <code>bsm2</code>.
		/// The transfer will generally be more efficient than using
		/// getPixel/setPixel.
		/// <pre>
		///       BandedSampleModel bsm1, bsm2;
		///       DataBufferInt db1, db2;
		///       bsm2.setDataElements(x, y, bsm1.getDataElements(x, y, null, db1),
		///                            db2);
		/// </pre>
		/// Using getDataElements/setDataElements to transfer between two
		/// DataBuffer/SampleModel pairs is legitimate if the SampleModels have
		/// the same number of bands, corresponding bands have the same number of
		/// bits per sample, and the TransferTypes are the same.
		/// </para>
		/// <para>
		/// obj must be a primitive array of type TransferType.  Otherwise,
		/// a ClassCastException is thrown.  An
		/// ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		/// not in bounds, or if obj is not large enough to hold the pixel data.
		/// </para>
		/// </summary>
		/// <param name="x">         The X coordinate of the pixel location </param>
		/// <param name="y">         The Y coordinate of the pixel location </param>
		/// <param name="obj">       If non-null, returns the primitive array in this
		///                  object </param>
		/// <param name="data">      The DataBuffer containing the image data </param>
		/// <seealso cref= #getDataElements(int, int, Object, DataBuffer) </seealso>
		public override void SetDataElements(int x, int y, Object obj, DataBuffer data)
		{
			if ((x < 0) || (y < 0) || (x >= Width_Renamed) || (y >= Height_Renamed))
			{
				throw new ArrayIndexOutOfBoundsException("Coordinate out of bounds!");
			}
			int type = TransferType;
			int numDataElems = NumDataElements;
			int pixelOffset = y * ScanlineStride_Renamed + x;

			switch (type)
			{

			case DataBuffer.TYPE_BYTE:

				sbyte[] barray = (sbyte[])obj;

				for (int i = 0; i < numDataElems; i++)
				{
					data.SetElem(BankIndices_Renamed[i], pixelOffset + BandOffsets_Renamed[i], barray[i] & 0xff);
				}
				break;

			case DataBuffer.TYPE_USHORT:
			case DataBuffer.TYPE_SHORT:

				short[] sarray = (short[])obj;

				for (int i = 0; i < numDataElems; i++)
				{
					data.SetElem(BankIndices_Renamed[i], pixelOffset + BandOffsets_Renamed[i], sarray[i] & 0xffff);
				}
				break;

			case DataBuffer.TYPE_INT:

				int[] iarray = (int[])obj;

				for (int i = 0; i < numDataElems; i++)
				{
					data.SetElem(BankIndices_Renamed[i], pixelOffset + BandOffsets_Renamed[i], iarray[i]);
				}
				break;

			case DataBuffer.TYPE_FLOAT:

				float[] farray = (float[])obj;

				for (int i = 0; i < numDataElems; i++)
				{
					data.SetElemFloat(BankIndices_Renamed[i], pixelOffset + BandOffsets_Renamed[i], farray[i]);
				}
				break;

			case DataBuffer.TYPE_DOUBLE:

				double[] darray = (double[])obj;

				for (int i = 0; i < numDataElems; i++)
				{
					data.SetElemDouble(BankIndices_Renamed[i], pixelOffset + BandOffsets_Renamed[i], darray[i]);
				}
				break;

			}
		}

		/// <summary>
		/// Sets a pixel in the DataBuffer using an int array of samples for input.
		/// ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		/// not in bounds. </summary>
		/// <param name="x">         The X coordinate of the pixel location </param>
		/// <param name="y">         The Y coordinate of the pixel location </param>
		/// <param name="iArray">    The input samples in an int array </param>
		/// <param name="data">      The DataBuffer containing the image data </param>
		/// <seealso cref= #getPixel(int, int, int[], DataBuffer) </seealso>
		public override void SetPixel(int x, int y, int[] iArray, DataBuffer data)
		{
			if ((x < 0) || (y < 0) || (x >= Width_Renamed) || (y >= Height_Renamed))
			{
				throw new ArrayIndexOutOfBoundsException("Coordinate out of bounds!");
			}
		   int pixelOffset = y * ScanlineStride_Renamed + x;
		   for (int i = 0; i < NumBands; i++)
		   {
			   data.SetElem(BankIndices_Renamed[i], pixelOffset + BandOffsets_Renamed[i], iArray[i]);
		   }
		}

		/// <summary>
		/// Sets all samples for a rectangle of pixels from an int array containing
		/// one sample per array element.
		/// ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		/// not in bounds. </summary>
		/// <param name="x">         The X coordinate of the upper left pixel location </param>
		/// <param name="y">         The Y coordinate of the upper left pixel location </param>
		/// <param name="w">         The width of the pixel rectangle </param>
		/// <param name="h">         The height of the pixel rectangle </param>
		/// <param name="iArray">    The input samples in an int array </param>
		/// <param name="data">      The DataBuffer containing the image data </param>
		/// <seealso cref= #getPixels(int, int, int, int, int[], DataBuffer) </seealso>
		public override void SetPixels(int x, int y, int w, int h, int[] iArray, DataBuffer data)
		{
			int x1 = x + w;
			int y1 = y + h;

			if (x < 0 || x >= Width_Renamed || w > Width_Renamed || x1 < 0 || x1 > Width_Renamed || y < 0 || y >= Height_Renamed || h > Height_Renamed || y1 < 0 || y1 > Height_Renamed)
			{
				throw new ArrayIndexOutOfBoundsException("Coordinate out of bounds!");
			}

			for (int k = 0; k < NumBands; k++)
			{
				int lineOffset = y * ScanlineStride_Renamed + x + BandOffsets_Renamed[k];
				int srcOffset = k;
				int bank = BankIndices_Renamed[k];

				for (int i = 0; i < h; i++)
				{
					int pixelOffset = lineOffset;
					for (int j = 0; j < w; j++)
					{
						data.SetElem(bank, pixelOffset++, iArray[srcOffset]);
						srcOffset += NumBands;
					}
					lineOffset += ScanlineStride_Renamed;
				}
			}
		}

		/// <summary>
		/// Sets a sample in the specified band for the pixel located at (x,y)
		/// in the DataBuffer using an int for input.
		/// ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		/// not in bounds. </summary>
		/// <param name="x">         The X coordinate of the pixel location </param>
		/// <param name="y">         The Y coordinate of the pixel location </param>
		/// <param name="b">         The band to set </param>
		/// <param name="s">         The input sample as an int </param>
		/// <param name="data">      The DataBuffer containing the image data </param>
		/// <seealso cref= #getSample(int, int, int, DataBuffer) </seealso>
		public override void SetSample(int x, int y, int b, int s, DataBuffer data)
		{
			// Bounds check for 'b' will be performed automatically
			if ((x < 0) || (y < 0) || (x >= Width_Renamed) || (y >= Height_Renamed))
			{
				throw new ArrayIndexOutOfBoundsException("Coordinate out of bounds!");
			}
			data.SetElem(BankIndices_Renamed[b], y * ScanlineStride_Renamed + x + BandOffsets_Renamed[b], s);
		}

		/// <summary>
		/// Sets a sample in the specified band for the pixel located at (x,y)
		/// in the DataBuffer using a float for input.
		/// ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		/// not in bounds. </summary>
		/// <param name="x">         The X coordinate of the pixel location </param>
		/// <param name="y">         The Y coordinate of the pixel location </param>
		/// <param name="b">         The band to set </param>
		/// <param name="s">         The input sample as a float </param>
		/// <param name="data">      The DataBuffer containing the image data </param>
		/// <seealso cref= #getSample(int, int, int, DataBuffer) </seealso>
		public override void SetSample(int x, int y, int b, float s, DataBuffer data)
		{
			// Bounds check for 'b' will be performed automatically
			if ((x < 0) || (y < 0) || (x >= Width_Renamed) || (y >= Height_Renamed))
			{
				throw new ArrayIndexOutOfBoundsException("Coordinate out of bounds!");
			}
			data.SetElemFloat(BankIndices_Renamed[b], y * ScanlineStride_Renamed + x + BandOffsets_Renamed[b], s);
		}

		/// <summary>
		/// Sets a sample in the specified band for the pixel located at (x,y)
		/// in the DataBuffer using a double for input.
		/// ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		/// not in bounds. </summary>
		/// <param name="x">         The X coordinate of the pixel location </param>
		/// <param name="y">         The Y coordinate of the pixel location </param>
		/// <param name="b">         The band to set </param>
		/// <param name="s">         The input sample as a double </param>
		/// <param name="data">      The DataBuffer containing the image data </param>
		/// <seealso cref= #getSample(int, int, int, DataBuffer) </seealso>
		public override void SetSample(int x, int y, int b, double s, DataBuffer data)
		{
			// Bounds check for 'b' will be performed automatically
			if ((x < 0) || (y < 0) || (x >= Width_Renamed) || (y >= Height_Renamed))
			{
				throw new ArrayIndexOutOfBoundsException("Coordinate out of bounds!");
			}
			data.SetElemDouble(BankIndices_Renamed[b], y * ScanlineStride_Renamed + x + BandOffsets_Renamed[b], s);
		}

		/// <summary>
		/// Sets the samples in the specified band for the specified rectangle
		/// of pixels from an int array containing one sample per data array element.
		/// ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		/// not in bounds. </summary>
		/// <param name="x">         The X coordinate of the upper left pixel location </param>
		/// <param name="y">         The Y coordinate of the upper left pixel location </param>
		/// <param name="w">         The width of the pixel rectangle </param>
		/// <param name="h">         The height of the pixel rectangle </param>
		/// <param name="b">         The band to set </param>
		/// <param name="iArray">    The input sample array </param>
		/// <param name="data">      The DataBuffer containing the image data </param>
		/// <seealso cref= #getSamples(int, int, int, int, int, int[], DataBuffer) </seealso>
		public override void SetSamples(int x, int y, int w, int h, int b, int[] iArray, DataBuffer data)
		{
			// Bounds check for 'b' will be performed automatically
			if ((x < 0) || (y < 0) || (x + w > Width_Renamed) || (y + h > Height_Renamed))
			{
				throw new ArrayIndexOutOfBoundsException("Coordinate out of bounds!");
			}
			int lineOffset = y * ScanlineStride_Renamed + x + BandOffsets_Renamed[b];
			int srcOffset = 0;
			int bank = BankIndices_Renamed[b];

			for (int i = 0; i < h; i++)
			{
			   int sampleOffset = lineOffset;
			   for (int j = 0; j < w; j++)
			   {
				  data.SetElem(bank, sampleOffset++, iArray[srcOffset++]);
			   }
			   lineOffset += ScanlineStride_Renamed;
			}
		}

		private static int[] CreateOffsetArray(int numBands)
		{
			int[] bandOffsets = new int[numBands];
			for (int i = 0; i < numBands; i++)
			{
				bandOffsets[i] = 0;
			}
			return bandOffsets;
		}

		private static int[] CreateIndicesArray(int numBands)
		{
			int[] bankIndices = new int[numBands];
			for (int i = 0; i < numBands; i++)
			{
				bankIndices[i] = i;
			}
			return bankIndices;
		}

		// Differentiate hash code from other ComponentSampleModel subclasses
		public override int HashCode()
		{
			return base.HashCode() ^ 0x2;
		}
	}

}