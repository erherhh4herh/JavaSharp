/*
 * Copyright (c) 1997, 1998, Oracle and/or its affiliates. All rights reserved.
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
	/// The <code>Composite</code> interface, along with
	/// <seealso cref="CompositeContext"/>, defines the methods to compose a draw
	/// primitive with the underlying graphics area.
	/// After the <code>Composite</code> is set in the
	/// <seealso cref="Graphics2D"/> context, it combines a shape, text, or an image
	/// being rendered with the colors that have already been rendered
	/// according to pre-defined rules. The classes
	/// implementing this interface provide the rules and a method to create
	/// the context for a particular operation.
	/// <code>CompositeContext</code> is an environment used by the
	/// compositing operation, which is created by the <code>Graphics2D</code>
	/// prior to the start of the operation.  <code>CompositeContext</code>
	/// contains private information and resources needed for a compositing
	/// operation.  When the <code>CompositeContext</code> is no longer needed,
	/// the <code>Graphics2D</code> object disposes of it in order to reclaim
	/// resources allocated for the operation.
	/// <para>
	/// Instances of classes implementing <code>Composite</code> must be
	/// immutable because the <code>Graphics2D</code> does not clone
	/// these objects when they are set as an attribute with the
	/// <code>setComposite</code> method or when the <code>Graphics2D</code>
	/// object is cloned.  This is to avoid undefined rendering behavior of
	/// <code>Graphics2D</code>, resulting from the modification of
	/// the <code>Composite</code> object after it has been set in the
	/// <code>Graphics2D</code> context.
	/// </para>
	/// <para>
	/// Since this interface must expose the contents of pixels on the
	/// target device or image to potentially arbitrary code, the use of
	/// custom objects which implement this interface when rendering directly
	/// to a screen device is governed by the <code>readDisplayPixels</code>
	/// <seealso cref="AWTPermission"/>.  The permission check will occur when such
	/// a custom object is passed to the <code>setComposite</code> method
	/// of a <code>Graphics2D</code> retrieved from a <seealso cref="Component"/>.
	/// </para>
	/// </summary>
	/// <seealso cref= AlphaComposite </seealso>
	/// <seealso cref= CompositeContext </seealso>
	/// <seealso cref= Graphics2D#setComposite </seealso>
	public interface Composite
	{

		/// <summary>
		/// Creates a context containing state that is used to perform
		/// the compositing operation.  In a multi-threaded environment,
		/// several contexts can exist simultaneously for a single
		/// <code>Composite</code> object. </summary>
		/// <param name="srcColorModel">  the <seealso cref="ColorModel"/> of the source </param>
		/// <param name="dstColorModel">  the <code>ColorModel</code> of the destination </param>
		/// <param name="hints"> the hint that the context object uses to choose between
		/// rendering alternatives </param>
		/// <returns> the <code>CompositeContext</code> object used to perform the
		/// compositing operation. </returns>
		CompositeContext CreateContext(ColorModel srcColorModel, ColorModel dstColorModel, RenderingHints hints);

	}

}