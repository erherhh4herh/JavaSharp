using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading;
using System.Runtime.InteropServices;

/*
 * Copyright (c) 1995, 2015, Oracle and/or its affiliates. All rights reserved.
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

	using sun.security.action;
	using InetAddressCachePolicy = sun.net.InetAddressCachePolicy;
	using IPAddressUtil = sun.net.util.IPAddressUtil;
	using sun.net.spi.nameservice;

	/// <summary>
	/// This class represents an Internet Protocol (IP) address.
	/// 
	/// <para> An IP address is either a 32-bit or 128-bit unsigned number
	/// used by IP, a lower-level protocol on which protocols like UDP and
	/// TCP are built. The IP address architecture is defined by <a
	/// href="http://www.ietf.org/rfc/rfc790.txt"><i>RFC&nbsp;790:
	/// Assigned Numbers</i></a>, <a
	/// href="http://www.ietf.org/rfc/rfc1918.txt"> <i>RFC&nbsp;1918:
	/// Address Allocation for Private Internets</i></a>, <a
	/// href="http://www.ietf.org/rfc/rfc2365.txt"><i>RFC&nbsp;2365:
	/// Administratively Scoped IP Multicast</i></a>, and <a
	/// href="http://www.ietf.org/rfc/rfc2373.txt"><i>RFC&nbsp;2373: IP
	/// Version 6 Addressing Architecture</i></a>. An instance of an
	/// InetAddress consists of an IP address and possibly its
	/// corresponding host name (depending on whether it is constructed
	/// with a host name or whether it has already done reverse host name
	/// resolution).
	/// 
	/// <h3> Address types </h3>
	/// 
	/// <blockquote><table cellspacing=2 summary="Description of unicast and multicast address types">
	///   <tr><th valign=top><i>unicast</i></th>
	///       <td>An identifier for a single interface. A packet sent to
	///         a unicast address is delivered to the interface identified by
	///         that address.
	/// 
	/// </para>
	///         <para> The Unspecified Address -- Also called anylocal or wildcard
	///         address. It must never be assigned to any node. It indicates the
	///         absence of an address. One example of its use is as the target of
	///         bind, which allows a server to accept a client connection on any
	///         interface, in case the server host has multiple interfaces.
	/// 
	/// </para>
	///         <para> The <i>unspecified</i> address must not be used as
	///         the destination address of an IP packet.
	/// 
	/// </para>
	///         <para> The <i>Loopback</i> Addresses -- This is the address
	///         assigned to the loopback interface. Anything sent to this
	///         IP address loops around and becomes IP input on the local
	///         host. This address is often used when testing a
	///         client.</td></tr>
	///   <tr><th valign=top><i>multicast</i></th>
	///       <td>An identifier for a set of interfaces (typically belonging
	///         to different nodes). A packet sent to a multicast address is
	///         delivered to all interfaces identified by that address.</td></tr>
	/// </table></blockquote>
	/// 
	/// <h4> IP address scope </h4>
	/// 
	/// </para>
	/// <para> <i>Link-local</i> addresses are designed to be used for addressing
	/// on a single link for purposes such as auto-address configuration,
	/// neighbor discovery, or when no routers are present.
	/// 
	/// </para>
	/// <para> <i>Site-local</i> addresses are designed to be used for addressing
	/// inside of a site without the need for a global prefix.
	/// 
	/// </para>
	/// <para> <i>Global</i> addresses are unique across the internet.
	/// 
	/// <h4> Textual representation of IP addresses </h4>
	/// 
	/// The textual representation of an IP address is address family specific.
	/// 
	/// </para>
	/// <para>
	/// 
	/// For IPv4 address format, please refer to <A
	/// HREF="Inet4Address.html#format">Inet4Address#format</A>; For IPv6
	/// address format, please refer to <A
	/// HREF="Inet6Address.html#format">Inet6Address#format</A>.
	/// 
	/// <P>There is a <a href="doc-files/net-properties.html#Ipv4IPv6">couple of
	/// System Properties</a> affecting how IPv4 and IPv6 addresses are used.</P>
	/// 
	/// <h4> Host Name Resolution </h4>
	/// 
	/// Host name-to-IP address <i>resolution</i> is accomplished through
	/// the use of a combination of local machine configuration information
	/// and network naming services such as the Domain Name System (DNS)
	/// and Network Information Service(NIS). The particular naming
	/// services(s) being used is by default the local machine configured
	/// one. For any host name, its corresponding IP address is returned.
	/// 
	/// </para>
	/// <para> <i>Reverse name resolution</i> means that for any IP address,
	/// the host associated with the IP address is returned.
	/// 
	/// </para>
	/// <para> The InetAddress class provides methods to resolve host names to
	/// their IP addresses and vice versa.
	/// 
	/// <h4> InetAddress Caching </h4>
	/// 
	/// The InetAddress class has a cache to store successful as well as
	/// unsuccessful host name resolutions.
	/// 
	/// </para>
	/// <para> By default, when a security manager is installed, in order to
	/// protect against DNS spoofing attacks,
	/// the result of positive host name resolutions are
	/// cached forever. When a security manager is not installed, the default
	/// behavior is to cache entries for a finite (implementation dependent)
	/// period of time. The result of unsuccessful host
	/// name resolution is cached for a very short period of time (10
	/// seconds) to improve performance.
	/// 
	/// </para>
	/// <para> If the default behavior is not desired, then a Java security property
	/// can be set to a different Time-to-live (TTL) value for positive
	/// caching. Likewise, a system admin can configure a different
	/// negative caching TTL value when needed.
	/// 
	/// </para>
	/// <para> Two Java security properties control the TTL values used for
	///  positive and negative host name resolution caching:
	/// 
	/// <blockquote>
	/// <dl>
	/// <dt><b>networkaddress.cache.ttl</b></dt>
	/// <dd>Indicates the caching policy for successful name lookups from
	/// the name service. The value is specified as as integer to indicate
	/// the number of seconds to cache the successful lookup. The default
	/// setting is to cache for an implementation specific period of time.
	/// </para>
	/// <para>
	/// A value of -1 indicates "cache forever".
	/// </dd>
	/// <dt><b>networkaddress.cache.negative.ttl</b> (default: 10)</dt>
	/// <dd>Indicates the caching policy for un-successful name lookups
	/// from the name service. The value is specified as as integer to
	/// indicate the number of seconds to cache the failure for
	/// un-successful lookups.
	/// </para>
	/// <para>
	/// A value of 0 indicates "never cache".
	/// A value of -1 indicates "cache forever".
	/// </dd>
	/// </dl>
	/// </blockquote>
	/// 
	/// @author  Chris Warth
	/// </para>
	/// </summary>
	/// <seealso cref=     java.net.InetAddress#getByAddress(byte[]) </seealso>
	/// <seealso cref=     java.net.InetAddress#getByAddress(java.lang.String, byte[]) </seealso>
	/// <seealso cref=     java.net.InetAddress#getAllByName(java.lang.String) </seealso>
	/// <seealso cref=     java.net.InetAddress#getByName(java.lang.String) </seealso>
	/// <seealso cref=     java.net.InetAddress#getLocalHost()
	/// @since JDK1.0 </seealso>
	[Serializable]
	public class InetAddress
	{
		/// <summary>
		/// Specify the address family: Internet Protocol, Version 4
		/// @since 1.4
		/// </summary>
		internal const int IPv4 = 1;

		/// <summary>
		/// Specify the address family: Internet Protocol, Version 6
		/// @since 1.4
		/// </summary>
		internal const int IPv6 = 2;

		/* Specify address family preference */
		[NonSerialized]
		internal static bool PreferIPv6Address = false;

		internal class InetAddressHolder
		{
			/// <summary>
			/// Reserve the original application specified hostname.
			/// 
			/// The original hostname is useful for domain-based endpoint
			/// identification (see RFC 2818 and RFC 6125).  If an address
			/// was created with a raw IP address, a reverse name lookup
			/// may introduce endpoint identification security issue via
			/// DNS forging.
			/// 
			/// Oracle JSSE provider is using this original hostname, via
			/// sun.misc.JavaNetAccess, for SSL/TLS endpoint identification.
			/// 
			/// Note: May define a new public method in the future if necessary.
			/// </summary>
			internal String OriginalHostName_Renamed;

			internal InetAddressHolder()
			{
			}

			internal InetAddressHolder(String hostName, int address, int family)
			{
				this.OriginalHostName_Renamed = hostName;
				this.HostName_Renamed = hostName;
				this.Address_Renamed = address;
				this.Family_Renamed = family;
			}

			internal virtual void Init(String hostName, int family)
			{
				this.OriginalHostName_Renamed = hostName;
				this.HostName_Renamed = hostName;
				if (family != -1)
				{
					this.Family_Renamed = family;
				}
			}

			internal String HostName_Renamed;

			internal virtual String HostName
			{
				get
				{
					return HostName_Renamed;
				}
			}

			internal virtual String OriginalHostName
			{
				get
				{
					return OriginalHostName_Renamed;
				}
			}

			/// <summary>
			/// Holds a 32-bit IPv4 address.
			/// </summary>
			internal int Address_Renamed;

			internal virtual int Address
			{
				get
				{
					return Address_Renamed;
				}
			}

			/// <summary>
			/// Specifies the address family type, for instance, '1' for IPv4
			/// addresses, and '2' for IPv6 addresses.
			/// </summary>
			internal int Family_Renamed;

			internal virtual int Family
			{
				get
				{
					return Family_Renamed;
				}
			}
		}

		/* Used to store the serializable fields of InetAddress */
		[NonSerialized]
		internal readonly InetAddressHolder Holder_Renamed;

		internal virtual InetAddressHolder Holder()
		{
			return Holder_Renamed;
		}

		/* Used to store the name service provider */
		private static IList<NameService> NameServices = null;

		/* Used to store the best available hostname */
		[NonSerialized]
		private String CanonicalHostName_Renamed = null;

		/// <summary>
		/// use serialVersionUID from JDK 1.0.2 for interoperability </summary>
		private const long SerialVersionUID = 3286316764910316507L;

		/*
		 * Load net library into runtime, and perform initializations.
		 */
		static InetAddress()
		{
			PreferIPv6Address = (bool)AccessController.doPrivileged(new GetBooleanAction("java.net.preferIPv6Addresses"));
			AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper());
			init();
			// create the impl
			Impl = InetAddressImplFactory.Create();

			// get name service if provided and requested
			String provider = null;
			String propPrefix = "sun.net.spi.nameservice.provider.";
			int n = 1;
			NameServices = new List<NameService>();
			provider = AccessController.doPrivileged(new GetPropertyAction(propPrefix + n));
			while (provider != null)
			{
				NameService ns = CreateNSProvider(provider);
				if (ns != null)
				{
					NameServices.Add(ns);
				}

				n++;
				provider = AccessController.doPrivileged(new GetPropertyAction(propPrefix + n));
			}

			// if not designate any name services provider,
			// create a default one
			if (NameServices.Count == 0)
			{
				NameService ns = CreateNSProvider("default");
				NameServices.Add(ns);
			}
			try
			{
				sun.misc.Unsafe @unsafe = sun.misc.Unsafe.Unsafe;
				FIELDS_OFFSET = @unsafe.objectFieldOffset(typeof(InetAddress).getDeclaredField("holder"));
				UNSAFE = @unsafe;
			}
			catch (ReflectiveOperationException e)
			{
				throw new Error(e);
			}
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
		/// Constructor for the Socket.accept() method.
		/// This creates an empty InetAddress, which is filled in by
		/// the accept() method.  This InetAddress, however, is not
		/// put in the address cache, since it is not created by name.
		/// </summary>
		internal InetAddress()
		{
			Holder_Renamed = new InetAddressHolder();
		}

		/// <summary>
		/// Replaces the de-serialized object with an Inet4Address object.
		/// </summary>
		/// <returns> the alternate object to the de-serialized object.
		/// </returns>
		/// <exception cref="ObjectStreamException"> if a new object replacing this
		/// object could not be created </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private Object readResolve() throws java.io.ObjectStreamException
		private Object ReadResolve()
		{
			// will replace the deserialized 'this' object
			return new Inet4Address(Holder().HostName, Holder().Address);
		}

		/// <summary>
		/// Utility routine to check if the InetAddress is an
		/// IP multicast address. </summary>
		/// <returns> a {@code boolean} indicating if the InetAddress is
		/// an IP multicast address
		/// @since   JDK1.1 </returns>
		public virtual bool MulticastAddress
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Utility routine to check if the InetAddress in a wildcard address. </summary>
		/// <returns> a {@code boolean} indicating if the Inetaddress is
		///         a wildcard address.
		/// @since 1.4 </returns>
		public virtual bool AnyLocalAddress
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Utility routine to check if the InetAddress is a loopback address.
		/// </summary>
		/// <returns> a {@code boolean} indicating if the InetAddress is
		/// a loopback address; or false otherwise.
		/// @since 1.4 </returns>
		public virtual bool LoopbackAddress
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Utility routine to check if the InetAddress is an link local address.
		/// </summary>
		/// <returns> a {@code boolean} indicating if the InetAddress is
		/// a link local address; or false if address is not a link local unicast address.
		/// @since 1.4 </returns>
		public virtual bool LinkLocalAddress
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Utility routine to check if the InetAddress is a site local address.
		/// </summary>
		/// <returns> a {@code boolean} indicating if the InetAddress is
		/// a site local address; or false if address is not a site local unicast address.
		/// @since 1.4 </returns>
		public virtual bool SiteLocalAddress
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Utility routine to check if the multicast address has global scope.
		/// </summary>
		/// <returns> a {@code boolean} indicating if the address has
		///         is a multicast address of global scope, false if it is not
		///         of global scope or it is not a multicast address
		/// @since 1.4 </returns>
		public virtual bool MCGlobal
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Utility routine to check if the multicast address has node scope.
		/// </summary>
		/// <returns> a {@code boolean} indicating if the address has
		///         is a multicast address of node-local scope, false if it is not
		///         of node-local scope or it is not a multicast address
		/// @since 1.4 </returns>
		public virtual bool MCNodeLocal
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Utility routine to check if the multicast address has link scope.
		/// </summary>
		/// <returns> a {@code boolean} indicating if the address has
		///         is a multicast address of link-local scope, false if it is not
		///         of link-local scope or it is not a multicast address
		/// @since 1.4 </returns>
		public virtual bool MCLinkLocal
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Utility routine to check if the multicast address has site scope.
		/// </summary>
		/// <returns> a {@code boolean} indicating if the address has
		///         is a multicast address of site-local scope, false if it is not
		///         of site-local scope or it is not a multicast address
		/// @since 1.4 </returns>
		public virtual bool MCSiteLocal
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Utility routine to check if the multicast address has organization scope.
		/// </summary>
		/// <returns> a {@code boolean} indicating if the address has
		///         is a multicast address of organization-local scope,
		///         false if it is not of organization-local scope
		///         or it is not a multicast address
		/// @since 1.4 </returns>
		public virtual bool MCOrgLocal
		{
			get
			{
				return false;
			}
		}


		/// <summary>
		/// Test whether that address is reachable. Best effort is made by the
		/// implementation to try to reach the host, but firewalls and server
		/// configuration may block requests resulting in a unreachable status
		/// while some specific ports may be accessible.
		/// A typical implementation will use ICMP ECHO REQUESTs if the
		/// privilege can be obtained, otherwise it will try to establish
		/// a TCP connection on port 7 (Echo) of the destination host.
		/// <para>
		/// The timeout value, in milliseconds, indicates the maximum amount of time
		/// the try should take. If the operation times out before getting an
		/// answer, the host is deemed unreachable. A negative value will result
		/// in an IllegalArgumentException being thrown.
		/// 
		/// </para>
		/// </summary>
		/// <param name="timeout"> the time, in milliseconds, before the call aborts </param>
		/// <returns> a {@code boolean} indicating if the address is reachable. </returns>
		/// <exception cref="IOException"> if a network error occurs </exception>
		/// <exception cref="IllegalArgumentException"> if {@code timeout} is negative.
		/// @since 1.5 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean isReachable(int timeout) throws java.io.IOException
		public virtual bool IsReachable(int timeout)
		{
			return IsReachable(null, 0, timeout);
		}

		/// <summary>
		/// Test whether that address is reachable. Best effort is made by the
		/// implementation to try to reach the host, but firewalls and server
		/// configuration may block requests resulting in a unreachable status
		/// while some specific ports may be accessible.
		/// A typical implementation will use ICMP ECHO REQUESTs if the
		/// privilege can be obtained, otherwise it will try to establish
		/// a TCP connection on port 7 (Echo) of the destination host.
		/// <para>
		/// The {@code network interface} and {@code ttl} parameters
		/// let the caller specify which network interface the test will go through
		/// and the maximum number of hops the packets should go through.
		/// A negative value for the {@code ttl} will result in an
		/// IllegalArgumentException being thrown.
		/// </para>
		/// <para>
		/// The timeout value, in milliseconds, indicates the maximum amount of time
		/// the try should take. If the operation times out before getting an
		/// answer, the host is deemed unreachable. A negative value will result
		/// in an IllegalArgumentException being thrown.
		/// 
		/// </para>
		/// </summary>
		/// <param name="netif">   the NetworkInterface through which the
		///                    test will be done, or null for any interface </param>
		/// <param name="ttl">     the maximum numbers of hops to try or 0 for the
		///                  default </param>
		/// <param name="timeout"> the time, in milliseconds, before the call aborts </param>
		/// <exception cref="IllegalArgumentException"> if either {@code timeout}
		///                          or {@code ttl} are negative. </exception>
		/// <returns> a {@code boolean}indicating if the address is reachable. </returns>
		/// <exception cref="IOException"> if a network error occurs
		/// @since 1.5 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean isReachable(NetworkInterface netif, int ttl, int timeout) throws java.io.IOException
		public virtual bool IsReachable(NetworkInterface netif, int ttl, int timeout)
		{
			if (ttl < 0)
			{
				throw new IllegalArgumentException("ttl can't be negative");
			}
			if (timeout < 0)
			{
				throw new IllegalArgumentException("timeout can't be negative");
			}

			return Impl.IsReachable(this, timeout, netif, ttl);
		}

		/// <summary>
		/// Gets the host name for this IP address.
		/// 
		/// <para>If this InetAddress was created with a host name,
		/// this host name will be remembered and returned;
		/// otherwise, a reverse name lookup will be performed
		/// and the result will be returned based on the system
		/// configured name lookup service. If a lookup of the name service
		/// is required, call
		/// <seealso cref="#getCanonicalHostName() getCanonicalHostName"/>.
		/// 
		/// </para>
		/// <para>If there is a security manager, its
		/// {@code checkConnect} method is first called
		/// with the hostname and {@code -1}
		/// as its arguments to see if the operation is allowed.
		/// If the operation is not allowed, it will return
		/// the textual representation of the IP address.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  the host name for this IP address, or if the operation
		///    is not allowed by the security check, the textual
		///    representation of the IP address.
		/// </returns>
		/// <seealso cref= InetAddress#getCanonicalHostName </seealso>
		/// <seealso cref= SecurityManager#checkConnect </seealso>
		public virtual String HostName
		{
			get
			{
				return GetHostName(true);
			}
		}

		/// <summary>
		/// Returns the hostname for this address.
		/// If the host is equal to null, then this address refers to any
		/// of the local machine's available network addresses.
		/// this is package private so SocketPermission can make calls into
		/// here without a security check.
		/// 
		/// <para>If there is a security manager, this method first
		/// calls its {@code checkConnect} method
		/// with the hostname and {@code -1}
		/// as its arguments to see if the calling code is allowed to know
		/// the hostname for this IP address, i.e., to connect to the host.
		/// If the operation is not allowed, it will return
		/// the textual representation of the IP address.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  the host name for this IP address, or if the operation
		///    is not allowed by the security check, the textual
		///    representation of the IP address.
		/// </returns>
		/// <param name="check"> make security check if true
		/// </param>
		/// <seealso cref= SecurityManager#checkConnect </seealso>
		internal virtual String GetHostName(bool check)
		{
			if (Holder().HostName == null)
			{
				Holder().HostName_Renamed = InetAddress.GetHostFromNameService(this, check);
			}
			return Holder().HostName;
		}

		/// <summary>
		/// Gets the fully qualified domain name for this IP address.
		/// Best effort method, meaning we may not be able to return
		/// the FQDN depending on the underlying system configuration.
		/// 
		/// <para>If there is a security manager, this method first
		/// calls its {@code checkConnect} method
		/// with the hostname and {@code -1}
		/// as its arguments to see if the calling code is allowed to know
		/// the hostname for this IP address, i.e., to connect to the host.
		/// If the operation is not allowed, it will return
		/// the textual representation of the IP address.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  the fully qualified domain name for this IP address,
		///    or if the operation is not allowed by the security check,
		///    the textual representation of the IP address.
		/// </returns>
		/// <seealso cref= SecurityManager#checkConnect
		/// 
		/// @since 1.4 </seealso>
		public virtual String CanonicalHostName
		{
			get
			{
				if (CanonicalHostName_Renamed == null)
				{
					CanonicalHostName_Renamed = InetAddress.GetHostFromNameService(this, true);
				}
				return CanonicalHostName_Renamed;
			}
		}

		/// <summary>
		/// Returns the hostname for this address.
		/// 
		/// <para>If there is a security manager, this method first
		/// calls its {@code checkConnect} method
		/// with the hostname and {@code -1}
		/// as its arguments to see if the calling code is allowed to know
		/// the hostname for this IP address, i.e., to connect to the host.
		/// If the operation is not allowed, it will return
		/// the textual representation of the IP address.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  the host name for this IP address, or if the operation
		///    is not allowed by the security check, the textual
		///    representation of the IP address.
		/// </returns>
		/// <param name="check"> make security check if true
		/// </param>
		/// <seealso cref= SecurityManager#checkConnect </seealso>
		private static String GetHostFromNameService(InetAddress addr, bool check)
		{
			String host = null;
			foreach (NameService nameService in NameServices)
			{
				try
				{
					// first lookup the hostname
					host = nameService.getHostByAddr(addr.Address);

					/* check to see if calling code is allowed to know
					 * the hostname for this IP address, ie, connect to the host
					 */
					if (check)
					{
						SecurityManager sec = System.SecurityManager;
						if (sec != null)
						{
							sec.CheckConnect(host, -1);
						}
					}

					/* now get all the IP addresses for this hostname,
					 * and make sure one of them matches the original IP
					 * address. We do this to try and prevent spoofing.
					 */

					InetAddress[] arr = InetAddress.GetAllByName0(host, check);
					bool ok = false;

					if (arr != null)
					{
						for (int i = 0; !ok && i < arr.Length; i++)
						{
							ok = addr.Equals(arr[i]);
						}
					}

					//XXX: if it looks a spoof just return the address?
					if (!ok)
					{
						host = addr.HostAddress;
						return host;
					}

					break;

				}
				catch (SecurityException)
				{
					host = addr.HostAddress;
					break;
				}
				catch (UnknownHostException)
				{
					host = addr.HostAddress;
					// let next provider resolve the hostname
				}
			}

			return host;
		}

		/// <summary>
		/// Returns the raw IP address of this {@code InetAddress}
		/// object. The result is in network byte order: the highest order
		/// byte of the address is in {@code getAddress()[0]}.
		/// </summary>
		/// <returns>  the raw IP address of this object. </returns>
		public virtual sbyte[] Address
		{
			get
			{
				return null;
			}
		}

		/// <summary>
		/// Returns the IP address string in textual presentation.
		/// </summary>
		/// <returns>  the raw IP address in a string format.
		/// @since   JDK1.0.2 </returns>
		public virtual String HostAddress
		{
			get
			{
				return null;
			}
		}

		/// <summary>
		/// Returns a hashcode for this IP address.
		/// </summary>
		/// <returns>  a hash code value for this IP address. </returns>
		public override int HashCode()
		{
			return -1;
		}

		/// <summary>
		/// Compares this object against the specified object.
		/// The result is {@code true} if and only if the argument is
		/// not {@code null} and it represents the same IP address as
		/// this object.
		/// <para>
		/// Two instances of {@code InetAddress} represent the same IP
		/// address if the length of the byte arrays returned by
		/// {@code getAddress} is the same for both, and each of the
		/// array components is the same for the byte arrays.
		/// 
		/// </para>
		/// </summary>
		/// <param name="obj">   the object to compare against. </param>
		/// <returns>  {@code true} if the objects are the same;
		///          {@code false} otherwise. </returns>
		/// <seealso cref=     java.net.InetAddress#getAddress() </seealso>
		public override bool Equals(Object obj)
		{
			return false;
		}

		/// <summary>
		/// Converts this IP address to a {@code String}. The
		/// string returned is of the form: hostname / literal IP
		/// address.
		/// 
		/// If the host name is unresolved, no reverse name service lookup
		/// is performed. The hostname part will be represented by an empty string.
		/// </summary>
		/// <returns>  a string representation of this IP address. </returns>
		public override String ToString()
		{
			String hostName = Holder().HostName;
			return ((hostName != null) ? hostName : "") + "/" + HostAddress;
		}

		/*
		 * Cached addresses - our own litle nis, not!
		 */
		private static Cache AddressCache = new Cache(Cache.Type.Positive);

		private static Cache NegativeCache = new Cache(Cache.Type.Negative);

		private static bool AddressCacheInit = false;

		internal static InetAddress[] Unknown_array; // put THIS in cache

		internal static InetAddressImpl Impl;

		private static readonly Dictionary<String, Void> LookupTable = new Dictionary<String, Void>();

		/// <summary>
		/// Represents a cache entry
		/// </summary>
		internal sealed class CacheEntry
		{

			internal CacheEntry(InetAddress[] addresses, long expiration)
			{
				this.Addresses = addresses;
				this.Expiration = expiration;
			}

			internal InetAddress[] Addresses;
			internal long Expiration;
		}

		/// <summary>
		/// A cache that manages entries based on a policy specified
		/// at creation time.
		/// </summary>
		internal sealed class Cache
		{
			internal LinkedHashMap<String, CacheEntry> Cache_Renamed;
			internal Type Type;

			internal enum Type
			{
				Positive,
				Negative
			}

			/// <summary>
			/// Create cache
			/// </summary>
			public Cache(Type type)
			{
				this.Type = type;
				Cache_Renamed = new LinkedHashMap<String, CacheEntry>();
			}

			internal int Policy
			{
				get
				{
					if (Type == Type.Positive)
					{
						return InetAddressCachePolicy.get();
					}
					else
					{
						return InetAddressCachePolicy.Negative;
					}
				}
			}

			/// <summary>
			/// Add an entry to the cache. If there's already an
			/// entry then for this host then the entry will be
			/// replaced.
			/// </summary>
			public Cache Put(String host, InetAddress[] addresses)
			{
				int policy = Policy;
				if (policy == InetAddressCachePolicy.NEVER)
				{
					return this;
				}

				// purge any expired entries

				if (policy != InetAddressCachePolicy.FOREVER)
				{

					// As we iterate in insertion order we can
					// terminate when a non-expired entry is found.
					LinkedList<String> expired = new LinkedList<String>();
					long now = DateTimeHelperClass.CurrentUnixTimeMillis();
					foreach (String key in Cache_Renamed.KeySet())
					{
						CacheEntry entry = Cache_Renamed[key];

						if (entry.Expiration >= 0 && entry.Expiration < now)
						{
							expired.AddLast(key);
						}
						else
						{
							break;
						}
					}

					foreach (String key in expired)
					{
						Cache_Renamed.Remove(key);
					}
				}

				// create new entry and add it to the cache
				// -- as a HashMap replaces existing entries we
				//    don't need to explicitly check if there is
				//    already an entry for this host.
				long expiration;
				if (policy == InetAddressCachePolicy.FOREVER)
				{
					expiration = -1;
				}
				else
				{
					expiration = DateTimeHelperClass.CurrentUnixTimeMillis() + (policy * 1000);
				}
				CacheEntry entry = new CacheEntry(addresses, expiration);
				Cache_Renamed[host] = entry;
				return this;
			}

			/// <summary>
			/// Query the cache for the specific host. If found then
			/// return its CacheEntry, or null if not found.
			/// </summary>
			public CacheEntry Get(String host)
			{
				int policy = Policy;
				if (policy == InetAddressCachePolicy.NEVER)
				{
					return null;
				}
				CacheEntry entry = Cache_Renamed[host];

				// check if entry has expired
				if (entry != null && policy != InetAddressCachePolicy.FOREVER)
				{
					if (entry.Expiration >= 0 && entry.Expiration < DateTimeHelperClass.CurrentUnixTimeMillis())
					{
						Cache_Renamed.Remove(host);
						entry = null;
					}
				}

				return entry;
			}
		}

		/*
		 * Initialize cache and insert anyLocalAddress into the
		 * unknown array with no expiry.
		 */
		private static void CacheInitIfNeeded()
		{
			Debug.Assert(Thread.holdsLock(AddressCache));
			if (AddressCacheInit)
			{
				return;
			}
			Unknown_array = new InetAddress[1];
			Unknown_array[0] = Impl.AnyLocalAddress();

			AddressCache.Put(Impl.AnyLocalAddress().HostName, Unknown_array);

			AddressCacheInit = true;
		}

		/*
		 * Cache the given hostname and addresses.
		 */
		private static void CacheAddresses(String hostname, InetAddress[] addresses, bool success)
		{
			hostname = hostname.ToLowerCase();
			lock (AddressCache)
			{
				CacheInitIfNeeded();
				if (success)
				{
					AddressCache.Put(hostname, addresses);
				}
				else
				{
					NegativeCache.Put(hostname, addresses);
				}
			}
		}

		/*
		 * Lookup hostname in cache (positive & negative cache). If
		 * found return addresses, null if not found.
		 */
		private static InetAddress[] GetCachedAddresses(String hostname)
		{
			hostname = hostname.ToLowerCase();

			// search both positive & negative caches

			lock (AddressCache)
			{
				CacheInitIfNeeded();

				CacheEntry entry = AddressCache.Get(hostname);
				if (entry == null)
				{
					entry = NegativeCache.Get(hostname);
				}

				if (entry != null)
				{
					return entry.Addresses;
				}
			}

			// not found
			return null;
		}

		private static NameService CreateNSProvider(String provider)
		{
			if (provider == null)
			{
				return null;
			}

			NameService nameService = null;
			if (provider.Equals("default"))
			{
				// initialize the default name service
				nameService = new NameServiceAnonymousInnerClassHelper();
			}
			else
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String providerName = provider;
				String providerName = provider;
				try
				{
					nameService = AccessController.doPrivileged(new PrivilegedExceptionActionAnonymousInnerClassHelper(providerName)
				   );
				}
				catch (java.security.PrivilegedActionException)
				{
				}
			}

			return nameService;
		}

		private class NameServiceAnonymousInnerClassHelper : NameService
		{
			public NameServiceAnonymousInnerClassHelper()
			{
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public InetAddress[] lookupAllHostAddr(String host) throws UnknownHostException
			public virtual InetAddress[] LookupAllHostAddr(String host)
			{
				return Impl.LookupAllHostAddr(host);
			}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getHostByAddr(byte[] addr) throws UnknownHostException
			public virtual String GetHostByAddr(sbyte[] addr)
			{
				return Impl.GetHostByAddr(addr);
			}
		}

		private class PrivilegedExceptionActionAnonymousInnerClassHelper : java.security.PrivilegedExceptionAction<NameService>
		{
			private string ProviderName;

			public PrivilegedExceptionActionAnonymousInnerClassHelper(string providerName)
			{
				this.ProviderName = providerName;
			}

			public virtual NameService Run()
			{
				IEnumerator<NameServiceDescriptor> itr = ServiceLoader.Load(typeof(NameServiceDescriptor)).Iterator();
				while (itr.MoveNext())
				{
					NameServiceDescriptor nsd = itr.Current;
					if (ProviderName.EqualsIgnoreCase(nsd.Type + "," + nsd.ProviderName))
					{
						try
						{
							return nsd.createNameService();
						}
						catch (Exception e)
						{
							e.PrintStackTrace();
							System.Console.Error.WriteLine("Cannot create name service:" + ProviderName + ": " + e);
						}
					}
				}

				return null;
			}
		}


		/// <summary>
		/// Creates an InetAddress based on the provided host name and IP address.
		/// No name service is checked for the validity of the address.
		/// 
		/// <para> The host name can either be a machine name, such as
		/// "{@code java.sun.com}", or a textual representation of its IP
		/// address.
		/// </para>
		/// <para> No validity checking is done on the host name either.
		/// 
		/// </para>
		/// <para> If addr specifies an IPv4 address an instance of Inet4Address
		/// will be returned; otherwise, an instance of Inet6Address
		/// will be returned.
		/// 
		/// </para>
		/// <para> IPv4 address byte array must be 4 bytes long and IPv6 byte array
		/// must be 16 bytes long
		/// 
		/// </para>
		/// </summary>
		/// <param name="host"> the specified host </param>
		/// <param name="addr"> the raw IP address in network byte order </param>
		/// <returns>  an InetAddress object created from the raw IP address. </returns>
		/// <exception cref="UnknownHostException">  if IP address is of illegal length
		/// @since 1.4 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static InetAddress getByAddress(String host, byte[] addr) throws UnknownHostException
		public static InetAddress GetByAddress(String host, sbyte[] addr)
		{
			if (host != null && host.Length() > 0 && host.CharAt(0) == '[')
			{
				if (host.CharAt(host.Length() - 1) == ']')
				{
					host = host.Substring(1, host.Length() - 1 - 1);
				}
			}
			if (addr != null)
			{
				if (addr.Length == Inet4Address.INADDRSZ)
				{
					return new Inet4Address(host, addr);
				}
				else if (addr.Length == Inet6Address.INADDRSZ)
				{
					sbyte[] newAddr = IPAddressUtil.convertFromIPv4MappedAddress(addr);
					if (newAddr != null)
					{
						return new Inet4Address(host, newAddr);
					}
					else
					{
						return new Inet6Address(host, addr);
					}
				}
			}
			throw new UnknownHostException("addr is of illegal length");
		}


		/// <summary>
		/// Determines the IP address of a host, given the host's name.
		/// 
		/// <para> The host name can either be a machine name, such as
		/// "{@code java.sun.com}", or a textual representation of its
		/// IP address. If a literal IP address is supplied, only the
		/// validity of the address format is checked.
		/// 
		/// </para>
		/// <para> For {@code host} specified in literal IPv6 address,
		/// either the form defined in RFC 2732 or the literal IPv6 address
		/// format defined in RFC 2373 is accepted. IPv6 scoped addresses are also
		/// supported. See <a href="Inet6Address.html#scoped">here</a> for a description of IPv6
		/// scoped addresses.
		/// 
		/// </para>
		/// <para> If the host is {@code null} then an {@code InetAddress}
		/// representing an address of the loopback interface is returned.
		/// See <a href="http://www.ietf.org/rfc/rfc3330.txt">RFC&nbsp;3330</a>
		/// section&nbsp;2 and <a href="http://www.ietf.org/rfc/rfc2373.txt">RFC&nbsp;2373</a>
		/// section&nbsp;2.5.3. </para>
		/// </summary>
		/// <param name="host">   the specified host, or {@code null}. </param>
		/// <returns>     an IP address for the given host name. </returns>
		/// <exception cref="UnknownHostException">  if no IP address for the
		///               {@code host} could be found, or if a scope_id was specified
		///               for a global IPv6 address. </exception>
		/// <exception cref="SecurityException"> if a security manager exists
		///             and its checkConnect method doesn't allow the operation </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static InetAddress getByName(String host) throws UnknownHostException
		public static InetAddress GetByName(String host)
		{
			return InetAddress.GetAllByName(host)[0];
		}

		// called from deployment cache manager
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static InetAddress getByName(String host, InetAddress reqAddr) throws UnknownHostException
		private static InetAddress GetByName(String host, InetAddress reqAddr)
		{
			return InetAddress.GetAllByName(host, reqAddr)[0];
		}

		/// <summary>
		/// Given the name of a host, returns an array of its IP addresses,
		/// based on the configured name service on the system.
		/// 
		/// <para> The host name can either be a machine name, such as
		/// "{@code java.sun.com}", or a textual representation of its IP
		/// address. If a literal IP address is supplied, only the
		/// validity of the address format is checked.
		/// 
		/// </para>
		/// <para> For {@code host} specified in <i>literal IPv6 address</i>,
		/// either the form defined in RFC 2732 or the literal IPv6 address
		/// format defined in RFC 2373 is accepted. A literal IPv6 address may
		/// also be qualified by appending a scoped zone identifier or scope_id.
		/// The syntax and usage of scope_ids is described
		/// <a href="Inet6Address.html#scoped">here</a>.
		/// </para>
		/// <para> If the host is {@code null} then an {@code InetAddress}
		/// representing an address of the loopback interface is returned.
		/// See <a href="http://www.ietf.org/rfc/rfc3330.txt">RFC&nbsp;3330</a>
		/// section&nbsp;2 and <a href="http://www.ietf.org/rfc/rfc2373.txt">RFC&nbsp;2373</a>
		/// section&nbsp;2.5.3. </para>
		/// 
		/// <para> If there is a security manager and {@code host} is not
		/// null and {@code host.length() } is not equal to zero, the
		/// security manager's
		/// {@code checkConnect} method is called
		/// with the hostname and {@code -1}
		/// as its arguments to see if the operation is allowed.
		/// 
		/// </para>
		/// </summary>
		/// <param name="host">   the name of the host, or {@code null}. </param>
		/// <returns>     an array of all the IP addresses for a given host name.
		/// </returns>
		/// <exception cref="UnknownHostException">  if no IP address for the
		///               {@code host} could be found, or if a scope_id was specified
		///               for a global IPv6 address. </exception>
		/// <exception cref="SecurityException">  if a security manager exists and its
		///               {@code checkConnect} method doesn't allow the operation.
		/// </exception>
		/// <seealso cref= SecurityManager#checkConnect </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static InetAddress[] getAllByName(String host) throws UnknownHostException
		public static InetAddress[] GetAllByName(String host)
		{
			return GetAllByName(host, null);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static InetAddress[] getAllByName(String host, InetAddress reqAddr) throws UnknownHostException
		private static InetAddress[] GetAllByName(String host, InetAddress reqAddr)
		{

			if (host == null || host.Length() == 0)
			{
				InetAddress[] ret = new InetAddress[1];
				ret[0] = Impl.LoopbackAddress();
				return ret;
			}

			bool ipv6Expected = false;
			if (host.CharAt(0) == '[')
			{
				// This is supposed to be an IPv6 literal
				if (host.Length() > 2 && host.CharAt(host.Length() - 1) == ']')
				{
					host = host.Substring(1, host.Length() - 1 - 1);
					ipv6Expected = true;
				}
				else
				{
					// This was supposed to be a IPv6 address, but it's not!
					throw new UnknownHostException(host + ": invalid IPv6 address");
				}
			}

			// if host is an IP address, we won't do further lookup
			if (Character.Digit(host.CharAt(0), 16) != -1 || (host.CharAt(0) == ':'))
			{
				sbyte[] addr = null;
				int numericZone = -1;
				String ifname = null;
				// see if it is IPv4 address
				addr = IPAddressUtil.textToNumericFormatV4(host);
				if (addr == null)
				{
					// This is supposed to be an IPv6 literal
					// Check if a numeric or string zone id is present
					int pos;
					if ((pos = host.IndexOf("%")) != -1)
					{
						numericZone = CheckNumericZone(host);
						if (numericZone == -1) // remainder of string must be an ifname
						{
							ifname = host.Substring(pos + 1);
						}
					}
					if ((addr = IPAddressUtil.textToNumericFormatV6(host)) == null && host.Contains(":"))
					{
						throw new UnknownHostException(host + ": invalid IPv6 address");
					}
				}
				else if (ipv6Expected)
				{
					// Means an IPv4 litteral between brackets!
					throw new UnknownHostException("[" + host + "]");
				}
				InetAddress[] ret = new InetAddress[1];
				if (addr != null)
				{
					if (addr.Length == Inet4Address.INADDRSZ)
					{
						ret[0] = new Inet4Address(null, addr);
					}
					else
					{
						if (ifname != null)
						{
							ret[0] = new Inet6Address(null, addr, ifname);
						}
						else
						{
							ret[0] = new Inet6Address(null, addr, numericZone);
						}
					}
					return ret;
				}
			}
			else if (ipv6Expected)
			{
				// We were expecting an IPv6 Litteral, but got something else
				throw new UnknownHostException("[" + host + "]");
			}
			return GetAllByName0(host, reqAddr, true);
		}

		/// <summary>
		/// Returns the loopback address.
		/// <para>
		/// The InetAddress returned will represent the IPv4
		/// loopback address, 127.0.0.1, or the IPv6 loopback
		/// address, ::1. The IPv4 loopback address returned
		/// is only one of many in the form 127.*.*.*
		/// 
		/// </para>
		/// </summary>
		/// <returns>  the InetAddress loopback instance.
		/// @since 1.7 </returns>
		public static InetAddress LoopbackAddress
		{
			get
			{
				return Impl.LoopbackAddress();
			}
		}


		/// <summary>
		/// check if the literal address string has %nn appended
		/// returns -1 if not, or the numeric value otherwise.
		/// 
		/// %nn may also be a string that represents the displayName of
		/// a currently available NetworkInterface.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static int checkNumericZone(String s) throws UnknownHostException
		private static int CheckNumericZone(String s)
		{
			int percent = s.IndexOf('%');
			int slen = s.Length();
			int digit , zone = 0;
			if (percent == -1)
			{
				return -1;
			}
			for (int i = percent + 1; i < slen; i++)
			{
				char c = s.CharAt(i);
				if (c == ']')
				{
					if (i == percent + 1)
					{
						/* empty per-cent field */
						return -1;
					}
					break;
				}
				if ((digit = Character.Digit(c, 10)) < 0)
				{
					return -1;
				}
				zone = (zone * 10) + digit;
			}
			return zone;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static InetAddress[] getAllByName0(String host) throws UnknownHostException
		private static InetAddress[] GetAllByName0(String host)
		{
			return GetAllByName0(host, true);
		}

		/// <summary>
		/// package private so SocketPermission can call it
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: static InetAddress[] getAllByName0(String host, boolean check) throws UnknownHostException
		internal static InetAddress[] GetAllByName0(String host, bool check)
		{
			return GetAllByName0(host, null, check);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static InetAddress[] getAllByName0(String host, InetAddress reqAddr, boolean check) throws UnknownHostException
		private static InetAddress[] GetAllByName0(String host, InetAddress reqAddr, bool check)
		{

			/* If it gets here it is presumed to be a hostname */
			/* Cache.get can return: null, unknownAddress, or InetAddress[] */

			/* make sure the connection to the host is allowed, before we
			 * give out a hostname
			 */
			if (check)
			{
				SecurityManager security = System.SecurityManager;
				if (security != null)
				{
					security.CheckConnect(host, -1);
				}
			}

			InetAddress[] addresses = GetCachedAddresses(host);

			/* If no entry in cache, then do the host lookup */
			if (addresses == null)
			{
				addresses = GetAddressesFromNameService(host, reqAddr);
			}

			if (addresses == Unknown_array)
			{
				throw new UnknownHostException(host);
			}

			return addresses.clone();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static InetAddress[] getAddressesFromNameService(String host, InetAddress reqAddr) throws UnknownHostException
		private static InetAddress[] GetAddressesFromNameService(String host, InetAddress reqAddr)
		{
			InetAddress[] addresses = null;
			bool success = false;
			UnknownHostException ex = null;

			// Check whether the host is in the lookupTable.
			// 1) If the host isn't in the lookupTable when
			//    checkLookupTable() is called, checkLookupTable()
			//    would add the host in the lookupTable and
			//    return null. So we will do the lookup.
			// 2) If the host is in the lookupTable when
			//    checkLookupTable() is called, the current thread
			//    would be blocked until the host is removed
			//    from the lookupTable. Then this thread
			//    should try to look up the addressCache.
			//     i) if it found the addresses in the
			//        addressCache, checkLookupTable()  would
			//        return the addresses.
			//     ii) if it didn't find the addresses in the
			//         addressCache for any reason,
			//         it should add the host in the
			//         lookupTable and return null so the
			//         following code would do  a lookup itself.
			if ((addresses = CheckLookupTable(host)) == null)
			{
				try
				{
					// This is the first thread which looks up the addresses
					// this host or the cache entry for this host has been
					// expired so this thread should do the lookup.
					foreach (NameService nameService in NameServices)
					{
						try
						{
							/*
							 * Do not put the call to lookup() inside the
							 * constructor.  if you do you will still be
							 * allocating space when the lookup fails.
							 */

							addresses = nameService.lookupAllHostAddr(host);
							success = true;
							break;
						}
						catch (UnknownHostException uhe)
						{
							if (host.EqualsIgnoreCase("localhost"))
							{
								InetAddress[] local = new InetAddress[] {Impl.LoopbackAddress()};
								addresses = local;
								success = true;
								break;
							}
							else
							{
								addresses = Unknown_array;
								success = false;
								ex = uhe;
							}
						}
					}

					// More to do?
					if (reqAddr != null && addresses.Length > 1 && !addresses[0].Equals(reqAddr))
					{
						// Find it?
						int i = 1;
						for (; i < addresses.Length; i++)
						{
							if (addresses[i].Equals(reqAddr))
							{
								break;
							}
						}
						// Rotate
						if (i < addresses.Length)
						{
							InetAddress tmp , tmp2 = reqAddr;
							for (int j = 0; j < i; j++)
							{
								tmp = addresses[j];
								addresses[j] = tmp2;
								tmp2 = tmp;
							}
							addresses[i] = tmp2;
						}
					}
					// Cache the address.
					CacheAddresses(host, addresses, success);

					if (!success && ex != null)
					{
						throw ex;
					}

				}
				finally
				{
					// Delete host from the lookupTable and notify
					// all threads waiting on the lookupTable monitor.
					UpdateLookupTable(host);
				}
			}

			return addresses;
		}


		private static InetAddress[] CheckLookupTable(String host)
		{
			lock (LookupTable)
			{
				// If the host isn't in the lookupTable, add it in the
				// lookuptable and return null. The caller should do
				// the lookup.
				if (LookupTable.ContainsKey(host) == false)
				{
					LookupTable[host] = null;
					return null;
				}

				// If the host is in the lookupTable, it means that another
				// thread is trying to look up the addresses of this host.
				// This thread should wait.
				while (LookupTable.ContainsKey(host))
				{
					try
					{
						Monitor.Wait(LookupTable);
					}
					catch (InterruptedException)
					{
					}
				}
			}

			// The other thread has finished looking up the addresses of
			// the host. This thread should retry to get the addresses
			// from the addressCache. If it doesn't get the addresses from
			// the cache, it will try to look up the addresses itself.
			InetAddress[] addresses = GetCachedAddresses(host);
			if (addresses == null)
			{
				lock (LookupTable)
				{
					LookupTable[host] = null;
					return null;
				}
			}

			return addresses;
		}

		private static void UpdateLookupTable(String host)
		{
			lock (LookupTable)
			{
				LookupTable.Remove(host);
				Monitor.PulseAll(LookupTable);
			}
		}

		/// <summary>
		/// Returns an {@code InetAddress} object given the raw IP address .
		/// The argument is in network byte order: the highest order
		/// byte of the address is in {@code getAddress()[0]}.
		/// 
		/// <para> This method doesn't block, i.e. no reverse name service lookup
		/// is performed.
		/// 
		/// </para>
		/// <para> IPv4 address byte array must be 4 bytes long and IPv6 byte array
		/// must be 16 bytes long
		/// 
		/// </para>
		/// </summary>
		/// <param name="addr"> the raw IP address in network byte order </param>
		/// <returns>  an InetAddress object created from the raw IP address. </returns>
		/// <exception cref="UnknownHostException">  if IP address is of illegal length
		/// @since 1.4 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static InetAddress getByAddress(byte[] addr) throws UnknownHostException
		public static InetAddress GetByAddress(sbyte[] addr)
		{
			return GetByAddress(null, addr);
		}

		private static InetAddress CachedLocalHost = null;
		private static long CacheTime = 0;
		private const long MaxCacheTime = 5000L;
		private static readonly Object CacheLock = new Object();

		/// <summary>
		/// Returns the address of the local host. This is achieved by retrieving
		/// the name of the host from the system, then resolving that name into
		/// an {@code InetAddress}.
		/// 
		/// <P>Note: The resolved address may be cached for a short period of time.
		/// </P>
		/// 
		/// <para>If there is a security manager, its
		/// {@code checkConnect} method is called
		/// with the local host name and {@code -1}
		/// as its arguments to see if the operation is allowed.
		/// If the operation is not allowed, an InetAddress representing
		/// the loopback address is returned.
		/// 
		/// </para>
		/// </summary>
		/// <returns>     the address of the local host.
		/// </returns>
		/// <exception cref="UnknownHostException">  if the local host name could not
		///             be resolved into an address.
		/// </exception>
		/// <seealso cref= SecurityManager#checkConnect </seealso>
		/// <seealso cref= java.net.InetAddress#getByName(java.lang.String) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static InetAddress getLocalHost() throws UnknownHostException
		public static InetAddress LocalHost
		{
			get
			{
    
				SecurityManager security = System.SecurityManager;
				try
				{
					String local = Impl.LocalHostName;
    
					if (security != null)
					{
						security.CheckConnect(local, -1);
					}
    
					if (local.Equals("localhost"))
					{
						return Impl.LoopbackAddress();
					}
    
					InetAddress ret = null;
					lock (CacheLock)
					{
						long now = DateTimeHelperClass.CurrentUnixTimeMillis();
						if (CachedLocalHost != null)
						{
							if ((now - CacheTime) < MaxCacheTime) // Less than 5s old?
							{
								ret = CachedLocalHost;
							}
							else
							{
								CachedLocalHost = null;
							}
						}
    
						// we are calling getAddressesFromNameService directly
						// to avoid getting localHost from cache
						if (ret == null)
						{
							InetAddress[] localAddrs;
							try
							{
								localAddrs = InetAddress.GetAddressesFromNameService(local, null);
							}
							catch (UnknownHostException uhe)
							{
								// Rethrow with a more informative error message.
								UnknownHostException uhe2 = new UnknownHostException(local + ": " + uhe.Message);
								uhe2.InitCause(uhe);
								throw uhe2;
							}
							CachedLocalHost = localAddrs[0];
							CacheTime = now;
							ret = localAddrs[0];
						}
					}
					return ret;
				}
				catch (java.lang.SecurityException)
				{
					return Impl.LoopbackAddress();
				}
			}
		}

		/// <summary>
		/// Perform class load-time initializations.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern void init();


		/*
		 * Returns the InetAddress representing anyLocalAddress
		 * (typically 0.0.0.0 or ::0)
		 */
		internal static InetAddress AnyLocalAddress()
		{
			return Impl.AnyLocalAddress();
		}

		/*
		 * Load and instantiate an underlying impl class
		 */
		internal static InetAddressImpl LoadImpl(String implName)
		{
			Object impl = null;

			/*
			 * Property "impl.prefix" will be prepended to the classname
			 * of the implementation object we instantiate, to which we
			 * delegate the real work (like native methods).  This
			 * property can vary across implementations of the java.
			 * classes.  The default is an empty String "".
			 */
			String prefix = AccessController.doPrivileged(new GetPropertyAction("impl.prefix", ""));
			try
			{
				impl = Class.ForName("java.net." + prefix + implName).NewInstance();
			}
			catch (ClassNotFoundException)
			{
				System.Console.Error.WriteLine("Class not found: java.net." + prefix + implName + ":\ncheck impl.prefix property " + "in your properties file.");
			}
			catch (InstantiationException)
			{
				System.Console.Error.WriteLine("Could not instantiate: java.net." + prefix + implName + ":\ncheck impl.prefix property " + "in your properties file.");
			}
			catch (IllegalAccessException)
			{
				System.Console.Error.WriteLine("Cannot access class: java.net." + prefix + implName + ":\ncheck impl.prefix property " + "in your properties file.");
			}

			if (impl == null)
			{
				try
				{
					impl = Class.ForName(implName).NewInstance();
				}
				catch (Exception)
				{
					throw new Error("System property impl.prefix incorrect");
				}
			}

			return (InetAddressImpl) impl;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObjectNoData(java.io.ObjectInputStream s) throws java.io.IOException, ClassNotFoundException
		private void ReadObjectNoData(ObjectInputStream s)
		{
			if (this.GetType().ClassLoader != null)
			{
				throw new SecurityException("invalid address type");
			}
		}

		private static readonly long FIELDS_OFFSET;
		private static readonly sun.misc.Unsafe UNSAFE;


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream s) throws java.io.IOException, ClassNotFoundException
		private void ReadObject(ObjectInputStream s)
		{
			if (this.GetType().ClassLoader != null)
			{
				throw new SecurityException("invalid address type");
			}
			ObjectInputStream.GetField gf = s.ReadFields();
			String host = (String)gf.Get("hostName", null);
			int address = gf.Get("address", 0);
			int family = gf.Get("family", 0);
			InetAddressHolder h = new InetAddressHolder(host, address, family);
			UNSAFE.putObject(this, FIELDS_OFFSET, h);
		}

		/* needed because the serializable fields no longer exist */

		/// <summary>
		/// @serialField hostName String
		/// @serialField address int
		/// @serialField family int
		/// </summary>
		private static readonly ObjectStreamField[] SerialPersistentFields = new ObjectStreamField[] {new ObjectStreamField("hostName", typeof(String)), new ObjectStreamField("address", typeof(int)), new ObjectStreamField("family", typeof(int))};

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream s) throws java.io.IOException
		private void WriteObject(ObjectOutputStream s)
		{
			if (this.GetType().ClassLoader != null)
			{
				throw new SecurityException("invalid address type");
			}
			ObjectOutputStream.PutField pf = s.PutFields();
			pf.Put("hostName", Holder().HostName);
			pf.Put("address", Holder().Address);
			pf.Put("family", Holder().Family);
			s.WriteFields();
		}
	}

	/*
	 * Simple factory to create the impl
	 */
	internal class InetAddressImplFactory
	{

		internal static InetAddressImpl Create()
		{
			return InetAddress.LoadImpl(IPv6Supported ? "Inet6AddressImpl" : "Inet4AddressImpl");
		}

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		internal static extern boolean isIPv6Supported();
	}

}