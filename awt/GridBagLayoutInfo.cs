using System;

/*
 * Copyright (c) 2006, Oracle and/or its affiliates. All rights reserved.
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

namespace java.awt
{

	/// <summary>
	/// The {@code GridBagLayoutInfo} is an utility class for
	/// {@code GridBagLayout} layout manager.
	/// It stores align, size and baseline parameters for every component within a container.
	/// <para>
	/// </para>
	/// </summary>
	/// <seealso cref=       java.awt.GridBagLayout </seealso>
	/// <seealso cref=       java.awt.GridBagConstraints
	/// @since 1.6 </seealso>
	[Serializable]
	public class GridBagLayoutInfo
	{
		/*
		 * serialVersionUID
		 */
		private const long SerialVersionUID = -4899416460737170217L;

		internal int Width, Height; // number of  cells: horizontal and vertical
		internal int Startx, Starty; // starting point for layout
		internal int[] MinWidth; // largest minWidth in each column
		internal int[] MinHeight; // largest minHeight in each row
		internal double[] WeightX; // largest weight in each column
		internal double[] WeightY; // largest weight in each row
		internal bool HasBaseline_Renamed; /* Whether or not baseline layout has been
	                                 * requested and one of the components
	                                 * has a valid baseline. */
		// These are only valid if hasBaseline is true and are indexed by
		// row.
		internal short[] BaselineType; /* The type of baseline for a particular
	                                 * row.  A mix of the BaselineResizeBehavior
	                                 * constants (1 << ordinal()) */
		internal int[] MaxAscent; // Max ascent (baseline).
		internal int[] MaxDescent; // Max descent (height - baseline)

		/// <summary>
		/// Creates an instance of GridBagLayoutInfo representing {@code GridBagLayout}
		/// grid cells with it's own parameters. </summary>
		/// <param name="width"> the columns </param>
		/// <param name="height"> the rows
		/// @since 6.0 </param>
		internal GridBagLayoutInfo(int width, int height)
		{
			this.Width = width;
			this.Height = height;
		}

		/// <summary>
		/// Returns true if the specified row has any component aligned on the
		/// baseline with a baseline resize behavior of CONSTANT_DESCENT.
		/// </summary>
		internal virtual bool HasConstantDescent(int row)
		{
			return ((BaselineType[row] & (1 << (int)Component.BaselineResizeBehavior.CONSTANT_DESCENT)) != 0);
		}

		/// <summary>
		/// Returns true if there is a baseline for the specified row.
		/// </summary>
		internal virtual bool HasBaseline(int row)
		{
			return (HasBaseline_Renamed && BaselineType[row] != 0);
		}
	}

}