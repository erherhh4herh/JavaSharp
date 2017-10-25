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
	/// A selectable channel for stream-oriented listening sockets.
	/// 
	/// <para> A server-socket channel is created by invoking the <seealso cref="#open() open"/>
	/// method of this class.  It is not possible to create a channel for an arbitrary,
	/// pre-existing <seealso cref="ServerSocket"/>. A newly-created server-socket channel is
	/// open but not yet bound.  An attempt to invoke the <seealso cref="#accept() accept"/>
	/// method of an unbound server-socket channel will cause a <seealso cref="NotYetBoundException"/>
	/// to be thrown. A server-socket channel can be bound by invoking one of the
	/// <seealso cref="#bind(java.net.SocketAddress,int) bind"/> methods defined by this class.
	/// 
	/// </para>
	/// <para> Socket options are configured using the {@link #setOption(SocketOption,Object)
	/// setOption} method. Server-socket channels support the following options:
	/// <blockquote>
	/// <table border summary="Socket options">
	///   <tr>
	///     <th>Option Name</th>
	///     <th>Description</th>
	///   </tr>
	///   <tr>
	///     <td> <seealso cref="java.net.StandardSocketOptions#SO_RCVBUF SO_RCVBUF"/> </td>
	///     <td> The size of the socket receive buffer </td>
	///   </tr>
	///   <tr>
	///     <td> <seealso cref="java.net.StandardSocketOptions#SO_REUSEADDR SO_REUSEADDR"/> </td>
	///     <td> Re-use address </td>
	///   </tr>
	/// </table>
	/// </blockquote>
	/// Additional (implementation specific) options may also be supported.
	/// 
	/// </para>
	/// <para> Server-socket channels are safe for use by multiple concurrent threads.
	/// </para>
	/// 
	/// @author Mark Reinhold
	/// @author JSR-51 Expert Group
	/// @since 1.4
	/// </summary>

	public abstract class ServerSocketChannel : AbstractSelectableChannel, NetworkChannel
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
		protected internal ServerSocketChannel(SelectorProvider provider) : base(provider)
		{
		}

		/// <summary>
		/// Opens a server-socket channel.
		/// 
		/// <para> The new channel is created by invoking the {@link
		/// java.nio.channels.spi.SelectorProvider#openServerSocketChannel
		/// openServerSocketChannel} method of the system-wide default {@link
		/// java.nio.channels.spi.SelectorProvider} object.
		/// 
		/// </para>
		/// <para> The new channel's socket is initially unbound; it must be bound to a
		/// specific address via one of its socket's {@link
		/// java.net.ServerSocket#bind(SocketAddress) bind} methods before
		/// connections can be accepted.  </para>
		/// </summary>
		/// <returns>  A new socket channel
		/// </returns>
		/// <exception cref="IOException">
		///          If an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static ServerSocketChannel open() throws java.io.IOException
		public static ServerSocketChannel Open()
		{
			return SelectorProvider.Provider().OpenServerSocketChannel();
		}

		/// <summary>
		/// Returns an operation set identifying this channel's supported
		/// operations.
		/// 
		/// <para> Server-socket channels only support the accepting of new
		/// connections, so this method returns <seealso cref="SelectionKey#OP_ACCEPT"/>.
		/// </para>
		/// </summary>
		/// <returns>  The valid-operation set </returns>
		public sealed override int ValidOps()
		{
			return SelectionKey.OP_ACCEPT;
		}


		// -- ServerSocket-specific operations --

		/// <summary>
		/// Binds the channel's socket to a local address and configures the socket
		/// to listen for connections.
		/// 
		/// <para> An invocation of this method is equivalent to the following:
		/// <blockquote><pre>
		/// bind(local, 0);
		/// </pre></blockquote>
		/// 
		/// </para>
		/// </summary>
		/// <param name="local">
		///          The local address to bind the socket, or {@code null} to bind
		///          to an automatically assigned socket address
		/// </param>
		/// <returns>  This channel
		/// </returns>
		/// <exception cref="AlreadyBoundException">               {@inheritDoc} </exception>
		/// <exception cref="UnsupportedAddressTypeException">     {@inheritDoc} </exception>
		/// <exception cref="ClosedChannelException">              {@inheritDoc} </exception>
		/// <exception cref="IOException">                         {@inheritDoc} </exception>
		/// <exception cref="SecurityException">
		///          If a security manager has been installed and its {@link
		///          SecurityManager#checkListen checkListen} method denies the
		///          operation
		/// 
		/// @since 1.7 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final ServerSocketChannel bind(java.net.SocketAddress local) throws java.io.IOException
		public ServerSocketChannel Bind(SocketAddress local)
		{
			return Bind(local, 0);
		}

		/// <summary>
		/// Binds the channel's socket to a local address and configures the socket to
		/// listen for connections.
		/// 
		/// <para> This method is used to establish an association between the socket and
		/// a local address. Once an association is established then the socket remains
		/// bound until the channel is closed.
		/// 
		/// </para>
		/// <para> The {@code backlog} parameter is the maximum number of pending
		/// connections on the socket. Its exact semantics are implementation specific.
		/// In particular, an implementation may impose a maximum length or may choose
		/// to ignore the parameter altogther. If the {@code backlog} parameter has
		/// the value {@code 0}, or a negative value, then an implementation specific
		/// default is used.
		/// 
		/// </para>
		/// </summary>
		/// <param name="local">
		///          The address to bind the socket, or {@code null} to bind to an
		///          automatically assigned socket address </param>
		/// <param name="backlog">
		///          The maximum number of pending connections
		/// </param>
		/// <returns>  This channel
		/// </returns>
		/// <exception cref="AlreadyBoundException">
		///          If the socket is already bound </exception>
		/// <exception cref="UnsupportedAddressTypeException">
		///          If the type of the given address is not supported </exception>
		/// <exception cref="ClosedChannelException">
		///          If this channel is closed </exception>
		/// <exception cref="IOException">
		///          If some other I/O error occurs </exception>
		/// <exception cref="SecurityException">
		///          If a security manager has been installed and its {@link
		///          SecurityManager#checkListen checkListen} method denies the
		///          operation
		/// 
		/// @since 1.7 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract ServerSocketChannel bind(java.net.SocketAddress local, int backlog) throws java.io.IOException;
		public abstract ServerSocketChannel Bind(SocketAddress local, int backlog);

		/// <exception cref="UnsupportedOperationException">           {@inheritDoc} </exception>
		/// <exception cref="IllegalArgumentException">                {@inheritDoc} </exception>
		/// <exception cref="ClosedChannelException">                  {@inheritDoc} </exception>
		/// <exception cref="IOException">                             {@inheritDoc}
		/// 
		/// @since 1.7 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract <T> ServerSocketChannel setOption(java.net.SocketOption<T> name, T value) throws java.io.IOException;
		public abstract ServerSocketChannel setOption<T>(SocketOption<T> name, T value);

		/// <summary>
		/// Retrieves a server socket associated with this channel.
		/// 
		/// <para> The returned object will not declare any public methods that are not
		/// declared in the <seealso cref="java.net.ServerSocket"/> class.  </para>
		/// </summary>
		/// <returns>  A server socket associated with this channel </returns>
		public abstract ServerSocket Socket();

		/// <summary>
		/// Accepts a connection made to this channel's socket.
		/// 
		/// <para> If this channel is in non-blocking mode then this method will
		/// immediately return <tt>null</tt> if there are no pending connections.
		/// Otherwise it will block indefinitely until a new connection is available
		/// or an I/O error occurs.
		/// 
		/// </para>
		/// <para> The socket channel returned by this method, if any, will be in
		/// blocking mode regardless of the blocking mode of this channel.
		/// 
		/// </para>
		/// <para> This method performs exactly the same security checks as the {@link
		/// java.net.ServerSocket#accept accept} method of the {@link
		/// java.net.ServerSocket} class.  That is, if a security manager has been
		/// installed then for each new connection this method verifies that the
		/// address and port number of the connection's remote endpoint are
		/// permitted by the security manager's {@link
		/// java.lang.SecurityManager#checkAccept checkAccept} method.  </para>
		/// </summary>
		/// <returns>  The socket channel for the new connection,
		///          or <tt>null</tt> if this channel is in non-blocking mode
		///          and no connection is available to be accepted
		/// </returns>
		/// <exception cref="ClosedChannelException">
		///          If this channel is closed
		/// </exception>
		/// <exception cref="AsynchronousCloseException">
		///          If another thread closes this channel
		///          while the accept operation is in progress
		/// </exception>
		/// <exception cref="ClosedByInterruptException">
		///          If another thread interrupts the current thread
		///          while the accept operation is in progress, thereby
		///          closing the channel and setting the current thread's
		///          interrupt status
		/// </exception>
		/// <exception cref="NotYetBoundException">
		///          If this channel's socket has not yet been bound
		/// </exception>
		/// <exception cref="SecurityException">
		///          If a security manager has been installed
		///          and it does not permit access to the remote endpoint
		///          of the new connection
		/// </exception>
		/// <exception cref="IOException">
		///          If some other I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract SocketChannel accept() throws java.io.IOException;
		public abstract SocketChannel Accept();

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