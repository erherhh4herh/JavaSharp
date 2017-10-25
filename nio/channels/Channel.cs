/*
 * Copyright (c) 2000, 2013, Oracle and/or its affiliates. All rights reserved.
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
	/// A nexus for I/O operations.
	/// 
	/// <para> A channel represents an open connection to an entity such as a hardware
	/// device, a file, a network socket, or a program component that is capable of
	/// performing one or more distinct I/O operations, for example reading or
	/// writing.
	/// 
	/// </para>
	/// <para> A channel is either open or closed.  A channel is open upon creation,
	/// and once closed it remains closed.  Once a channel is closed, any attempt to
	/// invoke an I/O operation upon it will cause a <seealso cref="ClosedChannelException"/>
	/// to be thrown.  Whether or not a channel is open may be tested by invoking
	/// its <seealso cref="#isOpen isOpen"/> method.
	/// 
	/// </para>
	/// <para> Channels are, in general, intended to be safe for multithreaded access
	/// as described in the specifications of the interfaces and classes that extend
	/// and implement this interface.
	/// 
	/// 
	/// @author Mark Reinhold
	/// @author JSR-51 Expert Group
	/// @since 1.4
	/// </para>
	/// </summary>

	public interface Channel : Closeable
	{

		/// <summary>
		/// Tells whether or not this channel is open.
		/// </summary>
		/// <returns> <tt>true</tt> if, and only if, this channel is open </returns>
		bool Open {get;}

		/// <summary>
		/// Closes this channel.
		/// 
		/// <para> After a channel is closed, any further attempt to invoke I/O
		/// operations upon it will cause a <seealso cref="ClosedChannelException"/> to be
		/// thrown.
		/// 
		/// </para>
		/// <para> If this channel is already closed then invoking this method has no
		/// effect.
		/// 
		/// </para>
		/// <para> This method may be invoked at any time.  If some other thread has
		/// already invoked it, however, then another invocation will block until
		/// the first invocation is complete, after which it will return without
		/// effect. </para>
		/// </summary>
		/// <exception cref="IOException">  If an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void close() throws java.io.IOException;
		void Close();

	}

}