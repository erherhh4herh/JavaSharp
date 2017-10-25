using System;

/*
 * Copyright (c) 1996, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.rmi.server
{

	/// <summary>
	/// An obsolete subclass of <seealso cref="ExportException"/>.
	/// 
	/// @author  Ann Wollrath
	/// @since   JDK1.1 </summary>
	/// @deprecated This class is obsolete. Use <seealso cref="ExportException"/> instead. 
	[Obsolete("This class is obsolete. Use <seealso cref="ExportException"/> instead.")]
	public class SocketSecurityException : ExportException
	{

		/* indicate compatibility with JDK 1.1.x version of class */
		private new const long SerialVersionUID = -7622072999407781979L;

		/// <summary>
		/// Constructs an <code>SocketSecurityException</code> with the specified
		/// detail message.
		/// </summary>
		/// <param name="s"> the detail message.
		/// @since JDK1.1 </param>
		public SocketSecurityException(String s) : base(s)
		{
		}

		/// <summary>
		/// Constructs an <code>SocketSecurityException</code> with the specified
		/// detail message and nested exception.
		/// </summary>
		/// <param name="s"> the detail message. </param>
		/// <param name="ex"> the nested exception
		/// @since JDK1.1 </param>
		public SocketSecurityException(String s, Exception ex) : base(s, ex)
		{
		}

	}

}