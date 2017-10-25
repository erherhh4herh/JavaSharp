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
	/// Signals that an error occurred while attempting to bind a
	/// socket to a local address and port.  Typically, the port is
	/// in use, or the requested local address could not be assigned.
	/// 
	/// @since   JDK1.1
	/// </summary>

	public class BindException : SocketException
	{
		private new const long SerialVersionUID = -5945005768251722951L;

		/// <summary>
		/// Constructs a new BindException with the specified detail
		/// message as to why the bind error occurred.
		/// A detail message is a String that gives a specific
		/// description of this error. </summary>
		/// <param name="msg"> the detail message </param>
		public BindException(String msg) : base(msg)
		{
		}

		/// <summary>
		/// Construct a new BindException with no detailed message.
		/// </summary>
		public BindException()
		{
		}
	}

}