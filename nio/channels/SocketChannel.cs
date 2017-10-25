using System.Diagnostics;

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
	/// A selectable channel for stream-oriented connecting sockets.
	/// 
	/// <para> A socket channel is created by invoking one of the <seealso cref="#open open"/>
	/// methods of this class.  It is not possible to create a channel for an arbitrary,
	/// pre-existing socket. A newly-created socket channel is open but not yet
	/// connected.  An attempt to invoke an I/O operation upon an unconnected
	/// channel will cause a <seealso cref="NotYetConnectedException"/> to be thrown.  A
	/// socket channel can be connected by invoking its <seealso cref="#connect connect"/>
	/// method; once connected, a socket channel remains connected until it is
	/// closed.  Whether or not a socket channel is connected may be determined by
	/// invoking its <seealso cref="#isConnected isConnected"/> method.
	/// 
	/// </para>
	/// <para> Socket channels support <i>non-blocking connection:</i>&nbsp;A socket
	/// channel may be created and the process of establishing the link to the
	/// remote socket may be initiated via the <seealso cref="#connect connect"/> method for
	/// later completion by the <seealso cref="#finishConnect finishConnect"/> method.
	/// Whether or not a connection operation is in progress may be determined by
	/// invoking the <seealso cref="#isConnectionPending isConnectionPending"/> method.
	/// 
	/// </para>
	/// <para> Socket channels support <i>asynchronous shutdown,</i> which is similar
	/// to the asynchronous close operation specified in the <seealso cref="Channel"/> class.
	/// If the input side of a socket is shut down by one thread while another
	/// thread is blocked in a read operation on the socket's channel, then the read
	/// operation in the blocked thread will complete without reading any bytes and
	/// will return <tt>-1</tt>.  If the output side of a socket is shut down by one
	/// thread while another thread is blocked in a write operation on the socket's
	/// channel, then the blocked thread will receive an {@link
	/// AsynchronousCloseException}.
	/// 
	/// </para>
	/// <para> Socket options are configured using the {@link #setOption(SocketOption,Object)
	/// setOption} method. Socket channels support the following options:
	/// <blockquote>
	/// <table border summary="Socket options">
	///   <tr>
	///     <th>Option Name</th>
	///     <th>Description</th>
	///   </tr>
	///   <tr>
	///     <td> <seealso cref="java.net.StandardSocketOptions#SO_SNDBUF SO_SNDBUF"/> </td>
	///     <td> The size of the socket send buffer </td>
	///   </tr>
	///   <tr>
	///     <td> <seealso cref="java.net.StandardSocketOptions#SO_RCVBUF SO_RCVBUF"/> </td>
	///     <td> The size of the socket receive buffer </td>
	///   </tr>
	///   <tr>
	///     <td> <seealso cref="java.net.StandardSocketOptions#SO_KEEPALIVE SO_KEEPALIVE"/> </td>
	///     <td> Keep connection alive </td>
	///   </tr>
	///   <tr>
	///     <td> <seealso cref="java.net.StandardSocketOptions#SO_REUSEADDR SO_REUSEADDR"/> </td>
	///     <td> Re-use address </td>
	///   </tr>
	///   <tr>
	///     <td> <seealso cref="java.net.StandardSocketOptions#SO_LINGER SO_LINGER"/> </td>
	///     <td> Linger on close if data is present (when configured in blocking mode
	///          only) </td>
	///   </tr>
	///   <tr>
	///     <td> <seealso cref="java.net.StandardSocketOptions#TCP_NODELAY TCP_NODELAY"/> </td>
	///     <td> Disable the Nagle algorithm </td>
	///   </tr>
	/// </table>
	/// </blockquote>
	/// Additional (implementation specific) options may also be supported.
	/// 
	/// </para>
	/// <para> Socket channels are safe for use by multiple concurrent threads.  They
	/// support concurrent reading and writing, though at most one thread may be
	/// reading and at most one thread may be writing at any given time.  The {@link
	/// #connect connect} and <seealso cref="#finishConnect finishConnect"/> methods are
	/// mutually synchronized against each other, and an attempt to initiate a read
	/// or write operation while an invocation of one of these methods is in
	/// progress will block until that invocation is complete.  </para>
	/// 
	/// @author Mark Reinhold
	/// @author JSR-51 Expert Group
	/// @since 1.4
	/// </summary>

	public abstract class SocketChannel : AbstractSelectableChannel, ByteChannel, ScatteringByteChannel, GatheringByteChannel, NetworkChannel
	{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public abstract java.util.Set<java.net.SocketOption<JavaToDotNetGenericWildcard>> supportedOptions();
		public abstract java.util.Set<SocketOption<?>> SupportedOptions();
		public abstract T GetOption(SocketOption<T> name);

		/// <summary>
		/// Initializes a new instance of this class.
		/// </summary>
		/// <param name="provider">
		///         The provider that created this channel </param>
		protected internal SocketChannel(SelectorProvider provider) : base(provider)
		{
		}

		/// <summary>
		/// Opens a socket channel.
		/// 
		/// <para> The new channel is created by invoking the {@link
		/// java.nio.channels.spi.SelectorProvider#openSocketChannel
		/// openSocketChannel} method of the system-wide default {@link
		/// java.nio.channels.spi.SelectorProvider} object.  </para>
		/// </summary>
		/// <returns>  A new socket channel
		/// </returns>
		/// <exception cref="IOException">
		///          If an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static SocketChannel open() throws java.io.IOException
		public static SocketChannel Open()
		{
			return SelectorProvider.Provider().OpenSocketChannel();
		}

		/// <summary>
		/// Opens a socket channel and connects it to a remote address.
		/// 
		/// <para> This convenience method works as if by invoking the <seealso cref="#open()"/>
		/// method, invoking the <seealso cref="#connect(SocketAddress) connect"/> method upon
		/// the resulting socket channel, passing it <tt>remote</tt>, and then
		/// returning that channel.  </para>
		/// </summary>
		/// <param name="remote">
		///         The remote address to which the new channel is to be connected
		/// </param>
		/// <returns>  A new, and connected, socket channel
		/// </returns>
		/// <exception cref="AsynchronousCloseException">
		///          If another thread closes this channel
		///          while the connect operation is in progress
		/// </exception>
		/// <exception cref="ClosedByInterruptException">
		///          If another thread interrupts the current thread
		///          while the connect operation is in progress, thereby
		///          closing the channel and setting the current thread's
		///          interrupt status
		/// </exception>
		/// <exception cref="UnresolvedAddressException">
		///          If the given remote address is not fully resolved
		/// </exception>
		/// <exception cref="UnsupportedAddressTypeException">
		///          If the type of the given remote address is not supported
		/// </exception>
		/// <exception cref="SecurityException">
		///          If a security manager has been installed
		///          and it does not permit access to the given remote endpoint
		/// </exception>
		/// <exception cref="IOException">
		///          If some other I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static SocketChannel open(java.net.SocketAddress remote) throws java.io.IOException
		public static SocketChannel Open(SocketAddress remote)
		{
			SocketChannel sc = Open();
			try
			{
				sc.Connect(remote);
			}
			catch (Throwable x)
			{
				try
				{
					sc.Close();
				}
				catch (Throwable suppressed)
				{
					x.AddSuppressed(suppressed);
				}
				throw x;
			}
			Debug.Assert(sc.Connected);
			return sc;
		}

		/// <summary>
		/// Returns an operation set identifying this channel's supported
		/// operations.
		/// 
		/// <para> Socket channels support connecting, reading, and writing, so this
		/// method returns <tt>(</tt><seealso cref="SelectionKey#OP_CONNECT"/>
		/// <tt>|</tt>&nbsp;<seealso cref="SelectionKey#OP_READ"/> <tt>|</tt>&nbsp;{@link
		/// SelectionKey#OP_WRITE}<tt>)</tt>.  </para>
		/// </summary>
		/// <returns>  The valid-operation set </returns>
		public sealed override int ValidOps()
		{
			return (SelectionKey.OP_READ | SelectionKey.OP_WRITE | SelectionKey.OP_CONNECT);
		}


		// -- Socket-specific operations --

		/// <exception cref="ConnectionPendingException">
		///          If a non-blocking connect operation is already in progress on
		///          this channel </exception>
		/// <exception cref="AlreadyBoundException">               {@inheritDoc} </exception>
		/// <exception cref="UnsupportedAddressTypeException">     {@inheritDoc} </exception>
		/// <exception cref="ClosedChannelException">              {@inheritDoc} </exception>
		/// <exception cref="IOException">                         {@inheritDoc} </exception>
		/// <exception cref="SecurityException">
		///          If a security manager has been installed and its
		///          <seealso cref="SecurityManager#checkListen checkListen"/> method denies
		///          the operation
		/// 
		/// @since 1.7 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public abstract SocketChannel bind(java.net.SocketAddress local) throws java.io.IOException;
		public override abstract SocketChannel Bind(SocketAddress local);

		/// <exception cref="UnsupportedOperationException">           {@inheritDoc} </exception>
		/// <exception cref="IllegalArgumentException">                {@inheritDoc} </exception>
		/// <exception cref="ClosedChannelException">                  {@inheritDoc} </exception>
		/// <exception cref="IOException">                             {@inheritDoc}
		/// 
		/// @since 1.7 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public abstract <T> SocketChannel setOption(java.net.SocketOption<T> name, T value) throws java.io.IOException;
		public override abstract SocketChannel setOption<T>(SocketOption<T> name, T value);

		/// <summary>
		/// Shutdown the connection for reading without closing the channel.
		/// 
		/// <para> Once shutdown for reading then further reads on the channel will
		/// return {@code -1}, the end-of-stream indication. If the input side of the
		/// connection is already shutdown then invoking this method has no effect.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  The channel
		/// </returns>
		/// <exception cref="NotYetConnectedException">
		///          If this channel is not yet connected </exception>
		/// <exception cref="ClosedChannelException">
		///          If this channel is closed </exception>
		/// <exception cref="IOException">
		///          If some other I/O error occurs
		/// 
		/// @since 1.7 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract SocketChannel shutdownInput() throws java.io.IOException;
		public abstract SocketChannel ShutdownInput();

		/// <summary>
		/// Shutdown the connection for writing without closing the channel.
		/// 
		/// <para> Once shutdown for writing then further attempts to write to the
		/// channel will throw <seealso cref="ClosedChannelException"/>. If the output side of
		/// the connection is already shutdown then invoking this method has no
		/// effect.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  The channel
		/// </returns>
		/// <exception cref="NotYetConnectedException">
		///          If this channel is not yet connected </exception>
		/// <exception cref="ClosedChannelException">
		///          If this channel is closed </exception>
		/// <exception cref="IOException">
		///          If some other I/O error occurs
		/// 
		/// @since 1.7 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract SocketChannel shutdownOutput() throws java.io.IOException;
		public abstract SocketChannel ShutdownOutput();

		/// <summary>
		/// Retrieves a socket associated with this channel.
		/// 
		/// <para> The returned object will not declare any public methods that are not
		/// declared in the <seealso cref="java.net.Socket"/> class.  </para>
		/// </summary>
		/// <returns>  A socket associated with this channel </returns>
		public abstract Socket Socket();

		/// <summary>
		/// Tells whether or not this channel's network socket is connected.
		/// </summary>
		/// <returns>  <tt>true</tt> if, and only if, this channel's network socket
		///          is <seealso cref="#isOpen open"/> and connected </returns>
		public abstract bool Connected {get;}

		/// <summary>
		/// Tells whether or not a connection operation is in progress on this
		/// channel.
		/// </summary>
		/// <returns>  <tt>true</tt> if, and only if, a connection operation has been
		///          initiated on this channel but not yet completed by invoking the
		///          <seealso cref="#finishConnect finishConnect"/> method </returns>
		public abstract bool ConnectionPending {get;}

		/// <summary>
		/// Connects this channel's socket.
		/// 
		/// <para> If this channel is in non-blocking mode then an invocation of this
		/// method initiates a non-blocking connection operation.  If the connection
		/// is established immediately, as can happen with a local connection, then
		/// this method returns <tt>true</tt>.  Otherwise this method returns
		/// <tt>false</tt> and the connection operation must later be completed by
		/// invoking the <seealso cref="#finishConnect finishConnect"/> method.
		/// 
		/// </para>
		/// <para> If this channel is in blocking mode then an invocation of this
		/// method will block until the connection is established or an I/O error
		/// occurs.
		/// 
		/// </para>
		/// <para> This method performs exactly the same security checks as the {@link
		/// java.net.Socket} class.  That is, if a security manager has been
		/// installed then this method verifies that its {@link
		/// java.lang.SecurityManager#checkConnect checkConnect} method permits
		/// connecting to the address and port number of the given remote endpoint.
		/// 
		/// </para>
		/// <para> This method may be invoked at any time.  If a read or write
		/// operation upon this channel is invoked while an invocation of this
		/// method is in progress then that operation will first block until this
		/// invocation is complete.  If a connection attempt is initiated but fails,
		/// that is, if an invocation of this method throws a checked exception,
		/// then the channel will be closed.  </para>
		/// </summary>
		/// <param name="remote">
		///         The remote address to which this channel is to be connected
		/// </param>
		/// <returns>  <tt>true</tt> if a connection was established,
		///          <tt>false</tt> if this channel is in non-blocking mode
		///          and the connection operation is in progress
		/// </returns>
		/// <exception cref="AlreadyConnectedException">
		///          If this channel is already connected
		/// </exception>
		/// <exception cref="ConnectionPendingException">
		///          If a non-blocking connection operation is already in progress
		///          on this channel
		/// </exception>
		/// <exception cref="ClosedChannelException">
		///          If this channel is closed
		/// </exception>
		/// <exception cref="AsynchronousCloseException">
		///          If another thread closes this channel
		///          while the connect operation is in progress
		/// </exception>
		/// <exception cref="ClosedByInterruptException">
		///          If another thread interrupts the current thread
		///          while the connect operation is in progress, thereby
		///          closing the channel and setting the current thread's
		///          interrupt status
		/// </exception>
		/// <exception cref="UnresolvedAddressException">
		///          If the given remote address is not fully resolved
		/// </exception>
		/// <exception cref="UnsupportedAddressTypeException">
		///          If the type of the given remote address is not supported
		/// </exception>
		/// <exception cref="SecurityException">
		///          If a security manager has been installed
		///          and it does not permit access to the given remote endpoint
		/// </exception>
		/// <exception cref="IOException">
		///          If some other I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract boolean connect(java.net.SocketAddress remote) throws java.io.IOException;
		public abstract bool Connect(SocketAddress remote);

		/// <summary>
		/// Finishes the process of connecting a socket channel.
		/// 
		/// <para> A non-blocking connection operation is initiated by placing a socket
		/// channel in non-blocking mode and then invoking its {@link #connect
		/// connect} method.  Once the connection is established, or the attempt has
		/// failed, the socket channel will become connectable and this method may
		/// be invoked to complete the connection sequence.  If the connection
		/// operation failed then invoking this method will cause an appropriate
		/// <seealso cref="java.io.IOException"/> to be thrown.
		/// 
		/// </para>
		/// <para> If this channel is already connected then this method will not block
		/// and will immediately return <tt>true</tt>.  If this channel is in
		/// non-blocking mode then this method will return <tt>false</tt> if the
		/// connection process is not yet complete.  If this channel is in blocking
		/// mode then this method will block until the connection either completes
		/// or fails, and will always either return <tt>true</tt> or throw a checked
		/// exception describing the failure.
		/// 
		/// </para>
		/// <para> This method may be invoked at any time.  If a read or write
		/// operation upon this channel is invoked while an invocation of this
		/// method is in progress then that operation will first block until this
		/// invocation is complete.  If a connection attempt fails, that is, if an
		/// invocation of this method throws a checked exception, then the channel
		/// will be closed.  </para>
		/// </summary>
		/// <returns>  <tt>true</tt> if, and only if, this channel's socket is now
		///          connected
		/// </returns>
		/// <exception cref="NoConnectionPendingException">
		///          If this channel is not connected and a connection operation
		///          has not been initiated
		/// </exception>
		/// <exception cref="ClosedChannelException">
		///          If this channel is closed
		/// </exception>
		/// <exception cref="AsynchronousCloseException">
		///          If another thread closes this channel
		///          while the connect operation is in progress
		/// </exception>
		/// <exception cref="ClosedByInterruptException">
		///          If another thread interrupts the current thread
		///          while the connect operation is in progress, thereby
		///          closing the channel and setting the current thread's
		///          interrupt status
		/// </exception>
		/// <exception cref="IOException">
		///          If some other I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract boolean finishConnect() throws java.io.IOException;
		public abstract bool FinishConnect();

		/// <summary>
		/// Returns the remote address to which this channel's socket is connected.
		/// 
		/// <para> Where the channel is bound and connected to an Internet Protocol
		/// socket address then the return value from this method is of type {@link
		/// java.net.InetSocketAddress}.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  The remote address; {@code null} if the channel's socket is not
		///          connected
		/// </returns>
		/// <exception cref="ClosedChannelException">
		///          If the channel is closed </exception>
		/// <exception cref="IOException">
		///          If an I/O error occurs
		/// 
		/// @since 1.7 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract java.net.SocketAddress getRemoteAddress() throws java.io.IOException;
		public abstract SocketAddress RemoteAddress {get;}

		// -- ByteChannel operations --

		/// <exception cref="NotYetConnectedException">
		///          If this channel is not yet connected </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract int read(java.nio.ByteBuffer dst) throws java.io.IOException;
		public abstract int Read(ByteBuffer dst);

		/// <exception cref="NotYetConnectedException">
		///          If this channel is not yet connected </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract long read(java.nio.ByteBuffer[] dsts, int offset, int length) throws java.io.IOException;
		public abstract long Read(ByteBuffer[] dsts, int offset, int length);

		/// <exception cref="NotYetConnectedException">
		///          If this channel is not yet connected </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final long read(java.nio.ByteBuffer[] dsts) throws java.io.IOException
		public long Read(ByteBuffer[] dsts)
		{
			return Read(dsts, 0, dsts.Length);
		}

		/// <exception cref="NotYetConnectedException">
		///          If this channel is not yet connected </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract int write(java.nio.ByteBuffer src) throws java.io.IOException;
		public abstract int Write(ByteBuffer src);

		/// <exception cref="NotYetConnectedException">
		///          If this channel is not yet connected </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract long write(java.nio.ByteBuffer[] srcs, int offset, int length) throws java.io.IOException;
		public abstract long Write(ByteBuffer[] srcs, int offset, int length);

		/// <exception cref="NotYetConnectedException">
		///          If this channel is not yet connected </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final long write(java.nio.ByteBuffer[] srcs) throws java.io.IOException
		public long Write(ByteBuffer[] srcs)
		{
			return Write(srcs, 0, srcs.Length);
		}

		/// <summary>
		/// {@inheritDoc}
		/// <para>
		/// If there is a security manager set, its {@code checkConnect} method is
		/// called with the local address and {@code -1} as its arguments to see
		/// if the operation is allowed. If the operation is not allowed,
		/// a {@code SocketAddress} representing the
		/// <seealso cref="java.net.InetAddress#getLoopbackAddress loopback"/> address and the
		/// local port of the channel's socket is returned.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  The {@code SocketAddress} that the socket is bound to, or the
		///          {@code SocketAddress} representing the loopback address if
		///          denied by the security manager, or {@code null} if the
		///          channel's socket is not bound
		/// </returns>
		/// <exception cref="ClosedChannelException">     {@inheritDoc} </exception>
		/// <exception cref="IOException">                {@inheritDoc} </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public abstract java.net.SocketAddress getLocalAddress() throws java.io.IOException;
		public override abstract SocketAddress LocalAddress {get;}

	}

}