﻿/*
 * Copyright (c) 1997, 2008, Oracle and/or its affiliates. All rights reserved.
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

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to .NET:
	import static sun.java2d.StateTrackable.State.*;

	/// <summary>
	/// This class extends <CODE>DataBuffer</CODE> and stores data internally as bytes.
	/// Values stored in the byte array(s) of this <CODE>DataBuffer</CODE> are treated as
	/// unsigned values.
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
	/// underlying storage as a Java array, as noted below in the
	/// documentation for those methods.
	/// </a>
	/// </para>
	/// </summary>
	public sealed class DataBufferByte : DataBuffer
	{
		/// <summary>
		/// The default data bank. </summary>
		internal sbyte[] Data_Renamed;

		/// <summary>
		/// All data banks </summary>
		internal sbyte[][] Bankdata;

		/// <summary>
		/// Constructs a byte-based <CODE>DataBuffer</CODE> with a single bank and the
		/// specified size.
		/// </summary>
		/// <param name="size"> The size of the <CODE>DataBuffer</CODE>. </param>
		public DataBufferByte(int size) : base(STABLE, TYPE_BYTE, size)
		{
		  Data_Renamed = new sbyte[size];
		  Bankdata = new sbyte[1][];
		  Bankdata[0] = Data_Renamed;
		}

		/// <summary>
		/// Constructs a byte based <CODE>DataBuffer</CODE> with the specified number of
		/// banks all of which are the specified size.
		/// </summary>
		/// <param name="size"> The size of the banks in the <CODE>DataBuffer</CODE>. </param>
		/// <param name="numBanks"> The number of banks in the a<CODE>DataBuffer</CODE>. </param>
		public DataBufferByte(int size, int numBanks) : base(STABLE, TYPE_BYTE, size, numBanks)
		{
			Bankdata = new sbyte[numBanks][];
			for (int i = 0; i < numBanks; i++)
			{
				Bankdata[i] = new sbyte[size];
			}
			Data_Renamed = Bankdata[0];
		}

		/// <summary>
		/// Constructs a byte-based <CODE>DataBuffer</CODE> with a single bank using the
		/// specified array.
		/// Only the first <CODE>size</CODE> elements should be used by accessors of
		/// this <CODE>DataBuffer</CODE>.  <CODE>dataArray</CODE> must be large enough to
		/// hold <CODE>size</CODE> elements.
		/// <para>
		/// Note that {@code DataBuffer} objects created by this constructor
		/// may be incompatible with <a href="#optimizations">performance
		/// optimizations</a> used by some implementations (such as caching
		/// an associated image in video memory).
		/// 
		/// </para>
		/// </summary>
		/// <param name="dataArray"> The byte array for the <CODE>DataBuffer</CODE>. </param>
		/// <param name="size"> The size of the <CODE>DataBuffer</CODE> bank. </param>
		public DataBufferByte(sbyte[] dataArray, int size) : base(UNTRACKABLE, TYPE_BYTE, size)
		{
			Data_Renamed = dataArray;
			Bankdata = new sbyte[1][];
			Bankdata[0] = Data_Renamed;
		}

		/// <summary>
		/// Constructs a byte-based <CODE>DataBuffer</CODE> with a single bank using the
		/// specified array, size, and offset.  <CODE>dataArray</CODE> must have at least
		/// <CODE>offset</CODE> + <CODE>size</CODE> elements.  Only elements <CODE>offset</CODE>
		/// through <CODE>offset</CODE> + <CODE>size</CODE> - 1
		/// should be used by accessors of this <CODE>DataBuffer</CODE>.
		/// <para>
		/// Note that {@code DataBuffer} objects created by this constructor
		/// may be incompatible with <a href="#optimizations">performance
		/// optimizations</a> used by some implementations (such as caching
		/// an associated image in video memory).
		/// 
		/// </para>
		/// </summary>
		/// <param name="dataArray"> The byte array for the <CODE>DataBuffer</CODE>. </param>
		/// <param name="size"> The size of the <CODE>DataBuffer</CODE> bank. </param>
		/// <param name="offset"> The offset into the <CODE>dataArray</CODE>. <CODE>dataArray</CODE>
		/// must have at least <CODE>offset</CODE> + <CODE>size</CODE> elements. </param>
		public DataBufferByte(sbyte[] dataArray, int size, int offset) : base(UNTRACKABLE, TYPE_BYTE, size, 1, offset)
		{
			Data_Renamed = dataArray;
			Bankdata = new sbyte[1][];
			Bankdata[0] = Data_Renamed;
		}

		/// <summary>
		/// Constructs a byte-based <CODE>DataBuffer</CODE> with the specified arrays.
		/// The number of banks is equal to <CODE>dataArray.length</CODE>.
		/// Only the first <CODE>size</CODE> elements of each array should be used by
		/// accessors of this <CODE>DataBuffer</CODE>.
		/// <para>
		/// Note that {@code DataBuffer} objects created by this constructor
		/// may be incompatible with <a href="#optimizations">performance
		/// optimizations</a> used by some implementations (such as caching
		/// an associated image in video memory).
		/// 
		/// </para>
		/// </summary>
		/// <param name="dataArray"> The byte arrays for the <CODE>DataBuffer</CODE>. </param>
		/// <param name="size"> The size of the banks in the <CODE>DataBuffer</CODE>. </param>
		public DataBufferByte(sbyte[][] dataArray, int size) : base(UNTRACKABLE, TYPE_BYTE, size, dataArray.Length)
		{
			Bankdata = (sbyte[][]) dataArray.clone();
			Data_Renamed = Bankdata[0];
		}

		/// <summary>
		/// Constructs a byte-based <CODE>DataBuffer</CODE> with the specified arrays, size,
		/// and offsets.
		/// The number of banks is equal to <CODE>dataArray.length</CODE>.  Each array must
		/// be at least as large as <CODE>size</CODE> + the corresponding <CODE>offset</CODE>.
		/// There must be an entry in the <CODE>offset</CODE> array for each <CODE>dataArray</CODE>
		/// entry.  For each bank, only elements <CODE>offset</CODE> through
		/// <CODE>offset</CODE> + <CODE>size</CODE> - 1 should be used by accessors of this
		/// <CODE>DataBuffer</CODE>.
		/// <para>
		/// Note that {@code DataBuffer} objects created by this constructor
		/// may be incompatible with <a href="#optimizations">performance
		/// optimizations</a> used by some implementations (such as caching
		/// an associated image in video memory).
		/// 
		/// </para>
		/// </summary>
		/// <param name="dataArray"> The byte arrays for the <CODE>DataBuffer</CODE>. </param>
		/// <param name="size"> The size of the banks in the <CODE>DataBuffer</CODE>. </param>
		/// <param name="offsets"> The offsets into each array. </param>
		public DataBufferByte(sbyte[][] dataArray, int size, int[] offsets) : base(UNTRACKABLE, TYPE_BYTE, size, dataArray.Length, offsets)
		{
			Bankdata = (sbyte[][]) dataArray.clone();
			Data_Renamed = Bankdata[0];
		}

		/// <summary>
		/// Returns the default (first) byte data array.
		/// <para>
		/// Note that calling this method may cause this {@code DataBuffer}
		/// object to be incompatible with <a href="#optimizations">performance
		/// optimizations</a> used by some implementations (such as caching
		/// an associated image in video memory).
		/// 
		/// </para>
		/// </summary>
		/// <returns> The first byte data array. </returns>
		public sbyte[] Data
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
		/// <param name="bank"> The bank whose data array you want to get. </param>
		/// <returns> The data array for the specified bank. </returns>
		public sbyte[] GetData(int bank)
		{
			TheTrackable.setUntrackable();
			return Bankdata[bank];
		}

		/// <summary>
		/// Returns the data arrays for all banks.
		/// <para>
		/// Note that calling this method may cause this {@code DataBuffer}
		/// object to be incompatible with <a href="#optimizations">performance
		/// optimizations</a> used by some implementations (such as caching
		/// an associated image in video memory).
		/// 
		/// </para>
		/// </summary>
		/// <returns> All of the data arrays. </returns>
		public sbyte[][] BankData
		{
			get
			{
				TheTrackable.setUntrackable();
				return (sbyte[][]) Bankdata.clone();
			}
		}

		/// <summary>
		/// Returns the requested data array element from the first (default) bank.
		/// </summary>
		/// <param name="i"> The data array element you want to get. </param>
		/// <returns> The requested data array element as an integer. </returns>
		/// <seealso cref= #setElem(int, int) </seealso>
		/// <seealso cref= #setElem(int, int, int) </seealso>
		public override int GetElem(int i)
		{
			return (int)(Data_Renamed[i + Offset_Renamed]) & 0xff;
		}

		/// <summary>
		/// Returns the requested data array element from the specified bank.
		/// </summary>
		/// <param name="bank"> The bank from which you want to get a data array element. </param>
		/// <param name="i"> The data array element you want to get. </param>
		/// <returns> The requested data array element as an integer. </returns>
		/// <seealso cref= #setElem(int, int) </seealso>
		/// <seealso cref= #setElem(int, int, int) </seealso>
		public override int GetElem(int bank, int i)
		{
			return (int)(Bankdata[bank][i + Offsets_Renamed[bank]]) & 0xff;
		}

		/// <summary>
		/// Sets the requested data array element in the first (default) bank
		/// to the specified value.
		/// </summary>
		/// <param name="i"> The data array element you want to set. </param>
		/// <param name="val"> The integer value to which you want to set the data array element. </param>
		/// <seealso cref= #getElem(int) </seealso>
		/// <seealso cref= #getElem(int, int) </seealso>
		public override void SetElem(int i, int val)
		{
			Data_Renamed[i + Offset_Renamed] = (sbyte)val;
			TheTrackable.markDirty();
		}

		/// <summary>
		/// Sets the requested data array element in the specified bank
		/// from the given integer. </summary>
		/// <param name="bank"> The bank in which you want to set the data array element. </param>
		/// <param name="i"> The data array element you want to set. </param>
		/// <param name="val"> The integer value to which you want to set the specified data array element. </param>
		/// <seealso cref= #getElem(int) </seealso>
		/// <seealso cref= #getElem(int, int) </seealso>
		public override void SetElem(int bank, int i, int val)
		{
			Bankdata[bank][i + Offsets_Renamed[bank]] = (sbyte)val;
			TheTrackable.markDirty();
		}
	}

}