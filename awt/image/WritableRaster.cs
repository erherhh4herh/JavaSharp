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
	/// This class extends Raster to provide pixel writing capabilities.
	/// Refer to the class comment for Raster for descriptions of how
	/// a Raster stores pixels.
	/// 
	/// <para> The constructors of this class are protected.  To instantiate
	/// a WritableRaster, use one of the createWritableRaster factory methods
	/// in the Raster class.
	/// </para>
	/// </summary>
	public class WritableRaster : Raster
	{

		/// <summary>
		///  Constructs a WritableRaster with the given SampleModel.  The
		///  WritableRaster's upper left corner is origin and it is the
		///  same size as the  SampleModel.  A DataBuffer large enough to
		///  describe the WritableRaster is automatically created. </summary>
		///  <param name="sampleModel">     The SampleModel that specifies the layout. </param>
		///  <param name="origin">          The Point that specifies the origin. </param>
		///  <exception cref="RasterFormatException"> if computing either
		///          <code>origin.x + sampleModel.getWidth()</code> or
		///          <code>origin.y + sampleModel.getHeight()</code> results
		///          in integer overflow </exception>
		protected internal WritableRaster(SampleModel sampleModel, Point origin) : this(sampleModel, sampleModel.CreateDataBuffer(), new Rectangle(origin.x, origin.y, sampleModel.Width, sampleModel.Height), origin, null)
		{
		}

		/// <summary>
		///  Constructs a WritableRaster with the given SampleModel and DataBuffer.
		///  The WritableRaster's upper left corner is origin and it is the same
		///  size as the SampleModel.  The DataBuffer is not initialized and must
		///  be compatible with SampleModel. </summary>
		///  <param name="sampleModel">     The SampleModel that specifies the layout. </param>
		///  <param name="dataBuffer">      The DataBuffer that contains the image data. </param>
		///  <param name="origin">          The Point that specifies the origin. </param>
		///  <exception cref="RasterFormatException"> if computing either
		///          <code>origin.x + sampleModel.getWidth()</code> or
		///          <code>origin.y + sampleModel.getHeight()</code> results
		///          in integer overflow </exception>
		protected internal WritableRaster(SampleModel sampleModel, DataBuffer dataBuffer, Point origin) : this(sampleModel, dataBuffer, new Rectangle(origin.x, origin.y, sampleModel.Width, sampleModel.Height), origin, null)
		{
		}

		/// <summary>
		/// Constructs a WritableRaster with the given SampleModel, DataBuffer,
		/// and parent.  aRegion specifies the bounding rectangle of the new
		/// Raster.  When translated into the base Raster's coordinate
		/// system, aRegion must be contained by the base Raster.
		/// (The base Raster is the Raster's ancestor which has no parent.)
		/// sampleModelTranslate specifies the sampleModelTranslateX and
		/// sampleModelTranslateY values of the new Raster.
		/// 
		/// Note that this constructor should generally be called by other
		/// constructors or create methods, it should not be used directly. </summary>
		/// <param name="sampleModel">     The SampleModel that specifies the layout. </param>
		/// <param name="dataBuffer">      The DataBuffer that contains the image data. </param>
		/// <param name="aRegion">         The Rectangle that specifies the image area. </param>
		/// <param name="sampleModelTranslate">  The Point that specifies the translation
		///                        from SampleModel to Raster coordinates. </param>
		/// <param name="parent">          The parent (if any) of this raster. </param>
		/// <exception cref="RasterFormatException"> if <code>aRegion</code> has width
		///         or height less than or equal to zero, or computing either
		///         <code>aRegion.x + aRegion.width</code> or
		///         <code>aRegion.y + aRegion.height</code> results in integer
		///         overflow </exception>
		protected internal WritableRaster(SampleModel sampleModel, DataBuffer dataBuffer, Rectangle aRegion, Point sampleModelTranslate, WritableRaster parent) : base(sampleModel,dataBuffer,aRegion,sampleModelTranslate,parent)
		{
		}

		/// <summary>
		/// Returns the parent WritableRaster (if any) of this WritableRaster,
		///  or else null. </summary>
		///  <returns> the parent of this <code>WritableRaster</code>, or
		///          <code>null</code>. </returns>
		public virtual WritableRaster WritableParent
		{
			get
			{
				return (WritableRaster)Parent_Renamed;
			}
		}

		/// <summary>
		/// Create a WritableRaster with the same size, SampleModel and DataBuffer
		/// as this one, but with a different location.  The new WritableRaster
		/// will possess a reference to the current WritableRaster, accessible
		/// through its getParent() and getWritableParent() methods.
		/// </summary>
		/// <param name="childMinX"> X coord of the upper left corner of the new Raster. </param>
		/// <param name="childMinY"> Y coord of the upper left corner of the new Raster. </param>
		/// <returns> a <code>WritableRaster</code> the same as this one except
		///         for the specified location. </returns>
		/// <exception cref="RasterFormatException"> if  computing either
		///         <code>childMinX + this.getWidth()</code> or
		///         <code>childMinY + this.getHeight()</code> results in integer
		///         overflow </exception>
		public virtual WritableRaster CreateWritableTranslatedChild(int childMinX, int childMinY)
		{
			return CreateWritableChild(MinX_Renamed,MinY_Renamed,Width_Renamed,Height_Renamed, childMinX,childMinY,null);
		}

		/// <summary>
		/// Returns a new WritableRaster which shares all or part of this
		/// WritableRaster's DataBuffer.  The new WritableRaster will
		/// possess a reference to the current WritableRaster, accessible
		/// through its getParent() and getWritableParent() methods.
		/// 
		/// <para> The parentX, parentY, width and height parameters form a
		/// Rectangle in this WritableRaster's coordinate space, indicating
		/// the area of pixels to be shared.  An error will be thrown if
		/// this Rectangle is not contained with the bounds of the current
		/// WritableRaster.
		/// 
		/// </para>
		/// <para> The new WritableRaster may additionally be translated to a
		/// different coordinate system for the plane than that used by the current
		/// WritableRaster.  The childMinX and childMinY parameters give
		/// the new (x, y) coordinate of the upper-left pixel of the
		/// returned WritableRaster; the coordinate (childMinX, childMinY)
		/// in the new WritableRaster will map to the same pixel as the
		/// coordinate (parentX, parentY) in the current WritableRaster.
		/// 
		/// </para>
		/// <para> The new WritableRaster may be defined to contain only a
		/// subset of the bands of the current WritableRaster, possibly
		/// reordered, by means of the bandList parameter.  If bandList is
		/// null, it is taken to include all of the bands of the current
		/// WritableRaster in their current order.
		/// 
		/// </para>
		/// <para> To create a new WritableRaster that contains a subregion of
		/// the current WritableRaster, but shares its coordinate system
		/// and bands, this method should be called with childMinX equal to
		/// parentX, childMinY equal to parentY, and bandList equal to
		/// null.
		/// 
		/// </para>
		/// </summary>
		/// <param name="parentX">    X coordinate of the upper left corner in this
		///                   WritableRaster's coordinates. </param>
		/// <param name="parentY">    Y coordinate of the upper left corner in this
		///                   WritableRaster's coordinates. </param>
		/// <param name="w">          Width of the region starting at (parentX, parentY). </param>
		/// <param name="h">          Height of the region starting at (parentX, parentY). </param>
		/// <param name="childMinX">  X coordinate of the upper left corner of
		///                   the returned WritableRaster. </param>
		/// <param name="childMinY">  Y coordinate of the upper left corner of
		///                   the returned WritableRaster. </param>
		/// <param name="bandList">   Array of band indices, or null to use all bands. </param>
		/// <returns> a <code>WritableRaster</code> sharing all or part of the
		///         <code>DataBuffer</code> of this <code>WritableRaster</code>. </returns>
		/// <exception cref="RasterFormatException"> if the subregion is outside of the
		///                               raster bounds. </exception>
		/// <exception cref="RasterFormatException"> if <code>w</code> or
		///         <code>h</code>
		///         is less than or equal to zero, or computing any of
		///         <code>parentX + w</code>, <code>parentY + h</code>,
		///         <code>childMinX + w</code>, or
		///         <code>childMinY + h</code> results in integer
		///         overflow </exception>
		public virtual WritableRaster CreateWritableChild(int parentX, int parentY, int w, int h, int childMinX, int childMinY, int[] bandList)
		{
			if (parentX < this.MinX_Renamed)
			{
				throw new RasterFormatException("parentX lies outside raster");
			}
			if (parentY < this.MinY_Renamed)
			{
				throw new RasterFormatException("parentY lies outside raster");
			}
			if ((parentX + w < parentX) || (parentX + w > this.Width_Renamed + this.MinX_Renamed))
			{
				throw new RasterFormatException("(parentX + width) is outside raster");
			}
			if ((parentY + h < parentY) || (parentY + h > this.Height_Renamed + this.MinY_Renamed))
			{
				throw new RasterFormatException("(parentY + height) is outside raster");
			}

			SampleModel sm;
			// Note: the SampleModel for the child Raster should have the same
			// width and height as that for the parent, since it represents
			// the physical layout of the pixel data.  The child Raster's width
			// and height represent a "virtual" view of the pixel data, so
			// they may be different than those of the SampleModel.
			if (bandList != null)
			{
				sm = SampleModel_Renamed.CreateSubsetSampleModel(bandList);
			}
			else
			{
				sm = SampleModel_Renamed;
			}

			int deltaX = childMinX - parentX;
			int deltaY = childMinY - parentY;

			return new WritableRaster(sm, DataBuffer, new Rectangle(childMinX,childMinY, w, h), new Point(SampleModelTranslateX_Renamed + deltaX, SampleModelTranslateY_Renamed + deltaY), this);
		}

		/// <summary>
		/// Sets the data for a single pixel from a
		/// primitive array of type TransferType.  For image data supported by
		/// the Java 2D(tm) API, this will be one of DataBuffer.TYPE_BYTE,
		/// DataBuffer.TYPE_USHORT, DataBuffer.TYPE_INT, DataBuffer.TYPE_SHORT,
		/// DataBuffer.TYPE_FLOAT, or DataBuffer.TYPE_DOUBLE.  Data in the array
		/// may be in a packed format, thus increasing efficiency for data
		/// transfers.
		/// An ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		/// not in bounds, or if inData is not large enough to hold the pixel data.
		/// However, explicit bounds checking is not guaranteed.
		/// A ClassCastException will be thrown if the input object is not null
		/// and references anything other than an array of TransferType. </summary>
		/// <seealso cref= java.awt.image.SampleModel#setDataElements(int, int, Object, DataBuffer) </seealso>
		/// <param name="x">        The X coordinate of the pixel location. </param>
		/// <param name="y">        The Y coordinate of the pixel location. </param>
		/// <param name="inData">   An object reference to an array of type defined by
		///                 getTransferType() and length getNumDataElements()
		///                 containing the pixel data to place at x,y.
		/// </param>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates are not
		/// in bounds, or if inData is too small to hold the input. </exception>
		public virtual void SetDataElements(int x, int y, Object inData)
		{
			SampleModel_Renamed.SetDataElements(x - SampleModelTranslateX_Renamed, y - SampleModelTranslateY_Renamed, inData, DataBuffer_Renamed);
		}

		/// <summary>
		/// Sets the data for a rectangle of pixels from an input Raster.
		/// The input Raster must be compatible with this WritableRaster
		/// in that they must have the same number of bands, corresponding bands
		/// must have the same number of bits per sample, the TransferTypes
		/// and NumDataElements must be the same, and the packing used by
		/// the getDataElements/setDataElements must be identical.
		/// An ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		/// not in bounds.
		/// However, explicit bounds checking is not guaranteed. </summary>
		/// <param name="x">        The X coordinate of the pixel location. </param>
		/// <param name="y">        The Y coordinate of the pixel location. </param>
		/// <param name="inRaster"> Raster containing data to place at x,y.
		/// </param>
		/// <exception cref="NullPointerException"> if inRaster is null. </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates are not
		/// in bounds. </exception>
		public virtual void SetDataElements(int x, int y, Raster inRaster)
		{
			int dstOffX = x + inRaster.MinX;
			int dstOffY = y + inRaster.MinY;
			int width = inRaster.Width;
			int height = inRaster.Height;
			if ((dstOffX < this.MinX_Renamed) || (dstOffY < this.MinY_Renamed) || (dstOffX + width > this.MinX_Renamed + this.Width_Renamed) || (dstOffY + height > this.MinY_Renamed + this.Height_Renamed))
			{
				throw new ArrayIndexOutOfBoundsException("Coordinate out of bounds!");
			}

			int srcOffX = inRaster.MinX;
			int srcOffY = inRaster.MinY;
			Object tdata = null;

			for (int startY = 0; startY < height; startY++)
			{
				tdata = inRaster.GetDataElements(srcOffX, srcOffY + startY, width, 1, tdata);
				SetDataElements(dstOffX, dstOffY + startY, width, 1, tdata);
			}
		}

		/// <summary>
		/// Sets the data for a rectangle of pixels from a
		/// primitive array of type TransferType.  For image data supported by
		/// the Java 2D API, this will be one of DataBuffer.TYPE_BYTE,
		/// DataBuffer.TYPE_USHORT, DataBuffer.TYPE_INT, DataBuffer.TYPE_SHORT,
		/// DataBuffer.TYPE_FLOAT, or DataBuffer.TYPE_DOUBLE.  Data in the array
		/// may be in a packed format, thus increasing efficiency for data
		/// transfers.
		/// An ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		/// not in bounds, or if inData is not large enough to hold the pixel data.
		/// However, explicit bounds checking is not guaranteed.
		/// A ClassCastException will be thrown if the input object is not null
		/// and references anything other than an array of TransferType. </summary>
		/// <seealso cref= java.awt.image.SampleModel#setDataElements(int, int, int, int, Object, DataBuffer) </seealso>
		/// <param name="x">        The X coordinate of the upper left pixel location. </param>
		/// <param name="y">        The Y coordinate of the upper left pixel location. </param>
		/// <param name="w">        Width of the pixel rectangle. </param>
		/// <param name="h">        Height of the pixel rectangle. </param>
		/// <param name="inData">   An object reference to an array of type defined by
		///                 getTransferType() and length w*h*getNumDataElements()
		///                 containing the pixel data to place between x,y and
		///                 x+w-1, y+h-1.
		/// </param>
		/// <exception cref="NullPointerException"> if inData is null. </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates are not
		/// in bounds, or if inData is too small to hold the input. </exception>
		public virtual void SetDataElements(int x, int y, int w, int h, Object inData)
		{
			SampleModel_Renamed.SetDataElements(x - SampleModelTranslateX_Renamed, y - SampleModelTranslateY_Renamed, w,h,inData,DataBuffer_Renamed);
		}

		/// <summary>
		/// Copies pixels from Raster srcRaster to this WritableRaster.  Each pixel
		/// in srcRaster is copied to the same x,y address in this raster, unless
		/// the address falls outside the bounds of this raster.  srcRaster
		/// must have the same number of bands as this WritableRaster.  The
		/// copy is a simple copy of source samples to the corresponding destination
		/// samples.
		/// <para>
		/// If all samples of both source and destination Rasters are of
		/// integral type and less than or equal to 32 bits in size, then calling
		/// this method is equivalent to executing the following code for all
		/// <code>x,y</code> addresses valid in both Rasters.
		/// <pre>{@code
		///       Raster srcRaster;
		///       WritableRaster dstRaster;
		///       for (int b = 0; b < srcRaster.getNumBands(); b++) {
		///           dstRaster.setSample(x, y, b, srcRaster.getSample(x, y, b));
		///       }
		/// }</pre>
		/// Thus, when copying an integral type source to an integral type
		/// destination, if the source sample size is greater than the destination
		/// sample size for a particular band, the high order bits of the source
		/// sample are truncated.  If the source sample size is less than the
		/// destination size for a particular band, the high order bits of the
		/// destination are zero-extended or sign-extended depending on whether
		/// srcRaster's SampleModel treats the sample as a signed or unsigned
		/// quantity.
		/// </para>
		/// <para>
		/// When copying a float or double source to an integral type destination,
		/// each source sample is cast to the destination type.  When copying an
		/// integral type source to a float or double destination, the source
		/// is first converted to a 32-bit int (if necessary), using the above
		/// rules for integral types, and then the int is cast to float or
		/// double.
		/// </para>
		/// <para>
		/// </para>
		/// </summary>
		/// <param name="srcRaster">  The  Raster from which to copy pixels.
		/// </param>
		/// <exception cref="NullPointerException"> if srcRaster is null. </exception>
		public virtual Raster Rect
		{
			set
			{
				SetRect(0,0,value);
			}
		}

		/// <summary>
		/// Copies pixels from Raster srcRaster to this WritableRaster.
		/// For each (x, y) address in srcRaster, the corresponding pixel
		/// is copied to address (x+dx, y+dy) in this WritableRaster,
		/// unless (x+dx, y+dy) falls outside the bounds of this raster.
		/// srcRaster must have the same number of bands as this WritableRaster.
		/// The copy is a simple copy of source samples to the corresponding
		/// destination samples.  For details, see
		/// <seealso cref="WritableRaster#setRect(Raster)"/>.
		/// </summary>
		/// <param name="dx">        The X translation factor from src space to dst space
		///                  of the copy. </param>
		/// <param name="dy">        The Y translation factor from src space to dst space
		///                  of the copy. </param>
		/// <param name="srcRaster"> The Raster from which to copy pixels.
		/// </param>
		/// <exception cref="NullPointerException"> if srcRaster is null. </exception>
		public virtual void SetRect(int dx, int dy, Raster srcRaster)
		{
			int width = srcRaster.Width;
			int height = srcRaster.Height;
			int srcOffX = srcRaster.MinX;
			int srcOffY = srcRaster.MinY;
			int dstOffX = dx + srcOffX;
			int dstOffY = dy + srcOffY;

			// Clip to this raster
			if (dstOffX < this.MinX_Renamed)
			{
				int skipX = this.MinX_Renamed - dstOffX;
				width -= skipX;
				srcOffX += skipX;
				dstOffX = this.MinX_Renamed;
			}
			if (dstOffY < this.MinY_Renamed)
			{
				int skipY = this.MinY_Renamed - dstOffY;
				height -= skipY;
				srcOffY += skipY;
				dstOffY = this.MinY_Renamed;
			}
			if (dstOffX + width > this.MinX_Renamed + this.Width_Renamed)
			{
				width = this.MinX_Renamed + this.Width_Renamed - dstOffX;
			}
			if (dstOffY + height > this.MinY_Renamed + this.Height_Renamed)
			{
				height = this.MinY_Renamed + this.Height_Renamed - dstOffY;
			}

			if (width <= 0 || height <= 0)
			{
				return;
			}

			switch (srcRaster.SampleModel.DataType)
			{
			case DataBuffer.TYPE_BYTE:
			case DataBuffer.TYPE_SHORT:
			case DataBuffer.TYPE_USHORT:
			case DataBuffer.TYPE_INT:
				int[] iData = null;
				for (int startY = 0; startY < height; startY++)
				{
					// Grab one scanline at a time
					iData = srcRaster.GetPixels(srcOffX, srcOffY + startY, width, 1, iData);
					SetPixels(dstOffX, dstOffY + startY, width, 1, iData);
				}
				break;

			case DataBuffer.TYPE_FLOAT:
				float[] fData = null;
				for (int startY = 0; startY < height; startY++)
				{
					fData = srcRaster.GetPixels(srcOffX, srcOffY + startY, width, 1, fData);
					SetPixels(dstOffX, dstOffY + startY, width, 1, fData);
				}
				break;

			case DataBuffer.TYPE_DOUBLE:
				double[] dData = null;
				for (int startY = 0; startY < height; startY++)
				{
					// Grab one scanline at a time
					dData = srcRaster.GetPixels(srcOffX, srcOffY + startY, width, 1, dData);
					SetPixels(dstOffX, dstOffY + startY, width, 1, dData);
				}
				break;
			}
		}

		/// <summary>
		/// Sets a pixel in the DataBuffer using an int array of samples for input.
		/// An ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		/// not in bounds.
		/// However, explicit bounds checking is not guaranteed. </summary>
		/// <param name="x">      The X coordinate of the pixel location. </param>
		/// <param name="y">      The Y coordinate of the pixel location. </param>
		/// <param name="iArray"> The input samples in a int array.
		/// </param>
		/// <exception cref="NullPointerException"> if iArray is null. </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates are not
		/// in bounds, or if iArray is too small to hold the input. </exception>
		public virtual void SetPixel(int x, int y, int[] iArray)
		{
			SampleModel_Renamed.SetPixel(x - SampleModelTranslateX_Renamed,y - SampleModelTranslateY_Renamed, iArray,DataBuffer_Renamed);
		}

		/// <summary>
		/// Sets a pixel in the DataBuffer using a float array of samples for input.
		/// An ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		/// not in bounds.
		/// However, explicit bounds checking is not guaranteed. </summary>
		/// <param name="x">      The X coordinate of the pixel location. </param>
		/// <param name="y">      The Y coordinate of the pixel location. </param>
		/// <param name="fArray"> The input samples in a float array.
		/// </param>
		/// <exception cref="NullPointerException"> if fArray is null. </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates are not
		/// in bounds, or if fArray is too small to hold the input. </exception>
		public virtual void SetPixel(int x, int y, float[] fArray)
		{
			SampleModel_Renamed.SetPixel(x - SampleModelTranslateX_Renamed,y - SampleModelTranslateY_Renamed, fArray,DataBuffer_Renamed);
		}

		/// <summary>
		/// Sets a pixel in the DataBuffer using a double array of samples for input.
		/// An ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		/// not in bounds.
		/// However, explicit bounds checking is not guaranteed. </summary>
		/// <param name="x">      The X coordinate of the pixel location. </param>
		/// <param name="y">      The Y coordinate of the pixel location. </param>
		/// <param name="dArray"> The input samples in a double array.
		/// </param>
		/// <exception cref="NullPointerException"> if dArray is null. </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates are not
		/// in bounds, or if dArray is too small to hold the input. </exception>
		public virtual void SetPixel(int x, int y, double[] dArray)
		{
			SampleModel_Renamed.SetPixel(x - SampleModelTranslateX_Renamed,y - SampleModelTranslateY_Renamed, dArray,DataBuffer_Renamed);
		}

		/// <summary>
		/// Sets all samples for a rectangle of pixels from an int array containing
		/// one sample per array element.
		/// An ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		/// not in bounds.
		/// However, explicit bounds checking is not guaranteed. </summary>
		/// <param name="x">        The X coordinate of the upper left pixel location. </param>
		/// <param name="y">        The Y coordinate of the upper left pixel location. </param>
		/// <param name="w">        Width of the pixel rectangle. </param>
		/// <param name="h">        Height of the pixel rectangle. </param>
		/// <param name="iArray">   The input int pixel array.
		/// </param>
		/// <exception cref="NullPointerException"> if iArray is null. </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates are not
		/// in bounds, or if iArray is too small to hold the input. </exception>
		public virtual void SetPixels(int x, int y, int w, int h, int[] iArray)
		{
			SampleModel_Renamed.SetPixels(x - SampleModelTranslateX_Renamed,y - SampleModelTranslateY_Renamed, w,h,iArray,DataBuffer_Renamed);
		}

		/// <summary>
		/// Sets all samples for a rectangle of pixels from a float array containing
		/// one sample per array element.
		/// An ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		/// not in bounds.
		/// However, explicit bounds checking is not guaranteed. </summary>
		/// <param name="x">        The X coordinate of the upper left pixel location. </param>
		/// <param name="y">        The Y coordinate of the upper left pixel location. </param>
		/// <param name="w">        Width of the pixel rectangle. </param>
		/// <param name="h">        Height of the pixel rectangle. </param>
		/// <param name="fArray">   The input float pixel array.
		/// </param>
		/// <exception cref="NullPointerException"> if fArray is null. </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates are not
		/// in bounds, or if fArray is too small to hold the input. </exception>
		public virtual void SetPixels(int x, int y, int w, int h, float[] fArray)
		{
			SampleModel_Renamed.SetPixels(x - SampleModelTranslateX_Renamed,y - SampleModelTranslateY_Renamed, w,h,fArray,DataBuffer_Renamed);
		}

		/// <summary>
		/// Sets all samples for a rectangle of pixels from a double array containing
		/// one sample per array element.
		/// An ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		/// not in bounds.
		/// However, explicit bounds checking is not guaranteed. </summary>
		/// <param name="x">        The X coordinate of the upper left pixel location. </param>
		/// <param name="y">        The Y coordinate of the upper left pixel location. </param>
		/// <param name="w">        Width of the pixel rectangle. </param>
		/// <param name="h">        Height of the pixel rectangle. </param>
		/// <param name="dArray">   The input double pixel array.
		/// </param>
		/// <exception cref="NullPointerException"> if dArray is null. </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates are not
		/// in bounds, or if dArray is too small to hold the input. </exception>
		public virtual void SetPixels(int x, int y, int w, int h, double[] dArray)
		{
			SampleModel_Renamed.SetPixels(x - SampleModelTranslateX_Renamed,y - SampleModelTranslateY_Renamed, w,h,dArray,DataBuffer_Renamed);
		}

		/// <summary>
		/// Sets a sample in the specified band for the pixel located at (x,y)
		/// in the DataBuffer using an int for input.
		/// An ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		/// not in bounds.
		/// However, explicit bounds checking is not guaranteed. </summary>
		/// <param name="x">        The X coordinate of the pixel location. </param>
		/// <param name="y">        The Y coordinate of the pixel location. </param>
		/// <param name="b">        The band to set. </param>
		/// <param name="s">        The input sample.
		/// </param>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates or
		/// the band index are not in bounds. </exception>
		public virtual void SetSample(int x, int y, int b, int s)
		{
			SampleModel_Renamed.SetSample(x - SampleModelTranslateX_Renamed, y - SampleModelTranslateY_Renamed, b, s, DataBuffer_Renamed);
		}

		/// <summary>
		/// Sets a sample in the specified band for the pixel located at (x,y)
		/// in the DataBuffer using a float for input.
		/// An ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		/// not in bounds.
		/// However, explicit bounds checking is not guaranteed. </summary>
		/// <param name="x">        The X coordinate of the pixel location. </param>
		/// <param name="y">        The Y coordinate of the pixel location. </param>
		/// <param name="b">        The band to set. </param>
		/// <param name="s">        The input sample as a float.
		/// </param>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates or
		/// the band index are not in bounds. </exception>
		public virtual void SetSample(int x, int y, int b, float s)
		{
			SampleModel_Renamed.SetSample(x - SampleModelTranslateX_Renamed,y - SampleModelTranslateY_Renamed, b,s,DataBuffer_Renamed);
		}

		/// <summary>
		/// Sets a sample in the specified band for the pixel located at (x,y)
		/// in the DataBuffer using a double for input.
		/// An ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		/// not in bounds.
		/// However, explicit bounds checking is not guaranteed. </summary>
		/// <param name="x">        The X coordinate of the pixel location. </param>
		/// <param name="y">        The Y coordinate of the pixel location. </param>
		/// <param name="b">        The band to set. </param>
		/// <param name="s">        The input sample as a double.
		/// </param>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates or
		/// the band index are not in bounds. </exception>
		public virtual void SetSample(int x, int y, int b, double s)
		{
			SampleModel_Renamed.SetSample(x - SampleModelTranslateX_Renamed,y - SampleModelTranslateY_Renamed, b,s,DataBuffer_Renamed);
		}

		/// <summary>
		/// Sets the samples in the specified band for the specified rectangle
		/// of pixels from an int array containing one sample per array element.
		/// An ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		/// not in bounds.
		/// However, explicit bounds checking is not guaranteed. </summary>
		/// <param name="x">        The X coordinate of the upper left pixel location. </param>
		/// <param name="y">        The Y coordinate of the upper left pixel location. </param>
		/// <param name="w">        Width of the pixel rectangle. </param>
		/// <param name="h">        Height of the pixel rectangle. </param>
		/// <param name="b">        The band to set. </param>
		/// <param name="iArray">   The input int sample array.
		/// </param>
		/// <exception cref="NullPointerException"> if iArray is null. </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates or
		/// the band index are not in bounds, or if iArray is too small to
		/// hold the input. </exception>
		public virtual void SetSamples(int x, int y, int w, int h, int b, int[] iArray)
		{
			SampleModel_Renamed.SetSamples(x - SampleModelTranslateX_Renamed,y - SampleModelTranslateY_Renamed, w,h,b,iArray,DataBuffer_Renamed);
		}

		/// <summary>
		/// Sets the samples in the specified band for the specified rectangle
		/// of pixels from a float array containing one sample per array element.
		/// An ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		/// not in bounds.
		/// However, explicit bounds checking is not guaranteed. </summary>
		/// <param name="x">        The X coordinate of the upper left pixel location. </param>
		/// <param name="y">        The Y coordinate of the upper left pixel location. </param>
		/// <param name="w">        Width of the pixel rectangle. </param>
		/// <param name="h">        Height of the pixel rectangle. </param>
		/// <param name="b">        The band to set. </param>
		/// <param name="fArray">   The input float sample array.
		/// </param>
		/// <exception cref="NullPointerException"> if fArray is null. </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates or
		/// the band index are not in bounds, or if fArray is too small to
		/// hold the input. </exception>
		public virtual void SetSamples(int x, int y, int w, int h, int b, float[] fArray)
		{
			SampleModel_Renamed.SetSamples(x - SampleModelTranslateX_Renamed,y - SampleModelTranslateY_Renamed, w,h,b,fArray,DataBuffer_Renamed);
		}

		/// <summary>
		/// Sets the samples in the specified band for the specified rectangle
		/// of pixels from a double array containing one sample per array element.
		/// An ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		/// not in bounds.
		/// However, explicit bounds checking is not guaranteed. </summary>
		/// <param name="x">        The X coordinate of the upper left pixel location. </param>
		/// <param name="y">        The Y coordinate of the upper left pixel location. </param>
		/// <param name="w">        Width of the pixel rectangle. </param>
		/// <param name="h">        Height of the pixel rectangle. </param>
		/// <param name="b">        The band to set. </param>
		/// <param name="dArray">   The input double sample array.
		/// </param>
		/// <exception cref="NullPointerException"> if dArray is null. </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates or
		/// the band index are not in bounds, or if dArray is too small to
		/// hold the input. </exception>
		public virtual void SetSamples(int x, int y, int w, int h, int b, double[] dArray)
		{
			SampleModel_Renamed.SetSamples(x - SampleModelTranslateX_Renamed,y - SampleModelTranslateY_Renamed, w,h,b,dArray,DataBuffer_Renamed);
		}

	}

}