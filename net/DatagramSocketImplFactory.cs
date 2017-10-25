/*
 * Copyright (c) 1999, 2013, Oracle and/or its affiliates. All rights reserved.
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
	/// This interface defines a factory for datagram socket implementations. It
	/// is used by the classes {@code DatagramSocket} to create actual socket
	/// implementations.
	/// 
	/// @author  Yingxian Wang </summary>
	/// <seealso cref=     java.net.DatagramSocket
	/// @since   1.3 </seealso>
	public interface DatagramSocketImplFactory
	{
		/// <summary>
		/// Creates a new {@code DatagramSocketImpl} instance.
		/// </summary>
		/// <returns>  a new instance of {@code DatagramSocketImpl}. </returns>
		/// <seealso cref=     java.net.DatagramSocketImpl </seealso>
		DatagramSocketImpl CreateDatagramSocketImpl();
	}

}