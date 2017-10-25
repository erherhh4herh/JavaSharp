/*
 * Copyright (c) 2000, 2007, Oracle and/or its affiliates. All rights reserved.
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

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to .NET:
	import static sun.java2d.StateTrackable.State.*;

	/// <summary>
	/// This class extends <code>DataBuffer</code> and stores data internally
	/// in <code>float</code> form.
	/// <para>
	/// <a name="optimizations">
	/// Note that some implementations may function more efficiently
	/// if they can maintain control over how the data for an image is
	/// stored.
	/// For example, optimizations such as caching an image in video
	/// memory require that the implementation track all modifications
	/// to that data.
	/// Other implementations may operate better if they can store the
	/// data in locations other than a Java array.
	/// To maintain optimum compatibility with various optimizations
	/// it is best to avoid constructors and methods which expose the
	/// underlying storage as a Java array as noted below in the
	/// documentation for those methods.
	/// </a>
	/// 
	/// @since 1.4
	/// </para>
	/// </summary>

	public sealed class DataBufferFloat : DataBuffer
	{

		/// <summary>
		/// The array of data banks. </summary>
		internal float[][] Bankdata;

		/// <summary>
		/// A reference to the default data bank. </summary>
		internal float[] Data_Renamed;

		/// <summary>
		/// Constructs a <code>float</code>-based <code>DataBuffer</code>
		/// with a specified size.
		/// </summary>
		/// <param name="size"> The number of elements in the DataBuffer. </param>
		public DataBufferFloat(int size) : base(STABLE, TYPE_FLOAT, size)
		{
			Data_Renamed = new float[size];
			Bankdata = new float[1][];
			Bankdata[0] = Data_Renamed;
		}

		/// <summary>
		/// Constructs a <code>float</code>-based <code>DataBuffer</code>
		/// with a specified number of banks, all of which are of a
		/// specified size.
		/// </summary>
		/// <param name="size"> The number of elements in each bank of the
		/// <code>DataBuffer</code>. </param>
		/// <param name="numBanks"> The number of banks in the
		///        <code>DataBuffer</code>. </param>
		public DataBufferFloat(int size, int numBanks) : base(STABLE, TYPE_FLOAT, size, numBanks)
		{
			Bankdata = new float[numBanks][];
			for (int i = 0; i < numBanks; i++)
			{
				Bankdata[i] = new float[size];
			}
			Data_Renamed = Bankdata[0];
		}

		/// <summary>
		/// Constructs a <code>float</code>-based <code>DataBuffer</code>
		/// with the specified data array.  Only the first
		/// <code>size</code> elements are available for use by this
		/// <code>DataBuffer</code>.  The array must be large enough to
		/// hold <code>size</code> elements.
		/// <para>
		/// Note that {@code DataBuffer} objects created by this constructor
		/// may be incompatible with <a href="#optimizations">performance
		/// optimizations</a> used by some implementations (such as caching
		/// an associated image in video memory).
		/// 
		/// </para>
		/// </summary>
		/// <param name="dataArray"> An array of <code>float</code>s to be used as the
		///                  first and only bank of this <code>DataBuffer</code>. </param>
		/// <param name="size"> The number of elements of the array to be used. </param>
		public DataBufferFloat(float[] dataArray, int size) : base(UNTRACKABLE, TYPE_FLOAT, size)
		{
			Data_Renamed = dataArray;
			Bankdata = new float[1][];
			Bankdata[0] = Data_Renamed;
		}

		/// <summary>
		/// Constructs a <code>float</code>-based <code>DataBuffer</code>
		/// with the specified data array.  Only the elements between
		/// <code>offset</code> and <code>offset + size - 1</code> are
		/// available for use by this <code>DataBuffer</code>.  The array
		/// must be large enough to hold <code>offset + size</code>
		/// elements.
		/// <para>
		/// Note that {@code DataBuffer} objects created by this constructor
		/// may be incompatible with <a href="#optimizations">performance
		/// optimizations</a> used by some implementations (such as caching
		/// an associated image in video memory).
		/// 
		/// </para>
		/// </summary>
		/// <param name="dataArray"> An array of <code>float</code>s to be used as the
		///                  first and only bank of this <code>DataBuffer</code>. </param>
		/// <param name="size"> The number of elements of the array to be used. </param>
		/// <param name="offset"> The offset of the first element of the array
		///               that will be used. </param>
		public DataBufferFloat(float[] dataArray, int size, int offset) : base(UNTRACKABLE, TYPE_FLOAT, size, 1, offset)
		{
			Data_Renamed = dataArray;
			Bankdata = new float[1][];
			Bankdata[0] = Data_Renamed;
		}

		/// <summary>
		/// Constructs a <code>float</code>-based <code>DataBuffer</code>
		/// with the specified data arrays.  Only the first
		/// <code>size</code> elements of each array are available for use
		/// by this <code>DataBuffer</code>.  The number of banks will be
		/// equal to <code>dataArray.length</code>.
		/// <para>
		/// Note that {@code DataBuffer} objects created by this constructor
		/// may be incompatible with <a href="#optimizations">performance
		/// optimizations</a> used by some implementations (such as caching
		/// an associated image in video memory).
		/// 
		/// </para>
		/// </summary>
		/// <param name="dataArray"> An array of arrays of <code>float</code>s to be
		///                  used as the banks of this <code>DataBuffer</code>. </param>
		/// <param name="size"> The number of elements of each array to be used. </param>
		public DataBufferFloat(float[][] dataArray, int size) : base(UNTRACKABLE, TYPE_FLOAT, size, dataArray.Length)
		{
			Bankdata = (float[][]) dataArray.clone();
			Data_Renamed = Bankdata[0];
		}

		/// <summary>
		/// Constructs a <code>float</code>-based <code>DataBuffer</code>
		/// with the specified data arrays, size, and per-bank offsets.
		/// The number of banks is equal to <code>dataArray.length</code>.
		/// Each array must be at least as large as <code>size</code> plus the
		/// corresponding offset.  There must be an entry in the offsets
		/// array for each data array.
		/// <para>
		/// Note that {@code DataBuffer} objects created by this constructor
		/// may be incompatible with <a href="#optimizations">performance
		/// optimizations</a> used by some implementations (such as caching
		/// an associated image in video memory).
		/// 
		/// </para>
		/// </summary>
		/// <param name="dataArray"> An array of arrays of <code>float</code>s to be
		///                  used as the banks of this <code>DataBuffer</code>. </param>
		/// <param name="size"> The number of elements of each array to be used. </param>
		/// <param name="offsets"> An array of integer offsets, one for each bank. </param>
		public DataBufferFloat(float[][] dataArray, int size, int[] offsets) : base(UNTRACKABLE, TYPE_FLOAT, size,dataArray.Length, offsets)
		{
			Bankdata = (float[][]) dataArray.clone();
			Data_Renamed = Bankdata[0];
		}

		/// <summary>
		/// Returns the default (first) <code>float</code> data array.
		/// <para>
		/// Note that calling this method may cause this {@code DataBuffer}
		/// object to be incompatible with <a href="#optimizations">performance
		/// optimizations</a> used by some implementations (such as caching
		/// an associated image in video memory).
		/// 
		/// </para>
		/// </summary>
		/// <returns> the first float data array. </returns>
		public float[] Data
		{
			get
			{
				TheTrackable.setUntrackable();
				return Data_Renamed;
			}
		}

		/// <summary>
		/// Returns the data array for the specified bank.
		/// <para>
		/// Note that calling this method may cause this {@code DataBuffer}
		/// object to be incompatible with <a href="#optimizations">performance
		/// optimizations</a> used by some implementations (such as caching
		/// an associated image in video memory).
		/// 
		/// </para>
		/// </summary>
		/// <param name="bank"> the data array </param>
		/// <returns> the data array specified by <code>bank</code>. </returns>
		public float[] GetData(int bank)
		{
			TheTrackable.setUntrackable();
			return Bankdata[bank];
		}

		/// <summary>
		/// Returns the data array for all banks.
		/// <para>
		/// Note that calling this method may cause this {@code DataBuffer}
		/// object to be incompatible with <a href="#optimizations">performance
		/// optimizations</a> used by some implementations (such as caching
		/// an associated image in video memory).
		/// 
		/// </para>
		/// </summary>
		/// <returns> all data arrays for this data buffer. </returns>
		public float[][] BankData
		{
			get
			{
				TheTrackable.setUntrackable();
				return (float[][]) Bankdata.clone();
			}
		}

		/// <summary>
		/// Returns the requested data array element from the first
		/// (default) bank as an <code>int</code>.
		/// </summary>
		/// <param name="i"> The desired data array element.
		/// </param>
		/// <returns> The data entry as an <code>int</code>. </returns>
		/// <seealso cref= #setElem(int, int) </seealso>
		/// <seealso cref= #setElem(int, int, int) </seealso>
		public override int GetElem(int i)
		{
			return (int)(Data_Renamed[i + Offset_Renamed]);
		}

		/// <summary>
		/// Returns the requested data array element from the specified
		/// bank as an <code>int</code>.
		/// </summary>
		/// <param name="bank"> The bank number. </param>
		/// <param name="i"> The desired data array element.
		/// </param>
		/// <returns> The data entry as an <code>int</code>. </returns>
		/// <seealso cref= #setElem(int, int) </seealso>
		/// <seealso cref= #setElem(int, int, int) </seealso>
		public override int GetElem(int bank, int i)
		{
			return (int)(Bankdata[bank][i + Offsets_Renamed[bank]]);
		}

		/// <summary>
		/// Sets the requested data array element in the first (default)
		/// bank to the given <code>int</code>.
		/// </summary>
		/// <param name="i"> The desired data array element. </param>
		/// <param name="val"> The value to be set. </param>
		/// <seealso cref= #getElem(int) </seealso>
		/// <seealso cref= #getElem(int, int) </seealso>
		public override void SetElem(int i, int val)
		{
			Data_Renamed[i + Offset_Renamed] = (float)val;
			TheTrackable.markDirty();
		}

		/// <summary>
		/// Sets the requested data array element in the specified bank to
		/// the given <code>int</code>.
		/// </summary>
		/// <param name="bank"> The bank number. </param>
		/// <param name="i"> The desired data array element. </param>
		/// <param name="val"> The value to be set. </param>
		/// <seealso cref= #getElem(int) </seealso>
		/// <seealso cref= #getElem(int, int) </seealso>
		public override void SetElem(int bank, int i, int val)
		{
			Bankdata[bank][i + Offsets_Renamed[bank]] = (float)val;
			TheTrackable.markDirty();
		}

		/// <summary>
		/// Returns the requested data array element from the first
		/// (default) bank as a <code>float</code>.
		/// </summary>
		/// <param name="i"> The desired data array element.
		/// </param>
		/// <returns> The data entry as a <code>float</code>. </returns>
		/// <seealso cref= #setElemFloat(int, float) </seealso>
		/// <seealso cref= #setElemFloat(int, int, float) </seealso>
		public override float GetElemFloat(int i)
		{
			return Data_Renamed[i + Offset_Renamed];
		}

		/// <summary>
		/// Returns the requested data array element from the specified
		/// bank as a <code>float</code>.
		/// </summary>
		/// <param name="bank"> The bank number. </param>
		/// <param name="i"> The desired data array element.
		/// </param>
		/// <returns> The data entry as a <code>float</code>. </returns>
		/// <seealso cref= #setElemFloat(int, float) </seealso>
		/// <seealso cref= #setElemFloat(int, int, float) </seealso>
		public override float GetElemFloat(int bank, int i)
		{
			return Bankdata[bank][i + Offsets_Renamed[bank]];
		}

		/// <summary>
		/// Sets the requested data array element in the first (default)
		/// bank to the given <code>float</code>.
		/// </summary>
		/// <param name="i"> The desired data array element. </param>
		/// <param name="val"> The value to be set. </param>
		/// <seealso cref= #getElemFloat(int) </seealso>
		/// <seealso cref= #getElemFloat(int, int) </seealso>
		public override void SetElemFloat(int i, float val)
		{
			Data_Renamed[i + Offset_Renamed] = val;
			TheTrackable.markDirty();
		}

		/// <summary>
		/// Sets the requested data array element in the specified bank to
		/// the given <code>float</code>.
		/// </summary>
		/// <param name="bank"> The bank number. </param>
		/// <param name="i"> The desired data array element. </param>
		/// <param name="val"> The value to be set. </param>
		/// <seealso cref= #getElemFloat(int) </seealso>
		/// <seealso cref= #getElemFloat(int, int) </seealso>
		public override void SetElemFloat(int bank, int i, float val)
		{
			Bankdata[bank][i + Offsets_Renamed[bank]] = val;
			TheTrackable.markDirty();
		}

		/// <summary>
		/// Returns the requested data array element from the first
		/// (default) bank as a <code>double</code>.
		/// </summary>
		/// <param name="i"> The desired data array element.
		/// </param>
		/// <returns> The data entry as a <code>double</code>. </returns>
		/// <seealso cref= #setElemDouble(int, double) </seealso>
		/// <seealso cref= #setElemDouble(int, int, double) </seealso>
		public override double GetElemDouble(int i)
		{
			return (double)Data_Renamed[i + Offset_Renamed];
		}

		/// <summary>
		/// Returns the requested data array element from the specified
		/// bank as a <code>double</code>.
		/// </summary>
		/// <param name="bank"> The bank number. </param>
		/// <param name="i"> The desired data array element.
		/// </param>
		/// <returns> The data entry as a <code>double</code>. </returns>
		/// <seealso cref= #setElemDouble(int, double) </seealso>
		/// <seealso cref= #setElemDouble(int, int, double) </seealso>
		public override double GetElemDouble(int bank, int i)
		{
			return (double)Bankdata[bank][i + Offsets_Renamed[bank]];
		}

		/// <summary>
		/// Sets the requested data array element in the first (default)
		/// bank to the given <code>double</code>.
		/// </summary>
		/// <param name="i"> The desired data array element. </param>
		/// <param name="val"> The value to be set. </param>
		/// <seealso cref= #getElemDouble(int) </seealso>
		/// <seealso cref= #getElemDouble(int, int) </seealso>
		public override void SetElemDouble(int i, double val)
		{
			Data_Renamed[i + Offset_Renamed] = (float)val;
			TheTrackable.markDirty();
		}

		/// <summary>
		/// Sets the requested data array element in the specified bank to
		/// the given <code>double</code>.
		/// </summary>
		/// <param name="bank"> The bank number. </param>
		/// <param name="i"> The desired data array element. </param>
		/// <param name="val"> The value to be set. </param>
		/// <seealso cref= #getElemDouble(int) </seealso>
		/// <seealso cref= #getElemDouble(int, int) </seealso>
		public override void SetElemDouble(int bank, int i, double val)
		{
			Bankdata[bank][i + Offsets_Renamed[bank]] = (float)val;
			TheTrackable.markDirty();
		}
	}

}