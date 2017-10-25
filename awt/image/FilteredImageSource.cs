using System.Collections;

/*
 * Copyright (c) 1995, 2003, Oracle and/or its affiliates. All rights reserved.
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
	/// takes an existing image and a filter object and uses them to produce
	/// image data for a new filtered version of the original image.
	/// Here is an example which filters an image by swapping the red and
	/// blue compents:
	/// <pre>
	/// 
	///      Image src = getImage("doc:///demo/images/duke/T1.gif");
	///      ImageFilter colorfilter = new RedBlueSwapFilter();
	///      Image img = createImage(new FilteredImageSource(src.getSource(),
	///                                                      colorfilter));
	/// 
	/// </pre>
	/// </summary>
	/// <seealso cref= ImageProducer
	/// 
	/// @author      Jim Graham </seealso>
	public class FilteredImageSource : ImageProducer
	{
		internal ImageProducer Src;
		internal ImageFilter Filter;

		/// <summary>
		/// Constructs an ImageProducer object from an existing ImageProducer
		/// and a filter object. </summary>
		/// <param name="orig"> the specified <code>ImageProducer</code> </param>
		/// <param name="imgf"> the specified <code>ImageFilter</code> </param>
		/// <seealso cref= ImageFilter </seealso>
		/// <seealso cref= java.awt.Component#createImage </seealso>
		public FilteredImageSource(ImageProducer orig, ImageFilter imgf)
		{
			Src = orig;
			Filter = imgf;
		}

		private Hashtable Proxies;

		/// <summary>
		/// Adds the specified <code>ImageConsumer</code>
		/// to the list of consumers interested in data for the filtered image.
		/// An instance of the original <code>ImageFilter</code>
		/// is created
		/// (using the filter's <code>getFilterInstance</code> method)
		/// to manipulate the image data
		/// for the specified <code>ImageConsumer</code>.
		/// The newly created filter instance
		/// is then passed to the <code>addConsumer</code> method
		/// of the original <code>ImageProducer</code>.
		/// 
		/// <para>
		/// This method is public as a side effect
		/// of this class implementing
		/// the <code>ImageProducer</code> interface.
		/// It should not be called from user code,
		/// and its behavior if called from user code is unspecified.
		/// 
		/// </para>
		/// </summary>
		/// <param name="ic">  the consumer for the filtered image </param>
		/// <seealso cref= ImageConsumer </seealso>
		public virtual void AddConsumer(ImageConsumer ic)
		{
			lock (this)
			{
				if (Proxies == null)
				{
					Proxies = new Hashtable();
				}
				if (!Proxies.ContainsKey(ic))
				{
					ImageFilter imgf = Filter.GetFilterInstance(ic);
					Proxies[ic] = imgf;
					Src.AddConsumer(imgf);
				}
			}
		}

		/// <summary>
		/// Determines whether an ImageConsumer is on the list of consumers
		/// currently interested in data for this image.
		/// 
		/// <para>
		/// This method is public as a side effect
		/// of this class implementing
		/// the <code>ImageProducer</code> interface.
		/// It should not be called from user code,
		/// and its behavior if called from user code is unspecified.
		/// 
		/// </para>
		/// </summary>
		/// <param name="ic"> the specified <code>ImageConsumer</code> </param>
		/// <returns> true if the ImageConsumer is on the list; false otherwise </returns>
		/// <seealso cref= ImageConsumer </seealso>
		public virtual bool IsConsumer(ImageConsumer ic)
		{
			lock (this)
			{
				return (Proxies != null && Proxies.ContainsKey(ic));
			}
		}

		/// <summary>
		/// Removes an ImageConsumer from the list of consumers interested in
		/// data for this image.
		/// 
		/// <para>
		/// This method is public as a side effect
		/// of this class implementing
		/// the <code>ImageProducer</code> interface.
		/// It should not be called from user code,
		/// and its behavior if called from user code is unspecified.
		/// 
		/// </para>
		/// </summary>
		/// <seealso cref= ImageConsumer </seealso>
		public virtual void RemoveConsumer(ImageConsumer ic)
		{
			lock (this)
			{
				if (Proxies != null)
				{
					ImageFilter imgf = (ImageFilter) Proxies[ic];
					if (imgf != null)
					{
						Src.RemoveConsumer(imgf);
						Proxies.Remove(ic);
						if (Proxies.Count == 0)
						{
							Proxies = null;
						}
					}
				}
			}
		}

		/// <summary>
		/// Starts production of the filtered image.
		/// If the specified <code>ImageConsumer</code>
		/// isn't already a consumer of the filtered image,
		/// an instance of the original <code>ImageFilter</code>
		/// is created
		/// (using the filter's <code>getFilterInstance</code> method)
		/// to manipulate the image data
		/// for the <code>ImageConsumer</code>.
		/// The filter instance for the <code>ImageConsumer</code>
		/// is then passed to the <code>startProduction</code> method
		/// of the original <code>ImageProducer</code>.
		/// 
		/// <para>
		/// This method is public as a side effect
		/// of this class implementing
		/// the <code>ImageProducer</code> interface.
		/// It should not be called from user code,
		/// and its behavior if called from user code is unspecified.
		/// 
		/// </para>
		/// </summary>
		/// <param name="ic">  the consumer for the filtered image </param>
		/// <seealso cref= ImageConsumer </seealso>
		public virtual void StartProduction(ImageConsumer ic)
		{
			if (Proxies == null)
			{
				Proxies = new Hashtable();
			}
			ImageFilter imgf = (ImageFilter) Proxies[ic];
			if (imgf == null)
			{
				imgf = Filter.GetFilterInstance(ic);
				Proxies[ic] = imgf;
			}
			Src.StartProduction(imgf);
		}

		/// <summary>
		/// Requests that a given ImageConsumer have the image data delivered
		/// one more time in top-down, left-right order.  The request is
		/// handed to the ImageFilter for further processing, since the
		/// ability to preserve the pixel ordering depends on the filter.
		/// 
		/// <para>
		/// This method is public as a side effect
		/// of this class implementing
		/// the <code>ImageProducer</code> interface.
		/// It should not be called from user code,
		/// and its behavior if called from user code is unspecified.
		/// 
		/// </para>
		/// </summary>
		/// <seealso cref= ImageConsumer </seealso>
		public virtual void RequestTopDownLeftRightResend(ImageConsumer ic)
		{
			if (Proxies != null)
			{
				ImageFilter imgf = (ImageFilter) Proxies[ic];
				if (imgf != null)
				{
					imgf.ResendTopDownLeftRight(Src);
				}
			}
		}
	}

}