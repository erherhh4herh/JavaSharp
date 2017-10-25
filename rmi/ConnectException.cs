using System;

/*
 * Copyright (c) 1996, 1998, Oracle and/or its affiliates. All rights reserved.
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

namespace java.rmi
{

	/// <summary>
	/// A <code>ConnectException</code> is thrown if a connection is refused
	/// to the remote host for a remote method call.
	/// 
	/// @author  Ann Wollrath
	/// @since   JDK1.1
	/// </summary>
	public class ConnectException : RemoteException
	{

		/* indicate compatibility with JDK 1.1.x version of class */
		 private new const long SerialVersionUID = 4863550261346652506L;

		/// <summary>
		/// Constructs a <code>ConnectException</code> with the specified
		/// detail message.
		/// </summary>
		/// <param name="s"> the detail message
		/// @since JDK1.1 </param>
		public ConnectException(String s) : base(s)
		{
		}

		/// <summary>
		/// Constructs a <code>ConnectException</code> with the specified
		/// detail message and nested exception.
		/// </summary>
		/// <param name="s"> the detail message </param>
		/// <param name="ex"> the nested exception
		/// @since JDK1.1 </param>
		public ConnectException(String s, Exception ex) : base(s, ex)
		{
		}
	}

}