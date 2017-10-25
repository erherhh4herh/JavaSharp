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
	/// An <code>UnexpectedException</code> is thrown if the client of a
	/// remote method call receives, as a result of the call, a checked
	/// exception that is not among the checked exception types declared in the
	/// <code>throws</code> clause of the method in the remote interface.
	/// 
	/// @author  Roger Riggs
	/// @since   JDK1.1
	/// </summary>
	public class UnexpectedException : RemoteException
	{

		/* indicate compatibility with JDK 1.1.x version of class */
		private new const long SerialVersionUID = 1800467484195073863L;

		/// <summary>
		/// Constructs an <code>UnexpectedException</code> with the specified
		/// detail message.
		/// </summary>
		/// <param name="s"> the detail message
		/// @since JDK1.1 </param>
		public UnexpectedException(String s) : base(s)
		{
		}

		/// <summary>
		/// Constructs a <code>UnexpectedException</code> with the specified
		/// detail message and nested exception.
		/// </summary>
		/// <param name="s"> the detail message </param>
		/// <param name="ex"> the nested exception
		/// @since JDK1.1 </param>
		public UnexpectedException(String s, Exception ex) : base(s, ex)
		{
		}
	}

}