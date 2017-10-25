using System.Collections.Generic;

/*
 * Copyright (c) 1996, 2004, Oracle and/or its affiliates. All rights reserved.
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

namespace java.awt.image
{


	/// <summary>
	/// An ImageFilter class for scaling images using the simplest algorithm.
	/// This class extends the basic ImageFilter Class to scale an existing
	/// image and provide a source for a new image containing the resampled
	/// image.  The pixels in the source image are sampled to produce pixels
	/// for an image of the specified size by replicating rows and columns of
	/// pixels to scale up or omitting rows and columns of pixels to scale
	/// down.
	/// <para>It is meant to be used in conjunction with a FilteredImageSource
	/// object to produce scaled versions of existing images.  Due to
	/// implementation dependencies, there may be differences in pixel values
	/// of an image filtered on different platforms.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= FilteredImageSource </seealso>
	/// <seealso cref= ImageFilter
	/// 
	/// @author      Jim Graham </seealso>
	public class ReplicateScaleFilter : ImageFilter
	{

		/// <summary>
		/// The width of the source image.
		/// </summary>
		protected internal int SrcWidth;

		/// <summary>
		/// The height of the source image.
		/// </summary>
		protected internal int SrcHeight;

		/// <summary>
		/// The target width to scale the image.
		/// </summary>
		protected internal int DestWidth;

		/// <summary>
		/// The target height to scale the image.
		/// </summary>
		protected internal int DestHeight;

		/// <summary>
		/// An <code>int</code> array containing information about a
		/// row of pixels.
		/// </summary>
		protected internal int[] Srcrows;

		/// <summary>
		/// An <code>int</code> array containing information about a
		/// column of pixels.
		/// </summary>
		protected internal int[] Srccols;

		/// <summary>
		/// A <code>byte</code> array initialized with a size of
		/// <seealso cref="#destWidth"/> and used to deliver a row of pixel
		/// data to the <seealso cref="ImageConsumer"/>.
		/// </summary>
		protected internal Object Outpixbuf;

		/// <summary>
		/// Constructs a ReplicateScaleFilter that scales the pixels from
		/// its source Image as specified by the width and height parameters. </summary>
		/// <param name="width"> the target width to scale the image </param>
		/// <param name="height"> the target height to scale the image </param>
		/// <exception cref="IllegalArgumentException"> if <code>width</code> equals
		///         zero or <code>height</code> equals zero </exception>
		public ReplicateScaleFilter(int width, int height)
		{
			if (width == 0 || height == 0)
			{
				throw new IllegalArgumentException("Width (" + width + ") and height (" + height + ") must be non-zero");
			}
			DestWidth = width;
			DestHeight = height;
		}

		/// <summary>
		/// Passes along the properties from the source object after adding a
		/// property indicating the scale applied.
		/// This method invokes <code>super.setProperties</code>,
		/// which might result in additional properties being added.
		/// <para>
		/// Note: This method is intended to be called by the
		/// <code>ImageProducer</code> of the <code>Image</code> whose pixels
		/// are being filtered. Developers using
		/// this class to filter pixels from an image should avoid calling
		/// this method directly since that operation could interfere
		/// with the filtering operation.
		/// </para>
		/// </summary>
		public override Dictionary<T1> Properties<T1>
		{
			set
			{
				Dictionary<Object, Object> p = (Dictionary<Object, Object>)value.clone();
				String key = "rescale";
				String val = DestWidth + "x" + DestHeight;
				Object o = p[key];
				if (o != null && o is String)
				{
					val = ((String) o) + ", " + val;
				}
				p[key] = val;
				base.Properties = p;
			}
		}

		/// <summary>
		/// Override the dimensions of the source image and pass the dimensions
		/// of the new scaled size to the ImageConsumer.
		/// <para>
		/// Note: This method is intended to be called by the
		/// <code>ImageProducer</code> of the <code>Image</code> whose pixels
		/// are being filtered. Developers using
		/// this class to filter pixels from an image should avoid calling
		/// this method directly since that operation could interfere
		/// with the filtering operation.
		/// </para>
		/// </summary>
		/// <seealso cref= ImageConsumer </seealso>
		public override void SetDimensions(int w, int h)
		{
			SrcWidth = w;
			SrcHeight = h;
			if (DestWidth < 0)
			{
				if (DestHeight < 0)
				{
					DestWidth = SrcWidth;
					DestHeight = SrcHeight;
				}
				else
				{
					DestWidth = SrcWidth * DestHeight / SrcHeight;
				}
			}
			else if (DestHeight < 0)
			{
				DestHeight = SrcHeight * DestWidth / SrcWidth;
			}
			Consumer.SetDimensions(DestWidth, DestHeight);
		}

		private void CalculateMaps()
		{
			Srcrows = new int[DestHeight + 1];
			for (int y = 0; y <= DestHeight; y++)
			{
				Srcrows[y] = (2 * y * SrcHeight + SrcHeight) / (2 * DestHeight);
			}
			Srccols = new int[DestWidth + 1];
			for (int x = 0; x <= DestWidth; x++)
			{
				Srccols[x] = (2 * x * SrcWidth + SrcWidth) / (2 * DestWidth);
			}
		}

		/// <summary>
		/// Choose which rows and columns of the delivered byte pixels are
		/// needed for the destination scaled image and pass through just
		/// those rows and columns that are needed, replicated as necessary.
		/// <para>
		/// Note: This method is intended to be called by the
		/// <code>ImageProducer</code> of the <code>Image</code> whose pixels
		/// are being filtered. Developers using
		/// this class to filter pixels from an image should avoid calling
		/// this method directly since that operation could interfere
		/// with the filtering operation.
		/// </para>
		/// </summary>
		public override void SetPixels(int x, int y, int w, int h, ColorModel model, sbyte[] pixels, int off, int scansize)
		{
			if (Srcrows == null || Srccols == null)
			{
				CalculateMaps();
			}
			int sx, sy;
			int dx1 = (2 * x * DestWidth + SrcWidth - 1) / (2 * SrcWidth);
			int dy1 = (2 * y * DestHeight + SrcHeight - 1) / (2 * SrcHeight);
			sbyte[] outpix;
			if (Outpixbuf != null && Outpixbuf is sbyte[])
			{
				outpix = (sbyte[]) Outpixbuf;
			}
			else
			{
				outpix = new sbyte[DestWidth];
				Outpixbuf = outpix;
			}
			for (int dy = dy1; (sy = Srcrows[dy]) < y + h; dy++)
			{
				int srcoff = off + scansize * (sy - y);
				int dx;
				for (dx = dx1; (sx = Srccols[dx]) < x + w; dx++)
				{
					outpix[dx] = pixels[srcoff + sx - x];
				}
				if (dx > dx1)
				{
					Consumer.SetPixels(dx1, dy, dx - dx1, 1, model, outpix, dx1, DestWidth);
				}
			}
		}

		/// <summary>
		/// Choose which rows and columns of the delivered int pixels are
		/// needed for the destination scaled image and pass through just
		/// those rows and columns that are needed, replicated as necessary.
		/// <para>
		/// Note: This method is intended to be called by the
		/// <code>ImageProducer</code> of the <code>Image</code> whose pixels
		/// are being filtered. Developers using
		/// this class to filter pixels from an image should avoid calling
		/// this method directly since that operation could interfere
		/// with the filtering operation.
		/// </para>
		/// </summary>
		public override void SetPixels(int x, int y, int w, int h, ColorModel model, int[] pixels, int off, int scansize)
		{
			if (Srcrows == null || Srccols == null)
			{
				CalculateMaps();
			}
			int sx, sy;
			int dx1 = (2 * x * DestWidth + SrcWidth - 1) / (2 * SrcWidth);
			int dy1 = (2 * y * DestHeight + SrcHeight - 1) / (2 * SrcHeight);
			int[] outpix;
			if (Outpixbuf != null && Outpixbuf is int[])
			{
				outpix = (int[]) Outpixbuf;
			}
			else
			{
				outpix = new int[DestWidth];
				Outpixbuf = outpix;
			}
			for (int dy = dy1; (sy = Srcrows[dy]) < y + h; dy++)
			{
				int srcoff = off + scansize * (sy - y);
				int dx;
				for (dx = dx1; (sx = Srccols[dx]) < x + w; dx++)
				{
					outpix[dx] = pixels[srcoff + sx - x];
				}
				if (dx > dx1)
				{
					Consumer.SetPixels(dx1, dy, dx - dx1, 1, model, outpix, dx1, DestWidth);
				}
			}
		}
	}

}