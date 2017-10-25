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

	using SharedSecrets = sun.misc.SharedSecrets;
	using JavaIOFileDescriptorAccess = sun.misc.JavaIOFileDescriptorAccess;

	/// <summary>
	/// This class defines the plain SocketImpl that is used on Windows platforms
	/// greater or equal to Windows Vista. These platforms have a dual
	/// layer TCP/IP stack and can handle both IPv4 and IPV6 through a
	/// single file descriptor.
	/// 
	/// @author Chris Hegarty
	/// </summary>

	internal class DualStackPlainSocketImpl : AbstractPlainSocketImpl
	{
		internal static JavaIOFileDescriptorAccess FdAccess = SharedSecrets.JavaIOFileDescriptorAccess;


		// true if this socket is exclusively bound
		private readonly bool ExclusiveBind;

		// emulates SO_REUSEADDR when exclusiveBind is true
		private bool IsReuseAddress;

		public DualStackPlainSocketImpl(bool exclBind)
		{
			ExclusiveBind = exclBind;
		}

		public DualStackPlainSocketImpl(FileDescriptor fd, bool exclBind)
		{
			this.Fd = fd;
			ExclusiveBind = exclBind;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void socketCreate(boolean stream) throws java.io.IOException
		internal override void SocketCreate(bool stream)
		{
			if (Fd == null)
			{
				throw new SocketException("Socket closed");
			}

			int newfd = socket0(stream, false); //v6 Only

			FdAccess.set(Fd, newfd);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void socketConnect(InetAddress address, int port, int timeout) throws java.io.IOException
		internal override void SocketConnect(InetAddress address, int port, int timeout)
		{
			int nativefd = CheckAndReturnNativeFD();

			if (address == null)
			{
				throw new NullPointerException("inet address argument is null.");
			}

			int connectResult;
			if (timeout <= 0)
			{
				connectResult = connect0(nativefd, address, port);
			}
			else
			{
				configureBlocking(nativefd, false);
				try
				{
					connectResult = connect0(nativefd, address, port);
					if (connectResult == WOULDBLOCK)
					{
						waitForConnect(nativefd, timeout);
					}
				}
				finally
				{
					configureBlocking(nativefd, true);
				}
			}
			/*
			 * We need to set the local port field. If bind was called
			 * previous to the connect (by the client) then localport field
			 * will already be set.
			 */
			if (Localport == 0)
			{
				Localport = localPort0(nativefd);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void socketBind(InetAddress address, int port) throws java.io.IOException
		internal override void SocketBind(InetAddress address, int port)
		{
			int nativefd = CheckAndReturnNativeFD();

			if (address == null)
			{
				throw new NullPointerException("inet address argument is null.");
			}

			bind0(nativefd, address, port, ExclusiveBind);
			if (port == 0)
			{
				Localport = localPort0(nativefd);
			}
			else
			{
				Localport = port;
			}

			this.Address = address;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void socketListen(int backlog) throws java.io.IOException
		internal override void SocketListen(int backlog)
		{
			int nativefd = CheckAndReturnNativeFD();

			listen0(nativefd, backlog);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void socketAccept(SocketImpl s) throws java.io.IOException
		internal override void SocketAccept(SocketImpl s)
		{
			int nativefd = CheckAndReturnNativeFD();

			if (s == null)
			{
				throw new NullPointerException("socket is null");
			}

			int newfd = -1;
			InetSocketAddress[] isaa = new InetSocketAddress[1];
			if (Timeout_Renamed <= 0)
			{
				newfd = accept0(nativefd, isaa);
			}
			else
			{
				configureBlocking(nativefd, false);
				try
				{
					waitForNewConnection(nativefd, Timeout_Renamed);
					newfd = accept0(nativefd, isaa);
					if (newfd != -1)
					{
						configureBlocking(newfd, true);
					}
				}
				finally
				{
					configureBlocking(nativefd, true);
				}
			}
			/* Update (SocketImpl)s' fd */
			FdAccess.set(s.Fd, newfd);
			/* Update socketImpls remote port, address and localport */
			InetSocketAddress isa = isaa[0];
			s.Port_Renamed = isa.Port;
			s.Address = isa.Address;
			s.Localport = Localport;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: int socketAvailable() throws java.io.IOException
		internal override int SocketAvailable()
		{
			int nativefd = CheckAndReturnNativeFD();
			return available0(nativefd);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void socketClose0(boolean useDeferredClose) throws java.io.IOException
		internal override void SocketClose0(bool useDeferredClose) //unused
		{
			if (Fd == null)
			{
				throw new SocketException("Socket closed");
			}

			if (!Fd.Valid())
			{
				return;
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nativefd = fdAccess.get(fd);
			int nativefd = FdAccess.get(Fd);
			FdAccess.set(Fd, -1);
			close0(nativefd);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void socketShutdown(int howto) throws java.io.IOException
		internal override void SocketShutdown(int howto)
		{
			int nativefd = CheckAndReturnNativeFD();
			shutdown0(nativefd, howto);
		}

		// Intentional fallthrough after SO_REUSEADDR
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("fallthrough") void socketSetOption(int opt, boolean on, Object value) throws SocketException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		internal override void SocketSetOption(int opt, bool on, Object value)
		{
			int nativefd = CheckAndReturnNativeFD();

			if (opt == SocketOptions_Fields.SO_TIMEOUT) // timeout implemented through select.
			{
				return;
			}

			int optionValue = 0;

			switch (opt)
			{
				case SocketOptions_Fields.SO_REUSEADDR :
					if (ExclusiveBind)
					{
						// SO_REUSEADDR emulated when using exclusive bind
						IsReuseAddress = on;
						return;
					}
					// intentional fallthrough
				case SocketOptions_Fields.TCP_NODELAY :
				case SocketOptions_Fields.SO_OOBINLINE :
				case SocketOptions_Fields.SO_KEEPALIVE :
					optionValue = on ? 1 : 0;
					break;
				case SocketOptions_Fields.SO_SNDBUF :
				case SocketOptions_Fields.SO_RCVBUF :
				case SocketOptions_Fields.IP_TOS :
					optionValue = ((Integer)value).IntValue();
					break;
				case SocketOptions_Fields.SO_LINGER :
					if (on)
					{
						optionValue = ((Integer)value).IntValue();
					}
					else
					{
						optionValue = -1;
					}
					break;
				default : // shouldn't get here
					throw new SocketException("Option not supported");
			}

			setIntOption(nativefd, opt, optionValue);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: int socketGetOption(int opt, Object iaContainerObj) throws SocketException
		internal override int SocketGetOption(int opt, Object iaContainerObj)
		{
			int nativefd = CheckAndReturnNativeFD();

			// SO_BINDADDR is not a socket option.
			if (opt == SocketOptions_Fields.SO_BINDADDR)
			{
				localAddress(nativefd, (InetAddressContainer)iaContainerObj);
				return 0; // return value doesn't matter.
			}

			// SO_REUSEADDR emulated when using exclusive bind
			if (opt == SocketOptions_Fields.SO_REUSEADDR && ExclusiveBind)
			{
				return IsReuseAddress? 1 : -1;
			}

			int value = getIntOption(nativefd, opt);

			switch (opt)
			{
				case SocketOptions_Fields.TCP_NODELAY :
				case SocketOptions_Fields.SO_OOBINLINE :
				case SocketOptions_Fields.SO_KEEPALIVE :
				case SocketOptions_Fields.SO_REUSEADDR :
					return (value == 0) ? - 1 : 1;
			}
			return value;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void socketSendUrgentData(int data) throws java.io.IOException
		internal override void SocketSendUrgentData(int data)
		{
			int nativefd = CheckAndReturnNativeFD();
			sendOOB(nativefd, data);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private int checkAndReturnNativeFD() throws SocketException
		private int CheckAndReturnNativeFD()
		{
			if (Fd == null || !Fd.Valid())
			{
				throw new SocketException("Socket closed");
			}

			return FdAccess.get(Fd);
		}

		internal const int WOULDBLOCK = -2; // Nothing available (non-blocking)

		static DualStackPlainSocketImpl()
		{
			initIDs();
		}

		/* Native methods */

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		internal static extern void initIDs();

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		internal static extern int socket0(bool stream, bool v6Only);

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		internal static extern void bind0(int fd, InetAddress localAddress, int localport, bool exclBind);

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		internal static extern int connect0(int fd, InetAddress remote, int remotePort);

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		internal static extern void waitForConnect(int fd, int timeout);

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		internal static extern int localPort0(int fd);

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		internal static extern void localAddress(int fd, InetAddressContainer @in);

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		internal static extern void listen0(int fd, int backlog);

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		internal static extern int accept0(int fd, InetSocketAddress[] isaa);

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		internal static extern void waitForNewConnection(int fd, int timeout);

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		internal static extern int available0(int fd);

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		internal static extern void close0(int fd);

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		internal static extern void shutdown0(int fd, int howto);

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		internal static extern void setIntOption(int fd, int cmd, int optionValue);

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		internal static extern int getIntOption(int fd, int cmd);

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		internal static extern void sendOOB(int fd, int data);

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		internal static extern void configureBlocking(int fd, bool blocking);
	}

}