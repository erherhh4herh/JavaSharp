/*
 * Copyright (c) 2007, 2013, Oracle and/or its affiliates. All rights reserved.
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
	/// An asynchronous channel that can read and write bytes.
	/// 
	/// <para> Some channels may not allow more than one read or write to be outstanding
	/// at any given time. If a thread invokes a read method before a previous read
	/// operation has completed then a <seealso cref="ReadPendingException"/> will be thrown.
	/// Similarly, if a write method is invoked before a previous write has completed
	/// then <seealso cref="WritePendingException"/> is thrown. Whether or not other kinds of
	/// I/O operations may proceed concurrently with a read operation depends upon
	/// the type of the channel.
	/// 
	/// </para>
	/// <para> Note that <seealso cref="java.nio.ByteBuffer ByteBuffers"/> are not safe for use by
	/// multiple concurrent threads. When a read or write operation is initiated then
	/// care must be taken to ensure that the buffer is not accessed until the
	/// operation completes.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= Channels#newInputStream(AsynchronousByteChannel) </seealso>
	/// <seealso cref= Channels#newOutputStream(AsynchronousByteChannel)
	/// 
	/// @since 1.7 </seealso>

	public interface AsynchronousByteChannel : AsynchronousChannel
	{
		/// <summary>
		/// Reads a sequence of bytes from this channel into the given buffer.
		/// 
		/// <para> This method initiates an asynchronous read operation to read a
		/// sequence of bytes from this channel into the given buffer. The {@code
		/// handler} parameter is a completion handler that is invoked when the read
		/// operation completes (or fails). The result passed to the completion
		/// handler is the number of bytes read or {@code -1} if no bytes could be
		/// read because the channel has reached end-of-stream.
		/// 
		/// </para>
		/// <para> The read operation may read up to <i>r</i> bytes from the channel,
		/// where <i>r</i> is the number of bytes remaining in the buffer, that is,
		/// {@code dst.remaining()} at the time that the read is attempted. Where
		/// <i>r</i> is 0, the read operation completes immediately with a result of
		/// {@code 0} without initiating an I/O operation.
		/// 
		/// </para>
		/// <para> Suppose that a byte sequence of length <i>n</i> is read, where
		/// <tt>0</tt>&nbsp;<tt>&lt;</tt>&nbsp;<i>n</i>&nbsp;<tt>&lt;=</tt>&nbsp;<i>r</i>.
		/// This byte sequence will be transferred into the buffer so that the first
		/// byte in the sequence is at index <i>p</i> and the last byte is at index
		/// <i>p</i>&nbsp;<tt>+</tt>&nbsp;<i>n</i>&nbsp;<tt>-</tt>&nbsp;<tt>1</tt>,
		/// where <i>p</i> is the buffer's position at the moment the read is
		/// performed. Upon completion the buffer's position will be equal to
		/// <i>p</i>&nbsp;<tt>+</tt>&nbsp;<i>n</i>; its limit will not have changed.
		/// 
		/// </para>
		/// <para> Buffers are not safe for use by multiple concurrent threads so care
		/// should be taken to not access the buffer until the operation has
		/// completed.
		/// 
		/// </para>
		/// <para> This method may be invoked at any time. Some channel types may not
		/// allow more than one read to be outstanding at any given time. If a thread
		/// initiates a read operation before a previous read operation has
		/// completed then a <seealso cref="ReadPendingException"/> will be thrown.
		/// 
		/// </para>
		/// </summary>
		/// @param   <A>
		///          The type of the attachment </param>
		/// <param name="dst">
		///          The buffer into which bytes are to be transferred </param>
		/// <param name="attachment">
		///          The object to attach to the I/O operation; can be {@code null} </param>
		/// <param name="handler">
		///          The completion handler
		/// </param>
		/// <exception cref="IllegalArgumentException">
		///          If the buffer is read-only </exception>
		/// <exception cref="ReadPendingException">
		///          If the channel does not allow more than one read to be outstanding
		///          and a previous read has not completed </exception>
		/// <exception cref="ShutdownChannelGroupException">
		///          If the channel is associated with a {@link AsynchronousChannelGroup
		///          group} that has terminated </exception>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: <A> void read(java.nio.ByteBuffer dst, A attachment, CompletionHandler<Integer,? base A> handler);
		void read<A, T1>(ByteBuffer dst, A attachment, CompletionHandler<T1> handler);

		/// <summary>
		/// Reads a sequence of bytes from this channel into the given buffer.
		/// 
		/// <para> This method initiates an asynchronous read operation to read a
		/// sequence of bytes from this channel into the given buffer. The method
		/// behaves in exactly the same manner as the {@link
		/// #read(ByteBuffer,Object,CompletionHandler)
		/// read(ByteBuffer,Object,CompletionHandler)} method except that instead
		/// of specifying a completion handler, this method returns a {@code Future}
		/// representing the pending result. The {@code Future}'s {@link Future#get()
		/// get} method returns the number of bytes read or {@code -1} if no bytes
		/// could be read because the channel has reached end-of-stream.
		/// 
		/// </para>
		/// </summary>
		/// <param name="dst">
		///          The buffer into which bytes are to be transferred
		/// </param>
		/// <returns>  A Future representing the result of the operation
		/// </returns>
		/// <exception cref="IllegalArgumentException">
		///          If the buffer is read-only </exception>
		/// <exception cref="ReadPendingException">
		///          If the channel does not allow more than one read to be outstanding
		///          and a previous read has not completed </exception>
		Future<Integer> Read(ByteBuffer dst);

		/// <summary>
		/// Writes a sequence of bytes to this channel from the given buffer.
		/// 
		/// <para> This method initiates an asynchronous write operation to write a
		/// sequence of bytes to this channel from the given buffer. The {@code
		/// handler} parameter is a completion handler that is invoked when the write
		/// operation completes (or fails). The result passed to the completion
		/// handler is the number of bytes written.
		/// 
		/// </para>
		/// <para> The write operation may write up to <i>r</i> bytes to the channel,
		/// where <i>r</i> is the number of bytes remaining in the buffer, that is,
		/// {@code src.remaining()} at the time that the write is attempted. Where
		/// <i>r</i> is 0, the write operation completes immediately with a result of
		/// {@code 0} without initiating an I/O operation.
		/// 
		/// </para>
		/// <para> Suppose that a byte sequence of length <i>n</i> is written, where
		/// <tt>0</tt>&nbsp;<tt>&lt;</tt>&nbsp;<i>n</i>&nbsp;<tt>&lt;=</tt>&nbsp;<i>r</i>.
		/// This byte sequence will be transferred from the buffer starting at index
		/// <i>p</i>, where <i>p</i> is the buffer's position at the moment the
		/// write is performed; the index of the last byte written will be
		/// <i>p</i>&nbsp;<tt>+</tt>&nbsp;<i>n</i>&nbsp;<tt>-</tt>&nbsp;<tt>1</tt>.
		/// Upon completion the buffer's position will be equal to
		/// <i>p</i>&nbsp;<tt>+</tt>&nbsp;<i>n</i>; its limit will not have changed.
		/// 
		/// </para>
		/// <para> Buffers are not safe for use by multiple concurrent threads so care
		/// should be taken to not access the buffer until the operation has
		/// completed.
		/// 
		/// </para>
		/// <para> This method may be invoked at any time. Some channel types may not
		/// allow more than one write to be outstanding at any given time. If a thread
		/// initiates a write operation before a previous write operation has
		/// completed then a <seealso cref="WritePendingException"/> will be thrown.
		/// 
		/// </para>
		/// </summary>
		/// @param   <A>
		///          The type of the attachment </param>
		/// <param name="src">
		///          The buffer from which bytes are to be retrieved </param>
		/// <param name="attachment">
		///          The object to attach to the I/O operation; can be {@code null} </param>
		/// <param name="handler">
		///          The completion handler object
		/// </param>
		/// <exception cref="WritePendingException">
		///          If the channel does not allow more than one write to be outstanding
		///          and a previous write has not completed </exception>
		/// <exception cref="ShutdownChannelGroupException">
		///          If the channel is associated with a {@link AsynchronousChannelGroup
		///          group} that has terminated </exception>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: <A> void write(java.nio.ByteBuffer src, A attachment, CompletionHandler<Integer,? base A> handler);
		void write<A, T1>(ByteBuffer src, A attachment, CompletionHandler<T1> handler);

		/// <summary>
		/// Writes a sequence of bytes to this channel from the given buffer.
		/// 
		/// <para> This method initiates an asynchronous write operation to write a
		/// sequence of bytes to this channel from the given buffer. The method
		/// behaves in exactly the same manner as the {@link
		/// #write(ByteBuffer,Object,CompletionHandler)
		/// write(ByteBuffer,Object,CompletionHandler)} method except that instead
		/// of specifying a completion handler, this method returns a {@code Future}
		/// representing the pending result. The {@code Future}'s {@link Future#get()
		/// get} method returns the number of bytes written.
		/// 
		/// </para>
		/// </summary>
		/// <param name="src">
		///          The buffer from which bytes are to be retrieved
		/// </param>
		/// <returns> A Future representing the result of the operation
		/// </returns>
		/// <exception cref="WritePendingException">
		///          If the channel does not allow more than one write to be outstanding
		///          and a previous write has not completed </exception>
		Future<Integer> Write(ByteBuffer src);
	}

}