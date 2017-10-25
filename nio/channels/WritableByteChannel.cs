/*
 * Copyright (c) 2000, 2005, Oracle and/or its affiliates. All rights reserved.
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

namespace java.nio.channels
{



	/// <summary>
	/// A channel that can write bytes.
	/// 
	/// <para> Only one write operation upon a writable channel may be in progress at
	/// any given time.  If one thread initiates a write operation upon a channel
	/// then any other thread that attempts to initiate another write operation will
	/// block until the first operation is complete.  Whether or not other kinds of
	/// I/O operations may proceed concurrently with a write operation depends upon
	/// the type of the channel. </para>
	/// 
	/// 
	/// @author Mark Reinhold
	/// @author JSR-51 Expert Group
	/// @since 1.4
	/// </summary>

	public interface WritableByteChannel : Channel
	{

		/// <summary>
		/// Writes a sequence of bytes to this channel from the given buffer.
		/// 
		/// <para> An attempt is made to write up to <i>r</i> bytes to the channel,
		/// where <i>r</i> is the number of bytes remaining in the buffer, that is,
		/// <tt>src.remaining()</tt>, at the moment this method is invoked.
		/// 
		/// </para>
		/// <para> Suppose that a byte sequence of length <i>n</i> is written, where
		/// <tt>0</tt>&nbsp;<tt>&lt;=</tt>&nbsp;<i>n</i>&nbsp;<tt>&lt;=</tt>&nbsp;<i>r</i>.
		/// This byte sequence will be transferred from the buffer starting at index
		/// <i>p</i>, where <i>p</i> is the buffer's position at the moment this
		/// method is invoked; the index of the last byte written will be
		/// <i>p</i>&nbsp;<tt>+</tt>&nbsp;<i>n</i>&nbsp;<tt>-</tt>&nbsp;<tt>1</tt>.
		/// Upon return the buffer's position will be equal to
		/// <i>p</i>&nbsp;<tt>+</tt>&nbsp;<i>n</i>; its limit will not have changed.
		/// 
		/// </para>
		/// <para> Unless otherwise specified, a write operation will return only after
		/// writing all of the <i>r</i> requested bytes.  Some types of channels,
		/// depending upon their state, may write only some of the bytes or possibly
		/// none at all.  A socket channel in non-blocking mode, for example, cannot
		/// write any more bytes than are free in the socket's output buffer.
		/// 
		/// </para>
		/// <para> This method may be invoked at any time.  If another thread has
		/// already initiated a write operation upon this channel, however, then an
		/// invocation of this method will block until the first operation is
		/// complete. </para>
		/// </summary>
		/// <param name="src">
		///         The buffer from which bytes are to be retrieved
		/// </param>
		/// <returns> The number of bytes written, possibly zero
		/// </returns>
		/// <exception cref="NonWritableChannelException">
		///          If this channel was not opened for writing
		/// </exception>
		/// <exception cref="ClosedChannelException">
		///          If this channel is closed
		/// </exception>
		/// <exception cref="AsynchronousCloseException">
		///          If another thread closes this channel
		///          while the write operation is in progress
		/// </exception>
		/// <exception cref="ClosedByInterruptException">
		///          If another thread interrupts the current thread
		///          while the write operation is in progress, thereby
		///          closing the channel and setting the current thread's
		///          interrupt status
		/// </exception>
		/// <exception cref="IOException">
		///          If some other I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int write(java.nio.ByteBuffer src) throws java.io.IOException;
		int Write(ByteBuffer src);

	}

}