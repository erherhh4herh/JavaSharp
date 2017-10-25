using System;
using System.Collections.Generic;

/*
 * Copyright (c) 1995, 2014, Oracle and/or its affiliates. All rights reserved.
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
	/// The multicast datagram socket class is useful for sending
	/// and receiving IP multicast packets.  A MulticastSocket is
	/// a (UDP) DatagramSocket, with additional capabilities for
	/// joining "groups" of other multicast hosts on the internet.
	/// <P>
	/// A multicast group is specified by a class D IP address
	/// and by a standard UDP port number. Class D IP addresses
	/// are in the range <CODE>224.0.0.0</CODE> to <CODE>239.255.255.255</CODE>,
	/// inclusive. The address 224.0.0.0 is reserved and should not be used.
	/// <P>
	/// One would join a multicast group by first creating a MulticastSocket
	/// with the desired port, then invoking the
	/// <CODE>joinGroup(InetAddress groupAddr)</CODE>
	/// method:
	/// <PRE>
	/// // join a Multicast group and send the group salutations
	/// ...
	/// String msg = "Hello";
	/// InetAddress group = InetAddress.getByName("228.5.6.7");
	/// MulticastSocket s = new MulticastSocket(6789);
	/// s.joinGroup(group);
	/// DatagramPacket hi = new DatagramPacket(msg.getBytes(), msg.length(),
	///                             group, 6789);
	/// s.send(hi);
	/// // get their responses!
	/// byte[] buf = new byte[1000];
	/// DatagramPacket recv = new DatagramPacket(buf, buf.length);
	/// s.receive(recv);
	/// ...
	/// // OK, I'm done talking - leave the group...
	/// s.leaveGroup(group);
	/// </PRE>
	/// 
	/// When one sends a message to a multicast group, <B>all</B> subscribing
	/// recipients to that host and port receive the message (within the
	/// time-to-live range of the packet, see below).  The socket needn't
	/// be a member of the multicast group to send messages to it.
	/// <P>
	/// When a socket subscribes to a multicast group/port, it receives
	/// datagrams sent by other hosts to the group/port, as do all other
	/// members of the group and port.  A socket relinquishes membership
	/// in a group by the leaveGroup(InetAddress addr) method.  <B>
	/// Multiple MulticastSocket's</B> may subscribe to a multicast group
	/// and port concurrently, and they will all receive group datagrams.
	/// <P>
	/// Currently applets are not allowed to use multicast sockets.
	/// 
	/// @author Pavani Diwanji
	/// @since  JDK1.1
	/// </summary>
	public class MulticastSocket : DatagramSocket
	{

		/// <summary>
		/// Used on some platforms to record if an outgoing interface
		/// has been set for this socket.
		/// </summary>
		private bool InterfaceSet;

		/// <summary>
		/// Create a multicast socket.
		/// 
		/// <para>If there is a security manager,
		/// its {@code checkListen} method is first called
		/// with 0 as its argument to ensure the operation is allowed.
		/// This could result in a SecurityException.
		/// </para>
		/// <para>
		/// When the socket is created the
		/// <seealso cref="DatagramSocket#setReuseAddress(boolean)"/> method is
		/// called to enable the SO_REUSEADDR socket option.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="IOException"> if an I/O exception occurs
		/// while creating the MulticastSocket </exception>
		/// <exception cref="SecurityException">  if a security manager exists and its
		///             {@code checkListen} method doesn't allow the operation. </exception>
		/// <seealso cref= SecurityManager#checkListen </seealso>
		/// <seealso cref= java.net.DatagramSocket#setReuseAddress(boolean) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public MulticastSocket() throws java.io.IOException
		public MulticastSocket() : this(new InetSocketAddress(0))
		{
		}

		/// <summary>
		/// Create a multicast socket and bind it to a specific port.
		/// 
		/// <para>If there is a security manager,
		/// its {@code checkListen} method is first called
		/// with the {@code port} argument
		/// as its argument to ensure the operation is allowed.
		/// This could result in a SecurityException.
		/// </para>
		/// <para>
		/// When the socket is created the
		/// <seealso cref="DatagramSocket#setReuseAddress(boolean)"/> method is
		/// called to enable the SO_REUSEADDR socket option.
		/// 
		/// </para>
		/// </summary>
		/// <param name="port"> port to use </param>
		/// <exception cref="IOException"> if an I/O exception occurs
		/// while creating the MulticastSocket </exception>
		/// <exception cref="SecurityException">  if a security manager exists and its
		///             {@code checkListen} method doesn't allow the operation. </exception>
		/// <seealso cref= SecurityManager#checkListen </seealso>
		/// <seealso cref= java.net.DatagramSocket#setReuseAddress(boolean) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public MulticastSocket(int port) throws java.io.IOException
		public MulticastSocket(int port) : this(new InetSocketAddress(port))
		{
		}

		/// <summary>
		/// Create a MulticastSocket bound to the specified socket address.
		/// <para>
		/// Or, if the address is {@code null}, create an unbound socket.
		/// 
		/// </para>
		/// <para>If there is a security manager,
		/// its {@code checkListen} method is first called
		/// with the SocketAddress port as its argument to ensure the operation is allowed.
		/// This could result in a SecurityException.
		/// </para>
		/// <para>
		/// When the socket is created the
		/// <seealso cref="DatagramSocket#setReuseAddress(boolean)"/> method is
		/// called to enable the SO_REUSEADDR socket option.
		/// 
		/// </para>
		/// </summary>
		/// <param name="bindaddr"> Socket address to bind to, or {@code null} for
		///                 an unbound socket. </param>
		/// <exception cref="IOException"> if an I/O exception occurs
		/// while creating the MulticastSocket </exception>
		/// <exception cref="SecurityException">  if a security manager exists and its
		///             {@code checkListen} method doesn't allow the operation. </exception>
		/// <seealso cref= SecurityManager#checkListen </seealso>
		/// <seealso cref= java.net.DatagramSocket#setReuseAddress(boolean)
		/// 
		/// @since 1.4 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public MulticastSocket(SocketAddress bindaddr) throws java.io.IOException
		public MulticastSocket(SocketAddress bindaddr) : base((SocketAddress) null)
		{

			// Enable SO_REUSEADDR before binding
			ReuseAddress = true;

			if (bindaddr != null)
			{
				try
				{
					Bind(bindaddr);
				}
				finally
				{
					if (!Bound)
					{
						Close();
					}
				}
			}
		}

		/// <summary>
		/// The lock on the socket's TTL. This is for set/getTTL and
		/// send(packet,ttl).
		/// </summary>
		private Object TtlLock = new Object();

		/// <summary>
		/// The lock on the socket's interface - used by setInterface
		/// and getInterface
		/// </summary>
		private Object InfLock = new Object();

		/// <summary>
		/// The "last" interface set by setInterface on this MulticastSocket
		/// </summary>
		private InetAddress InfAddress = null;


		/// <summary>
		/// Set the default time-to-live for multicast packets sent out
		/// on this {@code MulticastSocket} in order to control the
		/// scope of the multicasts.
		/// 
		/// <para>The ttl is an <b>unsigned</b> 8-bit quantity, and so <B>must</B> be
		/// in the range {@code 0 <= ttl <= 0xFF }.
		/// 
		/// </para>
		/// </summary>
		/// <param name="ttl"> the time-to-live </param>
		/// <exception cref="IOException"> if an I/O exception occurs
		/// while setting the default time-to-live value </exception>
		/// @deprecated use the setTimeToLive method instead, which uses
		/// <b>int</b> instead of <b>byte</b> as the type for ttl. 
		/// <seealso cref= #getTTL() </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deprecated("use the setTimeToLive method instead, which uses") public void setTTL(byte ttl) throws java.io.IOException
		[Obsolete("use the setTimeToLive method instead, which uses")]
		public virtual sbyte TTL
		{
			set
			{
				if (Closed)
				{
					throw new SocketException("Socket is closed");
				}
				Impl.TTL = value;
			}
			get
			{
				if (Closed)
				{
					throw new SocketException("Socket is closed");
				}
				return Impl.TTL;
			}
		}

		/// <summary>
		/// Set the default time-to-live for multicast packets sent out
		/// on this {@code MulticastSocket} in order to control the
		/// scope of the multicasts.
		/// 
		/// <P> The ttl <B>must</B> be in the range {@code  0 <= ttl <=
		/// 255} or an {@code IllegalArgumentException} will be thrown.
		/// Multicast packets sent with a TTL of {@code 0} are not transmitted
		/// on the network but may be delivered locally.
		/// </summary>
		/// <param name="ttl">
		///         the time-to-live
		/// </param>
		/// <exception cref="IOException">
		///          if an I/O exception occurs while setting the
		///          default time-to-live value
		/// </exception>
		/// <seealso cref= #getTimeToLive() </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setTimeToLive(int ttl) throws java.io.IOException
		public virtual int TimeToLive
		{
			set
			{
				if (value < 0 || value > 255)
				{
					throw new IllegalArgumentException("ttl out of range");
				}
				if (Closed)
				{
					throw new SocketException("Socket is closed");
				}
				Impl.TimeToLive = value;
			}
			get
			{
				if (Closed)
				{
					throw new SocketException("Socket is closed");
				}
				return Impl.TimeToLive;
			}
		}

		/// <summary>
		/// Get the default time-to-live for multicast packets sent out on
		/// the socket.
		/// </summary>
		/// <exception cref="IOException"> if an I/O exception occurs
		/// while getting the default time-to-live value </exception>
		/// <returns> the default time-to-live value </returns>
		/// @deprecated use the getTimeToLive method instead, which returns
		/// an <b>int</b> instead of a <b>byte</b>. 
		/// <seealso cref= #setTTL(byte) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deprecated("use the getTimeToLive method instead, which returns") public byte getTTL() throws java.io.IOException


		/// <summary>
		/// Joins a multicast group. Its behavior may be affected by
		/// {@code setInterface} or {@code setNetworkInterface}.
		/// 
		/// <para>If there is a security manager, this method first
		/// calls its {@code checkMulticast} method
		/// with the {@code mcastaddr} argument
		/// as its argument.
		/// 
		/// </para>
		/// </summary>
		/// <param name="mcastaddr"> is the multicast address to join
		/// </param>
		/// <exception cref="IOException"> if there is an error joining
		/// or when the address is not a multicast address. </exception>
		/// <exception cref="SecurityException">  if a security manager exists and its
		/// {@code checkMulticast} method doesn't allow the join.
		/// </exception>
		/// <seealso cref= SecurityManager#checkMulticast(InetAddress) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void joinGroup(InetAddress mcastaddr) throws java.io.IOException
		public virtual void JoinGroup(InetAddress mcastaddr)
		{
			if (Closed)
			{
				throw new SocketException("Socket is closed");
			}

			CheckAddress(mcastaddr, "joinGroup");
			SecurityManager security = System.SecurityManager;
			if (security != null)
			{
				security.CheckMulticast(mcastaddr);
			}

			if (!mcastaddr.MulticastAddress)
			{
				throw new SocketException("Not a multicast address");
			}

			/// <summary>
			/// required for some platforms where it's not possible to join
			/// a group without setting the interface first.
			/// </summary>
			NetworkInterface defaultInterface = NetworkInterface.Default;

			if (!InterfaceSet && defaultInterface != null)
			{
				NetworkInterface = defaultInterface;
			}

			Impl.Join(mcastaddr);
		}

		/// <summary>
		/// Leave a multicast group. Its behavior may be affected by
		/// {@code setInterface} or {@code setNetworkInterface}.
		/// 
		/// <para>If there is a security manager, this method first
		/// calls its {@code checkMulticast} method
		/// with the {@code mcastaddr} argument
		/// as its argument.
		/// 
		/// </para>
		/// </summary>
		/// <param name="mcastaddr"> is the multicast address to leave </param>
		/// <exception cref="IOException"> if there is an error leaving
		/// or when the address is not a multicast address. </exception>
		/// <exception cref="SecurityException">  if a security manager exists and its
		/// {@code checkMulticast} method doesn't allow the operation.
		/// </exception>
		/// <seealso cref= SecurityManager#checkMulticast(InetAddress) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void leaveGroup(InetAddress mcastaddr) throws java.io.IOException
		public virtual void LeaveGroup(InetAddress mcastaddr)
		{
			if (Closed)
			{
				throw new SocketException("Socket is closed");
			}

			CheckAddress(mcastaddr, "leaveGroup");
			SecurityManager security = System.SecurityManager;
			if (security != null)
			{
				security.CheckMulticast(mcastaddr);
			}

			if (!mcastaddr.MulticastAddress)
			{
				throw new SocketException("Not a multicast address");
			}

			Impl.Leave(mcastaddr);
		}

		/// <summary>
		/// Joins the specified multicast group at the specified interface.
		/// 
		/// <para>If there is a security manager, this method first
		/// calls its {@code checkMulticast} method
		/// with the {@code mcastaddr} argument
		/// as its argument.
		/// 
		/// </para>
		/// </summary>
		/// <param name="mcastaddr"> is the multicast address to join </param>
		/// <param name="netIf"> specifies the local interface to receive multicast
		///        datagram packets, or <i>null</i> to defer to the interface set by
		///       <seealso cref="MulticastSocket#setInterface(InetAddress)"/> or
		///       <seealso cref="MulticastSocket#setNetworkInterface(NetworkInterface)"/>
		/// </param>
		/// <exception cref="IOException"> if there is an error joining
		/// or when the address is not a multicast address. </exception>
		/// <exception cref="SecurityException">  if a security manager exists and its
		/// {@code checkMulticast} method doesn't allow the join. </exception>
		/// <exception cref="IllegalArgumentException"> if mcastaddr is null or is a
		///          SocketAddress subclass not supported by this socket
		/// </exception>
		/// <seealso cref= SecurityManager#checkMulticast(InetAddress)
		/// @since 1.4 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void joinGroup(SocketAddress mcastaddr, NetworkInterface netIf) throws java.io.IOException
		public virtual void JoinGroup(SocketAddress mcastaddr, NetworkInterface netIf)
		{
			if (Closed)
			{
				throw new SocketException("Socket is closed");
			}

			if (mcastaddr == null || !(mcastaddr is InetSocketAddress))
			{
				throw new IllegalArgumentException("Unsupported address type");
			}

			if (OldImpl)
			{
				throw new UnsupportedOperationException();
			}

			CheckAddress(((InetSocketAddress)mcastaddr).Address, "joinGroup");
			SecurityManager security = System.SecurityManager;
			if (security != null)
			{
				security.CheckMulticast(((InetSocketAddress)mcastaddr).Address);
			}

			if (!((InetSocketAddress)mcastaddr).Address.MulticastAddress)
			{
				throw new SocketException("Not a multicast address");
			}

			Impl.JoinGroup(mcastaddr, netIf);
		}

		/// <summary>
		/// Leave a multicast group on a specified local interface.
		/// 
		/// <para>If there is a security manager, this method first
		/// calls its {@code checkMulticast} method
		/// with the {@code mcastaddr} argument
		/// as its argument.
		/// 
		/// </para>
		/// </summary>
		/// <param name="mcastaddr"> is the multicast address to leave </param>
		/// <param name="netIf"> specifies the local interface or <i>null</i> to defer
		///             to the interface set by
		///             <seealso cref="MulticastSocket#setInterface(InetAddress)"/> or
		///             <seealso cref="MulticastSocket#setNetworkInterface(NetworkInterface)"/> </param>
		/// <exception cref="IOException"> if there is an error leaving
		/// or when the address is not a multicast address. </exception>
		/// <exception cref="SecurityException">  if a security manager exists and its
		/// {@code checkMulticast} method doesn't allow the operation. </exception>
		/// <exception cref="IllegalArgumentException"> if mcastaddr is null or is a
		///          SocketAddress subclass not supported by this socket
		/// </exception>
		/// <seealso cref= SecurityManager#checkMulticast(InetAddress)
		/// @since 1.4 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void leaveGroup(SocketAddress mcastaddr, NetworkInterface netIf) throws java.io.IOException
		public virtual void LeaveGroup(SocketAddress mcastaddr, NetworkInterface netIf)
		{
			if (Closed)
			{
				throw new SocketException("Socket is closed");
			}

			if (mcastaddr == null || !(mcastaddr is InetSocketAddress))
			{
				throw new IllegalArgumentException("Unsupported address type");
			}

			if (OldImpl)
			{
				throw new UnsupportedOperationException();
			}

			CheckAddress(((InetSocketAddress)mcastaddr).Address, "leaveGroup");
			SecurityManager security = System.SecurityManager;
			if (security != null)
			{
				security.CheckMulticast(((InetSocketAddress)mcastaddr).Address);
			}

			if (!((InetSocketAddress)mcastaddr).Address.MulticastAddress)
			{
				throw new SocketException("Not a multicast address");
			}

			Impl.LeaveGroup(mcastaddr, netIf);
		}

		/// <summary>
		/// Set the multicast network interface used by methods
		/// whose behavior would be affected by the value of the
		/// network interface. Useful for multihomed hosts. </summary>
		/// <param name="inf"> the InetAddress </param>
		/// <exception cref="SocketException"> if there is an error in
		/// the underlying protocol, such as a TCP error. </exception>
		/// <seealso cref= #getInterface() </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setInterface(InetAddress inf) throws SocketException
		public virtual InetAddress Interface
		{
			set
			{
				if (Closed)
				{
					throw new SocketException("Socket is closed");
				}
				CheckAddress(value, "setInterface");
				lock (InfLock)
				{
					Impl.SetOption(SocketOptions_Fields.IP_MULTICAST_IF, value);
					InfAddress = value;
					InterfaceSet = true;
				}
			}
			get
			{
				if (Closed)
				{
					throw new SocketException("Socket is closed");
				}
				lock (InfLock)
				{
					InetAddress ia = (InetAddress)Impl.GetOption(SocketOptions_Fields.IP_MULTICAST_IF);
    
					/// <summary>
					/// No previous setInterface or interface can be
					/// set using setNetworkInterface
					/// </summary>
					if (InfAddress == null)
					{
						return ia;
					}
    
					/// <summary>
					/// Same interface set with setInterface?
					/// </summary>
					if (ia.Equals(InfAddress))
					{
						return ia;
					}
    
					/// <summary>
					/// Different InetAddress from what we set with setInterface
					/// so enumerate the current interface to see if the
					/// address set by setInterface is bound to this interface.
					/// </summary>
					try
					{
						NetworkInterface ni = NetworkInterface.GetByInetAddress(ia);
						IEnumerator<InetAddress> addrs = ni.InetAddresses;
						while (addrs.MoveNext())
						{
							InetAddress addr = addrs.Current;
							if (addr.Equals(InfAddress))
							{
								return InfAddress;
							}
						}
    
						/// <summary>
						/// No match so reset infAddress to indicate that the
						/// interface has changed via means
						/// </summary>
						InfAddress = null;
						return ia;
					}
					catch (Exception)
					{
						return ia;
					}
				}
			}
		}


		/// <summary>
		/// Specify the network interface for outgoing multicast datagrams
		/// sent on this socket.
		/// </summary>
		/// <param name="netIf"> the interface </param>
		/// <exception cref="SocketException"> if there is an error in
		/// the underlying protocol, such as a TCP error. </exception>
		/// <seealso cref= #getNetworkInterface()
		/// @since 1.4 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setNetworkInterface(NetworkInterface netIf) throws SocketException
		public virtual NetworkInterface NetworkInterface
		{
			set
			{
    
				lock (InfLock)
				{
					Impl.SetOption(SocketOptions_Fields.IP_MULTICAST_IF2, value);
					InfAddress = null;
					InterfaceSet = true;
				}
			}
			get
			{
				NetworkInterface ni = (NetworkInterface)Impl.GetOption(SocketOptions_Fields.IP_MULTICAST_IF2);
				if ((ni.Index == 0) || (ni.Index == -1))
				{
					InetAddress[] addrs = new InetAddress[1];
					addrs[0] = InetAddress.AnyLocalAddress();
					return new NetworkInterface(addrs[0].HostName, 0, addrs);
				}
				else
				{
					return ni;
				}
			}
		}


		/// <summary>
		/// Disable/Enable local loopback of multicast datagrams
		/// The option is used by the platform's networking code as a hint
		/// for setting whether multicast data will be looped back to
		/// the local socket.
		/// 
		/// <para>Because this option is a hint, applications that want to
		/// verify what loopback mode is set to should call
		/// <seealso cref="#getLoopbackMode()"/>
		/// </para>
		/// </summary>
		/// <param name="disable"> {@code true} to disable the LoopbackMode </param>
		/// <exception cref="SocketException"> if an error occurs while setting the value
		/// @since 1.4 </exception>
		/// <seealso cref= #getLoopbackMode </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setLoopbackMode(boolean disable) throws SocketException
		public virtual bool LoopbackMode
		{
			set
			{
				Impl.SetOption(SocketOptions_Fields.IP_MULTICAST_LOOP, Convert.ToBoolean(value));
			}
			get
			{
				return ((Boolean)Impl.GetOption(SocketOptions_Fields.IP_MULTICAST_LOOP)).BooleanValue();
			}
		}


		/// <summary>
		/// Sends a datagram packet to the destination, with a TTL (time-
		/// to-live) other than the default for the socket.  This method
		/// need only be used in instances where a particular TTL is desired;
		/// otherwise it is preferable to set a TTL once on the socket, and
		/// use that default TTL for all packets.  This method does <B>not
		/// </B> alter the default TTL for the socket. Its behavior may be
		/// affected by {@code setInterface}.
		/// 
		/// <para>If there is a security manager, this method first performs some
		/// security checks. First, if {@code p.getAddress().isMulticastAddress()}
		/// is true, this method calls the
		/// security manager's {@code checkMulticast} method
		/// with {@code p.getAddress()} and {@code ttl} as its arguments.
		/// If the evaluation of that expression is false,
		/// this method instead calls the security manager's
		/// {@code checkConnect} method with arguments
		/// {@code p.getAddress().getHostAddress()} and
		/// {@code p.getPort()}. Each call to a security manager method
		/// could result in a SecurityException if the operation is not allowed.
		/// 
		/// </para>
		/// </summary>
		/// <param name="p"> is the packet to be sent. The packet should contain
		/// the destination multicast ip address and the data to be sent.
		/// One does not need to be the member of the group to send
		/// packets to a destination multicast address. </param>
		/// <param name="ttl"> optional time to live for multicast packet.
		/// default ttl is 1.
		/// </param>
		/// <exception cref="IOException"> is raised if an error occurs i.e
		/// error while setting ttl. </exception>
		/// <exception cref="SecurityException">  if a security manager exists and its
		///             {@code checkMulticast} or {@code checkConnect}
		///             method doesn't allow the send.
		/// </exception>
		/// @deprecated Use the following code or its equivalent instead:
		///  ......
		///  int ttl = mcastSocket.getTimeToLive();
		///  mcastSocket.setTimeToLive(newttl);
		///  mcastSocket.send(p);
		///  mcastSocket.setTimeToLive(ttl);
		///  ......
		/// 
		/// <seealso cref= DatagramSocket#send </seealso>
		/// <seealso cref= DatagramSocket#receive </seealso>
		/// <seealso cref= SecurityManager#checkMulticast(java.net.InetAddress, byte) </seealso>
		/// <seealso cref= SecurityManager#checkConnect </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deprecated("Use the following code or its equivalent instead:") public void send(DatagramPacket p, byte ttl) throws java.io.IOException
		[Obsolete("Use the following code or its equivalent instead:")]
		public virtual void Send(DatagramPacket p, sbyte ttl)
		{
				if (Closed)
				{
					throw new SocketException("Socket is closed");
				}
				CheckAddress(p.Address, "send");
				lock (TtlLock)
				{
					lock (p)
					{
						if (ConnectState == ST_NOT_CONNECTED)
						{
							// Security manager makes sure that the multicast address
							// is allowed one and that the ttl used is less
							// than the allowed maxttl.
							SecurityManager security = System.SecurityManager;
							if (security != null)
							{
								if (p.Address.MulticastAddress)
								{
									security.CheckMulticast(p.Address, ttl);
								}
								else
								{
									security.CheckConnect(p.Address.HostAddress, p.Port);
								}
							}
						}
						else
						{
							// we're connected
							InetAddress packetAddress = null;
							packetAddress = p.Address;
							if (packetAddress == null)
							{
								p.Address = ConnectedAddress;
								p.Port = ConnectedPort;
							}
							else if ((!packetAddress.Equals(ConnectedAddress)) || p.Port != ConnectedPort)
							{
								throw new SecurityException("connected address and packet address" + " differ");
							}
						}
						sbyte dttl = TTL;
						try
						{
							if (ttl != dttl)
							{
								// set the ttl
								Impl.TTL = ttl;
							}
							// call the datagram method to send
							Impl.Send(p);
						}
						finally
						{
							// set it back to default
							if (ttl != dttl)
							{
								Impl.TTL = dttl;
							}
						}
					} // synch p
				} //synch ttl
		} //method
	}

}