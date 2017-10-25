using System;
using System.Collections;
using System.Collections.Generic;

/*
 * Copyright (c) 1997, 2013, Oracle and/or its affiliates. All rights reserved.
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

	using IPAddressUtil = sun.net.util.IPAddressUtil;
	using RegisteredDomain = sun.net.RegisteredDomain;
	using PortConfig = sun.net.PortConfig;
	using SecurityConstants = sun.security.util.SecurityConstants;
	using Debug = sun.security.util.Debug;


	/// <summary>
	/// This class represents access to a network via sockets.
	/// A SocketPermission consists of a
	/// host specification and a set of "actions" specifying ways to
	/// connect to that host. The host is specified as
	/// <pre>
	///    host = (hostname | IPv4address | iPv6reference) [:portrange]
	///    portrange = portnumber | -portnumber | portnumber-[portnumber]
	/// </pre>
	/// The host is expressed as a DNS name, as a numerical IP address,
	/// or as "localhost" (for the local machine).
	/// The wildcard "*" may be included once in a DNS name host
	/// specification. If it is included, it must be in the leftmost
	/// position, as in "*.sun.com".
	/// <para>
	/// The format of the IPv6reference should follow that specified in <a
	/// href="http://www.ietf.org/rfc/rfc2732.txt"><i>RFC&nbsp;2732: Format
	/// for Literal IPv6 Addresses in URLs</i></a>:
	/// <pre>
	///    ipv6reference = "[" IPv6address "]"
	/// </pre>
	/// For example, you can construct a SocketPermission instance
	/// as the following:
	/// <pre>
	///    String hostAddress = inetaddress.getHostAddress();
	///    if (inetaddress instanceof Inet6Address) {
	///        sp = new SocketPermission("[" + hostAddress + "]:" + port, action);
	///    } else {
	///        sp = new SocketPermission(hostAddress + ":" + port, action);
	///    }
	/// </pre>
	/// or
	/// <pre>
	///    String host = url.getHost();
	///    sp = new SocketPermission(host + ":" + port, action);
	/// </pre>
	/// </para>
	/// <para>
	/// The <A HREF="Inet6Address.html#lform">full uncompressed form</A> of
	/// an IPv6 literal address is also valid.
	/// </para>
	/// <para>
	/// The port or portrange is optional. A port specification of the
	/// form "N-", where <i>N</i> is a port number, signifies all ports
	/// numbered <i>N</i> and above, while a specification of the
	/// form "-N" indicates all ports numbered <i>N</i> and below.
	/// The special port value {@code 0} refers to the entire <i>ephemeral</i>
	/// port range. This is a fixed range of ports a system may use to
	/// allocate dynamic ports from. The actual range may be system dependent.
	/// </para>
	/// <para>
	/// The possible ways to connect to the host are
	/// <pre>
	/// accept
	/// connect
	/// listen
	/// resolve
	/// </pre>
	/// The "listen" action is only meaningful when used with "localhost" and
	/// means the ability to bind to a specified port.
	/// The "resolve" action is implied when any of the other actions are present.
	/// The action "resolve" refers to host/ip name service lookups.
	/// <P>
	/// The actions string is converted to lowercase before processing.
	/// </para>
	/// <para>As an example of the creation and meaning of SocketPermissions,
	/// note that if the following permission:
	/// 
	/// <pre>
	///   p1 = new SocketPermission("puffin.eng.sun.com:7777", "connect,accept");
	/// </pre>
	/// 
	/// is granted to some code, it allows that code to connect to port 7777 on
	/// {@code puffin.eng.sun.com}, and to accept connections on that port.
	/// 
	/// </para>
	/// <para>Similarly, if the following permission:
	/// 
	/// <pre>
	///   p2 = new SocketPermission("localhost:1024-", "accept,connect,listen");
	/// </pre>
	/// 
	/// is granted to some code, it allows that code to
	/// accept connections on, connect to, or listen on any port between
	/// 1024 and 65535 on the local host.
	/// 
	/// </para>
	/// <para>Note: Granting code permission to accept or make connections to remote
	/// hosts may be dangerous because malevolent code can then more easily
	/// transfer and share confidential data among parties who may not
	/// otherwise have access to the data.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= java.security.Permissions </seealso>
	/// <seealso cref= SocketPermission
	/// 
	/// 
	/// @author Marianne Mueller
	/// @author Roland Schemers
	/// 
	/// @serial exclude </seealso>

	[Serializable]
	public sealed class SocketPermission : Permission
	{
		private const long SerialVersionUID = -7204263841984476862L;

		/// <summary>
		/// Connect to host:port
		/// </summary>
		private const int CONNECT = 0x1;

		/// <summary>
		/// Listen on host:port
		/// </summary>
		private const int LISTEN = 0x2;

		/// <summary>
		/// Accept a connection from host:port
		/// </summary>
		private const int ACCEPT = 0x4;

		/// <summary>
		/// Resolve DNS queries
		/// </summary>
		private const int RESOLVE = 0x8;

		/// <summary>
		/// No actions
		/// </summary>
		private const int NONE = 0x0;

		/// <summary>
		/// All actions
		/// </summary>
		private static readonly int ALL = CONNECT | LISTEN | ACCEPT | RESOLVE;

		// various port constants
		private const int PORT_MIN = 0;
		private const int PORT_MAX = 65535;
		private const int PRIV_PORT_MAX = 1023;
		private const int DEF_EPH_LOW = 49152;

		// the actions mask
		[NonSerialized]
		private int Mask_Renamed;

		/// <summary>
		/// the actions string.
		/// 
		/// @serial
		/// </summary>

		private String Actions_Renamed; // Left null as long as possible, then
								// created and re-used in the getAction function.

		// hostname part as it is passed
		[NonSerialized]
		private String Hostname;

		// the canonical name of the host
		// in the case of "*.foo.com", cname is ".foo.com".

		[NonSerialized]
		private String Cname;

		// all the IP addresses of the host
		[NonSerialized]
		private InetAddress[] Addresses;

		// true if the hostname is a wildcard (e.g. "*.sun.com")
		[NonSerialized]
		private bool Wildcard;

		// true if we were initialized with a single numeric IP address
		[NonSerialized]
		private bool Init_with_ip;

		// true if this SocketPermission represents an invalid/unknown host
		// used for implies when the delayed lookup has already failed
		[NonSerialized]
		private bool Invalid;

		// port range on host
		[NonSerialized]
		private int[] Portrange;

		[NonSerialized]
		private bool DefaultDeny = false;

		// true if this SocketPermission represents a hostname
		// that failed our reverse mapping heuristic test
		[NonSerialized]
		private bool Untrusted_Renamed;
		[NonSerialized]
		private bool Trusted;

		// true if the sun.net.trustNameService system property is set
		private static bool TrustNameService;

		private static Debug Debug_Renamed = null;
		private static bool DebugInit = false;

		// lazy initializer
		private class EphemeralRange
		{
			internal static readonly int Low = InitEphemeralPorts("low", DEF_EPH_LOW);
			internal static readonly int High = InitEphemeralPorts("high", PORT_MAX);
		}

		static SocketPermission()
		{
			Boolean tmp = AccessController.doPrivileged(new sun.security.action.GetBooleanAction("sun.net.trustNameService"));
			TrustNameService = tmp.BooleanValue();
		}

		private static Debug Debug
		{
			get
			{
				lock (typeof(SocketPermission))
				{
					if (!DebugInit)
					{
						Debug_Renamed = Debug.getInstance("access");
						DebugInit = true;
					}
					return Debug_Renamed;
				}
			}
		}

		/// <summary>
		/// Creates a new SocketPermission object with the specified actions.
		/// The host is expressed as a DNS name, or as a numerical IP address.
		/// Optionally, a port or a portrange may be supplied (separated
		/// from the DNS name or IP address by a colon).
		/// <para>
		/// To specify the local machine, use "localhost" as the <i>host</i>.
		/// Also note: An empty <i>host</i> String ("") is equivalent to "localhost".
		/// </para>
		/// <para>
		/// The <i>actions</i> parameter contains a comma-separated list of the
		/// actions granted for the specified host (and port(s)). Possible actions are
		/// "connect", "listen", "accept", "resolve", or
		/// any combination of those. "resolve" is automatically added
		/// when any of the other three are specified.
		/// </para>
		/// <para>
		/// Examples of SocketPermission instantiation are the following:
		/// <pre>
		///    nr = new SocketPermission("www.catalog.com", "connect");
		///    nr = new SocketPermission("www.sun.com:80", "connect");
		///    nr = new SocketPermission("*.sun.com", "connect");
		///    nr = new SocketPermission("*.edu", "resolve");
		///    nr = new SocketPermission("204.160.241.0", "connect");
		///    nr = new SocketPermission("localhost:1024-65535", "listen");
		///    nr = new SocketPermission("204.160.241.0:1024-65535", "connect");
		/// </pre>
		/// 
		/// </para>
		/// </summary>
		/// <param name="host"> the hostname or IPaddress of the computer, optionally
		/// including a colon followed by a port or port range. </param>
		/// <param name="action"> the action string. </param>
		public SocketPermission(String host, String action) : base(GetHost(host))
		{
			// name initialized to getHost(host); NPE detected in getHost()
			Init(Name, GetMask(action));
		}


		internal SocketPermission(String host, int mask) : base(GetHost(host))
		{
			// name initialized to getHost(host); NPE detected in getHost()
			Init(Name, mask);
		}

		private void SetDeny()
		{
			DefaultDeny = true;
		}

		private static String GetHost(String host)
		{
			if (host.Equals(""))
			{
				return "localhost";
			}
			else
			{
				/* IPv6 literal address used in this context should follow
				 * the format specified in RFC 2732;
				 * if not, we try to solve the unambiguous case
				 */
				int ind;
				if (host.CharAt(0) != '[')
				{
					if ((ind = host.IndexOf(':')) != host.LastIndexOf(':'))
					{
						/* More than one ":", meaning IPv6 address is not
						 * in RFC 2732 format;
						 * We will rectify user errors for all unambiguious cases
						 */
						StringTokenizer st = new StringTokenizer(host, ":");
						int tokens = st.CountTokens();
						if (tokens == 9)
						{
							// IPv6 address followed by port
							ind = host.LastIndexOf(':');
							host = "[" + host.Substring(0, ind) + "]" + host.Substring(ind);
						}
						else if (tokens == 8 && host.IndexOf("::") == -1)
						{
							// IPv6 address only, not followed by port
							host = "[" + host + "]";
						}
						else
						{
							// could be ambiguous
							throw new IllegalArgumentException("Ambiguous" + " hostport part");
						}
					}
				}
				return host;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private int[] parsePort(String port) throws Exception
		private int[] ParsePort(String port)
		{

			if (port == null || port.Equals("") || port.Equals("*"))
			{
				return new int[] {PORT_MIN, PORT_MAX};
			}

			int dash = port.IndexOf('-');

			if (dash == -1)
			{
				int p = Convert.ToInt32(port);
				return new int[] {p, p};
			}
			else
			{
				String low = port.Substring(0, dash);
				String high = port.Substring(dash + 1);
				int l, h;

				if (low.Equals(""))
				{
					l = PORT_MIN;
				}
				else
				{
					l = Convert.ToInt32(low);
				}

				if (high.Equals(""))
				{
					h = PORT_MAX;
				}
				else
				{
					h = Convert.ToInt32(high);
				}
				if (l < 0 || h < 0 || h < l)
				{
					throw new IllegalArgumentException("invalid port range");
				}

				return new int[] {l, h};
			}
		}

		/// <summary>
		/// Returns true if the permission has specified zero
		/// as its value (or lower bound) signifying the ephemeral range
		/// </summary>
		private bool IncludesEphemerals()
		{
			return Portrange[0] == 0;
		}

		/// <summary>
		/// Initialize the SocketPermission object. We don't do any DNS lookups
		/// as this point, instead we hold off until the implies method is
		/// called.
		/// </summary>
		private void Init(String host, int mask)
		{
			// Set the integer mask that represents the actions

			if ((mask & ALL) != mask)
			{
				throw new IllegalArgumentException("invalid actions mask");
			}

			// always OR in RESOLVE if we allow any of the others
			this.Mask_Renamed = mask | RESOLVE;

			// Parse the host name.  A name has up to three components, the
			// hostname, a port number, or two numbers representing a port
			// range.   "www.sun.com:8080-9090" is a valid host name.

			// With IPv6 an address can be 2010:836B:4179::836B:4179
			// An IPv6 address needs to be enclose in []
			// For ex: [2010:836B:4179::836B:4179]:8080-9090
			// Refer to RFC 2732 for more information.

			int rb = 0;
			int start = 0, end = 0;
			int sep = -1;
			String hostport = host;
			if (host.CharAt(0) == '[')
			{
				start = 1;
				rb = host.IndexOf(']');
				if (rb != -1)
				{
					host = host.Substring(start, rb - start);
				}
				else
				{
					throw new IllegalArgumentException("invalid host/port: " + host);
				}
				sep = hostport.IndexOf(':', rb + 1);
			}
			else
			{
				start = 0;
				sep = host.IndexOf(':', rb);
				end = sep;
				if (sep != -1)
				{
					host = host.Substring(start, end - start);
				}
			}

			if (sep != -1)
			{
				String port = hostport.Substring(sep + 1);
				try
				{
					Portrange = ParsePort(port);
				}
				catch (Exception)
				{
					throw new IllegalArgumentException("invalid port range: " + port);
				}
			}
			else
			{
				Portrange = new int[] {PORT_MIN, PORT_MAX};
			}

			Hostname = host;

			// is this a domain wildcard specification
			if (host.LastIndexOf('*') > 0)
			{
				throw new IllegalArgumentException("invalid host wildcard specification");
			}
			else if (host.StartsWith("*"))
			{
				Wildcard = true;
				if (host.Equals("*"))
				{
					Cname = "";
				}
				else if (host.StartsWith("*."))
				{
					Cname = host.Substring(1).ToLowerCase();
				}
				else
				{
				  throw new IllegalArgumentException("invalid host wildcard specification");
				}
				return;
			}
			else
			{
				if (host.Length() > 0)
				{
					// see if we are being initialized with an IP address.
					char ch = host.CharAt(0);
					if (ch == ':' || Character.Digit(ch, 16) != -1)
					{
						sbyte[] ip = IPAddressUtil.textToNumericFormatV4(host);
						if (ip == null)
						{
							ip = IPAddressUtil.textToNumericFormatV6(host);
						}
						if (ip != null)
						{
							try
							{
								Addresses = new InetAddress[] {InetAddress.GetByAddress(ip)};
								Init_with_ip = true;
							}
							catch (UnknownHostException)
							{
								// this shouldn't happen
								Invalid = true;
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Convert an action string to an integer actions mask.
		/// </summary>
		/// <param name="action"> the action string </param>
		/// <returns> the action mask </returns>
		private static int GetMask(String action)
		{

			if (action == null)
			{
				throw new NullPointerException("action can't be null");
			}

			if (action.Equals(""))
			{
				throw new IllegalArgumentException("action can't be empty");
			}

			int mask = NONE;

			// Use object identity comparison against known-interned strings for
			// performance benefit (these values are used heavily within the JDK).
			if (action == SecurityConstants.SOCKET_RESOLVE_ACTION)
			{
				return RESOLVE;
			}
			else if (action == SecurityConstants.SOCKET_CONNECT_ACTION)
			{
				return CONNECT;
			}
			else if (action == SecurityConstants.SOCKET_LISTEN_ACTION)
			{
				return LISTEN;
			}
			else if (action == SecurityConstants.SOCKET_ACCEPT_ACTION)
			{
				return ACCEPT;
			}
			else if (action == SecurityConstants.SOCKET_CONNECT_ACCEPT_ACTION)
			{
				return CONNECT | ACCEPT;
			}

			char[] a = action.ToCharArray();

			int i = a.Length - 1;
			if (i < 0)
			{
				return mask;
			}

			while (i != -1)
			{
				char c;

				// skip whitespace
				while ((i != -1) && ((c = a[i]) == ' ' || c == '\r' || c == '\n' || c == '\f' || c == '\t'))
				{
					i--;
				}

				// check for the known strings
				int matchlen;

				if (i >= 6 && (a[i - 6] == 'c' || a[i - 6] == 'C') && (a[i - 5] == 'o' || a[i - 5] == 'O') && (a[i - 4] == 'n' || a[i - 4] == 'N') && (a[i - 3] == 'n' || a[i - 3] == 'N') && (a[i - 2] == 'e' || a[i - 2] == 'E') && (a[i - 1] == 'c' || a[i - 1] == 'C') && (a[i] == 't' || a[i] == 'T'))
				{
					matchlen = 7;
					mask |= CONNECT;

				}
				else if (i >= 6 && (a[i - 6] == 'r' || a[i - 6] == 'R') && (a[i - 5] == 'e' || a[i - 5] == 'E') && (a[i - 4] == 's' || a[i - 4] == 'S') && (a[i - 3] == 'o' || a[i - 3] == 'O') && (a[i - 2] == 'l' || a[i - 2] == 'L') && (a[i - 1] == 'v' || a[i - 1] == 'V') && (a[i] == 'e' || a[i] == 'E'))
				{
					matchlen = 7;
					mask |= RESOLVE;

				}
				else if (i >= 5 && (a[i - 5] == 'l' || a[i - 5] == 'L') && (a[i - 4] == 'i' || a[i - 4] == 'I') && (a[i - 3] == 's' || a[i - 3] == 'S') && (a[i - 2] == 't' || a[i - 2] == 'T') && (a[i - 1] == 'e' || a[i - 1] == 'E') && (a[i] == 'n' || a[i] == 'N'))
				{
					matchlen = 6;
					mask |= LISTEN;

				}
				else if (i >= 5 && (a[i - 5] == 'a' || a[i - 5] == 'A') && (a[i - 4] == 'c' || a[i - 4] == 'C') && (a[i - 3] == 'c' || a[i - 3] == 'C') && (a[i - 2] == 'e' || a[i - 2] == 'E') && (a[i - 1] == 'p' || a[i - 1] == 'P') && (a[i] == 't' || a[i] == 'T'))
				{
					matchlen = 6;
					mask |= ACCEPT;

				}
				else
				{
					// parse error
					throw new IllegalArgumentException("invalid permission: " + action);
				}

				// make sure we didn't just match the tail of a word
				// like "ackbarfaccept".  Also, skip to the comma.
				bool seencomma = false;
				while (i >= matchlen && !seencomma)
				{
					switch (a[i - matchlen])
					{
					case ',':
						seencomma = true;
						break;
					case ' ':
				case '\r':
			case '\n':
					case '\f':
				case '\t':
						break;
					default:
						throw new IllegalArgumentException("invalid permission: " + action);
					}
					i--;
				}

				// point i at the location of the comma minus one (or -1).
				i -= matchlen;
			}

			return mask;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private boolean isUntrusted() throws UnknownHostException
		private bool Untrusted
		{
			get
			{
				if (Trusted)
				{
					return false;
				}
				if (Invalid || Untrusted_Renamed)
				{
					return true;
				}
				try
				{
					if (!TrustNameService && (DefaultDeny || sun.net.www.URLConnection.isProxiedHost(Hostname)))
					{
						if (this.Cname == null)
						{
							this.CanonName;
						}
						if (!Match(Cname, Hostname))
						{
							// Last chance
							if (!Authorized(Hostname, Addresses[0].Address))
							{
								Untrusted_Renamed = true;
								Debug debug = Debug;
								if (debug != null && Debug.isOn("failure"))
								{
									debug.println("socket access restriction: proxied host " + "(" + Addresses[0] + ")" + " does not match " + Cname + " from reverse lookup");
								}
								return true;
							}
						}
						Trusted = true;
					}
				}
				catch (UnknownHostException uhe)
				{
					Invalid = true;
					throw uhe;
				}
				return false;
			}
		}

		/// <summary>
		/// attempt to get the fully qualified domain name
		/// 
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void getCanonName() throws UnknownHostException
		internal void GetCanonName()
		{
			if (Cname != null || Invalid || Untrusted_Renamed)
			{
				return;
			}

			// attempt to get the canonical name

			try
			{
				// first get the IP addresses if we don't have them yet
				// this is because we need the IP address to then get
				// FQDN.
				if (Addresses == null)
				{
					IP;
				}

				// we have to do this check, otherwise we might not
				// get the fully qualified domain name
				if (Init_with_ip)
				{
					Cname = Addresses[0].GetHostName(false).ToLowerCase();
				}
				else
				{
				 Cname = InetAddress.GetByName(Addresses[0].HostAddress).GetHostName(false).ToLowerCase();
				}
			}
			catch (UnknownHostException uhe)
			{
				Invalid = true;
				throw uhe;
			}
		}

		[NonSerialized]
		private String Cdomain, Hdomain;

		private bool Match(String cname, String hname)
		{
			String a = cname.ToLowerCase();
			String b = hname.ToLowerCase();
			if (a.StartsWith(b) && ((a.Length() == b.Length()) || (a.CharAt(b.Length()) == '.')))
			{
				return true;
			}
			if (Cdomain == null)
			{
				Cdomain = RegisteredDomain.getRegisteredDomain(a);
			}
			if (Hdomain == null)
			{
				Hdomain = RegisteredDomain.getRegisteredDomain(b);
			}

			return Cdomain.Length() != 0 && Hdomain.Length() != 0 && Cdomain.Equals(Hdomain);
		}

		private bool Authorized(String cname, sbyte[] addr)
		{
			if (addr.Length == 4)
			{
				return AuthorizedIPv4(cname, addr);
			}
			else if (addr.Length == 16)
			{
				return AuthorizedIPv6(cname, addr);
			}
			else
			{
				return false;
			}
		}

		private bool AuthorizedIPv4(String cname, sbyte[] addr)
		{
			String authHost = "";
			InetAddress auth;

			try
			{
				authHost = "auth." + (addr[3] & 0xff) + "." + (addr[2] & 0xff) + "." + (addr[1] & 0xff) + "." + (addr[0] & 0xff) + ".in-addr.arpa";
				// Following check seems unnecessary
				// auth = InetAddress.getAllByName0(authHost, false)[0];
				authHost = Hostname + '.' + authHost;
				auth = InetAddress.GetAllByName0(authHost, false)[0];
				if (auth.Equals(InetAddress.GetByAddress(addr)))
				{
					return true;
				}
				Debug debug = Debug;
				if (debug != null && Debug.isOn("failure"))
				{
					debug.println("socket access restriction: IP address of " + auth + " != " + InetAddress.GetByAddress(addr));
				}
			}
			catch (UnknownHostException)
			{
				Debug debug = Debug;
				if (debug != null && Debug.isOn("failure"))
				{
					debug.println("socket access restriction: forward lookup failed for " + authHost);
				}
			}
			return false;
		}

		private bool AuthorizedIPv6(String cname, sbyte[] addr)
		{
			String authHost = "";
			InetAddress auth;

			try
			{
				StringBuffer sb = new StringBuffer(39);

				for (int i = 15; i >= 0; i--)
				{
					sb.Append(((addr[i]) & 0x0f).ToString("x"));
					sb.Append('.');
					sb.Append(((addr[i] >> 4) & 0x0f).ToString("x"));
					sb.Append('.');
				}
				authHost = "auth." + sb.ToString() + "IP6.ARPA";
				//auth = InetAddress.getAllByName0(authHost, false)[0];
				authHost = Hostname + '.' + authHost;
				auth = InetAddress.GetAllByName0(authHost, false)[0];
				if (auth.Equals(InetAddress.GetByAddress(addr)))
				{
					return true;
				}
				Debug debug = Debug;
				if (debug != null && Debug.isOn("failure"))
				{
					debug.println("socket access restriction: IP address of " + auth + " != " + InetAddress.GetByAddress(addr));
				}
			}
			catch (UnknownHostException)
			{
				Debug debug = Debug;
				if (debug != null && Debug.isOn("failure"))
				{
					debug.println("socket access restriction: forward lookup failed for " + authHost);
				}
			}
			return false;
		}


		/// <summary>
		/// get IP addresses. Sets invalid to true if we can't get them.
		/// 
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void getIP() throws UnknownHostException
		internal void GetIP()
		{
			if (Addresses != null || Wildcard || Invalid)
			{
				return;
			}

			try
			{
				// now get all the IP addresses
				String host;
				if (Name.CharAt(0) == '[')
				{
					// Literal IPv6 address
					host = Name.Substring(1, Name.IndexOf(']') - 1);
				}
				else
				{
					int i = Name.IndexOf(":");
					if (i == -1)
					{
						host = Name;
					}
					else
					{
						host = Name.Substring(0,i);
					}
				}

				Addresses = new InetAddress[] {InetAddress.GetAllByName0(host, false)[0]};

			}
			catch (UnknownHostException uhe)
			{
				Invalid = true;
				throw uhe;
			}
			catch (IndexOutOfBoundsException)
			{
				Invalid = true;
				throw new UnknownHostException(Name);
			}
		}

		/// <summary>
		/// Checks if this socket permission object "implies" the
		/// specified permission.
		/// <P>
		/// More specifically, this method first ensures that all of the following
		/// are true (and returns false if any of them are not):
		/// <ul>
		/// <li> <i>p</i> is an instanceof SocketPermission,
		/// <li> <i>p</i>'s actions are a proper subset of this
		/// object's actions, and
		/// <li> <i>p</i>'s port range is included in this port range. Note:
		/// port range is ignored when p only contains the action, 'resolve'.
		/// </ul>
		/// 
		/// Then {@code implies} checks each of the following, in order,
		/// and for each returns true if the stated condition is true:
		/// <ul>
		/// <li> If this object was initialized with a single IP address and one of <i>p</i>'s
		/// IP addresses is equal to this object's IP address.
		/// <li>If this object is a wildcard domain (such as *.sun.com), and
		/// <i>p</i>'s canonical name (the name without any preceding *)
		/// ends with this object's canonical host name. For example, *.sun.com
		/// implies *.eng.sun.com.
		/// <li>If this object was not initialized with a single IP address, and one of this
		/// object's IP addresses equals one of <i>p</i>'s IP addresses.
		/// <li>If this canonical name equals <i>p</i>'s canonical name.
		/// </ul>
		/// 
		/// If none of the above are true, {@code implies} returns false. </summary>
		/// <param name="p"> the permission to check against.
		/// </param>
		/// <returns> true if the specified permission is implied by this object,
		/// false if not. </returns>
		public override bool Implies(Permission p)
		{
			int i, j;

			if (!(p is SocketPermission))
			{
				return false;
			}

			if (p == this)
			{
				return true;
			}

			SocketPermission that = (SocketPermission) p;

			return ((this.Mask_Renamed & that.Mask_Renamed) == that.Mask_Renamed) && ImpliesIgnoreMask(that);
		}

		/// <summary>
		/// Checks if the incoming Permission's action are a proper subset of
		/// the this object's actions.
		/// <P>
		/// Check, in the following order:
		/// <ul>
		/// <li> Checks that "p" is an instanceof a SocketPermission
		/// <li> Checks that "p"'s actions are a proper subset of the
		/// current object's actions.
		/// <li> Checks that "p"'s port range is included in this port range
		/// <li> If this object was initialized with an IP address, checks that
		///      one of "p"'s IP addresses is equal to this object's IP address.
		/// <li> If either object is a wildcard domain (i.e., "*.sun.com"),
		///      attempt to match based on the wildcard.
		/// <li> If this object was not initialized with an IP address, attempt
		///      to find a match based on the IP addresses in both objects.
		/// <li> Attempt to match on the canonical hostnames of both objects.
		/// </ul> </summary>
		/// <param name="that"> the incoming permission request
		/// </param>
		/// <returns> true if "permission" is a proper subset of the current object,
		/// false if not. </returns>
		internal bool ImpliesIgnoreMask(SocketPermission that)
		{
			int i, j;

			if ((that.Mask_Renamed & RESOLVE) != that.Mask_Renamed)
			{

				// check simple port range
				if ((that.Portrange[0] < this.Portrange[0]) || (that.Portrange[1] > this.Portrange[1]))
				{

					// if either includes the ephemeral range, do full check
					if (this.IncludesEphemerals() || that.IncludesEphemerals())
					{
						if (!InRange(this.Portrange[0], this.Portrange[1], that.Portrange[0], that.Portrange[1]))
						{
									return false;
						}
					}
					else
					{
						return false;
					}
				}
			}

			// allow a "*" wildcard to always match anything
			if (this.Wildcard && "".Equals(this.Cname))
			{
				return true;
			}

			// return if either one of these NetPerm objects are invalid...
			if (this.Invalid || that.Invalid)
			{
				return CompareHostnames(that);
			}

			try
			{
				if (this.Init_with_ip) // we only check IP addresses
				{
					if (that.Wildcard)
					{
						return false;
					}

					if (that.Init_with_ip)
					{
						return (this.Addresses[0].Equals(that.Addresses[0]));
					}
					else
					{
						if (that.Addresses == null)
						{
							that.IP;
						}
						for (i = 0; i < that.Addresses.Length; i++)
						{
							if (this.Addresses[0].Equals(that.Addresses[i]))
							{
								return true;
							}
						}
					}
					// since "this" was initialized with an IP address, we
					// don't check any other cases
					return false;
				}

				// check and see if we have any wildcards...
				if (this.Wildcard || that.Wildcard)
				{
					// if they are both wildcards, return true iff
					// that's cname ends with this cname (i.e., *.sun.com
					// implies *.eng.sun.com)
					if (this.Wildcard && that.Wildcard)
					{
						return (that.Cname.EndsWith(this.Cname));
					}

					// a non-wildcard can't imply a wildcard
					if (that.Wildcard)
					{
						return false;
					}

					// this is a wildcard, lets see if that's cname ends with
					// it...
					if (that.Cname == null)
					{
						that.CanonName;
					}
					return (that.Cname.EndsWith(this.Cname));
				}

				// comapare IP addresses
				if (this.Addresses == null)
				{
					this.IP;
				}

				if (that.Addresses == null)
				{
					that.IP;
				}

				if (!(that.Init_with_ip && this.Untrusted))
				{
					for (j = 0; j < this.Addresses.Length; j++)
					{
						for (i = 0; i < that.Addresses.Length; i++)
						{
							if (this.Addresses[j].Equals(that.Addresses[i]))
							{
								return true;
							}
						}
					}

					// XXX: if all else fails, compare hostnames?
					// Do we really want this?
					if (this.Cname == null)
					{
						this.CanonName;
					}

					if (that.Cname == null)
					{
						that.CanonName;
					}

					return (this.Cname.EqualsIgnoreCase(that.Cname));
				}

			}
			catch (UnknownHostException)
			{
				return CompareHostnames(that);
			}

			// make sure the first thing that is done here is to return
			// false. If not, uncomment the return false in the above catch.

			return false;
		}

		private bool CompareHostnames(SocketPermission that)
		{
			// we see if the original names/IPs passed in were equal.

			String thisHost = Hostname;
			String thatHost = that.Hostname;

			if (thisHost == null)
			{
				return false;
			}
			else if (this.Wildcard)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int cnameLength = this.cname.length();
				int cnameLength = this.Cname.Length();
				return thatHost.RegionMatches(true, (thatHost.Length() - cnameLength), this.Cname, 0, cnameLength);
			}
			else
			{
				return thisHost.EqualsIgnoreCase(thatHost);
			}
		}

		/// <summary>
		/// Checks two SocketPermission objects for equality.
		/// <P> </summary>
		/// <param name="obj"> the object to test for equality with this object.
		/// </param>
		/// <returns> true if <i>obj</i> is a SocketPermission, and has the
		///  same hostname, port range, and actions as this
		///  SocketPermission object. However, port range will be ignored
		///  in the comparison if <i>obj</i> only contains the action, 'resolve'. </returns>
		public override bool Equals(Object obj)
		{
			if (obj == this)
			{
				return true;
			}

			if (!(obj is SocketPermission))
			{
				return false;
			}

			SocketPermission that = (SocketPermission) obj;

			//this is (overly?) complex!!!

			// check the mask first
			if (this.Mask_Renamed != that.Mask_Renamed)
			{
				return false;
			}

			if ((that.Mask_Renamed & RESOLVE) != that.Mask_Renamed)
			{
				// now check the port range...
				if ((this.Portrange[0] != that.Portrange[0]) || (this.Portrange[1] != that.Portrange[1]))
				{
					return false;
				}
			}

			// short cut. This catches:
			//  "crypto" equal to "crypto", or
			// "1.2.3.4" equal to "1.2.3.4.", or
			//  "*.edu" equal to "*.edu", but it
			//  does not catch "crypto" equal to
			// "crypto.eng.sun.com".

			if (this.Name.EqualsIgnoreCase(that.Name))
			{
				return true;
			}

			// we now attempt to get the Canonical (FQDN) name and
			// compare that. If this fails, about all we can do is return
			// false.

			try
			{
				this.CanonName;
				that.CanonName;
			}
			catch (UnknownHostException)
			{
				return false;
			}

			if (this.Invalid || that.Invalid)
			{
				return false;
			}

			if (this.Cname != null)
			{
				return this.Cname.EqualsIgnoreCase(that.Cname);
			}

			return false;
		}

		/// <summary>
		/// Returns the hash code value for this object.
		/// </summary>
		/// <returns> a hash code value for this object. </returns>

		public override int HashCode()
		{
			/*
			 * If this SocketPermission was initialized with an IP address
			 * or a wildcard, use getName().hashCode(), otherwise use
			 * the hashCode() of the host name returned from
			 * java.net.InetAddress.getHostName method.
			 */

			if (Init_with_ip || Wildcard)
			{
				return this.Name.HashCode();
			}

			try
			{
				CanonName;
			}
			catch (UnknownHostException)
			{

			}

			if (Invalid || Cname == null)
			{
				return this.Name.HashCode();
			}
			else
			{
				return this.Cname.HashCode();
			}
		}

		/// <summary>
		/// Return the current action mask.
		/// </summary>
		/// <returns> the actions mask. </returns>

		internal int Mask
		{
			get
			{
				return Mask_Renamed;
			}
		}

		/// <summary>
		/// Returns the "canonical string representation" of the actions in the
		/// specified mask.
		/// Always returns present actions in the following order:
		/// connect, listen, accept, resolve.
		/// </summary>
		/// <param name="mask"> a specific integer action mask to translate into a string </param>
		/// <returns> the canonical string representation of the actions </returns>
		private static String GetActions(int mask)
		{
			StringBuilder sb = new StringBuilder();
			bool comma = false;

			if ((mask & CONNECT) == CONNECT)
			{
				comma = true;
				sb.Append("connect");
			}

			if ((mask & LISTEN) == LISTEN)
			{
				if (comma)
				{
					sb.Append(',');
				}
				else
				{
					comma = true;
				}
				sb.Append("listen");
			}

			if ((mask & ACCEPT) == ACCEPT)
			{
				if (comma)
				{
					sb.Append(',');
				}
				else
				{
					comma = true;
				}
				sb.Append("accept");
			}


			if ((mask & RESOLVE) == RESOLVE)
			{
				if (comma)
				{
					sb.Append(',');
				}
				else
				{
					comma = true;
				}
				sb.Append("resolve");
			}

			return sb.ToString();
		}

		/// <summary>
		/// Returns the canonical string representation of the actions.
		/// Always returns present actions in the following order:
		/// connect, listen, accept, resolve.
		/// </summary>
		/// <returns> the canonical string representation of the actions. </returns>
		public override String Actions
		{
			get
			{
				if (Actions_Renamed == null)
				{
					Actions_Renamed = GetActions(this.Mask_Renamed);
				}
    
				return Actions_Renamed;
			}
		}

		/// <summary>
		/// Returns a new PermissionCollection object for storing SocketPermission
		/// objects.
		/// <para>
		/// SocketPermission objects must be stored in a manner that allows them
		/// to be inserted into the collection in any order, but that also enables the
		/// PermissionCollection {@code implies}
		/// method to be implemented in an efficient (and consistent) manner.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a new PermissionCollection object suitable for storing SocketPermissions. </returns>

		public override PermissionCollection NewPermissionCollection()
		{
			return new SocketPermissionCollection();
		}

		/// <summary>
		/// WriteObject is called to save the state of the SocketPermission
		/// to a stream. The actions are serialized, and the superclass
		/// takes care of the name.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private synchronized void writeObject(java.io.ObjectOutputStream s) throws java.io.IOException
		private void WriteObject(ObjectOutputStream s)
		{
			lock (this)
			{
				// Write out the actions. The superclass takes care of the name
				// call getActions to make sure actions field is initialized
				if (Actions_Renamed == null)
				{
					Actions;
				}
				s.DefaultWriteObject();
			}
		}

		/// <summary>
		/// readObject is called to restore the state of the SocketPermission from
		/// a stream.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private synchronized void readObject(java.io.ObjectInputStream s) throws java.io.IOException, ClassNotFoundException
		private void ReadObject(ObjectInputStream s)
		{
			lock (this)
			{
				// Read in the action, then initialize the rest
				s.DefaultReadObject();
				Init(Name,GetMask(Actions_Renamed));
			}
		}

		/// <summary>
		/// Check the system/security property for the ephemeral port range
		/// for this system. The suffix is either "high" or "low"
		/// </summary>
		private static int InitEphemeralPorts(String suffix, int defval)
		{
			return AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper(suffix)
		   );
		}

		private class PrivilegedActionAnonymousInnerClassHelper : PrivilegedAction<Integer>
		{
			private string Suffix;

			public PrivilegedActionAnonymousInnerClassHelper(string suffix)
			{
				this.Suffix = suffix;
			}

			public virtual Integer Run()
			{
				int val = Integer.GetInteger("jdk.net.ephemeralPortRange." + Suffix, -1);
				if (val != -1)
				{
					return val;
				}
				else
				{
					return Suffix.Equals("low") ? PortConfig.Lower : PortConfig.Upper;
				}
			}
		}

		/// <summary>
		/// Check if the target range is within the policy range
		/// together with the ephemeral range for this platform
		/// (if policy includes ephemeral range)
		/// </summary>
		private static bool InRange(int policyLow, int policyHigh, int targetLow, int targetHigh)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int ephemeralLow = EphemeralRange.low;
			int ephemeralLow = EphemeralRange.Low;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int ephemeralHigh = EphemeralRange.high;
			int ephemeralHigh = EphemeralRange.High;

			if (targetLow == 0)
			{
				// check policy includes ephemeral range
				if (!InRange(policyLow, policyHigh, ephemeralLow, ephemeralHigh))
				{
					return false;
				}
				if (targetHigh == 0)
				{
					// nothing left to do
					return true;
				}
				// continue check with first real port number
				targetLow = 1;
			}

			if (policyLow == 0 && policyHigh == 0)
			{
				// ephemeral range only
				return targetLow >= ephemeralLow && targetHigh <= ephemeralHigh;
			}

			if (policyLow != 0)
			{
				// simple check of policy only
				return targetLow >= policyLow && targetHigh <= policyHigh;
			}

			// policyLow == 0 which means possibly two ranges to check

			// first check if policy and ephem range overlap/contiguous

			if (policyHigh >= ephemeralLow - 1)
			{
				return targetHigh <= ephemeralHigh;
			}

			// policy and ephem range do not overlap

			// target range must lie entirely inside policy range or eph range

			return (targetLow <= policyHigh && targetHigh <= policyHigh) || (targetLow >= ephemeralLow && targetHigh <= ephemeralHigh);
		}
		/*
		public String toString()
		{
		    StringBuffer s = new StringBuffer(super.toString() + "\n" +
		        "cname = " + cname + "\n" +
		        "wildcard = " + wildcard + "\n" +
		        "invalid = " + invalid + "\n" +
		        "portrange = " + portrange[0] + "," + portrange[1] + "\n");
		    if (addresses != null) for (int i=0; i<addresses.length; i++) {
		        s.append( addresses[i].getHostAddress());
		        s.append("\n");
		    } else {
		        s.append("(no addresses)\n");
		    }
	
		    return s.toString();
		}
	
		public static void main(String args[]) throws Exception {
		    SocketPermission this_ = new SocketPermission(args[0], "connect");
		    SocketPermission that_ = new SocketPermission(args[1], "connect");
		    System.out.println("-----\n");
		    System.out.println("this.implies(that) = " + this_.implies(that_));
		    System.out.println("-----\n");
		    System.out.println("this = "+this_);
		    System.out.println("-----\n");
		    System.out.println("that = "+that_);
		    System.out.println("-----\n");
	
		    SocketPermissionCollection nps = new SocketPermissionCollection();
		    nps.add(this_);
		    nps.add(new SocketPermission("www-leland.stanford.edu","connect"));
		    nps.add(new SocketPermission("www-sun.com","connect"));
		    System.out.println("nps.implies(that) = " + nps.implies(that_));
		    System.out.println("-----\n");
		}
		*/
	}

	/// 
	/// <summary>
	/// if (init'd with IP, key is IP as string)
	/// if wildcard, its the wild card
	/// else its the cname?
	/// 
	/// </summary>
	/// <seealso cref= java.security.Permission </seealso>
	/// <seealso cref= java.security.Permissions </seealso>
	/// <seealso cref= java.security.PermissionCollection
	/// 
	/// 
	/// @author Roland Schemers
	/// 
	/// @serial include </seealso>

	[Serializable]
	internal sealed class SocketPermissionCollection : PermissionCollection
	{
		// Not serialized; see serialization section at end of class
		[NonSerialized]
		private IList<SocketPermission> Perms;

		/// <summary>
		/// Create an empty SocketPermissions object.
		/// 
		/// </summary>

		public SocketPermissionCollection()
		{
			Perms = new List<SocketPermission>();
		}

		/// <summary>
		/// Adds a permission to the SocketPermissions. The key for the hash is
		/// the name in the case of wildcards, or all the IP addresses.
		/// </summary>
		/// <param name="permission"> the Permission object to add.
		/// </param>
		/// <exception cref="IllegalArgumentException"> - if the permission is not a
		///                                       SocketPermission
		/// </exception>
		/// <exception cref="SecurityException"> - if this SocketPermissionCollection object
		///                                has been marked readonly </exception>
		public override void Add(Permission permission)
		{
			if (!(permission is SocketPermission))
			{
				throw new IllegalArgumentException("invalid permission: " + permission);
			}
			if (ReadOnly)
			{
				throw new SecurityException("attempt to add a Permission to a readonly PermissionCollection");
			}

			// optimization to ensure perms most likely to be tested
			// show up early (4301064)
			lock (this)
			{
				Perms.Insert(0, (SocketPermission)permission);
			}
		}

		/// <summary>
		/// Check and see if this collection of permissions implies the permissions
		/// expressed in "permission".
		/// </summary>
		/// <param name="permission"> the Permission object to compare
		/// </param>
		/// <returns> true if "permission" is a proper subset of a permission in
		/// the collection, false if not. </returns>

		public override bool Implies(Permission permission)
		{
			if (!(permission is SocketPermission))
			{
					return false;
			}

			SocketPermission np = (SocketPermission) permission;

			int desired = np.Mask;
			int effective = 0;
			int needed = desired;

			lock (this)
			{
				int len = Perms.Count;
				//System.out.println("implies "+np);
				for (int i = 0; i < len; i++)
				{
					SocketPermission x = Perms[i];
					//System.out.println("  trying "+x);
					if (((needed & x.Mask) != 0) && x.ImpliesIgnoreMask(np))
					{
						effective |= x.Mask;
						if ((effective & desired) == desired)
						{
							return true;
						}
						needed = (desired ^ effective);
					}
				}
			}
			return false;
		}

		/// <summary>
		/// Returns an enumeration of all the SocketPermission objects in the
		/// container.
		/// </summary>
		/// <returns> an enumeration of all the SocketPermission objects. </returns>

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.Iterator<java.security.Permission> elements()
		public override IEnumerator<Permission> Elements()
		{
			// Convert Iterator into Enumeration
			lock (this)
			{
				return Collections.Enumeration((IList<Permission>)(IList)Perms);
			}
		}

		private const long SerialVersionUID = 2787186408602843674L;

		// Need to maintain serialization interoperability with earlier releases,
		// which had the serializable field:

		//
		// The SocketPermissions for this set.
		// @serial
		//
		// private Vector permissions;

		/// <summary>
		/// @serialField permissions java.util.Vector
		///     A list of the SocketPermissions for this set.
		/// </summary>
		private static readonly ObjectStreamField[] SerialPersistentFields = new ObjectStreamField[] {new ObjectStreamField("permissions", typeof(ArrayList))};

		/// <summary>
		/// @serialData "permissions" field (a Vector containing the SocketPermissions).
		/// </summary>
		/*
		 * Writes the contents of the perms field out as a Vector for
		 * serialization compatibility with earlier releases.
		 */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream out) throws java.io.IOException
		private void WriteObject(ObjectOutputStream @out)
		{
			// Don't call out.defaultWriteObject()

			// Write out Vector
			List<SocketPermission> permissions = new List<SocketPermission>(Perms.Count);

			lock (this)
			{
				permissions.AddRange(Perms);
			}

			ObjectOutputStream.PutField pfields = @out.PutFields();
			pfields.Put("permissions", permissions);
			@out.WriteFields();
		}

		/*
		 * Reads in a Vector of SocketPermissions and saves them in the perms field.
		 */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream in) throws java.io.IOException, ClassNotFoundException
		private void ReadObject(ObjectInputStream @in)
		{
			// Don't call in.defaultReadObject()

			// Read in serialized fields
			ObjectInputStream.GetField gfields = @in.ReadFields();

			// Get the one we want
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") java.util.Vector<SocketPermission> permissions = (java.util.Vector<SocketPermission>)gfields.get("permissions", null);
			List<SocketPermission> permissions = (List<SocketPermission>)gfields.Get("permissions", null);
			Perms = new List<SocketPermission>(permissions.Count);
			Perms.AddRange(permissions);
		}
	}

}