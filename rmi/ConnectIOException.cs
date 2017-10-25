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
	/// A <code>ConnectIOException</code> is thrown if an
	/// <code>IOException</code> occurs while making a connection
	/// to the remote host for a remote method call.
	/// 
	/// @author  Ann Wollrath
	/// @since   JDK1.1
	/// </summary>
	public class ConnectIOException : RemoteException
	{

		/* indicate compatibility with JDK 1.1.x version of class */
		private new const long SerialVersionUID = -8087809532704668744L;

		/// <summary>
		/// Constructs a <code>ConnectIOException</code> with the specified
		/// detail message.
		/// </summary>
		/// <param name="s"> the detail message
		/// @since JDK1.1 </param>
		public ConnectIOException(String s) : base(s)
		{
		}


		/// <summary>
		/// Constructs a <code>ConnectIOException</code> with the specified
		/// detail message and nested exception.
		/// </summary>
		/// <param name="s"> the detail message </param>
		/// <param name="ex"> the nested exception
		/// @since JDK1.1 </param>
		public ConnectIOException(String s, Exception ex) : base(s, ex)
		{
		}
	}

}