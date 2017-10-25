using System;
using System.Diagnostics;

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


	/*
	 * This class PlainSocketImpl simply delegates to the appropriate real
	 * SocketImpl. We do this because PlainSocketImpl is already extended
	 * by SocksSocketImpl.
	 * <p>
	 * There are two possibilities for the real SocketImpl,
	 * TwoStacksPlainSocketImpl or DualStackPlainSocketImpl. We use
	 * DualStackPlainSocketImpl on systems that have a dual stack
	 * TCP implementation. Otherwise we create an instance of
	 * TwoStacksPlainSocketImpl and delegate to it.
	 *
	 * @author Chris Hegarty
	 */

	internal class PlainSocketImpl : AbstractPlainSocketImpl
	{
		private AbstractPlainSocketImpl Impl;

		/* the windows version. */
		private static float Version;

		/* java.net.preferIPv4Stack */
		private static bool PreferIPv4Stack = false;

		/* If the version supports a dual stack TCP implementation */
		private static bool UseDualStackImpl = false;

		/* sun.net.useExclusiveBind */
		private static String ExclBindProp;

		/* True if exclusive binding is on for Windows */
		private static bool ExclusiveBind = true;

		static PlainSocketImpl()
		{
			java.security.AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper());

			// (version >= 6.0) implies Vista or greater.
			if (Version >= 6.0 && !PreferIPv4Stack)
			{
					UseDualStackImpl = true;
			}

			if (ExclBindProp != null)
			{
				// sun.net.useExclusiveBind is true
				ExclusiveBind = ExclBindProp.Length() == 0 ? true : Convert.ToBoolean(ExclBindProp);
			}
			else if (Version < 6.0)
			{
				ExclusiveBind = false;
			}
		}

		private class PrivilegedActionAnonymousInnerClassHelper : PrivilegedAction<Object>
		{
			public PrivilegedActionAnonymousInnerClassHelper()
			{
			}

			public virtual Object Run()
			{
				Version = 0;
				try
				{
					Version = Convert.ToSingle(System.Properties.getProperty("os.version"));
					PreferIPv4Stack = Convert.ToBoolean(System.Properties.getProperty("java.net.preferIPv4Stack"));
					ExclBindProp = System.getProperty("sun.net.useExclusiveBind");
				}
				catch (NumberFormatException e)
				{
					Debug.Assert(false, e);
				}
				return null; // nothing to return
			}
		}

		/// <summary>
		/// Constructs an empty instance.
		/// </summary>
		internal PlainSocketImpl()
		{
			if (UseDualStackImpl)
			{
				Impl = new DualStackPlainSocketImpl(ExclusiveBind);
			}
			else
			{
				Impl = new TwoStacksPlainSocketImpl(ExclusiveBind);
			}
		}

		/// <summary>
		/// Constructs an instance with the given file descriptor.
		/// </summary>
		internal PlainSocketImpl(FileDescriptor fd)
		{
			if (UseDualStackImpl)
			{
				Impl = new DualStackPlainSocketImpl(fd, ExclusiveBind);
			}
			else
			{
				Impl = new TwoStacksPlainSocketImpl(fd, ExclusiveBind);
			}
		}

		// Override methods in SocketImpl that access impl's fields.

		protected internal override FileDescriptor FileDescriptor
		{
			get
			{
				return Impl.FileDescriptor;
			}
			set
			{
				Impl.FileDescriptor = value;
			}
		}

		protected internal override InetAddress InetAddress
		{
			get
			{
				return Impl.InetAddress;
			}
		}

		protected internal override int Port
		{
			get
			{
				return Impl.Port;
			}
			set
			{
				Impl.Port = value;
			}
		}

		protected internal override int LocalPort
		{
			get
			{
				return Impl.LocalPort;
			}
			set
			{
				Impl.LocalPort = value;
			}
		}

		internal override Socket Socket
		{
			set
			{
				Impl.Socket = value;
			}
			get
			{
				return Impl.Socket;
			}
		}


		internal override ServerSocket ServerSocket
		{
			set
			{
				Impl.ServerSocket = value;
			}
			get
			{
				return Impl.ServerSocket;
			}
		}


		public override String ToString()
		{
			return Impl.ToString();
		}

		// Override methods in AbstractPlainSocketImpl that access impl's fields.

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected synchronized void create(boolean stream) throws IOException
		protected internal override void Create(bool stream)
		{
			lock (this)
			{
				Impl.Create(stream);
        
				// set fd to delegate's fd to be compatible with older releases
				this.Fd = Impl.Fd;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void connect(String host, int port) throws UnknownHostException, IOException
		protected internal override void Connect(String host, int port)
		{
			Impl.Connect(host, port);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void connect(InetAddress address, int port) throws IOException
		protected internal override void Connect(InetAddress address, int port)
		{
			Impl.Connect(address, port);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void connect(SocketAddress address, int timeout) throws IOException
		protected internal override void Connect(SocketAddress address, int timeout)
		{
			Impl.Connect(address, timeout);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setOption(int opt, Object val) throws SocketException
		public override void SetOption(int opt, Object val)
		{
			Impl.SetOption(opt, val);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object getOption(int opt) throws SocketException
		public override Object GetOption(int opt)
		{
			return Impl.GetOption(opt);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: synchronized void doConnect(InetAddress address, int port, int timeout) throws IOException
		internal override void DoConnect(InetAddress address, int port, int timeout)
		{
			lock (this)
			{
				Impl.DoConnect(address, port, timeout);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected synchronized void bind(InetAddress address, int lport) throws IOException
		protected internal override void Bind(InetAddress address, int lport)
		{
			lock (this)
			{
				Impl.Bind(address, lport);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected synchronized void accept(SocketImpl s) throws IOException
		protected internal override void Accept(SocketImpl s)
		{
			lock (this)
			{
				if (s is PlainSocketImpl)
				{
					// pass in the real impl not the wrapper.
					SocketImpl @delegate = ((PlainSocketImpl)s).Impl;
					@delegate.Address = new InetAddress();
					@delegate.Fd = new FileDescriptor();
					Impl.Accept(@delegate);
					// set fd to delegate's fd to be compatible with older releases
					s.Fd = @delegate.Fd;
				}
				else
				{
					Impl.Accept(s);
				}
			}
		}


		internal override InetAddress Address
		{
			set
			{
				Impl.Address = value;
			}
		}



//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected synchronized InputStream getInputStream() throws IOException
		protected internal override InputStream GetInputStream()
		{
			lock (this)
			{
				return Impl.GetInputStream();
			}
		}

		internal override void SetInputStream(SocketInputStream @in)
		{
			Impl.SetInputStream(@in);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected synchronized OutputStream getOutputStream() throws IOException
		protected internal override OutputStream OutputStream
		{
			get
			{
				lock (this)
				{
					return Impl.OutputStream;
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void close() throws IOException
		protected internal override void Close()
		{
			try
			{
				Impl.Close();
			}
			finally
			{
				// set fd to delegate's fd to be compatible with older releases
				this.Fd = null;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void reset() throws IOException
		internal override void Reset()
		{
			try
			{
				Impl.Reset();
			}
			finally
			{
				// set fd to delegate's fd to be compatible with older releases
				this.Fd = null;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void shutdownInput() throws IOException
		protected internal override void ShutdownInput()
		{
			Impl.ShutdownInput();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void shutdownOutput() throws IOException
		protected internal override void ShutdownOutput()
		{
			Impl.ShutdownOutput();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void sendUrgentData(int data) throws IOException
		protected internal override void SendUrgentData(int data)
		{
			Impl.SendUrgentData(data);
		}

		internal override FileDescriptor AcquireFD()
		{
			return Impl.AcquireFD();
		}

		internal override void ReleaseFD()
		{
			Impl.ReleaseFD();
		}

		public override bool ConnectionReset
		{
			get
			{
				return Impl.ConnectionReset;
			}
		}

		public override bool ConnectionResetPending
		{
			get
			{
				return Impl.ConnectionResetPending;
			}
		}

		public override void SetConnectionReset()
		{
			Impl.SetConnectionReset();
		}

		public override void SetConnectionResetPending()
		{
			Impl.SetConnectionResetPending();
		}

		public override bool ClosedOrPending
		{
			get
			{
				return Impl.ClosedOrPending;
			}
		}

		public override int Timeout
		{
			get
			{
				return Impl.Timeout;
			}
		}

		// Override methods in AbstractPlainSocketImpl that need to be implemented.

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void socketCreate(boolean isServer) throws IOException
		internal override void SocketCreate(bool isServer)
		{
			Impl.SocketCreate(isServer);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void socketConnect(InetAddress address, int port, int timeout) throws IOException
		internal override void SocketConnect(InetAddress address, int port, int timeout)
		{
			Impl.SocketConnect(address, port, timeout);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void socketBind(InetAddress address, int port) throws IOException
		internal override void SocketBind(InetAddress address, int port)
		{
			Impl.SocketBind(address, port);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void socketListen(int count) throws IOException
		internal override void SocketListen(int count)
		{
			Impl.SocketListen(count);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void socketAccept(SocketImpl s) throws IOException
		internal override void SocketAccept(SocketImpl s)
		{
			Impl.SocketAccept(s);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: int socketAvailable() throws IOException
		internal override int SocketAvailable()
		{
			return Impl.SocketAvailable();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void socketClose0(boolean useDeferredClose) throws IOException
		internal override void SocketClose0(bool useDeferredClose)
		{
			Impl.SocketClose0(useDeferredClose);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void socketShutdown(int howto) throws IOException
		internal override void SocketShutdown(int howto)
		{
			Impl.SocketShutdown(howto);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void socketSetOption(int cmd, boolean on, Object value) throws SocketException
		internal override void SocketSetOption(int cmd, bool on, Object value)
		{
			Impl.SocketSetOption(cmd, on, value);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: int socketGetOption(int opt, Object iaContainerObj) throws SocketException
		internal override int SocketGetOption(int opt, Object iaContainerObj)
		{
			return Impl.SocketGetOption(opt, iaContainerObj);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void socketSendUrgentData(int data) throws IOException
		internal override void SocketSendUrgentData(int data)
		{
			Impl.SocketSendUrgentData(data);
		}
	}

}