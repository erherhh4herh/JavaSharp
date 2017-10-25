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
	/// A utility class to iterate over the path segments of a rectangle
	/// through the PathIterator interface.
	/// 
	/// @author      Jim Graham
	/// </summary>
	internal class RectIterator : PathIterator
	{
		internal double x, y, w, h;
		internal AffineTransform Affine;
		internal int Index;

		internal RectIterator(Rectangle2D r, AffineTransform at)
		{
			this.x = r.X;
			this.y = r.Y;
			this.w = r.Width;
			this.h = r.Height;
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
				throw new NoSuchElementException("rect iterator out of bounds");
			}
			if (Index == 5)
			{
				return PathIterator_Fields.SEG_CLOSE;
			}
			coords[0] = (float) x;
			coords[1] = (float) y;
			if (Index == 1 || Index == 2)
			{
				coords[0] += (float) w;
			}
			if (Index == 2 || Index == 3)
			{
				coords[1] += (float) h;
			}
			if (Affine != null)
			{
				Affine.Transform(coords, 0, coords, 0, 1);
			}
			return (Index == 0 ? PathIterator_Fields.SEG_MOVETO : PathIterator_Fields.SEG_LINETO);
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
				throw new NoSuchElementException("rect iterator out of bounds");
			}
			if (Index == 5)
			{
				return PathIterator_Fields.SEG_CLOSE;
			}
			coords[0] = x;
			coords[1] = y;
			if (Index == 1 || Index == 2)
			{
				coords[0] += w;
			}
			if (Index == 2 || Index == 3)
			{
				coords[1] += h;
			}
			if (Affine != null)
			{
				Affine.Transform(coords, 0, coords, 0, 1);
			}
			return (Index == 0 ? PathIterator_Fields.SEG_MOVETO : PathIterator_Fields.SEG_LINETO);
		}
	}

}