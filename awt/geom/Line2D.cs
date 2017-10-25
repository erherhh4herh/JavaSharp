using System;

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

namespace java.awt.geom
{


	/// <summary>
	/// This <code>Line2D</code> represents a line segment in {@code (x,y)}
	/// coordinate space.  This class, like all of the Java 2D API, uses a
	/// default coordinate system called <i>user space</i> in which the y-axis
	/// values increase downward and x-axis values increase to the right.  For
	/// more information on the user space coordinate system, see the
	/// <a href="https://docs.oracle.com/javase/1.3/docs/guide/2d/spec/j2d-intro.fm2.html#61857">
	/// Coordinate Systems</a> section of the Java 2D Programmer's Guide.
	/// <para>
	/// This class is only the abstract superclass for all objects that
	/// store a 2D line segment.
	/// The actual storage representation of the coordinates is left to
	/// the subclass.
	/// 
	/// @author      Jim Graham
	/// @since 1.2
	/// </para>
	/// </summary>
	public abstract class Line2D : Shape, Cloneable
	{
		public abstract java.awt.geom.Rectangle2D Bounds2D {get;}

		/// <summary>
		/// A line segment specified with float coordinates.
		/// @since 1.2
		/// </summary>
		[Serializable]
		public class Float : Line2D
		{
			/// <summary>
			/// The X coordinate of the start point of the line segment.
			/// @since 1.2
			/// @serial
			/// </summary>
			public float X1_Renamed;

			/// <summary>
			/// The Y coordinate of the start point of the line segment.
			/// @since 1.2
			/// @serial
			/// </summary>
			public float Y1_Renamed;

			/// <summary>
			/// The X coordinate of the end point of the line segment.
			/// @since 1.2
			/// @serial
			/// </summary>
			public float X2_Renamed;

			/// <summary>
			/// The Y coordinate of the end point of the line segment.
			/// @since 1.2
			/// @serial
			/// </summary>
			public float Y2_Renamed;

			/// <summary>
			/// Constructs and initializes a Line with coordinates (0, 0) &rarr; (0, 0).
			/// @since 1.2
			/// </summary>
			public Float()
			{
			}

			/// <summary>
			/// Constructs and initializes a Line from the specified coordinates. </summary>
			/// <param name="x1"> the X coordinate of the start point </param>
			/// <param name="y1"> the Y coordinate of the start point </param>
			/// <param name="x2"> the X coordinate of the end point </param>
			/// <param name="y2"> the Y coordinate of the end point
			/// @since 1.2 </param>
			public Float(float x1, float y1, float x2, float y2)
			{
				SetLine(x1, y1, x2, y2);
			}

			/// <summary>
			/// Constructs and initializes a <code>Line2D</code> from the
			/// specified <code>Point2D</code> objects. </summary>
			/// <param name="p1"> the start <code>Point2D</code> of this line segment </param>
			/// <param name="p2"> the end <code>Point2D</code> of this line segment
			/// @since 1.2 </param>
			public Float(Point2D p1, Point2D p2)
			{
				SetLine(p1, p2);
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.2
			/// </summary>
			public override double X1
			{
				get
				{
					return (double) X1_Renamed;
				}
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.2
			/// </summary>
			public override double Y1
			{
				get
				{
					return (double) Y1_Renamed;
				}
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.2
			/// </summary>
			public override Point2D P1
			{
				get
				{
					return new Point2D.Float(X1_Renamed, Y1_Renamed);
				}
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.2
			/// </summary>
			public override double X2
			{
				get
				{
					return (double) X2_Renamed;
				}
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.2
			/// </summary>
			public override double Y2
			{
				get
				{
					return (double) Y2_Renamed;
				}
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.2
			/// </summary>
			public override Point2D P2
			{
				get
				{
					return new Point2D.Float(X2_Renamed, Y2_Renamed);
				}
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.2
			/// </summary>
			public override void SetLine(double x1, double y1, double x2, double y2)
			{
				this.X1_Renamed = (float) x1;
				this.Y1_Renamed = (float) y1;
				this.X2_Renamed = (float) x2;
				this.Y2_Renamed = (float) y2;
			}

			/// <summary>
			/// Sets the location of the end points of this <code>Line2D</code>
			/// to the specified float coordinates. </summary>
			/// <param name="x1"> the X coordinate of the start point </param>
			/// <param name="y1"> the Y coordinate of the start point </param>
			/// <param name="x2"> the X coordinate of the end point </param>
			/// <param name="y2"> the Y coordinate of the end point
			/// @since 1.2 </param>
			public virtual void SetLine(float x1, float y1, float x2, float y2)
			{
				this.X1_Renamed = x1;
				this.Y1_Renamed = y1;
				this.X2_Renamed = x2;
				this.Y2_Renamed = y2;
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.2
			/// </summary>
			public override Rectangle2D Bounds2D
			{
				get
				{
					float x, y, w, h;
					if (X1_Renamed < X2_Renamed)
					{
						x = X1_Renamed;
						w = X2_Renamed - X1_Renamed;
					}
					else
					{
						x = X2_Renamed;
						w = X1_Renamed - X2_Renamed;
					}
					if (Y1_Renamed < Y2_Renamed)
					{
						y = Y1_Renamed;
						h = Y2_Renamed - Y1_Renamed;
					}
					else
					{
						y = Y2_Renamed;
						h = Y1_Renamed - Y2_Renamed;
					}
					return new Rectangle2D.Float(x, y, w, h);
				}
			}

			/*
			 * JDK 1.6 serialVersionUID
			 */
			internal const long SerialVersionUID = 6161772511649436349L;
		}

		/// <summary>
		/// A line segment specified with double coordinates.
		/// @since 1.2
		/// </summary>
		[Serializable]
		public class Double : Line2D
		{
			/// <summary>
			/// The X coordinate of the start point of the line segment.
			/// @since 1.2
			/// @serial
			/// </summary>
			public double X1_Renamed;

			/// <summary>
			/// The Y coordinate of the start point of the line segment.
			/// @since 1.2
			/// @serial
			/// </summary>
			public double Y1_Renamed;

			/// <summary>
			/// The X coordinate of the end point of the line segment.
			/// @since 1.2
			/// @serial
			/// </summary>
			public double X2_Renamed;

			/// <summary>
			/// The Y coordinate of the end point of the line segment.
			/// @since 1.2
			/// @serial
			/// </summary>
			public double Y2_Renamed;

			/// <summary>
			/// Constructs and initializes a Line with coordinates (0, 0) &rarr; (0, 0).
			/// @since 1.2
			/// </summary>
			public Double()
			{
			}

			/// <summary>
			/// Constructs and initializes a <code>Line2D</code> from the
			/// specified coordinates. </summary>
			/// <param name="x1"> the X coordinate of the start point </param>
			/// <param name="y1"> the Y coordinate of the start point </param>
			/// <param name="x2"> the X coordinate of the end point </param>
			/// <param name="y2"> the Y coordinate of the end point
			/// @since 1.2 </param>
			public Double(double x1, double y1, double x2, double y2)
			{
				SetLine(x1, y1, x2, y2);
			}

			/// <summary>
			/// Constructs and initializes a <code>Line2D</code> from the
			/// specified <code>Point2D</code> objects. </summary>
			/// <param name="p1"> the start <code>Point2D</code> of this line segment </param>
			/// <param name="p2"> the end <code>Point2D</code> of this line segment
			/// @since 1.2 </param>
			public Double(Point2D p1, Point2D p2)
			{
				SetLine(p1, p2);
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.2
			/// </summary>
			public override double X1
			{
				get
				{
					return X1_Renamed;
				}
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.2
			/// </summary>
			public override double Y1
			{
				get
				{
					return Y1_Renamed;
				}
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.2
			/// </summary>
			public override Point2D P1
			{
				get
				{
					return new Point2D.Double(X1_Renamed, Y1_Renamed);
				}
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.2
			/// </summary>
			public override double X2
			{
				get
				{
					return X2_Renamed;
				}
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.2
			/// </summary>
			public override double Y2
			{
				get
				{
					return Y2_Renamed;
				}
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.2
			/// </summary>
			public override Point2D P2
			{
				get
				{
					return new Point2D.Double(X2_Renamed, Y2_Renamed);
				}
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.2
			/// </summary>
			public override void SetLine(double x1, double y1, double x2, double y2)
			{
				this.X1_Renamed = x1;
				this.Y1_Renamed = y1;
				this.X2_Renamed = x2;
				this.Y2_Renamed = y2;
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.2
			/// </summary>
			public override Rectangle2D Bounds2D
			{
				get
				{
					double x, y, w, h;
					if (X1_Renamed < X2_Renamed)
					{
						x = X1_Renamed;
						w = X2_Renamed - X1_Renamed;
					}
					else
					{
						x = X2_Renamed;
						w = X1_Renamed - X2_Renamed;
					}
					if (Y1_Renamed < Y2_Renamed)
					{
						y = Y1_Renamed;
						h = Y2_Renamed - Y1_Renamed;
					}
					else
					{
						y = Y2_Renamed;
						h = Y1_Renamed - Y2_Renamed;
					}
					return new Rectangle2D.Double(x, y, w, h);
				}
			}

			/*
			 * JDK 1.6 serialVersionUID
			 */
			internal const long SerialVersionUID = 7979627399746467499L;
		}

		/// <summary>
		/// This is an abstract class that cannot be instantiated directly.
		/// Type-specific implementation subclasses are available for
		/// instantiation and provide a number of formats for storing
		/// the information necessary to satisfy the various accessory
		/// methods below.
		/// </summary>
		/// <seealso cref= java.awt.geom.Line2D.Float </seealso>
		/// <seealso cref= java.awt.geom.Line2D.Double
		/// @since 1.2 </seealso>
		protected internal Line2D()
		{
		}

		/// <summary>
		/// Returns the X coordinate of the start point in double precision. </summary>
		/// <returns> the X coordinate of the start point of this
		///         {@code Line2D} object.
		/// @since 1.2 </returns>
		public abstract double X1 {get;}

		/// <summary>
		/// Returns the Y coordinate of the start point in double precision. </summary>
		/// <returns> the Y coordinate of the start point of this
		///         {@code Line2D} object.
		/// @since 1.2 </returns>
		public abstract double Y1 {get;}

		/// <summary>
		/// Returns the start <code>Point2D</code> of this <code>Line2D</code>. </summary>
		/// <returns> the start <code>Point2D</code> of this <code>Line2D</code>.
		/// @since 1.2 </returns>
		public abstract Point2D P1 {get;}

		/// <summary>
		/// Returns the X coordinate of the end point in double precision. </summary>
		/// <returns> the X coordinate of the end point of this
		///         {@code Line2D} object.
		/// @since 1.2 </returns>
		public abstract double X2 {get;}

		/// <summary>
		/// Returns the Y coordinate of the end point in double precision. </summary>
		/// <returns> the Y coordinate of the end point of this
		///         {@code Line2D} object.
		/// @since 1.2 </returns>
		public abstract double Y2 {get;}

		/// <summary>
		/// Returns the end <code>Point2D</code> of this <code>Line2D</code>. </summary>
		/// <returns> the end <code>Point2D</code> of this <code>Line2D</code>.
		/// @since 1.2 </returns>
		public abstract Point2D P2 {get;}

		/// <summary>
		/// Sets the location of the end points of this <code>Line2D</code> to
		/// the specified double coordinates. </summary>
		/// <param name="x1"> the X coordinate of the start point </param>
		/// <param name="y1"> the Y coordinate of the start point </param>
		/// <param name="x2"> the X coordinate of the end point </param>
		/// <param name="y2"> the Y coordinate of the end point
		/// @since 1.2 </param>
		public abstract void SetLine(double x1, double y1, double x2, double y2);

		/// <summary>
		/// Sets the location of the end points of this <code>Line2D</code> to
		/// the specified <code>Point2D</code> coordinates. </summary>
		/// <param name="p1"> the start <code>Point2D</code> of the line segment </param>
		/// <param name="p2"> the end <code>Point2D</code> of the line segment
		/// @since 1.2 </param>
		public virtual void SetLine(Point2D p1, Point2D p2)
		{
			SetLine(p1.X, p1.Y, p2.X, p2.Y);
		}

		/// <summary>
		/// Sets the location of the end points of this <code>Line2D</code> to
		/// the same as those end points of the specified <code>Line2D</code>. </summary>
		/// <param name="l"> the specified <code>Line2D</code>
		/// @since 1.2 </param>
		public virtual Line2D Line
		{
			set
			{
				SetLine(value.X1, value.Y1, value.X2, value.Y2);
			}
		}

		/// <summary>
		/// Returns an indicator of where the specified point
		/// {@code (px,py)} lies with respect to the line segment from
		/// {@code (x1,y1)} to {@code (x2,y2)}.
		/// The return value can be either 1, -1, or 0 and indicates
		/// in which direction the specified line must pivot around its
		/// first end point, {@code (x1,y1)}, in order to point at the
		/// specified point {@code (px,py)}.
		/// <para>A return value of 1 indicates that the line segment must
		/// turn in the direction that takes the positive X axis towards
		/// the negative Y axis.  In the default coordinate system used by
		/// Java 2D, this direction is counterclockwise.
		/// </para>
		/// <para>A return value of -1 indicates that the line segment must
		/// turn in the direction that takes the positive X axis towards
		/// the positive Y axis.  In the default coordinate system, this
		/// direction is clockwise.
		/// </para>
		/// <para>A return value of 0 indicates that the point lies
		/// exactly on the line segment.  Note that an indicator value
		/// of 0 is rare and not useful for determining collinearity
		/// because of floating point rounding issues.
		/// </para>
		/// <para>If the point is colinear with the line segment, but
		/// not between the end points, then the value will be -1 if the point
		/// lies "beyond {@code (x1,y1)}" or 1 if the point lies
		/// "beyond {@code (x2,y2)}".
		/// 
		/// </para>
		/// </summary>
		/// <param name="x1"> the X coordinate of the start point of the
		///           specified line segment </param>
		/// <param name="y1"> the Y coordinate of the start point of the
		///           specified line segment </param>
		/// <param name="x2"> the X coordinate of the end point of the
		///           specified line segment </param>
		/// <param name="y2"> the Y coordinate of the end point of the
		///           specified line segment </param>
		/// <param name="px"> the X coordinate of the specified point to be
		///           compared with the specified line segment </param>
		/// <param name="py"> the Y coordinate of the specified point to be
		///           compared with the specified line segment </param>
		/// <returns> an integer that indicates the position of the third specified
		///                  coordinates with respect to the line segment formed
		///                  by the first two specified coordinates.
		/// @since 1.2 </returns>
		public static int RelativeCCW(double x1, double y1, double x2, double y2, double px, double py)
		{
			x2 -= x1;
			y2 -= y1;
			px -= x1;
			py -= y1;
			double ccw = px * y2 - py * x2;
			if (ccw == 0.0)
			{
				// The point is colinear, classify based on which side of
				// the segment the point falls on.  We can calculate a
				// relative value using the projection of px,py onto the
				// segment - a negative value indicates the point projects
				// outside of the segment in the direction of the particular
				// endpoint used as the origin for the projection.
				ccw = px * x2 + py * y2;
				if (ccw > 0.0)
				{
					// Reverse the projection to be relative to the original x2,y2
					// x2 and y2 are simply negated.
					// px and py need to have (x2 - x1) or (y2 - y1) subtracted
					//    from them (based on the original values)
					// Since we really want to get a positive answer when the
					//    point is "beyond (x2,y2)", then we want to calculate
					//    the inverse anyway - thus we leave x2 & y2 negated.
					px -= x2;
					py -= y2;
					ccw = px * x2 + py * y2;
					if (ccw < 0.0)
					{
						ccw = 0.0;
					}
				}
			}
			return (ccw < 0.0) ? - 1 : ((ccw > 0.0) ? 1 : 0);
		}

		/// <summary>
		/// Returns an indicator of where the specified point
		/// {@code (px,py)} lies with respect to this line segment.
		/// See the method comments of
		/// <seealso cref="#relativeCCW(double, double, double, double, double, double)"/>
		/// to interpret the return value. </summary>
		/// <param name="px"> the X coordinate of the specified point
		///           to be compared with this <code>Line2D</code> </param>
		/// <param name="py"> the Y coordinate of the specified point
		///           to be compared with this <code>Line2D</code> </param>
		/// <returns> an integer that indicates the position of the specified
		///         coordinates with respect to this <code>Line2D</code> </returns>
		/// <seealso cref= #relativeCCW(double, double, double, double, double, double)
		/// @since 1.2 </seealso>
		public virtual int RelativeCCW(double px, double py)
		{
			return RelativeCCW(X1, Y1, X2, Y2, px, py);
		}

		/// <summary>
		/// Returns an indicator of where the specified <code>Point2D</code>
		/// lies with respect to this line segment.
		/// See the method comments of
		/// <seealso cref="#relativeCCW(double, double, double, double, double, double)"/>
		/// to interpret the return value. </summary>
		/// <param name="p"> the specified <code>Point2D</code> to be compared
		///          with this <code>Line2D</code> </param>
		/// <returns> an integer that indicates the position of the specified
		///         <code>Point2D</code> with respect to this <code>Line2D</code> </returns>
		/// <seealso cref= #relativeCCW(double, double, double, double, double, double)
		/// @since 1.2 </seealso>
		public virtual int RelativeCCW(Point2D p)
		{
			return RelativeCCW(X1, Y1, X2, Y2, p.X, p.Y);
		}

		/// <summary>
		/// Tests if the line segment from {@code (x1,y1)} to
		/// {@code (x2,y2)} intersects the line segment from {@code (x3,y3)}
		/// to {@code (x4,y4)}.
		/// </summary>
		/// <param name="x1"> the X coordinate of the start point of the first
		///           specified line segment </param>
		/// <param name="y1"> the Y coordinate of the start point of the first
		///           specified line segment </param>
		/// <param name="x2"> the X coordinate of the end point of the first
		///           specified line segment </param>
		/// <param name="y2"> the Y coordinate of the end point of the first
		///           specified line segment </param>
		/// <param name="x3"> the X coordinate of the start point of the second
		///           specified line segment </param>
		/// <param name="y3"> the Y coordinate of the start point of the second
		///           specified line segment </param>
		/// <param name="x4"> the X coordinate of the end point of the second
		///           specified line segment </param>
		/// <param name="y4"> the Y coordinate of the end point of the second
		///           specified line segment </param>
		/// <returns> <code>true</code> if the first specified line segment
		///                  and the second specified line segment intersect
		///                  each other; <code>false</code> otherwise.
		/// @since 1.2 </returns>
		public static bool LinesIntersect(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4)
		{
			return ((RelativeCCW(x1, y1, x2, y2, x3, y3) * RelativeCCW(x1, y1, x2, y2, x4, y4) <= 0) && (RelativeCCW(x3, y3, x4, y4, x1, y1) * RelativeCCW(x3, y3, x4, y4, x2, y2) <= 0));
		}

		/// <summary>
		/// Tests if the line segment from {@code (x1,y1)} to
		/// {@code (x2,y2)} intersects this line segment.
		/// </summary>
		/// <param name="x1"> the X coordinate of the start point of the
		///           specified line segment </param>
		/// <param name="y1"> the Y coordinate of the start point of the
		///           specified line segment </param>
		/// <param name="x2"> the X coordinate of the end point of the
		///           specified line segment </param>
		/// <param name="y2"> the Y coordinate of the end point of the
		///           specified line segment </param>
		/// <returns> {@code <true>} if this line segment and the specified line segment
		///                  intersect each other; <code>false</code> otherwise.
		/// @since 1.2 </returns>
		public virtual bool IntersectsLine(double x1, double y1, double x2, double y2)
		{
			return LinesIntersect(x1, y1, x2, y2, X1, Y1, X2, Y2);
		}

		/// <summary>
		/// Tests if the specified line segment intersects this line segment. </summary>
		/// <param name="l"> the specified <code>Line2D</code> </param>
		/// <returns> <code>true</code> if this line segment and the specified line
		///                  segment intersect each other;
		///                  <code>false</code> otherwise.
		/// @since 1.2 </returns>
		public virtual bool IntersectsLine(Line2D l)
		{
			return LinesIntersect(l.X1, l.Y1, l.X2, l.Y2, X1, Y1, X2, Y2);
		}

		/// <summary>
		/// Returns the square of the distance from a point to a line segment.
		/// The distance measured is the distance between the specified
		/// point and the closest point between the specified end points.
		/// If the specified point intersects the line segment in between the
		/// end points, this method returns 0.0.
		/// </summary>
		/// <param name="x1"> the X coordinate of the start point of the
		///           specified line segment </param>
		/// <param name="y1"> the Y coordinate of the start point of the
		///           specified line segment </param>
		/// <param name="x2"> the X coordinate of the end point of the
		///           specified line segment </param>
		/// <param name="y2"> the Y coordinate of the end point of the
		///           specified line segment </param>
		/// <param name="px"> the X coordinate of the specified point being
		///           measured against the specified line segment </param>
		/// <param name="py"> the Y coordinate of the specified point being
		///           measured against the specified line segment </param>
		/// <returns> a double value that is the square of the distance from the
		///                  specified point to the specified line segment. </returns>
		/// <seealso cref= #ptLineDistSq(double, double, double, double, double, double)
		/// @since 1.2 </seealso>
		public static double PtSegDistSq(double x1, double y1, double x2, double y2, double px, double py)
		{
			// Adjust vectors relative to x1,y1
			// x2,y2 becomes relative vector from x1,y1 to end of segment
			x2 -= x1;
			y2 -= y1;
			// px,py becomes relative vector from x1,y1 to test point
			px -= x1;
			py -= y1;
			double dotprod = px * x2 + py * y2;
			double projlenSq;
			if (dotprod <= 0.0)
			{
				// px,py is on the side of x1,y1 away from x2,y2
				// distance to segment is length of px,py vector
				// "length of its (clipped) projection" is now 0.0
				projlenSq = 0.0;
			}
			else
			{
				// switch to backwards vectors relative to x2,y2
				// x2,y2 are already the negative of x1,y1=>x2,y2
				// to get px,py to be the negative of px,py=>x2,y2
				// the dot product of two negated vectors is the same
				// as the dot product of the two normal vectors
				px = x2 - px;
				py = y2 - py;
				dotprod = px * x2 + py * y2;
				if (dotprod <= 0.0)
				{
					// px,py is on the side of x2,y2 away from x1,y1
					// distance to segment is length of (backwards) px,py vector
					// "length of its (clipped) projection" is now 0.0
					projlenSq = 0.0;
				}
				else
				{
					// px,py is between x1,y1 and x2,y2
					// dotprod is the length of the px,py vector
					// projected on the x2,y2=>x1,y1 vector times the
					// length of the x2,y2=>x1,y1 vector
					projlenSq = dotprod * dotprod / (x2 * x2 + y2 * y2);
				}
			}
			// Distance to line is now the length of the relative point
			// vector minus the length of its projection onto the line
			// (which is zero if the projection falls outside the range
			//  of the line segment).
			double lenSq = px * px + py * py - projlenSq;
			if (lenSq < 0)
			{
				lenSq = 0;
			}
			return lenSq;
		}

		/// <summary>
		/// Returns the distance from a point to a line segment.
		/// The distance measured is the distance between the specified
		/// point and the closest point between the specified end points.
		/// If the specified point intersects the line segment in between the
		/// end points, this method returns 0.0.
		/// </summary>
		/// <param name="x1"> the X coordinate of the start point of the
		///           specified line segment </param>
		/// <param name="y1"> the Y coordinate of the start point of the
		///           specified line segment </param>
		/// <param name="x2"> the X coordinate of the end point of the
		///           specified line segment </param>
		/// <param name="y2"> the Y coordinate of the end point of the
		///           specified line segment </param>
		/// <param name="px"> the X coordinate of the specified point being
		///           measured against the specified line segment </param>
		/// <param name="py"> the Y coordinate of the specified point being
		///           measured against the specified line segment </param>
		/// <returns> a double value that is the distance from the specified point
		///                          to the specified line segment. </returns>
		/// <seealso cref= #ptLineDist(double, double, double, double, double, double)
		/// @since 1.2 </seealso>
		public static double PtSegDist(double x1, double y1, double x2, double y2, double px, double py)
		{
			return System.Math.Sqrt(PtSegDistSq(x1, y1, x2, y2, px, py));
		}

		/// <summary>
		/// Returns the square of the distance from a point to this line segment.
		/// The distance measured is the distance between the specified
		/// point and the closest point between the current line's end points.
		/// If the specified point intersects the line segment in between the
		/// end points, this method returns 0.0.
		/// </summary>
		/// <param name="px"> the X coordinate of the specified point being
		///           measured against this line segment </param>
		/// <param name="py"> the Y coordinate of the specified point being
		///           measured against this line segment </param>
		/// <returns> a double value that is the square of the distance from the
		///                  specified point to the current line segment. </returns>
		/// <seealso cref= #ptLineDistSq(double, double)
		/// @since 1.2 </seealso>
		public virtual double PtSegDistSq(double px, double py)
		{
			return PtSegDistSq(X1, Y1, X2, Y2, px, py);
		}

		/// <summary>
		/// Returns the square of the distance from a <code>Point2D</code> to
		/// this line segment.
		/// The distance measured is the distance between the specified
		/// point and the closest point between the current line's end points.
		/// If the specified point intersects the line segment in between the
		/// end points, this method returns 0.0. </summary>
		/// <param name="pt"> the specified <code>Point2D</code> being measured against
		///           this line segment. </param>
		/// <returns> a double value that is the square of the distance from the
		///                  specified <code>Point2D</code> to the current
		///                  line segment. </returns>
		/// <seealso cref= #ptLineDistSq(Point2D)
		/// @since 1.2 </seealso>
		public virtual double PtSegDistSq(Point2D pt)
		{
			return PtSegDistSq(X1, Y1, X2, Y2, pt.X, pt.Y);
		}

		/// <summary>
		/// Returns the distance from a point to this line segment.
		/// The distance measured is the distance between the specified
		/// point and the closest point between the current line's end points.
		/// If the specified point intersects the line segment in between the
		/// end points, this method returns 0.0.
		/// </summary>
		/// <param name="px"> the X coordinate of the specified point being
		///           measured against this line segment </param>
		/// <param name="py"> the Y coordinate of the specified point being
		///           measured against this line segment </param>
		/// <returns> a double value that is the distance from the specified
		///                  point to the current line segment. </returns>
		/// <seealso cref= #ptLineDist(double, double)
		/// @since 1.2 </seealso>
		public virtual double PtSegDist(double px, double py)
		{
			return PtSegDist(X1, Y1, X2, Y2, px, py);
		}

		/// <summary>
		/// Returns the distance from a <code>Point2D</code> to this line
		/// segment.
		/// The distance measured is the distance between the specified
		/// point and the closest point between the current line's end points.
		/// If the specified point intersects the line segment in between the
		/// end points, this method returns 0.0. </summary>
		/// <param name="pt"> the specified <code>Point2D</code> being measured
		///          against this line segment </param>
		/// <returns> a double value that is the distance from the specified
		///                          <code>Point2D</code> to the current line
		///                          segment. </returns>
		/// <seealso cref= #ptLineDist(Point2D)
		/// @since 1.2 </seealso>
		public virtual double PtSegDist(Point2D pt)
		{
			return PtSegDist(X1, Y1, X2, Y2, pt.X, pt.Y);
		}

		/// <summary>
		/// Returns the square of the distance from a point to a line.
		/// The distance measured is the distance between the specified
		/// point and the closest point on the infinitely-extended line
		/// defined by the specified coordinates.  If the specified point
		/// intersects the line, this method returns 0.0.
		/// </summary>
		/// <param name="x1"> the X coordinate of the start point of the specified line </param>
		/// <param name="y1"> the Y coordinate of the start point of the specified line </param>
		/// <param name="x2"> the X coordinate of the end point of the specified line </param>
		/// <param name="y2"> the Y coordinate of the end point of the specified line </param>
		/// <param name="px"> the X coordinate of the specified point being
		///           measured against the specified line </param>
		/// <param name="py"> the Y coordinate of the specified point being
		///           measured against the specified line </param>
		/// <returns> a double value that is the square of the distance from the
		///                  specified point to the specified line. </returns>
		/// <seealso cref= #ptSegDistSq(double, double, double, double, double, double)
		/// @since 1.2 </seealso>
		public static double PtLineDistSq(double x1, double y1, double x2, double y2, double px, double py)
		{
			// Adjust vectors relative to x1,y1
			// x2,y2 becomes relative vector from x1,y1 to end of segment
			x2 -= x1;
			y2 -= y1;
			// px,py becomes relative vector from x1,y1 to test point
			px -= x1;
			py -= y1;
			double dotprod = px * x2 + py * y2;
			// dotprod is the length of the px,py vector
			// projected on the x1,y1=>x2,y2 vector times the
			// length of the x1,y1=>x2,y2 vector
			double projlenSq = dotprod * dotprod / (x2 * x2 + y2 * y2);
			// Distance to line is now the length of the relative point
			// vector minus the length of its projection onto the line
			double lenSq = px * px + py * py - projlenSq;
			if (lenSq < 0)
			{
				lenSq = 0;
			}
			return lenSq;
		}

		/// <summary>
		/// Returns the distance from a point to a line.
		/// The distance measured is the distance between the specified
		/// point and the closest point on the infinitely-extended line
		/// defined by the specified coordinates.  If the specified point
		/// intersects the line, this method returns 0.0.
		/// </summary>
		/// <param name="x1"> the X coordinate of the start point of the specified line </param>
		/// <param name="y1"> the Y coordinate of the start point of the specified line </param>
		/// <param name="x2"> the X coordinate of the end point of the specified line </param>
		/// <param name="y2"> the Y coordinate of the end point of the specified line </param>
		/// <param name="px"> the X coordinate of the specified point being
		///           measured against the specified line </param>
		/// <param name="py"> the Y coordinate of the specified point being
		///           measured against the specified line </param>
		/// <returns> a double value that is the distance from the specified
		///                   point to the specified line. </returns>
		/// <seealso cref= #ptSegDist(double, double, double, double, double, double)
		/// @since 1.2 </seealso>
		public static double PtLineDist(double x1, double y1, double x2, double y2, double px, double py)
		{
			return System.Math.Sqrt(PtLineDistSq(x1, y1, x2, y2, px, py));
		}

		/// <summary>
		/// Returns the square of the distance from a point to this line.
		/// The distance measured is the distance between the specified
		/// point and the closest point on the infinitely-extended line
		/// defined by this <code>Line2D</code>.  If the specified point
		/// intersects the line, this method returns 0.0.
		/// </summary>
		/// <param name="px"> the X coordinate of the specified point being
		///           measured against this line </param>
		/// <param name="py"> the Y coordinate of the specified point being
		///           measured against this line </param>
		/// <returns> a double value that is the square of the distance from a
		///                  specified point to the current line. </returns>
		/// <seealso cref= #ptSegDistSq(double, double)
		/// @since 1.2 </seealso>
		public virtual double PtLineDistSq(double px, double py)
		{
			return PtLineDistSq(X1, Y1, X2, Y2, px, py);
		}

		/// <summary>
		/// Returns the square of the distance from a specified
		/// <code>Point2D</code> to this line.
		/// The distance measured is the distance between the specified
		/// point and the closest point on the infinitely-extended line
		/// defined by this <code>Line2D</code>.  If the specified point
		/// intersects the line, this method returns 0.0. </summary>
		/// <param name="pt"> the specified <code>Point2D</code> being measured
		///           against this line </param>
		/// <returns> a double value that is the square of the distance from a
		///                  specified <code>Point2D</code> to the current
		///                  line. </returns>
		/// <seealso cref= #ptSegDistSq(Point2D)
		/// @since 1.2 </seealso>
		public virtual double PtLineDistSq(Point2D pt)
		{
			return PtLineDistSq(X1, Y1, X2, Y2, pt.X, pt.Y);
		}

		/// <summary>
		/// Returns the distance from a point to this line.
		/// The distance measured is the distance between the specified
		/// point and the closest point on the infinitely-extended line
		/// defined by this <code>Line2D</code>.  If the specified point
		/// intersects the line, this method returns 0.0.
		/// </summary>
		/// <param name="px"> the X coordinate of the specified point being
		///           measured against this line </param>
		/// <param name="py"> the Y coordinate of the specified point being
		///           measured against this line </param>
		/// <returns> a double value that is the distance from a specified point
		///                  to the current line. </returns>
		/// <seealso cref= #ptSegDist(double, double)
		/// @since 1.2 </seealso>
		public virtual double PtLineDist(double px, double py)
		{
			return PtLineDist(X1, Y1, X2, Y2, px, py);
		}

		/// <summary>
		/// Returns the distance from a <code>Point2D</code> to this line.
		/// The distance measured is the distance between the specified
		/// point and the closest point on the infinitely-extended line
		/// defined by this <code>Line2D</code>.  If the specified point
		/// intersects the line, this method returns 0.0. </summary>
		/// <param name="pt"> the specified <code>Point2D</code> being measured </param>
		/// <returns> a double value that is the distance from a specified
		///                  <code>Point2D</code> to the current line. </returns>
		/// <seealso cref= #ptSegDist(Point2D)
		/// @since 1.2 </seealso>
		public virtual double PtLineDist(Point2D pt)
		{
			return PtLineDist(X1, Y1, X2, Y2, pt.X, pt.Y);
		}

		/// <summary>
		/// Tests if a specified coordinate is inside the boundary of this
		/// <code>Line2D</code>.  This method is required to implement the
		/// <seealso cref="Shape"/> interface, but in the case of <code>Line2D</code>
		/// objects it always returns <code>false</code> since a line contains
		/// no area. </summary>
		/// <param name="x"> the X coordinate of the specified point to be tested </param>
		/// <param name="y"> the Y coordinate of the specified point to be tested </param>
		/// <returns> <code>false</code> because a <code>Line2D</code> contains
		/// no area.
		/// @since 1.2 </returns>
		public virtual bool Contains(double x, double y)
		{
			return false;
		}

		/// <summary>
		/// Tests if a given <code>Point2D</code> is inside the boundary of
		/// this <code>Line2D</code>.
		/// This method is required to implement the <seealso cref="Shape"/> interface,
		/// but in the case of <code>Line2D</code> objects it always returns
		/// <code>false</code> since a line contains no area. </summary>
		/// <param name="p"> the specified <code>Point2D</code> to be tested </param>
		/// <returns> <code>false</code> because a <code>Line2D</code> contains
		/// no area.
		/// @since 1.2 </returns>
		public virtual bool Contains(Point2D p)
		{
			return false;
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 1.2
		/// </summary>
		public virtual bool Intersects(double x, double y, double w, double h)
		{
			return Intersects(new Rectangle2D.Double(x, y, w, h));
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 1.2
		/// </summary>
		public virtual bool Intersects(Rectangle2D r)
		{
			return r.IntersectsLine(X1, Y1, X2, Y2);
		}

		/// <summary>
		/// Tests if the interior of this <code>Line2D</code> entirely contains
		/// the specified set of rectangular coordinates.
		/// This method is required to implement the <code>Shape</code> interface,
		/// but in the case of <code>Line2D</code> objects it always returns
		/// false since a line contains no area. </summary>
		/// <param name="x"> the X coordinate of the upper-left corner of the
		///          specified rectangular area </param>
		/// <param name="y"> the Y coordinate of the upper-left corner of the
		///          specified rectangular area </param>
		/// <param name="w"> the width of the specified rectangular area </param>
		/// <param name="h"> the height of the specified rectangular area </param>
		/// <returns> <code>false</code> because a <code>Line2D</code> contains
		/// no area.
		/// @since 1.2 </returns>
		public virtual bool Contains(double x, double y, double w, double h)
		{
			return false;
		}

		/// <summary>
		/// Tests if the interior of this <code>Line2D</code> entirely contains
		/// the specified <code>Rectangle2D</code>.
		/// This method is required to implement the <code>Shape</code> interface,
		/// but in the case of <code>Line2D</code> objects it always returns
		/// <code>false</code> since a line contains no area. </summary>
		/// <param name="r"> the specified <code>Rectangle2D</code> to be tested </param>
		/// <returns> <code>false</code> because a <code>Line2D</code> contains
		/// no area.
		/// @since 1.2 </returns>
		public virtual bool Contains(Rectangle2D r)
		{
			return false;
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 1.2
		/// </summary>
		public virtual Rectangle Bounds
		{
			get
			{
				return Bounds2D.Bounds;
			}
		}

		/// <summary>
		/// Returns an iteration object that defines the boundary of this
		/// <code>Line2D</code>.
		/// The iterator for this class is not multi-threaded safe,
		/// which means that this <code>Line2D</code> class does not
		/// guarantee that modifications to the geometry of this
		/// <code>Line2D</code> object do not affect any iterations of that
		/// geometry that are already in process. </summary>
		/// <param name="at"> the specified <seealso cref="AffineTransform"/> </param>
		/// <returns> a <seealso cref="PathIterator"/> that defines the boundary of this
		///          <code>Line2D</code>.
		/// @since 1.2 </returns>
		public virtual PathIterator GetPathIterator(AffineTransform at)
		{
			return new LineIterator(this, at);
		}

		/// <summary>
		/// Returns an iteration object that defines the boundary of this
		/// flattened <code>Line2D</code>.
		/// The iterator for this class is not multi-threaded safe,
		/// which means that this <code>Line2D</code> class does not
		/// guarantee that modifications to the geometry of this
		/// <code>Line2D</code> object do not affect any iterations of that
		/// geometry that are already in process. </summary>
		/// <param name="at"> the specified <code>AffineTransform</code> </param>
		/// <param name="flatness"> the maximum amount that the control points for a
		///          given curve can vary from colinear before a subdivided
		///          curve is replaced by a straight line connecting the
		///          end points.  Since a <code>Line2D</code> object is
		///          always flat, this parameter is ignored. </param>
		/// <returns> a <code>PathIterator</code> that defines the boundary of the
		///                  flattened <code>Line2D</code>
		/// @since 1.2 </returns>
		public virtual PathIterator GetPathIterator(AffineTransform at, double flatness)
		{
			return new LineIterator(this, at);
		}

		/// <summary>
		/// Creates a new object of the same class as this object.
		/// </summary>
		/// <returns>     a clone of this instance. </returns>
		/// <exception cref="OutOfMemoryError">            if there is not enough memory. </exception>
		/// <seealso cref=        java.lang.Cloneable
		/// @since      1.2 </seealso>
		public virtual Object Clone()
		{
			try
			{
				return base.Clone();
			}
			catch (CloneNotSupportedException e)
			{
				// this shouldn't happen, since we are Cloneable
				throw new InternalError(e);
			}
		}
	}

}