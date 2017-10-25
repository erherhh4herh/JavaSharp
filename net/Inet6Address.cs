using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

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


	/// <summary>
	/// This class represents an Internet Protocol version 6 (IPv6) address.
	/// Defined by <a href="http://www.ietf.org/rfc/rfc2373.txt">
	/// <i>RFC&nbsp;2373: IP Version 6 Addressing Architecture</i></a>.
	/// 
	/// <h3> <A NAME="format">Textual representation of IP addresses</a> </h3>
	/// 
	/// Textual representation of IPv6 address used as input to methods
	/// takes one of the following forms:
	/// 
	/// <ol>
	///   <li><para> <A NAME="lform">The preferred form</a> is x:x:x:x:x:x:x:x,
	///   where the 'x's are
	///   the hexadecimal values of the eight 16-bit pieces of the
	///   address. This is the full form.  For example,
	/// 
	///   <blockquote><table cellpadding=0 cellspacing=0 summary="layout">
	///   <tr><td>{@code 1080:0:0:0:8:800:200C:417A}<td></tr>
	///   </table></blockquote>
	/// 
	/// </para>
	///   <para> Note that it is not necessary to write the leading zeros in
	///   an individual field. However, there must be at least one numeral
	///   in every field, except as described below.</li>
	/// 
	/// </para>
	///   <li><para> Due to some methods of allocating certain styles of IPv6
	///   addresses, it will be common for addresses to contain long
	///   strings of zero bits. In order to make writing addresses
	///   containing zero bits easier, a special syntax is available to
	///   compress the zeros. The use of "::" indicates multiple groups
	///   of 16-bits of zeros. The "::" can only appear once in an address.
	///   The "::" can also be used to compress the leading and/or trailing
	///   zeros in an address. For example,
	/// 
	///   <blockquote><table cellpadding=0 cellspacing=0 summary="layout">
	///   <tr><td>{@code 1080::8:800:200C:417A}<td></tr>
	///   </table></blockquote>
	/// 
	/// </para>
	///   <li><para> An alternative form that is sometimes more convenient
	///   when dealing with a mixed environment of IPv4 and IPv6 nodes is
	///   x:x:x:x:x:x:d.d.d.d, where the 'x's are the hexadecimal values
	///   of the six high-order 16-bit pieces of the address, and the 'd's
	///   are the decimal values of the four low-order 8-bit pieces of the
	///   standard IPv4 representation address, for example,
	/// 
	///   <blockquote><table cellpadding=0 cellspacing=0 summary="layout">
	///   <tr><td>{@code ::FFFF:129.144.52.38}<td></tr>
	///   <tr><td>{@code ::129.144.52.38}<td></tr>
	///   </table></blockquote>
	/// 
	/// </para>
	///   <para> where "::FFFF:d.d.d.d" and "::d.d.d.d" are, respectively, the
	///   general forms of an IPv4-mapped IPv6 address and an
	///   IPv4-compatible IPv6 address. Note that the IPv4 portion must be
	///   in the "d.d.d.d" form. The following forms are invalid:
	/// 
	///   <blockquote><table cellpadding=0 cellspacing=0 summary="layout">
	///   <tr><td>{@code ::FFFF:d.d.d}<td></tr>
	///   <tr><td>{@code ::FFFF:d.d}<td></tr>
	///   <tr><td>{@code ::d.d.d}<td></tr>
	///   <tr><td>{@code ::d.d}<td></tr>
	///   </table></blockquote>
	/// 
	/// </para>
	///   <para> The following form:
	/// 
	///   <blockquote><table cellpadding=0 cellspacing=0 summary="layout">
	///   <tr><td>{@code ::FFFF:d}<td></tr>
	///   </table></blockquote>
	/// 
	/// </para>
	///   <para> is valid, however it is an unconventional representation of
	///   the IPv4-compatible IPv6 address,
	/// 
	///   <blockquote><table cellpadding=0 cellspacing=0 summary="layout">
	///   <tr><td>{@code ::255.255.0.d}<td></tr>
	///   </table></blockquote>
	/// 
	/// </para>
	///   <para> while "::d" corresponds to the general IPv6 address
	///   "0:0:0:0:0:0:0:d".</li>
	/// </ol>
	/// 
	/// </para>
	/// <para> For methods that return a textual representation as output
	/// value, the full form is used. Inet6Address will return the full
	/// form because it is unambiguous when used in combination with other
	/// textual data.
	/// 
	/// <h4> Special IPv6 address </h4>
	/// 
	/// <blockquote>
	/// <table cellspacing=2 summary="Description of IPv4-mapped address">
	/// <tr><th valign=top><i>IPv4-mapped address</i></th>
	///         <td>Of the form::ffff:w.x.y.z, this IPv6 address is used to
	///         represent an IPv4 address. It allows the native program to
	///         use the same address data structure and also the same
	///         socket when communicating with both IPv4 and IPv6 nodes.
	/// 
	/// </para>
	///         <para>In InetAddress and Inet6Address, it is used for internal
	///         representation; it has no functional role. Java will never
	///         return an IPv4-mapped address.  These classes can take an
	///         IPv4-mapped address as input, both in byte array and text
	///         representation. However, it will be converted into an IPv4
	///         address.</td></tr>
	/// </table></blockquote>
	/// 
	/// <h4><A NAME="scoped">Textual representation of IPv6 scoped addresses</a></h4>
	/// 
	/// </para>
	/// <para> The textual representation of IPv6 addresses as described above can be
	/// extended to specify IPv6 scoped addresses. This extension to the basic
	/// addressing architecture is described in [draft-ietf-ipngwg-scoping-arch-04.txt].
	/// 
	/// </para>
	/// <para> Because link-local and site-local addresses are non-global, it is possible
	/// that different hosts may have the same destination address and may be
	/// reachable through different interfaces on the same originating system. In
	/// this case, the originating system is said to be connected to multiple zones
	/// of the same scope. In order to disambiguate which is the intended destination
	/// zone, it is possible to append a zone identifier (or <i>scope_id</i>) to an
	/// IPv6 address.
	/// 
	/// </para>
	/// <para> The general format for specifying the <i>scope_id</i> is the following:
	/// 
	/// <blockquote><i>IPv6-address</i>%<i>scope_id</i></blockquote>
	/// </para>
	/// <para> The IPv6-address is a literal IPv6 address as described above.
	/// The <i>scope_id</i> refers to an interface on the local system, and it can be
	/// specified in two ways.
	/// <ol><li><i>As a numeric identifier.</i> This must be a positive integer
	/// that identifies the particular interface and scope as understood by the
	/// system. Usually, the numeric values can be determined through administration
	/// tools on the system. Each interface may have multiple values, one for each
	/// scope. If the scope is unspecified, then the default value used is zero.</li>
	/// <li><i>As a string.</i> This must be the exact string that is returned by
	/// <seealso cref="java.net.NetworkInterface#getName()"/> for the particular interface in
	/// question. When an Inet6Address is created in this way, the numeric scope-id
	/// is determined at the time the object is created by querying the relevant
	/// NetworkInterface.</li></ol>
	/// 
	/// </para>
	/// <para> Note also, that the numeric <i>scope_id</i> can be retrieved from
	/// Inet6Address instances returned from the NetworkInterface class. This can be
	/// used to find out the current scope ids configured on the system.
	/// @since 1.4
	/// </para>
	/// </summary>

	public sealed class Inet6Address : InetAddress
	{
		internal const int INADDRSZ = 16;

		/*
		 * cached scope_id - for link-local address use only.
		 */
		[NonSerialized]
		private int Cached_scope_id; // 0

		private class Inet6AddressHolder
		{
			private readonly Inet6Address OuterInstance;


			internal Inet6AddressHolder(Inet6Address outerInstance)
			{
				this.OuterInstance = outerInstance;
				Ipaddress = new sbyte[INADDRSZ];
			}

			internal Inet6AddressHolder(Inet6Address outerInstance, sbyte[] ipaddress, int scope_id, bool scope_id_set, NetworkInterface ifname, bool scope_ifname_set)
			{
				this.OuterInstance = outerInstance;
				this.Ipaddress = ipaddress;
				this.Scope_id = scope_id;
				this.Scope_id_set = scope_id_set;
				this.Scope_ifname_set = scope_ifname_set;
				this.Scope_ifname = ifname;
			}

			/// <summary>
			/// Holds a 128-bit (16 bytes) IPv6 address.
			/// </summary>
			internal sbyte[] Ipaddress;

			/// <summary>
			/// scope_id. The scope specified when the object is created. If the object
			/// is created with an interface name, then the scope_id is not determined
			/// until the time it is needed.
			/// </summary>
			internal int Scope_id; // 0

			/// <summary>
			/// This will be set to true when the scope_id field contains a valid
			/// integer scope_id.
			/// </summary>
			internal bool Scope_id_set; // false

			/// <summary>
			/// scoped interface. scope_id is derived from this as the scope_id of the first
			/// address whose scope is the same as this address for the named interface.
			/// </summary>
			internal NetworkInterface Scope_ifname; // null

			/// <summary>
			/// set if the object is constructed with a scoped
			/// interface instead of a numeric scope id.
			/// </summary>
			internal bool Scope_ifname_set; // false;

			internal virtual sbyte[] Addr
			{
				set
				{
					if (value.Length == INADDRSZ) // normal IPv6 address
					{
						System.Array.Copy(value, 0, Ipaddress, 0, INADDRSZ);
					}
				}
			}

			internal virtual void Init(sbyte[] addr, int scope_id)
			{
				Addr = addr;

				if (scope_id >= 0)
				{
					this.Scope_id = scope_id;
					this.Scope_id_set = true;
				}
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void init(byte addr[] , NetworkInterface nif) throws UnknownHostException
			internal virtual void Init(sbyte[] addr, NetworkInterface nif)
			{
				Addr = addr;

				if (nif != null)
				{
					this.Scope_id = DeriveNumericScope(Ipaddress, nif);
					this.Scope_id_set = true;
					this.Scope_ifname = nif;
					this.Scope_ifname_set = true;
				}
			}

			internal virtual String HostAddress
			{
				get
				{
					String s = NumericToTextFormat(Ipaddress);
					if (Scope_ifname != null) // must check this first
					{
						s = s + "%" + Scope_ifname.Name;
					}
					else if (Scope_id_set)
					{
						s = s + "%" + Scope_id;
					}
					return s;
				}
			}

			public override bool Equals(Object o)
			{
				if (!(o is Inet6AddressHolder))
				{
					return false;
				}
				Inet6AddressHolder that = (Inet6AddressHolder)o;

				return Arrays.Equals(this.Ipaddress, that.Ipaddress);
			}

			public override int HashCode()
			{
				if (Ipaddress != null)
				{

					int hash = 0;
					int i = 0;
					while (i < INADDRSZ)
					{
						int j = 0;
						int component = 0;
						while (j < 4 && i < INADDRSZ)
						{
							component = (component << 8) + Ipaddress[i];
							j++;
							i++;
						}
						hash += component;
					}
					return hash;

				}
				else
				{
					return 0;
				}
			}

			internal virtual bool IPv4CompatibleAddress
			{
				get
				{
					if ((Ipaddress[0] == 0x00) && (Ipaddress[1] == 0x00) && (Ipaddress[2] == 0x00) && (Ipaddress[3] == 0x00) && (Ipaddress[4] == 0x00) && (Ipaddress[5] == 0x00) && (Ipaddress[6] == 0x00) && (Ipaddress[7] == 0x00) && (Ipaddress[8] == 0x00) && (Ipaddress[9] == 0x00) && (Ipaddress[10] == 0x00) && (Ipaddress[11] == 0x00))
					{
						return true;
					}
					return false;
				}
			}

			internal virtual bool MulticastAddress
			{
				get
				{
					return ((Ipaddress[0] & 0xff) == 0xff);
				}
			}

			internal virtual bool AnyLocalAddress
			{
				get
				{
					sbyte test = 0x00;
					for (int i = 0; i < INADDRSZ; i++)
					{
						test |= Ipaddress[i];
					}
					return (test == 0x00);
				}
			}

			internal virtual bool LoopbackAddress
			{
				get
				{
					sbyte test = 0x00;
					for (int i = 0; i < 15; i++)
					{
						test |= Ipaddress[i];
					}
					return (test == 0x00) && (Ipaddress[15] == 0x01);
				}
			}

			internal virtual bool LinkLocalAddress
			{
				get
				{
					return ((Ipaddress[0] & 0xff) == 0xfe && (Ipaddress[1] & 0xc0) == 0x80);
				}
			}


			internal virtual bool SiteLocalAddress
			{
				get
				{
					return ((Ipaddress[0] & 0xff) == 0xfe && (Ipaddress[1] & 0xc0) == 0xc0);
				}
			}

			internal virtual bool MCGlobal
			{
				get
				{
					return ((Ipaddress[0] & 0xff) == 0xff && (Ipaddress[1] & 0x0f) == 0x0e);
				}
			}

			internal virtual bool MCNodeLocal
			{
				get
				{
					return ((Ipaddress[0] & 0xff) == 0xff && (Ipaddress[1] & 0x0f) == 0x01);
				}
			}

			internal virtual bool MCLinkLocal
			{
				get
				{
					return ((Ipaddress[0] & 0xff) == 0xff && (Ipaddress[1] & 0x0f) == 0x02);
				}
			}

			internal virtual bool MCSiteLocal
			{
				get
				{
					return ((Ipaddress[0] & 0xff) == 0xff && (Ipaddress[1] & 0x0f) == 0x05);
				}
			}

			internal virtual bool MCOrgLocal
			{
				get
				{
					return ((Ipaddress[0] & 0xff) == 0xff && (Ipaddress[1] & 0x0f) == 0x08);
				}
			}
		}

		[NonSerialized]
		private readonly Inet6AddressHolder Holder6;

		private const long SerialVersionUID = 6880410070516793377L;

		// Perform native initialization
		static Inet6Address()
		{
			init();
			try
			{
				sun.misc.Unsafe @unsafe = sun.misc.Unsafe.Unsafe;
				FIELDS_OFFSET = @unsafe.objectFieldOffset(typeof(Inet6Address).getDeclaredField("holder6"));
				UNSAFE = @unsafe;
			}
			catch (ReflectiveOperationException e)
			{
				throw new Error(e);
			}
		}

		internal Inet6Address() : base()
		{
			Holder_Renamed.Init(null, IPv6);
			Holder6 = new Inet6AddressHolder(this);
		}

		/* checking of value for scope_id should be done by caller
		 * scope_id must be >= 0, or -1 to indicate not being set
		 */
		internal Inet6Address(String hostName, sbyte[] addr, int scope_id)
		{
			Holder_Renamed.Init(hostName, IPv6);
			Holder6 = new Inet6AddressHolder(this);
			Holder6.Init(addr, scope_id);
		}

		internal Inet6Address(String hostName, sbyte[] addr)
		{
			Holder6 = new Inet6AddressHolder(this);
			try
			{
				Initif(hostName, addr, null);
			} // cant happen if ifname is null
			catch (UnknownHostException)
			{
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: Inet6Address(String hostName, byte addr[] , NetworkInterface nif) throws UnknownHostException
		internal Inet6Address(String hostName, sbyte[] addr, NetworkInterface nif)
		{
			Holder6 = new Inet6AddressHolder(this);
			Initif(hostName, addr, nif);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: Inet6Address(String hostName, byte addr[] , String ifname) throws UnknownHostException
		internal Inet6Address(String hostName, sbyte[] addr, String ifname)
		{
			Holder6 = new Inet6AddressHolder(this);
			Initstr(hostName, addr, ifname);
		}

		/// <summary>
		/// Create an Inet6Address in the exact manner of {@link
		/// InetAddress#getByAddress(String,byte[])} except that the IPv6 scope_id is
		/// set to the value corresponding to the given interface for the address
		/// type specified in {@code addr}. The call will fail with an
		/// UnknownHostException if the given interface does not have a numeric
		/// scope_id assigned for the given address type (eg. link-local or site-local).
		/// See <a href="Inet6Address.html#scoped">here</a> for a description of IPv6
		/// scoped addresses.
		/// </summary>
		/// <param name="host"> the specified host </param>
		/// <param name="addr"> the raw IP address in network byte order </param>
		/// <param name="nif"> an interface this address must be associated with. </param>
		/// <returns>  an Inet6Address object created from the raw IP address. </returns>
		/// <exception cref="UnknownHostException">
		///          if IP address is of illegal length, or if the interface does not
		///          have a numeric scope_id assigned for the given address type.
		/// 
		/// @since 1.5 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static Inet6Address getByAddress(String host, byte[] addr, NetworkInterface nif) throws UnknownHostException
		public static Inet6Address GetByAddress(String host, sbyte[] addr, NetworkInterface nif)
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
				if (addr.Length == Inet6Address.INADDRSZ)
				{
					return new Inet6Address(host, addr, nif);
				}
			}
			throw new UnknownHostException("addr is of illegal length");
		}

		/// <summary>
		/// Create an Inet6Address in the exact manner of {@link
		/// InetAddress#getByAddress(String,byte[])} except that the IPv6 scope_id is
		/// set to the given numeric value. The scope_id is not checked to determine
		/// if it corresponds to any interface on the system.
		/// See <a href="Inet6Address.html#scoped">here</a> for a description of IPv6
		/// scoped addresses.
		/// </summary>
		/// <param name="host"> the specified host </param>
		/// <param name="addr"> the raw IP address in network byte order </param>
		/// <param name="scope_id"> the numeric scope_id for the address. </param>
		/// <returns>  an Inet6Address object created from the raw IP address. </returns>
		/// <exception cref="UnknownHostException">  if IP address is of illegal length.
		/// 
		/// @since 1.5 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static Inet6Address getByAddress(String host, byte[] addr, int scope_id) throws UnknownHostException
		public static Inet6Address GetByAddress(String host, sbyte[] addr, int scope_id)
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
				if (addr.Length == Inet6Address.INADDRSZ)
				{
					return new Inet6Address(host, addr, scope_id);
				}
			}
			throw new UnknownHostException("addr is of illegal length");
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void initstr(String hostName, byte addr[] , String ifname) throws UnknownHostException
		private void Initstr(String hostName, sbyte[] addr, String ifname)
		{
			try
			{
				NetworkInterface nif = NetworkInterface.GetByName(ifname);
				if (nif == null)
				{
					throw new UnknownHostException("no such interface " + ifname);
				}
				Initif(hostName, addr, nif);
			}
			catch (SocketException)
			{
				throw new UnknownHostException("SocketException thrown" + ifname);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void initif(String hostName, byte addr[] , NetworkInterface nif) throws UnknownHostException
		private void Initif(String hostName, sbyte[] addr, NetworkInterface nif)
		{
			int family = -1;
			Holder6.Init(addr, nif);

			if (addr.Length == INADDRSZ) // normal IPv6 address
			{
				family = IPv6;
			}
			Holder_Renamed.Init(hostName, family);
		}

		/* check the two Ipv6 addresses and return false if they are both
		 * non global address types, but not the same.
		 * (ie. one is sitelocal and the other linklocal)
		 * return true otherwise.
		 */

		private static bool IsDifferentLocalAddressType(sbyte[] thisAddr, sbyte[] otherAddr)
		{

			if (Inet6Address.IsLinkLocalAddress(thisAddr) && !Inet6Address.IsLinkLocalAddress(otherAddr))
			{
				return false;
			}
			if (Inet6Address.IsSiteLocalAddress(thisAddr) && !Inet6Address.IsSiteLocalAddress(otherAddr))
			{
				return false;
			}
			return true;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static int deriveNumericScope(byte[] thisAddr, NetworkInterface ifc) throws UnknownHostException
		private static int DeriveNumericScope(sbyte[] thisAddr, NetworkInterface ifc)
		{
			IEnumerator<InetAddress> addresses = ifc.InetAddresses;
			while (addresses.MoveNext())
			{
				InetAddress addr = addresses.Current;
				if (!(addr is Inet6Address))
				{
					continue;
				}
				Inet6Address ia6_addr = (Inet6Address)addr;
				/* check if site or link local prefixes match */
				if (!IsDifferentLocalAddressType(thisAddr, ia6_addr.Address))
				{
					/* type not the same, so carry on searching */
					continue;
				}
				/* found a matching address - return its scope_id */
				return ia6_addr.ScopeId;
			}
			throw new UnknownHostException("no scope_id found");
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private int deriveNumericScope(String ifname) throws UnknownHostException
		private int DeriveNumericScope(String ifname)
		{
			IEnumerator<NetworkInterface> en;
			try
			{
				en = NetworkInterface.NetworkInterfaces;
			}
			catch (SocketException)
			{
				throw new UnknownHostException("could not enumerate local network interfaces");
			}
			while (en.MoveNext())
			{
				NetworkInterface ifc = en.Current;
				if (ifc.Name.Equals(ifname))
				{
					return DeriveNumericScope(Holder6.Ipaddress, ifc);
				}
			}
			throw new UnknownHostException("No matching address found for interface : " + ifname);
		}

		/// <summary>
		/// @serialField ipaddress byte[]
		/// @serialField scope_id int
		/// @serialField scope_id_set boolean
		/// @serialField scope_ifname_set boolean
		/// @serialField ifname String
		/// </summary>

		private static readonly ObjectStreamField[] SerialPersistentFields = new ObjectStreamField[] {new ObjectStreamField("ipaddress", typeof(sbyte[])), new ObjectStreamField("scope_id", typeof(int)), new ObjectStreamField("scope_id_set", typeof(bool)), new ObjectStreamField("scope_ifname_set", typeof(bool)), new ObjectStreamField("ifname", typeof(String))};

		private static readonly long FIELDS_OFFSET;
		private static readonly sun.misc.Unsafe UNSAFE;


		/// <summary>
		/// restore the state of this object from stream
		/// including the scope information, only if the
		/// scoped interface name is valid on this system
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream s) throws java.io.IOException, ClassNotFoundException
		private void ReadObject(ObjectInputStream s)
		{
			NetworkInterface scope_ifname = null;

			if (this.GetType().ClassLoader != null)
			{
				throw new SecurityException("invalid address type");
			}

			ObjectInputStream.GetField gf = s.ReadFields();
			sbyte[] ipaddress = (sbyte[])gf.Get("ipaddress", null);
			int scope_id = (int)gf.Get("scope_id", -1);
			bool scope_id_set = (bool)gf.Get("scope_id_set", false);
			bool scope_ifname_set = (bool)gf.Get("scope_ifname_set", false);
			String ifname = (String)gf.Get("ifname", null);

			if (ifname != null && !"".Equals(ifname))
			{
				try
				{
					scope_ifname = NetworkInterface.GetByName(ifname);
					if (scope_ifname == null)
					{
						/* the interface does not exist on this system, so we clear
						 * the scope information completely */
						scope_id_set = false;
						scope_ifname_set = false;
						scope_id = 0;
					}
					else
					{
						scope_ifname_set = true;
						try
						{
							scope_id = DeriveNumericScope(ipaddress, scope_ifname);
						}
						catch (UnknownHostException)
						{
							// typically should not happen, but it may be that
							// the machine being used for deserialization has
							// the same interface name but without IPv6 configured.
						}
					}
				}
				catch (SocketException)
				{
				}
			}

			/* if ifname was not supplied, then the numeric info is used */

			ipaddress = ipaddress.clone();

			// Check that our invariants are satisfied
			if (ipaddress.Length != INADDRSZ)
			{
				throw new InvalidObjectException("invalid address length: " + ipaddress.Length);
			}

			if (Holder_Renamed.Family != IPv6)
			{
				throw new InvalidObjectException("invalid address family type");
			}

			Inet6AddressHolder h = new Inet6AddressHolder(this, ipaddress, scope_id, scope_id_set, scope_ifname, scope_ifname_set);

			UNSAFE.putObject(this, FIELDS_OFFSET, h);
		}

		/// <summary>
		/// default behavior is overridden in order to write the
		/// scope_ifname field as a String, rather than a NetworkInterface
		/// which is not serializable
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private synchronized void writeObject(java.io.ObjectOutputStream s) throws java.io.IOException
		private void WriteObject(ObjectOutputStream s)
		{
			lock (this)
			{
					String ifname = null;
        
				if (Holder6.Scope_ifname != null)
				{
					ifname = Holder6.Scope_ifname.Name;
					Holder6.Scope_ifname_set = true;
				}
				ObjectOutputStream.PutField pfields = s.PutFields();
				pfields.Put("ipaddress", Holder6.Ipaddress);
				pfields.Put("scope_id", Holder6.Scope_id);
				pfields.Put("scope_id_set", Holder6.Scope_id_set);
				pfields.Put("scope_ifname_set", Holder6.Scope_ifname_set);
				pfields.Put("ifname", ifname);
				s.WriteFields();
			}
		}

		/// <summary>
		/// Utility routine to check if the InetAddress is an IP multicast
		/// address. 11111111 at the start of the address identifies the
		/// address as being a multicast address.
		/// </summary>
		/// <returns> a {@code boolean} indicating if the InetAddress is an IP
		///         multicast address
		/// 
		/// @since JDK1.1 </returns>
		public override bool MulticastAddress
		{
			get
			{
				return Holder6.MulticastAddress;
			}
		}

		/// <summary>
		/// Utility routine to check if the InetAddress in a wildcard address.
		/// </summary>
		/// <returns> a {@code boolean} indicating if the Inetaddress is
		///         a wildcard address.
		/// 
		/// @since 1.4 </returns>
		public override bool AnyLocalAddress
		{
			get
			{
				return Holder6.AnyLocalAddress;
			}
		}

		/// <summary>
		/// Utility routine to check if the InetAddress is a loopback address.
		/// </summary>
		/// <returns> a {@code boolean} indicating if the InetAddress is a loopback
		///         address; or false otherwise.
		/// 
		/// @since 1.4 </returns>
		public override bool LoopbackAddress
		{
			get
			{
				return Holder6.LoopbackAddress;
			}
		}

		/// <summary>
		/// Utility routine to check if the InetAddress is an link local address.
		/// </summary>
		/// <returns> a {@code boolean} indicating if the InetAddress is a link local
		///         address; or false if address is not a link local unicast address.
		/// 
		/// @since 1.4 </returns>
		public override bool LinkLocalAddress
		{
			get
			{
				return Holder6.LinkLocalAddress;
			}
		}

		/* static version of above */
		internal static bool IsLinkLocalAddress(sbyte[] ipaddress)
		{
			return ((ipaddress[0] & 0xff) == 0xfe && (ipaddress[1] & 0xc0) == 0x80);
		}

		/// <summary>
		/// Utility routine to check if the InetAddress is a site local address.
		/// </summary>
		/// <returns> a {@code boolean} indicating if the InetAddress is a site local
		///         address; or false if address is not a site local unicast address.
		/// 
		/// @since 1.4 </returns>
		public override bool SiteLocalAddress
		{
			get
			{
				return Holder6.SiteLocalAddress;
			}
		}

		/* static version of above */
		internal static bool IsSiteLocalAddress(sbyte[] ipaddress)
		{
			return ((ipaddress[0] & 0xff) == 0xfe && (ipaddress[1] & 0xc0) == 0xc0);
		}

		/// <summary>
		/// Utility routine to check if the multicast address has global scope.
		/// </summary>
		/// <returns> a {@code boolean} indicating if the address has is a multicast
		///         address of global scope, false if it is not of global scope or
		///         it is not a multicast address
		/// 
		/// @since 1.4 </returns>
		public override bool MCGlobal
		{
			get
			{
				return Holder6.MCGlobal;
			}
		}

		/// <summary>
		/// Utility routine to check if the multicast address has node scope.
		/// </summary>
		/// <returns> a {@code boolean} indicating if the address has is a multicast
		///         address of node-local scope, false if it is not of node-local
		///         scope or it is not a multicast address
		/// 
		/// @since 1.4 </returns>
		public override bool MCNodeLocal
		{
			get
			{
				return Holder6.MCNodeLocal;
			}
		}

		/// <summary>
		/// Utility routine to check if the multicast address has link scope.
		/// </summary>
		/// <returns> a {@code boolean} indicating if the address has is a multicast
		///         address of link-local scope, false if it is not of link-local
		///         scope or it is not a multicast address
		/// 
		/// @since 1.4 </returns>
		public override bool MCLinkLocal
		{
			get
			{
				return Holder6.MCLinkLocal;
			}
		}

		/// <summary>
		/// Utility routine to check if the multicast address has site scope.
		/// </summary>
		/// <returns> a {@code boolean} indicating if the address has is a multicast
		///         address of site-local scope, false if it is not  of site-local
		///         scope or it is not a multicast address
		/// 
		/// @since 1.4 </returns>
		public override bool MCSiteLocal
		{
			get
			{
				return Holder6.MCSiteLocal;
			}
		}

		/// <summary>
		/// Utility routine to check if the multicast address has organization scope.
		/// </summary>
		/// <returns> a {@code boolean} indicating if the address has is a multicast
		///         address of organization-local scope, false if it is not of
		///         organization-local scope or it is not a multicast address
		/// 
		/// @since 1.4 </returns>
		public override bool MCOrgLocal
		{
			get
			{
				return Holder6.MCOrgLocal;
			}
		}
		/// <summary>
		/// Returns the raw IP address of this {@code InetAddress} object. The result
		/// is in network byte order: the highest order byte of the address is in
		/// {@code getAddress()[0]}.
		/// </summary>
		/// <returns>  the raw IP address of this object. </returns>
		public override sbyte[] Address
		{
			get
			{
				return Holder6.Ipaddress.clone();
			}
		}

		/// <summary>
		/// Returns the numeric scopeId, if this instance is associated with
		/// an interface. If no scoped_id is set, the returned value is zero.
		/// </summary>
		/// <returns> the scopeId, or zero if not set.
		/// 
		/// @since 1.5 </returns>
		 public int ScopeId
		 {
			 get
			 {
				return Holder6.Scope_id;
			 }
		 }

		/// <summary>
		/// Returns the scoped interface, if this instance was created with
		/// with a scoped interface.
		/// </summary>
		/// <returns> the scoped interface, or null if not set.
		/// @since 1.5 </returns>
		 public NetworkInterface ScopedInterface
		 {
			 get
			 {
				return Holder6.Scope_ifname;
			 }
		 }

		/// <summary>
		/// Returns the IP address string in textual presentation. If the instance
		/// was created specifying a scope identifier then the scope id is appended
		/// to the IP address preceded by a "%" (per-cent) character. This can be
		/// either a numeric value or a string, depending on which was used to create
		/// the instance.
		/// </summary>
		/// <returns>  the raw IP address in a string format. </returns>
		public override String HostAddress
		{
			get
			{
				return Holder6.HostAddress;
			}
		}

		/// <summary>
		/// Returns a hashcode for this IP address.
		/// </summary>
		/// <returns>  a hash code value for this IP address. </returns>
		public override int HashCode()
		{
			return Holder6.HashCode();
		}

		/// <summary>
		/// Compares this object against the specified object. The result is {@code
		/// true} if and only if the argument is not {@code null} and it represents
		/// the same IP address as this object.
		/// 
		/// <para> Two instances of {@code InetAddress} represent the same IP address
		/// if the length of the byte arrays returned by {@code getAddress} is the
		/// same for both, and each of the array components is the same for the byte
		/// arrays.
		/// 
		/// </para>
		/// </summary>
		/// <param name="obj">   the object to compare against.
		/// </param>
		/// <returns>  {@code true} if the objects are the same; {@code false} otherwise.
		/// </returns>
		/// <seealso cref=     java.net.InetAddress#getAddress() </seealso>
		public override bool Equals(Object obj)
		{
			if (obj == null || !(obj is Inet6Address))
			{
				return false;
			}

			Inet6Address inetAddr = (Inet6Address)obj;

			return Holder6.Equals(inetAddr.Holder6);
		}

		/// <summary>
		/// Utility routine to check if the InetAddress is an
		/// IPv4 compatible IPv6 address.
		/// </summary>
		/// <returns> a {@code boolean} indicating if the InetAddress is an IPv4
		///         compatible IPv6 address; or false if address is IPv4 address.
		/// 
		/// @since 1.4 </returns>
		public bool IPv4CompatibleAddress
		{
			get
			{
				return Holder6.IPv4CompatibleAddress;
			}
		}

		// Utilities
		private const int INT16SZ = 2;

		/*
		 * Convert IPv6 binary address into presentation (printable) format.
		 *
		 * @param src a byte array representing the IPv6 numeric address
		 * @return a String representing an IPv6 address in
		 *         textual representation format
		 * @since 1.4
		 */
		internal static String NumericToTextFormat(sbyte[] src)
		{
			StringBuilder sb = new StringBuilder(39);
			for (int i = 0; i < (INADDRSZ / INT16SZ); i++)
			{
				sb.Append((((src[i << 1] << 8) & 0xff00) | (src[(i << 1) + 1] & 0xff)).ToString("x"));
				if (i < (INADDRSZ / INT16SZ) - 1)
				{
				   sb.Append(":");
				}
			}
			return sb.ToString();
		}

		/// <summary>
		/// Perform class load-time initializations.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern void init();
	}

}