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
	/// This class implements server sockets. A server socket waits for
	/// requests to come in over the network. It performs some operation
	/// based on that request, and then possibly returns a result to the requester.
	/// <para>
	/// The actual work of the server socket is performed by an instance
	/// of the {@code SocketImpl} class. An application can
	/// change the socket factory that creates the socket
	/// implementation to configure itself to create sockets
	/// appropriate to the local firewall.
	/// 
	/// @author  unascribed
	/// </para>
	/// </summary>
	/// <seealso cref=     java.net.SocketImpl </seealso>
	/// <seealso cref=     java.net.ServerSocket#setSocketFactory(java.net.SocketImplFactory) </seealso>
	/// <seealso cref=     java.nio.channels.ServerSocketChannel
	/// @since   JDK1.0 </seealso>
	public class ServerSocket : java.io.Closeable
	{
		/// <summary>
		/// Various states of this socket.
		/// </summary>
		private bool Created = false;
		private bool Bound_Renamed = false;
		private bool Closed_Renamed = false;
		private Object CloseLock = new Object();

		/// <summary>
		/// The implementation of this Socket.
		/// </summary>
		private SocketImpl Impl_Renamed;

		/// <summary>
		/// Are we using an older SocketImpl?
		/// </summary>
		private bool OldImpl = false;

		/// <summary>
		/// Package-private constructor to create a ServerSocket associated with
		/// the given SocketImpl.
		/// </summary>
		internal ServerSocket(SocketImpl impl)
		{
			this.Impl_Renamed = impl;
			impl.ServerSocket = this;
		}

		/// <summary>
		/// Creates an unbound server socket.
		/// </summary>
		/// <exception cref="IOException"> IO error when opening the socket.
		/// @revised 1.4 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ServerSocket() throws java.io.IOException
		public ServerSocket()
		{
			SetImpl();
		}

		/// <summary>
		/// Creates a server socket, bound to the specified port. A port number
		/// of {@code 0} means that the port number is automatically
		/// allocated, typically from an ephemeral port range. This port
		/// number can then be retrieved by calling <seealso cref="#getLocalPort getLocalPort"/>.
		/// <para>
		/// The maximum queue length for incoming connection indications (a
		/// request to connect) is set to {@code 50}. If a connection
		/// indication arrives when the queue is full, the connection is refused.
		/// </para>
		/// <para>
		/// If the application has specified a server socket factory, that
		/// factory's {@code createSocketImpl} method is called to create
		/// the actual socket implementation. Otherwise a "plain" socket is created.
		/// </para>
		/// <para>
		/// If there is a security manager,
		/// its {@code checkListen} method is called
		/// with the {@code port} argument
		/// as its argument to ensure the operation is allowed.
		/// This could result in a SecurityException.
		/// 
		/// 
		/// </para>
		/// </summary>
		/// <param name="port">  the port number, or {@code 0} to use a port
		///                   number that is automatically allocated.
		/// </param>
		/// <exception cref="IOException">  if an I/O error occurs when opening the socket. </exception>
		/// <exception cref="SecurityException">
		/// if a security manager exists and its {@code checkListen}
		/// method doesn't allow the operation. </exception>
		/// <exception cref="IllegalArgumentException"> if the port parameter is outside
		///             the specified range of valid port values, which is between
		///             0 and 65535, inclusive.
		/// </exception>
		/// <seealso cref=        java.net.SocketImpl </seealso>
		/// <seealso cref=        java.net.SocketImplFactory#createSocketImpl() </seealso>
		/// <seealso cref=        java.net.ServerSocket#setSocketFactory(java.net.SocketImplFactory) </seealso>
		/// <seealso cref=        SecurityManager#checkListen </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ServerSocket(int port) throws java.io.IOException
		public ServerSocket(int port) : this(port, 50, null)
		{
		}

		/// <summary>
		/// Creates a server socket and binds it to the specified local port
		/// number, with the specified backlog.
		/// A port number of {@code 0} means that the port number is
		/// automatically allocated, typically from an ephemeral port range.
		/// This port number can then be retrieved by calling
		/// <seealso cref="#getLocalPort getLocalPort"/>.
		/// <para>
		/// The maximum queue length for incoming connection indications (a
		/// request to connect) is set to the {@code backlog} parameter. If
		/// a connection indication arrives when the queue is full, the
		/// connection is refused.
		/// </para>
		/// <para>
		/// If the application has specified a server socket factory, that
		/// factory's {@code createSocketImpl} method is called to create
		/// the actual socket implementation. Otherwise a "plain" socket is created.
		/// </para>
		/// <para>
		/// If there is a security manager,
		/// its {@code checkListen} method is called
		/// with the {@code port} argument
		/// as its argument to ensure the operation is allowed.
		/// This could result in a SecurityException.
		/// 
		/// The {@code backlog} argument is the requested maximum number of
		/// pending connections on the socket. Its exact semantics are implementation
		/// specific. In particular, an implementation may impose a maximum length
		/// or may choose to ignore the parameter altogther. The value provided
		/// should be greater than {@code 0}. If it is less than or equal to
		/// {@code 0}, then an implementation specific default will be used.
		/// <P>
		/// 
		/// </para>
		/// </summary>
		/// <param name="port">     the port number, or {@code 0} to use a port
		///                      number that is automatically allocated. </param>
		/// <param name="backlog">  requested maximum length of the queue of incoming
		///                      connections.
		/// </param>
		/// <exception cref="IOException">  if an I/O error occurs when opening the socket. </exception>
		/// <exception cref="SecurityException">
		/// if a security manager exists and its {@code checkListen}
		/// method doesn't allow the operation. </exception>
		/// <exception cref="IllegalArgumentException"> if the port parameter is outside
		///             the specified range of valid port values, which is between
		///             0 and 65535, inclusive.
		/// </exception>
		/// <seealso cref=        java.net.SocketImpl </seealso>
		/// <seealso cref=        java.net.SocketImplFactory#createSocketImpl() </seealso>
		/// <seealso cref=        java.net.ServerSocket#setSocketFactory(java.net.SocketImplFactory) </seealso>
		/// <seealso cref=        SecurityManager#checkListen </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ServerSocket(int port, int backlog) throws java.io.IOException
		public ServerSocket(int port, int backlog) : this(port, backlog, null)
		{
		}

		/// <summary>
		/// Create a server with the specified port, listen backlog, and
		/// local IP address to bind to.  The <i>bindAddr</i> argument
		/// can be used on a multi-homed host for a ServerSocket that
		/// will only accept connect requests to one of its addresses.
		/// If <i>bindAddr</i> is null, it will default accepting
		/// connections on any/all local addresses.
		/// The port must be between 0 and 65535, inclusive.
		/// A port number of {@code 0} means that the port number is
		/// automatically allocated, typically from an ephemeral port range.
		/// This port number can then be retrieved by calling
		/// <seealso cref="#getLocalPort getLocalPort"/>.
		/// 
		/// <P>If there is a security manager, this method
		/// calls its {@code checkListen} method
		/// with the {@code port} argument
		/// as its argument to ensure the operation is allowed.
		/// This could result in a SecurityException.
		/// 
		/// The {@code backlog} argument is the requested maximum number of
		/// pending connections on the socket. Its exact semantics are implementation
		/// specific. In particular, an implementation may impose a maximum length
		/// or may choose to ignore the parameter altogther. The value provided
		/// should be greater than {@code 0}. If it is less than or equal to
		/// {@code 0}, then an implementation specific default will be used.
		/// <P> </summary>
		/// <param name="port">  the port number, or {@code 0} to use a port
		///              number that is automatically allocated. </param>
		/// <param name="backlog"> requested maximum length of the queue of incoming
		///                connections. </param>
		/// <param name="bindAddr"> the local InetAddress the server will bind to
		/// </param>
		/// <exception cref="SecurityException"> if a security manager exists and
		/// its {@code checkListen} method doesn't allow the operation.
		/// </exception>
		/// <exception cref="IOException"> if an I/O error occurs when opening the socket. </exception>
		/// <exception cref="IllegalArgumentException"> if the port parameter is outside
		///             the specified range of valid port values, which is between
		///             0 and 65535, inclusive.
		/// </exception>
		/// <seealso cref= SocketOptions </seealso>
		/// <seealso cref= SocketImpl </seealso>
		/// <seealso cref= SecurityManager#checkListen
		/// @since   JDK1.1 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ServerSocket(int port, int backlog, InetAddress bindAddr) throws java.io.IOException
		public ServerSocket(int port, int backlog, InetAddress bindAddr)
		{
			SetImpl();
			if (port < 0 || port > 0xFFFF)
			{
				throw new IllegalArgumentException("Port value out of range: " + port);
			}
			if (backlog < 1)
			{
			  backlog = 50;
			}
			try
			{
				Bind(new InetSocketAddress(bindAddr, port), backlog);
			}
			catch (SecurityException e)
			{
				Close();
				throw e;
			}
			catch (IOException e)
			{
				Close();
				throw e;
			}
		}

		/// <summary>
		/// Get the {@code SocketImpl} attached to this socket, creating
		/// it if necessary.
		/// </summary>
		/// <returns>  the {@code SocketImpl} attached to that ServerSocket. </returns>
		/// <exception cref="SocketException"> if creation fails.
		/// @since 1.4 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: SocketImpl getImpl() throws SocketException
		internal virtual SocketImpl Impl
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

		private void CheckOldImpl()
		{
			if (Impl_Renamed == null)
			{
				return;
			}
			// SocketImpl.connect() is a protected method, therefore we need to use
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
			private readonly ServerSocket OuterInstance;

			public PrivilegedExceptionActionAnonymousInnerClassHelper(ServerSocket outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Void run() throws NoSuchMethodException
			public virtual Void Run()
			{
				OuterInstance.Impl_Renamed.GetType().getDeclaredMethod("connect", typeof(SocketAddress), typeof(int));
				return null;
			}
		}

		private void SetImpl()
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
				Impl_Renamed.ServerSocket = this;
			}
		}

		/// <summary>
		/// Creates the socket implementation.
		/// </summary>
		/// <exception cref="IOException"> if creation fails
		/// @since 1.4 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void createImpl() throws SocketException
		internal virtual void CreateImpl()
		{
			if (Impl_Renamed == null)
			{
				SetImpl();
			}
			try
			{
				Impl_Renamed.Create(true);
				Created = true;
			}
			catch (IOException e)
			{
				throw new SocketException(e.Message);
			}
		}

		/// 
		/// <summary>
		/// Binds the {@code ServerSocket} to a specific address
		/// (IP address and port number).
		/// <para>
		/// If the address is {@code null}, then the system will pick up
		/// an ephemeral port and a valid local address to bind the socket.
		/// </para>
		/// <para>
		/// </para>
		/// </summary>
		/// <param name="endpoint">        The IP address and port number to bind to. </param>
		/// <exception cref="IOException"> if the bind operation fails, or if the socket
		///                     is already bound. </exception>
		/// <exception cref="SecurityException">       if a {@code SecurityManager} is present and
		/// its {@code checkListen} method doesn't allow the operation. </exception>
		/// <exception cref="IllegalArgumentException"> if endpoint is a
		///          SocketAddress subclass not supported by this socket
		/// @since 1.4 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void bind(SocketAddress endpoint) throws java.io.IOException
		public virtual void Bind(SocketAddress endpoint)
		{
			Bind(endpoint, 50);
		}

		/// 
		/// <summary>
		/// Binds the {@code ServerSocket} to a specific address
		/// (IP address and port number).
		/// <para>
		/// If the address is {@code null}, then the system will pick up
		/// an ephemeral port and a valid local address to bind the socket.
		/// <P>
		/// The {@code backlog} argument is the requested maximum number of
		/// pending connections on the socket. Its exact semantics are implementation
		/// specific. In particular, an implementation may impose a maximum length
		/// or may choose to ignore the parameter altogther. The value provided
		/// should be greater than {@code 0}. If it is less than or equal to
		/// {@code 0}, then an implementation specific default will be used.
		/// </para>
		/// </summary>
		/// <param name="endpoint">        The IP address and port number to bind to. </param>
		/// <param name="backlog">         requested maximum length of the queue of
		///                          incoming connections. </param>
		/// <exception cref="IOException"> if the bind operation fails, or if the socket
		///                     is already bound. </exception>
		/// <exception cref="SecurityException">       if a {@code SecurityManager} is present and
		/// its {@code checkListen} method doesn't allow the operation. </exception>
		/// <exception cref="IllegalArgumentException"> if endpoint is a
		///          SocketAddress subclass not supported by this socket
		/// @since 1.4 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void bind(SocketAddress endpoint, int backlog) throws java.io.IOException
		public virtual void Bind(SocketAddress endpoint, int backlog)
		{
			if (Closed)
			{
				throw new SocketException("Socket is closed");
			}
			if (!OldImpl && Bound)
			{
				throw new SocketException("Already bound");
			}
			if (endpoint == null)
			{
				endpoint = new InetSocketAddress(0);
			}
			if (!(endpoint is InetSocketAddress))
			{
				throw new IllegalArgumentException("Unsupported address type");
			}
			InetSocketAddress epoint = (InetSocketAddress) endpoint;
			if (epoint.Unresolved)
			{
				throw new SocketException("Unresolved address");
			}
			if (backlog < 1)
			{
			  backlog = 50;
			}
			try
			{
				SecurityManager security = System.SecurityManager;
				if (security != null)
				{
					security.CheckListen(epoint.Port);
				}
				Impl.Bind(epoint.Address, epoint.Port);
				Impl.Listen(backlog);
				Bound_Renamed = true;
			}
			catch (SecurityException e)
			{
				Bound_Renamed = false;
				throw e;
			}
			catch (IOException e)
			{
				Bound_Renamed = false;
				throw e;
			}
		}

		/// <summary>
		/// Returns the local address of this server socket.
		/// <para>
		/// If the socket was bound prior to being <seealso cref="#close closed"/>,
		/// then this method will continue to return the local address
		/// after the socket is closed.
		/// </para>
		/// <para>
		/// If there is a security manager set, its {@code checkConnect} method is
		/// called with the local address and {@code -1} as its arguments to see
		/// if the operation is allowed. If the operation is not allowed,
		/// the <seealso cref="InetAddress#getLoopbackAddress loopback"/> address is returned.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  the address to which this socket is bound,
		///          or the loopback address if denied by the security manager,
		///          or {@code null} if the socket is unbound.
		/// </returns>
		/// <seealso cref= SecurityManager#checkConnect </seealso>
		public virtual InetAddress InetAddress
		{
			get
			{
				if (!Bound)
				{
					return null;
				}
				try
				{
					InetAddress @in = Impl.InetAddress;
					SecurityManager sm = System.SecurityManager;
					if (sm != null)
					{
						sm.CheckConnect(@in.HostAddress, -1);
					}
					return @in;
				}
				catch (SecurityException)
				{
					return InetAddress.LoopbackAddress;
				}
				catch (SocketException)
				{
					// nothing
					// If we're bound, the impl has been created
					// so we shouldn't get here
				}
				return null;
			}
		}

		/// <summary>
		/// Returns the port number on which this socket is listening.
		/// <para>
		/// If the socket was bound prior to being <seealso cref="#close closed"/>,
		/// then this method will continue to return the port number
		/// after the socket is closed.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  the port number to which this socket is listening or
		///          -1 if the socket is not bound yet. </returns>
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
					// nothing
					// If we're bound, the impl has been created
					// so we shouldn't get here
				}
				return -1;
			}
		}

		/// <summary>
		/// Returns the address of the endpoint this socket is bound to.
		/// <para>
		/// If the socket was bound prior to being <seealso cref="#close closed"/>,
		/// then this method will continue to return the address of the endpoint
		/// after the socket is closed.
		/// </para>
		/// <para>
		/// If there is a security manager set, its {@code checkConnect} method is
		/// called with the local address and {@code -1} as its arguments to see
		/// if the operation is allowed. If the operation is not allowed,
		/// a {@code SocketAddress} representing the
		/// <seealso cref="InetAddress#getLoopbackAddress loopback"/> address and the local
		/// port to which the socket is bound is returned.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a {@code SocketAddress} representing the local endpoint of
		///         this socket, or a {@code SocketAddress} representing the
		///         loopback address if denied by the security manager,
		///         or {@code null} if the socket is not bound yet.
		/// </returns>
		/// <seealso cref= #getInetAddress() </seealso>
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
				return new InetSocketAddress(InetAddress, LocalPort);
			}
		}

		/// <summary>
		/// Listens for a connection to be made to this socket and accepts
		/// it. The method blocks until a connection is made.
		/// 
		/// <para>A new Socket {@code s} is created and, if there
		/// is a security manager,
		/// the security manager's {@code checkAccept} method is called
		/// with {@code s.getInetAddress().getHostAddress()} and
		/// {@code s.getPort()}
		/// as its arguments to ensure the operation is allowed.
		/// This could result in a SecurityException.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="IOException">  if an I/O error occurs when waiting for a
		///               connection. </exception>
		/// <exception cref="SecurityException">  if a security manager exists and its
		///             {@code checkAccept} method doesn't allow the operation. </exception>
		/// <exception cref="SocketTimeoutException"> if a timeout was previously set with setSoTimeout and
		///             the timeout has been reached. </exception>
		/// <exception cref="java.nio.channels.IllegalBlockingModeException">
		///             if this socket has an associated channel, the channel is in
		///             non-blocking mode, and there is no connection ready to be
		///             accepted
		/// </exception>
		/// <returns> the new Socket </returns>
		/// <seealso cref= SecurityManager#checkAccept
		/// @revised 1.4
		/// @spec JSR-51 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Socket accept() throws java.io.IOException
		public virtual Socket Accept()
		{
			if (Closed)
			{
				throw new SocketException("Socket is closed");
			}
			if (!Bound)
			{
				throw new SocketException("Socket is not bound yet");
			}
			Socket s = new Socket((SocketImpl) null);
			ImplAccept(s);
			return s;
		}

		/// <summary>
		/// Subclasses of ServerSocket use this method to override accept()
		/// to return their own subclass of socket.  So a FooServerSocket
		/// will typically hand this method an <i>empty</i> FooSocket.  On
		/// return from implAccept the FooSocket will be connected to a client.
		/// </summary>
		/// <param name="s"> the Socket </param>
		/// <exception cref="java.nio.channels.IllegalBlockingModeException">
		///         if this socket has an associated channel,
		///         and the channel is in non-blocking mode </exception>
		/// <exception cref="IOException"> if an I/O error occurs when waiting
		/// for a connection.
		/// @since   JDK1.1
		/// @revised 1.4
		/// @spec JSR-51 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected final void implAccept(Socket s) throws java.io.IOException
		protected internal void ImplAccept(Socket s)
		{
			SocketImpl si = null;
			try
			{
				if (s.Impl_Renamed == null)
				{
				  s.SetImpl();
				}
				else
				{
					s.Impl_Renamed.reset();
				}
				si = s.Impl_Renamed;
				s.Impl_Renamed = null;
				si.Address = new InetAddress();
				si.Fd = new FileDescriptor();
				Impl.Accept(si);

				SecurityManager security = System.SecurityManager;
				if (security != null)
				{
					security.CheckAccept(si.InetAddress.HostAddress, si.Port);
				}
			}
			catch (IOException e)
			{
				if (si != null)
				{
					si.Reset();
				}
				s.Impl_Renamed = si;
				throw e;
			}
			catch (SecurityException e)
			{
				if (si != null)
				{
					si.Reset();
				}
				s.Impl_Renamed = si;
				throw e;
			}
			s.Impl_Renamed = si;
			s.PostAccept();
		}

		/// <summary>
		/// Closes this socket.
		/// 
		/// Any thread currently blocked in <seealso cref="#accept()"/> will throw
		/// a <seealso cref="SocketException"/>.
		/// 
		/// <para> If this socket has an associated channel then the channel is closed
		/// as well.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="IOException">  if an I/O error occurs when closing the socket.
		/// @revised 1.4
		/// @spec JSR-51 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void close() throws java.io.IOException
		public virtual void Close()
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

		/// <summary>
		/// Returns the unique <seealso cref="java.nio.channels.ServerSocketChannel"/> object
		/// associated with this socket, if any.
		/// 
		/// <para> A server socket will have a channel if, and only if, the channel
		/// itself was created via the {@link
		/// java.nio.channels.ServerSocketChannel#open ServerSocketChannel.open}
		/// method.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  the server-socket channel associated with this socket,
		///          or {@code null} if this socket was not created
		///          for a channel
		/// 
		/// @since 1.4
		/// @spec JSR-51 </returns>
		public virtual ServerSocketChannel Channel
		{
			get
			{
				return null;
			}
		}

		/// <summary>
		/// Returns the binding state of the ServerSocket.
		/// </summary>
		/// <returns> true if the ServerSocket successfully bound to an address
		/// @since 1.4 </returns>
		public virtual bool Bound
		{
			get
			{
				// Before 1.3 ServerSockets were always bound during creation
				return Bound_Renamed || OldImpl;
			}
		}

		/// <summary>
		/// Returns the closed state of the ServerSocket.
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
		/// Enable/disable <seealso cref="SocketOptions#SO_TIMEOUT SO_TIMEOUT"/> with the
		/// specified timeout, in milliseconds.  With this option set to a non-zero
		/// timeout, a call to accept() for this ServerSocket
		/// will block for only this amount of time.  If the timeout expires,
		/// a <B>java.net.SocketTimeoutException</B> is raised, though the
		/// ServerSocket is still valid.  The option <B>must</B> be enabled
		/// prior to entering the blocking operation to have effect.  The
		/// timeout must be {@code > 0}.
		/// A timeout of zero is interpreted as an infinite timeout. </summary>
		/// <param name="timeout"> the specified timeout, in milliseconds </param>
		/// <exception cref="SocketException"> if there is an error in
		/// the underlying protocol, such as a TCP error.
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
		/// Enabling <seealso cref="SocketOptions#SO_REUSEADDR SO_REUSEADDR"/> prior to
		/// binding the socket using <seealso cref="#bind(SocketAddress)"/> allows the socket
		/// to be bound even though a previous connection is in a timeout state.
		/// </para>
		/// <para>
		/// When a {@code ServerSocket} is created the initial setting
		/// of <seealso cref="SocketOptions#SO_REUSEADDR SO_REUSEADDR"/> is not defined.
		/// Applications can use <seealso cref="#getReuseAddress()"/> to determine the initial
		/// setting of <seealso cref="SocketOptions#SO_REUSEADDR SO_REUSEADDR"/>.
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
		/// <seealso cref= #isBound() </seealso>
		/// <seealso cref= #isClosed() </seealso>
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
		/// Returns the implementation address and implementation port of
		/// this socket as a {@code String}.
		/// <para>
		/// If there is a security manager set, its {@code checkConnect} method is
		/// called with the local address and {@code -1} as its arguments to see
		/// if the operation is allowed. If the operation is not allowed,
		/// an {@code InetAddress} representing the
		/// <seealso cref="InetAddress#getLoopbackAddress loopback"/> address is returned as
		/// the implementation address.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  a string representation of this socket. </returns>
		public override String ToString()
		{
			if (!Bound)
			{
				return "ServerSocket[unbound]";
			}
			InetAddress @in;
			if (System.SecurityManager != null)
			{
				@in = InetAddress.LoopbackAddress;
			}
			else
			{
				@in = Impl_Renamed.InetAddress;
			}
			return "ServerSocket[addr=" + @in + ",localport=" + Impl_Renamed.LocalPort + "]";
		}

		internal virtual void SetBound()
		{
			Bound_Renamed = true;
		}

		internal virtual void SetCreated()
		{
			Created = true;
		}

		/// <summary>
		/// The factory for all server sockets.
		/// </summary>
		private static SocketImplFactory Factory = null;

		/// <summary>
		/// Sets the server socket implementation factory for the
		/// application. The factory can be specified only once.
		/// <para>
		/// When an application creates a new server socket, the socket
		/// implementation factory's {@code createSocketImpl} method is
		/// called to create the actual socket implementation.
		/// </para>
		/// <para>
		/// Passing {@code null} to the method is a no-op unless the factory
		/// was already set.
		/// </para>
		/// <para>
		/// If there is a security manager, this method first calls
		/// the security manager's {@code checkSetFactory} method
		/// to ensure the operation is allowed.
		/// This could result in a SecurityException.
		/// 
		/// </para>
		/// </summary>
		/// <param name="fac">   the desired factory. </param>
		/// <exception cref="IOException">  if an I/O error occurs when setting the
		///               socket factory. </exception>
		/// <exception cref="SocketException">  if the factory has already been defined. </exception>
		/// <exception cref="SecurityException">  if a security manager exists and its
		///             {@code checkSetFactory} method doesn't allow the operation. </exception>
		/// <seealso cref=        java.net.SocketImplFactory#createSocketImpl() </seealso>
		/// <seealso cref=        SecurityManager#checkSetFactory </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static synchronized void setSocketFactory(SocketImplFactory fac) throws java.io.IOException
		public static SocketImplFactory SocketFactory
		{
			set
			{
				lock (typeof(ServerSocket))
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
		/// Sets a default proposed value for the
		/// <seealso cref="SocketOptions#SO_RCVBUF SO_RCVBUF"/> option for sockets
		/// accepted from this {@code ServerSocket}. The value actually set
		/// in the accepted socket must be determined by calling
		/// <seealso cref="Socket#getReceiveBufferSize()"/> after the socket
		/// is returned by <seealso cref="#accept()"/>.
		/// <para>
		/// The value of <seealso cref="SocketOptions#SO_RCVBUF SO_RCVBUF"/> is used both to
		/// set the size of the internal socket receive buffer, and to set the size
		/// of the TCP receive window that is advertized to the remote peer.
		/// </para>
		/// <para>
		/// It is possible to change the value subsequently, by calling
		/// <seealso cref="Socket#setReceiveBufferSize(int)"/>. However, if the application
		/// wishes to allow a receive window larger than 64K bytes, as defined by RFC1323
		/// then the proposed value must be set in the ServerSocket <B>before</B>
		/// it is bound to a local address. This implies, that the ServerSocket must be
		/// created with the no-argument constructor, then setReceiveBufferSize() must
		/// be called and lastly the ServerSocket is bound to an address by calling bind().
		/// </para>
		/// <para>
		/// Failure to do this will not cause an error, and the buffer size may be set to the
		/// requested value but the TCP receive window in sockets accepted from
		/// this ServerSocket will be no larger than 64K bytes.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="SocketException"> if there is an error
		/// in the underlying protocol, such as a TCP error.
		/// </exception>
		/// <param name="size"> the size to which to set the receive buffer
		/// size. This value must be greater than 0.
		/// </param>
		/// <exception cref="IllegalArgumentException"> if the
		/// value is 0 or is negative.
		/// 
		/// @since 1.4 </exception>
		/// <seealso cref= #getReceiveBufferSize </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public synchronized void setReceiveBufferSize(int size) throws SocketException
		 public virtual int ReceiveBufferSize
		 {
			 set
			 {
				 lock (this)
				 {
					if (!(value > 0))
					{
						throw new IllegalArgumentException("negative receive size");
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
		/// Sets performance preferences for this ServerSocket.
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
		/// compared, with larger values indicating stronger preferences.  If the
		/// application prefers short connection time over both low latency and high
		/// bandwidth, for example, then it could invoke this method with the values
		/// {@code (1, 0, 0)}.  If the application prefers high bandwidth above low
		/// latency, and low latency above short connection time, then it could
		/// invoke this method with the values {@code (0, 1, 2)}.
		/// 
		/// </para>
		/// <para> Invoking this method after this socket has been bound
		/// will have no effect. This implies that in order to use this capability
		/// requires the socket to be created with the no-argument constructor.
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