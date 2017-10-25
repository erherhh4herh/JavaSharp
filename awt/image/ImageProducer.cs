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
	/// The interface for objects which can produce the image data for Images.
	/// Each image contains an ImageProducer which is used to reconstruct
	/// the image whenever it is needed, for example, when a new size of the
	/// Image is scaled, or when the width or height of the Image is being
	/// requested.
	/// </summary>
	/// <seealso cref= ImageConsumer
	/// 
	/// @author      Jim Graham </seealso>
	public interface ImageProducer
	{
		/// <summary>
		/// Registers an <code>ImageConsumer</code> with the
		/// <code>ImageProducer</code> for access to the image data
		/// during a later reconstruction of the <code>Image</code>.
		/// The <code>ImageProducer</code> may, at its discretion,
		/// start delivering the image data to the consumer
		/// using the <code>ImageConsumer</code> interface immediately,
		/// or when the next available image reconstruction is triggered
		/// by a call to the <code>startProduction</code> method. </summary>
		/// <param name="ic"> the specified <code>ImageConsumer</code> </param>
		/// <seealso cref= #startProduction </seealso>
		void AddConsumer(ImageConsumer ic);

		/// <summary>
		/// Determines if a specified <code>ImageConsumer</code>
		/// object is currently registered with this
		/// <code>ImageProducer</code> as one of its consumers. </summary>
		/// <param name="ic"> the specified <code>ImageConsumer</code> </param>
		/// <returns> <code>true</code> if the specified
		///         <code>ImageConsumer</code> is registered with
		///         this <code>ImageProducer</code>;
		///         <code>false</code> otherwise. </returns>
		bool IsConsumer(ImageConsumer ic);

		/// <summary>
		/// Removes the specified <code>ImageConsumer</code> object
		/// from the list of consumers currently registered to
		/// receive image data.  It is not considered an error
		/// to remove a consumer that is not currently registered.
		/// The <code>ImageProducer</code> should stop sending data
		/// to this consumer as soon as is feasible. </summary>
		/// <param name="ic"> the specified <code>ImageConsumer</code> </param>
		void RemoveConsumer(ImageConsumer ic);

		/// <summary>
		/// Registers the specified <code>ImageConsumer</code> object
		/// as a consumer and starts an immediate reconstruction of
		/// the image data which will then be delivered to this
		/// consumer and any other consumer which might have already
		/// been registered with the producer.  This method differs
		/// from the addConsumer method in that a reproduction of
		/// the image data should be triggered as soon as possible. </summary>
		/// <param name="ic"> the specified <code>ImageConsumer</code> </param>
		/// <seealso cref= #addConsumer </seealso>
		void StartProduction(ImageConsumer ic);

		/// <summary>
		/// Requests, on behalf of the <code>ImageConsumer</code>,
		/// that the <code>ImageProducer</code> attempt to resend
		/// the image data one more time in TOPDOWNLEFTRIGHT order
		/// so that higher quality conversion algorithms which
		/// depend on receiving pixels in order can be used to
		/// produce a better output version of the image.  The
		/// <code>ImageProducer</code> is free to
		/// ignore this call if it cannot resend the data in that
		/// order.  If the data can be resent, the
		/// <code>ImageProducer</code> should respond by executing
		/// the following minimum set of <code>ImageConsumer</code>
		/// method calls:
		/// <pre>{@code
		///  ic.setHints(TOPDOWNLEFTRIGHT | < otherhints >);
		///  ic.setPixels(...);      // As many times as needed
		///  ic.imageComplete();
		/// }</pre> </summary>
		/// <param name="ic"> the specified <code>ImageConsumer</code> </param>
		/// <seealso cref= ImageConsumer#setHints </seealso>
		void RequestTopDownLeftRightResend(ImageConsumer ic);
	}

}