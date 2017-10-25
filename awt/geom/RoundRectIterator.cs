using System;

/*
 * Copyright (c) 1997, Oracle and/or its affiliates. All rights reserved.
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

namespace java.awt.geom
{

	/// <summary>
	/// A utility class to iterate over the path segments of an rounded rectangle
	/// through the PathIterator interface.
	/// 
	/// @author      Jim Graham
	/// </summary>
	internal class RoundRectIterator : PathIterator
	{
		internal double x, y, w, h, Aw, Ah;
		internal AffineTransform Affine;
		internal int Index;

		internal RoundRectIterator(RoundRectangle2D rr, AffineTransform at)
		{
			this.x = rr.X;
			this.y = rr.Y;
			this.w = rr.Width;
			this.h = rr.Height;
			this.Aw = System.Math.Min(w, System.Math.Abs(rr.ArcWidth));
			this.Ah = System.Math.Min(h, System.Math.Abs(rr.ArcHeight));
			this.Affine = at;
			if (Aw < 0 || Ah < 0)
			{
				// Don't draw anything...
				Index = Ctrlpts.Length;
			}
		}

		/// <summary>
		/// Return the winding rule for determining the insideness of the
		/// path. </summary>
		/// <seealso cref= #WIND_EVEN_ODD </seealso>
		/// <seealso cref= #WIND_NON_ZERO </seealso>
		public virtual int WindingRule
		{
			get
			{
				return PathIterator_Fields.WIND_NON_ZERO;
			}
		}

		/// <summary>
		/// Tests if there are more points to read. </summary>
		/// <returns> true if there are more points to read </returns>
		public virtual bool Done
		{
			get
			{
				return Index >= Ctrlpts.Length;
			}
		}

		/// <summary>
		/// Moves the iterator to the next segment of the path forwards
		/// along the primary direction of traversal as long as there are
		/// more points in that direction.
		/// </summary>
		public virtual void Next()
		{
			Index++;
		}

		private static readonly double Angle = Math.PI / 4.0;
		private static readonly double a = 1.0 - System.Math.Cos(Angle);
		private static readonly double b = System.Math.Tan(Angle);
		private static readonly double c = System.Math.Sqrt(1.0 + b * b) - 1 + a;
		private static readonly double Cv = 4.0 / 3.0 * a * b / c;
		private static readonly double Acv = (1.0 - Cv) / 2.0;

		// For each array:
		//     4 values for each point {v0, v1, v2, v3}:
		//         point = (x + v0 * w + v1 * arcWidth,
		//                  y + v2 * h + v3 * arcHeight);
		private static double[][] Ctrlpts = new double[][] {new double[] {0.0, 0.0, 0.0, 0.5}, new double[] {0.0, 0.0, 1.0, -0.5}, new double[] {0.0, 0.0, 1.0, -Acv, 0.0, Acv, 1.0, 0.0, 0.0, 0.5, 1.0, 0.0}, new double[] {1.0, -0.5, 1.0, 0.0}, new double[] {1.0, -Acv, 1.0, 0.0, 1.0, 0.0, 1.0, -Acv, 1.0, 0.0, 1.0, -0.5}, new double[] {1.0, 0.0, 0.0, 0.5}, new double[] {1.0, 0.0, 0.0, Acv, 1.0, -Acv, 0.0, 0.0, 1.0, -0.5, 0.0, 0.0}, new double[] {0.0, 0.5, 0.0, 0.0}, new double[] {0.0, Acv, 0.0, 0.0, 0.0, 0.0, 0.0, Acv, 0.0, 0.0, 0.0, 0.5}, new double[] {}};
		private static int[] Types = new int[] {PathIterator_Fields.SEG_MOVETO, PathIterator_Fields.SEG_LINETO, PathIterator_Fields.SEG_CUBICTO, PathIterator_Fields.SEG_LINETO, PathIterator_Fields.SEG_CUBICTO, PathIterator_Fields.SEG_LINETO, PathIterator_Fields.SEG_CUBICTO, PathIterator_Fields.SEG_LINETO, PathIterator_Fields.SEG_CUBICTO, PathIterator_Fields.SEG_CLOSE};

		/// <summary>
		/// Returns the coordinates and type of the current path segment in
		/// the iteration.
		/// The return value is the path segment type:
		/// SEG_MOVETO, SEG_LINETO, SEG_QUADTO, SEG_CUBICTO, or SEG_CLOSE.
		/// A float array of length 6 must be passed in and may be used to
		/// store the coordinates of the point(s).
		/// Each point is stored as a pair of float x,y coordinates.
		/// SEG_MOVETO and SEG_LINETO types will return one point,
		/// SEG_QUADTO will return two points,
		/// SEG_CUBICTO will return 3 points
		/// and SEG_CLOSE will not return any points. </summary>
		/// <seealso cref= #SEG_MOVETO </seealso>
		/// <seealso cref= #SEG_LINETO </seealso>
		/// <seealso cref= #SEG_QUADTO </seealso>
		/// <seealso cref= #SEG_CUBICTO </seealso>
		/// <seealso cref= #SEG_CLOSE </seealso>
		public virtual int CurrentSegment(float[] coords)
		{
			if (Done)
			{
				throw new NoSuchElementException("roundrect iterator out of bounds");
			}
			double[] ctrls = Ctrlpts[Index];
			int nc = 0;
			for (int i = 0; i < ctrls.Length; i += 4)
			{
				coords[nc++] = (float)(x + ctrls[i + 0] * w + ctrls[i + 1] * Aw);
				coords[nc++] = (float)(y + ctrls[i + 2] * h + ctrls[i + 3] * Ah);
			}
			if (Affine != null)
			{
				Affine.Transform(coords, 0, coords, 0, nc / 2);
			}
			return Types[Index];
		}

		/// <summary>
		/// Returns the coordinates and type of the current path segment in
		/// the iteration.
		/// The return value is the path segment type:
		/// SEG_MOVETO, SEG_LINETO, SEG_QUADTO, SEG_CUBICTO, or SEG_CLOSE.
		/// A double array of length 6 must be passed in and may be used to
		/// store the coordinates of the point(s).
		/// Each point is stored as a pair of double x,y coordinates.
		/// SEG_MOVETO and SEG_LINETO types will return one point,
		/// SEG_QUADTO will return two points,
		/// SEG_CUBICTO will return 3 points
		/// and SEG_CLOSE will not return any points. </summary>
		/// <seealso cref= #SEG_MOVETO </seealso>
		/// <seealso cref= #SEG_LINETO </seealso>
		/// <seealso cref= #SEG_QUADTO </seealso>
		/// <seealso cref= #SEG_CUBICTO </seealso>
		/// <seealso cref= #SEG_CLOSE </seealso>
		public virtual int CurrentSegment(double[] coords)
		{
			if (Done)
			{
				throw new NoSuchElementException("roundrect iterator out of bounds");
			}
			double[] ctrls = Ctrlpts[Index];
			int nc = 0;
			for (int i = 0; i < ctrls.Length; i += 4)
			{
				coords[nc++] = (x + ctrls[i + 0] * w + ctrls[i + 1] * Aw);
				coords[nc++] = (y + ctrls[i + 2] * h + ctrls[i + 3] * Ah);
			}
			if (Affine != null)
			{
				Affine.Transform(coords, 0, coords, 0, nc / 2);
			}
			return Types[Index];
		}
	}

}