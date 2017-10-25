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
	/// The interface for objects expressing interest in image data through
	/// the ImageProducer interfaces.  When a consumer is added to an image
	/// producer, the producer delivers all of the data about the image
	/// using the method calls defined in this interface.
	/// </summary>
	/// <seealso cref= ImageProducer
	/// 
	/// @author      Jim Graham </seealso>
	public interface ImageConsumer
	{
		/// <summary>
		/// The dimensions of the source image are reported using the
		/// setDimensions method call. </summary>
		/// <param name="width"> the width of the source image </param>
		/// <param name="height"> the height of the source image </param>
		void SetDimensions(int width, int height);

		/// <summary>
		/// Sets the extensible list of properties associated with this image. </summary>
		/// <param name="props"> the list of properties to be associated with this
		///        image </param>
		Dictionary<T1> Properties<T1> {set;}

		/// <summary>
		/// Sets the ColorModel object used for the majority of
		/// the pixels reported using the setPixels method
		/// calls.  Note that each set of pixels delivered using setPixels
		/// contains its own ColorModel object, so no assumption should
		/// be made that this model will be the only one used in delivering
		/// pixel values.  A notable case where multiple ColorModel objects
		/// may be seen is a filtered image when for each set of pixels
		/// that it filters, the filter
		/// determines  whether the
		/// pixels can be sent on untouched, using the original ColorModel,
		/// or whether the pixels should be modified (filtered) and passed
		/// on using a ColorModel more convenient for the filtering process. </summary>
		/// <param name="model"> the specified <code>ColorModel</code> </param>
		/// <seealso cref= ColorModel </seealso>
		ColorModel ColorModel {set;}

		/// <summary>
		/// Sets the hints that the ImageConsumer uses to process the
		/// pixels delivered by the ImageProducer.
		/// The ImageProducer can deliver the pixels in any order, but
		/// the ImageConsumer may be able to scale or convert the pixels
		/// to the destination ColorModel more efficiently or with higher
		/// quality if it knows some information about how the pixels will
		/// be delivered up front.  The setHints method should be called
		/// before any calls to any of the setPixels methods with a bit mask
		/// of hints about the manner in which the pixels will be delivered.
		/// If the ImageProducer does not follow the guidelines for the
		/// indicated hint, the results are undefined. </summary>
		/// <param name="hintflags"> a set of hints that the ImageConsumer uses to
		///        process the pixels </param>
		int Hints {set;}

		/// <summary>
		/// The pixels will be delivered in a random order.  This tells the
		/// ImageConsumer not to use any optimizations that depend on the
		/// order of pixel delivery, which should be the default assumption
		/// in the absence of any call to the setHints method. </summary>
		/// <seealso cref= #setHints </seealso>

		/// <summary>
		/// The pixels will be delivered in top-down, left-to-right order. </summary>
		/// <seealso cref= #setHints </seealso>

		/// <summary>
		/// The pixels will be delivered in (multiples of) complete scanlines
		/// at a time. </summary>
		/// <seealso cref= #setHints </seealso>

		/// <summary>
		/// The pixels will be delivered in a single pass.  Each pixel will
		/// appear in only one call to any of the setPixels methods.  An
		/// example of an image format which does not meet this criterion
		/// is a progressive JPEG image which defines pixels in multiple
		/// passes, each more refined than the previous. </summary>
		/// <seealso cref= #setHints </seealso>

		/// <summary>
		/// The image contain a single static image.  The pixels will be defined
		/// in calls to the setPixels methods and then the imageComplete method
		/// will be called with the STATICIMAGEDONE flag after which no more
		/// image data will be delivered.  An example of an image type which
		/// would not meet these criteria would be the output of a video feed,
		/// or the representation of a 3D rendering being manipulated
		/// by the user.  The end of each frame in those types of images will
		/// be indicated by calling imageComplete with the SINGLEFRAMEDONE flag. </summary>
		/// <seealso cref= #setHints </seealso>
		/// <seealso cref= #imageComplete </seealso>

		/// <summary>
		/// Delivers the pixels of the image with one or more calls
		/// to this method.  Each call specifies the location and
		/// size of the rectangle of source pixels that are contained in
		/// the array of pixels.  The specified ColorModel object should
		/// be used to convert the pixels into their corresponding color
		/// and alpha components.  Pixel (m,n) is stored in the pixels array
		/// at index (n * scansize + m + off).  The pixels delivered using
		/// this method are all stored as bytes. </summary>
		/// <param name="x"> the X coordinate of the upper-left corner of the
		///        area of pixels to be set </param>
		/// <param name="y"> the Y coordinate of the upper-left corner of the
		///        area of pixels to be set </param>
		/// <param name="w"> the width of the area of pixels </param>
		/// <param name="h"> the height of the area of pixels </param>
		/// <param name="model"> the specified <code>ColorModel</code> </param>
		/// <param name="pixels"> the array of pixels </param>
		/// <param name="off"> the offset into the <code>pixels</code> array </param>
		/// <param name="scansize"> the distance from one row of pixels to the next in
		/// the <code>pixels</code> array </param>
		/// <seealso cref= ColorModel </seealso>
		void SetPixels(int x, int y, int w, int h, ColorModel model, sbyte[] pixels, int off, int scansize);

		/// <summary>
		/// The pixels of the image are delivered using one or more calls
		/// to the setPixels method.  Each call specifies the location and
		/// size of the rectangle of source pixels that are contained in
		/// the array of pixels.  The specified ColorModel object should
		/// be used to convert the pixels into their corresponding color
		/// and alpha components.  Pixel (m,n) is stored in the pixels array
		/// at index (n * scansize + m + off).  The pixels delivered using
		/// this method are all stored as ints.
		/// this method are all stored as ints. </summary>
		/// <param name="x"> the X coordinate of the upper-left corner of the
		///        area of pixels to be set </param>
		/// <param name="y"> the Y coordinate of the upper-left corner of the
		///        area of pixels to be set </param>
		/// <param name="w"> the width of the area of pixels </param>
		/// <param name="h"> the height of the area of pixels </param>
		/// <param name="model"> the specified <code>ColorModel</code> </param>
		/// <param name="pixels"> the array of pixels </param>
		/// <param name="off"> the offset into the <code>pixels</code> array </param>
		/// <param name="scansize"> the distance from one row of pixels to the next in
		/// the <code>pixels</code> array </param>
		/// <seealso cref= ColorModel </seealso>
		void SetPixels(int x, int y, int w, int h, ColorModel model, int[] pixels, int off, int scansize);

		/// <summary>
		/// The imageComplete method is called when the ImageProducer is
		/// finished delivering all of the pixels that the source image
		/// contains, or when a single frame of a multi-frame animation has
		/// been completed, or when an error in loading or producing the
		/// image has occurred.  The ImageConsumer should remove itself from the
		/// list of consumers registered with the ImageProducer at this time,
		/// unless it is interested in successive frames. </summary>
		/// <param name="status"> the status of image loading </param>
		/// <seealso cref= ImageProducer#removeConsumer </seealso>
		void ImageComplete(int status);

		/// <summary>
		/// An error was encountered while producing the image. </summary>
		/// <seealso cref= #imageComplete </seealso>

		/// <summary>
		/// One frame of the image is complete but there are more frames
		/// to be delivered. </summary>
		/// <seealso cref= #imageComplete </seealso>

		/// <summary>
		/// The image is complete and there are no more pixels or frames
		/// to be delivered. </summary>
		/// <seealso cref= #imageComplete </seealso>

		/// <summary>
		/// The image creation process was deliberately aborted. </summary>
		/// <seealso cref= #imageComplete </seealso>
	}

	public static class ImageConsumer_Fields
	{
		public const int RANDOMPIXELORDER = 1;
		public const int TOPDOWNLEFTRIGHT = 2;
		public const int COMPLETESCANLINES = 4;
		public const int SINGLEPASS = 8;
		public const int SINGLEFRAME = 16;
		public const int IMAGEERROR = 1;
		public const int SINGLEFRAMEDONE = 2;
		public const int STATICIMAGEDONE = 3;
		public const int IMAGEABORTED = 4;
	}

}