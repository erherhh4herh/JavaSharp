﻿/*
 * Copyright (c) 2000, 2006, Oracle and/or its affiliates. All rights reserved.
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
	/// A channel that can read bytes into a sequence of buffers.
	/// 
	/// <para> A <i>scattering</i> read operation reads, in a single invocation, a
	/// sequence of bytes into one or more of a given sequence of buffers.
	/// Scattering reads are often useful when implementing network protocols or
	/// file formats that, for example, group data into segments consisting of one
	/// or more fixed-length headers followed by a variable-length body.  Similar
	/// <i>gathering</i> write operations are defined in the {@link
	/// GatheringByteChannel} interface.  </para>
	/// 
	/// 
	/// @author Mark Reinhold
	/// @author JSR-51 Expert Group
	/// @since 1.4
	/// </summary>

	public interface ScatteringByteChannel : ReadableByteChannel
	{

		/// <summary>
		/// Reads a sequence of bytes from this channel into a subsequence of the
		/// given buffers.
		/// 
		/// <para> An invocation of this method attempts to read up to <i>r</i> bytes
		/// from this channel, where <i>r</i> is the total number of bytes remaining
		/// the specified subsequence of the given buffer array, that is,
		/// 
		/// <blockquote><pre>
		/// dsts[offset].remaining()
		///     + dsts[offset+1].remaining()
		///     + ... + dsts[offset+length-1].remaining()</pre></blockquote>
		/// 
		/// at the moment that this method is invoked.
		/// 
		/// </para>
		/// <para> Suppose that a byte sequence of length <i>n</i> is read, where
		/// <tt>0</tt>&nbsp;<tt>&lt;=</tt>&nbsp;<i>n</i>&nbsp;<tt>&lt;=</tt>&nbsp;<i>r</i>.
		/// Up to the first <tt>dsts[offset].remaining()</tt> bytes of this sequence
		/// are transferred into buffer <tt>dsts[offset]</tt>, up to the next
		/// <tt>dsts[offset+1].remaining()</tt> bytes are transferred into buffer
		/// <tt>dsts[offset+1]</tt>, and so forth, until the entire byte sequence
		/// is transferred into the given buffers.  As many bytes as possible are
		/// transferred into each buffer, hence the final position of each updated
		/// buffer, except the last updated buffer, is guaranteed to be equal to
		/// that buffer's limit.
		/// 
		/// </para>
		/// <para> This method may be invoked at any time.  If another thread has
		/// already initiated a read operation upon this channel, however, then an
		/// invocation of this method will block until the first operation is
		/// complete. </para>
		/// </summary>
		/// <param name="dsts">
		///         The buffers into which bytes are to be transferred
		/// </param>
		/// <param name="offset">
		///         The offset within the buffer array of the first buffer into
		///         which bytes are to be transferred; must be non-negative and no
		///         larger than <tt>dsts.length</tt>
		/// </param>
		/// <param name="length">
		///         The maximum number of buffers to be accessed; must be
		///         non-negative and no larger than
		///         <tt>dsts.length</tt>&nbsp;-&nbsp;<tt>offset</tt>
		/// </param>
		/// <returns> The number of bytes read, possibly zero,
		///         or <tt>-1</tt> if the channel has reached end-of-stream
		/// </returns>
		/// <exception cref="IndexOutOfBoundsException">
		///          If the preconditions on the <tt>offset</tt> and <tt>length</tt>
		///          parameters do not hold
		/// </exception>
		/// <exception cref="NonReadableChannelException">
		///          If this channel was not opened for reading
		/// </exception>
		/// <exception cref="ClosedChannelException">
		///          If this channel is closed
		/// </exception>
		/// <exception cref="AsynchronousCloseException">
		///          If another thread closes this channel
		///          while the read operation is in progress
		/// </exception>
		/// <exception cref="ClosedByInterruptException">
		///          If another thread interrupts the current thread
		///          while the read operation is in progress, thereby
		///          closing the channel and setting the current thread's
		///          interrupt status
		/// </exception>
		/// <exception cref="IOException">
		///          If some other I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public long read(java.nio.ByteBuffer[] dsts, int offset, int length) throws java.io.IOException;
		long Read(ByteBuffer[] dsts, int offset, int length);

		/// <summary>
		/// Reads a sequence of bytes from this channel into the given buffers.
		/// 
		/// <para> An invocation of this method of the form <tt>c.read(dsts)</tt>
		/// behaves in exactly the same manner as the invocation
		/// 
		/// <blockquote><pre>
		/// c.read(dsts, 0, dsts.length);</pre></blockquote>
		/// 
		/// </para>
		/// </summary>
		/// <param name="dsts">
		///         The buffers into which bytes are to be transferred
		/// </param>
		/// <returns> The number of bytes read, possibly zero,
		///         or <tt>-1</tt> if the channel has reached end-of-stream
		/// </returns>
		/// <exception cref="NonReadableChannelException">
		///          If this channel was not opened for reading
		/// </exception>
		/// <exception cref="ClosedChannelException">
		///          If this channel is closed
		/// </exception>
		/// <exception cref="AsynchronousCloseException">
		///          If another thread closes this channel
		///          while the read operation is in progress
		/// </exception>
		/// <exception cref="ClosedByInterruptException">
		///          If another thread interrupts the current thread
		///          while the read operation is in progress, thereby
		///          closing the channel and setting the current thread's
		///          interrupt status
		/// </exception>
		/// <exception cref="IOException">
		///          If some other I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public long read(java.nio.ByteBuffer[] dsts) throws java.io.IOException;
		long Read(ByteBuffer[] dsts);

	}

}