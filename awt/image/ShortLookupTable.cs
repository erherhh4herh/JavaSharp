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
	/// as an unsigned short quantity.  The lookup table contains short
	/// data arrays for one or more bands (or components) of an image,
	/// and it contains an offset which will be subtracted from the
	/// input values before indexing the arrays.  This allows an array
	/// smaller than the native data size to be provided for a
	/// constrained input.  If there is only one array in the lookup
	/// table, it will be applied to all bands.
	/// </summary>
	/// <seealso cref= ByteLookupTable </seealso>
	/// <seealso cref= LookupOp </seealso>
	public class ShortLookupTable : LookupTable
	{

		/// <summary>
		/// Constants
		/// </summary>

		internal short[][] Data;

		/// <summary>
		/// Constructs a ShortLookupTable object from an array of short
		/// arrays representing a lookup table for each
		/// band.  The offset will be subtracted from the input
		/// values before indexing into the arrays.  The number of
		/// bands is the length of the data argument.  The
		/// data array for each band is stored as a reference. </summary>
		/// <param name="offset"> the value subtracted from the input values
		///        before indexing into the arrays </param>
		/// <param name="data"> an array of short arrays representing a lookup
		///        table for each band </param>
		public ShortLookupTable(int offset, short[][] data) : base(offset,data.Length)
		{
			NumComponents_Renamed = data.Length;
			NumEntries = data[0].Length;
			this.Data = new short[NumComponents_Renamed][];
			// Allocate the array and copy the data reference
			for (int i = 0; i < NumComponents_Renamed; i++)
			{
				this.Data[i] = data[i];
			}
		}

		/// <summary>
		/// Constructs a ShortLookupTable object from an array
		/// of shorts representing a lookup table for each
		/// band.  The offset will be subtracted from the input
		/// values before indexing into the array.  The
		/// data array is stored as a reference. </summary>
		/// <param name="offset"> the value subtracted from the input values
		///        before indexing into the arrays </param>
		/// <param name="data"> an array of shorts </param>
		public ShortLookupTable(int offset, short[] data) : base(offset,data.Length)
		{
			NumComponents_Renamed = 1;
			NumEntries = data.Length;
			this.Data = new short[1][];
			this.Data[0] = data;
		}

		/// <summary>
		/// Returns the lookup table data by reference.  If this ShortLookupTable
		/// was constructed using a single short array, the length of the returned
		/// array is one. </summary>
		/// <returns> ShortLookupTable data array. </returns>
		public short[][] Table
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
		///            {@code (src[i]&0xffff)-offset} is either less than
		///            zero or greater than or equal to the length of the
		///            lookup table for any band. </exception>
		public override int[] LookupPixel(int[] src, int[] dst)
		{
			if (dst == null)
			{
				// Need to alloc a new destination array
				dst = new int[src.Length];
			}

			if (NumComponents_Renamed == 1)
			{
				// Apply one LUT to all channels
				for (int i = 0; i < src.Length; i++)
				{
					int s = (src[i] & 0xffff) - Offset_Renamed;
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
					int s = (src[i] & 0xffff) - Offset_Renamed;
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
		///            {@code (src[i]&0xffff)-offset} is either less than
		///            zero or greater than or equal to the length of the
		///            lookup table for any band. </exception>
		public virtual short[] LookupPixel(short[] src, short[] dst)
		{
			if (dst == null)
			{
				// Need to alloc a new destination array
				dst = new short[src.Length];
			}

			if (NumComponents_Renamed == 1)
			{
				// Apply one LUT to all channels
				for (int i = 0; i < src.Length; i++)
				{
					int s = (src[i] & 0xffff) - Offset_Renamed;
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
					int s = (src[i] & 0xffff) - Offset_Renamed;
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