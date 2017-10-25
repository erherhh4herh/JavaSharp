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
	/// Thrown to indicate that an unknown service exception has
	/// occurred. Either the MIME type returned by a URL connection does
	/// not make sense, or the application is attempting to write to a
	/// read-only URL connection.
	/// 
	/// @author  unascribed
	/// @since   JDK1.0
	/// </summary>
	public class UnknownServiceException : IOException
	{
		private new const long SerialVersionUID = -4169033248853639508L;

		/// <summary>
		/// Constructs a new {@code UnknownServiceException} with no
		/// detail message.
		/// </summary>
		public UnknownServiceException()
		{
		}

		/// <summary>
		/// Constructs a new {@code UnknownServiceException} with the
		/// specified detail message.
		/// </summary>
		/// <param name="msg">   the detail message. </param>
		public UnknownServiceException(String msg) : base(msg)
		{
		}
	}

}