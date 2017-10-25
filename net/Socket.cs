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
	/// This class implements client sockets (also called just
	/// "sockets"). A socket is an endpoint for communication
	/// between two machines.
	/// <para>
	/// The actual work of the socket is performed by an instance of the
	/// {@code SocketImpl} class. An application, by changing
	/// the socket factory that creates the socket implementation,
	/// can configure itself to create sockets appropriate to the local
	/// firewall.
	/// 
	/// @author  unascribed
	/// </para>
	/// </summary>
	/// <seealso cref=     java.net.Socket#setSocketImplFactory(java.net.SocketImplFactory) </seealso>
	/// <seealso cref=     java.net.SocketImpl </seealso>
	/// <seealso cref=     java.nio.channels.SocketChannel
	/// @since   JDK1.0 </seealso>
	public class Socket : java.io.Closeable
	{
		/// <summary>
		/// Various states of this socket.
		/// </summary>
		private bool Created = false;
		private bool Bound_Renamed = false;
		private bool Connected_Renamed = false;
		private bool Closed_Renamed = false;
		private Object CloseLock = new Object();
		private bool ShutIn = false;
		private bool ShutOut = false;

		/// <summary>
		/// The implementation of this Socket.
		/// </summary>
		internal SocketImpl Impl_Renamed;

		/// <summary>
		/// Are we using an older SocketImpl?
		/// </summary>
		private bool OldImpl = false;

		/// <summary>
		/// Creates an unconnected socket, with the
		/// system-default type of SocketImpl.
		/// 
		/// @since   JDK1.1
		/// @revised 1.4
		/// </summary>
		public Socket()
		{
			SetImpl();
		}

		/// <summary>
		/// Creates an unconnected socket, specifying the type of proxy, if any,
		/// that should be used regardless of any other settings.
		/// <P>
		/// If there is a security manager, its {@code checkConnect} method
		/// is called with the proxy host address and port number
		/// as its arguments. This could result in a SecurityException.
		/// <P>
		/// Examples:
		/// <UL> <LI>{@code Socket s = new Socket(Proxy.NO_PROXY);} will create
		/// a plain socket ignoring any other proxy configuration.</LI>
		/// <LI>{@code Socket s = new Socket(new Proxy(Proxy.Type.SOCKS, new InetSocketAddress("socks.mydom.com", 1080)));}
		/// will create a socket connecting through the specified SOCKS proxy
		/// server.</LI>
		/// </UL>
		/// </summary>
		/// <param name="proxy"> a <seealso cref="java.net.Proxy Proxy"/> object specifying what kind
		///              of proxying should be used. </param>
		/// <exception cref="IllegalArgumentException"> if the proxy is of an invalid type
		///          or {@code null}. </exception>
		/// <exception cref="SecurityException"> if a security manager is present and
		///                           permission to connect to the proxy is
		///                           denied. </exception>
		/// <seealso cref= java.net.ProxySelector </seealso>
		/// <seealso cref= java.net.Proxy
		/// 
		/// @since   1.5 </seealso>
		public Socket(Proxy proxy)
		{
			// Create a copy of Proxy as a security measure
			if (proxy == null)
			{
				throw new IllegalArgumentException("Invalid Proxy");
			}
			Proxy p = proxy == Proxy.NO_PROXY ? Proxy.NO_PROXY : sun.net.ApplicationProxy.create(proxy);
			Proxy.Type type = p.Type();
			if (type == Proxy.Type.SOCKS || type == Proxy.Type.HTTP)
			{
				SecurityManager security = System.SecurityManager;
				InetSocketAddress epoint = (InetSocketAddress) p.Address();
				if (epoint.Address != null)
				{
					CheckAddress(epoint.Address, "Socket");
				}
				if (security != null)
				{
					if (epoint.Unresolved)
					{
						epoint = new InetSocketAddress(epoint.HostName, epoint.Port);
					}
					if (epoint.Unresolved)
					{
						security.CheckConnect(epoint.HostName, epoint.Port);
					}
					else
					{
						security.CheckConnect(epoint.Address.HostAddress, epoint.Port);
					}
				}
				Impl_Renamed = type == Proxy.Type.SOCKS ? new SocksSocketImpl(p) : new HttpConnectSocketImpl(p);
				Impl_Renamed.Socket = this;
			}
			else
			{
				if (p == Proxy.NO_PROXY)
				{
					if (Factory == null)
					{
						Impl_Renamed = new PlainSocketImpl();
						Impl_Renamed.Socket = this;
					}
					else
					{
						SetImpl();
					}
				}
				else
				{
					throw new IllegalArgumentException("Invalid Proxy");
				}
			}
		}

		/// <summary>
		/// Creates an unconnected Socket with a user-specified
		/// SocketImpl.
		/// <P> </summary>
		/// <param name="impl"> an instance of a <B>SocketImpl</B>
		/// the subclass wishes to use on the Socket.
		/// </param>
		/// <exception cref="SocketException"> if there is an error in the underlying protocol,
		/// such as a TCP error.
		/// @since   JDK1.1 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected Socket(SocketImpl impl) throws SocketException
		protected internal Socket(SocketImpl impl)
		{
			this.Impl_Renamed = impl;
			if (impl != null)
			{
				CheckOldImpl();
				this.Impl_Renamed.Socket = this;
			}
		}

		/// <summary>
		/// Creates a stream socket and connects it to the specified port
		/// number on the named host.
		/// <para>
		/// If the specified host is {@code null} it is the equivalent of
		/// specifying the address as
		/// <seealso cref="java.net.InetAddress#getByName InetAddress.getByName"/>{@code (null)}.
		/// In other words, it is equivalent to specifying an address of the
		/// loopback interface. </para>
		/// <para>
		/// If the application has specified a server socket factory, that
		/// factory's {@code createSocketImpl} method is called to create
		/// the actual socket implementation. Otherwise a "plain" socket is created.
		/// </para>
		/// <para>
		/// If there is a security manager, its
		/// {@code checkConnect} method is called
		/// with the host address and {@code port}
		/// as its arguments. This could result in a SecurityException.
		/// 
		/// </para>
		/// </summary>
		/// <param name="host">   the host name, or {@code null} for the loopback address. </param>
		/// <param name="port">   the port number.
		/// </param>
		/// <exception cref="UnknownHostException"> if the IP address of
		/// the host could not be determined.
		/// </exception>
		/// <exception cref="IOException">  if an I/O error occurs when creating the socket. </exception>
		/// <exception cref="SecurityException">  if a security manager exists and its
		///             {@code checkConnect} method doesn't allow the operation. </exception>
		/// <exception cref="IllegalArgumentException"> if the port parameter is outside
		///             the specified range of valid port values, which is between
		///             0 and 65535, inclusive. </exception>
		/// <seealso cref=        java.net.Socket#setSocketImplFactory(java.net.SocketImplFactory) </seealso>
		/// <seealso cref=        java.net.SocketImpl </seealso>
		/// <seealso cref=        java.net.SocketImplFactory#createSocketImpl() </seealso>
		/// <seealso cref=        SecurityManager#checkConnect </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Socket(String host, int port) throws UnknownHostException, java.io.IOException
		public Socket(String host, int port) : this(host != null ? new InetSocketAddress(host, port) : new InetSocketAddress(InetAddress.GetByName(null), port), (SocketAddress) null, true)
		{
		}

		/// <summary>
		/// Creates a stream socket and connects it to the specified port
		/// number at the specified IP address.
		/// <para>
		/// If the application has specified a socket factory, that factory's
		/// {@code createSocketImpl} method is called to create the
		/// actual socket implementation. Otherwise a "plain" socket is created.
		/// </para>
		/// <para>
		/// If there is a security manager, its
		/// {@code checkConnect} method is called
		/// with the host address and {@code port}
		/// as its arguments. This could result in a SecurityException.
		/// 
		/// </para>
		/// </summary>
		/// <param name="address">   the IP address. </param>
		/// <param name="port">      the port number. </param>
		/// <exception cref="IOException">  if an I/O error occurs when creating the socket. </exception>
		/// <exception cref="SecurityException">  if a security manager exists and its
		///             {@code checkConnect} method doesn't allow the operation. </exception>
		/// <exception cref="IllegalArgumentException"> if the port parameter is outside
		///             the specified range of valid port values, which is between
		///             0 and 65535, inclusive. </exception>
		/// <exception cref="NullPointerException"> if {@code address} is null. </exception>
		/// <seealso cref=        java.net.Socket#setSocketImplFactory(java.net.SocketImplFactory) </seealso>
		/// <seealso cref=        java.net.SocketImpl </seealso>
		/// <seealso cref=        java.net.SocketImplFactory#createSocketImpl() </seealso>
		/// <seealso cref=        SecurityManager#checkConnect </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Socket(InetAddress address, int port) throws java.io.IOException
		public Socket(InetAddress address, int port) : this(address != null ? new InetSocketAddress(address, port) : null, (SocketAddress) null, true)
		{
		}

		/// <summary>
		/// Creates a socket and connects it to the specified remote host on
		/// the specified remote port. The Socket will also bind() to the local
		/// address and port supplied.
		/// <para>
		/// If the specified host is {@code null} it is the equivalent of
		/// specifying the address as
		/// <seealso cref="java.net.InetAddress#getByName InetAddress.getByName"/>{@code (null)}.
		/// In other words, it is equivalent to specifying an address of the
		/// loopback interface. </para>
		/// <para>
		/// A local port number of {@code zero} will let the system pick up a
		/// free port in the {@code bind} operation.</para>
		/// <para>
		/// If there is a security manager, its
		/// {@code checkConnect} method is called
		/// with the host address and {@code port}
		/// as its arguments. This could result in a SecurityException.
		/// 
		/// </para>
		/// </summary>
		/// <param name="host"> the name of the remote host, or {@code null} for the loopback address. </param>
		/// <param name="port"> the remote port </param>
		/// <param name="localAddr"> the local address the socket is bound to, or
		///        {@code null} for the {@code anyLocal} address. </param>
		/// <param name="localPort"> the local port the socket is bound to, or
		///        {@code zero} for a system selected free port. </param>
		/// <exception cref="IOException">  if an I/O error occurs when creating the socket. </exception>
		/// <exception cref="SecurityException">  if a security manager exists and its
		///             {@code checkConnect} method doesn't allow the connection
		///             to the destination, or if its {@code checkListen} method
		///             doesn't allow the bind to the local port. </exception>
		/// <exception cref="IllegalArgumentException"> if the port parameter or localPort
		///             parameter is outside the specified range of valid port values,
		///             which is between 0 and 65535, inclusive. </exception>
		/// <seealso cref=        SecurityManager#checkConnect
		/// @since   JDK1.1 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Socket(String host, int port, InetAddress localAddr, int localPort) throws java.io.IOException
		public Socket(String host, int port, InetAddress localAddr, int localPort) : this(host != null ? new InetSocketAddress(host, port) : new InetSocketAddress(InetAddress.GetByName(null), port), new InetSocketAddress(localAddr, localPort), true)
		{
		}

		/// <summary>
		/// Creates a socket and connects it to the specified remote address on
		/// the specified remote port. The Socket will also bind() to the local
		/// address and port supplied.
		/// <para>
		/// If the specified local address is {@code null} it is the equivalent of
		/// specifying the address as the AnyLocal address
		/// (see <seealso cref="java.net.InetAddress#isAnyLocalAddress InetAddress.isAnyLocalAddress"/>{@code ()}).
		/// </para>
		/// <para>
		/// A local port number of {@code zero} will let the system pick up a
		/// free port in the {@code bind} operation.</para>
		/// <para>
		/// If there is a security manager, its
		/// {@code checkConnect} method is called
		/// with the host address and {@code port}
		/// as its arguments. This could result in a SecurityException.
		/// 
		/// </para>
		/// </summary>
		/// <param name="address"> the remote address </param>
		/// <param name="port"> the remote port </param>
		/// <param name="localAddr"> the local address the socket is bound to, or
		///        {@code null} for the {@code anyLocal} address. </param>
		/// <param name="localPort"> the local port the socket is bound to or
		///        {@code zero} for a system selected free port. </param>
		/// <exception cref="IOException">  if an I/O error occurs when creating the socket. </exception>
		/// <exception cref="SecurityException">  if a security manager exists and its
		///             {@code checkConnect} method doesn't allow the connection
		///             to the destination, or if its {@code checkListen} method
		///             doesn't allow the bind to the local port. </exception>
		/// <exception cref="IllegalArgumentException"> if the port parameter or localPort
		///             parameter is outside the specified range of valid port values,
		///             which is between 0 and 65535, inclusive. </exception>
		/// <exception cref="NullPointerException"> if {@code address} is null. </exception>
		/// <seealso cref=        SecurityManager#checkConnect
		/// @since   JDK1.1 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Socket(InetAddress address, int port, InetAddress localAddr, int localPort) throws java.io.IOException
		public Socket(InetAddress address, int port, InetAddress localAddr, int localPort) : this(address != null ? new InetSocketAddress(address, port) : null, new InetSocketAddress(localAddr, localPort), true)
		{
		}

		/// <summary>
		/// Creates a stream socket and connects it to the specified port
		/// number on the named host.
		/// <para>
		/// If the specified host is {@code null} it is the equivalent of
		/// specifying the address as
		/// <seealso cref="java.net.InetAddress#getByName InetAddress.getByName"/>{@code (null)}.
		/// In other words, it is equivalent to specifying an address of the
		/// loopback interface. </para>
		/// <para>
		/// If the stream argument is {@code true}, this creates a
		/// stream socket. If the stream argument is {@code false}, it
		/// creates a datagram socket.
		/// </para>
		/// <para>
		/// If the application has specified a server socket factory, that
		/// factory's {@code createSocketImpl} method is called to create
		/// the actual socket implementation. Otherwise a "plain" socket is created.
		/// </para>
		/// <para>
		/// If there is a security manager, its
		/// {@code checkConnect} method is called
		/// with the host address and {@code port}
		/// as its arguments. This could result in a SecurityException.
		/// </para>
		/// <para>
		/// If a UDP socket is used, TCP/IP related socket options will not apply.
		/// 
		/// </para>
		/// </summary>
		/// <param name="host">     the host name, or {@code null} for the loopback address. </param>
		/// <param name="port">     the port number. </param>
		/// <param name="stream">   a {@code boolean} indicating whether this is
		///                      a stream socket or a datagram socket. </param>
		/// <exception cref="IOException">  if an I/O error occurs when creating the socket. </exception>
		/// <exception cref="SecurityException">  if a security manager exists and its
		///             {@code checkConnect} method doesn't allow the operation. </exception>
		/// <exception cref="IllegalArgumentException"> if the port parameter is outside
		///             the specified range of valid port values, which is between
		///             0 and 65535, inclusive. </exception>
		/// <seealso cref=        java.net.Socket#setSocketImplFactory(java.net.SocketImplFactory) </seealso>
		/// <seealso cref=        java.net.SocketImpl </seealso>
		/// <seealso cref=        java.net.SocketImplFactory#createSocketImpl() </seealso>
		/// <seealso cref=        SecurityManager#checkConnect </seealso>
		/// @deprecated Use DatagramSocket instead for UDP transport. 
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deprecated("Use DatagramSocket instead for UDP transport.") public Socket(String host, int port, boolean stream) throws java.io.IOException
		[Obsolete("Use DatagramSocket instead for UDP transport.")]
		public Socket(String host, int port, bool stream) : this(host != null ? new InetSocketAddress(host, port) : new InetSocketAddress(InetAddress.GetByName(null), port), (SocketAddress) null, stream)
		{
		}

		/// <summary>
		/// Creates a socket and connects it to the specified port number at
		/// the specified IP address.
		/// <para>
		/// If the stream argument is {@code true}, this creates a
		/// stream socket. If the stream argument is {@code false}, it
		/// creates a datagram socket.
		/// </para>
		/// <para>
		/// If the application has specified a server socket factory, that
		/// factory's {@code createSocketImpl} method is called to create
		/// the actual socket implementation. Otherwise a "plain" socket is created.
		/// 
		/// </para>
		/// <para>If there is a security manager, its
		/// {@code checkConnect} method is called
		/// with {@code host.getHostAddress()} and {@code port}
		/// as its arguments. This could result in a SecurityException.
		/// </para>
		/// <para>
		/// If UDP socket is used, TCP/IP related socket options will not apply.
		/// 
		/// </para>
		/// </summary>
		/// <param name="host">     the IP address. </param>
		/// <param name="port">      the port number. </param>
		/// <param name="stream">    if {@code true}, create a stream socket;
		///                       otherwise, create a datagram socket. </param>
		/// <exception cref="IOException">  if an I/O error occurs when creating the socket. </exception>
		/// <exception cref="SecurityException">  if a security manager exists and its
		///             {@code checkConnect} method doesn't allow the operation. </exception>
		/// <exception cref="IllegalArgumentException"> if the port parameter is outside
		///             the specified range of valid port values, which is between
		///             0 and 65535, inclusive. </exception>
		/// <exception cref="NullPointerException"> if {@code host} is null. </exception>
		/// <seealso cref=        java.net.Socket#setSocketImplFactory(java.net.SocketImplFactory) </seealso>
		/// <seealso cref=        java.net.SocketImpl </seealso>
		/// <seealso cref=        java.net.SocketImplFactory#createSocketImpl() </seealso>
		/// <seealso cref=        SecurityManager#checkConnect </seealso>
		/// @deprecated Use DatagramSocket instead for UDP transport. 
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deprecated("Use DatagramSocket instead for UDP transport.") public Socket(InetAddress host, int port, boolean stream) throws java.io.IOException
		[Obsolete("Use DatagramSocket instead for UDP transport.")]
		public Socket(InetAddress host, int port, bool stream) : this(host != null ? new InetSocketAddress(host, port) : null, new InetSocketAddress(0), stream)
		{
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private Socket(SocketAddress address, SocketAddress localAddr, boolean stream) throws java.io.IOException
		private Socket(SocketAddress address, SocketAddress localAddr, bool stream)
		{
			SetImpl();

			// backward compatibility
			if (address == null)
			{
				throw new NullPointerException();
			}

			try
			{
				CreateImpl(stream);
				if (localAddr != null)
				{
					Bind(localAddr);
				}
				Connect(address);
			}
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java 'multi-catch' syntax:
			catch (IOException | IllegalArgumentException | SecurityException e)
			{
				try
				{
					Close();
				}
				catch (IOException ce)
				{
					e.addSuppressed(ce);
				}
				throw e;
			}
		}

		/// <summary>
		/// Creates the socket implementation.
		/// </summary>
		/// <param name="stream"> a {@code boolean} value : {@code true} for a TCP socket,
		///               {@code false} for UDP. </param>
		/// <exception cref="IOException"> if creation fails
		/// @since 1.4 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void createImpl(boolean stream) throws SocketException
		 internal virtual void CreateImpl(bool stream)
		 {
			if (Impl_Renamed == null)
			{
				SetImpl();
			}
			try
			{
				Impl_Renamed.Create(stream);
				Created = true;
			}
			catch (IOException e)
			{
				throw new SocketException(e.Message);
			}
		 }

		private void CheckOldImpl()
		{
			if (Impl_Renamed == null)
			{
				return;
			}
			// SocketImpl.connect() is a protected method, therefore we need to use
			// getDeclaredMethod, therefore we need permission to access the member

			OldImpl = AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper(this));
		}

		private class PrivilegedActionAnonymousInnerClassHelper : PrivilegedAction<Boolean>
		{
			private readonly Socket OuterInstance;

			public PrivilegedActionAnonymousInnerClassHelper(Socket outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			public virtual Boolean Run()
			{
				Class clazz = OuterInstance.Impl_Renamed.GetType();
				while (true)
				{
					try
					{
						clazz.getDeclaredMethod("connect", typeof(SocketAddress), typeof(int));
						return false;
					}
					catch (NoSuchMethodException)
					{
						clazz = clazz.BaseType;
						// java.net.SocketImpl class will always have this abstract method.
						// If we have not found it by now in the hierarchy then it does not
						// exist, we are an old style impl.
						if (clazz.Equals(typeof(java.net.SocketImpl)))
						{
							return true;
						}
					}
				}
			}
		}

		/// <summary>
		/// Sets impl to the system-default type of SocketImpl.
		/// @since 1.4
		/// </summary>
		internal virtual void SetImpl()
		{
			if (Factory != null)
			{
				Impl_Renamed = Factory.CreateSocketImpl();
				CheckOldImpl();
			}
			else
			{
				// No need to do a checkOldImpl() here, we know it's an up to date
				// SocketImpl!
				Impl_Renamed = new SocksSocketImpl();
			}
			if (Impl_Renamed != null)
			{
				Impl_Renamed.Socket = this;
			}
		}


		/// <summary>
		/// Get the {@code SocketImpl} attached to this socket, creating
		/// it if necessary.
		/// </summary>
		/// <returns>  the {@code SocketImpl} attached to that ServerSocket. </returns>
		/// <exception cref="SocketException"> if creation fails
		/// @since 1.4 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: SocketImpl getImpl() throws SocketException
		internal virtual SocketImpl Impl
		{
			get
			{
				if (!Created)
				{
					CreateImpl(true);
				}
				return Impl_Renamed;
			}
		}

		/// <summary>
		/// Connects this socket to the server.
		/// </summary>
		/// <param name="endpoint"> the {@code SocketAddress} </param>
		/// <exception cref="IOException"> if an error occurs during the connection </exception>
		/// <exception cref="java.nio.channels.IllegalBlockingModeException">
		///          if this socket has an associated channel,
		///          and the channel is in non-blocking mode </exception>
		/// <exception cref="IllegalArgumentException"> if endpoint is null or is a
		///          SocketAddress subclass not supported by this socket
		/// @since 1.4
		/// @spec JSR-51 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void connect(SocketAddress endpoint) throws java.io.IOException
		public virtual void Connect(SocketAddress endpoint)
		{
			Connect(endpoint, 0);
		}

		/// <summary>
		/// Connects this socket to the server with a specified timeout value.
		/// A timeout of zero is interpreted as an infinite timeout. The connection
		/// will then block until established or an error occurs.
		/// </summary>
		/// <param name="endpoint"> the {@code SocketAddress} </param>
		/// <param name="timeout">  the timeout value to be used in milliseconds. </param>
		/// <exception cref="IOException"> if an error occurs during the connection </exception>
		/// <exception cref="SocketTimeoutException"> if timeout expires before connecting </exception>
		/// <exception cref="java.nio.channels.IllegalBlockingModeException">
		///          if this socket has an associated channel,
		///          and the channel is in non-blocking mode </exception>
		/// <exception cref="IllegalArgumentException"> if endpoint is null or is a
		///          SocketAddress subclass not supported by this socket
		/// @since 1.4
		/// @spec JSR-51 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void connect(SocketAddress endpoint, int timeout) throws java.io.IOException
		public virtual void Connect(SocketAddress endpoint, int timeout)
		{
			if (endpoint == null)
			{
				throw new IllegalArgumentException("connect: The address can't be null");
			}

			if (timeout < 0)
			{
			  throw new IllegalArgumentException("connect: timeout can't be negative");
			}

			if (Closed)
			{
				throw new SocketException("Socket is closed");
			}

			if (!OldImpl && Connected)
			{
				throw new SocketException("already connected");
			}

			if (!(endpoint is InetSocketAddress))
			{
				throw new IllegalArgumentException("Unsupported address type");
			}

			InetSocketAddress epoint = (InetSocketAddress) endpoint;
			InetAddress addr = epoint.Address;
			int port = epoint.Port;
			CheckAddress(addr, "connect");

			SecurityManager security = System.SecurityManager;
			if (security != null)
			{
				if (epoint.Unresolved)
				{
					security.CheckConnect(epoint.HostName, port);
				}
				else
				{
					security.CheckConnect(addr.HostAddress, port);
				}
			}
			if (!Created)
			{
				CreateImpl(true);
			}
			if (!OldImpl)
			{
				Impl_Renamed.Connect(epoint, timeout);
			}
			else if (timeout == 0)
			{
				if (epoint.Unresolved)
				{
					Impl_Renamed.Connect(addr.HostName, port);
				}
				else
				{
					Impl_Renamed.Connect(addr, port);
				}
			}
			else
			{
				throw new UnsupportedOperationException("SocketImpl.connect(addr, timeout)");
			}
			Connected_Renamed = true;
			/*
			 * If the socket was not bound before the connect, it is now because
			 * the kernel will have picked an ephemeral port & a local address
			 */
			Bound_Renamed = true;
		}

		/// <summary>
		/// Binds the socket to a local address.
		/// <P>
		/// If the address is {@code null}, then the system will pick up
		/// an ephemeral port and a valid local address to bind the socket.
		/// </summary>
		/// <param name="bindpoint"> the {@code SocketAddress} to bind to </param>
		/// <exception cref="IOException"> if the bind operation fails, or if the socket
		///                     is already bound. </exception>
		/// <exception cref="IllegalArgumentException"> if bindpoint is a
		///          SocketAddress subclass not supported by this socket </exception>
		/// <exception cref="SecurityException">  if a security manager exists and its
		///          {@code checkListen} method doesn't allow the bind
		///          to the local port.
		/// 
		/// @since   1.4 </exception>
		/// <seealso cref= #isBound </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void bind(SocketAddress bindpoint) throws java.io.IOException
		public virtual void Bind(SocketAddress bindpoint)
		{
			if (Closed)
			{
				throw new SocketException("Socket is closed");
			}
			if (!OldImpl && Bound)
			{
				throw new SocketException("Already bound");
			}

			if (bindpoint != null && (!(bindpoint is InetSocketAddress)))
			{
				throw new IllegalArgumentException("Unsupported address type");
			}
			InetSocketAddress epoint = (InetSocketAddress) bindpoint;
			if (epoint != null && epoint.Unresolved)
			{
				throw new SocketException("Unresolved address");
			}
			if (epoint == null)
			{
				epoint = new InetSocketAddress(0);
			}
			InetAddress addr = epoint.Address;
			int port = epoint.Port;
			CheckAddress(addr, "bind");
			SecurityManager security = System.SecurityManager;
			if (security != null)
			{
				security.CheckListen(port);
			}
			Impl.Bind(addr, port);
			Bound_Renamed = true;
		}

		private void CheckAddress(InetAddress addr, String op)
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
		/// set the flags after an accept() call.
		/// </summary>
		internal void PostAccept()
		{
			Connected_Renamed = true;
			Created = true;
			Bound_Renamed = true;
		}

		internal virtual void SetCreated()
		{
			Created = true;
		}

		internal virtual void SetBound()
		{
			Bound_Renamed = true;
		}

		internal virtual void SetConnected()
		{
			Connected_Renamed = true;
		}

		/// <summary>
		/// Returns the address to which the socket is connected.
		/// <para>
		/// If the socket was connected prior to being <seealso cref="#close closed"/>,
		/// then this method will continue to return the connected address
		/// after the socket is closed.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  the remote IP address to which this socket is connected,
		///          or {@code null} if the socket is not connected. </returns>
		public virtual InetAddress InetAddress
		{
			get
			{
				if (!Connected)
				{
					return null;
				}
				try
				{
					return Impl.InetAddress;
				}
				catch (SocketException)
				{
				}
				return null;
			}
		}

		/// <summary>
		/// Gets the local address to which the socket is bound.
		/// <para>
		/// If there is a security manager set, its {@code checkConnect} method is
		/// called with the local address and {@code -1} as its arguments to see
		/// if the operation is allowed. If the operation is not allowed,
		/// the <seealso cref="InetAddress#getLoopbackAddress loopback"/> address is returned.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the local address to which the socket is bound,
		///         the loopback address if denied by the security manager, or
		///         the wildcard address if the socket is closed or not bound yet.
		/// @since   JDK1.1
		/// </returns>
		/// <seealso cref= SecurityManager#checkConnect </seealso>
		public virtual InetAddress LocalAddress
		{
			get
			{
				// This is for backward compatibility
				if (!Bound)
				{
					return InetAddress.AnyLocalAddress();
				}
				InetAddress @in = null;
				try
				{
					@in = (InetAddress) Impl.GetOption(SocketOptions_Fields.SO_BINDADDR);
					SecurityManager sm = System.SecurityManager;
					if (sm != null)
					{
						sm.CheckConnect(@in.HostAddress, -1);
					}
					if (@in.AnyLocalAddress)
					{
						@in = InetAddress.AnyLocalAddress();
					}
				}
				catch (SecurityException)
				{
					@in = InetAddress.LoopbackAddress;
				}
				catch (Exception)
				{
					@in = InetAddress.AnyLocalAddress(); // "0.0.0.0"
				}
				return @in;
			}
		}

		/// <summary>
		/// Returns the remote port number to which this socket is connected.
		/// <para>
		/// If the socket was connected prior to being <seealso cref="#close closed"/>,
		/// then this method will continue to return the connected port number
		/// after the socket is closed.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  the remote port number to which this socket is connected, or
		///          0 if the socket is not connected yet. </returns>
		public virtual int Port
		{
			get
			{
				if (!Connected)
				{
					return 0;
				}
				try
				{
					return Impl.Port;
				}
				catch (SocketException)
				{
					// Shouldn't happen as we're connected
				}
				return -1;
			}
		}

		/// <summary>
		/// Returns the local port number to which this socket is bound.
		/// <para>
		/// If the socket was bound prior to being <seealso cref="#close closed"/>,
		/// then this method will continue to return the local port number
		/// after the socket is closed.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  the local port number to which this socket is bound or -1
		///          if the socket is not bound yet. </returns>
		public virtual int LocalPort
		{
			get
			{
				if (!Bound)
				{
					return -1;
				}
				try
				{
					return Impl.LocalPort;
				}
				catch (SocketException)
				{
					// shouldn't happen as we're bound
				}
				return -1;
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
		/// 
		/// </para>
		/// </summary>
		/// <returns> a {@code SocketAddress} representing the remote endpoint of this
		///         socket, or {@code null} if it is not connected yet. </returns>
		/// <seealso cref= #getInetAddress() </seealso>
		/// <seealso cref= #getPort() </seealso>
		/// <seealso cref= #connect(SocketAddress, int) </seealso>
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
		/// <para>
		/// If a socket bound to an endpoint represented by an
		/// {@code InetSocketAddress } is <seealso cref="#close closed"/>,
		/// then this method will continue to return an {@code InetSocketAddress}
		/// after the socket is closed. In that case the returned
		/// {@code InetSocketAddress}'s address is the
		/// <seealso cref="InetAddress#isAnyLocalAddress wildcard"/> address
		/// and its port is the local port that it was bound to.
		/// </para>
		/// <para>
		/// If there is a security manager set, its {@code checkConnect} method is
		/// called with the local address and {@code -1} as its arguments to see
		/// if the operation is allowed. If the operation is not allowed,
		/// a {@code SocketAddress} representing the
		/// <seealso cref="InetAddress#getLoopbackAddress loopback"/> address and the local
		/// port to which this socket is bound is returned.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a {@code SocketAddress} representing the local endpoint of
		///         this socket, or a {@code SocketAddress} representing the
		///         loopback address if denied by the security manager, or
		///         {@code null} if the socket is not bound yet.
		/// </returns>
		/// <seealso cref= #getLocalAddress() </seealso>
		/// <seealso cref= #getLocalPort() </seealso>
		/// <seealso cref= #bind(SocketAddress) </seealso>
		/// <seealso cref= SecurityManager#checkConnect
		/// @since 1.4 </seealso>

		public virtual SocketAddress LocalSocketAddress
		{
			get
			{
				if (!Bound)
				{
					return null;
				}
				return new InetSocketAddress(LocalAddress, LocalPort);
			}
		}

		/// <summary>
		/// Returns the unique <seealso cref="java.nio.channels.SocketChannel SocketChannel"/>
		/// object associated with this socket, if any.
		/// 
		/// <para> A socket will have a channel if, and only if, the channel itself was
		/// created via the {@link java.nio.channels.SocketChannel#open
		/// SocketChannel.open} or {@link
		/// java.nio.channels.ServerSocketChannel#accept ServerSocketChannel.accept}
		/// methods.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  the socket channel associated with this socket,
		///          or {@code null} if this socket was not created
		///          for a channel
		/// 
		/// @since 1.4
		/// @spec JSR-51 </returns>
		public virtual SocketChannel Channel
		{
			get
			{
				return null;
			}
		}

		/// <summary>
		/// Returns an input stream for this socket.
		/// 
		/// <para> If this socket has an associated channel then the resulting input
		/// stream delegates all of its operations to the channel.  If the channel
		/// is in non-blocking mode then the input stream's {@code read} operations
		/// will throw an <seealso cref="java.nio.channels.IllegalBlockingModeException"/>.
		/// 
		/// </para>
		/// <para>Under abnormal conditions the underlying connection may be
		/// broken by the remote host or the network software (for example
		/// a connection reset in the case of TCP connections). When a
		/// broken connection is detected by the network software the
		/// following applies to the returned input stream :-
		/// 
		/// <ul>
		/// 
		/// </para>
		///   <li><para>The network software may discard bytes that are buffered
		///   by the socket. Bytes that aren't discarded by the network
		///   software can be read using <seealso cref="java.io.InputStream#read read"/>.
		/// 
		/// </para>
		///   <li><para>If there are no bytes buffered on the socket, or all
		///   buffered bytes have been consumed by
		///   <seealso cref="java.io.InputStream#read read"/>, then all subsequent
		///   calls to <seealso cref="java.io.InputStream#read read"/> will throw an
		///   <seealso cref="java.io.IOException IOException"/>.
		/// 
		/// </para>
		///   <li><para>If there are no bytes buffered on the socket, and the
		///   socket has not been closed using <seealso cref="#close close"/>, then
		///   <seealso cref="java.io.InputStream#available available"/> will
		///   return {@code 0}.
		/// 
		/// </ul>
		/// 
		/// </para>
		/// <para> Closing the returned <seealso cref="java.io.InputStream InputStream"/>
		/// will close the associated socket.
		/// 
		/// </para>
		/// </summary>
		/// <returns>     an input stream for reading bytes from this socket. </returns>
		/// <exception cref="IOException">  if an I/O error occurs when creating the
		///             input stream, the socket is closed, the socket is
		///             not connected, or the socket input has been shutdown
		///             using <seealso cref="#shutdownInput()"/>
		/// 
		/// @revised 1.4
		/// @spec JSR-51 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.io.InputStream getInputStream() throws java.io.IOException
		public virtual InputStream InputStream
		{
			get
			{
				if (Closed)
				{
					throw new SocketException("Socket is closed");
				}
				if (!Connected)
				{
					throw new SocketException("Socket is not connected");
				}
				if (InputShutdown)
				{
					throw new SocketException("Socket input is shutdown");
				}
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final Socket s = this;
				Socket s = this;
				InputStream @is = null;
				try
				{
					@is = AccessController.doPrivileged(new PrivilegedExceptionActionAnonymousInnerClassHelper(this));
				}
				catch (java.security.PrivilegedActionException e)
				{
					throw (IOException) e.Exception;
				}
				return @is;
			}
		}

		private class PrivilegedExceptionActionAnonymousInnerClassHelper : PrivilegedExceptionAction<InputStream>
		{
			private readonly Socket OuterInstance;

			public PrivilegedExceptionActionAnonymousInnerClassHelper(Socket outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.io.InputStream run() throws java.io.IOException
			public virtual InputStream Run()
			{
				return OuterInstance.Impl_Renamed.InputStream;
			}
		}

		/// <summary>
		/// Returns an output stream for this socket.
		/// 
		/// <para> If this socket has an associated channel then the resulting output
		/// stream delegates all of its operations to the channel.  If the channel
		/// is in non-blocking mode then the output stream's {@code write}
		/// operations will throw an {@link
		/// java.nio.channels.IllegalBlockingModeException}.
		/// 
		/// </para>
		/// <para> Closing the returned <seealso cref="java.io.OutputStream OutputStream"/>
		/// will close the associated socket.
		/// 
		/// </para>
		/// </summary>
		/// <returns>     an output stream for writing bytes to this socket. </returns>
		/// <exception cref="IOException">  if an I/O error occurs when creating the
		///               output stream or if the socket is not connected.
		/// @revised 1.4
		/// @spec JSR-51 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.io.OutputStream getOutputStream() throws java.io.IOException
		public virtual OutputStream OutputStream
		{
			get
			{
				if (Closed)
				{
					throw new SocketException("Socket is closed");
				}
				if (!Connected)
				{
					throw new SocketException("Socket is not connected");
				}
				if (OutputShutdown)
				{
					throw new SocketException("Socket output is shutdown");
				}
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final Socket s = this;
				Socket s = this;
				OutputStream os = null;
				try
				{
					os = AccessController.doPrivileged(new PrivilegedExceptionActionAnonymousInnerClassHelper2(this));
				}
				catch (java.security.PrivilegedActionException e)
				{
					throw (IOException) e.Exception;
				}
				return os;
			}
		}

		private class PrivilegedExceptionActionAnonymousInnerClassHelper2 : PrivilegedExceptionAction<OutputStream>
		{
			private readonly Socket OuterInstance;

			public PrivilegedExceptionActionAnonymousInnerClassHelper2(Socket outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.io.OutputStream run() throws java.io.IOException
			public virtual OutputStream Run()
			{
				return OuterInstance.Impl_Renamed.OutputStream;
			}
		}

		/// <summary>
		/// Enable/disable <seealso cref="SocketOptions#TCP_NODELAY TCP_NODELAY"/>
		/// (disable/enable Nagle's algorithm).
		/// </summary>
		/// <param name="on"> {@code true} to enable TCP_NODELAY,
		/// {@code false} to disable.
		/// </param>
		/// <exception cref="SocketException"> if there is an error
		/// in the underlying protocol, such as a TCP error.
		/// 
		/// @since   JDK1.1
		/// </exception>
		/// <seealso cref= #getTcpNoDelay() </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setTcpNoDelay(boolean on) throws SocketException
		public virtual bool TcpNoDelay
		{
			set
			{
				if (Closed)
				{
					throw new SocketException("Socket is closed");
				}
				Impl.SetOption(SocketOptions_Fields.TCP_NODELAY, Convert.ToBoolean(value));
			}
			get
			{
				if (Closed)
				{
					throw new SocketException("Socket is closed");
				}
				return ((Boolean) Impl.GetOption(SocketOptions_Fields.TCP_NODELAY)).BooleanValue();
			}
		}


		/// <summary>
		/// Enable/disable <seealso cref="SocketOptions#SO_LINGER SO_LINGER"/> with the
		/// specified linger time in seconds. The maximum timeout value is platform
		/// specific.
		/// 
		/// The setting only affects socket close.
		/// </summary>
		/// <param name="on">     whether or not to linger on. </param>
		/// <param name="linger"> how long to linger for, if on is true. </param>
		/// <exception cref="SocketException"> if there is an error
		/// in the underlying protocol, such as a TCP error. </exception>
		/// <exception cref="IllegalArgumentException"> if the linger value is negative.
		/// @since JDK1.1 </exception>
		/// <seealso cref= #getSoLinger() </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setSoLinger(boolean on, int linger) throws SocketException
		public virtual void SetSoLinger(bool on, int linger)
		{
			if (Closed)
			{
				throw new SocketException("Socket is closed");
			}
			if (!on)
			{
				Impl.SetOption(SocketOptions_Fields.SO_LINGER, new Boolean(on));
			}
			else
			{
				if (linger < 0)
				{
					throw new IllegalArgumentException("invalid value for SO_LINGER");
				}
				if (linger > 65535)
				{
					linger = 65535;
				}
				Impl.SetOption(SocketOptions_Fields.SO_LINGER, new Integer(linger));
			}
		}

		/// <summary>
		/// Returns setting for <seealso cref="SocketOptions#SO_LINGER SO_LINGER"/>.
		/// -1 returns implies that the
		/// option is disabled.
		/// 
		/// The setting only affects socket close.
		/// </summary>
		/// <returns> the setting for <seealso cref="SocketOptions#SO_LINGER SO_LINGER"/>. </returns>
		/// <exception cref="SocketException"> if there is an error
		/// in the underlying protocol, such as a TCP error.
		/// @since   JDK1.1 </exception>
		/// <seealso cref= #setSoLinger(boolean, int) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getSoLinger() throws SocketException
		public virtual int SoLinger
		{
			get
			{
				if (Closed)
				{
					throw new SocketException("Socket is closed");
				}
				Object o = Impl.GetOption(SocketOptions_Fields.SO_LINGER);
				if (o is Integer)
				{
					return ((Integer) o).IntValue();
				}
				else
				{
					return -1;
				}
			}
		}

		/// <summary>
		/// Send one byte of urgent data on the socket. The byte to be sent is the lowest eight
		/// bits of the data parameter. The urgent byte is
		/// sent after any preceding writes to the socket OutputStream
		/// and before any future writes to the OutputStream. </summary>
		/// <param name="data"> The byte of data to send </param>
		/// <exception cref="IOException"> if there is an error
		///  sending the data.
		/// @since 1.4 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void sendUrgentData(int data) throws java.io.IOException
		public virtual void SendUrgentData(int data)
		{
			if (!Impl.SupportsUrgentData())
			{
				throw new SocketException("Urgent data not supported");
			}
			Impl.SendUrgentData(data);
		}

		/// <summary>
		/// Enable/disable <seealso cref="SocketOptions#SO_OOBINLINE SO_OOBINLINE"/>
		/// (receipt of TCP urgent data)
		/// 
		/// By default, this option is disabled and TCP urgent data received on a
		/// socket is silently discarded. If the user wishes to receive urgent data, then
		/// this option must be enabled. When enabled, urgent data is received
		/// inline with normal data.
		/// <para>
		/// Note, only limited support is provided for handling incoming urgent
		/// data. In particular, no notification of incoming urgent data is provided
		/// and there is no capability to distinguish between normal data and urgent
		/// data unless provided by a higher level protocol.
		/// 
		/// </para>
		/// </summary>
		/// <param name="on"> {@code true} to enable
		///           <seealso cref="SocketOptions#SO_OOBINLINE SO_OOBINLINE"/>,
		///           {@code false} to disable.
		/// </param>
		/// <exception cref="SocketException"> if there is an error
		/// in the underlying protocol, such as a TCP error.
		/// 
		/// @since   1.4
		/// </exception>
		/// <seealso cref= #getOOBInline() </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setOOBInline(boolean on) throws SocketException
		public virtual bool OOBInline
		{
			set
			{
				if (Closed)
				{
					throw new SocketException("Socket is closed");
				}
				Impl.SetOption(SocketOptions_Fields.SO_OOBINLINE, Convert.ToBoolean(value));
			}
			get
			{
				if (Closed)
				{
					throw new SocketException("Socket is closed");
				}
				return ((Boolean) Impl.GetOption(SocketOptions_Fields.SO_OOBINLINE)).BooleanValue();
			}
		}


		/// <summary>
		///  Enable/disable <seealso cref="SocketOptions#SO_TIMEOUT SO_TIMEOUT"/>
		///  with the specified timeout, in milliseconds. With this option set
		///  to a non-zero timeout, a read() call on the InputStream associated with
		///  this Socket will block for only this amount of time.  If the timeout
		///  expires, a <B>java.net.SocketTimeoutException</B> is raised, though the
		///  Socket is still valid. The option <B>must</B> be enabled
		///  prior to entering the blocking operation to have effect. The
		///  timeout must be {@code > 0}.
		///  A timeout of zero is interpreted as an infinite timeout.
		/// </summary>
		/// <param name="timeout"> the specified timeout, in milliseconds. </param>
		/// <exception cref="SocketException"> if there is an error
		/// in the underlying protocol, such as a TCP error.
		/// @since   JDK 1.1 </exception>
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
					if (value < 0)
					{
					  throw new IllegalArgumentException("timeout can't be negative");
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
		/// Sets the <seealso cref="SocketOptions#SO_SNDBUF SO_SNDBUF"/> option to the
		/// specified value for this {@code Socket}.
		/// The <seealso cref="SocketOptions#SO_SNDBUF SO_SNDBUF"/> option is used by the
		/// platform's networking code as a hint for the size to set the underlying
		/// network I/O buffers.
		/// 
		/// <para>Because <seealso cref="SocketOptions#SO_SNDBUF SO_SNDBUF"/> is a hint,
		/// applications that want to verify what size the buffers were set to
		/// should call <seealso cref="#getSendBufferSize()"/>.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="SocketException"> if there is an error
		/// in the underlying protocol, such as a TCP error.
		/// </exception>
		/// <param name="size"> the size to which to set the send buffer
		/// size. This value must be greater than 0.
		/// </param>
		/// <exception cref="IllegalArgumentException"> if the
		/// value is 0 or is negative.
		/// </exception>
		/// <seealso cref= #getSendBufferSize()
		/// @since 1.2 </seealso>
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
		/// Sets the <seealso cref="SocketOptions#SO_RCVBUF SO_RCVBUF"/> option to the
		/// specified value for this {@code Socket}. The
		/// <seealso cref="SocketOptions#SO_RCVBUF SO_RCVBUF"/> option is
		/// used by the platform's networking code as a hint for the size to set
		/// the underlying network I/O buffers.
		/// 
		/// <para>Increasing the receive buffer size can increase the performance of
		/// network I/O for high-volume connection, while decreasing it can
		/// help reduce the backlog of incoming data.
		/// 
		/// </para>
		/// <para>Because <seealso cref="SocketOptions#SO_RCVBUF SO_RCVBUF"/> is a hint,
		/// applications that want to verify what size the buffers were set to
		/// should call <seealso cref="#getReceiveBufferSize()"/>.
		/// 
		/// </para>
		/// <para>The value of <seealso cref="SocketOptions#SO_RCVBUF SO_RCVBUF"/> is also used
		/// to set the TCP receive window that is advertized to the remote peer.
		/// Generally, the window size can be modified at any time when a socket is
		/// connected. However, if a receive window larger than 64K is required then
		/// this must be requested <B>before</B> the socket is connected to the
		/// remote peer. There are two cases to be aware of:
		/// <ol>
		/// <li>For sockets accepted from a ServerSocket, this must be done by calling
		/// <seealso cref="ServerSocket#setReceiveBufferSize(int)"/> before the ServerSocket
		/// </para>
		/// is bound to a local address.<para></li>
		/// <li>For client sockets, setReceiveBufferSize() must be called before
		/// connecting the socket to its remote peer.</li></ol>
		/// </para>
		/// </summary>
		/// <param name="size"> the size to which to set the receive buffer
		/// size. This value must be greater than 0.
		/// </param>
		/// <exception cref="IllegalArgumentException"> if the value is 0 or is
		/// negative.
		/// </exception>
		/// <exception cref="SocketException"> if there is an error
		/// in the underlying protocol, such as a TCP error.
		/// </exception>
		/// <seealso cref= #getReceiveBufferSize() </seealso>
		/// <seealso cref= ServerSocket#setReceiveBufferSize(int)
		/// @since 1.2 </seealso>
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
		/// Enable/disable <seealso cref="SocketOptions#SO_KEEPALIVE SO_KEEPALIVE"/>.
		/// </summary>
		/// <param name="on">  whether or not to have socket keep alive turned on. </param>
		/// <exception cref="SocketException"> if there is an error
		/// in the underlying protocol, such as a TCP error.
		/// @since 1.3 </exception>
		/// <seealso cref= #getKeepAlive() </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setKeepAlive(boolean on) throws SocketException
		public virtual bool KeepAlive
		{
			set
			{
				if (Closed)
				{
					throw new SocketException("Socket is closed");
				}
				Impl.SetOption(SocketOptions_Fields.SO_KEEPALIVE, Convert.ToBoolean(value));
			}
			get
			{
				if (Closed)
				{
					throw new SocketException("Socket is closed");
				}
				return ((Boolean) Impl.GetOption(SocketOptions_Fields.SO_KEEPALIVE)).BooleanValue();
			}
		}


		/// <summary>
		/// Sets traffic class or type-of-service octet in the IP
		/// header for packets sent from this Socket.
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
		/// As RFC 1122 section 4.2.4.2 indicates, a compliant TCP
		/// implementation should, but is not required to, let application
		/// change the TOS field during the lifetime of a connection.
		/// So whether the type-of-service field can be changed after the
		/// TCP connection has been established depends on the implementation
		/// in the underlying platform. Applications should not assume that
		/// they can change the TOS field after the connection.
		/// </para>
		/// <para>
		/// For Internet Protocol v6 {@code tc} is the value that
		/// would be placed into the sin6_flowinfo field of the IP header.
		/// 
		/// </para>
		/// </summary>
		/// <param name="tc">        an {@code int} value for the bitset. </param>
		/// <exception cref="SocketException"> if there is an error setting the
		/// traffic class or type-of-service
		/// @since 1.4 </exception>
		/// <seealso cref= #getTrafficClass </seealso>
		/// <seealso cref= SocketOptions#IP_TOS </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setTrafficClass(int tc) throws SocketException
		public virtual int TrafficClass
		{
			set
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
			get
			{
				return ((Integer)(Impl.GetOption(SocketOptions_Fields.IP_TOS))).IntValue();
			}
		}


		/// <summary>
		/// Enable/disable the <seealso cref="SocketOptions#SO_REUSEADDR SO_REUSEADDR"/>
		/// socket option.
		/// <para>
		/// When a TCP connection is closed the connection may remain
		/// in a timeout state for a period of time after the connection
		/// is closed (typically known as the {@code TIME_WAIT} state
		/// or {@code 2MSL} wait state).
		/// For applications using a well known socket address or port
		/// it may not be possible to bind a socket to the required
		/// {@code SocketAddress} if there is a connection in the
		/// timeout state involving the socket address or port.
		/// </para>
		/// <para>
		/// Enabling <seealso cref="SocketOptions#SO_REUSEADDR SO_REUSEADDR"/>
		/// prior to binding the socket using <seealso cref="#bind(SocketAddress)"/> allows
		/// the socket to be bound even though a previous connection is in a timeout
		/// state.
		/// </para>
		/// <para>
		/// When a {@code Socket} is created the initial setting
		/// of <seealso cref="SocketOptions#SO_REUSEADDR SO_REUSEADDR"/> is disabled.
		/// </para>
		/// <para>
		/// The behaviour when <seealso cref="SocketOptions#SO_REUSEADDR SO_REUSEADDR"/> is
		/// enabled or disabled after a socket is bound (See <seealso cref="#isBound()"/>)
		/// is not defined.
		/// 
		/// </para>
		/// </summary>
		/// <param name="on">  whether to enable or disable the socket option </param>
		/// <exception cref="SocketException"> if an error occurs enabling or
		///            disabling the <seealso cref="SocketOptions#SO_REUSEADDR SO_REUSEADDR"/>
		///            socket option, or the socket is closed.
		/// @since 1.4 </exception>
		/// <seealso cref= #getReuseAddress() </seealso>
		/// <seealso cref= #bind(SocketAddress) </seealso>
		/// <seealso cref= #isClosed() </seealso>
		/// <seealso cref= #isBound() </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setReuseAddress(boolean on) throws SocketException
		public virtual bool ReuseAddress
		{
			set
			{
				if (Closed)
				{
					throw new SocketException("Socket is closed");
				}
				Impl.SetOption(SocketOptions_Fields.SO_REUSEADDR, Convert.ToBoolean(value));
			}
			get
			{
				if (Closed)
				{
					throw new SocketException("Socket is closed");
				}
				return ((Boolean)(Impl.GetOption(SocketOptions_Fields.SO_REUSEADDR))).BooleanValue();
			}
		}


		/// <summary>
		/// Closes this socket.
		/// <para>
		/// Any thread currently blocked in an I/O operation upon this socket
		/// will throw a <seealso cref="SocketException"/>.
		/// </para>
		/// <para>
		/// Once a socket has been closed, it is not available for further networking
		/// use (i.e. can't be reconnected or rebound). A new socket needs to be
		/// created.
		/// 
		/// </para>
		/// <para> Closing this socket will also close the socket's
		/// <seealso cref="java.io.InputStream InputStream"/> and
		/// <seealso cref="java.io.OutputStream OutputStream"/>.
		/// 
		/// </para>
		/// <para> If this socket has an associated channel then the channel is closed
		/// as well.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="IOException">  if an I/O error occurs when closing this socket.
		/// @revised 1.4
		/// @spec JSR-51 </exception>
		/// <seealso cref= #isClosed </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public synchronized void close() throws java.io.IOException
		public virtual void Close()
		{
			lock (this)
			{
				lock (CloseLock)
				{
					if (Closed)
					{
						return;
					}
					if (Created)
					{
						Impl_Renamed.Close();
					}
					Closed_Renamed = true;
				}
			}
		}

		/// <summary>
		/// Places the input stream for this socket at "end of stream".
		/// Any data sent to the input stream side of the socket is acknowledged
		/// and then silently discarded.
		/// <para>
		/// If you read from a socket input stream after invoking this method on the
		/// socket, the stream's {@code available} method will return 0, and its
		/// {@code read} methods will return {@code -1} (end of stream).
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="IOException"> if an I/O error occurs when shutting down this
		/// socket.
		/// 
		/// @since 1.3 </exception>
		/// <seealso cref= java.net.Socket#shutdownOutput() </seealso>
		/// <seealso cref= java.net.Socket#close() </seealso>
		/// <seealso cref= java.net.Socket#setSoLinger(boolean, int) </seealso>
		/// <seealso cref= #isInputShutdown </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void shutdownInput() throws java.io.IOException
		public virtual void ShutdownInput()
		{
			if (Closed)
			{
				throw new SocketException("Socket is closed");
			}
			if (!Connected)
			{
				throw new SocketException("Socket is not connected");
			}
			if (InputShutdown)
			{
				throw new SocketException("Socket input is already shutdown");
			}
			Impl.ShutdownInput();
			ShutIn = true;
		}

		/// <summary>
		/// Disables the output stream for this socket.
		/// For a TCP socket, any previously written data will be sent
		/// followed by TCP's normal connection termination sequence.
		/// 
		/// If you write to a socket output stream after invoking
		/// shutdownOutput() on the socket, the stream will throw
		/// an IOException.
		/// </summary>
		/// <exception cref="IOException"> if an I/O error occurs when shutting down this
		/// socket.
		/// 
		/// @since 1.3 </exception>
		/// <seealso cref= java.net.Socket#shutdownInput() </seealso>
		/// <seealso cref= java.net.Socket#close() </seealso>
		/// <seealso cref= java.net.Socket#setSoLinger(boolean, int) </seealso>
		/// <seealso cref= #isOutputShutdown </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void shutdownOutput() throws java.io.IOException
		public virtual void ShutdownOutput()
		{
			if (Closed)
			{
				throw new SocketException("Socket is closed");
			}
			if (!Connected)
			{
				throw new SocketException("Socket is not connected");
			}
			if (OutputShutdown)
			{
				throw new SocketException("Socket output is already shutdown");
			}
			Impl.ShutdownOutput();
			ShutOut = true;
		}

		/// <summary>
		/// Converts this socket to a {@code String}.
		/// </summary>
		/// <returns>  a string representation of this socket. </returns>
		public override String ToString()
		{
			try
			{
				if (Connected)
				{
					return "Socket[addr=" + Impl.InetAddress + ",port=" + Impl.Port + ",localport=" + Impl.LocalPort + "]";
				}
			}
			catch (SocketException)
			{
			}
			return "Socket[unconnected]";
		}

		/// <summary>
		/// Returns the connection state of the socket.
		/// <para>
		/// Note: Closing a socket doesn't clear its connection state, which means
		/// this method will return {@code true} for a closed socket
		/// (see <seealso cref="#isClosed()"/>) if it was successfuly connected prior
		/// to being closed.
		/// 
		/// </para>
		/// </summary>
		/// <returns> true if the socket was successfuly connected to a server
		/// @since 1.4 </returns>
		public virtual bool Connected
		{
			get
			{
				// Before 1.3 Sockets were always connected during creation
				return Connected_Renamed || OldImpl;
			}
		}

		/// <summary>
		/// Returns the binding state of the socket.
		/// <para>
		/// Note: Closing a socket doesn't clear its binding state, which means
		/// this method will return {@code true} for a closed socket
		/// (see <seealso cref="#isClosed()"/>) if it was successfuly bound prior
		/// to being closed.
		/// 
		/// </para>
		/// </summary>
		/// <returns> true if the socket was successfuly bound to an address
		/// @since 1.4 </returns>
		/// <seealso cref= #bind </seealso>
		public virtual bool Bound
		{
			get
			{
				// Before 1.3 Sockets were always bound during creation
				return Bound_Renamed || OldImpl;
			}
		}

		/// <summary>
		/// Returns the closed state of the socket.
		/// </summary>
		/// <returns> true if the socket has been closed
		/// @since 1.4 </returns>
		/// <seealso cref= #close </seealso>
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
		/// Returns whether the read-half of the socket connection is closed.
		/// </summary>
		/// <returns> true if the input of the socket has been shutdown
		/// @since 1.4 </returns>
		/// <seealso cref= #shutdownInput </seealso>
		public virtual bool InputShutdown
		{
			get
			{
				return ShutIn;
			}
		}

		/// <summary>
		/// Returns whether the write-half of the socket connection is closed.
		/// </summary>
		/// <returns> true if the output of the socket has been shutdown
		/// @since 1.4 </returns>
		/// <seealso cref= #shutdownOutput </seealso>
		public virtual bool OutputShutdown
		{
			get
			{
				return ShutOut;
			}
		}

		/// <summary>
		/// The factory for all client sockets.
		/// </summary>
		private static SocketImplFactory Factory = null;

		/// <summary>
		/// Sets the client socket implementation factory for the
		/// application. The factory can be specified only once.
		/// <para>
		/// When an application creates a new client socket, the socket
		/// implementation factory's {@code createSocketImpl} method is
		/// called to create the actual socket implementation.
		/// </para>
		/// <para>
		/// Passing {@code null} to the method is a no-op unless the factory
		/// was already set.
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
		///               socket factory. </exception>
		/// <exception cref="SocketException">  if the factory is already defined. </exception>
		/// <exception cref="SecurityException">  if a security manager exists and its
		///             {@code checkSetFactory} method doesn't allow the operation. </exception>
		/// <seealso cref=        java.net.SocketImplFactory#createSocketImpl() </seealso>
		/// <seealso cref=        SecurityManager#checkSetFactory </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static synchronized void setSocketImplFactory(SocketImplFactory fac) throws java.io.IOException
		public static SocketImplFactory SocketImplFactory
		{
			set
			{
				lock (typeof(Socket))
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

		/// <summary>
		/// Sets performance preferences for this socket.
		/// 
		/// <para> Sockets use the TCP/IP protocol by default.  Some implementations
		/// may offer alternative protocols which have different performance
		/// characteristics than TCP/IP.  This method allows the application to
		/// express its own preferences as to how these tradeoffs should be made
		/// when the implementation chooses from the available protocols.
		/// 
		/// </para>
		/// <para> Performance preferences are described by three integers
		/// whose values indicate the relative importance of short connection time,
		/// low latency, and high bandwidth.  The absolute values of the integers
		/// are irrelevant; in order to choose a protocol the values are simply
		/// compared, with larger values indicating stronger preferences. Negative
		/// values represent a lower priority than positive values. If the
		/// application prefers short connection time over both low latency and high
		/// bandwidth, for example, then it could invoke this method with the values
		/// {@code (1, 0, 0)}.  If the application prefers high bandwidth above low
		/// latency, and low latency above short connection time, then it could
		/// invoke this method with the values {@code (0, 1, 2)}.
		/// 
		/// </para>
		/// <para> Invoking this method after this socket has been connected
		/// will have no effect.
		/// 
		/// </para>
		/// </summary>
		/// <param name="connectionTime">
		///         An {@code int} expressing the relative importance of a short
		///         connection time
		/// </param>
		/// <param name="latency">
		///         An {@code int} expressing the relative importance of low
		///         latency
		/// </param>
		/// <param name="bandwidth">
		///         An {@code int} expressing the relative importance of high
		///         bandwidth
		/// 
		/// @since 1.5 </param>
		public virtual void SetPerformancePreferences(int connectionTime, int latency, int bandwidth)
		{
			/* Not implemented yet */
		}
	}

}