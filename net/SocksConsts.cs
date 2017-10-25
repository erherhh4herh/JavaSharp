/*
 * Copyright (c) 2000, 2001, Oracle and/or its affiliates. All rights reserved.
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
	/// Constants used by the SOCKS protocol implementation.
	/// </summary>

	internal interface SocksConsts
	{
	}

	public static class SocksConsts_Fields
	{
		public const int PROTO_VERS4 = 4;
		public const int PROTO_VERS = 5;
		public const int DEFAULT_PORT = 1080;
		public const int NO_AUTH = 0;
		public const int GSSAPI = 1;
		public const int USER_PASSW = 2;
		public const int NO_METHODS = -1;
		public const int CONNECT = 1;
		public const int BIND = 2;
		public const int UDP_ASSOC = 3;
		public const int IPV4 = 1;
		public const int DOMAIN_NAME = 3;
		public const int IPV6 = 4;
		public const int REQUEST_OK = 0;
		public const int GENERAL_FAILURE = 1;
		public const int NOT_ALLOWED = 2;
		public const int NET_UNREACHABLE = 3;
		public const int HOST_UNREACHABLE = 4;
		public const int CONN_REFUSED = 5;
		public const int TTL_EXPIRED = 6;
		public const int CMD_NOT_SUPPORTED = 7;
		public const int ADDR_TYPE_NOT_SUP = 8;
	}

}