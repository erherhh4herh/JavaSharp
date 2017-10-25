/*
 * Copyright (c) 2000, 2008, Oracle and/or its affiliates. All rights reserved.
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
	/// Signals that a timeout has occurred on a socket read or accept.
	/// 
	/// @since   1.4
	/// </summary>

	public class SocketTimeoutException : java.io.InterruptedIOException
	{
		private new const long SerialVersionUID = -8846654841826352300L;

		/// <summary>
		/// Constructs a new SocketTimeoutException with a detail
		/// message. </summary>
		/// <param name="msg"> the detail message </param>
		public SocketTimeoutException(String msg) : base(msg)
		{
		}

		/// <summary>
		/// Construct a new SocketTimeoutException with no detailed message.
		/// </summary>
		public SocketTimeoutException()
		{
		}
	}

}