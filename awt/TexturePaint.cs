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
	/// The <code>TexturePaint</code> class provides a way to fill a
	/// <seealso cref="Shape"/> with a texture that is specified as
	/// a <seealso cref="BufferedImage"/>. The size of the <code>BufferedImage</code>
	/// object should be small because the <code>BufferedImage</code> data
	/// is copied by the <code>TexturePaint</code> object.
	/// At construction time, the texture is anchored to the upper
	/// left corner of a <seealso cref="Rectangle2D"/> that is
	/// specified in user space.  Texture is computed for
	/// locations in the device space by conceptually replicating the
	/// specified <code>Rectangle2D</code> infinitely in all directions
	/// in user space and mapping the <code>BufferedImage</code> to each
	/// replicated <code>Rectangle2D</code>. </summary>
	/// <seealso cref= Paint </seealso>
	/// <seealso cref= Graphics2D#setPaint
	/// @version 1.48, 06/05/07 </seealso>

	public class TexturePaint : Paint
	{

		internal BufferedImage BufImg;
		internal double Tx;
		internal double Ty;
		internal double Sx;
		internal double Sy;

		/// <summary>
		/// Constructs a <code>TexturePaint</code> object. </summary>
		/// <param name="txtr"> the <code>BufferedImage</code> object with the texture
		/// used for painting </param>
		/// <param name="anchor"> the <code>Rectangle2D</code> in user space used to
		/// anchor and replicate the texture </param>
		public TexturePaint(BufferedImage txtr, Rectangle2D anchor)
		{
			this.BufImg = txtr;
			this.Tx = anchor.X;
			this.Ty = anchor.Y;
			this.Sx = anchor.Width / BufImg.Width;
			this.Sy = anchor.Height / BufImg.Height;
		}

		/// <summary>
		/// Returns the <code>BufferedImage</code> texture used to
		/// fill the shapes. </summary>
		/// <returns> a <code>BufferedImage</code>. </returns>
		public virtual BufferedImage Image
		{
			get
			{
				return BufImg;
			}
		}

		/// <summary>
		/// Returns a copy of the anchor rectangle which positions and
		/// sizes the textured image. </summary>
		/// <returns> the <code>Rectangle2D</code> used to anchor and
		/// size this <code>TexturePaint</code>. </returns>
		public virtual Rectangle2D AnchorRect
		{
			get
			{
				return new Rectangle2D.Double(Tx, Ty, Sx * BufImg.Width, Sy * BufImg.Height);
			}
		}

		/// <summary>
		/// Creates and returns a <seealso cref="PaintContext"/> used to
		/// generate a tiled image pattern.
		/// See the <seealso cref="Paint#createContext specification"/> of the
		/// method in the <seealso cref="Paint"/> interface for information
		/// on null parameter handling.
		/// </summary>
		/// <param name="cm"> the preferred <seealso cref="ColorModel"/> which represents the most convenient
		///           format for the caller to receive the pixel data, or {@code null}
		///           if there is no preference. </param>
		/// <param name="deviceBounds"> the device space bounding box
		///                     of the graphics primitive being rendered. </param>
		/// <param name="userBounds"> the user space bounding box
		///                   of the graphics primitive being rendered. </param>
		/// <param name="xform"> the <seealso cref="AffineTransform"/> from user
		///              space into device space. </param>
		/// <param name="hints"> the set of hints that the context object can use to
		///              choose between rendering alternatives. </param>
		/// <returns> the {@code PaintContext} for
		///         generating color patterns. </returns>
		/// <seealso cref= Paint </seealso>
		/// <seealso cref= PaintContext </seealso>
		/// <seealso cref= ColorModel </seealso>
		/// <seealso cref= Rectangle </seealso>
		/// <seealso cref= Rectangle2D </seealso>
		/// <seealso cref= AffineTransform </seealso>
		/// <seealso cref= RenderingHints </seealso>
		public virtual PaintContext CreateContext(ColorModel cm, Rectangle deviceBounds, Rectangle2D userBounds, AffineTransform xform, RenderingHints hints)
		{
			if (xform == null)
			{
				xform = new AffineTransform();
			}
			else
			{
				xform = (AffineTransform) xform.Clone();
			}
			xform.Translate(Tx, Ty);
			xform.Scale(Sx, Sy);

			return TexturePaintContext.GetContext(BufImg, xform, hints, deviceBounds);
		}

		/// <summary>
		/// Returns the transparency mode for this <code>TexturePaint</code>. </summary>
		/// <returns> the transparency mode for this <code>TexturePaint</code>
		/// as an integer value. </returns>
		/// <seealso cref= Transparency </seealso>
		public virtual int Transparency
		{
			get
			{
				return (BufImg.ColorModel).Transparency;
			}
		}

	}

}