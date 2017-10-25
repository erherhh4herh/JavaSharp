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
	/// A network channel that supports Internet Protocol (IP) multicasting.
	/// 
	/// <para> IP multicasting is the transmission of IP datagrams to members of
	/// a <em>group</em> that is zero or more hosts identified by a single destination
	/// address.
	/// 
	/// </para>
	/// <para> In the case of a channel to an <seealso cref="StandardProtocolFamily#INET IPv4"/> socket,
	/// the underlying operating system supports <a href="http://www.ietf.org/rfc/rfc2236.txt">
	/// <i>RFC&nbsp;2236: Internet Group Management Protocol, Version 2 (IGMPv2)</i></a>.
	/// It may optionally support source filtering as specified by <a
	/// href="http://www.ietf.org/rfc/rfc3376.txt"> <i>RFC&nbsp;3376: Internet Group
	/// Management Protocol, Version 3 (IGMPv3)</i></a>.
	/// For channels to an <seealso cref="StandardProtocolFamily#INET6 IPv6"/> socket, the equivalent
	/// standards are <a href="http://www.ietf.org/rfc/rfc2710.txt"> <i>RFC&nbsp;2710:
	/// Multicast Listener Discovery (MLD) for IPv6</i></a> and <a
	/// href="http://www.ietf.org/rfc/rfc3810.txt"> <i>RFC&nbsp;3810: Multicast Listener
	/// Discovery Version 2 (MLDv2) for IPv6</i></a>.
	/// 
	/// </para>
	/// <para> The <seealso cref="#join(InetAddress,NetworkInterface)"/> method is used to
	/// join a group and receive all multicast datagrams sent to the group. A channel
	/// may join several multicast groups and may join the same group on several
	/// <seealso cref="NetworkInterface interfaces"/>. Membership is dropped by invoking the {@link
	/// MembershipKey#drop drop} method on the returned <seealso cref="MembershipKey"/>. If the
	/// underlying platform supports source filtering then the {@link MembershipKey#block
	/// block} and <seealso cref="MembershipKey#unblock unblock"/> methods can be used to block or
	/// unblock multicast datagrams from particular source addresses.
	/// 
	/// </para>
	/// <para> The <seealso cref="#join(InetAddress,NetworkInterface,InetAddress)"/> method
	/// is used to begin receiving datagrams sent to a group whose source address matches
	/// a given source address. This method throws <seealso cref="UnsupportedOperationException"/>
	/// if the underlying platform does not support source filtering.  Membership is
	/// <em>cumulative</em> and this method may be invoked again with the same group
	/// and interface to allow receiving datagrams from other source addresses. The
	/// method returns a <seealso cref="MembershipKey"/> that represents membership to receive
	/// datagrams from the given source address. Invoking the key's {@link
	/// MembershipKey#drop drop} method drops membership so that datagrams from the
	/// source address can no longer be received.
	/// 
	/// <h2>Platform dependencies</h2>
	/// 
	/// The multicast implementation is intended to map directly to the native
	/// multicasting facility. Consequently, the following items should be considered
	/// when developing an application that receives IP multicast datagrams:
	/// 
	/// <ol>
	/// 
	/// </para>
	/// <li><para> The creation of the channel should specify the <seealso cref="ProtocolFamily"/>
	/// that corresponds to the address type of the multicast groups that the channel
	/// will join. There is no guarantee that a channel to a socket in one protocol
	/// family can join and receive multicast datagrams when the address of the
	/// multicast group corresponds to another protocol family. For example, it is
	/// implementation specific if a channel to an <seealso cref="StandardProtocolFamily#INET6 IPv6"/>
	/// socket can join an <seealso cref="StandardProtocolFamily#INET IPv4"/> multicast group and receive
	/// multicast datagrams sent to the group. </para></li>
	/// 
	/// <li><para> The channel's socket should be bound to the {@link
	/// InetAddress#isAnyLocalAddress wildcard} address. If the socket is bound to
	/// a specific address, rather than the wildcard address then it is implementation
	/// specific if multicast datagrams are received by the socket. </para></li>
	/// 
	/// <li><para> The <seealso cref="StandardSocketOptions#SO_REUSEADDR SO_REUSEADDR"/> option should be
	/// enabled prior to <seealso cref="NetworkChannel#bind binding"/> the socket. This is
	/// required to allow multiple members of the group to bind to the same
	/// address. </para></li>
	/// 
	/// </ol>
	/// 
	/// <para> <b>Usage Example:</b>
	/// <pre>
	///     // join multicast group on this interface, and also use this
	///     // interface for outgoing multicast datagrams
	///     NetworkInterface ni = NetworkInterface.getByName("hme0");
	/// 
	///     DatagramChannel dc = DatagramChannel.open(StandardProtocolFamily.INET)
	///         .setOption(StandardSocketOptions.SO_REUSEADDR, true)
	///         .bind(new InetSocketAddress(5000))
	///         .setOption(StandardSocketOptions.IP_MULTICAST_IF, ni);
	/// 
	///     InetAddress group = InetAddress.getByName("225.4.5.6");
	/// 
	///     MembershipKey key = dc.join(group, ni);
	/// </pre>
	/// 
	/// @since 1.7
	/// </para>
	/// </summary>

	public interface MulticastChannel : NetworkChannel
	{
		/// <summary>
		/// Closes this channel.
		/// 
		/// <para> If the channel is a member of a multicast group then the membership
		/// is <seealso cref="MembershipKey#drop dropped"/>. Upon return, the {@link
		/// MembershipKey membership-key} will be {@link MembershipKey#isValid
		/// invalid}.
		/// 
		/// </para>
		/// <para> This method otherwise behaves exactly as specified by the {@link
		/// Channel} interface.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="IOException">
		///          If an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override void close() throws java.io.IOException;
		void Close();

		/// <summary>
		/// Joins a multicast group to begin receiving all datagrams sent to the group,
		/// returning a membership key.
		/// 
		/// <para> If this channel is currently a member of the group on the given
		/// interface to receive all datagrams then the membership key, representing
		/// that membership, is returned. Otherwise this channel joins the group and
		/// the resulting new membership key is returned. The resulting membership key
		/// is not <seealso cref="MembershipKey#sourceAddress source-specific"/>.
		/// 
		/// </para>
		/// <para> A multicast channel may join several multicast groups, including
		/// the same group on more than one interface. An implementation may impose a
		/// limit on the number of groups that may be joined at the same time.
		/// 
		/// </para>
		/// </summary>
		/// <param name="group">
		///          The multicast address to join </param>
		/// <param name="interf">
		///          The network interface on which to join the group
		/// </param>
		/// <returns>  The membership key
		/// </returns>
		/// <exception cref="IllegalArgumentException">
		///          If the group parameter is not a {@link InetAddress#isMulticastAddress
		///          multicast} address, or the group parameter is an address type
		///          that is not supported by this channel </exception>
		/// <exception cref="IllegalStateException">
		///          If the channel already has source-specific membership of the
		///          group on the interface </exception>
		/// <exception cref="UnsupportedOperationException">
		///          If the channel's socket is not an Internet Protocol socket </exception>
		/// <exception cref="ClosedChannelException">
		///          If this channel is closed </exception>
		/// <exception cref="IOException">
		///          If an I/O error occurs </exception>
		/// <exception cref="SecurityException">
		///          If a security manager is set, and its
		///          <seealso cref="SecurityManager#checkMulticast(InetAddress) checkMulticast"/>
		///          method denies access to the multiast group </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: MembershipKey join(java.net.InetAddress group, java.net.NetworkInterface interf) throws java.io.IOException;
		MembershipKey Join(InetAddress group, NetworkInterface interf);

		/// <summary>
		/// Joins a multicast group to begin receiving datagrams sent to the group
		/// from a given source address.
		/// 
		/// <para> If this channel is currently a member of the group on the given
		/// interface to receive datagrams from the given source address then the
		/// membership key, representing that membership, is returned. Otherwise this
		/// channel joins the group and the resulting new membership key is returned.
		/// The resulting membership key is {@link MembershipKey#sourceAddress
		/// source-specific}.
		/// 
		/// </para>
		/// <para> Membership is <em>cumulative</em> and this method may be invoked
		/// again with the same group and interface to allow receiving datagrams sent
		/// by other source addresses to the group.
		/// 
		/// </para>
		/// </summary>
		/// <param name="group">
		///          The multicast address to join </param>
		/// <param name="interf">
		///          The network interface on which to join the group </param>
		/// <param name="source">
		///          The source address
		/// </param>
		/// <returns>  The membership key
		/// </returns>
		/// <exception cref="IllegalArgumentException">
		///          If the group parameter is not a {@link
		///          InetAddress#isMulticastAddress multicast} address, the
		///          source parameter is not a unicast address, the group
		///          parameter is an address type that is not supported by this channel,
		///          or the source parameter is not the same address type as the group </exception>
		/// <exception cref="IllegalStateException">
		///          If the channel is currently a member of the group on the given
		///          interface to receive all datagrams </exception>
		/// <exception cref="UnsupportedOperationException">
		///          If the channel's socket is not an Internet Protocol socket or
		///          source filtering is not supported </exception>
		/// <exception cref="ClosedChannelException">
		///          If this channel is closed </exception>
		/// <exception cref="IOException">
		///          If an I/O error occurs </exception>
		/// <exception cref="SecurityException">
		///          If a security manager is set, and its
		///          <seealso cref="SecurityManager#checkMulticast(InetAddress) checkMulticast"/>
		///          method denies access to the multiast group </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: MembershipKey join(java.net.InetAddress group, java.net.NetworkInterface interf, java.net.InetAddress source) throws java.io.IOException;
		MembershipKey Join(InetAddress group, NetworkInterface interf, InetAddress source);
	}

}