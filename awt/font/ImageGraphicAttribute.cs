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
	/// The <code>ImageGraphicAttribute</code> class is an implementation of
	/// <seealso cref="GraphicAttribute"/> which draws images in
	/// a <seealso cref="TextLayout"/>. </summary>
	/// <seealso cref= GraphicAttribute </seealso>

	public sealed class ImageGraphicAttribute : GraphicAttribute
	{

		private Image FImage;
		private float FImageWidth, FImageHeight;
		private float FOriginX, FOriginY;

		/// <summary>
		/// Constucts an <code>ImageGraphicAttribute</code> from the specified
		/// <seealso cref="Image"/>.  The origin is at (0,&nbsp;0). </summary>
		/// <param name="image"> the <code>Image</code> rendered by this
		/// <code>ImageGraphicAttribute</code>.
		/// This object keeps a reference to <code>image</code>. </param>
		/// <param name="alignment"> one of the alignments from this
		/// <code>ImageGraphicAttribute</code> </param>
		public ImageGraphicAttribute(Image image, int alignment) : this(image, alignment, 0, 0)
		{

		}

		/// <summary>
		/// Constructs an <code>ImageGraphicAttribute</code> from the specified
		/// <code>Image</code>. The point
		/// (<code>originX</code>,&nbsp;<code>originY</code>) in the
		/// <code>Image</code> appears at the origin of the
		/// <code>ImageGraphicAttribute</code> within the text. </summary>
		/// <param name="image"> the <code>Image</code> rendered by this
		/// <code>ImageGraphicAttribute</code>.
		/// This object keeps a reference to <code>image</code>. </param>
		/// <param name="alignment"> one of the alignments from this
		/// <code>ImageGraphicAttribute</code> </param>
		/// <param name="originX"> the X coordinate of the point within
		/// the <code>Image</code> that appears at the origin of the
		/// <code>ImageGraphicAttribute</code> in the text line. </param>
		/// <param name="originY"> the Y coordinate of the point within
		/// the <code>Image</code> that appears at the origin of the
		/// <code>ImageGraphicAttribute</code> in the text line. </param>
		public ImageGraphicAttribute(Image image, int alignment, float originX, float originY) : base(alignment)
		{


			// Can't clone image
			// fImage = (Image) image.clone();
			FImage = image;

			FImageWidth = image.GetWidth(null);
			FImageHeight = image.GetHeight(null);

			// ensure origin is in Image?
			FOriginX = originX;
			FOriginY = originY;
		}

		/// <summary>
		/// Returns the ascent of this <code>ImageGraphicAttribute</code>.  The
		/// ascent of an <code>ImageGraphicAttribute</code> is the distance
		/// from the top of the image to the origin. </summary>
		/// <returns> the ascent of this <code>ImageGraphicAttribute</code>. </returns>
		public override float Ascent
		{
			get
			{
    
				return System.Math.Max(0, FOriginY);
			}
		}

		/// <summary>
		/// Returns the descent of this <code>ImageGraphicAttribute</code>.
		/// The descent of an <code>ImageGraphicAttribute</code> is the
		/// distance from the origin to the bottom of the image. </summary>
		/// <returns> the descent of this <code>ImageGraphicAttribute</code>. </returns>
		public override float Descent
		{
			get
			{
    
				return System.Math.Max(0, FImageHeight - FOriginY);
			}
		}

		/// <summary>
		/// Returns the advance of this <code>ImageGraphicAttribute</code>.
		/// The advance of an <code>ImageGraphicAttribute</code> is the
		/// distance from the origin to the right edge of the image. </summary>
		/// <returns> the advance of this <code>ImageGraphicAttribute</code>. </returns>
		public override float Advance
		{
			get
			{
    
				return System.Math.Max(0, FImageWidth - FOriginX);
			}
		}

		/// <summary>
		/// Returns a <seealso cref="Rectangle2D"/> that encloses all of the
		/// bits rendered by this <code>ImageGraphicAttribute</code>, relative
		/// to the rendering position.  A graphic can be rendered beyond its
		/// origin, ascent, descent, or advance;  but if it is, this
		/// method's implementation must indicate where the graphic is rendered. </summary>
		/// <returns> a <code>Rectangle2D</code> that encloses all of the bits
		/// rendered by this <code>ImageGraphicAttribute</code>. </returns>
		public override Rectangle2D Bounds
		{
			get
			{
    
				return new Rectangle2D.Float(-FOriginX, -FOriginY, FImageWidth, FImageHeight);
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override void Draw(Graphics2D graphics, float x, float y)
		{

			graphics.DrawImage(FImage, (int)(x - FOriginX), (int)(y - FOriginY), null);
		}

		/// <summary>
		/// Returns a hashcode for this <code>ImageGraphicAttribute</code>. </summary>
		/// <returns>  a hash code value for this object. </returns>
		public override int HashCode()
		{

			return FImage.HashCode();
		}

		/// <summary>
		/// Compares this <code>ImageGraphicAttribute</code> to the specified
		/// <seealso cref="Object"/>. </summary>
		/// <param name="rhs"> the <code>Object</code> to compare for equality </param>
		/// <returns> <code>true</code> if this
		/// <code>ImageGraphicAttribute</code> equals <code>rhs</code>;
		/// <code>false</code> otherwise. </returns>
		public override bool Equals(Object rhs)
		{

			try
			{
				return Equals((ImageGraphicAttribute) rhs);
			}
			catch (ClassCastException)
			{
				return false;
			}
		}

		/// <summary>
		/// Compares this <code>ImageGraphicAttribute</code> to the specified
		/// <code>ImageGraphicAttribute</code>. </summary>
		/// <param name="rhs"> the <code>ImageGraphicAttribute</code> to compare for
		/// equality </param>
		/// <returns> <code>true</code> if this
		/// <code>ImageGraphicAttribute</code> equals <code>rhs</code>;
		/// <code>false</code> otherwise. </returns>
		public bool Equals(ImageGraphicAttribute rhs)
		{

			if (rhs == null)
			{
				return false;
			}

			if (this == rhs)
			{
				return true;
			}

			if (FOriginX != rhs.FOriginX || FOriginY != rhs.FOriginY)
			{
				return false;
			}

			if (Alignment != rhs.Alignment)
			{
				return false;
			}

			if (!FImage.Equals(rhs.FImage))
			{
				return false;
			}

			return true;
		}
	}

}