/*
 * Copyright (c) 1995, 2013, Oracle and/or its affiliates. All rights reserved.
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
	/// Thrown to indicate that there is an error creating or accessing a Socket.
	/// 
	/// @author  Jonathan Payne
	/// @since   JDK1.0
	/// </summary>
	public class SocketException : IOException
	{
		private new const long SerialVersionUID = -5935874303556886934L;

		/// <summary>
		/// Constructs a new {@code SocketException} with the
		/// specified detail message.
		/// </summary>
		/// <param name="msg"> the detail message. </param>
		public SocketException(String msg) : base(msg)
		{
		}

		/// <summary>
		/// Constructs a new {@code SocketException} with no detail message.
		/// </summary>
		public SocketException()
		{
		}
	}

}