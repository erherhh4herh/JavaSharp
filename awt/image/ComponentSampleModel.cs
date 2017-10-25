using System;
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
	///  This class represents image data which is stored such that each sample
	///  of a pixel occupies one data element of the DataBuffer.  It stores the
	///  N samples which make up a pixel in N separate data array elements.
	///  Different bands may be in different banks of the DataBuffer.
	///  Accessor methods are provided so that image data can be manipulated
	///  directly. This class can support different kinds of interleaving, e.g.
	///  band interleaving, scanline interleaving, and pixel interleaving.
	///  Pixel stride is the number of data array elements between two samples
	///  for the same band on the same scanline. Scanline stride is the number
	///  of data array elements between a given sample and the corresponding sample
	///  in the same column of the next scanline.  Band offsets denote the number
	///  of data array elements from the first data array element of the bank
	///  of the DataBuffer holding each band to the first sample of the band.
	///  The bands are numbered from 0 to N-1.  This class can represent image
	///  data for which each sample is an unsigned integral number which can be
	///  stored in 8, 16, or 32 bits (using <code>DataBuffer.TYPE_BYTE</code>,
	///  <code>DataBuffer.TYPE_USHORT</code>, or <code>DataBuffer.TYPE_INT</code>,
	///  respectively), data for which each sample is a signed integral number
	///  which can be stored in 16 bits (using <code>DataBuffer.TYPE_SHORT</code>),
	///  or data for which each sample is a signed float or double quantity
	///  (using <code>DataBuffer.TYPE_FLOAT</code> or
	///  <code>DataBuffer.TYPE_DOUBLE</code>, respectively).
	///  All samples of a given ComponentSampleModel
	///  are stored with the same precision.  All strides and offsets must be
	///  non-negative.  This class supports
	///  <seealso cref="DataBuffer#TYPE_BYTE TYPE_BYTE"/>,
	///  <seealso cref="DataBuffer#TYPE_USHORT TYPE_USHORT"/>,
	///  <seealso cref="DataBuffer#TYPE_SHORT TYPE_SHORT"/>,
	///  <seealso cref="DataBuffer#TYPE_INT TYPE_INT"/>,
	///  <seealso cref="DataBuffer#TYPE_FLOAT TYPE_FLOAT"/>,
	///  <seealso cref="DataBuffer#TYPE_DOUBLE TYPE_DOUBLE"/>, </summary>
	///  <seealso cref= java.awt.image.PixelInterleavedSampleModel </seealso>
	///  <seealso cref= java.awt.image.BandedSampleModel </seealso>

	public class ComponentSampleModel : SampleModel
	{
		/// <summary>
		/// Offsets for all bands in data array elements. </summary>
		protected internal int[] BandOffsets_Renamed;

		/// <summary>
		/// Index for each bank storing a band of image data. </summary>
		protected internal int[] BankIndices_Renamed;

		/// <summary>
		/// The number of bands in this
		/// <code>ComponentSampleModel</code>.
		/// </summary>
		protected internal new int NumBands = 1;

		/// <summary>
		/// The number of banks in this
		/// <code>ComponentSampleModel</code>.
		/// </summary>
		protected internal int NumBanks = 1;

		/// <summary>
		///  Line stride (in data array elements) of the region of image
		///  data described by this ComponentSampleModel.
		/// </summary>
		protected internal int ScanlineStride_Renamed;

		/// <summary>
		/// Pixel stride (in data array elements) of the region of image
		///  data described by this ComponentSampleModel.
		/// </summary>
		protected internal int PixelStride_Renamed;

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern void initIDs();
		static ComponentSampleModel()
		{
			ColorModel.LoadLibraries();
			initIDs();
		}

		/// <summary>
		/// Constructs a ComponentSampleModel with the specified parameters.
		/// The number of bands will be given by the length of the bandOffsets array.
		/// All bands will be stored in the first bank of the DataBuffer. </summary>
		/// <param name="dataType">  the data type for storing samples </param>
		/// <param name="w">         the width (in pixels) of the region of
		///     image data described </param>
		/// <param name="h">         the height (in pixels) of the region of
		///     image data described </param>
		/// <param name="pixelStride"> the pixel stride of the region of image
		///     data described </param>
		/// <param name="scanlineStride"> the line stride of the region of image
		///     data described </param>
		/// <param name="bandOffsets"> the offsets of all bands </param>
		/// <exception cref="IllegalArgumentException"> if <code>w</code> or
		///         <code>h</code> is not greater than 0 </exception>
		/// <exception cref="IllegalArgumentException"> if <code>pixelStride</code>
		///         is less than 0 </exception>
		/// <exception cref="IllegalArgumentException"> if <code>scanlineStride</code>
		///         is less than 0 </exception>
		/// <exception cref="IllegalArgumentException"> if <code>numBands</code>
		///         is less than 1 </exception>
		/// <exception cref="IllegalArgumentException"> if the product of <code>w</code>
		///         and <code>h</code> is greater than
		///         <code>Integer.MAX_VALUE</code> </exception>
		/// <exception cref="IllegalArgumentException"> if <code>dataType</code> is not
		///         one of the supported data types </exception>
		public ComponentSampleModel(int dataType, int w, int h, int pixelStride, int scanlineStride, int[] bandOffsets) : base(dataType, w, h, bandOffsets.Length)
		{
			this.DataType_Renamed = dataType;
			this.PixelStride_Renamed = pixelStride;
			this.ScanlineStride_Renamed = scanlineStride;
			this.BandOffsets_Renamed = (int[])bandOffsets.clone();
			NumBands = this.BandOffsets_Renamed.Length;
			if (pixelStride < 0)
			{
				throw new IllegalArgumentException("Pixel stride must be >= 0");
			}
			// TODO - bug 4296691 - remove this check
			if (scanlineStride < 0)
			{
				throw new IllegalArgumentException("Scanline stride must be >= 0");
			}
			if (NumBands < 1)
			{
				throw new IllegalArgumentException("Must have at least one band.");
			}
			if ((dataType < DataBuffer.TYPE_BYTE) || (dataType > DataBuffer.TYPE_DOUBLE))
			{
				throw new IllegalArgumentException("Unsupported dataType.");
			}
			BankIndices_Renamed = new int[NumBands];
			for (int i = 0; i < NumBands; i++)
			{
				BankIndices_Renamed[i] = 0;
			}
			Verify();
		}


		/// <summary>
		/// Constructs a ComponentSampleModel with the specified parameters.
		/// The number of bands will be given by the length of the bandOffsets array.
		/// Different bands may be stored in different banks of the DataBuffer.
		/// </summary>
		/// <param name="dataType">  the data type for storing samples </param>
		/// <param name="w">         the width (in pixels) of the region of
		///     image data described </param>
		/// <param name="h">         the height (in pixels) of the region of
		///     image data described </param>
		/// <param name="pixelStride"> the pixel stride of the region of image
		///     data described </param>
		/// <param name="scanlineStride"> The line stride of the region of image
		///     data described </param>
		/// <param name="bankIndices"> the bank indices of all bands </param>
		/// <param name="bandOffsets"> the band offsets of all bands </param>
		/// <exception cref="IllegalArgumentException"> if <code>w</code> or
		///         <code>h</code> is not greater than 0 </exception>
		/// <exception cref="IllegalArgumentException"> if <code>pixelStride</code>
		///         is less than 0 </exception>
		/// <exception cref="IllegalArgumentException"> if <code>scanlineStride</code>
		///         is less than 0 </exception>
		/// <exception cref="IllegalArgumentException"> if the length of
		///         <code>bankIndices</code> does not equal the length of
		///         <code>bankOffsets</code> </exception>
		/// <exception cref="IllegalArgumentException"> if any of the bank indices
		///         of <code>bandIndices</code> is less than 0 </exception>
		/// <exception cref="IllegalArgumentException"> if <code>dataType</code> is not
		///         one of the supported data types </exception>
		public ComponentSampleModel(int dataType, int w, int h, int pixelStride, int scanlineStride, int[] bankIndices, int[] bandOffsets) : base(dataType, w, h, bandOffsets.Length)
		{
			this.DataType_Renamed = dataType;
			this.PixelStride_Renamed = pixelStride;
			this.ScanlineStride_Renamed = scanlineStride;
			this.BandOffsets_Renamed = (int[])bandOffsets.clone();
			this.BankIndices_Renamed = (int[]) bankIndices.clone();
			if (pixelStride < 0)
			{
				throw new IllegalArgumentException("Pixel stride must be >= 0");
			}
			// TODO - bug 4296691 - remove this check
			if (scanlineStride < 0)
			{
				throw new IllegalArgumentException("Scanline stride must be >= 0");
			}
			if ((dataType < DataBuffer.TYPE_BYTE) || (dataType > DataBuffer.TYPE_DOUBLE))
			{
				throw new IllegalArgumentException("Unsupported dataType.");
			}
			int maxBank = this.BankIndices_Renamed[0];
			if (maxBank < 0)
			{
				throw new IllegalArgumentException("Index of bank 0 is less than " + "0 (" + maxBank + ")");
			}
			for (int i = 1; i < this.BankIndices_Renamed.Length; i++)
			{
				if (this.BankIndices_Renamed[i] > maxBank)
				{
					maxBank = this.BankIndices_Renamed[i];
				}
				else if (this.BankIndices_Renamed[i] < 0)
				{
					throw new IllegalArgumentException("Index of bank " + i + " is less than 0 (" + maxBank + ")");
				}
			}
			NumBanks = maxBank + 1;
			NumBands = this.BandOffsets_Renamed.Length;
			if (this.BandOffsets_Renamed.Length != this.BankIndices_Renamed.Length)
			{
				throw new IllegalArgumentException("Length of bandOffsets must " + "equal length of bankIndices.");
			}
			Verify();
		}

		private void Verify()
		{
			int requiredSize = BufferSize;
		}

		/// <summary>
		/// Returns the size of the data buffer (in data elements) needed
		/// for a data buffer that matches this ComponentSampleModel.
		/// </summary>
		 private int BufferSize
		 {
			 get
			 {
				 int maxBandOff = BandOffsets_Renamed[0];
				 for (int i = 1; i < BandOffsets_Renamed.Length; i++)
				 {
					 maxBandOff = System.Math.Max(maxBandOff,BandOffsets_Renamed[i]);
				 }
    
				 if (maxBandOff < 0 || maxBandOff > (Integer.MaxValue - 1))
				 {
					 throw new IllegalArgumentException("Invalid band offset");
				 }
    
				 if (PixelStride_Renamed < 0 || PixelStride_Renamed > (Integer.MaxValue / Width_Renamed))
				 {
					 throw new IllegalArgumentException("Invalid pixel stride");
				 }
    
				 if (ScanlineStride_Renamed < 0 || ScanlineStride_Renamed > (Integer.MaxValue / Height_Renamed))
				 {
					 throw new IllegalArgumentException("Invalid scanline stride");
				 }
    
				 int size = maxBandOff + 1;
    
				 int val = PixelStride_Renamed * (Width_Renamed - 1);
    
				 if (val > (Integer.MaxValue - size))
				 {
					 throw new IllegalArgumentException("Invalid pixel stride");
				 }
    
				 size += val;
    
				 val = ScanlineStride_Renamed * (Height_Renamed - 1);
    
				 if (val > (Integer.MaxValue - size))
				 {
					 throw new IllegalArgumentException("Invalid scan stride");
				 }
    
				 size += val;
    
				 return size;
			 }
		 }

		 /// <summary>
		 /// Preserves band ordering with new step factor...
		 /// </summary>
		internal virtual int [] OrderBands(int[] orig, int step)
		{
			int[] map = new int[orig.Length];
			int[] ret = new int[orig.Length];

			for (int i = 0; i < map.Length; i++)
			{
				map[i] = i;
			}

			for (int i = 0; i < ret.Length; i++)
			{
				int index = i;
				for (int j = i + 1; j < ret.Length; j++)
				{
					if (orig[map[index]] > orig[map[j]])
					{
						index = j;
					}
				}
				ret[map[index]] = i * step;
				map[index] = map[i];
			}
			return ret;
		}

		/// <summary>
		/// Creates a new <code>ComponentSampleModel</code> with the specified
		/// width and height.  The new <code>SampleModel</code> will have the same
		/// number of bands, storage data type, interleaving scheme, and
		/// pixel stride as this <code>SampleModel</code>. </summary>
		/// <param name="w"> the width of the resulting <code>SampleModel</code> </param>
		/// <param name="h"> the height of the resulting <code>SampleModel</code> </param>
		/// <returns> a new <code>ComponentSampleModel</code> with the specified size </returns>
		/// <exception cref="IllegalArgumentException"> if <code>w</code> or
		///         <code>h</code> is not greater than 0 </exception>
		public override SampleModel CreateCompatibleSampleModel(int w, int h)
		{
			SampleModel ret = null;
			long size;
			int minBandOff = BandOffsets_Renamed[0];
			int maxBandOff = BandOffsets_Renamed[0];
			for (int i = 1; i < BandOffsets_Renamed.Length; i++)
			{
				minBandOff = System.Math.Min(minBandOff,BandOffsets_Renamed[i]);
				maxBandOff = System.Math.Max(maxBandOff,BandOffsets_Renamed[i]);
			}
			maxBandOff -= minBandOff;

			int bands = BandOffsets_Renamed.Length;
			int[] bandOff;
			int pStride = System.Math.Abs(PixelStride_Renamed);
			int lStride = System.Math.Abs(ScanlineStride_Renamed);
			int bStride = System.Math.Abs(maxBandOff);

			if (pStride > lStride)
			{
				if (pStride > bStride)
				{
					if (lStride > bStride) // pix > line > band
					{
						bandOff = new int[BandOffsets_Renamed.Length];
						for (int i = 0; i < bands; i++)
						{
							bandOff[i] = BandOffsets_Renamed[i] - minBandOff;
						}
						lStride = bStride+1;
						pStride = lStride * h;
					} // pix > band > line
					else
					{
						bandOff = OrderBands(BandOffsets_Renamed,lStride * h);
						pStride = bands * lStride * h;
					}
				} // band > pix > line
				else
				{
					pStride = lStride * h;
					bandOff = OrderBands(BandOffsets_Renamed,pStride * w);
				}
			}
			else
			{
				if (pStride > bStride) // line > pix > band
				{
					bandOff = new int[BandOffsets_Renamed.Length];
					for (int i = 0; i < bands; i++)
					{
						bandOff[i] = BandOffsets_Renamed[i] - minBandOff;
					}
					pStride = bStride+1;
					lStride = pStride * w;
				}
				else
				{
					if (lStride > bStride) // line > band > pix
					{
						bandOff = OrderBands(BandOffsets_Renamed,pStride * w);
						lStride = bands * pStride * w;
					} // band > line > pix
					else
					{
						lStride = pStride * w;
						bandOff = OrderBands(BandOffsets_Renamed,lStride * h);
					}
				}
			}

			// make sure we make room for negative offsets...
			int @base = 0;
			if (ScanlineStride_Renamed < 0)
			{
				@base += lStride * h;
				lStride *= -1;
			}
			if (PixelStride_Renamed < 0)
			{
				@base += pStride * w;
				pStride *= -1;
			}

			for (int i = 0; i < bands; i++)
			{
				bandOff[i] += @base;
			}
			return new ComponentSampleModel(DataType_Renamed, w, h, pStride, lStride, BankIndices_Renamed, bandOff);
		}

		/// <summary>
		/// Creates a new ComponentSampleModel with a subset of the bands
		/// of this ComponentSampleModel.  The new ComponentSampleModel can be
		/// used with any DataBuffer that the existing ComponentSampleModel
		/// can be used with.  The new ComponentSampleModel/DataBuffer
		/// combination will represent an image with a subset of the bands
		/// of the original ComponentSampleModel/DataBuffer combination. </summary>
		/// <param name="bands"> a subset of bands from this
		///              <code>ComponentSampleModel</code> </param>
		/// <returns> a <code>ComponentSampleModel</code> created with a subset
		///          of bands from this <code>ComponentSampleModel</code>. </returns>
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

			return new ComponentSampleModel(this.DataType_Renamed, Width_Renamed, Height_Renamed, this.PixelStride_Renamed, this.ScanlineStride_Renamed, newBankIndices, newBandOffsets);
		}

		/// <summary>
		/// Creates a <code>DataBuffer</code> that corresponds to this
		/// <code>ComponentSampleModel</code>.
		/// The <code>DataBuffer</code> object's data type, number of banks,
		/// and size are be consistent with this <code>ComponentSampleModel</code>. </summary>
		/// <returns> a <code>DataBuffer</code> whose data type, number of banks
		///         and size are consistent with this
		///         <code>ComponentSampleModel</code>. </returns>
		public override DataBuffer CreateDataBuffer()
		{
			DataBuffer dataBuffer = null;

			int size = BufferSize;
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
			}

			return dataBuffer;
		}


		/// <summary>
		/// Gets the offset for the first band of pixel (x,y).
		///  A sample of the first band can be retrieved from a
		/// <code>DataBuffer</code>
		///  <code>data</code> with a <code>ComponentSampleModel</code>
		/// <code>csm</code> as
		/// <pre>
		///        data.getElem(csm.getOffset(x, y));
		/// </pre> </summary>
		/// <param name="x"> the X location of the pixel </param>
		/// <param name="y"> the Y location of the pixel </param>
		/// <returns> the offset for the first band of the specified pixel. </returns>
		public virtual int GetOffset(int x, int y)
		{
			int offset = y * ScanlineStride_Renamed + x * PixelStride_Renamed + BandOffsets_Renamed[0];
			return offset;
		}

		/// <summary>
		/// Gets the offset for band b of pixel (x,y).
		///  A sample of band <code>b</code> can be retrieved from a
		///  <code>DataBuffer</code> <code>data</code>
		///  with a <code>ComponentSampleModel</code> <code>csm</code> as
		/// <pre>
		///       data.getElem(csm.getOffset(x, y, b));
		/// </pre> </summary>
		/// <param name="x"> the X location of the specified pixel </param>
		/// <param name="y"> the Y location of the specified pixel </param>
		/// <param name="b"> the specified band </param>
		/// <returns> the offset for the specified band of the specified pixel. </returns>
		public virtual int GetOffset(int x, int y, int b)
		{
			int offset = y * ScanlineStride_Renamed + x * PixelStride_Renamed + BandOffsets_Renamed[b];
			return offset;
		}

		/// <summary>
		/// Returns the number of bits per sample for all bands. </summary>
		///  <returns> an array containing the number of bits per sample
		///          for all bands, where each element in the array
		///          represents a band. </returns>
		public sealed override int[] SampleSize
		{
			get
			{
				int[] sampleSize = new int [NumBands];
				int sizeInBits = GetSampleSize(0);
    
				for (int i = 0; i < NumBands; i++)
				{
					sampleSize[i] = sizeInBits;
				}
    
				return sampleSize;
			}
		}

		/// <summary>
		/// Returns the number of bits per sample for the specified band. </summary>
		///  <param name="band"> the specified band </param>
		///  <returns> the number of bits per sample for the specified band. </returns>
		public sealed override int GetSampleSize(int band)
		{
			return DataBuffer.GetDataTypeSize(DataType_Renamed);
		}

		/// <summary>
		/// Returns the bank indices for all bands. </summary>
		///  <returns> the bank indices for all bands. </returns>
		public int [] BankIndices
		{
			get
			{
				return (int[]) BankIndices_Renamed.clone();
			}
		}

		/// <summary>
		/// Returns the band offset for all bands. </summary>
		///  <returns> the band offsets for all bands. </returns>
		public int [] BandOffsets
		{
			get
			{
				return (int[])BandOffsets_Renamed.clone();
			}
		}

		/// <summary>
		/// Returns the scanline stride of this ComponentSampleModel. </summary>
		///  <returns> the scanline stride of this <code>ComponentSampleModel</code>. </returns>
		public int ScanlineStride
		{
			get
			{
				return ScanlineStride_Renamed;
			}
		}

		/// <summary>
		/// Returns the pixel stride of this ComponentSampleModel. </summary>
		///  <returns> the pixel stride of this <code>ComponentSampleModel</code>. </returns>
		public int PixelStride
		{
			get
			{
				return PixelStride_Renamed;
			}
		}

		/// <summary>
		/// Returns the number of data elements needed to transfer a pixel
		/// with the
		/// <seealso cref="#getDataElements(int, int, Object, DataBuffer) "/> and
		/// <seealso cref="#setDataElements(int, int, Object, DataBuffer) "/>
		/// methods.
		/// For a <code>ComponentSampleModel</code>, this is identical to the
		/// number of bands. </summary>
		/// <returns> the number of data elements needed to transfer a pixel with
		///         the <code>getDataElements</code> and
		///         <code>setDataElements</code> methods. </returns>
		/// <seealso cref= java.awt.image.SampleModel#getNumDataElements </seealso>
		/// <seealso cref= #getNumBands </seealso>
		public sealed override int NumDataElements
		{
			get
			{
				return NumBands;
			}
		}

		/// <summary>
		/// Returns data for a single pixel in a primitive array of type
		/// <code>TransferType</code>.  For a <code>ComponentSampleModel</code>,
		/// this is the same as the data type, and samples are returned
		/// one per array element.  Generally, <code>obj</code> should
		/// be passed in as <code>null</code>, so that the <code>Object</code>
		/// is created automatically and is the right primitive data type.
		/// <para>
		/// The following code illustrates transferring data for one pixel from
		/// <code>DataBuffer</code> <code>db1</code>, whose storage layout is
		/// described by <code>ComponentSampleModel</code> <code>csm1</code>,
		/// to <code>DataBuffer</code> <code>db2</code>, whose storage layout
		/// is described by <code>ComponentSampleModel</code> <code>csm2</code>.
		/// The transfer is usually more efficient than using
		/// <code>getPixel</code> and <code>setPixel</code>.
		/// <pre>
		///       ComponentSampleModel csm1, csm2;
		///       DataBufferInt db1, db2;
		///       csm2.setDataElements(x, y,
		///                            csm1.getDataElements(x, y, null, db1), db2);
		/// </pre>
		/// 
		/// Using <code>getDataElements</code> and <code>setDataElements</code>
		/// to transfer between two <code>DataBuffer/SampleModel</code>
		/// pairs is legitimate if the <code>SampleModel</code> objects have
		/// the same number of bands, corresponding bands have the same number of
		/// bits per sample, and the <code>TransferType</code>s are the same.
		/// </para>
		/// <para>
		/// If <code>obj</code> is not <code>null</code>, it should be a
		/// primitive array of type <code>TransferType</code>.
		/// Otherwise, a <code>ClassCastException</code> is thrown.  An
		/// <code>ArrayIndexOutOfBoundsException</code> might be thrown if the
		/// coordinates are not in bounds, or if <code>obj</code> is not
		/// <code>null</code> and is not large enough to hold
		/// the pixel data.
		/// 
		/// </para>
		/// </summary>
		/// <param name="x">         the X coordinate of the pixel location </param>
		/// <param name="y">         the Y coordinate of the pixel location </param>
		/// <param name="obj">       if non-<code>null</code>, a primitive array
		///                  in which to return the pixel data </param>
		/// <param name="data">      the <code>DataBuffer</code> containing the image data </param>
		/// <returns> the data of the specified pixel </returns>
		/// <seealso cref= #setDataElements(int, int, Object, DataBuffer)
		/// </seealso>
		/// <exception cref="NullPointerException"> if data is null. </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates are
		/// not in bounds, or if obj is too small to hold the output. </exception>
		public override Object GetDataElements(int x, int y, Object obj, DataBuffer data)
		{
			if ((x < 0) || (y < 0) || (x >= Width_Renamed) || (y >= Height_Renamed))
			{
				throw new ArrayIndexOutOfBoundsException("Coordinate out of bounds!");
			}

			int type = TransferType;
			int numDataElems = NumDataElements;
			int pixelOffset = y * ScanlineStride_Renamed + x * PixelStride_Renamed;

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
		/// Returns all samples for the specified pixel in an int array,
		/// one sample per array element.
		/// An <code>ArrayIndexOutOfBoundsException</code> might be thrown if
		/// the coordinates are not in bounds. </summary>
		/// <param name="x">         the X coordinate of the pixel location </param>
		/// <param name="y">         the Y coordinate of the pixel location </param>
		/// <param name="iArray">    If non-null, returns the samples in this array </param>
		/// <param name="data">      The DataBuffer containing the image data </param>
		/// <returns> the samples of the specified pixel. </returns>
		/// <seealso cref= #setPixel(int, int, int[], DataBuffer)
		/// </seealso>
		/// <exception cref="NullPointerException"> if data is null. </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates are
		/// not in bounds, or if iArray is too small to hold the output. </exception>
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
			int pixelOffset = y * ScanlineStride_Renamed + x * PixelStride_Renamed;
			for (int i = 0; i < NumBands; i++)
			{
				pixels[i] = data.GetElem(BankIndices_Renamed[i], pixelOffset + BandOffsets_Renamed[i]);
			}
			return pixels;
		}

		/// <summary>
		/// Returns all samples for the specified rectangle of pixels in
		/// an int array, one sample per array element.
		/// An <code>ArrayIndexOutOfBoundsException</code> might be thrown if
		/// the coordinates are not in bounds. </summary>
		/// <param name="x">         The X coordinate of the upper left pixel location </param>
		/// <param name="y">         The Y coordinate of the upper left pixel location </param>
		/// <param name="w">         The width of the pixel rectangle </param>
		/// <param name="h">         The height of the pixel rectangle </param>
		/// <param name="iArray">    If non-null, returns the samples in this array </param>
		/// <param name="data">      The DataBuffer containing the image data </param>
		/// <returns> the samples of the pixels within the specified region. </returns>
		/// <seealso cref= #setPixels(int, int, int, int, int[], DataBuffer) </seealso>
		public override int[] GetPixels(int x, int y, int w, int h, int[] iArray, DataBuffer data)
		{
			int x1 = x + w;
			int y1 = y + h;

			if (x < 0 || x >= Width_Renamed || w > Width_Renamed || x1 < 0 || x1 > Width_Renamed || y < 0 || y >= Height_Renamed || y > Height_Renamed || y1 < 0 || y1 > Height_Renamed)
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
			   pixels = new int [w * h * NumBands];
			}
			int lineOffset = y * ScanlineStride_Renamed + x * PixelStride_Renamed;
			int srcOffset = 0;

			for (int i = 0; i < h; i++)
			{
			   int pixelOffset = lineOffset;
			   for (int j = 0; j < w; j++)
			   {
				  for (int k = 0; k < NumBands; k++)
				  {
					 pixels[srcOffset++] = data.GetElem(BankIndices_Renamed[k], pixelOffset + BandOffsets_Renamed[k]);
				  }
				  pixelOffset += PixelStride_Renamed;
			   }
			   lineOffset += ScanlineStride_Renamed;
			}
			return pixels;
		}

		/// <summary>
		/// Returns as int the sample in a specified band for the pixel
		/// located at (x,y).
		/// An <code>ArrayIndexOutOfBoundsException</code> might be thrown if
		/// the coordinates are not in bounds. </summary>
		/// <param name="x">         the X coordinate of the pixel location </param>
		/// <param name="y">         the Y coordinate of the pixel location </param>
		/// <param name="b">         the band to return </param>
		/// <param name="data">      the <code>DataBuffer</code> containing the image data </param>
		/// <returns> the sample in a specified band for the specified pixel </returns>
		/// <seealso cref= #setSample(int, int, int, int, DataBuffer) </seealso>
		public override int GetSample(int x, int y, int b, DataBuffer data)
		{
			// Bounds check for 'b' will be performed automatically
			if ((x < 0) || (y < 0) || (x >= Width_Renamed) || (y >= Height_Renamed))
			{
				throw new ArrayIndexOutOfBoundsException("Coordinate out of bounds!");
			}
			int sample = data.GetElem(BankIndices_Renamed[b], y * ScanlineStride_Renamed + x * PixelStride_Renamed + BandOffsets_Renamed[b]);
			return sample;
		}

		/// <summary>
		/// Returns the sample in a specified band
		/// for the pixel located at (x,y) as a float.
		/// An <code>ArrayIndexOutOfBoundsException</code> might be
		/// thrown if the coordinates are not in bounds. </summary>
		/// <param name="x">         The X coordinate of the pixel location </param>
		/// <param name="y">         The Y coordinate of the pixel location </param>
		/// <param name="b">         The band to return </param>
		/// <param name="data">      The DataBuffer containing the image data </param>
		/// <returns> a float value representing the sample in the specified
		/// band for the specified pixel. </returns>
		public override float GetSampleFloat(int x, int y, int b, DataBuffer data)
		{
			// Bounds check for 'b' will be performed automatically
			if ((x < 0) || (y < 0) || (x >= Width_Renamed) || (y >= Height_Renamed))
			{
				throw new ArrayIndexOutOfBoundsException("Coordinate out of bounds!");
			}

			float sample = data.GetElemFloat(BankIndices_Renamed[b], y * ScanlineStride_Renamed + x * PixelStride_Renamed + BandOffsets_Renamed[b]);
			return sample;
		}

		/// <summary>
		/// Returns the sample in a specified band
		/// for a pixel located at (x,y) as a double.
		/// An <code>ArrayIndexOutOfBoundsException</code> might be
		/// thrown if the coordinates are not in bounds. </summary>
		/// <param name="x">         The X coordinate of the pixel location </param>
		/// <param name="y">         The Y coordinate of the pixel location </param>
		/// <param name="b">         The band to return </param>
		/// <param name="data">      The DataBuffer containing the image data </param>
		/// <returns> a double value representing the sample in the specified
		/// band for the specified pixel. </returns>
		public override double GetSampleDouble(int x, int y, int b, DataBuffer data)
		{
			// Bounds check for 'b' will be performed automatically
			if ((x < 0) || (y < 0) || (x >= Width_Renamed) || (y >= Height_Renamed))
			{
				throw new ArrayIndexOutOfBoundsException("Coordinate out of bounds!");
			}

			double sample = data.GetElemDouble(BankIndices_Renamed[b], y * ScanlineStride_Renamed + x * PixelStride_Renamed + BandOffsets_Renamed[b]);
			return sample;
		}

		/// <summary>
		/// Returns the samples in a specified band for the specified rectangle
		/// of pixels in an int array, one sample per data array element.
		/// An <code>ArrayIndexOutOfBoundsException</code> might be thrown if
		/// the coordinates are not in bounds. </summary>
		/// <param name="x">         The X coordinate of the upper left pixel location </param>
		/// <param name="y">         The Y coordinate of the upper left pixel location </param>
		/// <param name="w">         the width of the pixel rectangle </param>
		/// <param name="h">         the height of the pixel rectangle </param>
		/// <param name="b">         the band to return </param>
		/// <param name="iArray">    if non-<code>null</code>, returns the samples
		///                  in this array </param>
		/// <param name="data">      the <code>DataBuffer</code> containing the image data </param>
		/// <returns> the samples in the specified band of the specified pixel </returns>
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
			int lineOffset = y * ScanlineStride_Renamed + x * PixelStride_Renamed + BandOffsets_Renamed[b];
			int srcOffset = 0;

			for (int i = 0; i < h; i++)
			{
			   int sampleOffset = lineOffset;
			   for (int j = 0; j < w; j++)
			   {
				  samples[srcOffset++] = data.GetElem(BankIndices_Renamed[b], sampleOffset);
				  sampleOffset += PixelStride_Renamed;
			   }
			   lineOffset += ScanlineStride_Renamed;
			}
			return samples;
		}

		/// <summary>
		/// Sets the data for a single pixel in the specified
		/// <code>DataBuffer</code> from a primitive array of type
		/// <code>TransferType</code>.  For a <code>ComponentSampleModel</code>,
		/// this is the same as the data type, and samples are transferred
		/// one per array element.
		/// <para>
		/// The following code illustrates transferring data for one pixel from
		/// <code>DataBuffer</code> <code>db1</code>, whose storage layout is
		/// described by <code>ComponentSampleModel</code> <code>csm1</code>,
		/// to <code>DataBuffer</code> <code>db2</code>, whose storage layout
		/// is described by <code>ComponentSampleModel</code> <code>csm2</code>.
		/// The transfer is usually more efficient than using
		/// <code>getPixel</code> and <code>setPixel</code>.
		/// <pre>
		///       ComponentSampleModel csm1, csm2;
		///       DataBufferInt db1, db2;
		///       csm2.setDataElements(x, y, csm1.getDataElements(x, y, null, db1),
		///                            db2);
		/// </pre>
		/// Using <code>getDataElements</code> and <code>setDataElements</code>
		/// to transfer between two <code>DataBuffer/SampleModel</code> pairs
		/// is legitimate if the <code>SampleModel</code> objects have
		/// the same number of bands, corresponding bands have the same number of
		/// bits per sample, and the <code>TransferType</code>s are the same.
		/// </para>
		/// <para>
		/// A <code>ClassCastException</code> is thrown if <code>obj</code> is not
		/// a primitive array of type <code>TransferType</code>.
		/// An <code>ArrayIndexOutOfBoundsException</code> might be thrown if
		/// the coordinates are not in bounds, or if <code>obj</code> is not large
		/// enough to hold the pixel data.
		/// </para>
		/// </summary>
		/// <param name="x">         the X coordinate of the pixel location </param>
		/// <param name="y">         the Y coordinate of the pixel location </param>
		/// <param name="obj">       a primitive array containing pixel data </param>
		/// <param name="data">      the DataBuffer containing the image data </param>
		/// <seealso cref= #getDataElements(int, int, Object, DataBuffer) </seealso>
		public override void SetDataElements(int x, int y, Object obj, DataBuffer data)
		{
			if ((x < 0) || (y < 0) || (x >= Width_Renamed) || (y >= Height_Renamed))
			{
				throw new ArrayIndexOutOfBoundsException("Coordinate out of bounds!");
			}

			int type = TransferType;
			int numDataElems = NumDataElements;
			int pixelOffset = y * ScanlineStride_Renamed + x * PixelStride_Renamed;

			switch (type)
			{

			case DataBuffer.TYPE_BYTE:

				sbyte[] barray = (sbyte[])obj;

				for (int i = 0; i < numDataElems; i++)
				{
					data.SetElem(BankIndices_Renamed[i], pixelOffset + BandOffsets_Renamed[i], ((int)barray[i]) & 0xff);
				}
				break;

			case DataBuffer.TYPE_USHORT:
			case DataBuffer.TYPE_SHORT:

				short[] sarray = (short[])obj;

				for (int i = 0; i < numDataElems; i++)
				{
					data.SetElem(BankIndices_Renamed[i], pixelOffset + BandOffsets_Renamed[i], ((int)sarray[i]) & 0xffff);
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
		/// Sets a pixel in the <code>DataBuffer</code> using an int array of
		/// samples for input.  An <code>ArrayIndexOutOfBoundsException</code>
		/// might be thrown if the coordinates are
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
		   int pixelOffset = y * ScanlineStride_Renamed + x * PixelStride_Renamed;
		   for (int i = 0; i < NumBands; i++)
		   {
			   data.SetElem(BankIndices_Renamed[i], pixelOffset + BandOffsets_Renamed[i],iArray[i]);
		   }
		}

		/// <summary>
		/// Sets all samples for a rectangle of pixels from an int array containing
		/// one sample per array element.
		/// An <code>ArrayIndexOutOfBoundsException</code> might be thrown if the
		/// coordinates are not in bounds. </summary>
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

			int lineOffset = y * ScanlineStride_Renamed + x * PixelStride_Renamed;
			int srcOffset = 0;

			for (int i = 0; i < h; i++)
			{
			   int pixelOffset = lineOffset;
			   for (int j = 0; j < w; j++)
			   {
				  for (int k = 0; k < NumBands; k++)
				  {
					 data.SetElem(BankIndices_Renamed[k], pixelOffset + BandOffsets_Renamed[k], iArray[srcOffset++]);
				  }
				  pixelOffset += PixelStride_Renamed;
			   }
			   lineOffset += ScanlineStride_Renamed;
			}
		}

		/// <summary>
		/// Sets a sample in the specified band for the pixel located at (x,y)
		/// in the <code>DataBuffer</code> using an int for input.
		/// An <code>ArrayIndexOutOfBoundsException</code> might be thrown if the
		/// coordinates are not in bounds. </summary>
		/// <param name="x">         The X coordinate of the pixel location </param>
		/// <param name="y">         The Y coordinate of the pixel location </param>
		/// <param name="b">         the band to set </param>
		/// <param name="s">         the input sample as an int </param>
		/// <param name="data">      the DataBuffer containing the image data </param>
		/// <seealso cref= #getSample(int, int, int, DataBuffer) </seealso>
		public override void SetSample(int x, int y, int b, int s, DataBuffer data)
		{
			// Bounds check for 'b' will be performed automatically
			if ((x < 0) || (y < 0) || (x >= Width_Renamed) || (y >= Height_Renamed))
			{
				throw new ArrayIndexOutOfBoundsException("Coordinate out of bounds!");
			}
			data.SetElem(BankIndices_Renamed[b], y * ScanlineStride_Renamed + x * PixelStride_Renamed + BandOffsets_Renamed[b], s);
		}

		/// <summary>
		/// Sets a sample in the specified band for the pixel located at (x,y)
		/// in the <code>DataBuffer</code> using a float for input.
		/// An <code>ArrayIndexOutOfBoundsException</code> might be thrown if
		/// the coordinates are not in bounds. </summary>
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
			data.SetElemFloat(BankIndices_Renamed[b], y * ScanlineStride_Renamed + x * PixelStride_Renamed + BandOffsets_Renamed[b], s);
		}

		/// <summary>
		/// Sets a sample in the specified band for the pixel located at (x,y)
		/// in the <code>DataBuffer</code> using a double for input.
		/// An <code>ArrayIndexOutOfBoundsException</code> might be thrown if
		/// the coordinates are not in bounds. </summary>
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
			data.SetElemDouble(BankIndices_Renamed[b], y * ScanlineStride_Renamed + x * PixelStride_Renamed + BandOffsets_Renamed[b], s);
		}

		/// <summary>
		/// Sets the samples in the specified band for the specified rectangle
		/// of pixels from an int array containing one sample per data array element.
		/// An <code>ArrayIndexOutOfBoundsException</code> might be thrown if the
		/// coordinates are not in bounds. </summary>
		/// <param name="x">         The X coordinate of the upper left pixel location </param>
		/// <param name="y">         The Y coordinate of the upper left pixel location </param>
		/// <param name="w">         The width of the pixel rectangle </param>
		/// <param name="h">         The height of the pixel rectangle </param>
		/// <param name="b">         The band to set </param>
		/// <param name="iArray">    The input samples in an int array </param>
		/// <param name="data">      The DataBuffer containing the image data </param>
		/// <seealso cref= #getSamples(int, int, int, int, int, int[], DataBuffer) </seealso>
		public override void SetSamples(int x, int y, int w, int h, int b, int[] iArray, DataBuffer data)
		{
			// Bounds check for 'b' will be performed automatically
			if ((x < 0) || (y < 0) || (x + w > Width_Renamed) || (y + h > Height_Renamed))
			{
				throw new ArrayIndexOutOfBoundsException("Coordinate out of bounds!");
			}
			int lineOffset = y * ScanlineStride_Renamed + x * PixelStride_Renamed + BandOffsets_Renamed[b];
			int srcOffset = 0;

			for (int i = 0; i < h; i++)
			{
			   int sampleOffset = lineOffset;
			   for (int j = 0; j < w; j++)
			   {
				  data.SetElem(BankIndices_Renamed[b], sampleOffset, iArray[srcOffset++]);
				  sampleOffset += PixelStride_Renamed;
			   }
			   lineOffset += ScanlineStride_Renamed;
			}
		}

		public override bool Equals(Object o)
		{
			if ((o == null) || !(o is ComponentSampleModel))
			{
				return false;
			}

			ComponentSampleModel that = (ComponentSampleModel)o;
			return this.Width_Renamed == that.Width_Renamed && this.Height_Renamed == that.Height_Renamed && this.NumBands == that.NumBands && this.DataType_Renamed == that.DataType_Renamed && Arrays.Equals(this.BandOffsets_Renamed, that.BandOffsets_Renamed) && Arrays.Equals(this.BankIndices_Renamed, that.BankIndices_Renamed) && this.NumBands == that.NumBands && this.NumBanks == that.NumBanks && this.ScanlineStride_Renamed == that.ScanlineStride_Renamed && this.PixelStride_Renamed == that.PixelStride_Renamed;
		}

		// If we implement equals() we must also implement hashCode
		public override int HashCode()
		{
			int hash = 0;
			hash = Width_Renamed;
			hash <<= 8;
			hash ^= Height_Renamed;
			hash <<= 8;
			hash ^= NumBands;
			hash <<= 8;
			hash ^= DataType_Renamed;
			hash <<= 8;
			for (int i = 0; i < BandOffsets_Renamed.Length; i++)
			{
				hash ^= BandOffsets_Renamed[i];
				hash <<= 8;
			}
			for (int i = 0; i < BankIndices_Renamed.Length; i++)
			{
				hash ^= BankIndices_Renamed[i];
				hash <<= 8;
			}
			hash ^= NumBands;
			hash <<= 8;
			hash ^= NumBanks;
			hash <<= 8;
			hash ^= ScanlineStride_Renamed;
			hash <<= 8;
			hash ^= PixelStride_Renamed;
			return hash;
		}
	}

}