/*
 * Copyright (c) 2001, Oracle and/or its affiliates. All rights reserved.
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

/*
 */

namespace java.nio.channels
{


	/// <summary>
	/// A channel that can be asynchronously closed and interrupted.
	/// 
	/// <para> A channel that implements this interface is <i>asynchronously
	/// closeable:</i> If a thread is blocked in an I/O operation on an
	/// interruptible channel then another thread may invoke the channel's {@link
	/// #close close} method.  This will cause the blocked thread to receive an
	/// <seealso cref="AsynchronousCloseException"/>.
	/// 
	/// </para>
	/// <para> A channel that implements this interface is also <i>interruptible:</i>
	/// If a thread is blocked in an I/O operation on an interruptible channel then
	/// another thread may invoke the blocked thread's {@link Thread#interrupt()
	/// interrupt} method.  This will cause the channel to be closed, the blocked
	/// thread to receive a <seealso cref="ClosedByInterruptException"/>, and the blocked
	/// thread's interrupt status to be set.
	/// 
	/// </para>
	/// <para> If a thread's interrupt status is already set and it invokes a blocking
	/// I/O operation upon a channel then the channel will be closed and the thread
	/// will immediately receive a <seealso cref="ClosedByInterruptException"/>; its interrupt
	/// status will remain set.
	/// 
	/// </para>
	/// <para> A channel supports asynchronous closing and interruption if, and only
	/// if, it implements this interface.  This can be tested at runtime, if
	/// necessary, via the <tt>instanceof</tt> operator.
	/// 
	/// 
	/// @author Mark Reinhold
	/// @author JSR-51 Expert Group
	/// @since 1.4
	/// </para>
	/// </summary>

	public interface InterruptibleChannel : Channel
	{

		/// <summary>
		/// Closes this channel.
		/// 
		/// <para> Any thread currently blocked in an I/O operation upon this channel
		/// will receive an <seealso cref="AsynchronousCloseException"/>.
		/// 
		/// </para>
		/// <para> This method otherwise behaves exactly as specified by the {@link
		/// Channel#close Channel} interface.  </para>
		/// </summary>
		/// <exception cref="IOException">  If an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void close() throws java.io.IOException;
		void Close();

	}

}