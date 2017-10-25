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

	using State = sun.java2d.StateTrackable.State;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to .NET:
	import static sun.java2d.StateTrackable.State.*;
	using StateTrackableDelegate = sun.java2d.StateTrackableDelegate;

	using SunWritableRaster = sun.awt.image.SunWritableRaster;

	/// <summary>
	/// This class exists to wrap one or more data arrays.  Each data array in
	/// the DataBuffer is referred to as a bank.  Accessor methods for getting
	/// and setting elements of the DataBuffer's banks exist with and without
	/// a bank specifier.  The methods without a bank specifier use the default 0th
	/// bank.  The DataBuffer can optionally take an offset per bank, so that
	/// data in an existing array can be used even if the interesting data
	/// doesn't start at array location zero.  Getting or setting the 0th
	/// element of a bank, uses the (0+offset)th element of the array.  The
	/// size field specifies how much of the data array is available for
	/// use.  Size + offset for a given bank should never be greater
	/// than the length of the associated data array.  The data type of
	/// a data buffer indicates the type of the data array(s) and may also
	/// indicate additional semantics, e.g. storing unsigned 8-bit data
	/// in elements of a byte array.  The data type may be TYPE_UNDEFINED
	/// or one of the types defined below.  Other types may be added in
	/// the future.  Generally, an object of class DataBuffer will be cast down
	/// to one of its data type specific subclasses to access data type specific
	/// methods for improved performance.  Currently, the Java 2D(tm) API
	/// image classes use TYPE_BYTE, TYPE_USHORT, TYPE_INT, TYPE_SHORT,
	/// TYPE_FLOAT, and TYPE_DOUBLE DataBuffers to store image data. </summary>
	/// <seealso cref= java.awt.image.Raster </seealso>
	/// <seealso cref= java.awt.image.SampleModel </seealso>
	public abstract class DataBuffer
	{

		/// <summary>
		/// Tag for unsigned byte data. </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public static final int TYPE_BYTE = 0;
		public const int TYPE_BYTE = 0;

		/// <summary>
		/// Tag for unsigned short data. </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public static final int TYPE_USHORT = 1;
		public const int TYPE_USHORT = 1;

		/// <summary>
		/// Tag for signed short data.  Placeholder for future use. </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public static final int TYPE_SHORT = 2;
		public const int TYPE_SHORT = 2;

		/// <summary>
		/// Tag for int data. </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public static final int TYPE_INT = 3;
		public const int TYPE_INT = 3;

		/// <summary>
		/// Tag for float data.  Placeholder for future use. </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public static final int TYPE_FLOAT = 4;
		public const int TYPE_FLOAT = 4;

		/// <summary>
		/// Tag for double data.  Placeholder for future use. </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public static final int TYPE_DOUBLE = 5;
		public const int TYPE_DOUBLE = 5;

		/// <summary>
		/// Tag for undefined data. </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public static final int TYPE_UNDEFINED = 32;
		public const int TYPE_UNDEFINED = 32;

		/// <summary>
		/// The data type of this DataBuffer. </summary>
		protected internal int DataType_Renamed;

		/// <summary>
		/// The number of banks in this DataBuffer. </summary>
		protected internal int Banks;

		/// <summary>
		/// Offset into default (first) bank from which to get the first element. </summary>
		protected internal int Offset_Renamed;

		/// <summary>
		/// Usable size of all banks. </summary>
		protected internal int Size_Renamed;

		/// <summary>
		/// Offsets into all banks. </summary>
		protected internal int[] Offsets_Renamed;

		/* The current StateTrackable state. */
		internal StateTrackableDelegate TheTrackable;

		/// <summary>
		/// Size of the data types indexed by DataType tags defined above. </summary>
		private static readonly int[] DataTypeSize = new int[] {8,16,16,32,32,64};

		/// <summary>
		/// Returns the size (in bits) of the data type, given a datatype tag. </summary>
		/// <param name="type"> the value of one of the defined datatype tags </param>
		/// <returns> the size of the data type </returns>
		/// <exception cref="IllegalArgumentException"> if <code>type</code> is less than
		///         zero or greater than <seealso cref="#TYPE_DOUBLE"/> </exception>
		public static int GetDataTypeSize(int type)
		{
			if (type < TYPE_BYTE || type > TYPE_DOUBLE)
			{
				throw new IllegalArgumentException("Unknown data type " + type);
			}
			return DataTypeSize[type];
		}

		/// <summary>
		///  Constructs a DataBuffer containing one bank of the specified
		///  data type and size.
		/// </summary>
		///  <param name="dataType"> the data type of this <code>DataBuffer</code> </param>
		///  <param name="size"> the size of the banks </param>
		protected internal DataBuffer(int dataType, int size) : this(UNTRACKABLE, dataType, size)
		{
		}

		/// <summary>
		///  Constructs a DataBuffer containing one bank of the specified
		///  data type and size with the indicated initial <seealso cref="State State"/>.
		/// </summary>
		///  <param name="initialState"> the initial <seealso cref="State State"/> state of the data </param>
		///  <param name="dataType"> the data type of this <code>DataBuffer</code> </param>
		///  <param name="size"> the size of the banks
		///  @since 1.7 </param>
		internal DataBuffer(State initialState, int dataType, int size)
		{
			this.TheTrackable = StateTrackableDelegate.createInstance(initialState);
			this.DataType_Renamed = dataType;
			this.Banks = 1;
			this.Size_Renamed = size;
			this.Offset_Renamed = 0;
			this.Offsets_Renamed = new int[1]; // init to 0 by new
		}

		/// <summary>
		///  Constructs a DataBuffer containing the specified number of
		///  banks.  Each bank has the specified size and an offset of 0.
		/// </summary>
		///  <param name="dataType"> the data type of this <code>DataBuffer</code> </param>
		///  <param name="size"> the size of the banks </param>
		///  <param name="numBanks"> the number of banks in this
		///         <code>DataBuffer</code> </param>
		protected internal DataBuffer(int dataType, int size, int numBanks) : this(UNTRACKABLE, dataType, size, numBanks)
		{
		}

		/// <summary>
		///  Constructs a DataBuffer containing the specified number of
		///  banks with the indicated initial <seealso cref="State State"/>.
		///  Each bank has the specified size and an offset of 0.
		/// </summary>
		///  <param name="initialState"> the initial <seealso cref="State State"/> state of the data </param>
		///  <param name="dataType"> the data type of this <code>DataBuffer</code> </param>
		///  <param name="size"> the size of the banks </param>
		///  <param name="numBanks"> the number of banks in this
		///         <code>DataBuffer</code>
		///  @since 1.7 </param>
		internal DataBuffer(State initialState, int dataType, int size, int numBanks)
		{
			this.TheTrackable = StateTrackableDelegate.createInstance(initialState);
			this.DataType_Renamed = dataType;
			this.Banks = numBanks;
			this.Size_Renamed = size;
			this.Offset_Renamed = 0;
			this.Offsets_Renamed = new int[Banks]; // init to 0 by new
		}

		/// <summary>
		///  Constructs a DataBuffer that contains the specified number
		///  of banks.  Each bank has the specified datatype, size and offset.
		/// </summary>
		///  <param name="dataType"> the data type of this <code>DataBuffer</code> </param>
		///  <param name="size"> the size of the banks </param>
		///  <param name="numBanks"> the number of banks in this
		///         <code>DataBuffer</code> </param>
		///  <param name="offset"> the offset for each bank </param>
		protected internal DataBuffer(int dataType, int size, int numBanks, int offset) : this(UNTRACKABLE, dataType, size, numBanks, offset)
		{
		}

		/// <summary>
		///  Constructs a DataBuffer that contains the specified number
		///  of banks with the indicated initial <seealso cref="State State"/>.
		///  Each bank has the specified datatype, size and offset.
		/// </summary>
		///  <param name="initialState"> the initial <seealso cref="State State"/> state of the data </param>
		///  <param name="dataType"> the data type of this <code>DataBuffer</code> </param>
		///  <param name="size"> the size of the banks </param>
		///  <param name="numBanks"> the number of banks in this
		///         <code>DataBuffer</code> </param>
		///  <param name="offset"> the offset for each bank
		///  @since 1.7 </param>
		internal DataBuffer(State initialState, int dataType, int size, int numBanks, int offset)
		{
			this.TheTrackable = StateTrackableDelegate.createInstance(initialState);
			this.DataType_Renamed = dataType;
			this.Banks = numBanks;
			this.Size_Renamed = size;
			this.Offset_Renamed = offset;
			this.Offsets_Renamed = new int[numBanks];
			for (int i = 0; i < numBanks; i++)
			{
				this.Offsets_Renamed[i] = offset;
			}
		}

		/// <summary>
		///  Constructs a DataBuffer which contains the specified number
		///  of banks.  Each bank has the specified datatype and size.  The
		///  offset for each bank is specified by its respective entry in
		///  the offsets array.
		/// </summary>
		///  <param name="dataType"> the data type of this <code>DataBuffer</code> </param>
		///  <param name="size"> the size of the banks </param>
		///  <param name="numBanks"> the number of banks in this
		///         <code>DataBuffer</code> </param>
		///  <param name="offsets"> an array containing an offset for each bank. </param>
		///  <exception cref="ArrayIndexOutOfBoundsException"> if <code>numBanks</code>
		///          does not equal the length of <code>offsets</code> </exception>
		protected internal DataBuffer(int dataType, int size, int numBanks, int[] offsets) : this(UNTRACKABLE, dataType, size, numBanks, offsets)
		{
		}

		/// <summary>
		///  Constructs a DataBuffer which contains the specified number
		///  of banks with the indicated initial <seealso cref="State State"/>.
		///  Each bank has the specified datatype and size.  The
		///  offset for each bank is specified by its respective entry in
		///  the offsets array.
		/// </summary>
		///  <param name="initialState"> the initial <seealso cref="State State"/> state of the data </param>
		///  <param name="dataType"> the data type of this <code>DataBuffer</code> </param>
		///  <param name="size"> the size of the banks </param>
		///  <param name="numBanks"> the number of banks in this
		///         <code>DataBuffer</code> </param>
		///  <param name="offsets"> an array containing an offset for each bank. </param>
		///  <exception cref="ArrayIndexOutOfBoundsException"> if <code>numBanks</code>
		///          does not equal the length of <code>offsets</code>
		///  @since 1.7 </exception>
		internal DataBuffer(State initialState, int dataType, int size, int numBanks, int[] offsets)
		{
			if (numBanks != offsets.Length)
			{
				throw new ArrayIndexOutOfBoundsException("Number of banks" + " does not match number of bank offsets");
			}
			this.TheTrackable = StateTrackableDelegate.createInstance(initialState);
			this.DataType_Renamed = dataType;
			this.Banks = numBanks;
			this.Size_Renamed = size;
			this.Offset_Renamed = offsets[0];
			this.Offsets_Renamed = (int[])offsets.clone();
		}

		/// <summary>
		///  Returns the data type of this DataBuffer. </summary>
		///   <returns> the data type of this <code>DataBuffer</code>. </returns>
		public virtual int DataType
		{
			get
			{
				return DataType_Renamed;
			}
		}

		/// <summary>
		///  Returns the size (in array elements) of all banks. </summary>
		///   <returns> the size of all banks. </returns>
		public virtual int Size
		{
			get
			{
				return Size_Renamed;
			}
		}

		/// <summary>
		/// Returns the offset of the default bank in array elements. </summary>
		///  <returns> the offset of the default bank. </returns>
		public virtual int Offset
		{
			get
			{
				return Offset_Renamed;
			}
		}

		/// <summary>
		/// Returns the offsets (in array elements) of all the banks. </summary>
		///  <returns> the offsets of all banks. </returns>
		public virtual int[] Offsets
		{
			get
			{
				return (int[])Offsets_Renamed.clone();
			}
		}

		/// <summary>
		/// Returns the number of banks in this DataBuffer. </summary>
		///  <returns> the number of banks. </returns>
		public virtual int NumBanks
		{
			get
			{
				return Banks;
			}
		}

		/// <summary>
		/// Returns the requested data array element from the first (default) bank
		/// as an integer. </summary>
		/// <param name="i"> the index of the requested data array element </param>
		/// <returns> the data array element at the specified index. </returns>
		/// <seealso cref= #setElem(int, int) </seealso>
		/// <seealso cref= #setElem(int, int, int) </seealso>
		public virtual int GetElem(int i)
		{
			return GetElem(0,i);
		}

		/// <summary>
		/// Returns the requested data array element from the specified bank
		/// as an integer. </summary>
		/// <param name="bank"> the specified bank </param>
		/// <param name="i"> the index of the requested data array element </param>
		/// <returns> the data array element at the specified index from the
		///         specified bank at the specified index. </returns>
		/// <seealso cref= #setElem(int, int) </seealso>
		/// <seealso cref= #setElem(int, int, int) </seealso>
		public abstract int GetElem(int bank, int i);

		/// <summary>
		/// Sets the requested data array element in the first (default) bank
		/// from the given integer. </summary>
		/// <param name="i"> the specified index into the data array </param>
		/// <param name="val"> the data to set the element at the specified index in
		/// the data array </param>
		/// <seealso cref= #getElem(int) </seealso>
		/// <seealso cref= #getElem(int, int) </seealso>
		public virtual void SetElem(int i, int val)
		{
			SetElem(0,i,val);
		}

		/// <summary>
		/// Sets the requested data array element in the specified bank
		/// from the given integer. </summary>
		/// <param name="bank"> the specified bank </param>
		/// <param name="i"> the specified index into the data array </param>
		/// <param name="val">  the data to set the element in the specified bank
		/// at the specified index in the data array </param>
		/// <seealso cref= #getElem(int) </seealso>
		/// <seealso cref= #getElem(int, int) </seealso>
		public abstract void SetElem(int bank, int i, int val);

		/// <summary>
		/// Returns the requested data array element from the first (default) bank
		/// as a float.  The implementation in this class is to cast getElem(i)
		/// to a float.  Subclasses may override this method if another
		/// implementation is needed. </summary>
		/// <param name="i"> the index of the requested data array element </param>
		/// <returns> a float value representing the data array element at the
		///  specified index. </returns>
		/// <seealso cref= #setElemFloat(int, float) </seealso>
		/// <seealso cref= #setElemFloat(int, int, float) </seealso>
		public virtual float GetElemFloat(int i)
		{
			return (float)GetElem(i);
		}

		/// <summary>
		/// Returns the requested data array element from the specified bank
		/// as a float.  The implementation in this class is to cast
		/// <seealso cref="#getElem(int, int)"/>
		/// to a float.  Subclasses can override this method if another
		/// implementation is needed. </summary>
		/// <param name="bank"> the specified bank </param>
		/// <param name="i"> the index of the requested data array element </param>
		/// <returns> a float value representing the data array element from the
		/// specified bank at the specified index. </returns>
		/// <seealso cref= #setElemFloat(int, float) </seealso>
		/// <seealso cref= #setElemFloat(int, int, float) </seealso>
		public virtual float GetElemFloat(int bank, int i)
		{
			return (float)GetElem(bank,i);
		}

		/// <summary>
		/// Sets the requested data array element in the first (default) bank
		/// from the given float.  The implementation in this class is to cast
		/// val to an int and call <seealso cref="#setElem(int, int)"/>.  Subclasses
		/// can override this method if another implementation is needed. </summary>
		/// <param name="i"> the specified index </param>
		/// <param name="val"> the value to set the element at the specified index in
		/// the data array </param>
		/// <seealso cref= #getElemFloat(int) </seealso>
		/// <seealso cref= #getElemFloat(int, int) </seealso>
		public virtual void SetElemFloat(int i, float val)
		{
			SetElem(i,(int)val);
		}

		/// <summary>
		/// Sets the requested data array element in the specified bank
		/// from the given float.  The implementation in this class is to cast
		/// val to an int and call <seealso cref="#setElem(int, int)"/>.  Subclasses can
		/// override this method if another implementation is needed. </summary>
		/// <param name="bank"> the specified bank </param>
		/// <param name="i"> the specified index </param>
		/// <param name="val"> the value to set the element in the specified bank at
		/// the specified index in the data array </param>
		/// <seealso cref= #getElemFloat(int) </seealso>
		/// <seealso cref= #getElemFloat(int, int) </seealso>
		public virtual void SetElemFloat(int bank, int i, float val)
		{
			SetElem(bank,i,(int)val);
		}

		/// <summary>
		/// Returns the requested data array element from the first (default) bank
		/// as a double.  The implementation in this class is to cast
		/// <seealso cref="#getElem(int)"/>
		/// to a double.  Subclasses can override this method if another
		/// implementation is needed. </summary>
		/// <param name="i"> the specified index </param>
		/// <returns> a double value representing the element at the specified
		/// index in the data array. </returns>
		/// <seealso cref= #setElemDouble(int, double) </seealso>
		/// <seealso cref= #setElemDouble(int, int, double) </seealso>
		public virtual double GetElemDouble(int i)
		{
			return (double)GetElem(i);
		}

		/// <summary>
		/// Returns the requested data array element from the specified bank as
		/// a double.  The implementation in this class is to cast getElem(bank, i)
		/// to a double.  Subclasses may override this method if another
		/// implementation is needed. </summary>
		/// <param name="bank"> the specified bank </param>
		/// <param name="i"> the specified index </param>
		/// <returns> a double value representing the element from the specified
		/// bank at the specified index in the data array. </returns>
		/// <seealso cref= #setElemDouble(int, double) </seealso>
		/// <seealso cref= #setElemDouble(int, int, double) </seealso>
		public virtual double GetElemDouble(int bank, int i)
		{
			return (double)GetElem(bank,i);
		}

		/// <summary>
		/// Sets the requested data array element in the first (default) bank
		/// from the given double.  The implementation in this class is to cast
		/// val to an int and call <seealso cref="#setElem(int, int)"/>.  Subclasses can
		/// override this method if another implementation is needed. </summary>
		/// <param name="i"> the specified index </param>
		/// <param name="val"> the value to set the element at the specified index
		/// in the data array </param>
		/// <seealso cref= #getElemDouble(int) </seealso>
		/// <seealso cref= #getElemDouble(int, int) </seealso>
		public virtual void SetElemDouble(int i, double val)
		{
			SetElem(i,(int)val);
		}

		/// <summary>
		/// Sets the requested data array element in the specified bank
		/// from the given double.  The implementation in this class is to cast
		/// val to an int and call <seealso cref="#setElem(int, int)"/>.  Subclasses can
		/// override this method if another implementation is needed. </summary>
		/// <param name="bank"> the specified bank </param>
		/// <param name="i"> the specified index </param>
		/// <param name="val"> the value to set the element in the specified bank
		/// at the specified index of the data array </param>
		/// <seealso cref= #getElemDouble(int) </seealso>
		/// <seealso cref= #getElemDouble(int, int) </seealso>
		public virtual void SetElemDouble(int bank, int i, double val)
		{
			SetElem(bank,i,(int)val);
		}

		internal static int[] ToIntArray(Object obj)
		{
			if (obj is int[])
			{
				return (int[])obj;
			}
			else if (obj == null)
			{
				return null;
			}
			else if (obj is short[])
			{
				short[] sdata = (short[])obj;
				int[] idata = new int[sdata.Length];
				for (int i = 0; i < sdata.Length; i++)
				{
					idata[i] = (int)sdata[i] & 0xffff;
				}
				return idata;
			}
			else if (obj is sbyte[])
			{
				sbyte[] bdata = (sbyte[])obj;
				int[] idata = new int[bdata.Length];
				for (int i = 0; i < bdata.Length; i++)
				{
					idata[i] = 0xff & (int)bdata[i];
				}
				return idata;
			}
			return null;
		}

		static DataBuffer()
		{
			SunWritableRaster.DataStealer = new DataStealerAnonymousInnerClassHelper();
		}

		private class DataStealerAnonymousInnerClassHelper : SunWritableRaster.DataStealer
		{
			public DataStealerAnonymousInnerClassHelper()
			{
			}

			public virtual sbyte[] GetData(DataBufferByte dbb, int bank)
			{
				return dbb.Bankdata[bank];
			}

			public virtual short[] GetData(DataBufferUShort dbus, int bank)
			{
				return dbus.Bankdata[bank];
			}

			public virtual int[] GetData(DataBufferInt dbi, int bank)
			{
				return dbi.Bankdata[bank];
			}

			public virtual StateTrackableDelegate GetTrackable(DataBuffer db)
			{
				return db.TheTrackable;
			}

			public virtual void SetTrackable(DataBuffer db, StateTrackableDelegate trackable)
			{
				db.TheTrackable = trackable;
			}
		}
	}

}