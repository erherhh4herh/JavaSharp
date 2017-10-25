using System;

/*
 * Copyright (c) 1995, 2013, Oracle and/or its affiliates. All rights reserved.
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

	using Crossings = sun.awt.geom.Crossings;

	/// <summary>
	/// The <code>Polygon</code> class encapsulates a description of a
	/// closed, two-dimensional region within a coordinate space. This
	/// region is bounded by an arbitrary number of line segments, each of
	/// which is one side of the polygon. Internally, a polygon
	/// comprises of a list of {@code (x,y)}
	/// coordinate pairs, where each pair defines a <i>vertex</i> of the
	/// polygon, and two successive pairs are the endpoints of a
	/// line that is a side of the polygon. The first and final
	/// pairs of {@code (x,y)} points are joined by a line segment
	/// that closes the polygon.  This <code>Polygon</code> is defined with
	/// an even-odd winding rule.  See
	/// <seealso cref="java.awt.geom.PathIterator#WIND_EVEN_ODD WIND_EVEN_ODD"/>
	/// for a definition of the even-odd winding rule.
	/// This class's hit-testing methods, which include the
	/// <code>contains</code>, <code>intersects</code> and <code>inside</code>
	/// methods, use the <i>insideness</i> definition described in the
	/// <seealso cref="Shape"/> class comments.
	/// 
	/// @author      Sami Shaio </summary>
	/// <seealso cref= Shape
	/// @author      Herb Jellinek
	/// @since       1.0 </seealso>
	[Serializable]
	public class Polygon : Shape
	{

		/// <summary>
		/// The total number of points.  The value of <code>npoints</code>
		/// represents the number of valid points in this <code>Polygon</code>
		/// and might be less than the number of elements in
		/// <seealso cref="#xpoints xpoints"/> or <seealso cref="#ypoints ypoints"/>.
		/// This value can be NULL.
		/// 
		/// @serial </summary>
		/// <seealso cref= #addPoint(int, int)
		/// @since 1.0 </seealso>
		public int Npoints;

		/// <summary>
		/// The array of X coordinates.  The number of elements in
		/// this array might be more than the number of X coordinates
		/// in this <code>Polygon</code>.  The extra elements allow new points
		/// to be added to this <code>Polygon</code> without re-creating this
		/// array.  The value of <seealso cref="#npoints npoints"/> is equal to the
		/// number of valid points in this <code>Polygon</code>.
		/// 
		/// @serial </summary>
		/// <seealso cref= #addPoint(int, int)
		/// @since 1.0 </seealso>
		public int[] Xpoints;

		/// <summary>
		/// The array of Y coordinates.  The number of elements in
		/// this array might be more than the number of Y coordinates
		/// in this <code>Polygon</code>.  The extra elements allow new points
		/// to be added to this <code>Polygon</code> without re-creating this
		/// array.  The value of <code>npoints</code> is equal to the
		/// number of valid points in this <code>Polygon</code>.
		/// 
		/// @serial </summary>
		/// <seealso cref= #addPoint(int, int)
		/// @since 1.0 </seealso>
		public int[] Ypoints;

		/// <summary>
		/// The bounds of this {@code Polygon}.
		/// This value can be null.
		/// 
		/// @serial </summary>
		/// <seealso cref= #getBoundingBox() </seealso>
		/// <seealso cref= #getBounds()
		/// @since 1.0 </seealso>
		protected internal Rectangle Bounds_Renamed;

		/*
		 * JDK 1.1 serialVersionUID
		 */
		private const long SerialVersionUID = -6460061437900069969L;

		/*
		 * Default length for xpoints and ypoints.
		 */
		private const int MIN_LENGTH = 4;

		/// <summary>
		/// Creates an empty polygon.
		/// @since 1.0
		/// </summary>
		public Polygon()
		{
			Xpoints = new int[MIN_LENGTH];
			Ypoints = new int[MIN_LENGTH];
		}

		/// <summary>
		/// Constructs and initializes a <code>Polygon</code> from the specified
		/// parameters. </summary>
		/// <param name="xpoints"> an array of X coordinates </param>
		/// <param name="ypoints"> an array of Y coordinates </param>
		/// <param name="npoints"> the total number of points in the
		///                          <code>Polygon</code> </param>
		/// <exception cref="NegativeArraySizeException"> if the value of
		///                       <code>npoints</code> is negative. </exception>
		/// <exception cref="IndexOutOfBoundsException"> if <code>npoints</code> is
		///             greater than the length of <code>xpoints</code>
		///             or the length of <code>ypoints</code>. </exception>
		/// <exception cref="NullPointerException"> if <code>xpoints</code> or
		///             <code>ypoints</code> is <code>null</code>.
		/// @since 1.0 </exception>
		public Polygon(int[] xpoints, int[] ypoints, int npoints)
		{
			// Fix 4489009: should throw IndexOutofBoundsException instead
			// of OutofMemoryException if npoints is huge and > {x,y}points.length
			if (npoints > xpoints.Length || npoints > ypoints.Length)
			{
				throw new IndexOutOfBoundsException("npoints > xpoints.length || " + "npoints > ypoints.length");
			}
			// Fix 6191114: should throw NegativeArraySizeException with
			// negative npoints
			if (npoints < 0)
			{
				throw new NegativeArraySizeException("npoints < 0");
			}
			// Fix 6343431: Applet compatibility problems if arrays are not
			// exactly npoints in length
			this.Npoints = npoints;
			this.Xpoints = Arrays.CopyOf(xpoints, npoints);
			this.Ypoints = Arrays.CopyOf(ypoints, npoints);
		}

		/// <summary>
		/// Resets this <code>Polygon</code> object to an empty polygon.
		/// The coordinate arrays and the data in them are left untouched
		/// but the number of points is reset to zero to mark the old
		/// vertex data as invalid and to start accumulating new vertex
		/// data at the beginning.
		/// All internally-cached data relating to the old vertices
		/// are discarded.
		/// Note that since the coordinate arrays from before the reset
		/// are reused, creating a new empty <code>Polygon</code> might
		/// be more memory efficient than resetting the current one if
		/// the number of vertices in the new polygon data is significantly
		/// smaller than the number of vertices in the data from before the
		/// reset. </summary>
		/// <seealso cref=         java.awt.Polygon#invalidate
		/// @since 1.4 </seealso>
		public virtual void Reset()
		{
			Npoints = 0;
			Bounds_Renamed = null;
		}

		/// <summary>
		/// Invalidates or flushes any internally-cached data that depends
		/// on the vertex coordinates of this <code>Polygon</code>.
		/// This method should be called after any direct manipulation
		/// of the coordinates in the <code>xpoints</code> or
		/// <code>ypoints</code> arrays to avoid inconsistent results
		/// from methods such as <code>getBounds</code> or <code>contains</code>
		/// that might cache data from earlier computations relating to
		/// the vertex coordinates. </summary>
		/// <seealso cref=         java.awt.Polygon#getBounds
		/// @since 1.4 </seealso>
		public virtual void Invalidate()
		{
			Bounds_Renamed = null;
		}

		/// <summary>
		/// Translates the vertices of the <code>Polygon</code> by
		/// <code>deltaX</code> along the x axis and by
		/// <code>deltaY</code> along the y axis. </summary>
		/// <param name="deltaX"> the amount to translate along the X axis </param>
		/// <param name="deltaY"> the amount to translate along the Y axis
		/// @since 1.1 </param>
		public virtual void Translate(int deltaX, int deltaY)
		{
			for (int i = 0; i < Npoints; i++)
			{
				Xpoints[i] += deltaX;
				Ypoints[i] += deltaY;
			}
			if (Bounds_Renamed != null)
			{
				Bounds_Renamed.Translate(deltaX, deltaY);
			}
		}

		/*
		 * Calculates the bounding box of the points passed to the constructor.
		 * Sets <code>bounds</code> to the result.
		 * @param xpoints[] array of <i>x</i> coordinates
		 * @param ypoints[] array of <i>y</i> coordinates
		 * @param npoints the total number of points
		 */
		internal virtual void CalculateBounds(int[] xpoints, int[] ypoints, int npoints)
		{
			int boundsMinX = Integer.MaxValue;
			int boundsMinY = Integer.MaxValue;
			int boundsMaxX = Integer.MinValue;
			int boundsMaxY = Integer.MinValue;

			for (int i = 0; i < npoints; i++)
			{
				int x = xpoints[i];
				boundsMinX = System.Math.Min(boundsMinX, x);
				boundsMaxX = System.Math.Max(boundsMaxX, x);
				int y = ypoints[i];
				boundsMinY = System.Math.Min(boundsMinY, y);
				boundsMaxY = System.Math.Max(boundsMaxY, y);
			}
			Bounds_Renamed = new Rectangle(boundsMinX, boundsMinY, boundsMaxX - boundsMinX, boundsMaxY - boundsMinY);
		}

		/*
		 * Resizes the bounding box to accommodate the specified coordinates.
		 * @param x,&nbsp;y the specified coordinates
		 */
		internal virtual void UpdateBounds(int x, int y)
		{
			if (x < Bounds_Renamed.x)
			{
				Bounds_Renamed.Width_Renamed = Bounds_Renamed.Width_Renamed + (Bounds_Renamed.x - x);
				Bounds_Renamed.x = x;
			}
			else
			{
				Bounds_Renamed.Width_Renamed = System.Math.Max(Bounds_Renamed.Width_Renamed, x - Bounds_Renamed.x);
				// bounds.x = bounds.x;
			}

			if (y < Bounds_Renamed.y)
			{
				Bounds_Renamed.Height_Renamed = Bounds_Renamed.Height_Renamed + (Bounds_Renamed.y - y);
				Bounds_Renamed.y = y;
			}
			else
			{
				Bounds_Renamed.Height_Renamed = System.Math.Max(Bounds_Renamed.Height_Renamed, y - Bounds_Renamed.y);
				// bounds.y = bounds.y;
			}
		}

		/// <summary>
		/// Appends the specified coordinates to this <code>Polygon</code>.
		/// <para>
		/// If an operation that calculates the bounding box of this
		/// <code>Polygon</code> has already been performed, such as
		/// <code>getBounds</code> or <code>contains</code>, then this
		/// method updates the bounding box.
		/// </para>
		/// </summary>
		/// <param name="x"> the specified X coordinate </param>
		/// <param name="y"> the specified Y coordinate </param>
		/// <seealso cref=         java.awt.Polygon#getBounds </seealso>
		/// <seealso cref=         java.awt.Polygon#contains
		/// @since 1.0 </seealso>
		public virtual void AddPoint(int x, int y)
		{
			if (Npoints >= Xpoints.Length || Npoints >= Ypoints.Length)
			{
				int newLength = Npoints * 2;
				// Make sure that newLength will be greater than MIN_LENGTH and
				// aligned to the power of 2
				if (newLength < MIN_LENGTH)
				{
					newLength = MIN_LENGTH;
				}
				else if ((newLength & (newLength - 1)) != 0)
				{
					newLength = Integer.HighestOneBit(newLength);
				}

				Xpoints = Arrays.CopyOf(Xpoints, newLength);
				Ypoints = Arrays.CopyOf(Ypoints, newLength);
			}
			Xpoints[Npoints] = x;
			Ypoints[Npoints] = y;
			Npoints++;
			if (Bounds_Renamed != null)
			{
				UpdateBounds(x, y);
			}
		}

		/// <summary>
		/// Gets the bounding box of this <code>Polygon</code>.
		/// The bounding box is the smallest <seealso cref="Rectangle"/> whose
		/// sides are parallel to the x and y axes of the
		/// coordinate space, and can completely contain the <code>Polygon</code>. </summary>
		/// <returns> a <code>Rectangle</code> that defines the bounds of this
		/// <code>Polygon</code>.
		/// @since 1.1 </returns>
		public virtual Rectangle Bounds
		{
			get
			{
				return BoundingBox;
			}
		}

		/// <summary>
		/// Returns the bounds of this <code>Polygon</code>. </summary>
		/// <returns> the bounds of this <code>Polygon</code>. </returns>
		/// @deprecated As of JDK version 1.1,
		/// replaced by <code>getBounds()</code>.
		/// @since 1.0 
		[Obsolete("As of JDK version 1.1,")]
		public virtual Rectangle BoundingBox
		{
			get
			{
				if (Npoints == 0)
				{
					return new Rectangle();
				}
				if (Bounds_Renamed == null)
				{
					CalculateBounds(Xpoints, Ypoints, Npoints);
				}
				return Bounds_Renamed.Bounds;
			}
		}

		/// <summary>
		/// Determines whether the specified <seealso cref="Point"/> is inside this
		/// <code>Polygon</code>. </summary>
		/// <param name="p"> the specified <code>Point</code> to be tested </param>
		/// <returns> <code>true</code> if the <code>Polygon</code> contains the
		///                  <code>Point</code>; <code>false</code> otherwise. </returns>
		/// <seealso cref= #contains(double, double)
		/// @since 1.0 </seealso>
		public virtual bool Contains(Point p)
		{
			return Contains(p.x, p.y);
		}

		/// <summary>
		/// Determines whether the specified coordinates are inside this
		/// <code>Polygon</code>.
		/// <para>
		/// </para>
		/// </summary>
		/// <param name="x"> the specified X coordinate to be tested </param>
		/// <param name="y"> the specified Y coordinate to be tested </param>
		/// <returns> {@code true} if this {@code Polygon} contains
		///         the specified coordinates {@code (x,y)};
		///         {@code false} otherwise. </returns>
		/// <seealso cref= #contains(double, double)
		/// @since 1.1 </seealso>
		public virtual bool Contains(int x, int y)
		{
			return Contains((double) x, (double) y);
		}

		/// <summary>
		/// Determines whether the specified coordinates are contained in this
		/// <code>Polygon</code>. </summary>
		/// <param name="x"> the specified X coordinate to be tested </param>
		/// <param name="y"> the specified Y coordinate to be tested </param>
		/// <returns> {@code true} if this {@code Polygon} contains
		///         the specified coordinates {@code (x,y)};
		///         {@code false} otherwise. </returns>
		/// <seealso cref= #contains(double, double) </seealso>
		/// @deprecated As of JDK version 1.1,
		/// replaced by <code>contains(int, int)</code>.
		/// @since 1.0 
		[Obsolete("As of JDK version 1.1,")]
		public virtual bool Inside(int x, int y)
		{
			return Contains((double) x, (double) y);
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 1.2
		/// </summary>
		public virtual Rectangle2D Bounds2D
		{
			get
			{
				return Bounds;
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 1.2
		/// </summary>
		public virtual bool Contains(double x, double y)
		{
			if (Npoints <= 2 || !BoundingBox.Contains(x, y))
			{
				return false;
			}
			int hits = 0;

			int lastx = Xpoints[Npoints - 1];
			int lasty = Ypoints[Npoints - 1];
			int curx, cury;

			// Walk the edges of the polygon
			for (int i = 0; i < Npoints; lastx = curx, lasty = cury, i++)
			{
				curx = Xpoints[i];
				cury = Ypoints[i];

				if (cury == lasty)
				{
					continue;
				}

				int leftx;
				if (curx < lastx)
				{
					if (x >= lastx)
					{
						continue;
					}
					leftx = curx;
				}
				else
				{
					if (x >= curx)
					{
						continue;
					}
					leftx = lastx;
				}

				double test1, test2;
				if (cury < lasty)
				{
					if (y < cury || y >= lasty)
					{
						continue;
					}
					if (x < leftx)
					{
						hits++;
						continue;
					}
					test1 = x - curx;
					test2 = y - cury;
				}
				else
				{
					if (y < lasty || y >= cury)
					{
						continue;
					}
					if (x < leftx)
					{
						hits++;
						continue;
					}
					test1 = x - lastx;
					test2 = y - lasty;
				}

				if (test1 < (test2 / (lasty - cury) * (lastx - curx)))
				{
					hits++;
				}
			}

			return ((hits & 1) != 0);
		}

		private Crossings GetCrossings(double xlo, double ylo, double xhi, double yhi)
		{
			Crossings cross = new Crossings.EvenOdd(xlo, ylo, xhi, yhi);
			int lastx = Xpoints[Npoints - 1];
			int lasty = Ypoints[Npoints - 1];
			int curx, cury;

			// Walk the edges of the polygon
			for (int i = 0; i < Npoints; i++)
			{
				curx = Xpoints[i];
				cury = Ypoints[i];
				if (cross.accumulateLine(lastx, lasty, curx, cury))
				{
					return null;
				}
				lastx = curx;
				lasty = cury;
			}

			return cross;
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 1.2
		/// </summary>
		public virtual bool Contains(Point2D p)
		{
			return Contains(p.X, p.Y);
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 1.2
		/// </summary>
		public virtual bool Intersects(double x, double y, double w, double h)
		{
			if (Npoints <= 0 || !BoundingBox.Intersects(x, y, w, h))
			{
				return false;
			}

			Crossings cross = GetCrossings(x, y, x + w, y + h);
			return (cross == null || !cross.Empty);
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 1.2
		/// </summary>
		public virtual bool Intersects(Rectangle2D r)
		{
			return Intersects(r.X, r.Y, r.Width, r.Height);
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 1.2
		/// </summary>
		public virtual bool Contains(double x, double y, double w, double h)
		{
			if (Npoints <= 0 || !BoundingBox.Intersects(x, y, w, h))
			{
				return false;
			}

			Crossings cross = GetCrossings(x, y, x + w, y + h);
			return (cross != null && cross.covers(y, y + h));
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 1.2
		/// </summary>
		public virtual bool Contains(Rectangle2D r)
		{
			return Contains(r.X, r.Y, r.Width, r.Height);
		}

		/// <summary>
		/// Returns an iterator object that iterates along the boundary of this
		/// <code>Polygon</code> and provides access to the geometry
		/// of the outline of this <code>Polygon</code>.  An optional
		/// <seealso cref="AffineTransform"/> can be specified so that the coordinates
		/// returned in the iteration are transformed accordingly. </summary>
		/// <param name="at"> an optional <code>AffineTransform</code> to be applied to the
		///          coordinates as they are returned in the iteration, or
		///          <code>null</code> if untransformed coordinates are desired </param>
		/// <returns> a <seealso cref="PathIterator"/> object that provides access to the
		///          geometry of this <code>Polygon</code>.
		/// @since 1.2 </returns>
		public virtual PathIterator GetPathIterator(AffineTransform at)
		{
			return new PolygonPathIterator(this, this, at);
		}

		/// <summary>
		/// Returns an iterator object that iterates along the boundary of
		/// the <code>Shape</code> and provides access to the geometry of the
		/// outline of the <code>Shape</code>.  Only SEG_MOVETO, SEG_LINETO, and
		/// SEG_CLOSE point types are returned by the iterator.
		/// Since polygons are already flat, the <code>flatness</code> parameter
		/// is ignored.  An optional <code>AffineTransform</code> can be specified
		/// in which case the coordinates returned in the iteration are transformed
		/// accordingly. </summary>
		/// <param name="at"> an optional <code>AffineTransform</code> to be applied to the
		///          coordinates as they are returned in the iteration, or
		///          <code>null</code> if untransformed coordinates are desired </param>
		/// <param name="flatness"> the maximum amount that the control points
		///          for a given curve can vary from colinear before a subdivided
		///          curve is replaced by a straight line connecting the
		///          endpoints.  Since polygons are already flat the
		///          <code>flatness</code> parameter is ignored. </param>
		/// <returns> a <code>PathIterator</code> object that provides access to the
		///          <code>Shape</code> object's geometry.
		/// @since 1.2 </returns>
		public virtual PathIterator GetPathIterator(AffineTransform at, double flatness)
		{
			return GetPathIterator(at);
		}

		internal class PolygonPathIterator : PathIterator
		{
			private readonly Polygon OuterInstance;

			internal Polygon Poly;
			internal AffineTransform Transform;
			internal int Index;

			public PolygonPathIterator(Polygon outerInstance, Polygon pg, AffineTransform at)
			{
				this.OuterInstance = outerInstance;
				Poly = pg;
				Transform = at;
				if (pg.Npoints == 0)
				{
					// Prevent a spurious SEG_CLOSE segment
					Index = 1;
				}
			}

			/// <summary>
			/// Returns the winding rule for determining the interior of the
			/// path. </summary>
			/// <returns> an integer representing the current winding rule. </returns>
			/// <seealso cref= PathIterator#WIND_NON_ZERO </seealso>
			public virtual int WindingRule
			{
				get
				{
					return geom.PathIterator_Fields.WIND_EVEN_ODD;
				}
			}

			/// <summary>
			/// Tests if there are more points to read. </summary>
			/// <returns> <code>true</code> if there are more points to read;
			///          <code>false</code> otherwise. </returns>
			public virtual bool Done
			{
				get
				{
					return Index > Poly.Npoints;
				}
			}

			/// <summary>
			/// Moves the iterator forwards, along the primary direction of
			/// traversal, to the next segment of the path when there are
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
			/// SEG_MOVETO, SEG_LINETO, or SEG_CLOSE.
			/// A <code>float</code> array of length 2 must be passed in and
			/// can be used to store the coordinates of the point(s).
			/// Each point is stored as a pair of <code>float</code> x,&nbsp;y
			/// coordinates.  SEG_MOVETO and SEG_LINETO types return one
			/// point, and SEG_CLOSE does not return any points. </summary>
			/// <param name="coords"> a <code>float</code> array that specifies the
			/// coordinates of the point(s) </param>
			/// <returns> an integer representing the type and coordinates of the
			///              current path segment. </returns>
			/// <seealso cref= PathIterator#SEG_MOVETO </seealso>
			/// <seealso cref= PathIterator#SEG_LINETO </seealso>
			/// <seealso cref= PathIterator#SEG_CLOSE </seealso>
			public virtual int CurrentSegment(float[] coords)
			{
				if (Index >= Poly.Npoints)
				{
					return geom.PathIterator_Fields.SEG_CLOSE;
				}
				coords[0] = Poly.Xpoints[Index];
				coords[1] = Poly.Ypoints[Index];
				if (Transform != null)
				{
					Transform.Transform(coords, 0, coords, 0, 1);
				}
				return (Index == 0 ? geom.PathIterator_Fields.SEG_MOVETO : geom.PathIterator_Fields.SEG_LINETO);
			}

			/// <summary>
			/// Returns the coordinates and type of the current path segment in
			/// the iteration.
			/// The return value is the path segment type:
			/// SEG_MOVETO, SEG_LINETO, or SEG_CLOSE.
			/// A <code>double</code> array of length 2 must be passed in and
			/// can be used to store the coordinates of the point(s).
			/// Each point is stored as a pair of <code>double</code> x,&nbsp;y
			/// coordinates.
			/// SEG_MOVETO and SEG_LINETO types return one point,
			/// and SEG_CLOSE does not return any points. </summary>
			/// <param name="coords"> a <code>double</code> array that specifies the
			/// coordinates of the point(s) </param>
			/// <returns> an integer representing the type and coordinates of the
			///              current path segment. </returns>
			/// <seealso cref= PathIterator#SEG_MOVETO </seealso>
			/// <seealso cref= PathIterator#SEG_LINETO </seealso>
			/// <seealso cref= PathIterator#SEG_CLOSE </seealso>
			public virtual int CurrentSegment(double[] coords)
			{
				if (Index >= Poly.Npoints)
				{
					return geom.PathIterator_Fields.SEG_CLOSE;
				}
				coords[0] = Poly.Xpoints[Index];
				coords[1] = Poly.Ypoints[Index];
				if (Transform != null)
				{
					Transform.Transform(coords, 0, coords, 0, 1);
				}
				return (Index == 0 ? geom.PathIterator_Fields.SEG_MOVETO : geom.PathIterator_Fields.SEG_LINETO);
			}
		}
	}

}