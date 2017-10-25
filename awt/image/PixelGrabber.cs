using System;
using System.Collections.Generic;
using System.Threading;

/*
 * Copyright (c) 1995, 2013, Oracle and/or its affiliates. All rights reserved.
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
	/// The PixelGrabber class implements an ImageConsumer which can be attached
	/// to an Image or ImageProducer object to retrieve a subset of the pixels
	/// in that image.  Here is an example:
	/// <pre>{@code
	/// 
	/// public void handlesinglepixel(int x, int y, int pixel) {
	///      int alpha = (pixel >> 24) & 0xff;
	///      int red   = (pixel >> 16) & 0xff;
	///      int green = (pixel >>  8) & 0xff;
	///      int blue  = (pixel      ) & 0xff;
	///      // Deal with the pixel as necessary...
	/// }
	/// 
	/// public void handlepixels(Image img, int x, int y, int w, int h) {
	///      int[] pixels = new int[w * h];
	///      PixelGrabber pg = new PixelGrabber(img, x, y, w, h, pixels, 0, w);
	///      try {
	///          pg.grabPixels();
	///      } catch (InterruptedException e) {
	///          System.err.println("interrupted waiting for pixels!");
	///          return;
	///      }
	///      if ((pg.getStatus() & ImageObserver.ABORT) != 0) {
	///          System.err.println("image fetch aborted or errored");
	///          return;
	///      }
	///      for (int j = 0; j < h; j++) {
	///          for (int i = 0; i < w; i++) {
	///              handlesinglepixel(x+i, y+j, pixels[j * w + i]);
	///          }
	///      }
	/// }
	/// 
	/// }</pre>
	/// </summary>
	/// <seealso cref= ColorModel#getRGBdefault
	/// 
	/// @author      Jim Graham </seealso>
	public class PixelGrabber : ImageConsumer
	{
		internal ImageProducer Producer;

		internal int DstX;
		internal int DstY;
		internal int DstW;
		internal int DstH;

		internal ColorModel ImageModel;
		internal sbyte[] BytePixels;
		internal int[] IntPixels;
		internal int DstOff;
		internal int DstScan;

		private bool Grabbing;
		private int Flags;

		private static readonly int GRABBEDBITS = (ImageObserver_Fields.FRAMEBITS | ImageObserver_Fields.ALLBITS);
		private static readonly int DONEBITS = (GRABBEDBITS | ImageObserver_Fields.ERROR);

		/// <summary>
		/// Create a PixelGrabber object to grab the (x, y, w, h) rectangular
		/// section of pixels from the specified image into the given array.
		/// The pixels are stored into the array in the default RGB ColorModel.
		/// The RGB data for pixel (i, j) where (i, j) is inside the rectangle
		/// (x, y, w, h) is stored in the array at
		/// <tt>pix[(j - y) * scansize + (i - x) + off]</tt>. </summary>
		/// <seealso cref= ColorModel#getRGBdefault </seealso>
		/// <param name="img"> the image to retrieve pixels from </param>
		/// <param name="x"> the x coordinate of the upper left corner of the rectangle
		/// of pixels to retrieve from the image, relative to the default
		/// (unscaled) size of the image </param>
		/// <param name="y"> the y coordinate of the upper left corner of the rectangle
		/// of pixels to retrieve from the image </param>
		/// <param name="w"> the width of the rectangle of pixels to retrieve </param>
		/// <param name="h"> the height of the rectangle of pixels to retrieve </param>
		/// <param name="pix"> the array of integers which are to be used to hold the
		/// RGB pixels retrieved from the image </param>
		/// <param name="off"> the offset into the array of where to store the first pixel </param>
		/// <param name="scansize"> the distance from one row of pixels to the next in
		/// the array </param>
		public PixelGrabber(Image img, int x, int y, int w, int h, int[] pix, int off, int scansize) : this(img.Source, x, y, w, h, pix, off, scansize)
		{
		}

		/// <summary>
		/// Create a PixelGrabber object to grab the (x, y, w, h) rectangular
		/// section of pixels from the image produced by the specified
		/// ImageProducer into the given array.
		/// The pixels are stored into the array in the default RGB ColorModel.
		/// The RGB data for pixel (i, j) where (i, j) is inside the rectangle
		/// (x, y, w, h) is stored in the array at
		/// <tt>pix[(j - y) * scansize + (i - x) + off]</tt>. </summary>
		/// <param name="ip"> the <code>ImageProducer</code> that produces the
		/// image from which to retrieve pixels </param>
		/// <param name="x"> the x coordinate of the upper left corner of the rectangle
		/// of pixels to retrieve from the image, relative to the default
		/// (unscaled) size of the image </param>
		/// <param name="y"> the y coordinate of the upper left corner of the rectangle
		/// of pixels to retrieve from the image </param>
		/// <param name="w"> the width of the rectangle of pixels to retrieve </param>
		/// <param name="h"> the height of the rectangle of pixels to retrieve </param>
		/// <param name="pix"> the array of integers which are to be used to hold the
		/// RGB pixels retrieved from the image </param>
		/// <param name="off"> the offset into the array of where to store the first pixel </param>
		/// <param name="scansize"> the distance from one row of pixels to the next in
		/// the array </param>
		/// <seealso cref= ColorModel#getRGBdefault </seealso>
		public PixelGrabber(ImageProducer ip, int x, int y, int w, int h, int[] pix, int off, int scansize)
		{
			Producer = ip;
			DstX = x;
			DstY = y;
			DstW = w;
			DstH = h;
			DstOff = off;
			DstScan = scansize;
			IntPixels = pix;
			ImageModel = ColorModel.RGBdefault;
		}

		/// <summary>
		/// Create a PixelGrabber object to grab the (x, y, w, h) rectangular
		/// section of pixels from the specified image.  The pixels are
		/// accumulated in the original ColorModel if the same ColorModel
		/// is used for every call to setPixels, otherwise the pixels are
		/// accumulated in the default RGB ColorModel.  If the forceRGB
		/// parameter is true, then the pixels will be accumulated in the
		/// default RGB ColorModel anyway.  A buffer is allocated by the
		/// PixelGrabber to hold the pixels in either case.  If {@code (w < 0)} or
		/// {@code (h < 0)}, then they will default to the remaining width and
		/// height of the source data when that information is delivered. </summary>
		/// <param name="img"> the image to retrieve the image data from </param>
		/// <param name="x"> the x coordinate of the upper left corner of the rectangle
		/// of pixels to retrieve from the image, relative to the default
		/// (unscaled) size of the image </param>
		/// <param name="y"> the y coordinate of the upper left corner of the rectangle
		/// of pixels to retrieve from the image </param>
		/// <param name="w"> the width of the rectangle of pixels to retrieve </param>
		/// <param name="h"> the height of the rectangle of pixels to retrieve </param>
		/// <param name="forceRGB"> true if the pixels should always be converted to
		/// the default RGB ColorModel </param>
		public PixelGrabber(Image img, int x, int y, int w, int h, bool forceRGB)
		{
			Producer = img.Source;
			DstX = x;
			DstY = y;
			DstW = w;
			DstH = h;
			if (forceRGB)
			{
				ImageModel = ColorModel.RGBdefault;
			}
		}

		/// <summary>
		/// Request the PixelGrabber to start fetching the pixels.
		/// </summary>
		public virtual void StartGrabbing()
		{
			lock (this)
			{
				if ((Flags & DONEBITS) != 0)
				{
					return;
				}
				if (!Grabbing)
				{
					Grabbing = true;
					Flags &= ~(ImageObserver_Fields.ABORT);
					Producer.StartProduction(this);
				}
			}
		}

		/// <summary>
		/// Request the PixelGrabber to abort the image fetch.
		/// </summary>
		public virtual void AbortGrabbing()
		{
			lock (this)
			{
				ImageComplete(ImageConsumer_Fields.IMAGEABORTED);
			}
		}

		/// <summary>
		/// Request the Image or ImageProducer to start delivering pixels and
		/// wait for all of the pixels in the rectangle of interest to be
		/// delivered. </summary>
		/// <returns> true if the pixels were successfully grabbed, false on
		/// abort, error or timeout </returns>
		/// <exception cref="InterruptedException">
		///            Another thread has interrupted this thread. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean grabPixels() throws InterruptedException
		public virtual bool GrabPixels()
		{
			return GrabPixels(0);
		}

		/// <summary>
		/// Request the Image or ImageProducer to start delivering pixels and
		/// wait for all of the pixels in the rectangle of interest to be
		/// delivered or until the specified timeout has elapsed.  This method
		/// behaves in the following ways, depending on the value of
		/// <code>ms</code>:
		/// <ul>
		/// <li> If {@code ms == 0}, waits until all pixels are delivered
		/// <li> If {@code ms > 0}, waits until all pixels are delivered
		/// as timeout expires.
		/// <li> If {@code ms < 0}, returns <code>true</code> if all pixels
		/// are grabbed, <code>false</code> otherwise and does not wait.
		/// </ul> </summary>
		/// <param name="ms"> the number of milliseconds to wait for the image pixels
		/// to arrive before timing out </param>
		/// <returns> true if the pixels were successfully grabbed, false on
		/// abort, error or timeout </returns>
		/// <exception cref="InterruptedException">
		///            Another thread has interrupted this thread. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public synchronized boolean grabPixels(long ms) throws InterruptedException
		public virtual bool GrabPixels(long ms)
		{
			lock (this)
			{
				if ((Flags & DONEBITS) != 0)
				{
					return (Flags & GRABBEDBITS) != 0;
				}
				long end = ms + DateTimeHelperClass.CurrentUnixTimeMillis();
				if (!Grabbing)
				{
					Grabbing = true;
					Flags &= ~(ImageObserver_Fields.ABORT);
					Producer.StartProduction(this);
				}
				while (Grabbing)
				{
					long timeout;
					if (ms == 0)
					{
						timeout = 0;
					}
					else
					{
						timeout = end - DateTimeHelperClass.CurrentUnixTimeMillis();
						if (timeout <= 0)
						{
							break;
						}
					}
					Monitor.Wait(this, TimeSpan.FromMilliseconds(timeout));
				}
				return (Flags & GRABBEDBITS) != 0;
			}
		}

		/// <summary>
		/// Return the status of the pixels.  The ImageObserver flags
		/// representing the available pixel information are returned. </summary>
		/// <returns> the bitwise OR of all relevant ImageObserver flags </returns>
		/// <seealso cref= ImageObserver </seealso>
		public virtual int Status
		{
			get
			{
				lock (this)
				{
					return Flags;
				}
			}
		}

		/// <summary>
		/// Get the width of the pixel buffer (after adjusting for image width).
		/// If no width was specified for the rectangle of pixels to grab then
		/// then this information will only be available after the image has
		/// delivered the dimensions. </summary>
		/// <returns> the final width used for the pixel buffer or -1 if the width
		/// is not yet known </returns>
		/// <seealso cref= #getStatus </seealso>
		public virtual int Width
		{
			get
			{
				lock (this)
				{
					return (DstW < 0) ? - 1 : DstW;
				}
			}
		}

		/// <summary>
		/// Get the height of the pixel buffer (after adjusting for image height).
		/// If no width was specified for the rectangle of pixels to grab then
		/// then this information will only be available after the image has
		/// delivered the dimensions. </summary>
		/// <returns> the final height used for the pixel buffer or -1 if the height
		/// is not yet known </returns>
		/// <seealso cref= #getStatus </seealso>
		public virtual int Height
		{
			get
			{
				lock (this)
				{
					return (DstH < 0) ? - 1 : DstH;
				}
			}
		}

		/// <summary>
		/// Get the pixel buffer.  If the PixelGrabber was not constructed
		/// with an explicit pixel buffer to hold the pixels then this method
		/// will return null until the size and format of the image data is
		/// known.
		/// Since the PixelGrabber may fall back on accumulating the data
		/// in the default RGB ColorModel at any time if the source image
		/// uses more than one ColorModel to deliver the data, the array
		/// object returned by this method may change over time until the
		/// image grab is complete. </summary>
		/// <returns> either a byte array or an int array </returns>
		/// <seealso cref= #getStatus </seealso>
		/// <seealso cref= #setPixels(int, int, int, int, ColorModel, byte[], int, int) </seealso>
		/// <seealso cref= #setPixels(int, int, int, int, ColorModel, int[], int, int) </seealso>
		public virtual Object Pixels
		{
			get
			{
				lock (this)
				{
					return (BytePixels == null) ? ((Object) IntPixels) : ((Object) BytePixels);
				}
			}
		}

		/// <summary>
		/// Get the ColorModel for the pixels stored in the array.  If the
		/// PixelGrabber was constructed with an explicit pixel buffer then
		/// this method will always return the default RGB ColorModel,
		/// otherwise it may return null until the ColorModel used by the
		/// ImageProducer is known.
		/// Since the PixelGrabber may fall back on accumulating the data
		/// in the default RGB ColorModel at any time if the source image
		/// uses more than one ColorModel to deliver the data, the ColorModel
		/// object returned by this method may change over time until the
		/// image grab is complete and may not reflect any of the ColorModel
		/// objects that was used by the ImageProducer to deliver the pixels. </summary>
		/// <returns> the ColorModel object used for storing the pixels </returns>
		/// <seealso cref= #getStatus </seealso>
		/// <seealso cref= ColorModel#getRGBdefault </seealso>
		/// <seealso cref= #setColorModel(ColorModel) </seealso>
		public virtual ColorModel ColorModel
		{
			get
			{
				lock (this)
				{
					return ImageModel;
				}
			}
			set
			{
				return;
			}
		}

		/// <summary>
		/// The setDimensions method is part of the ImageConsumer API which
		/// this class must implement to retrieve the pixels.
		/// <para>
		/// Note: This method is intended to be called by the ImageProducer
		/// of the Image whose pixels are being grabbed.  Developers using
		/// this class to retrieve pixels from an image should avoid calling
		/// this method directly since that operation could result in problems
		/// with retrieving the requested pixels.
		/// </para>
		/// </summary>
		/// <param name="width"> the width of the dimension </param>
		/// <param name="height"> the height of the dimension </param>
		public virtual void SetDimensions(int width, int height)
		{
			if (DstW < 0)
			{
				DstW = width - DstX;
			}
			if (DstH < 0)
			{
				DstH = height - DstY;
			}
			if (DstW <= 0 || DstH <= 0)
			{
				ImageComplete(ImageConsumer_Fields.STATICIMAGEDONE);
			}
			else if (IntPixels == null && ImageModel == ColorModel.RGBdefault)
			{
				IntPixels = new int[DstW * DstH];
				DstScan = DstW;
				DstOff = 0;
			}
			Flags |= (ImageObserver_Fields.WIDTH | ImageObserver_Fields.HEIGHT);
		}

		/// <summary>
		/// The setHints method is part of the ImageConsumer API which
		/// this class must implement to retrieve the pixels.
		/// <para>
		/// Note: This method is intended to be called by the ImageProducer
		/// of the Image whose pixels are being grabbed.  Developers using
		/// this class to retrieve pixels from an image should avoid calling
		/// this method directly since that operation could result in problems
		/// with retrieving the requested pixels.
		/// </para>
		/// </summary>
		/// <param name="hints"> a set of hints used to process the pixels </param>
		public virtual int Hints
		{
			set
			{
				return;
			}
		}

		/// <summary>
		/// The setProperties method is part of the ImageConsumer API which
		/// this class must implement to retrieve the pixels.
		/// <para>
		/// Note: This method is intended to be called by the ImageProducer
		/// of the Image whose pixels are being grabbed.  Developers using
		/// this class to retrieve pixels from an image should avoid calling
		/// this method directly since that operation could result in problems
		/// with retrieving the requested pixels.
		/// </para>
		/// </summary>
		/// <param name="props"> the list of properties </param>
		public virtual Dictionary<T1> Properties<T1>
		{
			set
			{
				return;
			}
		}


		private void ConvertToRGB()
		{
			int size = DstW * DstH;
			int[] newpixels = new int[size];
			if (BytePixels != null)
			{
				for (int i = 0; i < size; i++)
				{
					newpixels[i] = ImageModel.GetRGB(BytePixels[i] & 0xff);
				}
			}
			else if (IntPixels != null)
			{
				for (int i = 0; i < size; i++)
				{
					newpixels[i] = ImageModel.GetRGB(IntPixels[i]);
				}
			}
			BytePixels = null;
			IntPixels = newpixels;
			DstScan = DstW;
			DstOff = 0;
			ImageModel = ColorModel.RGBdefault;
		}

		/// <summary>
		/// The setPixels method is part of the ImageConsumer API which
		/// this class must implement to retrieve the pixels.
		/// <para>
		/// Note: This method is intended to be called by the ImageProducer
		/// of the Image whose pixels are being grabbed.  Developers using
		/// this class to retrieve pixels from an image should avoid calling
		/// this method directly since that operation could result in problems
		/// with retrieving the requested pixels.
		/// </para>
		/// </summary>
		/// <param name="srcX"> the X coordinate of the upper-left corner
		///        of the area of pixels to be set </param>
		/// <param name="srcY"> the Y coordinate of the upper-left corner
		///        of the area of pixels to be set </param>
		/// <param name="srcW"> the width of the area of pixels </param>
		/// <param name="srcH"> the height of the area of pixels </param>
		/// <param name="model"> the specified <code>ColorModel</code> </param>
		/// <param name="pixels"> the array of pixels </param>
		/// <param name="srcOff"> the offset into the pixels array </param>
		/// <param name="srcScan"> the distance from one row of pixels to the next
		///        in the pixels array </param>
		/// <seealso cref= #getPixels </seealso>
		public virtual void SetPixels(int srcX, int srcY, int srcW, int srcH, ColorModel model, sbyte[] pixels, int srcOff, int srcScan)
		{
			if (srcY < DstY)
			{
				int diff = DstY - srcY;
				if (diff >= srcH)
				{
					return;
				}
				srcOff += srcScan * diff;
				srcY += diff;
				srcH -= diff;
			}
			if (srcY + srcH > DstY + DstH)
			{
				srcH = (DstY + DstH) - srcY;
				if (srcH <= 0)
				{
					return;
				}
			}
			if (srcX < DstX)
			{
				int diff = DstX - srcX;
				if (diff >= srcW)
				{
					return;
				}
				srcOff += diff;
				srcX += diff;
				srcW -= diff;
			}
			if (srcX + srcW > DstX + DstW)
			{
				srcW = (DstX + DstW) - srcX;
				if (srcW <= 0)
				{
					return;
				}
			}
			int dstPtr = DstOff + (srcY - DstY) * DstScan + (srcX - DstX);
			if (IntPixels == null)
			{
				if (BytePixels == null)
				{
					BytePixels = new sbyte[DstW * DstH];
					DstScan = DstW;
					DstOff = 0;
					ImageModel = model;
				}
				else if (ImageModel != model)
				{
					ConvertToRGB();
				}
				if (BytePixels != null)
				{
					for (int h = srcH; h > 0; h--)
					{
						System.Array.Copy(pixels, srcOff, BytePixels, dstPtr, srcW);
						srcOff += srcScan;
						dstPtr += DstScan;
					}
				}
			}
			if (IntPixels != null)
			{
				int dstRem = DstScan - srcW;
				int srcRem = srcScan - srcW;
				for (int h = srcH; h > 0; h--)
				{
					for (int w = srcW; w > 0; w--)
					{
						IntPixels[dstPtr++] = model.GetRGB(pixels[srcOff++] & 0xff);
					}
					srcOff += srcRem;
					dstPtr += dstRem;
				}
			}
			Flags |= ImageObserver_Fields.SOMEBITS;
		}

		/// <summary>
		/// The setPixels method is part of the ImageConsumer API which
		/// this class must implement to retrieve the pixels.
		/// <para>
		/// Note: This method is intended to be called by the ImageProducer
		/// of the Image whose pixels are being grabbed.  Developers using
		/// this class to retrieve pixels from an image should avoid calling
		/// this method directly since that operation could result in problems
		/// with retrieving the requested pixels.
		/// </para>
		/// </summary>
		/// <param name="srcX"> the X coordinate of the upper-left corner
		///        of the area of pixels to be set </param>
		/// <param name="srcY"> the Y coordinate of the upper-left corner
		///        of the area of pixels to be set </param>
		/// <param name="srcW"> the width of the area of pixels </param>
		/// <param name="srcH"> the height of the area of pixels </param>
		/// <param name="model"> the specified <code>ColorModel</code> </param>
		/// <param name="pixels"> the array of pixels </param>
		/// <param name="srcOff"> the offset into the pixels array </param>
		/// <param name="srcScan"> the distance from one row of pixels to the next
		///        in the pixels array </param>
		/// <seealso cref= #getPixels </seealso>
		public virtual void SetPixels(int srcX, int srcY, int srcW, int srcH, ColorModel model, int[] pixels, int srcOff, int srcScan)
		{
			if (srcY < DstY)
			{
				int diff = DstY - srcY;
				if (diff >= srcH)
				{
					return;
				}
				srcOff += srcScan * diff;
				srcY += diff;
				srcH -= diff;
			}
			if (srcY + srcH > DstY + DstH)
			{
				srcH = (DstY + DstH) - srcY;
				if (srcH <= 0)
				{
					return;
				}
			}
			if (srcX < DstX)
			{
				int diff = DstX - srcX;
				if (diff >= srcW)
				{
					return;
				}
				srcOff += diff;
				srcX += diff;
				srcW -= diff;
			}
			if (srcX + srcW > DstX + DstW)
			{
				srcW = (DstX + DstW) - srcX;
				if (srcW <= 0)
				{
					return;
				}
			}
			if (IntPixels == null)
			{
				if (BytePixels == null)
				{
					IntPixels = new int[DstW * DstH];
					DstScan = DstW;
					DstOff = 0;
					ImageModel = model;
				}
				else
				{
					ConvertToRGB();
				}
			}
			int dstPtr = DstOff + (srcY - DstY) * DstScan + (srcX - DstX);
			if (ImageModel == model)
			{
				for (int h = srcH; h > 0; h--)
				{
					System.Array.Copy(pixels, srcOff, IntPixels, dstPtr, srcW);
					srcOff += srcScan;
					dstPtr += DstScan;
				}
			}
			else
			{
				if (ImageModel != ColorModel.RGBdefault)
				{
					ConvertToRGB();
				}
				int dstRem = DstScan - srcW;
				int srcRem = srcScan - srcW;
				for (int h = srcH; h > 0; h--)
				{
					for (int w = srcW; w > 0; w--)
					{
						IntPixels[dstPtr++] = model.GetRGB(pixels[srcOff++]);
					}
					srcOff += srcRem;
					dstPtr += dstRem;
				}
			}
			Flags |= ImageObserver_Fields.SOMEBITS;
		}

		/// <summary>
		/// The imageComplete method is part of the ImageConsumer API which
		/// this class must implement to retrieve the pixels.
		/// <para>
		/// Note: This method is intended to be called by the ImageProducer
		/// of the Image whose pixels are being grabbed.  Developers using
		/// this class to retrieve pixels from an image should avoid calling
		/// this method directly since that operation could result in problems
		/// with retrieving the requested pixels.
		/// </para>
		/// </summary>
		/// <param name="status"> the status of image loading </param>
		public virtual void ImageComplete(int status)
		{
			lock (this)
			{
				Grabbing = false;
				switch (status)
				{
				default:
					goto case ImageConsumer_Fields.IMAGEERROR;
				case ImageConsumer_Fields.IMAGEERROR:
					Flags |= ImageObserver_Fields.ERROR | ImageObserver_Fields.ABORT;
					break;
				case ImageConsumer_Fields.IMAGEABORTED:
					Flags |= ImageObserver_Fields.ABORT;
					break;
				case ImageConsumer_Fields.STATICIMAGEDONE:
					Flags |= ImageObserver_Fields.ALLBITS;
					break;
				case ImageConsumer_Fields.SINGLEFRAMEDONE:
					Flags |= ImageObserver_Fields.FRAMEBITS;
					break;
				}
				Producer.RemoveConsumer(this);
				Monitor.PulseAll(this);
			}
		}

		/// <summary>
		/// Returns the status of the pixels.  The ImageObserver flags
		/// representing the available pixel information are returned.
		/// This method and <seealso cref="#getStatus() getStatus"/> have the
		/// same implementation, but <code>getStatus</code> is the
		/// preferred method because it conforms to the convention of
		/// naming information-retrieval methods with the form
		/// "getXXX". </summary>
		/// <returns> the bitwise OR of all relevant ImageObserver flags </returns>
		/// <seealso cref= ImageObserver </seealso>
		/// <seealso cref= #getStatus() </seealso>
		public virtual int Status()
		{
			lock (this)
			{
				return Flags;
			}
		}
	}

}