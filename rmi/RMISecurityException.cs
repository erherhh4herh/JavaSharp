using System;

/*
 * Copyright (c) 1996, 2004, Oracle and/or its affiliates. All rights reserved.
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
	/// An <code>RMISecurityException</code> signals that a security exception
	/// has occurred during the execution of one of
	/// <code>java.rmi.RMISecurityManager</code>'s methods.
	/// 
	/// @author  Roger Riggs
	/// @since   JDK1.1 </summary>
	/// @deprecated Use <seealso cref="java.lang.SecurityException"/> instead.
	/// Application code should never directly reference this class, and
	/// <code>RMISecurityManager</code> no longer throws this subclass of
	/// <code>java.lang.SecurityException</code>. 
	[Obsolete("Use <seealso cref="java.lang.SecurityException"/> instead.")]
	public class RMISecurityException : java.lang.SecurityException
	{

		/* indicate compatibility with JDK 1.1.x version of class */
		 private new const long SerialVersionUID = -8433406075740433514L;

		/// <summary>
		/// Construct an <code>RMISecurityException</code> with a detail message. </summary>
		/// <param name="name"> the detail message
		/// @since JDK1.1 </param>
		/// @deprecated no replacement 
		[Obsolete("no replacement")]
		public RMISecurityException(String name) : base(name)
		{
		}

		/// <summary>
		/// Construct an <code>RMISecurityException</code> with a detail message. </summary>
		/// <param name="name"> the detail message </param>
		/// <param name="arg"> ignored
		/// @since JDK1.1 </param>
		/// @deprecated no replacement 
		[Obsolete("no replacement")]
		public RMISecurityException(String name, String arg) : this(name)
		{
		}
	}

}