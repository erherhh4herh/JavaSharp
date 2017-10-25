using System;

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

namespace java.net
{

	/// <summary>
	/// Defines the <em>standard</em> socket options.
	/// 
	/// <para> The <seealso cref="SocketOption#name name"/> of each socket option defined by this
	/// class is its field name.
	/// 
	/// </para>
	/// <para> In this release, the socket options defined here are used by {@link
	/// java.nio.channels.NetworkChannel network} channels in the {@link
	/// java.nio.channels channels} package.
	/// 
	/// @since 1.7
	/// </para>
	/// </summary>

	public sealed class StandardSocketOptions
	{
		private StandardSocketOptions()
		{
		}

		// -- SOL_SOCKET --

		/// <summary>
		/// Allow transmission of broadcast datagrams.
		/// 
		/// <para> The value of this socket option is a {@code Boolean} that represents
		/// whether the option is enabled or disabled. The option is specific to
		/// datagram-oriented sockets sending to <seealso cref="java.net.Inet4Address IPv4"/>
		/// broadcast addresses. When the socket option is enabled then the socket
		/// can be used to send <em>broadcast datagrams</em>.
		/// 
		/// </para>
		/// <para> The initial value of this socket option is {@code FALSE}. The socket
		/// option may be enabled or disabled at any time. Some operating systems may
		/// require that the Java virtual machine be started with implementation
		/// specific privileges to enable this option or send broadcast datagrams.
		/// 
		/// </para>
		/// </summary>
		/// <seealso cref= <a href="http://www.ietf.org/rfc/rfc919.txt">RFC&nbsp;929:
		/// Broadcasting Internet Datagrams</a> </seealso>
		/// <seealso cref= DatagramSocket#setBroadcast </seealso>
		public static readonly SocketOption<Boolean> SO_BROADCAST = new StdSocketOption<Boolean>("SO_BROADCAST", typeof(Boolean));

		/// <summary>
		/// Keep connection alive.
		/// 
		/// <para> The value of this socket option is a {@code Boolean} that represents
		/// whether the option is enabled or disabled. When the {@code SO_KEEPALIVE}
		/// option is enabled the operating system may use a <em>keep-alive</em>
		/// mechanism to periodically probe the other end of a connection when the
		/// connection is otherwise idle. The exact semantics of the keep alive
		/// mechanism is system dependent and therefore unspecified.
		/// 
		/// </para>
		/// <para> The initial value of this socket option is {@code FALSE}. The socket
		/// option may be enabled or disabled at any time.
		/// 
		/// </para>
		/// </summary>
		/// <seealso cref= <a href="http://www.ietf.org/rfc/rfc1122.txt">RFC&nbsp;1122
		/// Requirements for Internet Hosts -- Communication Layers</a> </seealso>
		/// <seealso cref= Socket#setKeepAlive </seealso>
		public static readonly SocketOption<Boolean> SO_KEEPALIVE = new StdSocketOption<Boolean>("SO_KEEPALIVE", typeof(Boolean));

		/// <summary>
		/// The size of the socket send buffer.
		/// 
		/// <para> The value of this socket option is an {@code Integer} that is the
		/// size of the socket send buffer in bytes. The socket send buffer is an
		/// output buffer used by the networking implementation. It may need to be
		/// increased for high-volume connections. The value of the socket option is
		/// a <em>hint</em> to the implementation to size the buffer and the actual
		/// size may differ. The socket option can be queried to retrieve the actual
		/// size.
		/// 
		/// </para>
		/// <para> For datagram-oriented sockets, the size of the send buffer may limit
		/// the size of the datagrams that may be sent by the socket. Whether
		/// datagrams larger than the buffer size are sent or discarded is system
		/// dependent.
		/// 
		/// </para>
		/// <para> The initial/default size of the socket send buffer and the range of
		/// allowable values is system dependent although a negative size is not
		/// allowed. An attempt to set the socket send buffer to larger than its
		/// maximum size causes it to be set to its maximum size.
		/// 
		/// </para>
		/// <para> An implementation allows this socket option to be set before the
		/// socket is bound or connected. Whether an implementation allows the
		/// socket send buffer to be changed after the socket is bound is system
		/// dependent.
		/// 
		/// </para>
		/// </summary>
		/// <seealso cref= Socket#setSendBufferSize </seealso>
		public static readonly SocketOption<Integer> SO_SNDBUF = new StdSocketOption<Integer>("SO_SNDBUF", typeof(Integer));


		/// <summary>
		/// The size of the socket receive buffer.
		/// 
		/// <para> The value of this socket option is an {@code Integer} that is the
		/// size of the socket receive buffer in bytes. The socket receive buffer is
		/// an input buffer used by the networking implementation. It may need to be
		/// increased for high-volume connections or decreased to limit the possible
		/// backlog of incoming data. The value of the socket option is a
		/// <em>hint</em> to the implementation to size the buffer and the actual
		/// size may differ.
		/// 
		/// </para>
		/// <para> For datagram-oriented sockets, the size of the receive buffer may
		/// limit the size of the datagrams that can be received. Whether datagrams
		/// larger than the buffer size can be received is system dependent.
		/// Increasing the socket receive buffer may be important for cases where
		/// datagrams arrive in bursts faster than they can be processed.
		/// 
		/// </para>
		/// <para> In the case of stream-oriented sockets and the TCP/IP protocol, the
		/// size of the socket receive buffer may be used when advertising the size
		/// of the TCP receive window to the remote peer.
		/// 
		/// </para>
		/// <para> The initial/default size of the socket receive buffer and the range
		/// of allowable values is system dependent although a negative size is not
		/// allowed. An attempt to set the socket receive buffer to larger than its
		/// maximum size causes it to be set to its maximum size.
		/// 
		/// </para>
		/// <para> An implementation allows this socket option to be set before the
		/// socket is bound or connected. Whether an implementation allows the
		/// socket receive buffer to be changed after the socket is bound is system
		/// dependent.
		/// 
		/// </para>
		/// </summary>
		/// <seealso cref= <a href="http://www.ietf.org/rfc/rfc1323.txt">RFC&nbsp;1323: TCP
		/// Extensions for High Performance</a> </seealso>
		/// <seealso cref= Socket#setReceiveBufferSize </seealso>
		/// <seealso cref= ServerSocket#setReceiveBufferSize </seealso>
		public static readonly SocketOption<Integer> SO_RCVBUF = new StdSocketOption<Integer>("SO_RCVBUF", typeof(Integer));

		/// <summary>
		/// Re-use address.
		/// 
		/// <para> The value of this socket option is a {@code Boolean} that represents
		/// whether the option is enabled or disabled. The exact semantics of this
		/// socket option are socket type and system dependent.
		/// 
		/// </para>
		/// <para> In the case of stream-oriented sockets, this socket option will
		/// usually determine whether the socket can be bound to a socket address
		/// when a previous connection involving that socket address is in the
		/// <em>TIME_WAIT</em> state. On implementations where the semantics differ,
		/// and the socket option is not required to be enabled in order to bind the
		/// socket when a previous connection is in this state, then the
		/// implementation may choose to ignore this option.
		/// 
		/// </para>
		/// <para> For datagram-oriented sockets the socket option is used to allow
		/// multiple programs bind to the same address. This option should be enabled
		/// when the socket is to be used for Internet Protocol (IP) multicasting.
		/// 
		/// </para>
		/// <para> An implementation allows this socket option to be set before the
		/// socket is bound or connected. Changing the value of this socket option
		/// after the socket is bound has no effect. The default value of this
		/// socket option is system dependent.
		/// 
		/// </para>
		/// </summary>
		/// <seealso cref= <a href="http://www.ietf.org/rfc/rfc793.txt">RFC&nbsp;793: Transmission
		/// Control Protocol</a> </seealso>
		/// <seealso cref= ServerSocket#setReuseAddress </seealso>
		public static readonly SocketOption<Boolean> SO_REUSEADDR = new StdSocketOption<Boolean>("SO_REUSEADDR", typeof(Boolean));

		/// <summary>
		/// Linger on close if data is present.
		/// 
		/// <para> The value of this socket option is an {@code Integer} that controls
		/// the action taken when unsent data is queued on the socket and a method
		/// to close the socket is invoked. If the value of the socket option is zero
		/// or greater, then it represents a timeout value, in seconds, known as the
		/// <em>linger interval</em>. The linger interval is the timeout for the
		/// {@code close} method to block while the operating system attempts to
		/// transmit the unsent data or it decides that it is unable to transmit the
		/// data. If the value of the socket option is less than zero then the option
		/// is disabled. In that case the {@code close} method does not wait until
		/// unsent data is transmitted; if possible the operating system will transmit
		/// any unsent data before the connection is closed.
		/// 
		/// </para>
		/// <para> This socket option is intended for use with sockets that are configured
		/// in <seealso cref="java.nio.channels.SelectableChannel#isBlocking() blocking"/> mode
		/// only. The behavior of the {@code close} method when this option is
		/// enabled on a non-blocking socket is not defined.
		/// 
		/// </para>
		/// <para> The initial value of this socket option is a negative value, meaning
		/// that the option is disabled. The option may be enabled, or the linger
		/// interval changed, at any time. The maximum value of the linger interval
		/// is system dependent. Setting the linger interval to a value that is
		/// greater than its maximum value causes the linger interval to be set to
		/// its maximum value.
		/// 
		/// </para>
		/// </summary>
		/// <seealso cref= Socket#setSoLinger </seealso>
		public static readonly SocketOption<Integer> SO_LINGER = new StdSocketOption<Integer>("SO_LINGER", typeof(Integer));


		// -- IPPROTO_IP --

		/// <summary>
		/// The Type of Service (ToS) octet in the Internet Protocol (IP) header.
		/// 
		/// <para> The value of this socket option is an {@code Integer} representing
		/// the value of the ToS octet in IP packets sent by sockets to an {@link
		/// StandardProtocolFamily#INET IPv4} socket. The interpretation of the ToS
		/// octet is network specific and is not defined by this class. Further
		/// information on the ToS octet can be found in <a
		/// href="http://www.ietf.org/rfc/rfc1349.txt">RFC&nbsp;1349</a> and <a
		/// href="http://www.ietf.org/rfc/rfc2474.txt">RFC&nbsp;2474</a>. The value
		/// of the socket option is a <em>hint</em>. An implementation may ignore the
		/// value, or ignore specific values.
		/// 
		/// </para>
		/// <para> The initial/default value of the TOS field in the ToS octet is
		/// implementation specific but will typically be {@code 0}. For
		/// datagram-oriented sockets the option may be configured at any time after
		/// the socket has been bound. The new value of the octet is used when sending
		/// subsequent datagrams. It is system dependent whether this option can be
		/// queried or changed prior to binding the socket.
		/// 
		/// </para>
		/// <para> The behavior of this socket option on a stream-oriented socket, or an
		/// <seealso cref="StandardProtocolFamily#INET6 IPv6"/> socket, is not defined in this
		/// release.
		/// 
		/// </para>
		/// </summary>
		/// <seealso cref= DatagramSocket#setTrafficClass </seealso>
		public static readonly SocketOption<Integer> IP_TOS = new StdSocketOption<Integer>("IP_TOS", typeof(Integer));

		/// <summary>
		/// The network interface for Internet Protocol (IP) multicast datagrams.
		/// 
		/// <para> The value of this socket option is a <seealso cref="NetworkInterface"/> that
		/// represents the outgoing interface for multicast datagrams sent by the
		/// datagram-oriented socket. For <seealso cref="StandardProtocolFamily#INET6 IPv6"/>
		/// sockets then it is system dependent whether setting this option also
		/// sets the outgoing interface for multicast datagrams sent to IPv4
		/// addresses.
		/// 
		/// </para>
		/// <para> The initial/default value of this socket option may be {@code null}
		/// to indicate that outgoing interface will be selected by the operating
		/// system, typically based on the network routing tables. An implementation
		/// allows this socket option to be set after the socket is bound. Whether
		/// the socket option can be queried or changed prior to binding the socket
		/// is system dependent.
		/// 
		/// </para>
		/// </summary>
		/// <seealso cref= java.nio.channels.MulticastChannel </seealso>
		/// <seealso cref= MulticastSocket#setInterface </seealso>
		public static readonly SocketOption<NetworkInterface> IP_MULTICAST_IF = new StdSocketOption<NetworkInterface>("IP_MULTICAST_IF", typeof(NetworkInterface));

		/// <summary>
		/// The <em>time-to-live</em> for Internet Protocol (IP) multicast datagrams.
		/// 
		/// <para> The value of this socket option is an {@code Integer} in the range
		/// {@code 0 <= value <= 255}. It is used to control the scope of multicast
		/// datagrams sent by the datagram-oriented socket.
		/// In the case of an <seealso cref="StandardProtocolFamily#INET IPv4"/> socket
		/// the option is the time-to-live (TTL) on multicast datagrams sent by the
		/// socket. Datagrams with a TTL of zero are not transmitted on the network
		/// but may be delivered locally. In the case of an {@link
		/// StandardProtocolFamily#INET6 IPv6} socket the option is the
		/// <em>hop limit</em> which is number of <em>hops</em> that the datagram can
		/// pass through before expiring on the network. For IPv6 sockets it is
		/// system dependent whether the option also sets the <em>time-to-live</em>
		/// on multicast datagrams sent to IPv4 addresses.
		/// 
		/// </para>
		/// <para> The initial/default value of the time-to-live setting is typically
		/// {@code 1}. An implementation allows this socket option to be set after
		/// the socket is bound. Whether the socket option can be queried or changed
		/// prior to binding the socket is system dependent.
		/// 
		/// </para>
		/// </summary>
		/// <seealso cref= java.nio.channels.MulticastChannel </seealso>
		/// <seealso cref= MulticastSocket#setTimeToLive </seealso>
		public static readonly SocketOption<Integer> IP_MULTICAST_TTL = new StdSocketOption<Integer>("IP_MULTICAST_TTL", typeof(Integer));

		/// <summary>
		/// Loopback for Internet Protocol (IP) multicast datagrams.
		/// 
		/// <para> The value of this socket option is a {@code Boolean} that controls
		/// the <em>loopback</em> of multicast datagrams. The value of the socket
		/// option represents if the option is enabled or disabled.
		/// 
		/// </para>
		/// <para> The exact semantics of this socket options are system dependent.
		/// In particular, it is system dependent whether the loopback applies to
		/// multicast datagrams sent from the socket or received by the socket.
		/// For <seealso cref="StandardProtocolFamily#INET6 IPv6"/> sockets then it is
		/// system dependent whether the option also applies to multicast datagrams
		/// sent to IPv4 addresses.
		/// 
		/// </para>
		/// <para> The initial/default value of this socket option is {@code TRUE}. An
		/// implementation allows this socket option to be set after the socket is
		/// bound. Whether the socket option can be queried or changed prior to
		/// binding the socket is system dependent.
		/// 
		/// </para>
		/// </summary>
		/// <seealso cref= java.nio.channels.MulticastChannel </seealso>
		///  <seealso cref= MulticastSocket#setLoopbackMode </seealso>
		public static readonly SocketOption<Boolean> IP_MULTICAST_LOOP = new StdSocketOption<Boolean>("IP_MULTICAST_LOOP", typeof(Boolean));


		// -- IPPROTO_TCP --

		/// <summary>
		/// Disable the Nagle algorithm.
		/// 
		/// <para> The value of this socket option is a {@code Boolean} that represents
		/// whether the option is enabled or disabled. The socket option is specific to
		/// stream-oriented sockets using the TCP/IP protocol. TCP/IP uses an algorithm
		/// known as <em>The Nagle Algorithm</em> to coalesce short segments and
		/// improve network efficiency.
		/// 
		/// </para>
		/// <para> The default value of this socket option is {@code FALSE}. The
		/// socket option should only be enabled in cases where it is known that the
		/// coalescing impacts performance. The socket option may be enabled at any
		/// time. In other words, the Nagle Algorithm can be disabled. Once the option
		/// is enabled, it is system dependent whether it can be subsequently
		/// disabled. If it cannot, then invoking the {@code setOption} method to
		/// disable the option has no effect.
		/// 
		/// </para>
		/// </summary>
		/// <seealso cref= <a href="http://www.ietf.org/rfc/rfc1122.txt">RFC&nbsp;1122:
		/// Requirements for Internet Hosts -- Communication Layers</a> </seealso>
		/// <seealso cref= Socket#setTcpNoDelay </seealso>
		public static readonly SocketOption<Boolean> TCP_NODELAY = new StdSocketOption<Boolean>("TCP_NODELAY", typeof(Boolean));


		private class StdSocketOption<T> : SocketOption<T>
		{
			internal readonly String Name_Renamed;
			internal readonly Class Type_Renamed;
			internal StdSocketOption(String name, Class type)
			{
				this.Name_Renamed = name;
				this.Type_Renamed = type;
			}
			public override String Name()
			{
				return Name_Renamed;
			}
			public override Class Type()
			{
				return Type_Renamed;
			}
			public override String ToString()
			{
				return Name_Renamed;
			}
		}
	}

}