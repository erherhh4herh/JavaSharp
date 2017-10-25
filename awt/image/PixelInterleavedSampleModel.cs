using System;

/*
 * Copyright (c) 1998, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.awt.image
{

	/// <summary>
	///  This class represents image data which is stored in a pixel interleaved
	///  fashion and for
	///  which each sample of a pixel occupies one data element of the DataBuffer.
	///  It subclasses ComponentSampleModel but provides a more efficient
	///  implementation for accessing pixel interleaved image data than is provided
	///  by ComponentSampleModel.  This class
	///  stores sample data for all bands in a single bank of the
	///  DataBuffer. Accessor methods are provided so that image data can be
	///  manipulated directly. Pixel stride is the number of
	///  data array elements between two samples for the same band on the same
	///  scanline. Scanline stride is the number of data array elements between
	///  a given sample and the corresponding sample in the same column of the next
	///  scanline.  Band offsets denote the number
	///  of data array elements from the first data array element of the bank
	///  of the DataBuffer holding each band to the first sample of the band.
	///  The bands are numbered from 0 to N-1.
	///  Bank indices denote the correspondence between a bank of the data buffer
	///  and a band of image data.
	///  This class supports
	///  <seealso cref="DataBuffer#TYPE_BYTE TYPE_BYTE"/>,
	///  <seealso cref="DataBuffer#TYPE_USHORT TYPE_USHORT"/>,
	///  <seealso cref="DataBuffer#TYPE_SHORT TYPE_SHORT"/>,
	///  <seealso cref="DataBuffer#TYPE_INT TYPE_INT"/>,
	///  <seealso cref="DataBuffer#TYPE_FLOAT TYPE_FLOAT"/> and
	///  <seealso cref="DataBuffer#TYPE_DOUBLE TYPE_DOUBLE"/> datatypes.
	/// </summary>

	public class PixelInterleavedSampleModel : ComponentSampleModel
	{
		/// <summary>
		/// Constructs a PixelInterleavedSampleModel with the specified parameters.
		/// The number of bands will be given by the length of the bandOffsets
		/// array. </summary>
		/// <param name="dataType">  The data type for storing samples. </param>
		/// <param name="w">         The width (in pixels) of the region of
		///                  image data described. </param>
		/// <param name="h">         The height (in pixels) of the region of
		///                  image data described. </param>
		/// <param name="pixelStride"> The pixel stride of the image data. </param>
		/// <param name="scanlineStride"> The line stride of the image data. </param>
		/// <param name="bandOffsets"> The offsets of all bands. </param>
		/// <exception cref="IllegalArgumentException"> if <code>w</code> or
		///         <code>h</code> is not greater than 0 </exception>
		/// <exception cref="IllegalArgumentException"> if any offset between bands is
		///         greater than the scanline stride </exception>
		/// <exception cref="IllegalArgumentException"> if the product of
		///         <code>pixelStride</code> and <code>w</code> is greater
		///         than <code>scanlineStride</code> </exception>
		/// <exception cref="IllegalArgumentException"> if <code>pixelStride</code> is
		///         less than any offset between bands </exception>
		/// <exception cref="IllegalArgumentException"> if <code>dataType</code> is not
		///         one of the supported data types </exception>
		public PixelInterleavedSampleModel(int dataType, int w, int h, int pixelStride, int scanlineStride, int[] bandOffsets) : base(dataType, w, h, pixelStride, scanlineStride, bandOffsets)
		{
			int minBandOff = this.BandOffsets_Renamed[0];
			int maxBandOff = this.BandOffsets_Renamed[0];
			for (int i = 1; i < this.BandOffsets_Renamed.Length; i++)
			{
				minBandOff = System.Math.Min(minBandOff,this.BandOffsets_Renamed[i]);
				maxBandOff = System.Math.Max(maxBandOff,this.BandOffsets_Renamed[i]);
			}
			maxBandOff -= minBandOff;
			if (maxBandOff > scanlineStride)
			{
				throw new IllegalArgumentException("Offsets between bands must be" + " less than the scanline " + " stride");
			}
			if (pixelStride * w > scanlineStride)
			{
				throw new IllegalArgumentException("Pixel stride times width " + "must be less than or " + "equal to the scanline " + "stride");
			}
			if (pixelStride < maxBandOff)
			{
				throw new IllegalArgumentException("Pixel stride must be greater" + " than or equal to the offsets" + " between bands");
			}
		}

		/// <summary>
		/// Creates a new PixelInterleavedSampleModel with the specified
		/// width and height.  The new PixelInterleavedSampleModel will have the
		/// same number of bands, storage data type, and pixel stride
		/// as this PixelInterleavedSampleModel.  The band offsets may be
		/// compressed such that the minimum of all of the band offsets is zero. </summary>
		/// <param name="w"> the width of the resulting <code>SampleModel</code> </param>
		/// <param name="h"> the height of the resulting <code>SampleModel</code> </param>
		/// <returns> a new <code>SampleModel</code> with the specified width
		///         and height. </returns>
		/// <exception cref="IllegalArgumentException"> if <code>w</code> or
		///         <code>h</code> is not greater than 0 </exception>
		public override SampleModel CreateCompatibleSampleModel(int w, int h)
		{
			int minBandoff = BandOffsets_Renamed[0];
			int numBands = BandOffsets_Renamed.Length;
			for (int i = 1; i < numBands; i++)
			{
				if (BandOffsets_Renamed[i] < minBandoff)
				{
					minBandoff = BandOffsets_Renamed[i];
				}
			}
			int[] bandOff;
			if (minBandoff > 0)
			{
				bandOff = new int[numBands];
				for (int i = 0; i < numBands; i++)
				{
					bandOff[i] = BandOffsets_Renamed[i] - minBandoff;
				}
			}
			else
			{
				bandOff = BandOffsets_Renamed;
			}
			return new PixelInterleavedSampleModel(DataType_Renamed, w, h, PixelStride_Renamed, PixelStride_Renamed * w, bandOff);
		}

		/// <summary>
		/// Creates a new PixelInterleavedSampleModel with a subset of the
		/// bands of this PixelInterleavedSampleModel.  The new
		/// PixelInterleavedSampleModel can be used with any DataBuffer that the
		/// existing PixelInterleavedSampleModel can be used with.  The new
		/// PixelInterleavedSampleModel/DataBuffer combination will represent
		/// an image with a subset of the bands of the original
		/// PixelInterleavedSampleModel/DataBuffer combination.
		/// </summary>
		public override SampleModel CreateSubsetSampleModel(int[] bands)
		{
			int[] newBandOffsets = new int[bands.Length];
			for (int i = 0; i < bands.Length; i++)
			{
				newBandOffsets[i] = BandOffsets_Renamed[bands[i]];
			}
			return new PixelInterleavedSampleModel(this.DataType_Renamed, Width_Renamed, Height_Renamed, this.PixelStride_Renamed, ScanlineStride_Renamed, newBandOffsets);
		}

		// Differentiate hash code from other ComponentSampleModel subclasses
		public override int HashCode()
		{
			return base.HashCode() ^ 0x1;
		}
	}

}