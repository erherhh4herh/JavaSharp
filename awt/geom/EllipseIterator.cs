/*
 * Copyright (c) 1997, 2003, Oracle and/or its affiliates. All rights reserved.
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
	/// A utility class to iterate over the path segments of an ellipse
	/// through the PathIterator interface.
	/// 
	/// @author      Jim Graham
	/// </summary>
	internal class EllipseIterator : PathIterator
	{
		internal double x, y, w, h;
		internal AffineTransform Affine;
		internal int Index;

		internal EllipseIterator(Ellipse2D e, AffineTransform at)
		{
			this.x = e.X;
			this.y = e.Y;
			this.w = e.Width;
			this.h = e.Height;
			this.Affine = at;
			if (w < 0 || h < 0)
			{
				Index = 6;
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
				return Index > 5;
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

		// ArcIterator.btan(Math.PI/2)
		public const double CtrlVal = 0.5522847498307933;

		/*
		 * ctrlpts contains the control points for a set of 4 cubic
		 * bezier curves that approximate a circle of radius 0.5
		 * centered at 0.5, 0.5
		 */
		private static readonly double Pcv = 0.5 + CtrlVal * 0.5;
		private static readonly double Ncv = 0.5 - CtrlVal * 0.5;
		private static double[][] Ctrlpts = new double[][] {new double[] {1.0, Pcv, Pcv, 1.0, 0.5, 1.0}, new double[] {Ncv, 1.0, 0.0, Pcv, 0.0, 0.5}, new double[] {0.0, Ncv, Ncv, 0.0, 0.5, 0.0}, new double[] {Pcv, 0.0, 1.0, Ncv, 1.0, 0.5}};

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
				throw new NoSuchElementException("ellipse iterator out of bounds");
			}
			if (Index == 5)
			{
				return PathIterator_Fields.SEG_CLOSE;
			}
			if (Index == 0)
			{
				double[] ctrls = Ctrlpts[3];
				coords[0] = (float)(x + ctrls[4] * w);
				coords[1] = (float)(y + ctrls[5] * h);
				if (Affine != null)
				{
					Affine.Transform(coords, 0, coords, 0, 1);
				}
				return PathIterator_Fields.SEG_MOVETO;
			}
			double[] ctrls = Ctrlpts[Index - 1];
			coords[0] = (float)(x + ctrls[0] * w);
			coords[1] = (float)(y + ctrls[1] * h);
			coords[2] = (float)(x + ctrls[2] * w);
			coords[3] = (float)(y + ctrls[3] * h);
			coords[4] = (float)(x + ctrls[4] * w);
			coords[5] = (float)(y + ctrls[5] * h);
			if (Affine != null)
			{
				Affine.Transform(coords, 0, coords, 0, 3);
			}
			return PathIterator_Fields.SEG_CUBICTO;
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
				throw new NoSuchElementException("ellipse iterator out of bounds");
			}
			if (Index == 5)
			{
				return PathIterator_Fields.SEG_CLOSE;
			}
			if (Index == 0)
			{
				double[] ctrls = Ctrlpts[3];
				coords[0] = x + ctrls[4] * w;
				coords[1] = y + ctrls[5] * h;
				if (Affine != null)
				{
					Affine.Transform(coords, 0, coords, 0, 1);
				}
				return PathIterator_Fields.SEG_MOVETO;
			}
			double[] ctrls = Ctrlpts[Index - 1];
			coords[0] = x + ctrls[0] * w;
			coords[1] = y + ctrls[1] * h;
			coords[2] = x + ctrls[2] * w;
			coords[3] = y + ctrls[3] * h;
			coords[4] = x + ctrls[4] * w;
			coords[5] = y + ctrls[5] * h;
			if (Affine != null)
			{
				Affine.Transform(coords, 0, coords, 0, 3);
			}
			return PathIterator_Fields.SEG_CUBICTO;
		}
	}

}