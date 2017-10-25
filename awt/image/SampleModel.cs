using System.Runtime.InteropServices;

/*
 * Copyright (c) 1997, 2011, Oracle and/or its affiliates. All rights reserved.
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
	///  This abstract class defines an interface for extracting samples of pixels
	///  in an image.  All image data is expressed as a collection of pixels.
	///  Each pixel consists of a number of samples. A sample is a datum
	///  for one band of an image and a band consists of all samples of a
	///  particular type in an image.  For example, a pixel might contain
	///  three samples representing its red, green and blue components.
	///  There are three bands in the image containing this pixel.  One band
	///  consists of all the red samples from all pixels in the
	///  image.  The second band consists of all the green samples and
	///  the remaining band consists of all of the blue samples.  The pixel
	///  can be stored in various formats.  For example, all samples from
	///  a particular band can be stored contiguously or all samples from a
	///  single pixel can be stored contiguously.
	///  <para>
	///  Subclasses of SampleModel specify the types of samples they can
	///  represent (e.g. unsigned 8-bit byte, signed 16-bit short, etc.)
	///  and may specify how the samples are organized in memory.
	///  In the Java 2D(tm) API, built-in image processing operators may
	///  not operate on all possible sample types, but generally will work
	///  for unsigned integral samples of 16 bits or less.  Some operators
	///  support a wider variety of sample types.
	/// </para>
	///  <para>
	///  A collection of pixels is represented as a Raster, which consists of
	///  a DataBuffer and a SampleModel.  The SampleModel allows access to
	///  samples in the DataBuffer and may provide low-level information that
	///  a programmer can use to directly manipulate samples and pixels in the
	///  DataBuffer.
	/// </para>
	///  <para>
	///  This class is generally a fall back method for dealing with
	///  images.  More efficient code will cast the SampleModel to the
	///  appropriate subclass and extract the information needed to directly
	///  manipulate pixels in the DataBuffer.
	/// 
	/// </para>
	/// </summary>
	///  <seealso cref= java.awt.image.DataBuffer </seealso>
	///  <seealso cref= java.awt.image.Raster </seealso>
	///  <seealso cref= java.awt.image.ComponentSampleModel </seealso>
	///  <seealso cref= java.awt.image.PixelInterleavedSampleModel </seealso>
	///  <seealso cref= java.awt.image.BandedSampleModel </seealso>
	///  <seealso cref= java.awt.image.MultiPixelPackedSampleModel </seealso>
	///  <seealso cref= java.awt.image.SinglePixelPackedSampleModel </seealso>

	public abstract class SampleModel
	{

		/// <summary>
		/// Width in pixels of the region of image data that this SampleModel
		///  describes.
		/// </summary>
		protected internal int Width_Renamed;

		/// <summary>
		/// Height in pixels of the region of image data that this SampleModel
		///  describes.
		/// </summary>
		protected internal int Height_Renamed;

		/// <summary>
		/// Number of bands of the image data that this SampleModel describes. </summary>
		protected internal int NumBands_Renamed;

		/// <summary>
		/// Data type of the DataBuffer storing the pixel data. </summary>
		///  <seealso cref= java.awt.image.DataBuffer </seealso>
		protected internal int DataType_Renamed;

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern void initIDs();
		static SampleModel()
		{
			ColorModel.LoadLibraries();
			initIDs();
		}

		/// <summary>
		/// Constructs a SampleModel with the specified parameters. </summary>
		/// <param name="dataType">  The data type of the DataBuffer storing the pixel data. </param>
		/// <param name="w">         The width (in pixels) of the region of image data. </param>
		/// <param name="h">         The height (in pixels) of the region of image data. </param>
		/// <param name="numBands">  The number of bands of the image data. </param>
		/// <exception cref="IllegalArgumentException"> if <code>w</code> or <code>h</code>
		///         is not greater than 0 </exception>
		/// <exception cref="IllegalArgumentException"> if the product of <code>w</code>
		///         and <code>h</code> is greater than
		///         <code>Integer.MAX_VALUE</code> </exception>
		/// <exception cref="IllegalArgumentException"> if <code>dataType</code> is not
		///         one of the supported data types </exception>
		public SampleModel(int dataType, int w, int h, int numBands)
		{
			long size = (long)w * h;
			if (w <= 0 || h <= 0)
			{
				throw new IllegalArgumentException("Width (" + w + ") and height (" + h + ") must be > 0");
			}
			if (size >= Integer.MaxValue)
			{
				throw new IllegalArgumentException("Dimensions (width=" + w + " height=" + h + ") are too large");
			}

			if (dataType < DataBuffer.TYPE_BYTE || (dataType > DataBuffer.TYPE_DOUBLE && dataType != DataBuffer.TYPE_UNDEFINED))
			{
				throw new IllegalArgumentException("Unsupported dataType: " + dataType);
			}

			if (numBands <= 0)
			{
				throw new IllegalArgumentException("Number of bands must be > 0");
			}

			this.DataType_Renamed = dataType;
			this.Width_Renamed = w;
			this.Height_Renamed = h;
			this.NumBands_Renamed = numBands;
		}

		/// <summary>
		/// Returns the width in pixels. </summary>
		///  <returns> the width in pixels of the region of image data
		///          that this <code>SampleModel</code> describes. </returns>
		public int Width
		{
			get
			{
				 return Width_Renamed;
			}
		}

		/// <summary>
		/// Returns the height in pixels. </summary>
		///  <returns> the height in pixels of the region of image data
		///          that this <code>SampleModel</code> describes. </returns>
		public int Height
		{
			get
			{
				 return Height_Renamed;
			}
		}

		/// <summary>
		/// Returns the total number of bands of image data. </summary>
		///  <returns> the number of bands of image data that this
		///          <code>SampleModel</code> describes. </returns>
		public int NumBands
		{
			get
			{
				 return NumBands_Renamed;
			}
		}

		/// <summary>
		/// Returns the number of data elements needed to transfer a pixel
		///  via the getDataElements and setDataElements methods.  When pixels
		///  are transferred via these methods, they may be transferred in a
		///  packed or unpacked format, depending on the implementation of the
		///  SampleModel.  Using these methods, pixels are transferred as an
		///  array of getNumDataElements() elements of a primitive type given
		///  by getTransferType().  The TransferType may or may not be the same
		///  as the storage DataType. </summary>
		///  <returns> the number of data elements. </returns>
		///  <seealso cref= #getDataElements(int, int, Object, DataBuffer) </seealso>
		///  <seealso cref= #getDataElements(int, int, int, int, Object, DataBuffer) </seealso>
		///  <seealso cref= #setDataElements(int, int, Object, DataBuffer) </seealso>
		///  <seealso cref= #setDataElements(int, int, int, int, Object, DataBuffer) </seealso>
		///  <seealso cref= #getTransferType </seealso>
		public abstract int NumDataElements {get;}

		/// <summary>
		/// Returns the data type of the DataBuffer storing the pixel data. </summary>
		///  <returns> the data type. </returns>
		public int DataType
		{
			get
			{
				return DataType_Renamed;
			}
		}

		/// <summary>
		/// Returns the TransferType used to transfer pixels via the
		///  getDataElements and setDataElements methods.  When pixels
		///  are transferred via these methods, they may be transferred in a
		///  packed or unpacked format, depending on the implementation of the
		///  SampleModel.  Using these methods, pixels are transferred as an
		///  array of getNumDataElements() elements of a primitive type given
		///  by getTransferType().  The TransferType may or may not be the same
		///  as the storage DataType.  The TransferType will be one of the types
		///  defined in DataBuffer. </summary>
		///  <returns> the transfer type. </returns>
		///  <seealso cref= #getDataElements(int, int, Object, DataBuffer) </seealso>
		///  <seealso cref= #getDataElements(int, int, int, int, Object, DataBuffer) </seealso>
		///  <seealso cref= #setDataElements(int, int, Object, DataBuffer) </seealso>
		///  <seealso cref= #setDataElements(int, int, int, int, Object, DataBuffer) </seealso>
		///  <seealso cref= #getNumDataElements </seealso>
		///  <seealso cref= java.awt.image.DataBuffer </seealso>
		public virtual int TransferType
		{
			get
			{
				return DataType_Renamed;
			}
		}

		/// <summary>
		/// Returns the samples for a specified pixel in an int array,
		/// one sample per array element.
		/// ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		/// not in bounds. </summary>
		/// <param name="x">         The X coordinate of the pixel location </param>
		/// <param name="y">         The Y coordinate of the pixel location </param>
		/// <param name="iArray">    If non-null, returns the samples in this array </param>
		/// <param name="data">      The DataBuffer containing the image data </param>
		/// <returns> the samples for the specified pixel. </returns>
		/// <seealso cref= #setPixel(int, int, int[], DataBuffer)
		/// </seealso>
		/// <exception cref="NullPointerException"> if data is null. </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates are
		/// not in bounds, or if iArray is too small to hold the output. </exception>
		public virtual int[] GetPixel(int x, int y, int[] iArray, DataBuffer data)
		{

			int[] pixels;

			if (iArray != null)
			{
				pixels = iArray;
			}
			else
			{
				pixels = new int[NumBands_Renamed];
			}

			for (int i = 0; i < NumBands_Renamed; i++)
			{
				pixels[i] = GetSample(x, y, i, data);
			}

			return pixels;
		}

		/// <summary>
		/// Returns data for a single pixel in a primitive array of type
		/// TransferType.  For image data supported by the Java 2D API, this
		/// will be one of DataBuffer.TYPE_BYTE, DataBuffer.TYPE_USHORT,
		/// DataBuffer.TYPE_INT, DataBuffer.TYPE_SHORT, DataBuffer.TYPE_FLOAT,
		/// or DataBuffer.TYPE_DOUBLE.  Data may be returned in a packed format,
		/// thus increasing efficiency for data transfers. Generally, obj
		/// should be passed in as null, so that the Object will be created
		/// automatically and will be of the right primitive data type.
		/// <para>
		/// The following code illustrates transferring data for one pixel from
		/// DataBuffer <code>db1</code>, whose storage layout is described by
		/// SampleModel <code>sm1</code>, to DataBuffer <code>db2</code>, whose
		/// storage layout is described by SampleModel <code>sm2</code>.
		/// The transfer will generally be more efficient than using
		/// getPixel/setPixel.
		/// <pre>
		///       SampleModel sm1, sm2;
		///       DataBuffer db1, db2;
		///       sm2.setDataElements(x, y, sm1.getDataElements(x, y, null, db1), db2);
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
		/// <returns> the data elements for the specified pixel. </returns>
		/// <seealso cref= #getNumDataElements </seealso>
		/// <seealso cref= #getTransferType </seealso>
		/// <seealso cref= java.awt.image.DataBuffer </seealso>
		/// <seealso cref= #setDataElements(int, int, Object, DataBuffer)
		/// </seealso>
		/// <exception cref="NullPointerException"> if data is null. </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates are
		/// not in bounds, or if obj is too small to hold the output. </exception>
		public abstract Object GetDataElements(int x, int y, Object obj, DataBuffer data);

		/// <summary>
		/// Returns the pixel data for the specified rectangle of pixels in a
		/// primitive array of type TransferType.
		/// For image data supported by the Java 2D API, this
		/// will be one of DataBuffer.TYPE_BYTE, DataBuffer.TYPE_USHORT,
		/// DataBuffer.TYPE_INT, DataBuffer.TYPE_SHORT, DataBuffer.TYPE_FLOAT,
		/// or DataBuffer.TYPE_DOUBLE.  Data may be returned in a packed format,
		/// thus increasing efficiency for data transfers. Generally, obj
		/// should be passed in as null, so that the Object will be created
		/// automatically and will be of the right primitive data type.
		/// <para>
		/// The following code illustrates transferring data for a rectangular
		/// region of pixels from
		/// DataBuffer <code>db1</code>, whose storage layout is described by
		/// SampleModel <code>sm1</code>, to DataBuffer <code>db2</code>, whose
		/// storage layout is described by SampleModel <code>sm2</code>.
		/// The transfer will generally be more efficient than using
		/// getPixels/setPixels.
		/// <pre>
		///       SampleModel sm1, sm2;
		///       DataBuffer db1, db2;
		///       sm2.setDataElements(x, y, w, h, sm1.getDataElements(x, y, w,
		///                           h, null, db1), db2);
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
		/// <param name="x">         The minimum X coordinate of the pixel rectangle. </param>
		/// <param name="y">         The minimum Y coordinate of the pixel rectangle. </param>
		/// <param name="w">         The width of the pixel rectangle. </param>
		/// <param name="h">         The height of the pixel rectangle. </param>
		/// <param name="obj">       If non-null, a primitive array in which to return
		///                  the pixel data. </param>
		/// <param name="data">      The DataBuffer containing the image data. </param>
		/// <returns> the data elements for the specified region of pixels. </returns>
		/// <seealso cref= #getNumDataElements </seealso>
		/// <seealso cref= #getTransferType </seealso>
		/// <seealso cref= #setDataElements(int, int, int, int, Object, DataBuffer) </seealso>
		/// <seealso cref= java.awt.image.DataBuffer
		/// </seealso>
		/// <exception cref="NullPointerException"> if data is null. </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates are
		/// not in bounds, or if obj is too small to hold the output. </exception>
		public virtual Object GetDataElements(int x, int y, int w, int h, Object obj, DataBuffer data)
		{

			int type = TransferType;
			int numDataElems = NumDataElements;
			int cnt = 0;
			Object o = null;

			int x1 = x + w;
			int y1 = y + h;

			if (x < 0 || x >= Width_Renamed || w > Width_Renamed || x1 < 0 || x1 > Width_Renamed || y < 0 || y >= Height_Renamed || h > Height_Renamed || y1 < 0 || y1 > Height_Renamed)
			{
				throw new ArrayIndexOutOfBoundsException("Invalid coordinates.");
			}

			switch (type)
			{

			case DataBuffer.TYPE_BYTE:

				sbyte[] btemp;
				sbyte[] bdata;

				if (obj == null)
				{
					bdata = new sbyte[numDataElems * w * h];
				}
				else
				{
					bdata = (sbyte[])obj;
				}

				for (int i = y; i < y1; i++)
				{
					for (int j = x; j < x1; j++)
					{
						o = GetDataElements(j, i, o, data);
						btemp = (sbyte[])o;
						for (int k = 0; k < numDataElems; k++)
						{
							bdata[cnt++] = btemp[k];
						}
					}
				}
				obj = (Object)bdata;
				break;

			case DataBuffer.TYPE_USHORT:
			case DataBuffer.TYPE_SHORT:

				short[] sdata;
				short[] stemp;

				if (obj == null)
				{
					sdata = new short[numDataElems * w * h];
				}
				else
				{
					sdata = (short[])obj;
				}

				for (int i = y; i < y1; i++)
				{
					for (int j = x; j < x1; j++)
					{
						o = GetDataElements(j, i, o, data);
						stemp = (short[])o;
						for (int k = 0; k < numDataElems; k++)
						{
							sdata[cnt++] = stemp[k];
						}
					}
				}

				obj = (Object)sdata;
				break;

			case DataBuffer.TYPE_INT:

				int[] idata;
				int[] itemp;

				if (obj == null)
				{
					idata = new int[numDataElems * w * h];
				}
				else
				{
					idata = (int[])obj;
				}

				for (int i = y; i < y1; i++)
				{
					for (int j = x; j < x1; j++)
					{
						o = GetDataElements(j, i, o, data);
						itemp = (int[])o;
						for (int k = 0; k < numDataElems; k++)
						{
							idata[cnt++] = itemp[k];
						}
					}
				}

				obj = (Object)idata;
				break;

			case DataBuffer.TYPE_FLOAT:

				float[] fdata;
				float[] ftemp;

				if (obj == null)
				{
					fdata = new float[numDataElems * w * h];
				}
				else
				{
					fdata = (float[])obj;
				}

				for (int i = y; i < y1; i++)
				{
					for (int j = x; j < x1; j++)
					{
						o = GetDataElements(j, i, o, data);
						ftemp = (float[])o;
						for (int k = 0; k < numDataElems; k++)
						{
							fdata[cnt++] = ftemp[k];
						}
					}
				}

				obj = (Object)fdata;
				break;

			case DataBuffer.TYPE_DOUBLE:

				double[] ddata;
				double[] dtemp;

				if (obj == null)
				{
					ddata = new double[numDataElems * w * h];
				}
				else
				{
					ddata = (double[])obj;
				}

				for (int i = y; i < y1; i++)
				{
					for (int j = x; j < x1; j++)
					{
						o = GetDataElements(j, i, o, data);
						dtemp = (double[])o;
						for (int k = 0; k < numDataElems; k++)
						{
							ddata[cnt++] = dtemp[k];
						}
					}
				}

				obj = (Object)ddata;
				break;
			}

			return obj;
		}

		/// <summary>
		/// Sets the data for a single pixel in the specified DataBuffer from a
		/// primitive array of type TransferType.  For image data supported by
		/// the Java 2D API, this will be one of DataBuffer.TYPE_BYTE,
		/// DataBuffer.TYPE_USHORT, DataBuffer.TYPE_INT, DataBuffer.TYPE_SHORT,
		/// DataBuffer.TYPE_FLOAT, or DataBuffer.TYPE_DOUBLE.  Data in the array
		/// may be in a packed format, thus increasing efficiency for data
		/// transfers.
		/// <para>
		/// The following code illustrates transferring data for one pixel from
		/// DataBuffer <code>db1</code>, whose storage layout is described by
		/// SampleModel <code>sm1</code>, to DataBuffer <code>db2</code>, whose
		/// storage layout is described by SampleModel <code>sm2</code>.
		/// The transfer will generally be more efficient than using
		/// getPixel/setPixel.
		/// <pre>
		///       SampleModel sm1, sm2;
		///       DataBuffer db1, db2;
		///       sm2.setDataElements(x, y, sm1.getDataElements(x, y, null, db1),
		///                           db2);
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
		/// <seealso cref= #getNumDataElements </seealso>
		/// <seealso cref= #getTransferType </seealso>
		/// <seealso cref= #getDataElements(int, int, Object, DataBuffer) </seealso>
		/// <seealso cref= java.awt.image.DataBuffer
		/// </seealso>
		/// <exception cref="NullPointerException"> if data is null. </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates are
		/// not in bounds, or if obj is too small to hold the input. </exception>
		public abstract void SetDataElements(int x, int y, Object obj, DataBuffer data);

		/// <summary>
		/// Sets the data for a rectangle of pixels in the specified DataBuffer
		/// from a primitive array of type TransferType.  For image data supported
		/// by the Java 2D API, this will be one of DataBuffer.TYPE_BYTE,
		/// DataBuffer.TYPE_USHORT, DataBuffer.TYPE_INT, DataBuffer.TYPE_SHORT,
		/// DataBuffer.TYPE_FLOAT, or DataBuffer.TYPE_DOUBLE.  Data in the array
		/// may be in a packed format, thus increasing efficiency for data
		/// transfers.
		/// <para>
		/// The following code illustrates transferring data for a rectangular
		/// region of pixels from
		/// DataBuffer <code>db1</code>, whose storage layout is described by
		/// SampleModel <code>sm1</code>, to DataBuffer <code>db2</code>, whose
		/// storage layout is described by SampleModel <code>sm2</code>.
		/// The transfer will generally be more efficient than using
		/// getPixels/setPixels.
		/// <pre>
		///       SampleModel sm1, sm2;
		///       DataBuffer db1, db2;
		///       sm2.setDataElements(x, y, w, h, sm1.getDataElements(x, y, w, h,
		///                           null, db1), db2);
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
		/// <param name="x">         The minimum X coordinate of the pixel rectangle. </param>
		/// <param name="y">         The minimum Y coordinate of the pixel rectangle. </param>
		/// <param name="w">         The width of the pixel rectangle. </param>
		/// <param name="h">         The height of the pixel rectangle. </param>
		/// <param name="obj">       A primitive array containing pixel data. </param>
		/// <param name="data">      The DataBuffer containing the image data. </param>
		/// <seealso cref= #getNumDataElements </seealso>
		/// <seealso cref= #getTransferType </seealso>
		/// <seealso cref= #getDataElements(int, int, int, int, Object, DataBuffer) </seealso>
		/// <seealso cref= java.awt.image.DataBuffer
		/// </seealso>
		/// <exception cref="NullPointerException"> if data is null. </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates are
		/// not in bounds, or if obj is too small to hold the input. </exception>
		public virtual void SetDataElements(int x, int y, int w, int h, Object obj, DataBuffer data)
		{

			int cnt = 0;
			Object o = null;
			int type = TransferType;
			int numDataElems = NumDataElements;

			int x1 = x + w;
			int y1 = y + h;

			if (x < 0 || x >= Width_Renamed || w > Width_Renamed || x1 < 0 || x1 > Width_Renamed || y < 0 || y >= Height_Renamed || h > Height_Renamed || y1 < 0 || y1 > Height_Renamed)
			{
				throw new ArrayIndexOutOfBoundsException("Invalid coordinates.");
			}

			switch (type)
			{

			case DataBuffer.TYPE_BYTE:

				sbyte[] barray = (sbyte[])obj;
				sbyte[] btemp = new sbyte[numDataElems];

				for (int i = y; i < y1; i++)
				{
					for (int j = x; j < x1; j++)
					{
						for (int k = 0; k < numDataElems; k++)
						{
							btemp[k] = barray[cnt++];
						}

						SetDataElements(j, i, btemp, data);
					}
				}
				break;

			case DataBuffer.TYPE_USHORT:
			case DataBuffer.TYPE_SHORT:

				short[] sarray = (short[])obj;
				short[] stemp = new short[numDataElems];

				for (int i = y; i < y1; i++)
				{
					for (int j = x; j < x1; j++)
					{
						for (int k = 0; k < numDataElems; k++)
						{
							stemp[k] = sarray[cnt++];
						}

						SetDataElements(j, i, stemp, data);
					}
				}
				break;

			case DataBuffer.TYPE_INT:

				int[] iArray = (int[])obj;
				int[] itemp = new int[numDataElems];

				for (int i = y; i < y1; i++)
				{
					for (int j = x; j < x1; j++)
					{
						for (int k = 0; k < numDataElems; k++)
						{
							itemp[k] = iArray[cnt++];
						}

						SetDataElements(j, i, itemp, data);
					}
				}
				break;

			case DataBuffer.TYPE_FLOAT:

				float[] fArray = (float[])obj;
				float[] ftemp = new float[numDataElems];

				for (int i = y; i < y1; i++)
				{
					for (int j = x; j < x1; j++)
					{
						for (int k = 0; k < numDataElems; k++)
						{
							ftemp[k] = fArray[cnt++];
						}

						SetDataElements(j, i, ftemp, data);
					}
				}
				break;

			case DataBuffer.TYPE_DOUBLE:

				double[] dArray = (double[])obj;
				double[] dtemp = new double[numDataElems];

				for (int i = y; i < y1; i++)
				{
					for (int j = x; j < x1; j++)
					{
						for (int k = 0; k < numDataElems; k++)
						{
							dtemp[k] = dArray[cnt++];
						}

						SetDataElements(j, i, dtemp, data);
					}
				}
				break;
			}

		}

		/// <summary>
		/// Returns the samples for the specified pixel in an array of float.
		/// ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		/// not in bounds. </summary>
		/// <param name="x">         The X coordinate of the pixel location. </param>
		/// <param name="y">         The Y coordinate of the pixel location. </param>
		/// <param name="fArray">    If non-null, returns the samples in this array. </param>
		/// <param name="data">      The DataBuffer containing the image data. </param>
		/// <returns> the samples for the specified pixel. </returns>
		/// <seealso cref= #setPixel(int, int, float[], DataBuffer)
		/// </seealso>
		/// <exception cref="NullPointerException"> if data is null. </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates are
		/// not in bounds, or if fArray is too small to hold the output. </exception>
		public virtual float[] GetPixel(int x, int y, float[] fArray, DataBuffer data)
		{

			float[] pixels;

			if (fArray != null)
			{
				pixels = fArray;
			}
			else
			{
				pixels = new float[NumBands_Renamed];
			}

			for (int i = 0; i < NumBands_Renamed; i++)
			{
				pixels[i] = GetSampleFloat(x, y, i, data);
			}

			return pixels;
		}

		/// <summary>
		/// Returns the samples for the specified pixel in an array of double.
		/// ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		/// not in bounds. </summary>
		/// <param name="x">         The X coordinate of the pixel location. </param>
		/// <param name="y">         The Y coordinate of the pixel location. </param>
		/// <param name="dArray">    If non-null, returns the samples in this array. </param>
		/// <param name="data">      The DataBuffer containing the image data. </param>
		/// <returns> the samples for the specified pixel. </returns>
		/// <seealso cref= #setPixel(int, int, double[], DataBuffer)
		/// </seealso>
		/// <exception cref="NullPointerException"> if data is null. </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates are
		/// not in bounds, or if dArray is too small to hold the output. </exception>
		public virtual double[] GetPixel(int x, int y, double[] dArray, DataBuffer data)
		{

			double[] pixels;

			if (dArray != null)
			{
				pixels = dArray;
			}
			else
			{
				pixels = new double[NumBands_Renamed];
			}

			for (int i = 0; i < NumBands_Renamed; i++)
			{
				pixels[i] = GetSampleDouble(x, y, i, data);
			}

			return pixels;
		}

		/// <summary>
		/// Returns all samples for a rectangle of pixels in an
		/// int array, one sample per array element.
		/// ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		/// not in bounds. </summary>
		/// <param name="x">         The X coordinate of the upper left pixel location. </param>
		/// <param name="y">         The Y coordinate of the upper left pixel location. </param>
		/// <param name="w">         The width of the pixel rectangle. </param>
		/// <param name="h">         The height of the pixel rectangle. </param>
		/// <param name="iArray">    If non-null, returns the samples in this array. </param>
		/// <param name="data">      The DataBuffer containing the image data. </param>
		/// <returns> the samples for the specified region of pixels. </returns>
		/// <seealso cref= #setPixels(int, int, int, int, int[], DataBuffer)
		/// </seealso>
		/// <exception cref="NullPointerException"> if data is null. </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates are
		/// not in bounds, or if iArray is too small to hold the output. </exception>
		public virtual int[] GetPixels(int x, int y, int w, int h, int[] iArray, DataBuffer data)
		{

			int[] pixels;
			int Offset = 0;
			int x1 = x + w;
			int y1 = y + h;

			if (x < 0 || x >= Width_Renamed || w > Width_Renamed || x1 < 0 || x1 > Width_Renamed || y < 0 || y >= Height_Renamed || h > Height_Renamed || y1 < 0 || y1 > Height_Renamed)
			{
				throw new ArrayIndexOutOfBoundsException("Invalid coordinates.");
			}

			if (iArray != null)
			{
				pixels = iArray;
			}
			else
			{
				pixels = new int[NumBands_Renamed * w * h];
			}

			for (int i = y; i < y1; i++)
			{
				for (int j = x; j < x1; j++)
				{
					for (int k = 0; k < NumBands_Renamed; k++)
					{
						pixels[Offset++] = GetSample(j, i, k, data);
					}
				}
			}

			return pixels;
		}

		/// <summary>
		/// Returns all samples for a rectangle of pixels in a float
		/// array, one sample per array element.
		/// ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		/// not in bounds. </summary>
		/// <param name="x">         The X coordinate of the upper left pixel location. </param>
		/// <param name="y">         The Y coordinate of the upper left pixel location. </param>
		/// <param name="w">         The width of the pixel rectangle. </param>
		/// <param name="h">         The height of the pixel rectangle. </param>
		/// <param name="fArray">    If non-null, returns the samples in this array. </param>
		/// <param name="data">      The DataBuffer containing the image data. </param>
		/// <returns> the samples for the specified region of pixels. </returns>
		/// <seealso cref= #setPixels(int, int, int, int, float[], DataBuffer)
		/// </seealso>
		/// <exception cref="NullPointerException"> if data is null. </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates are
		/// not in bounds, or if fArray is too small to hold the output. </exception>
		public virtual float[] GetPixels(int x, int y, int w, int h, float[] fArray, DataBuffer data)
		{

			float[] pixels;
			int Offset = 0;
			int x1 = x + w;
			int y1 = y + h;

			if (x < 0 || x >= Width_Renamed || w > Width_Renamed || x1 < 0 || x1 > Width_Renamed || y < 0 || y >= Height_Renamed || h > Height_Renamed || y1 < 0 || y1 > Height_Renamed)
			{
				throw new ArrayIndexOutOfBoundsException("Invalid coordinates.");
			}

			if (fArray != null)
			{
				pixels = fArray;
			}
			else
			{
				pixels = new float[NumBands_Renamed * w * h];
			}

			for (int i = y; i < y1; i++)
			{
				for (int j = x; j < x1; j++)
				{
					for (int k = 0; k < NumBands_Renamed; k++)
					{
						pixels[Offset++] = GetSampleFloat(j, i, k, data);
					}
				}
			}

			return pixels;
		}

		/// <summary>
		/// Returns all samples for a rectangle of pixels in a double
		/// array, one sample per array element.
		/// ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		/// not in bounds. </summary>
		/// <param name="x">         The X coordinate of the upper left pixel location. </param>
		/// <param name="y">         The Y coordinate of the upper left pixel location. </param>
		/// <param name="w">         The width of the pixel rectangle. </param>
		/// <param name="h">         The height of the pixel rectangle. </param>
		/// <param name="dArray">    If non-null, returns the samples in this array. </param>
		/// <param name="data">      The DataBuffer containing the image data. </param>
		/// <returns> the samples for the specified region of pixels. </returns>
		/// <seealso cref= #setPixels(int, int, int, int, double[], DataBuffer)
		/// </seealso>
		/// <exception cref="NullPointerException"> if data is null. </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates are
		/// not in bounds, or if dArray is too small to hold the output. </exception>
		public virtual double[] GetPixels(int x, int y, int w, int h, double[] dArray, DataBuffer data)
		{
			double[] pixels;
			int Offset = 0;
			int x1 = x + w;
			int y1 = y + h;

			if (x < 0 || x >= Width_Renamed || w > Width_Renamed || x1 < 0 || x1 > Width_Renamed || y < 0 || y >= Height_Renamed || h > Height_Renamed || y1 < 0 || y1 > Height_Renamed)
			{
				throw new ArrayIndexOutOfBoundsException("Invalid coordinates.");
			}

			if (dArray != null)
			{
				pixels = dArray;
			}
			else
			{
				pixels = new double[NumBands_Renamed * w * h];
			}

			// Fix 4217412
			for (int i = y; i < y1; i++)
			{
				for (int j = x; j < x1; j++)
				{
					for (int k = 0; k < NumBands_Renamed; k++)
					{
						pixels[Offset++] = GetSampleDouble(j, i, k, data);
					}
				}
			}

			return pixels;
		}


		/// <summary>
		/// Returns the sample in a specified band for the pixel located
		/// at (x,y) as an int.
		/// ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		/// not in bounds. </summary>
		/// <param name="x">         The X coordinate of the pixel location. </param>
		/// <param name="y">         The Y coordinate of the pixel location. </param>
		/// <param name="b">         The band to return. </param>
		/// <param name="data">      The DataBuffer containing the image data. </param>
		/// <returns> the sample in a specified band for the specified pixel. </returns>
		/// <seealso cref= #setSample(int, int, int, int, DataBuffer)
		/// </seealso>
		/// <exception cref="NullPointerException"> if data is null. </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates or
		/// the band index are not in bounds. </exception>
		public abstract int GetSample(int x, int y, int b, DataBuffer data);


		/// <summary>
		/// Returns the sample in a specified band
		/// for the pixel located at (x,y) as a float.
		/// ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		/// not in bounds. </summary>
		/// <param name="x">         The X coordinate of the pixel location. </param>
		/// <param name="y">         The Y coordinate of the pixel location. </param>
		/// <param name="b">         The band to return. </param>
		/// <param name="data">      The DataBuffer containing the image data. </param>
		/// <returns> the sample in a specified band for the specified pixel.
		/// </returns>
		/// <exception cref="NullPointerException"> if data is null. </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates or
		/// the band index are not in bounds. </exception>
		public virtual float GetSampleFloat(int x, int y, int b, DataBuffer data)
		{

			float sample;
			sample = (float) GetSample(x, y, b, data);
			return sample;
		}

		/// <summary>
		/// Returns the sample in a specified band
		/// for a pixel located at (x,y) as a double.
		/// ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		/// not in bounds. </summary>
		/// <param name="x">         The X coordinate of the pixel location. </param>
		/// <param name="y">         The Y coordinate of the pixel location. </param>
		/// <param name="b">         The band to return. </param>
		/// <param name="data">      The DataBuffer containing the image data. </param>
		/// <returns> the sample in a specified band for the specified pixel.
		/// </returns>
		/// <exception cref="NullPointerException"> if data is null. </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates or
		/// the band index are not in bounds. </exception>
		public virtual double GetSampleDouble(int x, int y, int b, DataBuffer data)
		{

			double sample;

			sample = (double) GetSample(x, y, b, data);
			return sample;
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
		/// <returns> the samples for the specified band for the specified region
		///         of pixels. </returns>
		/// <seealso cref= #setSamples(int, int, int, int, int, int[], DataBuffer)
		/// </seealso>
		/// <exception cref="NullPointerException"> if data is null. </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates or
		/// the band index are not in bounds, or if iArray is too small to
		/// hold the output. </exception>
		public virtual int[] GetSamples(int x, int y, int w, int h, int b, int[] iArray, DataBuffer data)
		{
			int[] pixels;
			int Offset = 0;
			int x1 = x + w;
			int y1 = y + h;

			if (x < 0 || x1 < x || x1 > Width_Renamed || y < 0 || y1 < y || y1 > Height_Renamed)
			{
				throw new ArrayIndexOutOfBoundsException("Invalid coordinates.");
			}

			if (iArray != null)
			{
				pixels = iArray;
			}
			else
			{
				pixels = new int[w * h];
			}

			for (int i = y; i < y1; i++)
			{
				for (int j = x; j < x1; j++)
				{
					pixels[Offset++] = GetSample(j, i, b, data);
				}
			}

			return pixels;
		}

		/// <summary>
		/// Returns the samples for a specified band for the specified rectangle
		/// of pixels in a float array, one sample per array element.
		/// ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		/// not in bounds. </summary>
		/// <param name="x">         The X coordinate of the upper left pixel location. </param>
		/// <param name="y">         The Y coordinate of the upper left pixel location. </param>
		/// <param name="w">         The width of the pixel rectangle. </param>
		/// <param name="h">         The height of the pixel rectangle. </param>
		/// <param name="b">         The band to return. </param>
		/// <param name="fArray">    If non-null, returns the samples in this array. </param>
		/// <param name="data">      The DataBuffer containing the image data. </param>
		/// <returns> the samples for the specified band for the specified region
		///         of pixels. </returns>
		/// <seealso cref= #setSamples(int, int, int, int, int, float[], DataBuffer)
		/// </seealso>
		/// <exception cref="NullPointerException"> if data is null. </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates or
		/// the band index are not in bounds, or if fArray is too small to
		/// hold the output. </exception>
		public virtual float[] GetSamples(int x, int y, int w, int h, int b, float[] fArray, DataBuffer data)
		{
			float[] pixels;
			int Offset = 0;
			int x1 = x + w;
			int y1 = y + h;

			if (x < 0 || x1 < x || x1 > Width_Renamed || y < 0 || y1 < y || y1 > Height_Renamed)
			{
				throw new ArrayIndexOutOfBoundsException("Invalid coordinates");
			}

			if (fArray != null)
			{
				pixels = fArray;
			}
			else
			{
				pixels = new float[w * h];
			}

			for (int i = y; i < y1; i++)
			{
				for (int j = x; j < x1; j++)
				{
					pixels[Offset++] = GetSampleFloat(j, i, b, data);
				}
			}

			return pixels;
		}

		/// <summary>
		/// Returns the samples for a specified band for a specified rectangle
		/// of pixels in a double array, one sample per array element.
		/// ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		/// not in bounds. </summary>
		/// <param name="x">         The X coordinate of the upper left pixel location. </param>
		/// <param name="y">         The Y coordinate of the upper left pixel location. </param>
		/// <param name="w">         The width of the pixel rectangle. </param>
		/// <param name="h">         The height of the pixel rectangle. </param>
		/// <param name="b">         The band to return. </param>
		/// <param name="dArray">    If non-null, returns the samples in this array. </param>
		/// <param name="data">      The DataBuffer containing the image data. </param>
		/// <returns> the samples for the specified band for the specified region
		///         of pixels. </returns>
		/// <seealso cref= #setSamples(int, int, int, int, int, double[], DataBuffer)
		/// </seealso>
		/// <exception cref="NullPointerException"> if data is null. </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates or
		/// the band index are not in bounds, or if dArray is too small to
		/// hold the output. </exception>
		public virtual double[] GetSamples(int x, int y, int w, int h, int b, double[] dArray, DataBuffer data)
		{
			double[] pixels;
			int Offset = 0;
			int x1 = x + w;
			int y1 = y + h;

			if (x < 0 || x1 < x || x1 > Width_Renamed || y < 0 || y1 < y || y1 > Height_Renamed)
			{
				throw new ArrayIndexOutOfBoundsException("Invalid coordinates");
			}

			if (dArray != null)
			{
				pixels = dArray;
			}
			else
			{
				pixels = new double[w * h];
			}

			for (int i = y; i < y1; i++)
			{
				for (int j = x; j < x1; j++)
				{
					pixels[Offset++] = GetSampleDouble(j, i, b, data);
				}
			}

			return pixels;
		}

		/// <summary>
		/// Sets a pixel in  the DataBuffer using an int array of samples for input.
		/// ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		/// not in bounds. </summary>
		/// <param name="x">         The X coordinate of the pixel location. </param>
		/// <param name="y">         The Y coordinate of the pixel location. </param>
		/// <param name="iArray">    The input samples in an int array. </param>
		/// <param name="data">      The DataBuffer containing the image data. </param>
		/// <seealso cref= #getPixel(int, int, int[], DataBuffer)
		/// </seealso>
		/// <exception cref="NullPointerException"> if iArray or data is null. </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates are
		/// not in bounds, or if iArray is too small to hold the input. </exception>
		public virtual void SetPixel(int x, int y, int[] iArray, DataBuffer data)
		{

			for (int i = 0; i < NumBands_Renamed; i++)
			{
				SetSample(x, y, i, iArray[i], data);
			}
		}

		/// <summary>
		/// Sets a pixel in the DataBuffer using a float array of samples for input.
		/// ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		/// not in bounds. </summary>
		/// <param name="x">         The X coordinate of the pixel location. </param>
		/// <param name="y">         The Y coordinate of the pixel location. </param>
		/// <param name="fArray">    The input samples in a float array. </param>
		/// <param name="data">      The DataBuffer containing the image data. </param>
		/// <seealso cref= #getPixel(int, int, float[], DataBuffer)
		/// </seealso>
		/// <exception cref="NullPointerException"> if fArray or data is null. </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates are
		/// not in bounds, or if fArray is too small to hold the input. </exception>
		public virtual void SetPixel(int x, int y, float[] fArray, DataBuffer data)
		{

			for (int i = 0; i < NumBands_Renamed; i++)
			{
				SetSample(x, y, i, fArray[i], data);
			}
		}

		/// <summary>
		/// Sets a pixel in the DataBuffer using a double array of samples
		/// for input. </summary>
		/// <param name="x">         The X coordinate of the pixel location. </param>
		/// <param name="y">         The Y coordinate of the pixel location. </param>
		/// <param name="dArray">    The input samples in a double array. </param>
		/// <param name="data">      The DataBuffer containing the image data. </param>
		/// <seealso cref= #getPixel(int, int, double[], DataBuffer)
		/// </seealso>
		/// <exception cref="NullPointerException"> if dArray or data is null. </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates are
		/// not in bounds, or if fArray is too small to hold the input. </exception>
		public virtual void SetPixel(int x, int y, double[] dArray, DataBuffer data)
		{

			for (int i = 0; i < NumBands_Renamed; i++)
			{
				SetSample(x, y, i, dArray[i], data);
			}
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
		/// <seealso cref= #getPixels(int, int, int, int, int[], DataBuffer)
		/// </seealso>
		/// <exception cref="NullPointerException"> if iArray or data is null. </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates are
		/// not in bounds, or if iArray is too small to hold the input. </exception>
		public virtual void SetPixels(int x, int y, int w, int h, int[] iArray, DataBuffer data)
		{
			int Offset = 0;
			int x1 = x + w;
			int y1 = y + h;

			if (x < 0 || x >= Width_Renamed || w > Width_Renamed || x1 < 0 || x1 > Width_Renamed || y < 0 || y >= Height_Renamed || h > Height_Renamed || y1 < 0 || y1 > Height_Renamed)
			{
				throw new ArrayIndexOutOfBoundsException("Invalid coordinates.");
			}

			for (int i = y; i < y1; i++)
			{
				for (int j = x; j < x1; j++)
				{
					for (int k = 0; k < NumBands_Renamed; k++)
					{
						SetSample(j, i, k, iArray[Offset++], data);
					}
				}
			}
		}

		/// <summary>
		/// Sets all samples for a rectangle of pixels from a float array containing
		/// one sample per array element.
		/// ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		/// not in bounds. </summary>
		/// <param name="x">         The X coordinate of the upper left pixel location. </param>
		/// <param name="y">         The Y coordinate of the upper left pixel location. </param>
		/// <param name="w">         The width of the pixel rectangle. </param>
		/// <param name="h">         The height of the pixel rectangle. </param>
		/// <param name="fArray">    The input samples in a float array. </param>
		/// <param name="data">      The DataBuffer containing the image data. </param>
		/// <seealso cref= #getPixels(int, int, int, int, float[], DataBuffer)
		/// </seealso>
		/// <exception cref="NullPointerException"> if fArray or data is null. </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates are
		/// not in bounds, or if fArray is too small to hold the input. </exception>
		public virtual void SetPixels(int x, int y, int w, int h, float[] fArray, DataBuffer data)
		{
			int Offset = 0;
			int x1 = x + w;
			int y1 = y + h;

			if (x < 0 || x >= Width_Renamed || w > Width_Renamed || x1 < 0 || x1 > Width_Renamed || y < 0 || y >= Height_Renamed || h > Height_Renamed || y1 < 0 || y1 > Height_Renamed)
			{
				throw new ArrayIndexOutOfBoundsException("Invalid coordinates.");
			}

			for (int i = y; i < y1; i++)
			{
				for (int j = x; j < x1; j++)
				{
					for (int k = 0; k < NumBands_Renamed; k++)
					{
						SetSample(j, i, k, fArray[Offset++], data);
					}
				}
			}
		}

		/// <summary>
		/// Sets all samples for a rectangle of pixels from a double array
		/// containing one sample per array element.
		/// ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		/// not in bounds. </summary>
		/// <param name="x">         The X coordinate of the upper left pixel location. </param>
		/// <param name="y">         The Y coordinate of the upper left pixel location. </param>
		/// <param name="w">         The width of the pixel rectangle. </param>
		/// <param name="h">         The height of the pixel rectangle. </param>
		/// <param name="dArray">    The input samples in a double array. </param>
		/// <param name="data">      The DataBuffer containing the image data. </param>
		/// <seealso cref= #getPixels(int, int, int, int, double[], DataBuffer)
		/// </seealso>
		/// <exception cref="NullPointerException"> if dArray or data is null. </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates are
		/// not in bounds, or if dArray is too small to hold the input. </exception>
		public virtual void SetPixels(int x, int y, int w, int h, double[] dArray, DataBuffer data)
		{
			int Offset = 0;
			int x1 = x + w;
			int y1 = y + h;

			if (x < 0 || x >= Width_Renamed || w > Width_Renamed || x1 < 0 || x1 > Width_Renamed || y < 0 || y >= Height_Renamed || h > Height_Renamed || y1 < 0 || y1 > Height_Renamed)
			{
				throw new ArrayIndexOutOfBoundsException("Invalid coordinates.");
			}

			for (int i = y; i < y1; i++)
			{
				for (int j = x; j < x1; j++)
				{
					for (int k = 0; k < NumBands_Renamed; k++)
					{
						SetSample(j, i, k, dArray[Offset++], data);
					}
				}
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
		/// <seealso cref= #getSample(int, int, int,  DataBuffer)
		/// </seealso>
		/// <exception cref="NullPointerException"> if data is null. </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates or
		/// the band index are not in bounds. </exception>
		public abstract void SetSample(int x, int y, int b, int s, DataBuffer data);

		/// <summary>
		/// Sets a sample in the specified band for the pixel located at (x,y)
		/// in the DataBuffer using a float for input.
		/// The default implementation of this method casts the input
		/// float sample to an int and then calls the
		/// <code>setSample(int, int, int, DataBuffer)</code> method using
		/// that int value.
		/// ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		/// not in bounds. </summary>
		/// <param name="x">         The X coordinate of the pixel location. </param>
		/// <param name="y">         The Y coordinate of the pixel location. </param>
		/// <param name="b">         The band to set. </param>
		/// <param name="s">         The input sample as a float. </param>
		/// <param name="data">      The DataBuffer containing the image data. </param>
		/// <seealso cref= #getSample(int, int, int, DataBuffer)
		/// </seealso>
		/// <exception cref="NullPointerException"> if data is null. </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates or
		/// the band index are not in bounds. </exception>
		public virtual void SetSample(int x, int y, int b, float s, DataBuffer data)
		{
			int sample = (int)s;

			SetSample(x, y, b, sample, data);
		}

		/// <summary>
		/// Sets a sample in the specified band for the pixel located at (x,y)
		/// in the DataBuffer using a double for input.
		/// The default implementation of this method casts the input
		/// double sample to an int and then calls the
		/// <code>setSample(int, int, int, DataBuffer)</code> method using
		/// that int value.
		/// ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		/// not in bounds. </summary>
		/// <param name="x">         The X coordinate of the pixel location. </param>
		/// <param name="y">         The Y coordinate of the pixel location. </param>
		/// <param name="b">         The band to set. </param>
		/// <param name="s">         The input sample as a double. </param>
		/// <param name="data">      The DataBuffer containing the image data. </param>
		/// <seealso cref= #getSample(int, int, int, DataBuffer)
		/// </seealso>
		/// <exception cref="NullPointerException"> if data is null. </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates or
		/// the band index are not in bounds. </exception>
		public virtual void SetSample(int x, int y, int b, double s, DataBuffer data)
		{
			int sample = (int)s;

			SetSample(x, y, b, sample, data);
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
		/// <seealso cref= #getSamples(int, int, int, int, int, int[], DataBuffer)
		/// </seealso>
		/// <exception cref="NullPointerException"> if iArray or data is null. </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates or
		/// the band index are not in bounds, or if iArray is too small to
		/// hold the input. </exception>
		public virtual void SetSamples(int x, int y, int w, int h, int b, int[] iArray, DataBuffer data)
		{

			int Offset = 0;
			int x1 = x + w;
			int y1 = y + h;
			if (x < 0 || x >= Width_Renamed || w > Width_Renamed || x1 < 0 || x1 > Width_Renamed || y < 0 || y >= Height_Renamed || h > Height_Renamed || y1 < 0 || y1 > Height_Renamed)
			{
				throw new ArrayIndexOutOfBoundsException("Invalid coordinates.");
			}

			for (int i = y; i < y1; i++)
			{
				for (int j = x; j < x1; j++)
				{
					SetSample(j, i, b, iArray[Offset++], data);
				}
			}
		}

		/// <summary>
		/// Sets the samples in the specified band for the specified rectangle
		/// of pixels from a float array containing one sample per array element.
		/// ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		/// not in bounds. </summary>
		/// <param name="x">         The X coordinate of the upper left pixel location. </param>
		/// <param name="y">         The Y coordinate of the upper left pixel location. </param>
		/// <param name="w">         The width of the pixel rectangle. </param>
		/// <param name="h">         The height of the pixel rectangle. </param>
		/// <param name="b">         The band to set. </param>
		/// <param name="fArray">    The input samples in a float array. </param>
		/// <param name="data">      The DataBuffer containing the image data. </param>
		/// <seealso cref= #getSamples(int, int, int, int, int, float[], DataBuffer)
		/// </seealso>
		/// <exception cref="NullPointerException"> if fArray or data is null. </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates or
		/// the band index are not in bounds, or if fArray is too small to
		/// hold the input. </exception>
		public virtual void SetSamples(int x, int y, int w, int h, int b, float[] fArray, DataBuffer data)
		{
			int Offset = 0;
			int x1 = x + w;
			int y1 = y + h;

			if (x < 0 || x >= Width_Renamed || w > Width_Renamed || x1 < 0 || x1 > Width_Renamed || y < 0 || y >= Height_Renamed || h > Height_Renamed || y1 < 0 || y1 > Height_Renamed)
			{
				throw new ArrayIndexOutOfBoundsException("Invalid coordinates.");
			}

			for (int i = y; i < y1; i++)
			{
				for (int j = x; j < x1; j++)
				{
					SetSample(j, i, b, fArray[Offset++], data);
				}
			}
		}

		/// <summary>
		/// Sets the samples in the specified band for the specified rectangle
		/// of pixels from a double array containing one sample per array element.
		/// ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		/// not in bounds. </summary>
		/// <param name="x">         The X coordinate of the upper left pixel location. </param>
		/// <param name="y">         The Y coordinate of the upper left pixel location. </param>
		/// <param name="w">         The width of the pixel rectangle. </param>
		/// <param name="h">         The height of the pixel rectangle. </param>
		/// <param name="b">         The band to set. </param>
		/// <param name="dArray">    The input samples in a double array. </param>
		/// <param name="data">      The DataBuffer containing the image data. </param>
		/// <seealso cref= #getSamples(int, int, int, int, int, double[], DataBuffer)
		/// </seealso>
		/// <exception cref="NullPointerException"> if dArray or data is null. </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates or
		/// the band index are not in bounds, or if dArray is too small to
		/// hold the input. </exception>
		public virtual void SetSamples(int x, int y, int w, int h, int b, double[] dArray, DataBuffer data)
		{
			int Offset = 0;
			int x1 = x + w;
			int y1 = y + h;


			if (x < 0 || x >= Width_Renamed || w > Width_Renamed || x1 < 0 || x1 > Width_Renamed || y < 0 || y >= Height_Renamed || h > Height_Renamed || y1 < 0 || y1 > Height_Renamed)
			{
				throw new ArrayIndexOutOfBoundsException("Invalid coordinates.");
			}

			for (int i = y; i < y1; i++)
			{
				for (int j = x; j < x1; j++)
				{
					SetSample(j, i, b, dArray[Offset++], data);
				}
			}
		}

		/// <summary>
		///  Creates a SampleModel which describes data in this SampleModel's
		///  format, but with a different width and height. </summary>
		///  <param name="w"> the width of the image data </param>
		///  <param name="h"> the height of the image data </param>
		///  <returns> a <code>SampleModel</code> describing the same image
		///          data as this <code>SampleModel</code>, but with a
		///          different size. </returns>
		public abstract SampleModel CreateCompatibleSampleModel(int w, int h);

		/// <summary>
		/// Creates a new SampleModel
		/// with a subset of the bands of this
		/// SampleModel. </summary>
		/// <param name="bands"> the subset of bands of this <code>SampleModel</code> </param>
		/// <returns> a <code>SampleModel</code> with a subset of bands of this
		///         <code>SampleModel</code>. </returns>
		public abstract SampleModel CreateSubsetSampleModel(int[] bands);

		/// <summary>
		/// Creates a DataBuffer that corresponds to this SampleModel.
		/// The DataBuffer's width and height will match this SampleModel's. </summary>
		/// <returns> a <code>DataBuffer</code> corresponding to this
		///         <code>SampleModel</code>. </returns>
		public abstract DataBuffer CreateDataBuffer();

		/// <summary>
		/// Returns the size in bits of samples for all bands. </summary>
		///  <returns> the size of samples for all bands. </returns>
		public abstract int[] SampleSize {get;}

		/// <summary>
		/// Returns the size in bits of samples for the specified band. </summary>
		///  <param name="band"> the specified band </param>
		///  <returns> the size of the samples of the specified band. </returns>
		public abstract int GetSampleSize(int band);

	}

}