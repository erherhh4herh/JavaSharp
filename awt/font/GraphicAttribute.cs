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

/*
 * (C) Copyright Taligent, Inc. 1996 - 1997, All Rights Reserved
 * (C) Copyright IBM Corp. 1996 - 1998, All Rights Reserved
 *
 * The original version of this source code and documentation is
 * copyrighted and owned by Taligent, Inc., a wholly-owned subsidiary
 * of IBM. These materials are provided under terms of a License
 * Agreement between Taligent and Sun. This technology is protected
 * by multiple US and International patents.
 *
 * This notice and attribution to Taligent may not be removed.
 * Taligent is a registered trademark of Taligent, Inc.
 *
 */

namespace java.awt.font
{


	/// <summary>
	/// This class is used with the CHAR_REPLACEMENT attribute.
	/// <para>
	/// The <code>GraphicAttribute</code> class represents a graphic embedded
	/// in text. Clients subclass this class to implement their own char
	/// replacement graphics.  Clients wishing to embed shapes and images in
	/// text need not subclass this class.  Instead, clients can use the
	/// <seealso cref="ShapeGraphicAttribute"/> and <seealso cref="ImageGraphicAttribute"/>
	/// classes.
	/// </para>
	/// <para>
	/// Subclasses must ensure that their objects are immutable once they
	/// are constructed.  Mutating a <code>GraphicAttribute</code> that
	/// is used in a <seealso cref="TextLayout"/> results in undefined behavior from the
	/// <code>TextLayout</code>.
	/// </para>
	/// </summary>
	public abstract class GraphicAttribute
	{

		private int FAlignment;

		/// <summary>
		/// Aligns top of graphic to top of line.
		/// </summary>
		public const int TOP_ALIGNMENT = -1;

		/// <summary>
		/// Aligns bottom of graphic to bottom of line.
		/// </summary>
		public const int BOTTOM_ALIGNMENT = -2;

		/// <summary>
		/// Aligns origin of graphic to roman baseline of line.
		/// </summary>
		public const int ROMAN_BASELINE = Font.ROMAN_BASELINE;

		/// <summary>
		/// Aligns origin of graphic to center baseline of line.
		/// </summary>
		public const int CENTER_BASELINE = Font.CENTER_BASELINE;

		/// <summary>
		/// Aligns origin of graphic to hanging baseline of line.
		/// </summary>
		public const int HANGING_BASELINE = Font.HANGING_BASELINE;

		/// <summary>
		/// Constructs a <code>GraphicAttribute</code>.
		/// Subclasses use this to define the alignment of the graphic. </summary>
		/// <param name="alignment"> an int representing one of the
		/// <code>GraphicAttribute</code> alignment fields </param>
		/// <exception cref="IllegalArgumentException"> if alignment is not one of the
		/// five defined values. </exception>
		protected internal GraphicAttribute(int alignment)
		{
			if (alignment < BOTTOM_ALIGNMENT || alignment > HANGING_BASELINE)
			{
			  throw new IllegalArgumentException("bad alignment");
			}
			FAlignment = alignment;
		}

		/// <summary>
		/// Returns the ascent of this <code>GraphicAttribute</code>.  A
		/// graphic can be rendered above its ascent. </summary>
		/// <returns> the ascent of this <code>GraphicAttribute</code>. </returns>
		/// <seealso cref= #getBounds() </seealso>
		public abstract float Ascent {get;}


		/// <summary>
		/// Returns the descent of this <code>GraphicAttribute</code>.  A
		/// graphic can be rendered below its descent. </summary>
		/// <returns> the descent of this <code>GraphicAttribute</code>. </returns>
		/// <seealso cref= #getBounds() </seealso>
		public abstract float Descent {get;}

		/// <summary>
		/// Returns the advance of this <code>GraphicAttribute</code>.  The
		/// <code>GraphicAttribute</code> object's advance is the distance
		/// from the point at which the graphic is rendered and the point where
		/// the next character or graphic is rendered.  A graphic can be
		/// rendered beyond its advance </summary>
		/// <returns> the advance of this <code>GraphicAttribute</code>. </returns>
		/// <seealso cref= #getBounds() </seealso>
		public abstract float Advance {get;}

		/// <summary>
		/// Returns a <seealso cref="Rectangle2D"/> that encloses all of the
		/// bits drawn by this <code>GraphicAttribute</code> relative to the
		/// rendering position.
		/// A graphic may be rendered beyond its origin, ascent, descent,
		/// or advance;  but if it is, this method's implementation must
		/// indicate where the graphic is rendered.
		/// Default bounds is the rectangle (0, -ascent, advance, ascent+descent). </summary>
		/// <returns> a <code>Rectangle2D</code> that encloses all of the bits
		/// rendered by this <code>GraphicAttribute</code>. </returns>
		public virtual Rectangle2D Bounds
		{
			get
			{
				float ascent = Ascent;
				return new Rectangle2D.Float(0, -ascent, Advance, ascent + Descent);
			}
		}

		/// <summary>
		/// Return a <seealso cref="java.awt.Shape"/> that represents the region that
		/// this <code>GraphicAttribute</code> renders.  This is used when a
		/// <seealso cref="TextLayout"/> is requested to return the outline of the text.
		/// The (untransformed) shape must not extend outside the rectangular
		/// bounds returned by <code>getBounds</code>.
		/// The default implementation returns the rectangle returned by
		/// <seealso cref="#getBounds"/>, transformed by the provided <seealso cref="AffineTransform"/>
		/// if present. </summary>
		/// <param name="tx"> an optional <seealso cref="AffineTransform"/> to apply to the
		///   outline of this <code>GraphicAttribute</code>. This can be null. </param>
		/// <returns> a <code>Shape</code> representing this graphic attribute,
		///   suitable for stroking or filling.
		/// @since 1.6 </returns>
		public virtual Shape GetOutline(AffineTransform tx)
		{
			Shape b = Bounds;
			if (tx != null)
			{
				b = tx.CreateTransformedShape(b);
			}
			return b;
		}

		/// <summary>
		/// Renders this <code>GraphicAttribute</code> at the specified
		/// location. </summary>
		/// <param name="graphics"> the <seealso cref="Graphics2D"/> into which to render the
		/// graphic </param>
		/// <param name="x"> the user-space X coordinate where the graphic is rendered </param>
		/// <param name="y"> the user-space Y coordinate where the graphic is rendered </param>
		public abstract void Draw(Graphics2D graphics, float x, float y);

		/// <summary>
		/// Returns the alignment of this <code>GraphicAttribute</code>.
		/// Alignment can be to a particular baseline, or to the absolute top
		/// or bottom of a line. </summary>
		/// <returns> the alignment of this <code>GraphicAttribute</code>. </returns>
		public int Alignment
		{
			get
			{
    
				return FAlignment;
			}
		}

		/// <summary>
		/// Returns the justification information for this
		/// <code>GraphicAttribute</code>.  Subclasses
		/// can override this method to provide different justification
		/// information. </summary>
		/// <returns> a <seealso cref="GlyphJustificationInfo"/> object that contains the
		/// justification information for this <code>GraphicAttribute</code>. </returns>
		public virtual GlyphJustificationInfo JustificationInfo
		{
			get
			{
    
				// should we cache this?
				float advance = Advance;
    
				return new GlyphJustificationInfo(advance, false, 2, advance / 3, advance / 3, false, 1, 0, 0); // shrinkRightLimit -  shrinkLeftLimit -  shrinkPriority -  shrinkAbsorb -  growRightLimit -  growLeftLimit -  growPriority -  growAbsorb -  weight
			}
		}
	}

}