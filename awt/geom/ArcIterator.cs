using System;

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
	/// A utility class to iterate over the path segments of an arc
	/// through the PathIterator interface.
	/// 
	/// @author      Jim Graham
	/// </summary>
	internal class ArcIterator : PathIterator
	{
		internal double x, y, w, h, AngStRad, Increment, Cv;
		internal AffineTransform Affine;
		internal int Index;
		internal int ArcSegs;
		internal int LineSegs;

		internal ArcIterator(Arc2D a, AffineTransform at)
		{
			this.w = a.Width / 2;
			this.h = a.Height / 2;
			this.x = a.X + w;
			this.y = a.Y + h;
			this.AngStRad = -Math.ToRadians(a.GetAngleStart());
			this.Affine = at;
			double ext = -a.AngleExtent;
			if (ext >= 360.0 || ext <= -360)
			{
				ArcSegs = 4;
				this.Increment = Math.PI / 2;
				// btan(Math.PI / 2);
				this.Cv = 0.5522847498307933;
				if (ext < 0)
				{
					Increment = -Increment;
					Cv = -Cv;
				}
			}
			else
			{
				ArcSegs = (int) System.Math.Ceiling(System.Math.Abs(ext) / 90.0);
				this.Increment = Math.ToRadians(ext / ArcSegs);
				this.Cv = Btan(Increment);
				if (Cv == 0)
				{
					ArcSegs = 0;
				}
			}
			switch (a.ArcType)
			{
			case Arc2D.OPEN:
				LineSegs = 0;
				break;
			case Arc2D.CHORD:
				LineSegs = 1;
				break;
			case Arc2D.PIE:
				LineSegs = 2;
				break;
			}
			if (w < 0 || h < 0)
			{
				ArcSegs = LineSegs = -1;
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
				return Index > ArcSegs + LineSegs;
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

		/*
		 * btan computes the length (k) of the control segments at
		 * the beginning and end of a cubic bezier that approximates
		 * a segment of an arc with extent less than or equal to
		 * 90 degrees.  This length (k) will be used to generate the
		 * 2 bezier control points for such a segment.
		 *
		 *   Assumptions:
		 *     a) arc is centered on 0,0 with radius of 1.0
		 *     b) arc extent is less than 90 degrees
		 *     c) control points should preserve tangent
		 *     d) control segments should have equal length
		 *
		 *   Initial data:
		 *     start angle: ang1
		 *     end angle:   ang2 = ang1 + extent
		 *     start point: P1 = (x1, y1) = (cos(ang1), sin(ang1))
		 *     end point:   P4 = (x4, y4) = (cos(ang2), sin(ang2))
		 *
		 *   Control points:
		 *     P2 = (x2, y2)
		 *     | x2 = x1 - k * sin(ang1) = cos(ang1) - k * sin(ang1)
		 *     | y2 = y1 + k * cos(ang1) = sin(ang1) + k * cos(ang1)
		 *
		 *     P3 = (x3, y3)
		 *     | x3 = x4 + k * sin(ang2) = cos(ang2) + k * sin(ang2)
		 *     | y3 = y4 - k * cos(ang2) = sin(ang2) - k * cos(ang2)
		 *
		 * The formula for this length (k) can be found using the
		 * following derivations:
		 *
		 *   Midpoints:
		 *     a) bezier (t = 1/2)
		 *        bPm = P1 * (1-t)^3 +
		 *              3 * P2 * t * (1-t)^2 +
		 *              3 * P3 * t^2 * (1-t) +
		 *              P4 * t^3 =
		 *            = (P1 + 3P2 + 3P3 + P4)/8
		 *
		 *     b) arc
		 *        aPm = (cos((ang1 + ang2)/2), sin((ang1 + ang2)/2))
		 *
		 *   Let angb = (ang2 - ang1)/2; angb is half of the angle
		 *   between ang1 and ang2.
		 *
		 *   Solve the equation bPm == aPm
		 *
		 *     a) For xm coord:
		 *        x1 + 3*x2 + 3*x3 + x4 = 8*cos((ang1 + ang2)/2)
		 *
		 *        cos(ang1) + 3*cos(ang1) - 3*k*sin(ang1) +
		 *        3*cos(ang2) + 3*k*sin(ang2) + cos(ang2) =
		 *        = 8*cos((ang1 + ang2)/2)
		 *
		 *        4*cos(ang1) + 4*cos(ang2) + 3*k*(sin(ang2) - sin(ang1)) =
		 *        = 8*cos((ang1 + ang2)/2)
		 *
		 *        8*cos((ang1 + ang2)/2)*cos((ang2 - ang1)/2) +
		 *        6*k*sin((ang2 - ang1)/2)*cos((ang1 + ang2)/2) =
		 *        = 8*cos((ang1 + ang2)/2)
		 *
		 *        4*cos(angb) + 3*k*sin(angb) = 4
		 *
		 *        k = 4 / 3 * (1 - cos(angb)) / sin(angb)
		 *
		 *     b) For ym coord we derive the same formula.
		 *
		 * Since this formula can generate "NaN" values for small
		 * angles, we will derive a safer form that does not involve
		 * dividing by very small values:
		 *     (1 - cos(angb)) / sin(angb) =
		 *     = (1 - cos(angb))*(1 + cos(angb)) / sin(angb)*(1 + cos(angb)) =
		 *     = (1 - cos(angb)^2) / sin(angb)*(1 + cos(angb)) =
		 *     = sin(angb)^2 / sin(angb)*(1 + cos(angb)) =
		 *     = sin(angb) / (1 + cos(angb))
		 *
		 */
		private static double Btan(double increment)
		{
			increment /= 2.0;
			return 4.0 / 3.0 * System.Math.Sin(increment) / (1.0 + System.Math.Cos(increment));
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
				throw new NoSuchElementException("arc iterator out of bounds");
			}
			double angle = AngStRad;
			if (Index == 0)
			{
				coords[0] = (float)(x + System.Math.Cos(angle) * w);
				coords[1] = (float)(y + System.Math.Sin(angle) * h);
				if (Affine != null)
				{
					Affine.Transform(coords, 0, coords, 0, 1);
				}
				return PathIterator_Fields.SEG_MOVETO;
			}
			if (Index > ArcSegs)
			{
				if (Index == ArcSegs + LineSegs)
				{
					return PathIterator_Fields.SEG_CLOSE;
				}
				coords[0] = (float) x;
				coords[1] = (float) y;
				if (Affine != null)
				{
					Affine.Transform(coords, 0, coords, 0, 1);
				}
				return PathIterator_Fields.SEG_LINETO;
			}
			angle += Increment * (Index - 1);
			double relx = System.Math.Cos(angle);
			double rely = System.Math.Sin(angle);
			coords[0] = (float)(x + (relx - Cv * rely) * w);
			coords[1] = (float)(y + (rely + Cv * relx) * h);
			angle += Increment;
			relx = System.Math.Cos(angle);
			rely = System.Math.Sin(angle);
			coords[2] = (float)(x + (relx + Cv * rely) * w);
			coords[3] = (float)(y + (rely - Cv * relx) * h);
			coords[4] = (float)(x + relx * w);
			coords[5] = (float)(y + rely * h);
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
				throw new NoSuchElementException("arc iterator out of bounds");
			}
			double angle = AngStRad;
			if (Index == 0)
			{
				coords[0] = x + System.Math.Cos(angle) * w;
				coords[1] = y + System.Math.Sin(angle) * h;
				if (Affine != null)
				{
					Affine.Transform(coords, 0, coords, 0, 1);
				}
				return PathIterator_Fields.SEG_MOVETO;
			}
			if (Index > ArcSegs)
			{
				if (Index == ArcSegs + LineSegs)
				{
					return PathIterator_Fields.SEG_CLOSE;
				}
				coords[0] = x;
				coords[1] = y;
				if (Affine != null)
				{
					Affine.Transform(coords, 0, coords, 0, 1);
				}
				return PathIterator_Fields.SEG_LINETO;
			}
			angle += Increment * (Index - 1);
			double relx = System.Math.Cos(angle);
			double rely = System.Math.Sin(angle);
			coords[0] = x + (relx - Cv * rely) * w;
			coords[1] = y + (rely + Cv * relx) * h;
			angle += Increment;
			relx = System.Math.Cos(angle);
			rely = System.Math.Sin(angle);
			coords[2] = x + (relx + Cv * rely) * w;
			coords[3] = y + (rely - Cv * relx) * h;
			coords[4] = x + relx * w;
			coords[5] = y + rely * h;
			if (Affine != null)
			{
				Affine.Transform(coords, 0, coords, 0, 3);
			}
			return PathIterator_Fields.SEG_CUBICTO;
		}
	}

}