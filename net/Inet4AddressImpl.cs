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
	 * Package private implementation of InetAddressImpl for IPv4.
	 *
	 * @since 1.4
	 */
	internal class Inet4AddressImpl : InetAddressImpl
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
		private extern boolean isReachable0(sbyte[] addr, int timeout, sbyte[] ifaddr, int ttl);

		public virtual InetAddress AnyLocalAddress()
		{
			lock (this)
			{
				if (AnyLocalAddress_Renamed == null)
				{
					AnyLocalAddress_Renamed = new Inet4Address(); // {0x00,0x00,0x00,0x00}
					AnyLocalAddress_Renamed.Holder().HostName_Renamed = "0.0.0.0";
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
					sbyte[] loopback = new sbyte[] {0x7f,0x00,0x00,0x01};
					LoopbackAddress_Renamed = new Inet4Address("localhost", loopback);
				}
				return LoopbackAddress_Renamed;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean isReachable(InetAddress addr, int timeout, NetworkInterface netif, int ttl) throws java.io.IOException
	  public virtual bool IsReachable(InetAddress addr, int timeout, NetworkInterface netif, int ttl)
	  {
		  sbyte[] ifaddr = null;
		  if (netif != null)
		  {
			  /*
			   * Let's make sure we use an address of the proper family
			   */
			  IEnumerator<InetAddress> it = netif.InetAddresses;
			  InetAddress inetaddr = null;
			  while (!(inetaddr is Inet4Address) && it.MoveNext())
			  {
				  inetaddr = it.Current;
			  }
			  if (inetaddr is Inet4Address)
			  {
				  ifaddr = inetaddr.Address;
			  }
		  }
		  return isReachable0(addr.Address, timeout, ifaddr, ttl);
	  }
		private InetAddress AnyLocalAddress_Renamed;
		private InetAddress LoopbackAddress_Renamed;
	}

}