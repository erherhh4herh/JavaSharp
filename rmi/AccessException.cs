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
	/// An <code>AccessException</code> is thrown by certain methods of the
	/// <code>java.rmi.Naming</code> class (specifically <code>bind</code>,
	/// <code>rebind</code>, and <code>unbind</code>) and methods of the
	/// <code>java.rmi.activation.ActivationSystem</code> interface to
	/// indicate that the caller does not have permission to perform the action
	/// requested by the method call.  If the method was invoked from a non-local
	/// host, then an <code>AccessException</code> is thrown.
	/// 
	/// @author  Ann Wollrath
	/// @author  Roger Riggs
	/// @since   JDK1.1 </summary>
	/// <seealso cref=     java.rmi.Naming </seealso>
	/// <seealso cref=     java.rmi.activation.ActivationSystem </seealso>
	public class AccessException : java.rmi.RemoteException
	{

		/* indicate compatibility with JDK 1.1.x version of class */
		 private new const long SerialVersionUID = 6314925228044966088L;

		/// <summary>
		/// Constructs an <code>AccessException</code> with the specified
		/// detail message.
		/// </summary>
		/// <param name="s"> the detail message
		/// @since JDK1.1 </param>
		public AccessException(String s) : base(s)
		{
		}

		/// <summary>
		/// Constructs an <code>AccessException</code> with the specified
		/// detail message and nested exception.
		/// </summary>
		/// <param name="s"> the detail message </param>
		/// <param name="ex"> the nested exception
		/// @since JDK1.1 </param>
		public AccessException(String s, Exception ex) : base(s, ex)
		{
		}
	}

}