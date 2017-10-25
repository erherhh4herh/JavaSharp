/*
 * Copyright (c) 1997, 2008, Oracle and/or its affiliates. All rights reserved.
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
	/// This <code>Paint</code> interface defines how color patterns
	/// can be generated for <seealso cref="Graphics2D"/> operations.  A class
	/// implementing the <code>Paint</code> interface is added to the
	/// <code>Graphics2D</code> context in order to define the color
	/// pattern used by the <code>draw</code> and <code>fill</code> methods.
	/// <para>
	/// Instances of classes implementing <code>Paint</code> must be
	/// read-only because the <code>Graphics2D</code> does not clone
	/// these objects when they are set as an attribute with the
	/// <code>setPaint</code> method or when the <code>Graphics2D</code>
	/// object is itself cloned.
	/// </para>
	/// </summary>
	/// <seealso cref= PaintContext </seealso>
	/// <seealso cref= Color </seealso>
	/// <seealso cref= GradientPaint </seealso>
	/// <seealso cref= TexturePaint </seealso>
	/// <seealso cref= Graphics2D#setPaint
	/// @version 1.36, 06/05/07 </seealso>

	public interface Paint : Transparency
	{
		/// <summary>
		/// Creates and returns a <seealso cref="PaintContext"/> used to
		/// generate the color pattern.
		/// The arguments to this method convey additional information
		/// about the rendering operation that may be
		/// used or ignored on various implementations of the {@code Paint} interface.
		/// A caller must pass non-{@code null} values for all of the arguments
		/// except for the {@code ColorModel} argument which may be {@code null} to
		/// indicate that no specific {@code ColorModel} type is preferred.
		/// Implementations of the {@code Paint} interface are allowed to use or ignore
		/// any of the arguments as makes sense for their function, and are
		/// not constrained to use the specified {@code ColorModel} for the returned
		/// {@code PaintContext}, even if it is not {@code null}.
		/// Implementations are allowed to throw {@code NullPointerException} for
		/// any {@code null} argument other than the {@code ColorModel} argument,
		/// but are not required to do so.
		/// </summary>
		/// <param name="cm"> the preferred <seealso cref="ColorModel"/> which represents the most convenient
		///           format for the caller to receive the pixel data, or {@code null}
		///           if there is no preference. </param>
		/// <param name="deviceBounds"> the device space bounding box
		///                     of the graphics primitive being rendered.
		///                     Implementations of the {@code Paint} interface
		///                     are allowed to throw {@code NullPointerException}
		///                     for a {@code null} {@code deviceBounds}. </param>
		/// <param name="userBounds"> the user space bounding box
		///                   of the graphics primitive being rendered.
		///                     Implementations of the {@code Paint} interface
		///                     are allowed to throw {@code NullPointerException}
		///                     for a {@code null} {@code userBounds}. </param>
		/// <param name="xform"> the <seealso cref="AffineTransform"/> from user
		///              space into device space.
		///                     Implementations of the {@code Paint} interface
		///                     are allowed to throw {@code NullPointerException}
		///                     for a {@code null} {@code xform}. </param>
		/// <param name="hints"> the set of hints that the context object can use to
		///              choose between rendering alternatives.
		///                     Implementations of the {@code Paint} interface
		///                     are allowed to throw {@code NullPointerException}
		///                     for a {@code null} {@code hints}. </param>
		/// <returns> the {@code PaintContext} for
		///         generating color patterns. </returns>
		/// <seealso cref= PaintContext </seealso>
		/// <seealso cref= ColorModel </seealso>
		/// <seealso cref= Rectangle </seealso>
		/// <seealso cref= Rectangle2D </seealso>
		/// <seealso cref= AffineTransform </seealso>
		/// <seealso cref= RenderingHints </seealso>
		PaintContext CreateContext(ColorModel cm, Rectangle deviceBounds, Rectangle2D userBounds, AffineTransform xform, RenderingHints hints);

	}

}