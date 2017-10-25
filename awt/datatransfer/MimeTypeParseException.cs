using System;

/*
 * Copyright (c) 1997, 2006, Oracle and/or its affiliates. All rights reserved.
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

namespace java.awt.datatransfer
{


	/// <summary>
	///    A class to encapsulate MimeType parsing related exceptions
	/// 
	/// @serial exclude
	/// @since 1.3
	/// </summary>
	public class MimeTypeParseException : Exception
	{

		// use serialVersionUID from JDK 1.2.2 for interoperability
		private new const long SerialVersionUID = -5604407764691570741L;

		/// <summary>
		/// Constructs a MimeTypeParseException with no specified detail message.
		/// </summary>
		public MimeTypeParseException() : base()
		{
		}

		/// <summary>
		/// Constructs a MimeTypeParseException with the specified detail message.
		/// </summary>
		/// <param name="s">   the detail message. </param>
		public MimeTypeParseException(String s) : base(s)
		{
		}
	} // class MimeTypeParseException

}