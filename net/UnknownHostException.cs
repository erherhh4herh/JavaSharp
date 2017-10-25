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
	/// Thrown to indicate that the IP address of a host could not be determined.
	/// 
	/// @author  Jonathan Payne
	/// @since   JDK1.0
	/// </summary>
	public class UnknownHostException : IOException
	{
		private new const long SerialVersionUID = -4639126076052875403L;

		/// <summary>
		/// Constructs a new {@code UnknownHostException} with the
		/// specified detail message.
		/// </summary>
		/// <param name="host">   the detail message. </param>
		public UnknownHostException(String host) : base(host)
		{
		}

		/// <summary>
		/// Constructs a new {@code UnknownHostException} with no detail
		/// message.
		/// </summary>
		public UnknownHostException()
		{
		}
	}

}