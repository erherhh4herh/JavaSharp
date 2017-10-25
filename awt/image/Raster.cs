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

	using ByteInterleavedRaster = sun.awt.image.ByteInterleavedRaster;
	using ShortInterleavedRaster = sun.awt.image.ShortInterleavedRaster;
	using IntegerInterleavedRaster = sun.awt.image.IntegerInterleavedRaster;
	using ByteBandedRaster = sun.awt.image.ByteBandedRaster;
	using ShortBandedRaster = sun.awt.image.ShortBandedRaster;
	using BytePackedRaster = sun.awt.image.BytePackedRaster;
	using SunWritableRaster = sun.awt.image.SunWritableRaster;

	/// <summary>
	/// A class representing a rectangular array of pixels.  A Raster
	/// encapsulates a DataBuffer that stores the sample values and a
	/// SampleModel that describes how to locate a given sample value in a
	/// DataBuffer.
	/// <para>
	/// A Raster defines values for pixels occupying a particular
	/// rectangular area of the plane, not necessarily including (0, 0).
	/// The rectangle, known as the Raster's bounding rectangle and
	/// available by means of the getBounds method, is defined by minX,
	/// minY, width, and height values.  The minX and minY values define
	/// the coordinate of the upper left corner of the Raster.  References
	/// to pixels outside of the bounding rectangle may result in an
	/// exception being thrown, or may result in references to unintended
	/// elements of the Raster's associated DataBuffer.  It is the user's
	/// responsibility to avoid accessing such pixels.
	/// </para>
	/// <para>
	/// A SampleModel describes how samples of a Raster
	/// are stored in the primitive array elements of a DataBuffer.
	/// Samples may be stored one per data element, as in a
	/// PixelInterleavedSampleModel or BandedSampleModel, or packed several to
	/// an element, as in a SinglePixelPackedSampleModel or
	/// MultiPixelPackedSampleModel.  The SampleModel is also
	/// controls whether samples are sign extended, allowing unsigned
	/// data to be stored in signed Java data types such as byte, short, and
	/// int.
	/// </para>
	/// <para>
	/// Although a Raster may live anywhere in the plane, a SampleModel
	/// makes use of a simple coordinate system that starts at (0, 0).  A
	/// Raster therefore contains a translation factor that allows pixel
	/// locations to be mapped between the Raster's coordinate system and
	/// that of the SampleModel.  The translation from the SampleModel
	/// coordinate system to that of the Raster may be obtained by the
	/// getSampleModelTranslateX and getSampleModelTranslateY methods.
	/// </para>
	/// <para>
	/// A Raster may share a DataBuffer with another Raster either by
	/// explicit construction or by the use of the createChild and
	/// createTranslatedChild methods.  Rasters created by these methods
	/// can return a reference to the Raster they were created from by
	/// means of the getParent method.  For a Raster that was not
	/// constructed by means of a call to createTranslatedChild or
	/// createChild, getParent will return null.
	/// </para>
	/// <para>
	/// The createTranslatedChild method returns a new Raster that
	/// shares all of the data of the current Raster, but occupies a
	/// bounding rectangle of the same width and height but with a
	/// different starting point.  For example, if the parent Raster
	/// occupied the region (10, 10) to (100, 100), and the translated
	/// Raster was defined to start at (50, 50), then pixel (20, 20) of the
	/// parent and pixel (60, 60) of the child occupy the same location in
	/// the DataBuffer shared by the two Rasters.  In the first case, (-10,
	/// -10) should be added to a pixel coordinate to obtain the
	/// corresponding SampleModel coordinate, and in the second case (-50,
	/// -50) should be added.
	/// </para>
	/// <para>
	/// The translation between a parent and child Raster may be
	/// determined by subtracting the child's sampleModelTranslateX and
	/// sampleModelTranslateY values from those of the parent.
	/// </para>
	/// <para>
	/// The createChild method may be used to create a new Raster
	/// occupying only a subset of its parent's bounding rectangle
	/// (with the same or a translated coordinate system) or
	/// with a subset of the bands of its parent.
	/// </para>
	/// <para>
	/// All constructors are protected.  The correct way to create a
	/// Raster is to use one of the static create methods defined in this
	/// class.  These methods create instances of Raster that use the
	/// standard Interleaved, Banded, and Packed SampleModels and that may
	/// be processed more efficiently than a Raster created by combining
	/// an externally generated SampleModel and DataBuffer.
	/// </para>
	/// </summary>
	/// <seealso cref= java.awt.image.DataBuffer </seealso>
	/// <seealso cref= java.awt.image.SampleModel </seealso>
	/// <seealso cref= java.awt.image.PixelInterleavedSampleModel </seealso>
	/// <seealso cref= java.awt.image.BandedSampleModel </seealso>
	/// <seealso cref= java.awt.image.SinglePixelPackedSampleModel </seealso>
	/// <seealso cref= java.awt.image.MultiPixelPackedSampleModel </seealso>
	public class Raster
	{

		/// <summary>
		/// The SampleModel that describes how pixels from this Raster
		/// are stored in the DataBuffer.
		/// </summary>
		protected internal SampleModel SampleModel_Renamed;

		/// <summary>
		/// The DataBuffer that stores the image data. </summary>
		protected internal DataBuffer DataBuffer_Renamed;

		/// <summary>
		/// The X coordinate of the upper-left pixel of this Raster. </summary>
		protected internal int MinX_Renamed;

		/// <summary>
		/// The Y coordinate of the upper-left pixel of this Raster. </summary>
		protected internal int MinY_Renamed;

		/// <summary>
		/// The width of this Raster. </summary>
		protected internal int Width_Renamed;

		/// <summary>
		/// The height of this Raster. </summary>
		protected internal int Height_Renamed;

		/// <summary>
		/// The X translation from the coordinate space of the
		/// Raster's SampleModel to that of the Raster.
		/// </summary>
		protected internal int SampleModelTranslateX_Renamed;

		/// <summary>
		/// The Y translation from the coordinate space of the
		/// Raster's SampleModel to that of the Raster.
		/// </summary>
		protected internal int SampleModelTranslateY_Renamed;

		/// <summary>
		/// The number of bands in the Raster. </summary>
		protected internal int NumBands_Renamed;

		/// <summary>
		/// The number of DataBuffer data elements per pixel. </summary>
		protected internal int NumDataElements_Renamed;

		/// <summary>
		/// The parent of this Raster, or null. </summary>
		protected internal Raster Parent_Renamed;

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern void initIDs();
		static Raster()
		{
			ColorModel.LoadLibraries();
			initIDs();
		}

		/// <summary>
		/// Creates a Raster based on a PixelInterleavedSampleModel with the
		/// specified data type, width, height, and number of bands.
		/// 
		/// <para> The upper left corner of the Raster is given by the
		/// location argument.  If location is null, (0, 0) will be used.
		/// The dataType parameter should be one of the enumerated values
		/// defined in the DataBuffer class.
		/// 
		/// </para>
		/// <para> Note that interleaved <code>DataBuffer.TYPE_INT</code>
		/// Rasters are not supported.  To create a 1-band Raster of type
		/// <code>DataBuffer.TYPE_INT</code>, use
		/// Raster.createPackedRaster().
		/// </para>
		/// <para> The only dataTypes supported currently are TYPE_BYTE
		/// and TYPE_USHORT.
		/// </para>
		/// </summary>
		/// <param name="dataType">  the data type for storing samples </param>
		/// <param name="w">         the width in pixels of the image data </param>
		/// <param name="h">         the height in pixels of the image data </param>
		/// <param name="bands">     the number of bands </param>
		/// <param name="location">  the upper-left corner of the <code>Raster</code> </param>
		/// <returns> a WritableRaster object with the specified data type,
		///         width, height and number of bands. </returns>
		/// <exception cref="RasterFormatException"> if <code>w</code> or <code>h</code>
		///         is less than or equal to zero, or computing either
		///         <code>location.x + w</code> or
		///         <code>location.y + h</code> results in integer
		///         overflow </exception>
		public static WritableRaster CreateInterleavedRaster(int dataType, int w, int h, int bands, Point location)
		{
			int[] bandOffsets = new int[bands];
			for (int i = 0; i < bands; i++)
			{
				bandOffsets[i] = i;
			}
			return CreateInterleavedRaster(dataType, w, h, w * bands, bands, bandOffsets, location);
		}

		/// <summary>
		/// Creates a Raster based on a PixelInterleavedSampleModel with the
		/// specified data type, width, height, scanline stride, pixel
		/// stride, and band offsets.  The number of bands is inferred from
		/// bandOffsets.length.
		/// 
		/// <para> The upper left corner of the Raster is given by the
		/// location argument.  If location is null, (0, 0) will be used.
		/// The dataType parameter should be one of the enumerated values
		/// defined in the DataBuffer class.
		/// 
		/// </para>
		/// <para> Note that interleaved <code>DataBuffer.TYPE_INT</code>
		/// Rasters are not supported.  To create a 1-band Raster of type
		/// <code>DataBuffer.TYPE_INT</code>, use
		/// Raster.createPackedRaster().
		/// </para>
		/// <para> The only dataTypes supported currently are TYPE_BYTE
		/// and TYPE_USHORT.
		/// </para>
		/// </summary>
		/// <param name="dataType">  the data type for storing samples </param>
		/// <param name="w">         the width in pixels of the image data </param>
		/// <param name="h">         the height in pixels of the image data </param>
		/// <param name="scanlineStride"> the line stride of the image data </param>
		/// <param name="pixelStride"> the pixel stride of the image data </param>
		/// <param name="bandOffsets"> the offsets of all bands </param>
		/// <param name="location">  the upper-left corner of the <code>Raster</code> </param>
		/// <returns> a WritableRaster object with the specified data type,
		///         width, height, scanline stride, pixel stride and band
		///         offsets. </returns>
		/// <exception cref="RasterFormatException"> if <code>w</code> or <code>h</code>
		///         is less than or equal to zero, or computing either
		///         <code>location.x + w</code> or
		///         <code>location.y + h</code> results in integer
		///         overflow </exception>
		/// <exception cref="IllegalArgumentException"> if <code>dataType</code> is not
		///         one of the supported data types, which are
		///         <code>DataBuffer.TYPE_BYTE</code>, or
		///         <code>DataBuffer.TYPE_USHORT</code>. </exception>
		public static WritableRaster CreateInterleavedRaster(int dataType, int w, int h, int scanlineStride, int pixelStride, int[] bandOffsets, Point location)
		{
			DataBuffer d;

			int size = scanlineStride * (h - 1) + pixelStride * w; // last scan -  fisrt (h - 1) scans

			switch (dataType)
			{
			case DataBuffer.TYPE_BYTE:
				d = new DataBufferByte(size);
				break;

			case DataBuffer.TYPE_USHORT:
				d = new DataBufferUShort(size);
				break;

			default:
				throw new IllegalArgumentException("Unsupported data type " + dataType);
			}

			return CreateInterleavedRaster(d, w, h, scanlineStride, pixelStride, bandOffsets, location);
		}

		/// <summary>
		/// Creates a Raster based on a BandedSampleModel with the
		/// specified data type, width, height, and number of bands.
		/// 
		/// <para> The upper left corner of the Raster is given by the
		/// location argument.  If location is null, (0, 0) will be used.
		/// The dataType parameter should be one of the enumerated values
		/// defined in the DataBuffer class.
		/// 
		/// </para>
		/// <para> The only dataTypes supported currently are TYPE_BYTE, TYPE_USHORT,
		/// and TYPE_INT.
		/// </para>
		/// </summary>
		/// <param name="dataType">  the data type for storing samples </param>
		/// <param name="w">         the width in pixels of the image data </param>
		/// <param name="h">         the height in pixels of the image data </param>
		/// <param name="bands">     the number of bands </param>
		/// <param name="location">  the upper-left corner of the <code>Raster</code> </param>
		/// <returns> a WritableRaster object with the specified data type,
		///         width, height and number of bands. </returns>
		/// <exception cref="RasterFormatException"> if <code>w</code> or <code>h</code>
		///         is less than or equal to zero, or computing either
		///         <code>location.x + w</code> or
		///         <code>location.y + h</code> results in integer
		///         overflow </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if <code>bands</code>
		///         is less than 1 </exception>
		public static WritableRaster CreateBandedRaster(int dataType, int w, int h, int bands, Point location)
		{
			if (bands < 1)
			{
				throw new ArrayIndexOutOfBoundsException("Number of bands (" + bands + ") must" + " be greater than 0");
			}
			int[] bankIndices = new int[bands];
			int[] bandOffsets = new int[bands];
			for (int i = 0; i < bands; i++)
			{
				bankIndices[i] = i;
				bandOffsets[i] = 0;
			}

			return CreateBandedRaster(dataType, w, h, w, bankIndices, bandOffsets, location);
		}

		/// <summary>
		/// Creates a Raster based on a BandedSampleModel with the
		/// specified data type, width, height, scanline stride, bank
		/// indices and band offsets.  The number of bands is inferred from
		/// bankIndices.length and bandOffsets.length, which must be the
		/// same.
		/// 
		/// <para> The upper left corner of the Raster is given by the
		/// location argument.  The dataType parameter should be one of the
		/// enumerated values defined in the DataBuffer class.
		/// 
		/// </para>
		/// <para> The only dataTypes supported currently are TYPE_BYTE, TYPE_USHORT,
		/// and TYPE_INT.
		/// </para>
		/// </summary>
		/// <param name="dataType">  the data type for storing samples </param>
		/// <param name="w">         the width in pixels of the image data </param>
		/// <param name="h">         the height in pixels of the image data </param>
		/// <param name="scanlineStride"> the line stride of the image data </param>
		/// <param name="bankIndices"> the bank indices for each band </param>
		/// <param name="bandOffsets"> the offsets of all bands </param>
		/// <param name="location">  the upper-left corner of the <code>Raster</code> </param>
		/// <returns> a WritableRaster object with the specified data type,
		///         width, height, scanline stride, bank indices and band
		///         offsets. </returns>
		/// <exception cref="RasterFormatException"> if <code>w</code> or <code>h</code>
		///         is less than or equal to zero, or computing either
		///         <code>location.x + w</code> or
		///         <code>location.y + h</code> results in integer
		///         overflow </exception>
		/// <exception cref="IllegalArgumentException"> if <code>dataType</code> is not
		///         one of the supported data types, which are
		///         <code>DataBuffer.TYPE_BYTE</code>,
		///         <code>DataBuffer.TYPE_USHORT</code>
		///         or <code>DataBuffer.TYPE_INT</code> </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if <code>bankIndices</code>
		///         or <code>bandOffsets</code> is <code>null</code> </exception>
		public static WritableRaster CreateBandedRaster(int dataType, int w, int h, int scanlineStride, int[] bankIndices, int[] bandOffsets, Point location)
		{
			DataBuffer d;
			int bands = bandOffsets.Length;

			if (bankIndices == null)
			{
				throw new ArrayIndexOutOfBoundsException("Bank indices array is null");
			}
			if (bandOffsets == null)
			{
				throw new ArrayIndexOutOfBoundsException("Band offsets array is null");
			}

			// Figure out the #banks and the largest band offset
			int maxBank = bankIndices[0];
			int maxBandOff = bandOffsets[0];
			for (int i = 1; i < bands; i++)
			{
				if (bankIndices[i] > maxBank)
				{
					maxBank = bankIndices[i];
				}
				if (bandOffsets[i] > maxBandOff)
				{
					maxBandOff = bandOffsets[i];
				}
			}
			int banks = maxBank + 1;
			int size = maxBandOff + scanlineStride * (h - 1) + w; // last scan -  fisrt (h - 1) scans

			switch (dataType)
			{
			case DataBuffer.TYPE_BYTE:
				d = new DataBufferByte(size, banks);
				break;

			case DataBuffer.TYPE_USHORT:
				d = new DataBufferUShort(size, banks);
				break;

			case DataBuffer.TYPE_INT:
				d = new DataBufferInt(size, banks);
				break;

			default:
				throw new IllegalArgumentException("Unsupported data type " + dataType);
			}

			return CreateBandedRaster(d, w, h, scanlineStride, bankIndices, bandOffsets, location);
		}

		/// <summary>
		/// Creates a Raster based on a SinglePixelPackedSampleModel with
		/// the specified data type, width, height, and band masks.
		/// The number of bands is inferred from bandMasks.length.
		/// 
		/// <para> The upper left corner of the Raster is given by the
		/// location argument.  If location is null, (0, 0) will be used.
		/// The dataType parameter should be one of the enumerated values
		/// defined in the DataBuffer class.
		/// 
		/// </para>
		/// <para> The only dataTypes supported currently are TYPE_BYTE, TYPE_USHORT,
		/// and TYPE_INT.
		/// </para>
		/// </summary>
		/// <param name="dataType">  the data type for storing samples </param>
		/// <param name="w">         the width in pixels of the image data </param>
		/// <param name="h">         the height in pixels of the image data </param>
		/// <param name="bandMasks"> an array containing an entry for each band </param>
		/// <param name="location">  the upper-left corner of the <code>Raster</code> </param>
		/// <returns> a WritableRaster object with the specified data type,
		///         width, height, and band masks. </returns>
		/// <exception cref="RasterFormatException"> if <code>w</code> or <code>h</code>
		///         is less than or equal to zero, or computing either
		///         <code>location.x + w</code> or
		///         <code>location.y + h</code> results in integer
		///         overflow </exception>
		/// <exception cref="IllegalArgumentException"> if <code>dataType</code> is not
		///         one of the supported data types, which are
		///         <code>DataBuffer.TYPE_BYTE</code>,
		///         <code>DataBuffer.TYPE_USHORT</code>
		///         or <code>DataBuffer.TYPE_INT</code> </exception>
		public static WritableRaster CreatePackedRaster(int dataType, int w, int h, int[] bandMasks, Point location)
		{
			DataBuffer d;

			switch (dataType)
			{
			case DataBuffer.TYPE_BYTE:
				d = new DataBufferByte(w * h);
				break;

			case DataBuffer.TYPE_USHORT:
				d = new DataBufferUShort(w * h);
				break;

			case DataBuffer.TYPE_INT:
				d = new DataBufferInt(w * h);
				break;

			default:
				throw new IllegalArgumentException("Unsupported data type " + dataType);
			}

			return CreatePackedRaster(d, w, h, w, bandMasks, location);
		}

		/// <summary>
		/// Creates a Raster based on a packed SampleModel with the
		/// specified data type, width, height, number of bands, and bits
		/// per band.  If the number of bands is one, the SampleModel will
		/// be a MultiPixelPackedSampleModel.
		/// 
		/// <para> If the number of bands is more than one, the SampleModel
		/// will be a SinglePixelPackedSampleModel, with each band having
		/// bitsPerBand bits.  In either case, the requirements on dataType
		/// and bitsPerBand imposed by the corresponding SampleModel must
		/// be met.
		/// 
		/// </para>
		/// <para> The upper left corner of the Raster is given by the
		/// location argument.  If location is null, (0, 0) will be used.
		/// The dataType parameter should be one of the enumerated values
		/// defined in the DataBuffer class.
		/// 
		/// </para>
		/// <para> The only dataTypes supported currently are TYPE_BYTE, TYPE_USHORT,
		/// and TYPE_INT.
		/// </para>
		/// </summary>
		/// <param name="dataType">  the data type for storing samples </param>
		/// <param name="w">         the width in pixels of the image data </param>
		/// <param name="h">         the height in pixels of the image data </param>
		/// <param name="bands">     the number of bands </param>
		/// <param name="bitsPerBand"> the number of bits per band </param>
		/// <param name="location">  the upper-left corner of the <code>Raster</code> </param>
		/// <returns> a WritableRaster object with the specified data type,
		///         width, height, number of bands, and bits per band. </returns>
		/// <exception cref="RasterFormatException"> if <code>w</code> or <code>h</code>
		///         is less than or equal to zero, or computing either
		///         <code>location.x + w</code> or
		///         <code>location.y + h</code> results in integer
		///         overflow </exception>
		/// <exception cref="IllegalArgumentException"> if the product of
		///         <code>bitsPerBand</code> and <code>bands</code> is
		///         greater than the number of bits held by
		///         <code>dataType</code> </exception>
		/// <exception cref="IllegalArgumentException"> if <code>bitsPerBand</code> or
		///         <code>bands</code> is not greater than zero </exception>
		/// <exception cref="IllegalArgumentException"> if <code>dataType</code> is not
		///         one of the supported data types, which are
		///         <code>DataBuffer.TYPE_BYTE</code>,
		///         <code>DataBuffer.TYPE_USHORT</code>
		///         or <code>DataBuffer.TYPE_INT</code> </exception>
		public static WritableRaster CreatePackedRaster(int dataType, int w, int h, int bands, int bitsPerBand, Point location)
		{
			DataBuffer d;

			if (bands <= 0)
			{
				throw new IllegalArgumentException("Number of bands (" + bands + ") must be greater than 0");
			}

			if (bitsPerBand <= 0)
			{
				throw new IllegalArgumentException("Bits per band (" + bitsPerBand + ") must be greater than 0");
			}

			if (bands != 1)
			{
				int[] masks = new int[bands];
				int mask = (1 << bitsPerBand) - 1;
				int shift = (bands - 1) * bitsPerBand;

				/* Make sure the total mask size will fit in the data type */
				if (shift + bitsPerBand > DataBuffer.GetDataTypeSize(dataType))
				{
					throw new IllegalArgumentException("bitsPerBand(" + bitsPerBand + ") * bands is " + " greater than data type " + "size.");
				}
				switch (dataType)
				{
				case DataBuffer.TYPE_BYTE:
				case DataBuffer.TYPE_USHORT:
				case DataBuffer.TYPE_INT:
					break;
				default:
					throw new IllegalArgumentException("Unsupported data type " + dataType);
				}

				for (int i = 0; i < bands; i++)
				{
					masks[i] = mask << shift;
					shift = shift - bitsPerBand;
				}

				return CreatePackedRaster(dataType, w, h, masks, location);
			}
			else
			{
				double fw = w;
				switch (dataType)
				{
				case DataBuffer.TYPE_BYTE:
					d = new DataBufferByte((int)(System.Math.Ceiling(fw / (8 / bitsPerBand))) * h);
					break;

				case DataBuffer.TYPE_USHORT:
					d = new DataBufferUShort((int)(System.Math.Ceiling(fw / (16 / bitsPerBand))) * h);
					break;

				case DataBuffer.TYPE_INT:
					d = new DataBufferInt((int)(System.Math.Ceiling(fw / (32 / bitsPerBand))) * h);
					break;

				default:
					throw new IllegalArgumentException("Unsupported data type " + dataType);
				}

				return CreatePackedRaster(d, w, h, bitsPerBand, location);
			}
		}

		/// <summary>
		/// Creates a Raster based on a PixelInterleavedSampleModel with the
		/// specified DataBuffer, width, height, scanline stride, pixel
		/// stride, and band offsets.  The number of bands is inferred from
		/// bandOffsets.length.  The upper left corner of the Raster
		/// is given by the location argument.  If location is null, (0, 0)
		/// will be used.
		/// <para> Note that interleaved <code>DataBuffer.TYPE_INT</code>
		/// Rasters are not supported.  To create a 1-band Raster of type
		/// <code>DataBuffer.TYPE_INT</code>, use
		/// Raster.createPackedRaster().
		/// </para>
		/// </summary>
		/// <param name="dataBuffer"> the <code>DataBuffer</code> that contains the
		///        image data </param>
		/// <param name="w">         the width in pixels of the image data </param>
		/// <param name="h">         the height in pixels of the image data </param>
		/// <param name="scanlineStride"> the line stride of the image data </param>
		/// <param name="pixelStride"> the pixel stride of the image data </param>
		/// <param name="bandOffsets"> the offsets of all bands </param>
		/// <param name="location">  the upper-left corner of the <code>Raster</code> </param>
		/// <returns> a WritableRaster object with the specified
		///         <code>DataBuffer</code>, width, height, scanline stride,
		///         pixel stride and band offsets. </returns>
		/// <exception cref="RasterFormatException"> if <code>w</code> or <code>h</code>
		///         is less than or equal to zero, or computing either
		///         <code>location.x + w</code> or
		///         <code>location.y + h</code> results in integer
		///         overflow </exception>
		/// <exception cref="IllegalArgumentException"> if <code>dataType</code> is not
		///         one of the supported data types, which are
		///         <code>DataBuffer.TYPE_BYTE</code>,
		///         <code>DataBuffer.TYPE_USHORT</code> </exception>
		/// <exception cref="RasterFormatException"> if <code>dataBuffer</code> has more
		///         than one bank. </exception>
		/// <exception cref="NullPointerException"> if <code>dataBuffer</code> is null </exception>
		public static WritableRaster CreateInterleavedRaster(DataBuffer dataBuffer, int w, int h, int scanlineStride, int pixelStride, int[] bandOffsets, Point location)
		{
			if (dataBuffer == null)
			{
				throw new NullPointerException("DataBuffer cannot be null");
			}
			if (location == null)
			{
				location = new Point(0, 0);
			}
			int dataType = dataBuffer.DataType;

			PixelInterleavedSampleModel csm = new PixelInterleavedSampleModel(dataType, w, h, pixelStride, scanlineStride, bandOffsets);
			switch (dataType)
			{
			case DataBuffer.TYPE_BYTE:
				return new ByteInterleavedRaster(csm, dataBuffer, location);

			case DataBuffer.TYPE_USHORT:
				return new ShortInterleavedRaster(csm, dataBuffer, location);

			default:
				throw new IllegalArgumentException("Unsupported data type " + dataType);
			}
		}

		/// <summary>
		/// Creates a Raster based on a BandedSampleModel with the
		/// specified DataBuffer, width, height, scanline stride, bank
		/// indices, and band offsets.  The number of bands is inferred
		/// from bankIndices.length and bandOffsets.length, which must be
		/// the same.  The upper left corner of the Raster is given by the
		/// location argument.  If location is null, (0, 0) will be used. </summary>
		/// <param name="dataBuffer"> the <code>DataBuffer</code> that contains the
		///        image data </param>
		/// <param name="w">         the width in pixels of the image data </param>
		/// <param name="h">         the height in pixels of the image data </param>
		/// <param name="scanlineStride"> the line stride of the image data </param>
		/// <param name="bankIndices"> the bank indices for each band </param>
		/// <param name="bandOffsets"> the offsets of all bands </param>
		/// <param name="location">  the upper-left corner of the <code>Raster</code> </param>
		/// <returns> a WritableRaster object with the specified
		///         <code>DataBuffer</code>, width, height, scanline stride,
		///         bank indices and band offsets. </returns>
		/// <exception cref="RasterFormatException"> if <code>w</code> or <code>h</code>
		///         is less than or equal to zero, or computing either
		///         <code>location.x + w</code> or
		///         <code>location.y + h</code> results in integer
		///         overflow </exception>
		/// <exception cref="IllegalArgumentException"> if <code>dataType</code> is not
		///         one of the supported data types, which are
		///         <code>DataBuffer.TYPE_BYTE</code>,
		///         <code>DataBuffer.TYPE_USHORT</code>
		///         or <code>DataBuffer.TYPE_INT</code> </exception>
		/// <exception cref="NullPointerException"> if <code>dataBuffer</code> is null </exception>
		public static WritableRaster CreateBandedRaster(DataBuffer dataBuffer, int w, int h, int scanlineStride, int[] bankIndices, int[] bandOffsets, Point location)
		{
			if (dataBuffer == null)
			{
				throw new NullPointerException("DataBuffer cannot be null");
			}
			if (location == null)
			{
			   location = new Point(0,0);
			}
			int dataType = dataBuffer.DataType;

			int bands = bankIndices.Length;
			if (bandOffsets.Length != bands)
			{
				throw new IllegalArgumentException("bankIndices.length != bandOffsets.length");
			}

			BandedSampleModel bsm = new BandedSampleModel(dataType, w, h, scanlineStride, bankIndices, bandOffsets);

			switch (dataType)
			{
			case DataBuffer.TYPE_BYTE:
				return new ByteBandedRaster(bsm, dataBuffer, location);

			case DataBuffer.TYPE_USHORT:
				return new ShortBandedRaster(bsm, dataBuffer, location);

			case DataBuffer.TYPE_INT:
				return new SunWritableRaster(bsm, dataBuffer, location);

			default:
				throw new IllegalArgumentException("Unsupported data type " + dataType);
			}
		}

		/// <summary>
		/// Creates a Raster based on a SinglePixelPackedSampleModel with
		/// the specified DataBuffer, width, height, scanline stride, and
		/// band masks.  The number of bands is inferred from bandMasks.length.
		/// The upper left corner of the Raster is given by
		/// the location argument.  If location is null, (0, 0) will be used. </summary>
		/// <param name="dataBuffer"> the <code>DataBuffer</code> that contains the
		///        image data </param>
		/// <param name="w">         the width in pixels of the image data </param>
		/// <param name="h">         the height in pixels of the image data </param>
		/// <param name="scanlineStride"> the line stride of the image data </param>
		/// <param name="bandMasks"> an array containing an entry for each band </param>
		/// <param name="location">  the upper-left corner of the <code>Raster</code> </param>
		/// <returns> a WritableRaster object with the specified
		///         <code>DataBuffer</code>, width, height, scanline stride,
		///         and band masks. </returns>
		/// <exception cref="RasterFormatException"> if <code>w</code> or <code>h</code>
		///         is less than or equal to zero, or computing either
		///         <code>location.x + w</code> or
		///         <code>location.y + h</code> results in integer
		///         overflow </exception>
		/// <exception cref="IllegalArgumentException"> if <code>dataType</code> is not
		///         one of the supported data types, which are
		///         <code>DataBuffer.TYPE_BYTE</code>,
		///         <code>DataBuffer.TYPE_USHORT</code>
		///         or <code>DataBuffer.TYPE_INT</code> </exception>
		/// <exception cref="RasterFormatException"> if <code>dataBuffer</code> has more
		///         than one bank. </exception>
		/// <exception cref="NullPointerException"> if <code>dataBuffer</code> is null </exception>
		public static WritableRaster CreatePackedRaster(DataBuffer dataBuffer, int w, int h, int scanlineStride, int[] bandMasks, Point location)
		{
			if (dataBuffer == null)
			{
				throw new NullPointerException("DataBuffer cannot be null");
			}
			if (location == null)
			{
			   location = new Point(0,0);
			}
			int dataType = dataBuffer.DataType;

			SinglePixelPackedSampleModel sppsm = new SinglePixelPackedSampleModel(dataType, w, h, scanlineStride, bandMasks);

			switch (dataType)
			{
			case DataBuffer.TYPE_BYTE:
				return new ByteInterleavedRaster(sppsm, dataBuffer, location);

			case DataBuffer.TYPE_USHORT:
				return new ShortInterleavedRaster(sppsm, dataBuffer, location);

			case DataBuffer.TYPE_INT:
				return new IntegerInterleavedRaster(sppsm, dataBuffer, location);

			default:
				throw new IllegalArgumentException("Unsupported data type " + dataType);
			}
		}

		/// <summary>
		/// Creates a Raster based on a MultiPixelPackedSampleModel with the
		/// specified DataBuffer, width, height, and bits per pixel.  The upper
		/// left corner of the Raster is given by the location argument.  If
		/// location is null, (0, 0) will be used. </summary>
		/// <param name="dataBuffer"> the <code>DataBuffer</code> that contains the
		///        image data </param>
		/// <param name="w">         the width in pixels of the image data </param>
		/// <param name="h">         the height in pixels of the image data </param>
		/// <param name="bitsPerPixel"> the number of bits for each pixel </param>
		/// <param name="location">  the upper-left corner of the <code>Raster</code> </param>
		/// <returns> a WritableRaster object with the specified
		///         <code>DataBuffer</code>, width, height, and
		///         bits per pixel. </returns>
		/// <exception cref="RasterFormatException"> if <code>w</code> or <code>h</code>
		///         is less than or equal to zero, or computing either
		///         <code>location.x + w</code> or
		///         <code>location.y + h</code> results in integer
		///         overflow </exception>
		/// <exception cref="IllegalArgumentException"> if <code>dataType</code> is not
		///         one of the supported data types, which are
		///         <code>DataBuffer.TYPE_BYTE</code>,
		///         <code>DataBuffer.TYPE_USHORT</code>
		///         or <code>DataBuffer.TYPE_INT</code> </exception>
		/// <exception cref="RasterFormatException"> if <code>dataBuffer</code> has more
		///         than one bank. </exception>
		/// <exception cref="NullPointerException"> if <code>dataBuffer</code> is null </exception>
		public static WritableRaster CreatePackedRaster(DataBuffer dataBuffer, int w, int h, int bitsPerPixel, Point location)
		{
			if (dataBuffer == null)
			{
				throw new NullPointerException("DataBuffer cannot be null");
			}
			if (location == null)
			{
			   location = new Point(0,0);
			}
			int dataType = dataBuffer.DataType;

			if (dataType != DataBuffer.TYPE_BYTE && dataType != DataBuffer.TYPE_USHORT && dataType != DataBuffer.TYPE_INT)
			{
				throw new IllegalArgumentException("Unsupported data type " + dataType);
			}

			if (dataBuffer.NumBanks != 1)
			{
				throw new RasterFormatException("DataBuffer for packed Rasters" + " must only have 1 bank.");
			}

			MultiPixelPackedSampleModel mppsm = new MultiPixelPackedSampleModel(dataType, w, h, bitsPerPixel);

			if (dataType == DataBuffer.TYPE_BYTE && (bitsPerPixel == 1 || bitsPerPixel == 2 || bitsPerPixel == 4))
			{
				return new BytePackedRaster(mppsm, dataBuffer, location);
			}
			else
			{
				return new SunWritableRaster(mppsm, dataBuffer, location);
			}
		}


		/// <summary>
		///  Creates a Raster with the specified SampleModel and DataBuffer.
		///  The upper left corner of the Raster is given by the location argument.
		///  If location is null, (0, 0) will be used. </summary>
		///  <param name="sm"> the specified <code>SampleModel</code> </param>
		///  <param name="db"> the specified <code>DataBuffer</code> </param>
		///  <param name="location"> the upper-left corner of the <code>Raster</code> </param>
		///  <returns> a <code>Raster</code> with the specified
		///          <code>SampleModel</code>, <code>DataBuffer</code>, and
		///          location. </returns>
		/// <exception cref="RasterFormatException"> if computing either
		///         <code>location.x + sm.getWidth()</code> or
		///         <code>location.y + sm.getHeight()</code> results in integer
		///         overflow </exception>
		/// <exception cref="RasterFormatException"> if <code>db</code> has more
		///         than one bank and <code>sm</code> is a
		///         PixelInterleavedSampleModel, SinglePixelPackedSampleModel,
		///         or MultiPixelPackedSampleModel. </exception>
		///  <exception cref="NullPointerException"> if either SampleModel or DataBuffer is
		///          null </exception>
		public static Raster CreateRaster(SampleModel sm, DataBuffer db, Point location)
		{
			if ((sm == null) || (db == null))
			{
				throw new NullPointerException("SampleModel and DataBuffer cannot be null");
			}

			if (location == null)
			{
			   location = new Point(0,0);
			}
			int dataType = sm.DataType;

			if (sm is PixelInterleavedSampleModel)
			{
				switch (dataType)
				{
					case DataBuffer.TYPE_BYTE:
						return new ByteInterleavedRaster(sm, db, location);

					case DataBuffer.TYPE_USHORT:
						return new ShortInterleavedRaster(sm, db, location);
				}
			}
			else if (sm is SinglePixelPackedSampleModel)
			{
				switch (dataType)
				{
					case DataBuffer.TYPE_BYTE:
						return new ByteInterleavedRaster(sm, db, location);

					case DataBuffer.TYPE_USHORT:
						return new ShortInterleavedRaster(sm, db, location);

					case DataBuffer.TYPE_INT:
						return new IntegerInterleavedRaster(sm, db, location);
				}
			}
			else if (sm is MultiPixelPackedSampleModel && dataType == DataBuffer.TYPE_BYTE && sm.GetSampleSize(0) < 8)
			{
				return new BytePackedRaster(sm, db, location);
			}

			// we couldn't do anything special - do the generic thing

			return new Raster(sm,db,location);
		}

		/// <summary>
		///  Creates a WritableRaster with the specified SampleModel.
		///  The upper left corner of the Raster is given by the location argument.
		///  If location is null, (0, 0) will be used. </summary>
		///  <param name="sm"> the specified <code>SampleModel</code> </param>
		///  <param name="location"> the upper-left corner of the
		///         <code>WritableRaster</code> </param>
		///  <returns> a <code>WritableRaster</code> with the specified
		///          <code>SampleModel</code> and location. </returns>
		///  <exception cref="RasterFormatException"> if computing either
		///          <code>location.x + sm.getWidth()</code> or
		///          <code>location.y + sm.getHeight()</code> results in integer
		///          overflow </exception>
		public static WritableRaster CreateWritableRaster(SampleModel sm, Point location)
		{
			if (location == null)
			{
			   location = new Point(0,0);
			}

			return CreateWritableRaster(sm, sm.CreateDataBuffer(), location);
		}

		/// <summary>
		///  Creates a WritableRaster with the specified SampleModel and DataBuffer.
		///  The upper left corner of the Raster is given by the location argument.
		///  If location is null, (0, 0) will be used. </summary>
		///  <param name="sm"> the specified <code>SampleModel</code> </param>
		///  <param name="db"> the specified <code>DataBuffer</code> </param>
		///  <param name="location"> the upper-left corner of the
		///         <code>WritableRaster</code> </param>
		///  <returns> a <code>WritableRaster</code> with the specified
		///          <code>SampleModel</code>, <code>DataBuffer</code>, and
		///          location. </returns>
		/// <exception cref="RasterFormatException"> if computing either
		///         <code>location.x + sm.getWidth()</code> or
		///         <code>location.y + sm.getHeight()</code> results in integer
		///         overflow </exception>
		/// <exception cref="RasterFormatException"> if <code>db</code> has more
		///         than one bank and <code>sm</code> is a
		///         PixelInterleavedSampleModel, SinglePixelPackedSampleModel,
		///         or MultiPixelPackedSampleModel. </exception>
		/// <exception cref="NullPointerException"> if either SampleModel or DataBuffer is null </exception>
		public static WritableRaster CreateWritableRaster(SampleModel sm, DataBuffer db, Point location)
		{
			if ((sm == null) || (db == null))
			{
				throw new NullPointerException("SampleModel and DataBuffer cannot be null");
			}
			if (location == null)
			{
			   location = new Point(0,0);
			}

			int dataType = sm.DataType;

			if (sm is PixelInterleavedSampleModel)
			{
				switch (dataType)
				{
					case DataBuffer.TYPE_BYTE:
						return new ByteInterleavedRaster(sm, db, location);

					case DataBuffer.TYPE_USHORT:
						return new ShortInterleavedRaster(sm, db, location);
				}
			}
			else if (sm is SinglePixelPackedSampleModel)
			{
				switch (dataType)
				{
					case DataBuffer.TYPE_BYTE:
						return new ByteInterleavedRaster(sm, db, location);

					case DataBuffer.TYPE_USHORT:
						return new ShortInterleavedRaster(sm, db, location);

					case DataBuffer.TYPE_INT:
						return new IntegerInterleavedRaster(sm, db, location);
				}
			}
			else if (sm is MultiPixelPackedSampleModel && dataType == DataBuffer.TYPE_BYTE && sm.GetSampleSize(0) < 8)
			{
				return new BytePackedRaster(sm, db, location);
			}

			// we couldn't do anything special - do the generic thing

			return new SunWritableRaster(sm,db,location);
		}

		/// <summary>
		///  Constructs a Raster with the given SampleModel.  The Raster's
		///  upper left corner is origin and it is the same size as the
		///  SampleModel.  A DataBuffer large enough to describe the
		///  Raster is automatically created. </summary>
		///  <param name="sampleModel">     The SampleModel that specifies the layout </param>
		///  <param name="origin">          The Point that specified the origin </param>
		///  <exception cref="RasterFormatException"> if computing either
		///          <code>origin.x + sampleModel.getWidth()</code> or
		///          <code>origin.y + sampleModel.getHeight()</code> results in
		///          integer overflow </exception>
		///  <exception cref="NullPointerException"> either <code>sampleModel</code> or
		///          <code>origin</code> is null </exception>
		protected internal Raster(SampleModel sampleModel, Point origin) : this(sampleModel, sampleModel.CreateDataBuffer(), new Rectangle(origin.x, origin.y, sampleModel.Width, sampleModel.Height), origin, null)
		{
		}

		/// <summary>
		///  Constructs a Raster with the given SampleModel and DataBuffer.
		///  The Raster's upper left corner is origin and it is the same size
		///  as the SampleModel.  The DataBuffer is not initialized and must
		///  be compatible with SampleModel. </summary>
		///  <param name="sampleModel">     The SampleModel that specifies the layout </param>
		///  <param name="dataBuffer">      The DataBuffer that contains the image data </param>
		///  <param name="origin">          The Point that specifies the origin </param>
		///  <exception cref="RasterFormatException"> if computing either
		///          <code>origin.x + sampleModel.getWidth()</code> or
		///          <code>origin.y + sampleModel.getHeight()</code> results in
		///          integer overflow </exception>
		///  <exception cref="NullPointerException"> either <code>sampleModel</code> or
		///          <code>origin</code> is null </exception>
		protected internal Raster(SampleModel sampleModel, DataBuffer dataBuffer, Point origin) : this(sampleModel, dataBuffer, new Rectangle(origin.x, origin.y, sampleModel.Width, sampleModel.Height), origin, null)
		{
		}

		/// <summary>
		/// Constructs a Raster with the given SampleModel, DataBuffer, and
		/// parent.  aRegion specifies the bounding rectangle of the new
		/// Raster.  When translated into the base Raster's coordinate
		/// system, aRegion must be contained by the base Raster.
		/// (The base Raster is the Raster's ancestor which has no parent.)
		/// sampleModelTranslate specifies the sampleModelTranslateX and
		/// sampleModelTranslateY values of the new Raster.
		/// 
		/// Note that this constructor should generally be called by other
		/// constructors or create methods, it should not be used directly. </summary>
		/// <param name="sampleModel">     The SampleModel that specifies the layout </param>
		/// <param name="dataBuffer">      The DataBuffer that contains the image data </param>
		/// <param name="aRegion">         The Rectangle that specifies the image area </param>
		/// <param name="sampleModelTranslate">  The Point that specifies the translation
		///                        from SampleModel to Raster coordinates </param>
		/// <param name="parent">          The parent (if any) of this raster </param>
		/// <exception cref="NullPointerException"> if any of <code>sampleModel</code>,
		///         <code>dataBuffer</code>, <code>aRegion</code> or
		///         <code>sampleModelTranslate</code> is null </exception>
		/// <exception cref="RasterFormatException"> if <code>aRegion</code> has width
		///         or height less than or equal to zero, or computing either
		///         <code>aRegion.x + aRegion.width</code> or
		///         <code>aRegion.y + aRegion.height</code> results in integer
		///         overflow </exception>
		protected internal Raster(SampleModel sampleModel, DataBuffer dataBuffer, Rectangle aRegion, Point sampleModelTranslate, Raster parent)
		{

			if ((sampleModel == null) || (dataBuffer == null) || (aRegion == null) || (sampleModelTranslate == null))
			{
				throw new NullPointerException("SampleModel, dataBuffer, aRegion and " + "sampleModelTranslate cannot be null");
			}
		   this.SampleModel_Renamed = sampleModel;
		   this.DataBuffer_Renamed = dataBuffer;
		   MinX_Renamed = aRegion.x;
		   MinY_Renamed = aRegion.y;
		   Width_Renamed = aRegion.Width_Renamed;
		   Height_Renamed = aRegion.Height_Renamed;
		   if (Width_Renamed <= 0 || Height_Renamed <= 0)
		   {
			   throw new RasterFormatException("negative or zero " + ((Width_Renamed <= 0) ? "width" : "height"));
		   }
		   if ((MinX_Renamed + Width_Renamed) < MinX_Renamed)
		   {
			   throw new RasterFormatException("overflow condition for X coordinates of Raster");
		   }
		   if ((MinY_Renamed + Height_Renamed) < MinY_Renamed)
		   {
			   throw new RasterFormatException("overflow condition for Y coordinates of Raster");
		   }

		   SampleModelTranslateX_Renamed = sampleModelTranslate.x;
		   SampleModelTranslateY_Renamed = sampleModelTranslate.y;

		   NumBands_Renamed = sampleModel.NumBands;
		   NumDataElements_Renamed = sampleModel.NumDataElements;
		   this.Parent_Renamed = parent;
		}


		/// <summary>
		/// Returns the parent Raster (if any) of this Raster or null. </summary>
		/// <returns> the parent Raster or <code>null</code>. </returns>
		public virtual Raster Parent
		{
			get
			{
				return Parent_Renamed;
			}
		}

		/// <summary>
		/// Returns the X translation from the coordinate system of the
		/// SampleModel to that of the Raster.  To convert a pixel's X
		/// coordinate from the Raster coordinate system to the SampleModel
		/// coordinate system, this value must be subtracted. </summary>
		/// <returns> the X translation from the coordinate space of the
		///         Raster's SampleModel to that of the Raster. </returns>
		public int SampleModelTranslateX
		{
			get
			{
				return SampleModelTranslateX_Renamed;
			}
		}

		/// <summary>
		/// Returns the Y translation from the coordinate system of the
		/// SampleModel to that of the Raster.  To convert a pixel's Y
		/// coordinate from the Raster coordinate system to the SampleModel
		/// coordinate system, this value must be subtracted. </summary>
		/// <returns> the Y translation from the coordinate space of the
		///         Raster's SampleModel to that of the Raster. </returns>
		public int SampleModelTranslateY
		{
			get
			{
				return SampleModelTranslateY_Renamed;
			}
		}

		/// <summary>
		/// Create a compatible WritableRaster the same size as this Raster with
		/// the same SampleModel and a new initialized DataBuffer. </summary>
		/// <returns> a compatible <code>WritableRaster</code> with the same sample
		///         model and a new data buffer. </returns>
		public virtual WritableRaster CreateCompatibleWritableRaster()
		{
			return new SunWritableRaster(SampleModel_Renamed, new Point(0,0));
		}

		/// <summary>
		/// Create a compatible WritableRaster with the specified size, a new
		/// SampleModel, and a new initialized DataBuffer. </summary>
		/// <param name="w"> the specified width of the new <code>WritableRaster</code> </param>
		/// <param name="h"> the specified height of the new <code>WritableRaster</code> </param>
		/// <returns> a compatible <code>WritableRaster</code> with the specified
		///         size and a new sample model and data buffer. </returns>
		/// <exception cref="RasterFormatException"> if the width or height is less than
		///                               or equal to zero. </exception>
		public virtual WritableRaster CreateCompatibleWritableRaster(int w, int h)
		{
			if (w <= 0 || h <= 0)
			{
				throw new RasterFormatException("negative " + ((w <= 0) ? "width" : "height"));
			}

			SampleModel sm = SampleModel_Renamed.CreateCompatibleSampleModel(w,h);

			return new SunWritableRaster(sm, new Point(0,0));
		}

		/// <summary>
		/// Create a compatible WritableRaster with location (minX, minY)
		/// and size (width, height) specified by rect, a
		/// new SampleModel, and a new initialized DataBuffer. </summary>
		/// <param name="rect"> a <code>Rectangle</code> that specifies the size and
		///        location of the <code>WritableRaster</code> </param>
		/// <returns> a compatible <code>WritableRaster</code> with the specified
		///         size and location and a new sample model and data buffer. </returns>
		/// <exception cref="RasterFormatException"> if <code>rect</code> has width
		///         or height less than or equal to zero, or computing either
		///         <code>rect.x + rect.width</code> or
		///         <code>rect.y + rect.height</code> results in integer
		///         overflow </exception>
		/// <exception cref="NullPointerException"> if <code>rect</code> is null </exception>
		public virtual WritableRaster CreateCompatibleWritableRaster(Rectangle rect)
		{
			if (rect == null)
			{
				throw new NullPointerException("Rect cannot be null");
			}
			return CreateCompatibleWritableRaster(rect.x, rect.y, rect.Width_Renamed, rect.Height_Renamed);
		}

		/// <summary>
		/// Create a compatible WritableRaster with the specified
		/// location (minX, minY) and size (width, height), a
		/// new SampleModel, and a new initialized DataBuffer. </summary>
		/// <param name="x"> the X coordinate of the upper-left corner of
		///        the <code>WritableRaster</code> </param>
		/// <param name="y"> the Y coordinate of the upper-left corner of
		///        the <code>WritableRaster</code> </param>
		/// <param name="w"> the specified width of the <code>WritableRaster</code> </param>
		/// <param name="h"> the specified height of the <code>WritableRaster</code> </param>
		/// <returns> a compatible <code>WritableRaster</code> with the specified
		///         size and location and a new sample model and data buffer. </returns>
		/// <exception cref="RasterFormatException"> if <code>w</code> or <code>h</code>
		///         is less than or equal to zero, or computing either
		///         <code>x + w</code> or
		///         <code>y + h</code> results in integer
		///         overflow </exception>
		public virtual WritableRaster CreateCompatibleWritableRaster(int x, int y, int w, int h)
		{
			WritableRaster ret = CreateCompatibleWritableRaster(w, h);
			return ret.CreateWritableChild(0,0,w,h,x,y,null);
		}

		/// <summary>
		/// Create a Raster with the same size, SampleModel and DataBuffer
		/// as this one, but with a different location.  The new Raster
		/// will possess a reference to the current Raster, accessible
		/// through its getParent() method.
		/// </summary>
		/// <param name="childMinX"> the X coordinate of the upper-left
		///        corner of the new <code>Raster</code> </param>
		/// <param name="childMinY"> the Y coordinate of the upper-left
		///        corner of the new <code>Raster</code> </param>
		/// <returns> a new <code>Raster</code> with the same size, SampleModel,
		///         and DataBuffer as this <code>Raster</code>, but with the
		///         specified location. </returns>
		/// <exception cref="RasterFormatException"> if  computing either
		///         <code>childMinX + this.getWidth()</code> or
		///         <code>childMinY + this.getHeight()</code> results in integer
		///         overflow </exception>
		public virtual Raster CreateTranslatedChild(int childMinX, int childMinY)
		{
			return CreateChild(MinX_Renamed,MinY_Renamed,Width_Renamed,Height_Renamed, childMinX,childMinY,null);
		}

		/// <summary>
		/// Returns a new Raster which shares all or part of this Raster's
		/// DataBuffer.  The new Raster will possess a reference to the
		/// current Raster, accessible through its getParent() method.
		/// 
		/// <para> The parentX, parentY, width and height parameters
		/// form a Rectangle in this Raster's coordinate space,
		/// indicating the area of pixels to be shared.  An error will
		/// be thrown if this Rectangle is not contained with the bounds
		/// of the current Raster.
		/// 
		/// </para>
		/// <para> The new Raster may additionally be translated to a
		/// different coordinate system for the plane than that used by the current
		/// Raster.  The childMinX and childMinY parameters give the new
		/// (x, y) coordinate of the upper-left pixel of the returned
		/// Raster; the coordinate (childMinX, childMinY) in the new Raster
		/// will map to the same pixel as the coordinate (parentX, parentY)
		/// in the current Raster.
		/// 
		/// </para>
		/// <para> The new Raster may be defined to contain only a subset of
		/// the bands of the current Raster, possibly reordered, by means
		/// of the bandList parameter.  If bandList is null, it is taken to
		/// include all of the bands of the current Raster in their current
		/// order.
		/// 
		/// </para>
		/// <para> To create a new Raster that contains a subregion of the current
		/// Raster, but shares its coordinate system and bands,
		/// this method should be called with childMinX equal to parentX,
		/// childMinY equal to parentY, and bandList equal to null.
		/// 
		/// </para>
		/// </summary>
		/// <param name="parentX"> The X coordinate of the upper-left corner
		///        in this Raster's coordinates </param>
		/// <param name="parentY"> The Y coordinate of the upper-left corner
		///        in this Raster's coordinates </param>
		/// <param name="width">      Width of the region starting at (parentX, parentY) </param>
		/// <param name="height">     Height of the region starting at (parentX, parentY). </param>
		/// <param name="childMinX"> The X coordinate of the upper-left corner
		///                   of the returned Raster </param>
		/// <param name="childMinY"> The Y coordinate of the upper-left corner
		///                   of the returned Raster </param>
		/// <param name="bandList">   Array of band indices, or null to use all bands </param>
		/// <returns> a new <code>Raster</code>. </returns>
		/// <exception cref="RasterFormatException"> if the specified subregion is outside
		///                               of the raster bounds. </exception>
		/// <exception cref="RasterFormatException"> if <code>width</code> or
		///         <code>height</code>
		///         is less than or equal to zero, or computing any of
		///         <code>parentX + width</code>, <code>parentY + height</code>,
		///         <code>childMinX + width</code>, or
		///         <code>childMinY + height</code> results in integer
		///         overflow </exception>
		public virtual Raster CreateChild(int parentX, int parentY, int width, int height, int childMinX, int childMinY, int[] bandList)
		{
			if (parentX < this.MinX_Renamed)
			{
				throw new RasterFormatException("parentX lies outside raster");
			}
			if (parentY < this.MinY_Renamed)
			{
				throw new RasterFormatException("parentY lies outside raster");
			}
			if ((parentX + width < parentX) || (parentX + width > this.Width_Renamed + this.MinX_Renamed))
			{
				throw new RasterFormatException("(parentX + width) is outside raster");
			}
			if ((parentY + height < parentY) || (parentY + height > this.Height_Renamed + this.MinY_Renamed))
			{
				throw new RasterFormatException("(parentY + height) is outside raster");
			}

			SampleModel subSampleModel;
			// Note: the SampleModel for the child Raster should have the same
			// width and height as that for the parent, since it represents
			// the physical layout of the pixel data.  The child Raster's width
			// and height represent a "virtual" view of the pixel data, so
			// they may be different than those of the SampleModel.
			if (bandList == null)
			{
				subSampleModel = SampleModel_Renamed;
			}
			else
			{
				subSampleModel = SampleModel_Renamed.CreateSubsetSampleModel(bandList);
			}

			int deltaX = childMinX - parentX;
			int deltaY = childMinY - parentY;

			return new Raster(subSampleModel, DataBuffer, new Rectangle(childMinX, childMinY, width, height), new Point(SampleModelTranslateX_Renamed + deltaX, SampleModelTranslateY_Renamed + deltaY), this);
		}

		/// <summary>
		/// Returns the bounding Rectangle of this Raster. This function returns
		/// the same information as getMinX/MinY/Width/Height. </summary>
		/// <returns> the bounding box of this <code>Raster</code>. </returns>
		public virtual Rectangle Bounds
		{
			get
			{
				return new Rectangle(MinX_Renamed, MinY_Renamed, Width_Renamed, Height_Renamed);
			}
		}

		/// <summary>
		/// Returns the minimum valid X coordinate of the Raster. </summary>
		///  <returns> the minimum x coordinate of this <code>Raster</code>. </returns>
		public int MinX
		{
			get
			{
				return MinX_Renamed;
			}
		}

		/// <summary>
		/// Returns the minimum valid Y coordinate of the Raster. </summary>
		///  <returns> the minimum y coordinate of this <code>Raster</code>. </returns>
		public int MinY
		{
			get
			{
				return MinY_Renamed;
			}
		}

		/// <summary>
		/// Returns the width in pixels of the Raster. </summary>
		///  <returns> the width of this <code>Raster</code>. </returns>
		public int Width
		{
			get
			{
				return Width_Renamed;
			}
		}

		/// <summary>
		/// Returns the height in pixels of the Raster. </summary>
		///  <returns> the height of this <code>Raster</code>. </returns>
		public int Height
		{
			get
			{
				return Height_Renamed;
			}
		}

		/// <summary>
		/// Returns the number of bands (samples per pixel) in this Raster. </summary>
		///  <returns> the number of bands of this <code>Raster</code>. </returns>
		public int NumBands
		{
			get
			{
				return NumBands_Renamed;
			}
		}

		/// <summary>
		///  Returns the number of data elements needed to transfer one pixel
		///  via the getDataElements and setDataElements methods.  When pixels
		///  are transferred via these methods, they may be transferred in a
		///  packed or unpacked format, depending on the implementation of the
		///  underlying SampleModel.  Using these methods, pixels are transferred
		///  as an array of getNumDataElements() elements of a primitive type given
		///  by getTransferType().  The TransferType may or may not be the same
		///  as the storage data type of the DataBuffer. </summary>
		///  <returns> the number of data elements. </returns>
		public int NumDataElements
		{
			get
			{
				return SampleModel_Renamed.NumDataElements;
			}
		}

		/// <summary>
		///  Returns the TransferType used to transfer pixels via the
		///  getDataElements and setDataElements methods.  When pixels
		///  are transferred via these methods, they may be transferred in a
		///  packed or unpacked format, depending on the implementation of the
		///  underlying SampleModel.  Using these methods, pixels are transferred
		///  as an array of getNumDataElements() elements of a primitive type given
		///  by getTransferType().  The TransferType may or may not be the same
		///  as the storage data type of the DataBuffer.  The TransferType will
		///  be one of the types defined in DataBuffer. </summary>
		///  <returns> this transfer type. </returns>
		public int TransferType
		{
			get
			{
				return SampleModel_Renamed.TransferType;
			}
		}

		/// <summary>
		/// Returns the DataBuffer associated with this Raster. </summary>
		///  <returns> the <code>DataBuffer</code> of this <code>Raster</code>. </returns>
		public virtual DataBuffer DataBuffer
		{
			get
			{
				return DataBuffer_Renamed;
			}
		}

		/// <summary>
		/// Returns the SampleModel that describes the layout of the image data. </summary>
		///  <returns> the <code>SampleModel</code> of this <code>Raster</code>. </returns>
		public virtual SampleModel SampleModel
		{
			get
			{
				return SampleModel_Renamed;
			}
		}

		/// <summary>
		/// Returns data for a single pixel in a primitive array of type
		/// TransferType.  For image data supported by the Java 2D(tm) API,
		/// this will be one of DataBuffer.TYPE_BYTE, DataBuffer.TYPE_USHORT,
		/// DataBuffer.TYPE_INT, DataBuffer.TYPE_SHORT, DataBuffer.TYPE_FLOAT,
		/// or DataBuffer.TYPE_DOUBLE.  Data may be returned in a packed format,
		/// thus increasing efficiency for data transfers.
		/// An ArrayIndexOutOfBoundsException may be thrown
		/// if the coordinates are not in bounds.  However, explicit bounds
		/// checking is not guaranteed.
		/// A ClassCastException will be thrown if the input object is non null
		/// and references anything other than an array of TransferType. </summary>
		/// <seealso cref= java.awt.image.SampleModel#getDataElements(int, int, Object, DataBuffer) </seealso>
		/// <param name="x">        The X coordinate of the pixel location </param>
		/// <param name="y">        The Y coordinate of the pixel location </param>
		/// <param name="outData">  An object reference to an array of type defined by
		///                 getTransferType() and length getNumDataElements().
		///                 If null, an array of appropriate type and size will be
		///                 allocated </param>
		/// <returns>         An object reference to an array of type defined by
		///                 getTransferType() with the requested pixel data.
		/// </returns>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates are not
		/// in bounds, or if outData is too small to hold the output. </exception>
		public virtual Object GetDataElements(int x, int y, Object outData)
		{
			return SampleModel_Renamed.GetDataElements(x - SampleModelTranslateX_Renamed, y - SampleModelTranslateY_Renamed, outData, DataBuffer_Renamed);
		}

		/// <summary>
		/// Returns the pixel data for the specified rectangle of pixels in a
		/// primitive array of type TransferType.
		/// For image data supported by the Java 2D API, this
		/// will be one of DataBuffer.TYPE_BYTE, DataBuffer.TYPE_USHORT,
		/// DataBuffer.TYPE_INT, DataBuffer.TYPE_SHORT, DataBuffer.TYPE_FLOAT,
		/// or DataBuffer.TYPE_DOUBLE.  Data may be returned in a packed format,
		/// thus increasing efficiency for data transfers.
		/// An ArrayIndexOutOfBoundsException may be thrown
		/// if the coordinates are not in bounds.  However, explicit bounds
		/// checking is not guaranteed.
		/// A ClassCastException will be thrown if the input object is non null
		/// and references anything other than an array of TransferType. </summary>
		/// <seealso cref= java.awt.image.SampleModel#getDataElements(int, int, int, int, Object, DataBuffer) </seealso>
		/// <param name="x">    The X coordinate of the upper-left pixel location </param>
		/// <param name="y">    The Y coordinate of the upper-left pixel location </param>
		/// <param name="w">    Width of the pixel rectangle </param>
		/// <param name="h">   Height of the pixel rectangle </param>
		/// <param name="outData">  An object reference to an array of type defined by
		///                 getTransferType() and length w*h*getNumDataElements().
		///                 If null, an array of appropriate type and size will be
		///                 allocated. </param>
		/// <returns>         An object reference to an array of type defined by
		///                 getTransferType() with the requested pixel data.
		/// </returns>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates are not
		/// in bounds, or if outData is too small to hold the output. </exception>
		public virtual Object GetDataElements(int x, int y, int w, int h, Object outData)
		{
			return SampleModel_Renamed.GetDataElements(x - SampleModelTranslateX_Renamed, y - SampleModelTranslateY_Renamed, w, h, outData, DataBuffer_Renamed);
		}

		/// <summary>
		/// Returns the samples in an array of int for the specified pixel.
		/// An ArrayIndexOutOfBoundsException may be thrown
		/// if the coordinates are not in bounds.  However, explicit bounds
		/// checking is not guaranteed. </summary>
		/// <param name="x"> The X coordinate of the pixel location </param>
		/// <param name="y"> The Y coordinate of the pixel location </param>
		/// <param name="iArray"> An optionally preallocated int array </param>
		/// <returns> the samples for the specified pixel.
		/// </returns>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates are not
		/// in bounds, or if iArray is too small to hold the output. </exception>
		public virtual int[] GetPixel(int x, int y, int[] iArray)
		{
			return SampleModel_Renamed.GetPixel(x - SampleModelTranslateX_Renamed, y - SampleModelTranslateY_Renamed, iArray, DataBuffer_Renamed);
		}

		/// <summary>
		/// Returns the samples in an array of float for the
		/// specified pixel.
		/// An ArrayIndexOutOfBoundsException may be thrown
		/// if the coordinates are not in bounds.  However, explicit bounds
		/// checking is not guaranteed. </summary>
		/// <param name="x"> The X coordinate of the pixel location </param>
		/// <param name="y"> The Y coordinate of the pixel location </param>
		/// <param name="fArray"> An optionally preallocated float array </param>
		/// <returns> the samples for the specified pixel.
		/// </returns>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates are not
		/// in bounds, or if fArray is too small to hold the output. </exception>
		public virtual float[] GetPixel(int x, int y, float[] fArray)
		{
			return SampleModel_Renamed.GetPixel(x - SampleModelTranslateX_Renamed, y - SampleModelTranslateY_Renamed, fArray, DataBuffer_Renamed);
		}

		/// <summary>
		/// Returns the samples in an array of double for the specified pixel.
		/// An ArrayIndexOutOfBoundsException may be thrown
		/// if the coordinates are not in bounds.  However, explicit bounds
		/// checking is not guaranteed. </summary>
		/// <param name="x"> The X coordinate of the pixel location </param>
		/// <param name="y"> The Y coordinate of the pixel location </param>
		/// <param name="dArray"> An optionally preallocated double array </param>
		/// <returns> the samples for the specified pixel.
		/// </returns>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates are not
		/// in bounds, or if dArray is too small to hold the output. </exception>
		public virtual double[] GetPixel(int x, int y, double[] dArray)
		{
			return SampleModel_Renamed.GetPixel(x - SampleModelTranslateX_Renamed, y - SampleModelTranslateY_Renamed, dArray, DataBuffer_Renamed);
		}

		/// <summary>
		/// Returns an int array containing all samples for a rectangle of pixels,
		/// one sample per array element.
		/// An ArrayIndexOutOfBoundsException may be thrown
		/// if the coordinates are not in bounds.  However, explicit bounds
		/// checking is not guaranteed. </summary>
		/// <param name="x">      The X coordinate of the upper-left pixel location </param>
		/// <param name="y">      The Y coordinate of the upper-left pixel location </param>
		/// <param name="w">      Width of the pixel rectangle </param>
		/// <param name="h">      Height of the pixel rectangle </param>
		/// <param name="iArray"> An optionally pre-allocated int array </param>
		/// <returns> the samples for the specified rectangle of pixels.
		/// </returns>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates are not
		/// in bounds, or if iArray is too small to hold the output. </exception>
		public virtual int[] GetPixels(int x, int y, int w, int h, int[] iArray)
		{
			return SampleModel_Renamed.GetPixels(x - SampleModelTranslateX_Renamed, y - SampleModelTranslateY_Renamed, w, h, iArray, DataBuffer_Renamed);
		}

		/// <summary>
		/// Returns a float array containing all samples for a rectangle of pixels,
		/// one sample per array element.
		/// An ArrayIndexOutOfBoundsException may be thrown
		/// if the coordinates are not in bounds.  However, explicit bounds
		/// checking is not guaranteed. </summary>
		/// <param name="x">        The X coordinate of the pixel location </param>
		/// <param name="y">        The Y coordinate of the pixel location </param>
		/// <param name="w">        Width of the pixel rectangle </param>
		/// <param name="h">        Height of the pixel rectangle </param>
		/// <param name="fArray">   An optionally pre-allocated float array </param>
		/// <returns> the samples for the specified rectangle of pixels.
		/// </returns>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates are not
		/// in bounds, or if fArray is too small to hold the output. </exception>
		public virtual float[] GetPixels(int x, int y, int w, int h, float[] fArray)
		{
			return SampleModel_Renamed.GetPixels(x - SampleModelTranslateX_Renamed, y - SampleModelTranslateY_Renamed, w, h, fArray, DataBuffer_Renamed);
		}

		/// <summary>
		/// Returns a double array containing all samples for a rectangle of pixels,
		/// one sample per array element.
		/// An ArrayIndexOutOfBoundsException may be thrown
		/// if the coordinates are not in bounds.  However, explicit bounds
		/// checking is not guaranteed. </summary>
		/// <param name="x">        The X coordinate of the upper-left pixel location </param>
		/// <param name="y">        The Y coordinate of the upper-left pixel location </param>
		/// <param name="w">        Width of the pixel rectangle </param>
		/// <param name="h">        Height of the pixel rectangle </param>
		/// <param name="dArray">   An optionally pre-allocated double array </param>
		/// <returns> the samples for the specified rectangle of pixels.
		/// </returns>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates are not
		/// in bounds, or if dArray is too small to hold the output. </exception>
		public virtual double[] GetPixels(int x, int y, int w, int h, double[] dArray)
		{
			return SampleModel_Renamed.GetPixels(x - SampleModelTranslateX_Renamed, y - SampleModelTranslateY_Renamed, w, h, dArray, DataBuffer_Renamed);
		}


		/// <summary>
		/// Returns the sample in a specified band for the pixel located
		/// at (x,y) as an int.
		/// An ArrayIndexOutOfBoundsException may be thrown
		/// if the coordinates are not in bounds.  However, explicit bounds
		/// checking is not guaranteed. </summary>
		/// <param name="x">        The X coordinate of the pixel location </param>
		/// <param name="y">        The Y coordinate of the pixel location </param>
		/// <param name="b">        The band to return </param>
		/// <returns> the sample in the specified band for the pixel at the
		///         specified coordinate.
		/// </returns>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates or
		/// the band index are not in bounds. </exception>
		public virtual int GetSample(int x, int y, int b)
		{
			return SampleModel_Renamed.GetSample(x - SampleModelTranslateX_Renamed, y - SampleModelTranslateY_Renamed, b, DataBuffer_Renamed);
		}

		/// <summary>
		/// Returns the sample in a specified band
		/// for the pixel located at (x,y) as a float.
		/// An ArrayIndexOutOfBoundsException may be thrown
		/// if the coordinates are not in bounds.  However, explicit bounds
		/// checking is not guaranteed. </summary>
		/// <param name="x">        The X coordinate of the pixel location </param>
		/// <param name="y">        The Y coordinate of the pixel location </param>
		/// <param name="b">        The band to return </param>
		/// <returns> the sample in the specified band for the pixel at the
		///         specified coordinate.
		/// </returns>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates or
		/// the band index are not in bounds. </exception>
		public virtual float GetSampleFloat(int x, int y, int b)
		{
			return SampleModel_Renamed.GetSampleFloat(x - SampleModelTranslateX_Renamed, y - SampleModelTranslateY_Renamed, b, DataBuffer_Renamed);
		}

		/// <summary>
		/// Returns the sample in a specified band
		/// for a pixel located at (x,y) as a double.
		/// An ArrayIndexOutOfBoundsException may be thrown
		/// if the coordinates are not in bounds.  However, explicit bounds
		/// checking is not guaranteed. </summary>
		/// <param name="x">        The X coordinate of the pixel location </param>
		/// <param name="y">        The Y coordinate of the pixel location </param>
		/// <param name="b">        The band to return </param>
		/// <returns> the sample in the specified band for the pixel at the
		///         specified coordinate.
		/// </returns>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates or
		/// the band index are not in bounds. </exception>
		public virtual double GetSampleDouble(int x, int y, int b)
		{
			return SampleModel_Renamed.GetSampleDouble(x - SampleModelTranslateX_Renamed, y - SampleModelTranslateY_Renamed, b, DataBuffer_Renamed);
		}

		/// <summary>
		/// Returns the samples for a specified band for the specified rectangle
		/// of pixels in an int array, one sample per array element.
		/// An ArrayIndexOutOfBoundsException may be thrown
		/// if the coordinates are not in bounds.  However, explicit bounds
		/// checking is not guaranteed. </summary>
		/// <param name="x">        The X coordinate of the upper-left pixel location </param>
		/// <param name="y">        The Y coordinate of the upper-left pixel location </param>
		/// <param name="w">        Width of the pixel rectangle </param>
		/// <param name="h">        Height of the pixel rectangle </param>
		/// <param name="b">        The band to return </param>
		/// <param name="iArray">   An optionally pre-allocated int array </param>
		/// <returns> the samples for the specified band for the specified
		///         rectangle of pixels.
		/// </returns>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates or
		/// the band index are not in bounds, or if iArray is too small to
		/// hold the output. </exception>
		public virtual int[] GetSamples(int x, int y, int w, int h, int b, int[] iArray)
		{
			return SampleModel_Renamed.GetSamples(x - SampleModelTranslateX_Renamed, y - SampleModelTranslateY_Renamed, w, h, b, iArray, DataBuffer_Renamed);
		}

		/// <summary>
		/// Returns the samples for a specified band for the specified rectangle
		/// of pixels in a float array, one sample per array element.
		/// An ArrayIndexOutOfBoundsException may be thrown
		/// if the coordinates are not in bounds.  However, explicit bounds
		/// checking is not guaranteed. </summary>
		/// <param name="x">        The X coordinate of the upper-left pixel location </param>
		/// <param name="y">        The Y coordinate of the upper-left pixel location </param>
		/// <param name="w">        Width of the pixel rectangle </param>
		/// <param name="h">        Height of the pixel rectangle </param>
		/// <param name="b">        The band to return </param>
		/// <param name="fArray">   An optionally pre-allocated float array </param>
		/// <returns> the samples for the specified band for the specified
		///         rectangle of pixels.
		/// </returns>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates or
		/// the band index are not in bounds, or if fArray is too small to
		/// hold the output. </exception>
		public virtual float[] GetSamples(int x, int y, int w, int h, int b, float[] fArray)
		{
			return SampleModel_Renamed.GetSamples(x - SampleModelTranslateX_Renamed, y - SampleModelTranslateY_Renamed, w, h, b, fArray, DataBuffer_Renamed);
		}

		/// <summary>
		/// Returns the samples for a specified band for a specified rectangle
		/// of pixels in a double array, one sample per array element.
		/// An ArrayIndexOutOfBoundsException may be thrown
		/// if the coordinates are not in bounds.  However, explicit bounds
		/// checking is not guaranteed. </summary>
		/// <param name="x">        The X coordinate of the upper-left pixel location </param>
		/// <param name="y">        The Y coordinate of the upper-left pixel location </param>
		/// <param name="w">        Width of the pixel rectangle </param>
		/// <param name="h">        Height of the pixel rectangle </param>
		/// <param name="b">        The band to return </param>
		/// <param name="dArray">   An optionally pre-allocated double array </param>
		/// <returns> the samples for the specified band for the specified
		///         rectangle of pixels.
		/// </returns>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates or
		/// the band index are not in bounds, or if dArray is too small to
		/// hold the output. </exception>
		public virtual double[] GetSamples(int x, int y, int w, int h, int b, double[] dArray)
		{
			 return SampleModel_Renamed.GetSamples(x - SampleModelTranslateX_Renamed, y - SampleModelTranslateY_Renamed, w, h, b, dArray, DataBuffer_Renamed);
		}

	}

}