/*
 * Copyright (c) 2006, 2013, Oracle and/or its affiliates. All rights reserved.
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
	/// Provides the actual implementation for the LinearGradientPaint.
	/// This is where the pixel processing is done.
	/// </summary>
	/// <seealso cref= java.awt.LinearGradientPaint </seealso>
	/// <seealso cref= java.awt.PaintContext </seealso>
	/// <seealso cref= java.awt.Paint
	/// @author Nicholas Talian, Vincent Hardy, Jim Graham, Jerry Evans </seealso>
	internal sealed class LinearGradientPaintContext : MultipleGradientPaintContext
	{

		/// <summary>
		/// The following invariants are used to process the gradient value from
		/// a device space coordinate, (X, Y):
		///     g(X, Y) = dgdX*X + dgdY*Y + gc
		/// </summary>
		private float DgdX, DgdY, Gc;

		/// <summary>
		/// Constructor for LinearGradientPaintContext.
		/// </summary>
		/// <param name="paint"> the {@code LinearGradientPaint} from which this context
		///              is created </param>
		/// <param name="cm"> {@code ColorModel} that receives
		///           the <code>Paint</code> data. This is used only as a hint. </param>
		/// <param name="deviceBounds"> the device space bounding box of the
		///                     graphics primitive being rendered </param>
		/// <param name="userBounds"> the user space bounding box of the
		///                   graphics primitive being rendered </param>
		/// <param name="t"> the {@code AffineTransform} from user
		///          space into device space (gradientTransform should be
		///          concatenated with this) </param>
		/// <param name="hints"> the hints that the context object uses to choose
		///              between rendering alternatives </param>
		/// <param name="start"> gradient start point, in user space </param>
		/// <param name="end"> gradient end point, in user space </param>
		/// <param name="fractions"> the fractions specifying the gradient distribution </param>
		/// <param name="colors"> the gradient colors </param>
		/// <param name="cycleMethod"> either NO_CYCLE, REFLECT, or REPEAT </param>
		/// <param name="colorSpace"> which colorspace to use for interpolation,
		///                   either SRGB or LINEAR_RGB </param>
		internal LinearGradientPaintContext(LinearGradientPaint paint, ColorModel cm, Rectangle deviceBounds, Rectangle2D userBounds, AffineTransform t, RenderingHints hints, Point2D start, Point2D end, float[] fractions, Color[] colors, CycleMethod cycleMethod, ColorSpaceType colorSpace) : base(paint, cm, deviceBounds, userBounds, t, hints, fractions, colors, cycleMethod, colorSpace)
		{

			// A given point in the raster should take on the same color as its
			// projection onto the gradient vector.
			// Thus, we want the projection of the current position vector
			// onto the gradient vector, then normalized with respect to the
			// length of the gradient vector, giving a value which can be mapped
			// into the range 0-1.
			//    projection =
			//        currentVector dot gradientVector / length(gradientVector)
			//    normalized = projection / length(gradientVector)

			float startx = (float)start.X;
			float starty = (float)start.Y;
			float endx = (float)end.X;
			float endy = (float)end.Y;

			float dx = endx - startx; // change in x from start to end
			float dy = endy - starty; // change in y from start to end
			float dSq = dx * dx + dy * dy; // total distance squared

			// avoid repeated calculations by doing these divides once
			float constX = dx / dSq;
			float constY = dy / dSq;

			// incremental change along gradient for +x
			DgdX = A00 * constX + A10 * constY;
			// incremental change along gradient for +y
			DgdY = A01 * constX + A11 * constY;

			// constant, incorporates the translation components from the matrix
			Gc = (A02 - startx) * constX + (A12 - starty) * constY;
		}

		/// <summary>
		/// Return a Raster containing the colors generated for the graphics
		/// operation.  This is where the area is filled with colors distributed
		/// linearly.
		/// </summary>
		/// <param name="x">,y,w,h the area in device space for which colors are
		/// generated. </param>
		protected internal override void FillRaster(int[] pixels, int off, int adjust, int x, int y, int w, int h)
		{
			// current value for row gradients
			float g = 0;

			// used to end iteration on rows
			int rowLimit = off + w;

			// constant which can be pulled out of the inner loop
			float initConst = (DgdX * x) + Gc;

			for (int i = 0; i < h; i++) // for every row
			{

				// initialize current value to be start
				g = initConst + DgdY * (y + i);

				while (off < rowLimit) // for every pixel in this row
				{
					// get the color
					pixels[off++] = IndexIntoGradientsArrays(g);

					// incremental change in g
					g += DgdX;
				}

				// change in off from row to row
				off += adjust;

				//rowlimit is width + offset
				rowLimit = off + w;
			}
		}
	}

}