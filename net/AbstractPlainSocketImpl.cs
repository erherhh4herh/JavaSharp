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


	using ConnectionResetException = sun.net.ConnectionResetException;
	using NetHooks = sun.net.NetHooks;
	using ResourceManager = sun.net.ResourceManager;

	/// <summary>
	/// Default Socket Implementation. This implementation does
	/// not implement any security checks.
	/// Note this class should <b>NOT</b> be public.
	/// 
	/// @author  Steven B. Byrne
	/// </summary>
	internal abstract class AbstractPlainSocketImpl : SocketImpl
	{
		/* instance variable for SO_TIMEOUT */
		internal int Timeout_Renamed; // timeout in millisec
		// traffic class
		private int TrafficClass;

		private bool Shut_rd = false;
		private bool Shut_wr = false;

		private SocketInputStream SocketInputStream = null;
		private SocketOutputStream SocketOutputStream = null;

		/* number of threads using the FileDescriptor */
		protected internal int FdUseCount = 0;

		/* lock when increment/decrementing fdUseCount */
		protected internal readonly Object FdLock = new Object();

		/* indicates a close is pending on the file descriptor */
		protected internal bool ClosePending = false;

		/* indicates connection reset state */
		private int CONNECTION_NOT_RESET = 0;
		private int CONNECTION_RESET_PENDING = 1;
		private int CONNECTION_RESET = 2;
		private int ResetState;
		private readonly Object ResetLock = new Object();

	   /* whether this Socket is a stream (TCP) socket or not (UDP)
	    */
		protected internal bool Stream;

		/// <summary>
		/// Load net library into runtime.
		/// </summary>
		static AbstractPlainSocketImpl()
		{
			java.security.AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper());
		}

		private class PrivilegedActionAnonymousInnerClassHelper : java.security.PrivilegedAction<Void>
		{
			public PrivilegedActionAnonymousInnerClassHelper()
			{
			}

			public virtual Void Run()
			{
//JAVA TO C# CONVERTER TODO TASK: The library is specified in the 'DllImport' attribute for .NET:
//				System.loadLibrary("net");
				return null;
			}
		}

		/// <summary>
		/// Creates a socket with a boolean that specifies whether this
		/// is a stream socket (true) or an unconnected UDP socket (false).
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected synchronized void create(boolean stream) throws java.io.IOException
		protected internal override void Create(bool stream)
		{
			lock (this)
			{
				this.Stream = stream;
				if (!stream)
				{
					ResourceManager.beforeUdpCreate();
					// only create the fd after we know we will be able to create the socket
					Fd = new FileDescriptor();
					try
					{
						SocketCreate(false);
					}
					catch (IOException ioe)
					{
						ResourceManager.afterUdpClose();
						Fd = null;
						throw ioe;
					}
				}
				else
				{
					Fd = new FileDescriptor();
					SocketCreate(true);
				}
				if (Socket_Renamed != null)
				{
					Socket_Renamed.SetCreated();
				}
				if (ServerSocket_Renamed != null)
				{
					ServerSocket_Renamed.SetCreated();
				}
			}
		}

		/// <summary>
		/// Creates a socket and connects it to the specified port on
		/// the specified host. </summary>
		/// <param name="host"> the specified host </param>
		/// <param name="port"> the specified port </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void connect(String host, int port) throws UnknownHostException, java.io.IOException
		protected internal override void Connect(String host, int port)
		{
			bool connected = false;
			try
			{
				InetAddress address = InetAddress.GetByName(host);
				this.Port_Renamed = port;
				this.Address = address;

				ConnectToAddress(address, port, Timeout_Renamed);
				connected = true;
			}
			finally
			{
				if (!connected)
				{
					try
					{
						Close();
					}
					catch (IOException)
					{
						/* Do nothing. If connect threw an exception then
						   it will be passed up the call stack */
					}
				}
			}
		}

		/// <summary>
		/// Creates a socket and connects it to the specified address on
		/// the specified port. </summary>
		/// <param name="address"> the address </param>
		/// <param name="port"> the specified port </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void connect(InetAddress address, int port) throws java.io.IOException
		protected internal override void Connect(InetAddress address, int port)
		{
			this.Port_Renamed = port;
			this.Address = address;

			try
			{
				ConnectToAddress(address, port, Timeout_Renamed);
				return;
			}
			catch (IOException e)
			{
				// everything failed
				Close();
				throw e;
			}
		}

		/// <summary>
		/// Creates a socket and connects it to the specified address on
		/// the specified port. </summary>
		/// <param name="address"> the address </param>
		/// <param name="timeout"> the timeout value in milliseconds, or zero for no timeout. </param>
		/// <exception cref="IOException"> if connection fails </exception>
		/// <exception cref="IllegalArgumentException"> if address is null or is a
		///          SocketAddress subclass not supported by this socket
		/// @since 1.4 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void connect(SocketAddress address, int timeout) throws java.io.IOException
		protected internal override void Connect(SocketAddress address, int timeout)
		{
			bool connected = false;
			try
			{
				if (address == null || !(address is InetSocketAddress))
				{
					throw new IllegalArgumentException("unsupported address type");
				}
				InetSocketAddress addr = (InetSocketAddress) address;
				if (addr.Unresolved)
				{
					throw new UnknownHostException(addr.HostName);
				}
				this.Port_Renamed = addr.Port;
				this.Address = addr.Address;

				ConnectToAddress(this.Address, Port_Renamed, timeout);
				connected = true;
			}
			finally
			{
				if (!connected)
				{
					try
					{
						Close();
					}
					catch (IOException)
					{
						/* Do nothing. If connect threw an exception then
						   it will be passed up the call stack */
					}
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void connectToAddress(InetAddress address, int port, int timeout) throws java.io.IOException
		private void ConnectToAddress(InetAddress address, int port, int timeout)
		{
			if (address.AnyLocalAddress)
			{
				DoConnect(InetAddress.LocalHost, port, timeout);
			}
			else
			{
				DoConnect(address, port, timeout);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setOption(int opt, Object val) throws SocketException
		public override void SetOption(int opt, Object val)
		{
			if (ClosedOrPending)
			{
				throw new SocketException("Socket Closed");
			}
			bool on = true;
			switch (opt)
			{
				/* check type safety b4 going native.  These should never
				 * fail, since only java.Socket* has access to
				 * PlainSocketImpl.setOption().
				 */
			case SocketOptions_Fields.SO_LINGER:
				if (val == null || (!(val is Integer) && !(val is Boolean)))
				{
					throw new SocketException("Bad parameter for option");
				}
				if (val is Boolean)
				{
					/* true only if disabling - enabling should be Integer */
					on = false;
				}
				break;
			case SocketOptions_Fields.SO_TIMEOUT:
				if (val == null || (!(val is Integer)))
				{
					throw new SocketException("Bad parameter for SO_TIMEOUT");
				}
				int tmp = ((Integer) val).IntValue();
				if (tmp < 0)
				{
					throw new IllegalArgumentException("timeout < 0");
				}
				Timeout_Renamed = tmp;
				break;
			case SocketOptions_Fields.IP_TOS:
				 if (val == null || !(val is Integer))
				 {
					 throw new SocketException("bad argument for IP_TOS");
				 }
				 TrafficClass = ((Integer)val).IntValue();
				 break;
			case SocketOptions_Fields.SO_BINDADDR:
				throw new SocketException("Cannot re-bind socket");
			case SocketOptions_Fields.TCP_NODELAY:
				if (val == null || !(val is Boolean))
				{
					throw new SocketException("bad parameter for TCP_NODELAY");
				}
				on = ((Boolean)val).BooleanValue();
				break;
			case SocketOptions_Fields.SO_SNDBUF:
			case SocketOptions_Fields.SO_RCVBUF:
				if (val == null || !(val is Integer) || !(((Integer)val).IntValue() > 0))
				{
					throw new SocketException("bad parameter for SO_SNDBUF " + "or SO_RCVBUF");
				}
				break;
			case SocketOptions_Fields.SO_KEEPALIVE:
				if (val == null || !(val is Boolean))
				{
					throw new SocketException("bad parameter for SO_KEEPALIVE");
				}
				on = ((Boolean)val).BooleanValue();
				break;
			case SocketOptions_Fields.SO_OOBINLINE:
				if (val == null || !(val is Boolean))
				{
					throw new SocketException("bad parameter for SO_OOBINLINE");
				}
				on = ((Boolean)val).BooleanValue();
				break;
			case SocketOptions_Fields.SO_REUSEADDR:
				if (val == null || !(val is Boolean))
				{
					throw new SocketException("bad parameter for SO_REUSEADDR");
				}
				on = ((Boolean)val).BooleanValue();
				break;
			default:
				throw new SocketException("unrecognized TCP option: " + opt);
			}
			SocketSetOption(opt, on, val);
		}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object getOption(int opt) throws SocketException
		public override Object GetOption(int opt)
		{
			if (ClosedOrPending)
			{
				throw new SocketException("Socket Closed");
			}
			if (opt == SocketOptions_Fields.SO_TIMEOUT)
			{
				return new Integer(Timeout_Renamed);
			}
			int ret = 0;
			/*
			 * The native socketGetOption() knows about 3 options.
			 * The 32 bit value it returns will be interpreted according
			 * to what we're asking.  A return of -1 means it understands
			 * the option but its turned off.  It will raise a SocketException
			 * if "opt" isn't one it understands.
			 */

			switch (opt)
			{
			case SocketOptions_Fields.TCP_NODELAY:
				ret = SocketGetOption(opt, null);
				return Convert.ToBoolean(ret != -1);
			case SocketOptions_Fields.SO_OOBINLINE:
				ret = SocketGetOption(opt, null);
				return Convert.ToBoolean(ret != -1);
			case SocketOptions_Fields.SO_LINGER:
				ret = SocketGetOption(opt, null);
				return (ret == -1) ? false: (Object)(new Integer(ret));
			case SocketOptions_Fields.SO_REUSEADDR:
				ret = SocketGetOption(opt, null);
				return Convert.ToBoolean(ret != -1);
			case SocketOptions_Fields.SO_BINDADDR:
				InetAddressContainer @in = new InetAddressContainer();
				ret = SocketGetOption(opt, @in);
				return @in.Addr;
			case SocketOptions_Fields.SO_SNDBUF:
			case SocketOptions_Fields.SO_RCVBUF:
				ret = SocketGetOption(opt, null);
				return new Integer(ret);
			case SocketOptions_Fields.IP_TOS:
				try
				{
					ret = SocketGetOption(opt, null);
					if (ret == -1) // ipv6 tos
					{
						return TrafficClass;
					}
					else
					{
						return ret;
					}
				}
				catch (SocketException)
				{
					// TODO - should make better effort to read TOS or TCLASS
					return TrafficClass; // ipv6 tos
				}
			case SocketOptions_Fields.SO_KEEPALIVE:
				ret = SocketGetOption(opt, null);
				return Convert.ToBoolean(ret != -1);
			// should never get here
			default:
				return null;
			}
		}

		/// <summary>
		/// The workhorse of the connection operation.  Tries several times to
		/// establish a connection to the given <host, port>.  If unsuccessful,
		/// throws an IOException indicating what went wrong.
		/// </summary>

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: synchronized void doConnect(InetAddress address, int port, int timeout) throws java.io.IOException
		internal virtual void DoConnect(InetAddress address, int port, int timeout)
		{
			lock (this)
			{
				lock (FdLock)
				{
					if (!ClosePending && (Socket_Renamed == null || !Socket_Renamed.Bound))
					{
						NetHooks.beforeTcpConnect(Fd, address, port);
					}
				}
				try
				{
					AcquireFD();
					try
					{
						SocketConnect(address, port, timeout);
						/* socket may have been closed during poll/select */
						lock (FdLock)
						{
							if (ClosePending)
							{
								throw new SocketException("Socket closed");
							}
						}
						// If we have a ref. to the Socket, then sets the flags
						// created, bound & connected to true.
						// This is normally done in Socket.connect() but some
						// subclasses of Socket may call impl.connect() directly!
						if (Socket_Renamed != null)
						{
							Socket_Renamed.SetBound();
							Socket_Renamed.SetConnected();
						}
					}
					finally
					{
						ReleaseFD();
					}
				}
				catch (IOException e)
				{
					Close();
					throw e;
				}
			}
		}

		/// <summary>
		/// Binds the socket to the specified address of the specified local port. </summary>
		/// <param name="address"> the address </param>
		/// <param name="lport"> the port </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected synchronized void bind(InetAddress address, int lport) throws java.io.IOException
		protected internal override void Bind(InetAddress address, int lport)
		{
			lock (this)
			{
			   lock (FdLock)
			   {
					if (!ClosePending && (Socket_Renamed == null || !Socket_Renamed.Bound))
					{
						NetHooks.beforeTcpBind(Fd, address, lport);
					}
			   }
				SocketBind(address, lport);
				if (Socket_Renamed != null)
				{
					Socket_Renamed.SetBound();
				}
				if (ServerSocket_Renamed != null)
				{
					ServerSocket_Renamed.SetBound();
				}
			}
		}

		/// <summary>
		/// Listens, for a specified amount of time, for connections. </summary>
		/// <param name="count"> the amount of time to listen for connections </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected synchronized void listen(int count) throws java.io.IOException
		protected internal override void Listen(int count)
		{
			lock (this)
			{
				SocketListen(count);
			}
		}

		/// <summary>
		/// Accepts connections. </summary>
		/// <param name="s"> the connection </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void accept(SocketImpl s) throws java.io.IOException
		protected internal override void Accept(SocketImpl s)
		{
			AcquireFD();
			try
			{
				SocketAccept(s);
			}
			finally
			{
				ReleaseFD();
			}
		}

		/// <summary>
		/// Gets an InputStream for this socket.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected synchronized java.io.InputStream getInputStream() throws java.io.IOException
		protected internal override InputStream GetInputStream()
		{
			lock (this)
			{
				lock (FdLock)
				{
					if (ClosedOrPending)
					{
						throw new IOException("Socket Closed");
					}
					if (Shut_rd)
					{
						throw new IOException("Socket input is shutdown");
					}
					if (SocketInputStream == null)
					{
						SocketInputStream = new SocketInputStream(this);
					}
				}
				return SocketInputStream;
			}
		}

		internal virtual void SetInputStream(SocketInputStream @in)
		{
			SocketInputStream = @in;
		}

		/// <summary>
		/// Gets an OutputStream for this socket.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected synchronized java.io.OutputStream getOutputStream() throws java.io.IOException
		protected internal override OutputStream OutputStream
		{
			get
			{
				lock (this)
				{
					lock (FdLock)
					{
						if (ClosedOrPending)
						{
							throw new IOException("Socket Closed");
						}
						if (Shut_wr)
						{
							throw new IOException("Socket output is shutdown");
						}
						if (SocketOutputStream == null)
						{
							SocketOutputStream = new SocketOutputStream(this);
						}
					}
					return SocketOutputStream;
				}
			}
		}

		internal virtual FileDescriptor FileDescriptor
		{
			set
			{
				this.Fd = value;
			}
		}

		internal virtual InetAddress Address
		{
			set
			{
				this.Address = value;
			}
		}

		internal virtual int Port
		{
			set
			{
				this.Port_Renamed = value;
			}
		}

		internal virtual int LocalPort
		{
			set
			{
				this.Localport = value;
			}
		}

		/// <summary>
		/// Returns the number of bytes that can be read without blocking.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected synchronized int available() throws java.io.IOException
		protected internal override int Available()
		{
			lock (this)
			{
				if (ClosedOrPending)
				{
					throw new IOException("Stream closed.");
				}
        
				/*
				 * If connection has been reset or shut down for input, then return 0
				 * to indicate there are no buffered bytes.
				 */
				if (ConnectionReset || Shut_rd)
				{
					return 0;
				}
        
				/*
				 * If no bytes available and we were previously notified
				 * of a connection reset then we move to the reset state.
				 *
				 * If are notified of a connection reset then check
				 * again if there are bytes buffered on the socket.
				 */
				int n = 0;
				try
				{
					n = SocketAvailable();
					if (n == 0 && ConnectionResetPending)
					{
						SetConnectionReset();
					}
				}
				catch (ConnectionResetException)
				{
					SetConnectionResetPending();
					try
					{
						n = SocketAvailable();
						if (n == 0)
						{
							SetConnectionReset();
						}
					}
					catch (ConnectionResetException)
					{
					}
				}
				return n;
			}
		}

		/// <summary>
		/// Closes the socket.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void close() throws java.io.IOException
		protected internal override void Close()
		{
			lock (FdLock)
			{
				if (Fd != null)
				{
					if (!Stream)
					{
						ResourceManager.afterUdpClose();
					}
					if (FdUseCount == 0)
					{
						if (ClosePending)
						{
							return;
						}
						ClosePending = true;
						/*
						 * We close the FileDescriptor in two-steps - first the
						 * "pre-close" which closes the socket but doesn't
						 * release the underlying file descriptor. This operation
						 * may be lengthy due to untransmitted data and a long
						 * linger interval. Once the pre-close is done we do the
						 * actual socket to release the fd.
						 */
						try
						{
							SocketPreClose();
						}
						finally
						{
							SocketClose();
						}
						Fd = null;
						return;
					}
					else
					{
						/*
						 * If a thread has acquired the fd and a close
						 * isn't pending then use a deferred close.
						 * Also decrement fdUseCount to signal the last
						 * thread that releases the fd to close it.
						 */
						if (!ClosePending)
						{
							ClosePending = true;
							FdUseCount--;
							SocketPreClose();
						}
					}
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void reset() throws java.io.IOException
		internal override void Reset()
		{
			if (Fd != null)
			{
				SocketClose();
			}
			Fd = null;
			base.Reset();
		}


		/// <summary>
		/// Shutdown read-half of the socket connection;
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void shutdownInput() throws java.io.IOException
		protected internal override void ShutdownInput()
		{
		  if (Fd != null)
		  {
			  SocketShutdown(SHUT_RD);
			  if (SocketInputStream != null)
			  {
				  SocketInputStream.EOF = true;
			  }
			  Shut_rd = true;
		  }
		}

		/// <summary>
		/// Shutdown write-half of the socket connection;
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void shutdownOutput() throws java.io.IOException
		protected internal override void ShutdownOutput()
		{
		  if (Fd != null)
		  {
			  SocketShutdown(SHUT_WR);
			  Shut_wr = true;
		  }
		}

		protected internal override bool SupportsUrgentData()
		{
			return true;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void sendUrgentData(int data) throws java.io.IOException
		protected internal override void SendUrgentData(int data)
		{
			if (Fd == null)
			{
				throw new IOException("Socket Closed");
			}
			SocketSendUrgentData(data);
		}

		/// <summary>
		/// Cleans up if the user forgets to close it.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void finalize() throws java.io.IOException
		~AbstractPlainSocketImpl()
		{
			Close();
		}

		/*
		 * "Acquires" and returns the FileDescriptor for this impl
		 *
		 * A corresponding releaseFD is required to "release" the
		 * FileDescriptor.
		 */
		internal virtual FileDescriptor AcquireFD()
		{
			lock (FdLock)
			{
				FdUseCount++;
				return Fd;
			}
		}

		/*
		 * "Release" the FileDescriptor for this impl.
		 *
		 * If the use count goes to -1 then the socket is closed.
		 */
		internal virtual void ReleaseFD()
		{
			lock (FdLock)
			{
				FdUseCount--;
				if (FdUseCount == -1)
				{
					if (Fd != null)
					{
						try
						{
							SocketClose();
						}
						catch (IOException)
						{
						}
						finally
						{
							Fd = null;
						}
					}
				}
			}
		}

		public virtual bool ConnectionReset
		{
			get
			{
				lock (ResetLock)
				{
					return (ResetState == CONNECTION_RESET);
				}
			}
		}

		public virtual bool ConnectionResetPending
		{
			get
			{
				lock (ResetLock)
				{
					return (ResetState == CONNECTION_RESET_PENDING);
				}
			}
		}

		public virtual void SetConnectionReset()
		{
			lock (ResetLock)
			{
				ResetState = CONNECTION_RESET;
			}
		}

		public virtual void SetConnectionResetPending()
		{
			lock (ResetLock)
			{
				if (ResetState == CONNECTION_NOT_RESET)
				{
					ResetState = CONNECTION_RESET_PENDING;
				}
			}

		}

		/*
		 * Return true if already closed or close is pending
		 */
		public virtual bool ClosedOrPending
		{
			get
			{
				/*
				 * Lock on fdLock to ensure that we wait if a
				 * close is in progress.
				 */
				lock (FdLock)
				{
					if (ClosePending || (Fd == null))
					{
						return true;
					}
					else
					{
						return false;
					}
				}
			}
		}

		/*
		 * Return the current value of SO_TIMEOUT
		 */
		public virtual int Timeout
		{
			get
			{
				return Timeout_Renamed;
			}
		}

		/*
		 * "Pre-close" a socket by dup'ing the file descriptor - this enables
		 * the socket to be closed without releasing the file descriptor.
		 */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void socketPreClose() throws java.io.IOException
		private void SocketPreClose()
		{
			SocketClose0(true);
		}

		/*
		 * Close the socket (and release the file descriptor).
		 */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void socketClose() throws java.io.IOException
		protected internal virtual void SocketClose()
		{
			SocketClose0(false);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: abstract void socketCreate(boolean isServer) throws java.io.IOException;
		internal abstract void SocketCreate(bool isServer);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: abstract void socketConnect(InetAddress address, int port, int timeout) throws java.io.IOException;
		internal abstract void SocketConnect(InetAddress address, int port, int timeout);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: abstract void socketBind(InetAddress address, int port) throws java.io.IOException;
		internal abstract void SocketBind(InetAddress address, int port);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: abstract void socketListen(int count) throws java.io.IOException;
		internal abstract void SocketListen(int count);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: abstract void socketAccept(SocketImpl s) throws java.io.IOException;
		internal abstract void SocketAccept(SocketImpl s);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: abstract int socketAvailable() throws java.io.IOException;
		internal abstract int SocketAvailable();
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: abstract void socketClose0(boolean useDeferredClose) throws java.io.IOException;
		internal abstract void SocketClose0(bool useDeferredClose);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: abstract void socketShutdown(int howto) throws java.io.IOException;
		internal abstract void SocketShutdown(int howto);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: abstract void socketSetOption(int cmd, boolean on, Object value) throws SocketException;
		internal abstract void SocketSetOption(int cmd, bool on, Object value);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: abstract int socketGetOption(int opt, Object iaContainerObj) throws SocketException;
		internal abstract int SocketGetOption(int opt, Object iaContainerObj);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: abstract void socketSendUrgentData(int data) throws java.io.IOException;
		internal abstract void SocketSendUrgentData(int data);

		public const int SHUT_RD = 0;
		public const int SHUT_WR = 1;
	}

}