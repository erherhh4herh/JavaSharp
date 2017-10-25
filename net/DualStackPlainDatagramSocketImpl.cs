using System;
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
	/// This class defines the plain DatagramSocketImpl that is used on
	/// Windows platforms greater than or equal to Windows Vista. These
	/// platforms have a dual layer TCP/IP stack and can handle both IPv4
	/// and IPV6 through a single file descriptor.
	/// <para>
	/// Note: Multicasting on a dual layer TCP/IP stack is always done with
	/// TwoStacksPlainDatagramSocketImpl. This is to overcome the lack
	/// of behavior defined for multicasting over a dual layer socket by the RFC.
	/// 
	/// @author Chris Hegarty
	/// </para>
	/// </summary>

	internal class DualStackPlainDatagramSocketImpl : AbstractPlainDatagramSocketImpl
	{
		internal static JavaIOFileDescriptorAccess FdAccess = SharedSecrets.JavaIOFileDescriptorAccess;

		// true if this socket is exclusively bound
		private readonly bool ExclusiveBind;

		/*
		 * Set to true if SO_REUSEADDR is set after the socket is bound to
		 * indicate SO_REUSEADDR is being emulated
		 */
		private bool ReuseAddressEmulated;

		// emulates SO_REUSEADDR when exclusiveBind is true and socket is bound
		private bool IsReuseAddress;

		internal DualStackPlainDatagramSocketImpl(bool exclBind)
		{
			ExclusiveBind = exclBind;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void datagramSocketCreate() throws SocketException
		protected internal override void DatagramSocketCreate()
		{
			if (Fd == null)
			{
				throw new SocketException("Socket closed");
			}

			int newfd = socketCreate(false); // v6Only

			FdAccess.set(Fd, newfd);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected synchronized void bind0(int lport, InetAddress laddr) throws SocketException
		protected internal override void Bind0(int lport, InetAddress laddr)
		{
			lock (this)
			{
				int nativefd = CheckAndReturnNativeFD();
        
				if (laddr == null)
				{
					throw new NullPointerException("argument address");
				}
        
				socketBind(nativefd, laddr, lport, ExclusiveBind);
				if (lport == 0)
				{
					LocalPort_Renamed = socketLocalPort(nativefd);
				}
				else
				{
					LocalPort_Renamed = lport;
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected synchronized int peek(InetAddress address) throws java.io.IOException
		protected internal override int Peek(InetAddress address)
		{
			lock (this)
			{
				int nativefd = CheckAndReturnNativeFD();
        
				if (address == null)
				{
					throw new NullPointerException("Null address in peek()");
				}
        
				// Use peekData()
				DatagramPacket peekPacket = new DatagramPacket(new sbyte[1], 1);
				int peekPort = PeekData(peekPacket);
				address = peekPacket.Address;
				return peekPort;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected synchronized int peekData(DatagramPacket p) throws java.io.IOException
		protected internal override int PeekData(DatagramPacket p)
		{
			lock (this)
			{
				int nativefd = CheckAndReturnNativeFD();
        
				if (p == null)
				{
					throw new NullPointerException("packet");
				}
				if (p.Data == null)
				{
					throw new NullPointerException("packet buffer");
				}
        
				return socketReceiveOrPeekData(nativefd, p, Timeout, Connected, true); //peek
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected synchronized void receive0(DatagramPacket p) throws java.io.IOException
		protected internal override void Receive0(DatagramPacket p)
		{
			lock (this)
			{
				int nativefd = CheckAndReturnNativeFD();
        
				if (p == null)
				{
					throw new NullPointerException("packet");
				}
				if (p.Data == null)
				{
					throw new NullPointerException("packet buffer");
				}
        
				socketReceiveOrPeekData(nativefd, p, Timeout, Connected, false); //receive
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void send(DatagramPacket p) throws java.io.IOException
		protected internal override void Send(DatagramPacket p)
		{
			int nativefd = CheckAndReturnNativeFD();

			if (p == null)
			{
				throw new NullPointerException("null packet");
			}

			if (p.Address == null || p.Data == null)
			{
				throw new NullPointerException("null address || null buffer");
			}

			socketSend(nativefd, p.Data, p.Offset, p.Length, p.Address, p.Port, Connected);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void connect0(InetAddress address, int port) throws SocketException
		protected internal override void Connect0(InetAddress address, int port)
		{
			int nativefd = CheckAndReturnNativeFD();

			if (address == null)
			{
				throw new NullPointerException("address");
			}

			socketConnect(nativefd, address, port);
		}

		protected internal override void Disconnect0(int family) //unused
		{
			if (Fd == null || !Fd.Valid())
			{
				return; // disconnect doesn't throw any exceptions
			}

			socketDisconnect(FdAccess.get(Fd));
		}

		protected internal override void DatagramSocketClose()
		{
			if (Fd == null || !Fd.Valid())
			{
				return; // close doesn't throw any exceptions
			}

			socketClose(FdAccess.get(Fd));
			FdAccess.set(Fd, -1);
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("fallthrough") protected void socketSetOption(int opt, Object val) throws SocketException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		protected internal override void SocketSetOption(int opt, Object val)
		{
			int nativefd = CheckAndReturnNativeFD();

			int optionValue = 0;

			switch (opt)
			{
				case SocketOptions_Fields.IP_TOS :
				case SocketOptions_Fields.SO_RCVBUF :
				case SocketOptions_Fields.SO_SNDBUF :
					optionValue = ((Integer)val).IntValue();
					break;
				case SocketOptions_Fields.SO_REUSEADDR :
					if (ExclusiveBind && LocalPort_Renamed != 0)
					{
						// socket already bound, emulate SO_REUSEADDR
						ReuseAddressEmulated = true;
						IsReuseAddress = (Boolean)val;
						return;
					}
					//Intentional fallthrough
				case SocketOptions_Fields.SO_BROADCAST :
					optionValue = ((Boolean)val).BooleanValue() ? 1 : 0;
					break;
				default: // shouldn't get here
					throw new SocketException("Option not supported");
			}

			socketSetIntOption(nativefd, opt, optionValue);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected Object socketGetOption(int opt) throws SocketException
		protected internal override Object SocketGetOption(int opt)
		{
			int nativefd = CheckAndReturnNativeFD();

			 // SO_BINDADDR is not a socket option.
			if (opt == SocketOptions_Fields.SO_BINDADDR)
			{
				return socketLocalAddress(nativefd);
			}
			if (opt == SocketOptions_Fields.SO_REUSEADDR && ReuseAddressEmulated)
			{
				return IsReuseAddress;
			}

			int value = socketGetIntOption(nativefd, opt);
			Object returnValue = null;

			switch (opt)
			{
				case SocketOptions_Fields.SO_REUSEADDR :
				case SocketOptions_Fields.SO_BROADCAST :
					returnValue = (value == 0) ? false : true;
					break;
				case SocketOptions_Fields.IP_TOS :
				case SocketOptions_Fields.SO_RCVBUF :
				case SocketOptions_Fields.SO_SNDBUF :
					returnValue = new Integer(value);
					break;
				default: // shouldn't get here
					throw new SocketException("Option not supported");
			}

			return returnValue;
		}

		/* Multicast specific methods.
		 * Multicasting on a dual layer TCP/IP stack is always done with
		 * TwoStacksPlainDatagramSocketImpl. This is to overcome the lack
		 * of behavior defined for multicasting over a dual layer socket by the RFC.
		 */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void join(InetAddress inetaddr, NetworkInterface netIf) throws java.io.IOException
		protected internal override void Join(InetAddress inetaddr, NetworkInterface netIf)
		{
			throw new IOException("Method not implemented!");
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void leave(InetAddress inetaddr, NetworkInterface netIf) throws java.io.IOException
		protected internal override void Leave(InetAddress inetaddr, NetworkInterface netIf)
		{
			throw new IOException("Method not implemented!");
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void setTimeToLive(int ttl) throws java.io.IOException
		protected internal override int TimeToLive
		{
			set
			{
				throw new IOException("Method not implemented!");
			}
			get
			{
				throw new IOException("Method not implemented!");
			}
		}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deprecated protected void setTTL(byte ttl) throws java.io.IOException
		[Obsolete]
		protected internal override sbyte TTL
		{
			set
			{
				throw new IOException("Method not implemented!");
			}
			get
			{
				throw new IOException("Method not implemented!");
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deprecated protected byte getTTL() throws java.io.IOException
		/* END Multicast specific methods */

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

		/* Native methods */

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern void initIDs();

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern int socketCreate(bool v6Only);

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern void socketBind(int fd, InetAddress localAddress, int localport, bool exclBind);

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern void socketConnect(int fd, InetAddress address, int port);

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern void socketDisconnect(int fd);

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern void socketClose(int fd);

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern int socketLocalPort(int fd);

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern Object socketLocalAddress(int fd);

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern int socketReceiveOrPeekData(int fd, DatagramPacket packet, int timeout, bool connected, bool peek);

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern void socketSend(int fd, sbyte[] data, int offset, int length, InetAddress address, int port, bool connected);

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern void socketSetIntOption(int fd, int cmd, int optionValue);

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern int socketGetIntOption(int fd, int cmd);
	}

}