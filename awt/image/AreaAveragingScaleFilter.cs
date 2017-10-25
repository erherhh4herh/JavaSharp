using System;

/*
 * Copyright (c) 1996, 2002, Oracle and/or its affiliates. All rights reserved.
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
	/// An ImageFilter class for scaling images using a simple area averaging
	/// algorithm that produces smoother results than the nearest neighbor
	/// algorithm.
	/// <para>This class extends the basic ImageFilter Class to scale an existing
	/// image and provide a source for a new image containing the resampled
	/// image.  The pixels in the source image are blended to produce pixels
	/// for an image of the specified size.  The blending process is analogous
	/// to scaling up the source image to a multiple of the destination size
	/// using pixel replication and then scaling it back down to the destination
	/// size by simply averaging all the pixels in the supersized image that
	/// fall within a given pixel of the destination image.  If the data from
	/// the source is not delivered in TopDownLeftRight order then the filter
	/// will back off to a simple pixel replication behavior and utilize the
	/// requestTopDownLeftRightResend() method to refilter the pixels in a
	/// better way at the end.
	/// </para>
	/// <para>It is meant to be used in conjunction with a FilteredImageSource
	/// object to produce scaled versions of existing images.  Due to
	/// implementation dependencies, there may be differences in pixel values
	/// of an image filtered on different platforms.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= FilteredImageSource </seealso>
	/// <seealso cref= ReplicateScaleFilter </seealso>
	/// <seealso cref= ImageFilter
	/// 
	/// @author      Jim Graham </seealso>
	public class AreaAveragingScaleFilter : ReplicateScaleFilter
	{
		private static readonly ColorModel Rgbmodel = ColorModel.RGBdefault;
		private static readonly int NeededHints = (ImageConsumer_Fields.TOPDOWNLEFTRIGHT | ImageConsumer_Fields.COMPLETESCANLINES);

		private bool Passthrough;
		private float[] Reds; private float[] Greens; private float[] Blues; private float[] Alphas;
		private int Savedy;
		private int Savedyrem;

		/// <summary>
		/// Constructs an AreaAveragingScaleFilter that scales the pixels from
		/// its source Image as specified by the width and height parameters. </summary>
		/// <param name="width"> the target width to scale the image </param>
		/// <param name="height"> the target height to scale the image </param>
		public AreaAveragingScaleFilter(int width, int height) : base(width, height)
		{
		}

		/// <summary>
		/// Detect if the data is being delivered with the necessary hints
		/// to allow the averaging algorithm to do its work.
		/// <para>
		/// Note: This method is intended to be called by the
		/// <code>ImageProducer</code> of the <code>Image</code> whose
		/// pixels are being filtered.  Developers using
		/// this class to filter pixels from an image should avoid calling
		/// this method directly since that operation could interfere
		/// with the filtering operation.
		/// </para>
		/// </summary>
		/// <seealso cref= ImageConsumer#setHints </seealso>
		public override int Hints
		{
			set
			{
				Passthrough = ((value & NeededHints) != NeededHints);
				base.Hints = value;
			}
		}

		private void MakeAccumBuffers()
		{
			Reds = new float[DestWidth];
			Greens = new float[DestWidth];
			Blues = new float[DestWidth];
			Alphas = new float[DestWidth];
		}

		private int[] CalcRow()
		{
			float origmult = ((float) SrcWidth) * SrcHeight;
			if (Outpixbuf == null || !(Outpixbuf is int[]))
			{
				Outpixbuf = new int[DestWidth];
			}
			int[] outpix = (int[]) Outpixbuf;
			for (int x = 0; x < DestWidth; x++)
			{
				float mult = origmult;
				int a = System.Math.Round(Alphas[x] / mult);
				if (a <= 0)
				{
					a = 0;
				}
				else if (a >= 255)
				{
					a = 255;
				}
				else
				{
					// un-premultiply the components (by modifying mult here, we
					// are effectively doing the divide by mult and divide by
					// alpha in the same step)
					mult = Alphas[x] / 255;
				}
				int r = System.Math.Round(Reds[x] / mult);
				int g = System.Math.Round(Greens[x] / mult);
				int b = System.Math.Round(Blues[x] / mult);
				if (r < 0)
				{
					r = 0;
				}
				else if (r > 255)
				{
					r = 255;
				}
				if (g < 0)
				{
					g = 0;
				}
				else if (g > 255)
				{
					g = 255;
				}
				if (b < 0)
				{
					b = 0;
				}
				else if (b > 255)
				{
					b = 255;
				}
				outpix[x] = (a << 24 | r << 16 | g << 8 | b);
			}
			return outpix;
		}

		private void AccumPixels(int x, int y, int w, int h, ColorModel model, Object pixels, int off, int scansize)
		{
			if (Reds == null)
			{
				MakeAccumBuffers();
			}
			int sy = y;
			int syrem = DestHeight;
			int dy, dyrem;
			if (sy == 0)
			{
				dy = 0;
				dyrem = 0;
			}
			else
			{
				dy = Savedy;
				dyrem = Savedyrem;
			}
			while (sy < y + h)
			{
				int amty;
				if (dyrem == 0)
				{
					for (int i = 0; i < DestWidth; i++)
					{
						Alphas[i] = Reds[i] = Greens[i] = Blues[i] = 0f;
					}
					dyrem = SrcHeight;
				}
				if (syrem < dyrem)
				{
					amty = syrem;
				}
				else
				{
					amty = dyrem;
				}
				int sx = 0;
				int dx = 0;
				int sxrem = 0;
				int dxrem = SrcWidth;
				float a = 0f, r = 0f, g = 0f, b = 0f;
				while (sx < w)
				{
					if (sxrem == 0)
					{
						sxrem = DestWidth;
						int rgb;
						if (pixels is sbyte[])
						{
							rgb = ((sbyte[]) pixels)[off + sx] & 0xff;
						}
						else
						{
							rgb = ((int[]) pixels)[off + sx];
						}
						// getRGB() always returns non-premultiplied components
						rgb = model.GetRGB(rgb);
						a = (int)((uint)rgb >> 24);
						r = (rgb >> 16) & 0xff;
						g = (rgb >> 8) & 0xff;
						b = rgb & 0xff;
						// premultiply the components if necessary
						if (a != 255.0f)
						{
							float ascale = a / 255.0f;
							r *= ascale;
							g *= ascale;
							b *= ascale;
						}
					}
					int amtx;
					if (sxrem < dxrem)
					{
						amtx = sxrem;
					}
					else
					{
						amtx = dxrem;
					}
					float mult = ((float) amtx) * amty;
					Alphas[dx] += mult * a;
					Reds[dx] += mult * r;
					Greens[dx] += mult * g;
					Blues[dx] += mult * b;
					if ((sxrem -= amtx) == 0)
					{
						sx++;
					}
					if ((dxrem -= amtx) == 0)
					{
						dx++;
						dxrem = SrcWidth;
					}
				}
				if ((dyrem -= amty) == 0)
				{
					int[] outpix = CalcRow();
					do
					{
						Consumer.SetPixels(0, dy, DestWidth, 1, Rgbmodel, outpix, 0, DestWidth);
						dy++;
					} while ((syrem -= amty) >= amty && amty == SrcHeight);
				}
				else
				{
					syrem -= amty;
				}
				if (syrem == 0)
				{
					syrem = DestHeight;
					sy++;
					off += scansize;
				}
			}
			Savedyrem = dyrem;
			Savedy = dy;
		}

		/// <summary>
		/// Combine the components for the delivered byte pixels into the
		/// accumulation arrays and send on any averaged data for rows of
		/// pixels that are complete.  If the correct hints were not
		/// specified in the setHints call then relay the work to our
		/// superclass which is capable of scaling pixels regardless of
		/// the delivery hints.
		/// <para>
		/// Note: This method is intended to be called by the
		/// <code>ImageProducer</code> of the <code>Image</code>
		/// whose pixels are being filtered.  Developers using
		/// this class to filter pixels from an image should avoid calling
		/// this method directly since that operation could interfere
		/// with the filtering operation.
		/// </para>
		/// </summary>
		/// <seealso cref= ReplicateScaleFilter </seealso>
		public override void SetPixels(int x, int y, int w, int h, ColorModel model, sbyte[] pixels, int off, int scansize)
		{
			if (Passthrough)
			{
				base.SetPixels(x, y, w, h, model, pixels, off, scansize);
			}
			else
			{
				AccumPixels(x, y, w, h, model, pixels, off, scansize);
			}
		}

		/// <summary>
		/// Combine the components for the delivered int pixels into the
		/// accumulation arrays and send on any averaged data for rows of
		/// pixels that are complete.  If the correct hints were not
		/// specified in the setHints call then relay the work to our
		/// superclass which is capable of scaling pixels regardless of
		/// the delivery hints.
		/// <para>
		/// Note: This method is intended to be called by the
		/// <code>ImageProducer</code> of the <code>Image</code>
		/// whose pixels are being filtered.  Developers using
		/// this class to filter pixels from an image should avoid calling
		/// this method directly since that operation could interfere
		/// with the filtering operation.
		/// </para>
		/// </summary>
		/// <seealso cref= ReplicateScaleFilter </seealso>
		public override void SetPixels(int x, int y, int w, int h, ColorModel model, int[] pixels, int off, int scansize)
		{
			if (Passthrough)
			{
				base.SetPixels(x, y, w, h, model, pixels, off, scansize);
			}
			else
			{
				AccumPixels(x, y, w, h, model, pixels, off, scansize);
			}
		}
	}

}