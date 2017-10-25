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
	/// socket to a remote address and port.  Typically, the connection
	/// was refused remotely (e.g., no process is listening on the
	/// remote address/port).
	/// 
	/// @since   JDK1.1
	/// </summary>
	public class ConnectException : SocketException
	{
		private new const long SerialVersionUID = 3831404271622369215L;

		/// <summary>
		/// Constructs a new ConnectException with the specified detail
		/// message as to why the connect error occurred.
		/// A detail message is a String that gives a specific
		/// description of this error. </summary>
		/// <param name="msg"> the detail message </param>
		public ConnectException(String msg) : base(msg)
		{
		}

		/// <summary>
		/// Construct a new ConnectException with no detailed message.
		/// </summary>
		public ConnectException()
		{
		}
	}

}