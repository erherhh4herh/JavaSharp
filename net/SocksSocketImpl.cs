using System;
using System.Diagnostics;
using System.Collections.Generic;

/*
 * Copyright (c) 2000, 2013, Oracle and/or its affiliates. All rights reserved.
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
	using SocksProxy = sun.net.SocksProxy;
	using ParseUtil = sun.net.www.ParseUtil;
	/* import org.ietf.jgss.*; */

	/// <summary>
	/// SOCKS (V4 & V5) TCP socket implementation (RFC 1928).
	/// This is a subclass of PlainSocketImpl.
	/// Note this class should <b>NOT</b> be public.
	/// </summary>

	internal class SocksSocketImpl : PlainSocketImpl, SocksConsts
	{
		private String Server = null;
		private int ServerPort = SocksConsts_Fields.DEFAULT_PORT;
		private InetSocketAddress External_address;
		private bool UseV4 = false;
		private Socket Cmdsock = null;
		private InputStream CmdIn = null;
		private OutputStream CmdOut = null;
		/* true if the Proxy has been set programatically */
		private bool ApplicationSetProxy; // false


		internal SocksSocketImpl()
		{
			// Nothing needed
		}

		internal SocksSocketImpl(String server, int port)
		{
			this.Server = server;
			this.ServerPort = (port == -1 ? SocksConsts_Fields.DEFAULT_PORT : port);
		}

		internal SocksSocketImpl(Proxy proxy)
		{
			SocketAddress a = proxy.Address();
			if (a is InetSocketAddress)
			{
				InetSocketAddress ad = (InetSocketAddress) a;
				// Use getHostString() to avoid reverse lookups
				Server = ad.HostString;
				ServerPort = ad.Port;
			}
		}

		internal virtual void SetV4()
		{
			UseV4 = true;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private synchronized void privilegedConnect(final String host, final int port, final int timeout) throws java.io.IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		private void PrivilegedConnect(String host, int port, int timeout)
		{
			lock (this)
			{
				try
				{
					AccessController.doPrivileged(new PrivilegedExceptionActionAnonymousInnerClassHelper(this, host, port, timeout));
				}
				catch (java.security.PrivilegedActionException pae)
				{
					throw (IOException) pae.Exception;
				}
			}
		}

		private class PrivilegedExceptionActionAnonymousInnerClassHelper : PrivilegedExceptionAction<Void>
		{
			private readonly SocksSocketImpl OuterInstance;

			private string Host;
			private int Port;
			private int Timeout;

			public PrivilegedExceptionActionAnonymousInnerClassHelper(SocksSocketImpl outerInstance, string host, int port, int timeout)
			{
				this.OuterInstance = outerInstance;
				this.Host = host;
				this.Port = port;
				this.Timeout = timeout;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Void run() throws java.io.IOException
			public virtual Void Run()
			{
					  outerInstance.SuperConnectServer(Host, Port, Timeout);
					  OuterInstance.CmdIn = outerInstance.GetInputStream();
					  OuterInstance.CmdOut = outerInstance.OutputStream;
					  return null;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void superConnectServer(String host, int port, int timeout) throws java.io.IOException
		private void SuperConnectServer(String host, int port, int timeout)
		{
			base.Connect(new InetSocketAddress(host, port), timeout);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static int remainingMillis(long deadlineMillis) throws java.io.IOException
		private static int RemainingMillis(long deadlineMillis)
		{
			if (deadlineMillis == 0L)
			{
				return 0;
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final long remaining = deadlineMillis - System.currentTimeMillis();
			long remaining = deadlineMillis - DateTimeHelperClass.CurrentUnixTimeMillis();
			if (remaining > 0)
			{
				return (int) remaining;
			}

			throw new SocketTimeoutException();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private int readSocksReply(java.io.InputStream in, byte[] data) throws java.io.IOException
		private int ReadSocksReply(InputStream @in, sbyte[] data)
		{
			return ReadSocksReply(@in, data, 0L);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private int readSocksReply(java.io.InputStream in, byte[] data, long deadlineMillis) throws java.io.IOException
		private int ReadSocksReply(InputStream @in, sbyte[] data, long deadlineMillis)
		{
			int len = data.Length;
			int received = 0;
			for (int attempts = 0; received < len && attempts < 3; attempts++)
			{
				int count;
				try
				{
					count = ((SocketInputStream)@in).Read(data, received, len - received, RemainingMillis(deadlineMillis));
				}
				catch (SocketTimeoutException)
				{
					throw new SocketTimeoutException("Connect timed out");
				}
				if (count < 0)
				{
					throw new SocketException("Malformed reply from SOCKS server");
				}
				received += count;
			}
			return received;
		}

		/// <summary>
		/// Provides the authentication machanism required by the proxy.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private boolean authenticate(byte method, java.io.InputStream in, java.io.BufferedOutputStream out) throws java.io.IOException
		private bool Authenticate(sbyte method, InputStream @in, BufferedOutputStream @out)
		{
			return Authenticate(method, @in, @out, 0L);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private boolean authenticate(byte method, java.io.InputStream in, java.io.BufferedOutputStream out, long deadlineMillis) throws java.io.IOException
		private bool Authenticate(sbyte method, InputStream @in, BufferedOutputStream @out, long deadlineMillis)
		{
			// No Authentication required. We're done then!
			if (method == SocksConsts_Fields.NO_AUTH)
			{
				return true;
			}
			/// <summary>
			/// User/Password authentication. Try, in that order :
			/// - The application provided Authenticator, if any
			/// - the user.name & no password (backward compatibility behavior).
			/// </summary>
			if (method == SocksConsts_Fields.USER_PASSW)
			{
				String userName;
				String password = null;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final InetAddress addr = InetAddress.getByName(server);
				InetAddress addr = InetAddress.GetByName(Server);
				PasswordAuthentication pw = AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper(this, addr));
				if (pw != null)
				{
					userName = pw.UserName;
					password = new String(pw.Password);
				}
				else
				{
					userName = AccessController.doPrivileged(new sun.security.action.GetPropertyAction("user.name"));
				}
				if (userName == null)
				{
					return false;
				}
				@out.Write(1);
				@out.Write(userName.Length());
				try
				{
					@out.Write(userName.GetBytes("ISO-8859-1"));
				}
				catch (java.io.UnsupportedEncodingException)
				{
					Debug.Assert(false);
				}
				if (password != null)
				{
					@out.Write(password.Length());
					try
					{
						@out.Write(password.GetBytes("ISO-8859-1"));
					}
					catch (java.io.UnsupportedEncodingException)
					{
						Debug.Assert(false);
					}
				}
				else
				{
					@out.Write(0);
				}
				@out.Flush();
				sbyte[] data = new sbyte[2];
				int i = ReadSocksReply(@in, data, deadlineMillis);
				if (i != 2 || data[1] != 0)
				{
					/* RFC 1929 specifies that the connection MUST be closed if
					   authentication fails */
					@out.Close();
					@in.Close();
					return false;
				}
				/* Authentication succeeded */
				return true;
			}
			/// <summary>
			/// GSSAPI authentication mechanism.
			/// Unfortunately the RFC seems out of sync with the Reference
			/// implementation. I'll leave this in for future completion.
			/// </summary>
	//      if (method == GSSAPI) {
	//          try {
	//              GSSManager manager = GSSManager.getInstance();
	//              GSSName name = manager.createName("SERVICE:socks@"+server,
	//                                                   null);
	//              GSSContext context = manager.createContext(name, null, null,
	//                                                         GSSContext.DEFAULT_LIFETIME);
	//              context.requestMutualAuth(true);
	//              context.requestReplayDet(true);
	//              context.requestSequenceDet(true);
	//              context.requestCredDeleg(true);
	//              byte []inToken = new byte[0];
	//              while (!context.isEstablished()) {
	//                  byte[] outToken
	//                      = context.initSecContext(inToken, 0, inToken.length);
	//                  // send the output token if generated
	//                  if (outToken != null) {
	//                      out.write(1);
	//                      out.write(1);
	//                      out.writeShort(outToken.length);
	//                      out.write(outToken);
	//                      out.flush();
	//                      data = new byte[2];
	//                      i = readSocksReply(in, data, deadlineMillis);
	//                      if (i != 2 || data[1] == 0xff) {
	//                          in.close();
	//                          out.close();
	//                          return false;
	//                      }
	//                      i = readSocksReply(in, data, deadlineMillis);
	//                      int len = 0;
	//                      len = ((int)data[0] & 0xff) << 8;
	//                      len += data[1];
	//                      data = new byte[len];
	//                      i = readSocksReply(in, data, deadlineMillis);
	//                      if (i == len)
	//                          return true;
	//                      in.close();
	//                      out.close();
	//                  }
	//              }
	//          } catch (GSSException e) {
	//              /* RFC 1961 states that if Context initialisation fails the connection
	//                 MUST be closed */
	//              e.printStackTrace();
	//              in.close();
	//              out.close();
	//          }
	//      }
			return false;
		}

		private class PrivilegedActionAnonymousInnerClassHelper : PrivilegedAction<PasswordAuthentication>
		{
			private readonly SocksSocketImpl OuterInstance;

			private java.net.InetAddress Addr;

			public PrivilegedActionAnonymousInnerClassHelper(SocksSocketImpl outerInstance, java.net.InetAddress addr)
			{
				this.OuterInstance = outerInstance;
				this.Addr = addr;
			}

			public virtual PasswordAuthentication Run()
			{
					return Authenticator.RequestPasswordAuthentication(OuterInstance.Server, Addr, OuterInstance.ServerPort, "SOCKS5", "SOCKS authentication", null);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void connectV4(java.io.InputStream in, java.io.OutputStream out, InetSocketAddress endpoint, long deadlineMillis) throws java.io.IOException
		private void ConnectV4(InputStream @in, OutputStream @out, InetSocketAddress endpoint, long deadlineMillis)
		{
			if (!(endpoint.Address is Inet4Address))
			{
				throw new SocketException("SOCKS V4 requires IPv4 only addresses");
			}
			@out.Write(SocksConsts_Fields.PROTO_VERS4);
			@out.Write(SocksConsts_Fields.CONNECT);
			@out.Write((endpoint.Port >> 8) & 0xff);
			@out.Write((endpoint.Port >> 0) & 0xff);
			@out.Write(endpoint.Address.Address);
			String userName = UserName;
			try
			{
				@out.Write(userName.GetBytes("ISO-8859-1"));
			}
			catch (java.io.UnsupportedEncodingException)
			{
				Debug.Assert(false);
			}
			@out.Write(0);
			@out.Flush();
			sbyte[] data = new sbyte[8];
			int n = ReadSocksReply(@in, data, deadlineMillis);
			if (n != 8)
			{
				throw new SocketException("Reply from SOCKS server has bad length: " + n);
			}
			if (data[0] != 0 && data[0] != 4)
			{
				throw new SocketException("Reply from SOCKS server has bad version");
			}
			SocketException ex = null;
			switch (data[1])
			{
			case 90:
				// Success!
				External_address = endpoint;
				break;
			case 91:
				ex = new SocketException("SOCKS request rejected");
				break;
			case 92:
				ex = new SocketException("SOCKS server couldn't reach destination");
				break;
			case 93:
				ex = new SocketException("SOCKS authentication failed");
				break;
			default:
				ex = new SocketException("Reply from SOCKS server contains bad status");
				break;
			}
			if (ex != null)
			{
				@in.Close();
				@out.Close();
				throw ex;
			}
		}

		/// <summary>
		/// Connects the Socks Socket to the specified endpoint. It will first
		/// connect to the SOCKS proxy and negotiate the access. If the proxy
		/// grants the connections, then the connect is successful and all
		/// further traffic will go to the "real" endpoint.
		/// </summary>
		/// <param name="endpoint">        the {@code SocketAddress} to connect to. </param>
		/// <param name="timeout">         the timeout value in milliseconds </param>
		/// <exception cref="IOException">     if the connection can't be established. </exception>
		/// <exception cref="SecurityException"> if there is a security manager and it
		///                          doesn't allow the connection </exception>
		/// <exception cref="IllegalArgumentException"> if endpoint is null or a
		///          SocketAddress subclass not supported by this socket </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void connect(SocketAddress endpoint, int timeout) throws java.io.IOException
		protected internal override void Connect(SocketAddress endpoint, int timeout)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final long deadlineMillis;
			long deadlineMillis;

			if (timeout == 0)
			{
				deadlineMillis = 0L;
			}
			else
			{
				long finish = DateTimeHelperClass.CurrentUnixTimeMillis() + timeout;
				deadlineMillis = finish < 0 ? Long.MaxValue : finish;
			}

			SecurityManager security = System.SecurityManager;
			if (endpoint == null || !(endpoint is InetSocketAddress))
			{
				throw new IllegalArgumentException("Unsupported address type");
			}
			InetSocketAddress epoint = (InetSocketAddress) endpoint;
			if (security != null)
			{
				if (epoint.Unresolved)
				{
					security.CheckConnect(epoint.HostName, epoint.Port);
				}
				else
				{
					security.CheckConnect(epoint.Address.HostAddress, epoint.Port);
				}
			}
			if (Server == null)
			{
				// This is the general case
				// server is not null only when the socket was created with a
				// specified proxy in which case it does bypass the ProxySelector
				ProxySelector sel = AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper2(this));
				if (sel == null)
				{
					/*
					 * No default proxySelector --> direct connection
					 */
					base.Connect(epoint, RemainingMillis(deadlineMillis));
					return;
				}
				URI uri;
				// Use getHostString() to avoid reverse lookups
				String host = epoint.HostString;
				// IPv6 litteral?
				if (epoint.Address is Inet6Address && (!host.StartsWith("[")) && (host.IndexOf(":") >= 0))
				{
					host = "[" + host + "]";
				}
				try
				{
					uri = new URI("socket://" + ParseUtil.encodePath(host) + ":" + epoint.Port);
				}
				catch (URISyntaxException e)
				{
					// This shouldn't happen
					Debug.Assert(false, e);
					uri = null;
				}
				Proxy p = null;
				IOException savedExc = null;
				IEnumerator<Proxy> iProxy = null;
				iProxy = sel.Select(uri).GetEnumerator();
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				if (iProxy == null || !(iProxy.hasNext()))
				{
					base.Connect(epoint, RemainingMillis(deadlineMillis));
					return;
				}
				while (iProxy.MoveNext())
				{
					p = iProxy.Current;
					if (p == null || p.Type() != Proxy.Type.SOCKS)
					{
						base.Connect(epoint, RemainingMillis(deadlineMillis));
						return;
					}

					if (!(p.Address() is InetSocketAddress))
					{
						throw new SocketException("Unknown address type for proxy: " + p);
					}
					// Use getHostString() to avoid reverse lookups
					Server = ((InetSocketAddress) p.Address()).HostString;
					ServerPort = ((InetSocketAddress) p.Address()).Port;
					if (p is SocksProxy)
					{
						if (((SocksProxy)p).protocolVersion() == 4)
						{
							UseV4 = true;
						}
					}

					// Connects to the SOCKS server
					try
					{
						PrivilegedConnect(Server, ServerPort, RemainingMillis(deadlineMillis));
						// Worked, let's get outta here
						break;
					}
					catch (IOException e)
					{
						// Ooops, let's notify the ProxySelector
						sel.ConnectFailed(uri,p.Address(),e);
						Server = null;
						ServerPort = -1;
						savedExc = e;
						// Will continue the while loop and try the next proxy
					}
				}

				/*
				 * If server is still null at this point, none of the proxy
				 * worked
				 */
				if (Server == null)
				{
					throw new SocketException("Can't connect to SOCKS proxy:" + savedExc.Message);
				}
			}
			else
			{
				// Connects to the SOCKS server
				try
				{
					PrivilegedConnect(Server, ServerPort, RemainingMillis(deadlineMillis));
				}
				catch (IOException e)
				{
					throw new SocketException(e.Message);
				}
			}

			// cmdIn & cmdOut were initialized during the privilegedConnect() call
			BufferedOutputStream @out = new BufferedOutputStream(CmdOut, 512);
			InputStream @in = CmdIn;

			if (UseV4)
			{
				// SOCKS Protocol version 4 doesn't know how to deal with
				// DOMAIN type of addresses (unresolved addresses here)
				if (epoint.Unresolved)
				{
					throw new UnknownHostException(epoint.ToString());
				}
				ConnectV4(@in, @out, epoint, deadlineMillis);
				return;
			}

			// This is SOCKS V5
			@out.Write(SocksConsts_Fields.PROTO_VERS);
			@out.Write(2);
			@out.Write(SocksConsts_Fields.NO_AUTH);
			@out.Write(SocksConsts_Fields.USER_PASSW);
			@out.Flush();
			sbyte[] data = new sbyte[2];
			int i = ReadSocksReply(@in, data, deadlineMillis);
			if (i != 2 || ((int)data[0]) != SocksConsts_Fields.PROTO_VERS)
			{
				// Maybe it's not a V5 sever after all
				// Let's try V4 before we give up
				// SOCKS Protocol version 4 doesn't know how to deal with
				// DOMAIN type of addresses (unresolved addresses here)
				if (epoint.Unresolved)
				{
					throw new UnknownHostException(epoint.ToString());
				}
				ConnectV4(@in, @out, epoint, deadlineMillis);
				return;
			}
			if (((int)data[1]) == SocksConsts_Fields.NO_METHODS)
			{
				throw new SocketException("SOCKS : No acceptable methods");
			}
			if (!Authenticate(data[1], @in, @out, deadlineMillis))
			{
				throw new SocketException("SOCKS : authentication failed");
			}
			@out.Write(SocksConsts_Fields.PROTO_VERS);
			@out.Write(SocksConsts_Fields.CONNECT);
			@out.Write(0);
			/* Test for IPV4/IPV6/Unresolved */
			if (epoint.Unresolved)
			{
				@out.Write(SocksConsts_Fields.DOMAIN_NAME);
				@out.Write(epoint.HostName.Length());
				try
				{
					@out.Write(epoint.HostName.GetBytes("ISO-8859-1"));
				}
				catch (java.io.UnsupportedEncodingException)
				{
					Debug.Assert(false);
				}
				@out.Write((epoint.Port >> 8) & 0xff);
				@out.Write((epoint.Port >> 0) & 0xff);
			}
			else if (epoint.Address is Inet6Address)
			{
				@out.Write(SocksConsts_Fields.IPV6);
				@out.Write(epoint.Address.Address);
				@out.Write((epoint.Port >> 8) & 0xff);
				@out.Write((epoint.Port >> 0) & 0xff);
			}
			else
			{
				@out.Write(SocksConsts_Fields.IPV4);
				@out.Write(epoint.Address.Address);
				@out.Write((epoint.Port >> 8) & 0xff);
				@out.Write((epoint.Port >> 0) & 0xff);
			}
			@out.Flush();
			data = new sbyte[4];
			i = ReadSocksReply(@in, data, deadlineMillis);
			if (i != 4)
			{
				throw new SocketException("Reply from SOCKS server has bad length");
			}
			SocketException ex = null;
			int len;
			sbyte[] addr;
			switch (data[1])
			{
			case SocksConsts_Fields.REQUEST_OK:
				// success!
				switch (data[3])
				{
				case SocksConsts_Fields.IPV4:
					addr = new sbyte[4];
					i = ReadSocksReply(@in, addr, deadlineMillis);
					if (i != 4)
					{
						throw new SocketException("Reply from SOCKS server badly formatted");
					}
					data = new sbyte[2];
					i = ReadSocksReply(@in, data, deadlineMillis);
					if (i != 2)
					{
						throw new SocketException("Reply from SOCKS server badly formatted");
					}
					break;
				case SocksConsts_Fields.DOMAIN_NAME:
					len = data[1];
					sbyte[] host = new sbyte[len];
					i = ReadSocksReply(@in, host, deadlineMillis);
					if (i != len)
					{
						throw new SocketException("Reply from SOCKS server badly formatted");
					}
					data = new sbyte[2];
					i = ReadSocksReply(@in, data, deadlineMillis);
					if (i != 2)
					{
						throw new SocketException("Reply from SOCKS server badly formatted");
					}
					break;
				case SocksConsts_Fields.IPV6:
					len = data[1];
					addr = new sbyte[len];
					i = ReadSocksReply(@in, addr, deadlineMillis);
					if (i != len)
					{
						throw new SocketException("Reply from SOCKS server badly formatted");
					}
					data = new sbyte[2];
					i = ReadSocksReply(@in, data, deadlineMillis);
					if (i != 2)
					{
						throw new SocketException("Reply from SOCKS server badly formatted");
					}
					break;
				default:
					ex = new SocketException("Reply from SOCKS server contains wrong code");
					break;
				}
				break;
			case SocksConsts_Fields.GENERAL_FAILURE:
				ex = new SocketException("SOCKS server general failure");
				break;
			case SocksConsts_Fields.NOT_ALLOWED:
				ex = new SocketException("SOCKS: Connection not allowed by ruleset");
				break;
			case SocksConsts_Fields.NET_UNREACHABLE:
				ex = new SocketException("SOCKS: Network unreachable");
				break;
			case SocksConsts_Fields.HOST_UNREACHABLE:
				ex = new SocketException("SOCKS: Host unreachable");
				break;
			case SocksConsts_Fields.CONN_REFUSED:
				ex = new SocketException("SOCKS: Connection refused");
				break;
			case SocksConsts_Fields.TTL_EXPIRED:
				ex = new SocketException("SOCKS: TTL expired");
				break;
			case SocksConsts_Fields.CMD_NOT_SUPPORTED:
				ex = new SocketException("SOCKS: Command not supported");
				break;
			case SocksConsts_Fields.ADDR_TYPE_NOT_SUP:
				ex = new SocketException("SOCKS: address type not supported");
				break;
			}
			if (ex != null)
			{
				@in.Close();
				@out.Close();
				throw ex;
			}
			External_address = epoint;
		}

		private class PrivilegedActionAnonymousInnerClassHelper2 : PrivilegedAction<ProxySelector>
		{
			private readonly SocksSocketImpl OuterInstance;

			public PrivilegedActionAnonymousInnerClassHelper2(SocksSocketImpl outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			public virtual ProxySelector Run()
			{
					return ProxySelector.Default;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void bindV4(java.io.InputStream in, java.io.OutputStream out, InetAddress baddr, int lport) throws java.io.IOException
		private void BindV4(InputStream @in, OutputStream @out, InetAddress baddr, int lport)
		{
			if (!(baddr is Inet4Address))
			{
				throw new SocketException("SOCKS V4 requires IPv4 only addresses");
			}
			base.Bind(baddr, lport);
			sbyte[] addr1 = baddr.Address;
			/* Test for AnyLocal */
			InetAddress naddr = baddr;
			if (naddr.AnyLocalAddress)
			{
				naddr = AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper3(this));
				addr1 = naddr.Address;
			}
			@out.Write(SocksConsts_Fields.PROTO_VERS4);
			@out.Write(SocksConsts_Fields.BIND);
			@out.Write((base.LocalPort >> 8) & 0xff);
			@out.Write((base.LocalPort >> 0) & 0xff);
			@out.Write(addr1);
			String userName = UserName;
			try
			{
				@out.Write(userName.GetBytes("ISO-8859-1"));
			}
			catch (java.io.UnsupportedEncodingException)
			{
				Debug.Assert(false);
			}
			@out.Write(0);
			@out.Flush();
			sbyte[] data = new sbyte[8];
			int n = ReadSocksReply(@in, data);
			if (n != 8)
			{
				throw new SocketException("Reply from SOCKS server has bad length: " + n);
			}
			if (data[0] != 0 && data[0] != 4)
			{
				throw new SocketException("Reply from SOCKS server has bad version");
			}
			SocketException ex = null;
			switch (data[1])
			{
			case 90:
				// Success!
				External_address = new InetSocketAddress(baddr, lport);
				break;
			case 91:
				ex = new SocketException("SOCKS request rejected");
				break;
			case 92:
				ex = new SocketException("SOCKS server couldn't reach destination");
				break;
			case 93:
				ex = new SocketException("SOCKS authentication failed");
				break;
			default:
				ex = new SocketException("Reply from SOCKS server contains bad status");
				break;
			}
			if (ex != null)
			{
				@in.Close();
				@out.Close();
				throw ex;
			}

		}

		private class PrivilegedActionAnonymousInnerClassHelper3 : PrivilegedAction<InetAddress>
		{
			private readonly SocksSocketImpl OuterInstance;

			public PrivilegedActionAnonymousInnerClassHelper3(SocksSocketImpl outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			public virtual InetAddress Run()
			{
				return OuterInstance.Cmdsock.LocalAddress;

			}
		}

		/// <summary>
		/// Sends the Bind request to the SOCKS proxy. In the SOCKS protocol, bind
		/// means "accept incoming connection from", so the SocketAddress is the
		/// the one of the host we do accept connection from.
		/// </summary>
		/// <param name="saddr">   the Socket address of the remote host. </param>
		/// <exception cref="IOException">  if an I/O error occurs when binding this socket. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected synchronized void socksBind(InetSocketAddress saddr) throws java.io.IOException
		protected internal virtual void SocksBind(InetSocketAddress saddr)
		{
			lock (this)
			{
				if (Socket_Renamed != null)
				{
					// this is a client socket, not a server socket, don't
					// call the SOCKS proxy for a bind!
					return;
				}
        
				// Connects to the SOCKS server
        
				if (Server == null)
				{
					// This is the general case
					// server is not null only when the socket was created with a
					// specified proxy in which case it does bypass the ProxySelector
					ProxySelector sel = AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper4(this));
					if (sel == null)
					{
						/*
						 * No default proxySelector --> direct connection
						 */
						return;
					}
					URI uri;
					// Use getHostString() to avoid reverse lookups
					String host = saddr.HostString;
					// IPv6 litteral?
					if (saddr.Address is Inet6Address && (!host.StartsWith("[")) && (host.IndexOf(":") >= 0))
					{
						host = "[" + host + "]";
					}
					try
					{
						uri = new URI("serversocket://" + ParseUtil.encodePath(host) + ":" + saddr.Port);
					}
					catch (URISyntaxException e)
					{
						// This shouldn't happen
						Debug.Assert(false, e);
						uri = null;
					}
					Proxy p = null;
					Exception savedExc = null;
					IEnumerator<Proxy> iProxy = null;
					iProxy = sel.Select(uri).GetEnumerator();
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					if (iProxy == null || !(iProxy.hasNext()))
					{
						return;
					}
					while (iProxy.MoveNext())
					{
						p = iProxy.Current;
						if (p == null || p.Type() != Proxy.Type.SOCKS)
						{
							return;
						}
        
						if (!(p.Address() is InetSocketAddress))
						{
							throw new SocketException("Unknown address type for proxy: " + p);
						}
						// Use getHostString() to avoid reverse lookups
						Server = ((InetSocketAddress) p.Address()).HostString;
						ServerPort = ((InetSocketAddress) p.Address()).Port;
						if (p is SocksProxy)
						{
							if (((SocksProxy)p).protocolVersion() == 4)
							{
								UseV4 = true;
							}
						}
        
						// Connects to the SOCKS server
						try
						{
							AccessController.doPrivileged(new PrivilegedExceptionActionAnonymousInnerClassHelper2(this));
						}
						catch (Exception e)
						{
							// Ooops, let's notify the ProxySelector
							sel.ConnectFailed(uri,p.Address(),new SocketException(e.Message));
							Server = null;
							ServerPort = -1;
							Cmdsock = null;
							savedExc = e;
							// Will continue the while loop and try the next proxy
						}
					}
        
					/*
					 * If server is still null at this point, none of the proxy
					 * worked
					 */
					if (Server == null || Cmdsock == null)
					{
						throw new SocketException("Can't connect to SOCKS proxy:" + savedExc.Message);
					}
				}
				else
				{
					try
					{
						AccessController.doPrivileged(new PrivilegedExceptionActionAnonymousInnerClassHelper3(this));
					}
					catch (Exception e)
					{
						throw new SocketException(e.Message);
					}
				}
				BufferedOutputStream @out = new BufferedOutputStream(CmdOut, 512);
				InputStream @in = CmdIn;
				if (UseV4)
				{
					BindV4(@in, @out, saddr.Address, saddr.Port);
					return;
				}
				@out.Write(SocksConsts_Fields.PROTO_VERS);
				@out.Write(2);
				@out.Write(SocksConsts_Fields.NO_AUTH);
				@out.Write(SocksConsts_Fields.USER_PASSW);
				@out.Flush();
				sbyte[] data = new sbyte[2];
				int i = ReadSocksReply(@in, data);
				if (i != 2 || ((int)data[0]) != SocksConsts_Fields.PROTO_VERS)
				{
					// Maybe it's not a V5 sever after all
					// Let's try V4 before we give up
					BindV4(@in, @out, saddr.Address, saddr.Port);
					return;
				}
				if (((int)data[1]) == SocksConsts_Fields.NO_METHODS)
				{
					throw new SocketException("SOCKS : No acceptable methods");
				}
				if (!Authenticate(data[1], @in, @out))
				{
					throw new SocketException("SOCKS : authentication failed");
				}
				// We're OK. Let's issue the BIND command.
				@out.Write(SocksConsts_Fields.PROTO_VERS);
				@out.Write(SocksConsts_Fields.BIND);
				@out.Write(0);
				int lport = saddr.Port;
				if (saddr.Unresolved)
				{
					@out.Write(SocksConsts_Fields.DOMAIN_NAME);
					@out.Write(saddr.HostName.Length());
					try
					{
						@out.Write(saddr.HostName.GetBytes("ISO-8859-1"));
					}
					catch (java.io.UnsupportedEncodingException)
					{
						Debug.Assert(false);
					}
					@out.Write((lport >> 8) & 0xff);
					@out.Write((lport >> 0) & 0xff);
				}
				else if (saddr.Address is Inet4Address)
				{
					sbyte[] addr1 = saddr.Address.Address;
					@out.Write(SocksConsts_Fields.IPV4);
					@out.Write(addr1);
					@out.Write((lport >> 8) & 0xff);
					@out.Write((lport >> 0) & 0xff);
					@out.Flush();
				}
				else if (saddr.Address is Inet6Address)
				{
					sbyte[] addr1 = saddr.Address.Address;
					@out.Write(SocksConsts_Fields.IPV6);
					@out.Write(addr1);
					@out.Write((lport >> 8) & 0xff);
					@out.Write((lport >> 0) & 0xff);
					@out.Flush();
				}
				else
				{
					Cmdsock.Close();
					throw new SocketException("unsupported address type : " + saddr);
				}
				data = new sbyte[4];
				i = ReadSocksReply(@in, data);
				SocketException ex = null;
				int len, nport;
				sbyte[] addr;
				switch (data[1])
				{
				case SocksConsts_Fields.REQUEST_OK:
					// success!
					switch (data[3])
					{
					case SocksConsts_Fields.IPV4:
						addr = new sbyte[4];
						i = ReadSocksReply(@in, addr);
						if (i != 4)
						{
							throw new SocketException("Reply from SOCKS server badly formatted");
						}
						data = new sbyte[2];
						i = ReadSocksReply(@in, data);
						if (i != 2)
						{
							throw new SocketException("Reply from SOCKS server badly formatted");
						}
						nport = ((int)data[0] & 0xff) << 8;
						nport += ((int)data[1] & 0xff);
						External_address = new InetSocketAddress(new Inet4Address("", addr), nport);
						break;
					case SocksConsts_Fields.DOMAIN_NAME:
						len = data[1];
						sbyte[] host = new sbyte[len];
						i = ReadSocksReply(@in, host);
						if (i != len)
						{
							throw new SocketException("Reply from SOCKS server badly formatted");
						}
						data = new sbyte[2];
						i = ReadSocksReply(@in, data);
						if (i != 2)
						{
							throw new SocketException("Reply from SOCKS server badly formatted");
						}
						nport = ((int)data[0] & 0xff) << 8;
						nport += ((int)data[1] & 0xff);
						External_address = new InetSocketAddress(StringHelperClass.NewString(host), nport);
						break;
					case SocksConsts_Fields.IPV6:
						len = data[1];
						addr = new sbyte[len];
						i = ReadSocksReply(@in, addr);
						if (i != len)
						{
							throw new SocketException("Reply from SOCKS server badly formatted");
						}
						data = new sbyte[2];
						i = ReadSocksReply(@in, data);
						if (i != 2)
						{
							throw new SocketException("Reply from SOCKS server badly formatted");
						}
						nport = ((int)data[0] & 0xff) << 8;
						nport += ((int)data[1] & 0xff);
						External_address = new InetSocketAddress(new Inet6Address("", addr), nport);
						break;
					}
					break;
				case SocksConsts_Fields.GENERAL_FAILURE:
					ex = new SocketException("SOCKS server general failure");
					break;
				case SocksConsts_Fields.NOT_ALLOWED:
					ex = new SocketException("SOCKS: Bind not allowed by ruleset");
					break;
				case SocksConsts_Fields.NET_UNREACHABLE:
					ex = new SocketException("SOCKS: Network unreachable");
					break;
				case SocksConsts_Fields.HOST_UNREACHABLE:
					ex = new SocketException("SOCKS: Host unreachable");
					break;
				case SocksConsts_Fields.CONN_REFUSED:
					ex = new SocketException("SOCKS: Connection refused");
					break;
				case SocksConsts_Fields.TTL_EXPIRED:
					ex = new SocketException("SOCKS: TTL expired");
					break;
				case SocksConsts_Fields.CMD_NOT_SUPPORTED:
					ex = new SocketException("SOCKS: Command not supported");
					break;
				case SocksConsts_Fields.ADDR_TYPE_NOT_SUP:
					ex = new SocketException("SOCKS: address type not supported");
					break;
				}
				if (ex != null)
				{
					@in.Close();
					@out.Close();
					Cmdsock.Close();
					Cmdsock = null;
					throw ex;
				}
				CmdIn = @in;
				CmdOut = @out;
			}
		}

		private class PrivilegedActionAnonymousInnerClassHelper4 : PrivilegedAction<ProxySelector>
		{
			private readonly SocksSocketImpl OuterInstance;

			public PrivilegedActionAnonymousInnerClassHelper4(SocksSocketImpl outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			public virtual ProxySelector Run()
			{
					return ProxySelector.Default;
			}
		}

		private class PrivilegedExceptionActionAnonymousInnerClassHelper2 : PrivilegedExceptionAction<Void>
		{
			private readonly SocksSocketImpl OuterInstance;

			public PrivilegedExceptionActionAnonymousInnerClassHelper2(SocksSocketImpl outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Void run() throws Exception
			public virtual Void Run()
			{
				OuterInstance.Cmdsock = new Socket(new PlainSocketImpl());
				OuterInstance.Cmdsock.Connect(new InetSocketAddress(OuterInstance.Server, OuterInstance.ServerPort));
				OuterInstance.CmdIn = OuterInstance.Cmdsock.InputStream;
				OuterInstance.CmdOut = OuterInstance.Cmdsock.OutputStream;
				return null;
			}
		}

		private class PrivilegedExceptionActionAnonymousInnerClassHelper3 : PrivilegedExceptionAction<Void>
		{
			private readonly SocksSocketImpl OuterInstance;

			public PrivilegedExceptionActionAnonymousInnerClassHelper3(SocksSocketImpl outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Void run() throws Exception
			public virtual Void Run()
			{
				OuterInstance.Cmdsock = new Socket(new PlainSocketImpl());
				OuterInstance.Cmdsock.Connect(new InetSocketAddress(OuterInstance.Server, OuterInstance.ServerPort));
				OuterInstance.CmdIn = OuterInstance.Cmdsock.InputStream;
				OuterInstance.CmdOut = OuterInstance.Cmdsock.OutputStream;
				return null;
			}
		}

		/// <summary>
		/// Accepts a connection from a specific host.
		/// </summary>
		/// <param name="s">   the accepted connection. </param>
		/// <param name="saddr"> the socket address of the host we do accept
		///               connection from </param>
		/// <exception cref="IOException">  if an I/O error occurs when accepting the
		///               connection. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void acceptFrom(SocketImpl s, InetSocketAddress saddr) throws java.io.IOException
		protected internal virtual void AcceptFrom(SocketImpl s, InetSocketAddress saddr)
		{
			if (Cmdsock == null)
			{
				// Not a Socks ServerSocket.
				return;
			}
			InputStream @in = CmdIn;
			// Sends the "SOCKS BIND" request.
			SocksBind(saddr);
			@in.Read();
			int i = @in.Read();
			@in.Read();
			SocketException ex = null;
			int nport;
			sbyte[] addr;
			InetSocketAddress real_end = null;
			switch (i)
			{
			case SocksConsts_Fields.REQUEST_OK:
				// success!
				i = @in.Read();
				switch (i)
				{
				case SocksConsts_Fields.IPV4:
					addr = new sbyte[4];
					ReadSocksReply(@in, addr);
					nport = @in.Read() << 8;
					nport += @in.Read();
					real_end = new InetSocketAddress(new Inet4Address("", addr), nport);
					break;
				case SocksConsts_Fields.DOMAIN_NAME:
					int len = @in.Read();
					addr = new sbyte[len];
					ReadSocksReply(@in, addr);
					nport = @in.Read() << 8;
					nport += @in.Read();
					real_end = new InetSocketAddress(StringHelperClass.NewString(addr), nport);
					break;
				case SocksConsts_Fields.IPV6:
					addr = new sbyte[16];
					ReadSocksReply(@in, addr);
					nport = @in.Read() << 8;
					nport += @in.Read();
					real_end = new InetSocketAddress(new Inet6Address("", addr), nport);
					break;
				}
				break;
			case SocksConsts_Fields.GENERAL_FAILURE:
				ex = new SocketException("SOCKS server general failure");
				break;
			case SocksConsts_Fields.NOT_ALLOWED:
				ex = new SocketException("SOCKS: Accept not allowed by ruleset");
				break;
			case SocksConsts_Fields.NET_UNREACHABLE:
				ex = new SocketException("SOCKS: Network unreachable");
				break;
			case SocksConsts_Fields.HOST_UNREACHABLE:
				ex = new SocketException("SOCKS: Host unreachable");
				break;
			case SocksConsts_Fields.CONN_REFUSED:
				ex = new SocketException("SOCKS: Connection refused");
				break;
			case SocksConsts_Fields.TTL_EXPIRED:
				ex = new SocketException("SOCKS: TTL expired");
				break;
			case SocksConsts_Fields.CMD_NOT_SUPPORTED:
				ex = new SocketException("SOCKS: Command not supported");
				break;
			case SocksConsts_Fields.ADDR_TYPE_NOT_SUP:
				ex = new SocketException("SOCKS: address type not supported");
				break;
			}
			if (ex != null)
			{
				CmdIn.Close();
				CmdOut.Close();
				Cmdsock.Close();
				Cmdsock = null;
				throw ex;
			}

			/// <summary>
			/// This is where we have to do some fancy stuff.
			/// The datastream from the socket "accepted" by the proxy will
			/// come through the cmdSocket. So we have to swap the socketImpls
			/// </summary>
			if (s is SocksSocketImpl)
			{
				((SocksSocketImpl)s).External_address = real_end;
			}
			if (s is PlainSocketImpl)
			{
				PlainSocketImpl psi = (PlainSocketImpl) s;
				psi.SetInputStream((SocketInputStream) @in);
				psi.FileDescriptor = Cmdsock.Impl.FileDescriptor;
				psi.Address = Cmdsock.Impl.InetAddress;
				psi.Port = Cmdsock.Impl.Port;
				psi.LocalPort = Cmdsock.Impl.LocalPort;
			}
			else
			{
				s.Fd = Cmdsock.Impl.Fd;
				s.Address = Cmdsock.Impl.Address;
				s.Port_Renamed = Cmdsock.Impl.Port_Renamed;
				s.Localport = Cmdsock.Impl.Localport;
			}

			// Need to do that so that the socket won't be closed
			// when the ServerSocket is closed by the user.
			// It kinds of detaches the Socket because it is now
			// used elsewhere.
			Cmdsock = null;
		}


		/// <summary>
		/// Returns the value of this socket's {@code address} field.
		/// </summary>
		/// <returns>  the value of this socket's {@code address} field. </returns>
		/// <seealso cref=     java.net.SocketImpl#address </seealso>
		protected internal override InetAddress InetAddress
		{
			get
			{
				if (External_address != null)
				{
					return External_address.Address;
				}
				else
				{
					return base.InetAddress;
				}
			}
		}

		/// <summary>
		/// Returns the value of this socket's {@code port} field.
		/// </summary>
		/// <returns>  the value of this socket's {@code port} field. </returns>
		/// <seealso cref=     java.net.SocketImpl#port </seealso>
		protected internal override int Port
		{
			get
			{
				if (External_address != null)
				{
					return External_address.Port;
				}
				else
				{
					return base.Port;
				}
			}
		}

		protected internal override int LocalPort
		{
			get
			{
				if (Socket_Renamed != null)
				{
					return base.LocalPort;
				}
				if (External_address != null)
				{
					return External_address.Port;
				}
				else
				{
					return base.LocalPort;
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void close() throws java.io.IOException
		protected internal override void Close()
		{
			if (Cmdsock != null)
			{
				Cmdsock.Close();
			}
			Cmdsock = null;
			base.Close();
		}

		private String UserName
		{
			get
			{
				String userName = "";
				if (ApplicationSetProxy)
				{
					try
					{
						userName = System.getProperty("user.name");
					} // swallow Exception
					catch (SecurityException)
					{
					}
				}
				else
				{
					userName = AccessController.doPrivileged(new sun.security.action.GetPropertyAction("user.name"));
				}
				return userName;
			}
		}
	}

}