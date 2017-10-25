using System.Collections.Generic;
using System.Runtime.InteropServices;

/*
 * Copyright (c) 2002, 2013, Oracle and/or its affiliates. All rights reserved.
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

	/*
	 * Package private implementation of InetAddressImpl for dual
	 * IPv4/IPv6 stack.
	 * <p>
	 * If InetAddress.preferIPv6Address is true then anyLocalAddress(),
	 * loopbackAddress(), and localHost() will return IPv6 addresses,
	 * otherwise IPv4 addresses.
	 *
	 * @since 1.4
	 */

	internal class Inet6AddressImpl : InetAddressImpl
	{
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		public extern String getLocalHostName();
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		public extern InetAddress[] lookupAllHostAddr(String hostname);
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		public extern String getHostByAddr(sbyte[] addr);
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern boolean isReachable0(sbyte[] addr, int scope, int timeout, sbyte[] inf, int ttl, int if_scope);

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean isReachable(InetAddress addr, int timeout, NetworkInterface netif, int ttl) throws java.io.IOException
		public virtual bool IsReachable(InetAddress addr, int timeout, NetworkInterface netif, int ttl)
		{
			sbyte[] ifaddr = null;
			int scope = -1;
			int netif_scope = -1;
			if (netif != null)
			{
				/*
				 * Let's make sure we bind to an address of the proper family.
				 * Which means same family as addr because at this point it could
				 * be either an IPv6 address or an IPv4 address (case of a dual
				 * stack system).
				 */
				IEnumerator<InetAddress> it = netif.InetAddresses;
				InetAddress inetaddr = null;
				while (it.MoveNext())
				{
					inetaddr = it.Current;
					if (inetaddr.GetType().IsInstanceOfType(addr))
					{
						ifaddr = inetaddr.Address;
						if (inetaddr is Inet6Address)
						{
							netif_scope = ((Inet6Address) inetaddr).ScopeId;
						}
						break;
					}
				}
				if (ifaddr == null)
				{
					// Interface doesn't support the address family of
					// the destination
					return false;
				}
			}
			if (addr is Inet6Address)
			{
				scope = ((Inet6Address) addr).ScopeId;
			}
			return isReachable0(addr.Address, scope, timeout, ifaddr, ttl, netif_scope);
		}

		public virtual InetAddress AnyLocalAddress()
		{
			lock (this)
			{
				if (AnyLocalAddress_Renamed == null)
				{
					if (InetAddress.PreferIPv6Address)
					{
						AnyLocalAddress_Renamed = new Inet6Address();
						AnyLocalAddress_Renamed.Holder().HostName_Renamed = "::";
					}
					else
					{
						AnyLocalAddress_Renamed = (new Inet4AddressImpl()).AnyLocalAddress();
					}
				}
				return AnyLocalAddress_Renamed;
			}
		}

		public virtual InetAddress LoopbackAddress()
		{
			lock (this)
			{
				if (LoopbackAddress_Renamed == null)
				{
					 if (InetAddress.PreferIPv6Address)
					 {
						 sbyte[] loopback = new sbyte[] {0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01};
						 LoopbackAddress_Renamed = new Inet6Address("localhost", loopback);
					 }
					 else
					 {
						LoopbackAddress_Renamed = (new Inet4AddressImpl()).LoopbackAddress();
					 }
				}
				return LoopbackAddress_Renamed;
			}
		}

		private InetAddress AnyLocalAddress_Renamed;
		private InetAddress LoopbackAddress_Renamed;
	}

}