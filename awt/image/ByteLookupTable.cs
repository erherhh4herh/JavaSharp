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

namespace java.awt.image
{


	/// <summary>
	/// This class defines a lookup table object.  The output of a
	/// lookup operation using an object of this class is interpreted
	/// as an unsigned byte quantity.  The lookup table contains byte
	/// data arrays for one or more bands (or components) of an image,
	/// and it contains an offset which will be subtracted from the
	/// input values before indexing the arrays.  This allows an array
	/// smaller than the native data size to be provided for a
	/// constrained input.  If there is only one array in the lookup
	/// table, it will be applied to all bands.
	/// </summary>
	/// <seealso cref= ShortLookupTable </seealso>
	/// <seealso cref= LookupOp </seealso>
	public class ByteLookupTable : LookupTable
	{

		/// <summary>
		/// Constants
		/// </summary>

		internal sbyte[][] Data;

		/// <summary>
		/// Constructs a ByteLookupTable object from an array of byte
		/// arrays representing a lookup table for each
		/// band.  The offset will be subtracted from input
		/// values before indexing into the arrays.  The number of
		/// bands is the length of the data argument.  The
		/// data array for each band is stored as a reference. </summary>
		/// <param name="offset"> the value subtracted from the input values
		///        before indexing into the arrays </param>
		/// <param name="data"> an array of byte arrays representing a lookup
		///        table for each band </param>
		/// <exception cref="IllegalArgumentException"> if <code>offset</code> is
		///         is less than 0 or if the length of <code>data</code>
		///         is less than 1 </exception>
		public ByteLookupTable(int offset, sbyte[][] data) : base(offset,data.Length)
		{
			NumComponents_Renamed = data.Length;
			NumEntries = data[0].Length;
			this.Data = new sbyte[NumComponents_Renamed][];
			// Allocate the array and copy the data reference
			for (int i = 0; i < NumComponents_Renamed; i++)
			{
				this.Data[i] = data[i];
			}
		}

		/// <summary>
		/// Constructs a ByteLookupTable object from an array
		/// of bytes representing a lookup table to be applied to all
		/// bands.  The offset will be subtracted from input
		/// values before indexing into the array.
		/// The data array is stored as a reference. </summary>
		/// <param name="offset"> the value subtracted from the input values
		///        before indexing into the array </param>
		/// <param name="data"> an array of bytes </param>
		/// <exception cref="IllegalArgumentException"> if <code>offset</code> is
		///         is less than 0 or if the length of <code>data</code>
		///         is less than 1 </exception>
		public ByteLookupTable(int offset, sbyte[] data) : base(offset,data.Length)
		{
			NumComponents_Renamed = 1;
			NumEntries = data.Length;
			this.Data = new sbyte[1][];
			this.Data[0] = data;
		}

		/// <summary>
		/// Returns the lookup table data by reference.  If this ByteLookupTable
		/// was constructed using a single byte array, the length of the returned
		/// array is one. </summary>
		/// <returns> the data array of this <code>ByteLookupTable</code>. </returns>
		public sbyte[][] Table
		{
			get
			{
				return Data;
			}
		}

		/// <summary>
		/// Returns an array of samples of a pixel, translated with the lookup
		/// table. The source and destination array can be the same array.
		/// Array <code>dst</code> is returned.
		/// </summary>
		/// <param name="src"> the source array. </param>
		/// <param name="dst"> the destination array. This array must be at least as
		///         long as <code>src</code>.  If <code>dst</code> is
		///         <code>null</code>, a new array will be allocated having the
		///         same length as <code>src</code>. </param>
		/// <returns> the array <code>dst</code>, an <code>int</code> array of
		///         samples. </returns>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if <code>src</code> is
		///            longer than <code>dst</code> or if for any element
		///            <code>i</code> of <code>src</code>,
		///            <code>src[i]-offset</code> is either less than zero or
		///            greater than or equal to the length of the lookup table
		///            for any band. </exception>
		public override int[] LookupPixel(int[] src, int[] dst)
		{
			if (dst == null)
			{
				// Need to alloc a new destination array
				dst = new int[src.Length];
			}

			if (NumComponents_Renamed == 1)
			{
				// Apply one LUT to all bands
				for (int i = 0; i < src.Length; i++)
				{
					int s = src[i] - Offset_Renamed;
					if (s < 0)
					{
						throw new ArrayIndexOutOfBoundsException("src[" + i + "]-offset is " + "less than zero");
					}
					dst[i] = (int) Data[0][s];
				}
			}
			else
			{
				for (int i = 0; i < src.Length; i++)
				{
					int s = src[i] - Offset_Renamed;
					if (s < 0)
					{
						throw new ArrayIndexOutOfBoundsException("src[" + i + "]-offset is " + "less than zero");
					}
					dst[i] = (int) Data[i][s];
				}
			}
			return dst;
		}

		/// <summary>
		/// Returns an array of samples of a pixel, translated with the lookup
		/// table. The source and destination array can be the same array.
		/// Array <code>dst</code> is returned.
		/// </summary>
		/// <param name="src"> the source array. </param>
		/// <param name="dst"> the destination array. This array must be at least as
		///         long as <code>src</code>.  If <code>dst</code> is
		///         <code>null</code>, a new array will be allocated having the
		///         same length as <code>src</code>. </param>
		/// <returns> the array <code>dst</code>, an <code>int</code> array of
		///         samples. </returns>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if <code>src</code> is
		///            longer than <code>dst</code> or if for any element
		///            <code>i</code> of <code>src</code>,
		///            {@code (src[i]&0xff)-offset} is either less than
		///            zero or greater than or equal to the length of the
		///            lookup table for any band. </exception>
		public virtual sbyte[] LookupPixel(sbyte[] src, sbyte[] dst)
		{
			if (dst == null)
			{
				// Need to alloc a new destination array
				dst = new sbyte[src.Length];
			}

			if (NumComponents_Renamed == 1)
			{
				// Apply one LUT to all bands
				for (int i = 0; i < src.Length; i++)
				{
					int s = (src[i] & 0xff) - Offset_Renamed;
					if (s < 0)
					{
						throw new ArrayIndexOutOfBoundsException("src[" + i + "]-offset is " + "less than zero");
					}
					dst[i] = Data[0][s];
				}
			}
			else
			{
				for (int i = 0; i < src.Length; i++)
				{
					int s = (src[i] & 0xff) - Offset_Renamed;
					if (s < 0)
					{
						throw new ArrayIndexOutOfBoundsException("src[" + i + "]-offset is " + "less than zero");
					}
					dst[i] = Data[i][s];
				}
			}
			return dst;
		}

	}

}