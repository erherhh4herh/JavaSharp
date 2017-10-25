/*
 * Copyright (c) 2002, 2005, Oracle and/or its affiliates. All rights reserved.
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
	 * Package private interface to "implementation" used by
	 * {@link InetAddress}.
	 * <p>
	 * See {@link java.net.Inet4AddressImp} and
	 * {@link java.net.Inet6AddressImp}.
	 *
	 * @since 1.4
	 */
	internal interface InetAddressImpl
	{

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: String getLocalHostName() throws UnknownHostException;
		String LocalHostName {get;}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: InetAddress[] lookupAllHostAddr(String hostname) throws UnknownHostException;
		InetAddress[] LookupAllHostAddr(String hostname);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: String getHostByAddr(byte[] addr) throws UnknownHostException;
		String GetHostByAddr(sbyte[] addr);

		InetAddress AnyLocalAddress();
		InetAddress LoopbackAddress();
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean isReachable(InetAddress addr, int timeout, NetworkInterface netif, int ttl) throws java.io.IOException;
		bool IsReachable(InetAddress addr, int timeout, NetworkInterface netif, int ttl);
	}

}