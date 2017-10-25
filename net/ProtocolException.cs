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
	/// Thrown to indicate that there is an error in the underlying
	/// protocol, such as a TCP error.
	/// 
	/// @author  Chris Warth
	/// @since   JDK1.0
	/// </summary>
	public class ProtocolException : IOException
	{
		private new const long SerialVersionUID = -6098449442062388080L;

		/// <summary>
		/// Constructs a new {@code ProtocolException} with the
		/// specified detail message.
		/// </summary>
		/// <param name="host">   the detail message. </param>
		public ProtocolException(String host) : base(host)
		{
		}

		/// <summary>
		/// Constructs a new {@code ProtocolException} with no detail message.
		/// </summary>
		public ProtocolException()
		{
		}
	}

}