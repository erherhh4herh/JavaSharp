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
	/// An asynchronous channel for stream-oriented listening sockets.
	/// 
	/// <para> An asynchronous server-socket channel is created by invoking the
	/// <seealso cref="#open open"/> method of this class.
	/// A newly-created asynchronous server-socket channel is open but not yet bound.
	/// It can be bound to a local address and configured to listen for connections
	/// by invoking the <seealso cref="#bind(SocketAddress,int) bind"/> method. Once bound,
	/// the <seealso cref="#accept(Object,CompletionHandler) accept"/> method
	/// is used to initiate the accepting of connections to the channel's socket.
	/// An attempt to invoke the <tt>accept</tt> method on an unbound channel will
	/// cause a <seealso cref="NotYetBoundException"/> to be thrown.
	/// 
	/// </para>
	/// <para> Channels of this type are safe for use by multiple concurrent threads
	/// though at most one accept operation can be outstanding at any time.
	/// If a thread initiates an accept operation before a previous accept operation
	/// has completed then an <seealso cref="AcceptPendingException"/> will be thrown.
	/// 
	/// </para>
	/// <para> Socket options are configured using the {@link #setOption(SocketOption,Object)
	/// setOption} method. Channels of this type support the following options:
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
	/// <para> <b>Usage Example:</b>
	/// <pre>
	///  final AsynchronousServerSocketChannel listener =
	///      AsynchronousServerSocketChannel.open().bind(new InetSocketAddress(5000));
	/// 
	///  listener.accept(null, new CompletionHandler&lt;AsynchronousSocketChannel,Void&gt;() {
	///      public void completed(AsynchronousSocketChannel ch, Void att) {
	///          // accept the next connection
	///          listener.accept(null, this);
	/// 
	///          // handle this connection
	///          handle(ch);
	///      }
	///      public void failed(Throwable exc, Void att) {
	///          ...
	///      }
	///  });
	/// </pre>
	/// 
	/// @since 1.7
	/// </para>
	/// </summary>

	public abstract class AsynchronousServerSocketChannel : AsynchronousChannel, NetworkChannel
	{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public abstract java.util.Set<java.net.SocketOption<JavaToDotNetGenericWildcard>> supportedOptions();
		public abstract java.util.Set<SocketOption<?>> SupportedOptions();
		public abstract T GetOption(SocketOption<T> name);
		public abstract bool Open {get;}
		public abstract void Close();
		private readonly AsynchronousChannelProvider Provider_Renamed;

		/// <summary>
		/// Initializes a new instance of this class.
		/// </summary>
		/// <param name="provider">
		///         The provider that created this channel </param>
		protected internal AsynchronousServerSocketChannel(AsynchronousChannelProvider provider)
		{
			this.Provider_Renamed = provider;
		}

		/// <summary>
		/// Returns the provider that created this channel.
		/// </summary>
		/// <returns>  The provider that created this channel </returns>
		public AsynchronousChannelProvider Provider()
		{
			return Provider_Renamed;
		}

		/// <summary>
		/// Opens an asynchronous server-socket channel.
		/// 
		/// <para> The new channel is created by invoking the {@link
		/// java.nio.channels.spi.AsynchronousChannelProvider#openAsynchronousServerSocketChannel
		/// openAsynchronousServerSocketChannel} method on the {@link
		/// java.nio.channels.spi.AsynchronousChannelProvider} object that created
		/// the given group. If the group parameter is <tt>null</tt> then the
		/// resulting channel is created by the system-wide default provider, and
		/// bound to the <em>default group</em>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="group">
		///          The group to which the newly constructed channel should be bound,
		///          or <tt>null</tt> for the default group
		/// </param>
		/// <returns>  A new asynchronous server socket channel
		/// </returns>
		/// <exception cref="ShutdownChannelGroupException">
		///          If the channel group is shutdown </exception>
		/// <exception cref="IOException">
		///          If an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static AsynchronousServerSocketChannel open(AsynchronousChannelGroup group) throws java.io.IOException
		public static AsynchronousServerSocketChannel Open(AsynchronousChannelGroup group)
		{
			AsynchronousChannelProvider provider = (group == null) ? AsynchronousChannelProvider.Provider() : group.Provider();
			return provider.OpenAsynchronousServerSocketChannel(group);
		}

		/// <summary>
		/// Opens an asynchronous server-socket channel.
		/// 
		/// <para> This method returns an asynchronous server socket channel that is
		/// bound to the <em>default group</em>. This method is equivalent to evaluating
		/// the expression:
		/// <blockquote><pre>
		/// open((AsynchronousChannelGroup)null);
		/// </pre></blockquote>
		/// 
		/// </para>
		/// </summary>
		/// <returns>  A new asynchronous server socket channel
		/// </returns>
		/// <exception cref="IOException">
		///          If an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static AsynchronousServerSocketChannel open() throws java.io.IOException
		public static AsynchronousServerSocketChannel Open()
		{
			return Open(null);
		}

		/// <summary>
		/// Binds the channel's socket to a local address and configures the socket to
		/// listen for connections.
		/// 
		/// <para> An invocation of this method is equivalent to the following:
		/// <blockquote><pre>
		/// bind(local, 0);
		/// </pre></blockquote>
		/// 
		/// </para>
		/// </summary>
		/// <param name="local">
		///          The local address to bind the socket, or <tt>null</tt> to bind
		///          to an automatically assigned socket address
		/// </param>
		/// <returns>  This channel
		/// </returns>
		/// <exception cref="AlreadyBoundException">               {@inheritDoc} </exception>
		/// <exception cref="UnsupportedAddressTypeException">     {@inheritDoc} </exception>
		/// <exception cref="SecurityException">                   {@inheritDoc} </exception>
		/// <exception cref="ClosedChannelException">              {@inheritDoc} </exception>
		/// <exception cref="IOException">                         {@inheritDoc} </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final AsynchronousServerSocketChannel bind(java.net.SocketAddress local) throws java.io.IOException
		public AsynchronousServerSocketChannel Bind(SocketAddress local)
		{
			return Bind(local, 0);
		}

		/// <summary>
		/// Binds the channel's socket to a local address and configures the socket to
		/// listen for connections.
		/// 
		/// <para> This method is used to establish an association between the socket and
		/// a local address. Once an association is established then the socket remains
		/// bound until the associated channel is closed.
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
		///          The local address to bind the socket, or {@code null} to bind
		///          to an automatically assigned socket address </param>
		/// <param name="backlog">
		///          The maximum number of pending connections
		/// </param>
		/// <returns>  This channel
		/// </returns>
		/// <exception cref="AlreadyBoundException">
		///          If the socket is already bound </exception>
		/// <exception cref="UnsupportedAddressTypeException">
		///          If the type of the given address is not supported </exception>
		/// <exception cref="SecurityException">
		///          If a security manager has been installed and its {@link
		///          SecurityManager#checkListen checkListen} method denies the operation </exception>
		/// <exception cref="ClosedChannelException">
		///          If the channel is closed </exception>
		/// <exception cref="IOException">
		///          If some other I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract AsynchronousServerSocketChannel bind(java.net.SocketAddress local, int backlog) throws java.io.IOException;
		public abstract AsynchronousServerSocketChannel Bind(SocketAddress local, int backlog);

		/// <exception cref="IllegalArgumentException">                {@inheritDoc} </exception>
		/// <exception cref="ClosedChannelException">                  {@inheritDoc} </exception>
		/// <exception cref="IOException">                             {@inheritDoc} </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract <T> AsynchronousServerSocketChannel setOption(java.net.SocketOption<T> name, T value) throws java.io.IOException;
		public abstract AsynchronousServerSocketChannel setOption<T>(SocketOption<T> name, T value);

		/// <summary>
		/// Accepts a connection.
		/// 
		/// <para> This method initiates an asynchronous operation to accept a
		/// connection made to this channel's socket. The {@code handler} parameter is
		/// a completion handler that is invoked when a connection is accepted (or
		/// the operation fails). The result passed to the completion handler is
		/// the <seealso cref="AsynchronousSocketChannel"/> to the new connection.
		/// 
		/// </para>
		/// <para> When a new connection is accepted then the resulting {@code
		/// AsynchronousSocketChannel} will be bound to the same {@link
		/// AsynchronousChannelGroup} as this channel. If the group is {@link
		/// AsynchronousChannelGroup#isShutdown shutdown} and a connection is accepted,
		/// then the connection is closed, and the operation completes with an {@code
		/// IOException} and cause <seealso cref="ShutdownChannelGroupException"/>.
		/// 
		/// </para>
		/// <para> To allow for concurrent handling of new connections, the completion
		/// handler is not invoked directly by the initiating thread when a new
		/// connection is accepted immediately (see <a
		/// href="AsynchronousChannelGroup.html#threading">Threading</a>).
		/// 
		/// </para>
		/// <para> If a security manager has been installed then it verifies that the
		/// address and port number of the connection's remote endpoint are permitted
		/// by the security manager's <seealso cref="SecurityManager#checkAccept checkAccept"/>
		/// method. The permission check is performed with privileges that are restricted
		/// by the calling context of this method. If the permission check fails then
		/// the connection is closed and the operation completes with a {@link
		/// SecurityException}.
		/// 
		/// </para>
		/// </summary>
		/// @param   <A>
		///          The type of the attachment </param>
		/// <param name="attachment">
		///          The object to attach to the I/O operation; can be {@code null} </param>
		/// <param name="handler">
		///          The handler for consuming the result
		/// </param>
		/// <exception cref="AcceptPendingException">
		///          If an accept operation is already in progress on this channel </exception>
		/// <exception cref="NotYetBoundException">
		///          If this channel's socket has not yet been bound </exception>
		/// <exception cref="ShutdownChannelGroupException">
		///          If the channel group has terminated </exception>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public abstract <A> void accept(A attachment, CompletionHandler<AsynchronousSocketChannel,? base A> handler);
		public abstract void accept<A, T1>(A attachment, CompletionHandler<T1> handler);

		/// <summary>
		/// Accepts a connection.
		/// 
		/// <para> This method initiates an asynchronous operation to accept a
		/// connection made to this channel's socket. The method behaves in exactly
		/// the same manner as the <seealso cref="#accept(Object, CompletionHandler)"/> method
		/// except that instead of specifying a completion handler, this method
		/// returns a {@code Future} representing the pending result. The {@code
		/// Future}'s <seealso cref="Future#get() get"/> method returns the {@link
		/// AsynchronousSocketChannel} to the new connection on successful completion.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  a {@code Future} object representing the pending result
		/// </returns>
		/// <exception cref="AcceptPendingException">
		///          If an accept operation is already in progress on this channel </exception>
		/// <exception cref="NotYetBoundException">
		///          If this channel's socket has not yet been bound </exception>
		public abstract Future<AsynchronousSocketChannel> Accept();

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