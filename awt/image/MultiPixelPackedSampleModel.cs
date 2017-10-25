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
	/// The <code>MultiPixelPackedSampleModel</code> class represents
	/// one-banded images and can pack multiple one-sample
	/// pixels into one data element.  Pixels are not allowed to span data elements.
	/// The data type can be DataBuffer.TYPE_BYTE, DataBuffer.TYPE_USHORT,
	/// or DataBuffer.TYPE_INT.  Each pixel must be a power of 2 number of bits
	/// and a power of 2 number of pixels must fit exactly in one data element.
	/// Pixel bit stride is equal to the number of bits per pixel.  Scanline
	/// stride is in data elements and the last several data elements might be
	/// padded with unused pixels.  Data bit offset is the offset in bits from
	/// the beginning of the <seealso cref="DataBuffer"/> to the first pixel and must be
	/// a multiple of pixel bit stride.
	/// <para>
	/// The following code illustrates extracting the bits for pixel
	/// <code>x,&nbsp;y</code> from <code>DataBuffer</code> <code>data</code>
	/// and storing the pixel data in data elements of type
	/// <code>dataType</code>:
	/// <pre>{@code
	///      int dataElementSize = DataBuffer.getDataTypeSize(dataType);
	///      int bitnum = dataBitOffset + x*pixelBitStride;
	///      int element = data.getElem(y*scanlineStride + bitnum/dataElementSize);
	///      int shift = dataElementSize - (bitnum & (dataElementSize-1))
	///                  - pixelBitStride;
	///      int pixel = (element >> shift) & ((1 << pixelBitStride) - 1);
	/// }</pre>
	/// </para>
	/// </summary>

	public class MultiPixelPackedSampleModel : SampleModel
	{
		/// <summary>
		/// The number of bits from one pixel to the next. </summary>
		internal int PixelBitStride_Renamed;

		/// <summary>
		/// Bitmask that extracts the rightmost pixel of a data element. </summary>
		internal int BitMask;

		/// <summary>
		/// The number of pixels that fit in a data element.  Also used
		/// as the number of bits per pixel.
		/// </summary>
		internal int PixelsPerDataElement;

		/// <summary>
		/// The size of a data element in bits. </summary>
		internal int DataElementSize;

		/// <summary>
		/// The bit offset into the data array where the first pixel begins.
		/// </summary>
		internal int DataBitOffset_Renamed;

		/// <summary>
		/// ScanlineStride of the data buffer described in data array elements. </summary>
		internal int ScanlineStride_Renamed;

		/// <summary>
		/// Constructs a <code>MultiPixelPackedSampleModel</code> with the
		/// specified data type, width, height and number of bits per pixel. </summary>
		/// <param name="dataType">  the data type for storing samples </param>
		/// <param name="w">         the width, in pixels, of the region of
		///                  image data described </param>
		/// <param name="h">         the height, in pixels, of the region of
		///                  image data described </param>
		/// <param name="numberOfBits"> the number of bits per pixel </param>
		/// <exception cref="IllegalArgumentException"> if <code>dataType</code> is not
		///         either <code>DataBuffer.TYPE_BYTE</code>,
		///         <code>DataBuffer.TYPE_USHORT</code>, or
		///         <code>DataBuffer.TYPE_INT</code> </exception>
		public MultiPixelPackedSampleModel(int dataType, int w, int h, int numberOfBits) : this(dataType,w,h, numberOfBits, (w * numberOfBits + DataBuffer.GetDataTypeSize(dataType) - 1) / DataBuffer.GetDataTypeSize(dataType), 0)
		{
			if (dataType != DataBuffer.TYPE_BYTE && dataType != DataBuffer.TYPE_USHORT && dataType != DataBuffer.TYPE_INT)
			{
				throw new IllegalArgumentException("Unsupported data type " + dataType);
			}
		}

		/// <summary>
		/// Constructs a <code>MultiPixelPackedSampleModel</code> with
		/// specified data type, width, height, number of bits per pixel,
		/// scanline stride and data bit offset. </summary>
		/// <param name="dataType">  the data type for storing samples </param>
		/// <param name="w">         the width, in pixels, of the region of
		///                  image data described </param>
		/// <param name="h">         the height, in pixels, of the region of
		///                  image data described </param>
		/// <param name="numberOfBits"> the number of bits per pixel </param>
		/// <param name="scanlineStride"> the line stride of the image data </param>
		/// <param name="dataBitOffset"> the data bit offset for the region of image
		///                  data described </param>
		/// <exception cref="RasterFormatException"> if the number of bits per pixel
		///                  is not a power of 2 or if a power of 2 number of
		///                  pixels do not fit in one data element. </exception>
		/// <exception cref="IllegalArgumentException"> if <code>w</code> or
		///         <code>h</code> is not greater than 0 </exception>
		/// <exception cref="IllegalArgumentException"> if <code>dataType</code> is not
		///         either <code>DataBuffer.TYPE_BYTE</code>,
		///         <code>DataBuffer.TYPE_USHORT</code>, or
		///         <code>DataBuffer.TYPE_INT</code> </exception>
		public MultiPixelPackedSampleModel(int dataType, int w, int h, int numberOfBits, int scanlineStride, int dataBitOffset) : base(dataType, w, h, 1)
		{
			if (dataType != DataBuffer.TYPE_BYTE && dataType != DataBuffer.TYPE_USHORT && dataType != DataBuffer.TYPE_INT)
			{
				throw new IllegalArgumentException("Unsupported data type " + dataType);
			}
			this.DataType_Renamed = dataType;
			this.PixelBitStride_Renamed = numberOfBits;
			this.ScanlineStride_Renamed = scanlineStride;
			this.DataBitOffset_Renamed = dataBitOffset;
			this.DataElementSize = DataBuffer.GetDataTypeSize(dataType);
			this.PixelsPerDataElement = DataElementSize / numberOfBits;
			if (PixelsPerDataElement * numberOfBits != DataElementSize)
			{
			   throw new RasterFormatException("MultiPixelPackedSampleModel " + "does not allow pixels to " + "span data element boundaries");
			}
			this.BitMask = (1 << numberOfBits) - 1;
		}


		/// <summary>
		/// Creates a new <code>MultiPixelPackedSampleModel</code> with the
		/// specified width and height.  The new
		/// <code>MultiPixelPackedSampleModel</code> has the
		/// same storage data type and number of bits per pixel as this
		/// <code>MultiPixelPackedSampleModel</code>. </summary>
		/// <param name="w"> the specified width </param>
		/// <param name="h"> the specified height </param>
		/// <returns> a <seealso cref="SampleModel"/> with the specified width and height
		/// and with the same storage data type and number of bits per pixel
		/// as this <code>MultiPixelPackedSampleModel</code>. </returns>
		/// <exception cref="IllegalArgumentException"> if <code>w</code> or
		///         <code>h</code> is not greater than 0 </exception>
		public override SampleModel CreateCompatibleSampleModel(int w, int h)
		{
		  SampleModel sampleModel = new MultiPixelPackedSampleModel(DataType_Renamed, w, h, PixelBitStride_Renamed);
		  return sampleModel;
		}

		/// <summary>
		/// Creates a <code>DataBuffer</code> that corresponds to this
		/// <code>MultiPixelPackedSampleModel</code>.  The
		/// <code>DataBuffer</code> object's data type and size
		/// is consistent with this <code>MultiPixelPackedSampleModel</code>.
		/// The <code>DataBuffer</code> has a single bank. </summary>
		/// <returns> a <code>DataBuffer</code> with the same data type and
		/// size as this <code>MultiPixelPackedSampleModel</code>. </returns>
		public override DataBuffer CreateDataBuffer()
		{
			DataBuffer dataBuffer = null;

			int size = (int)ScanlineStride_Renamed * Height_Renamed;
			switch (DataType_Renamed)
			{
			case DataBuffer.TYPE_BYTE:
				dataBuffer = new DataBufferByte(size + (DataBitOffset_Renamed + 7) / 8);
				break;
			case DataBuffer.TYPE_USHORT:
				dataBuffer = new DataBufferUShort(size + (DataBitOffset_Renamed + 15) / 16);
				break;
			case DataBuffer.TYPE_INT:
				dataBuffer = new DataBufferInt(size + (DataBitOffset_Renamed + 31) / 32);
				break;
			}
			return dataBuffer;
		}

		/// <summary>
		/// Returns the number of data elements needed to transfer one pixel
		/// via the <seealso cref="#getDataElements"/> and <seealso cref="#setDataElements"/>
		/// methods.  For a <code>MultiPixelPackedSampleModel</code>, this is
		/// one. </summary>
		/// <returns> the number of data elements. </returns>
		public override int NumDataElements
		{
			get
			{
				return 1;
			}
		}

		/// <summary>
		/// Returns the number of bits per sample for all bands. </summary>
		/// <returns> the number of bits per sample. </returns>
		public override int[] SampleSize
		{
			get
			{
				int[] sampleSize = new int[] {PixelBitStride_Renamed};
				return sampleSize;
			}
		}

		/// <summary>
		/// Returns the number of bits per sample for the specified band. </summary>
		/// <param name="band"> the specified band </param>
		/// <returns> the number of bits per sample for the specified band. </returns>
		public override int GetSampleSize(int band)
		{
			return PixelBitStride_Renamed;
		}

		/// <summary>
		/// Returns the offset of pixel (x,&nbsp;y) in data array elements. </summary>
		/// <param name="x"> the X coordinate of the specified pixel </param>
		/// <param name="y"> the Y coordinate of the specified pixel </param>
		/// <returns> the offset of the specified pixel. </returns>
		public virtual int GetOffset(int x, int y)
		{
			int offset = y * ScanlineStride_Renamed;
			offset += (x * PixelBitStride_Renamed + DataBitOffset_Renamed) / DataElementSize;
			return offset;
		}

		/// <summary>
		///  Returns the offset, in bits, into the data element in which it is
		///  stored for the <code>x</code>th pixel of a scanline.
		///  This offset is the same for all scanlines. </summary>
		///  <param name="x"> the specified pixel </param>
		///  <returns> the bit offset of the specified pixel. </returns>
		public virtual int GetBitOffset(int x)
		{
		   return (x * PixelBitStride_Renamed + DataBitOffset_Renamed) % DataElementSize;
		}

		/// <summary>
		/// Returns the scanline stride. </summary>
		/// <returns> the scanline stride of this
		/// <code>MultiPixelPackedSampleModel</code>. </returns>
		public virtual int ScanlineStride
		{
			get
			{
				return ScanlineStride_Renamed;
			}
		}

		/// <summary>
		/// Returns the pixel bit stride in bits.  This value is the same as
		/// the number of bits per pixel. </summary>
		/// <returns> the <code>pixelBitStride</code> of this
		/// <code>MultiPixelPackedSampleModel</code>. </returns>
		public virtual int PixelBitStride
		{
			get
			{
				return PixelBitStride_Renamed;
			}
		}

		/// <summary>
		/// Returns the data bit offset in bits. </summary>
		/// <returns> the <code>dataBitOffset</code> of this
		/// <code>MultiPixelPackedSampleModel</code>. </returns>
		public virtual int DataBitOffset
		{
			get
			{
				return DataBitOffset_Renamed;
			}
		}

		/// <summary>
		///  Returns the TransferType used to transfer pixels by way of the
		///  <code>getDataElements</code> and <code>setDataElements</code>
		///  methods. The TransferType might or might not be the same as the
		///  storage DataType.  The TransferType is one of
		///  DataBuffer.TYPE_BYTE, DataBuffer.TYPE_USHORT,
		///  or DataBuffer.TYPE_INT. </summary>
		///  <returns> the transfertype. </returns>
		public override int TransferType
		{
			get
			{
				if (PixelBitStride_Renamed > 16)
				{
					return DataBuffer.TYPE_INT;
				}
				else if (PixelBitStride_Renamed > 8)
				{
					return DataBuffer.TYPE_USHORT;
				}
				else
				{
					return DataBuffer.TYPE_BYTE;
				}
			}
		}

		/// <summary>
		/// Creates a new <code>MultiPixelPackedSampleModel</code> with a
		/// subset of the bands of this
		/// <code>MultiPixelPackedSampleModel</code>.  Since a
		/// <code>MultiPixelPackedSampleModel</code> only has one band, the
		/// bands argument must have a length of one and indicate the zeroth
		/// band. </summary>
		/// <param name="bands"> the specified bands </param>
		/// <returns> a new <code>SampleModel</code> with a subset of bands of
		/// this <code>MultiPixelPackedSampleModel</code>. </returns>
		/// <exception cref="RasterFormatException"> if the number of bands requested
		/// is not one. </exception>
		/// <exception cref="IllegalArgumentException"> if <code>w</code> or
		///         <code>h</code> is not greater than 0 </exception>
		public override SampleModel CreateSubsetSampleModel(int[] bands)
		{
			if (bands != null)
			{
			   if (bands.Length != 1)
			   {
				throw new RasterFormatException("MultiPixelPackedSampleModel has " + "only one band.");
			   }
			}
			SampleModel sm = CreateCompatibleSampleModel(Width_Renamed, Height_Renamed);
			return sm;
		}

		/// <summary>
		/// Returns as <code>int</code> the sample in a specified band for the
		/// pixel located at (x,&nbsp;y).  An
		/// <code>ArrayIndexOutOfBoundsException</code> is thrown if the
		/// coordinates are not in bounds. </summary>
		/// <param name="x">         the X coordinate of the specified pixel </param>
		/// <param name="y">         the Y coordinate of the specified pixel </param>
		/// <param name="b">         the band to return, which is assumed to be 0 </param>
		/// <param name="data">      the <code>DataBuffer</code> containing the image
		///                  data </param>
		/// <returns> the specified band containing the sample of the specified
		/// pixel. </returns>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if the specified
		///          coordinates are not in bounds. </exception>
		/// <seealso cref= #setSample(int, int, int, int, DataBuffer) </seealso>
		public override int GetSample(int x, int y, int b, DataBuffer data)
		{
			// 'b' must be 0
			if ((x < 0) || (y < 0) || (x >= Width_Renamed) || (y >= Height_Renamed) || (b != 0))
			{
				throw new ArrayIndexOutOfBoundsException("Coordinate out of bounds!");
			}
			int bitnum = DataBitOffset_Renamed + x * PixelBitStride_Renamed;
			int element = data.GetElem(y * ScanlineStride_Renamed + bitnum / DataElementSize);
			int shift = DataElementSize - (bitnum & (DataElementSize-1)) - PixelBitStride_Renamed;
			return (element >> shift) & BitMask;
		}

		/// <summary>
		/// Sets a sample in the specified band for the pixel located at
		/// (x,&nbsp;y) in the <code>DataBuffer</code> using an
		/// <code>int</code> for input.
		/// An <code>ArrayIndexOutOfBoundsException</code> is thrown if the
		/// coordinates are not in bounds. </summary>
		/// <param name="x"> the X coordinate of the specified pixel </param>
		/// <param name="y"> the Y coordinate of the specified pixel </param>
		/// <param name="b"> the band to return, which is assumed to be 0 </param>
		/// <param name="s"> the input sample as an <code>int</code> </param>
		/// <param name="data"> the <code>DataBuffer</code> where image data is stored </param>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates are
		/// not in bounds. </exception>
		/// <seealso cref= #getSample(int, int, int, DataBuffer) </seealso>
		public override void SetSample(int x, int y, int b, int s, DataBuffer data)
		{
			// 'b' must be 0
			if ((x < 0) || (y < 0) || (x >= Width_Renamed) || (y >= Height_Renamed) || (b != 0))
			{
				throw new ArrayIndexOutOfBoundsException("Coordinate out of bounds!");
			}
			int bitnum = DataBitOffset_Renamed + x * PixelBitStride_Renamed;
			int index = y * ScanlineStride_Renamed + (bitnum / DataElementSize);
			int shift = DataElementSize - (bitnum & (DataElementSize-1)) - PixelBitStride_Renamed;
			int element = data.GetElem(index);
			element &= ~(BitMask << shift);
			element |= (s & BitMask) << shift;
			data.SetElem(index,element);
		}

		/// <summary>
		/// Returns data for a single pixel in a primitive array of type
		/// TransferType.  For a <code>MultiPixelPackedSampleModel</code>,
		/// the array has one element, and the type is the smallest of
		/// DataBuffer.TYPE_BYTE, DataBuffer.TYPE_USHORT, or DataBuffer.TYPE_INT
		/// that can hold a single pixel.  Generally, <code>obj</code>
		/// should be passed in as <code>null</code>, so that the
		/// <code>Object</code> is created automatically and is the
		/// correct primitive data type.
		/// <para>
		/// The following code illustrates transferring data for one pixel from
		/// <code>DataBuffer</code> <code>db1</code>, whose storage layout is
		/// described by <code>MultiPixelPackedSampleModel</code>
		/// <code>mppsm1</code>, to <code>DataBuffer</code> <code>db2</code>,
		/// whose storage layout is described by
		/// <code>MultiPixelPackedSampleModel</code> <code>mppsm2</code>.
		/// The transfer is generally more efficient than using
		/// <code>getPixel</code> or <code>setPixel</code>.
		/// <pre>
		///       MultiPixelPackedSampleModel mppsm1, mppsm2;
		///       DataBufferInt db1, db2;
		///       mppsm2.setDataElements(x, y, mppsm1.getDataElements(x, y, null,
		///                              db1), db2);
		/// </pre>
		/// Using <code>getDataElements</code> or <code>setDataElements</code>
		/// to transfer between two <code>DataBuffer/SampleModel</code> pairs
		/// is legitimate if the <code>SampleModels</code> have the same number
		/// of bands, corresponding bands have the same number of
		/// bits per sample, and the TransferTypes are the same.
		/// </para>
		/// <para>
		/// If <code>obj</code> is not <code>null</code>, it should be a
		/// primitive array of type TransferType.  Otherwise, a
		/// <code>ClassCastException</code> is thrown.  An
		/// <code>ArrayIndexOutOfBoundsException</code> is thrown if the
		/// coordinates are not in bounds, or if <code>obj</code> is not
		/// <code>null</code> and is not large enough to hold the pixel data.
		/// </para>
		/// </summary>
		/// <param name="x"> the X coordinate of the specified pixel </param>
		/// <param name="y"> the Y coordinate of the specified pixel </param>
		/// <param name="obj"> a primitive array in which to return the pixel data or
		///          <code>null</code>. </param>
		/// <param name="data"> the <code>DataBuffer</code> containing the image data. </param>
		/// <returns> an <code>Object</code> containing data for the specified
		///  pixel. </returns>
		/// <exception cref="ClassCastException"> if <code>obj</code> is not a
		///  primitive array of type TransferType or is not <code>null</code> </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates are
		/// not in bounds, or if <code>obj</code> is not <code>null</code> or
		/// not large enough to hold the pixel data </exception>
		/// <seealso cref= #setDataElements(int, int, Object, DataBuffer) </seealso>
		public override Object GetDataElements(int x, int y, Object obj, DataBuffer data)
		{
			if ((x < 0) || (y < 0) || (x >= Width_Renamed) || (y >= Height_Renamed))
			{
				throw new ArrayIndexOutOfBoundsException("Coordinate out of bounds!");
			}

			int type = TransferType;
			int bitnum = DataBitOffset_Renamed + x * PixelBitStride_Renamed;
			int shift = DataElementSize - (bitnum & (DataElementSize-1)) - PixelBitStride_Renamed;
			int element = 0;

			switch (type)
			{

			case DataBuffer.TYPE_BYTE:

				sbyte[] bdata;

				if (obj == null)
				{
					bdata = new sbyte[1];
				}
				else
				{
					bdata = (sbyte[])obj;
				}

				element = data.GetElem(y * ScanlineStride_Renamed + bitnum / DataElementSize);
				bdata[0] = (sbyte)((element >> shift) & BitMask);

				obj = (Object)bdata;
				break;

			case DataBuffer.TYPE_USHORT:

				short[] sdata;

				if (obj == null)
				{
					sdata = new short[1];
				}
				else
				{
					sdata = (short[])obj;
				}

				element = data.GetElem(y * ScanlineStride_Renamed + bitnum / DataElementSize);
				sdata[0] = (short)((element >> shift) & BitMask);

				obj = (Object)sdata;
				break;

			case DataBuffer.TYPE_INT:

				int[] idata;

				if (obj == null)
				{
					idata = new int[1];
				}
				else
				{
					idata = (int[])obj;
				}

				element = data.GetElem(y * ScanlineStride_Renamed + bitnum / DataElementSize);
				idata[0] = (element >> shift) & BitMask;

				obj = (Object)idata;
				break;
			}

			return obj;
		}

		/// <summary>
		/// Returns the specified single band pixel in the first element
		/// of an <code>int</code> array.
		/// <code>ArrayIndexOutOfBoundsException</code> is thrown if the
		/// coordinates are not in bounds. </summary>
		/// <param name="x"> the X coordinate of the specified pixel </param>
		/// <param name="y"> the Y coordinate of the specified pixel </param>
		/// <param name="iArray"> the array containing the pixel to be returned or
		///  <code>null</code> </param>
		/// <param name="data"> the <code>DataBuffer</code> where image data is stored </param>
		/// <returns> an array containing the specified pixel. </returns>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates
		///  are not in bounds </exception>
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
			   pixels = new int [NumBands_Renamed];
			}
			int bitnum = DataBitOffset_Renamed + x * PixelBitStride_Renamed;
			int element = data.GetElem(y * ScanlineStride_Renamed + bitnum / DataElementSize);
			int shift = DataElementSize - (bitnum & (DataElementSize-1)) - PixelBitStride_Renamed;
			pixels[0] = (element >> shift) & BitMask;
			return pixels;
		}

		/// <summary>
		/// Sets the data for a single pixel in the specified
		/// <code>DataBuffer</code> from a primitive array of type
		/// TransferType.  For a <code>MultiPixelPackedSampleModel</code>,
		/// only the first element of the array holds valid data,
		/// and the type must be the smallest of
		/// DataBuffer.TYPE_BYTE, DataBuffer.TYPE_USHORT, or DataBuffer.TYPE_INT
		/// that can hold a single pixel.
		/// <para>
		/// The following code illustrates transferring data for one pixel from
		/// <code>DataBuffer</code> <code>db1</code>, whose storage layout is
		/// described by <code>MultiPixelPackedSampleModel</code>
		/// <code>mppsm1</code>, to <code>DataBuffer</code> <code>db2</code>,
		/// whose storage layout is described by
		/// <code>MultiPixelPackedSampleModel</code> <code>mppsm2</code>.
		/// The transfer is generally more efficient than using
		/// <code>getPixel</code> or <code>setPixel</code>.
		/// <pre>
		///       MultiPixelPackedSampleModel mppsm1, mppsm2;
		///       DataBufferInt db1, db2;
		///       mppsm2.setDataElements(x, y, mppsm1.getDataElements(x, y, null,
		///                              db1), db2);
		/// </pre>
		/// Using <code>getDataElements</code> or <code>setDataElements</code> to
		/// transfer between two <code>DataBuffer/SampleModel</code> pairs is
		/// legitimate if the <code>SampleModel</code> objects have
		/// the same number of bands, corresponding bands have the same number of
		/// bits per sample, and the TransferTypes are the same.
		/// </para>
		/// <para>
		/// <code>obj</code> must be a primitive array of type TransferType.
		/// Otherwise, a <code>ClassCastException</code> is thrown.  An
		/// <code>ArrayIndexOutOfBoundsException</code> is thrown if the
		/// coordinates are not in bounds, or if <code>obj</code> is not large
		/// enough to hold the pixel data.
		/// </para>
		/// </summary>
		/// <param name="x"> the X coordinate of the pixel location </param>
		/// <param name="y"> the Y coordinate of the pixel location </param>
		/// <param name="obj"> a primitive array containing pixel data </param>
		/// <param name="data"> the <code>DataBuffer</code> containing the image data </param>
		/// <seealso cref= #getDataElements(int, int, Object, DataBuffer) </seealso>
		public override void SetDataElements(int x, int y, Object obj, DataBuffer data)
		{
			if ((x < 0) || (y < 0) || (x >= Width_Renamed) || (y >= Height_Renamed))
			{
				throw new ArrayIndexOutOfBoundsException("Coordinate out of bounds!");
			}

			int type = TransferType;
			int bitnum = DataBitOffset_Renamed + x * PixelBitStride_Renamed;
			int index = y * ScanlineStride_Renamed + (bitnum / DataElementSize);
			int shift = DataElementSize - (bitnum & (DataElementSize-1)) - PixelBitStride_Renamed;
			int element = data.GetElem(index);
			element &= ~(BitMask << shift);

			switch (type)
			{

			case DataBuffer.TYPE_BYTE:

				sbyte[] barray = (sbyte[])obj;
				element |= (((int)(barray[0]) & 0xff) & BitMask) << shift;
				data.SetElem(index, element);
				break;

			case DataBuffer.TYPE_USHORT:

				short[] sarray = (short[])obj;
				element |= (((int)(sarray[0]) & 0xffff) & BitMask) << shift;
				data.SetElem(index, element);
				break;

			case DataBuffer.TYPE_INT:

				int[] iarray = (int[])obj;
				element |= (iarray[0] & BitMask) << shift;
				data.SetElem(index, element);
				break;
			}
		}

		/// <summary>
		/// Sets a pixel in the <code>DataBuffer</code> using an
		/// <code>int</code> array for input.
		/// <code>ArrayIndexOutOfBoundsException</code> is thrown if
		/// the coordinates are not in bounds. </summary>
		/// <param name="x"> the X coordinate of the pixel location </param>
		/// <param name="y"> the Y coordinate of the pixel location </param>
		/// <param name="iArray"> the input pixel in an <code>int</code> array </param>
		/// <param name="data"> the <code>DataBuffer</code> containing the image data </param>
		/// <seealso cref= #getPixel(int, int, int[], DataBuffer) </seealso>
		public override void SetPixel(int x, int y, int[] iArray, DataBuffer data)
		{
			if ((x < 0) || (y < 0) || (x >= Width_Renamed) || (y >= Height_Renamed))
			{
				throw new ArrayIndexOutOfBoundsException("Coordinate out of bounds!");
			}
			int bitnum = DataBitOffset_Renamed + x * PixelBitStride_Renamed;
			int index = y * ScanlineStride_Renamed + (bitnum / DataElementSize);
			int shift = DataElementSize - (bitnum & (DataElementSize-1)) - PixelBitStride_Renamed;
			int element = data.GetElem(index);
			element &= ~(BitMask << shift);
			element |= (iArray[0] & BitMask) << shift;
			data.SetElem(index,element);
		}

		public override bool Equals(Object o)
		{
			if ((o == null) || !(o is MultiPixelPackedSampleModel))
			{
				return false;
			}

			MultiPixelPackedSampleModel that = (MultiPixelPackedSampleModel)o;
			return this.Width_Renamed == that.Width_Renamed && this.Height_Renamed == that.Height_Renamed && this.NumBands_Renamed == that.NumBands_Renamed && this.DataType_Renamed == that.DataType_Renamed && this.PixelBitStride_Renamed == that.PixelBitStride_Renamed && this.BitMask == that.BitMask && this.PixelsPerDataElement == that.PixelsPerDataElement && this.DataElementSize == that.DataElementSize && this.DataBitOffset_Renamed == that.DataBitOffset_Renamed && this.ScanlineStride_Renamed == that.ScanlineStride_Renamed;
		}

		// If we implement equals() we must also implement hashCode
		public override int HashCode()
		{
			int hash = 0;
			hash = Width_Renamed;
			hash <<= 8;
			hash ^= Height_Renamed;
			hash <<= 8;
			hash ^= NumBands_Renamed;
			hash <<= 8;
			hash ^= DataType_Renamed;
			hash <<= 8;
			hash ^= PixelBitStride_Renamed;
			hash <<= 8;
			hash ^= BitMask;
			hash <<= 8;
			hash ^= PixelsPerDataElement;
			hash <<= 8;
			hash ^= DataElementSize;
			hash <<= 8;
			hash ^= DataBitOffset_Renamed;
			hash <<= 8;
			hash ^= ScanlineStride_Renamed;
			return hash;
		}
	}

}