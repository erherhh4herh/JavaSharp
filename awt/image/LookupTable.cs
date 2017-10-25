/*
 * Copyright (c) 1997, 2000, Oracle and/or its affiliates. All rights reserved.
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
	/// This abstract class defines a lookup table object.  ByteLookupTable
	/// and ShortLookupTable are subclasses, which
	/// contain byte and short data, respectively.  A lookup table
	/// contains data arrays for one or more bands (or components) of an image
	/// (for example, separate arrays for R, G, and B),
	/// and it contains an offset which will be subtracted from the
	/// input values before indexing into the arrays.  This allows an array
	/// smaller than the native data size to be provided for a
	/// constrained input.  If there is only one array in the lookup
	/// table, it will be applied to all bands.  All arrays must be the
	/// same size.
	/// </summary>
	/// <seealso cref= ByteLookupTable </seealso>
	/// <seealso cref= ShortLookupTable </seealso>
	/// <seealso cref= LookupOp </seealso>
	public abstract class LookupTable : Object
	{

		/// <summary>
		/// Constants
		/// </summary>

		internal int NumComponents_Renamed;
		internal int Offset_Renamed;
		internal int NumEntries;

		/// <summary>
		/// Constructs a new LookupTable from the number of components and an offset
		/// into the lookup table. </summary>
		/// <param name="offset"> the offset to subtract from input values before indexing
		///        into the data arrays for this <code>LookupTable</code> </param>
		/// <param name="numComponents"> the number of data arrays in this
		///        <code>LookupTable</code> </param>
		/// <exception cref="IllegalArgumentException"> if <code>offset</code> is less than 0
		///         or if <code>numComponents</code> is less than 1 </exception>
		protected internal LookupTable(int offset, int numComponents)
		{
			if (offset < 0)
			{
				throw new IllegalArgumentException("Offset must be greater than 0");
			}
			if (numComponents < 1)
			{
				throw new IllegalArgumentException("Number of components must " + " be at least 1");
			}
			this.NumComponents_Renamed = numComponents;
			this.Offset_Renamed = offset;
		}

		/// <summary>
		/// Returns the number of components in the lookup table. </summary>
		/// <returns> the number of components in this <code>LookupTable</code>. </returns>
		public virtual int NumComponents
		{
			get
			{
				return NumComponents_Renamed;
			}
		}

		/// <summary>
		/// Returns the offset. </summary>
		/// <returns> the offset of this <code>LookupTable</code>. </returns>
		public virtual int Offset
		{
			get
			{
				return Offset_Renamed;
			}
		}

		/// <summary>
		/// Returns an <code>int</code> array of components for
		/// one pixel.  The <code>dest</code> array contains the
		/// result of the lookup and is returned.  If dest is
		/// <code>null</code>, a new array is allocated.  The
		/// source and destination can be equal. </summary>
		/// <param name="src"> the source array of components of one pixel </param>
		/// <param name="dest"> the destination array of components for one pixel,
		///        translated with this <code>LookupTable</code> </param>
		/// <returns> an <code>int</code> array of components for one
		///         pixel. </returns>
		public abstract int[] LookupPixel(int[] src, int[] dest);

	}

}