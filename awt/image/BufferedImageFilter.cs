/*
 * Copyright (c) 1997, 2000, Oracle and/or its affiliates. All rights reserved.
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
	/// The <code>BufferedImageFilter</code> class subclasses an
	/// <code>ImageFilter</code> to provide a simple means of
	/// using a single-source/single-destination image operator
	/// (<seealso cref="BufferedImageOp"/>) to filter a <code>BufferedImage</code>
	/// in the Image Producer/Consumer/Observer
	/// paradigm. Examples of these image operators are: <seealso cref="ConvolveOp"/>,
	/// <seealso cref="AffineTransformOp"/> and <seealso cref="LookupOp"/>.
	/// </summary>
	/// <seealso cref= ImageFilter </seealso>
	/// <seealso cref= BufferedImage </seealso>
	/// <seealso cref= BufferedImageOp </seealso>

	public class BufferedImageFilter : ImageFilter, Cloneable
	{
		internal BufferedImageOp BufferedImageOp_Renamed;
		internal ColorModel Model;
		internal int Width;
		internal int Height;
		internal sbyte[] BytePixels;
		internal int[] IntPixels;

		/// <summary>
		/// Constructs a <code>BufferedImageFilter</code> with the
		/// specified single-source/single-destination operator. </summary>
		/// <param name="op"> the specified <code>BufferedImageOp</code> to
		///           use to filter a <code>BufferedImage</code> </param>
		/// <exception cref="NullPointerException"> if op is null </exception>
		public BufferedImageFilter(BufferedImageOp op) : base()
		{
			if (op == null)
			{
				throw new NullPointerException("Operation cannot be null");
			}
			BufferedImageOp_Renamed = op;
		}

		/// <summary>
		/// Returns the <code>BufferedImageOp</code>. </summary>
		/// <returns> the operator of this <code>BufferedImageFilter</code>. </returns>
		public virtual BufferedImageOp BufferedImageOp
		{
			get
			{
				return BufferedImageOp_Renamed;
			}
		}

		/// <summary>
		/// Filters the information provided in the
		/// <seealso cref="ImageConsumer#setDimensions(int, int) setDimensions "/> method
		/// of the <seealso cref="ImageConsumer"/> interface.
		/// <para>
		/// Note: This method is intended to be called by the
		/// <seealso cref="ImageProducer"/> of the <code>Image</code> whose pixels are
		/// being filtered. Developers using this class to retrieve pixels from
		/// an image should avoid calling this method directly since that
		/// operation could result in problems with retrieving the requested
		/// pixels.
		/// </para>
		/// <para>
		/// </para>
		/// </summary>
		/// <param name="width"> the width to which to set the width of this
		///        <code>BufferedImageFilter</code> </param>
		/// <param name="height"> the height to which to set the height of this
		///        <code>BufferedImageFilter</code> </param>
		/// <seealso cref= ImageConsumer#setDimensions </seealso>
		public override void SetDimensions(int width, int height)
		{
			if (width <= 0 || height <= 0)
			{
				ImageComplete(ImageConsumer_Fields.STATICIMAGEDONE);
				return;
			}
			this.Width = width;
			this.Height = height;
		}

		/// <summary>
		/// Filters the information provided in the
		/// <seealso cref="ImageConsumer#setColorModel(ColorModel) setColorModel"/> method
		/// of the <code>ImageConsumer</code> interface.
		/// <para>
		/// If <code>model</code> is <code>null</code>, this
		/// method clears the current <code>ColorModel</code> of this
		/// <code>BufferedImageFilter</code>.
		/// </para>
		/// <para>
		/// Note: This method is intended to be called by the
		/// <code>ImageProducer</code> of the <code>Image</code>
		/// whose pixels are being filtered.  Developers using this
		/// class to retrieve pixels from an image
		/// should avoid calling this method directly since that
		/// operation could result in problems with retrieving the
		/// requested pixels.
		/// </para>
		/// </summary>
		/// <param name="model"> the <seealso cref="ColorModel"/> to which to set the
		///        <code>ColorModel</code> of this <code>BufferedImageFilter</code> </param>
		/// <seealso cref= ImageConsumer#setColorModel </seealso>
		public override ColorModel ColorModel
		{
			set
			{
				this.Model = value;
			}
		}

		private void ConvertToRGB()
		{
			int size = Width * Height;
			int[] newpixels = new int[size];
			if (BytePixels != null)
			{
				for (int i = 0; i < size; i++)
				{
					newpixels[i] = this.Model.GetRGB(BytePixels[i] & 0xff);
				}
			}
			else if (IntPixels != null)
			{
				for (int i = 0; i < size; i++)
				{
					newpixels[i] = this.Model.GetRGB(IntPixels[i]);
				}
			}
			BytePixels = null;
			IntPixels = newpixels;
			this.Model = ColorModel.RGBdefault;
		}

		/// <summary>
		/// Filters the information provided in the <code>setPixels</code>
		/// method of the <code>ImageConsumer</code> interface which takes
		/// an array of bytes.
		/// <para>
		/// Note: This method is intended to be called by the
		/// <code>ImageProducer</code> of the <code>Image</code> whose pixels
		/// are being filtered.  Developers using
		/// this class to retrieve pixels from an image should avoid calling
		/// this method directly since that operation could result in problems
		/// with retrieving the requested pixels.
		/// </para>
		/// </summary>
		/// <exception cref="IllegalArgumentException"> if width or height are less than
		/// zero. </exception>
		/// <seealso cref= ImageConsumer#setPixels(int, int, int, int, ColorModel, byte[],
		///                                int, int) </seealso>
		public override void SetPixels(int x, int y, int w, int h, ColorModel model, sbyte[] pixels, int off, int scansize)
		{
			// Fix 4184230
			if (w < 0 || h < 0)
			{
				throw new IllegalArgumentException("Width (" + w + ") and height (" + h + ") must be > 0");
			}
			// Nothing to do
			if (w == 0 || h == 0)
			{
				return;
			}
			if (y < 0)
			{
				int diff = -y;
				if (diff >= h)
				{
					return;
				}
				off += scansize * diff;
				y += diff;
				h -= diff;
			}
			if (y + h > Height)
			{
				h = Height - y;
				if (h <= 0)
				{
					return;
				}
			}
			if (x < 0)
			{
				int diff = -x;
				if (diff >= w)
				{
					return;
				}
				off += diff;
				x += diff;
				w -= diff;
			}
			if (x + w > Width)
			{
				w = Width - x;
				if (w <= 0)
				{
					return;
				}
			}
			int dstPtr = y * Width + x;
			if (IntPixels == null)
			{
				if (BytePixels == null)
				{
					BytePixels = new sbyte[Width * Height];
					this.Model = model;
				}
				else if (this.Model != model)
				{
					ConvertToRGB();
				}
				if (BytePixels != null)
				{
					for (int sh = h; sh > 0; sh--)
					{
						System.Array.Copy(pixels, off, BytePixels, dstPtr, w);
						off += scansize;
						dstPtr += Width;
					}
				}
			}
			if (IntPixels != null)
			{
				int dstRem = Width - w;
				int srcRem = scansize - w;
				for (int sh = h; sh > 0; sh--)
				{
					for (int sw = w; sw > 0; sw--)
					{
						IntPixels[dstPtr++] = model.GetRGB(pixels[off++] & 0xff);
					}
					off += srcRem;
					dstPtr += dstRem;
				}
			}
		}
		/// <summary>
		/// Filters the information provided in the <code>setPixels</code>
		/// method of the <code>ImageConsumer</code> interface which takes
		/// an array of integers.
		/// <para>
		/// Note: This method is intended to be called by the
		/// <code>ImageProducer</code> of the <code>Image</code> whose
		/// pixels are being filtered.  Developers using this class to
		/// retrieve pixels from an image should avoid calling this method
		/// directly since that operation could result in problems
		/// with retrieving the requested pixels.
		/// </para>
		/// </summary>
		/// <exception cref="IllegalArgumentException"> if width or height are less than
		/// zero. </exception>
		/// <seealso cref= ImageConsumer#setPixels(int, int, int, int, ColorModel, int[],
		///                                int, int) </seealso>
		public override void SetPixels(int x, int y, int w, int h, ColorModel model, int[] pixels, int off, int scansize)
		{
			// Fix 4184230
			if (w < 0 || h < 0)
			{
				throw new IllegalArgumentException("Width (" + w + ") and height (" + h + ") must be > 0");
			}
			// Nothing to do
			if (w == 0 || h == 0)
			{
				return;
			}
			if (y < 0)
			{
				int diff = -y;
				if (diff >= h)
				{
					return;
				}
				off += scansize * diff;
				y += diff;
				h -= diff;
			}
			if (y + h > Height)
			{
				h = Height - y;
				if (h <= 0)
				{
					return;
				}
			}
			if (x < 0)
			{
				int diff = -x;
				if (diff >= w)
				{
					return;
				}
				off += diff;
				x += diff;
				w -= diff;
			}
			if (x + w > Width)
			{
				w = Width - x;
				if (w <= 0)
				{
					return;
				}
			}

			if (IntPixels == null)
			{
				if (BytePixels == null)
				{
					IntPixels = new int[Width * Height];
					this.Model = model;
				}
				else
				{
					ConvertToRGB();
				}
			}
			int dstPtr = y * Width + x;
			if (this.Model == model)
			{
				for (int sh = h; sh > 0; sh--)
				{
					System.Array.Copy(pixels, off, IntPixels, dstPtr, w);
					off += scansize;
					dstPtr += Width;
				}
			}
			else
			{
				if (this.Model != ColorModel.RGBdefault)
				{
					ConvertToRGB();
				}
				int dstRem = Width - w;
				int srcRem = scansize - w;
				for (int sh = h; sh > 0; sh--)
				{
					for (int sw = w; sw > 0; sw--)
					{
						IntPixels[dstPtr++] = model.GetRGB(pixels[off++]);
					}
					off += srcRem;
					dstPtr += dstRem;
				}
			}
		}

		/// <summary>
		/// Filters the information provided in the <code>imageComplete</code>
		/// method of the <code>ImageConsumer</code> interface.
		/// <para>
		/// Note: This method is intended to be called by the
		/// <code>ImageProducer</code> of the <code>Image</code> whose pixels
		/// are being filtered.  Developers using
		/// this class to retrieve pixels from an image should avoid calling
		/// this method directly since that operation could result in problems
		/// with retrieving the requested pixels.
		/// </para>
		/// </summary>
		/// <param name="status"> the status of image loading </param>
		/// <exception cref="ImagingOpException"> if there was a problem calling the filter
		/// method of the <code>BufferedImageOp</code> associated with this
		/// instance. </exception>
		/// <seealso cref= ImageConsumer#imageComplete </seealso>
		public override void ImageComplete(int status)
		{
			WritableRaster wr;
			switch (status)
			{
			case ImageConsumer_Fields.IMAGEERROR:
			case ImageConsumer_Fields.IMAGEABORTED:
				// reinitialize the params
				Model = null;
				Width = -1;
				Height = -1;
				IntPixels = null;
				BytePixels = null;
				break;

			case ImageConsumer_Fields.SINGLEFRAMEDONE:
			case ImageConsumer_Fields.STATICIMAGEDONE:
				if (Width <= 0 || Height <= 0)
				{
					break;
				}
				if (Model is DirectColorModel)
				{
					if (IntPixels == null)
					{
						break;
					}
					wr = CreateDCMraster();
				}
				else if (Model is IndexColorModel)
				{
					int[] bandOffsets = new int[] {0};
					if (BytePixels == null)
					{
						break;
					}
					DataBufferByte db = new DataBufferByte(BytePixels, Width * Height);
					wr = Raster.CreateInterleavedRaster(db, Width, Height, Width, 1, bandOffsets, null);
				}
				else
				{
					ConvertToRGB();
					if (IntPixels == null)
					{
						break;
					}
					wr = CreateDCMraster();
				}
				BufferedImage bi = new BufferedImage(Model, wr, Model.AlphaPremultiplied, null);
				bi = BufferedImageOp_Renamed.Filter(bi, null);
				WritableRaster r = bi.Raster;
				ColorModel cm = bi.ColorModel;
				int w = r.Width;
				int h = r.Height;
				Consumer.SetDimensions(w, h);
				Consumer.ColorModel = cm;
				if (cm is DirectColorModel)
				{
					DataBufferInt db = (DataBufferInt) r.DataBuffer;
					Consumer.SetPixels(0, 0, w, h, cm, db.Data, 0, w);
				}
				else if (cm is IndexColorModel)
				{
					DataBufferByte db = (DataBufferByte) r.DataBuffer;
					Consumer.SetPixels(0, 0, w, h, cm, db.Data, 0, w);
				}
				else
				{
					throw new InternalError("Unknown color model " + cm);
				}
				break;
			}
			Consumer.ImageComplete(status);
		}

		private WritableRaster CreateDCMraster()
		{
			WritableRaster wr;
			DirectColorModel dcm = (DirectColorModel) Model;
			bool hasAlpha = Model.HasAlpha();
			int[] bandMasks = new int[3 + (hasAlpha ? 1 : 0)];
			bandMasks[0] = dcm.RedMask;
			bandMasks[1] = dcm.GreenMask;
			bandMasks[2] = dcm.BlueMask;
			if (hasAlpha)
			{
				bandMasks[3] = dcm.AlphaMask;
			}
			DataBufferInt db = new DataBufferInt(IntPixels, Width * Height);
			wr = Raster.CreatePackedRaster(db, Width, Height, Width, bandMasks, null);
			return wr;
		}

	}

}