/*
 * Copyright (c) 2010, Oracle and/or its affiliates. All rights reserved.
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


	using SdpSupport = sun.net.sdp.SdpSupport;

	/// <summary>
	/// SocketImpl that supports the SDP protocol
	/// </summary>
	internal class SdpSocketImpl : PlainSocketImpl
	{
		internal SdpSocketImpl()
		{
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void create(boolean stream) throws java.io.IOException
		protected internal override void Create(bool stream)
		{
			if (!stream)
			{
				throw new UnsupportedOperationException("Must be a stream socket");
			}
			Fd = SdpSupport.createSocket();
			if (Socket_Renamed != null)
			{
				Socket_Renamed.SetCreated();
			}
			if (ServerSocket_Renamed != null)
			{
				ServerSocket_Renamed.SetCreated();
			}
		}
	}

}