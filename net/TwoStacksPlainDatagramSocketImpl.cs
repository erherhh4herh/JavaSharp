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

	/// <summary>
	/// This class defines the plain DatagramSocketImpl that is used for all
	/// Windows versions lower than Vista. It adds support for IPv6 on
	/// these platforms where available.
	/// 
	/// For backward compatibility windows platforms that do not have IPv6
	/// support also use this implementation, and fd1 gets set to null
	/// during socket creation.
	/// 
	/// @author Chris Hegarty
	/// </summary>

	internal class TwoStacksPlainDatagramSocketImpl : AbstractPlainDatagramSocketImpl
	{
		/* Used for IPv6 on Windows only */
		private FileDescriptor Fd1;

		/*
		 * Needed for ipv6 on windows because we need to know
		 * if the socket was bound to ::0 or 0.0.0.0, when a caller
		 * asks for it. In this case, both sockets are used, but we
		 * don't know whether the caller requested ::0 or 0.0.0.0
		 * and need to remember it here.
		 */
		private InetAddress AnyLocalBoundAddr = null;

		private int Fduse = -1; // saved between peek() and receive() calls

		/* saved between successive calls to receive, if data is detected
		 * on both sockets at same time. To ensure that one socket is not
		 * starved, they rotate using this field
		 */
		private int Lastfd = -1;

		static TwoStacksPlainDatagramSocketImpl()
		{
			init();
		}

		// true if this socket is exclusively bound
		private readonly bool ExclusiveBind;

		/*
		 * Set to true if SO_REUSEADDR is set after the socket is bound to
		 * indicate SO_REUSEADDR is being emulated
		 */
		private bool ReuseAddressEmulated;

		// emulates SO_REUSEADDR when exclusiveBind is true and socket is bound
		private bool IsReuseAddress;

		internal TwoStacksPlainDatagramSocketImpl(bool exclBind)
		{
			ExclusiveBind = exclBind;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected synchronized void create() throws SocketException
		protected internal override void Create()
		{
			lock (this)
			{
				Fd1 = new FileDescriptor();
				try
				{
					base.Create();
				}
				catch (SocketException e)
				{
					Fd1 = null;
					throw e;
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected synchronized void bind(int lport, InetAddress laddr) throws SocketException
		protected internal override void Bind(int lport, InetAddress laddr)
		{
			lock (this)
			{
				base.Bind(lport, laddr);
				if (laddr.AnyLocalAddress)
				{
					AnyLocalBoundAddr = laddr;
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected synchronized void bind0(int lport, InetAddress laddr) throws SocketException
		protected internal override void Bind0(int lport, InetAddress laddr)
		{
			lock (this)
			{
				bind0(lport, laddr, ExclusiveBind);
        
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected synchronized void receive(DatagramPacket p) throws java.io.IOException
		protected internal override void Receive(DatagramPacket p)
		{
			lock (this)
			{
				try
				{
					Receive0(p);
				}
				finally
				{
					Fduse = -1;
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object getOption(int optID) throws SocketException
		public override Object GetOption(int optID)
		{
			if (Closed)
			{
				throw new SocketException("Socket Closed");
			}

			if (optID == SocketOptions_Fields.SO_BINDADDR)
			{
				if ((Fd != null && Fd1 != null) && !Connected)
				{
					return AnyLocalBoundAddr;
				}
				int family = ConnectedAddress == null ? - 1 : ConnectedAddress.Holder().Family;
				return socketLocalAddress(family);
			}
			else if (optID == SocketOptions_Fields.SO_REUSEADDR && ReuseAddressEmulated)
			{
				return IsReuseAddress;
			}
			else
			{
				return base.GetOption(optID);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void socketSetOption(int opt, Object val) throws SocketException
		protected internal override void SocketSetOption(int opt, Object val)
		{
			if (opt == SocketOptions_Fields.SO_REUSEADDR && ExclusiveBind && LocalPort_Renamed != 0)
			{
				// socket already bound, emulate
				ReuseAddressEmulated = true;
				IsReuseAddress = (Boolean)val;
			}
			else
			{
				socketNativeSetOption(opt, val);
			}

		}

		protected internal override bool Closed
		{
			get
			{
				return (Fd == null && Fd1 == null) ? true : false;
			}
		}

		protected internal override void Close()
		{
			if (Fd != null || Fd1 != null)
			{
				DatagramSocketClose();
				ResourceManager.afterUdpClose();
				Fd = null;
				Fd1 = null;
			}
		}

		/* Native methods */

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		protected internal extern void bind0(int lport, InetAddress laddr, bool exclBind);

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		protected internal extern void send(DatagramPacket p);

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		protected internal extern int peek(InetAddress i);

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		protected internal extern int peekData(DatagramPacket p);

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		protected internal extern void receive0(DatagramPacket p);

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		protected internal extern void setTimeToLive(int ttl);

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		protected internal extern int getTimeToLive();

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		protected internal extern void setTTL(sbyte ttl);

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		protected internal extern byte getTTL();

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		protected internal extern void join(InetAddress inetaddr, NetworkInterface netIf);

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		protected internal extern void leave(InetAddress inetaddr, NetworkInterface netIf);

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		protected internal extern void datagramSocketCreate();

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		protected internal extern void datagramSocketClose();

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		protected internal extern void socketNativeSetOption(int opt, Object val);

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		protected internal extern Object socketGetOption(int opt);

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		protected internal extern void connect0(InetAddress address, int port);

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		protected internal extern Object socketLocalAddress(int family);

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		protected internal extern void disconnect0(int family);

		/// <summary>
		/// Perform class load-time initializations.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern static void init();
	}

}