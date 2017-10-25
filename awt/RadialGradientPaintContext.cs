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
	/// Provides the actual implementation for the RadialGradientPaint.
	/// This is where the pixel processing is done.  A RadialGradienPaint
	/// only supports circular gradients, but it should be possible to scale
	/// the circle to look approximately elliptical, by means of a
	/// gradient transform passed into the RadialGradientPaint constructor.
	/// 
	/// @author Nicholas Talian, Vincent Hardy, Jim Graham, Jerry Evans
	/// </summary>
	internal sealed class RadialGradientPaintContext : MultipleGradientPaintContext
	{

		/// <summary>
		/// True when (focus == center). </summary>
		private bool IsSimpleFocus = false;

		/// <summary>
		/// True when (cycleMethod == NO_CYCLE). </summary>
		private bool IsNonCyclic = false;

		/// <summary>
		/// Radius of the outermost circle defining the 100% gradient stop. </summary>
		private float Radius;

		/// <summary>
		/// Variables representing center and focus points. </summary>
		private float CenterX, CenterY, FocusX, FocusY;

		/// <summary>
		/// Radius of the gradient circle squared. </summary>
		private float RadiusSq;

		/// <summary>
		/// Constant part of X, Y user space coordinates. </summary>
		private float ConstA, ConstB;

		/// <summary>
		/// Constant second order delta for simple loop. </summary>
		private float GDeltaDelta;

		/// <summary>
		/// This value represents the solution when focusX == X.  It is called
		/// trivial because it is easier to calculate than the general case.
		/// </summary>
		private float Trivial;

		/// <summary>
		/// Amount for offset when clamping focus. </summary>
		private const float SCALEBACK = .99f;

		/// <summary>
		/// Constructor for RadialGradientPaintContext.
		/// </summary>
		/// <param name="paint"> the {@code RadialGradientPaint} from which this context
		///              is created </param>
		/// <param name="cm"> the {@code ColorModel} that receives
		///           the {@code Paint} data (this is used only as a hint) </param>
		/// <param name="deviceBounds"> the device space bounding box of the
		///                     graphics primitive being rendered </param>
		/// <param name="userBounds"> the user space bounding box of the
		///                   graphics primitive being rendered </param>
		/// <param name="t"> the {@code AffineTransform} from user
		///          space into device space (gradientTransform should be
		///          concatenated with this) </param>
		/// <param name="hints"> the hints that the context object uses to choose
		///              between rendering alternatives </param>
		/// <param name="cx"> the center X coordinate in user space of the circle defining
		///           the gradient.  The last color of the gradient is mapped to
		///           the perimeter of this circle. </param>
		/// <param name="cy"> the center Y coordinate in user space of the circle defining
		///           the gradient.  The last color of the gradient is mapped to
		///           the perimeter of this circle. </param>
		/// <param name="r"> the radius of the circle defining the extents of the
		///          color gradient </param>
		/// <param name="fx"> the X coordinate in user space to which the first color
		///           is mapped </param>
		/// <param name="fy"> the Y coordinate in user space to which the first color
		///           is mapped </param>
		/// <param name="fractions"> the fractions specifying the gradient distribution </param>
		/// <param name="colors"> the gradient colors </param>
		/// <param name="cycleMethod"> either NO_CYCLE, REFLECT, or REPEAT </param>
		/// <param name="colorSpace"> which colorspace to use for interpolation,
		///                   either SRGB or LINEAR_RGB </param>
		internal RadialGradientPaintContext(RadialGradientPaint paint, ColorModel cm, Rectangle deviceBounds, Rectangle2D userBounds, AffineTransform t, RenderingHints hints, float cx, float cy, float r, float fx, float fy, float[] fractions, Color[] colors, CycleMethod cycleMethod, ColorSpaceType colorSpace) : base(paint, cm, deviceBounds, userBounds, t, hints, fractions, colors, cycleMethod, colorSpace)
		{

			// copy some parameters
			CenterX = cx;
			CenterY = cy;
			FocusX = fx;
			FocusY = fy;
			Radius = r;

			this.IsSimpleFocus = (FocusX == CenterX) && (FocusY == CenterY);
			this.IsNonCyclic = (cycleMethod == CycleMethod.NO_CYCLE);

			// for use in the quadractic equation
			RadiusSq = Radius * Radius;

			float dX = FocusX - CenterX;
			float dY = FocusY - CenterY;

			double distSq = (dX * dX) + (dY * dY);

			// test if distance from focus to center is greater than the radius
			if (distSq > RadiusSq * SCALEBACK)
			{
				// clamp focus to radius
				float scalefactor = (float)System.Math.Sqrt(RadiusSq * SCALEBACK / distSq);
				dX = dX * scalefactor;
				dY = dY * scalefactor;
				FocusX = CenterX + dX;
				FocusY = CenterY + dY;
			}

			// calculate the solution to be used in the case where X == focusX
			// in cyclicCircularGradientFillRaster()
			Trivial = (float)System.Math.Sqrt(RadiusSq - (dX * dX));

			// constant parts of X, Y user space coordinates
			ConstA = A02 - CenterX;
			ConstB = A12 - CenterY;

			// constant second order delta for simple loop
			GDeltaDelta = 2 * (A00 * A00 + A10 * A10) / RadiusSq;
		}

		/// <summary>
		/// Return a Raster containing the colors generated for the graphics
		/// operation.
		/// </summary>
		/// <param name="x">,y,w,h the area in device space for which colors are
		/// generated. </param>
		protected internal override void FillRaster(int[] pixels, int off, int adjust, int x, int y, int w, int h)
		{
			if (IsSimpleFocus && IsNonCyclic && IsSimpleLookup)
			{
				SimpleNonCyclicFillRaster(pixels, off, adjust, x, y, w, h);
			}
			else
			{
				CyclicCircularGradientFillRaster(pixels, off, adjust, x, y, w, h);
			}
		}

		/// <summary>
		/// This code works in the simplest of cases, where the focus == center
		/// point, the gradient is noncyclic, and the gradient lookup method is
		/// fast (single array index, no conversion necessary).
		/// </summary>
		private void SimpleNonCyclicFillRaster(int[] pixels, int off, int adjust, int x, int y, int w, int h)
		{
			/* We calculate sqrt(X^2 + Y^2) relative to the radius
			 * size to get the fraction for the color to use.
			 *
			 * Each step along the scanline adds (a00, a10) to (X, Y).
			 * If we precalculate:
			 *   gRel = X^2+Y^2
			 * for the start of the row, then for each step we need to
			 * calculate:
			 *   gRel' = (X+a00)^2 + (Y+a10)^2
			 *         = X^2 + 2*X*a00 + a00^2 + Y^2 + 2*Y*a10 + a10^2
			 *         = (X^2+Y^2) + 2*(X*a00+Y*a10) + (a00^2+a10^2)
			 *         = gRel + 2*(X*a00+Y*a10) + (a00^2+a10^2)
			 *         = gRel + 2*DP + SD
			 * (where DP = dot product between X,Y and a00,a10
			 *  and   SD = dot product square of the delta vector)
			 * For the step after that we get:
			 *   gRel'' = (X+2*a00)^2 + (Y+2*a10)^2
			 *          = X^2 + 4*X*a00 + 4*a00^2 + Y^2 + 4*Y*a10 + 4*a10^2
			 *          = (X^2+Y^2) + 4*(X*a00+Y*a10) + 4*(a00^2+a10^2)
			 *          = gRel  + 4*DP + 4*SD
			 *          = gRel' + 2*DP + 3*SD
			 * The increment changed by:
			 *     (gRel'' - gRel') - (gRel' - gRel)
			 *   = (2*DP + 3*SD) - (2*DP + SD)
			 *   = 2*SD
			 * Note that this value depends only on the (inverse of the)
			 * transformation matrix and so is a constant for the loop.
			 * To make this all relative to the unit circle, we need to
			 * divide all values as follows:
			 *   [XY] /= radius
			 *   gRel /= radiusSq
			 *   DP   /= radiusSq
			 *   SD   /= radiusSq
			 */
			// coordinates of UL corner in "user space" relative to center
			float rowX = (A00 * x) + (A01 * y) + ConstA;
			float rowY = (A10 * x) + (A11 * y) + ConstB;

			// second order delta calculated in constructor
			float gDeltaDelta = this.GDeltaDelta;

			// adjust is (scan-w) of pixels array, we need (scan)
			adjust += w;

			// rgb of the 1.0 color used when the distance exceeds gradient radius
			int rgbclip = Gradient[FastGradientArraySize];

			for (int j = 0; j < h; j++)
			{
				// these values depend on the coordinates of the start of the row
				float gRel = (rowX * rowX + rowY * rowY) / RadiusSq;
				float gDelta = (2 * (A00 * rowX + A10 * rowY) / RadiusSq + gDeltaDelta / 2);

				/* Use optimized loops for any cases where gRel >= 1.
				 * We do not need to calculate sqrt(gRel) for these
				 * values since sqrt(N>=1) == (M>=1).
				 * Note that gRel follows a parabola which can only be < 1
				 * for a small region around the center on each scanline. In
				 * particular:
				 *   gDeltaDelta is always positive
				 *   gDelta is <0 until it crosses the midpoint, then >0
				 * To the left and right of that region, it will always be
				 * >=1 out to infinity, so we can process the line in 3
				 * regions:
				 *   out to the left  - quick fill until gRel < 1, updating gRel
				 *   in the heart     - slow fraction=sqrt fill while gRel < 1
				 *   out to the right - quick fill rest of scanline, ignore gRel
				 */
				int i = 0;
				// Quick fill for "out to the left"
				while (i < w && gRel >= 1.0f)
				{
					pixels[off + i] = rgbclip;
					gRel += gDelta;
					gDelta += gDeltaDelta;
					i++;
				}
				// Slow fill for "in the heart"
				while (i < w && gRel < 1.0f)
				{
					int gIndex;

					if (gRel <= 0)
					{
						gIndex = 0;
					}
					else
					{
						float fIndex = gRel * SQRT_LUT_SIZE;
						int iIndex = (int)(fIndex);
						float s0 = SqrtLut[iIndex];
						float s1 = SqrtLut[iIndex + 1] - s0;
						fIndex = s0 + (fIndex - iIndex) * s1;
						gIndex = (int)(fIndex * FastGradientArraySize);
					}

					// store the color at this point
					pixels[off + i] = Gradient[gIndex];

					// incremental calculation
					gRel += gDelta;
					gDelta += gDeltaDelta;
					i++;
				}
				// Quick fill to end of line for "out to the right"
				while (i < w)
				{
					pixels[off + i] = rgbclip;
					i++;
				}

				off += adjust;
				rowX += A01;
				rowY += A11;
			}
		}

		// SQRT_LUT_SIZE must be a power of 2 for the test above to work.
		private static readonly int SQRT_LUT_SIZE = (1 << 11);
		private static float[] SqrtLut = new float[SQRT_LUT_SIZE+1];
		static RadialGradientPaintContext()
		{
			for (int i = 0; i < SqrtLut.Length; i++)
			{
				SqrtLut[i] = (float) System.Math.Sqrt(i / ((float) SQRT_LUT_SIZE));
			}
		}

		/// <summary>
		/// Fill the raster, cycling the gradient colors when a point falls outside
		/// of the perimeter of the 100% stop circle.
		/// 
		/// This calculation first computes the intersection point of the line
		/// from the focus through the current point in the raster, and the
		/// perimeter of the gradient circle.
		/// 
		/// Then it determines the percentage distance of the current point along
		/// that line (focus is 0%, perimeter is 100%).
		/// 
		/// Equation of a circle centered at (a,b) with radius r:
		///     (x-a)^2 + (y-b)^2 = r^2
		/// Equation of a line with slope m and y-intercept b:
		///     y = mx + b
		/// Replacing y in the circle equation and solving using the quadratic
		/// formula produces the following set of equations.  Constant factors have
		/// been extracted out of the inner loop.
		/// </summary>
		private void CyclicCircularGradientFillRaster(int[] pixels, int off, int adjust, int x, int y, int w, int h)
		{
			// constant part of the C factor of the quadratic equation
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double constC = -radiusSq + (centerX * centerX) + (centerY * centerY);
			double constC = -RadiusSq + (CenterX * CenterX) + (CenterY * CenterY);

			// coefficients of the quadratic equation (Ax^2 + Bx + C = 0)
			double A, B, C;

			// slope and y-intercept of the focus-perimeter line
			double slope, yintcpt;

			// intersection with circle X,Y coordinate
			double solutionX, solutionY;

			// constant parts of X, Y coordinates
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final float constX = (a00*x) + (a01*y) + a02;
			float constX = (A00 * x) + (A01 * y) + A02;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final float constY = (a10*x) + (a11*y) + a12;
			float constY = (A10 * x) + (A11 * y) + A12;

			// constants in inner loop quadratic formula
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final float precalc2 = 2 * centerY;
			float precalc2 = 2 * CenterY;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final float precalc3 = -2 * centerX;
			float precalc3 = -2 * CenterX;

			// value between 0 and 1 specifying position in the gradient
			float g;

			// determinant of quadratic formula (should always be > 0)
			float det;

			// sq distance from the current point to focus
			float currentToFocusSq;

			// sq distance from the intersect point to focus
			float intersectToFocusSq;

			// temp variables for change in X,Y squared
			float deltaXSq, deltaYSq;

			// used to index pixels array
			int indexer = off;

			// incremental index change for pixels array
			int pixInc = w + adjust;

			// for every row
			for (int j = 0; j < h; j++)
			{

				// user space point; these are constant from column to column
				float X = (A01 * j) + constX;
				float Y = (A11 * j) + constY;

				// for every column (inner loop begins here)
				for (int i = 0; i < w; i++)
				{

					if (X == FocusX)
					{
						// special case to avoid divide by zero
						solutionX = FocusX;
						solutionY = CenterY;
						solutionY += (Y > FocusY) ? Trivial : -Trivial;
					}
					else
					{
						// slope and y-intercept of the focus-perimeter line
						slope = (Y - FocusY) / (X - FocusX);
						yintcpt = Y - (slope * X);

						// use the quadratic formula to calculate the
						// intersection point
						A = (slope * slope) + 1;
						B = precalc3 + (-2 * slope * (CenterY - yintcpt));
						C = constC + (yintcpt * (yintcpt - precalc2));

						det = (float)System.Math.Sqrt((B * B) - (4 * A * C));
						solutionX = -B;

						// choose the positive or negative root depending
						// on where the X coord lies with respect to the focus
						solutionX += (X < FocusX)? - det : det;
						solutionX = solutionX / (2 * A); // divisor
						solutionY = (slope * solutionX) + yintcpt;
					}

					// Calculate the square of the distance from the current point
					// to the focus and the square of the distance from the
					// intersection point to the focus. Want the squares so we can
					// do 1 square root after division instead of 2 before.

					deltaXSq = X - FocusX;
					deltaXSq = deltaXSq * deltaXSq;

					deltaYSq = Y - FocusY;
					deltaYSq = deltaYSq * deltaYSq;

					currentToFocusSq = deltaXSq + deltaYSq;

					deltaXSq = (float)solutionX - FocusX;
					deltaXSq = deltaXSq * deltaXSq;

					deltaYSq = (float)solutionY - FocusY;
					deltaYSq = deltaYSq * deltaYSq;

					intersectToFocusSq = deltaXSq + deltaYSq;

					// get the percentage (0-1) of the current point along the
					// focus-circumference line
					g = (float)System.Math.Sqrt(currentToFocusSq / intersectToFocusSq);

					// store the color at this point
					pixels[indexer + i] = IndexIntoGradientsArrays(g);

					// incremental change in X, Y
					X += A00;
					Y += A10;
				} //end inner loop

				indexer += pixInc;
			} //end outer loop
		}
	}

}