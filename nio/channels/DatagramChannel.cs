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
	/// A selectable channel for datagram-oriented sockets.
	/// 
	/// <para> A datagram channel is created by invoking one of the <seealso cref="#open open"/> methods
	/// of this class. It is not possible to create a channel for an arbitrary,
	/// pre-existing datagram socket. A newly-created datagram channel is open but not
	/// connected. A datagram channel need not be connected in order for the {@link #send
	/// send} and <seealso cref="#receive receive"/> methods to be used.  A datagram channel may be
	/// connected, by invoking its <seealso cref="#connect connect"/> method, in order to
	/// avoid the overhead of the security checks are otherwise performed as part of
	/// every send and receive operation.  A datagram channel must be connected in
	/// order to use the <seealso cref="#read(java.nio.ByteBuffer) read"/> and {@link
	/// #write(java.nio.ByteBuffer) write} methods, since those methods do not
	/// accept or return socket addresses.
	/// 
	/// </para>
	/// <para> Once connected, a datagram channel remains connected until it is
	/// disconnected or closed.  Whether or not a datagram channel is connected may
	/// be determined by invoking its <seealso cref="#isConnected isConnected"/> method.
	/// 
	/// </para>
	/// <para> Socket options are configured using the {@link #setOption(SocketOption,Object)
	/// setOption} method. A datagram channel to an Internet Protocol socket supports
	/// the following options:
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
	///     <td> <seealso cref="java.net.StandardSocketOptions#SO_REUSEADDR SO_REUSEADDR"/> </td>
	///     <td> Re-use address </td>
	///   </tr>
	///   <tr>
	///     <td> <seealso cref="java.net.StandardSocketOptions#SO_BROADCAST SO_BROADCAST"/> </td>
	///     <td> Allow transmission of broadcast datagrams </td>
	///   </tr>
	///   <tr>
	///     <td> <seealso cref="java.net.StandardSocketOptions#IP_TOS IP_TOS"/> </td>
	///     <td> The Type of Service (ToS) octet in the Internet Protocol (IP) header </td>
	///   </tr>
	///   <tr>
	///     <td> <seealso cref="java.net.StandardSocketOptions#IP_MULTICAST_IF IP_MULTICAST_IF"/> </td>
	///     <td> The network interface for Internet Protocol (IP) multicast datagrams </td>
	///   </tr>
	///   <tr>
	///     <td> {@link java.net.StandardSocketOptions#IP_MULTICAST_TTL
	///       IP_MULTICAST_TTL} </td>
	///     <td> The <em>time-to-live</em> for Internet Protocol (IP) multicast
	///       datagrams </td>
	///   </tr>
	///   <tr>
	///     <td> {@link java.net.StandardSocketOptions#IP_MULTICAST_LOOP
	///       IP_MULTICAST_LOOP} </td>
	///     <td> Loopback for Internet Protocol (IP) multicast datagrams </td>
	///   </tr>
	/// </table>
	/// </blockquote>
	/// Additional (implementation specific) options may also be supported.
	/// 
	/// </para>
	/// <para> Datagram channels are safe for use by multiple concurrent threads.  They
	/// support concurrent reading and writing, though at most one thread may be
	/// reading and at most one thread may be writing at any given time.  </para>
	/// 
	/// @author Mark Reinhold
	/// @author JSR-51 Expert Group
	/// @since 1.4
	/// </summary>

	public abstract class DatagramChannel : AbstractSelectableChannel, ByteChannel, ScatteringByteChannel, GatheringByteChannel, MulticastChannel
	{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public abstract java.util.Set<java.net.SocketOption<JavaToDotNetGenericWildcard>> supportedOptions();
		public abstract java.util.Set<SocketOption<?>> SupportedOptions();
		public abstract T GetOption(SocketOption<T> name);
		public abstract MembershipKey Join(java.net.InetAddress group, java.net.NetworkInterface interf, java.net.InetAddress source);
		public abstract MembershipKey Join(java.net.InetAddress group, java.net.NetworkInterface interf);

		/// <summary>
		/// Initializes a new instance of this class.
		/// </summary>
		/// <param name="provider">
		///         The provider that created this channel </param>
		protected internal DatagramChannel(SelectorProvider provider) : base(provider)
		{
		}

		/// <summary>
		/// Opens a datagram channel.
		/// 
		/// <para> The new channel is created by invoking the {@link
		/// java.nio.channels.spi.SelectorProvider#openDatagramChannel()
		/// openDatagramChannel} method of the system-wide default {@link
		/// java.nio.channels.spi.SelectorProvider} object.  The channel will not be
		/// connected.
		/// 
		/// </para>
		/// <para> The <seealso cref="ProtocolFamily ProtocolFamily"/> of the channel's socket
		/// is platform (and possibly configuration) dependent and therefore unspecified.
		/// The <seealso cref="#open(ProtocolFamily) open"/> allows the protocol family to be
		/// selected when opening a datagram channel, and should be used to open
		/// datagram channels that are intended for Internet Protocol multicasting.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  A new datagram channel
		/// </returns>
		/// <exception cref="IOException">
		///          If an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static DatagramChannel open() throws java.io.IOException
		public static DatagramChannel Open()
		{
			return SelectorProvider.Provider().OpenDatagramChannel();
		}

		/// <summary>
		/// Opens a datagram channel.
		/// 
		/// <para> The {@code family} parameter is used to specify the {@link
		/// ProtocolFamily}. If the datagram channel is to be used for IP multicasting
		/// then this should correspond to the address type of the multicast groups
		/// that this channel will join.
		/// 
		/// </para>
		/// <para> The new channel is created by invoking the {@link
		/// java.nio.channels.spi.SelectorProvider#openDatagramChannel(ProtocolFamily)
		/// openDatagramChannel} method of the system-wide default {@link
		/// java.nio.channels.spi.SelectorProvider} object.  The channel will not be
		/// connected.
		/// 
		/// </para>
		/// </summary>
		/// <param name="family">
		///          The protocol family
		/// </param>
		/// <returns>  A new datagram channel
		/// </returns>
		/// <exception cref="UnsupportedOperationException">
		///          If the specified protocol family is not supported. For example,
		///          suppose the parameter is specified as {@link
		///          java.net.StandardProtocolFamily#INET6 StandardProtocolFamily.INET6}
		///          but IPv6 is not enabled on the platform. </exception>
		/// <exception cref="IOException">
		///          If an I/O error occurs
		/// 
		/// @since   1.7 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static DatagramChannel open(java.net.ProtocolFamily family) throws java.io.IOException
		public static DatagramChannel Open(ProtocolFamily family)
		{
			return SelectorProvider.Provider().OpenDatagramChannel(family);
		}

		/// <summary>
		/// Returns an operation set identifying this channel's supported
		/// operations.
		/// 
		/// <para> Datagram channels support reading and writing, so this method
		/// returns <tt>(</tt><seealso cref="SelectionKey#OP_READ"/> <tt>|</tt>&nbsp;{@link
		/// SelectionKey#OP_WRITE}<tt>)</tt>.  </para>
		/// </summary>
		/// <returns>  The valid-operation set </returns>
		public sealed override int ValidOps()
		{
			return (SelectionKey.OP_READ | SelectionKey.OP_WRITE);
		}


		// -- Socket-specific operations --

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
//ORIGINAL LINE: public abstract DatagramChannel bind(java.net.SocketAddress local) throws java.io.IOException;
		public abstract DatagramChannel Bind(SocketAddress local);

		/// <exception cref="UnsupportedOperationException">           {@inheritDoc} </exception>
		/// <exception cref="IllegalArgumentException">                {@inheritDoc} </exception>
		/// <exception cref="ClosedChannelException">                  {@inheritDoc} </exception>
		/// <exception cref="IOException">                             {@inheritDoc}
		/// 
		/// @since 1.7 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract <T> DatagramChannel setOption(java.net.SocketOption<T> name, T value) throws java.io.IOException;
		public abstract DatagramChannel setOption<T>(SocketOption<T> name, T value);

		/// <summary>
		/// Retrieves a datagram socket associated with this channel.
		/// 
		/// <para> The returned object will not declare any public methods that are not
		/// declared in the <seealso cref="java.net.DatagramSocket"/> class.  </para>
		/// </summary>
		/// <returns>  A datagram socket associated with this channel </returns>
		public abstract DatagramSocket Socket();

		/// <summary>
		/// Tells whether or not this channel's socket is connected.
		/// </summary>
		/// <returns>  {@code true} if, and only if, this channel's socket
		///          is <seealso cref="#isOpen open"/> and connected </returns>
		public abstract bool Connected {get;}

		/// <summary>
		/// Connects this channel's socket.
		/// 
		/// <para> The channel's socket is configured so that it only receives
		/// datagrams from, and sends datagrams to, the given remote <i>peer</i>
		/// address.  Once connected, datagrams may not be received from or sent to
		/// any other address.  A datagram socket remains connected until it is
		/// explicitly disconnected or until it is closed.
		/// 
		/// </para>
		/// <para> This method performs exactly the same security checks as the {@link
		/// java.net.DatagramSocket#connect connect} method of the {@link
		/// java.net.DatagramSocket} class.  That is, if a security manager has been
		/// installed then this method verifies that its {@link
		/// java.lang.SecurityManager#checkAccept checkAccept} and {@link
		/// java.lang.SecurityManager#checkConnect checkConnect} methods permit
		/// datagrams to be received from and sent to, respectively, the given
		/// remote address.
		/// 
		/// </para>
		/// <para> This method may be invoked at any time.  It will not have any effect
		/// on read or write operations that are already in progress at the moment
		/// that it is invoked. If this channel's socket is not bound then this method
		/// will first cause the socket to be bound to an address that is assigned
		/// automatically, as if invoking the <seealso cref="#bind bind"/> method with a
		/// parameter of {@code null}. </para>
		/// </summary>
		/// <param name="remote">
		///         The remote address to which this channel is to be connected
		/// </param>
		/// <returns>  This datagram channel
		/// </returns>
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
		/// <exception cref="SecurityException">
		///          If a security manager has been installed
		///          and it does not permit access to the given remote address
		/// </exception>
		/// <exception cref="IOException">
		///          If some other I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract DatagramChannel connect(java.net.SocketAddress remote) throws java.io.IOException;
		public abstract DatagramChannel Connect(SocketAddress remote);

		/// <summary>
		/// Disconnects this channel's socket.
		/// 
		/// <para> The channel's socket is configured so that it can receive datagrams
		/// from, and sends datagrams to, any remote address so long as the security
		/// manager, if installed, permits it.
		/// 
		/// </para>
		/// <para> This method may be invoked at any time.  It will not have any effect
		/// on read or write operations that are already in progress at the moment
		/// that it is invoked.
		/// 
		/// </para>
		/// <para> If this channel's socket is not connected, or if the channel is
		/// closed, then invoking this method has no effect.  </para>
		/// </summary>
		/// <returns>  This datagram channel
		/// </returns>
		/// <exception cref="IOException">
		///          If some other I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract DatagramChannel disconnect() throws java.io.IOException;
		public abstract DatagramChannel Disconnect();

		/// <summary>
		/// Returns the remote address to which this channel's socket is connected.
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

		/// <summary>
		/// Receives a datagram via this channel.
		/// 
		/// <para> If a datagram is immediately available, or if this channel is in
		/// blocking mode and one eventually becomes available, then the datagram is
		/// copied into the given byte buffer and its source address is returned.
		/// If this channel is in non-blocking mode and a datagram is not
		/// immediately available then this method immediately returns
		/// <tt>null</tt>.
		/// 
		/// </para>
		/// <para> The datagram is transferred into the given byte buffer starting at
		/// its current position, as if by a regular {@link
		/// ReadableByteChannel#read(java.nio.ByteBuffer) read} operation.  If there
		/// are fewer bytes remaining in the buffer than are required to hold the
		/// datagram then the remainder of the datagram is silently discarded.
		/// 
		/// </para>
		/// <para> This method performs exactly the same security checks as the {@link
		/// java.net.DatagramSocket#receive receive} method of the {@link
		/// java.net.DatagramSocket} class.  That is, if the socket is not connected
		/// to a specific remote address and a security manager has been installed
		/// then for each datagram received this method verifies that the source's
		/// address and port number are permitted by the security manager's {@link
		/// java.lang.SecurityManager#checkAccept checkAccept} method.  The overhead
		/// of this security check can be avoided by first connecting the socket via
		/// the <seealso cref="#connect connect"/> method.
		/// 
		/// </para>
		/// <para> This method may be invoked at any time.  If another thread has
		/// already initiated a read operation upon this channel, however, then an
		/// invocation of this method will block until the first operation is
		/// complete. If this channel's socket is not bound then this method will
		/// first cause the socket to be bound to an address that is assigned
		/// automatically, as if invoking the <seealso cref="#bind bind"/> method with a
		/// parameter of {@code null}. </para>
		/// </summary>
		/// <param name="dst">
		///         The buffer into which the datagram is to be transferred
		/// </param>
		/// <returns>  The datagram's source address,
		///          or <tt>null</tt> if this channel is in non-blocking mode
		///          and no datagram was immediately available
		/// </returns>
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
		/// <exception cref="SecurityException">
		///          If a security manager has been installed
		///          and it does not permit datagrams to be accepted
		///          from the datagram's sender
		/// </exception>
		/// <exception cref="IOException">
		///          If some other I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract java.net.SocketAddress receive(java.nio.ByteBuffer dst) throws java.io.IOException;
		public abstract SocketAddress Receive(ByteBuffer dst);

		/// <summary>
		/// Sends a datagram via this channel.
		/// 
		/// <para> If this channel is in non-blocking mode and there is sufficient room
		/// in the underlying output buffer, or if this channel is in blocking mode
		/// and sufficient room becomes available, then the remaining bytes in the
		/// given buffer are transmitted as a single datagram to the given target
		/// address.
		/// 
		/// </para>
		/// <para> The datagram is transferred from the byte buffer as if by a regular
		/// <seealso cref="WritableByteChannel#write(java.nio.ByteBuffer) write"/> operation.
		/// 
		/// </para>
		/// <para> This method performs exactly the same security checks as the {@link
		/// java.net.DatagramSocket#send send} method of the {@link
		/// java.net.DatagramSocket} class.  That is, if the socket is not connected
		/// to a specific remote address and a security manager has been installed
		/// then for each datagram sent this method verifies that the target address
		/// and port number are permitted by the security manager's {@link
		/// java.lang.SecurityManager#checkConnect checkConnect} method.  The
		/// overhead of this security check can be avoided by first connecting the
		/// socket via the <seealso cref="#connect connect"/> method.
		/// 
		/// </para>
		/// <para> This method may be invoked at any time.  If another thread has
		/// already initiated a write operation upon this channel, however, then an
		/// invocation of this method will block until the first operation is
		/// complete. If this channel's socket is not bound then this method will
		/// first cause the socket to be bound to an address that is assigned
		/// automatically, as if by invoking the <seealso cref="#bind bind"/> method with a
		/// parameter of {@code null}. </para>
		/// </summary>
		/// <param name="src">
		///         The buffer containing the datagram to be sent
		/// </param>
		/// <param name="target">
		///         The address to which the datagram is to be sent
		/// </param>
		/// <returns>   The number of bytes sent, which will be either the number
		///           of bytes that were remaining in the source buffer when this
		///           method was invoked or, if this channel is non-blocking, may be
		///           zero if there was insufficient room for the datagram in the
		///           underlying output buffer
		/// </returns>
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
		/// <exception cref="SecurityException">
		///          If a security manager has been installed
		///          and it does not permit datagrams to be sent
		///          to the given address
		/// </exception>
		/// <exception cref="IOException">
		///          If some other I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract int send(java.nio.ByteBuffer src, java.net.SocketAddress target) throws java.io.IOException;
		public abstract int Send(ByteBuffer src, SocketAddress target);


		// -- ByteChannel operations --

		/// <summary>
		/// Reads a datagram from this channel.
		/// 
		/// <para> This method may only be invoked if this channel's socket is
		/// connected, and it only accepts datagrams from the socket's peer.  If
		/// there are more bytes in the datagram than remain in the given buffer
		/// then the remainder of the datagram is silently discarded.  Otherwise
		/// this method behaves exactly as specified in the {@link
		/// ReadableByteChannel} interface.  </para>
		/// </summary>
		/// <exception cref="NotYetConnectedException">
		///          If this channel's socket is not connected </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract int read(java.nio.ByteBuffer dst) throws java.io.IOException;
		public abstract int Read(ByteBuffer dst);

		/// <summary>
		/// Reads a datagram from this channel.
		/// 
		/// <para> This method may only be invoked if this channel's socket is
		/// connected, and it only accepts datagrams from the socket's peer.  If
		/// there are more bytes in the datagram than remain in the given buffers
		/// then the remainder of the datagram is silently discarded.  Otherwise
		/// this method behaves exactly as specified in the {@link
		/// ScatteringByteChannel} interface.  </para>
		/// </summary>
		/// <exception cref="NotYetConnectedException">
		///          If this channel's socket is not connected </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract long read(java.nio.ByteBuffer[] dsts, int offset, int length) throws java.io.IOException;
		public abstract long Read(ByteBuffer[] dsts, int offset, int length);

		/// <summary>
		/// Reads a datagram from this channel.
		/// 
		/// <para> This method may only be invoked if this channel's socket is
		/// connected, and it only accepts datagrams from the socket's peer.  If
		/// there are more bytes in the datagram than remain in the given buffers
		/// then the remainder of the datagram is silently discarded.  Otherwise
		/// this method behaves exactly as specified in the {@link
		/// ScatteringByteChannel} interface.  </para>
		/// </summary>
		/// <exception cref="NotYetConnectedException">
		///          If this channel's socket is not connected </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final long read(java.nio.ByteBuffer[] dsts) throws java.io.IOException
		public long Read(ByteBuffer[] dsts)
		{
			return Read(dsts, 0, dsts.Length);
		}

		/// <summary>
		/// Writes a datagram to this channel.
		/// 
		/// <para> This method may only be invoked if this channel's socket is
		/// connected, in which case it sends datagrams directly to the socket's
		/// peer.  Otherwise it behaves exactly as specified in the {@link
		/// WritableByteChannel} interface.  </para>
		/// </summary>
		/// <exception cref="NotYetConnectedException">
		///          If this channel's socket is not connected </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract int write(java.nio.ByteBuffer src) throws java.io.IOException;
		public abstract int Write(ByteBuffer src);

		/// <summary>
		/// Writes a datagram to this channel.
		/// 
		/// <para> This method may only be invoked if this channel's socket is
		/// connected, in which case it sends datagrams directly to the socket's
		/// peer.  Otherwise it behaves exactly as specified in the {@link
		/// GatheringByteChannel} interface.  </para>
		/// </summary>
		/// <returns>   The number of bytes sent, which will be either the number
		///           of bytes that were remaining in the source buffer when this
		///           method was invoked or, if this channel is non-blocking, may be
		///           zero if there was insufficient room for the datagram in the
		///           underlying output buffer
		/// </returns>
		/// <exception cref="NotYetConnectedException">
		///          If this channel's socket is not connected </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract long write(java.nio.ByteBuffer[] srcs, int offset, int length) throws java.io.IOException;
		public abstract long Write(ByteBuffer[] srcs, int offset, int length);

		/// <summary>
		/// Writes a datagram to this channel.
		/// 
		/// <para> This method may only be invoked if this channel's socket is
		/// connected, in which case it sends datagrams directly to the socket's
		/// peer.  Otherwise it behaves exactly as specified in the {@link
		/// GatheringByteChannel} interface.  </para>
		/// </summary>
		/// <returns>   The number of bytes sent, which will be either the number
		///           of bytes that were remaining in the source buffer when this
		///           method was invoked or, if this channel is non-blocking, may be
		///           zero if there was insufficient room for the datagram in the
		///           underlying output buffer
		/// </returns>
		/// <exception cref="NotYetConnectedException">
		///          If this channel's socket is not connected </exception>
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