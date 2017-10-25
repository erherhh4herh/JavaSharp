using System;

/*
 * Copyright (c) 2013, Oracle and/or its affiliates. All rights reserved.
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

	/// <summary>
	/// Parses a string containing a host/domain name and port range
	/// </summary>
	internal class HostPortrange
	{

		internal String Hostname_Renamed;
		internal String Scheme;
		internal int[] Portrange_Renamed;

		internal bool Wildcard_Renamed;
		internal bool Literal_Renamed;
		internal bool Ipv6, Ipv4;
		internal const int PORT_MIN = 0;
		internal static readonly int PORT_MAX = (1 << 16) - 1;

		internal virtual bool Equals(HostPortrange that)
		{
			return this.Hostname_Renamed.Equals(that.Hostname_Renamed) && this.Portrange_Renamed[0] == that.Portrange_Renamed[0] && this.Portrange_Renamed[1] == that.Portrange_Renamed[1] && this.Wildcard_Renamed == that.Wildcard_Renamed && this.Literal_Renamed == that.Literal_Renamed;
		}

		public override int HashCode()
		{
			return Hostname_Renamed.HashCode() + Portrange_Renamed[0] + Portrange_Renamed[1];
		}

		internal HostPortrange(String scheme, String str)
		{
			// Parse the host name.  A name has up to three components, the
			// hostname, a port number, or two numbers representing a port
			// range.   "www.sun.com:8080-9090" is a valid host name.

			// With IPv6 an address can be 2010:836B:4179::836B:4179
			// An IPv6 address needs to be enclose in []
			// For ex: [2010:836B:4179::836B:4179]:8080-9090
			// Refer to RFC 2732 for more information.

			// first separate string into two fields: hoststr, portstr
			String hoststr , portstr = null;
			this.Scheme = scheme;

			// check for IPv6 address
			if (str.CharAt(0) == '[')
			{
				Ipv6 = Literal_Renamed = true;
				int rb = str.IndexOf(']');
				if (rb != -1)
				{
					hoststr = str.Substring(1, rb - 1);
				}
				else
				{
					throw new IllegalArgumentException("invalid IPv6 address: " + str);
				}
				int sep = str.IndexOf(':', rb + 1);
				if (sep != -1 && str.Length() > sep)
				{
					portstr = str.Substring(sep + 1);
				}
				// need to normalize hoststr now
				sbyte[] ip = IPAddressUtil.textToNumericFormatV6(hoststr);
				if (ip == null)
				{
					throw new IllegalArgumentException("illegal IPv6 address");
				}
				StringBuilder sb = new StringBuilder();
				Formatter formatter = new Formatter(sb, Locale.US);
				formatter.format("%02x%02x:%02x%02x:%02x%02x:%02x" + "%02x:%02x%02x:%02x%02x:%02x%02x:%02x%02x", ip[0], ip[1], ip[2], ip[3], ip[4], ip[5], ip[6], ip[7], ip[8], ip[9], ip[10], ip[11], ip[12], ip[13], ip[14], ip[15]);
				Hostname_Renamed = sb.ToString();
			}
			else
			{
				// not IPv6 therefore ':' is the port separator

				int sep = str.IndexOf(':');
				if (sep != -1 && str.Length() > sep)
				{
					hoststr = str.Substring(0, sep);
					portstr = str.Substring(sep + 1);
				}
				else
				{
					hoststr = sep == -1 ? str : str.Substring(0, sep);
				}
				// is this a domain wildcard specification?
				if (hoststr.LastIndexOf('*') > 0)
				{
					throw new IllegalArgumentException("invalid host wildcard specification");
				}
				else if (hoststr.StartsWith("*"))
				{
					Wildcard_Renamed = true;
					if (hoststr.Equals("*"))
					{
						hoststr = "";
					}
					else if (hoststr.StartsWith("*."))
					{
						hoststr = ToLowerCase(hoststr.Substring(1));
					}
					else
					{
						throw new IllegalArgumentException("invalid host wildcard specification");
					}
				}
				else
				{
					// check if ipv4 (if rightmost label a number)
					// The normal way to specify ipv4 is 4 decimal labels
					// but actually three, two or single label formats valid also
					// So, we recognise ipv4 by just testing the rightmost label
					// being a number.
					int lastdot = hoststr.LastIndexOf('.');
					if (lastdot != -1 && (hoststr.Length() > 1))
					{
						bool ipv4 = true;

						for (int i = lastdot + 1, len = hoststr.Length(); i < len; i++)
						{
							char c = hoststr.CharAt(i);
							if (c < '0' || c > '9')
							{
								ipv4 = false;
								break;
							}
						}
						this.Ipv4 = this.Literal_Renamed = ipv4;
						if (ipv4)
						{
							sbyte[] ip = IPAddressUtil.textToNumericFormatV4(hoststr);
							if (ip == null)
							{
								throw new IllegalArgumentException("illegal IPv4 address");
							}
							StringBuilder sb = new StringBuilder();
							Formatter formatter = new Formatter(sb, Locale.US);
							formatter.format("%d.%d.%d.%d", ip[0], ip[1], ip[2], ip[3]);
							hoststr = sb.ToString();
						}
						else
						{
							// regular domain name
							hoststr = ToLowerCase(hoststr);
						}
					}
				}
				Hostname_Renamed = hoststr;
			}

			try
			{
				Portrange_Renamed = ParsePort(portstr);
			}
			catch (Exception)
			{
				throw new IllegalArgumentException("invalid port range: " + portstr);
			}
		}

		internal static readonly int CASE_DIFF = 'A' - 'a';

		/// <summary>
		/// Convert to lower case, and check that all chars are ascii
		/// alphanumeric, '-' or '.' only.
		/// </summary>
		internal static String ToLowerCase(String s)
		{
			int len = s.Length();
			StringBuilder sb = null;

			for (int i = 0; i < len; i++)
			{
				char c = s.CharAt(i);
				if ((c >= 'a' && c <= 'z') || (c == '.'))
				{
					if (sb != null)
					{
						sb.Append(c);
					}
				}
				else if ((c >= '0' && c <= '9') || (c == '-'))
				{
					if (sb != null)
					{
						sb.Append(c);
					}
				}
				else if (c >= 'A' && c <= 'Z')
				{
					if (sb == null)
					{
						sb = new StringBuilder(len);
						sb.Append(s, 0, i);
					}
					sb.Append((char)(c - CASE_DIFF));
				}
				else
				{
					throw new IllegalArgumentException("Invalid characters in hostname");
				}
			}
			return sb == null ? s : sb.ToString();
		}


		public virtual bool Literal()
		{
			return Literal_Renamed;
		}

		public virtual bool Ipv4Literal()
		{
			return Ipv4;
		}

		public virtual bool Ipv6Literal()
		{
			return Ipv6;
		}

		public virtual String Hostname()
		{
			return Hostname_Renamed;
		}

		public virtual int[] Portrange()
		{
			return Portrange_Renamed;
		}

		/// <summary>
		/// returns true if the hostname part started with *
		/// hostname returns the remaining part of the host component
		/// eg "*.foo.com" -> ".foo.com" or "*" -> ""
		/// 
		/// @return
		/// </summary>
		public virtual bool Wildcard()
		{
			return Wildcard_Renamed;
		}

		// these shouldn't leak outside the implementation
		internal static readonly int[] HTTP_PORT = new int[] {80, 80};
		internal static readonly int[] HTTPS_PORT = new int[] {443, 443};
		internal static readonly int[] NO_PORT = new int[] {-1, -1};

		internal virtual int[] DefaultPort()
		{
			if (Scheme.Equals("http"))
			{
				return HTTP_PORT;
			}
			else if (Scheme.Equals("https"))
			{
				return HTTPS_PORT;
			}
			return NO_PORT;
		}

		internal virtual int[] ParsePort(String port)
		{

			if (port == null || port.Equals(""))
			{
				return DefaultPort();
			}

			if (port.Equals("*"))
			{
				return new int[] {PORT_MIN, PORT_MAX};
			}

			try
			{
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
						return DefaultPort();
					}
					return new int[] {l, h};
				}
			}
			catch (IllegalArgumentException)
			{
				return DefaultPort();
			}
		}
	}

}