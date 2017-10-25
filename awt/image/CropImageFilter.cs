using System.Collections.Generic;

/*
 * Copyright (c) 1995, 2004, Oracle and/or its affiliates. All rights reserved.
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
	/// An ImageFilter class for cropping images.
	/// This class extends the basic ImageFilter Class to extract a given
	/// rectangular region of an existing Image and provide a source for a
	/// new image containing just the extracted region.  It is meant to
	/// be used in conjunction with a FilteredImageSource object to produce
	/// cropped versions of existing images.
	/// </summary>
	/// <seealso cref= FilteredImageSource </seealso>
	/// <seealso cref= ImageFilter
	/// 
	/// @author      Jim Graham </seealso>
	public class CropImageFilter : ImageFilter
	{
		internal int CropX;
		internal int CropY;
		internal int CropW;
		internal int CropH;

		/// <summary>
		/// Constructs a CropImageFilter that extracts the absolute rectangular
		/// region of pixels from its source Image as specified by the x, y,
		/// w, and h parameters. </summary>
		/// <param name="x"> the x location of the top of the rectangle to be extracted </param>
		/// <param name="y"> the y location of the top of the rectangle to be extracted </param>
		/// <param name="w"> the width of the rectangle to be extracted </param>
		/// <param name="h"> the height of the rectangle to be extracted </param>
		public CropImageFilter(int x, int y, int w, int h)
		{
			CropX = x;
			CropY = y;
			CropW = w;
			CropH = h;
		}

		/// <summary>
		/// Passes along  the properties from the source object after adding a
		/// property indicating the cropped region.
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
				p["croprect"] = new Rectangle(CropX, CropY, CropW, CropH);
				base.Properties = p;
			}
		}

		/// <summary>
		/// Override the source image's dimensions and pass the dimensions
		/// of the rectangular cropped region to the ImageConsumer.
		/// <para>
		/// Note: This method is intended to be called by the
		/// <code>ImageProducer</code> of the <code>Image</code> whose
		/// pixels are being filtered. Developers using
		/// this class to filter pixels from an image should avoid calling
		/// this method directly since that operation could interfere
		/// with the filtering operation.
		/// </para>
		/// </summary>
		/// <seealso cref= ImageConsumer </seealso>
		public override void SetDimensions(int w, int h)
		{
			Consumer.SetDimensions(CropW, CropH);
		}

		/// <summary>
		/// Determine whether the delivered byte pixels intersect the region to
		/// be extracted and passes through only that subset of pixels that
		/// appear in the output region.
		/// <para>
		/// Note: This method is intended to be called by the
		/// <code>ImageProducer</code> of the <code>Image</code> whose
		/// pixels are being filtered. Developers using
		/// this class to filter pixels from an image should avoid calling
		/// this method directly since that operation could interfere
		/// with the filtering operation.
		/// </para>
		/// </summary>
		public override void SetPixels(int x, int y, int w, int h, ColorModel model, sbyte[] pixels, int off, int scansize)
		{
			int x1 = x;
			if (x1 < CropX)
			{
				x1 = CropX;
			}
		int x2 = AddWithoutOverflow(x, w);
			if (x2 > CropX + CropW)
			{
				x2 = CropX + CropW;
			}
			int y1 = y;
			if (y1 < CropY)
			{
				y1 = CropY;
			}

		int y2 = AddWithoutOverflow(y, h);
			if (y2 > CropY + CropH)
			{
				y2 = CropY + CropH;
			}
			if (x1 >= x2 || y1 >= y2)
			{
				return;
			}
			Consumer.SetPixels(x1 - CropX, y1 - CropY, (x2 - x1), (y2 - y1), model, pixels, off + (y1 - y) * scansize + (x1 - x), scansize);
		}

		/// <summary>
		/// Determine if the delivered int pixels intersect the region to
		/// be extracted and pass through only that subset of pixels that
		/// appear in the output region.
		/// <para>
		/// Note: This method is intended to be called by the
		/// <code>ImageProducer</code> of the <code>Image</code> whose
		/// pixels are being filtered. Developers using
		/// this class to filter pixels from an image should avoid calling
		/// this method directly since that operation could interfere
		/// with the filtering operation.
		/// </para>
		/// </summary>
		public override void SetPixels(int x, int y, int w, int h, ColorModel model, int[] pixels, int off, int scansize)
		{
			int x1 = x;
			if (x1 < CropX)
			{
				x1 = CropX;
			}
		int x2 = AddWithoutOverflow(x, w);
			if (x2 > CropX + CropW)
			{
				x2 = CropX + CropW;
			}
			int y1 = y;
			if (y1 < CropY)
			{
				y1 = CropY;
			}

		int y2 = AddWithoutOverflow(y, h);
			if (y2 > CropY + CropH)
			{
				y2 = CropY + CropH;
			}
			if (x1 >= x2 || y1 >= y2)
			{
				return;
			}
			Consumer.SetPixels(x1 - CropX, y1 - CropY, (x2 - x1), (y2 - y1), model, pixels, off + (y1 - y) * scansize + (x1 - x), scansize);
		}

		//check for potential overflow (see bug 4801285)
		private int AddWithoutOverflow(int x, int w)
		{
			int x2 = x + w;
			if (x > 0 && w > 0 && x2 < 0)
			{
				x2 = Integer.MaxValue;
			}
			else if (x < 0 && w < 0 && x2 > 0)
			{
				x2 = Integer.MinValue;
			}
			return x2;
		}
	}

}