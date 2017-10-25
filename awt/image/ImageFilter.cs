﻿using System.Collections.Generic;

/*
 * Copyright (c) 1995, 2011, Oracle and/or its affiliates. All rights reserved.
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
	/// This class implements a filter for the set of interface methods that
	/// are used to deliver data from an ImageProducer to an ImageConsumer.
	/// It is meant to be used in conjunction with a FilteredImageSource
	/// object to produce filtered versions of existing images.  It is a
	/// base class that provides the calls needed to implement a "Null filter"
	/// which has no effect on the data being passed through.  Filters should
	/// subclass this class and override the methods which deal with the
	/// data that needs to be filtered and modify it as necessary.
	/// </summary>
	/// <seealso cref= FilteredImageSource </seealso>
	/// <seealso cref= ImageConsumer
	/// 
	/// @author      Jim Graham </seealso>
	public class ImageFilter : ImageConsumer, Cloneable
	{
		/// <summary>
		/// The consumer of the particular image data stream for which this
		/// instance of the ImageFilter is filtering data.  It is not
		/// initialized during the constructor, but rather during the
		/// getFilterInstance() method call when the FilteredImageSource
		/// is creating a unique instance of this object for a particular
		/// image data stream. </summary>
		/// <seealso cref= #getFilterInstance </seealso>
		/// <seealso cref= ImageConsumer </seealso>
		protected internal ImageConsumer Consumer;

		/// <summary>
		/// Returns a unique instance of an ImageFilter object which will
		/// actually perform the filtering for the specified ImageConsumer.
		/// The default implementation just clones this object.
		/// <para>
		/// Note: This method is intended to be called by the ImageProducer
		/// of the Image whose pixels are being filtered.  Developers using
		/// this class to filter pixels from an image should avoid calling
		/// this method directly since that operation could interfere
		/// with the filtering operation.
		/// </para>
		/// </summary>
		/// <param name="ic"> the specified <code>ImageConsumer</code> </param>
		/// <returns> an <code>ImageFilter</code> used to perform the
		///         filtering for the specified <code>ImageConsumer</code>. </returns>
		public virtual ImageFilter GetFilterInstance(ImageConsumer ic)
		{
			ImageFilter instance = (ImageFilter) Clone();
			instance.Consumer = ic;
			return instance;
		}

		/// <summary>
		/// Filters the information provided in the setDimensions method
		/// of the ImageConsumer interface.
		/// <para>
		/// Note: This method is intended to be called by the ImageProducer
		/// of the Image whose pixels are being filtered.  Developers using
		/// this class to filter pixels from an image should avoid calling
		/// this method directly since that operation could interfere
		/// with the filtering operation.
		/// </para>
		/// </summary>
		/// <seealso cref= ImageConsumer#setDimensions </seealso>
		public virtual void SetDimensions(int width, int height)
		{
			Consumer.SetDimensions(width, height);
		}

		/// <summary>
		/// Passes the properties from the source object along after adding a
		/// property indicating the stream of filters it has been run through.
		/// <para>
		/// Note: This method is intended to be called by the ImageProducer
		/// of the Image whose pixels are being filtered.  Developers using
		/// this class to filter pixels from an image should avoid calling
		/// this method directly since that operation could interfere
		/// with the filtering operation.
		/// 
		/// </para>
		/// </summary>
		/// <param name="props"> the properties from the source object </param>
		/// <exception cref="NullPointerException"> if <code>props</code> is null </exception>
		public virtual Dictionary<T1> Properties<T1>
		{
			set
			{
				Dictionary<Object, Object> p = (Dictionary<Object, Object>)value.clone();
				Object o = p["filters"];
				if (o == null)
				{
					p["filters"] = ToString();
				}
				else if (o is String)
				{
					p["filters"] = ((String) o) + ToString();
				}
				Consumer.Properties = p;
			}
		}

		/// <summary>
		/// Filter the information provided in the setColorModel method
		/// of the ImageConsumer interface.
		/// <para>
		/// Note: This method is intended to be called by the ImageProducer
		/// of the Image whose pixels are being filtered.  Developers using
		/// this class to filter pixels from an image should avoid calling
		/// this method directly since that operation could interfere
		/// with the filtering operation.
		/// </para>
		/// </summary>
		/// <seealso cref= ImageConsumer#setColorModel </seealso>
		public virtual ColorModel ColorModel
		{
			set
			{
				Consumer.ColorModel = value;
			}
		}

		/// <summary>
		/// Filters the information provided in the setHints method
		/// of the ImageConsumer interface.
		/// <para>
		/// Note: This method is intended to be called by the ImageProducer
		/// of the Image whose pixels are being filtered.  Developers using
		/// this class to filter pixels from an image should avoid calling
		/// this method directly since that operation could interfere
		/// with the filtering operation.
		/// </para>
		/// </summary>
		/// <seealso cref= ImageConsumer#setHints </seealso>
		public virtual int Hints
		{
			set
			{
				Consumer.Hints = value;
			}
		}

		/// <summary>
		/// Filters the information provided in the setPixels method of the
		/// ImageConsumer interface which takes an array of bytes.
		/// <para>
		/// Note: This method is intended to be called by the ImageProducer
		/// of the Image whose pixels are being filtered.  Developers using
		/// this class to filter pixels from an image should avoid calling
		/// this method directly since that operation could interfere
		/// with the filtering operation.
		/// </para>
		/// </summary>
		/// <seealso cref= ImageConsumer#setPixels </seealso>
		public virtual void SetPixels(int x, int y, int w, int h, ColorModel model, sbyte[] pixels, int off, int scansize)
		{
			Consumer.SetPixels(x, y, w, h, model, pixels, off, scansize);
		}

		/// <summary>
		/// Filters the information provided in the setPixels method of the
		/// ImageConsumer interface which takes an array of integers.
		/// <para>
		/// Note: This method is intended to be called by the ImageProducer
		/// of the Image whose pixels are being filtered.  Developers using
		/// this class to filter pixels from an image should avoid calling
		/// this method directly since that operation could interfere
		/// with the filtering operation.
		/// </para>
		/// </summary>
		/// <seealso cref= ImageConsumer#setPixels </seealso>
		public virtual void SetPixels(int x, int y, int w, int h, ColorModel model, int[] pixels, int off, int scansize)
		{
			Consumer.SetPixels(x, y, w, h, model, pixels, off, scansize);
		}

		/// <summary>
		/// Filters the information provided in the imageComplete method of
		/// the ImageConsumer interface.
		/// <para>
		/// Note: This method is intended to be called by the ImageProducer
		/// of the Image whose pixels are being filtered.  Developers using
		/// this class to filter pixels from an image should avoid calling
		/// this method directly since that operation could interfere
		/// with the filtering operation.
		/// </para>
		/// </summary>
		/// <seealso cref= ImageConsumer#imageComplete </seealso>
		public virtual void ImageComplete(int status)
		{
			Consumer.ImageComplete(status);
		}

		/// <summary>
		/// Responds to a request for a TopDownLeftRight (TDLR) ordered resend
		/// of the pixel data from an <code>ImageConsumer</code>.
		/// When an <code>ImageConsumer</code> being fed
		/// by an instance of this <code>ImageFilter</code>
		/// requests a resend of the data in TDLR order,
		/// the <code>FilteredImageSource</code>
		/// invokes this method of the <code>ImageFilter</code>.
		/// 
		/// <para>
		/// 
		/// An <code>ImageFilter</code> subclass might override this method or not,
		/// depending on if and how it can send data in TDLR order.
		/// Three possibilities exist:
		/// 
		/// <ul>
		/// <li>
		/// Do not override this method.
		/// This makes the subclass use the default implementation,
		/// which is to
		/// forward the request
		/// to the indicated <code>ImageProducer</code>
		/// using this filter as the requesting <code>ImageConsumer</code>.
		/// This behavior
		/// is appropriate if the filter can determine
		/// that it will forward the pixels
		/// in TDLR order if its upstream producer object
		/// sends them in TDLR order.
		/// 
		/// <li>
		/// Override the method to simply send the data.
		/// This is appropriate if the filter can handle the request itself &#151;
		/// for example,
		/// if the generated pixels have been saved in some sort of buffer.
		/// 
		/// <li>
		/// Override the method to do nothing.
		/// This is appropriate
		/// if the filter cannot produce filtered data in TDLR order.
		/// </ul>
		/// 
		/// </para>
		/// </summary>
		/// <seealso cref= ImageProducer#requestTopDownLeftRightResend </seealso>
		/// <param name="ip"> the ImageProducer that is feeding this instance of
		/// the filter - also the ImageProducer that the request should be
		/// forwarded to if necessary </param>
		/// <exception cref="NullPointerException"> if <code>ip</code> is null </exception>
		public virtual void ResendTopDownLeftRight(ImageProducer ip)
		{
			ip.RequestTopDownLeftRightResend(this);
		}

		/// <summary>
		/// Clones this object.
		/// </summary>
		public virtual Object Clone()
		{
			try
			{
				return base.Clone();
			}
			catch (CloneNotSupportedException e)
			{
				// this shouldn't happen, since we are Cloneable
				throw new InternalError(e);
			}
		}
	}

}