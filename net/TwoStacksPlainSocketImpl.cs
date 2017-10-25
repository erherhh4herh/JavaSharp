using System.Runtime.InteropServices;

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

	using ResourceManager = sun.net.ResourceManager;

	/*
	 * This class defines the plain SocketImpl that is used for all
	 * Windows version lower than Vista. It adds support for IPv6 on
	 * these platforms where available.
	 *
	 * For backward compatibility Windows platforms that do not have IPv6
	 * support also use this implementation, and fd1 gets set to null
	 * during socket creation.
	 *
	 * @author Chris Hegarty
	 */

	internal class TwoStacksPlainSocketImpl : AbstractPlainSocketImpl
	{
		/* second fd, used for ipv6 on windows only.
		 * fd1 is used for listeners and for client sockets at initialization
		 * until the socket is connected. Up to this point fd always refers
		 * to the ipv4 socket and fd1 to the ipv6 socket. After the socket
		 * becomes connected, fd always refers to the connected socket
		 * (either v4 or v6) and fd1 is closed.
		 *
		 * For ServerSockets, fd always refers to the v4 listener and
		 * fd1 the v6 listener.
		 */
		private FileDescriptor Fd1;

		/*
		 * Needed for ipv6 on windows because we need to know
		 * if the socket is bound to ::0 or 0.0.0.0, when a caller
		 * asks for it. Otherwise we don't know which socket to ask.
		 */
		private InetAddress AnyLocalBoundAddr = null;

		/* to prevent starvation when listening on two sockets, this is
		 * is used to hold the id of the last socket we accepted on.
		 */
		private int Lastfd = -1;

		// true if this socket is exclusively bound
		private readonly bool ExclusiveBind;

		// emulates SO_REUSEADDR when exclusiveBind is true
		private bool IsReuseAddress;

		static TwoStacksPlainSocketImpl()
		{
			initProto();
		}

		public TwoStacksPlainSocketImpl(bool exclBind)
		{
			ExclusiveBind = exclBind;
		}

		public TwoStacksPlainSocketImpl(FileDescriptor fd, bool exclBind)
		{
			this.Fd = fd;
			ExclusiveBind = exclBind;
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
				Fd1 = new FileDescriptor();
				try
				{
					base.Create(stream);
				}
				catch (IOException e)
				{
					Fd1 = null;
					throw e;
				}
			}
		}

		 /// <summary>
		 /// Binds the socket to the specified address of the specified local port. </summary>
		 /// <param name="address"> the address </param>
		 /// <param name="port"> the port </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected synchronized void bind(InetAddress address, int lport) throws java.io.IOException
		protected internal override void Bind(InetAddress address, int lport)
		{
			lock (this)
			{
				base.Bind(address, lport);
				if (address.AnyLocalAddress)
				{
					AnyLocalBoundAddr = address;
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object getOption(int opt) throws SocketException
		public override Object GetOption(int opt)
		{
			if (ClosedOrPending)
			{
				throw new SocketException("Socket Closed");
			}
			if (opt == SocketOptions_Fields.SO_BINDADDR)
			{
				if (Fd != null && Fd1 != null)
				{
					/* must be unbound or else bound to anyLocal */
					return AnyLocalBoundAddr;
				}
				InetAddressContainer @in = new InetAddressContainer();
				SocketGetOption(opt, @in);
				return @in.Addr;
			}
			else if (opt == SocketOptions_Fields.SO_REUSEADDR && ExclusiveBind)
			{
				// SO_REUSEADDR emulated when using exclusive bind
				return IsReuseAddress;
			}
			else
			{
				return base.GetOption(opt);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override void socketBind(InetAddress address, int port) throws java.io.IOException
		internal override void SocketBind(InetAddress address, int port)
		{
			socketBind(address, port, ExclusiveBind);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override void socketSetOption(int opt, boolean on, Object value) throws SocketException
		internal override void SocketSetOption(int opt, bool on, Object value)
		{
			// SO_REUSEADDR emulated when using exclusive bind
			if (opt == SocketOptions_Fields.SO_REUSEADDR && ExclusiveBind)
			{
				IsReuseAddress = on;
			}
			else
			{
				socketNativeSetOption(opt, on, value);
			}
		}

		/// <summary>
		/// Closes the socket.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void close() throws java.io.IOException
		protected internal override void Close()
		{
			lock (FdLock)
			{
				if (Fd != null || Fd1 != null)
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
						SocketClose();
						Fd = null;
						Fd1 = null;
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
							SocketClose();
						}
					}
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override void reset() throws java.io.IOException
		internal override void Reset()
		{
			if (Fd != null || Fd1 != null)
			{
				SocketClose();
			}
			Fd = null;
			Fd1 = null;
			base.Reset();
		}

		/*
		 * Return true if already closed or close is pending
		 */
		public override bool ClosedOrPending
		{
			get
			{
				/*
				 * Lock on fdLock to ensure that we wait if a
				 * close is in progress.
				 */
				lock (FdLock)
				{
					if (ClosePending || (Fd == null && Fd1 == null))
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

		/* Native methods */

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		internal static extern void initProto();

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		internal extern void socketCreate(bool isServer);

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		internal extern void socketConnect(InetAddress address, int port, int timeout);

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		internal extern void socketBind(InetAddress address, int port, bool exclBind);

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		internal extern void socketListen(int count);

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		internal extern void socketAccept(SocketImpl s);

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		internal extern int socketAvailable();

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		internal extern void socketClose0(bool useDeferredClose);

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		internal extern void socketShutdown(int howto);

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		internal extern void socketNativeSetOption(int cmd, bool on, Object value);

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		internal extern int socketGetOption(int opt, Object iaContainerObj);

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		internal extern void socketSendUrgentData(int data);
	}

}