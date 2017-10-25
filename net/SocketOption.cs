using System;

/*
 * Copyright (c) 2007, 2013, Oracle and/or its affiliates. All rights reserved.
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
	/// A socket option associated with a socket.
	/// 
	/// <para> In the <seealso cref="java.nio.channels channels"/> package, the {@link
	/// java.nio.channels.NetworkChannel} interface defines the {@link
	/// java.nio.channels.NetworkChannel#setOption(SocketOption,Object) setOption}
	/// and <seealso cref="java.nio.channels.NetworkChannel#getOption(SocketOption) getOption"/>
	/// methods to set and query the channel's socket options.
	/// 
	/// </para>
	/// </summary>
	/// @param   <T>     The type of the socket option value.
	/// 
	/// @since 1.7
	/// </param>
	/// <seealso cref= StandardSocketOptions </seealso>

	public interface SocketOption<T>
	{

		/// <summary>
		/// Returns the name of the socket option.
		/// </summary>
		/// <returns> the name of the socket option </returns>
		String Name();

		/// <summary>
		/// Returns the type of the socket option value.
		/// </summary>
		/// <returns> the type of the socket option value </returns>
		Class Type();
	}

}