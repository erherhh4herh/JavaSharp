using System.Collections;

/*
 * Copyright (c) 1998, 2008, Oracle and/or its affiliates. All rights reserved.
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

/* ********************************************************************
 **********************************************************************
 **********************************************************************
 *** COPYRIGHT (c) Eastman Kodak Company, 1997                      ***
 *** As  an unpublished  work pursuant to Title 17 of the United    ***
 *** States Code.  All rights reserved.                             ***
 **********************************************************************
 **********************************************************************
 **********************************************************************/

namespace java.awt.image.renderable
{

	/// <summary>
	/// An adapter class that implements ImageProducer to allow the
	/// asynchronous production of a RenderableImage.  The size of the
	/// ImageConsumer is determined by the scale factor of the usr2dev
	/// transform in the RenderContext.  If the RenderContext is null, the
	/// default rendering of the RenderableImage is used.  This class
	/// implements an asynchronous production that produces the image in
	/// one thread at one resolution.  This class may be subclassed to
	/// implement versions that will render the image using several
	/// threads.  These threads could render either the same image at
	/// progressively better quality, or different sections of the image at
	/// a single resolution.
	/// </summary>
	public class RenderableImageProducer : ImageProducer, Runnable
	{

		/// <summary>
		/// The RenderableImage source for the producer. </summary>
		internal RenderableImage RdblImage;

		/// <summary>
		/// The RenderContext to use for producing the image. </summary>
		internal RenderContext Rc;

		/// <summary>
		/// A Vector of image consumers. </summary>
		internal ArrayList Ics = new ArrayList();

		/// <summary>
		/// Constructs a new RenderableImageProducer from a RenderableImage
		/// and a RenderContext.
		/// </summary>
		/// <param name="rdblImage"> the RenderableImage to be rendered. </param>
		/// <param name="rc"> the RenderContext to use for producing the pixels. </param>
		public RenderableImageProducer(RenderableImage rdblImage, RenderContext rc)
		{
			this.RdblImage = rdblImage;
			this.Rc = rc;
		}

		/// <summary>
		/// Sets a new RenderContext to use for the next startProduction() call.
		/// </summary>
		/// <param name="rc"> the new RenderContext. </param>
		public virtual RenderContext RenderContext
		{
			set
			{
				lock (this)
				{
					this.Rc = value;
				}
			}
		}

	   /// <summary>
	   /// Adds an ImageConsumer to the list of consumers interested in
	   /// data for this image.
	   /// </summary>
	   /// <param name="ic"> an ImageConsumer to be added to the interest list. </param>
		public virtual void AddConsumer(ImageConsumer ic)
		{
			lock (this)
			{
				if (!Ics.Contains(ic))
				{
					Ics.Add(ic);
				}
			}
		}

		/// <summary>
		/// Determine if an ImageConsumer is on the list of consumers
		/// currently interested in data for this image.
		/// </summary>
		/// <param name="ic"> the ImageConsumer to be checked. </param>
		/// <returns> true if the ImageConsumer is on the list; false otherwise. </returns>
		public virtual bool IsConsumer(ImageConsumer ic)
		{
			lock (this)
			{
				return Ics.Contains(ic);
			}
		}

		/// <summary>
		/// Remove an ImageConsumer from the list of consumers interested in
		/// data for this image.
		/// </summary>
		/// <param name="ic"> the ImageConsumer to be removed. </param>
		public virtual void RemoveConsumer(ImageConsumer ic)
		{
			lock (this)
			{
				Ics.Remove(ic);
			}
		}

		/// <summary>
		/// Adds an ImageConsumer to the list of consumers interested in
		/// data for this image, and immediately starts delivery of the
		/// image data through the ImageConsumer interface.
		/// </summary>
		/// <param name="ic"> the ImageConsumer to be added to the list of consumers. </param>
		public virtual void StartProduction(ImageConsumer ic)
		{
			lock (this)
			{
				AddConsumer(ic);
				// Need to build a runnable object for the Thread.
				Thread thread = new Thread(this, "RenderableImageProducer Thread");
				thread.Start();
			}
		}

		/// <summary>
		/// Requests that a given ImageConsumer have the image data delivered
		/// one more time in top-down, left-right order.
		/// </summary>
		/// <param name="ic"> the ImageConsumer requesting the resend. </param>
		public virtual void RequestTopDownLeftRightResend(ImageConsumer ic)
		{
			// So far, all pixels are already sent in TDLR order
		}

		/// <summary>
		/// The runnable method for this class. This will produce an image using
		/// the current RenderableImage and RenderContext and send it to all the
		/// ImageConsumer currently registered with this class.
		/// </summary>
		public virtual void Run()
		{
			// First get the rendered image
			RenderedImage rdrdImage;
			if (Rc != null)
			{
				rdrdImage = RdblImage.CreateRendering(Rc);
			}
			else
			{
				rdrdImage = RdblImage.CreateDefaultRendering();
			}

			// And its ColorModel
			ColorModel colorModel = rdrdImage.ColorModel;
			Raster raster = rdrdImage.Data;
			SampleModel sampleModel = raster.SampleModel;
			DataBuffer dataBuffer = raster.DataBuffer;

			if (colorModel == null)
			{
				colorModel = ColorModel.RGBdefault;
			}
			int minX = raster.MinX;
			int minY = raster.MinY;
			int width = raster.Width;
			int height = raster.Height;

			System.Collections.IEnumerator icList;
			ImageConsumer ic;
			// Set up the ImageConsumers
			icList = Ics.elements();
			while (icList.hasMoreElements())
			{
				ic = (ImageConsumer)icList.nextElement();
				ic.SetDimensions(width,height);
				ic.Hints = java.awt.image.ImageConsumer_Fields.TOPDOWNLEFTRIGHT | java.awt.image.ImageConsumer_Fields.COMPLETESCANLINES | java.awt.image.ImageConsumer_Fields.SINGLEPASS | java.awt.image.ImageConsumer_Fields.SINGLEFRAME;
			}

			// Get RGB pixels from the raster scanline by scanline and
			// send to consumers.
			int[] pix = new int[width];
			int i, j;
			int numBands = sampleModel.NumBands;
			int[] tmpPixel = new int[numBands];
			for (j = 0; j < height; j++)
			{
				for (i = 0; i < width; i++)
				{
					sampleModel.GetPixel(i, j, tmpPixel, dataBuffer);
					pix[i] = colorModel.GetDataElement(tmpPixel, 0);
				}
				// Now send the scanline to the Consumers
				icList = Ics.elements();
				while (icList.hasMoreElements())
				{
					ic = (ImageConsumer)icList.nextElement();
					ic.SetPixels(0, j, width, 1, colorModel, pix, 0, width);
				}
			}

			// Now tell the consumers we're done.
			icList = Ics.elements();
			while (icList.hasMoreElements())
			{
				ic = (ImageConsumer)icList.nextElement();
				ic.ImageComplete(java.awt.image.ImageConsumer_Fields.STATICIMAGEDONE);
			}
		}
	}

}