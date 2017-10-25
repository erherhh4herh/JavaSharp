using System;

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


	/// 
	/// <summary>
	/// This class represents a Socket Address with no protocol attachment.
	/// As an abstract class, it is meant to be subclassed with a specific,
	/// protocol dependent, implementation.
	/// <para>
	/// It provides an immutable object used by sockets for binding, connecting, or
	/// as returned values.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= java.net.Socket </seealso>
	/// <seealso cref= java.net.ServerSocket
	/// @since 1.4 </seealso>
	[Serializable]
	public abstract class SocketAddress
	{

		internal const long SerialVersionUID = 5215720748342549866L;

	}

}