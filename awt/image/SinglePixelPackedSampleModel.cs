using System.Runtime.InteropServices;

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
	///  This class represents pixel data packed such that the N samples which make
	///  up a single pixel are stored in a single data array element, and each data
	///  data array element holds samples for only one pixel.
	///  This class supports
	///  <seealso cref="DataBuffer#TYPE_BYTE TYPE_BYTE"/>,
	///  <seealso cref="DataBuffer#TYPE_USHORT TYPE_USHORT"/>,
	///  <seealso cref="DataBuffer#TYPE_INT TYPE_INT"/> data types.
	///  All data array elements reside
	///  in the first bank of a DataBuffer.  Accessor methods are provided so
	///  that the image data can be manipulated directly. Scanline stride is the
	///  number of data array elements between a given sample and the corresponding
	///  sample in the same column of the next scanline. Bit masks are the masks
	///  required to extract the samples representing the bands of the pixel.
	///  Bit offsets are the offsets in bits into the data array
	///  element of the samples representing the bands of the pixel.
	/// <para>
	/// The following code illustrates extracting the bits of the sample
	/// representing band <code>b</code> for pixel <code>x,y</code>
	/// from DataBuffer <code>data</code>:
	/// <pre>{@code
	///      int sample = data.getElem(y * scanlineStride + x);
	///      sample = (sample & bitMasks[b]) >>> bitOffsets[b];
	/// }</pre>
	/// </para>
	/// </summary>

	public class SinglePixelPackedSampleModel : SampleModel
	{
		/// <summary>
		/// Bit masks for all bands of the image data. </summary>
		private int[] BitMasks_Renamed;

		/// <summary>
		/// Bit Offsets for all bands of the image data. </summary>
		private int[] BitOffsets_Renamed;

		/// <summary>
		/// Bit sizes for all the bands of the image data. </summary>
		private int[] BitSizes;

		/// <summary>
		/// Maximum bit size. </summary>
		private int MaxBitSize;

		/// <summary>
		/// Line stride of the region of image data described by this
		///  SinglePixelPackedSampleModel.
		/// </summary>
		private int ScanlineStride_Renamed;

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern void initIDs();
		static SinglePixelPackedSampleModel()
		{
			ColorModel.LoadLibraries();
			initIDs();
		}

		/// <summary>
		/// Constructs a SinglePixelPackedSampleModel with bitMasks.length bands.
		/// Each sample is stored in a data array element in the position of
		/// its corresponding bit mask.  Each bit mask must be contiguous and
		/// masks must not overlap. Bit masks exceeding data type capacity are
		/// truncated. </summary>
		/// <param name="dataType">  The data type for storing samples. </param>
		/// <param name="w">         The width (in pixels) of the region of the
		///                  image data described. </param>
		/// <param name="h">         The height (in pixels) of the region of the
		///                  image data described. </param>
		/// <param name="bitMasks">  The bit masks for all bands. </param>
		/// <exception cref="IllegalArgumentException"> if <code>dataType</code> is not
		///         either <code>DataBuffer.TYPE_BYTE</code>,
		///         <code>DataBuffer.TYPE_USHORT</code>, or
		///         <code>DataBuffer.TYPE_INT</code> </exception>
		public SinglePixelPackedSampleModel(int dataType, int w, int h, int[] bitMasks) : this(dataType, w, h, w, bitMasks)
		{
			if (dataType != DataBuffer.TYPE_BYTE && dataType != DataBuffer.TYPE_USHORT && dataType != DataBuffer.TYPE_INT)
			{
				throw new IllegalArgumentException("Unsupported data type " + dataType);
			}
		}

		/// <summary>
		/// Constructs a SinglePixelPackedSampleModel with bitMasks.length bands
		/// and a scanline stride equal to scanlineStride data array elements.
		/// Each sample is stored in a data array element in the position of
		/// its corresponding bit mask.  Each bit mask must be contiguous and
		/// masks must not overlap. Bit masks exceeding data type capacity are
		/// truncated. </summary>
		/// <param name="dataType">  The data type for storing samples. </param>
		/// <param name="w">         The width (in pixels) of the region of
		///                  image data described. </param>
		/// <param name="h">         The height (in pixels) of the region of
		///                  image data described. </param>
		/// <param name="scanlineStride"> The line stride of the image data. </param>
		/// <param name="bitMasks"> The bit masks for all bands. </param>
		/// <exception cref="IllegalArgumentException"> if <code>w</code> or
		///         <code>h</code> is not greater than 0 </exception>
		/// <exception cref="IllegalArgumentException"> if any mask in
		///         <code>bitMask</code> is not contiguous </exception>
		/// <exception cref="IllegalArgumentException"> if <code>dataType</code> is not
		///         either <code>DataBuffer.TYPE_BYTE</code>,
		///         <code>DataBuffer.TYPE_USHORT</code>, or
		///         <code>DataBuffer.TYPE_INT</code> </exception>
		public SinglePixelPackedSampleModel(int dataType, int w, int h, int scanlineStride, int[] bitMasks) : base(dataType, w, h, bitMasks.Length)
		{
			if (dataType != DataBuffer.TYPE_BYTE && dataType != DataBuffer.TYPE_USHORT && dataType != DataBuffer.TYPE_INT)
			{
				throw new IllegalArgumentException("Unsupported data type " + dataType);
			}
			this.DataType_Renamed = dataType;
			this.BitMasks_Renamed = (int[]) bitMasks.clone();
			this.ScanlineStride_Renamed = scanlineStride;

			this.BitOffsets_Renamed = new int[NumBands_Renamed];
			this.BitSizes = new int[NumBands_Renamed];

			int maxMask = (int)((1L << DataBuffer.GetDataTypeSize(dataType)) - 1);

			this.MaxBitSize = 0;
			for (int i = 0; i < NumBands_Renamed; i++)
			{
				int bitOffset = 0, bitSize = 0, mask ;
				this.BitMasks_Renamed[i] &= maxMask;
				mask = this.BitMasks_Renamed[i];
				if (mask != 0)
				{
					while ((mask & 1) == 0)
					{
						mask = (int)((uint)mask >> 1);
						bitOffset++;
					}
					while ((mask & 1) == 1)
					{
						mask = (int)((uint)mask >> 1);
						bitSize++;
					}
					if (mask != 0)
					{
						throw new IllegalArgumentException("Mask " + bitMasks[i] + " must be contiguous");
					}
				}
				BitOffsets_Renamed[i] = bitOffset;
				BitSizes[i] = bitSize;
				if (bitSize > MaxBitSize)
				{
					MaxBitSize = bitSize;
				}
			}
		}

		/// <summary>
		/// Returns the number of data elements needed to transfer one pixel
		/// via the getDataElements and setDataElements methods.
		/// For a SinglePixelPackedSampleModel, this is one.
		/// </summary>
		public override int NumDataElements
		{
			get
			{
				return 1;
			}
		}

		/// <summary>
		/// Returns the size of the buffer (in data array elements)
		/// needed for a data buffer that matches this
		/// SinglePixelPackedSampleModel.
		/// </summary>
		private long BufferSize
		{
			get
			{
			  long size = ScanlineStride_Renamed * (Height_Renamed - 1) + Width_Renamed;
			  return size;
			}
		}

		/// <summary>
		/// Creates a new SinglePixelPackedSampleModel with the specified
		/// width and height.  The new SinglePixelPackedSampleModel will have the
		/// same storage data type and bit masks as this
		/// SinglePixelPackedSampleModel. </summary>
		/// <param name="w"> the width of the resulting <code>SampleModel</code> </param>
		/// <param name="h"> the height of the resulting <code>SampleModel</code> </param>
		/// <returns> a <code>SinglePixelPackedSampleModel</code> with the
		///         specified width and height. </returns>
		/// <exception cref="IllegalArgumentException"> if <code>w</code> or
		///         <code>h</code> is not greater than 0 </exception>
		public override SampleModel CreateCompatibleSampleModel(int w, int h)
		{
		  SampleModel sampleModel = new SinglePixelPackedSampleModel(DataType_Renamed, w, h, BitMasks_Renamed);
		  return sampleModel;
		}

		/// <summary>
		/// Creates a DataBuffer that corresponds to this
		/// SinglePixelPackedSampleModel.  The DataBuffer's data type and size
		/// will be consistent with this SinglePixelPackedSampleModel.  The
		/// DataBuffer will have a single bank.
		/// </summary>
		public override DataBuffer CreateDataBuffer()
		{
			DataBuffer dataBuffer = null;

			int size = (int)BufferSize;
			switch (DataType_Renamed)
			{
			case DataBuffer.TYPE_BYTE:
				dataBuffer = new DataBufferByte(size);
				break;
			case DataBuffer.TYPE_USHORT:
				dataBuffer = new DataBufferUShort(size);
				break;
			case DataBuffer.TYPE_INT:
				dataBuffer = new DataBufferInt(size);
				break;
			}
			return dataBuffer;
		}

		/// <summary>
		/// Returns the number of bits per sample for all bands. </summary>
		public override int[] SampleSize
		{
			get
			{
				return BitSizes.clone();
			}
		}

		/// <summary>
		/// Returns the number of bits per sample for the specified band. </summary>
		public override int GetSampleSize(int band)
		{
			return BitSizes[band];
		}

		/// <summary>
		/// Returns the offset (in data array elements) of pixel (x,y).
		///  The data element containing pixel <code>x,y</code>
		///  can be retrieved from a DataBuffer <code>data</code> with a
		///  SinglePixelPackedSampleModel <code>sppsm</code> as:
		/// <pre>
		///        data.getElem(sppsm.getOffset(x, y));
		/// </pre> </summary>
		/// <param name="x"> the X coordinate of the specified pixel </param>
		/// <param name="y"> the Y coordinate of the specified pixel </param>
		/// <returns> the offset of the specified pixel. </returns>
		public virtual int GetOffset(int x, int y)
		{
			int offset = y * ScanlineStride_Renamed + x;
			return offset;
		}

		/// <summary>
		/// Returns the bit offsets into the data array element representing
		///  a pixel for all bands. </summary>
		///  <returns> the bit offsets representing a pixel for all bands. </returns>
		public virtual int [] BitOffsets
		{
			get
			{
			  return (int[])BitOffsets_Renamed.clone();
			}
		}

		/// <summary>
		/// Returns the bit masks for all bands. </summary>
		///  <returns> the bit masks for all bands. </returns>
		public virtual int [] BitMasks
		{
			get
			{
			  return (int[])BitMasks_Renamed.clone();
			}
		}

		/// <summary>
		/// Returns the scanline stride of this SinglePixelPackedSampleModel. </summary>
		///  <returns> the scanline stride of this
		///          <code>SinglePixelPackedSampleModel</code>. </returns>
		public virtual int ScanlineStride
		{
			get
			{
			  return ScanlineStride_Renamed;
			}
		}

		/// <summary>
		/// This creates a new SinglePixelPackedSampleModel with a subset of the
		/// bands of this SinglePixelPackedSampleModel.  The new
		/// SinglePixelPackedSampleModel can be used with any DataBuffer that the
		/// existing SinglePixelPackedSampleModel can be used with.  The new
		/// SinglePixelPackedSampleModel/DataBuffer combination will represent
		/// an image with a subset of the bands of the original
		/// SinglePixelPackedSampleModel/DataBuffer combination. </summary>
		/// <exception cref="RasterFormatException"> if the length of the bands argument is
		///                                  greater than the number of bands in
		///                                  the sample model. </exception>
		public override SampleModel CreateSubsetSampleModel(int[] bands)
		{
			if (bands.Length > NumBands_Renamed)
			{
				throw new RasterFormatException("There are only " + NumBands_Renamed + " bands");
			}
			int[] newBitMasks = new int[bands.Length];
			for (int i = 0; i < bands.Length; i++)
			{
				newBitMasks[i] = BitMasks_Renamed[bands[i]];
			}

			return new SinglePixelPackedSampleModel(this.DataType_Renamed, Width_Renamed, Height_Renamed, this.ScanlineStride_Renamed, newBitMasks);
		}

		/// <summary>
		/// Returns data for a single pixel in a primitive array of type
		/// TransferType.  For a SinglePixelPackedSampleModel, the array will
		/// have one element, and the type will be the same as the storage
		/// data type.  Generally, obj
		/// should be passed in as null, so that the Object will be created
		/// automatically and will be of the right primitive data type.
		/// <para>
		/// The following code illustrates transferring data for one pixel from
		/// DataBuffer <code>db1</code>, whose storage layout is described by
		/// SinglePixelPackedSampleModel <code>sppsm1</code>, to
		/// DataBuffer <code>db2</code>, whose storage layout is described by
		/// SinglePixelPackedSampleModel <code>sppsm2</code>.
		/// The transfer will generally be more efficient than using
		/// getPixel/setPixel.
		/// <pre>
		///       SinglePixelPackedSampleModel sppsm1, sppsm2;
		///       DataBufferInt db1, db2;
		///       sppsm2.setDataElements(x, y, sppsm1.getDataElements(x, y, null,
		///                              db1), db2);
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
		/// <param name="x">         The X coordinate of the pixel location. </param>
		/// <param name="y">         The Y coordinate of the pixel location. </param>
		/// <param name="obj">       If non-null, a primitive array in which to return
		///                  the pixel data. </param>
		/// <param name="data">      The DataBuffer containing the image data. </param>
		/// <returns> the data for the specified pixel. </returns>
		/// <seealso cref= #setDataElements(int, int, Object, DataBuffer) </seealso>
		public override Object GetDataElements(int x, int y, Object obj, DataBuffer data)
		{
			// Bounds check for 'b' will be performed automatically
			if ((x < 0) || (y < 0) || (x >= Width_Renamed) || (y >= Height_Renamed))
			{
				throw new ArrayIndexOutOfBoundsException("Coordinate out of bounds!");
			}

			int type = TransferType;

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

				bdata[0] = (sbyte)data.GetElem(y * ScanlineStride_Renamed + x);

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

				sdata[0] = (short)data.GetElem(y * ScanlineStride_Renamed + x);

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

				idata[0] = data.GetElem(y * ScanlineStride_Renamed + x);

				obj = (Object)idata;
				break;
			}

			return obj;
		}

		/// <summary>
		/// Returns all samples in for the specified pixel in an int array.
		/// ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		/// not in bounds. </summary>
		/// <param name="x">         The X coordinate of the pixel location. </param>
		/// <param name="y">         The Y coordinate of the pixel location. </param>
		/// <param name="iArray">    If non-null, returns the samples in this array </param>
		/// <param name="data">      The DataBuffer containing the image data. </param>
		/// <returns> all samples for the specified pixel. </returns>
		/// <seealso cref= #setPixel(int, int, int[], DataBuffer) </seealso>
		public override int [] GetPixel(int x, int y, int[] iArray, DataBuffer data)
		{
			if ((x < 0) || (y < 0) || (x >= Width_Renamed) || (y >= Height_Renamed))
			{
				throw new ArrayIndexOutOfBoundsException("Coordinate out of bounds!");
			}
			int[] pixels;
			if (iArray == null)
			{
				pixels = new int [NumBands_Renamed];
			}
			else
			{
				pixels = iArray;
			}

			int value = data.GetElem(y * ScanlineStride_Renamed + x);
			for (int i = 0; i < NumBands_Renamed; i++)
			{
				pixels[i] = (int)((uint)(value & BitMasks_Renamed[i]) >> BitOffsets_Renamed[i]);
			}
			return pixels;
		}

		/// <summary>
		/// Returns all samples for the specified rectangle of pixels in
		/// an int array, one sample per array element.
		/// ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		/// not in bounds. </summary>
		/// <param name="x">         The X coordinate of the upper left pixel location. </param>
		/// <param name="y">         The Y coordinate of the upper left pixel location. </param>
		/// <param name="w">         The width of the pixel rectangle. </param>
		/// <param name="h">         The height of the pixel rectangle. </param>
		/// <param name="iArray">    If non-null, returns the samples in this array. </param>
		/// <param name="data">      The DataBuffer containing the image data. </param>
		/// <returns> all samples for the specified region of pixels. </returns>
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
			   pixels = new int [w * h * NumBands_Renamed];
			}
			int lineOffset = y * ScanlineStride_Renamed + x;
			int dstOffset = 0;

			for (int i = 0; i < h; i++)
			{
			   for (int j = 0; j < w; j++)
			   {
				  int value = data.GetElem(lineOffset + j);
				  for (int k = 0; k < NumBands_Renamed; k++)
				  {
					  pixels[dstOffset++] = ((int)((uint)(value & BitMasks_Renamed[k]) >> BitOffsets_Renamed[k]));
				  }
			   }
			   lineOffset += ScanlineStride_Renamed;
			}
			return pixels;
		}

		/// <summary>
		/// Returns as int the sample in a specified band for the pixel
		/// located at (x,y).
		/// ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		/// not in bounds. </summary>
		/// <param name="x">         The X coordinate of the pixel location. </param>
		/// <param name="y">         The Y coordinate of the pixel location. </param>
		/// <param name="b">         The band to return. </param>
		/// <param name="data">      The DataBuffer containing the image data. </param>
		/// <returns> the sample in a specified band for the specified
		///         pixel. </returns>
		/// <seealso cref= #setSample(int, int, int, int, DataBuffer) </seealso>
		public override int GetSample(int x, int y, int b, DataBuffer data)
		{
			// Bounds check for 'b' will be performed automatically
			if ((x < 0) || (y < 0) || (x >= Width_Renamed) || (y >= Height_Renamed))
			{
				throw new ArrayIndexOutOfBoundsException("Coordinate out of bounds!");
			}
			int sample = data.GetElem(y * ScanlineStride_Renamed + x);
			return ((int)((uint)(sample & BitMasks_Renamed[b]) >> BitOffsets_Renamed[b]));
		}

		/// <summary>
		/// Returns the samples for a specified band for the specified rectangle
		/// of pixels in an int array, one sample per array element.
		/// ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		/// not in bounds. </summary>
		/// <param name="x">         The X coordinate of the upper left pixel location. </param>
		/// <param name="y">         The Y coordinate of the upper left pixel location. </param>
		/// <param name="w">         The width of the pixel rectangle. </param>
		/// <param name="h">         The height of the pixel rectangle. </param>
		/// <param name="b">         The band to return. </param>
		/// <param name="iArray">    If non-null, returns the samples in this array. </param>
		/// <param name="data">      The DataBuffer containing the image data. </param>
		/// <returns> the samples for the specified band for the specified
		///         region of pixels. </returns>
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
			int lineOffset = y * ScanlineStride_Renamed + x;
			int dstOffset = 0;

			for (int i = 0; i < h; i++)
			{
			   for (int j = 0; j < w; j++)
			   {
				  int value = data.GetElem(lineOffset + j);
				  samples[dstOffset++] = ((int)((uint)(value & BitMasks_Renamed[b]) >> BitOffsets_Renamed[b]));
			   }
			   lineOffset += ScanlineStride_Renamed;
			}
			return samples;
		}

		/// <summary>
		/// Sets the data for a single pixel in the specified DataBuffer from a
		/// primitive array of type TransferType.  For a
		/// SinglePixelPackedSampleModel, only the first element of the array
		/// will hold valid data, and the type of the array must be the same as
		/// the storage data type of the SinglePixelPackedSampleModel.
		/// <para>
		/// The following code illustrates transferring data for one pixel from
		/// DataBuffer <code>db1</code>, whose storage layout is described by
		/// SinglePixelPackedSampleModel <code>sppsm1</code>,
		/// to DataBuffer <code>db2</code>, whose storage layout is described by
		/// SinglePixelPackedSampleModel <code>sppsm2</code>.
		/// The transfer will generally be more efficient than using
		/// getPixel/setPixel.
		/// <pre>
		///       SinglePixelPackedSampleModel sppsm1, sppsm2;
		///       DataBufferInt db1, db2;
		///       sppsm2.setDataElements(x, y, sppsm1.getDataElements(x, y, null,
		///                              db1), db2);
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
		/// <param name="x">         The X coordinate of the pixel location. </param>
		/// <param name="y">         The Y coordinate of the pixel location. </param>
		/// <param name="obj">       A primitive array containing pixel data. </param>
		/// <param name="data">      The DataBuffer containing the image data. </param>
		/// <seealso cref= #getDataElements(int, int, Object, DataBuffer) </seealso>
		public override void SetDataElements(int x, int y, Object obj, DataBuffer data)
		{
			if ((x < 0) || (y < 0) || (x >= Width_Renamed) || (y >= Height_Renamed))
			{
				throw new ArrayIndexOutOfBoundsException("Coordinate out of bounds!");
			}

			int type = TransferType;

			switch (type)
			{

			case DataBuffer.TYPE_BYTE:

				sbyte[] barray = (sbyte[])obj;
				data.SetElem(y * ScanlineStride_Renamed + x, ((int)barray[0]) & 0xff);
				break;

			case DataBuffer.TYPE_USHORT:

				short[] sarray = (short[])obj;
				data.SetElem(y * ScanlineStride_Renamed + x, ((int)sarray[0]) & 0xffff);
				break;

			case DataBuffer.TYPE_INT:

				int[] iarray = (int[])obj;
				data.SetElem(y * ScanlineStride_Renamed + x, iarray[0]);
				break;
			}
		}

		/// <summary>
		/// Sets a pixel in the DataBuffer using an int array of samples for input.
		/// ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		/// not in bounds. </summary>
		/// <param name="x">         The X coordinate of the pixel location. </param>
		/// <param name="y">         The Y coordinate of the pixel location. </param>
		/// <param name="iArray">    The input samples in an int array. </param>
		/// <param name="data">      The DataBuffer containing the image data. </param>
		/// <seealso cref= #getPixel(int, int, int[], DataBuffer) </seealso>
		public override void SetPixel(int x, int y, int[] iArray, DataBuffer data)
		{
			if ((x < 0) || (y < 0) || (x >= Width_Renamed) || (y >= Height_Renamed))
			{
				throw new ArrayIndexOutOfBoundsException("Coordinate out of bounds!");
			}
			int lineOffset = y * ScanlineStride_Renamed + x;
			int value = data.GetElem(lineOffset);
			for (int i = 0; i < NumBands_Renamed; i++)
			{
				value &= ~BitMasks_Renamed[i];
				value |= ((iArray[i] << BitOffsets_Renamed[i]) & BitMasks_Renamed[i]);
			}
			data.SetElem(lineOffset, value);
		}

		/// <summary>
		/// Sets all samples for a rectangle of pixels from an int array containing
		/// one sample per array element.
		/// ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		/// not in bounds. </summary>
		/// <param name="x">         The X coordinate of the upper left pixel location. </param>
		/// <param name="y">         The Y coordinate of the upper left pixel location. </param>
		/// <param name="w">         The width of the pixel rectangle. </param>
		/// <param name="h">         The height of the pixel rectangle. </param>
		/// <param name="iArray">    The input samples in an int array. </param>
		/// <param name="data">      The DataBuffer containing the image data. </param>
		/// <seealso cref= #getPixels(int, int, int, int, int[], DataBuffer) </seealso>
		public override void SetPixels(int x, int y, int w, int h, int[] iArray, DataBuffer data)
		{
			int x1 = x + w;
			int y1 = y + h;

			if (x < 0 || x >= Width_Renamed || w > Width_Renamed || x1 < 0 || x1 > Width_Renamed || y < 0 || y >= Height_Renamed || h > Height_Renamed || y1 < 0 || y1 > Height_Renamed)
			{
				throw new ArrayIndexOutOfBoundsException("Coordinate out of bounds!");
			}

			int lineOffset = y * ScanlineStride_Renamed + x;
			int srcOffset = 0;

			for (int i = 0; i < h; i++)
			{
			   for (int j = 0; j < w; j++)
			   {
				   int value = data.GetElem(lineOffset + j);
				   for (int k = 0; k < NumBands_Renamed; k++)
				   {
					   value &= ~BitMasks_Renamed[k];
					   int srcValue = iArray[srcOffset++];
					   value |= ((srcValue << BitOffsets_Renamed[k]) & BitMasks_Renamed[k]);
				   }
				   data.SetElem(lineOffset + j, value);
			   }
			   lineOffset += ScanlineStride_Renamed;
			}
		}

		/// <summary>
		/// Sets a sample in the specified band for the pixel located at (x,y)
		/// in the DataBuffer using an int for input.
		/// ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		/// not in bounds. </summary>
		/// <param name="x">         The X coordinate of the pixel location. </param>
		/// <param name="y">         The Y coordinate of the pixel location. </param>
		/// <param name="b">         The band to set. </param>
		/// <param name="s">         The input sample as an int. </param>
		/// <param name="data">      The DataBuffer containing the image data. </param>
		/// <seealso cref= #getSample(int, int, int, DataBuffer) </seealso>
		public override void SetSample(int x, int y, int b, int s, DataBuffer data)
		{
			// Bounds check for 'b' will be performed automatically
			if ((x < 0) || (y < 0) || (x >= Width_Renamed) || (y >= Height_Renamed))
			{
				throw new ArrayIndexOutOfBoundsException("Coordinate out of bounds!");
			}
			int value = data.GetElem(y * ScanlineStride_Renamed + x);
			value &= ~BitMasks_Renamed[b];
			value |= (s << BitOffsets_Renamed[b]) & BitMasks_Renamed[b];
			data.SetElem(y * ScanlineStride_Renamed + x,value);
		}

		/// <summary>
		/// Sets the samples in the specified band for the specified rectangle
		/// of pixels from an int array containing one sample per array element.
		/// ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		/// not in bounds. </summary>
		/// <param name="x">         The X coordinate of the upper left pixel location. </param>
		/// <param name="y">         The Y coordinate of the upper left pixel location. </param>
		/// <param name="w">         The width of the pixel rectangle. </param>
		/// <param name="h">         The height of the pixel rectangle. </param>
		/// <param name="b">         The band to set. </param>
		/// <param name="iArray">    The input samples in an int array. </param>
		/// <param name="data">      The DataBuffer containing the image data. </param>
		/// <seealso cref= #getSamples(int, int, int, int, int, int[], DataBuffer) </seealso>
		public override void SetSamples(int x, int y, int w, int h, int b, int[] iArray, DataBuffer data)
		{
			// Bounds check for 'b' will be performed automatically
			if ((x < 0) || (y < 0) || (x + w > Width_Renamed) || (y + h > Height_Renamed))
			{
				throw new ArrayIndexOutOfBoundsException("Coordinate out of bounds!");
			}
			int lineOffset = y * ScanlineStride_Renamed + x;
			int srcOffset = 0;

			for (int i = 0; i < h; i++)
			{
			   for (int j = 0; j < w; j++)
			   {
				  int value = data.GetElem(lineOffset + j);
				  value &= ~BitMasks_Renamed[b];
				  int sample = iArray[srcOffset++];
				  value |= ((int)sample << BitOffsets_Renamed[b]) & BitMasks_Renamed[b];
				  data.SetElem(lineOffset + j,value);
			   }
			   lineOffset += ScanlineStride_Renamed;
			}
		}

		public override bool Equals(Object o)
		{
			if ((o == null) || !(o is SinglePixelPackedSampleModel))
			{
				return false;
			}

			SinglePixelPackedSampleModel that = (SinglePixelPackedSampleModel)o;
			return this.Width_Renamed == that.Width_Renamed && this.Height_Renamed == that.Height_Renamed && this.NumBands_Renamed == that.NumBands_Renamed && this.DataType_Renamed == that.DataType_Renamed && Arrays.Equals(this.BitMasks_Renamed, that.BitMasks_Renamed) && Arrays.Equals(this.BitOffsets_Renamed, that.BitOffsets_Renamed) && Arrays.Equals(this.BitSizes, that.BitSizes) && this.MaxBitSize == that.MaxBitSize && this.ScanlineStride_Renamed == that.ScanlineStride_Renamed;
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
			for (int i = 0; i < BitMasks_Renamed.Length; i++)
			{
				hash ^= BitMasks_Renamed[i];
				hash <<= 8;
			}
			for (int i = 0; i < BitOffsets_Renamed.Length; i++)
			{
				hash ^= BitOffsets_Renamed[i];
				hash <<= 8;
			}
			for (int i = 0; i < BitSizes.Length; i++)
			{
				hash ^= BitSizes[i];
				hash <<= 8;
			}
			hash ^= MaxBitSize;
			hash <<= 8;
			hash ^= ScanlineStride_Renamed;
			return hash;
		}
	}

}