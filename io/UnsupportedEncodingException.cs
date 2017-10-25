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
namespace java.io
{

	/// <summary>
	/// The Character Encoding is not supported.
	/// 
	/// @author  Asmus Freytag
	/// @since   JDK1.1
	/// </summary>
	public class UnsupportedEncodingException : IOException
	{
		private new const long SerialVersionUID = -4274276298326136670L;

		/// <summary>
		/// Constructs an UnsupportedEncodingException without a detail message.
		/// </summary>
		public UnsupportedEncodingException() : base()
		{
		}

		/// <summary>
		/// Constructs an UnsupportedEncodingException with a detail message. </summary>
		/// <param name="s"> Describes the reason for the exception. </param>
		public UnsupportedEncodingException(String s) : base(s)
		{
		}
	}

}