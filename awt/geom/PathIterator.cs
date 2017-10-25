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
	/// The <code>PathIterator</code> interface provides the mechanism
	/// for objects that implement the <seealso cref="java.awt.Shape Shape"/>
	/// interface to return the geometry of their boundary by allowing
	/// a caller to retrieve the path of that boundary a segment at a
	/// time.  This interface allows these objects to retrieve the path of
	/// their boundary a segment at a time by using 1st through 3rd order
	/// B&eacute;zier curves, which are lines and quadratic or cubic
	/// B&eacute;zier splines.
	/// <para>
	/// Multiple subpaths can be expressed by using a "MOVETO" segment to
	/// create a discontinuity in the geometry to move from the end of
	/// one subpath to the beginning of the next.
	/// </para>
	/// <para>
	/// Each subpath can be closed manually by ending the last segment in
	/// the subpath on the same coordinate as the beginning "MOVETO" segment
	/// for that subpath or by using a "CLOSE" segment to append a line
	/// segment from the last point back to the first.
	/// Be aware that manually closing an outline as opposed to using a
	/// "CLOSE" segment to close the path might result in different line
	/// style decorations being used at the end points of the subpath.
	/// For example, the <seealso cref="java.awt.BasicStroke BasicStroke"/> object
	/// uses a line "JOIN" decoration to connect the first and last points
	/// if a "CLOSE" segment is encountered, whereas simply ending the path
	/// on the same coordinate as the beginning coordinate results in line
	/// "CAP" decorations being used at the ends.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= java.awt.Shape </seealso>
	/// <seealso cref= java.awt.BasicStroke
	/// 
	/// @author Jim Graham </seealso>
	public interface PathIterator
	{
		/// <summary>
		/// The winding rule constant for specifying an even-odd rule
		/// for determining the interior of a path.
		/// The even-odd rule specifies that a point lies inside the
		/// path if a ray drawn in any direction from that point to
		/// infinity is crossed by path segments an odd number of times.
		/// </summary>

		/// <summary>
		/// The winding rule constant for specifying a non-zero rule
		/// for determining the interior of a path.
		/// The non-zero rule specifies that a point lies inside the
		/// path if a ray drawn in any direction from that point to
		/// infinity is crossed by path segments a different number
		/// of times in the counter-clockwise direction than the
		/// clockwise direction.
		/// </summary>

		/// <summary>
		/// The segment type constant for a point that specifies the
		/// starting location for a new subpath.
		/// </summary>

		/// <summary>
		/// The segment type constant for a point that specifies the
		/// end point of a line to be drawn from the most recently
		/// specified point.
		/// </summary>

		/// <summary>
		/// The segment type constant for the pair of points that specify
		/// a quadratic parametric curve to be drawn from the most recently
		/// specified point.
		/// The curve is interpolated by solving the parametric control
		/// equation in the range <code>(t=[0..1])</code> using
		/// the most recently specified (current) point (CP),
		/// the first control point (P1),
		/// and the final interpolated control point (P2).
		/// The parametric control equation for this curve is:
		/// <pre>
		///          P(t) = B(2,0)*CP + B(2,1)*P1 + B(2,2)*P2
		///          0 &lt;= t &lt;= 1
		/// 
		///        B(n,m) = mth coefficient of nth degree Bernstein polynomial
		///               = C(n,m) * t^(m) * (1 - t)^(n-m)
		///        C(n,m) = Combinations of n things, taken m at a time
		///               = n! / (m! * (n-m)!)
		/// </pre>
		/// </summary>

		/// <summary>
		/// The segment type constant for the set of 3 points that specify
		/// a cubic parametric curve to be drawn from the most recently
		/// specified point.
		/// The curve is interpolated by solving the parametric control
		/// equation in the range <code>(t=[0..1])</code> using
		/// the most recently specified (current) point (CP),
		/// the first control point (P1),
		/// the second control point (P2),
		/// and the final interpolated control point (P3).
		/// The parametric control equation for this curve is:
		/// <pre>
		///          P(t) = B(3,0)*CP + B(3,1)*P1 + B(3,2)*P2 + B(3,3)*P3
		///          0 &lt;= t &lt;= 1
		/// 
		///        B(n,m) = mth coefficient of nth degree Bernstein polynomial
		///               = C(n,m) * t^(m) * (1 - t)^(n-m)
		///        C(n,m) = Combinations of n things, taken m at a time
		///               = n! / (m! * (n-m)!)
		/// </pre>
		/// This form of curve is commonly known as a B&eacute;zier curve.
		/// </summary>

		/// <summary>
		/// The segment type constant that specifies that
		/// the preceding subpath should be closed by appending a line segment
		/// back to the point corresponding to the most recent SEG_MOVETO.
		/// </summary>

		/// <summary>
		/// Returns the winding rule for determining the interior of the
		/// path. </summary>
		/// <returns> the winding rule. </returns>
		/// <seealso cref= #WIND_EVEN_ODD </seealso>
		/// <seealso cref= #WIND_NON_ZERO </seealso>
		int WindingRule {get;}

		/// <summary>
		/// Tests if the iteration is complete. </summary>
		/// <returns> <code>true</code> if all the segments have
		/// been read; <code>false</code> otherwise. </returns>
		bool Done {get;}

		/// <summary>
		/// Moves the iterator to the next segment of the path forwards
		/// along the primary direction of traversal as long as there are
		/// more points in that direction.
		/// </summary>
		void Next();

		/// <summary>
		/// Returns the coordinates and type of the current path segment in
		/// the iteration.
		/// The return value is the path-segment type:
		/// SEG_MOVETO, SEG_LINETO, SEG_QUADTO, SEG_CUBICTO, or SEG_CLOSE.
		/// A float array of length 6 must be passed in and can be used to
		/// store the coordinates of the point(s).
		/// Each point is stored as a pair of float x,y coordinates.
		/// SEG_MOVETO and SEG_LINETO types returns one point,
		/// SEG_QUADTO returns two points,
		/// SEG_CUBICTO returns 3 points
		/// and SEG_CLOSE does not return any points. </summary>
		/// <param name="coords"> an array that holds the data returned from
		/// this method </param>
		/// <returns> the path-segment type of the current path segment. </returns>
		/// <seealso cref= #SEG_MOVETO </seealso>
		/// <seealso cref= #SEG_LINETO </seealso>
		/// <seealso cref= #SEG_QUADTO </seealso>
		/// <seealso cref= #SEG_CUBICTO </seealso>
		/// <seealso cref= #SEG_CLOSE </seealso>
		int CurrentSegment(float[] coords);

		/// <summary>
		/// Returns the coordinates and type of the current path segment in
		/// the iteration.
		/// The return value is the path-segment type:
		/// SEG_MOVETO, SEG_LINETO, SEG_QUADTO, SEG_CUBICTO, or SEG_CLOSE.
		/// A double array of length 6 must be passed in and can be used to
		/// store the coordinates of the point(s).
		/// Each point is stored as a pair of double x,y coordinates.
		/// SEG_MOVETO and SEG_LINETO types returns one point,
		/// SEG_QUADTO returns two points,
		/// SEG_CUBICTO returns 3 points
		/// and SEG_CLOSE does not return any points. </summary>
		/// <param name="coords"> an array that holds the data returned from
		/// this method </param>
		/// <returns> the path-segment type of the current path segment. </returns>
		/// <seealso cref= #SEG_MOVETO </seealso>
		/// <seealso cref= #SEG_LINETO </seealso>
		/// <seealso cref= #SEG_QUADTO </seealso>
		/// <seealso cref= #SEG_CUBICTO </seealso>
		/// <seealso cref= #SEG_CLOSE </seealso>
		int CurrentSegment(double[] coords);
	}

	public static class PathIterator_Fields
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public static final int WIND_EVEN_ODD = 0;
		public const int WIND_EVEN_ODD = 0;
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public static final int WIND_NON_ZERO = 1;
		public const int WIND_NON_ZERO = 1;
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public static final int SEG_MOVETO = 0;
		public const int SEG_MOVETO = 0;
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public static final int SEG_LINETO = 1;
		public const int SEG_LINETO = 1;
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public static final int SEG_QUADTO = 2;
		public const int SEG_QUADTO = 2;
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public static final int SEG_CUBICTO = 3;
		public const int SEG_CUBICTO = 3;
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public static final int SEG_CLOSE = 4;
		public const int SEG_CLOSE = 4;
	}

}