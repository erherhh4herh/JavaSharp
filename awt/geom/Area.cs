using System.Collections;

/*
 * Copyright (c) 1998, 2006, Oracle and/or its affiliates. All rights reserved.
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

	using Curve = sun.awt.geom.Curve;
	using Crossings = sun.awt.geom.Crossings;
	using AreaOp = sun.awt.geom.AreaOp;

	/// <summary>
	/// An <code>Area</code> object stores and manipulates a
	/// resolution-independent description of an enclosed area of
	/// 2-dimensional space.
	/// <code>Area</code> objects can be transformed and can perform
	/// various Constructive Area Geometry (CAG) operations when combined
	/// with other <code>Area</code> objects.
	/// The CAG operations include area
	/// <seealso cref="#add addition"/>, <seealso cref="#subtract subtraction"/>,
	/// <seealso cref="#intersect intersection"/>, and <seealso cref="#exclusiveOr exclusive or"/>.
	/// See the linked method documentation for examples of the various
	/// operations.
	/// <para>
	/// The <code>Area</code> class implements the <code>Shape</code>
	/// interface and provides full support for all of its hit-testing
	/// and path iteration facilities, but an <code>Area</code> is more
	/// specific than a generalized path in a number of ways:
	/// <ul>
	/// <li>Only closed paths and sub-paths are stored.
	///     <code>Area</code> objects constructed from unclosed paths
	///     are implicitly closed during construction as if those paths
	///     had been filled by the <code>Graphics2D.fill</code> method.
	/// <li>The interiors of the individual stored sub-paths are all
	///     non-empty and non-overlapping.  Paths are decomposed during
	///     construction into separate component non-overlapping parts,
	///     empty pieces of the path are discarded, and then these
	///     non-empty and non-overlapping properties are maintained
	///     through all subsequent CAG operations.  Outlines of different
	///     component sub-paths may touch each other, as long as they
	///     do not cross so that their enclosed areas overlap.
	/// <li>The geometry of the path describing the outline of the
	///     <code>Area</code> resembles the path from which it was
	///     constructed only in that it describes the same enclosed
	///     2-dimensional area, but may use entirely different types
	///     and ordering of the path segments to do so.
	/// </ul>
	/// Interesting issues which are not always obvious when using
	/// the <code>Area</code> include:
	/// <ul>
	/// <li>Creating an <code>Area</code> from an unclosed (open)
	///     <code>Shape</code> results in a closed outline in the
	///     <code>Area</code> object.
	/// <li>Creating an <code>Area</code> from a <code>Shape</code>
	///     which encloses no area (even when "closed") produces an
	///     empty <code>Area</code>.  A common example of this issue
	///     is that producing an <code>Area</code> from a line will
	///     be empty since the line encloses no area.  An empty
	///     <code>Area</code> will iterate no geometry in its
	///     <code>PathIterator</code> objects.
	/// <li>A self-intersecting <code>Shape</code> may be split into
	///     two (or more) sub-paths each enclosing one of the
	///     non-intersecting portions of the original path.
	/// <li>An <code>Area</code> may take more path segments to
	///     describe the same geometry even when the original
	///     outline is simple and obvious.  The analysis that the
	///     <code>Area</code> class must perform on the path may
	///     not reflect the same concepts of "simple and obvious"
	///     as a human being perceives.
	/// </ul>
	/// 
	/// @since 1.2
	/// </para>
	/// </summary>
	public class Area : Shape, Cloneable
	{
		private static ArrayList EmptyCurves = new ArrayList();

		private ArrayList Curves;

		/// <summary>
		/// Default constructor which creates an empty area.
		/// @since 1.2
		/// </summary>
		public Area()
		{
			Curves = EmptyCurves;
		}

		/// <summary>
		/// The <code>Area</code> class creates an area geometry from the
		/// specified <seealso cref="Shape"/> object.  The geometry is explicitly
		/// closed, if the <code>Shape</code> is not already closed.  The
		/// fill rule (even-odd or winding) specified by the geometry of the
		/// <code>Shape</code> is used to determine the resulting enclosed area. </summary>
		/// <param name="s">  the <code>Shape</code> from which the area is constructed </param>
		/// <exception cref="NullPointerException"> if <code>s</code> is null
		/// @since 1.2 </exception>
		public Area(Shape s)
		{
			if (s is Area)
			{
				Curves = ((Area) s).Curves;
			}
			else
			{
				Curves = PathToCurves(s.GetPathIterator(null));
			}
		}

		private static ArrayList PathToCurves(PathIterator pi)
		{
			ArrayList curves = new ArrayList();
			int windingRule = pi.WindingRule;
			// coords array is big enough for holding:
			//     coordinates returned from currentSegment (6)
			//     OR
			//         two subdivided quadratic curves (2+4+4=10)
			//         AND
			//             0-1 horizontal splitting parameters
			//             OR
			//             2 parametric equation derivative coefficients
			//     OR
			//         three subdivided cubic curves (2+6+6+6=20)
			//         AND
			//             0-2 horizontal splitting parameters
			//             OR
			//             3 parametric equation derivative coefficients
			double[] coords = new double[23];
			double movx = 0, movy = 0;
			double curx = 0, cury = 0;
			double newx, newy;
			while (!pi.Done)
			{
				switch (pi.CurrentSegment(coords))
				{
				case PathIterator_Fields.SEG_MOVETO:
					Curve.insertLine(curves, curx, cury, movx, movy);
					curx = movx = coords[0];
					cury = movy = coords[1];
					Curve.insertMove(curves, movx, movy);
					break;
				case PathIterator_Fields.SEG_LINETO:
					newx = coords[0];
					newy = coords[1];
					Curve.insertLine(curves, curx, cury, newx, newy);
					curx = newx;
					cury = newy;
					break;
				case PathIterator_Fields.SEG_QUADTO:
					newx = coords[2];
					newy = coords[3];
					Curve.insertQuad(curves, curx, cury, coords);
					curx = newx;
					cury = newy;
					break;
				case PathIterator_Fields.SEG_CUBICTO:
					newx = coords[4];
					newy = coords[5];
					Curve.insertCubic(curves, curx, cury, coords);
					curx = newx;
					cury = newy;
					break;
				case PathIterator_Fields.SEG_CLOSE:
					Curve.insertLine(curves, curx, cury, movx, movy);
					curx = movx;
					cury = movy;
					break;
				}
				pi.Next();
			}
			Curve.insertLine(curves, curx, cury, movx, movy);
			AreaOp @operator;
			if (windingRule == PathIterator_Fields.WIND_EVEN_ODD)
			{
				@operator = new AreaOp.EOWindOp();
			}
			else
			{
				@operator = new AreaOp.NZWindOp();
			}
			return @operator.calculate(curves, EmptyCurves);
		}

		/// <summary>
		/// Adds the shape of the specified <code>Area</code> to the
		/// shape of this <code>Area</code>.
		/// The resulting shape of this <code>Area</code> will include
		/// the union of both shapes, or all areas that were contained
		/// in either this or the specified <code>Area</code>.
		/// <pre>
		///     // Example:
		///     Area a1 = new Area([triangle 0,0 =&gt; 8,0 =&gt; 0,8]);
		///     Area a2 = new Area([triangle 0,0 =&gt; 8,0 =&gt; 8,8]);
		///     a1.add(a2);
		/// 
		///        a1(before)     +         a2         =     a1(after)
		/// 
		///     ################     ################     ################
		///     ##############         ##############     ################
		///     ############             ############     ################
		///     ##########                 ##########     ################
		///     ########                     ########     ################
		///     ######                         ######     ######    ######
		///     ####                             ####     ####        ####
		///     ##                                 ##     ##            ##
		/// </pre> </summary>
		/// <param name="rhs">  the <code>Area</code> to be added to the
		///          current shape </param>
		/// <exception cref="NullPointerException"> if <code>rhs</code> is null
		/// @since 1.2 </exception>
		public virtual void Add(Area rhs)
		{
			Curves = (new AreaOp.AddOp()).calculate(this.Curves, rhs.Curves);
			InvalidateBounds();
		}

		/// <summary>
		/// Subtracts the shape of the specified <code>Area</code> from the
		/// shape of this <code>Area</code>.
		/// The resulting shape of this <code>Area</code> will include
		/// areas that were contained only in this <code>Area</code>
		/// and not in the specified <code>Area</code>.
		/// <pre>
		///     // Example:
		///     Area a1 = new Area([triangle 0,0 =&gt; 8,0 =&gt; 0,8]);
		///     Area a2 = new Area([triangle 0,0 =&gt; 8,0 =&gt; 8,8]);
		///     a1.subtract(a2);
		/// 
		///        a1(before)     -         a2         =     a1(after)
		/// 
		///     ################     ################
		///     ##############         ##############     ##
		///     ############             ############     ####
		///     ##########                 ##########     ######
		///     ########                     ########     ########
		///     ######                         ######     ######
		///     ####                             ####     ####
		///     ##                                 ##     ##
		/// </pre> </summary>
		/// <param name="rhs">  the <code>Area</code> to be subtracted from the
		///          current shape </param>
		/// <exception cref="NullPointerException"> if <code>rhs</code> is null
		/// @since 1.2 </exception>
		public virtual void Subtract(Area rhs)
		{
			Curves = (new AreaOp.SubOp()).calculate(this.Curves, rhs.Curves);
			InvalidateBounds();
		}

		/// <summary>
		/// Sets the shape of this <code>Area</code> to the intersection of
		/// its current shape and the shape of the specified <code>Area</code>.
		/// The resulting shape of this <code>Area</code> will include
		/// only areas that were contained in both this <code>Area</code>
		/// and also in the specified <code>Area</code>.
		/// <pre>
		///     // Example:
		///     Area a1 = new Area([triangle 0,0 =&gt; 8,0 =&gt; 0,8]);
		///     Area a2 = new Area([triangle 0,0 =&gt; 8,0 =&gt; 8,8]);
		///     a1.intersect(a2);
		/// 
		///      a1(before)   intersect     a2         =     a1(after)
		/// 
		///     ################     ################     ################
		///     ##############         ##############       ############
		///     ############             ############         ########
		///     ##########                 ##########           ####
		///     ########                     ########
		///     ######                         ######
		///     ####                             ####
		///     ##                                 ##
		/// </pre> </summary>
		/// <param name="rhs">  the <code>Area</code> to be intersected with this
		///          <code>Area</code> </param>
		/// <exception cref="NullPointerException"> if <code>rhs</code> is null
		/// @since 1.2 </exception>
		public virtual void Intersect(Area rhs)
		{
			Curves = (new AreaOp.IntOp()).calculate(this.Curves, rhs.Curves);
			InvalidateBounds();
		}

		/// <summary>
		/// Sets the shape of this <code>Area</code> to be the combined area
		/// of its current shape and the shape of the specified <code>Area</code>,
		/// minus their intersection.
		/// The resulting shape of this <code>Area</code> will include
		/// only areas that were contained in either this <code>Area</code>
		/// or in the specified <code>Area</code>, but not in both.
		/// <pre>
		///     // Example:
		///     Area a1 = new Area([triangle 0,0 =&gt; 8,0 =&gt; 0,8]);
		///     Area a2 = new Area([triangle 0,0 =&gt; 8,0 =&gt; 8,8]);
		///     a1.exclusiveOr(a2);
		/// 
		///        a1(before)    xor        a2         =     a1(after)
		/// 
		///     ################     ################
		///     ##############         ##############     ##            ##
		///     ############             ############     ####        ####
		///     ##########                 ##########     ######    ######
		///     ########                     ########     ################
		///     ######                         ######     ######    ######
		///     ####                             ####     ####        ####
		///     ##                                 ##     ##            ##
		/// </pre> </summary>
		/// <param name="rhs">  the <code>Area</code> to be exclusive ORed with this
		///          <code>Area</code>. </param>
		/// <exception cref="NullPointerException"> if <code>rhs</code> is null
		/// @since 1.2 </exception>
		public virtual void ExclusiveOr(Area rhs)
		{
			Curves = (new AreaOp.XorOp()).calculate(this.Curves, rhs.Curves);
			InvalidateBounds();
		}

		/// <summary>
		/// Removes all of the geometry from this <code>Area</code> and
		/// restores it to an empty area.
		/// @since 1.2
		/// </summary>
		public virtual void Reset()
		{
			Curves = new ArrayList();
			InvalidateBounds();
		}

		/// <summary>
		/// Tests whether this <code>Area</code> object encloses any area. </summary>
		/// <returns>    <code>true</code> if this <code>Area</code> object
		/// represents an empty area; <code>false</code> otherwise.
		/// @since 1.2 </returns>
		public virtual bool Empty
		{
			get
			{
				return (Curves.Count == 0);
			}
		}

		/// <summary>
		/// Tests whether this <code>Area</code> consists entirely of
		/// straight edged polygonal geometry. </summary>
		/// <returns>    <code>true</code> if the geometry of this
		/// <code>Area</code> consists entirely of line segments;
		/// <code>false</code> otherwise.
		/// @since 1.2 </returns>
		public virtual bool Polygonal
		{
			get
			{
				System.Collections.IEnumerator enum_ = Curves.elements();
				while (enum_.hasMoreElements())
				{
					if (((Curve) enum_.nextElement()).Order > 1)
					{
						return false;
					}
				}
				return true;
			}
		}

		/// <summary>
		/// Tests whether this <code>Area</code> is rectangular in shape. </summary>
		/// <returns>    <code>true</code> if the geometry of this
		/// <code>Area</code> is rectangular in shape; <code>false</code>
		/// otherwise.
		/// @since 1.2 </returns>
		public virtual bool Rectangular
		{
			get
			{
				int size = Curves.Count;
				if (size == 0)
				{
					return true;
				}
				if (size > 3)
				{
					return false;
				}
				Curve c1 = (Curve) Curves[1];
				Curve c2 = (Curve) Curves[2];
				if (c1.Order != 1 || c2.Order != 1)
				{
					return false;
				}
				if (c1.XTop != c1.XBot || c2.XTop != c2.XBot)
				{
					return false;
				}
				if (c1.YTop != c2.YTop || c1.YBot != c2.YBot)
				{
					// One might be able to prove that this is impossible...
					return false;
				}
				return true;
			}
		}

		/// <summary>
		/// Tests whether this <code>Area</code> is comprised of a single
		/// closed subpath.  This method returns <code>true</code> if the
		/// path contains 0 or 1 subpaths, or <code>false</code> if the path
		/// contains more than 1 subpath.  The subpaths are counted by the
		/// number of <seealso cref="PathIterator#SEG_MOVETO SEG_MOVETO"/>  segments
		/// that appear in the path. </summary>
		/// <returns>    <code>true</code> if the <code>Area</code> is comprised
		/// of a single basic geometry; <code>false</code> otherwise.
		/// @since 1.2 </returns>
		public virtual bool Singular
		{
			get
			{
				if (Curves.Count < 3)
				{
					return true;
				}
				System.Collections.IEnumerator enum_ = Curves.elements();
				enum_.nextElement(); // First Order0 "moveto"
				while (enum_.hasMoreElements())
				{
					if (((Curve) enum_.nextElement()).Order == 0)
					{
						return false;
					}
				}
				return true;
			}
		}

		private Rectangle2D CachedBounds_Renamed;
		private void InvalidateBounds()
		{
			CachedBounds_Renamed = null;
		}
		private Rectangle2D CachedBounds
		{
			get
			{
				if (CachedBounds_Renamed != null)
				{
					return CachedBounds_Renamed;
				}
				Rectangle2D r = new Rectangle2D.Double();
				if (Curves.Count > 0)
				{
					Curve c = (Curve) Curves[0];
					// First point is always an order 0 curve (moveto)
					r.SetRect(c.X0, c.Y0, 0, 0);
					for (int i = 1; i < Curves.Count; i++)
					{
						((Curve) Curves[i]).enlarge(r);
					}
				}
				return (CachedBounds_Renamed = r);
			}
		}

		/// <summary>
		/// Returns a high precision bounding <seealso cref="Rectangle2D"/> that
		/// completely encloses this <code>Area</code>.
		/// <para>
		/// The Area class will attempt to return the tightest bounding
		/// box possible for the Shape.  The bounding box will not be
		/// padded to include the control points of curves in the outline
		/// of the Shape, but should tightly fit the actual geometry of
		/// the outline itself.
		/// </para>
		/// </summary>
		/// <returns>    the bounding <code>Rectangle2D</code> for the
		/// <code>Area</code>.
		/// @since 1.2 </returns>
		public virtual Rectangle2D Bounds2D
		{
			get
			{
				return CachedBounds.Bounds2D;
			}
		}

		/// <summary>
		/// Returns a bounding <seealso cref="Rectangle"/> that completely encloses
		/// this <code>Area</code>.
		/// <para>
		/// The Area class will attempt to return the tightest bounding
		/// box possible for the Shape.  The bounding box will not be
		/// padded to include the control points of curves in the outline
		/// of the Shape, but should tightly fit the actual geometry of
		/// the outline itself.  Since the returned object represents
		/// the bounding box with integers, the bounding box can only be
		/// as tight as the nearest integer coordinates that encompass
		/// the geometry of the Shape.
		/// </para>
		/// </summary>
		/// <returns>    the bounding <code>Rectangle</code> for the
		/// <code>Area</code>.
		/// @since 1.2 </returns>
		public virtual Rectangle Bounds
		{
			get
			{
				return CachedBounds.Bounds;
			}
		}

		/// <summary>
		/// Returns an exact copy of this <code>Area</code> object. </summary>
		/// <returns>    Created clone object
		/// @since 1.2 </returns>
		public virtual Object Clone()
		{
			return new Area(this);
		}

		/// <summary>
		/// Tests whether the geometries of the two <code>Area</code> objects
		/// are equal.
		/// This method will return false if the argument is null. </summary>
		/// <param name="other">  the <code>Area</code> to be compared to this
		///          <code>Area</code> </param>
		/// <returns>  <code>true</code> if the two geometries are equal;
		///          <code>false</code> otherwise.
		/// @since 1.2 </returns>
		public virtual bool Equals(Area other)
		{
			// REMIND: A *much* simpler operation should be possible...
			// Should be able to do a curve-wise comparison since all Areas
			// should evaluate their curves in the same top-down order.
			if (other == this)
			{
				return true;
			}
			if (other == null)
			{
				return false;
			}
			ArrayList c = (new AreaOp.XorOp()).calculate(this.Curves, other.Curves);
			return c.Count == 0;
		}

		/// <summary>
		/// Transforms the geometry of this <code>Area</code> using the specified
		/// <seealso cref="AffineTransform"/>.  The geometry is transformed in place, which
		/// permanently changes the enclosed area defined by this object. </summary>
		/// <param name="t">  the transformation used to transform the area </param>
		/// <exception cref="NullPointerException"> if <code>t</code> is null
		/// @since 1.2 </exception>
		public virtual void Transform(AffineTransform t)
		{
			if (t == null)
			{
				throw new NullPointerException("transform must not be null");
			}
			// REMIND: A simpler operation can be performed for some types
			// of transform.
			Curves = PathToCurves(GetPathIterator(t));
			InvalidateBounds();
		}

		/// <summary>
		/// Creates a new <code>Area</code> object that contains the same
		/// geometry as this <code>Area</code> transformed by the specified
		/// <code>AffineTransform</code>.  This <code>Area</code> object
		/// is unchanged. </summary>
		/// <param name="t">  the specified <code>AffineTransform</code> used to transform
		///           the new <code>Area</code> </param>
		/// <exception cref="NullPointerException"> if <code>t</code> is null </exception>
		/// <returns>   a new <code>Area</code> object representing the transformed
		///           geometry.
		/// @since 1.2 </returns>
		public virtual Area CreateTransformedArea(AffineTransform t)
		{
			Area a = new Area(this);
			a.Transform(t);
			return a;
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 1.2
		/// </summary>
		public virtual bool Contains(double x, double y)
		{
			if (!CachedBounds.Contains(x, y))
			{
				return false;
			}
			System.Collections.IEnumerator enum_ = Curves.elements();
			int crossings = 0;
			while (enum_.hasMoreElements())
			{
				Curve c = (Curve) enum_.nextElement();
				crossings += c.crossingsFor(x, y);
			}
			return ((crossings & 1) == 1);
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
		public virtual bool Contains(double x, double y, double w, double h)
		{
			if (w < 0 || h < 0)
			{
				return false;
			}
			if (!CachedBounds.Contains(x, y, w, h))
			{
				return false;
			}
			Crossings c = Crossings.findCrossings(Curves, x, y, x + w, y + h);
			return (c != null && c.covers(y, y + h));
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
		/// {@inheritDoc}
		/// @since 1.2
		/// </summary>
		public virtual bool Intersects(double x, double y, double w, double h)
		{
			if (w < 0 || h < 0)
			{
				return false;
			}
			if (!CachedBounds.Intersects(x, y, w, h))
			{
				return false;
			}
			Crossings c = Crossings.findCrossings(Curves, x, y, x + w, y + h);
			return (c == null || !c.Empty);
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
		/// Creates a <seealso cref="PathIterator"/> for the outline of this
		/// <code>Area</code> object.  This <code>Area</code> object is unchanged. </summary>
		/// <param name="at"> an optional <code>AffineTransform</code> to be applied to
		/// the coordinates as they are returned in the iteration, or
		/// <code>null</code> if untransformed coordinates are desired </param>
		/// <returns>    the <code>PathIterator</code> object that returns the
		///          geometry of the outline of this <code>Area</code>, one
		///          segment at a time.
		/// @since 1.2 </returns>
		public virtual PathIterator GetPathIterator(AffineTransform at)
		{
			return new AreaIterator(Curves, at);
		}

		/// <summary>
		/// Creates a <code>PathIterator</code> for the flattened outline of
		/// this <code>Area</code> object.  Only uncurved path segments
		/// represented by the SEG_MOVETO, SEG_LINETO, and SEG_CLOSE point
		/// types are returned by the iterator.  This <code>Area</code>
		/// object is unchanged. </summary>
		/// <param name="at"> an optional <code>AffineTransform</code> to be
		/// applied to the coordinates as they are returned in the
		/// iteration, or <code>null</code> if untransformed coordinates
		/// are desired </param>
		/// <param name="flatness"> the maximum amount that the control points
		/// for a given curve can vary from colinear before a subdivided
		/// curve is replaced by a straight line connecting the end points </param>
		/// <returns>    the <code>PathIterator</code> object that returns the
		/// geometry of the outline of this <code>Area</code>, one segment
		/// at a time.
		/// @since 1.2 </returns>
		public virtual PathIterator GetPathIterator(AffineTransform at, double flatness)
		{
			return new FlatteningPathIterator(GetPathIterator(at), flatness);
		}
	}

	internal class AreaIterator : PathIterator
	{
		private AffineTransform Transform;
		private ArrayList Curves;
		private int Index;
		private Curve Prevcurve;
		private Curve Thiscurve;

		public AreaIterator(ArrayList curves, AffineTransform at)
		{
			this.Curves = curves;
			this.Transform = at;
			if (curves.Count >= 1)
			{
				Thiscurve = (Curve) curves[0];
			}
		}

		public virtual int WindingRule
		{
			get
			{
				// REMIND: Which is better, EVEN_ODD or NON_ZERO?
				//         The paths calculated could be classified either way.
				//return WIND_EVEN_ODD;
				return PathIterator_Fields.WIND_NON_ZERO;
			}
		}

		public virtual bool Done
		{
			get
			{
				return (Prevcurve == null && Thiscurve == null);
			}
		}

		public virtual void Next()
		{
			if (Prevcurve != null)
			{
				Prevcurve = null;
			}
			else
			{
				Prevcurve = Thiscurve;
				Index++;
				if (Index < Curves.Count)
				{
					Thiscurve = (Curve) Curves[Index];
					if (Thiscurve.Order != 0 && Prevcurve.X1 == Thiscurve.X0 && Prevcurve.Y1 == Thiscurve.Y0)
					{
						Prevcurve = null;
					}
				}
				else
				{
					Thiscurve = null;
				}
			}
		}

		public virtual int CurrentSegment(float[] coords)
		{
			double[] dcoords = new double[6];
			int segtype = CurrentSegment(dcoords);
			int numpoints = (segtype == PathIterator_Fields.SEG_CLOSE ? 0 : (segtype == PathIterator_Fields.SEG_QUADTO ? 2 : (segtype == PathIterator_Fields.SEG_CUBICTO ? 3 : 1)));
			for (int i = 0; i < numpoints * 2; i++)
			{
				coords[i] = (float) dcoords[i];
			}
			return segtype;
		}

		public virtual int CurrentSegment(double[] coords)
		{
			int segtype;
			int numpoints;
			if (Prevcurve != null)
			{
				// Need to finish off junction between curves
				if (Thiscurve == null || Thiscurve.Order == 0)
				{
					return PathIterator_Fields.SEG_CLOSE;
				}
				coords[0] = Thiscurve.X0;
				coords[1] = Thiscurve.Y0;
				segtype = PathIterator_Fields.SEG_LINETO;
				numpoints = 1;
			}
			else if (Thiscurve == null)
			{
				throw new NoSuchElementException("area iterator out of bounds");
			}
			else
			{
				segtype = Thiscurve.getSegment(coords);
				numpoints = Thiscurve.Order;
				if (numpoints == 0)
				{
					numpoints = 1;
				}
			}
			if (Transform != null)
			{
				Transform.Transform(coords, 0, coords, 0, numpoints);
			}
			return segtype;
		}
	}

}