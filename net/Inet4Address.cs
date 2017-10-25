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
	/// This class represents an Internet Protocol version 4 (IPv4) address.
	/// Defined by <a href="http://www.ietf.org/rfc/rfc790.txt">
	/// <i>RFC&nbsp;790: Assigned Numbers</i></a>,
	/// <a href="http://www.ietf.org/rfc/rfc1918.txt">
	/// <i>RFC&nbsp;1918: Address Allocation for Private Internets</i></a>,
	/// and <a href="http://www.ietf.org/rfc/rfc2365.txt"><i>RFC&nbsp;2365:
	/// Administratively Scoped IP Multicast</i></a>
	/// 
	/// <h3> <A NAME="format">Textual representation of IP addresses</a> </h3>
	/// 
	/// Textual representation of IPv4 address used as input to methods
	/// takes one of the following forms:
	/// 
	/// <blockquote><table cellpadding=0 cellspacing=0 summary="layout">
	/// <tr><td>{@code d.d.d.d}</td></tr>
	/// <tr><td>{@code d.d.d}</td></tr>
	/// <tr><td>{@code d.d}</td></tr>
	/// <tr><td>{@code d}</td></tr>
	/// </table></blockquote>
	/// 
	/// <para> When four parts are specified, each is interpreted as a byte of
	/// data and assigned, from left to right, to the four bytes of an IPv4
	/// address.
	/// 
	/// </para>
	/// <para> When a three part address is specified, the last part is
	/// interpreted as a 16-bit quantity and placed in the right most two
	/// bytes of the network address. This makes the three part address
	/// format convenient for specifying Class B net- work addresses as
	/// 128.net.host.
	/// 
	/// </para>
	/// <para> When a two part address is supplied, the last part is
	/// interpreted as a 24-bit quantity and placed in the right most three
	/// bytes of the network address. This makes the two part address
	/// format convenient for specifying Class A network addresses as
	/// net.host.
	/// 
	/// </para>
	/// <para> When only one part is given, the value is stored directly in
	/// the network address without any byte rearrangement.
	/// 
	/// </para>
	/// <para> For methods that return a textual representation as output
	/// value, the first form, i.e. a dotted-quad string, is used.
	/// 
	/// <h4> The Scope of a Multicast Address </h4>
	/// 
	/// Historically the IPv4 TTL field in the IP header has doubled as a
	/// multicast scope field: a TTL of 0 means node-local, 1 means
	/// link-local, up through 32 means site-local, up through 64 means
	/// region-local, up through 128 means continent-local, and up through
	/// 255 are global. However, the administrative scoping is preferred.
	/// Please refer to <a href="http://www.ietf.org/rfc/rfc2365.txt">
	/// <i>RFC&nbsp;2365: Administratively Scoped IP Multicast</i></a>
	/// @since 1.4
	/// </para>
	/// </summary>

	public sealed class Inet4Address : InetAddress
	{
		internal const int INADDRSZ = 4;

		/// <summary>
		/// use serialVersionUID from InetAddress, but Inet4Address instance
		///  is always replaced by an InetAddress instance before being
		///  serialized 
		/// </summary>
		private const long SerialVersionUID = 3286316764910316507L;

		/*
		 * Perform initializations.
		 */
		static Inet4Address()
		{
			init();
		}

		internal Inet4Address() : base()
		{
			Holder().HostName_Renamed = null;
			Holder().Address_Renamed = 0;
			Holder().Family_Renamed = IPv4;
		}

		internal Inet4Address(String hostName, sbyte[] addr)
		{
			Holder().HostName_Renamed = hostName;
			Holder().Family_Renamed = IPv4;
			if (addr != null)
			{
				if (addr.Length == INADDRSZ)
				{
					int address = addr[3] & 0xFF;
					address |= ((addr[2] << 8) & 0xFF00);
					address |= ((addr[1] << 16) & 0xFF0000);
					address |= ((addr[0] << 24) & 0xFF000000);
					Holder().Address_Renamed = address;
				}
			}
			Holder().OriginalHostName_Renamed = hostName;
		}
		internal Inet4Address(String hostName, int address)
		{
			Holder().HostName_Renamed = hostName;
			Holder().Family_Renamed = IPv4;
			Holder().Address_Renamed = address;
			Holder().OriginalHostName_Renamed = hostName;
		}

		/// <summary>
		/// Replaces the object to be serialized with an InetAddress object.
		/// </summary>
		/// <returns> the alternate object to be serialized.
		/// </returns>
		/// <exception cref="ObjectStreamException"> if a new object replacing this
		/// object could not be created </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private Object writeReplace() throws java.io.ObjectStreamException
		private Object WriteReplace()
		{
			// will replace the to be serialized 'this' object
			InetAddress inet = new InetAddress();
			inet.Holder().HostName_Renamed = Holder().HostName;
			inet.Holder().Address_Renamed = Holder().Address;

			/// <summary>
			/// Prior to 1.4 an InetAddress was created with a family
			/// based on the platform AF_INET value (usually 2).
			/// For compatibility reasons we must therefore write the
			/// the InetAddress with this family.
			/// </summary>
			inet.Holder().Family_Renamed = 2;

			return inet;
		}

		/// <summary>
		/// Utility routine to check if the InetAddress is an
		/// IP multicast address. IP multicast address is a Class D
		/// address i.e first four bits of the address are 1110. </summary>
		/// <returns> a {@code boolean} indicating if the InetAddress is
		/// an IP multicast address
		/// @since   JDK1.1 </returns>
		public override bool MulticastAddress
		{
			get
			{
				return ((Holder().Address & 0xf0000000) == 0xe0000000);
			}
		}

		/// <summary>
		/// Utility routine to check if the InetAddress in a wildcard address. </summary>
		/// <returns> a {@code boolean} indicating if the Inetaddress is
		///         a wildcard address.
		/// @since 1.4 </returns>
		public override bool AnyLocalAddress
		{
			get
			{
				return Holder().Address == 0;
			}
		}

		/// <summary>
		/// Utility routine to check if the InetAddress is a loopback address.
		/// </summary>
		/// <returns> a {@code boolean} indicating if the InetAddress is
		/// a loopback address; or false otherwise.
		/// @since 1.4 </returns>
		public override bool LoopbackAddress
		{
			get
			{
				/* 127.x.x.x */
				sbyte[] byteAddr = Address;
				return byteAddr[0] == 127;
			}
		}

		/// <summary>
		/// Utility routine to check if the InetAddress is an link local address.
		/// </summary>
		/// <returns> a {@code boolean} indicating if the InetAddress is
		/// a link local address; or false if address is not a link local unicast address.
		/// @since 1.4 </returns>
		public override bool LinkLocalAddress
		{
			get
			{
				// link-local unicast in IPv4 (169.254.0.0/16)
				// defined in "Documenting Special Use IPv4 Address Blocks
				// that have been Registered with IANA" by Bill Manning
				// draft-manning-dsua-06.txt
				int address = Holder().Address;
				return ((((int)((uint)address >> 24)) & 0xFF) == 169) && ((((int)((uint)address >> 16)) & 0xFF) == 254);
			}
		}

		/// <summary>
		/// Utility routine to check if the InetAddress is a site local address.
		/// </summary>
		/// <returns> a {@code boolean} indicating if the InetAddress is
		/// a site local address; or false if address is not a site local unicast address.
		/// @since 1.4 </returns>
		public override bool SiteLocalAddress
		{
			get
			{
				// refer to RFC 1918
				// 10/8 prefix
				// 172.16/12 prefix
				// 192.168/16 prefix
				int address = Holder().Address;
				return ((((int)((uint)address >> 24)) & 0xFF) == 10) || (((((int)((uint)address >> 24)) & 0xFF) == 172) && ((((int)((uint)address >> 16)) & 0xF0) == 16)) || (((((int)((uint)address >> 24)) & 0xFF) == 192) && ((((int)((uint)address >> 16)) & 0xFF) == 168));
			}
		}

		/// <summary>
		/// Utility routine to check if the multicast address has global scope.
		/// </summary>
		/// <returns> a {@code boolean} indicating if the address has
		///         is a multicast address of global scope, false if it is not
		///         of global scope or it is not a multicast address
		/// @since 1.4 </returns>
		public override bool MCGlobal
		{
			get
			{
				// 224.0.1.0 to 238.255.255.255
				sbyte[] byteAddr = Address;
				return ((byteAddr[0] & 0xff) >= 224 && (byteAddr[0] & 0xff) <= 238) && !((byteAddr[0] & 0xff) == 224 && byteAddr[1] == 0 && byteAddr[2] == 0);
			}
		}

		/// <summary>
		/// Utility routine to check if the multicast address has node scope.
		/// </summary>
		/// <returns> a {@code boolean} indicating if the address has
		///         is a multicast address of node-local scope, false if it is not
		///         of node-local scope or it is not a multicast address
		/// @since 1.4 </returns>
		public override bool MCNodeLocal
		{
			get
			{
				// unless ttl == 0
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
		public override bool MCLinkLocal
		{
			get
			{
				// 224.0.0/24 prefix and ttl == 1
				int address = Holder().Address;
				return ((((int)((uint)address >> 24)) & 0xFF) == 224) && ((((int)((uint)address >> 16)) & 0xFF) == 0) && ((((int)((uint)address >> 8)) & 0xFF) == 0);
			}
		}

		/// <summary>
		/// Utility routine to check if the multicast address has site scope.
		/// </summary>
		/// <returns> a {@code boolean} indicating if the address has
		///         is a multicast address of site-local scope, false if it is not
		///         of site-local scope or it is not a multicast address
		/// @since 1.4 </returns>
		public override bool MCSiteLocal
		{
			get
			{
				// 239.255/16 prefix or ttl < 32
				int address = Holder().Address;
				return ((((int)((uint)address >> 24)) & 0xFF) == 239) && ((((int)((uint)address >> 16)) & 0xFF) == 255);
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
		public override bool MCOrgLocal
		{
			get
			{
				// 239.192 - 239.195
				int address = Holder().Address;
				return ((((int)((uint)address >> 24)) & 0xFF) == 239) && ((((int)((uint)address >> 16)) & 0xFF) >= 192) && ((((int)((uint)address >> 16)) & 0xFF) <= 195);
			}
		}

		/// <summary>
		/// Returns the raw IP address of this {@code InetAddress}
		/// object. The result is in network byte order: the highest order
		/// byte of the address is in {@code getAddress()[0]}.
		/// </summary>
		/// <returns>  the raw IP address of this object. </returns>
		public override sbyte[] Address
		{
			get
			{
				int address = Holder().Address;
				sbyte[] addr = new sbyte[INADDRSZ];
    
				addr[0] = unchecked((sbyte)(((int)((uint)address >> 24)) & 0xFF));
				addr[1] = unchecked((sbyte)(((int)((uint)address >> 16)) & 0xFF));
				addr[2] = unchecked((sbyte)(((int)((uint)address >> 8)) & 0xFF));
				addr[3] = unchecked((sbyte)(address & 0xFF));
				return addr;
			}
		}

		/// <summary>
		/// Returns the IP address string in textual presentation form.
		/// </summary>
		/// <returns>  the raw IP address in a string format.
		/// @since   JDK1.0.2 </returns>
		public override String HostAddress
		{
			get
			{
				return NumericToTextFormat(Address);
			}
		}

		/// <summary>
		/// Returns a hashcode for this IP address.
		/// </summary>
		/// <returns>  a hash code value for this IP address. </returns>
		public override int HashCode()
		{
			return Holder().Address;
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
			return (obj != null) && (obj is Inet4Address) && (((InetAddress)obj).Holder().Address == Holder().Address);
		}

		// Utilities
		/*
		 * Converts IPv4 binary address into a string suitable for presentation.
		 *
		 * @param src a byte array representing an IPv4 numeric address
		 * @return a String representing the IPv4 address in
		 *         textual representation format
		 * @since 1.4
		 */

		internal static String NumericToTextFormat(sbyte[] src)
		{
			return (src[0] & 0xff) + "." + (src[1] & 0xff) + "." + (src[2] & 0xff) + "." + (src[3] & 0xff);
		}

		/// <summary>
		/// Perform class load-time initializations.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern void init();
	}

}