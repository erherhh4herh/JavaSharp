/*
 * Copyright (c) 1996, 1999, Oracle and/or its affiliates. All rights reserved.
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
	/// The <code>Stroke</code> interface allows a
	/// <seealso cref="Graphics2D"/> object to obtain a <seealso cref="Shape"/> that is the
	/// decorated outline, or stylistic representation of the outline,
	/// of the specified <code>Shape</code>.
	/// Stroking a <code>Shape</code> is like tracing its outline with a
	/// marking pen of the appropriate size and shape.
	/// The area where the pen would place ink is the area enclosed by the
	/// outline <code>Shape</code>.
	/// <para>
	/// The methods of the <code>Graphics2D</code> interface that use the
	/// outline <code>Shape</code> returned by a <code>Stroke</code> object
	/// include <code>draw</code> and any other methods that are
	/// implemented in terms of that method, such as
	/// <code>drawLine</code>, <code>drawRect</code>,
	/// <code>drawRoundRect</code>, <code>drawOval</code>,
	/// <code>drawArc</code>, <code>drawPolyline</code>,
	/// and <code>drawPolygon</code>.
	/// </para>
	/// <para>
	/// The objects of the classes implementing <code>Stroke</code>
	/// must be read-only because <code>Graphics2D</code> does not
	/// clone these objects either when they are set as an attribute
	/// with the <code>setStroke</code> method or when the
	/// <code>Graphics2D</code> object is itself cloned.
	/// If a <code>Stroke</code> object is modified after it is set in
	/// the <code>Graphics2D</code> context then the behavior
	/// of subsequent rendering would be undefined.
	/// </para>
	/// </summary>
	/// <seealso cref= BasicStroke </seealso>
	/// <seealso cref= Graphics2D#setStroke </seealso>
	public interface Stroke
	{
		/// <summary>
		/// Returns an outline <code>Shape</code> which encloses the area that
		/// should be painted when the <code>Shape</code> is stroked according
		/// to the rules defined by the
		/// object implementing the <code>Stroke</code> interface. </summary>
		/// <param name="p"> a <code>Shape</code> to be stroked </param>
		/// <returns> the stroked outline <code>Shape</code>. </returns>
		Shape CreateStrokedShape(Shape p);
	}

}