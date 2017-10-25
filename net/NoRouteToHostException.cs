/*
 * Copyright (c) 1996, 2008, Oracle and/or its affiliates. All rights reserved.
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
	/// Signals that an error occurred while attempting to connect a
	/// socket to a remote address and port.  Typically, the remote
	/// host cannot be reached because of an intervening firewall, or
	/// if an intermediate router is down.
	/// 
	/// @since   JDK1.1
	/// </summary>
	public class NoRouteToHostException : SocketException
	{
		private new const long SerialVersionUID = -1897550894873493790L;

		/// <summary>
		/// Constructs a new NoRouteToHostException with the specified detail
		/// message as to why the remote host cannot be reached.
		/// A detail message is a String that gives a specific
		/// description of this error. </summary>
		/// <param name="msg"> the detail message </param>
		public NoRouteToHostException(String msg) : base(msg)
		{
		}

		/// <summary>
		/// Construct a new NoRouteToHostException with no detailed message.
		/// </summary>
		public NoRouteToHostException()
		{
		}
	}

}