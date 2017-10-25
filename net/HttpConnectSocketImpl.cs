using System.Collections.Generic;

/*
 * Copyright (c) 2010, 2013, Oracle and/or its affiliates. All rights reserved.
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
	/// Basic SocketImpl that relies on the internal HTTP protocol handler
	/// implementation to perform the HTTP tunneling and authentication. The
	/// sockets impl is swapped out and replaced with the socket from the HTTP
	/// handler after the tunnel is successfully setup.
	/// 
	/// @since 1.8
	/// </summary>

	/*package*/	 internal class HttpConnectSocketImpl : PlainSocketImpl
	 {

		private const String HttpURLClazzStr = "sun.net.www.protocol.http.HttpURLConnection";
		private const String NetClientClazzStr = "sun.net.NetworkClient";
		private const String DoTunnelingStr = "doTunneling";
		private static readonly Field HttpField;
		private static readonly Field ServerSocketField;
		private static readonly Method DoTunneling_Renamed;

		private readonly String Server;
		private InetSocketAddress External_address;
		private Dictionary<Integer, Object> OptionsMap = new Dictionary<Integer, Object>();

		static HttpConnectSocketImpl()
		{
			try
			{
				Class httpClazz = Class.ForName(HttpURLClazzStr, true, null);
				HttpField = httpClazz.GetDeclaredField("http");
				DoTunneling_Renamed = httpClazz.getDeclaredMethod(DoTunnelingStr);
				Class netClientClazz = Class.ForName(NetClientClazzStr, true, null);
				ServerSocketField = netClientClazz.GetDeclaredField("serverSocket");

				java.security.AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper());
			}
			catch (ReflectiveOperationException x)
			{
				throw new InternalError("Should not reach here", x);
			}
		}

		private class PrivilegedActionAnonymousInnerClassHelper : java.security.PrivilegedAction<Void>
		{
			public PrivilegedActionAnonymousInnerClassHelper()
			{
			}

			public virtual Void Run()
			{
				HttpField.Accessible = true;
				ServerSocketField.Accessible = true;
				return null;
			}
		}

		internal HttpConnectSocketImpl(String server, int port)
		{
			this.Server = server;
			this.Port_Renamed = port;
		}

		internal HttpConnectSocketImpl(Proxy proxy)
		{
			SocketAddress a = proxy.Address();
			if (!(a is InetSocketAddress))
			{
				throw new IllegalArgumentException("Unsupported address type");
			}

			InetSocketAddress ad = (InetSocketAddress) a;
			Server = ad.HostString;
			Port_Renamed = ad.Port;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void connect(SocketAddress endpoint, int timeout) throws java.io.IOException
		protected internal override void Connect(SocketAddress endpoint, int timeout)
		{
			if (endpoint == null || !(endpoint is InetSocketAddress))
			{
				throw new IllegalArgumentException("Unsupported address type");
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final InetSocketAddress epoint = (InetSocketAddress)endpoint;
			InetSocketAddress epoint = (InetSocketAddress)endpoint;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String destHost = epoint.isUnresolved() ? epoint.getHostName() : epoint.getAddress().getHostAddress();
			String destHost = epoint.Unresolved ? epoint.HostName : epoint.Address.HostAddress;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int destPort = epoint.getPort();
			int destPort = epoint.Port;

			SecurityManager security = System.SecurityManager;
			if (security != null)
			{
				security.CheckConnect(destHost, destPort);
			}

			// Connect to the HTTP proxy server
			String urlString = "http://" + destHost + ":" + destPort;
			Socket httpSocket = PrivilegedDoTunnel(urlString, timeout);

			// Success!
			External_address = epoint;

			// close the original socket impl and release its descriptor
			Close();

			// update the Sockets impl to the impl from the http Socket
			AbstractPlainSocketImpl psi = (AbstractPlainSocketImpl) httpSocket.Impl_Renamed;
			this.Socket.Impl_Renamed = psi;

			// best effort is made to try and reset options previously set
//JAVA TO C# CONVERTER TODO TASK: There is no .NET Dictionary equivalent to the Java 'entrySet' method:
			Set<java.util.Map_Entry<Integer, Object>> options = OptionsMap.entrySet();
			try
			{
				foreach (java.util.Map_Entry<Integer, Object> entry in options)
				{
					psi.SetOption(entry.Key, entry.Value);
				}
			} // gulp!
			catch (IOException)
			{
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void setOption(int opt, Object val) throws SocketException
		public override void SetOption(int opt, Object val)
		{
			base.SetOption(opt, val);

			if (External_address != null)
			{
				return; // we're connected, just return
			}

			// store options so that they can be re-applied to the impl after connect
			OptionsMap[opt] = val;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private Socket privilegedDoTunnel(final String urlString, final int timeout) throws java.io.IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		private Socket PrivilegedDoTunnel(String urlString, int timeout)
		{
			try
			{
				return java.security.AccessController.doPrivileged(new PrivilegedExceptionActionAnonymousInnerClassHelper(this, urlString, timeout));
			}
			catch (java.security.PrivilegedActionException pae)
			{
				throw (IOException) pae.Exception;
			}
		}

		private class PrivilegedExceptionActionAnonymousInnerClassHelper : java.security.PrivilegedExceptionAction<Socket>
		{
			private readonly HttpConnectSocketImpl OuterInstance;

			private string UrlString;
			private int Timeout;

			public PrivilegedExceptionActionAnonymousInnerClassHelper(HttpConnectSocketImpl outerInstance, string urlString, int timeout)
			{
				this.OuterInstance = outerInstance;
				this.UrlString = urlString;
				this.Timeout = timeout;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Socket run() throws java.io.IOException
			public virtual Socket Run()
			{
				return outerInstance.DoTunnel(UrlString, Timeout);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private Socket doTunnel(String urlString, int connectTimeout) throws java.io.IOException
		private Socket DoTunnel(String urlString, int connectTimeout)
		{
			Proxy proxy = new Proxy(Proxy.Type.HTTP, new InetSocketAddress(Server, Port_Renamed));
			URL destURL = new URL(urlString);
			HttpURLConnection conn = (HttpURLConnection) destURL.OpenConnection(proxy);
			conn.ConnectTimeout = connectTimeout;
			conn.ReadTimeout = this.Timeout_Renamed;
			conn.Connect();
			DoTunneling(conn);
			try
			{
				Object httpClient = HttpField.get(conn);
				return (Socket) ServerSocketField.get(httpClient);
			}
			catch (IllegalAccessException x)
			{
				throw new InternalError("Should not reach here", x);
			}
		}

		private void DoTunneling(HttpURLConnection conn)
		{
			try
			{
				DoTunneling_Renamed.invoke(conn);
			}
			catch (ReflectiveOperationException x)
			{
				throw new InternalError("Should not reach here", x);
			}
		}

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
	 }

}