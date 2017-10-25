/*
 * Copyright (c) 2001, 2013, Oracle and/or its affiliates. All rights reserved.
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
	/// Signals that an ICMP Port Unreachable message has been
	/// received on a connected datagram.
	/// 
	/// @since   1.4
	/// </summary>

	public class PortUnreachableException : SocketException
	{
		private new const long SerialVersionUID = 8462541992376507323L;

		/// <summary>
		/// Constructs a new {@code PortUnreachableException} with a
		/// detail message. </summary>
		/// <param name="msg"> the detail message </param>
		public PortUnreachableException(String msg) : base(msg)
		{
		}

		/// <summary>
		/// Construct a new {@code PortUnreachableException} with no
		/// detailed message.
		/// </summary>
		public PortUnreachableException()
		{
		}
	}

}