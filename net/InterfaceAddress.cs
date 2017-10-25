/*
 * Copyright (c) 2005, 2013, Oracle and/or its affiliates. All rights reserved.
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
	/// This class represents a Network Interface address. In short it's an
	/// IP address, a subnet mask and a broadcast address when the address is
	/// an IPv4 one. An IP address and a network prefix length in the case
	/// of IPv6 address.
	/// </summary>
	/// <seealso cref= java.net.NetworkInterface
	/// @since 1.6 </seealso>
	public class InterfaceAddress
	{
		private InetAddress Address_Renamed = null;
		private Inet4Address Broadcast_Renamed = null;
		private short MaskLength = 0;

		/*
		 * Package private constructor. Can't be built directly, instances are
		 * obtained through the NetworkInterface class.
		 */
		internal InterfaceAddress()
		{
		}

		/// <summary>
		/// Returns an {@code InetAddress} for this address.
		/// </summary>
		/// <returns> the {@code InetAddress} for this address. </returns>
		public virtual InetAddress Address
		{
			get
			{
				return Address_Renamed;
			}
		}

		/// <summary>
		/// Returns an {@code InetAddress} for the broadcast address
		/// for this InterfaceAddress.
		/// <para>
		/// Only IPv4 networks have broadcast address therefore, in the case
		/// of an IPv6 network, {@code null} will be returned.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the {@code InetAddress} representing the broadcast
		///         address or {@code null} if there is no broadcast address. </returns>
		public virtual InetAddress Broadcast
		{
			get
			{
				return Broadcast_Renamed;
			}
		}

		/// <summary>
		/// Returns the network prefix length for this address. This is also known
		/// as the subnet mask in the context of IPv4 addresses.
		/// Typical IPv4 values would be 8 (255.0.0.0), 16 (255.255.0.0)
		/// or 24 (255.255.255.0). <para>
		/// Typical IPv6 values would be 128 (::1/128) or 10 (fe80::203:baff:fe27:1243/10)
		/// 
		/// </para>
		/// </summary>
		/// <returns> a {@code short} representing the prefix length for the
		///         subnet of that address. </returns>
		 public virtual short NetworkPrefixLength
		 {
			 get
			 {
				return MaskLength;
			 }
		 }

		/// <summary>
		/// Compares this object against the specified object.
		/// The result is {@code true} if and only if the argument is
		/// not {@code null} and it represents the same interface address as
		/// this object.
		/// <para>
		/// Two instances of {@code InterfaceAddress} represent the same
		/// address if the InetAddress, the prefix length and the broadcast are
		/// the same for both.
		/// 
		/// </para>
		/// </summary>
		/// <param name="obj">   the object to compare against. </param>
		/// <returns>  {@code true} if the objects are the same;
		///          {@code false} otherwise. </returns>
		/// <seealso cref=     java.net.InterfaceAddress#hashCode() </seealso>
		public override bool Equals(Object obj)
		{
			if (!(obj is InterfaceAddress))
			{
				return false;
			}
			InterfaceAddress cmp = (InterfaceAddress) obj;
			if (!(Address_Renamed == null ? cmp.Address_Renamed == null : Address_Renamed.Equals(cmp.Address_Renamed)))
			{
				return false;
			}
			if (!(Broadcast_Renamed == null ? cmp.Broadcast_Renamed == null : Broadcast_Renamed.Equals(cmp.Broadcast_Renamed)))
			{
				return false;
			}
			if (MaskLength != cmp.MaskLength)
			{
				return false;
			}
			return true;
		}

		/// <summary>
		/// Returns a hashcode for this Interface address.
		/// </summary>
		/// <returns>  a hash code value for this Interface address. </returns>
		public override int HashCode()
		{
			return Address_Renamed.HashCode() + ((Broadcast_Renamed != null) ? Broadcast_Renamed.HashCode() : 0) + MaskLength;
		}

		/// <summary>
		/// Converts this Interface address to a {@code String}. The
		/// string returned is of the form: InetAddress / prefix length [ broadcast address ].
		/// </summary>
		/// <returns>  a string representation of this Interface address. </returns>
		public override String ToString()
		{
			return Address_Renamed + "/" + MaskLength + " [" + Broadcast_Renamed + "]";
		}

	}

}