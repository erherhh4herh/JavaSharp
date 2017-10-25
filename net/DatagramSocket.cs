using System;

/*
 * Copyright (c) 1995, 2013, Oracle and/or its affiliates. All rights reserved.
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
	/// This class represents a socket for sending and receiving datagram packets.
	/// 
	/// <para>A datagram socket is the sending or receiving point for a packet
	/// delivery service. Each packet sent or received on a datagram socket
	/// is individually addressed and routed. Multiple packets sent from
	/// one machine to another may be routed differently, and may arrive in
	/// any order.
	/// 
	/// </para>
	/// <para> Where possible, a newly constructed {@code DatagramSocket} has the
	/// <seealso cref="SocketOptions#SO_BROADCAST SO_BROADCAST"/> socket option enabled so as
	/// to allow the transmission of broadcast datagrams. In order to receive
	/// broadcast packets a DatagramSocket should be bound to the wildcard address.
	/// In some implementations, broadcast packets may also be received when
	/// a DatagramSocket is bound to a more specific address.
	/// </para>
	/// <para>
	/// Example:
	/// {@code
	///              DatagramSocket s = new DatagramSocket(null);
	///              s.bind(new InetSocketAddress(8888));
	/// }
	/// Which is equivalent to:
	/// {@code
	///              DatagramSocket s = new DatagramSocket(8888);
	/// }
	/// Both cases will create a DatagramSocket able to receive broadcasts on
	/// UDP port 8888.
	/// 
	/// @author  Pavani Diwanji
	/// </para>
	/// </summary>
	/// <seealso cref=     java.net.DatagramPacket </seealso>
	/// <seealso cref=     java.nio.channels.DatagramChannel
	/// @since JDK1.0 </seealso>
	public class DatagramSocket : java.io.Closeable
	{
		/// <summary>
		/// Various states of this socket.
		/// </summary>
		private bool Created = false;
		private bool Bound_Renamed = false;
		private bool Closed_Renamed = false;
		private Object CloseLock = new Object();

		/*
		 * The implementation of this DatagramSocket.
		 */
		internal DatagramSocketImpl Impl_Renamed;

		/// <summary>
		/// Are we using an older DatagramSocketImpl?
		/// </summary>
		internal bool OldImpl = false;

		/// <summary>
		/// Set when a socket is ST_CONNECTED until we are certain
		/// that any packets which might have been received prior
		/// to calling connect() but not read by the application
		/// have been read. During this time we check the source
		/// address of all packets received to be sure they are from
		/// the connected destination. Other packets are read but
		/// silently dropped.
		/// </summary>
		private bool ExplicitFilter = false;
		private int BytesLeftToFilter;
		/*
		 * Connection state:
		 * ST_NOT_CONNECTED = socket not connected
		 * ST_CONNECTED = socket connected
		 * ST_CONNECTED_NO_IMPL = socket connected but not at impl level
		 */
		internal const int ST_NOT_CONNECTED = 0;
		internal const int ST_CONNECTED = 1;
		internal const int ST_CONNECTED_NO_IMPL = 2;

		internal int ConnectState = ST_NOT_CONNECTED;

		/*
		 * Connected address & port
		 */
		internal InetAddress ConnectedAddress = null;
		internal int ConnectedPort = -1;

		/// <summary>
		/// Connects this socket to a remote socket address (IP address + port number).
		/// Binds socket if not already bound.
		/// <para>
		/// </para>
		/// </summary>
		/// <param name="address"> The remote address. </param>
		/// <param name="port">    The remote port </param>
		/// <exception cref="SocketException"> if binding the socket fails. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private synchronized void connectInternal(InetAddress address, int port) throws SocketException
		private void ConnectInternal(InetAddress address, int port)
		{
			lock (this)
			{
				if (port < 0 || port > 0xFFFF)
				{
					throw new IllegalArgumentException("connect: " + port);
				}
				if (address == null)
				{
					throw new IllegalArgumentException("connect: null address");
				}
				CheckAddress(address, "connect");
				if (Closed)
				{
					return;
				}
				SecurityManager security = System.SecurityManager;
				if (security != null)
				{
					if (address.MulticastAddress)
					{
						security.CheckMulticast(address);
					}
					else
					{
						security.CheckConnect(address.HostAddress, port);
						security.CheckAccept(address.HostAddress, port);
					}
				}
        
				if (!Bound)
				{
				  Bind(new InetSocketAddress(0));
				}
        
				// old impls do not support connect/disconnect
				if (OldImpl || (Impl_Renamed is AbstractPlainDatagramSocketImpl && ((AbstractPlainDatagramSocketImpl)Impl_Renamed).NativeConnectDisabled()))
				{
					ConnectState = ST_CONNECTED_NO_IMPL;
				}
				else
				{
					try
					{
						Impl.Connect(address, port);
        
						// socket is now connected by the impl
						ConnectState = ST_CONNECTED;
						// Do we need to filter some packets?
						int avail = Impl.DataAvailable();
						if (avail == -1)
						{
							throw new SocketException();
						}
						ExplicitFilter = avail > 0;
						if (ExplicitFilter)
						{
							BytesLeftToFilter = ReceiveBufferSize;
						}
					}
					catch (SocketException)
					{
        
						// connection will be emulated by DatagramSocket
						ConnectState = ST_CONNECTED_NO_IMPL;
					}
				}
        
				ConnectedAddress = address;
				ConnectedPort = port;
			}
		}


		/// <summary>
		/// Constructs a datagram socket and binds it to any available port
		/// on the local host machine.  The socket will be bound to the
		/// <seealso cref="InetAddress#isAnyLocalAddress wildcard"/> address,
		/// an IP address chosen by the kernel.
		/// 
		/// <para>If there is a security manager,
		/// its {@code checkListen} method is first called
		/// with 0 as its argument to ensure the operation is allowed.
		/// This could result in a SecurityException.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="SocketException">  if the socket could not be opened,
		///               or the socket could not bind to the specified local port. </exception>
		/// <exception cref="SecurityException">  if a security manager exists and its
		///             {@code checkListen} method doesn't allow the operation.
		/// </exception>
		/// <seealso cref= SecurityManager#checkListen </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DatagramSocket() throws SocketException
		public DatagramSocket() : this(new InetSocketAddress(0))
		{
		}

		/// <summary>
		/// Creates an unbound datagram socket with the specified
		/// DatagramSocketImpl.
		/// </summary>
		/// <param name="impl"> an instance of a <B>DatagramSocketImpl</B>
		///        the subclass wishes to use on the DatagramSocket.
		/// @since   1.4 </param>
		protected internal DatagramSocket(DatagramSocketImpl impl)
		{
			if (impl == null)
			{
				throw new NullPointerException();
			}
			this.Impl_Renamed = impl;
			CheckOldImpl();
		}

		/// <summary>
		/// Creates a datagram socket, bound to the specified local
		/// socket address.
		/// <para>
		/// If, if the address is {@code null}, creates an unbound socket.
		/// 
		/// </para>
		/// <para>If there is a security manager,
		/// its {@code checkListen} method is first called
		/// with the port from the socket address
		/// as its argument to ensure the operation is allowed.
		/// This could result in a SecurityException.
		/// 
		/// </para>
		/// </summary>
		/// <param name="bindaddr"> local socket address to bind, or {@code null}
		///                 for an unbound socket.
		/// </param>
		/// <exception cref="SocketException">  if the socket could not be opened,
		///               or the socket could not bind to the specified local port. </exception>
		/// <exception cref="SecurityException">  if a security manager exists and its
		///             {@code checkListen} method doesn't allow the operation.
		/// </exception>
		/// <seealso cref= SecurityManager#checkListen
		/// @since   1.4 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DatagramSocket(SocketAddress bindaddr) throws SocketException
		public DatagramSocket(SocketAddress bindaddr)
		{
			// create a datagram socket.
			CreateImpl();
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
		/// Constructs a datagram socket and binds it to the specified port
		/// on the local host machine.  The socket will be bound to the
		/// <seealso cref="InetAddress#isAnyLocalAddress wildcard"/> address,
		/// an IP address chosen by the kernel.
		/// 
		/// <para>If there is a security manager,
		/// its {@code checkListen} method is first called
		/// with the {@code port} argument
		/// as its argument to ensure the operation is allowed.
		/// This could result in a SecurityException.
		/// 
		/// </para>
		/// </summary>
		/// <param name="port"> port to use. </param>
		/// <exception cref="SocketException">  if the socket could not be opened,
		///               or the socket could not bind to the specified local port. </exception>
		/// <exception cref="SecurityException">  if a security manager exists and its
		///             {@code checkListen} method doesn't allow the operation.
		/// </exception>
		/// <seealso cref= SecurityManager#checkListen </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DatagramSocket(int port) throws SocketException
		public DatagramSocket(int port) : this(port, null)
		{
		}

		/// <summary>
		/// Creates a datagram socket, bound to the specified local
		/// address.  The local port must be between 0 and 65535 inclusive.
		/// If the IP address is 0.0.0.0, the socket will be bound to the
		/// <seealso cref="InetAddress#isAnyLocalAddress wildcard"/> address,
		/// an IP address chosen by the kernel.
		/// 
		/// <para>If there is a security manager,
		/// its {@code checkListen} method is first called
		/// with the {@code port} argument
		/// as its argument to ensure the operation is allowed.
		/// This could result in a SecurityException.
		/// 
		/// </para>
		/// </summary>
		/// <param name="port"> local port to use </param>
		/// <param name="laddr"> local address to bind
		/// </param>
		/// <exception cref="SocketException">  if the socket could not be opened,
		///               or the socket could not bind to the specified local port. </exception>
		/// <exception cref="SecurityException">  if a security manager exists and its
		///             {@code checkListen} method doesn't allow the operation.
		/// </exception>
		/// <seealso cref= SecurityManager#checkListen
		/// @since   JDK1.1 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DatagramSocket(int port, InetAddress laddr) throws SocketException
		public DatagramSocket(int port, InetAddress laddr) : this(new InetSocketAddress(laddr, port))
		{
		}

		private void CheckOldImpl()
		{
			if (Impl_Renamed == null)
			{
				return;
			}
			// DatagramSocketImpl.peekdata() is a protected method, therefore we need to use
			// getDeclaredMethod, therefore we need permission to access the member
			try
			{
				AccessController.doPrivileged(new PrivilegedExceptionActionAnonymousInnerClassHelper(this));
			}
			catch (java.security.PrivilegedActionException)
			{
				OldImpl = true;
			}
		}

		private class PrivilegedExceptionActionAnonymousInnerClassHelper : PrivilegedExceptionAction<Void>
		{
			private readonly DatagramSocket OuterInstance;

			public PrivilegedExceptionActionAnonymousInnerClassHelper(DatagramSocket outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Void run() throws NoSuchMethodException
			public virtual Void Run()
			{
				Class[] cl = new Class[1];
				cl[0] = typeof(DatagramPacket);
				OuterInstance.Impl_Renamed.GetType().getDeclaredMethod("peekData", cl);
				return null;
			}
		}

		internal static Class ImplClass = null;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void createImpl() throws SocketException
		internal virtual void CreateImpl()
		{
			if (Impl_Renamed == null)
			{
				if (Factory != null)
				{
					Impl_Renamed = Factory.CreateDatagramSocketImpl();
					CheckOldImpl();
				}
				else
				{
					bool isMulticast = (this is MulticastSocket) ? true : false;
					Impl_Renamed = DefaultDatagramSocketImplFactory.CreateDatagramSocketImpl(isMulticast);

					CheckOldImpl();
				}
			}
			// creates a udp socket
			Impl_Renamed.Create();
			Impl_Renamed.DatagramSocket = this;
			Created = true;
		}

		/// <summary>
		/// Get the {@code DatagramSocketImpl} attached to this socket,
		/// creating it if necessary.
		/// </summary>
		/// <returns>  the {@code DatagramSocketImpl} attached to that
		///          DatagramSocket </returns>
		/// <exception cref="SocketException"> if creation fails.
		/// @since 1.4 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: DatagramSocketImpl getImpl() throws SocketException
		internal virtual DatagramSocketImpl Impl
		{
			get
			{
				if (!Created)
				{
					CreateImpl();
				}
				return Impl_Renamed;
			}
		}

		/// <summary>
		/// Binds this DatagramSocket to a specific address and port.
		/// <para>
		/// If the address is {@code null}, then the system will pick up
		/// an ephemeral port and a valid local address to bind the socket.
		/// </para>
		/// <para>
		/// </para>
		/// </summary>
		/// <param name="addr"> The address and port to bind to. </param>
		/// <exception cref="SocketException"> if any error happens during the bind, or if the
		///          socket is already bound. </exception>
		/// <exception cref="SecurityException">  if a security manager exists and its
		///             {@code checkListen} method doesn't allow the operation. </exception>
		/// <exception cref="IllegalArgumentException"> if addr is a SocketAddress subclass
		///         not supported by this socket.
		/// @since 1.4 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public synchronized void bind(SocketAddress addr) throws SocketException
		public virtual void Bind(SocketAddress addr)
		{
			lock (this)
			{
				if (Closed)
				{
					throw new SocketException("Socket is closed");
				}
				if (Bound)
				{
					throw new SocketException("already bound");
				}
				if (addr == null)
				{
					addr = new InetSocketAddress(0);
				}
				if (!(addr is InetSocketAddress))
				{
					throw new IllegalArgumentException("Unsupported address type!");
				}
				InetSocketAddress epoint = (InetSocketAddress) addr;
				if (epoint.Unresolved)
				{
					throw new SocketException("Unresolved address");
				}
				InetAddress iaddr = epoint.Address;
				int port = epoint.Port;
				CheckAddress(iaddr, "bind");
				SecurityManager sec = System.SecurityManager;
				if (sec != null)
				{
					sec.CheckListen(port);
				}
				try
				{
					Impl.Bind(port, iaddr);
				}
				catch (SocketException e)
				{
					Impl.Close();
					throw e;
				}
				Bound_Renamed = true;
			}
		}

		internal virtual void CheckAddress(InetAddress addr, String op)
		{
			if (addr == null)
			{
				return;
			}
			if (!(addr is Inet4Address || addr is Inet6Address))
			{
				throw new IllegalArgumentException(op + ": invalid address type");
			}
		}

		/// <summary>
		/// Connects the socket to a remote address for this socket. When a
		/// socket is connected to a remote address, packets may only be
		/// sent to or received from that address. By default a datagram
		/// socket is not connected.
		/// 
		/// <para>If the remote destination to which the socket is connected does not
		/// exist, or is otherwise unreachable, and if an ICMP destination unreachable
		/// packet has been received for that address, then a subsequent call to
		/// send or receive may throw a PortUnreachableException. Note, there is no
		/// guarantee that the exception will be thrown.
		/// 
		/// </para>
		/// <para> If a security manager has been installed then it is invoked to check
		/// access to the remote address. Specifically, if the given {@code address}
		/// is a <seealso cref="InetAddress#isMulticastAddress multicast address"/>,
		/// the security manager's {@link
		/// java.lang.SecurityManager#checkMulticast(InetAddress)
		/// checkMulticast} method is invoked with the given {@code address}.
		/// Otherwise, the security manager's {@link
		/// java.lang.SecurityManager#checkConnect(String,int) checkConnect}
		/// and <seealso cref="java.lang.SecurityManager#checkAccept checkAccept"/> methods
		/// are invoked, with the given {@code address} and {@code port}, to
		/// verify that datagrams are permitted to be sent and received
		/// respectively.
		/// 
		/// </para>
		/// <para> When a socket is connected, <seealso cref="#receive receive"/> and
		/// <seealso cref="#send send"/> <b>will not perform any security checks</b>
		/// on incoming and outgoing packets, other than matching the packet's
		/// and the socket's address and port. On a send operation, if the
		/// packet's address is set and the packet's address and the socket's
		/// address do not match, an {@code IllegalArgumentException} will be
		/// thrown. A socket connected to a multicast address may only be used
		/// to send packets.
		/// 
		/// </para>
		/// </summary>
		/// <param name="address"> the remote address for the socket
		/// </param>
		/// <param name="port"> the remote port for the socket.
		/// </param>
		/// <exception cref="IllegalArgumentException">
		///         if the address is null, or the port is out of range.
		/// </exception>
		/// <exception cref="SecurityException">
		///         if a security manager has been installed and it does
		///         not permit access to the given remote address
		/// </exception>
		/// <seealso cref= #disconnect </seealso>
		public virtual void Connect(InetAddress address, int port)
		{
			try
			{
				ConnectInternal(address, port);
			}
			catch (SocketException se)
			{
				throw new Error("connect failed", se);
			}
		}

		/// <summary>
		/// Connects this socket to a remote socket address (IP address + port number).
		/// 
		/// <para> If given an <seealso cref="InetSocketAddress InetSocketAddress"/>, this method
		/// behaves as if invoking <seealso cref="#connect(InetAddress,int) connect(InetAddress,int)"/>
		/// with the the given socket addresses IP address and port number.
		/// 
		/// </para>
		/// </summary>
		/// <param name="addr">    The remote address.
		/// </param>
		/// <exception cref="SocketException">
		///          if the connect fails
		/// </exception>
		/// <exception cref="IllegalArgumentException">
		///         if {@code addr} is {@code null}, or {@code addr} is a SocketAddress
		///         subclass not supported by this socket
		/// </exception>
		/// <exception cref="SecurityException">
		///         if a security manager has been installed and it does
		///         not permit access to the given remote address
		/// 
		/// @since 1.4 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void connect(SocketAddress addr) throws SocketException
		public virtual void Connect(SocketAddress addr)
		{
			if (addr == null)
			{
				throw new IllegalArgumentException("Address can't be null");
			}
			if (!(addr is InetSocketAddress))
			{
				throw new IllegalArgumentException("Unsupported address type");
			}
			InetSocketAddress epoint = (InetSocketAddress) addr;
			if (epoint.Unresolved)
			{
				throw new SocketException("Unresolved address");
			}
			ConnectInternal(epoint.Address, epoint.Port);
		}

		/// <summary>
		/// Disconnects the socket. If the socket is closed or not connected,
		/// then this method has no effect.
		/// </summary>
		/// <seealso cref= #connect </seealso>
		public virtual void Disconnect()
		{
			lock (this)
			{
				if (Closed)
				{
					return;
				}
				if (ConnectState == ST_CONNECTED)
				{
					Impl_Renamed.Disconnect();
				}
				ConnectedAddress = null;
				ConnectedPort = -1;
				ConnectState = ST_NOT_CONNECTED;
				ExplicitFilter = false;
			}
		}

		/// <summary>
		/// Returns the binding state of the socket.
		/// <para>
		/// If the socket was bound prior to being <seealso cref="#close closed"/>,
		/// then this method will continue to return {@code true}
		/// after the socket is closed.
		/// 
		/// </para>
		/// </summary>
		/// <returns> true if the socket successfully bound to an address
		/// @since 1.4 </returns>
		public virtual bool Bound
		{
			get
			{
				return Bound_Renamed;
			}
		}

		/// <summary>
		/// Returns the connection state of the socket.
		/// <para>
		/// If the socket was connected prior to being <seealso cref="#close closed"/>,
		/// then this method will continue to return {@code true}
		/// after the socket is closed.
		/// 
		/// </para>
		/// </summary>
		/// <returns> true if the socket successfully connected to a server
		/// @since 1.4 </returns>
		public virtual bool Connected
		{
			get
			{
				return ConnectState != ST_NOT_CONNECTED;
			}
		}

		/// <summary>
		/// Returns the address to which this socket is connected. Returns
		/// {@code null} if the socket is not connected.
		/// <para>
		/// If the socket was connected prior to being <seealso cref="#close closed"/>,
		/// then this method will continue to return the connected address
		/// after the socket is closed.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the address to which this socket is connected. </returns>
		public virtual InetAddress InetAddress
		{
			get
			{
				return ConnectedAddress;
			}
		}

		/// <summary>
		/// Returns the port number to which this socket is connected.
		/// Returns {@code -1} if the socket is not connected.
		/// <para>
		/// If the socket was connected prior to being <seealso cref="#close closed"/>,
		/// then this method will continue to return the connected port number
		/// after the socket is closed.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the port number to which this socket is connected. </returns>
		public virtual int Port
		{
			get
			{
				return ConnectedPort;
			}
		}

		/// <summary>
		/// Returns the address of the endpoint this socket is connected to, or
		/// {@code null} if it is unconnected.
		/// <para>
		/// If the socket was connected prior to being <seealso cref="#close closed"/>,
		/// then this method will continue to return the connected address
		/// after the socket is closed.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a {@code SocketAddress} representing the remote
		///         endpoint of this socket, or {@code null} if it is
		///         not connected yet. </returns>
		/// <seealso cref= #getInetAddress() </seealso>
		/// <seealso cref= #getPort() </seealso>
		/// <seealso cref= #connect(SocketAddress)
		/// @since 1.4 </seealso>
		public virtual SocketAddress RemoteSocketAddress
		{
			get
			{
				if (!Connected)
				{
					return null;
				}
				return new InetSocketAddress(InetAddress, Port);
			}
		}

		/// <summary>
		/// Returns the address of the endpoint this socket is bound to.
		/// </summary>
		/// <returns> a {@code SocketAddress} representing the local endpoint of this
		///         socket, or {@code null} if it is closed or not bound yet. </returns>
		/// <seealso cref= #getLocalAddress() </seealso>
		/// <seealso cref= #getLocalPort() </seealso>
		/// <seealso cref= #bind(SocketAddress)
		/// @since 1.4 </seealso>

		public virtual SocketAddress LocalSocketAddress
		{
			get
			{
				if (Closed)
				{
					return null;
				}
				if (!Bound)
				{
					return null;
				}
				return new InetSocketAddress(LocalAddress, LocalPort);
			}
		}

		/// <summary>
		/// Sends a datagram packet from this socket. The
		/// {@code DatagramPacket} includes information indicating the
		/// data to be sent, its length, the IP address of the remote host,
		/// and the port number on the remote host.
		/// 
		/// <para>If there is a security manager, and the socket is not currently
		/// connected to a remote address, this method first performs some
		/// security checks. First, if {@code p.getAddress().isMulticastAddress()}
		/// is true, this method calls the
		/// security manager's {@code checkMulticast} method
		/// with {@code p.getAddress()} as its argument.
		/// If the evaluation of that expression is false,
		/// this method instead calls the security manager's
		/// {@code checkConnect} method with arguments
		/// {@code p.getAddress().getHostAddress()} and
		/// {@code p.getPort()}. Each call to a security manager method
		/// could result in a SecurityException if the operation is not allowed.
		/// 
		/// </para>
		/// </summary>
		/// <param name="p">   the {@code DatagramPacket} to be sent.
		/// </param>
		/// <exception cref="IOException">  if an I/O error occurs. </exception>
		/// <exception cref="SecurityException">  if a security manager exists and its
		///             {@code checkMulticast} or {@code checkConnect}
		///             method doesn't allow the send. </exception>
		/// <exception cref="PortUnreachableException"> may be thrown if the socket is connected
		///             to a currently unreachable destination. Note, there is no
		///             guarantee that the exception will be thrown. </exception>
		/// <exception cref="java.nio.channels.IllegalBlockingModeException">
		///             if this socket has an associated channel,
		///             and the channel is in non-blocking mode. </exception>
		/// <exception cref="IllegalArgumentException"> if the socket is connected,
		///             and connected address and packet address differ.
		/// </exception>
		/// <seealso cref=        java.net.DatagramPacket </seealso>
		/// <seealso cref=        SecurityManager#checkMulticast(InetAddress) </seealso>
		/// <seealso cref=        SecurityManager#checkConnect
		/// @revised 1.4
		/// @spec JSR-51 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void send(DatagramPacket p) throws java.io.IOException
		public virtual void Send(DatagramPacket p)
		{
			InetAddress packetAddress = null;
			lock (p)
			{
				if (Closed)
				{
					throw new SocketException("Socket is closed");
				}
				CheckAddress(p.Address, "send");
				if (ConnectState == ST_NOT_CONNECTED)
				{
					// check the address is ok wiht the security manager on every send.
					SecurityManager security = System.SecurityManager;

					// The reason you want to synchronize on datagram packet
					// is because you don't want an applet to change the address
					// while you are trying to send the packet for example
					// after the security check but before the send.
					if (security != null)
					{
						if (p.Address.MulticastAddress)
						{
							security.CheckMulticast(p.Address);
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
					packetAddress = p.Address;
					if (packetAddress == null)
					{
						p.Address = ConnectedAddress;
						p.Port = ConnectedPort;
					}
					else if ((!packetAddress.Equals(ConnectedAddress)) || p.Port != ConnectedPort)
					{
						throw new IllegalArgumentException("connected address " + "and packet address" + " differ");
					}
				}
				// Check whether the socket is bound
				if (!Bound)
				{
					Bind(new InetSocketAddress(0));
				}
				// call the  method to send
				Impl.Send(p);
			}
		}

		/// <summary>
		/// Receives a datagram packet from this socket. When this method
		/// returns, the {@code DatagramPacket}'s buffer is filled with
		/// the data received. The datagram packet also contains the sender's
		/// IP address, and the port number on the sender's machine.
		/// <para>
		/// This method blocks until a datagram is received. The
		/// {@code length} field of the datagram packet object contains
		/// the length of the received message. If the message is longer than
		/// the packet's length, the message is truncated.
		/// </para>
		/// <para>
		/// If there is a security manager, a packet cannot be received if the
		/// security manager's {@code checkAccept} method
		/// does not allow it.
		/// 
		/// </para>
		/// </summary>
		/// <param name="p">   the {@code DatagramPacket} into which to place
		///                 the incoming data. </param>
		/// <exception cref="IOException">  if an I/O error occurs. </exception>
		/// <exception cref="SocketTimeoutException">  if setSoTimeout was previously called
		///                 and the timeout has expired. </exception>
		/// <exception cref="PortUnreachableException"> may be thrown if the socket is connected
		///             to a currently unreachable destination. Note, there is no guarantee that the
		///             exception will be thrown. </exception>
		/// <exception cref="java.nio.channels.IllegalBlockingModeException">
		///             if this socket has an associated channel,
		///             and the channel is in non-blocking mode. </exception>
		/// <seealso cref=        java.net.DatagramPacket </seealso>
		/// <seealso cref=        java.net.DatagramSocket
		/// @revised 1.4
		/// @spec JSR-51 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public synchronized void receive(DatagramPacket p) throws java.io.IOException
		public virtual void Receive(DatagramPacket p)
		{
			lock (this)
			{
				lock (p)
				{
					if (!Bound)
					{
						Bind(new InetSocketAddress(0));
					}
					if (ConnectState == ST_NOT_CONNECTED)
					{
						// check the address is ok with the security manager before every recv.
						SecurityManager security = System.SecurityManager;
						if (security != null)
						{
							while (true)
							{
								String peekAd = null;
								int peekPort = 0;
								// peek at the packet to see who it is from.
								if (!OldImpl)
								{
									// We can use the new peekData() API
									DatagramPacket peekPacket = new DatagramPacket(new sbyte[1], 1);
									peekPort = Impl.PeekData(peekPacket);
									peekAd = peekPacket.Address.HostAddress;
								}
								else
								{
									InetAddress adr = new InetAddress();
									peekPort = Impl.Peek(adr);
									peekAd = adr.HostAddress;
								}
								try
								{
									security.CheckAccept(peekAd, peekPort);
									// security check succeeded - so now break
									// and recv the packet.
									break;
								}
								catch (SecurityException)
								{
									// Throw away the offending packet by consuming
									// it in a tmp buffer.
									DatagramPacket tmp = new DatagramPacket(new sbyte[1], 1);
									Impl.Receive(tmp);
        
									// silently discard the offending packet
									// and continue: unknown/malicious
									// entities on nets should not make
									// runtime throw security exception and
									// disrupt the applet by sending random
									// datagram packets.
									continue;
								}
							} // end of while
						}
					}
					DatagramPacket tmp = null;
					if ((ConnectState == ST_CONNECTED_NO_IMPL) || ExplicitFilter)
					{
						// We have to do the filtering the old fashioned way since
						// the native impl doesn't support connect or the connect
						// via the impl failed, or .. "explicitFilter" may be set when
						// a socket is connected via the impl, for a period of time
						// when packets from other sources might be queued on socket.
						bool stop = false;
						while (!stop)
						{
							InetAddress peekAddress = null;
							int peekPort = -1;
							// peek at the packet to see who it is from.
							if (!OldImpl)
							{
								// We can use the new peekData() API
								DatagramPacket peekPacket = new DatagramPacket(new sbyte[1], 1);
								peekPort = Impl.PeekData(peekPacket);
								peekAddress = peekPacket.Address;
							}
							else
							{
								// this api only works for IPv4
								peekAddress = new InetAddress();
								peekPort = Impl.Peek(peekAddress);
							}
							if ((!ConnectedAddress.Equals(peekAddress)) || (ConnectedPort != peekPort))
							{
								// throw the packet away and silently continue
								tmp = new DatagramPacket(new sbyte[1024], 1024);
								Impl.Receive(tmp);
								if (ExplicitFilter)
								{
									if (CheckFiltering(tmp))
									{
										stop = true;
									}
								}
							}
							else
							{
								stop = true;
							}
						}
					}
					// If the security check succeeds, or the datagram is
					// connected then receive the packet
					Impl.Receive(p);
					if (ExplicitFilter && tmp == null)
					{
						// packet was not filtered, account for it here
						CheckFiltering(p);
					}
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private boolean checkFiltering(DatagramPacket p) throws SocketException
		private bool CheckFiltering(DatagramPacket p)
		{
			BytesLeftToFilter -= p.Length;
			if (BytesLeftToFilter <= 0 || Impl.DataAvailable() <= 0)
			{
				ExplicitFilter = false;
				return true;
			}
			return false;
		}

		/// <summary>
		/// Gets the local address to which the socket is bound.
		/// 
		/// <para>If there is a security manager, its
		/// {@code checkConnect} method is first called
		/// with the host address and {@code -1}
		/// as its arguments to see if the operation is allowed.
		/// 
		/// </para>
		/// </summary>
		/// <seealso cref= SecurityManager#checkConnect </seealso>
		/// <returns>  the local address to which the socket is bound,
		///          {@code null} if the socket is closed, or
		///          an {@code InetAddress} representing
		///          <seealso cref="InetAddress#isAnyLocalAddress wildcard"/>
		///          address if either the socket is not bound, or
		///          the security manager {@code checkConnect}
		///          method does not allow the operation
		/// @since   1.1 </returns>
		public virtual InetAddress LocalAddress
		{
			get
			{
				if (Closed)
				{
					return null;
				}
				InetAddress @in = null;
				try
				{
					@in = (InetAddress) Impl.GetOption(SocketOptions_Fields.SO_BINDADDR);
					if (@in.AnyLocalAddress)
					{
						@in = InetAddress.AnyLocalAddress();
					}
					SecurityManager s = System.SecurityManager;
					if (s != null)
					{
						s.CheckConnect(@in.HostAddress, -1);
					}
				}
				catch (Exception)
				{
					@in = InetAddress.AnyLocalAddress(); // "0.0.0.0"
				}
				return @in;
			}
		}

		/// <summary>
		/// Returns the port number on the local host to which this socket
		/// is bound.
		/// </summary>
		/// <returns>  the port number on the local host to which this socket is bound,
		///            {@code -1} if the socket is closed, or
		///            {@code 0} if it is not bound yet. </returns>
		public virtual int LocalPort
		{
			get
			{
				if (Closed)
				{
					return -1;
				}
				try
				{
					return Impl.LocalPort;
				}
				catch (Exception)
				{
					return 0;
				}
			}
		}

		/// <summary>
		/// Enable/disable SO_TIMEOUT with the specified timeout, in
		///  milliseconds. With this option set to a non-zero timeout,
		///  a call to receive() for this DatagramSocket
		///  will block for only this amount of time.  If the timeout expires,
		///  a <B>java.net.SocketTimeoutException</B> is raised, though the
		///  DatagramSocket is still valid.  The option <B>must</B> be enabled
		///  prior to entering the blocking operation to have effect.  The
		///  timeout must be {@code > 0}.
		///  A timeout of zero is interpreted as an infinite timeout.
		/// </summary>
		/// <param name="timeout"> the specified timeout in milliseconds. </param>
		/// <exception cref="SocketException"> if there is an error in the underlying protocol, such as an UDP error.
		/// @since   JDK1.1 </exception>
		/// <seealso cref= #getSoTimeout() </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public synchronized void setSoTimeout(int timeout) throws SocketException
		public virtual int SoTimeout
		{
			set
			{
				lock (this)
				{
					if (Closed)
					{
						throw new SocketException("Socket is closed");
					}
					Impl.SetOption(SocketOptions_Fields.SO_TIMEOUT, new Integer(value));
				}
			}
			get
			{
				lock (this)
				{
					if (Closed)
					{
						throw new SocketException("Socket is closed");
					}
					if (Impl == null)
					{
						return 0;
					}
					Object o = Impl.GetOption(SocketOptions_Fields.SO_TIMEOUT);
					/* extra type safety */
					if (o is Integer)
					{
						return ((Integer) o).IntValue();
					}
					else
					{
						return 0;
					}
				}
			}
		}


		/// <summary>
		/// Sets the SO_SNDBUF option to the specified value for this
		/// {@code DatagramSocket}. The SO_SNDBUF option is used by the
		/// network implementation as a hint to size the underlying
		/// network I/O buffers. The SO_SNDBUF setting may also be used
		/// by the network implementation to determine the maximum size
		/// of the packet that can be sent on this socket.
		/// <para>
		/// As SO_SNDBUF is a hint, applications that want to verify
		/// what size the buffer is should call <seealso cref="#getSendBufferSize()"/>.
		/// </para>
		/// <para>
		/// Increasing the buffer size may allow multiple outgoing packets
		/// to be queued by the network implementation when the send rate
		/// is high.
		/// </para>
		/// <para>
		/// Note: If <seealso cref="#send(DatagramPacket)"/> is used to send a
		/// {@code DatagramPacket} that is larger than the setting
		/// of SO_SNDBUF then it is implementation specific if the
		/// packet is sent or discarded.
		/// 
		/// </para>
		/// </summary>
		/// <param name="size"> the size to which to set the send buffer
		/// size. This value must be greater than 0.
		/// </param>
		/// <exception cref="SocketException"> if there is an error
		/// in the underlying protocol, such as an UDP error. </exception>
		/// <exception cref="IllegalArgumentException"> if the value is 0 or is
		/// negative. </exception>
		/// <seealso cref= #getSendBufferSize() </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public synchronized void setSendBufferSize(int size) throws SocketException
		public virtual int SendBufferSize
		{
			set
			{
				lock (this)
				{
					if (!(value > 0))
					{
						throw new IllegalArgumentException("negative send size");
					}
					if (Closed)
					{
						throw new SocketException("Socket is closed");
					}
					Impl.SetOption(SocketOptions_Fields.SO_SNDBUF, new Integer(value));
				}
			}
			get
			{
				lock (this)
				{
					if (Closed)
					{
						throw new SocketException("Socket is closed");
					}
					int result = 0;
					Object o = Impl.GetOption(SocketOptions_Fields.SO_SNDBUF);
					if (o is Integer)
					{
						result = ((Integer)o).IntValue();
					}
					return result;
				}
			}
		}


		/// <summary>
		/// Sets the SO_RCVBUF option to the specified value for this
		/// {@code DatagramSocket}. The SO_RCVBUF option is used by the
		/// the network implementation as a hint to size the underlying
		/// network I/O buffers. The SO_RCVBUF setting may also be used
		/// by the network implementation to determine the maximum size
		/// of the packet that can be received on this socket.
		/// <para>
		/// Because SO_RCVBUF is a hint, applications that want to
		/// verify what size the buffers were set to should call
		/// <seealso cref="#getReceiveBufferSize()"/>.
		/// </para>
		/// <para>
		/// Increasing SO_RCVBUF may allow the network implementation
		/// to buffer multiple packets when packets arrive faster than
		/// are being received using <seealso cref="#receive(DatagramPacket)"/>.
		/// </para>
		/// <para>
		/// Note: It is implementation specific if a packet larger
		/// than SO_RCVBUF can be received.
		/// 
		/// </para>
		/// </summary>
		/// <param name="size"> the size to which to set the receive buffer
		/// size. This value must be greater than 0.
		/// </param>
		/// <exception cref="SocketException"> if there is an error in
		/// the underlying protocol, such as an UDP error. </exception>
		/// <exception cref="IllegalArgumentException"> if the value is 0 or is
		/// negative. </exception>
		/// <seealso cref= #getReceiveBufferSize() </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public synchronized void setReceiveBufferSize(int size) throws SocketException
		public virtual int ReceiveBufferSize
		{
			set
			{
				lock (this)
				{
					if (value <= 0)
					{
						throw new IllegalArgumentException("invalid receive size");
					}
					if (Closed)
					{
						throw new SocketException("Socket is closed");
					}
					Impl.SetOption(SocketOptions_Fields.SO_RCVBUF, new Integer(value));
				}
			}
			get
			{
				lock (this)
				{
					if (Closed)
					{
						throw new SocketException("Socket is closed");
					}
					int result = 0;
					Object o = Impl.GetOption(SocketOptions_Fields.SO_RCVBUF);
					if (o is Integer)
					{
						result = ((Integer)o).IntValue();
					}
					return result;
				}
			}
		}


		/// <summary>
		/// Enable/disable the SO_REUSEADDR socket option.
		/// <para>
		/// For UDP sockets it may be necessary to bind more than one
		/// socket to the same socket address. This is typically for the
		/// purpose of receiving multicast packets
		/// (See <seealso cref="java.net.MulticastSocket"/>). The
		/// {@code SO_REUSEADDR} socket option allows multiple
		/// sockets to be bound to the same socket address if the
		/// {@code SO_REUSEADDR} socket option is enabled prior
		/// to binding the socket using <seealso cref="#bind(SocketAddress)"/>.
		/// </para>
		/// <para>
		/// Note: This functionality is not supported by all existing platforms,
		/// so it is implementation specific whether this option will be ignored
		/// or not. However, if it is not supported then
		/// <seealso cref="#getReuseAddress()"/> will always return {@code false}.
		/// </para>
		/// <para>
		/// When a {@code DatagramSocket} is created the initial setting
		/// of {@code SO_REUSEADDR} is disabled.
		/// </para>
		/// <para>
		/// The behaviour when {@code SO_REUSEADDR} is enabled or
		/// disabled after a socket is bound (See <seealso cref="#isBound()"/>)
		/// is not defined.
		/// 
		/// </para>
		/// </summary>
		/// <param name="on">  whether to enable or disable the </param>
		/// <exception cref="SocketException"> if an error occurs enabling or
		///            disabling the {@code SO_RESUEADDR} socket option,
		///            or the socket is closed.
		/// @since 1.4 </exception>
		/// <seealso cref= #getReuseAddress() </seealso>
		/// <seealso cref= #bind(SocketAddress) </seealso>
		/// <seealso cref= #isBound() </seealso>
		/// <seealso cref= #isClosed() </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public synchronized void setReuseAddress(boolean on) throws SocketException
		public virtual bool ReuseAddress
		{
			set
			{
				lock (this)
				{
					if (Closed)
					{
						throw new SocketException("Socket is closed");
					}
					// Integer instead of Boolean for compatibility with older DatagramSocketImpl
					if (OldImpl)
					{
						Impl.SetOption(SocketOptions_Fields.SO_REUSEADDR, new Integer(value? - 1:0));
					}
					else
					{
						Impl.SetOption(SocketOptions_Fields.SO_REUSEADDR, Convert.ToBoolean(value));
					}
				}
			}
			get
			{
				lock (this)
				{
					if (Closed)
					{
						throw new SocketException("Socket is closed");
					}
					Object o = Impl.GetOption(SocketOptions_Fields.SO_REUSEADDR);
					return ((Boolean)o).BooleanValue();
				}
			}
		}


		/// <summary>
		/// Enable/disable SO_BROADCAST.
		/// 
		/// <para> Some operating systems may require that the Java virtual machine be
		/// started with implementation specific privileges to enable this option or
		/// send broadcast datagrams.
		/// 
		/// </para>
		/// </summary>
		/// <param name="on">
		///         whether or not to have broadcast turned on.
		/// </param>
		/// <exception cref="SocketException">
		///          if there is an error in the underlying protocol, such as an UDP
		///          error.
		/// 
		/// @since 1.4 </exception>
		/// <seealso cref= #getBroadcast() </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public synchronized void setBroadcast(boolean on) throws SocketException
		public virtual bool Broadcast
		{
			set
			{
				lock (this)
				{
					if (Closed)
					{
						throw new SocketException("Socket is closed");
					}
					Impl.SetOption(SocketOptions_Fields.SO_BROADCAST, Convert.ToBoolean(value));
				}
			}
			get
			{
				lock (this)
				{
					if (Closed)
					{
						throw new SocketException("Socket is closed");
					}
					return ((Boolean)(Impl.GetOption(SocketOptions_Fields.SO_BROADCAST))).BooleanValue();
				}
			}
		}


		/// <summary>
		/// Sets traffic class or type-of-service octet in the IP
		/// datagram header for datagrams sent from this DatagramSocket.
		/// As the underlying network implementation may ignore this
		/// value applications should consider it a hint.
		/// 
		/// <P> The tc <B>must</B> be in the range {@code 0 <= tc <=
		/// 255} or an IllegalArgumentException will be thrown.
		/// <para>Notes:
		/// </para>
		/// <para>For Internet Protocol v4 the value consists of an
		/// {@code integer}, the least significant 8 bits of which
		/// represent the value of the TOS octet in IP packets sent by
		/// the socket.
		/// RFC 1349 defines the TOS values as follows:
		/// 
		/// <UL>
		/// <LI><CODE>IPTOS_LOWCOST (0x02)</CODE></LI>
		/// <LI><CODE>IPTOS_RELIABILITY (0x04)</CODE></LI>
		/// <LI><CODE>IPTOS_THROUGHPUT (0x08)</CODE></LI>
		/// <LI><CODE>IPTOS_LOWDELAY (0x10)</CODE></LI>
		/// </UL>
		/// The last low order bit is always ignored as this
		/// corresponds to the MBZ (must be zero) bit.
		/// </para>
		/// <para>
		/// Setting bits in the precedence field may result in a
		/// SocketException indicating that the operation is not
		/// permitted.
		/// </para>
		/// <para>
		/// for Internet Protocol v6 {@code tc} is the value that
		/// would be placed into the sin6_flowinfo field of the IP header.
		/// 
		/// </para>
		/// </summary>
		/// <param name="tc">        an {@code int} value for the bitset. </param>
		/// <exception cref="SocketException"> if there is an error setting the
		/// traffic class or type-of-service
		/// @since 1.4 </exception>
		/// <seealso cref= #getTrafficClass </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public synchronized void setTrafficClass(int tc) throws SocketException
		public virtual int TrafficClass
		{
			set
			{
				lock (this)
				{
					if (value < 0 || value > 255)
					{
						throw new IllegalArgumentException("tc is not in range 0 -- 255");
					}
            
					if (Closed)
					{
						throw new SocketException("Socket is closed");
					}
					try
					{
						Impl.SetOption(SocketOptions_Fields.IP_TOS, value);
					}
					catch (SocketException se)
					{
						// not supported if socket already connected
						// Solaris returns error in such cases
						if (!Connected)
						{
							throw se;
						}
					}
				}
			}
			get
			{
				lock (this)
				{
					if (Closed)
					{
						throw new SocketException("Socket is closed");
					}
					return ((Integer)(Impl.GetOption(SocketOptions_Fields.IP_TOS))).IntValue();
				}
			}
		}


		/// <summary>
		/// Closes this datagram socket.
		/// <para>
		/// Any thread currently blocked in <seealso cref="#receive"/> upon this socket
		/// will throw a <seealso cref="SocketException"/>.
		/// 
		/// </para>
		/// <para> If this socket has an associated channel then the channel is closed
		/// as well.
		/// 
		/// @revised 1.4
		/// @spec JSR-51
		/// </para>
		/// </summary>
		public virtual void Close()
		{
			lock (CloseLock)
			{
				if (Closed)
				{
					return;
				}
				Impl_Renamed.Close();
				Closed_Renamed = true;
			}
		}

		/// <summary>
		/// Returns whether the socket is closed or not.
		/// </summary>
		/// <returns> true if the socket has been closed
		/// @since 1.4 </returns>
		public virtual bool Closed
		{
			get
			{
				lock (CloseLock)
				{
					return Closed_Renamed;
				}
			}
		}

		/// <summary>
		/// Returns the unique <seealso cref="java.nio.channels.DatagramChannel"/> object
		/// associated with this datagram socket, if any.
		/// 
		/// <para> A datagram socket will have a channel if, and only if, the channel
		/// itself was created via the {@link java.nio.channels.DatagramChannel#open
		/// DatagramChannel.open} method.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  the datagram channel associated with this datagram socket,
		///          or {@code null} if this socket was not created for a channel
		/// 
		/// @since 1.4
		/// @spec JSR-51 </returns>
		public virtual DatagramChannel Channel
		{
			get
			{
				return null;
			}
		}

		/// <summary>
		/// User defined factory for all datagram sockets.
		/// </summary>
		internal static DatagramSocketImplFactory Factory;

		/// <summary>
		/// Sets the datagram socket implementation factory for the
		/// application. The factory can be specified only once.
		/// <para>
		/// When an application creates a new datagram socket, the socket
		/// implementation factory's {@code createDatagramSocketImpl} method is
		/// called to create the actual datagram socket implementation.
		/// </para>
		/// <para>
		/// Passing {@code null} to the method is a no-op unless the factory
		/// was already set.
		/// 
		/// </para>
		/// <para>If there is a security manager, this method first calls
		/// the security manager's {@code checkSetFactory} method
		/// to ensure the operation is allowed.
		/// This could result in a SecurityException.
		/// 
		/// </para>
		/// </summary>
		/// <param name="fac">   the desired factory. </param>
		/// <exception cref="IOException">  if an I/O error occurs when setting the
		///              datagram socket factory. </exception>
		/// <exception cref="SocketException">  if the factory is already defined. </exception>
		/// <exception cref="SecurityException">  if a security manager exists and its
		///             {@code checkSetFactory} method doesn't allow the
		/// operation.
		/// @see
		/// java.net.DatagramSocketImplFactory#createDatagramSocketImpl() </exception>
		/// <seealso cref=       SecurityManager#checkSetFactory
		/// @since 1.3 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static synchronized void setDatagramSocketImplFactory(DatagramSocketImplFactory fac) throws java.io.IOException
		public static DatagramSocketImplFactory DatagramSocketImplFactory
		{
			set
			{
				lock (typeof(DatagramSocket))
				{
					if (Factory != null)
					{
						throw new SocketException("factory already defined");
					}
					SecurityManager security = System.SecurityManager;
					if (security != null)
					{
						security.CheckSetFactory();
					}
					Factory = value;
				}
			}
		}
	}

}