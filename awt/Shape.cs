/*
 * Copyright (c) 1996, 2013, Oracle and/or its affiliates. All rights reserved.
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
	/// The <code>Shape</code> interface provides definitions for objects
	/// that represent some form of geometric shape.  The <code>Shape</code>
	/// is described by a <seealso cref="PathIterator"/> object, which can express the
	/// outline of the <code>Shape</code> as well as a rule for determining
	/// how the outline divides the 2D plane into interior and exterior
	/// points.  Each <code>Shape</code> object provides callbacks to get the
	/// bounding box of the geometry, determine whether points or
	/// rectangles lie partly or entirely within the interior
	/// of the <code>Shape</code>, and retrieve a <code>PathIterator</code>
	/// object that describes the trajectory path of the <code>Shape</code>
	/// outline.
	/// <para>
	/// <a name="def_insideness"><b>Definition of insideness:</b></a>
	/// A point is considered to lie inside a
	/// <code>Shape</code> if and only if:
	/// <ul>
	/// <li> it lies completely
	/// inside the<code>Shape</code> boundary <i>or</i>
	/// <li>
	/// it lies exactly on the <code>Shape</code> boundary <i>and</i> the
	/// space immediately adjacent to the
	/// point in the increasing <code>X</code> direction is
	/// entirely inside the boundary <i>or</i>
	/// <li>
	/// it lies exactly on a horizontal boundary segment <b>and</b> the
	/// space immediately adjacent to the point in the
	/// increasing <code>Y</code> direction is inside the boundary.
	/// </ul>
	/// </para>
	/// <para>The <code>contains</code> and <code>intersects</code> methods
	/// consider the interior of a <code>Shape</code> to be the area it
	/// encloses as if it were filled.  This means that these methods
	/// consider
	/// unclosed shapes to be implicitly closed for the purpose of
	/// determining if a shape contains or intersects a rectangle or if a
	/// shape contains a point.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= java.awt.geom.PathIterator </seealso>
	/// <seealso cref= java.awt.geom.AffineTransform </seealso>
	/// <seealso cref= java.awt.geom.FlatteningPathIterator </seealso>
	/// <seealso cref= java.awt.geom.GeneralPath
	/// 
	/// @author Jim Graham
	/// @since 1.2 </seealso>
	public interface Shape
	{
		/// <summary>
		/// Returns an integer <seealso cref="Rectangle"/> that completely encloses the
		/// <code>Shape</code>.  Note that there is no guarantee that the
		/// returned <code>Rectangle</code> is the smallest bounding box that
		/// encloses the <code>Shape</code>, only that the <code>Shape</code>
		/// lies entirely within the indicated  <code>Rectangle</code>.  The
		/// returned <code>Rectangle</code> might also fail to completely
		/// enclose the <code>Shape</code> if the <code>Shape</code> overflows
		/// the limited range of the integer data type.  The
		/// <code>getBounds2D</code> method generally returns a
		/// tighter bounding box due to its greater flexibility in
		/// representation.
		/// 
		/// <para>
		/// Note that the <a href="{@docRoot}/java/awt/Shape.html#def_insideness">
		/// definition of insideness</a> can lead to situations where points
		/// on the defining outline of the {@code shape} may not be considered
		/// contained in the returned {@code bounds} object, but only in cases
		/// where those points are also not considered contained in the original
		/// {@code shape}.
		/// </para>
		/// <para>
		/// If a {@code point} is inside the {@code shape} according to the
		/// <seealso cref="#contains(double x, double y) contains(point)"/> method, then
		/// it must be inside the returned {@code Rectangle} bounds object
		/// according to the <seealso cref="#contains(double x, double y) contains(point)"/>
		/// method of the {@code bounds}. Specifically:
		/// </para>
		/// <para>
		///  {@code shape.contains(x,y)} requires {@code bounds.contains(x,y)}
		/// </para>
		/// <para>
		/// If a {@code point} is not inside the {@code shape}, then it might
		/// still be contained in the {@code bounds} object:
		/// </para>
		/// <para>
		///  {@code bounds.contains(x,y)} does not imply {@code shape.contains(x,y)}
		/// </para> </summary>
		/// <returns> an integer <code>Rectangle</code> that completely encloses
		///                 the <code>Shape</code>. </returns>
		/// <seealso cref= #getBounds2D
		/// @since 1.2 </seealso>
		Rectangle Bounds {get;}

		/// <summary>
		/// Returns a high precision and more accurate bounding box of
		/// the <code>Shape</code> than the <code>getBounds</code> method.
		/// Note that there is no guarantee that the returned
		/// <seealso cref="Rectangle2D"/> is the smallest bounding box that encloses
		/// the <code>Shape</code>, only that the <code>Shape</code> lies
		/// entirely within the indicated <code>Rectangle2D</code>.  The
		/// bounding box returned by this method is usually tighter than that
		/// returned by the <code>getBounds</code> method and never fails due
		/// to overflow problems since the return value can be an instance of
		/// the <code>Rectangle2D</code> that uses double precision values to
		/// store the dimensions.
		/// 
		/// <para>
		/// Note that the <a href="{@docRoot}/java/awt/Shape.html#def_insideness">
		/// definition of insideness</a> can lead to situations where points
		/// on the defining outline of the {@code shape} may not be considered
		/// contained in the returned {@code bounds} object, but only in cases
		/// where those points are also not considered contained in the original
		/// {@code shape}.
		/// </para>
		/// <para>
		/// If a {@code point} is inside the {@code shape} according to the
		/// <seealso cref="#contains(Point2D p) contains(point)"/> method, then it must
		/// be inside the returned {@code Rectangle2D} bounds object according
		/// to the <seealso cref="#contains(Point2D p) contains(point)"/> method of the
		/// {@code bounds}. Specifically:
		/// </para>
		/// <para>
		///  {@code shape.contains(p)} requires {@code bounds.contains(p)}
		/// </para>
		/// <para>
		/// If a {@code point} is not inside the {@code shape}, then it might
		/// still be contained in the {@code bounds} object:
		/// </para>
		/// <para>
		///  {@code bounds.contains(p)} does not imply {@code shape.contains(p)}
		/// </para> </summary>
		/// <returns> an instance of <code>Rectangle2D</code> that is a
		///                 high-precision bounding box of the <code>Shape</code>. </returns>
		/// <seealso cref= #getBounds
		/// @since 1.2 </seealso>
		Rectangle2D Bounds2D {get;}

		/// <summary>
		/// Tests if the specified coordinates are inside the boundary of the
		/// <code>Shape</code>, as described by the
		/// <a href="{@docRoot}/java/awt/Shape.html#def_insideness">
		/// definition of insideness</a>. </summary>
		/// <param name="x"> the specified X coordinate to be tested </param>
		/// <param name="y"> the specified Y coordinate to be tested </param>
		/// <returns> <code>true</code> if the specified coordinates are inside
		///         the <code>Shape</code> boundary; <code>false</code>
		///         otherwise.
		/// @since 1.2 </returns>
		bool Contains(double x, double y);

		/// <summary>
		/// Tests if a specified <seealso cref="Point2D"/> is inside the boundary
		/// of the <code>Shape</code>, as described by the
		/// <a href="{@docRoot}/java/awt/Shape.html#def_insideness">
		/// definition of insideness</a>. </summary>
		/// <param name="p"> the specified <code>Point2D</code> to be tested </param>
		/// <returns> <code>true</code> if the specified <code>Point2D</code> is
		///          inside the boundary of the <code>Shape</code>;
		///          <code>false</code> otherwise.
		/// @since 1.2 </returns>
		bool Contains(Point2D p);

		/// <summary>
		/// Tests if the interior of the <code>Shape</code> intersects the
		/// interior of a specified rectangular area.
		/// The rectangular area is considered to intersect the <code>Shape</code>
		/// if any point is contained in both the interior of the
		/// <code>Shape</code> and the specified rectangular area.
		/// <para>
		/// The {@code Shape.intersects()} method allows a {@code Shape}
		/// implementation to conservatively return {@code true} when:
		/// <ul>
		/// <li>
		/// there is a high probability that the rectangular area and the
		/// <code>Shape</code> intersect, but
		/// <li>
		/// the calculations to accurately determine this intersection
		/// are prohibitively expensive.
		/// </ul>
		/// This means that for some {@code Shapes} this method might
		/// return {@code true} even though the rectangular area does not
		/// intersect the {@code Shape}.
		/// The <seealso cref="java.awt.geom.Area Area"/> class performs
		/// more accurate computations of geometric intersection than most
		/// {@code Shape} objects and therefore can be used if a more precise
		/// answer is required.
		/// 
		/// </para>
		/// </summary>
		/// <param name="x"> the X coordinate of the upper-left corner
		///          of the specified rectangular area </param>
		/// <param name="y"> the Y coordinate of the upper-left corner
		///          of the specified rectangular area </param>
		/// <param name="w"> the width of the specified rectangular area </param>
		/// <param name="h"> the height of the specified rectangular area </param>
		/// <returns> <code>true</code> if the interior of the <code>Shape</code> and
		///          the interior of the rectangular area intersect, or are
		///          both highly likely to intersect and intersection calculations
		///          would be too expensive to perform; <code>false</code> otherwise. </returns>
		/// <seealso cref= java.awt.geom.Area
		/// @since 1.2 </seealso>
		bool Intersects(double x, double y, double w, double h);

		/// <summary>
		/// Tests if the interior of the <code>Shape</code> intersects the
		/// interior of a specified <code>Rectangle2D</code>.
		/// The {@code Shape.intersects()} method allows a {@code Shape}
		/// implementation to conservatively return {@code true} when:
		/// <ul>
		/// <li>
		/// there is a high probability that the <code>Rectangle2D</code> and the
		/// <code>Shape</code> intersect, but
		/// <li>
		/// the calculations to accurately determine this intersection
		/// are prohibitively expensive.
		/// </ul>
		/// This means that for some {@code Shapes} this method might
		/// return {@code true} even though the {@code Rectangle2D} does not
		/// intersect the {@code Shape}.
		/// The <seealso cref="java.awt.geom.Area Area"/> class performs
		/// more accurate computations of geometric intersection than most
		/// {@code Shape} objects and therefore can be used if a more precise
		/// answer is required.
		/// </summary>
		/// <param name="r"> the specified <code>Rectangle2D</code> </param>
		/// <returns> <code>true</code> if the interior of the <code>Shape</code> and
		///          the interior of the specified <code>Rectangle2D</code>
		///          intersect, or are both highly likely to intersect and intersection
		///          calculations would be too expensive to perform; <code>false</code>
		///          otherwise. </returns>
		/// <seealso cref= #intersects(double, double, double, double)
		/// @since 1.2 </seealso>
		bool Intersects(Rectangle2D r);

		/// <summary>
		/// Tests if the interior of the <code>Shape</code> entirely contains
		/// the specified rectangular area.  All coordinates that lie inside
		/// the rectangular area must lie within the <code>Shape</code> for the
		/// entire rectangular area to be considered contained within the
		/// <code>Shape</code>.
		/// <para>
		/// The {@code Shape.contains()} method allows a {@code Shape}
		/// implementation to conservatively return {@code false} when:
		/// <ul>
		/// <li>
		/// the <code>intersect</code> method returns <code>true</code> and
		/// <li>
		/// the calculations to determine whether or not the
		/// <code>Shape</code> entirely contains the rectangular area are
		/// prohibitively expensive.
		/// </ul>
		/// This means that for some {@code Shapes} this method might
		/// return {@code false} even though the {@code Shape} contains
		/// the rectangular area.
		/// The <seealso cref="java.awt.geom.Area Area"/> class performs
		/// more accurate geometric computations than most
		/// {@code Shape} objects and therefore can be used if a more precise
		/// answer is required.
		/// 
		/// </para>
		/// </summary>
		/// <param name="x"> the X coordinate of the upper-left corner
		///          of the specified rectangular area </param>
		/// <param name="y"> the Y coordinate of the upper-left corner
		///          of the specified rectangular area </param>
		/// <param name="w"> the width of the specified rectangular area </param>
		/// <param name="h"> the height of the specified rectangular area </param>
		/// <returns> <code>true</code> if the interior of the <code>Shape</code>
		///          entirely contains the specified rectangular area;
		///          <code>false</code> otherwise or, if the <code>Shape</code>
		///          contains the rectangular area and the
		///          <code>intersects</code> method returns <code>true</code>
		///          and the containment calculations would be too expensive to
		///          perform. </returns>
		/// <seealso cref= java.awt.geom.Area </seealso>
		/// <seealso cref= #intersects
		/// @since 1.2 </seealso>
		bool Contains(double x, double y, double w, double h);

		/// <summary>
		/// Tests if the interior of the <code>Shape</code> entirely contains the
		/// specified <code>Rectangle2D</code>.
		/// The {@code Shape.contains()} method allows a {@code Shape}
		/// implementation to conservatively return {@code false} when:
		/// <ul>
		/// <li>
		/// the <code>intersect</code> method returns <code>true</code> and
		/// <li>
		/// the calculations to determine whether or not the
		/// <code>Shape</code> entirely contains the <code>Rectangle2D</code>
		/// are prohibitively expensive.
		/// </ul>
		/// This means that for some {@code Shapes} this method might
		/// return {@code false} even though the {@code Shape} contains
		/// the {@code Rectangle2D}.
		/// The <seealso cref="java.awt.geom.Area Area"/> class performs
		/// more accurate geometric computations than most
		/// {@code Shape} objects and therefore can be used if a more precise
		/// answer is required.
		/// </summary>
		/// <param name="r"> The specified <code>Rectangle2D</code> </param>
		/// <returns> <code>true</code> if the interior of the <code>Shape</code>
		///          entirely contains the <code>Rectangle2D</code>;
		///          <code>false</code> otherwise or, if the <code>Shape</code>
		///          contains the <code>Rectangle2D</code> and the
		///          <code>intersects</code> method returns <code>true</code>
		///          and the containment calculations would be too expensive to
		///          perform. </returns>
		/// <seealso cref= #contains(double, double, double, double)
		/// @since 1.2 </seealso>
		bool Contains(Rectangle2D r);

		/// <summary>
		/// Returns an iterator object that iterates along the
		/// <code>Shape</code> boundary and provides access to the geometry of the
		/// <code>Shape</code> outline.  If an optional <seealso cref="AffineTransform"/>
		/// is specified, the coordinates returned in the iteration are
		/// transformed accordingly.
		/// <para>
		/// Each call to this method returns a fresh <code>PathIterator</code>
		/// object that traverses the geometry of the <code>Shape</code> object
		/// independently from any other <code>PathIterator</code> objects in use
		/// at the same time.
		/// </para>
		/// <para>
		/// It is recommended, but not guaranteed, that objects
		/// implementing the <code>Shape</code> interface isolate iterations
		/// that are in process from any changes that might occur to the original
		/// object's geometry during such iterations.
		/// 
		/// </para>
		/// </summary>
		/// <param name="at"> an optional <code>AffineTransform</code> to be applied to the
		///          coordinates as they are returned in the iteration, or
		///          <code>null</code> if untransformed coordinates are desired </param>
		/// <returns> a new <code>PathIterator</code> object, which independently
		///          traverses the geometry of the <code>Shape</code>.
		/// @since 1.2 </returns>
		PathIterator GetPathIterator(AffineTransform at);

		/// <summary>
		/// Returns an iterator object that iterates along the <code>Shape</code>
		/// boundary and provides access to a flattened view of the
		/// <code>Shape</code> outline geometry.
		/// <para>
		/// Only SEG_MOVETO, SEG_LINETO, and SEG_CLOSE point types are
		/// returned by the iterator.
		/// </para>
		/// <para>
		/// If an optional <code>AffineTransform</code> is specified,
		/// the coordinates returned in the iteration are transformed
		/// accordingly.
		/// </para>
		/// <para>
		/// The amount of subdivision of the curved segments is controlled
		/// by the <code>flatness</code> parameter, which specifies the
		/// maximum distance that any point on the unflattened transformed
		/// curve can deviate from the returned flattened path segments.
		/// Note that a limit on the accuracy of the flattened path might be
		/// silently imposed, causing very small flattening parameters to be
		/// treated as larger values.  This limit, if there is one, is
		/// defined by the particular implementation that is used.
		/// </para>
		/// <para>
		/// Each call to this method returns a fresh <code>PathIterator</code>
		/// object that traverses the <code>Shape</code> object geometry
		/// independently from any other <code>PathIterator</code> objects in use at
		/// the same time.
		/// </para>
		/// <para>
		/// It is recommended, but not guaranteed, that objects
		/// implementing the <code>Shape</code> interface isolate iterations
		/// that are in process from any changes that might occur to the original
		/// object's geometry during such iterations.
		/// 
		/// </para>
		/// </summary>
		/// <param name="at"> an optional <code>AffineTransform</code> to be applied to the
		///          coordinates as they are returned in the iteration, or
		///          <code>null</code> if untransformed coordinates are desired </param>
		/// <param name="flatness"> the maximum distance that the line segments used to
		///          approximate the curved segments are allowed to deviate
		///          from any point on the original curve </param>
		/// <returns> a new <code>PathIterator</code> that independently traverses
		///         a flattened view of the geometry of the  <code>Shape</code>.
		/// @since 1.2 </returns>
		PathIterator GetPathIterator(AffineTransform at, double flatness);
	}

}