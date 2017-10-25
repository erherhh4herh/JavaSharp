using System;

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
	/// The <code>ShapeGraphicAttribute</code> class is an implementation of
	/// <seealso cref="GraphicAttribute"/> that draws shapes in a <seealso cref="TextLayout"/>. </summary>
	/// <seealso cref= GraphicAttribute </seealso>
	public sealed class ShapeGraphicAttribute : GraphicAttribute
	{

		private Shape FShape;
		private bool FStroke;

		/// <summary>
		/// A key indicating the shape should be stroked with a 1-pixel wide stroke.
		/// </summary>
		public const bool STROKE = true;

		/// <summary>
		/// A key indicating the shape should be filled.
		/// </summary>
		public const bool FILL = false;

		// cache shape bounds, since GeneralPath doesn't
		private Rectangle2D FShapeBounds;

		/// <summary>
		/// Constructs a <code>ShapeGraphicAttribute</code> for the specified
		/// <seealso cref="Shape"/>. </summary>
		/// <param name="shape"> the <code>Shape</code> to render.  The
		/// <code>Shape</code> is rendered with its origin at the origin of
		/// this <code>ShapeGraphicAttribute</code> in the
		/// host <code>TextLayout</code>.  This object maintains a reference to
		/// <code>shape</code>. </param>
		/// <param name="alignment"> one of the alignments from this
		/// <code>ShapeGraphicAttribute</code>. </param>
		/// <param name="stroke"> <code>true</code> if the <code>Shape</code> should be
		/// stroked; <code>false</code> if the <code>Shape</code> should be
		/// filled. </param>
		public ShapeGraphicAttribute(Shape shape, int alignment, bool stroke) : base(alignment)
		{


			FShape = shape;
			FStroke = stroke;
			FShapeBounds = FShape.Bounds2D;
		}

		/// <summary>
		/// Returns the ascent of this <code>ShapeGraphicAttribute</code>.  The
		/// ascent of a <code>ShapeGraphicAttribute</code> is the positive
		/// distance from the origin of its <code>Shape</code> to the top of
		/// bounds of its <code>Shape</code>. </summary>
		/// <returns> the ascent of this <code>ShapeGraphicAttribute</code>. </returns>
		public override float Ascent
		{
			get
			{
    
				return (float) System.Math.Max(0, -FShapeBounds.MinY);
			}
		}

		/// <summary>
		/// Returns the descent of this <code>ShapeGraphicAttribute</code>.
		/// The descent of a <code>ShapeGraphicAttribute</code> is the distance
		/// from the origin of its <code>Shape</code> to the bottom of the
		/// bounds of its <code>Shape</code>. </summary>
		/// <returns> the descent of this <code>ShapeGraphicAttribute</code>. </returns>
		public override float Descent
		{
			get
			{
    
				return (float) System.Math.Max(0, FShapeBounds.MaxY);
			}
		}

		/// <summary>
		/// Returns the advance of this <code>ShapeGraphicAttribute</code>.
		/// The advance of a <code>ShapeGraphicAttribute</code> is the distance
		/// from the origin of its <code>Shape</code> to the right side of the
		/// bounds of its <code>Shape</code>. </summary>
		/// <returns> the advance of this <code>ShapeGraphicAttribute</code>. </returns>
		public override float Advance
		{
			get
			{
    
				return (float) System.Math.Max(0, FShapeBounds.MaxX);
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override void Draw(Graphics2D graphics, float x, float y)
		{

			// translating graphics to draw Shape !!!
			graphics.Translate((int)x, (int)y);

			try
			{
				if (FStroke == STROKE)
				{
					// REMIND: set stroke to correct size
					graphics.Draw(FShape);
				}
				else
				{
					graphics.Fill(FShape);
				}
			}
			finally
			{
				graphics.Translate(-(int)x, -(int)y);
			}
		}

		/// <summary>
		/// Returns a <seealso cref="Rectangle2D"/> that encloses all of the
		/// bits drawn by this <code>ShapeGraphicAttribute</code> relative to
		/// the rendering position.  A graphic can be rendered beyond its
		/// origin, ascent, descent, or advance;  but if it does, this method's
		/// implementation should indicate where the graphic is rendered. </summary>
		/// <returns> a <code>Rectangle2D</code> that encloses all of the bits
		/// rendered by this <code>ShapeGraphicAttribute</code>. </returns>
		public override Rectangle2D Bounds
		{
			get
			{
    
				Rectangle2D.Float bounds = new Rectangle2D.Float();
				bounds.Rect = FShapeBounds;
    
				if (FStroke == STROKE)
				{
					++bounds.Width_Renamed;
					++bounds.Height_Renamed;
				}
    
				return bounds;
			}
		}

		/// <summary>
		/// Return a <seealso cref="java.awt.Shape"/> that represents the region that
		/// this <code>ShapeGraphicAttribute</code> renders.  This is used when a
		/// <seealso cref="TextLayout"/> is requested to return the outline of the text.
		/// The (untransformed) shape must not extend outside the rectangular
		/// bounds returned by <code>getBounds</code>. </summary>
		/// <param name="tx"> an optional <seealso cref="AffineTransform"/> to apply to the
		///   this <code>ShapeGraphicAttribute</code>. This can be null. </param>
		/// <returns> the <code>Shape</code> representing this graphic attribute,
		///   suitable for stroking or filling.
		/// @since 1.6 </returns>
		public override Shape GetOutline(AffineTransform tx)
		{
			return tx == null ? FShape : tx.CreateTransformedShape(FShape);
		}

		/// <summary>
		/// Returns a hashcode for this <code>ShapeGraphicAttribute</code>. </summary>
		/// <returns>  a hash code value for this
		/// <code>ShapeGraphicAttribute</code>. </returns>
		public override int HashCode()
		{

			return FShape.HashCode();
		}

		/// <summary>
		/// Compares this <code>ShapeGraphicAttribute</code> to the specified
		/// <code>Object</code>. </summary>
		/// <param name="rhs"> the <code>Object</code> to compare for equality </param>
		/// <returns> <code>true</code> if this
		/// <code>ShapeGraphicAttribute</code> equals <code>rhs</code>;
		/// <code>false</code> otherwise. </returns>
		public override bool Equals(Object rhs)
		{

			try
			{
				return Equals((ShapeGraphicAttribute) rhs);
			}
			catch (ClassCastException)
			{
				return false;
			}
		}

		/// <summary>
		/// Compares this <code>ShapeGraphicAttribute</code> to the specified
		/// <code>ShapeGraphicAttribute</code>. </summary>
		/// <param name="rhs"> the <code>ShapeGraphicAttribute</code> to compare for
		/// equality </param>
		/// <returns> <code>true</code> if this
		/// <code>ShapeGraphicAttribute</code> equals <code>rhs</code>;
		/// <code>false</code> otherwise. </returns>
		public bool Equals(ShapeGraphicAttribute rhs)
		{

			if (rhs == null)
			{
				return false;
			}

			if (this == rhs)
			{
				return true;
			}

			if (FStroke != rhs.FStroke)
			{
				return false;
			}

			if (Alignment != rhs.Alignment)
			{
				return false;
			}

			if (!FShape.Equals(rhs.FShape))
			{
				return false;
			}

			return true;
		}
	}

}