using System;

/*
 * Copyright (c) 1998, 1999, Oracle and/or its affiliates. All rights reserved.
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

namespace java.rmi.activation
{

	/// <summary>
	/// This exception is thrown by the RMI runtime when activation
	/// fails during a remote call to an activatable object.
	/// 
	/// @author      Ann Wollrath
	/// @since       1.2
	/// </summary>
	public class ActivateFailedException : java.rmi.RemoteException
	{

		/// <summary>
		/// indicate compatibility with the Java 2 SDK v1.2 version of class </summary>
		private new const long SerialVersionUID = 4863550261346652506L;

		/// <summary>
		/// Constructs an <code>ActivateFailedException</code> with the specified
		/// detail message.
		/// </summary>
		/// <param name="s"> the detail message
		/// @since 1.2 </param>
		public ActivateFailedException(String s) : base(s)
		{
		}

		/// <summary>
		/// Constructs an <code>ActivateFailedException</code> with the specified
		/// detail message and nested exception.
		/// </summary>
		/// <param name="s"> the detail message </param>
		/// <param name="ex"> the nested exception
		/// @since 1.2 </param>
		public ActivateFailedException(String s, Exception ex) : base(s, ex)
		{
		}
	}

}