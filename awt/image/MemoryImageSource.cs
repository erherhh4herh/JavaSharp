using System;
using System.Collections;
using System.Collections.Generic;

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
	/// This class is an implementation of the ImageProducer interface which
	/// uses an array to produce pixel values for an Image.  Here is an example
	/// which calculates a 100x100 image representing a fade from black to blue
	/// along the X axis and a fade from black to red along the Y axis:
	/// <pre>{@code
	/// 
	///      int w = 100;
	///      int h = 100;
	///      int pix[] = new int[w * h];
	///      int index = 0;
	///      for (int y = 0; y < h; y++) {
	///          int red = (y * 255) / (h - 1);
	///          for (int x = 0; x < w; x++) {
	///              int blue = (x * 255) / (w - 1);
	///              pix[index++] = (255 << 24) | (red << 16) | blue;
	///          }
	///      }
	///      Image img = createImage(new MemoryImageSource(w, h, pix, 0, w));
	/// 
	/// }</pre>
	/// The MemoryImageSource is also capable of managing a memory image which
	/// varies over time to allow animation or custom rendering.  Here is an
	/// example showing how to set up the animation source and signal changes
	/// in the data (adapted from the MemoryAnimationSourceDemo by Garth Dickie):
	/// <pre>{@code
	/// 
	///      int pixels[];
	///      MemoryImageSource source;
	/// 
	///      public void init() {
	///          int width = 50;
	///          int height = 50;
	///          int size = width * height;
	///          pixels = new int[size];
	/// 
	///          int value = getBackground().getRGB();
	///          for (int i = 0; i < size; i++) {
	///              pixels[i] = value;
	///          }
	/// 
	///          source = new MemoryImageSource(width, height, pixels, 0, width);
	///          source.setAnimated(true);
	///          image = createImage(source);
	///      }
	/// 
	///      public void run() {
	///          Thread me = Thread.currentThread( );
	///          me.setPriority(Thread.MIN_PRIORITY);
	/// 
	///          while (true) {
	///              try {
	///                  Thread.sleep(10);
	///              } catch( InterruptedException e ) {
	///                  return;
	///              }
	/// 
	///              // Modify the values in the pixels array at (x, y, w, h)
	/// 
	///              // Send the new data to the interested ImageConsumers
	///              source.newPixels(x, y, w, h);
	///          }
	///      }
	/// 
	/// }</pre>
	/// </summary>
	/// <seealso cref= ImageProducer
	/// 
	/// @author      Jim Graham
	/// @author      Animation capabilities inspired by the
	///              MemoryAnimationSource class written by Garth Dickie </seealso>
	public class MemoryImageSource : ImageProducer
	{
		internal int Width;
		internal int Height;
		internal ColorModel Model;
		internal Object Pixels;
		internal int Pixeloffset;
		internal int Pixelscan;
		internal Hashtable Properties;
		internal ArrayList TheConsumers = new ArrayList();
		internal bool Animating;
		internal bool Fullbuffers;

		/// <summary>
		/// Constructs an ImageProducer object which uses an array of bytes
		/// to produce data for an Image object. </summary>
		/// <param name="w"> the width of the rectangle of pixels </param>
		/// <param name="h"> the height of the rectangle of pixels </param>
		/// <param name="cm"> the specified <code>ColorModel</code> </param>
		/// <param name="pix"> an array of pixels </param>
		/// <param name="off"> the offset into the array of where to store the
		///        first pixel </param>
		/// <param name="scan"> the distance from one row of pixels to the next in
		///        the array </param>
		/// <seealso cref= java.awt.Component#createImage </seealso>
		public MemoryImageSource(int w, int h, ColorModel cm, sbyte[] pix, int off, int scan)
		{
			Initialize(w, h, cm, (Object) pix, off, scan, null);
		}

		/// <summary>
		/// Constructs an ImageProducer object which uses an array of bytes
		/// to produce data for an Image object. </summary>
		/// <param name="w"> the width of the rectangle of pixels </param>
		/// <param name="h"> the height of the rectangle of pixels </param>
		/// <param name="cm"> the specified <code>ColorModel</code> </param>
		/// <param name="pix"> an array of pixels </param>
		/// <param name="off"> the offset into the array of where to store the
		///        first pixel </param>
		/// <param name="scan"> the distance from one row of pixels to the next in
		///        the array </param>
		/// <param name="props"> a list of properties that the <code>ImageProducer</code>
		///        uses to process an image </param>
		/// <seealso cref= java.awt.Component#createImage </seealso>
		public MemoryImageSource<T1>(int w, int h, ColorModel cm, sbyte[] pix, int off, int scan, Dictionary<T1> props)
		{
			Initialize(w, h, cm, (Object) pix, off, scan, props);
		}

		/// <summary>
		/// Constructs an ImageProducer object which uses an array of integers
		/// to produce data for an Image object. </summary>
		/// <param name="w"> the width of the rectangle of pixels </param>
		/// <param name="h"> the height of the rectangle of pixels </param>
		/// <param name="cm"> the specified <code>ColorModel</code> </param>
		/// <param name="pix"> an array of pixels </param>
		/// <param name="off"> the offset into the array of where to store the
		///        first pixel </param>
		/// <param name="scan"> the distance from one row of pixels to the next in
		///        the array </param>
		/// <seealso cref= java.awt.Component#createImage </seealso>
		public MemoryImageSource(int w, int h, ColorModel cm, int[] pix, int off, int scan)
		{
			Initialize(w, h, cm, (Object) pix, off, scan, null);
		}

		/// <summary>
		/// Constructs an ImageProducer object which uses an array of integers
		/// to produce data for an Image object. </summary>
		/// <param name="w"> the width of the rectangle of pixels </param>
		/// <param name="h"> the height of the rectangle of pixels </param>
		/// <param name="cm"> the specified <code>ColorModel</code> </param>
		/// <param name="pix"> an array of pixels </param>
		/// <param name="off"> the offset into the array of where to store the
		///        first pixel </param>
		/// <param name="scan"> the distance from one row of pixels to the next in
		///        the array </param>
		/// <param name="props"> a list of properties that the <code>ImageProducer</code>
		///        uses to process an image </param>
		/// <seealso cref= java.awt.Component#createImage </seealso>
		public MemoryImageSource<T1>(int w, int h, ColorModel cm, int[] pix, int off, int scan, Dictionary<T1> props)
		{
			Initialize(w, h, cm, (Object) pix, off, scan, props);
		}

		private void Initialize(int w, int h, ColorModel cm, Object pix, int off, int scan, Hashtable props)
		{
			Width = w;
			Height = h;
			Model = cm;
			Pixels = pix;
			Pixeloffset = off;
			Pixelscan = scan;
			if (props == null)
			{
				props = new Hashtable();
			}
			Properties = props;
		}

		/// <summary>
		/// Constructs an ImageProducer object which uses an array of integers
		/// in the default RGB ColorModel to produce data for an Image object. </summary>
		/// <param name="w"> the width of the rectangle of pixels </param>
		/// <param name="h"> the height of the rectangle of pixels </param>
		/// <param name="pix"> an array of pixels </param>
		/// <param name="off"> the offset into the array of where to store the
		///        first pixel </param>
		/// <param name="scan"> the distance from one row of pixels to the next in
		///        the array </param>
		/// <seealso cref= java.awt.Component#createImage </seealso>
		/// <seealso cref= ColorModel#getRGBdefault </seealso>
		public MemoryImageSource(int w, int h, int[] pix, int off, int scan)
		{
			Initialize(w, h, ColorModel.RGBdefault, (Object) pix, off, scan, null);
		}

		/// <summary>
		/// Constructs an ImageProducer object which uses an array of integers
		/// in the default RGB ColorModel to produce data for an Image object. </summary>
		/// <param name="w"> the width of the rectangle of pixels </param>
		/// <param name="h"> the height of the rectangle of pixels </param>
		/// <param name="pix"> an array of pixels </param>
		/// <param name="off"> the offset into the array of where to store the
		///        first pixel </param>
		/// <param name="scan"> the distance from one row of pixels to the next in
		///        the array </param>
		/// <param name="props"> a list of properties that the <code>ImageProducer</code>
		///        uses to process an image </param>
		/// <seealso cref= java.awt.Component#createImage </seealso>
		/// <seealso cref= ColorModel#getRGBdefault </seealso>
		public MemoryImageSource<T1>(int w, int h, int[] pix, int off, int scan, Dictionary<T1> props)
		{
			Initialize(w, h, ColorModel.RGBdefault, (Object) pix, off, scan, props);
		}

		/// <summary>
		/// Adds an ImageConsumer to the list of consumers interested in
		/// data for this image. </summary>
		/// <param name="ic"> the specified <code>ImageConsumer</code> </param>
		/// <exception cref="NullPointerException"> if the specified
		///           <code>ImageConsumer</code> is null </exception>
		/// <seealso cref= ImageConsumer </seealso>
		public virtual void AddConsumer(ImageConsumer ic)
		{
			lock (this)
			{
				if (TheConsumers.Contains(ic))
				{
					return;
				}
				TheConsumers.Add(ic);
				try
				{
					InitConsumer(ic);
					SendPixels(ic, 0, 0, Width, Height);
					if (IsConsumer(ic))
					{
						ic.ImageComplete(Animating ? java.awt.image.ImageConsumer_Fields.SINGLEFRAMEDONE : java.awt.image.ImageConsumer_Fields.STATICIMAGEDONE);
						if (!Animating && IsConsumer(ic))
						{
							ic.ImageComplete(java.awt.image.ImageConsumer_Fields.IMAGEERROR);
							RemoveConsumer(ic);
						}
					}
				}
				catch (Exception)
				{
					if (IsConsumer(ic))
					{
						ic.ImageComplete(java.awt.image.ImageConsumer_Fields.IMAGEERROR);
					}
				}
			}
		}

		/// <summary>
		/// Determines if an ImageConsumer is on the list of consumers currently
		/// interested in data for this image. </summary>
		/// <param name="ic"> the specified <code>ImageConsumer</code> </param>
		/// <returns> <code>true</code> if the <code>ImageConsumer</code>
		/// is on the list; <code>false</code> otherwise. </returns>
		/// <seealso cref= ImageConsumer </seealso>
		public virtual bool IsConsumer(ImageConsumer ic)
		{
			lock (this)
			{
				return TheConsumers.Contains(ic);
			}
		}

		/// <summary>
		/// Removes an ImageConsumer from the list of consumers interested in
		/// data for this image. </summary>
		/// <param name="ic"> the specified <code>ImageConsumer</code> </param>
		/// <seealso cref= ImageConsumer </seealso>
		public virtual void RemoveConsumer(ImageConsumer ic)
		{
			lock (this)
			{
				TheConsumers.Remove(ic);
			}
		}

		/// <summary>
		/// Adds an ImageConsumer to the list of consumers interested in
		/// data for this image and immediately starts delivery of the
		/// image data through the ImageConsumer interface. </summary>
		/// <param name="ic"> the specified <code>ImageConsumer</code>
		/// image data through the ImageConsumer interface. </param>
		/// <seealso cref= ImageConsumer </seealso>
		public virtual void StartProduction(ImageConsumer ic)
		{
			AddConsumer(ic);
		}

		/// <summary>
		/// Requests that a given ImageConsumer have the image data delivered
		/// one more time in top-down, left-right order. </summary>
		/// <param name="ic"> the specified <code>ImageConsumer</code> </param>
		/// <seealso cref= ImageConsumer </seealso>
		public virtual void RequestTopDownLeftRightResend(ImageConsumer ic)
		{
			// Ignored.  The data is either single frame and already in TDLR
			// format or it is multi-frame and TDLR resends aren't critical.
		}

		/// <summary>
		/// Changes this memory image into a multi-frame animation or a
		/// single-frame static image depending on the animated parameter.
		/// <para>This method should be called immediately after the
		/// MemoryImageSource is constructed and before an image is
		/// created with it to ensure that all ImageConsumers will
		/// receive the correct multi-frame data.  If an ImageConsumer
		/// is added to this ImageProducer before this flag is set then
		/// that ImageConsumer will see only a snapshot of the pixel
		/// data that was available when it connected.
		/// </para>
		/// </summary>
		/// <param name="animated"> <code>true</code> if the image is a
		///       multi-frame animation </param>
		public virtual bool Animated
		{
			set
			{
				lock (this)
				{
					this.Animating = value;
					if (!Animating)
					{
						System.Collections.IEnumerator enum_ = TheConsumers.elements();
						while (enum_.hasMoreElements())
						{
							ImageConsumer ic = (ImageConsumer) enum_.nextElement();
							ic.ImageComplete(java.awt.image.ImageConsumer_Fields.STATICIMAGEDONE);
							if (IsConsumer(ic))
							{
								ic.ImageComplete(java.awt.image.ImageConsumer_Fields.IMAGEERROR);
							}
						}
						TheConsumers.Clear();
					}
				}
			}
		}

		/// <summary>
		/// Specifies whether this animated memory image should always be
		/// updated by sending the complete buffer of pixels whenever
		/// there is a change.
		/// This flag is ignored if the animation flag is not turned on
		/// through the setAnimated() method.
		/// <para>This method should be called immediately after the
		/// MemoryImageSource is constructed and before an image is
		/// created with it to ensure that all ImageConsumers will
		/// receive the correct pixel delivery hints.
		/// </para>
		/// </summary>
		/// <param name="fullbuffers"> <code>true</code> if the complete pixel
		///             buffer should always
		/// be sent </param>
		/// <seealso cref= #setAnimated </seealso>
		public virtual bool FullBufferUpdates
		{
			set
			{
				lock (this)
				{
					if (this.Fullbuffers == value)
					{
						return;
					}
					this.Fullbuffers = value;
					if (Animating)
					{
						System.Collections.IEnumerator enum_ = TheConsumers.elements();
						while (enum_.hasMoreElements())
						{
							ImageConsumer ic = (ImageConsumer) enum_.nextElement();
							ic.Hints = value ? (java.awt.image.ImageConsumer_Fields.TOPDOWNLEFTRIGHT | java.awt.image.ImageConsumer_Fields.COMPLETESCANLINES) : java.awt.image.ImageConsumer_Fields.RANDOMPIXELORDER;
						}
					}
				}
			}
		}

		/// <summary>
		/// Sends a whole new buffer of pixels to any ImageConsumers that
		/// are currently interested in the data for this image and notify
		/// them that an animation frame is complete.
		/// This method only has effect if the animation flag has been
		/// turned on through the setAnimated() method. </summary>
		/// <seealso cref= #newPixels(int, int, int, int, boolean) </seealso>
		/// <seealso cref= ImageConsumer </seealso>
		/// <seealso cref= #setAnimated </seealso>
		public virtual void NewPixels()
		{
			NewPixels(0, 0, Width, Height, true);
		}

		/// <summary>
		/// Sends a rectangular region of the buffer of pixels to any
		/// ImageConsumers that are currently interested in the data for
		/// this image and notify them that an animation frame is complete.
		/// This method only has effect if the animation flag has been
		/// turned on through the setAnimated() method.
		/// If the full buffer update flag was turned on with the
		/// setFullBufferUpdates() method then the rectangle parameters
		/// will be ignored and the entire buffer will always be sent. </summary>
		/// <param name="x"> the x coordinate of the upper left corner of the rectangle
		/// of pixels to be sent </param>
		/// <param name="y"> the y coordinate of the upper left corner of the rectangle
		/// of pixels to be sent </param>
		/// <param name="w"> the width of the rectangle of pixels to be sent </param>
		/// <param name="h"> the height of the rectangle of pixels to be sent </param>
		/// <seealso cref= #newPixels(int, int, int, int, boolean) </seealso>
		/// <seealso cref= ImageConsumer </seealso>
		/// <seealso cref= #setAnimated </seealso>
		/// <seealso cref= #setFullBufferUpdates </seealso>
		public virtual void NewPixels(int x, int y, int w, int h)
		{
			lock (this)
			{
				NewPixels(x, y, w, h, true);
			}
		}

		/// <summary>
		/// Sends a rectangular region of the buffer of pixels to any
		/// ImageConsumers that are currently interested in the data for
		/// this image.
		/// If the framenotify parameter is true then the consumers are
		/// also notified that an animation frame is complete.
		/// This method only has effect if the animation flag has been
		/// turned on through the setAnimated() method.
		/// If the full buffer update flag was turned on with the
		/// setFullBufferUpdates() method then the rectangle parameters
		/// will be ignored and the entire buffer will always be sent. </summary>
		/// <param name="x"> the x coordinate of the upper left corner of the rectangle
		/// of pixels to be sent </param>
		/// <param name="y"> the y coordinate of the upper left corner of the rectangle
		/// of pixels to be sent </param>
		/// <param name="w"> the width of the rectangle of pixels to be sent </param>
		/// <param name="h"> the height of the rectangle of pixels to be sent </param>
		/// <param name="framenotify"> <code>true</code> if the consumers should be sent a
		/// <seealso cref="ImageConsumer#SINGLEFRAMEDONE SINGLEFRAMEDONE"/> notification </param>
		/// <seealso cref= ImageConsumer </seealso>
		/// <seealso cref= #setAnimated </seealso>
		/// <seealso cref= #setFullBufferUpdates </seealso>
		public virtual void NewPixels(int x, int y, int w, int h, bool framenotify)
		{
			lock (this)
			{
				if (Animating)
				{
					if (Fullbuffers)
					{
						x = y = 0;
						w = Width;
						h = Height;
					}
					else
					{
						if (x < 0)
						{
							w += x;
							x = 0;
						}
						if (x + w > Width)
						{
							w = Width - x;
						}
						if (y < 0)
						{
							h += y;
							y = 0;
						}
						if (y + h > Height)
						{
							h = Height - y;
						}
					}
					if ((w <= 0 || h <= 0) && !framenotify)
					{
						return;
					}
					System.Collections.IEnumerator enum_ = TheConsumers.elements();
					while (enum_.hasMoreElements())
					{
						ImageConsumer ic = (ImageConsumer) enum_.nextElement();
						if (w > 0 && h > 0)
						{
							SendPixels(ic, x, y, w, h);
						}
						if (framenotify && IsConsumer(ic))
						{
							ic.ImageComplete(java.awt.image.ImageConsumer_Fields.SINGLEFRAMEDONE);
						}
					}
				}
			}
		}

		/// <summary>
		/// Changes to a new byte array to hold the pixels for this image.
		/// If the animation flag has been turned on through the setAnimated()
		/// method, then the new pixels will be immediately delivered to any
		/// ImageConsumers that are currently interested in the data for
		/// this image. </summary>
		/// <param name="newpix"> the new pixel array </param>
		/// <param name="newmodel"> the specified <code>ColorModel</code> </param>
		/// <param name="offset"> the offset into the array </param>
		/// <param name="scansize"> the distance from one row of pixels to the next in
		/// the array </param>
		/// <seealso cref= #newPixels(int, int, int, int, boolean) </seealso>
		/// <seealso cref= #setAnimated </seealso>
		public virtual void NewPixels(sbyte[] newpix, ColorModel newmodel, int offset, int scansize)
		{
			lock (this)
			{
				this.Pixels = newpix;
				this.Model = newmodel;
				this.Pixeloffset = offset;
				this.Pixelscan = scansize;
				NewPixels();
			}
		}

		/// <summary>
		/// Changes to a new int array to hold the pixels for this image.
		/// If the animation flag has been turned on through the setAnimated()
		/// method, then the new pixels will be immediately delivered to any
		/// ImageConsumers that are currently interested in the data for
		/// this image. </summary>
		/// <param name="newpix"> the new pixel array </param>
		/// <param name="newmodel"> the specified <code>ColorModel</code> </param>
		/// <param name="offset"> the offset into the array </param>
		/// <param name="scansize"> the distance from one row of pixels to the next in
		/// the array </param>
		/// <seealso cref= #newPixels(int, int, int, int, boolean) </seealso>
		/// <seealso cref= #setAnimated </seealso>
		public virtual void NewPixels(int[] newpix, ColorModel newmodel, int offset, int scansize)
		{
			lock (this)
			{
				this.Pixels = newpix;
				this.Model = newmodel;
				this.Pixeloffset = offset;
				this.Pixelscan = scansize;
				NewPixels();
			}
		}

		private void InitConsumer(ImageConsumer ic)
		{
			if (IsConsumer(ic))
			{
				ic.SetDimensions(Width, Height);
			}
			if (IsConsumer(ic))
			{
				ic.Properties = Properties;
			}
			if (IsConsumer(ic))
			{
				ic.ColorModel = Model;
			}
			if (IsConsumer(ic))
			{
				ic.Hints = Animating ? (Fullbuffers ? (java.awt.image.ImageConsumer_Fields.TOPDOWNLEFTRIGHT | java.awt.image.ImageConsumer_Fields.COMPLETESCANLINES) : java.awt.image.ImageConsumer_Fields.RANDOMPIXELORDER) : (java.awt.image.ImageConsumer_Fields.TOPDOWNLEFTRIGHT | java.awt.image.ImageConsumer_Fields.COMPLETESCANLINES | java.awt.image.ImageConsumer_Fields.SINGLEPASS | java.awt.image.ImageConsumer_Fields.SINGLEFRAME);
			}
		}

		private void SendPixels(ImageConsumer ic, int x, int y, int w, int h)
		{
			int off = Pixeloffset + Pixelscan * y + x;
			if (IsConsumer(ic))
			{
				if (Pixels is sbyte[])
				{
					ic.SetPixels(x, y, w, h, Model, ((sbyte[]) Pixels), off, Pixelscan);
				}
				else
				{
					ic.SetPixels(x, y, w, h, Model, ((int[]) Pixels), off, Pixelscan);
				}
			}
		}
	}

}