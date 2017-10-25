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
	/// Thrown to indicate that a malformed URL has occurred. Either no
	/// legal protocol could be found in a specification string or the
	/// string could not be parsed.
	/// 
	/// @author  Arthur van Hoff
	/// @since   JDK1.0
	/// </summary>
	public class MalformedURLException : IOException
	{
		private new const long SerialVersionUID = -182787522200415866L;

		/// <summary>
		/// Constructs a {@code MalformedURLException} with no detail message.
		/// </summary>
		public MalformedURLException()
		{
		}

		/// <summary>
		/// Constructs a {@code MalformedURLException} with the
		/// specified detail message.
		/// </summary>
		/// <param name="msg">   the detail message. </param>
		public MalformedURLException(String msg) : base(msg)
		{
		}
	}

}