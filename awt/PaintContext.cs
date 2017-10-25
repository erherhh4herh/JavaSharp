/*
 * Copyright (c) 1997, 2007, Oracle and/or its affiliates. All rights reserved.
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
	/// The <code>PaintContext</code> interface defines the encapsulated
	/// and optimized environment to generate color patterns in device
	/// space for fill or stroke operations on a
	/// <seealso cref="Graphics2D"/>.  The <code>PaintContext</code> provides
	/// the necessary colors for <code>Graphics2D</code> operations in the
	/// form of a <seealso cref="Raster"/> associated with a <seealso cref="ColorModel"/>.
	/// The <code>PaintContext</code> maintains state for a particular paint
	/// operation.  In a multi-threaded environment, several
	/// contexts can exist simultaneously for a single <seealso cref="Paint"/> object. </summary>
	/// <seealso cref= Paint </seealso>

	public interface PaintContext
	{
		/// <summary>
		/// Releases the resources allocated for the operation.
		/// </summary>
		void Dispose();

		/// <summary>
		/// Returns the <code>ColorModel</code> of the output.  Note that
		/// this <code>ColorModel</code> might be different from the hint
		/// specified in the
		/// {@link Paint#createContext(ColorModel, Rectangle, Rectangle2D,
		/// AffineTransform, RenderingHints) createContext} method of
		/// <code>Paint</code>.  Not all <code>PaintContext</code> objects are
		/// capable of generating color patterns in an arbitrary
		/// <code>ColorModel</code>. </summary>
		/// <returns> the <code>ColorModel</code> of the output. </returns>
		ColorModel ColorModel {get;}

		/// <summary>
		/// Returns a <code>Raster</code> containing the colors generated for
		/// the graphics operation. </summary>
		/// <param name="x"> the x coordinate of the area in device space
		/// for which colors are generated. </param>
		/// <param name="y"> the y coordinate of the area in device space
		/// for which colors are generated. </param>
		/// <param name="w"> the width of the area in device space </param>
		/// <param name="h"> the height of the area in device space </param>
		/// <returns> a <code>Raster</code> representing the specified
		/// rectangular area and containing the colors generated for
		/// the graphics operation. </returns>
		Raster GetRaster(int x, int y, int w, int h);

	}

}